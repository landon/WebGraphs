using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SuperSlimSwapAnalyzer
    {
        bool ProofFindingMode { get; set; }
        public Dictionary<SuperSlimBoard, GameTreeInfo> WinTreeInfo { get; private set; }
        public Dictionary<SuperSlimBoard, GameTreeInfo> LossTreeInfo { get; private set; }

        ulong[] _fixerResponses;
        int _fixerResponseCount;
        Dictionary<ulong, List<List<ulong>>> _breakerChoicesCache = new Dictionary<ulong, List<List<ulong>>>();

        public SuperSlimSwapAnalyzer(int n, bool proofFindingMode = false)
        {
            _fixerResponses = new ulong[8192];
            ProofFindingMode = proofFindingMode;
            if (ProofFindingMode)
            {
                WinTreeInfo = new Dictionary<SuperSlimBoard, GameTreeInfo>();
                LossTreeInfo = new Dictionary<SuperSlimBoard, GameTreeInfo>();
            }
        }

        public bool Analyze(SuperSlimBoard board, HashSet<SuperSlimBoard> wonBoards)
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

        public bool AnalyzeForProof(SuperSlimBoard board, HashSet<SuperSlimBoard> wonBoards, bool singletonOnly = false)
        {
            var lossInfo = new GameTreeInfo();
            LossTreeInfo[board] = lossInfo;

            var winInfos = new GameTreeInfo[board._length, board._length];
            GameTreeInfo bestWinInfo = null;
            var minWinSwaps = int.MaxValue;

            for (int i = 0; i < board._length; i++)
            {
                for (int j = i + 1; j < board._length; j++)
                {
                    winInfos[i, j] = new GameTreeInfo();

                    var x = board._trace[i];
                    var y = board._trace[j];
                    var swappable = x ^ y;

                    var winSwaps = 0;
                    var winningSwapAlwaysExists = true;
                    foreach (var breakerChoice in GetBreakerChoices(swappable))
                    {
                        var winningSwapExists = false;

                        GetFixerResponses(breakerChoice);
                        var responses = Enumerable.Range(1, _fixerResponseCount - 1).Select(k => _fixerResponses[k]).OrderBy(fr => fr.PopulationCount());
                        foreach (var response in responses)
                        {
                            if (singletonOnly && response.PopulationCount() > 1)
                                continue;

                            var childBoard = new SuperSlimBoard(board._trace, i, j, response, board._stackCount);
                            if (wonBoards.Contains(childBoard))
                            {
                                winningSwapExists = true;
                                winInfos[i,j].Add(breakerChoice, i, j, response);
                                winSwaps += response.PopulationCount();
                                break;
                            }
                            else
                            {
                                lossInfo.Add(breakerChoice, i, j, response);
                            }
                        }

                        if (!winningSwapExists)
                        {
                            winInfos[i, j].Clear();
                            winningSwapAlwaysExists = false;
                            break;
                        }
                    }

                    if (winningSwapAlwaysExists)
                    {
                        if (winSwaps < minWinSwaps)
                        {
                            bestWinInfo = winInfos[i, j];
                            minWinSwaps = winSwaps;
                        }
                    }
                }
            }

            if (bestWinInfo != null)
            {
                WinTreeInfo[board] = bestWinInfo;
                return true;
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
