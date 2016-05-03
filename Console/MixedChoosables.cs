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
    public static class MixedChoosables
    {
        const int MinVertices = 2;
        const int MaxVertices = 9;

        const bool Offline = false;
        const bool AT = false;
        const bool Mic = false;
        const bool TwoConnectedOnly = false;
        const int MaxIndependenceNumber = int.MaxValue;
        const int Fold = 1;
        const int Spread = 10;
        const int MaxHighs = 1;
        const bool Not = false;

        const int MaxDegree = int.MaxValue;
        const int MinDegree = 0;
        const int LowMinDegree = 0;
        const bool OddAT = false;

        const bool LineGraph = false;

        static readonly string WinnersFile = (OddAT ? "odd AT " : "") + (MinDegree > 0 ? "min deg " + MinDegree : "") + (LowMinDegree > 0 ? LowMinDegree + "low min " : "") + (Not ? "not " : "") + " " + MaxVertices + " vertex " + "Mixed spread " + Spread + " " + (MaxHighs < int.MaxValue ? "max high " + MaxHighs + " " : "") + (TwoConnectedOnly ? "kappa2 " : "") + (Mic ? "mic " : "") + (MaxIndependenceNumber < int.MaxValue ? "alpha at most " + MaxIndependenceNumber + " " : "") + (AT ? "AT " : "") + (Offline ? "offline " : "") + (LineGraph ? "line graph " : "") + (MaxDegree < int.MaxValue ? "max degree " + MaxDegree + "_" : "") + string.Format("winners{0}.txt", Fold);
        public static void Go()
        {
            using (var graphEnumerator = new GraphEnumerator(WinnersFile, MinVertices, MaxVertices, false))
            {
                graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";
                graphEnumerator.WeightCondition = WeightCondition;
                graphEnumerator.OnlyExcludeBySpanningSubgraphs = false;
                foreach (var g in graphEnumerator.EnumerateGraph6File(Filter, EnumerateWeightings, induced: true))
                {
                    if (MaxIndependenceNumber < int.MaxValue)
                    {
                        if (g.EnumerateMaximalIndependentSets().Any(mis => mis.Count > MaxIndependenceNumber))
                            continue;
                    }

                    if (g.MaxDegree > MaxDegree)
                        continue;

                    if (g.MinDegree < MinDegree)
                        continue;

                    if (Mic)
                    {
                        System.Console.Write("checking " + g.ToGraph6() + " with weights [" + string.Join(",", g.VertexWeight) + "] ...");

                        var micScore = g.Vertices.Sum(v => g.VertexWeight[v] + 1);

                        if (g.Mic() >= micScore)
                        {
                            graphEnumerator.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is f-KP"));
                        }
                        else
                            System.Console.WriteLine(" not f-mic'able");
                    }
                    else if (AT)
                    {
                        System.Console.Write("checking " + g.ToGraph6() + " with weights [" + string.Join(",", g.VertexWeight) + "] ...");
                        if (!Not)
                        {
                            if (!g.IsOnlineFGChoosable(v => g.Degree(v) - g.VertexWeight[v], v => 1))
                            {
                                System.Console.WriteLine(" not paintable");
                            }
                            else
                            {
                                var result = HasFOrientation(g, v => g.Degree(v) - g.VertexWeight[v], true);

                                if (result != null)
                                {
                                    if (OddAT)
                                    {
                                        if (Math.Abs(result.Even - result.Odd) % 2 == 1)
                                        {
                                            graphEnumerator.AddWinner(g, result.Graph);
                                            System.Console.WriteLine(string.Format(" is odd f-AT"));
                                        }
                                        else
                                        {
                                            System.Console.WriteLine(string.Format(" not odd f-AT"));
                                        }

                                    }
                                    else
                                    {
                                        graphEnumerator.AddWinner(g, result.Graph);
                                        System.Console.WriteLine(string.Format(" is f-AT"));
                                    }
                                }
                                else
                                    System.Console.WriteLine(" not AT");
                            }
                        }
                        else
                        {
                            var good = !g.IsOnlineFGChoosable(v => g.Degree(v) - g.VertexWeight[v], v => 1);
                            var result = HasFOrientation(g, v => g.Degree(v) - g.VertexWeight[v], true);
                            good |= result == null || OddAT && Math.Abs(result.Even - result.Odd) % 2 == 0;
                            if (good)
                            {
                                graphEnumerator.AddWinner(g);
                                System.Console.WriteLine(string.Format(" not f-AT"));
                            }
                            else
                                System.Console.WriteLine(" is AT");
                        }
                    }
                    else if (Offline)
                    {
                        List<List<int>> badAssignment;
                        var bg = new BitGraph_long(g.GetEdgeWeights());

                        System.Console.Write("checking " + " with weights [" + string.Join(",", g.VertexWeight) + "] ...");
                        if (bg.IsFChoosable(v => bg.Degree(v) - g.VertexWeight[v], out badAssignment))
                        {
                            graphEnumerator.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is {0}-fold f-choosable", Fold));
                        }
                        else
                            System.Console.WriteLine(" not choosable");
                    }
                    else
                    {
                        System.Console.Write("checking " + " with weights [" + string.Join(",", g.VertexWeight) + "] ...");
                        if (!Not)
                        {
                            if (g.IsOnlineFGChoosable(v => Fold * g.Degree(v) - g.VertexWeight[v], v => Fold))
                            {
                                graphEnumerator.AddWinner(g);
                                System.Console.WriteLine(string.Format(" is {0}-fold f-paintable", Fold));
                            }
                            else
                                System.Console.WriteLine(" not paintable");
                        }
                        else
                        {
                            if (!g.IsOnlineFGChoosable(v => Fold * g.Degree(v) - g.VertexWeight[v], v => Fold))
                            {
                                graphEnumerator.AddWinner(g);
                                System.Console.WriteLine(string.Format(" is not {0}-fold f-paintable", Fold));
                            }
                            else
                                System.Console.WriteLine(" paintable");
                        }
                    }
                }
            }

            if (Not)
                EliminateDoubleEdgeNotSubdivisions.Go(WinnersFile, AT);
            else
                EliminiteDoubleEdgeSubdivisions.Go(WinnersFile);
        }

        public static bool WeightCondition(Graph self, Graph A, int selfV, int av)
        {
            if (Not)
                return GraphEnumerator.WeightConditionUp(self, A, selfV, av);
            return GraphEnumerator.WeightConditionDown(self, A, selfV, av);
        }

        static bool Filter(Choosability.Graph g)
        {
            if (TwoConnectedOnly)
                return g.Vertices.All(v => g.FindComponents(g.Vertices.Except(new[] { v }).ToList()).GetEquivalenceClasses().Count() <= 1);
         
            return true;
        }

        static IEnumerable<Choosability.Graph> EnumerateWeightings(Choosability.Graph g)
        {
            var space = Not ? g.Vertices.Select(v => Enumerable.Range(0, Spread)).CartesianProduct() : g.Vertices.Select(v => Enumerable.Range(0, Spread).Reverse()).CartesianProduct();
            foreach (var weighting in space)
            {
                var www = weighting.ToList();
                if (www.Count(w => w > 0) > MaxHighs)
                    continue;

                if (g.Vertices.Any(v => g.Degree(v) - www[v] <= 1))
                    continue;

                if (LowMinDegree > 0)
                {
                    var low = www.IndicesWhere(w => w == 0).ToList();
                    if (g.InducedSubgraph(low).MinDegree < LowMinDegree)
                        continue;
                }

                var gg = g.Clone();
                gg.VertexWeight = www;

                yield return gg;
            }
        }

        public static OrientationResult HasFOrientation(Graph g, Func<int, int> f, bool useSchauz = true)
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

                var result = CheckOrientation(o, degreeSequences, useSchauz);
                if (result != null)
                    return result;
            }

            foreach (var orientation in g.EnumerateOrientations(v => g.Degree(v) + 1 - f(v)))
            {
                var result = CheckOrientation(orientation, degreeSequences, useSchauz);
                if (result != null)
                    return result;
            }

            return null;
        }

        static OrientationResult CheckOrientation(Graph orientation, List<int[]> degreeSequences, bool useSchauz = true)
        {
            if (degreeSequences.Any(seq => Enumerable.SequenceEqual(seq, orientation.InDegreeSequence.Value)))
                return null;

            degreeSequences.Add(orientation.InDegreeSequence.Value);

            if (useSchauz)
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

        public class OrientationResult
        {
            public Graph Graph { get; set; }
            public int Even { get; set; }
            public int Odd { get; set; }
        }
    }
}
