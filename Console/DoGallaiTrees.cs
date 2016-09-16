using LinearFractionalProgram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability;

namespace Console
{
    public static class DoGallaiTrees
    {
        const int MinVertices = 1;
        const int MaxVertices = 10;
        const int K = 7;
        const int MaxBlocks = 5;
        const int MaxOddCycle = 3;

        public static void Go()
        {
            var gtg = new GallaiTreeGenerator(K);
            var gallaiTrees = new List<Choosability.Graph> { Choosability.Graphs.K(1) };

            for (int n = MinVertices; n <= MaxVertices; n++)
            {
                foreach (var T in string.Format(@"C:\Users\landon\Google Drive\research\Graph6\trees\trees{0}.g6", n).EnumerateGraph6File())
                {
                    var cuts = FindBlockCuts(T);
                    if (cuts == null)
                        continue;
                    if (cuts.Count <= 0)
                    {
                        gallaiTrees.AddRange(gtg.EnumerateAll(1, MaxOddCycle));
                    }
                    else
                    {
                        gallaiTrees.AddRange(gtg.EnumerateAllForBlockTree(T, cuts, MaxOddCycle));
                    }
                }
            }

            System.Console.WriteLine("computing invariants...");
            var data = gallaiTrees.Select(g => new GraphInvariantPile()
            {
                G = g,
                N = g.N,
                E = g.E,
                Beta = g.IndependenceNumberBronKerbosch(g.VerticesOfDegree(K - 1)),
                Q = g.Vertices.Count(v => g.Degree(v) == K - 2 && g.IsClique(g.Neighbors[v])),
            }).ToList();

            System.Console.WriteLine("removing duplicates...");
            data = data.DistinctBy(x => x.Key).ToList();

            foreach (var d in data)
                d.ComputeRemainingInvariants();

            System.Console.WriteLine("generating glpk...");
            var glpk = MakeCodes(data);

            System.Console.WriteLine("writing file...");
            using (var sw = new StreamWriter("code.txt"))
                sw.Write(glpk);
        }

        static List<int> FindBlockCuts(Graph T)
        {
            if (T.N <= 1)
                return new List<int>();

            var blocks = new HashSet<int>(T.VerticesOfDegree(1));
            var cuts = new HashSet<int>();

            while (true)
            {
                var bc = blocks.Count;
                var cc = cuts.Count;

                foreach (var b in blocks)
                {
                    foreach (var v in T.Neighbors[b])
                        cuts.Add(v);
                }
                foreach (var c in cuts)
                {
                    foreach (var v in T.Neighbors[c])
                        blocks.Add(v);
                }

                if (bc == blocks.Count && cc == cuts.Count)
                    break;
            }

            if (cuts.Count == T.N)
                return null;

            return cuts.ToList();
        }

        static void GoSlow()
        {
            var gtg = new GallaiTreeGenerator(K);

            var gallaiTrees = new List<Choosability.Graph> { Choosability.Graphs.K(1) };
            for (int blocks = 1; blocks <= MaxBlocks; blocks++)
            {
                System.Console.Write("generating " + blocks + " block Gallai trees for K=" + K + "...");
                var all = gtg.EnumerateAll(blocks, MaxOddCycle);
                gallaiTrees.AddRange(all);
                System.Console.WriteLine(" " + gallaiTrees.Count);
            }

            System.Console.WriteLine("computing invariants...");
            var data = gallaiTrees.Select(g => new GraphInvariantPile()
            {
                G = g,
                N = g.N,
                E = g.E,
                Beta = g.IndependenceNumberBronKerbosch(g.VerticesOfDegree(K - 1)),
                Q = g.Vertices.Count(v => g.Degree(v) == K - 2 && g.IsClique(g.Neighbors[v])),
            }).ToList();

            System.Console.WriteLine("removing duplicates...");
            data = data.DistinctBy(x => x.Key).ToList();

            foreach (var d in data)
                d.ComputeRemainingInvariants();

            System.Console.WriteLine("generating glpk...");
            var glpk = MakeCodes(data);

            System.Console.WriteLine("writing file...");
            using (var sw = new StreamWriter("code.txt"))
                sw.Write(glpk);
        }

        static string MakeCodes(List<GraphInvariantPile> data)
        {
            var Ac = new List<double>();
            foreach (var dd in data)
            {
                Ac.Add(-dd.N);
                if (K > 4)
                    Ac.Add(-dd.Q);
                else
                    Ac.Add(0);
                Ac.Add(-dd.Beta);

                if (dd.HasKMinusOneClique)
                    Ac.Add(-1);
                else
                    Ac.Add(0);
            }

            int hCoefficient;
            if (K == 4)
            {
                hCoefficient = 0;
                Ac.AddRange(new double[] { 0, 0, 0, 1 });
            }
            else if (K <= 6)
            {
                hCoefficient = 4;
                Ac.AddRange(new double[] { 0, 1, 0, 1 });
            }
            else
            {
                hCoefficient = 3;
                Ac.AddRange(new double[] { 0, 2, 0, 1 });
            }

            Ac.AddRange(new double[] { 0, 0, -1, 0 });

            var bc = new List<double>();
            foreach (var dd in data)
            {
                bc.Add(-(2 * dd.E - (K - 3) * dd.N));
            }

            bc.Add(0);
            bc.Add(-2);

            var A = new Matrix(4, Ac.ToArray());
            var b = new Matrix(1, bc.ToArray());
            var c = new Matrix(4, -1, 0, -1.0 / (K - 1), 0);
            var d = new Matrix(4, -1, hCoefficient, -(K - 2) / (2.0 * (K - 1)), 0);

            var lfp = new LFP(A, b, c, d, 2, K + 1);
            return lfp.GLPK;
        }

        static void GenerateWebpage()
        {
            var gtg = new GallaiTreeGenerator(K);

            var gallaiTrees = new List<Choosability.Graph>();
            for (int blocks = 1; blocks <= MaxBlocks; blocks++)
            {
                System.Console.WriteLine("generating " + blocks + " block Gallai trees for K=" + K + "...");
                var all = gtg.EnumerateAll(blocks, MaxOddCycle).ToList();
                System.Console.WriteLine("removing isomorphs...");
                var distinct = all.RemoveSelfIsomorphs();
                gallaiTrees.AddRange(distinct);
            }

            System.Console.WriteLine("removing isomorphs...");
            gallaiTrees = gallaiTrees.RemoveSelfIsomorphs();
            System.Console.WriteLine("generating vector graphics...");
            gallaiTrees.ToWebPageSimple("gallai\\" + K + "\\" + MaxBlocks + "\\" + MaxOddCycle + "\\", K);
        }

        public class GraphInvariantPile
        {
            public Choosability.Graph G;
            public int N;
            public int E;
            public int Beta;
            public int Q;
            public bool HasKMinusOneClique;

            public void ComputeRemainingInvariants()
            {
                HasKMinusOneClique = G.CliqueNumberBronKerbosch() >= K - 1;
            }

            public override string ToString()
            {
                return string.Format("{0}, N = {1}, E = {2}, Beta = {3}, Q = {4}", G.ToGraph6(), N, E, Beta, Q);
            }

            public string Key
            {
                get { return string.Format("N = {0}, E = {1}, Beta = {2}, Q = {3}", N, E, Beta, Q); }
            }
        }
    }
}
