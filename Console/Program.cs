using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var uiG = GraphsCore.CompactSerializer.Deserialize("webgraph:7q2N!!.4bH!!#8TL^4tHcj+(]!Ajde)5%L+#;Z?:+U+G?.0H.u!7W\\+T(jdSOoSI=(IJEI,o%*C0GP6+!!*'#!6>-?!X'&:\":t\\=&-W.La9FI4#6Y&-!!%NLa8c2");
            var potSize = uiG.Vertices.Max(v => int.Parse(v.Label));

            var G = new Choosability.Graph(uiG.GetEdgeWeights());
            var template = new Template(G.Vertices.Select(v => potSize + G.Degree(v) - uiG.Vertices[v].Label.TryParseInt().Value).ToList());
            DrawGraph(G, @"C:\game trees\G.pdf");

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
                    DrawTree(tree, root + @"\" + i + " depth " + tree.GetDepth() + " board " + j + ".pdf");
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
                    DrawTree(tree, @"C:\game trees\test" + i + ".pdf");
                }
            }
        }

        static void DrawTree(GameTree tree, string path)
        {
            var renderer = new DotRenderer(@"C:\Program Files (x86)\Graphviz2.36\bin\dot.exe");

            var vv = tree.BuildGraph();
            renderer.Render(vv.Item1, vv.Item2, path, DotRenderType.pdf, true);
        }
        static void DrawGraph(Choosability.Graph g, string path)
        {
            var renderer = new DotRenderer(@"C:\Program Files (x86)\Graphviz2.36\bin\dot.exe");
            renderer.Render(g.ToDot(), path, DotRenderType.pdf);
        }
    }

      //async void CreateFixerBreakerGameTree(bool onlyNearColorings, int missingEdgeIndex = -1)
      //  {
      //      var blob = AlgorithmBlob.Create(SelectedTabCanvas);
      //      if (blob == null)
      //          return;

      //      int potSize;
      //      try
      //      {
      //          potSize = blob.UIGraph.Vertices.Max(v => v.Label.TryParseInt().Value);
      //      }
      //      catch
      //      {
      //          return;
      //      }
      //      var G = blob.AlgorithmGraph;
      //      var template = new Template(G.Vertices.Select(v => potSize + G.Degree(v) - blob.UIGraph.Vertices[v].Label.TryParseInt().Value).ToList());

      //      var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(G);
      //      mind.MaxPot = potSize;
      //      mind.OnlyNearlyColorable = true;
      //      mind.MissingEdgeIndex = missingEdgeIndex;

      //      GameTree tree = null;
      //      using (var resultWindow = new ResultWindow(true))
      //      {
      //          resultWindow.Show();
      //          tree = await Task.Factory.StartNew<GameTree>(() =>
      //          {
      //              if (Enumerable.Range(0, G.N).Any(i => template.Sizes[i] < G.Degree(i)))
      //                  return null;

      //              var win = mind.Analyze(template, null);
      //              return mind.BuildGameTree(mind.DeepestBoards[0]);
      //          });
      //      }

      //      if (tree == null)
      //          return;

      //      var h = tree.BuildGraph();
      //      var H = new Graphs.Graph(h.Item1, Layout.GetSpringsLayout(h.Item1, 12));

      //      for (int i = 0; i < H.Vertices.Count; i++)
      //      {
      //          H.Vertices[i].Label = h.Item2[i].Board.Stacks.Value.ToSetString();
      //      }

      //      AddTab(H, "tree");
      //  }
}
