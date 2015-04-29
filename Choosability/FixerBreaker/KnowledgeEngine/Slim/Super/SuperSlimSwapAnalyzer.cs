using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SuperSlimSwapAnalyzer
    {
        bool StoreTreeInfo { get; set; }
        public Dictionary<SuperSlimBoard, GameTreeInfo> WinTreeInfo { get; private set; }
        public Dictionary<SuperSlimBoard, GameTreeInfo> LossTreeInfo { get; private set; }

        ulong[] _fixerResponses;
        int _fixerResponseCount;
        Dictionary<ulong, List<List<ulong>>> _breakerChoicesCache = new Dictionary<ulong, List<List<ulong>>>();

        public SuperSlimSwapAnalyzer(int n, bool storeTreeInfo = true)
        {
            _fixerResponses = new ulong[1 << ((n + 1) >> 1)];
            StoreTreeInfo = storeTreeInfo;
            if (StoreTreeInfo)
            {
                WinTreeInfo = new Dictionary<SuperSlimBoard, GameTreeInfo>();
                LossTreeInfo = new Dictionary<SuperSlimBoard, GameTreeInfo>();
            }
        }

        public bool Analyze(SuperSlimBoard board, HashSet<SuperSlimBoard> wonBoards)
        {
            GameTreeInfo winInfo = null;
            GameTreeInfo lossInfo = null;
            if (StoreTreeInfo)
            {
                winInfo = new GameTreeInfo();
                WinTreeInfo[board] = winInfo;

                lossInfo = new GameTreeInfo();
                LossTreeInfo[board] = lossInfo;
            }

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
                                if (StoreTreeInfo)
                                    winInfo.Add(breakerChoice, i, j, _fixerResponses[k]);
                                break;
                            }
                            else
                            {
                                if (StoreTreeInfo)
                                    lossInfo.Add(breakerChoice, i, j, _fixerResponses[k]);
                            }
                        }

                        if (!winningSwapExists)
                        {
                            if (StoreTreeInfo)
                                winInfo.Clear();
                            winningSwapAlwaysExists = false;
                            break;
                        }
                        else
                        {
                           // if (StoreTreeInfo)
                           //     lossInfo.Clear();
                        }
                    }

                    if (winningSwapAlwaysExists)
                        return true;
                }
            }

            return false;
        }

        void GetFixerResponses(List<ulong> possibleMoves)
        {
            _fixerResponseCount = 1 << possibleMoves.Count;

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
