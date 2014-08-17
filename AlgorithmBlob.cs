using Graphs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Choosability;

namespace WebGraphs
{
    public class AlgorithmBlob
    {
        public Graphs.Graph UIGraph { get; private set; }
        public Choosability.Graph AlgorithmGraph { get; private set; }
        public BitLevelGeneration.BitGraph BitGraph { get; private set; }
        public List<int> SelectedVertices { get; private set; }
        public List<Tuple<int, int>> SelectedEdges { get; private set; }
        public List<int> SelectedEdgeIndices { get; private set; }
        public Dictionary<Tuple<int, int>, int> EdgeIndexLookup { get; private set; }

        public static AlgorithmBlob Create(TabCanvas tabCanvas)
        {
            if (tabCanvas == null)
                return null;

            return new AlgorithmBlob(tabCanvas);
        }

        AlgorithmBlob(TabCanvas tabCanvas)
        {
            UIGraph = tabCanvas.Operations.Graph;
            AlgorithmGraph = new Choosability.Graph(UIGraph.GetEdgeWeights());
            BitGraph = new BitLevelGeneration.BitGraph(UIGraph.GetEdgeWeights());
            SelectedVertices = UIGraph.Vertices.Select((v, i) => v.IsSelected ? i : -1).Where(x => x >= 0).ToList();
            SelectedEdges = UIGraph.Edges.Where(e => e.IsSelected).Select(e => new Tuple<int, int>(UIGraph.Vertices.IndexOf(e.V1), UIGraph.Vertices.IndexOf(e.V2))).ToList();

            EdgeIndexLookup = new Dictionary<Tuple<int, int>, int>();
            int k = 0;
            for (int i = 0; i < AlgorithmGraph.N; i++)
            {
                for (int j = i + 1; j < AlgorithmGraph.N; j++)
                {
                    if (AlgorithmGraph[i, j])
                    {
                        EdgeIndexLookup[new Tuple<int, int>(i, j)] = k;
                        EdgeIndexLookup[new Tuple<int, int>(j, i)] = k;
                        k++;
                    }
                }
            }

            SelectedEdgeIndices = SelectedEdges.Select(tuple => EdgeIndexLookup[tuple]).ToList();
        }
    }
}
