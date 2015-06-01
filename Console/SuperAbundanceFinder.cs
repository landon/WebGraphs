using Choosability.Utility;
using Choosability.FixerBreaker.KnowledgeEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class SuperAbundanceFinder
    {
        static int MaxVertices = 20;
        static int MaxDegree = 3;
        static bool TreesOnly = true;
        static int ExtraPsi = 2;
        static readonly string WinnersFile = "nar " + (ExtraPsi > 0 ? ExtraPsi + " extra psi " : "") + (MaxDegree != int.MaxValue ? "max degree " + MaxDegree : "") + (TreesOnly ? "trees only " : "") + "superabundance.txt";

        public static void Go()
        {
            using (var graphEnumerator = new GraphEnumerator(WinnersFile, 2, MaxVertices))
            {
                if (TreesOnly)
                    graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\trees\trees";
                else
                    graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";

                graphEnumerator.DoNotUsePreviousWinners = true;
                foreach (var gg in graphEnumerator.EnumerateGraph6File(Filter, null))
                {
                    System.Console.Write("checking " + gg.ToGraph6() + "... ");

                    var weightings = EnumerateWeightings(gg).ToList();
                    var winners = new List<Choosability.Graph>();
                    foreach (var g in weightings)
                    {
                        var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g, false, true);
                        mind.MaxPot = int.MaxValue;
                        mind.SuperabundantOnly = true;
                        mind.DoComplexSwapsInProof = false;
                        mind.ExtraPsi = ExtraPsi;

                        var template = new Template(g.VertexWeight);
                        var win = mind.Analyze(template, null);

                        if (win)
                            winners.Add(g);
                    }
                    
                    var globalWinners = winners.Where(w => SubsetEqualSpecial(weightings.Where(aa => w.VertexWeight.Zip(aa.VertexWeight, (x, y) => x <= y).All(b => b)).ToList(), winners)).ToList();
                    globalWinners = globalWinners.Where(w => IntersectionCountSpecial(weightings.Where(aa => w.VertexWeight.Zip(aa.VertexWeight, (x, y) => x >= y).All(b => b)).ToList(), globalWinners) == 1).ToList();
                    
                    if (globalWinners.Count > 0)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Green;
                        System.Console.WriteLine(" fixer wins");
                        System.Console.ForegroundColor = ConsoleColor.White;

                        foreach (var ww in globalWinners)
                            graphEnumerator.AddWinner(ww);
                    }
                    else
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine(" breaker wins");
                        System.Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }

        static bool SubsetEqualSpecial(List<Choosability.Graph> list1, List<Choosability.Graph> list2)
        {
            foreach (var g in list1)
            {
                var bad = true;
                foreach (var gg in list2)
                {
                    if (g.VertexWeight.SequenceEqual(gg.VertexWeight))
                    {
                        bad = false;
                        break;
                    }
                }

                if (bad)
                    return false;
            }

            return true;
        }

        static int IntersectionCountSpecial(List<Choosability.Graph> list1, List<Choosability.Graph> list2)
        {
            var count = 0;
            foreach (var g in list1)
            {
                var bad = true;
                foreach (var gg in list2)
                {
                    if (g.VertexWeight.SequenceEqual(gg.VertexWeight))
                    {
                        bad = false;
                        break;
                    }
                }

                if (!bad)
                    count++;
            }

            return count;
        }

        static bool Filter(Choosability.Graph g)
        {
            if (g.MaxDegree > MaxDegree)
                return false;

            return true;
        }

        static IEnumerable<Choosability.Graph> EnumerateWeightings(Choosability.Graph g)
        {
            foreach (var weighting in g.Vertices.Select(v => Enumerable.Range(g.Degree(v), 2 + 1)).CartesianProduct())
            {
                var gg = g.Clone();
                gg.VertexWeight = weighting.ToList();
                yield return gg;
            }
        }
    }
}
