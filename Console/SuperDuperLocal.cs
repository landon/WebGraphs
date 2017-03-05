using Choosability;
using Choosability.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class SuperDuperLocal
    {
        const int R = 3;
        const int MinN = 2;
        const int MaxN = 10;
        const string GraphFileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";
        static readonly string OutputFile = "superduperlocal.txt";

        public static void Go()
        {
            var gn = new GraphEnumerator(OutputFile, MinN, MaxN, false);
            gn.FileRoot = GraphFileRoot;
            foreach (var G in gn.EnumerateGraph6File())
            {
                  var Q = G.LineGraph.Value;
               // var Q = G;
                var gi = GammaInfinity(Q, R);
                var gim = GammaInfinityMonotone(Q, R);
                if (gim - gi > 0)
                {
                    System.Console.WriteLine(string.Format("{0} - {1} = {2}, {3}", gim, gi, gim - gi, G.ToGraph6()));
                    System.Console.ReadKey();
                }
            }
        }

        static int GammaInfinityMonotone(Graph G, int r)
        {
            return Math.Max(GammaInfinity(G, r), G.Vertices.Max(v => GammaInfinity(G.RemoveVertex(v), r)));
        }

        static int GammaInfinity(Graph G, int r)
        {
            var RCliques = G.N >= r ? ListUtility.EnumerateSublists(G.Vertices, r).Where(X => G.IsClique(X)) : Enumerable.Empty<List<int>>();
            return ReedAverage(G, G.EnumerateMaximalCliques().Concat(RCliques));
        }

        static int GammaInfinityMonotone(Graph G)
        {
            return Math.Max(GammaInfinity(G), G.Vertices.Max(v => GammaInfinity(G.RemoveVertex(v))));
        }

        static int GammaInfinity(Graph G)
        {
            return ReedAverage(G, G.EnumerateMaximalCliques());
        }

        static int ReedAverage(Graph G, IEnumerable<List<int>> subsets)
        {
            return subsets.Max(X => X.Sum(v => (G.Degree(v) + G.Omega(v)) / 2));
        }
    }
}
