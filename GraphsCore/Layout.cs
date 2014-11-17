using MathNet.Numerics.LinearAlgebra.Double;
using Satsuma;
using Satsuma.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Choosability;

namespace GraphsCore
{
    public static class Layout
    {
        public delegate List<Graphs.Vector> Algorithm(Choosability.Graph g, List<Graphs.Vector> layout = null);

        public static Supergraph CreateSatsumaGraph(this Choosability.Graph g)
        {
            var satsumaGraph = new CustomGraph();

            var nodes = new Dictionary<int, Node>();
            foreach (var v in g.Vertices)
            {
                var node = satsumaGraph.AddNode();
                nodes[v] = node;
            }

            for (int i = 0; i < g.N; i++)
            {
                for (int j = i + 1; j < g.N; j++)
                {
                    if (g.Directed[i, j])
                        satsumaGraph.AddArc(nodes[i], nodes[j], Directedness.Directed);
                    else if (g.Directed[j, i])
                        satsumaGraph.AddArc(nodes[j], nodes[i], Directedness.Directed);
                    else if (g.Adjacent[i, j])
                        satsumaGraph.AddArc(nodes[i], nodes[j], Directedness.Undirected);
                }
            }

            return satsumaGraph;
        }

        public static List<Graphs.Vector> GetSpringsLayout(this Choosability.Graph g, List<Graphs.Vector> layout = null)
        {
            return GetSpringsLayout(g, 0);
        }

        public static List<Graphs.Vector> GetSpringsLayout(this Choosability.Graph g, int randomSeed)
        {
            var satsumaGraph = g.CreateSatsumaGraph();
            var layout = new ForceDirectedLayout(satsumaGraph, null, randomSeed);
            layout.Run();

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach(var p in layout.NodePositions)
            {
                minX = Math.Min(minX, p.Value.X);
                minY = Math.Min(minY, p.Value.Y);
                maxX = Math.Max(maxX, p.Value.X);
                maxY = Math.Max(maxY, p.Value.Y);
            }

            var width = maxX - minX;
            var height = maxY - minY;

            return satsumaGraph.Nodes().Select(n => new Graphs.Vector(0.1 + 0.7 * (layout.NodePositions[n].X - minX) / width, 0.1 + 0.7 * (layout.NodePositions[n].Y - minY) / height)).ToList();
        }

        public static List<Graphs.Vector> GetLaplacianLayout(this Choosability.Graph g, List<Graphs.Vector> layout = null)
        {
            var D = Matrix.Build.Diagonal(g.N, g.N, v => g.Degree(v));
            var A = Matrix.Build.Dense(g.N, g.N, (v, w) => g[v, w] ? 1 : 0);
            var L = D - A;

            var evd = L.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Symmetric);
            var x = evd.EigenVectors.Column(1);
            var y = evd.EigenVectors.Column(2);

            return GetEigenVectorLayout(x, y);
        }

        public static List<Graphs.Vector> GetWalkMatrixLayout(this Choosability.Graph g, List<Graphs.Vector> layout = null)
        {
            var D = Matrix.Build.Diagonal(g.N, g.N, v => g.Degree(v));
            var A = Matrix.Build.Dense(g.N, g.N, (v, w) => g[v, w] ? 1 : 0);
            var L = D.Inverse() * A;

            var evd = L.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Symmetric);
            int xi = 1;
            int yi = 2;

            for (int i = 0; i < evd.EigenValues.Count - 1; i++)
            {
                if (evd.EigenValues[i].Real <= 1 && evd.EigenValues[i + 1].Real >= 1)
                {
                    xi = i;
                    yi = i + 1;
                    break;
                }
            }
            var x = evd.EigenVectors.Column(xi);
            var y = evd.EigenVectors.Column(yi);

            return GetEigenVectorLayout(x, y);
        }

        static List<Graphs.Vector> GetEigenVectorLayout(MathNet.Numerics.LinearAlgebra.Vector<double> x, MathNet.Numerics.LinearAlgebra.Vector<double> y)
        {
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            for (int v = 0; v < x.Count; v++)
            {
                minX = Math.Min(minX, x[v]);
                minY = Math.Min(minY, y[v]);
                maxX = Math.Max(maxX, x[v]);
                maxY = Math.Max(maxY, y[v]);
            }

            var width = maxX - minX;
            var height = maxY - minY;

            return Enumerable.Range(0, x.Count).Select(v => new Graphs.Vector(0.1 + 0.7 * (x[v] - minX) / width, 0.1 + 0.7 * (y[v] - minY) / height)).ToList();
        }

        public static List<Graphs.Vector> GetUnitDistanceLayout(this Choosability.Graph g, List<Graphs.Vector> layout = null)
        {
            var modifiedLayout = layout.ToList();

            var maxDegree = g.MaxDegree;

            for (int qqq = 0; qqq < 50; qqq++)
            {
                var maxes = g.Vertices.ToList();
                maxes.Shuffle();

                foreach (var first in maxes)
                {
                    var nonzeroCount = 0;
                    var total = 0.0;
                    for (int i = 0; i < g.N; i++)
                    {
                        for (int j = i + 1; j < g.N; j++)
                        {
                            if (g[i, j])
                            {
                                var d = modifiedLayout[i].Distance(modifiedLayout[j]);
                                if (d > 0)
                                {
                                    nonzeroCount++;
                                    total += d;
                                }
                            }
                        }
                    }

                    var length = total / Math.Max(1, nonzeroCount);

                    for (int qq = 0; qq < 10; qq++)
                    {
                        modifiedLayout = modifiedLayout.ToList();
                        var remaining = g.Vertices.Where(v => v != first).ToList();
                        remaining.Shuffle();
                        var anchor = new List<Tuple<int, Graphs.Vector>>() { new Tuple<int, Graphs.Vector>(first, modifiedLayout[first]) };

                        while (remaining.Count > 0)
                        {
                            var count = remaining.Count;
                            var removed = new List<int>();
                            foreach (var v in remaining)
                            {
                                var neighbors = anchor.Where(a => g[a.Item1, v]).ToList();
                                if (neighbors.Count <= 0)
                                    continue;

                                if (neighbors.Count >= 2)
                                {
                                    var a = neighbors.First().Item2;
                                    var b = neighbors.Skip(1).First().Item2;

                                    double x1, y1, x2, y2;
                                    if (IntersectionOfTwoCircles(a.X, a.Y, length, b.X, b.Y, length, out x1, out y1, out x2, out y2))
                                    {
                                        var d1 = new Graphs.Vector(x1, y1).Distance(modifiedLayout[v]);
                                        var d2 = new Graphs.Vector(x2, y2).Distance(modifiedLayout[v]);

                                        if (d1 < d2 && d1 > 0.001)
                                        {
                                            modifiedLayout[v] = new Graphs.Vector(x1, y1);
                                        }
                                        else if (d2 < d1 && d2 > 0.001)
                                        {
                                            modifiedLayout[v] = new Graphs.Vector(x2, y2);
                                        }

                                        removed.Add(v);
                                        anchor.Add(new Tuple<int, Graphs.Vector>(v, modifiedLayout[v]));
                                    }
                                }
                            }

                            var tt = remaining.RemoveAll(v => removed.Contains(v));
                            removed.Clear();

                            if (tt == 0)
                            {
                                foreach (var v in remaining)
                                {
                                    var neighbors = anchor.Where(a => g[a.Item1, v]).ToList();
                                    if (neighbors.Count <= 0)
                                        continue;

                                    var p = neighbors.First().Item2;
                                    var q = modifiedLayout[v];

                                    var offset = q - p;
                                    var ok = offset.Normalize();

                                    if (ok)
                                    {
                                        offset *= length;
                                        modifiedLayout[v] = p + offset;

                                        removed.Add(v);
                                        anchor.Add(new Tuple<int, Graphs.Vector>(v, modifiedLayout[v]));
                                        goto skipper;
                                    }
                                }

                            skipper:
                                remaining.RemoveAll(v => removed.Contains(v));
                                if (remaining.Count == count)
                                {
                                    var x = remaining[0];
                                    remaining.RemoveAt(0);
                                    anchor.Add(new Tuple<int, Graphs.Vector>(x, modifiedLayout[x]));
                                }
                            }
                        }
                    }
                }
            }

            return modifiedLayout;
        }

        static bool IntersectionOfTwoCircles(double x0, double y0, double r0, double x1, double y1, double r1, out double xi, out double yi, out double xi_prime, out double yi_prime)
        {
            xi = yi = xi_prime = yi_prime = 0.0;
            double a, dx, dy, d, h, rx, ry;
            double x2, y2;

            /* dx and dy are the vertical and horizontal distances between
             * the circle centers.
             */
            dx = x1 - x0;
            dy = y1 - y0;

            /* Determine the straight-line distance between the centers. */
            //d = sqrt((dy*dy) + (dx*dx));
            d = Math.Sqrt(dy * dy + dx * dx);

            /* Check for solvability. */
            if (d > (r0 + r1))
            {
                /* no solution. circles do not intersect. */
                return false;
            }
            if (d < Math.Abs(r0 - r1))
            {
                /* no solution. one circle is contained in the other */
                return false;
            }

            /* 'point 2' is the point where the line through the circle
             * intersection points crosses the line between the circle
             * centers.  
             */

            /* Determine the distance from point 0 to point 2. */
            a = ((r0 * r0) - (r1 * r1) + (d * d)) / (2.0 * d);

            /* Determine the coordinates of point 2. */
            x2 = x0 + (dx * a / d);
            y2 = y0 + (dy * a / d);

            /* Determine the distance from point 2 to either of the
             * intersection points.
             */
            h = Math.Sqrt((r0 * r0) - (a * a));

            /* Now determine the offsets of the intersection points from
             * point 2.
             */
            rx = -dy * (h / d);
            ry = dx * (h / d);

            /* Determine the absolute intersection points. */
            xi = x2 + rx;
            xi_prime = x2 - rx;
            yi = y2 + ry;
            yi_prime = y2 - ry;

            return true;
        }

    }
}
