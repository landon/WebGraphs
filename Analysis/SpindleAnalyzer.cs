using Choosability.Utility;
using Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static Graph BuildSerendipitousEdgeGraph(AlgorithmBlob blob, List<Vector> p, List<List<int>> diamonds)
        {
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
                            if (Math.Abs(rotatedDiamonds[a][i].Distance(rotatedDiamonds[b][j]) - r) < MinDelta)
                            {
                                edges.Add(new Edge(vertices[a], vertices[b]));
                            }
                        }
                    }
                }
            }

            return new Graph(vertices, edges);
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
