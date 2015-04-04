using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability.Utility;
using Choosability.FixerBreaker.KnowledgeEngine;

namespace Console
{
    public static class FindFixerBreaker
    {
        const int Delta = 4;
        const int MaxVertices = 10;
        const bool NearColorings = false;
        const bool TreesOnly = true;
        static readonly string WinnersFile = (TreesOnly ? "trees only " : "") + (NearColorings ? "near colorings " : "") + "FixerBreaker winners Delta=" + Delta + ".txt";

        public static void Go()
        {
            using (var graphIO = new GraphEnumerator(WinnersFile, 2, MaxVertices))
            {
                foreach (var g in graphIO.EnumerateGraph6File(Filter, EnumerateWeightings))
                {
                    System.Console.Write("checking " + g.ToGraph6() + " with degrees [" + string.Join(",", g.VertexWeight) + "] ...");

                    var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g);
                    mind.MaxPot = Delta;
                    mind.OnlyNearlyColorable = NearColorings;

                    var template = new Template(g.VertexWeight.Select((ambientDegree, v) => Delta - (ambientDegree - g.Degree(v))).ToList());
                    var win = mind.Analyze(template, null);

                    if (win)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Blue;
                        System.Console.WriteLine(" fixer wins");
                        System.Console.ForegroundColor = ConsoleColor.White;
                        graphIO.AddWinner(g);
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

        static bool Filter(Choosability.Graph g)
        {
            if (TreesOnly)
                return g.E == g.N - 1;

            return true;
        }

        static IEnumerable<Choosability.Graph> EnumerateWeightings(Choosability.Graph g)
        {
            if (g.MaxDegree > Delta)
                yield break;

            foreach (var weighting in g.Vertices.Select(v => Enumerable.Range(g.Degree(v), Delta + 1 - g.Degree(v)).Reverse()).CartesianProduct())
            {
                var gg = g.Clone();
                gg.VertexWeight = weighting.ToList();
                yield return gg;
            }
        }
    }
}
