﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SuperSlimSwapAnalyzer
    {
        bool ProofFindingMode { get; set; }
        FixerBreakerSwapMode _swapMode;

        public Dictionary<SuperSlimBoard, GameTreeInfo> WinTreeInfo { get; private set; }
        public Dictionary<SuperSlimBoard, GameTreeInfo> LossTreeInfo { get; private set; }

        ulong[] _fixerResponses;
        int _fixerResponseCount;
        Dictionary<ulong, List<List<ulong>>> _breakerChoicesCache = new Dictionary<ulong, List<List<ulong>>>();

        public SuperSlimSwapAnalyzer(int n, bool proofFindingMode, FixerBreakerSwapMode swapMode)
        {
            _fixerResponses = new ulong[8192];

            ProofFindingMode = proofFindingMode;
            _swapMode = swapMode;
            if (ProofFindingMode)
            {
                WinTreeInfo = new Dictionary<SuperSlimBoard, GameTreeInfo>();
                LossTreeInfo = new Dictionary<SuperSlimBoard, GameTreeInfo>();
            }
        }

        public bool Analyze(SuperSlimBoard board, HashSet<SuperSlimBoard> wonBoards)
        {
            if (ProofFindingMode)
            {
                if (_swapMode == FixerBreakerSwapMode.Original)
                    return AnalyzeForProofInternalOriginalFixerBreaker(board, wonBoards);

                return AnalyzeForProofInternal(board, wonBoards);
            }

            if (_swapMode == FixerBreakerSwapMode.Original)
                return AnalyzeOriginalFixerBreaker(board, wonBoards);

            return AnalyzeInternal(board, wonBoards);
        }
       
        bool AnalyzeInternal(SuperSlimBoard board, HashSet<SuperSlimBoard> wonBoards)
        {
            for (int i = 0; i < board._length; i++)
            {
                for (int j = i + 1; j < board._length; j++)
                {
                    var x = board._trace[i];
                    var y = board._trace[j];
                    var swappable = x ^ y;
                    
                    var winningSwapAlwaysExists = true;
                    foreach (var breakerChoice in GetBreakerChoices(swappable))
                    {
                        var winningSwapExists = false;

                        GetFixerResponses(breakerChoice);
                        for (int k = 1; k < _fixerResponseCount; k++)
                        {
                            if (_swapMode == FixerBreakerSwapMode.SingleSwap && _fixerResponses[k].PopulationCount() > 2)
                                continue;

                            var childBoard = new SuperSlimBoard(board._trace, i, j, _fixerResponses[k], board._stackCount);
                            if (wonBoards.Contains(childBoard))
                            {
                                winningSwapExists = true;
                                break;
                            }
                        }

                        if (!winningSwapExists)
                        {
                            winningSwapAlwaysExists = false;
                            break;
                        }
                    }

                    if (winningSwapAlwaysExists)
                        return true;
                }
            }

            return false;
        }

        bool AnalyzeForProofInternal(SuperSlimBoard board, HashSet<SuperSlimBoard> wonBoards)
        {
            var winInfo = new GameTreeInfo();
            WinTreeInfo[board] = winInfo;

            var lossInfo = new GameTreeInfo();
            LossTreeInfo[board] = lossInfo;

            var colorPairs = new List<Tuple<int, int>>();
            for (int i = 0; i < board._length; i++)
                for (int j = i + 1; j < board._length; j++)
                    colorPairs.Add(new Tuple<int, int>(i, j));

            foreach (var cp in colorPairs.OrderBy(cp => (board._trace[cp.Item1] ^ board._trace[cp.Item2]).PopulationCount()))
            {
                var i = cp.Item1;
                var j = cp.Item2;

                var x = board._trace[i];
                var y = board._trace[j];
                var swappable = x ^ y;

                var winningSwapAlwaysExists = true;
                foreach (var breakerChoice in GetBreakerChoices(swappable))
                {
                    var winningSwapExists = false;

                    GetFixerResponses(breakerChoice);
                    var responses = Enumerable.Range(1, _fixerResponseCount - 1).Select(k => _fixerResponses[k]).OrderBy(fr => fr.PopulationCount());
                    foreach (var response in responses)
                    {
                        if (_swapMode == FixerBreakerSwapMode.SingleSwap && response.PopulationCount() > 2)
                            break;

                        var childBoard = new SuperSlimBoard(board._trace, i, j, response, board._stackCount);
                        if (wonBoards.Contains(childBoard))
                        {
                            winningSwapExists = true;
                            winInfo.Add(breakerChoice, i, j, response);
                            break;
                        }
                        else
                        {
                            lossInfo.Add(breakerChoice, i, j, response);
                        }
                    }

                    if (!winningSwapExists)
                    {
                        winInfo.Clear();
                        winningSwapAlwaysExists = false;
                        break;
                    }
                }

                if (winningSwapAlwaysExists)
                {
                    return true;
                }
            }

            return false;
        }

        bool AnalyzeForProofInternalOriginalFixerBreaker(SuperSlimBoard board, HashSet<SuperSlimBoard> wonBoards)
        {
            var winInfo = new GameTreeInfo();
            WinTreeInfo[board] = winInfo;

            var lossInfo = new GameTreeInfo();
            LossTreeInfo[board] = lossInfo;

            var colorPairs = new List<Tuple<int, int>>();
            for (int i = 0; i < board._length; i++)
                for (int j = i + 1; j < board._length; j++)
                    colorPairs.Add(new Tuple<int, int>(i, j));

            foreach (var cp in colorPairs.OrderBy(cp => (board._trace[cp.Item1] ^ board._trace[cp.Item2]).PopulationCount()))
            {
                var i = cp.Item1;
                var j = cp.Item2;

                var x = board._trace[i];
                var y = board._trace[j];
                var swappable = x ^ y;

                foreach (var v in swappable.GetBits())
                {
                    var good = true;
                    var responses = new [] {v}.Concat((swappable ^ v).GetBits().Select(w => w | v));
                    foreach (var response in responses)
                    {
                        var childBoard = new SuperSlimBoard(board._trace, i, j, response, board._stackCount);
                        if (wonBoards.Contains(childBoard))
                        {
                            winInfo.Add(new List<ulong>() { response }, i, j, response);
                        }
                        else
                        {
                            lossInfo.Add(new List<ulong>() { response }, i, j, response);
                            good = false;
                            break;
                        }
                    }

                    if (good)
                        return true;

                    winInfo.Clear();
                }
            }

            return false;
        }

        bool AnalyzeOriginalFixerBreaker(SuperSlimBoard board, HashSet<SuperSlimBoard> wonBoards)
        {
            var colorPairs = new List<Tuple<int, int>>();
            for (int i = 0; i < board._length; i++)
                for (int j = i + 1; j < board._length; j++)
                    colorPairs.Add(new Tuple<int, int>(i, j));

            foreach (var cp in colorPairs.OrderBy(cp => (board._trace[cp.Item1] ^ board._trace[cp.Item2]).PopulationCount()))
            {
                var i = cp.Item1;
                var j = cp.Item2;

                var x = board._trace[i];
                var y = board._trace[j];
                var swappable = x ^ y;

                foreach (var v in swappable.GetBits())
                {
                    var good = true;
                    var responses = new [] {v}.Concat((swappable ^ v).GetBits().Select(w => w | v));

                    foreach (var response in responses)
                    {
                        var childBoard = new SuperSlimBoard(board._trace, i, j, response, board._stackCount);
                        if (!wonBoards.Contains(childBoard))
                        {
                            good = false;
                            break;
                        }
                    }

                    if (good)
                        return true;
                }
            }

            return false;
        }

        void GetFixerResponses(List<ulong> possibleMoves)
        {
            _fixerResponseCount = 1 << possibleMoves.Count;
            var tableLength = _fixerResponses.Length;
            while (tableLength <= _fixerResponseCount)
                tableLength *= 2;
            if (tableLength > _fixerResponses.Length)
                _fixerResponses = new ulong[tableLength];

            var subset = 1;
            while (subset < _fixerResponseCount)
            {
                var response = 0UL;
                var x = subset;

                while (x != 0)
                    response |= possibleMoves[Int32Usage.GetAndClearLeastSignificantBit(ref x)];

                _fixerResponses[subset] = response;
                subset++;
            }
        }

        List<List<ulong>> GetBreakerChoices(ulong swappable)
        {
            List<List<ulong>> choices;
            if (!_breakerChoicesCache.TryGetValue(swappable, out choices))
            {
                var bits = swappable.GetBits();
                var partitions = Choosability.FixerBreaker.Chronicle.BranchGenerator.GetPartitions(bits.Count);
                choices = new List<List<ulong>>(partitions.Count);

                foreach (var partition in partitions)
                {
                    var choice = new List<ulong>(partition.Count);
                    choices.Add(choice);

                    foreach (var part in partition)
                    {
                        var x = 0UL;
                        foreach (var i in part)
                            x |= bits[i];

                        choice.Add(x);
                    }
                }

                _breakerChoicesCache[swappable] = choices;
            }

            return choices;
        }
    }
}
