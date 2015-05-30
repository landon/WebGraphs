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
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs;

namespace Console
{
    public static class FixerBreakerTrees
    {
        public static void Go()
        {
            WriteProof();
        }

        public static void WriteProof()
        {
          //  var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7qM`$!.4bH!!!R&a9[/.`!KrS!Aa_K%n$+I!*C1-pbPd:TFX<n1&s-S7JJ\\_\"eHt>8?!.J7JQ4#6hiD:#64`)!<=YO!!!'6/-5ne/-I40\"U#Ji#\"0\"-!!!\"L\"p\"]T!<<"); //small tree aka fig1 left
       //     var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7oB<e!-S>B!!#8TJd$*0\\-lgI!AXY2';,k%#;Z?^)[2gP(C-Of'0ujY!!N?&!.ZPL!<E0O!=+(&I\"$HlIKBKg/-?eA!uhdS2\\:FcIXV"); // fig1 middle

         //   var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7qDZ#!2KSp!!#7s_?WaT\\-lgI!Aa^d)5%L-#>?-AShqSoSn&^p'0ujY!)+Du>Qb&\"e2/>6>R(6/!<E0O!=+(&I\"$HlIK0Hg/-6b%!s0Al\"<.mU.hDa^!.Y84&:T\"UIK"); // fig1 right

          //  var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:8!X,T!$D:B!!&)lOptM4\\-lgI!Aa^@+,/T#,DG#?+.s-1#;Q9])[2f],6a[K$V?KmJ;)mR%u&ns'8>>R$O*,X'*Xu#%gAPD(XNCQ#;Z>g!!*'#!6>-?Uf-F^IXZZnI\"$MF!ZK,^!?0#^$P3Il&-`@Xa9[/)$6$tl\"rbPjHP\"$p#ljsUL^OUs(<CrPIXV"); // big tree
            
            var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7RI.U!'1)[!!@T`X:8\\?!AXXc)8lhl!!+kU9HC2DA,lWc!!3-$!!*'N!=+(&I\"$HlIK9KN!%\\-UIXZZnI\"$M"); // val
            
            var potSize = uiG.Vertices.Max(v => int.Parse(v.Label));
            var g = new Choosability.Graph(uiG.GetEdgeWeights());
            var template = new Template(g.Vertices.Select(v => potSize + g.Degree(v) - uiG.Vertices[v].Label.TryParseInt().Value).ToList());

            var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g, proofFindingMode: true);
            mind.MaxPot = potSize;
         //   mind.MissingEdgeIndex = 0;
         //   mind.OnlyConsiderNearlyColorableBoards = true;

            int j = 0;
            var win = mind.Analyze(template);

            if (win)
            {
                var pb = new ProofBuilder(mind);
                var proof = pb.WriteProof();
            }
        }

        public static void BuildTreePictures()
        {
            var root = @"C:\game trees\smalltree\fixable";
            Directory.CreateDirectory(root);

         //   var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7p5lm!.4bH!!#8TV?g9C\\-lgI!AXXW)k[^-#;Z?F*sJ5a,6aYM-O65sh^B_'h`q79;LfjChZj,^!!*)@!!!'$'Z^@ja9NC\"!sTF[\">gYm\"T\\VE!!!");
          //  var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7s+e3!3?/#!!#:+U^KF0PS%F+!AXX?+05o;#;\\Xf`!Jd9-mM7W`P)m27JKt(6hiE-#!f!b-mM`?paS`-&1>N4<IG2S!<<-#a8c2A!>NTW'Z^Ila9)SZa98IB!.Zm;#6Y^]#ZM?A<\"KBC!<C1@!!");
            var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7qM`$!.4bH!!!R&a9[/.`!KrS!Aa_K%n$+I!*C1-pbPd:TFX<n1&s-S7JJ\\_\"eHt>8?!.J7JQ4#6hiD:#64`)!<=YO!!!'6/-5ne/-I40\"U#Ji#\"0\"-!!!\"L\"p\"]T!<<");
            var potSize = uiG.Vertices.Max(v => int.Parse(v.Label));
            var g = new Choosability.Graph(uiG.GetEdgeWeights());
            var template = new Template(g.Vertices.Select(v => potSize + g.Degree(v) - uiG.Vertices[v].Label.TryParseInt().Value).ToList());

            GraphViz.DrawGraph(g, root + @"\G.pdf", true);

            var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g, proofFindingMode: true);
            mind.MaxPot = potSize;
          //  mind.OnlyConsiderNearlyColorableBoards = true;
          //  mind.MissingEdgeIndex = 0;

            int j = 0;
            var win = mind.Analyze(template);

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

    }
}
