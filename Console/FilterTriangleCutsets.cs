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
    static class FilterTriangleCutsets
    {
        public static void FilterGraph6(string path)
        {
            var ep = Path.Combine(Path.GetDirectoryName(path), "tri_" + Path.GetFileName(path));
            File.Delete(ep);

            foreach (var g in path.EnumerateGraph6().FilterNonFacialTriangles())
                g.AppendGraph6ToFile(ep);
        }

        public static void FilterGraph6AndMinWeight(string path)
        {
            var ep = path + ".tri.weighted.txt";
            File.Delete(ep);

            foreach (var g in path.EnumerateGraph6().FilterNonFacialTrianglesAndWeight())
                g.AppendWeightStringToFile(ep);
        }

        public static IEnumerable<Graph> FilterNonFacialTriangles(this IEnumerable<Graph> graphs)
        {
            foreach (var g in graphs)
            {
                if (g.Vertices.All(v => g.Degree(v) >= 5 || g.EdgesOn(g.Neighbors[v]) != g.Degree(v)))
                    yield return g;
            }
        }

        public static IEnumerable<Graph> FilterNonFacialTrianglesAndWeight(this IEnumerable<Graph> graphs)
        {
            foreach (var g in graphs)
            {
                var vw = g.Vertices.Select(v =>
                    {
                        if (g.EdgesOn(g.Neighbors[v]) != g.Degree(v))
                            return 99;

                        return g.Degree(v);
                    }).ToList();

                if (vw.Min() >= 5)
                {
                    g.VertexWeight = vw;
                    yield return g;
                }
            }
        }
    }
}
