using Choosability;
using Choosability.Utility;
using Choosability.WordGame;
using Choosability.WordGame.Optimized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class FindTarpits
    {
        static int Length = 4;
        static StreamWriter _sw;

        public static void Go()
        {
            using (_sw = new StreamWriter("tarpits cart attempt" + Length + ".txt"))
            {
                _sw.AutoFlush = true;
                System.Console.ForegroundColor = ConsoleColor.White;

                var tarpitEnumerator = new FastTarpitEnumerator(Length);

                var count = 0;
                var tarpitCores = new List<List<string>>();

                tarpitEnumerator.GenerateMinimalTarpits(tarpit =>
                    {
                        var isClosed = TarpitEnumerator.IsPermutationClosed(tarpit);
                        var core = TarpitEnumerator.RemovePermutationRedundancies(tarpit);
                        core = TarpitEnumerator.ReorderAlphabetByOccurenceRate(core);

                        WriteLine(string.Join(" ", core));

                        System.Console.ForegroundColor = ConsoleColor.White;
                        count++;

                        tarpitCores.Add(core);
                    });

                
                WriteLine();
                Write("found " + count + " tarpits. ");
                WriteLine("computing transversal hypergraph...");
                WriteLine();

                var H = new Hypergraph<string>(tarpitCores);
                var transversals = H.Tr();

                var ordered = transversals.E.OrderBy(l => l.Count(ss => ss.Contains('z')) + string.Join("", l.OrderWords())).Select(t => t.OrderWords().ToList()).ToList();

                foreach (var t in ordered)
                {
                    var ss = string.Join(" ", t);
                    WriteLine(ss);
                }
                WriteLine();

               var chunks = Boxification.SplitMultiway(ordered);
               foreach (var chunk in chunks)
                   WriteLine(Boxification.ToMultiChunkString(chunk));

               // var chunks = Boxification.Split(ordered);
               //foreach (var chunk in chunks)
               //    WriteLine(Boxification.ToChunkString(chunk));
            }
        }

        static List<string> OrderWords(this IEnumerable<string> l)
        {
            return l.OrderByDescending(WordOrder).ToList();
        }

        static int WordOrder(string w)
        {
            return 100 * w.ToCharArray().Count(ccc => ccc == 'x') + w.ToCharArray().Count(ccc => ccc == 'y') - 10000 * w.ToCharArray().Distinct().Count();
        }

        static int CommonPrefixCount(List<string> l1, List<string> l2)
        {
            int i = 0;
            for (; i < Math.Min(l1.Count, l2.Count); i++)
            {
                if (l1[i] != l2[i])
                    return i;
            }

            return i;
        }

        static bool SameUpToCoordinatePermutation(List<string> l1, List<string> l2)
        {
            if (l1.Count <= 0 && l2.Count <= 0)
                return true;
            if (l2.Count <= 0)
                return false;
            var n = l2[0].Length;

            l1 = l1.OrderBy(s => s).ToList();
            foreach (var p in Permutation.EnumerateAll(n))
            {
                var vvv = TarpitEnumerator.EnumerateAlphabetPermutations(l2.Select(s => string.Join("", p.Apply(s.ToCharArray().ToList()))).OrderBy(s => s).ToList()).ToList();

                var vv = Enumerable.Range(0, l2.Count).Select(i => vvv.Select(l => l[i]).OrderBy(s => s).First());
                if (vv.SequenceEqual(l1))
                    return true;
            }

            return false;
        }

        static void Write(string s)
        {
            System.Console.Write(s);
            _sw.Write(s);
        }

        static void WriteLine(string s)
        {
            System.Console.WriteLine(s);
            _sw.WriteLine(s);
        }

        static void WriteLine()
        {
            System.Console.WriteLine();
            _sw.WriteLine();
        }
    }
}
