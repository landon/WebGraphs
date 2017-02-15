using Choosability;
using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using Choosability.Utility;
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
        Graphs.Graph _visualizationGraph;
        GraphCanvas _graphCanvas;

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

            _boardToTree = _mind.NonColorableBoards.Concat(_mind.ColorableBoards).Select(b => new { Board = b, Tree = _mind.BuildGameTree(b, true) }).ToDictionary(x => x.Board, x => x.Tree);

            var ll = _boardToTree.ToList();
            ll.Sort((t1, t2) =>
            {
                var cc = t1.Value.GetDepth().CompareTo(t2.Value.GetDepth());
                if (cc > 0)
                    return -1;
                if (cc < 0)
                    return 1;
                return t1.Value.Board.ToListStringInLexOrder(_mind.MaxPot).CompareTo(t2.Value.Board.ToListStringInLexOrder(_mind.MaxPot));
            });
            foreach (var kvp in ll)
            {
                var treeItem = new TreeViewItem();
                InitializeTreeItem(treeItem, kvp.Value);
                _theTree.Items.Add(treeItem);
                AddTreeItems(treeItem, kvp.Value);
            }
        }

        void AddTreeItems(TreeViewItem item, GameTree tree)
        {
            var ll = tree.Children.ToList();
            ll.Sort((t1, t2) =>
            {
                var cc = t1.GetDepth().CompareTo(t2.GetDepth());
                if (cc > 0)
                    return -1;
                if (cc < 0)
                    return 1;
                return t1.Board.ToListStringInLexOrder(_mind.MaxPot).CompareTo(t2.Board.ToListStringInLexOrder(_mind.MaxPot));
            });
            foreach (var child in ll)
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
            _visualizationGraph = _blob.UIGraph.Clone();
            _graphCanvas = new GraphCanvas(_visualizationGraph);
            _graphCanvas.SnapToGrid = false;
            _graphCanvas.DrawGrid = false;
            _graphCanvas.DoClearLabels();
            _visualizationGraph.ToggleVertexIndices();

            if (gameTree.Info != null)
            {
                Permutation pp;
                var listString = gameTree.Parent.Board.ToListStringInLexOrder(out pp, _mind.MaxPot);

                var alpha = Math.Min(pp[gameTree.Info.Alpha], pp[gameTree.Info.Beta]);
                var beta = Math.Max(pp[gameTree.Info.Alpha], pp[gameTree.Info.Beta]);

                _swapInfoLabel.Text = alpha + " <--> " + beta + " at " + gameTree.Info.SwapVertices.ToSetString();
            }
            else
            {
                _swapInfoLabel.Text = "";
            }

            ColorGraph(gameTree, _visualizationGraph);
            RepaintCanvas();
        }

        void RepaintCanvas()
        {
            Dispatcher.BeginInvoke(() =>
            {
                var g = new Graphics(_theCanvas);
                _graphCanvas.Paint(g, (int)_theCanvas.ActualWidth, (int)_theCanvas.ActualHeight);
                _graphCanvas.DoZoomFit();

                g = new Graphics(_theCanvas);
                _graphCanvas.Paint(g, (int)_theCanvas.ActualWidth, (int)_theCanvas.ActualHeight);
                _graphCanvas.DoZoomFit();
            });
        }

        void ColorGraph(GameTree tree, Graphs.Graph clone)
        {
            var lists = tree.Board.Stacks.Value.Select(s => s.ToSet()).ToList();
            Permutation pp;
            var listString = tree.Board.ToListStringInLexOrder(out pp, _mind.MaxPot);

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
                    e.Label = pp[c].ToString();
                }

                for (int q = 0; q < lists.Count; q++)
                {
                    clone.Vertices[q].Label = string.Join(",", lists[q].Select(x => pp[x]).OrderBy(x => x));
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
                    e.Label = pp[c].ToString();
                }

                for (int q = 0; q < lists.Count; q++)
                {
                    clone.Vertices[q].Label = string.Join(",", lists[q].Select(x => pp[x]).OrderBy(x => x));
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
        }

        void OnToggleShowIndices(object sender, RoutedEventArgs e)
        {
            if (_visualizationGraph != null)
            {
                _visualizationGraph.ToggleVertexIndices();
                RepaintCanvas();
            }
        }

        void _searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _infoBox.Text = "";
            var s = _searchBox.Text;
            var x = _theTree.Items.FirstOrDefault(ii => ((GameTree)((TreeViewItem)ii).Tag).Board.ToListStringInLexOrder(_mind.MaxPot).StartsWith(s)) as TreeViewItem;
            if (x != null)
            {
                _theTree.SelectItem(x);
                x.IsExpanded = true;
                return;
            }

            try
            {
                var lists = s.Split(new[] { "|", "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Enumerable.Range(0, a.Length).Select(i => int.Parse(a[i].ToString())).ToList()).ToList();
                var board = SuperSlimBoard.FromLists(lists);
                var ss = board.ToListStringInLexOrder();

                x = _theTree.Items.FirstOrDefault(ii => ((GameTree)((TreeViewItem)ii).Tag).Board.ToListStringInLexOrder(_mind.MaxPot).StartsWith(ss)) as TreeViewItem;
                if (x != null)
                {
                    _theTree.SelectItem(x);
                    x.IsExpanded = true;
                    return;
                }
            }
            catch { }

            _infoBox.Text = "no match found";
        }
    }
}

