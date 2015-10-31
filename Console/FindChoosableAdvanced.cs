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
using System.Threading;

namespace Console
{
    public static class FindChoosablesAdvanced
    {
        const int MinVertices = 4;
        const int MaxVertices = 14;
        const int MinRingSize = 4;
        const int MaxRingSize = 14;
        const int MinDegree = 6;
        const int MaxDegree = 8;

        const bool Offline = false;
        const bool AT = true;

        static readonly string WinnersFile = "PreFilterW " + ("ring size " + MinRingSize + " -- " + MaxRingSize) + ("degrees " + MinDegree + " -- " + MaxDegree) + ("planar triangulation") + (AT ? "AT " : "") + (Offline ? "offline " : "") + string.Format("winners.txt");
        public static void Go()
        {
            File.Delete(WinnersFile);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            int winnersFound = 0;
            for (int N = MinVertices; N <= MaxVertices; N++)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                System.Console.WriteLine("Checking " + N + " vertex graphs...");
                System.Console.ForegroundColor = ConsoleColor.White;

                for (var R = MinRingSize; R <= MaxRingSize; R++)
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkGray;
                    System.Console.WriteLine("Checking ring size " + R + "...");
                    System.Console.ForegroundColor = ConsoleColor.White;

                    var path = string.Format(@"C:\Users\landon\Google Drive\research\Graph6\triangulation\disk\triangulation{0}_{1}.g6.tri.weighted.txt", N, R);
                    if (!File.Exists(path))
                        continue;

                    foreach (var h in path.EnumerateWeightedGraphs())
                    {
                        foreach (var g in EnumerateWeightings(h))
                        {
                            if (AT)
                            {
                                var result = HasFOrientation(g, v => g.Degree(v) - g.VertexWeight[v], true);
                                if (result != null)
                                {
                                    winnersFound++;
                                    result.Graph.VertexWeight = g.VertexWeight;
                                    result.Graph.AppendToFile(WinnersFile);
                                    System.Console.ForegroundColor = ConsoleColor.Green;
                                    System.Console.WriteLine(string.Format("found {0} AT graph{1} so far", winnersFound, winnersFound > 1 ? "s" : ""));
                                    System.Console.ForegroundColor = ConsoleColor.White;
                                }
                                else
                                {

                                }
                            }
                            else if (Offline)
                            {
                                List<List<int>> badAssignment;
                                var bg = new BitGraph_long(g.GetEdgeWeights());

                                System.Console.Write("checking " + g.ToGraph6() + "...");
                                if (bg.IsFChoosable(v => bg.Degree(v) - g.VertexWeight[v], out badAssignment))
                                {
                                    g.AppendToFile(WinnersFile);
                                    System.Console.WriteLine(string.Format(" is choosable"));
                                }
                                else
                                    System.Console.WriteLine(" not choosable");
                            }
                            else
                            {
                                System.Console.Write("checking " + g.ToGraph6() + "...");
                                if (g.IsOnlineFChoosable(v => g.Degree(v) - g.VertexWeight[v], token))
                                {
                                    g.AppendToFile(WinnersFile);
                                    System.Console.WriteLine(string.Format(" is paintable"));
                                }
                                else
                                    System.Console.WriteLine(" not paintable");
                            }
                        }
                    }
                }
            }
        }

        static IEnumerable<Graph> EnumerateWeightings(Graph g)
        {
            var space = g.Vertices.Select(v => Enumerable.Range(MinDegree - 5, Math.Min(g.VertexWeight[v], MaxDegree) - MinDegree + 1).Reverse()).CartesianProduct();
            foreach (var weighting in space)
            {
                var www = weighting.ToList();

                if (g.Vertices.Any(v => g.Degree(v) > www[v] + 5))
                    continue;

                if (g.Vertices.Any(v => www[v] >= g.Degree(v)))
                    continue;

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

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var tasks = new Task[2];
            tasks[0] = Task<bool>.Factory.StartNew(() => g.IsOnlineFChoosable(v => g.Degree(v) - g.VertexWeight[v], token), token);
            tasks[1] = Task<OrientationResult>.Factory.StartNew(() =>
                {
                    foreach (var orientation in g.EnumerateOrientations(v => g.Degree(v) + 1 - f(v)))
                    {
                        if (token.IsCancellationRequested)
                            return null;
                        var result = CheckOrientation(orientation, degreeSequences, useDerivative);
                        if (result != null)
                            return result;
                    }

                    return null;
                }, token);


            var doneTask = tasks[Task.WaitAny(tasks)];
            if (doneTask is Task<OrientationResult>)
            {
                var result = ((Task<OrientationResult>)doneTask).Result;
                tokenSource.Cancel();

                return result;
            }
            else
            {
                if (((Task<bool>)doneTask).Result)
                {
                    return ((Task<OrientationResult>)tasks[1]).Result;
                }
                else
                {
                    tokenSource.Cancel();
                    return null;
                }
            }
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

