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
        public static List<Graph> GenerateWeightedNeighborhoods(int d, int dmin, int dmax, int rad, List<Graph> excluded = null, int maxHigh = int.MaxValue)
        {
            var degrees = Enumerable.Range(dmin, dmax - dmin + 1).ToList();

            if (rad <= 0)
            {
                return new List<Graph>();
            }
            else if (rad == 1)
            {
                var g = Choosability.Graphs.K(1);
                g = g.Join(Choosability.Graphs.C(d));
                g.VertexWeight = new int[g.N].ToList();

                var local = new List<Graph>();
                foreach (var a in Enumerable.Range(0, g.N - 1).Select(i => degrees.ToList()).CartesianProduct())
                {
                    var aa = a.ToList();
                    if (aa.Count(ww => ww == dmax) > maxHigh)
                        continue;

                    g.VertexWeight = aa;
                    g.VertexWeight.Insert(0, d);
                    local.Add(g.Clone());
                }

                if (excluded != null)
                {
                    local = local.RemovePrioritizedIsomorphs(excluded);
                }

                return local.RemoveSelfIsomorphs(true);
            }
            else
            {
                var inner = GenerateWeightedNeighborhoods(d, dmin, dmax, rad - 1, excluded, maxHigh);
                var all = new List<Graph>();
                foreach (var h in inner)
                {
                    var pre = h.VertexWeight.Count(ww => ww == dmax);
                    var ld = degrees;
                    if (pre >= maxHigh)
                        ld = degrees.Except(new[] { dmax }).ToList();

                    var g = Triangulation.Extend(h);

                    var local = new List<Graph>();
                    foreach (var a in Enumerable.Range(h.N, g.N - h.N).Select(i => ld.ToList()).CartesianProduct())
                    {
                        var aa = a.ToList();
                        if (pre + aa.Count(ww => ww == dmax) > maxHigh)
                            continue;

                        for (int i = h.N; i < g.N; i++)
                            g.VertexWeight[i] = aa[i - h.N];

                        local.Add(g.Clone());
                    }

                    if (excluded != null)
                    {
                        local = local.RemovePrioritizedIsomorphs(excluded);
                    }

                    local = local.RemoveSelfIsomorphs(true);

                    if (local.Count > 0)
                    {
                        System.Console.Write(all.Count + " + " + local.Count);
                        all.AddRange(local);
                        System.Console.WriteLine(" = " + all.Count);
                    }
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
