﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BitLevelGeneration
{
    public delegate bool IsPaintable(IGraph_long graph, long[] colorGraph, int c, int[] g);

    public static class GraphChoosability_uint
    {
        public static bool IsFChoosable(this IGraph_uint graph, Func<int, int> f, out List<List<int>> badAssignment)
        {
            long nodesVisited, cacheHits;
            return graph.IsFChoosable(f, out badAssignment, out nodesVisited, out cacheHits);
        }
        public static bool IsFChoosable(this IGraph_uint graph, Func<int, int> f, out List<List<int>> badAssignment, out long nodesVisited, out long cacheHits)
        {
            badAssignment = null;

            nodesVisited = 0;
            cacheHits = 0;
            var liveVertexBits = Enumerable.Range(0, graph.N).To_uint();
            var sizes = graph.Vertices.Select(v => f(v)).ToList();
            var maxListSize = sizes.Max();
            
            for (int potSize = maxListSize; potSize < graph.N; potSize++)
            {
                //var cache = new Dictionary<BitLevelGeneration.HashedAssignment_uint, bool>();
                Dictionary<BitLevelGeneration.HashedAssignment_uint, bool> cache = null;

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

        public static bool IsChoosable(this IGraph_uint graph, uint[] colorGraph)
        {
            long nodesVisited, cacheHits;
            return graph.IsChoosable(colorGraph, out nodesVisited, out cacheHits);
        }

        public static bool IsChoosable(this IGraph_uint graph, uint[] colorGraph, out long nodesVisited, out long cacheHits)
        {
            nodesVisited = 0;
            cacheHits = 0;
            //var cache = new Dictionary<BitLevelGeneration.HashedAssignment_uint, bool>();
            Dictionary<BitLevelGeneration.HashedAssignment_uint, bool> cache = null;

            return graph.IsChoosable(colorGraph, Enumerable.Range(0, graph.N).To_uint(), 0, cache, ref nodesVisited, ref cacheHits);
        }

        static bool IsChoosable(this IGraph_uint graph, uint[] colorGraph, uint liveVertexBits, int c, Dictionary<BitLevelGeneration.HashedAssignment_uint, bool> cache, ref long nodesVisited, ref long cacheHits)
        {
            nodesVisited++;

            graph.BeGreedy(colorGraph, ref liveVertexBits, c);
            if (liveVertexBits == 0)
                return true;
            if ((liveVertexBits & ~colorGraph.Or(c)) != 0)
                return false;

            //bool cachedResult;
            //var key = new BitLevelGeneration.HashedAssignment_uint(colorGraph, c, liveVertexBits);
            //if (cache.TryGetValue(key, out cachedResult))
            //{
            //    cacheHits++;
            //    return cachedResult;
            //}

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

            //cache[key] = choosable;
            return choosable;
        }

        static void BeGreedy(this IGraph_uint graph, uint[] colorGraph, ref uint liveVertexBits, int c)
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

    public static class GraphChoosability_long
    {
        public static int NodesVisited;
        public static IsPaintable IsPaintable;

        public static bool IsFGChoosable(this IGraph_long graph, Func<int, int> f, Func<int, int> g, out List<List<int>> badAssignment, Action<int> potSizeFinished)
        {
            NodesVisited = 0;

            badAssignment = null;
            
            var liveVertexBits = Enumerable.Range(0, graph.N).To_long();
            var sizes = graph.Vertices.Select(v => f(v)).ToList();
            var maxListSize = sizes.Max();
            var gsum = Enumerable.Range(0, graph.N).Sum(x => g(x));

            for (int potSize = maxListSize; potSize < gsum; potSize++)
            {
                foreach (var colorGraph in BitLevelGeneration.Assignments_long.Enumerate(sizes, potSize))
                {
                    if (potSize > maxListSize)
                    {
                        for (int i = 0; i < colorGraph.Length; i++)
                            for (int j = i + 1; j < colorGraph.Length; j++)
                                if ((colorGraph[i] & colorGraph[j]) == 0)
                                    goto skip;

                        var data = colorGraph.Select(subset => ComponentsInInducedSubgraph(graph, subset)).ToList();
                        for (int i = 0; i < data.Count; i++)
                        {
                            if (data[i].All(component => !colorGraph.All(c => (component & c) != 0)))
                                goto skip;
                        }
                    }

                    var gg = new int[graph.N];
                    for(int i = 0; i < graph.N; i++)
                        gg[i] = g(i);

                    if (!graph.IsGChoosable(colorGraph, 0, gg))
                    {
                        badAssignment = new List<List<int>>();
                        foreach (var v in graph.Vertices)
                        {
                            var list = new List<int>();
                            var bit = 1L << v;
                            for (int i = 0; i < colorGraph.Length; i++)
                            {
                                if ((bit & colorGraph[i]) != 0)
                                    list.Add(i);
                            }

                            badAssignment.Add(list);
                        }

                        return false;
                    }

                skip:;
                }

                if (potSizeFinished != null)
                    potSizeFinished(potSize);
            }

            return true;
        }

        static List<long> ComponentsInInducedSubgraph(this IGraph_long graph, long subset)
        {
            var equivalenceRelation = new EquivalenceRelation<int>();

            var S = BitUsage_long.ToSet(subset);
            foreach (var i in S)
                equivalenceRelation.AddElement(i);

            foreach (var i in S)
            {
                foreach (var j in S)
                {
                    if (i == j || graph.IsIndependent((((long)1) << i) | (((long)1) << j)))
                        continue;

                    equivalenceRelation.Relate(i, j);
                }
            }

            return equivalenceRelation.GetEquivalenceClasses().Select(bits => bits.To_long()).ToList();
        }

        public static bool IsGChoosable(this IGraph_long graph, long[] colorGraph, int[] g)
        {
            return graph.IsGChoosable(colorGraph, 0, g);
        }

        public static bool IsChoosable(this IGraph_long graph, long[] colorGraph, int[] g)
        {
            return graph.IsGChoosable(colorGraph, 0, g);
        }

        static bool IsGChoosable(this IGraph_long graph, long[] colorGraph, int c, int[] g)
        {
            NodesVisited++;

            graph.BeGreedyG(colorGraph, g, c);
            if (g.Sum() == 0)
                return true;
            if (c >= colorGraph.Length)
                return false;

            var choosable = false;
            var liveVertexBits = Enumerable.Range(0, graph.N).Where(i => g[i] > 0).To_long();
            var V = colorGraph[c] & liveVertexBits;

            foreach (var C in graph.MaximalIndependentSubsets(V))
            {
                var gp = new int[g.Length];
                for (int i = 0; i < g.Length; i++)
                    gp[i] = g[i];

                foreach (var i in BitUsage_long.ToSet(C))
                    gp[i]--;

                
                if (IsPaintable != null & IsPaintable(graph, colorGraph, c + 1, gp))
                    return true;
                if (graph.IsGChoosable(colorGraph, c + 1, gp))
                    return true;
            }

            return choosable;
        }

        static void BeGreedyG(this IGraph_long graph, long[] colorGraph, int[] g, int c)
        {
            while (true)
            {
                var sum = g.Sum();
                var liveVertexBits = Enumerable.Range(0, graph.N).Where(i => g[i] > 0).To_long();
                var bits = liveVertexBits;
                while (bits != 0)
                {
                    var bit = bits & -bits;
                    bits ^= bit;

                    var colorCount = 0;
                    for (int i = c; i < colorGraph.Length; i++)
                    {
                        if ((bit & colorGraph[i]) != 0)
                            colorCount++;
                    }

                    if (colorCount >= BitUsage_long.ToSet(graph.NeighborsInSet(bit.Extract(), liveVertexBits) | bit).Sum(x => g[x]))
                    {
                        g[bit.Extract()] = 0;
                        liveVertexBits ^= bit;
                    }
                }

                if (g.Sum() == sum)
                    break;
            }
        }

        public static bool IsFChoosable(this IGraph_long graph, Func<int, int> f, out List<List<int>> badAssignment)
        {
            badAssignment = null;

            var liveVertexBits = Enumerable.Range(0, graph.N).To_long();
            var sizes = graph.Vertices.Select(v => f(v)).ToList();
            var maxListSize = sizes.Max();

            for (int potSize = maxListSize; potSize < graph.N; potSize++)
            {
                foreach (var colorGraph in BitLevelGeneration.Assignments_long.Enumerate(sizes, potSize))
                {
                    if (potSize > maxListSize)
                    {
                        var xx = 0L;
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


                    if (!graph.IsChoosable(colorGraph, liveVertexBits, 0))
                    {
                        badAssignment = new List<List<int>>();
                        foreach (var v in graph.Vertices)
                        {
                            var list = new List<int>();
                            var bit = 1L << v;
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

        public static bool IsChoosable(this IGraph_long graph, long[] colorGraph)
        {
            return graph.IsChoosable(colorGraph, Enumerable.Range(0, graph.N).To_long(), 0);
        }

        public static bool IsChoosable(this IGraph_long graph, long[] colorGraph, long subset)
        {
            return graph.IsChoosable(colorGraph, subset, 0);
        }

        static bool IsChoosable(this IGraph_long graph, long[] colorGraph, long liveVertexBits, int c)
        {
            graph.BeGreedy(colorGraph, ref liveVertexBits, c);
            if (liveVertexBits == 0)
                return true;
            if ((liveVertexBits & ~colorGraph.Or(c)) != 0)
                return false;

            var choosable = false;
            var V = colorGraph[c] & liveVertexBits;
            foreach (var C in graph.MaximalIndependentSubsets(V))
            {
                if (graph.IsChoosable(colorGraph, liveVertexBits ^ C, c + 1))
                {
                    choosable = true;
                    break;
                }
            }

            return choosable;
        }

        static void BeGreedy(this IGraph_long graph, long[] colorGraph, ref long liveVertexBits, int c)
        {
            while (true)
            {
                var originalBits = liveVertexBits;
                var bits = liveVertexBits;
                while (bits != 0)
                {
                    var bit = bits & -bits;
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

        public static bool IsSubsetTwoColorable(IGraph_long g, long set)
        {
            var c = new int[g.N];
            var q = new int[g.N];

            var leftover = set;
            var s = 0;
            while (leftover != 0)
            {
                var r = leftover.LeastSignificantBit();
                q[s] = r;
                c[r] = 1;
                var e = s;

                while (s <= e)
                {
                    var v = q[s++];
                    var n = g.NeighborsInSet(v, set);

                    while (n != 0)
                    {
                        var bit = n & -n;
                        var w = bit.Extract();

                        if (c[w] == 0)
                        {
                            q[++e] = w;
                            c[w] = 3 - c[v];
                        }
                        else if (c[w] != 3 - c[v])
                        {
                            return false;
                        }

                        n ^= bit;
                    }

                    leftover ^= (1L << v);
                }
            }

            return true;
        }
    }
}
