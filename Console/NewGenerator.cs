using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability.Utility;
using Choosability;
using Choosability.Planar;

namespace Console
{
    static class NewGenerator
    {
        public static List<Graph> GenerateWeightedNeighborhoods(int d, int dmin, int dmax, int rad, List<Graph> excluded = null)
        {
            var degrees = Enumerable.Range(dmin, dmax - dmin + 1);

            if (rad <= 0)
            {
                return new List<Graph>();
            }
            else if (rad == 1)
            {
                var g = Choosability.Graphs.C(d);
                g.VertexWeight = new int[g.N].ToList();
                g = g.AttachNewVertex(g.Vertices);

                var local = new List<Graph>();
                foreach (var a in Enumerable.Range(0, g.N - 1).Select(i => degrees.ToList()).CartesianProduct())
                {
                    g.VertexWeight = a.ToList();
                    g.VertexWeight.Add(d);
                    local.Add(g.Clone());
                }

                if (excluded != null)
                    local = local.RemoveIsomorphs(excluded, true, IsomorphRemover.WeightConditionDown);

                return local.RemoveSelfIsomorphs(true, IsomorphRemover.WeightConditionEqual);
            }
            else
            {
                var inner = GenerateWeightedNeighborhoods(d, dmin, dmax, rad - 1);
                var all = new List<Graph>();
                foreach (var h in inner)
                {
                    var g = Triangulation.Extend(h);

                    var local = new List<Graph>();
                    foreach (var a in Enumerable.Range(h.N, g.N - h.N).Select(i => degrees.ToList()).CartesianProduct())
                    {
                        var aa = a.ToList();
                        for (int i = h.N; i < g.N; i++)
                            g.VertexWeight[i] = aa[i - h.N];

                        local.Add(g.Clone());
                    }

                    if (excluded != null)
                        local = local.RemoveIsomorphs(excluded, true, IsomorphRemover.WeightConditionDown);

                    local = local.RemoveSelfIsomorphs(true, IsomorphRemover.WeightConditionEqual);

                    all.AddRange(local);
                }

                return all;
            }
        }

        public static void DoUiGraph()
        {
            var output = "generated.txt";
            File.Delete(output);

            Graphs.Graph uiG;
            using (var sr = new StreamReader("uigraph.txt"))
                uiG = GraphsCore.CompactSerializer.Deserialize(sr.ReadToEnd());

            var excluded = "excluded.txt".EnumerateWeightedGraphs().ToList();

            var g = new Choosability.Graph(uiG.GetEdgeWeights(), uiG.Vertices.Select(v =>
                {
                    int d;
                    if (!int.TryParse(v.Label, out d))
                        return 0;

                    return d-5;
                }).ToList());

            var all = new List<Graph>();

            var zi = g.VertexWeight.IndicesWhere(w => w == 0).ToList();
            foreach (var a in zi.Select(i => new[] { 1, 2 }).CartesianProduct())
            {
                var aa = a.ToList();
                for (int j = 0; j < aa.Count; j++)
                    g.VertexWeight[zi[j]] = aa[j];

                all.Add(g.Clone());
            }

            foreach (var gg in all.RemoveIsomorphs(excluded))
                gg.AppendWeightStringToFile(output);
        }
    }
}
