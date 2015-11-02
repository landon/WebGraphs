using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability.Utility;
using Choosability;

namespace Console
{
    static class NewGenerator
    {
        public static void Go()
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

                    return d;
                }).ToList());

            var all = new List<Graph>();

            var zi = g.VertexWeight.IndicesWhere(w => w == 0).ToList();
            foreach (var a in zi.Select(i => new[] { 6, 7 }).CartesianProduct())
            {
                var aa = a.ToList();
                for (int j = 0; j < aa.Count; j++)
                    g.VertexWeight[zi[j]] = aa[j];

                all.Add(g.Clone());
            }

            all = all.ToList();

            foreach (var gg in all.RemoveIsomorphs(excluded))
                gg.AppendWeightStringToFile(output);
        }
    }
}
