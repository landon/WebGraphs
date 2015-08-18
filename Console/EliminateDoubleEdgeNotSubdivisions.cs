using Choosability;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public class EliminateDoubleEdgeNotSubdivisions
    {
        static bool AT;
        public static void Go(string path, bool at)
        {
            AT = at;
            Eliminate(path);
        }

        static void Eliminate(string path)
        {
            var eliminatedPath = path + ".not.eliminated.txt";
            File.Delete(eliminatedPath);

            var graphs = GraphEnumerator.EnumerateGraphFile(path).ToList();
            var excluded = graphs.SelectMany(g => OneStepEliminators(g)).ToList();
            foreach (var gg in graphs.Where(g => excluded.All(h => !Graph.Isomorphic(h, g))))
                WriteGraph(gg, eliminatedPath);
        }

        static IEnumerable<Graph> OneStepEliminators(Graph g)
        {
            for (int v = 0; v < g.N; v++)
            {
                for (int w = v + 1; w < g.N; w++)
                {
                    if (g[v, w])
                    {
                        var ge = g.RemoveEdge(v, w);
                        var good = !ge.IsOnlineFGChoosable(vv => ge.Degree(vv) - g.VertexWeight[vv], vv => 1);

                        if (!good && AT)
                            good |= MixedChoosables.HasFOrientation(ge, vv => ge.Degree(vv) - g.VertexWeight[vv]) == null;

                        if (good)
                        {
                            yield return ge.AttachNewVertex(v)
                              .AttachNewVertex(g.N)
                              .AddEdge(g.N + 1, w);
                        }
                    }
                }
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
