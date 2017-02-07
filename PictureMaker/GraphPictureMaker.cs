using GraphsCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureMaker
{
    public class GraphPictureMaker
    {
        DotRenderer Renderer = new DotRenderer(@"C:\Program Files (x86)\Graphviz2.38\bin\neato.exe");
        static readonly List<string> DotColors = new List<string>() { "cadetblue", "brown", "dodgerblue", "turquoise", "orchid", "blue", "red", "green", 
                                                                      "yellow", "cyan",
                                                                      "limegreen",  "pink", 
                                                                      "orange",  "goldenrod", "aquamarine", "black", "white"};

        public GraphPictureMaker(bool skipLayout = true)
        {
            Renderer.SkipLayout = skipLayout;
        }


        public string Draw(Graphs.Graph g, string path, DotRenderType renderType= DotRenderType.svg)
        {
            if (g == null)
                return null;

            var imageFile = Renderer.Render(ToDot(g), path, renderType);
         
            if (renderType == DotRenderType.svg)
                FixSvg(imageFile);

            return imageFile;
        }

        public static void FixSvg(string svgFile)
        {
            string[] lines;
            using (var sr = new StreamReader(svgFile))
                lines = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            using (var sw = new StreamWriter(svgFile))
            {
                foreach (var line in lines)
                {
                    var l = line;

                    if (l.StartsWith("<polygon fill=\"white\""))
                        continue;
                    if (l.StartsWith("<svg"))
                        l = "<svg width=\"100%\" height=\"100%\"";

                    sw.WriteLine(l);
                }
            }
        }

        string ToDot(Graphs.Graph uiG)
        {
            var sb = new StringBuilder();

            if (uiG.Edges.All(e => e.Orientation == Graphs.Edge.Orientations.None))
                sb.AppendLine("graph G {");
            else
                sb.AppendLine("digraph G {");
            sb.AppendLine("overlap = false;");
            sb.AppendLine("splines=true;");
            sb.AppendLine("sep=0.0;");
            sb.AppendLine("node[fontsize=42, fontname=\"Latin Modern Math\" color=black; shape=circle, penwidth=1, width = .92, height=.92, fixedsize=true];");
            sb.AppendLine("edge[style=bold, color=black, penwidth=2];");

            int q;
            var allNumbers = uiG.Vertices.All(v => int.TryParse(v.Label, out q));

            int nextIndex = 0;
            var colorIndex = new Dictionary<string, int>();
            foreach (var v in uiG.Vertices)
            {
                var l = v.Label ?? "";

                if (allNumbers)
                {
                    colorIndex[l] = int.Parse(v.Label) + 9992;
                }
                else
                {
                    if (!colorIndex.ContainsKey(l))
                        colorIndex[l] = nextIndex++;
                }
            }

            foreach (var v in uiG.Vertices)
            {
                sb.AppendLine(string.Format(@"{0} [label = ""{2}"", style = filled, fillcolor = ""{1}"", pos = ""{3},{4}""];", uiG.Vertices.IndexOf(v), DotColors[colorIndex[v.Label ?? ""] % DotColors.Count], v.Label ?? "", v.X, -v.Y));
            }

            foreach (var e in uiG.Edges)
            {
                var i = uiG.Vertices.IndexOf(e.V1);
                var j = uiG.Vertices.IndexOf(e.V2);

                sb.AppendLine(string.Format(@"{0} {3} {1} [label = ""{2}""]", i, j, e.Label ?? "", OrientationString(e.Orientation)));
                //sb.AppendLine(string.Format(@"{0} {2} {1}", i, j, OrientationString(e.Orientation)));
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        string OrientationString(Graphs.Edge.Orientations o)
        {
            switch (o)
            {
                case Graphs.Edge.Orientations.None:
                    return "--";
                case Graphs.Edge.Orientations.Forward:
                    return "->";
                case Graphs.Edge.Orientations.Backward:
                    return "<-";
            }

            return "--";
        }
    }
}
