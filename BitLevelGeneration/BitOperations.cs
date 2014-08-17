using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitLevelGeneration
{
    public static class BitOperations
    {
        public static ulong To_ulong(this IEnumerable<int> bits)
        {
            ulong x = 0;
            foreach (var bit in bits)
                x |= 1UL << bit;

            return x;
        }

        public static ushort To_ushort(this IEnumerable<int> bits)
        {
            ushort x = 0;
            foreach (var bit in bits)
                x |= (ushort)(1 << bit);

            return x;
        }

        public static byte To_byte(this IEnumerable<int> bits)
        {
            byte x = 0;
            foreach (var bit in bits)
                x |= (byte)(1 << bit);

            return x;
        }

		public static ulong RightFillToMSB(this ulong x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x |= x >> 32;

            return x;
        }
        
        public static ushort RightFillToMSB(this ushort x)
        {
            x |= (ushort)(x >> 1);
            x |= (ushort)(x >> 2);
            x |= (ushort)(x >> 4);
            x |= (ushort)(x >> 8);

            return x;
        }
        public static byte RightFillToMSB(this byte x)
        {
            x |= (byte)(x >> 1);
            x |= (byte)(x >> 2);
            x |= (byte)(x >> 4);

            return x;
        }
	}

    public static class BitUsage_uint
    {
        const uint DeBruijnMultiplier = 0x077CB531U;
        static int[] DeBruijnLookup = new int[32] { 0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9 };

        public static int Extract(this uint x)
        {
            return DeBruijnLookup[(x * DeBruijnMultiplier) >> 27];
        }

        public static int LeastSignificantBit(this uint x)
        {
            return DeBruijnLookup[((x & (~x + 1)) * DeBruijnMultiplier) >> 27];
        }

        public static int GetAndClearLeastSignificantBit(ref uint x)
        {
            var m = x & (~x + 1);
            x ^= m;
            return DeBruijnLookup[(m * DeBruijnMultiplier) >> 27];
        }

        public static int PopulationCount(this uint b)
        {
            int q = 0;
            while (b > 0)
            {
                q++;
                b &= b - 1;
            }
            return q;
        }
        public static List<int> ToSet(this uint x)
        {
            var onBits = new List<int>(10);
            while (x != 0)
                onBits.Add(GetAndClearLeastSignificantBit(ref x));

            return onBits;
        }
        public static uint RightFillToMSB(this uint x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;

            return x;
        }
        public static uint To_uint(this IEnumerable<int> bits)
        {
            uint x = 0;
            foreach (var bit in bits)
                x |= 1U << bit;

            return x;
        }

        public static uint Or(this uint[] colorGraph, int offset)
        {
            var result = 0U;
            for (int i = offset; i < colorGraph.Length; i++)
                result |= colorGraph[i];
            return result;
        }

        public static bool TrueForAllBitIndices(this uint x, Func<int, bool> predicate)
        {
            while (x != 0)
            {
                if (!predicate(GetAndClearLeastSignificantBit(ref x)))
                    return false;
            }

            return true;
        }
    }
}
