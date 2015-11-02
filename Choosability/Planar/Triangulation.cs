using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.Planar
{
    public static class Triangulation
    {
        public static Graph Extend(Graph g)
        {
            var h = g.Clone();

            foreach (var v in h.Vertices)
            {
                if (IsWheel(h, v))
                {
                    h.VertexWeight[v] = h.Degree(v);
                }
                else if (IsGem(h, v))
                {
                    var add = h.VertexWeight[v] - h.Degree(v);
                    if (add >= 0)
                    {
                        var ends = GemEnds(h, v);
                        if (add == 0)
                        {
                            h = h.AddEdge(ends[0], ends[1]);
                        }
                        else
                        {
                            h = h.AttachNewVertex(ends[0], v);
                            add--;
                            while (add >= 1)
                            {
                                h = h.AttachNewVertex(h.N - 1, v);
                                add--;
                            }

                            h = h.AddEdge(h.N - 1, ends[1]);
                        }

                        h.VertexWeight[v] = h.Degree(v);
                    }
                    else
                    {
                        h.VertexWeight[v] = 0;
                    }
                }
                else
                {
                    return null;
                }
            }

            return h;
        }

        static bool IsGem(Graph h, int v)
        {
            var degrees = h.Neighbors[v].Select(w => h.DegreeInSubgraph(w, h.Neighbors[v])).ToList();
            return degrees.Sum() == 2 * (h.Degree(v) - 1) && degrees.Max() <= 2;
        }

        static bool IsWheel(Graph h, int v)
        {
            var degrees = h.Neighbors[v].Select(w => h.DegreeInSubgraph(w, h.Neighbors[v])).ToList();
            return h.IsConnected(h.Neighbors[v]) && degrees.All(d => d == 2);
        }

        static List<int> GemEnds(Graph h, int v)
        {
            return h.Neighbors[v].Where(w => h.DegreeInSubgraph(w, h.Neighbors[v]) == 1).ToList();
        }
    }
}
