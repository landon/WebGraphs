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
        static int MaxDegree = int.MaxValue;
        static bool CheckNearColorings = true;
        static bool TreesOnly = false;
        static readonly string WinnersFile = (CheckNearColorings ? "no near colorings lost " : "") + (MaxDegree != int.MaxValue ? "max degree " + MaxDegree : "") + (TreesOnly ? "trees only " : "") + "superabundance.txt";

        public static void Go()
        {
            using (var graphEnumerator = new GraphEnumerator(WinnersFile, 2, MaxVertices))
            {
                if (TreesOnly)
                    graphEnumerator.FileRoot = GraphEnumerator.TreeFileRoot;
                graphEnumerator.WeightCondition = GraphEnumerator.WeightConditionFalse;

                foreach (var g in graphEnumerator.EnumerateGraph6File(Filter, EnumerateWeightings))
                {
                    System.Console.Write("checking " + g.ToGraph6() + " with degrees [" + string.Join(",", g.VertexWeight) + "] ...");

                    var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g);
                    mind.MaxPot = int.MaxValue;
                    mind.SuperabundantOnly = true;

                    var template = new Template(g.VertexWeight);
                    var win = mind.Analyze(template, null);

                    if (win)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Blue;
                        System.Console.WriteLine(" fixer wins");
                        System.Console.ForegroundColor = ConsoleColor.White;
                        graphEnumerator.AddWinner(g);
                        continue;
                    }
                    else if (CheckNearColorings)
                    {
                        if (mind.BreakerWonBoards.All(bb => !mind.NearlyColorableForSomeEdge(bb)))
                        {
                            System.Console.ForegroundColor = ConsoleColor.Green;
                            System.Console.WriteLine(" fixer wins all nearly colorable boards");
                            System.Console.ForegroundColor = ConsoleColor.White;
                            graphEnumerator.AddWinner(g);
                            continue;
                        }
                    }
                 
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(" breaker wins");
                    System.Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        static bool Filter(Choosability.Graph g)
        {
            if (g.MaxDegree > MaxDegree)
                return false;

            return true;
        }

        static IEnumerable<Choosability.Graph> EnumerateWeightings(Choosability.Graph g)
        {
            foreach (var weighting in g.Vertices.Select(v => Enumerable.Range(g.Degree(v), 2)).CartesianProduct())
            {
                var www = weighting.ToList();
                if (weighting.Any(w => w <= 1))
                    continue;

                var gg = g.Clone();
                gg.VertexWeight = www;
                yield return gg;
            }
        }
    }
}
