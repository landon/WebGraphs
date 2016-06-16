using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability;
using Choosability.Utility;
using System.IO;

namespace Console
{
    public static class FindKP
    {
        const int MinVertices = 5;
        const int MaxVertices = 7;
        const int MaxDegree = int.MaxValue;

        static Dictionary<string, int> BestKP = new Dictionary<string, int>();
        static Dictionary<string, int> Ng6 = new Dictionary<string, int>();
        static Dictionary<string, int> Eg6 = new Dictionary<string, int>();
        static Dictionary<string, int> Micg6 = new Dictionary<string, int>();

        static readonly string WinnersFile = MinVertices + " -- " + MaxVertices + " " + (MaxDegree < int.MaxValue ? "max degree " + MaxDegree + "_" : "") + "winners";
        public static void Go()
        {
            using (var swbest = new StreamWriter(WinnersFile + ".best.txt"))
            using (var swg = new StreamWriter(WinnersFile + ".txt"))
            using (var swb = new StreamWriter(WinnersFile + ".bad" + ".txt"))
            {
                swbest.AutoFlush = true;
                swg.AutoFlush = true;
                swb.AutoFlush = true;

                using (var graphIO = new GraphEnumerator(WinnersFile + ".blah", MinVertices, MaxVertices))
                {
                  //   graphIO.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";
                    graphIO.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\VertexCritical\chi";

                    var lastg6 = "";
                    foreach (var g in graphIO.EnumerateGraph6File(null, EnumerateEdgeSubsets))
                    {
                        if (MaxDegree < int.MaxValue && g.MaxDegree > MaxDegree)
                            continue;

                        var g6 = g.ToGraph6();

                        if (g6 != lastg6)
                        {
                            if (!string.IsNullOrEmpty(lastg6))
                            {
                                var gain = BestKP[lastg6] - Ng6[lastg6];
                                var sgain = BestKP[lastg6] - Micg6[lastg6];
                                swbest.WriteLine(lastg6 + " " + BestKP[lastg6] + " N: " + Ng6[lastg6] + " E: " + Eg6[lastg6] + " gain: " + gain + " micgain: " + sgain); 
                            }

                            System.Console.WriteLine("checking " + g6);
                            BestKP[g6] = 0;
                            Ng6[g6] = g.N;
                            Eg6[g6] = g.E;
                            Micg6[g6] = g.Mic();
                            lastg6 = g6;
                        }

                        //System.Console.ForegroundColor = ConsoleColor.DarkGray;
                        //System.Console.Write(g.EdgeWeightsWithMultiplicity.ToSetString() + "... ");
                        //System.Console.ForegroundColor = ConsoleColor.White;

                        var symmetricEdges = g.EdgeWeightsWithMultiplicity.IndicesWhere(w => w == 2).ToList();
                        List<int> badSubgraph;
                        var badOrientation = g.CheckKernelPerfectForAllOrientations(symmetricEdges, out badSubgraph);

                        if (badOrientation != null)
                        {
                            //var symmetric = new bool[g.N * (g.N - 1) / 2];
                            //var w = new List<int>();
                            //int k = 0;
                            //for (int i = 0; i < g.N; i++)
                            //{
                            //    for (int j = i + 1; j < g.N; j++)
                            //    {
                            //        if (g[i, j])
                            //        {
                            //            symmetric[k] = badOrientation[i].Contains(j) && badOrientation[j].Contains(i);
                            //            if (badOrientation[j].Contains(i))
                            //                w.Add(-1);
                            //            else
                            //                w.Add(1);
                            //        }
                            //        else
                            //        {
                            //            w.Add(0);
                            //        }

                            //        k++;
                            //    }
                            //}

                            //var gg = GraphsCore.GraphIO.GraphFromEdgeWeightString(string.Join(" ", w));
                            //for (int i = 0; i < g.E; i++)
                            //{
                            //    if (symmetric[i])
                            //        gg.Edges[i].Multiplicity = 2;
                            //}

                            //var webgraph = GraphsCore.CompactSerializer.Serialize(gg);

                            //System.Console.WriteLine();
                            //swb.WriteLine(webgraph);
                        }
                        else
                        {
                            _winnerSets.Add(symmetricEdges);
                            //var gg = GraphsCore.GraphIO.GraphFromGraph6(g.ToGraph6());
                            //foreach (var i in symmetricEdges)
                            //    gg.Edges[i].Multiplicity = 2;

                            //var webgraph = GraphsCore.CompactSerializer.Serialize(gg);

                            //System.Console.ForegroundColor = ConsoleColor.Green;
                            //System.Console.WriteLine("good");
                            //System.Console.ForegroundColor = ConsoleColor.White;

                            //swg.WriteLine(webgraph);

                            BestKP[g6] = Math.Max(BestKP[g6], g.E - symmetricEdges.Count);
                        }
                    }
                }
            }
        }

        static List<List<int>> _winnerSets = new List<List<int>>();
        static IEnumerable<Graph> EnumerateEdgeSubsets(Graph g)
        {
            _winnerSets.Clear();

            foreach (var F in Enumerable.Range(0, g.E)
                             .ToList()
                             .EnumerateSublists())
            {

                if (_winnerSets.Any(ws => ws.SubsetEqual(F)))
                    continue;

                var gc = g.Clone();
                gc.EdgeWeightsWithMultiplicity = Enumerable.Range(0, g.E).Select(e => F.Contains(e) ? 2 : 1).ToList();
                yield return gc;
            }

        }
    }
}
