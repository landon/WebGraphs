using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public enum FixerBreakerSwapMode
    {
        Original,
        SingleSwap,
        MultiSwap
    }

    public class SuperSlimMind : IMind
    {
        List<SuperSlimBoard> _remainingBoards;
        SuperSlimSwapAnalyzer _swapAnalyzer;
        SuperSlimColoringAnalyzer _coloringAnalyzer;
        int _lastProgress;

        public Graph G { get; private set; }
        Graph _lineG;
        List<Tuple<int, int>> _edges;

        public int MinPot { get; set; }
        public int MaxPot { get; set; }

        public int TotalBoards { get; private set; }
        public List<int> BoardCounts { get; private set; }
        public bool OnlyConsiderNearlyColorableBoards { get; set; }
        public bool ExcludeNonNearlyColorableNonSuperabundantBoards { get; set; }
        public int MissingEdgeIndex { get; set; }
        public bool OnlySuperabundantBoards { get; set; }
        public bool AllIntermediateBoardsInRestrictedClass { get; set; }

        public SuperSlimColoringAnalyzer ColoringAnalyzer { get { return _coloringAnalyzer; } }
        public List<SuperSlimBoard> PlayableBoards { get; private set; }
        public List<SuperSlimBoard> ColorableBoards { get; private set; }
        public List<SuperSlimBoard> NearlyColorableBoards { get; private set; }
        public List<SuperSlimBoard> SuperabundantBoards { get; private set; }
        public List<SuperSlimBoard> SuperabundantWithExtraPsiBoards { get; private set; }
        public List<SuperSlimBoard> BreakerWonBoards { get; private set; }
        public HashSet<SuperSlimBoard> FixerWonBoards { get; private set; }
        public int ExtraPsi { get; set; }

        public int NonSuperabundantBoardCount { get; private set; }
        public int NonSuperabundantExtraPsiBoardCount { get; private set; }
        public int NonNearlyColorableBoardCount { get; private set; }

        public SuperSlimMind(Graph g, bool proofFindingMode = false, FixerBreakerSwapMode swapMode = FixerBreakerSwapMode.SingleSwap)
        {
            G = g;
            BuildLineGraph();

            _coloringAnalyzer = new SuperSlimColoringAnalyzer(_lineG, GetEdgeColorList);
            _swapAnalyzer = new SuperSlimSwapAnalyzer(g.N, proofFindingMode, swapMode);
            
            MissingEdgeIndex = -1;
        }

        public bool Analyze(Template template, Action<Tuple<string, int>> progress = null)
        {
            _lastProgress = -1;
            _remainingBoards = new List<SuperSlimBoard>();
            FixerWonBoards = new HashSet<SuperSlimBoard>();
            BreakerWonBoards = new List<SuperSlimBoard>();
            BoardCounts = new List<int>();
            PlayableBoards = new List<SuperSlimBoard>();
            ColorableBoards = new List<SuperSlimBoard>();
            NearlyColorableBoards = new List<SuperSlimBoard>();
            SuperabundantBoards = new List<SuperSlimBoard>();
            SuperabundantWithExtraPsiBoards = new List<SuperSlimBoard>();
            
            var minimumColorCount = Math.Max(MinPot, template.Sizes.Max());
            var maximumColorCount = Math.Min(MaxPot, template.Sizes.Sum());

            for (int colorCount = minimumColorCount; colorCount <= maximumColorCount; colorCount++)
                _remainingBoards.AddRange(EnumerateAllBoards(template, colorCount, progress));

            TotalBoards = _remainingBoards.Count;
            FindColorableBoards(progress);
            FindNearlyColorableBoards(progress);
            FindSuperabundantBoards(progress);

            PlayableBoards.AddRange(_remainingBoards);

            Analyze(progress);

            var leftover = BreakerWonBoards.Union(_remainingBoards);
            if (OnlyConsiderNearlyColorableBoards)
                leftover = leftover.Intersect(NearlyColorableBoards);
            if (OnlySuperabundantBoards)
            {
                if (ExtraPsi <= 0)
                    leftover = leftover.Intersect(SuperabundantBoards);
                else
                    leftover = leftover.Intersect(SuperabundantWithExtraPsiBoards);
            }

            BreakerWonBoards = leftover.ToList();
            return BreakerWonBoards.Count <= 0;
        }

        void FindColorableBoards(Action<Tuple<string, int>> progress)
        {
            for (int i = _remainingBoards.Count - 1; i >= 0; i--)
            {
                var b = _remainingBoards[i];
                if (_coloringAnalyzer.Analyze(b))
                {
                    _remainingBoards.RemoveAt(i);
                    FixerWonBoards.Add(b);
                    ColorableBoards.Add(b);
                }

                DoProgress(progress, "Finding all colorable positions...");
            }

            BoardCounts.Add(ColorableBoards.Count);
        }

        void FindNearlyColorableBoards(Action<Tuple<string, int>> progress)
        {
            NonNearlyColorableBoardCount = 0;

            if (!OnlyConsiderNearlyColorableBoards)
                return;
            
            for (int i = _remainingBoards.Count - 1; i >= 0; i--)
            {
                var b = _remainingBoards[i];
                var nearlyColorable = MissingEdgeIndex >= 0 ? NearlyColorableForEdge(b, MissingEdgeIndex) : NearlyColorableForSomeEdge(b);

                if (nearlyColorable)
                    NearlyColorableBoards.Add(b);
                else
                {
                    NonNearlyColorableBoardCount++;
                    if (AllIntermediateBoardsInRestrictedClass)
                    {
                        _remainingBoards.RemoveAt(i);
                        DoProgress(progress, "Finding all nearly colorable positions...");
                    }
                }
            }
        }

        void FindSuperabundantBoards(Action<Tuple<string, int>> progress)
        {
            NonSuperabundantBoardCount = 0;
            NonSuperabundantExtraPsiBoardCount = 0;

            if (!OnlySuperabundantBoards)
                return;

            for (int i = _remainingBoards.Count - 1; i >= 0; i--)
            {
                var b = _remainingBoards[i];
                var superabundant = IsSuperabundantForGraph(b, G, 0);

                if (superabundant)
                {
                    SuperabundantBoards.Add(b);

                    if (ExtraPsi > 0)
                    {
                        if (ComputeAbundanceSurplus(b) >= ExtraPsi)
                        {
                            SuperabundantWithExtraPsiBoards.Add(b);
                        }
                        else
                        {
                            NonSuperabundantExtraPsiBoardCount++;

                            if (AllIntermediateBoardsInRestrictedClass)
                            {
                                _remainingBoards.RemoveAt(i);
                                DoProgress(progress, "Finding all superabundant positions with extra psi...");
                            }
                        }
                    }
                }
                else
                {
                    NonSuperabundantBoardCount++;

                    BreakerWonBoards.Add(b);
                    
                    _remainingBoards.RemoveAt(i);
                    DoProgress(progress, "Finding all superabundant positions...");
                }
            }
        }

        void Analyze(Action<Tuple<string, int>> progress)
        {
            while (_remainingBoards.Count > 0)
            {
                var wonBoards = new List<SuperSlimBoard>();
                for (int i = _remainingBoards.Count - 1; i >= 0; i--)
                {
                    var b = _remainingBoards[i];
                    if (_swapAnalyzer.Analyze(b, FixerWonBoards))
                    {
                        _remainingBoards.RemoveAt(i);
                        wonBoards.Add(b);

                        DoProgress(progress, string.Format("Finding all {0} move wins...", BoardCounts.Count));
                    }
                }

                if (wonBoards.Count > 0)
                {
                    BoardCounts.Add(wonBoards.Count);

                    foreach (var b in wonBoards)
                        FixerWonBoards.Add(b);
                }
                else
                {
                    break;
                }
            }
        }

        void DoProgress(Action<Tuple<string, int>> progress, string message)
        {
            if (progress != null)
            {
                var p = 100 * (TotalBoards - _remainingBoards.Count) / TotalBoards;
                if (p > _lastProgress)
                {
                    progress(new Tuple<string, int>(message, p));
                    _lastProgress = p;
                }
            }
        }

        bool NearlyColorableForEdge(SuperSlimBoard board, int edgeIndex)
        {
            return _coloringAnalyzer.ColorableWithoutEdge(board, edgeIndex);
        }

        bool ExistsNearlyColorableBoardForEachEdge(List<SuperSlimBoard> boards)
        {
            return Enumerable.Range(0, _lineG.N).All(e => boards.Any(b => NearlyColorableForEdge(b, e)));
        }

        public bool NearlyColorableForSomeEdge(SuperSlimBoard board)
        {
            return Enumerable.Range(0, _lineG.N).Any(e => NearlyColorableForEdge(board, e));
        }

        public bool IsSuperabundant(SuperSlimBoard b)
        {
            return IsSuperabundantForGraph(b, G);
        }

        public static bool IsSuperabundantForGraph(SuperSlimBoard b, Graph g, int extraPsi = 0)
        {
            ulong subset = 0;

            int total = 0;
            while (subset < (1UL << b._stackCount))
            {
                total = 0;
                for (int i = 0; i < b._length; i++)
                    total += (subset & b._trace[i]).PopulationCount() / 2;

                var e = g.EdgesOn(subset.ToSet());
                if (total < e)
                    return false;

                subset++;
            }

            if (extraPsi > 0)
                return total >= g.E + extraPsi;

            return true;
        }

        public int ComputeAbundanceSurplus(SuperSlimBoard b)
        {
            int total = 0;
            for (int i = 0; i < b._length; i++)
                total += b._trace[i].PopulationCount() / 2;

            return total - G.E;
        }

        IEnumerable<SuperSlimBoard> EnumerateAllBoards(Template template, int colorCount, Action<Tuple<string, int>> progress = null)
        {
            if (progress != null)
                progress(new Tuple<string, int>("Finding all positions...", 0));

            return BitLevelGeneration.Assignments_ulong.Generate(template.Sizes, colorCount).Select(t => new SuperSlimBoard(t, template.Sizes.Count));
        }

        void BuildLineGraph()
        {
            _edges = G.Edges.Value;
            _lineG = G.LineGraph.Value;
        }

        long GetEdgeColorList(SuperSlimBoard b, int e)
        {
            var v1 = _edges[e].Item1;
            var v2 = _edges[e].Item2;
            var stacks = b.Stacks.Value;

            return stacks[v1] & stacks[v2];
        }

        public GameTreeInfo GetWinTreeInfo(SuperSlimBoard board)
        {
            return _swapAnalyzer.WinTreeInfo[board];
        }

        int _gameTreeIndex;
        public GameTree BuildGameTree(SuperSlimBoard board, bool win = true)
        {
            var seenBoards =  new Dictionary<SuperSlimBoard, int>();
            _gameTreeIndex = 1;
            return BuildGameTree(board, seenBoards, win);
        }

        GameTree BuildGameTree(SuperSlimBoard board, Dictionary<SuperSlimBoard, int> seenBoards, bool win = true)
        {
            seenBoards[board] = _gameTreeIndex;
            var tree = new GameTree() { Board = board };
            tree.IsColorable = _coloringAnalyzer.Analyze(board);
            tree.IsSuperabundant = win || IsSuperabundant(board);
            tree.GameTreeIndex = _gameTreeIndex;
            _gameTreeIndex++;

            if (tree.IsColorable)
                return tree;

            if (!tree.IsSuperabundant)
                return tree;

            var treeInfo = win ? _swapAnalyzer.WinTreeInfo[board] : _swapAnalyzer.LossTreeInfo[board];
            foreach (var bc in treeInfo)
            {
                var childBoard = new SuperSlimBoard(board._trace, bc.Alpha, bc.Beta, bc.Response, board._stackCount);
                int index;
                if (seenBoards.TryGetValue(childBoard, out index))
                {
                    var ct = new GameTree() { Board = childBoard };
                    ct.IsColorable = _coloringAnalyzer.Analyze(childBoard);
                    ct.IsSuperabundant = win || IsSuperabundant(board);
                    ct.GameTreeIndex = _gameTreeIndex++;
                    ct.SameAsIndex = index;
                    tree.AddChild(ct, bc);
                    continue;
                }

                var childTree = BuildGameTree(childBoard, seenBoards, win);
                tree.AddChild(childTree, bc);
            }

            return tree;
        }
    }
}
