using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void AppendToFile(this Choosability.Graph g, string path)
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

        public static Choosability.Graph FromWeightString(this string s)
        {
            var parts = s.Split(' ');
            var edgeWeights = parts.Where(p => !p.StartsWith("[")).Select(x => int.Parse(x)).ToList();

            List<int> vertexWeights = null;
            var vwp = parts.FirstOrDefault(p => p.StartsWith("["));
            if (vwp != null)
                vertexWeights = vwp.Trim('[').Trim(']').Split(',').Select(x => int.Parse(x)).ToList();

            return new Choosability.Graph(edgeWeights, vertexWeights);
        }

        public static IEnumerable<Choosability.Graph> EnumerateWeightedGraphs(this string path)
        {
            using (var sr = new StreamReader(path))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        break;

                    yield return line.FromWeightString();
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
    }
}
