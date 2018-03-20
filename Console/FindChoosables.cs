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
        const int MaxVertices = 16;

        const bool Offline = false;
        const bool AT = false;
        const int MaxIndependenceNumber = int.MaxValue;
        const int Fold = 1;
        
        const int MaxDegree = int.MaxValue;
        const bool DiamondFreeOnly = false;
        const bool LineGraph = false;
        const bool ListerRestrictedEleE = false;
        const bool ListerRestrictedEgeE = false;
        const bool ListerRestrictedWeird = false;

        const int FixedNotK = 3;
        const bool IsFixedKColorable = false;

        const bool MinimizeAverageDegree = true;

        static readonly string WinnersFile = ("bit level help ") + (MinimizeAverageDegree ? "minimizing average degree " : "") + (IsFixedKColorable ? $" is {FixedNotK} colorable " : "") + (FixedNotK != int.MaxValue ? "fixed not " + FixedNotK + " " : "") + (ListerRestrictedWeird ? " lister restricted weird " : "") + (ListerRestrictedEgeE ? " lister restricted ge " : "") + (ListerRestrictedEleE ? " lister restricted le " : "") + (MaxIndependenceNumber < int.MaxValue ? "alpha at most " + MaxIndependenceNumber + " " : "") + (DiamondFreeOnly ? "cliquey neighborhoods " : "") + (AT ? "AT " : "") + (Offline ? "offline " : "") + (LineGraph ? "line graph " : "") + (MaxDegree < int.MaxValue ? "max degree " + MaxDegree + "_" : "") + string.Format("winners{0}.txt", Fold);
        public static void Go()
        {
            var minAverageDegree = double.MaxValue;

            var diamond = Choosability.Graphs.Diamond;
            using (var graphIO = new GraphEnumerator(WinnersFile, MinVertices, MaxVertices))
            {
                // graphIO.FileRoot = @"C:\Graph6\graph";
                graphIO.FileRoot = @"F:\graphs\4pccar\4pccar";
                
                foreach (var g in graphIO.EnumerateGraph6File())
                {
                    if (MaxIndependenceNumber < int.MaxValue)
                    {
                        if (g.EnumerateMaximalIndependentSets().Any(mis => mis.Count > MaxIndependenceNumber))
                            continue;
                    }
                    
                    if (MaxDegree < int.MaxValue && g.MaxDegree > MaxDegree)
                        continue;

                    if (DiamondFreeOnly && g.ContainsInduced(diamond))
                        continue;

                    if (MinimizeAverageDegree)
                    {
                        var avgd = 2.0 * g.E / g.N;
                        if (avgd > minAverageDegree)
                        {
                            System.Console.WriteLine("avgd excluded: " + minAverageDegree);
                            continue;
                        }

                        var mic = g.Mic();
                        if (2 * g.E < (FixedNotK - 1) * g.N + mic + 1)
                        {
                            System.Console.WriteLine("mic excluded: " + minAverageDegree);
                            continue;
                        }

                        var notKColorable = false;

                        if (FixedNotK == 3)
                        {
                            notKColorable = !g.Is3Colorable();
                        }
                        else
                        {
                            var chi = g.FindChiColoring().Count;
                            notKColorable = chi > FixedNotK;
                        }

                        if (notKColorable)
                        {
                            System.Console.WriteLine(FixedNotK + "-color excluded: " + minAverageDegree);
                            continue;
                        }
                    }

                    if (AT)
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
                        if (bg.IsFChoosable(v => bg.Degree(v) - 1, out badAssignment))
                        {
                            graphIO.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is {0}-fold d_1-choosable", Fold));
                        }
                        else
                            System.Console.WriteLine(" not choosable");
                    }
                    else if (ListerRestrictedEleE)
                    {
                        System.Console.Write("checking " + g.ToGraph6() + "...");
                        if (g.IsOnlineFGChoosableListerRestricted(v => Fold * g.Degree(v) - 1, v => Fold, CompareEleE))
                        {
                            graphIO.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is {0}-fold d_1-paintable (lister restricted || < ||)", Fold));
                        }
                        else
                        {
                            System.Console.WriteLine(" not paintable");
                        }
                    }
                    else if (ListerRestrictedEgeE)
                    {
                        System.Console.Write("checking " + g.ToGraph6() + "...");
                        if (g.IsOnlineFGChoosableListerRestricted(v => Fold * g.Degree(v) - 1, v => Fold, CompareEgeE))
                        {
                            graphIO.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is {0}-fold d_1-paintable (lister restricted || > ||)", Fold));
                        }
                        else
                        {
                            System.Console.WriteLine(" not paintable");
                        }
                    }
                    else if (ListerRestrictedWeird)
                    {
                        System.Console.Write("checking " + g.ToGraph6() + "...");
                        if (g.IsOnlineFGChoosableListerRestricted(v => Fold * g.Degree(v) - 1, v => Fold, CompareWeird))
                        {
                            graphIO.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is {0}-fold d_1-paintable (lister restricted weird)", Fold));
                        }
                        else
                        {
                            System.Console.WriteLine(" not paintable");
                        }
                    }
                    else if (FixedNotK != int.MaxValue)
                    {
                        System.Console.Write("checking " + g.ToGraph6() + "...");
                        if (!g.IsOnlineFGChoosable2(v => FixedNotK, v => 1))
                        {
                            if (IsFixedKColorable)
                            {
                                var chi = g.FindChiColoring().Count;
                                if (chi <= FixedNotK)
                                {
                                    graphIO.AddWinner(g);
                                    System.Console.WriteLine($" is not {FixedNotK}-paintable, but is so colorable");
                                }
                                else
                                {
                                    System.Console.WriteLine($" is not {FixedNotK}-colorable even");
                                }
                            }
                            else
                            {
                                var s = "";
                                if (MinimizeAverageDegree)
                                {
                                    var critical = true;
                                    foreach (var v in g.Vertices)
                                    {
                                        if (!g.IsOnlineFGChoosable2(w => FixedNotK, w => (w == v ? 0 : 1)))
                                        {
                                            critical = false;
                                            break;
                                        }
                                    }

                                    if (!critical)
                                        continue;

                                    if (!g.IsComplete())
                                    {
                                        minAverageDegree = 2.0 * g.E / g.N;
                                        s = "average degree = " + minAverageDegree;
                                    }
                                }

                                graphIO.AddWinner(g);
                                System.Console.WriteLine($" is not {FixedNotK}-paintable " + s);
                            }
                        }
                        else
                        {
                            var s = "";
                            if (MinimizeAverageDegree)
                            {
                                s = "best average degree = " + minAverageDegree;
                            }

                            System.Console.WriteLine($" is {FixedNotK}-paintable " + s);
                        }
                    }
                    else
                    {
                        System.Console.Write("checking " + g.ToGraph6() + "...");
                        if (g.IsOnlineFGChoosable(v => Fold * g.Degree(v) - 1, v => Fold))
                        {
                            graphIO.AddWinner(g);
                            System.Console.WriteLine(string.Format(" is {0}-fold d_1-paintable", Fold));
                        }
                        else
                        {
                            System.Console.WriteLine(" not paintable");
                        }
                    }
                }
            }
        }

        static int CompareEleE(Choosability.Graph current, Choosability.Graph last)
        {
            return current.E.CompareTo(last.E);
        }

        static int CompareEgeE(Choosability.Graph current, Choosability.Graph last)
        {
            return last.E.CompareTo(current.E);
        }

        static int CompareWeird(Choosability.Graph current, Choosability.Graph last)
        {
            return last.MaxDegree.CompareTo(current.MaxDegree);
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

