using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.WordGame.Optimized
{
    public class FastAccessibilityChecker
    {
        ulong[] _fixerResponses;
        int _fixerResponseCount;
        Dictionary<int, List<List<ulong>>> _breakerChoicesCache = new Dictionary<int, List<List<ulong>>>();

        public FastAccessibilityChecker(int n)
        {
            _fixerResponses = new ulong[1 << ((n + 1) >> 1)];
        }

        public bool IsAccessible(FastWord board, HashSet<FastWord> wonBoards)
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
                            var childBoard = new FastWord(board._trace, i, j, _fixerResponses[k], board._stackCount);
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
                    {
                        return true;
                    }
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
            var count = swappable.PopulationCount();
            List<List<ulong>> choices;
            if (!_breakerChoicesCache.TryGetValue(count, out choices))
            {
                var partitions = Choosability.FixerBreaker.Chronicle.BranchGenerator.GetPartitions(count);

                choices = new List<List<ulong>>(partitions.Count);
                var bits = swappable.GetBits();
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

                _breakerChoicesCache[count] = choices;
            }

            return choices;
        }
    }
}
