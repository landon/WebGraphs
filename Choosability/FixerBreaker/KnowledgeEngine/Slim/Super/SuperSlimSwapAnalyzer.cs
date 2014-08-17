using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SuperSlimSwapAnalyzer
    {
        ulong[] _fixerResponses;
        int _fixerResponseCount;

        public SuperSlimSwapAnalyzer(int n)
        {
            _fixerResponses = new ulong[1 << ((n + 1) >> 1)];
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
                            var childBoard = new SuperSlimBoard(board._trace, i, j, _fixerResponses[k]);
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
            // TODO: do this all in bit land
            var bits = swappable.GetBits();
            var count = bits.Count();
            var partitions = Choosability.FixerBreaker.Chronicle.BranchGenerator.GetPartitions(count);

            var choices = new List<List<ulong>>(partitions.Count);
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

            return choices;
        }

        static ulong Gosper(ulong a)
        {
            ulong c = a & (0 - a);
            ulong r = a + c;
            return (((r ^ a) >> 2) / c) | r;
        }
    }
}
