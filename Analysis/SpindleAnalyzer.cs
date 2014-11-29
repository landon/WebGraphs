using Choosability.Utility;
using Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Choosability;
using System.Collections;
using BitLevelGeneration;

namespace WebGraphs.Analysis
{
    public static class SpindleAnalyzer
    {
        const double MinDelta = 0.001;

        public enum DiamondType
        {
            U,
            D,
            UL,
            UR,
            DL,
            DR
        }

        public static void DoTiling(Graphs.Graph g, List<Vector> p, List<int> set)
        {
            if (g.Edges.Count <= 0)
                return;

            var r = new Vector(g.Edges[0].V1.X, g.Edges[0].V1.Y).Distance(new Vector(g.Edges[0].V2.X, g.Edges[0].V2.Y));

            var edgesToAdd = new List<Tuple<int, int>>();
            for (int i = 0; i < set.Count; i++)
            {
                g.Vertices[set[i]].Padding = 0.02f;
                for (int j = i + 1; j < set.Count; j++)
                {
                    if (p[set[i]].Distance(p[set[j]]) < 3 * r - MinDelta)
                    {
                        edgesToAdd.Add(new Tuple<int, int>(set[i], set[j]));
                    }
                }
            }

            foreach (var e in edgesToAdd)
            {
                var vs = new List<int>() {e.Item1, e.Item2};
                var nonIncident = edgesToAdd.Where(ea => !vs.Contains(ea.Item1) && !vs.Contains(ea.Item2)).ToList();

                if (nonIncident.Any(ea => SegmentsIntersect(g.Vertices[e.Item1], g.Vertices[e.Item2], g.Vertices[ea.Item1], g.Vertices[ea.Item2])))
                    continue;

                var between = g.Vertices.Where(v => OnLineSegment(g.Vertices[e.Item1], g.Vertices[e.Item2], v)).Where(v => v != g.Vertices[e.Item1] && v != g.Vertices[e.Item2]).ToList();

                var thickness = 6;
                if (between.Count == 0)
                {
                    g.AddEdge(g.Vertices[e.Item1], g.Vertices[e.Item2], Edge.Orientations.None, 1, thickness);
                }
                else
                {
                    for (int i = 0; i < between.Count; i++)
                    {
                        var ee = g.GetEdge(between[i], g.Vertices[e.Item1]);
                        if (ee != null)
                            ee.Thickness = thickness;

                        ee = g.GetEdge(between[i], g.Vertices[e.Item2]);
                        if (ee != null)
                            ee.Thickness = thickness;
                    }
                }
            }
        }

        static bool OnLineSegment(Vertex a1, Vertex a2, Vertex v)
        {
            return Math.Abs(CrossProduct(v, a1, a2)) < MinDelta && (a1.X <= v.X && v.X <= a2.X || a2.X <= v.X && v.X <= a1.X) && (a1.Y <= v.Y && v.Y <= a2.Y || a2.Y <= v.Y && v.Y <= a1.Y);
        }
     
        static double CrossProduct(Vertex v1, Vertex v2, Vertex v3)
        {
            return (v2.X - v1.X) * (v3.Y - v1.Y) - (v3.X - v1.X) * (v2.Y - v1.Y);
        }
        static bool SegmentsIntersect(Vertex a1, Vertex a2, Vertex b1, Vertex b2)
        {
            return CrossProduct(a1, a2, b1) * CrossProduct(a1, a2, b2) < 0 && CrossProduct(b1, b2, a1) * CrossProduct(b1, b2, a2) < 0;
        }

        public static Graphs.Graph BuildSerendipitousEdgeGraph(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds, out Graphs.Graph rotatedGraph)
        {
            var dim = GraphicsLayer.ARGB.FromFractional(0.5, 0.5, 0.5, 0.5);
            var transparent = GraphicsLayer.ARGB.FromFractional(0.0, 0.5, 0.5, 0.5);

            var vertices = new List<Vertex>();
            for (int i = 0; i < diamonds.Count; i++)
            {
                var v = new Vertex(diamonds[i].Select(t => p[t].X).Average(), diamonds[i].Select(t => p[t].Y).Average());
                v.Label = Enum.GetName(typeof(DiamondType), ClassifyDiamond(p, diamonds[i]));
                vertices.Add(v);
            }

            var rotatedDiamonds = diamonds.Select(d => RotateCoordinates(p, d)).ToList();
            var edges = new List<Edge>();

            var r = p[diamonds[0][0]].Distance(p[diamonds[0][2]]);

            var rotatedVertices = new List<Vertex>();
            var rotatedEdges = new List<Edge>();
            
            var rotatedVertexLookup = new Vertex[diamonds.Count, 4];
            for (int a = 0; a < diamonds.Count; a++)
            {
                for (int i = 0; i < 4; i++)
                {
                    var v = new Vertex(rotatedDiamonds[a][i].X, rotatedDiamonds[a][i].Y);
                    if (i == 0)
                        v.Padding = 0.02f;

                    rotatedVertices.Add(v);
                    rotatedVertexLookup[a, i] = v;
                }

                var originals = new Vertex[4];
                for (int i = 1; i < 4; i++)
                {
                    var v = new Vertex(p[diamonds[a][i]].X, p[diamonds[a][i]].Y);
                    v.Color = transparent;
                    v.Padding = 0.02f;

                    rotatedVertices.Add(v);
                    originals[i] = v;
                }

                rotatedEdges.Add(new Edge(originals[2], originals[3]) { Color = dim });
                rotatedEdges.Add(new Edge(originals[1], originals[2]) { Color = dim });
                rotatedEdges.Add(new Edge(originals[1], originals[3]) { Color = dim });

                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 0], originals[2]) { Color = dim });
                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 0], originals[3]) { Color = dim });

                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 0], rotatedVertexLookup[a, 2]));
                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 0], rotatedVertexLookup[a, 3]));
                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 1], rotatedVertexLookup[a, 2]));
                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 1], rotatedVertexLookup[a, 3]));
                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 2], rotatedVertexLookup[a, 3]));
                rotatedEdges.Add(new Edge(originals[1], rotatedVertexLookup[a, 1]));
            }
            

            for(int a = 0; a < diamonds.Count; a++)
            {
                for(int b = a + 1; b < diamonds.Count; b++)
                {
                    if (ClassifyDiamond(p, diamonds[a]) == ClassifyDiamond(p, diamonds[b]))
                        continue;

                    for (int i = 1; i < 4; i++)
                    {
                        for (int j = 1; j < 4; j++)
                        {
                            var distance = rotatedDiamonds[a][i].Distance(rotatedDiamonds[b][j]);
                            var offset = Math.Abs(distance - r);
                            if (offset < MinDelta)
                            {
                                var e = new Edge(vertices[a], vertices[b]);
                                e.Thickness = 6;
                                edges.Add(e);

                                var ee = new Edge(rotatedVertexLookup[a, i], rotatedVertexLookup[b, j]);
                                ee.Thickness = 6;
                                rotatedEdges.Add(ee);
                            }
                        }
                    }
                }
            }

            rotatedGraph = new Graphs.Graph(rotatedVertices, rotatedEdges);
            return new Graphs.Graph(vertices, edges);
        }

        public static double ComputeTotalWeight(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds, List<double> w)
        {
            return blob.AlgorithmGraph.Vertices.Sum(v => w[v]) + 3 * diamonds.Count;
        }

        public static double ComputeBestWeight(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds, List<double> w, out List<int> heaviestIndependentSet)
        {
            heaviestIndependentSet = new List<int>();
            var maxWeight = 0.0;

            var DU = diamonds.Where(d => ClassifyDiamond(p, d) == DiamondType.U).ToList();
            var HU = BuildSingleDirectionBitGraph(p, DU);

            var DDL = diamonds.Where(d => ClassifyDiamond(p, d) == DiamondType.DL).ToList();
            var HDL = BuildSingleDirectionBitGraph(p, DDL);

            var DDR = diamonds.Where(d => ClassifyDiamond(p, d) == DiamondType.DR).ToList();
            var HDR = BuildSingleDirectionBitGraph(p, DDR);

            foreach (var X in blob.AlgorithmGraph.EnumerateMaximalIndependentSets())
            {
                var weight = X.Sum(v => w[v]) + diamonds.Count;
                if (weight <= maxWeight) continue;
                weight -= ComputeLostSpindles(X, HU, DU);
                if (weight <= maxWeight) continue;
                weight -= ComputeLostSpindles(X, HDL, DDL);
                if (weight <= maxWeight) continue;
                weight -= ComputeLostSpindles(X, HDR, DDR);

                if (weight > maxWeight)
                {
                    maxWeight = weight;
                    heaviestIndependentSet = X;
                }
            }

            return maxWeight;
        }

        public static string ComputeTotalWeightFormula(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds, List<string> w)
        {
            var spindle = 3 * diamonds.Count;
            return string.Join(" + ", blob.AlgorithmGraph.Vertices.GroupBy(v => w[v]).OrderBy(x => x.Key).Select(x => (x.Count() > 1 ? x.Count().ToString() : "") + x.Key)) + " + " + spindle + "s";
        }

        static string GeneratePrettyConstraint(List<int> X, List<string> w, int diamondCount, BitGraph_long HU, BitGraph_long HDL, BitGraph_long HDR, List<List<int>> DU, List<List<int>> DDL, List<List<int>> DDR)
        {
            var spindleCount = ComputeSpindleCount(X, diamondCount, HU, HDL, HDR, DU, DDL, DDR);
            return string.Join(" + ", X.GroupBy(v => w[v]).OrderBy(x => x.Key).Select(x => (x.Count() > 1 ? x.Count().ToString() : "") + x.Key)) + " + " + spindleCount + "s <= 1";
        }

        public static string GenerateGLPKCode(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds, List<string> w)
        {
            var spindle = 3 * diamonds.Count;
            var variables = w.Distinct().OrderBy(x => x).ToList();

            var DU = diamonds.Where(d => ClassifyDiamond(p, d) == DiamondType.U).ToList();
            var HU = BuildSingleDirectionBitGraph(p, DU);

            var DDL = diamonds.Where(d => ClassifyDiamond(p, d) == DiamondType.DL).ToList();
            var HDL = BuildSingleDirectionBitGraph(p, DDL);

            var DDR = diamonds.Where(d => ClassifyDiamond(p, d) == DiamondType.DR).ToList();
            var HDR = BuildSingleDirectionBitGraph(p, DDR);

            var sb = new StringBuilder();
            sb.AppendLine("maximize");
            sb.AppendLine(string.Join(" + ", blob.AlgorithmGraph.Vertices.GroupBy(v => w[v]).OrderBy(x => x.Key).Select(x => (x.Count() > 1 ? x.Count().ToString() : "") + x.Key)) + " + " + spindle + "s");

            sb.AppendLine();
            sb.AppendLine("subject to");
            sb.AppendLine(string.Join(Environment.NewLine, blob.AlgorithmGraph.EnumerateMaximalIndependentSets().Select(X => GeneratePrettyConstraint(X, w, diamonds.Count, HU, HDL, HDR, DU, DDL, DDR)).Distinct()));

            sb.AppendLine();
            sb.AppendLine("bounds");
            foreach (var v in variables)
                sb.AppendLine(v + " > 0");

            sb.AppendLine("s > 0");

            sb.AppendLine();
            sb.AppendLine("generals");
            foreach (var v in variables)
                sb.AppendLine(v);
            sb.AppendLine("s");

            sb.AppendLine();
            sb.AppendLine("end");
            return sb.ToString();
        }

        static int CountOccurrences(string x, List<string> w, List<int> subset)
        {
            return subset.Count(i => x == w[i]);
        }

        private static int ComputeSpindleCount(List<int> X, int diamondCount, BitGraph_long HU, BitGraph_long HDL, BitGraph_long HDR, List<List<int>> DU, List<List<int>> DDL, List<List<int>> DDR)
        {
            var lostSpindlesU = ComputeLostSpindles(X, HU, DU);
            var lostSpindlesDL = ComputeLostSpindles(X, HDL, DDL);
            var lostSpindlesDR = ComputeLostSpindles(X, HDR, DDR);

            return diamondCount - lostSpindlesU - lostSpindlesDL - lostSpindlesDR;
        }

        public static int ComputeLostSpindles(List<int> independentSet, BitGraph_long H, List<List<int>> directionDiamonds)
        {
            var all = (1L << H.N) - 1;
            var bases = directionDiamonds.IndicesWhere(d => independentSet.Contains(d[0])).ToInt64();
            var tops = directionDiamonds.IndicesWhere(d => independentSet.Contains(d[1])).ToInt64();

            var red = all ^ tops;
            var blueGreen = all ^ bases;

            var minimumMissed = H.N;
            foreach (var R in H.MaximalIndependentSubsets(red))
            {
                var Vb = blueGreen & ~R;

                foreach (var B in H.MaximalIndependentSubsets(Vb))
                {
                    var Vg = Vb & ~B;

                    var missed = H.MaximalIndependentSubsets(Vg).Min(G => BitUsage_long.PopulationCount(all ^ (G | R | B)));
                    if (missed < minimumMissed)
                        minimumMissed = missed;
                }
            }

            return minimumMissed;
        }

        static Choosability.Graph BuildSingleDirectionGraph(List<Vector> p, List<List<int>> diamonds)
        {
            var a = new bool[diamonds.Count, diamonds.Count];
            var r = p[diamonds[0][0]].Distance(p[diamonds[0][2]]);

            for (int v = 0; v < diamonds.Count; v++)
            {
                for (int w = v + 1; w < diamonds.Count; w++)
                {
                    var distance = p[diamonds[v][0]].Distance(p[diamonds[w][0]]);
                    var offset = Math.Abs(distance - r);
                    if (offset < MinDelta)
                    {
                        a[v, w] = true;
                        a[w, v] = true;
                    }
                }
            }

            return new Choosability.Graph(a);
        }

        static BitGraph_long BuildSingleDirectionBitGraph(List<Vector> p, List<List<int>> diamonds)
        {
            var a = new bool[diamonds.Count, diamonds.Count];
            var r = p[diamonds[0][0]].Distance(p[diamonds[0][2]]);

            var neighborhood = new long[diamonds.Count];
            for (int i = 0; i < diamonds.Count; i++)
            {
                var iBit = 1L << i;
                for (int j = i + 1; j < diamonds.Count; j++)
                {
                    var jBit = 1L << j;

                    var distance = p[diamonds[i][0]].Distance(p[diamonds[j][0]]);
                    var offset = Math.Abs(distance - r);
                    if (offset < MinDelta)
                    {
                        neighborhood[i] |= jBit;
                        neighborhood[j] |= iBit;
                    }
                }
            }

            return new BitGraph_long(diamonds.Count, neighborhood);
        }

        public static List<string> FindSerendipitousEdges(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds, out List<string> identifications)
        {
            var r = p[diamonds[0][0]].Distance(p[diamonds[0][2]]);
            var c = new int[8];

            var rotatedDiamonds = new Dictionary<string, List<Vector>>();
            foreach (var diamond in diamonds)
            {
                var t = ClassifyDiamond(p, diamond);
                var i = c[(int)t]++;

                rotatedDiamonds[Enum.GetName(typeof(DiamondType), t) + "_" + i] = RotateCoordinates(p, diamond);
            }

            var edges = new List<string>();
            identifications = new List<string>();
            foreach (var a in rotatedDiamonds)
            {
                foreach (var b in rotatedDiamonds)
                {
                    if (a.Key.Substring(0, a.Key.IndexOf("_")) == b.Key.Substring(0, b.Key.IndexOf("_")))
                        continue;

                    for (int i = 1; i < 4; i++)
                    {
                        for (int j = 1; j < 4; j++)
                        {
                            if (Math.Abs(a.Value[i].Distance(b.Value[j]) - r) < MinDelta)
                            {
                                var s = a.Key + "_" + GetKind(i) + " <--> " + b.Key + "_" + GetKind(j);
                                edges.Add(s);
                            }
                            else if (a.Value[i].Distance(b.Value[j]) < MinDelta)
                            {
                                var s = a.Key + "_" + GetKind(i) + " == " + b.Key + "_" + GetKind(j);
                                identifications.Add(s);
                            }
                        }
                    }
                }
            }

            return edges.Distinct((x, y) => x.Split(new[] { " <--> " }, StringSplitOptions.RemoveEmptyEntries).SequenceEqual(y.Split(new[] { " <--> " }, StringSplitOptions.RemoveEmptyEntries).Reverse())).ToList();
        }

        public static List<List<int>> FindDiamonds(AlgorithmBlob blob, List<Vector> p)
        {
            var diamonds = new List<List<int>>();

            var d = Choosability.Graphs.Diamond;
            
            foreach (var X in blob.AlgorithmGraph.Vertices.EnumerateSublists(4))
            {
                if (blob.AlgorithmGraph.InducedSubgraph(X).ContainsInduced(d))
                {
                    var Y = X.OrderBy(x => p[x].X).OrderBy(x => blob.AlgorithmGraph.DegreeInSubgraph(x, X)).ToList();

                    var type = ClassifyDiamond(p, Y);
                    if (type != DiamondType.U && type != DiamondType.DR && type != DiamondType.DL)
                    {
                        var temp = Y[0];
                        Y[0] = Y[1];
                        Y[1] = temp;
                    }

                    diamonds.Add(Y);
                }
            }

            return diamonds;
        }

        public static List<List<int>> FindAllDiamonds(AlgorithmBlob blob, List<Vector> p)
        {
            var diamonds = new List<List<int>>();

            var d = Choosability.Graphs.Diamond;

            foreach (var X in blob.AlgorithmGraph.Vertices.EnumerateSublists(4))
            {
                if (blob.AlgorithmGraph.InducedSubgraph(X).ContainsInduced(d))
                {
                    var Y = X.OrderBy(x => blob.AlgorithmGraph.DegreeInSubgraph(x, X)).ToList();

                    diamonds.Add(Y);

                    var Y2 = Y.ToList();
                    var temp = Y2[0];
                    Y2[0] = Y[1];
                    Y2[1] = temp;

                    diamonds.Add(Y2);
                }
            }

            return diamonds;
        }

        static string GetKind(int k)
        {
            switch (k)
            {
                case 1:
                    return "top";
                case 2:
                    return "bottom1";
                case 3:
                    return "bottom2";
            }

            return "???";
        }

        public static List<Vector> RotateCoordinates(List<Vector> p, List<int> diamond)
        {
            var type = ClassifyDiamond(p, diamond);
            if (type == DiamondType.U || type == DiamondType.DR || type == DiamondType.DL)
                return diamond.Select(v => Rotate(p[diamond[0]], p[v])).ToList();
            else
                return diamond.Select(v => Rotate(p[diamond[0]], p[v], clockwise:true)).ToList();
        }

        static Vector Rotate(Vector center, Vector v, bool clockwise = false)
        {
            double cos = 5.0 / 6.0;
            double sin = Math.Sqrt(11) / 6.0;
            if (!clockwise)
                sin = -sin;

            var x = center.X + (v.X - center.X) * cos - (v.Y - center.Y) * sin;
            var y = center.Y + (v.X - center.X) * sin + (v.Y - center.Y) * cos;

            return new Vector(x, y);
        }

        static DiamondType ClassifyDiamond(List<Vector> p, List<int> diamond)
        {
            var b = p[diamond[0]];
            var t = p[diamond[1]];

            if (Equalish(b.X, t.X))
            {
                if (b.Y > t.Y)
                    return DiamondType.U;
                return DiamondType.D;
            }
            else if (b.X < t.X)
            {
                if (b.Y > t.Y)
                    return DiamondType.UR;
                return DiamondType.DR;
            }
            else
            {
                if (b.Y > t.Y)
                    return DiamondType.UL;
                return DiamondType.DL;
            }
        }

        static bool Equalish(double a, double b)
        {
            return Math.Abs(a - b) < MinDelta;
        }
    }
}
