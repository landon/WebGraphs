using Choosability.Utility;
using Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Choosability;

namespace WebGraphs.Analysis
{
    public static class SpindleAnalyzer
    {
        const double MinDelta = 0.001;

        public enum DiamondType
        {
            U,
            D,
            L,
            R,
            UL,
            UR,
            DL,
            DR
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

                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 0], originals[2]) { Color = dim });
                rotatedEdges.Add(new Edge(rotatedVertexLookup[a, 0], originals[3]) { Color = dim });
                rotatedEdges.Add(new Edge(originals[2], originals[3]) { Color = dim });
                rotatedEdges.Add(new Edge(originals[1], originals[2]) { Color = dim });
                rotatedEdges.Add(new Edge(originals[1], originals[3]) { Color = dim });

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

            foreach (var X in blob.AlgorithmGraph.EnumerateMaximalIndependentSets())
            {
                var weight = X.Sum(v => w[v]) + diamonds.Count;
                if (weight <= maxWeight) continue;
                weight -= ComputeLostSpindles(p, diamonds, X, DiamondType.U);
                if (weight <= maxWeight) continue;
                weight -= ComputeLostSpindles(p, diamonds, X, DiamondType.DL);
                if (weight <= maxWeight) continue;
                weight -= ComputeLostSpindles(p, diamonds, X, DiamondType.DR);

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

        static string GeneratePrettyConstraint(List<int> X, List<Vector> p, List<List<int>> diamonds, List<string> w)
        {
            var spindleCount = ComputeSpindleCount(X, p, diamonds);
            return string.Join(" + ", X.GroupBy(v => w[v]).OrderBy(x => x.Key).Select(x => (x.Count() > 1 ? x.Count().ToString() : "") + x.Key)) + " + " + spindleCount + "s <= 1";
        }

        public static string GenerateGLPKCode(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds, List<string> w)
        {
            var spindle = 3 * diamonds.Count;
            var variables = w.Distinct().OrderBy(x => x).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("maximize");
            sb.AppendLine(string.Join(" + ", blob.AlgorithmGraph.Vertices.GroupBy(v => w[v]).OrderBy(x => x.Key).Select(x => (x.Count() > 1 ? x.Count().ToString() : "") + x.Key)) + " + " + spindle + "s");

            sb.AppendLine();
            sb.AppendLine("subject to");
            sb.AppendLine(string.Join(Environment.NewLine, blob.AlgorithmGraph.EnumerateMaximalIndependentSets().Select(X => GeneratePrettyConstraint(X, p, diamonds, w)).Distinct()));

            sb.AppendLine();
            sb.AppendLine("bounds");
            foreach (var v in variables)
                sb.AppendLine(v + " >= 0");

            sb.AppendLine("s >= 0");

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

        private static int ComputeSpindleCount(List<int> X, List<Vector> p, List<List<int>> diamonds)
        {
            var lostSpindlesU = ComputeLostSpindles(p, diamonds, X, DiamondType.U);
            var lostSpindlesDL = ComputeLostSpindles(p, diamonds, X, DiamondType.DL);
            var lostSpindlesDR = ComputeLostSpindles(p, diamonds, X, DiamondType.DR);

            return diamonds.Count - lostSpindlesU - lostSpindlesDL - lostSpindlesDR;
        }
    
        public static int ComputeLostSpindles(List<Vector> p, List<List<int>> diamonds, List<int> independentSet, DiamondType direction)
        {
            var directionDiamonds = diamonds.Where(d => ClassifyDiamond(p, d) == direction).ToList();

            var bases = directionDiamonds.IndicesWhere(d => independentSet.Contains(d[0])).ToList();
            var tops = directionDiamonds.IndicesWhere(d => independentSet.Contains(d[1])).ToList();
            
            var H = BuildSingleDirectionGraph(p, directionDiamonds);
            var lists = Enumerable.Repeat(7L, H.N).ToList();

            for (int i = 0; i < lists.Count; i++)
            {
                if (bases.Contains(i))
                    lists[i] &= 1;
                if (tops.Contains(i))
                    lists[i] &= 6;
            }

            var vs = H.Vertices.Where(v => lists[v] != 0).ToList();
            
            //var best = 0;
            //for (int size = 1; size <= vs.Count; size++)
            //{
            //    foreach (var set in vs.EnumerateSublists(size))
            //    {
            //        if (H.IsChoosable(lists, set))
            //        {
            //            best = size;
            //            break;
            //        }
            //    }
            //}

            //return H.N - best;

            var low = 0;
            var high = H.N + 1;

            while (low <= high - 2)
            {
                var size = (low + high) / 2;

                var down = true;
                if (vs.Count >= size)
                {
                    foreach (var set in vs.EnumerateSublists(size))
                    {
                        if (H.IsChoosable(lists, set))
                        {
                            low = size;
                            down = false;
                            break;
                        }
                    }
                }

                if (down)
                    high = size;
            }

            return H.N - low;
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
                    }
                }
            }

            return new Choosability.Graph(a);
        }

        public static List<string> FindSerendipitousEdges(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds)
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
            return diamond.Select(v => Rotate(p[diamond[0]], p[v])).ToList();
        }

        static Vector Rotate(Vector center, Vector v)
        {
            double cos = 5.0 / 6.0;
            double sin = -Math.Sqrt(11) / 6.0;

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
                if (Equalish(b.Y, t.Y))
                    return DiamondType.R;
                else if (b.Y > t.Y)
                    return DiamondType.UR;
                return DiamondType.DR;
            }
            else
            {
                if (Equalish(b.Y, t.Y))
                    return DiamondType.L;
                else if (b.Y > t.Y)
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
