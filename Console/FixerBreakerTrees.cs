using Choosability.FixerBreaker.KnowledgeEngine;
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
            var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7q2N!!.4bH!!#8TL^4tHcj+(]!Ajde)5%L+#;Z?:+U+G?.0H.u!7W\\+T(jdSOoSI=(IJEI,o%*C0GP6+!!*'#!6>-?!X'&:\":t\\=&-W.La9FI4#6Y&-!!%NLa8c2");
            var potSize = uiG.Vertices.Max(v => int.Parse(v.Label));

            var G = new Choosability.Graph(uiG.GetEdgeWeights());
            var template = new Template(G.Vertices.Select(v => potSize + G.Degree(v) - uiG.Vertices[v].Label.TryParseInt().Value).ToList());
            GraphViz.DrawGraph(G, @"C:\game trees\G.pdf");

            // for (int i = 0; i < uiG.Edges.Count; i++)
            {
                int i = 4;
                System.Console.WriteLine("Doing edge " + i + "...");
                DoEdgeAlls(uiG, potSize, G, template, i);
            }
        }

        static void DoEdgeAlls(Graphs.Graph uiG, int potSize, Choosability.Graph G, Template template, int i)
        {
            var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(G);
            mind.MaxPot = potSize;
            mind.OnlyNearlyColorable = true;
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
                mind.OnlyNearlyColorable = true;
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
