using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitLevelGeneration;
using Choosability;
using Choosability.Polynomials;
using Choosability.Utility;

namespace Console
{
    public static class FindChoosablesAdvanced
    {
        const int MinVertices = 4;
        const int MaxVertices = 14;
        const int RingSize = 11;

        const bool Offline = false;
        const bool AT = true;
        const int Spread = 4;
        const int MinSpread = 0;

        const int MaxDegree = int.MaxValue;
        const bool PlanarTriangulation = true;

        static readonly string WinnersFile = ("ring size " + RingSize + " ") + ("min spread " + MinSpread + " ") + ("spread " + Spread + " ") + (PlanarTriangulation ? "planar triangulation" : "") + (AT ? "AT " : "") + (Offline ? "offline " : "") + string.Format("winners.txt");
        public static void Go()
        {
            using (var graphIO = new GraphEnumerator(WinnersFile, MinVertices, MaxVertices, true))
            {
                graphIO.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\triangulation\disk\triangulation";
                graphIO.RingSize = RingSize;

                foreach (var g in graphIO.EnumerateGraph6File(Filter, EnumerateWeightings))
                {
                    if (AT)
                    {
                        System.Console.Write("checking " + g.ToGraph6() + "...");
                        if (!g.IsOnlineFGChoosable(v => g.Degree(v) - g.VertexWeight[v], v => 1))
                        {
                            System.Console.WriteLine(" not paintable");
                        }
                        else
                        {
                            var result = HasFOrientation(g, v => g.Degree(v) - g.VertexWeight[v], true);
                            if (result != null)
                            {
                                graphIO.AddWinner(g, result.Graph);
                                System.Console.WriteLine(string.Format(" is d_1-AT"));
                            }
                            else
                                System.Console.WriteLine(" not AT");
                        }
                    }
                    else if (Offline)
                    {
                        List<List<int>> badAssignment;
                        var bg = new BitGraph_long(g.GetEdgeWeights());

                        System.Console.Write("checking " + g.ToGraph6() + "...");
                        if (bg.IsFChoosable(v => bg.Degree(v) - g.VertexWeight[v], out badAssignment))
                        {
                            graphIO.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is choosable"));
                        }
                        else
                            System.Console.WriteLine(" not choosable");
                    }
                    else
                    {
                        System.Console.Write("checking " + g.ToGraph6() + "...");
                        if (g.IsOnlineFGChoosable(v => g.Degree(v) - g.VertexWeight[v], v => 1))
                        {
                            graphIO.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is paintable"));
                        }
                        else
                            System.Console.WriteLine(" not paintable");
                    }
                }
            }
        }

        static bool Filter(Graph g)
        {
            return true;
        }

        static IEnumerable<Graph> EnumerateWeightings(Graph g)
        {
            var space = g.Vertices.Select(v => Enumerable.Range(MinSpread, Spread).Reverse()).CartesianProduct();
            foreach (var weighting in space)
            {
                var www = weighting.ToList();

                if (g.Vertices.Any(v => g.Degree(v) > www[v] + 5))
                    continue;

                if (g.Vertices.Any(v => www[v] >= g.Degree(v) - 1))
                    continue;

                if (g.Vertices.Any(v =>
                {
                    if (g.EdgesOn(g.Neighbors[v]) != g.Degree(v))
                        return false;

                    return www[v] + 5 > g.Degree(v) || g.Degree(v) <= 4;
                }))
                {
                    continue;
                }

                if (g.Vertices.Any(v => 
                    {
                        if (www[v] > 1)
                            return false;

                        var zn = g.Neighbors[v].Where(w => www[w] == 0).ToList();

                        return g.InducedSubgraph(zn).MaxDegree > 1;
                    }))
                {
                    continue;
                }

                var gg = g.Clone();
                gg.VertexWeight = www;

                yield return gg;
            }
        }

        static OrientationResult HasFOrientation(Graph g, Func<int, int> f, bool useDerivative = false)
        {
            const int RandomTries = 10;
            int MaxFails = 100;

            var degreeSequences = new List<int[]>();

            for (int i = 0; i < RandomTries; i++)
            {
                var o = g.GenerateRandomOrientation();

                if (Enumerable.Range(0, o.N).Any(v => f(v) <= o.OutDegree(v)))
                {
                    i--;
                    MaxFails--;
                    if (MaxFails < 0)
                        break;
                    continue;
                }

                var result = CheckOrientation(o, degreeSequences, useDerivative);
                if (result != null)
                    return result;
            }

            foreach (var orientation in g.EnumerateOrientations(v => g.Degree(v) + 1 - f(v)))
            {
                var result = CheckOrientation(orientation, degreeSequences, useDerivative);
                if (result != null)
                    return result;
            }

            return null;
        }

        static OrientationResult CheckOrientation(Graph orientation, List<int[]> degreeSequences, bool useDerivative = false)
        {
            if (degreeSequences.Any(seq => Enumerable.SequenceEqual(seq, orientation.InDegreeSequence.Value)))
                return null;

            degreeSequences.Add(orientation.InDegreeSequence.Value);

            if (useDerivative)
            {
                var c = orientation.GetCoefficient(orientation.Vertices.Select(v => orientation.OutDegree(v)).ToArray());
                if (c != 0)
                    return new OrientationResult() { Graph = orientation, Even = c, Odd = 0 };
            }
            else
            {
                int even, odd;
                orientation.CountSpanningEulerianSubgraphs(out even, out odd);

                if (even != odd)
                    return new OrientationResult() { Graph = orientation, Even = even, Odd = odd };
            }

            return null;
        }

        class OrientationResult
        {
            public Graph Graph { get; set; }
            public int Even { get; set; }
            public int Odd { get; set; }
        }
    }
}

