using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitLevelGeneration;
using Choosability;
using Choosability.Polynomials;

namespace Console
{
    public static class FindChoosables
    {
        const int MinVertices = 4;
        const int MaxVertices = 9;

        const bool Offline = false;
        const bool AT = true;

        const bool Neighborhood = true;
        const int MaxIndependenceNumber = 2;

        const int Fold = 1;
        
        const int MaxDegree = int.MaxValue;
        const bool DiamondFreeOnly = false;
        const bool LineGraph = false;
        const string Graph6FileRoot = @"C:\Graph6\graph";
        static readonly string WinnersFile = (MaxIndependenceNumber < int.MaxValue ? "alpha at most " + MaxIndependenceNumber + " " : "") + (DiamondFreeOnly ? "cliquey neighborhoods " : "") + (AT ? "AT " : "") + (Offline ? "offline " : "") + (LineGraph ? "line graph " : "") + (MaxDegree < int.MaxValue ? "max degree " + MaxDegree + "_" : "") + string.Format((Neighborhood ? "nhbd_" : "") + "winners{0}.txt", Fold);
        static readonly string LastTriedFile = "last tried " + WinnersFile;

        public static void Go()
        {
            System.Console.WriteLine("Loading previous winners...");
            var choosables = LoadPreviousWinners();
            System.Console.WriteLine("Found " + choosables.Count + " previous winners.");

            using (var sw = new StreamWriter(WinnersFile))
            {
                Graph last = null;

                foreach (var g in choosables)
                {
                    WriteGraph(sw, g);
                    last = g;
                }

                if (File.Exists(LastTriedFile))
                {               
                        using (var sr = new StreamReader(LastTriedFile))
                        {
                            var ew = sr.ReadToEnd().GetEdgeWeights();
                            last = new Graph(ew);
                            System.Console.WriteLine("last tried graph: " + ew.ToGraph6());
                        }
                }
                else if (last != null)
                    System.Console.WriteLine("last tried graph: " + last.GetEdgeWeights().ToGraph6());

                foreach (var gg in EnumerateGraph6File(Graph6FileRoot, last))
                {
                    if (MaxIndependenceNumber < int.MaxValue)
                    {
                        if (gg.EnumerateMaximalIndependentSets().Any(mis => mis.Count > MaxIndependenceNumber))
                            continue;
                    }

                    var g = Neighborhood ? Choosability.Graphs.K(1).Join(gg) : gg;

                    if (MaxDegree < int.MaxValue && g.MaxDegree > MaxDegree)
                        continue;

                    if (AT)
                    {
                        if (choosables.All(h => !g.ContainsInduced(h)))
                        {
                            System.Console.Write("checking " + g.ToGraph6() + "...");
                            if (!g.IsOnlineFGChoosable(v => g.Degree(v) - 1, v => 1))
                            {
                                System.Console.WriteLine(" not paintable");
                            }
                            else
                            {
                                var result = HasFOrientation(g, v => g.Degree(v) - 1, true);
                                if (result != null)
                                {
                                    choosables.Add(g);
                                    WriteGraph(sw, result.Graph);

                                    System.Console.WriteLine(string.Format(" is d_1-AT"));
                                }
                                else
                                    System.Console.WriteLine(" not AT");
                            }
                        }
                        else
                            System.Console.WriteLine("skipping supergraph " + g.ToGraph6());
                    }
                    else if (Offline)
                    {
                        List<List<int>> badAssignment;
                        var bg = new BitGraph_long(g.GetEdgeWeights());
                        if (choosables.All(h => !g.ContainsInduced(h)))
                        {
                            System.Console.Write("checking " + g.ToGraph6() + "...");
                            if (bg.IsFChoosable(v => bg.Degree(v) - 1, out badAssignment))
                            {

                                choosables.Add(g);
                                WriteGraph(sw, g);

                                System.Console.WriteLine(string.Format(" is {0}-fold d_1-choosable", Fold));
                            }
                            else
                                System.Console.WriteLine(" not choosable");
                        }
                        else
                            System.Console.WriteLine("skipping supergraph " + g.ToGraph6());
                    }
                    else
                    {
                        if (!DiamondFreeOnly || !g.ContainsInduced(Choosability.Graphs.Diamond))
                        {
                            if (choosables.All(h => !g.ContainsInduced(h)))
                            {
                                System.Console.Write("checking " + g.ToGraph6() + "...");
                                if (g.IsOnlineFGChoosable(v => Fold * g.Degree(v) - 1, v => Fold))
                                {

                                    choosables.Add(g);
                                    WriteGraph(sw, g);

                                    System.Console.WriteLine(string.Format(" is {0}-fold d_1-paintable", Fold));
                                }
                                else
                                    System.Console.WriteLine(" not paintable");
                            }
                            else
                                System.Console.WriteLine("skipping supergraph " + g.ToGraph6());
                        }
                        else
                            System.Console.WriteLine("skipping due to diamond " + g.ToGraph6());
                    }
                }
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

        static void WriteGraph(StreamWriter sw, Graph g)
        {
            var w = string.Join(" ", g.GetEdgeWeights().Select(x => x.ToString()));
            sw.WriteLine(w);
            sw.Flush();
        }

        static List<Graph> LoadPreviousWinners()
        {
            var winners = new List<Graph>();
            try
            {
                try
                {
                    File.Copy(WinnersFile, "previous " + WinnersFile);
                }
                catch { }

                using (var sr = new StreamReader("previous " + WinnersFile))
                {
                    while (true)
                    {
                        var line = sr.ReadLine();
                        if (line == null)
                            break;

                        winners.Add(new Graph(line.Split(' ').Select(x => int.Parse(x)).ToList()));
                    }
                }
            }
            catch { }

            return winners;
        }

        static IEnumerable<Graph> EnumerateGraph6File(string file, Graph previousLast = null)
        {
            var min = MinVertices;
            string lastGraph6 = null;
            var foundPreviousLast = true;
            if (previousLast != null)
            {
                min = previousLast.N;
                lastGraph6 = previousLast.GetEdgeWeights().ToGraph6();
                foundPreviousLast = false;
            }

            for (int N = min; N <= MaxVertices; N++)
            {
                System.Console.WriteLine("Checking " + N + " vertex graphs...");
                using (var sr = new StreamReader(file + N + ".txt"))
                {
                    while (true)
                    {
                        var line = sr.ReadLine();
                        if (line == null)
                            goto next;

                        if (!foundPreviousLast)
                        {
                            if (line.StartsWith(lastGraph6))
                                foundPreviousLast = true;
                            continue;
                        }

                        var ew = line.GetEdgeWeights();
                        yield return new Graph(ew);

                        using (var sw = new StreamWriter(LastTriedFile))
                            sw.WriteLine(ew.ToGraph6());
                    }
                }

            next: ;
            }
        }
    }
}

