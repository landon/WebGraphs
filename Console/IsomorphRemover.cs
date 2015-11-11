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
        public static List<Graph> RemoveSelfIsomorphs(this IEnumerable<Graph> graphs, bool induced = true, Func<Graph, Graph, int, int, bool> weightCondition = null)
        {
            if (weightCondition == null)
                weightCondition = WeightConditionDown;

            var all = graphs.ToList();
            var some = new List<Graph>(all.Count);

            for (int i = 0; i < all.Count; i++)
            {
                var g = all[i];
                var good = true;

                foreach (var h in some)
                {
                    if (!Graph.MaybeIsomorphic(g, h))
                        continue;

                    if (!g.VertexWeight.OrderBy(x => x).SequenceEqual(h.VertexWeight.OrderBy(x => x)))
                        continue;

                    if (g.Contains(h, induced, weightCondition))
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

        public static IEnumerable<Graph> RemoveIsomorphs(this IEnumerable<Graph> graphs, IEnumerable<Graph> excluded, bool induced = true, Func<Graph, Graph, int, int, bool> weightCondition = null)
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
    }
}
