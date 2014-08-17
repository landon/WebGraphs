using Satsuma;
using Satsuma.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphsCore
{
    public static class Layout
    {
        public static Supergraph CreateSatsumaGraph(this Choosability.Graph g)
        {
            var satsumaGraph = new CustomGraph();

            var nodes = new Dictionary<int, Node>();
            foreach (var v in g.Vertices)
            {
                var node = satsumaGraph.AddNode();
                nodes[v] = node;
            }

            for (int i = 0; i < g.N; i++)
            {
                for (int j = i + 1; j < g.N; j++)
                {
                    if (g.Directed[i, j])
                        satsumaGraph.AddArc(nodes[i], nodes[j], Directedness.Directed);
                    else if (g.Directed[j, i])
                        satsumaGraph.AddArc(nodes[j], nodes[i], Directedness.Directed);
                    else if (g.Adjacent[i, j])
                        satsumaGraph.AddArc(nodes[i], nodes[j], Directedness.Undirected);
                }
            }

            return satsumaGraph;
        }

        public static List<Tuple<double, double>> GetSpringsLayout(this Choosability.Graph g, int randomSeed = 0)
        {
            var satsumaGraph = g.CreateSatsumaGraph();
            var layout = new ForceDirectedLayout(satsumaGraph, null, randomSeed);
            layout.Run();

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach(var p in layout.NodePositions)
            {
                minX = Math.Min(minX, p.Value.X);
                minY = Math.Min(minY, p.Value.Y);
                maxX = Math.Max(maxX, p.Value.X);
                maxY = Math.Max(maxY, p.Value.Y);
            }

            var w = maxX - minX;
            var h = maxY - minY;

            return satsumaGraph.Nodes().Select(n => new Tuple<double, double>(0.1 + 0.7 * (layout.NodePositions[n].X - minX) / w, 0.1 + 0.7 * (layout.NodePositions[n].Y - minY) / h)).ToList();
        }
    }
}
