﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BitLevelGeneration
{
    public static class GraphChoosability
    {
        public static bool IsFChoosable(this IGraph graph, Func<int, int> f, out List<List<int>> badAssignment)
        {
            long nodesVisited, cacheHits;
            return graph.IsFChoosable(f, out badAssignment, out nodesVisited, out cacheHits);
        }
        public static bool IsFChoosable(this IGraph graph, Func<int, int> f, out List<List<int>> badAssignment, out long nodesVisited, out long cacheHits)
        {
            badAssignment = null;

            nodesVisited = 0;
            cacheHits = 0;
            var liveVertexBits = Enumerable.Range(0, graph.N).To_uint();
            var sizes = graph.Vertices.Select(v => f(v)).ToList();
            var maxListSize = sizes.Max();
            
            for (int potSize = maxListSize; potSize < graph.N; potSize++)
            {
                var cache = new Dictionary<BitLevelGeneration.HashedAssignment_uint, bool>();

                foreach (var colorGraph in BitLevelGeneration.Assignments_uint.Enumerate(sizes, potSize))
                {
                    if (potSize > maxListSize)
                    {
                        uint xx = 0;
                        for (int i = 0; i < colorGraph.Length; i++)
                        {
                            xx |= colorGraph[i];

                            if (xx.PopulationCount() <= i + 1)
                                goto skip;
                        }

                        for (int i = 0; i < colorGraph.Length; i++)
                            for (int j = i + 1; j < colorGraph.Length; j++)
                                if ((colorGraph[i] & colorGraph[j]) == 0)
                                    goto skip;

                        foreach (var color in colorGraph)
                            if (graph.IsIndependent(color))
                                goto skip;
                    }

                    
                    if (!graph.IsChoosable(colorGraph, liveVertexBits, 0, cache, ref nodesVisited, ref cacheHits))
                    {
                        badAssignment = new List<List<int>>();
                        foreach (var v in graph.Vertices)
                        {
                            var list = new List<int>();
                            uint bit = 1U << v;
                            for (int i = 0; i < colorGraph.Length; i++)
                            {
                                if ((bit & colorGraph[i]) != 0)
                                    list.Add(i);
                            }

                            badAssignment.Add(list);
                        }

                        return false;
                    }

                skip: ;
                }
            }

            return true;
        }

        public static bool IsChoosable(this IGraph graph, uint[] colorGraph)
        {
            long nodesVisited, cacheHits;
            return graph.IsChoosable(colorGraph, out nodesVisited, out cacheHits);
        }

        public static bool IsChoosable(this IGraph graph, uint[] colorGraph, out long nodesVisited, out long cacheHits)
        {
            nodesVisited = 0;
            cacheHits = 0;
            var cache = new Dictionary<BitLevelGeneration.HashedAssignment_uint, bool>();

            return graph.IsChoosable(colorGraph, Enumerable.Repeat(1, graph.N).To_uint(), 0, cache, ref nodesVisited, ref cacheHits);
        }

        static bool IsChoosable(this IGraph graph, uint[] colorGraph, uint liveVertexBits, int c, Dictionary<BitLevelGeneration.HashedAssignment_uint, bool> cache, ref long nodesVisited, ref long cacheHits)
        {
            nodesVisited++;

            graph.BeGreedy(colorGraph, ref liveVertexBits, c);
            if (liveVertexBits == 0)
                return true;
            if ((liveVertexBits & ~colorGraph.Or(c)) != 0)
                return false;

            bool cachedResult;
            var key = new BitLevelGeneration.HashedAssignment_uint(colorGraph, c, liveVertexBits);
            if (cache.TryGetValue(key, out cachedResult))
            {
                cacheHits++;
                return cachedResult;
            }

            var choosable = false;
            var V = colorGraph[c] & liveVertexBits;
            foreach (var C in graph.MaximalIndependentSubsets(V))
            {
                if (graph.IsChoosable(colorGraph, liveVertexBits ^ C, c + 1, cache, ref nodesVisited, ref cacheHits))
                {
                    choosable = true;
                    break;
                }
            }

            cache[key] = choosable;
            return choosable;
        }

        static void BeGreedy(this IGraph graph, uint[] colorGraph, ref uint liveVertexBits, int c)
        {
            while(true)
            {
                var originalBits = liveVertexBits;
                var bits = liveVertexBits;
                while (bits != 0)
                {
                    var bit = bits & (~bits + 1);
                    bits ^= bit;

                    var colorCount = 0;
                    for (int i = c; i < colorGraph.Length; i++)
                    {
                        if ((bit & colorGraph[i]) != 0)
                            colorCount++;
                    }

                    if (colorCount > graph.DegreeInSet(bit.Extract(), liveVertexBits))
                        liveVertexBits ^= bit;
                }

                if (liveVertexBits == originalBits)
                    break;
            }
        }
    }
}