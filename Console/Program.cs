using BitLevelGeneration;
using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static int Attempt = 1;
        static int EdgesSeen = -1;
        static Choosability.Utility.Permutation CurrentP = null;

        static Dictionary<Choosability.OnlineChoiceHashGraph, bool> cache = new Dictionary<Choosability.OnlineChoiceHashGraph, bool>();
        static void Main(string[] args)
        {
            //var smalls = new[] { Choosability.Graphs.K(1) }
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph2.g6".EnumerateGraph6File())
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph3.g6".EnumerateGraph6File())
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph4.g6".EnumerateGraph6File())
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph5.g6".EnumerateGraph6File())
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph6.g6".EnumerateGraph6File())
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph7.g6".EnumerateGraph6File())
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph8.g6".EnumerateGraph6File())
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph9.g6".EnumerateGraph6File())
            //        .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph10.g6".EnumerateGraph6File());

            //int threshold = 3;
            //foreach(var g in smalls)
            //{
            //    if (g.Vertices.All(v => g.Degree(v) - g.Omega(v) >= threshold))
            //    {
            //        System.Console.WriteLine(g.ToGraph6());
            //    }
            //}

           // GraphChoosability_long.IsPaintable = IsPaintable;
           //  var g = GraphsCore.GraphIO.GraphFromGraph6("GliC?K");  // done through pot 8
          // var g = GraphsCore.GraphIO.GraphFromGraph6("I]G_W_O?W");
            //var g = GraphsCore.GraphIO.GraphFromGraph6("GhEKOK");   // done through pot 8
            // var g = GraphsCore.GraphIO.GraphFromGraph6("GlgO_K");  // done through pot 8
            //var gg = new BitGraph_long(g.GetEdgeWeights());

            // List<List<int>> bad;
            // var choosable = gg.IsFGChoosable((x) => 4, (x) => 2, out bad, x => System.Console.WriteLine("finished " + x + " in " + GraphChoosability_long.NodesVisited + " nodes "));



            // WeaklyFixableTester.Go();
            //  FixerBreakerTrees.Go();
            //   SuperAbundanceFinder.Go();
            //  FindFixerBreaker.Go();
            //MakePictures.Go();

            // MixedChoosables.Go();
            //EliminiteDoubleEdgeSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\Mixed spread 2 AT winners1.txt");

            //  FindTarpits.Go();
            // FindFixerBreaker.Go();
            FindChoosables.Go();

            //   EliminateDoubleEdgeNotSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\not 10 vertex Mixed spread 2 max high 1 kappa2 winners1.txt");

            //ConAndNon.Go();
            System.Console.WriteLine();
            System.Console.WriteLine("done.");
            System.Console.ReadKey();
        }



        static bool IsPaintable(IGraph_long graph, long[] colorGraph, int c, int[] g)
        {
            var liveVertices = Enumerable.Range(0, graph.N).Where(i => g[i] > 0).ToList();
            
            var f = new int[g.Length];
            for(int i = 0; i < f.Length; i++)
            {
                var iBit = 1L << i;

                f[i] = colorGraph.Skip(c).Count(cg => (cg & iBit) != 0);
            }

            var ff = new List<int>();
            var gg = new List<int>();
            foreach(var v in liveVertices)
            {
                ff.Add(f[v]);
                gg.Add(g[v]);
            }

            var G = new Choosability.Graph(graph.GetEdgeWeights()).InducedSubgraph(liveVertices);

            return G.IsOnlineFGChoosableListerRestricted(x => ff[x], x => gg[x], Compare, cache);
        }

        static int Compare(Choosability.Graph g1, Choosability.Graph g2)
        {
            return g1.E.CompareTo(g2.E);
        }
    }
}
