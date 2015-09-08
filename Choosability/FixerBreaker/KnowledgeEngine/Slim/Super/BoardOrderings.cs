using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public static class BoardOrderings
    {
        public static int CurrentOrder(SuperSlimMind mind, SuperSlimBoard a, SuperSlimBoard b)
        {
            var ep = GlobalExtraPsiOrder(mind, a, b);
            if (ep != 0)
                return ep;

            var ii = InternalIntersectionOrder(mind, a, b);
            if (ii != 0)
                return ii;

            return 0;
        }

        public static int InternalIntersectionOrder(SuperSlimMind mind, SuperSlimBoard a, SuperSlimBoard b)
        {
            var asum = -InternalIntersectionCount(mind.G, a);
            var bsum = -InternalIntersectionCount(mind.G, b);

            if (asum < bsum)
                return -1;
            if (asum > bsum)
                return 1;
            return 0;
        }

        public static int GlobalExtraPsiOrder(SuperSlimMind mind, SuperSlimBoard a, SuperSlimBoard b)
        {
            return mind.ComputeAbundanceSurplus(a).CompareTo(mind.ComputeAbundanceSurplus(b));
        }

        public static int InternalVertexMatchingLexOrder(SuperSlimMind mind, SuperSlimBoard a, SuperSlimBoard b)
        {
            var al = VertexMatchingLex(mind.G, a);
            var bl = VertexMatchingLex(mind.G, b);

            var av = al[0] - al[al.Count - 1];
            var bv = bl[0] - bl[bl.Count - 1];

            if (av < bv)
                return -1;
            if (av > bv)
                return 1;
            return 0;
        }

        public static int InternalVertexMatchingSumOrder(SuperSlimMind mind, SuperSlimBoard a, SuperSlimBoard b)
        {
            var asum = VertexMatchingSum(mind.G, a);
            var bsum = VertexMatchingSum(mind.G, b);

            if (asum < bsum)
                return -1;
            if (asum > bsum)
                return 1;
            return 0;
        }

        static int InternalIntersectionCount(Graph g, SuperSlimBoard board)
        {
            long and = -1L;
            foreach (var v in g.Vertices)
            {
                if (g.Degree(v) <= 1)
                    continue;

                and &= board.Stacks.Value[v];
            }

            return and.PopulationCount();
        }

        static int VertexMatchingSum(Graph g, SuperSlimBoard board)
        {
            var sum = 0;
            foreach (var v in g.Vertices)
            {
                if (g.Degree(v) <= 1)
                    continue;

                sum += VertexMatchingCount(g, board, v);
            }

            return sum;
        }

        static List<int> VertexMatchingLex(Graph g, SuperSlimBoard board)
        {
            var list = new List<int>();
            foreach (var v in g.Vertices)
            {
                if (g.Degree(v) <= 1)
                    continue;

                list.Add(VertexMatchingCount(g, board, v));
            }

            return list.OrderByDescending(x => x).ToList();
        }

        static int VertexMatchingCount(Graph g, SuperSlimBoard board, int v)
        {
            long commonColors = 0;
            foreach(var w in g.Neighbors[v])
            {
                commonColors |= board.Stacks.Value[v] & board.Stacks.Value[w];
            }

            return commonColors.PopulationCount();
        }
    }
}
