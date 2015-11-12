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

        public static Graph ExtendOrdered(Graph g)
        {
            var h = g.Clone();
            var remainingVertices = g.Vertices.ToList();
            int w = -1;
            while (remainingVertices.Count > 0)
            {
                int v;
                if (w == -1)
                {
                    v = remainingVertices.First();
                }
                else
                {
                    v = h.Neighbors[w].Intersect(remainingVertices).FirstOrDefault();
                    if (!remainingVertices.Contains(v))
                        v = remainingVertices.First();
                }

                w = -1;
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
                        if (!h[ends[0], h.N - 1])
                        {
                            var t = ends[0];
                            ends[0] = ends[1];
                            ends[1] = t;
                        }

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

                        w = h.N - 1;
                    }
                    else
                    {
                        h.VertexWeight[v] = 0;
                        w = v;
                    }
                }
                else
                {
                    return null;
                }

                remainingVertices.Remove(v);
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
