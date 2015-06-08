using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability.Utility;
using Choosability.FixerBreaker.KnowledgeEngine;
using System.IO;

namespace Console
{
    public static class FindFixerBreaker
    {
        static int Delta = 6;
        static int MaxVertices = 20;
        static bool TreesOnly = false;
        static bool TriangleFree = false;
        static bool TreesOrTreesPlusEdgeOnly = false;
        static bool Planar = true;
        static bool LowGirth = false;
        static bool WeaklyFixable = false;
        const bool NearColorings = true;

        static bool MakeWebPage = true;
        static string WebpageRoot = @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Automated";

        static readonly string WinnersFile = (WeaklyFixable ? "weakly " : "") + (LowGirth ? "low girth induced " : "") + (Planar ? "planar " : "") + (TreesOrTreesPlusEdgeOnly ? "trees or trees plus edge only " : "") + (TriangleFree ? "triangle-free " : "") + (TreesOnly ? "trees only " : "") + (NearColorings ? "near colorings " : "") + "FixerBreaker winners Delta=" + Delta + ".txt";

        public static void Go()
        {
            using (var graphEnumerator = new GraphEnumerator(WinnersFile, 2, MaxVertices))
            {
                if (TreesOnly)
                    graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\trees\trees";
                else if (TreesOrTreesPlusEdgeOnly)
                    graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\degree5treesplusedge\geng";
                else if (Planar)
                    graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\planar\planar_conn.";
                else
                    graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";

                foreach (var g in graphEnumerator.EnumerateGraph6File(Filter, EnumerateWeightings))
                {
                    System.Console.Write("checking " + g.ToGraph6() + " with degrees [" + string.Join(",", g.VertexWeight) + "] ...");

                    var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g, false, WeaklyFixable);
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

                        if (MakeWebPage)
                        {
                            MakePictures.MakeWebpage(WinnersFile, Path.Combine(WebpageRoot, Path.GetFileNameWithoutExtension(WinnersFile)));
                        }
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
        static Graphs.Graph Bad = GraphsCore.CompactSerializer.Deserialize(@"webgraph:7pc5r!-8,?!!!RbO9]_@`!KrS!Aa^X)b'O@(T\'7#K=Df)3b_!#;Q9E+%,fY!!GP:JVXt.p]gd!!W`;B!!!!''?C1S'Z^@ja9<6u!X'1X!sAW)a8c2?!.Y%");
        static Choosability.Graph BadG = new Choosability.Graph(Bad.GetEdgeWeights(), Bad.Vertices.Select(v => int.Parse(v.Label)).ToList());
        static bool Filter(Choosability.Graph g)
        {
            if (TriangleFree)
                return g.Vertices.All(v => g.IsIndependent(g.Neighbors[v]));

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

                if (LowGirth)
                {
                    if (!gg.Contains(BadG, true, GraphEnumerator.WeightConditionEqual))
                        continue;
                }

                yield return gg;
            }
        }
    }
}
