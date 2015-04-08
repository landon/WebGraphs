using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class GraphViz
    {
        public static void DrawTree(GameTree tree, string path)
        {
            var renderer = new DotRenderer(@"C:\Program Files\Graphviz2.36\bin\dot.exe");

            var vv = tree.BuildGraph();
            renderer.Render(vv.Item1, vv.Item2, path, DotRenderType.pdf, true);
        }

        public static void DrawGraph(Choosability.Graph g, string path)
        {
            var renderer = new DotRenderer(@"C:\Program Files\Graphviz2.36\bin\dot.exe");
            renderer.Render(g.ToDot(), path, DotRenderType.pdf);
        }
    }
}
