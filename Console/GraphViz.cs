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
            var renderer = new DotRenderer(@"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe");

            var vv = tree.BuildGraph();
            renderer.Render(vv.Item1, vv.Item2, path, DotRenderType.png, true);
        }

        public static void DrawGraph(Choosability.Graph g, string path, bool labelEdges = false)
        {
            var renderer = new DotRenderer(@"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe");
            renderer.Render(g.ToDotWithFactors(labelEdges), path, DotRenderType.png);
        }
    }
}
