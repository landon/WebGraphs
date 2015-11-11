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

namespace Console
{
    public static class Extensions
    {
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
