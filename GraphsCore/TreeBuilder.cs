using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Choosability;
using Choosability.DataStructures;

namespace GraphsCore
{
    public class TreeBuilder
    {
        const double Scale = 1000.0;

        public Graphs.Graph BuildWinTree(Graphs.Graph g, SuperSlimMind mind, SuperSlimBoard board, Bijection<int, string> numbering)
        {
            var tree = mind.BuildGameTree(board, true);
            var gg = g.Clone();

            foreach (var v in gg.Vertices)
            {
                v.X = v.X / Scale;
                v.Y = v.Y / Scale;
            }

            var bounds = new Bounds(gg);
            var potSize = board.Stacks.Value.SelectMany(l => l.ToSet()).Distinct().Count();
            var treeG = BuildWinTree(tree, g, mind, bounds, 0, numbering, new Choosability.Utility.Permutation(Enumerable.Range(0, potSize).ToList()), null);

            return treeG;
        }

        Graphs.Graph BuildWinTree(GameTree tree, Graphs.Graph g, SuperSlimMind mind, Bounds original, int level, Bijection<int, string> numbering, Choosability.Utility.Permutation pp, List<string> lastListStrings)
        {
            var clone = g.Clone();
            var lists = tree.Board.Stacks.Value.Select(s => s.ToSet()).ToList();

            var ppp = pp;
            if (tree.Parent != null)
            {
                var potSize = lists.SelectMany(l => l).Distinct().Count();
                var bestScore = int.MinValue;

                foreach (var p in Choosability.Utility.Permutation.EnumerateAll(potSize))
                {
                    var strings = lists.Select(ll => string.Join(",", ll.Select(n => numbering[p[n]]).OrderBy(s => s))).ToList();
                    
                    int score = 0;
                    for (int ii = 0; ii < strings.Count; ii++)
                    {
                        if (strings[ii] == lastListStrings[ii])
                            score += strings[ii].Length;
                    }

                    if (score > bestScore)
                    {
                        bestScore = score;
                        ppp = p;
                    }
                }

                lists = lists.Select(l => l.Select(vv => ppp[vv]).ToList()).ToList();
            }

            var listStrings = lists.Select(ll => string.Join(",", ll.Select(n => numbering[n]).OrderBy(s => s))).ToList();

            if (tree.IsColorable)
            {
                Dictionary<int, long> coloring;
                mind.ColoringAnalyzer.Analyze(tree.Board, out coloring);

                for (int jj = 0; jj < clone.Edges.Count; jj++)
                {
                    var v1 = mind._edges[jj].Item1;
                    var v2 = mind._edges[jj].Item2;

                    var c = coloring[jj].LeastSignificantBit();
                    if (ppp != null)
                        c = ppp[c];

                    if (!lists[v1].Contains(c))
                        System.Diagnostics.Debugger.Break();
                    if (!lists[v2].Contains(c))
                        System.Diagnostics.Debugger.Break();

                    lists[v1].Remove(c);
                    lists[v2].Remove(c);

                    var e = clone.Edges.First(ee => Choosability.Utility.ListUtility.Equal(new List<int>() { clone.Vertices.IndexOf(ee.V1), clone.Vertices.IndexOf(ee.V2) }, new List<int>() {v1,v2}));
                    e.Label = numbering[c];
                }
            }
            else
            {
                foreach (var e in clone.Edges)
                {
                    var v1 = clone.Vertices.IndexOf(e.V1);
                    var v2 = clone.Vertices.IndexOf(e.V2);

                    if (!string.IsNullOrWhiteSpace(e.Label))
                    {
                        var c = numbering[e.Label];
                        if (lists[v1].Contains(c) && lists[v2].Contains(c))
                        {
                            lists[v1].Remove(c);
                            lists[v2].Remove(c);
                        }
                        else
                        {
                            e.Label = "";
                        }
                    }
                }

                foreach (var e in clone.Edges)
                {
                    if (!string.IsNullOrWhiteSpace(e.Label) || string.IsNullOrWhiteSpace(g.Edges[clone.Edges.IndexOf(e)].Label))
                        continue;

                    var v1 = clone.Vertices.IndexOf(e.V1);
                    var v2 = clone.Vertices.IndexOf(e.V2);

                    var common = Choosability.Utility.ListUtility.Intersection(lists[v1], lists[v2]);
                    if (common.Count > 0)
                    {
                        e.Label = numbering[common[0]];
                        lists[v1].Remove(common[0]);
                        lists[v2].Remove(common[0]);
                    }
                }
            }

            for (int q = 0; q < lists.Count; q++)
            {
                clone.Vertices[q].Label = string.Join(",", numbering.Apply(lists[q].OrderBy(x => x)));
            }

            clone.Translate(new Graphs.Vector(-original.Left, -original.Top));

            if (tree.Children.Count <= 0)
            {
                return clone;
            }

            var children = tree.Children.Distinct().ToList();

            var childGraphs = children.Select(child => BuildWinTree(child, g, mind, original, level++, numbering, ppp, listStrings)).ToList();
            var childBounds = childGraphs.Select(cg => new Bounds(cg)).ToList();

            var min = Math.Max(original.Width * Scale, original.Height * Scale);

            var W = Math.Max(min, childBounds.Max(cb => cb.Width));
            var H = Math.Max(min, childBounds.Max(cb => cb.Height));

            var uberGraph = new Graphs.Graph();
            uberGraph.DisjointUnion(clone);

            int i = 0;
            double xoff = 0.0;
            foreach (var cg in childGraphs)
            {
                var gg = cg;
                gg.Translate(new Graphs.Vector(xoff, 2 * min));

                uberGraph.DisjointUnion(gg);
                xoff += childBounds[i].Width + min;

                var a = children[i].Info == null ? "" : numbering[ppp[children[i].Info.Alpha]];
                var b = children[i].Info == null ? "" : numbering[ppp[children[i].Info.Beta]];

                var maxYY = clone.Vertices.Max(vvv => vvv.Y);
                var pvs = clone.Vertices.Where(vvv => vvv.Y == maxYY).ToList();

                var avgXX = pvs.Sum(vvv => vvv.X) / pvs.Count;
                var daXX = pvs.Min(vvv => Math.Abs(vvv.X - avgXX));
                var pv = pvs.FirstOrDefault(vvv => Math.Abs(vvv.X - avgXX) == daXX);
                if (pv == null)
                    pv = pvs.First();

                var maxYY2 = gg.Vertices.Min(vvv => vvv.Y);
                var pvs2 = gg.Vertices.Where(vvv => vvv.Y == maxYY2).ToList();
                var avgXX2 = pvs2.Sum(vvv => vvv.X) / pvs2.Count;
                var daXX2 = pvs2.Min(vvv => Math.Abs(vvv.X - avgXX2));
                var pv2 = pvs2.FirstOrDefault(vvv => Math.Abs(vvv.X - avgXX2) == daXX2);
                if (pv2 == null)
                    pv2 = pvs2.First();

                uberGraph.AddEdge(pv, pv2, Graphs.Edge.Orientations.Forward, 1, 1, "blue", a + " - " + b);

                foreach (var vi in children[i].Info.SwapVertices)
                {
                    gg.Vertices[vi].Color = new GraphicsLayer.ARGB(120, 0, 255, 0);
                    gg.Vertices[vi].Style = "green";
                }

                i++;
            }

            clone.Translate(new Graphs.Vector((xoff / 2) - min - original.Width / 2, 0));

            return uberGraph;
        }
    }


    class Bounds
    {
        public double Left;
        public double Right;
        public double Top;
        public double Bottom;
        public double Width;
        public double Height;

        public Bounds(Graphs.Graph g)
        {
            Left = double.MaxValue;
            Right = double.MinValue;
            Top = double.MaxValue;
            Bottom = double.MinValue;

            foreach (var v in g.Vertices)
            {
                Left = Math.Min(Left, v.X);
                Right = Math.Max(Right, v.X);
                Top = Math.Min(Top, v.Y);
                Bottom = Math.Max(Bottom, v.Y);
            }

            Width = Right - Left;
            Height = Bottom - Top;
        }
    }
}
