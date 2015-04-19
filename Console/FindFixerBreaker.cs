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
        const int Delta = 3;
        const int MaxVertices = 20;
        const bool TreesOnly = true;
        
        const bool NearColorings = false;
        static readonly string WinnersFile = (TreesOnly ? "trees only " : "") + (NearColorings ? "near colorings " : "") + "FixerBreaker winners Delta=" + Delta + ".txt";

        public static void Go()
        {
            using (var graphEnumerator = new GraphEnumerator(WinnersFile, 2, MaxVertices))
            {
                graphEnumerator.TreesOnly = TreesOnly;
                foreach (var g in graphEnumerator.EnumerateGraph6File(Filter, EnumerateWeightings))
                {
                    System.Console.Write("checking " + g.ToGraph6() + " with degrees [" + string.Join(",", g.VertexWeight) + "] ...");

                    var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g);
                    mind.MaxPot = Delta;
                    mind.OnlyConsiderNearlyColorableBoards = NearColorings;

                    var template = new Template(g.VertexWeight.Select((ambientDegree, v) => Delta - (ambientDegree - g.Degree(v))).ToList());
                    var win = mind.Analyze(template, null);

                    if (win)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Blue;
                        System.Console.WriteLine(" fixer wins");
                        System.Console.ForegroundColor = ConsoleColor.White;
                        graphEnumerator.AddWinner(g);
                        _wonWeightings.Add(g.VertexWeight);
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
            return true;
        }

        static List<List<int>> _wonWeightings;
        static IEnumerable<Choosability.Graph> EnumerateWeightings(Choosability.Graph g)
        {
            if (g.MaxDegree > Delta)
                yield break;

            _wonWeightings = new List<List<int>>();
            foreach (var weighting in g.Vertices.Select(v => Enumerable.Range(g.Degree(v), Delta + 1 - g.Degree(v)).Reverse()).CartesianProduct())
            {
                var www = weighting.ToList();
                if (_wonWeightings.Any(ww => ww.Zip(www, (a, b) => a - b).Min() >= 0))
                    continue;

                var gg = g.Clone();
                gg.VertexWeight = www;
                yield return gg;
            }
        }
    }
}
