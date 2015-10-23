using Choosability;
using Choosability.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    static class SinkEliminator
    {
        public static void Go(string path)
        {
            Eliminate(path);
        }

        static void Eliminate(string path)
        {
            var eliminatedPath = path + ".nosink.txt";
            File.Delete(eliminatedPath);

            foreach (var g in GraphEnumerator.EnumerateGraphFile(path))
            {
                var h = g.Clone();
                while (h.E > 0 && h.Vertices.Any(v => h.OutDegree(v) == 0))
                {
                    var vv = h.Vertices.FirstOrDefault(v => h.OutDegree(v) == 0);
                    h = h.RemoveVertex(vv);
                }

                if (h.E > 0)
                    WriteGraph(h, eliminatedPath);
            }
        }

        static void WriteGraph(Graph g, string path)
        {
            var edgeWeights = string.Join(" ", g.GetEdgeWeights().Select(x => x.ToString()));
            var vertexWeights = "";
            if (g.VertexWeight != null)
                vertexWeights = " [" + string.Join(",", g.VertexWeight) + "]";

            using (var sw = new StreamWriter(path, append: true))
                sw.WriteLine(edgeWeights + vertexWeights);
        }
    }
}
