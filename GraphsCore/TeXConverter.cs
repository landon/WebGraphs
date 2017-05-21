using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Graphs
{
    public static class TeXConverter
    {
        static readonly Regex VertexRegex = new Regex(@"\\Vertex\[[^x]*x\s*=\s*([-+]?\d*\.?\d+)\s*,\s*y\s*=\s*([-+]?\d*\.?\d+)\s*,\s*L\s*=\s*[^\{]*\{([^\}]*)\}\s*]\{([^\}]*)\}");
        static readonly Regex EdgeRegex = new Regex(@"\\Edge\[\s*([^\]]*)\s*]\(([^\)]+)\)\(([^\)]+)\)");

        public static Graph FromTikz(string tikz)
        {
            var idToVertex = new Dictionary<string, Vertex>();

            foreach (Match match in VertexRegex.Matches(tikz))
            {
                if (match.Groups.Count != 5)
                    return null;

                double x;
                if (!double.TryParse(match.Groups[1].Value, out x))
                    return null;
                double y;
                if (!double.TryParse(match.Groups[2].Value, out y))
                    return null;
                var label = match.Groups[3].Value ?? "";
                var id = match.Groups[4].Value;
                if (string.IsNullOrEmpty(id))
                    return null;

                idToVertex[id] = new Vertex(x, 1.0 - y, label.Trim('$'));
            }

            var edges = new List<Edge>();
            foreach (Match match in EdgeRegex.Matches(tikz))
            {
                if (match.Groups.Count != 4)
                    return null;

                var style = match.Groups[1].Value ?? "";
                var id1 = match.Groups[2].Value;
                var id2 = match.Groups[3].Value;

                if (string.IsNullOrEmpty(id1) || string.IsNullOrEmpty(id2))
                    return null;

                var orientation = Edge.Orientations.None;
                if (style.Contains("pre"))
                    orientation = Edge.Orientations.Backward;
                else if (style.Contains("post"))
                    orientation = Edge.Orientations.Forward;


                edges.Add(new Edge(idToVertex[id1], idToVertex[id2], orientation));
            }

            return new Graph(idToVertex.Values, edges);
        }

        public static string ToTikz(Graph graph)
        {
            var scale = 10.0;

            var noLabels = graph.Vertices.All(v => string.IsNullOrEmpty(v.Label));

            var sb = new StringBuilder();
            sb.AppendLine("\\begin{tikzpicture}[scale = " + scale + "]");
            sb.AppendLine(@"\tikzstyle{VertexStyle} = []");
            sb.AppendLine(@"\tikzstyle{EdgeStyle} = [line width=3pt]");
            sb.AppendLine(string.Format(@"\tikzstyle{{labeledStyle}}=[shape = circle, minimum size = 6pt, inner sep = 5pt, outer sep = 5pt, draw]"));
            sb.AppendLine(string.Format(@"\tikzstyle{{unlabeledStyle}}=[shape = circle, minimum size = 6pt, inner sep = 5pt, outer sep = 5pt, draw, fill]"));

            var vertexStyleLookup = new Dictionary<string, string>();
            var edgeStyleLookup = new Dictionary<string, string>();
            
            var vertexStyles = graph.Vertices.Where(v => !string.IsNullOrWhiteSpace(v.Style)).Select(v => v.Style.Trim()).Distinct().ToList();
            var edgeStyles = graph.Edges.Where(e => !string.IsNullOrWhiteSpace(e.Style)).Select(e => e.Style.Trim()).Distinct().ToList();

            foreach (var style in vertexStyles)
            {
                string id = style;
                if (!style.StartsWith("_"))
                {
                    id = Guid.NewGuid().ToString().ToLower();
                    var s = @"\tikzstyle{" + id + @"}=[";
                    if (!style.Contains("shape"))
                        s += "shape = circle,";
                    if (!style.Contains("minimum size"))
                        s += "minimum size = 6pt,";
                    if (!style.Contains("inner sep"))
                        s += "inner sep = 5pt,";
                    if (!style.Contains("outer sep"))
                        s += "outer sep = 5pt,";

                    s += "draw,";
                    s += style;
                    s = s.TrimEnd(',') + "]";

                    sb.AppendLine(s);
                }

                vertexStyleLookup[style] = id;
            }

            foreach (var style in edgeStyles)
            {
                string id = style;
                if (!style.StartsWith("_"))
                {
                    id = Guid.NewGuid().ToString().ToLower();
                    var s = @"\tikzstyle{" + id + @"}=[";
                    s += style;
                    s = s.TrimEnd(',') + "]";

                    sb.AppendLine(s);
                }

                edgeStyleLookup[style] = id;
            }

            var vertexNameMap = new Dictionary<Vertex, string>();

            int vertexCount = 0;
            foreach (var v in graph.Vertices)
            {
                string vertexName = "v" + vertexCount;

                vertexNameMap[v] = vertexName;

                double x = v.X;
                double y = 1.0 - v.Y;

                if (string.IsNullOrEmpty(v.Style))
                    sb.AppendLine(string.Format(@"\Vertex[style = {4}, x = {0:0.000}, y = {1:0.000}, L = \tiny {{{2}}}]{{{3}}}", x, y, Mathify(v.Label), vertexName, string.IsNullOrEmpty(v.Label) ? "unlabeledStyle" : "labeledStyle"));
                else
                    sb.AppendLine(string.Format(@"\Vertex[style = {4}, x = {0:0.000}, y = {1:0.000}, L = \tiny {{{2}}}]{{{3}}}", x, y, Mathify(v.Label), vertexName, vertexStyleLookup[v.Style.Trim()]));

                vertexCount++;
            }

            foreach (var e in graph.Edges)
            {
                if (string.IsNullOrEmpty(e.Style))
                    sb.AppendLine(string.Format(@"\Edge[label = \tiny {{{2}}}, labelstyle={{auto=right, fill=none}}]({0})({1})", vertexNameMap[e.V1], vertexNameMap[e.V2], Mathify(e.Label)));
                else
                    sb.AppendLine(string.Format(@"\Edge[style = {2}, label = \tiny {{{3}}}, labelstyle={{auto=right, fill=none}}]({0})({1})", vertexNameMap[e.V1], vertexNameMap[e.V2], edgeStyleLookup[e.Style.Trim()], Mathify(e.Label)));
            }

            sb.AppendLine(@"\end{tikzpicture}");

            return sb.ToString();
        }

        static string Mathify(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                return p;

            if (!p.StartsWith("$"))
                p = "$" + p;
            if (!p.EndsWith("$"))
                p = p + "$";

            return p;
        }
    }
}
