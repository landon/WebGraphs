using Choosability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
