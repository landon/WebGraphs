using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using Choosability;
using Choosability.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class FixerBreakerTrees
    {
        public static void Go()
        {
            var root = @"C:\game trees\C5\all";
            Directory.CreateDirectory(root);

            var g = Choosability.Graphs.C(5);
            var sizes = Enumerable.Repeat(3, 5).ToList();

            var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g);
            mind.MaxPot = int.MaxValue;
            mind.SuperabundantOnly = true;
            //mind.OnlyConsiderNearlyColorableBoards = true;

            int j = 0;
            var win = mind.Analyze(new Template(sizes));

            if (win)
            {
                foreach (var board in mind.NonColorableBoards)
                {
                    var tree = mind.BuildGameTree(board, win);
                    GraphViz.DrawTree(tree, root + @"\" + " depth " + tree.GetDepth() + " board " + j + ".png");
                    j++;
                }
            }
            else
            {
                foreach (var board in mind.BreakerWonBoards)
                {
                    var tree = mind.BuildGameTree(board, win);
                    GraphViz.DrawTree(tree, root + @"\" + " depth " + tree.GetDepth() + " board " + j + ".png");
                    j++;
                }
            }
        }

        public static void SimplifyProgol()
        {
            var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7qDZ#!.4bH!!#7iTa6#\\kQ>>q!Aa_K&&\\cA#CI8Z%m^FU!5o1r=u$([)hhIgOoV/V%gAPP'h.nj%te%A!!`K(\"9AMD!!!!''Z^Lma8lGYaE.spA.ShC!!%NS!<C1@!!");
            //var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7sY.8!3?/#!!!QsP7)=G\\-lgI!Aa^(,DG#K*L%.'#B1Ju(I89]+eobcPS&Q*1&rRk!71itL#5QE'bbO&<IH8?<IIO]KASHb$31&0!<C1@!!!9=a9iU%1_g\"@!<s7Z#o(IV&-<(Ma9&.K\",%3g#lt%I!!!");
            var potSize = uiG.Vertices.Max(v => int.Parse(v.Label));

            var G = new Choosability.Graph(uiG.GetEdgeWeights());
            var template = new Template(G.Vertices.Select(v => potSize + G.Degree(v) - uiG.Vertices[v].Label.TryParseInt().Value).ToList());
            GraphViz.DrawGraph(G, @"C:\game trees\G.pdf");

            var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(G);
            mind.MaxPot = potSize;

            var classes = new List<List<string>>();
            
            var win = mind.Analyze(template, null);
            if (win)
            {
                foreach (var kvp in mind.BoardsOfDepth)
                {
                    classes.Add(kvp.Value.SelectMany(b =>
                        {
                            var p = GetProgolStatement(b);
                            return Choosability.Utility.Permutation.EnumerateAll(3).Select(pi => PermuteDigits(p, pi));
                        }).ToList());
                }
            }

            var n = mind.BoardsOfDepth[0][0].Stacks.Value.Select(l => string.Join("", l.ToSet()))
                                                 .Count(s => s.Length == 2);
            var all = Enumerable.Repeat(Enumerable.Range(0, 3).Select(x => x.ToString()), n).CartesianProduct().Select(z => "good(" + string.Join(",", z) + ").").ToList();
            var losers = classes.Select(c => all.Except(c).ToList()).ToList();

            var cs = classes.Select(c => string.Join(Environment.NewLine, c)).ToList();
            var ls = losers.Select(c => string.Join(Environment.NewLine, c.Select(s => ":- " + s))).ToList();

            var sb = new StringBuilder();
            sb.AppendLine(":- set(verbose,1)?");
            for (int i = 0; i < cs.Count; i++)
                sb.AppendLine(string.Format(":- modeh(1,depth_{0}({1}))?", i, string.Join(",", Enumerable.Repeat("+const", n))));

            sb.AppendLine(":- modeb(1,neq(+const,+const))?");
            sb.AppendLine(":- modeb(1, count(+const,+const,-nn))?");
            sb.AppendLine(":- modeb(1, count(+const,+const,+const,-nn))?");
            sb.AppendLine("nn(0). nn(1). nn(2). nn(3). nn(4).");
            sb.AppendLine("const(0). const(1). const(2).");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i != j)
                        sb.AppendLine(string.Format("neq({0},{1}).", i, j));

                    sb.AppendLine(string.Format("count({0},{1},{2}).", i, j, new[] { i, j }.Distinct().Count()));

                    for (int k = 0; k < 3; k++)
                    {
                        sb.AppendLine(string.Format("count({0},{1},{2},{3}).", i, j, k, new[] { i, j, k }.Distinct().Count()));
                    }
                }
            }

            for (int i = 0; i < cs.Count; i++)
            {
                sb.AppendLine(cs[i].Replace("good", "depth_" + i));
                sb.AppendLine(ls[i].Replace("good", "depth_" + i));
            }

            using (var sw = new StreamWriter(@"C:\progol\fixerbreaker.pl"))
                sw.Write(sb.ToString());
        }

        static string PermuteDigits(string s, Choosability.Utility.Permutation pi)
        {
            return s.Replace("0", "X")
                    .Replace("1", "Y")
                    .Replace("2", "Z")
                    .Replace("X", pi[0].ToString())
                    .Replace("Y", pi[1].ToString())
                    .Replace("Z", pi[2].ToString());
        }

        static string GetProgolStatement(SuperSlimBoard b)
        {
            return "good(" + string.Join(",", b.Stacks.Value.Select(l => string.Join("", l.ToSet()))
                                 .Where(s => s.Length == 2)
                                 .Select(s =>
                                     {
                                         switch (s)
                                         {
                                             case "01":
                                                 return "0";
                                             case "02":
                                                 return "1";
                                             case "12":
                                                 return "2";
                                         }

                                         return "?";
                                     })) + ").";
        }

        static void DoEdgeAlls(Graphs.Graph uiG, int potSize, Choosability.Graph G, Template template, int i)
        {
            var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(G);
            mind.MaxPot = potSize;
            mind.OnlyConsiderNearlyColorableBoards = true;
            mind.MissingEdgeIndex = i;

            var root = @"C:\game trees\alls2\" + i;
            Directory.CreateDirectory(root);

            var win = mind.Analyze(template, null);
            if (win)
            {
                int j = 0;
                foreach (var board in mind.NonColorableBoards)
                {
                    var tree = mind.BuildGameTree(board);
                    GraphViz.DrawTree(tree, root + @"\" + i + " depth " + tree.GetDepth() + " board " + j + ".pdf");
                    j++;
                }
            }
        }

        static void DoAllEdges(Graphs.Graph uiG, int potSize, Choosability.Graph G, Template template)
        {
            for (int i = 0; i < uiG.Edges.Count; i++)
            {
                var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(G);
                mind.MaxPot = potSize;
                mind.OnlyConsiderNearlyColorableBoards = true;
                mind.MissingEdgeIndex = i;

                var win = mind.Analyze(template, null);
                if (win)
                {
                    var tree = mind.BuildGameTree(mind.DeepestBoards[0]);
                    GraphViz.DrawTree(tree, @"C:\game trees\test" + i + ".pdf");
                }
            }
        }
    }
}
