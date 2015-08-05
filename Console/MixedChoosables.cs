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
        const int MinVertices = 3;
        const int MaxVertices = 8;

        const bool Offline = true;
        const bool AT = false;
        const int MaxIndependenceNumber = int.MaxValue;
        const int Fold = 1;
        const int Spread = 2;

        const int MaxDegree = int.MaxValue;
        const bool DiamondFreeOnly = false;
        const bool LineGraph = false;

        static readonly string WinnersFile = "Mixed spread " + Spread + " " + (MaxIndependenceNumber < int.MaxValue ? "alpha at most " + MaxIndependenceNumber + " " : "") + (DiamondFreeOnly ? "cliquey neighborhoods " : "") + (AT ? "AT " : "") + (Offline ? "offline " : "") + (LineGraph ? "line graph " : "") + (MaxDegree < int.MaxValue ? "max degree " + MaxDegree + "_" : "") + string.Format("winners{0}.txt", Fold);
        public static void Go()
        {
            using (var graphEnumerator = new GraphEnumerator(WinnersFile, MinVertices, MaxVertices))
            {
                graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";
                foreach (var g in graphEnumerator.EnumerateGraph6File(Filter, EnumerateWeightings, induced:true))
                {
                    if (MaxIndependenceNumber < int.MaxValue)
                    {
                        if (g.EnumerateMaximalIndependentSets().Any(mis => mis.Count > MaxIndependenceNumber))
                            continue;
                    }

                    if (MaxDegree < int.MaxValue && g.MaxDegree > MaxDegree)
                        continue;

                    if (AT)
                    {
                        System.Console.Write("checking " + g.ToGraph6() + " with weights [" + string.Join(",", g.VertexWeight) + "] ...");
                        if (!g.IsOnlineFGChoosable(v => g.Degree(v) - g.VertexWeight[v], v => 1))
                        {
                            System.Console.WriteLine(" not paintable");
                        }
                        else
                        {
                            var result = HasFOrientation(g, v => g.Degree(v) - g.VertexWeight[v], true);
                            if (result != null)
                            {
                                graphEnumerator.AddWinner(g, result.Graph);
                                System.Console.WriteLine(string.Format(" is f-AT"));
                            }
                            else
                                System.Console.WriteLine(" not AT");
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
                        if (!DiamondFreeOnly || !g.ContainsInduced(Choosability.Graphs.Diamond))
                        {
                            System.Console.Write("checking " + " with weights [" + string.Join(",", g.VertexWeight) + "] ...");
                            if (g.IsOnlineFGChoosable(v => Fold * g.Degree(v) - g.VertexWeight[v], v => Fold))
                            {
                                graphEnumerator.AddWinner(g);
                                System.Console.WriteLine(string.Format(" is {0}-fold f-paintable", Fold));
                            }
                            else
                                System.Console.WriteLine(" not paintable");
                        }
                        else
                            System.Console.WriteLine("skipping due to diamond " + g.ToGraph6());
                    }
                }
            }
        }

        static bool Filter(Choosability.Graph g)
        {
            return true;
        }

        static IEnumerable<Choosability.Graph> EnumerateWeightings(Choosability.Graph g)
        {
            foreach (var weighting in g.Vertices.Select(v => Enumerable.Range(0, Spread).Reverse()).CartesianProduct())
            {
                var gg = g.Clone();
                gg.VertexWeight = weighting.ToList();

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
