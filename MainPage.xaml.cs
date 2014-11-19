using Choosability.FixerBreaker;
using Choosability.FixerBreaker.KnowledgeEngine;
using Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Choosability;
using GraphicsLayer;
using System.Windows.Browser;
using GraphsCore;
using BitLevelGeneration;
using Choosability.Polynomials;
using SLPropertyGrid.MultiObject;
using System.IO.IsolatedStorage;
using Choosability.IndependenceRatio;
using GraphsCore.Famlies;
using WebGraphs.Analysis;
using System.Text;

namespace WebGraphs
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            _mainMenu.NewClicked += NewTab;
            _mainMenu.CloseClicked += CloseSelectedTab;
            _mainMenu.CloseAllClicked += CloseAllTabs;
            _mainMenu.TogglePropertiesClicked += () => _propertiesContainer.Visibility = _propertiesContainer.Visibility == System.Windows.Visibility.Collapsed ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            _mainMenu.UndoClicked += Undo;
            _mainMenu.RedoClicked += Redo;
            _mainMenu.CutClicked += Cut;
            _mainMenu.CopyClicked += Copy;
            _mainMenu.PasteClicked += Paste;
            _mainMenu.ZoomInClicked += ZoomIn;
            _mainMenu.ZoomOutClicked += ZoomOut;
            _mainMenu.ZoomFitClicked += ZoomFit;
            _mainMenu.LabelWithDegreesClicked += LabelWithDegrees;
            _mainMenu.LabelWithInDegreesClicked += LabelWithInDegrees;
            _mainMenu.LabelWithOutDegreesClicked += LabelWithOutDegrees;
            _mainMenu.ClearAllLabelsClicked += ClearLabels;
            _mainMenu.CountEulerianSubgraphs += CountEulerianSubgraphs;
            _mainMenu.CheckD1AT += CheckD1AT;
            _mainMenu.FindATNumber += FindATNumber;
            _mainMenu.FindStrongComponents += FindStrongComponents;
            _mainMenu.CheckD1Paintable += CheckD1Paintable;
            _mainMenu.Check2FoldD1Paintable += Check2FoldD1Paintable;
            _mainMenu.FindPaintNumber += FindPaintNumber;
            _mainMenu.ExportTeX += ExportTeX;
            _mainMenu.AnalyzeFixerBreaker += AnalyzeFixerBreaker;
            _mainMenu.DoMozhan += DoMozhan;
            _mainMenu.DoWebLink += ExportWebLink;
            _mainMenu.DoLightWebLink += ExportLightWebLink;
            _mainMenu.CheckfAT += CheckfAT;
            _mainMenu.CheckfATUsingFormula += CheckfATUsingFormula;
            _mainMenu.CheckfPaintable += CheckfPaintable;
            _mainMenu.LabelWithDegreeSymbol += LabelWithDegreeSymbol;
            _mainMenu.LabelWithDegreeSymbolMinusOne += LabelWithDegreeSymbolMinusOne;
            _mainMenu.DoGraph6 += DoGraph6;
            _mainMenu.DoEdgeWeights += DoEdgeWeights;
            _mainMenu.DoAdjacencyMatrix += DoAdjacencyMatrix;
            _mainMenu.ComputeCoefficient += ComputeCoefficient;
            _mainMenu.GenerateRandomOrientation += GenerateRandomOrientation;
            _mainMenu.GenerateBalancedOrientation += GenerateBalancedOrientation;
            _mainMenu.DoSpringsLayout += DoSpringsLayout;
            _mainMenu.CheckfChoosable += CheckfChoosable;
            _mainMenu.OnInstructions += OnInstructions;
            _mainMenu.OnAbout += OnAbout;
            _mainMenu.LabelWithOutDegreesPlusOneClicked += LabelWithOutDegreesPlusOne;
            _mainMenu.ComputeSignSum += ComputeSignSum;
            _mainMenu.ClearOrientation += ClearOrientation;
            _mainMenu.AnalyzeOnlyNearColorings += AnalyzeOnlyNearColorings;
            _mainMenu.AnalyzeOnlyNearColoringsForSelectedEdge += AnalyzeOnlyNearColoringsForSelectedEdge;
            _mainMenu.CheckFGPaintable += CheckFGPaintable;
            _mainMenu.DoLaplacianLayout += DoLaplacianLayout;
            _mainMenu.DoWalkMatrixLayout += DoWalkMatrixLayout;
            _mainMenu.FindGood3Partition += FindGood3Partition;
            _mainMenu.DoGridToggle += _mainMenu_DoGridToggle;
            _mainMenu.DoUnitDistanceLayout += _mainMenu_DoUnitDistanceLayout;
            _mainMenu.DoMakeHexGrid += _mainMenu_DoMakeHexGrid;
            _mainMenu.DoListExtraSpindleEdges += _mainMenu_DoListExtraSpindleEdges;
            _mainMenu.DoBasesAndTopsWeighting += _mainMenu_DoBasesAndTopsWeighting;
            _mainMenu.DoMaxFractionalClique += _mainMenu_DoMaxFractionalClique;
            _mainMenu.DoSolveLP += _mainMenu_DoSolveLP;

            _propertyGrid.SomethingChanged += _propertyGrid_SomethingChanged;

            DoAutoLoad();
        }

   
        void DoAutoLoad()
        {
            try
            {
                var c = HtmlPage.Window.Invoke("GetURLParameter", "c") as string;
                if (!string.IsNullOrEmpty(c))
                {
                    AddTab(CompactSerializer.Deserialize(c), FindUnusedName());
                    return;
                }
            }
            catch { }

            try
            {
                var jib = HtmlPage.Window.Invoke("GetURLParameter", "jib") as string;
                if (!string.IsNullOrEmpty(jib))
                {
                    var grid = HtmlPage.Window.Invoke("GetURLParameter", "grid") as string;

                    var json = Utility.Decompress(jib);
                    AddTab(Graphs.Graph.Deserialize(json), FindUnusedName(), grid != "False");
                    return;
                }
            }
            catch { }
            try
            {
                HtmlPage.Window.Invoke("GetAutoLoad", (Action<string>)(json =>
                {
                    if (!string.IsNullOrEmpty(json))
                    {
                        AddTab(Graphs.Graph.Deserialize(json), FindUnusedName());
                        return;
                    }
                }));
            }
            catch { }

            try
            {
                var graph6 = HtmlPage.Window.Invoke("GetURLParameter", "graph6") as string;
                if (!string.IsNullOrEmpty(graph6))
                {
                    var gg = GraphsCore.GraphIO.GraphFromGraph6(graph6);
                    gg.Name = graph6;
                    AddTab(gg, graph6);
                    return;
                }
            }
            catch { }

            try
            {
                var ew = HtmlPage.Window.Invoke("GetURLParameter", "ew") as string;
                if (!string.IsNullOrEmpty(ew))
                {
                    AddTab(GraphsCore.GraphIO.GraphFromEdgeWeightString(ew), FindUnusedName());
                    return;
                }
            }
            catch { }

            if (!LoadFromLocalStorage())
                NewTab();
        }

        bool LoadFromLocalStorage()
        {
            var loadedSomething = false;
            foreach (var name in Storage.GetFileNames())
            {
                var g = Storage.Load(name);
                AddTab(g, name);
                loadedSomething = true;
            }

            return loadedSomething;
        }

        void _propertyGrid_SomethingChanged()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas != null)
                tabCanvas.Invalidate();
        }

        #region tab stuff
        void NewTab()
        {
            AddTab(null, FindUnusedName());
        }

        void AddTab(Graphs.Graph g, string name, bool snapToGrid = true)
        {
            var item = new TabItem();
            item.Header = name;
            _tabControl.Items.Add(item);

            var canvas = new Canvas();
            item.Content = canvas;
            canvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            canvas.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            var gc = new GraphCanvas(g);
            gc.SnapToGrid = snapToGrid;
            gc.DrawGrid = snapToGrid;
            var tabCanvas = new TabCanvas(canvas, gc, _propertyGrid, item);
            tabCanvas.Title = name;
            item.Tag = tabCanvas;

            _tabControl.SelectedItem = item;
        }

        string FindUnusedName()
        {
            var root = "scratch ";
            var next = 1;
            foreach (TabItem ti in _tabControl.Items)
            {
                var headerText = ti.Header as string;

                if (headerText.StartsWith(root))
                {
                    int i;
                    if (int.TryParse(headerText.Replace(root, ""), out i))
                        next = Math.Max(next, i + 1);
                }
            }

            return root + next;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }

        TabCanvas GetTabCanvas(TabItem ti)
        {
            if (ti == null)
                return null;

            return (TabCanvas)ti.Tag;
        }

        TabCanvas SelectedTabCanvas { get { return GetTabCanvas(_tabControl.SelectedItem as TabItem); } }

        void CloseSelectedTab()
        {
            var selected = _tabControl.SelectedItem as TabItem;
            if (selected == null)
                return;

            var tc = (TabCanvas)selected.Tag;
            Storage.Delete(tc.Title);
            _tabControl.Items.Remove(selected);
        }

        void CloseAllTabs()
        {
            foreach (TabItem item in _tabControl.Items)
            {
                var tc = (TabCanvas)item.Tag;
                Storage.Delete(tc.Title);
            }
            _tabControl.Items.Clear();
        }

        void FocusSelectedTab()
        {
            var item = _tabControl.SelectedItem as TabItem;
            if (item == null)
                return;

            item.Focus();
        }
                
        #endregion

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            var p = e.GetPosition(tabCanvas.Canvas);
            tabCanvas.GraphCanvas.DoZoom(e.Delta / 120, new GraphicsLayer.Box((double)p.X / tabCanvas.Canvas.ActualWidth, (double)p.Y / tabCanvas.Canvas.ActualHeight));

            base.OnMouseWheel(e);
        }
        void ToolStripButtonClicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as Image;
            switch (((BitmapImage)item.Source).UriSource.OriginalString.Replace("/images/", ""))
            {
                case "new.png":
                    NewTab();
                    break;
                case "open.png":
                    break;
                case "delete.png":
                    DeleteSelected();
                    break;
                case "complement.png":
                    ComplementSelected();
                    break;
                case "compress-hi.png":
                    ContractSelected();
                    break;
                case "copy.png":
                    Copy();
                    break;
                case "cut.png":
                    Cut();
                    break;
                case "paste.png":
                    Paste();
                    break;
                case "undo.png":
                    Undo();
                    break;
                case "redo.png":
                    Redo();
                    break;
                case "squared.png":
                    TakeSquare();
                    break;
                case "line graph.png":
                    TakeLineGraph();
                    break;
                case "clear.png":
                    ClearLabels();
                    break;
                case "zoom fit.png":
                    ZoomFit();
                    break;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.A:
                    break;
                case Key.Add:
                    break;
                case Key.Alt:
                    break;
                case Key.B:
                    break;
                case Key.Back:
                    break;
                case Key.C:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        Copy();
                    break;
                case Key.CapsLock:
                    break;
                case Key.Ctrl:
                    break;
                case Key.D:
                    break;
                case Key.D0:
                    break;
                case Key.D1:
                    break;
                case Key.D2:
                    break;
                case Key.D3:
                    break;
                case Key.D4:
                    break;
                case Key.D5:
                    break;
                case Key.D6:
                    break;
                case Key.D7:
                    break;
                case Key.D8:
                    break;
                case Key.D9:
                    break;
                case Key.Decimal:
                    break;
                case Key.Delete:
                    break;
                case Key.Divide:
                    break;
                case Key.Down:
                    break;
                case Key.E:
                    break;
                case Key.End:
                    break;
                case Key.Enter:
                    break;
                case Key.Escape:
                    break;
                case Key.F:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        ZoomFit();
                    break;
                case Key.F1:
                    break;
                case Key.F10:
                    break;
                case Key.F11:
                    break;
                case Key.F12:
                    break;
                case Key.F2:
                    break;
                case Key.F3:
                    break;
                case Key.F4:
                    break;
                case Key.F5:
                    break;
                case Key.F6:
                    break;
                case Key.F7:
                    break;
                case Key.F8:
                    break;
                case Key.F9:
                    break;
                case Key.G:
                    break;
                case Key.H:
                    break;
                case Key.Home:
                    break;
                case Key.I:
                    break;
                case Key.Insert:
                    break;
                case Key.J:
                    break;
                case Key.K:
                    break;
                case Key.L:
                    break;
                case Key.Left:
                    break;
                case Key.M:
                    break;
                case Key.Multiply:
                    break;
                case Key.N:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        NewTab();
                    break;
                case Key.None:
                    break;
                case Key.NumPad0:
                    break;
                case Key.NumPad1:
                    break;
                case Key.NumPad2:
                    break;
                case Key.NumPad3:
                    break;
                case Key.NumPad4:
                    break;
                case Key.NumPad5:
                    break;
                case Key.NumPad6:
                    break;
                case Key.NumPad7:
                    break;
                case Key.NumPad8:
                    break;
                case Key.NumPad9:
                    break;
                case Key.O:
                    break;
                case Key.P:
                    break;
                case Key.PageDown:
                    break;
                case Key.PageUp:
                    break;
                case Key.Q:
                    break;
                case Key.R:
                    break;
                case Key.Right:
                    break;
                case Key.S:
                    break;
                case Key.Shift:
                    break;
                case Key.Space:
                    break;
                case Key.Subtract:
                    break;
                case Key.T:
                    break;
                case Key.Tab:
                    break;
                case Key.U:
                    break;
                case Key.Unknown:
                    break;
                case Key.Up:
                    break;
                case Key.V:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        Paste();
                    break;
                case Key.W:
                    break;
                case Key.X:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        Cut();
                    break;
                case Key.Y:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        Redo();
                    break;
                case Key.Z:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        Undo();
                    break;
                default:
                    break;
            }
        }

        #region menu actions
        void OnAbout()
        {
            MessageBox.Show("web graphs\n\n\nlandon made", "about", MessageBoxButton.OK);
        }
        void OnInstructions()
        {
            MessageBox.Show(@"* Double left click to add vertex.
  * Hold down left mouse button and draw a curve, surround some
vertices, it will select them.
  * You can left click on selected vertices and drag them around.
  * With vertices selected, double left click on any vertex and it
will join the selected to that vertex.
  * If you draw the curve across some edges it will select them.
  * Hold ctrl key to select symmetric difference.
  * Mouse wheel zooms in and out.
  * You can delete anything when you have it selected by hitting the
trash can button.
  * The toolbar buttons have tool tips.
  * To use the f-choosability algorithms, put the list sizes as the vertex labels.
  * In the vertex labels you can use numbers, but also things like `d-1' to get degree minus one.
  * There are some helpers under the `Label' menu item.
  * You can paste in graphs in graph6 format.", "instructions", MessageBoxButton.OK);
        }
        void DeleteSelected()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoDelete();

            FocusSelectedTab();
        }
        void ComplementSelected()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoComplement();

            FocusSelectedTab();
        }
        void ContractSelected()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoContractSelectedSubgraph();

            FocusSelectedTab();
        }
        void ZoomFit()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoZoomFit();

            FocusSelectedTab();
        }
        void ZoomIn()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoZoom(1, new GraphicsLayer.Box(0.5, 0.5));

            FocusSelectedTab();
        }
        void ZoomOut()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoZoom(-1, new GraphicsLayer.Box(0.5, 0.5));

            FocusSelectedTab();
        }
        void Cut()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoCut();

            FocusSelectedTab();
        }
        void Copy()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoCopy();

            FocusSelectedTab();
        }
        void Paste()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoPaste();

            FocusSelectedTab();
        }
        void Undo()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoUndo();

            FocusSelectedTab();
        }
        void Redo()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoRedo();

            FocusSelectedTab();
        }
        void TakeSquare()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            var g = tabCanvas.GraphCanvas.DoSquare();
            AddTab(g, string.Format("({0})^2", tabCanvas.Title));

            FocusSelectedTab();
        }
        void TakeLineGraph()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            var g = tabCanvas.GraphCanvas.DoLineGraph();
            AddTab(g, string.Format("L({0})", tabCanvas.Title));

            FocusSelectedTab();
        }
        void LabelWithDegrees()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoDegreeLabeling();

            FocusSelectedTab();
        }
        void LabelWithInDegrees()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoInDegreeLabeling();

            FocusSelectedTab();
        }
        void LabelWithOutDegrees()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoOutDegreeLabeling();

            FocusSelectedTab();
        }
        void LabelWithOutDegreesPlusOne()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoOutDegreePlusOneLabeling();

            FocusSelectedTab();
        }
        void LabelWithDegreeSymbol()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoClearLabels("d", AlgorithmBlob.Create(SelectedTabCanvas).SelectedVertices);
        }
        void LabelWithDegreeSymbolMinusOne()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoClearLabels("d-1", AlgorithmBlob.Create(SelectedTabCanvas).SelectedVertices);
        }

        void _mainMenu_DoMakeHexGrid()
        {
            int n = 5;

            if (SelectedTabCanvas != null)
            {
                n = SelectedTabCanvas.GraphCanvas.Graph.Vertices.Select(v =>
                    {
                        int q;
                        int.TryParse(v.Label, out q);
                        return q;
                    }).Aggregate(0, (t, x) => Math.Max(t, x));

                if (n <= 0)
                    n = 5;
            }

            var g = new HexGrid().Create(1.0, n);

            AddTab(g, "hex " + n, snapToGrid: false);

            FocusSelectedTab();
        }


        async void _mainMenu_DoListExtraSpindleEdges()
        {
            if (SelectedTabCanvas == null)
                return;

            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            var p = blob.UIGraph.Vertices.Select(v => new Vector(v.X, v.Y)).ToList();

            var e = await Task.Factory.StartNew<List<string>>(() =>
            {
                var diamonds = SpindleAnalyzer.FindDiamonds(blob, p);
                return SpindleAnalyzer.FindSerendipitousEdges(blob, p, diamonds);
            });

            var gg = await Task.Factory.StartNew<Tuple<Graphs.Graph, Graphs.Graph>>(() =>
            {
                var diamonds = SpindleAnalyzer.FindDiamonds(blob, p);
                Graphs.Graph rotated;
                var g = SpindleAnalyzer.BuildSerendipitousEdgeGraph(blob, p, diamonds, out rotated);

                return new Tuple<Graphs.Graph, Graphs.Graph>(g, rotated);
            });

            blob.UIGraph.DisjointUnion(gg.Item1);
            SelectedTabCanvas.Invalidate();

            AddTab(gg.Item2, "rotated", snapToGrid:false);

            ShowText("total: " + e.Count + Environment.NewLine + string.Join(Environment.NewLine, e));
        }

        void _mainMenu_DoBasesAndTopsWeighting()
        {
            if (SelectedTabCanvas == null)
                return;

            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            var p = blob.UIGraph.Vertices.Select(v => new Vector(v.X, v.Y)).ToList();

            var diamonds = SpindleAnalyzer.FindDiamonds(blob, p);

            var bases = Enumerable.Range(0, blob.AlgorithmGraph.N).Select(v => diamonds.Count(d => d[0] == v)).ToList();
            var tops = Enumerable.Range(0, blob.AlgorithmGraph.N).Select(v => diamonds.Count(d => d[1] == v)).ToList();

            for (int v = 0; v < blob.UIGraph.Vertices.Count; v++)
            {
                blob.UIGraph.Vertices[v].Label = (2 * bases[v] + tops[v]).ToString();
            }

            SelectedTabCanvas.Invalidate();
        }


        async void _mainMenu_DoMaxFractionalClique()
        {
            if (SelectedTabCanvas == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                var blob = AlgorithmBlob.Create(SelectedTabCanvas);
                var p = blob.UIGraph.Vertices.Select(v => new Vector(v.X, v.Y)).ToList();

                var diamonds = SpindleAnalyzer.FindDiamonds(blob, p);
                var w = blob.AlgorithmGraph.Vertices.Select(v => blob.UIGraph.Vertices[v].Label.TryParseDouble().Value).ToList();

                var total = SpindleAnalyzer.ComputeTotalWeight(blob, p, diamonds, w);

                var best = await Task.Factory.StartNew<Tuple<double, List<int>>>(() =>
                    {
                        List<int> heaviestIndependentSet;
                        var weight = SpindleAnalyzer.ComputeBestWeight(blob, p, diamonds, w, out heaviestIndependentSet);

                        return new Tuple<double, List<int>>(weight, heaviestIndependentSet);
                    });

                blob.UIGraph.SelectVertices(best.Item2.Select(v => blob.UIGraph.Vertices[v]), false);
                SelectedTabCanvas.Invalidate();

                resultWindow.AddChild(new TextBlock() { Text = total + " / " + best.Item1 + " = " + ((double)total / best.Item1) });
            }
        }

        async void _mainMenu_DoSolveLP()
        {
            if (SelectedTabCanvas == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                var blob = AlgorithmBlob.Create(SelectedTabCanvas);
                var p = blob.UIGraph.Vertices.Select(v => new Vector(v.X, v.Y)).ToList();

                var diamonds = SpindleAnalyzer.FindDiamonds(blob, p);
                var w = blob.AlgorithmGraph.Vertices.Select(v => blob.UIGraph.Vertices[v].Label).ToList();
                
            /*    var totalFormula = SpindleAnalyzer.ComputeTotalWeightFormula(blob, p, diamonds, w);
                var constraints = await Task.Factory.StartNew<List<string>>(() => SpindleAnalyzer.ComputeLPConstraints(blob, p, diamonds, w));

                var sb = new StringBuilder();
                sb.AppendLine("Maximize t = " + totalFormula + " subject to");
                foreach (var c in constraints.Distinct())
                {
                    sb.AppendLine(c + " <= 1");
                }*/

                var code = await Task.Factory.StartNew<string>(() => SpindleAnalyzer.GenerateGLPKCode(blob, p, diamonds, w));

                resultWindow.AddChild(new TextBox() { Text = code });
            }
        }

        void ClearLabels()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            tabCanvas.GraphCanvas.DoClearLabels("", AlgorithmBlob.Create(SelectedTabCanvas).SelectedVertices);

            FocusSelectedTab();
        }
        void ExportTeX()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            var tikz = TeXConverter.ToTikz(tabCanvas.GraphCanvas.Graph);
            if (!string.IsNullOrEmpty(tikz))
            {
                ShowText(tikz);
            }
        }

        async void ExportWebLink()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            var json = tabCanvas.GraphCanvas.Graph.Serialize();
            var g = HttpUtility.UrlEncode(Utility.Compress(json));

            var url = @"https://dl.dropboxusercontent.com/u/8609833/Web/WebGraphs/WebGraphsTestPage.html?grid=" + tabCanvas.GraphCanvas.SnapToGrid + "&jib=" + g;

            try
            {
                var address = new System.Uri("http://tinyurl.com/api-create.php?url=" + url);
                var client = new System.Net.WebClient();
                url = await AsyncPlatformExtensions.DownloadStringTaskAsync(client, address);

            }
            catch { }

            if (!string.IsNullOrEmpty(url))
                ShowText(url);
        }

        async void ExportLightWebLink()
        {
            var tabCanvas = SelectedTabCanvas;
            if (tabCanvas == null)
                return;

            var json = tabCanvas.GraphCanvas.Graph.Serialize();
            var g = HttpUtility.UrlEncode(json);
            var url = @"https://dl.dropboxusercontent.com/u/8609833/Web/Playground/CanvasGraphs/canvasgraphs.html?graph=" + g;

            try
            {
                var address = new System.Uri("http://tinyurl.com/api-create.php?url=" + url);
                var client = new System.Net.WebClient();
                url = await AsyncPlatformExtensions.DownloadStringTaskAsync(client, address);
            }
            catch { }

            if (!string.IsNullOrEmpty(url))
                ShowText(url);
        }

        void ShowText(string s)
        {
            using (var r = new ResultWindow())
            {
                r.ClearChildren();
                var t = new TextBox();
                t.IsReadOnly = true;
                t.Text = s;
                t.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                t.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                t.Height = 250;
                r.AddChild(t);
                r.Show();
            }
        }
        void DoAdjacencyMatrix()
        {
            ShowText(SelectedTabCanvas.GraphCanvas.Graph.GetEdgeWeights().ToAdjacencyMatrix(SelectedTabCanvas.GraphCanvas.Graph.IsDirected));
        }
        void DoEdgeWeights()
        {
            ShowText(string.Join(" ", SelectedTabCanvas.GraphCanvas.Graph.GetEdgeWeights()));
        }
        void DoGraph6()
        {
            ShowText(SelectedTabCanvas.GraphCanvas.Graph.GetEdgeWeights().ToGraph6());
        }

        void _mainMenu_DoGridToggle()
        {
            if (SelectedTabCanvas == null)
                return;

            SelectedTabCanvas.GraphCanvas.SnapToGrid = !SelectedTabCanvas.GraphCanvas.SnapToGrid;
            SelectedTabCanvas.GraphCanvas.DrawGrid = SelectedTabCanvas.GraphCanvas.SnapToGrid;
        }

        async void CountEulerianSubgraphs()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                int even = 0;
                int odd = 0;

                await Task.Factory.StartNew(() =>
                {
                    if (blob.SelectedVertices.Count > 0)
                        blob.AlgorithmGraph.CountSpanningEulerianSubgraphsUsingVertices(blob.SelectedVertices, out even, out odd);
                    else if (blob.SelectedEdges.Count > 0)
                        blob.AlgorithmGraph.CountSpanningEulerianSubgraphsUsingEdges(blob.SelectedEdges, out even, out odd);
                    else
                        blob.AlgorithmGraph.CountSpanningEulerianSubgraphs(out even, out odd);
                });

                resultWindow.AddChild(new TextBlock() { Text = "even:\t\t" + even + Environment.NewLine + "odd:\t\t" + odd });
            }
        }
        async void CheckD1AT()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                int even = 0;
                int odd = 0;
                Choosability.Graph orientation = null;

                if (blob.AlgorithmGraph.N >= 2)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        foreach (var o in blob.AlgorithmGraph.EnumerateOrientations(v => 2 - (blob.UIGraph.Vertices[v].IsSelected ? 1 : 0) - blob.UIGraph.Vertices[v].Modifier))
                        {
                            o.CountSpanningEulerianSubgraphs(out even, out odd);

                            if (even != odd)
                            {
                                orientation = o;
                                break;
                            }
                        }
                    });
                }

                if (orientation == null)
                {
                    resultWindow.AddChild(new TextBlock() { Text = "not d1-AT" });
                }
                else
                {
                    blob.UIGraph.ModifyOrientation(orientation.GetEdgeWeights());
                    SelectedTabCanvas.Invalidate();

                    resultWindow.AddChild(new TextBlock() { Text = "is d1-AT" + Environment.NewLine + "even:\t\t" + even + Environment.NewLine + "odd:\t\t" + odd });
                }
            }
        }

        async void CheckfATUsingFormula()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                var listSizes = ParseListSizes(blob, resultWindow);
                if (listSizes == null)
                    return;

                int coefficient = 0;
                Choosability.Graph orientation = null;

                if (blob.AlgorithmGraph.N >= 2)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        foreach (var o in blob.AlgorithmGraph.EnumerateOrientations(v => blob.AlgorithmGraph.Degree(v) - listSizes[v] + 1))
                        {
                            var c = o.GetCoefficient(o.Vertices.Select(v => o.OutDegree(v)).ToArray());

                            if (c != 0)
                            {
                                coefficient = c;
                                orientation = o;
                                break;
                            }
                        }
                    });
                }

                if (orientation == null)
                {
                    resultWindow.AddChild(new TextBlock() { Text = "not f-AT" });
                }
                else
                {
                    blob.UIGraph.ModifyOrientation(orientation.GetEdgeWeights());
                    SelectedTabCanvas.Invalidate();

                    resultWindow.AddChild(new TextBlock() { Text = "is f-AT" + Environment.NewLine + "coefficient:\t\t" + coefficient });
                }
            }
        }

        bool ParseListAndDemandSizes(AlgorithmBlob blob, ResultWindow resultWindow, out List<int> f, out List<int> g)
        {
            f = new List<int>();
            g = new List<int>();

            foreach (var v in blob.AlgorithmGraph.Vertices)
            {
                var s = blob.UIGraph.Vertices[v].Label.Trim(' ', '(', ')');
                var parts = s.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                    goto fail;

                var fv = ParseDegreeDependentString(parts[0], blob.AlgorithmGraph.Degree(v));
                var gv = ParseDegreeDependentString(parts[1], blob.AlgorithmGraph.Degree(v));

                if (fv < 0 || gv < 0)
                    goto fail;

                f.Add(fv);
                g.Add(gv);

                continue;
        
            fail:
                resultWindow.AddChild(new TextBlock() { Text = "label vertices with desired list sizes and demands like (d:2)\nunable to parse " + blob.UIGraph.Vertices[v].Label });
                return false;
            }

            return true;
        }

        List<int> ParseListSizes(AlgorithmBlob blob, ResultWindow resultWindow)
        {
            var listSizes = new List<int>();
            foreach (var v in blob.AlgorithmGraph.Vertices)
            {
                var s = ParseDegreeDependentString(blob.UIGraph.Vertices[v].Label, blob.AlgorithmGraph.Degree(v));

                if (s < 0)
                {
                    resultWindow.AddChild(new TextBlock() { Text = "label vertices with desired list sizes\nunable to parse " + blob.UIGraph.Vertices[v].Label });
                    return null;
                }

                listSizes.Add(s);
            }
            return listSizes;
        }
        void ComputeCoefficient()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            var f = new ChooseTermWindow(blob);
            f.Show();  
        }
        void ComputeSignSum()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            var f = new ChooseTermWindow(blob, true);
            f.Show();  
        }
        async void GenerateBalancedOrientation()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            var orientation = await Task.Factory.StartNew<Choosability.Graph>(() =>
            {
                var d = blob.AlgorithmGraph.E / blob.AlgorithmGraph.N;

                return blob.AlgorithmGraph.EnumerateOrientations(v => Math.Min(blob.AlgorithmGraph.Degree(v), d)).Where(g => g.Vertices.All(w => g.InDegree(w) <= d + 1)).FirstOrDefault();
            });

            if (orientation != null)
            {
                blob.UIGraph.ModifyOrientation(orientation.GetEdgeWeights());
                SelectedTabCanvas.Invalidate();
            }
            else
            {
                using (var resultWindow = new ResultWindow())
                {
                    resultWindow.AddChild(new TextBlock() { Text = "no balanced orientation exists" });
                }
            }
        }

        void GenerateRandomOrientation()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            blob.UIGraph.ModifyOrientation(blob.AlgorithmGraph.GenerateRandomOrientation().GetEdgeWeights());
            SelectedTabCanvas.Invalidate();
        }

        void ClearOrientation()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            foreach (var v in blob.UIGraph.Edges)
                v.Orientation = Edge.Orientations.None;
            SelectedTabCanvas.Invalidate();
        }

        async void CheckfAT()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                var listSizes = ParseListSizes(blob, resultWindow);
                if (listSizes == null)
                    return;

                int even = 0;
                int odd = 0;
                Choosability.Graph orientation = null;

                if (blob.AlgorithmGraph.N >= 2)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        foreach (var o in blob.AlgorithmGraph.EnumerateOrientations(v => blob.AlgorithmGraph.Degree(v) - listSizes[v] + 1))
                        {
                            o.CountSpanningEulerianSubgraphs(out even, out odd);

                            if (even != odd)
                            {
                                orientation = o;
                                break;
                            }
                        }
                    });
                }

                if (orientation == null)
                {
                    resultWindow.AddChild(new TextBlock() { Text = "not f-AT" });
                }
                else
                {
                    blob.UIGraph.ModifyOrientation(orientation.GetEdgeWeights());
                    SelectedTabCanvas.Invalidate();

                    resultWindow.AddChild(new TextBlock() { Text = "is f-AT" + Environment.NewLine + "even:\t\t" + even + Environment.NewLine + "odd:\t\t" + odd });
                }
            }
        }


        async void CheckD1Paintable()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                var paintable = false;
                await Task.Factory.StartNew(() =>
                {
                    paintable = blob.AlgorithmGraph.IsOnlineFChoosable(v => blob.AlgorithmGraph.Degree(v) - (blob.UIGraph.Vertices[v].IsSelected ? 0 : 1) - blob.UIGraph.Vertices[v].Modifier);
                });

                resultWindow.AddChild(new TextBlock() { Text = (paintable ? "is d1-paintable" : "not d1-paintable") + Environment.NewLine + Choosability.Graph.NodesVisited + " nodes visited" + Environment.NewLine + Choosability.Graph.CacheHits + " cache hits" });
            }
        }
        async void CheckfPaintable()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                var listSizes = ParseListSizes(blob, resultWindow);
                if (listSizes == null)
                    return;

                var paintable = false;
                await Task.Factory.StartNew(() =>
                {
                    paintable = blob.AlgorithmGraph.IsOnlineFChoosable(v => listSizes[v]);
                });

                resultWindow.AddChild(new TextBlock() { Text = (paintable ? "is f-paintable" : "not f-paintable") + Environment.NewLine + Choosability.Graph.NodesVisited + " nodes visited" + Environment.NewLine + Choosability.Graph.CacheHits + " cache hits" });
            }
        }

        async void CheckFGPaintable()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                List<int> f;
                List<int> g;
                if (!ParseListAndDemandSizes(blob, resultWindow, out f, out g))
                    return;

                var paintable = false;
                await Task.Factory.StartNew(() =>
                {
                    paintable = blob.AlgorithmGraph.IsOnlineFGChoosable(v => f[v], v => g[v]);
                });

                resultWindow.AddChild(new TextBlock() { Text = (paintable ? "is (f:g)-paintable" : "not (f:g)-paintable") + Environment.NewLine + Choosability.Graph.NodesVisited + " nodes visited" + Environment.NewLine + Choosability.Graph.CacheHits + " cache hits" });
            }
        }
        
        async void Check2FoldD1Paintable()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                var paintable = false;
                await Task.Factory.StartNew(() =>
                {
                    paintable = blob.AlgorithmGraph.IsOnlineFGChoosable(v => 2 * blob.AlgorithmGraph.Degree(v) - (blob.UIGraph.Vertices[v].IsSelected ? 0 : 1) - blob.UIGraph.Vertices[v].Modifier, v => 2);
                });

                resultWindow.AddChild(new TextBlock() { Text = (paintable ? "is 2-fold d1-paintable" : "not 2-fold d1-paintable") + Environment.NewLine + Choosability.Graph.NodesVisited + " nodes visited" + Environment.NewLine + Choosability.Graph.CacheHits + " cache hits" });
            }
        }

        async void CheckfChoosable()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {
                var listSizes = ParseListSizes(blob, resultWindow);
                if (listSizes == null)
                    return;

                try
                {
                    var choosable = false;
                    List<List<int>> badAssignment = null;
                    long nodesVisited = 0;
                    long cacheHits = 0;
                    await Task.Factory.StartNew(() =>
                    {
                        choosable = blob.BitGraph.IsFChoosable(v => listSizes[v], out badAssignment, out nodesVisited, out cacheHits);
                    });

                    if (badAssignment != null)
                    {
                        var gr = blob.UIGraph.Clone();
                        for (int i = 0; i < gr.Vertices.Count; i++)
                            gr.Vertices[i].Label = string.Join(", ", badAssignment[i]);

                        AddTab(gr, "bad f-assignment");
                    }
                    resultWindow.AddChild(new TextBlock() { Text = (choosable ? "is f-choosable" : "not f-choosable") + Environment.NewLine + nodesVisited + " nodes visited" + Environment.NewLine + cacheHits + " cache hits" });
                }
                catch (Exception ex)
                {
                    resultWindow.AddChild(new TextBlock() { Text = ex.Message });
                }
            }
        }

        void DoSpringsLayout()
        {
            DoLayout((Layout.Algorithm)Layout.GetSpringsLayout);
        }

        void DoLaplacianLayout()
        {
            DoLayout((Layout.Algorithm)Layout.GetLaplacianLayout);
        }

        void DoWalkMatrixLayout()
        {
            DoLayout((Layout.Algorithm)Layout.GetWalkMatrixLayout);
        }
        
        void _mainMenu_DoUnitDistanceLayout()
        {
            if (SelectedTabCanvas == null)
                return;

            SelectedTabCanvas.GraphCanvas.SnapToGrid = false;
            SelectedTabCanvas.GraphCanvas.DrawGrid = SelectedTabCanvas.GraphCanvas.SnapToGrid;

            DoLayout((Layout.Algorithm)Layout.GetUnitDistanceLayout, SelectedTabCanvas.GraphCanvas.Graph.Vertices.Select(v => v.Location).ToList());
        }


        void DoLayout(Layout.Algorithm algorithm, List<Vector> layout = null)
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            var snap = SelectedTabCanvas.GraphCanvas.SnapToGrid;
            SelectedTabCanvas.GraphCanvas.SnapToGrid = false;
            var layoutAnimation = new LayoutAnimation(blob, () =>
            {
                SelectedTabCanvas.GraphCanvas.Invalidate();
            }
             , () =>
             {
                 SelectedTabCanvas.GraphCanvas.SnapToGrid = snap;
                 blob.UIGraph.ParametersDirty = true;
                 SelectedTabCanvas.GraphCanvas.GraphChanged();
                 SelectedTabCanvas.GraphCanvas.Invalidate();
             }, algorithm, layout);
        }

        int ParseDegreeDependentString(string p, int degree)
        {
            if (string.IsNullOrEmpty(p))
                return -1;

            var code = string.Format("var d = {0};{1};", degree, p);

            try
            {
                return Convert.ToInt32(HtmlPage.Window.Eval(code));
            }
            catch { }
            return -1;
        }

        void FindATNumber()
        {
        }
        void FindStrongComponents()
        {
        }
        void FindPaintNumber()
        {
        }

        void AnalyzeOnlyNearColoringsForSelectedEdge()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            if (blob.SelectedEdgeIndices.Count <= 0)
            {
                MessageBox.Show("no edges selected");
                return;
            }
            if (blob.SelectedEdgeIndices.Count >= 2)
            {
                MessageBox.Show("too many edges selected");
                return;
            }

            AnalyzeFixerBreaker(true, blob.SelectedEdgeIndices.First());
        }
        void AnalyzeOnlyNearColorings()
        {
            AnalyzeFixerBreaker(true);
        }
        void AnalyzeFixerBreaker()
        {
            AnalyzeFixerBreaker(false);
        }

        async void AnalyzeFixerBreaker(bool onlyNearColorings, int missingEdgeIndex = -1)
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            int potSize;
            try
            {
                potSize = blob.UIGraph.Vertices.Max(v => v.Label.TryParseInt().Value);
            }
            catch
            {
                return;
            }
            var G = blob.AlgorithmGraph;
            var template = new Template(G.Vertices.Select(v => potSize + G.Degree(v) - blob.UIGraph.Vertices[v].Label.TryParseInt().Value).ToList());

            var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(G);
            mind.MaxPot = potSize;
            mind.OnlyNearlyColorable = true;
            mind.MissingEdgeIndex = missingEdgeIndex;
            
            using (var resultWindow = new ResultWindow(true))
            {
                resultWindow.Show();
                var result = await Task.Factory.StartNew<string>(() =>
                {
                    if (Enumerable.Range(0, G.N).Any(i => template.Sizes[i] < G.Degree(i)))
                        return "some list is too small";

                    var win = mind.Analyze(template, resultWindow.OnProgress);

                    if (onlyNearColorings)
                    {
                        if (missingEdgeIndex >= 0)
                        {
                            if (mind.HasNonSuperabundantBoardThatIsNearlyColorable)
                            {
                                return "Breaker wins since there is a non-superabundant nearly colorable board";
                            }
                            else
                            {
                                var stats = mind.BoardCounts[0] + " nearly colorable boards for selected edge\n" + (mind.BoardCounts[0] - mind.BoardCounts[1]) + " colorable boards\n" + (mind.BoardCounts[1] - mind.BoardCounts[2]) + " non-superabundant boards\n";
                                for (int i = 3; i < mind.BoardCounts.Count - 1; i++)
                                    stats += (mind.BoardCounts[i - 1] - mind.BoardCounts[i]) + " depth " + (i - 2) + " boards\n";

                                var finalDifference = mind.BoardCounts[mind.BoardCounts.Count - 2] - mind.BoardCounts[mind.BoardCounts.Count - 1];
                                if (finalDifference == 0)
                                    stats += mind.BoardCounts[mind.BoardCounts.Count - 1] + " lost boards";
                                else
                                    stats += finalDifference + " depth " + (mind.BoardCounts.Count - 3) + " boards\n";

                                if (win)
                                    return "Fixer wins\n\n" + stats;
                                if (mind.FixerWonAllNearlyColorableBoards)
                                    return "Fixer wins on all nearly colorable boards (for some edge)\n\n" + stats;

                                return "Breaker wins\n\n" + stats;
                            }
                        }
                        else
                        {
                            if (mind.HasNonSuperabundantBoardThatIsNearlyColorable)
                            {
                                return "Breaker wins since there is a non-superabundant nearly colorable board";
                            }
                            else
                            {
                                var stats = mind.BoardCounts[0] + " nearly colorable boards\n" + (mind.BoardCounts[0] - mind.BoardCounts[1]) + " colorable boards\n" + (mind.BoardCounts[1] - mind.BoardCounts[2]) + " non-superabundant boards\n";
                                for (int i = 3; i < mind.BoardCounts.Count - 1; i++)
                                    stats += (mind.BoardCounts[i - 1] - mind.BoardCounts[i]) + " depth " + (i - 2) + " boards\n";

                                var finalDifference = mind.BoardCounts[mind.BoardCounts.Count - 2] - mind.BoardCounts[mind.BoardCounts.Count - 1];
                                if (finalDifference == 0)
                                    stats += mind.BoardCounts[mind.BoardCounts.Count - 1] + " lost boards";
                                else
                                    stats += finalDifference + " depth " + (mind.BoardCounts.Count - 3) + " boards\n";

                                if (win)
                                    return "Fixer wins\n\n" + stats;
                                if (mind.FixerWonAllNearlyColorableBoards)
                                    return "Fixer wins on all nearly colorable boards (for some edge)\n\n" + stats;

                                return "Breaker wins\n\n" + stats;
                            }
                        }
                    }
                    else
                    {
                        if (mind.HasNonSuperabundantBoardThatIsNearlyColorable)
                        {
                            return "Breaker wins since there is a non-superabundant nearly colorable board";
                        }
                        else
                        {
                            var stats = mind.BoardCounts[0] + " total boards\n" + (mind.BoardCounts[0] - mind.BoardCounts[1]) + " colorable boards\n" + (mind.BoardCounts[1] - mind.BoardCounts[2]) + " non-superabundant boards (none nearly colorable)\n";
                            for (int i = 3; i < mind.BoardCounts.Count - 1; i++)
                                stats += (mind.BoardCounts[i - 1] - mind.BoardCounts[i]) + " depth " + (i - 2) + " boards\n";

                            var finalDifference = mind.BoardCounts[mind.BoardCounts.Count - 2] - mind.BoardCounts[mind.BoardCounts.Count - 1];
                            if (finalDifference == 0)
                                stats += mind.BoardCounts[mind.BoardCounts.Count - 1] + " lost boards";
                            else
                                stats += finalDifference + " depth " + (mind.BoardCounts.Count - 3) + " boards\n";

                            if (win)
                                return "Fixer wins\n\n" + stats;
                            if (mind.FixerWonAllNearlyColorableBoards)
                                return "Fixer wins on all nearly colorable boards (for some edge)\n\n" + stats;

                            return "Breaker wins\n\n" + stats;
                        }
                    }
                });

                resultWindow.ClearChildren();

                var t = new TextBlock();
                t.Text = result;
                resultWindow.AddChild(t);

                if (mind.BreakerWonBoard != null)
                {
                    var gr = blob.UIGraph.Clone();
                    for (int i = 0; i < gr.Vertices.Count; i++)
                        gr.Vertices[i].Label = string.Join(", ", mind.BreakerWonBoard.Stacks.Value[i].ToSet());

                    AddTab(gr, ((TabItem)_tabControl.SelectedItem).Header + " (Breaker win)");
                }
            }
        }

        void OnThoughtProgress(ThoughtProgress p, VisualizationWindow w, GraphCanvas graphCanvas)
        {
            Dispatcher.BeginInvoke(() => DrawVisualization(p, w, graphCanvas));
            w.UpdateVisualization(p);
        }
        void DrawVisualization(ThoughtProgress p, VisualizationWindow w, GraphCanvas graphCanvas)
        {
            if (!graphCanvas.HasPainted)
                return;

            var graph = graphCanvas.Graph;

            Board board = null;
            if (p.BoardsAdded != null && p.BoardsAdded.Count > 0)
                board = p.BoardsAdded[0];
            else if (p.BoardsRemoved != null && p.BoardsRemoved.Count > 0)
                board = p.BoardsRemoved[0];

            if (board != null)
            {
                for (int i = 0; i < graph.Vertices.Count; i++)
                    graph.Vertices[i].Label = string.Join(", ", board[i].ToSet());
            }

            graphCanvas.DoZoomFit();
            DrawVisualizationCanvas(w, graphCanvas);
        }
        void DrawVisualizationCanvas(VisualizationWindow w, GraphCanvas graphCanvas)
        {
            var g = new Graphics(w.Canvas);
            graphCanvas.Paint(g, (int)w.Canvas.ActualWidth, (int)w.Canvas.ActualHeight);
        }

        async void DoMozhan()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            var visualizationGraph = blob.UIGraph.Clone();
            var graphCanvas = new GraphCanvas(visualizationGraph);
            graphCanvas.SnapToGrid = false;
            graphCanvas.DrawGrid = false;

            graphCanvas.DoClearLabels();
            foreach (var v in graphCanvas.Graph.Vertices)
            {
                v.Color = GraphicsLayer.ARGB.BasicPalette(-1);
                v.Padding = 1.0f / 50.0f;
            }

            var w = new MozhanWindow();
            w.HasCloseButton = false;
            w.Canvas.Loaded += (s, e) =>
            {
                PaintMozhan(graphCanvas, w.Canvas);
                PaintMozhan(graphCanvas, w.Canvas);
            };
            w.Show();

            var mozhan = new Mozhan(blob.AlgorithmGraph);
            mozhan.PauseTime_ms = 500;
            mozhan.OnGreedyColored += (v, coloring) => mozhan_OnGreedyColored(mozhan, graphCanvas, w.Canvas, v, coloring);
            mozhan.OnKempeChainPulled += (path, coloring) => mozhan_OnKempeChainPulled(mozhan, graphCanvas, w.Canvas, path, coloring);
            mozhan.OnLevelSwap += (vIn, vOut, coloring) => mozhan_OnLevelSwap(mozhan, graphCanvas, w.Canvas, vIn, vOut, coloring);
            var result = await Task.Factory.StartNew(() => mozhan.TryColor());

            if (!result)
            {
                var G = graphCanvas.Graph;
                G.UnselectAll();

                foreach (var x in mozhan.BigClique)
                    foreach (var y in mozhan.BigClique)
                    {
                        var e = G.GetEdge(G.Vertices[x], G.Vertices[y]);
                        if (e != null)
                            e.Color = new GraphicsLayer.ARGB(255, 131, 0);
                    }

                PaintMozhan(graphCanvas, w.Canvas);
            }

            w.HasCloseButton = true;
        }

        void mozhan_OnLevelSwap(Mozhan mozhan, GraphCanvas graphCanvas, Canvas canvas, int vIn, int vOut, List<List<int>> coloring)
        {
            if (!graphCanvas.HasPainted)
                return;

            SetText("performing balanced swap", canvas);
            ClearEdgeColors(graphCanvas, canvas);

            var G = graphCanvas.Graph;
            var e = G.GetEdge(G.Vertices[vIn], G.Vertices[vOut]);
            e.Color = new GraphicsLayer.ARGB(0, 255, 0);
            e.Thickness = 4;
            PaintMozhan(graphCanvas, canvas);

            ApplyColoring(coloring, graphCanvas, canvas);
            PaintMozhan(graphCanvas, canvas);

            mozhan.Pause();

            ApplyColoring(coloring, graphCanvas, canvas);
            PaintMozhan(graphCanvas, canvas);
            ClearEdgeColors(graphCanvas, canvas);
        }

        void mozhan_OnKempeChainPulled(Mozhan mozhan, GraphCanvas graphCanvas, Canvas canvas, List<int> path, List<List<int>> coloring)
        {
            if (!graphCanvas.HasPainted)
                return;

            SetText("pulling kempe chain", canvas);
            ClearEdgeColors(graphCanvas, canvas);

            var G = graphCanvas.Graph;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var e = G.GetEdge(G.Vertices[path[i]], G.Vertices[path[i + 1]]);
                e.Color = new GraphicsLayer.ARGB(0, 255, 0);
                e.Thickness = 4;
            }
            PaintMozhan(graphCanvas, canvas);

            mozhan.Pause();
            
            ApplyColoring(coloring, graphCanvas, canvas);
            PaintMozhan(graphCanvas, canvas);
            ClearEdgeColors(graphCanvas, canvas);
        }

        void mozhan_OnGreedyColored(Mozhan mozhan, GraphCanvas graphCanvas, Canvas canvas, int v, List<List<int>> coloring)
        {
            if (!graphCanvas.HasPainted)
                return;

            SetText("greedy colored vertex", canvas);
            ClearEdgeColors(graphCanvas, canvas);
            ApplyColoring(coloring, graphCanvas, canvas);

            PaintMozhan(graphCanvas, canvas);
        }

        void ApplyColoring(List<List<int>> coloring, GraphCanvas graphCanvas, Canvas canvas)
        {
            var G = graphCanvas.Graph;
            foreach (var v in Enumerable.Range(0, G.Vertices.Count))
            {
                var c = coloring.FirstIndex(l => l.Contains(v));
                G.Vertices[v].Color = GraphicsLayer.ARGB.BasicPalette(c);
            }

            PaintMozhan(graphCanvas, canvas);
        }

        void PaintMozhan(GraphCanvas graphCanvas, Canvas canvas)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var g = new Graphics(canvas);
                graphCanvas.Paint(g, (int)canvas.ActualWidth, (int)canvas.ActualHeight);
                graphCanvas.DoZoomFit();
            });
        }

        void SetText(string text, Canvas canvas)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    var w = canvas.Parent as ChildWindow;
                    w.Title = text;
                });
        }

        void ClearEdgeColors(GraphCanvas graphCanvas, Canvas canvas)
        {
            var G = graphCanvas.Graph;
            foreach (var e in G.Edges)
            {
                e.Color = new GraphicsLayer.ARGB(0, 0, 0);
                e.Thickness = 2;
            }

            PaintMozhan(graphCanvas, canvas);
        }

        async void FindGood3Partition()
        {
            var blob = AlgorithmBlob.Create(SelectedTabCanvas);
            if (blob == null)
                return;

            using (var resultWindow = new ResultWindow())
            {

                var r = await Task.Factory.StartNew<List<Tuple<List<int>, List<int>>>>(() =>
                                {
                                    return MinorFinder.FindWithJ(blob.AlgorithmGraph, blob.UIGraph.SelectedVertices.Select(sv => blob.UIGraph.Vertices.IndexOf(sv)).ToList()).FirstOrDefault();
                                });

                if (r == null)
                {
                    resultWindow.AddChild(new TextBlock() { Text = "no good 3-partitions" });
                }
                else
                {
                    foreach (var v in blob.UIGraph.Vertices)
                        v.Label = "";

                    for(int i = 0; i < 3; i++)
                    {
                        foreach(var v in r[i].Item1)
                            blob.UIGraph.Vertices[v].Label += (i + 1) + ",";
                        foreach(var v in r[i].Item2)
                            blob.UIGraph.Vertices[v].Label += "I_" + (i + 1) + ",";
                    }

                    foreach(var v in blob.UIGraph.Vertices)
                        v.Label = v.Label.TrimEnd(',');

                    SelectedTabCanvas.Invalidate();
                    resultWindow.AddChild(new TextBlock() { Text = "marked a good 3-partition on the graph" });
                }
            }
        }
        #endregion
    }
}

