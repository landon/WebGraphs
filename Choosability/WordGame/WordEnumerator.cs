using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Choosability;
using Choosability.Utility;

namespace Choosability.WordGame
{
    public class WordEnumerator
    {
        List<char> _alphabet;

        public WordEnumerator(params char[] alphabet) : this(((IEnumerable<char>)alphabet)) { }
        public WordEnumerator(IEnumerable<char> alphabet)
        {
            _alphabet = alphabet.ToList();
        }

        public IEnumerable<string> EnumerateWords(int length)
        {
            return Enumerable.Repeat(_alphabet, length).CartesianProduct().Select(chars => new string(chars.ToArray()));
        }
    }
}
