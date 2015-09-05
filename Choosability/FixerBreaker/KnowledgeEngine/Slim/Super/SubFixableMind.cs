using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SubFixableMind
    {
        public bool ProofFindingMode;
        public FixerBreakerSwapMode SwapMode = FixerBreakerSwapMode.SingleSwap;
        public Action<Tuple<string, int>> Progress;

        public Reduction CanReduceToSuperabundantExtraPsi(Graph g, SuperSlimBoard board, Tuple<int, int> forbiddenEdge = null)
        {
            return CanReduce(g, board, (h, l) => l.FirstOrDefault(r => SuperSlimMind.IsSuperabundantForGraph(r.Board, h, 1)), forbiddenEdge);
        }

        public Reduction CanReduceToSuperabundant(Graph g, SuperSlimBoard board, Tuple<int, int> forbiddenEdge = null)
        {
            return CanReduce(g, board, (h, l) => l.FirstOrDefault(r => SuperSlimMind.IsSuperabundantForGraph(r.Board, h)), forbiddenEdge);
        }

        public Reduction CanReduceToWin(Graph g, SuperSlimBoard board, Tuple<int, int> forbiddenEdge = null)
        {
            return CanReduce(g, board, CheckReductionForWin, forbiddenEdge);
        }

        Reduction CanReduce(Graph g, SuperSlimBoard board, Func<Graph, List<Reduction>, Reduction> reducibilityChecker, Tuple<int, int> forbiddenEdge = null)
        {
            var edges = g.Edges.Value;
            var pendantEdges = g.PendantEdges.Value;

            foreach (var tup in pendantEdges)
            {
                var v = tup.Item1;
                var w = tup.Item2;

                if (g.Degree(w) > 1)
                {
                    v = tup.Item2;
                    w = tup.Item1;
                }

                if (forbiddenEdge != null && (w == forbiddenEdge.Item1 || w == forbiddenEdge.Item2))
                    continue;

                var h = g.RemoveEdge(tup);
                var list = board.Stacks.Value[v] & board.Stacks.Value[w];

                var reductions = new List<Reduction>();
                foreach (var c in list.GetBits())
                {
                    var reducedStacks = board.Stacks.Value.ToList();
                    reducedStacks[w] = 0;
                    reducedStacks[v] ^= c;

                    var reducedBoard = new SuperSlimBoard(reducedStacks);
                    reductions.Add(new Reduction()
                    {
                        Color = c,
                        Board = reducedBoard,
                        Stem = v,
                        Leaf = w
                    });
                }

                var r = reducibilityChecker(h, reductions);
                if (r != null)
                    return r;
            }

            return null;
        }

        Reduction CheckReductionForWin(Graph h, List<Reduction> reductions)
        {
            var possibleReductions = reductions.Where(r => SuperSlimMind.IsSuperabundantForGraph(r.Board, h)).ToList();
            if (possibleReductions.Count <= 0)
                return null;

            var sizes = possibleReductions[0].Board.Stacks.Value.Select(s => s.PopulationCount()).ToList();
            var mind = new SuperSlimMind(h, ProofFindingMode, SwapMode);
            mind.Analyze(new Template(sizes), Progress);

            return possibleReductions.FirstOrDefault(r => mind.FixerWonBoards.Contains(r.Board));
        }
    }

    public class Reduction
    {
        public int Stem;
        public int Leaf;
        public SuperSlimBoard Board;
        public long Color;
    }
}
