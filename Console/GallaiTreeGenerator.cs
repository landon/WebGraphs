using Choosability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability.Utility;

namespace Console
{
    public class GallaiTreeGenerator
    {
        static Random _rng = new Random(DateTime.Now.Millisecond);
        public int K { get; private set; }
        
        public GallaiTreeGenerator(int k)
        {
            K = k;
        }

        public IEnumerable<Graph> EnumerateAll(int blocks, int maxOddCycle = 3)
        {
            if (blocks <= 0)
                yield break;
            else if (blocks == 1)
            {
                for (int d = 1; d <= K - 2; d++)
                {
                    yield return Extend(Choosability.Graphs.K(1), 0, d, d + 1);

                    if (d == 2)
                    {
                        for (int l = 5; l <= maxOddCycle; l += 2)
                            yield return Extend(Choosability.Graphs.K(1), 0, 2, l);
                    }
                }
            }
            else
            {
                foreach(var g in EnumerateAll(blocks - 1, maxOddCycle))
                {
                    foreach(var v in EnumerateExtendableVertices(g))
                    {
                        for(int d = 1; d < K - Math.Max(1, g.Degree(v)); d++)
                        {
                            yield return Extend(g, v, d, d + 1);

                            if (d == 2)
                            {
                                for (int l = 5; l <= maxOddCycle; l += 2)
                                    yield return Extend(g, v, 2, l);
                            }
                        }
                    }
                }
            }
        }

        class CutData
        {
            public int W;
            public int N;
        }

        public IEnumerable<Graph> EnumerateAllForBlockTree(Graph T, List<int> cutIndices, int maxOddCycle)
        {
            var blocks = T.Vertices.Except(cutIndices).ToList();
            var cut = new Dictionary<int, CutData>();
            foreach (var c in cutIndices)
                cut[c] = new CutData() { W = K - 1, N = T.Degree(c) };

            foreach (var w in EnumerateAllBlockTreeWeightings(T, blocks, maxOddCycle, new int[blocks.Count], cut, 0))
            {
                foreach (var g in EnumerateAllForWeightedBlockTree(T, w, blocks, cutIndices, maxOddCycle))
                {
                    g.VertexWeight = null;
                    yield return g;
                }
            }
        }

        IEnumerable<List<int>> EnumerateAllBlockTreeWeightings(Graph T, List<int> blocks, int maxOddCycle, int[] blockWeights, Dictionary<int, CutData> cut, int i)
        {
            if (i >= blocks.Count)
            {
                yield return blockWeights.ToList();
            }
            else
            {
                var v = blocks[i];
                var nn = T.Neighbors[v];
                var max = nn.Min(w =>
                    {
                        var c = cut[w];
                        return c.W - (c.N - 1);
                    });

                for (int q = 1; q <= max; q++)
                {
                    if (q != 2 && q + 1 < nn.Count || q == 2 && maxOddCycle < nn.Count)
                        continue;

                    blockWeights[i] = q;
                    foreach (var w in nn)
                    {
                        var c = cut[w];
                        c.N--;
                        c.W -= q;
                    }

                    foreach (var bw in EnumerateAllBlockTreeWeightings(T, blocks, maxOddCycle, blockWeights, cut, i + 1))
                        yield return bw;

                    foreach (var w in nn)
                    {
                        var c = cut[w];
                        c.N++;
                        c.W += q;
                    }
                }
            }
        }

        IEnumerable<Graph> EnumerateAllForWeightedBlockTree(Graph T, List<int> w, List<int> blocks, List<int> cutIndices, int maxOddCycle)
        {
            foreach (var tuple in blocks.Select((b, j) =>
                {
                    var bb = new List<Graph>();
                    var blockDegree = w[j];
                    if (blockDegree == 2)
                    {
                        for (int x = 2 * (T.Degree(b) / 2) + 1; x <= maxOddCycle; x += 2)
                            bb.Add(Choosability.Graphs.C(x));
                    }
                    else
                    {
                        bb.Add(Choosability.Graphs.K(blockDegree + 1));
                    }

                    return bb;
                }).CartesianProduct())
            {
                yield return BuildGallaiTree(T, tuple.ToList(), blocks, cutIndices);
            }
        }

        Graph BuildGallaiTree(Graph T, List<Graph> list, List<int> blocks, List<int> cutIndices)
        {
            foreach(var l in list)
                l.VertexWeight = new int[l.N].ToList();

            var j = 1;
            foreach (var p in cutIndices)
            {
                var bb = T.Neighbors[p].Select(b => blocks.IndexOf(b)).ToList();
                foreach (var b in bb)
                {
                    for (int q = 0; q < list[b].N; q++)
                    {
                        if (list[b].VertexWeight[q] == 0)
                        {
                            list[b].VertexWeight[q] = j;
                            break;
                        }
                    }
                }

                j++;
            }

            var uber = Graph.DisjointUnion(list);
            return uber.IdentifyLikeVertexWeights();
        }

        public Graph GenerateRandomBlock(int maxOddCycle = 3)
        {
            return GenerateRandomExtension(Choosability.Graphs.K(1), maxOddCycle);
        }

        public Graph GenerateRandomExtension(Graph g, int maxOddCycle = 3)
        {
            var vs = EnumerateExtendableVertices(g).ToList();
            var v = vs[_rng.Next(vs.Count)];

            var blockDegree = _rng.Next(1, K  - Math.Max(1, g.Degree(v)));
            var blockOrder = blockDegree + 1;
            if (blockDegree == 2)
                blockOrder = 3 + 2 * _rng.Next(0, (maxOddCycle - 3) / 2 + 1);

            return Extend(g, v, blockDegree, blockOrder);
        }

        IEnumerable<int> EnumerateExtendableVertices(Graph g)
        {
            return g.Vertices.Where(v => g.Degree(v) < K);
        }

        public static Graph Extend(Graph g, int v, int blockDegree, int blockOrder)
        {
            Graph h;
            if (blockDegree == 2)
                h = Choosability.Graphs.C(blockOrder);
            else
                h = Choosability.Graphs.K(blockOrder);

            return g.Identify(v, h, 0);
        }
    }
}
