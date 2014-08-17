using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Graphs
{
    public static class TeXConverter
    {
        public static string ToTikz(GraphCanvas canvas)
        {
            var scale = 10.0;

            var graph = canvas.Graph;

            var noLabels = graph.Vertices.All(v => string.IsNullOrEmpty(v.Label));

            var sb = new StringBuilder();
            sb.AppendLine("\\begin{tikzpicture}[scale = " + scale + "]");
            sb.AppendLine(string.Format(@"\tikzstyle{{VertexStyle}}=[shape = circle,	
								 minimum size = 6pt,
								 inner sep = 1.2pt,{0}
                                 draw]", noLabels ? "fill," : ""));

            var vertexNameMap = new Dictionary<Vertex, string>();

            int vertexCount = 0;
            foreach (var v in graph.Vertices)
            {
                string vertexName = "v" + vertexCount;

                vertexNameMap[v] = vertexName;

                double x = v.X;
                double y = 1.0 - v.Y;

                if (string.IsNullOrEmpty(v.Style))
                    sb.AppendLine(string.Format(@"\Vertex[x = {0}, y = {1}, L = \tiny {{{2}}}]{{{3}}}", x, y, Mathify(v.Label), vertexName));
                else
                    sb.AppendLine(string.Format(@"\Vertex[style = {{{4}}}, x = {0}, y = {1}, L = \tiny {{{2}}}]{{{3}}}", x, y, Mathify(v.Label), vertexName, v.Style));

                vertexCount++;
            }

            foreach (var e in graph.Edges)
            {
                if (string.IsNullOrEmpty(e.Style))
                    sb.AppendLine(string.Format(@"\Edge[]({0})({1})", vertexNameMap[e.V1], vertexNameMap[e.V2]));
                else
                    sb.AppendLine(string.Format(@"\Edge[style = {{{2}}}]({0})({1})", vertexNameMap[e.V1], vertexNameMap[e.V2], e.Style));
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
