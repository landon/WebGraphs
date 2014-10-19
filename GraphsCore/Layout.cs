using MathNet.Numerics.LinearAlgebra.Double;
using Satsuma;
using Satsuma.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphsCore
{
    public static class Layout
    {
        public delegate List<Tuple<double, double>> Algorithm(Choosability.Graph g);

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

        public static List<Tuple<double, double>> GetSpringsLayout(this Choosability.Graph g)
        {
            return GetSpringsLayout(g, 0);
        }

        public static List<Tuple<double, double>> GetSpringsLayout(this Choosability.Graph g, int randomSeed)
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

            return satsumaGraph.Nodes().Select(n => new Tuple<double, double>(0.1 + 0.7 * (layout.NodePositions[n].X - minX) / width, 0.1 + 0.7 * (layout.NodePositions[n].Y - minY) / height)).ToList();
        }

        public static List<Tuple<double, double>> GetLaplacianLayout(this Choosability.Graph g)
        {
            var D = Matrix.Build.Diagonal(g.N, g.N, v => g.Degree(v));
            var A = Matrix.Build.Dense(g.N, g.N, (v, w) => g[v, w] ? 1 : 0);
            var L = D - A;

            var evd = L.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Symmetric);
            var x = evd.EigenVectors.Column(1);
            var y = evd.EigenVectors.Column(2);

            return GetEigenVectorLayout(x, y);
        }

        public static List<Tuple<double, double>> GetWalkMatrixLayout(this Choosability.Graph g)
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

        static List<Tuple<double, double>> GetEigenVectorLayout(MathNet.Numerics.LinearAlgebra.Vector<double> x, MathNet.Numerics.LinearAlgebra.Vector<double> y)
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

            return Enumerable.Range(0, x.Count).Select(v => new Tuple<double, double>(0.1 + 0.7 * (x[v] - minX) / width, 0.1 + 0.7 * (y[v] - minY) / height)).ToList();
        }
    }
}
