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
        public static IEnumerable<Graph> RemoveSelfIsomorphs(this IEnumerable<Graph> graphs, bool induced = true, Func<Graph, Graph, int, int, bool> weightCondition = null)
        {
            if (weightCondition == null)
                weightCondition = WeightConditionDown;

            var all = graphs.ToList();

            for (int i = 0; i < all.Count; i++)
            {
                var g = all[i];
                var good = true;
                for (int j = 0; j < all.Count; j++)
                {
                    if (j == i)
                        continue;

                    if (g.Contains(all[j], induced, weightCondition))
                    {
                        if (i < j || !all[j].Contains(g, induced, weightCondition))
                        {
                            good = false;
                            break;
                        }
                    }
                }

                if (good)
                    yield return g;
            }
        }

        public static IEnumerable<Graph> RemoveIsomorphs(this IEnumerable<Graph> graphs, IEnumerable<Graph> excluded, bool induced = true, Func<Graph, Graph, int, int, bool> weightCondition = null)
        {
            if (weightCondition == null)
                weightCondition = WeightConditionDown;

            return graphs.Where(g => !excluded.Any(h => g.Contains(h, induced, weightCondition)));

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
