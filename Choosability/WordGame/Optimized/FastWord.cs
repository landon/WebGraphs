﻿using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.WordGame.Optimized
{
    public class FastWord
    {
        public Lazy<long[]> Stacks { get; private set; }
        public ulong[] _trace;
        public int _length;
        public int _stackCount;
        int _hashCode;

        public FastWord(ulong[] trace, int stackCount)
        {
            _trace = trace;
            _length = _trace.Length;
            _stackCount = stackCount;
            _hashCode = Hashing.Hash(_trace, _length);

            MakeLazyStacks();
        }

        public FastWord(ulong[] trace, int i, int j, ulong swap, int stackCount)
        {
            _trace = new ulong[trace.Length];
            _length = 0;
            for (int k = 0; k < trace.Length; k++)
            {
                ulong v = 0;
                if (k == i || k == j)
                    v = trace[k] ^ swap;
                else
                    v = trace[k];

                int q = _length;
                while (q > 0 && _trace[q - 1] > v)
                    q--;

                if (q < _length)
                    Buffer.BlockCopy(_trace, q << 3, _trace, (q + 1) << 3, (_length - q) << 3);
                _trace[q] = v;

                _length++;
            }

            _stackCount = stackCount;
            MakeLazyStacks();

            _hashCode = Hashing.Hash(_trace, _length);
        }

        void MakeLazyStacks()
        {
            Stacks = new Lazy<long[]>(() =>
            {
                var s = new long[_stackCount];
                var traceBits = _trace.Select(t => t.ToSet()).ToList();
                for (int c = 0; c < traceBits.Count; c++)
                    foreach (var i in traceBits[c])
                        s[i] |= 1L << c;
                return s;
            });
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FastWord);
        }

        public bool Equals(FastWord other)
        {
            if (other == null || _length != other._length)
                return false;

            for (int i = 0; i < _length; i++)
                if (_trace[i] != other._trace[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return string.Join("|", Stacks.Value.Select(l => string.Join("", l.ToSet())));
        }
    }
}