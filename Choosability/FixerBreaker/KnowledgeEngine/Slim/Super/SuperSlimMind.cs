﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SuperSlimMind : IMind
    {
        List<SuperSlimBoard> _remainingBoards;
        HashSet<SuperSlimBoard> _wonBoards;
        SuperSlimSwapAnalyzer _swapAnalyzer;
        SuperSlimColoringAnalyzer _coloringAnalyzer;
        Graph _graph;
        Graph _lineGraph;
        List<Tuple<int, int>> _edges;
        int _totalPositions;

        public int MinPot { get; set; }
        public int MaxPot { get; set; }
        public bool FixerWonAllNearlyColorableBoards { get; private set; }
        public bool HasNonSuperabundantBoardThatIsNearlyColorable { get; private set; }
        public int TotalPositions { get { return _totalPositions; } }
        public SuperSlimBoard BreakerWonBoard { get; private set; }
        public List<int> BoardCounts { get; private set; }
        public List<List<int>> BoardCountsList { get; private set; }
        public bool OnlyConsiderNearlyColorableBoards { get; set; }
        public bool ExcludeNonNearlyColorableNonSuperabundantBoards { get; set; }
        public int MissingEdgeIndex { get; set; }
        public bool SuperabundantOnly { get; set; }

        public List<SuperSlimBoard> NonColorableBoards { get; private set; }
        public List<SuperSlimBoard> DeepestBoards { get; private set; }
        public Dictionary<int, List<SuperSlimBoard>> BoardsOfDepth { get; private set; }

        public SuperSlimMind(Graph g)
        {
            _graph = g;
            BuildLineGraph();

            _coloringAnalyzer = new SuperSlimColoringAnalyzer(_lineGraph, GetEdgeColorList);
            _swapAnalyzer = new SuperSlimSwapAnalyzer(g.N);
            _wonBoards = new HashSet<SuperSlimBoard>();
            _remainingBoards = new List<SuperSlimBoard>();

            MissingEdgeIndex = -1;
        }

        public bool Analyze(Template template, Action<Tuple<string, int>> progress)
        {
            _wonBoards.Clear();
            _remainingBoards.Clear();
            BoardCountsList = new List<List<int>>();

            FixerWonAllNearlyColorableBoards = true;

            var minimumColorCount = Math.Max(MinPot, template.Sizes.Max());
            var maximumColorCount = Math.Min(MaxPot, template.Sizes.Sum());

            var foundAtLeastOneBoard = false;
            var fixerWin = true;
            for (int colorCount = minimumColorCount; colorCount <= maximumColorCount; colorCount++)
            {
                GenerateAllBoards(template, colorCount, progress);
                if (OnlyConsiderNearlyColorableBoards)
                {
                    if (MissingEdgeIndex >= 0)
                        _remainingBoards.RemoveAll(b => !NearlyColorableForEdge(b, MissingEdgeIndex));
                    else
                        _remainingBoards.RemoveAll(b => !NearlyColorableForSomeEdge(b));
                }

                if (foundAtLeastOneBoard && _remainingBoards.Count <= 0)
                    break;

                _totalPositions = _remainingBoards.Count + _wonBoards.Count;
                foundAtLeastOneBoard = true;

                fixerWin &= Analyze(progress);
            }

            return fixerWin;
        }

        bool Analyze(Action<Tuple<string, int>> progress = null)
        {
            int winLength = 0;
            var totalBoards = _remainingBoards.Count;
            var lastP = -1;

            BoardsOfDepth = new Dictionary<int, List<SuperSlimBoard>>();
            BoardCounts = new List<int>();
            BoardCountsList.Add(BoardCounts);
            BoardCounts.Add(_remainingBoards.Count);

            for (int i = _remainingBoards.Count - 1; i >= 0; i--)
            {
                var b = _remainingBoards[i];
                if (_coloringAnalyzer.Analyze(b))
                {
                    _remainingBoards.RemoveAt(i);
                    _wonBoards.Add(b);
                }

                if (progress != null)
                {
                    var p = 100 * (totalBoards - _remainingBoards.Count) / totalBoards;
                    if (p > lastP)
                    {
                        progress(new Tuple<string, int>("Finding all colorable positions...", p));
                        lastP = p;
                    }
                }
            }

            BoardsOfDepth[winLength] = _wonBoards.ToList();
            BoardCounts.Add(_remainingBoards.Count);

            var nonSuperabundantBoards = new List<SuperSlimBoard>();

            for (int i = _remainingBoards.Count - 1; i >= 0; i--)
            {
                var b = _remainingBoards[i];
                if (!IsSuperabundant(b))
                {
                    if (OnlyConsiderNearlyColorableBoards && MissingEdgeIndex >= 0)
                    {
                        HasNonSuperabundantBoardThatIsNearlyColorable = true;
                        BreakerWonBoard = b;
                        return false;
                    }

                    _remainingBoards.RemoveAt(i);
                    nonSuperabundantBoards.Add(b);
                }

                if (progress != null)
                {
                    var p = 100 * (totalBoards - _remainingBoards.Count) / totalBoards;
                    if (p > lastP)
                    {
                        progress(new Tuple<string, int>("Finding all non-superabundant positions...", p));
                        lastP = p;
                    }
                }
            }

            if (nonSuperabundantBoards.Count > 0 && !SuperabundantOnly)
            {
                if (!OnlyConsiderNearlyColorableBoards && !ExcludeNonNearlyColorableNonSuperabundantBoards || ExistsNearlyColorableBoardForEachEdge(nonSuperabundantBoards))
                {
                    HasNonSuperabundantBoardThatIsNearlyColorable = true;
                    BreakerWonBoard = nonSuperabundantBoards[0];
                    return false;
                }
            }

            BoardCounts.Add(_remainingBoards.Count);
            NonColorableBoards = _remainingBoards.ToList();

            while (_remainingBoards.Count > 0)
            {
                var wonBoards = new List<SuperSlimBoard>();
                winLength++;

                var count = _remainingBoards.Count;
                if (count > 0)
                    DeepestBoards = _remainingBoards.ToList();

                for (int i = _remainingBoards.Count - 1; i >= 0; i--)
                {
                    var b = _remainingBoards[i];
                    if (_swapAnalyzer.Analyze(b, _wonBoards))
                    {
                        _remainingBoards.RemoveAt(i);
                        wonBoards.Add(b);

                        if (progress != null)
                        {
                            var p = 100 * (totalBoards - _remainingBoards.Count) / totalBoards;
                            if (p > lastP)
                            {
                                progress(new Tuple<string, int>(string.Format("Finding all {0} move wins...", winLength), p));
                                lastP = p;
                            }
                        }
                    }
                }
                
                foreach (var b in wonBoards)
                    _wonBoards.Add(b);

                BoardsOfDepth[winLength] = wonBoards;
                BoardCounts.Add(_remainingBoards.Count);

                if (_remainingBoards.Count == count)
                {
                    if (OnlyConsiderNearlyColorableBoards && MissingEdgeIndex >= 0)
                    {
                        FixerWonAllNearlyColorableBoards = false;
                        BreakerWonBoard = _remainingBoards[0];
                    }
                    else if (ExistsNearlyColorableBoardForEachEdge(_remainingBoards))
                    {
                        FixerWonAllNearlyColorableBoards = false;
                        BreakerWonBoard = _remainingBoards.FirstOrDefault(b => _coloringAnalyzer.ColorableWithoutEdge(b, 0));

                        if (BreakerWonBoard == null)
                            BreakerWonBoard = _remainingBoards.First(b => _coloringAnalyzer.ColorableWithoutEdge(b, 0));
                    }

                    return false;
                }
            }

            return true;
        }

        bool NearlyColorableForEdge(SuperSlimBoard board, int edgeIndex)
        {
            return _coloringAnalyzer.ColorableWithoutEdge(board, edgeIndex);
        }

        bool ExistsNearlyColorableBoardForEachEdge(List<SuperSlimBoard> boards)
        {
            return Enumerable.Range(0, _lineGraph.N).All(e => boards.Any(b => NearlyColorableForEdge(b, e)));
        }

        bool NearlyColorableForSomeEdge(SuperSlimBoard board)
        {
            return Enumerable.Range(0, _lineGraph.N).Any(e => NearlyColorableForEdge(board, e));
        }

        void LookAtSuperabundance()
        {
            var packs = Enumerable.Range(0, _lineGraph.N).Select(e => _remainingBoards.Where(b => _coloringAnalyzer.ColorableWithoutEdge(b, e)).ToList()).ToList();
            var goodPacks = packs.Where(pack => pack.All(ssb => IsSuperabundant(ssb))).ToList();
        }

        bool IsSuperabundant(SuperSlimBoard b)
        {
            ulong subset = 0;

            while (subset < (1UL << b._stackCount))
            {
                int total = 0;
                for (int i = 0; i < b._length; i++)
                    total += (subset & b._trace[i]).PopulationCount() / 2;

                var e = _graph.EdgesOn(subset.ToSet());
                if (total < e)
                    return false;

                subset++;
            }

            return true;
        }

        void GenerateAllBoards(Template template, int colorCount, Action<Tuple<string, int>> progress = null)
        {
            if (progress != null)
                progress(new Tuple<string, int>("Finding all positions...", 0));

            foreach (var t in BitLevelGeneration.Assignments_ulong.Generate(template.Sizes, colorCount))
            {
                var b = new SuperSlimBoard(t, template.Sizes.Count);
                _remainingBoards.Add(b);
            }
        }

        void BuildLineGraph()
        {
            var adjacent = _graph.Adjacent;
            int n = adjacent.GetUpperBound(0) + 1;

            _edges = new List<Tuple<int, int>>();
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (adjacent[i, j])
                        _edges.Add(new Tuple<int, int>(i, j));

            var meets = new bool[_edges.Count, _edges.Count];
            for (int i = 0; i < _edges.Count; i++)
                for (int j = i + 1; j < _edges.Count; j++)
                    if (_edges[i].Item1 == _edges[j].Item1 ||
                        _edges[i].Item1 == _edges[j].Item2 ||
                        _edges[i].Item2 == _edges[j].Item1 ||
                        _edges[i].Item2 == _edges[j].Item2)
                        meets[i, j] = meets[j, i] = true;

            _lineGraph = new Graph(meets);
        }

        long GetEdgeColorList(SuperSlimBoard b, int e)
        {
            var v1 = _edges[e].Item1;
            var v2 = _edges[e].Item2;
            var stacks = b.Stacks.Value;

            return stacks[v1] & stacks[v2];
        }

        public GameTree BuildGameTree(SuperSlimBoard board)
        {
            var seenBoards =  new HashSet<SuperSlimBoard>();
            return BuildGameTree(board, seenBoards);
        }

        public GameTree BuildGameTree(SuperSlimBoard board, HashSet<SuperSlimBoard> seenBoards)
        {
            seenBoards.Add(board);
            var tree = new GameTree() { Board = board };
            tree.IsColorable = _coloringAnalyzer.Analyze(board);

            if (tree.IsColorable)
                return tree;

            foreach (var bc in _swapAnalyzer.TreeInfo[board])
            {
                var childBoard = new SuperSlimBoard(board._trace, bc.Alpha, bc.Beta, bc.Response, board._stackCount);
                if (seenBoards.Contains(childBoard))
                    continue;

                var childTree = BuildGameTree(childBoard, seenBoards);
                tree.AddChild(childTree, bc);
            }

            return tree;
        }
    }
}
