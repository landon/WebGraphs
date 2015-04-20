using Choosability;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public class GraphPictureMaker
    {
        static readonly DotRenderer Renderer = new DotRenderer(@"C:\Program Files\Graphviz2.36\bin\dot.exe");
        static readonly List<string> DotColors = new List<string>() { "cadetblue", "brown", "dodgerblue", "turquoise", "orchid", "blue", "red", "green", 
                                                                      "yellow", "cyan",
                                                                      "limegreen",  "pink", 
                                                                      "orange",  "goldenrod", "aquamarine", "black", };

        IEnumerable<Graph> _graphs;

        public GraphPictureMaker(string graphFile) : this(GraphEnumerator.EnumerateGraphFile(graphFile)) { }
        public GraphPictureMaker(params Graph[] graphs) : this((IEnumerable<Graph>)graphs) { }
        public GraphPictureMaker(IEnumerable<Graph> graphs)
        {
            _graphs = graphs;
        }

        public void DrawAll(string outputDirectory, DotRenderType renderType = DotRenderType.png)
        {
            Directory.CreateDirectory(outputDirectory);

            foreach (var g in _graphs)
            {
                var name = string.Join("", g.GetEdgeWeights());
                if (g.VertexWeight != null)
                    name += "[" + string.Join(",", g.VertexWeight) + "]";

                Draw(g, Path.Combine(outputDirectory, name), renderType);
            }
        }

        public void GenerateAllDots(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);

            foreach (var g in _graphs)
            {
                var name = string.Join("", g.GetEdgeWeights());
                if (g.VertexWeight != null)
                    name += "[" + string.Join(",", g.VertexWeight) + "]";

                using(var sw = new StreamWriter(Path.Combine(outputDirectory, name) + ".dot"))
                    sw.Write(ToDot(g));
            }
        }

        void Draw(Graph g, string path, DotRenderType renderType = DotRenderType.png)
        {
            Renderer.Render(ToDot(g), path, renderType);
        }

        static string ToDot(Graph g)
        {
            var sb = new StringBuilder();

            sb.AppendLine("graph G {");
            sb.AppendLine("overlap = false;");
            sb.AppendLine("splines=true;");
            sb.AppendLine("sep=0.2;");
            sb.AppendLine("node[fontsize=20, style=bold, color=black; shape=circle, penwidth=1];");
            sb.AppendLine("edge[style=bold, color=black, penwidth=4];");

            foreach (int v in g.Vertices)
            {
                int colorIndex = g.VertexWeight == null ? 0 : g.VertexWeight[v] - 2;
                if (colorIndex < 0)
                    colorIndex += 13;
                sb.AppendLine(string.Format(@"{0} [label = ""{2}"", style = filled, fillcolor = ""{1}""];", v, DotColors[colorIndex], g.VertexWeight == null ? "" : g.VertexWeight[v].ToString()));
            }

            for (int i = 0; i < g.N; i++)
                for (int j = i + 1; j < g.N; j++)
                {
                    if (g[i, j])
                    {
                        sb.AppendLine(string.Format("{0} -- {1}", i, j));
                    }
                }

            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
