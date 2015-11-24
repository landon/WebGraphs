using BitLevelGeneration;
using Choosability.Utility;
using Choosability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Console
{
    public static class NonCrossing
    {
        public static void Generate()
        {
            for (int i = 4; i <= 15; i++)
            {
                var dq = GeneratePartitions(i);
                var ncq = dq.Where(ll => ll.All(l => NoAdjacent(l, i))).ToList();

                var ndd = ncq.Where(ll => NC(ll, i)).ToList();
                var ndq = MaximalElements(ndd);
                var j = 0;
                using (var sw = new StreamWriter("non_crossing" + i + ".txt"))
                {
                    foreach (var ll in ndq)
                    {
                        var ss = string.Join(",", ll.Select(l => l.ToSetString()));
                        sw.WriteLine(ss);

                        j++;
                    }
                }

                System.Console.WriteLine(i + " : " + j);
            }
        }

        public static void MakePicture(int n)
        {
            var gs = LoadGraphs("non_crossing" + n + ".txt", n);

            var gpm = new GraphPictureMaker(gs);
            gpm.CompressName = false;
            gpm.UseLaplacian = true;
            gpm.DrawAllAndMakeWebpage(@"C:\Users\landon\Dropbox\Public\Web\GraphData\NonCrossing\" + n);
        }

        public static List<Graph> LoadGraphs(string path, int n)
        {
            var lll = Load(path);

            var gs = new List<Graph>();
            foreach (var ll in lll)
            {
                var g = Choosability.Graphs.C(n);
                foreach (var l in ll)
                {
                    if (l.Count <= 1)
                        continue;

                    foreach (var pair in ListUtility.EnumerateSublists(l, 2))
                    {
                        g = g.AddEdge(pair[0], pair[1]);
                    }
                }

                g.VertexWeight = g.Vertices.ToList();
                gs.Add(g);
            }
            return gs;
        }

        static List<List<List<int>>> Load(string file)
        {
            string s;
            using (var sr = new StreamReader(file))
                s = sr.ReadToEnd();

            return s.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(ss => ss.Split(new [] {'{'}, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim('{', '}', ',').Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries).Select(pp => int.Parse(pp)).ToList()).ToList()).ToList();
        }

        public static List<List<List<int>>> MaximalElements(List<List<List<int>>> A)
        {
            var chainHeads = new List<List<List<int>>>();

            foreach (var set in A)
            {
                var removed = chainHeads.RemoveAll(head => Contained(head, set));
                if (removed > 0 || chainHeads.All(head => !Contained(set, head)))
                    chainHeads.Add(set);
            }

            return chainHeads;
        }

        static bool Contained(List<List<int>> A, List<List<int>> B)
        {
            return A.All(ll => B.Any(lll => ll.SubsetEqualSorted(lll)));
        }

        static bool NC(List<List<int>> ll, int n)
        {
            var cc = ll.Select(z => GetComponents(z, n)).ToList();

            for (int i = 0; i < ll.Count; i++)
            {
                for (int j = i + 1; j < ll.Count; j++)
                {
                    if (cc[j].All(c => !ll[i].SubsetEqualSorted(c)))
                        return false;
                }
            }

            return true;
        }

        static List<List<int>> GetComponents(List<int> l, int n)
        {
            var components = new List<List<int>>();
            List<int> c = null;

            int i = 0;
            while (i < n)
            {
                while (l.Contains(i) && i < n)
                {
                    c = null;
                    i++;
                }

                if (i >= n)
                    break;

                if (c == null)
                {
                    c = new List<int>();
                    components.Add(c);
                }

                c.Add(i);

                i++;
            }

            if (components.Count > 1 && !l.Contains(n - 1))
            {
                components[0].AddRange(components[components.Count - 1]);
                components.RemoveAt(components.Count - 1);
            }

            return components;
        }

        static bool NoAdjacent(List<int> l, int n)
        {
            return l.Select((i, j) => i - j).Distinct().Count() == l.Count && (l[l.Count - 1] != n - 1 || l[0] != 0);
        }

        static List<List<List<int>>> GeneratePartitions(int n)
        {
            var pp = Assignments_ulong.Generate(Enumerable.Repeat(1, n).Concat(n).ToList(), n);
            return pp.Select(ss => MakePartition(MakeStacks(ss, n + 1).Take(n).SelectMany(t => t).ToList()).ToList()).ToList();
        }

        static List<List<int>> MakePartition(List<int> t)
        {
            var parts = new List<List<int>>();
            foreach (var group in t.Select((x, i) => new { X = x, I = i }).GroupBy(T => T.X))
            {
                var part = group.Select(T => T.I).ToList();
                part.Sort();
                parts.Add(part);
            }

            return parts;
        }

        static List<List<int>> MakeStacks(ulong[] trace, int count)
        {
            var s = new long[count];
            var traceBits = trace.Select(t => t.ToSet()).ToList();
            for (int c = 0; c < traceBits.Count; c++)
                foreach (var i in traceBits[c])
                    s[i] |= 1L << c;
            return s.Select(aa => aa.ToSet()).ToList();
        }
    }
}
