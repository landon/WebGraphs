using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.WordGame.Optimized
{
    public class FastTarpitEnumerator : TarpitEnumerator
    {
        List<FastWord> _words;
        FastAccessibilityChecker _accessibilityChecker;

        public FastTarpitEnumerator(int n)
        {
            _words = new FastWordGenerator().GenerateWords(n);
            _accessibilityChecker = new FastAccessibilityChecker(n);
        }

        public override IEnumerable<List<string>> EnumerateMinimalTarpits()
        {
            return EnumerateMinimalTarpitsFast().Select(list => list.Select(Wordify).ToList());
        }

        public IEnumerable<List<FastWord>> EnumerateMinimalTarpitsFast()
        {
            var seen = new HashSet<List<FastWord>>(new SortedListComparer());
            return EnumerateMinimalTarpitsFastIn(_words, seen);
        }

        string Wordify(FastWord b)
        {
            return string.Join("", b.Stacks.Value.Select(s =>
                {
                    switch (s)
                    {
                        case 1: return 'x';
                        case 2: return 'y';
                        case 4: return 'z';
                        default: return '?';
                    }
                }));
        }

        IEnumerable<List<FastWord>> EnumerateMinimalTarpitsFastIn(List<FastWord> S, HashSet<List<FastWord>> seen)
        {
            var excluded = false;
            foreach (var T in S.Select(w => S.Except(new[] { w }).ToList()))
            {
                var W = RunEscape(T);
                if (W.Count <= 0)
                    continue;

                excluded = true;

                if (seen.Contains(W))
                    continue;
                seen.Add(W);

                foreach (var TP in EnumerateMinimalTarpitsFastIn(W, seen))
                    yield return TP;
            }

            if (!excluded)
                yield return S.ToList();
        }

        List<FastWord> RunEscape(List<FastWord> S)
        {
            var T = S.ToList();
            while (true)
            {
                var R = new HashSet<FastWord>(_words.Except(T));
                if (T.RemoveAll(w => _accessibilityChecker.IsAccessible(w, R)) <= 0)
                    break;
            }

            return T;
        }

        class SortedListComparer : IEqualityComparer<List<FastWord>>
        {
            public bool Equals(List<FastWord> x, List<FastWord> y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(List<FastWord> list)
            {
                unchecked
                {
                    int hash = 19;
                    foreach (var x in list)
                        hash = hash * 31 + x.GetHashCode();

                    return hash;
                }
            }
        }
    }
}
