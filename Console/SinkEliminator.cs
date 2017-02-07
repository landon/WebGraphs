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

            foreach (var g in GraphEnumerator.EnumerateGraphFile(path).EliminateSinks())
                g.AppendWeightStringToFile(eliminatedPath);
        }

        public static IEnumerable<Graph> EliminateSinks(this IEnumerable<Graph> graphs)
        {
            foreach (var g in graphs)
            {
                var h = EliminateSinks(g);
                if (h.E > 0)
                    yield return h;
            }
        }

        public static Graph EliminateSinks(this Graph g)
        {
            var h = g.Clone();
            while (h.E > 0 && h.Vertices.Any(v => h.OutDegree(v) == 0))
            {
                var vv = h.Vertices.FirstOrDefault(v => h.OutDegree(v) == 0);
                h = h.RemoveVertex(vv);
            }

            return h;
        }
    }
}
