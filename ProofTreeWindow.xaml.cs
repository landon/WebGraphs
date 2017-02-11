using Choosability;
using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WebGraphs
{
    public partial class ProofTreeWindow : ChildWindow
    {
        AlgorithmBlob _blob;
        SuperSlimMind _mind;
        Dictionary<SuperSlimBoard, GameTree> _boardToTree;

        public ProofTreeWindow()
        {
            InitializeComponent();
        }

        public ProofTreeWindow(AlgorithmBlob blob)
            : this()
        {
            _blob = blob;
        }

        public async Task BuildTree()
        {
            var potSize = _blob.UIGraph.Vertices.Max(v => v.Label.TryParseInt().Value);

            var G = _blob.AlgorithmGraph;
            var template = new Template(G.Vertices.Select(v => potSize + G.Degree(v) - _blob.UIGraph.Vertices[v].Label.TryParseInt().Value).ToList());

            _mind = new SuperSlimMind(G, true, true);
            _mind.MaxPot = potSize;
            _mind.SuperabundantOnly = false;
            _mind.OnlyConsiderNearlyColorableBoards = true;
            _mind.MissingEdgeIndex = _blob.SelectedEdgeIndices.First();
            _mind.ThinkHarder = false;

            using (var resultWindow = new ResultWindow(true))
            {
                var win = await Task.Factory.StartNew<bool>(() => _mind.Analyze(template, resultWindow.OnProgress));

                resultWindow.Close();

                if (!win)
                {
                    MessageBox.Show("Fixer loses!");
                    Close();
                    return;
                }
            }

            _boardToTree = _mind.NonColorableBoards.Select(b => new { Board = b, Tree = _mind.BuildGameTree(b, true) }).ToDictionary(x => x.Board, x => x.Tree);

            foreach (var kvp in _boardToTree.OrderByDescending(kv => kv.Value.GetDepth()))
            {
                var treeItem = new TreeViewItem();
                InitializeTreeItem(treeItem, kvp.Value);
                _theTree.Items.Add(treeItem);
                AddTreeItems(treeItem, kvp.Value);
            }
        }

        void AddTreeItems(TreeViewItem item, GameTree tree)
        {
            foreach (var child in tree.Children.OrderByDescending(kv => kv.GetDepth()))
            {
                var childItem = new TreeViewItem();
                InitializeTreeItem(childItem, child);
                item.Items.Add(childItem);
            }

            item.Expanded += (s, e) =>
            {
                foreach(TreeViewItem it in item.Items)
                {
                    if (it.Items.Count <= 0)
                        AddTreeItems(it, it.Tag as GameTree);
                }
            };
        }

        void InitializeTreeItem(TreeViewItem item, GameTree tree)
        {
            item.Header = tree.Board.ToListStringInLexOrder(_mind.MaxPot);
            item.Tag = tree;
            item.Selected += Item_Selected;
        }

        void Item_Selected(object sender, RoutedEventArgs e)
        {
            var gameTree = (sender as TreeViewItem).Tag as GameTree;
            var visualizationGraph = _blob.UIGraph.Clone();
            var graphCanvas = new GraphCanvas(visualizationGraph);
            graphCanvas.SnapToGrid = false;
            graphCanvas.DrawGrid = false;
            graphCanvas.DoClearLabels();

            ColorGraph(gameTree, visualizationGraph);
            Dispatcher.BeginInvoke(() =>
            {
                var g = new Graphics(_theCanvas);
                graphCanvas.Paint(g, (int)_theCanvas.ActualWidth, (int)_theCanvas.ActualHeight);
                graphCanvas.DoZoomFit();

                g = new Graphics(_theCanvas);
                graphCanvas.Paint(g, (int)_theCanvas.ActualWidth, (int)_theCanvas.ActualHeight);
                graphCanvas.DoZoomFit();
            });
        }

        void ColorGraph(GameTree tree, Graphs.Graph clone)
        {
            var lists = tree.Board.Stacks.Value.Select(s => s.ToSet()).ToList();

            if (tree.IsColorable)
            {
                Dictionary<int, long> coloring;
                _mind.ColoringAnalyzer.Analyze(tree.Board, out coloring);

                for (int jj = 0; jj < clone.Edges.Count; jj++)
                {
                    var v1 = _mind._edges[jj].Item1;
                    var v2 = _mind._edges[jj].Item2;

                    var c = coloring[jj].LeastSignificantBit();

                    if (!lists[v1].Contains(c))
                        System.Diagnostics.Debugger.Break();
                    if (!lists[v2].Contains(c))
                        System.Diagnostics.Debugger.Break();

                    lists[v1].Remove(c);
                    lists[v2].Remove(c);

                    var e = clone.Edges.First(ee => Choosability.Utility.ListUtility.Equal(new List<int>() { clone.Vertices.IndexOf(ee.V1), clone.Vertices.IndexOf(ee.V2) }, new List<int>() { v1, v2 }));
                    e.Label = c.ToString();
                }
            }
            else
            {
                var selected = _blob.SelectedEdgeIndices.First();
                Dictionary<int, long> coloring;
                _mind.ColoringAnalyzer.AnalyzeWithoutEdge(tree.Board, out coloring, selected);

                for (int jj = 0; jj < clone.Edges.Count; jj++)
                {
                    if (jj == selected)
                        continue;

                    var v1 = _mind._edges[jj].Item1;
                    var v2 = _mind._edges[jj].Item2;

                    var c = coloring[jj].LeastSignificantBit();

                    if (!lists[v1].Contains(c))
                        System.Diagnostics.Debugger.Break();
                    if (!lists[v2].Contains(c))
                        System.Diagnostics.Debugger.Break();

                    lists[v1].Remove(c);
                    lists[v2].Remove(c);

                    var e = clone.Edges.First(ee => Choosability.Utility.ListUtility.Equal(new List<int>() { clone.Vertices.IndexOf(ee.V1), clone.Vertices.IndexOf(ee.V2) }, new List<int>() { v1, v2 }));
                    e.Label = c.ToString();
                }

                for (int q = 0; q < lists.Count; q++)
                {
                    clone.Vertices[q].Label = string.Join(",", lists[q].OrderBy(x => x));
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
        }
    }
}

