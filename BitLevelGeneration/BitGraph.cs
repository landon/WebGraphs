using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitLevelGeneration
{
    public class BitGraph : IGraph
    {
        int[] _vertices;
        uint[] _neighborhood;

        public BitGraph(List<int> edgeWeights)
        {
            var n = (int)((1 + Math.Sqrt(1 + 8 * edgeWeights.Count)) / 2);
            _vertices = Enumerable.Range(0, n).ToArray();

            _neighborhood = new uint[n];

            int k = 0;
            for (int i = 0; i < n; i++)
            {
                var iBit = 1U << i;
                for (int j = i + 1; j < n; j++)
                {
                    var jBit = 1U << j;

                    if (edgeWeights[k] != 0)
                    {
                        _neighborhood[i] |= jBit;
                        _neighborhood[j] |= iBit;
                    }

                    k++;
                }
            }
        }

        public int N { get { return _vertices.Length; } }
        public IEnumerable<int> Vertices { get { return _vertices; } }

        public bool IsIndependent(uint set)
        {
            return set.TrueForAllBitIndices(i => (_neighborhood[i] & set) == 0);
        }
        public int Degree(int v)
        {
            return _neighborhood[v].PopulationCount();
        }
        public int DegreeInSet(int v, uint set)
        {
            return (_neighborhood[v] & set).PopulationCount();
        }

        public IEnumerable<uint> MaximalIndependentSubsets(uint set)
        {
            var heads = new List<uint>();
            
            uint subset = 0;
            do
            {
                subset = (subset - set) & set;
                if (!IsIndependent(subset))
                    continue;

                var removed = heads.RemoveAll(h => (h | subset) == subset);
                if (removed > 0 || heads.All(h => (h | subset) != h))
                    heads.Add(subset);

            } while (subset != set);

            return heads;
        }
    }
}
