using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability;
using System.IO;

namespace Console
{
    public static class LocalGraphExtensions
    {
        public static IEnumerable<Graph> ToEdgeWeightsFile(this IEnumerable<Graph> graphs, string file)
        {
            if (!file.EndsWith(".ew"))
                file += ".ew";
            using (var sw = new StreamWriter(file))
            {
                foreach (var g in graphs)
                    sw.WriteLine(string.Join(" ", g.GetEdgeWeights()));
            }

            return graphs;
        }

        public static IEnumerable<Graph> ToGraph6File(this IEnumerable<Graph> graphs, string file)
        {
            if (!file.EndsWith(".g6"))
                file += ".g6";
            using (var sw = new StreamWriter(file))
            {
                foreach (var g in graphs)
                    sw.WriteLine(g.ToGraph6());
            }

            return graphs;
        }

        public static IEnumerable<Graph> EnumerateGraph6File(this string graph6File)
        {
            if (!File.Exists(graph6File))
                yield break;
            using (var sr = new StreamReader(graph6File))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                        break;

                    yield return new Graph(line.GetEdgeWeights());
                }
            }
        }

        public static List<int> GetEdgeWeights(this string graph6)
        {
            var chars = graph6.ToCharArray();
            var bytes = ASCIIEncoding.ASCII.GetBytes(chars);

            var N = bytes[0] - 63;
            var h = N * (N - 1) / 2;
            var w = bytes.Skip(1).SelectMany(b => Low6((byte)(b - 63))).Take(h).ToList();

            var p = RowToColumnPermutation(N).ToList();
            var wp = new List<int>(h);

            for (int i = 0; i < p.Count; i++)
                wp.Add(w[p[i]]);

            return wp;
        }

        public static string ToGraph6(this Graph g)
        {
            return g.GetEdgeWeights().ToGraph6();
        }

        public static string ToGraph6(this List<int> w)
        {
            var n = (int)((1 + Math.Sqrt(1 + 8 * w.Count)) / 2);
            if (n > 62)
                throw new NotImplementedException("i have yet to write/read graph6 files with more than 62 vertices.");

            var p = RowToColumnPermutation(n).ToList();
            var wp = new List<int>(w.Count);

            for (int i = 0; i < p.Count; i++)
                wp.Add(w[p.IndexOf(i)]);

            while (wp.Count % 6 != 0)
                wp.Add(0);

            return ASCIIEncoding.ASCII.GetString(wp.Batch<int, byte>(6, bits => (byte)(bits.Reverse().Index().Select(pair => pair.Value << pair.Key).Sum() + 63)).Prepend((byte)(n + 63)).ToArray());
        }

        static IEnumerable<int> RowToColumnPermutation(int n)
        {
            var x = 0;
            for (int j = 0; j < n - 1; j++)
            {
                var y = x;
                for (int i = 0; i < n - 1 - j; i++)
                {
                    yield return y;

                    y += 1 + j + i;
                }

                x += 2 + j;
            }
        }

        static IEnumerable<int> Low6(byte b)
        {
            for (int i = 5; i >= 0; i--)
                yield return (b & (1 << i)) >> i;
        }


        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            return Batch(source, size, x => x);
        }

        public static IEnumerable<TResult> Batch<TSource, TResult>(this IEnumerable<TSource> source, int size,
            Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            return BatchImpl(source, size, resultSelector);
        }

        private static IEnumerable<TResult> BatchImpl<TSource, TResult>(this IEnumerable<TSource> source, int size,
            Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                {
                    bucket = new TSource[size];
                }

                bucket[count++] = item;

                // The bucket is fully buffered before it's yielded
                if (count != size)
                {
                    continue;
                }

                // Select is necessary so bucket contents are streamed too
                yield return resultSelector(bucket.Select(x => x));

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
            {
                yield return resultSelector(bucket.Take(count));
            }
        }

        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(this IEnumerable<TSource> source)
        {
            return source.Index(0);
        }

        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(this IEnumerable<TSource> source, int startIndex)
        {
            return source.Select((item, index) => new KeyValuePair<int, TSource>(startIndex + index, item));
        }

        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return System.Linq.Enumerable.Concat(System.Linq.Enumerable.Repeat(value, 1), source);
        }
    }
}
