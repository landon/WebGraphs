using Choosability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class IsomorphRemover
    {
        public static List<Graph> RemoveSelfIsomorphs(this IEnumerable<Graph> graphs, bool zeroToZero = false, Func<Graph, Graph, int, int, int> priority = null, bool allowUnequal = false)
        {
            if (priority == null)
            {
                allowUnequal = false;
                priority = PriorityEqual;
            }

            var all = graphs.ToList();
            var some = new List<Graph>(all.Count);

            for (int i = 0; i < all.Count; i++)
            {
                var g = all[i];
                var good = true;

                foreach (var h in some)
                {
                    if (g.N != h.N)
                        continue;
                    if (g.E != h.E)
                        continue;

                    if (!allowUnequal)
                    {
                        if (!g.VertexWeight.OrderBy(x => x).SequenceEqual(h.VertexWeight.OrderBy(x => x)))
                            continue;
                    }

                    bool contains;
                    if (zeroToZero)
                    {
                        var tau = new int[g.N];
                        tau[0] = 0;
                        contains = g.ContainsPrioritized(h, true, priority, tau, new List<int>() { 0 }, 1);
                    }
                    else
                    {
                        contains = g.ContainsPrioritized(h, true, priority);
                    }

                    if (contains)
                    {
                        good = false;
                        break;
                    }
                }

                if (good)
                    some.Add(g);
            }

            return some;
        }

        public static IEnumerable<Graph> EnumerateRemoveIsomorphs(this IEnumerable<Graph> graphs, IEnumerable<Graph> excluded, bool induced = true, Func<Graph, Graph, int, int, bool> weightCondition = null)
        {
            if (weightCondition == null)
                weightCondition = WeightConditionDown;

            foreach (var g in graphs)
            {
                var qq = excluded.FirstOrDefault(h => g.Contains(h, induced, weightCondition));
                if (qq == null)
                    yield return g;
            }
        }

        public static List<Graph> RemoveIsomorphs(this List<Graph> graphs, List<Graph> excluded, bool induced = true, Func<Graph, Graph, int, int, bool> weightCondition = null)
        {
            if (weightCondition == null)
                weightCondition = WeightConditionDown;

            return graphs.Where(g => excluded.FirstOrDefault(h => g.Contains(h, induced, weightCondition)) == null).ToList();
        }

        public static IEnumerable<Graph> EnumerateRemovePrioritizedIsomorphs(this IEnumerable<Graph> graphs, IEnumerable<Graph> excluded, bool induced = true, Func<Graph, Graph, int, int, int> priority = null)
        {
            if (priority == null)
                priority = PriorityDown;

            foreach (var g in graphs)
            {
                var qq = excluded.FirstOrDefault(h => g.ContainsPrioritized(h, induced, priority));
                if (qq == null)
                    yield return g;
            }
        }

        public static List<Graph> RemovePrioritizedIsomorphs(this List<Graph> graphs, List<Graph> excluded, bool induced = true, Func<Graph, Graph, int, int, int> priority = null)
        {
            if (priority == null)
                priority = PriorityDown;

            return graphs.Where(g => excluded.FirstOrDefault(h => g.ContainsPrioritized(h, induced, priority)) == null).ToList();
        }

        public static bool WeightConditionEqual(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] == self.VertexWeight[selfV];
        }

        public static bool WeightConditionDown(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] >= self.VertexWeight[selfV];
        }

        public static bool WeightConditionUp(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] <= self.VertexWeight[selfV];
        }

        public static bool WeightConditionFalse(Graph self, Graph A, int selfV, int av)
        {
            return false;
        }

        public static int PriorityDown(Graph self, Graph A, int selfV, int av)
        {
            var p = A.VertexWeight[av] - self.VertexWeight[selfV];
            if (p < 0)
                return -1;

            return 1000 - p;
        }

        public static int PriorityUp(Graph self, Graph A, int selfV, int av)
        {
            var p = self.VertexWeight[selfV] - A.VertexWeight[av];
            if (p < 0)
                return -1;

            return 1000 - p;
        }

        public static int PriorityEqual(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] == self.VertexWeight[selfV] ? 0 : -1;
        }
    }
}
