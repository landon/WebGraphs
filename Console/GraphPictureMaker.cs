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
        static readonly DotRenderer Renderer = new DotRenderer(@"C:\Program Files (x86)\Graphviz2.38\bin\neato.exe");
        static readonly List<string> DotColors = new List<string>() { "cadetblue", "brown", "dodgerblue", "turquoise", "orchid", "blue", "red", "green", 
                                                                      "yellow", "cyan",
                                                                      "limegreen",  "pink", 
                                                                      "orange",  "goldenrod", "aquamarine", "black", "white"};

        static readonly List<int> ColoringMap = new List<int>() { 6, 5, 7, 8, 0, 1, 2, 3, 4, 9, 10, 11, 12, 13, 14, 15, 16 };

        IEnumerable<Graph> _graphs;

        public bool Directed { get; set; }
        public bool ShowFactors { get; set; }
        public bool InDegreeTerms { get; set; }
        public bool IsLowPlus { get; set; }
        public bool ProperColor { get; set; }

        public GraphPictureMaker(string graphFile) : this(GraphEnumerator.EnumerateGraphFile(graphFile)) { }
        public GraphPictureMaker(params Graph[] graphs) : this((IEnumerable<Graph>)graphs) { }
        public GraphPictureMaker(IEnumerable<Graph> graphs)
        {
            _graphs = graphs;
        }

        public void DrawAllAndMakeWebpage(string outputDirectory)
        {
            var imageFiles = DrawAll(outputDirectory, DotRenderType.svg);
            var graphs = string.Join(",", imageFiles.Select(path => "'" + Path.GetFileName(path) + "'"));
            using (var sw = new StreamWriter(Path.Combine(outputDirectory, "index.html")))
                sw.Write(Resource.index.Replace("__FILL_THIS_SLOT__", graphs));
        }

        public List<string> DrawAll(string outputDirectory, DotRenderType renderType = DotRenderType.png)
        {
            var imageFiles = new List<string>();
            Directory.CreateDirectory(outputDirectory);
            foreach (var g in _graphs)
            {
                var name = string.Join("", g.GetEdgeWeights().Select(ew => Math.Abs(ew)));
                if (g.VertexWeight != null)
                    name += "[" + string.Join(",", g.VertexWeight) + "]";

                if (name == "")
                    name = "0";
                var path = Path.Combine(outputDirectory, name);
                var imageFile = Draw(g, path, renderType);

                imageFiles.Add(imageFile);
            }

            return imageFiles;
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
                    sw.Write(ToDot(g, Directed, ShowFactors));
            }
        }

        string Draw(Graph g, string path, DotRenderType renderType = DotRenderType.png)
        {
            var imageFile = Renderer.Render(ToDot(g, Directed, ShowFactors, InDegreeTerms, IsLowPlus, ProperColor), path, renderType);
         
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

        static string ToDot(Graph g, bool directed = false, bool showFactors = false, bool inDegreeTerms = false, bool isLowPlus = false, bool properColor = false)
        {
            if (showFactors)
                return g.ToDotWithFactors();

            var sb = new StringBuilder();

            if (directed)
                sb.AppendLine("digraph G {");
            else
                sb.AppendLine("graph G {");
            sb.AppendLine("overlap = false;");
            sb.AppendLine("splines=true;");
            sb.AppendLine("sep=0.0;");
            sb.AppendLine("node[fontsize=42, fontname=\"Latin Modern Math\" color=black; shape=circle, penwidth=1, width = .92, height=.92, fixedsize=true];");
            sb.AppendLine("edge[style=bold, color=black, penwidth=2];");

            List<List<int>> coloring = null;
            if (properColor)
                coloring = g.FindChiColoring();

            foreach (int v in g.Vertices)
            {
                int colorIndex = 0;
                var label = "";

                if (properColor)
                {
                    var ii = coloring.FindIndex(I => I.Contains(v));
                    colorIndex = ColoringMap[ii % ColoringMap.Count];
                }
                else
                {
                    if (directed)
                    {
                        label = g.InDegree(v).ToString();
                        colorIndex = g.InDegree(v);
                    }
                    else
                    {
                        if (g.VertexWeight == null)
                        {
                            label = "";
                            colorIndex = 2;
                        }
                        else if (isLowPlus)
                        {
                            label = g.VertexWeight[v].ToString();
                            colorIndex = g.VertexWeight[v] + 2;
                        }
                        else if (inDegreeTerms)
                        {
                            var dd = g.VertexWeight[v] - g.Degree(v);

                            if (dd == 0)
                            {
                                label = "d";
                            }
                            else
                            {
                                label = "d+";
                            }
                            //label = "d";
                            //if (dd > 0)
                            //    label = "+" + dd;


                            //  colorIndex = dd + 2;
                            colorIndex = DotColors.Count + 1;
                        }
                        else
                        {
                            label = g.VertexWeight[v].ToString();
                            //colorIndex = g.VertexWeight[v];
                            colorIndex = DotColors.Count + 1;
                        }
                    }

                    colorIndex -= 2;
                    if (colorIndex < 0)
                        colorIndex += 13;
                }

                sb.AppendLine(string.Format(@"{0} [label = ""{2}"", style = filled, fillcolor = ""{1}""];", v, DotColors[colorIndex % DotColors.Count], label));
            }

            for (int i = 0; i < g.N; i++)
                for (int j = i + 1; j < g.N; j++)
                {
                    if (g[i, j])
                    {
                        if (directed)
                        {
                            if (g.Directed[i,j])
                                sb.AppendLine(string.Format("{0} -> {1}", i, j));
                            else
                                sb.AppendLine(string.Format("{1} -> {0}", i, j));
                        }
                        else
                            sb.AppendLine(string.Format("{0} -- {1}", i, j));
                    }
                }

            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
