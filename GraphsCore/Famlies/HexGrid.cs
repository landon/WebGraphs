using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphsCore.Famlies
{
    public class HexGrid : IFamily
    {
        public string Name { get { return "hex grid"; } }

        public Graphs.Graph Create(double w, object data)
        {
            var n = (int)data;
            var r = 1.4 * w / n;

            var vertices = new List<Graphs.Vertex>();
            var edges = new List<Graphs.Edge>();
            List<Graphs.Vertex> previousLine = null;
            for (int i = 0; i < n; i++)
            {
                previousLine = AddLine(vertices, edges, previousLine, n, r);
            }

            return new Graphs.Graph(vertices, edges);
        }

        List<Graphs.Vertex> AddLine(List<Graphs.Vertex> vertices, List<Graphs.Edge> edges, List<Graphs.Vertex> previousLine, int n, double r)
        {
            var x = 0.15;
            var y = 0.05;

            if (previousLine != null)
            {
                x = previousLine[0].X + r / 2;
                y = previousLine[0].Y + r * Math.Sqrt(3) / 2.0;
            }

            var line = new List<Graphs.Vertex>();
            for (int j = 0; j < n; j++)
            {
                var v = new Graphs.Vertex(x + r * j, y);

                vertices.Add(v);
                line.Add(v);

                if (j > 0)
                {
                    edges.Add(new Graphs.Edge(v, line[j - 1]));
                }

                if (previousLine != null)
                {
                    edges.Add(new Graphs.Edge(v, previousLine[j]));
                    if (j + 1 < n)
                        edges.Add(new Graphs.Edge(v, previousLine[j + 1]));
                }
            }

            return line;
        }
    }
}
