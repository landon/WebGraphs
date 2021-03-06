﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public static class BoardOrderings
    {
        public static Func<SuperSlimMind, SuperSlimBoard, SuperSlimBoard, int> OrderingFunc = CurrentOrder;

        public static int CurrentOrder(SuperSlimMind mind, SuperSlimBoard a, SuperSlimBoard b)
        {
            return BestKnownOrder(mind, a, b);
        }

        public static int BestKnownOrder(SuperSlimMind mind, SuperSlimBoard a, SuperSlimBoard b)
        {
            var ep = GlobalExtraPsiOrder(mind, a, b);
            if (ep != 0)
                return ep;

            var ii = InternalIntersectionOrder(mind, a, b);
            if (ii != 0)
                return ii;

            return 0;
        }

        public static int InternalIntersectionInCommonOrder(SuperSlimMind mind, SuperSlimBoard a, SuperSlimBoard b)
        {
            var asum = -InternalIntersectionCount(mind.G, a);
            var bsum = -InternalIntersectionCount(mind.G, b);

            if (asum < bsum)
                return -1;
            if (asum > bsum)
                return 1;
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

        static int InternalIntersectionInCommonCount(Graph g, SuperSlimBoard board)
        {
            long and = -1L;
            foreach (var v in g.Vertices)
            {
                if (g.Degree(v) <= 1)
                    continue;

                var neighborStack = 0L;
                foreach (var w in g.Neighbors[v])
                {
                    if (g.Degree(w) > 1)
                        continue;

                    neighborStack |= board.Stacks.Value[w];
                }

                and &= board.Stacks.Value[v] & neighborStack;
            }

            return and.PopulationCount();
        }
    }
}
