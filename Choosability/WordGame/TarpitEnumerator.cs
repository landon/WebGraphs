using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Choosability.Utility;

namespace Choosability.WordGame
{
    public class TarpitEnumerator
    {
        List<char> _alphabet;
        List<string> _words;
        AccessibilityChecker _accessibilityChecker;

        public TarpitEnumerator(int n)
        {
            _alphabet = new List<char>() { 'x', 'y', 'z' };

            _words = new WordEnumerator(_alphabet).EnumerateWords(n).ToList();
            _accessibilityChecker = new AccessibilityChecker(_alphabet);
        }

        public bool IsPermutationClosed(List<string> S)
        {
            return EnumerateAlphabetPermutations(S).Select(T => string.Join(",", T)).Distinct().Count() == 1;
        }

        public IEnumerable<List<string>> EnumerateMinimalTarpits()
        {
            var seen = new HashSet<string>();
            return EnumerateMinimalTarpitsIn(_words, seen);
        }

        IEnumerable<List<string>> EnumerateMinimalTarpitsIn(List<string> S, HashSet<string> seen)
        {
            if (S.Count <= 0)
                yield break;

            var key = GenerateSimpleKey(S);
            if (seen.Contains(key))
                yield break;
            seen.Add(key);

            var properlyContainsTarpit = false;
            foreach (var T in S.Select(w => S.Except(new[] { w }).ToList()))
            {
                foreach (var TP in EnumerateMinimalTarpitsIn(RunEscape(T), seen))
                {
                    properlyContainsTarpit = true;
                    yield return TP;
                }
            }

            if (!properlyContainsTarpit)
                yield return S.OrderBy(s => s).ToList();
        }

        List<string> RunEscape(List<string> S)
        {
            var T = S.ToList();
            while (true)
            {
                var R = _words.Difference(T);
                if (T.RemoveAll(w => _accessibilityChecker.IsAccessible(w, R)) <= 0)
                    break;
            }

            return T;
        }

        string GenerateSimpleKey(List<string> S)
        {
            return string.Join(",", S.OrderBy(s => s));
        }
        string GeneratePermutedKey(List<string> S)
        {
            return EnumerateAlphabetPermutations(S).Select(T => string.Join(",", T)).OrderBy(s => s).First();
        }

        IEnumerable<List<string>> EnumerateAlphabetPermutations(List<string> S)
        {
            var uppered = S.Select(s => s.ToUpper()).ToList();
            foreach (var permutation in Permutation.EnumerateAll(3))
            {
                var p = permutation.Apply(_alphabet);
                yield return uppered.Select(s => s.Replace('X', p[0]).Replace('Y', p[1]).Replace('Z', p[2])).OrderBy(s => s).ToList();
            }
        }
    }
}
