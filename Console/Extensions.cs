using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitLevelGeneration;
using Choosability;
using Choosability.Polynomials;
using Choosability.Utility;
using System.Threading;
using System.Diagnostics;

namespace Console
{
    public static class Extensions
    {
        public static string BrowserPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        public static string GraphDataWebRoot = @"https://dl.dropboxusercontent.com/u/8609833/Web/GraphData";
        public static void ToWebPageSimple(this IEnumerable<Choosability.Graph> graphs, string relativePath, int K = 0)
        {
            var maker = new GraphPictureMaker(graphs);
            maker.NameGraph6 = true;
            maker.K = K;
            var path = Path.Combine(@"C:\Users\landon\Dropbox\Public\Web\GraphData", relativePath);

            MakePictures.MakeWebpage(maker, path, directed: false, showFactors: false, lowPlus: false, fivePlus: false, useLaplacian: false);

            Path.Combine(GraphDataWebRoot, relativePath, "index.html").ToBrowser();
        }

        public static void ToWebPage(this IEnumerable<Choosability.Graph> graphs, string relativePath, bool directed = true, bool fivePlus = true, bool useLaplacian = true, bool compressName = true, bool lowPlus = false)
        {
            var maker = new GraphPictureMaker(graphs);
            maker.CompressName = compressName;
            var path = Path.Combine(@"C:\Users\landon\Dropbox\Public\Web\GraphData", relativePath);

            MakePictures.MakeWebpage(maker, path, directed: directed, showFactors: false, lowPlus: lowPlus, fivePlus: fivePlus, useLaplacian: useLaplacian);

            Path.Combine(GraphDataWebRoot, relativePath, "index.html").ToBrowser();
        }

        public static string LegalizeFileName(this string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c.ToString(), "(" + ASCIIEncoding.ASCII.GetBytes(c.ToString())[0] + ")");

            return name;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
        }

        public static void ToBrowser(this string url)
        {
            Process.Start(BrowserPath, "\"" + url + "\"");
        }

        public static void WriteToWeightFile(this IEnumerable<Choosability.Graph> graphs, string path)
        {
            using (var sw = new StreamWriter(path, append: false))
            {
                foreach (var g in graphs)
                    sw.WriteLine(g.ToWeightString());
            }
        }

        public static void AppendWeightStringToFile(this Choosability.Graph g, string path)
        {
            using (var sw = new StreamWriter(path, append: true))
                sw.WriteLine(g.ToWeightString());
        }

        public static void AppendGraph6ToFile(this Choosability.Graph g, string path)
        {
            using (var sw = new StreamWriter(path, append: true))
                sw.WriteLine(g.ToGraph6());
        }

        public static string ToWeightString(this Choosability.Graph g)
        {
            var edgeWeights = string.Join(" ", g.GetEdgeWeights().Select(x => x.ToString()));
            var vertexWeights = "";
            if (g.VertexWeight != null)
                vertexWeights = " [" + string.Join(",", g.VertexWeight) + "]";

            return edgeWeights + vertexWeights;
        }

        public static Choosability.Graph FromWeightString(this string s, bool removeOrientation = false, int weightAdjustment = 0)
        {
            var parts = s.Split(' ');
            var edgeWeights = parts.Where(p => !p.StartsWith("[")).Select(x => removeOrientation ? Math.Abs(int.Parse(x)) : int.Parse(x)).ToList();

            List<int> vertexWeights = null;
            var vwp = parts.FirstOrDefault(p => p.StartsWith("["));
            if (vwp != null)
                vertexWeights = vwp.Trim('[').Trim(']').Split(',').Select(x => weightAdjustment + int.Parse(x)).ToList();

            return new Choosability.Graph(edgeWeights, vertexWeights);
        }

        public static Graph WebgraphToGraph(this string webgraph)
        {
            var uiG = GraphsCore.CompactSerializer.Deserialize(webgraph);

            return new Choosability.Graph(uiG.GetEdgeWeights(), uiG.Vertices.Select(v =>
            {
                int d;
                if (!int.TryParse(v.Label, out d))
                    return 0;

                return d;
            }).ToList());
        }

        public static IEnumerable<Choosability.Graph> EnumerateWeightedGraphs(this string path, bool removeOrientation = false, int weightAdjustment = 0)
        {
            using (var sr = new StreamReader(path))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        break;

                    yield return line.FromWeightString(removeOrientation, weightAdjustment);
                }
            }
        }

        public static List<List<int>> ToStackList(this IEnumerable<ulong> trace, int count)
        {
            var s = trace.ToStackLong(count);
            return s.Select(aa => aa.ToSet()).ToList();
        }

        public static List<long> ToStackLong(this IEnumerable<ulong> trace, int count)
        {
            var s = new long[count];
            var traceBits = trace.Select(t => t.ToSet()).ToList();
            for (int c = 0; c < traceBits.Count; c++)
            {
                foreach (var i in traceBits[c])
                    s[i] |= 1L << c;
            }

            return s.ToList();
        }

        public static IEnumerable<List<long>> EnumerateListAssignments(this IEnumerable<int> sizes, int potSize)
        {
            var augmented = sizes.Concat(potSize).ToList();
            return Assignments_ulong.Enumerate(augmented, potSize).Select(ula =>
                {
                    var ll = ula.ToStackLong(augmented.Count);
                    ll.RemoveAt(ll.Count - 1);

                    return ll;
                });
        }

        public static List<List<long>> GenerateListAssignments(this IEnumerable<int> sizes, int potSize)
        {
            var augmented = sizes.Concat(potSize).ToList();
            return Assignments_ulong.Generate(augmented, potSize).Select(ula =>
            {
                var ll = ula.ToStackLong(augmented.Count);
                ll.RemoveAt(ll.Count - 1);

                return ll;
            }).ToList();
        }

        public static IEnumerable<Choosability.Graph> EnumerateGraph6(this string path)
        {
            using (var sr = new StreamReader(path))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                        break;

                    var ew = line.GetEdgeWeights();
                    yield return new Choosability.Graph(ew);
                }
            }
        }

        public static OrientationResult HasFOrientation(this Graph g, Func<int, int> f)
        {
            const int RandomTries = 10;
            int MaxFails = 100;

            var degreeSequences = new List<int[]>();

            for (int i = 0; i < RandomTries; i++)
            {
                var o = g.GenerateRandomOrientation();

                if (Enumerable.Range(0, o.N).Any(v => f(v) <= o.OutDegree(v)))
                {
                    i--;
                    MaxFails--;
                    if (MaxFails < 0)
                        break;
                    continue;
                }

                var result = CheckOrientation(o, degreeSequences);
                if (result != null)
                    return result;
            }

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var tasks = new Task[2];
            tasks[0] = Task<bool>.Factory.StartNew(() => g.IsOnlineFChoosable(v => f(v), token), token);
            tasks[1] = Task<OrientationResult>.Factory.StartNew(() =>
            {
                foreach (var orientation in g.EnumerateOrientations(v => g.Degree(v) + 1 - f(v)))
                {
                    if (token.IsCancellationRequested)
                        return null;
                    var result = CheckOrientation(orientation, degreeSequences);
                    if (result != null)
                        return result;
                }

                return null;
            }, token);


            var doneTask = tasks[Task.WaitAny(tasks)];
            if (doneTask is Task<OrientationResult>)
            {
                var result = ((Task<OrientationResult>)doneTask).Result;
                tokenSource.Cancel();

                return result;
            }
            else
            {
                if (((Task<bool>)doneTask).Result)
                {
                    return ((Task<OrientationResult>)tasks[1]).Result;
                }
                else
                {
                    tokenSource.Cancel();
                    return null;
                }
            }
        }

        public static OrientationResult HasFOrientationSkipPaint(this Graph g, Func<int, int> f)
        {
            var degreeSequences = new List<int[]>();
            foreach (var orientation in g.EnumerateOrientations(v => g.Degree(v) + 1 - f(v)))
            {
                var result = CheckOrientation(orientation, degreeSequences);
                if (result != null)
                    return result;
            }

            return null;
        }

        static OrientationResult CheckOrientation(Graph orientation, List<int[]> degreeSequences)
        {
            if (degreeSequences.Any(seq => Enumerable.SequenceEqual(seq, orientation.InDegreeSequence.Value)))
                return null;

            degreeSequences.Add(orientation.InDegreeSequence.Value);

            var c = orientation.GetCoefficient(orientation.Vertices.Select(v => orientation.OutDegree(v)).ToArray());
            if (c != 0)
                return new OrientationResult() { Graph = orientation, Even = c, Odd = 0 };

            return null;
        }

        public class OrientationResult
        {
            public Graph Graph { get; set; }
            public int Even { get; set; }
            public int Odd { get; set; }
        }
    }
}
