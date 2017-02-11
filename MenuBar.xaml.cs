using SilverlightEnhancedMenuItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WebGraphs
{
    public partial class MenuBar : UserControl
    {
        public event Action NewClicked;
        public event Action OpenClicked;
        public event Action CloseClicked;
        public event Action CloseAllClicked;
        public event Action TogglePropertiesClicked;
        public event Action UndoClicked;
        public event Action RedoClicked;
        public event Action CutClicked;
        public event Action CopyClicked;
        public event Action PasteClicked;
        public event Action ZoomInClicked;
        public event Action ZoomOutClicked;
        public event Action ZoomFitClicked;
        public event Action LabelWithDegreesClicked;
        public event Action LabelWithInDegreesClicked;
        public event Action LabelWithOutDegreesClicked;
        public event Action LabelWithOutDegreesPlusOneClicked;
        public event Action ClearAllLabelsClicked;
        public event Action CountEulerianSubgraphs;
        public event Action CheckD1AT;
        public event Action FindATNumber;
        public event Action FindStrongComponents;
        public event Action CheckD1Paintable;
        public event Action Check2FoldD1Paintable;
        public event Action FindPaintNumber;
        public event Action ExportTeX;
        public event Action AnalyzeFixerBreaker;
        public event Action AnalyzeFixerBreakerWeaklyFixable;
        public event Action DoMozhan;
        public event Action DoWebLink;
        public event Action DoLightWebLink;
        public event Action CheckfAT;
        public event Action CheckfATUsingFormula;
        public event Action CheckfPaintable;
        public event Action LabelWithDegreeSymbol;
        public event Action LabelWithDegreeSymbolMinusOne;
        public event Action DoGraph6;
        public event Action DoEdgeWeights;
        public event Action DoAdjacencyMatrix;
        public event Action ComputeCoefficient;
        public event Action GenerateRandomOrientation;
        public event Action GenerateBalancedOrientation;
        public event Action DoSpringsLayout;
        public event Action DoUnitDistanceLayout;
        public event Action CheckfChoosable;
        public event Action OnInstructions;
        public event Action OnAbout;
        public event Action ComputeSignSum;
        public event Action ClearOrientation;
        public event Action AnalyzeOnlyNearColorings;
        public event Action AnalyzeOnlyNearColoringsForSelectedEdge;
        public event Action CheckFGPaintable;
        public event Action DoLaplacianLayout;
        public event Action DoWalkMatrixLayout;
        public event Action FindGood3Partition;
        public event Action DoGridToggle;
        public event Action DoMakeHexGrid;
        public event Action DoListExtraSpindleEdges;
        public event Action DoBasesAndTopsWeighting;
        public event Action DoMaxFractionalClique;
        public event Action DoSolveLP;
        public event Action DoSixFoldWay;
        public event Action DoTiling;
        public event Action DoSpin;
        public event Action DoSuperabundantOnly;
        public event Action DoSuperabundantOnlyWeakly;
        public event Action DoGenerateProof;
        public event Action DoGenerateProofSelectedEdge;
        public event Action DoGenerateProofSelectedEdgeUsePermutations;
        public event Action OnToggleFixerBreakerThinkHarder;
        public event Action DoSuperabundantOnlyNearColorings;
        public event Action OnAddClockSpindle;
        public event Action OnAddCClockSpindle;
        public event Action OnAnalyzeCurrentBoard;
        public event Action LookupIsomorphismClass;
        public event Action LaunchProofExplorer;



        public MenuBar()
        {
            InitializeComponent();
        }

        void MenuTopLevelClick(object sender, RoutedEventArgs e)
        {
            var menu = ContextMenuService.GetContextMenu((Button)sender);
            if (menu != null)
                menu.IsOpen = true;
        }

        void MenuTopLevelRightClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void MenuItemClick(object sender, RoutedEventArgs e)
        {
            var item = sender as SuperMenuItem;

            switch ((string)item.Header)
            {
                case "New":
                    A(NewClicked);
                    break;
                case "Open":
                    A(OpenClicked);
                    break;
                case "Close":
                    A(CloseClicked);
                    break;
                case "Close All":
                    A(CloseAllClicked);
                    break;
                case "Hide Properties":
                    var x = ContextMenuService.GetContextMenu((Button)_viewButton).Items[0] as SuperMenuItem;
                    x.Header = "Show Properties";
                    A(TogglePropertiesClicked);
                    break;
                case "Show Properties":
                    var y = ContextMenuService.GetContextMenu((Button)_viewButton).Items[0] as SuperMenuItem;
                    y.Header = "Hide Properties";
                    A(TogglePropertiesClicked);
                    break;
                case "Undo":
                    A(UndoClicked);
                    break;
                case "Redo":
                    A(RedoClicked);
                    break;
                case "Cut":
                    A(CutClicked);
                    break;
                case "Copy":
                    A(CopyClicked);
                    break;
                case "Paste":
                    A(PasteClicked);
                    break;
                case "In":
                    A(ZoomInClicked);
                    break;
                case "Out":
                    A(ZoomOutClicked);
                    break;
                case "Fit":
                    A(ZoomFitClicked);
                    break;
                case "with degrees":
                    A(LabelWithDegreesClicked);
                    break;
                case "with in degrees":
                    A(LabelWithInDegreesClicked);
                    break;
                case "with out degrees":
                    A(LabelWithOutDegreesClicked);
                    break;
                case "with out degrees plus one":
                    A(LabelWithOutDegreesPlusOneClicked);
                    break;
                case "clear selected":
                    A(ClearAllLabelsClicked);
                    break;
                case "count eulerian subgraphs":
                    A(CountEulerianSubgraphs);
                    break;
                case "check d1-AT (slow)":
                    A(CheckD1AT);
                    break;
                case "find AT number":
                    A(FindATNumber);
                    break;
                case "find strong components":
                    A(FindStrongComponents);
                    break;
                case "check d1-paintable":
                    A(CheckD1Paintable);
                    break;
                case "check 2-fold d1-paintable":
                    A(Check2FoldD1Paintable);
                    break;
                case "find paint number":
                    A(FindPaintNumber);
                    break;
                case "Export TeX":
                    A(ExportTeX);
                    break;
                case "analyze":
                    A(AnalyzeFixerBreaker);
                    break;
                case "analyze weakly fixable":
                    A(AnalyzeFixerBreakerWeaklyFixable);
                    break;
                case "Δ-1 color":
                    A(DoMozhan);
                    break;
                case "Export web link":
                    A(DoWebLink);
                    break;
                case "Export mobile web link":
                    A(DoLightWebLink);
                    break;
                case "check f-AT (slow)":
                    A(CheckfAT);
                    break;
                case "check f-AT using formula":
                    A(CheckfATUsingFormula);
                    break;
                case "check f-paintable":
                    A(CheckfPaintable);
                    break;
                case "selected with 'd'":
                    A(LabelWithDegreeSymbol);
                    break;
                case "selected with 'd-1'":
                    A(LabelWithDegreeSymbolMinusOne);
                    break;
                case "Export graph6":
                    A(DoGraph6);
                    break;
                case "Export edge weights":
                    A(DoEdgeWeights);
                    break;
                case "Export adjacency matrix":
                    A(DoAdjacencyMatrix);
                    break;
                case "compute coefficient":
                    A(ComputeCoefficient);
                    break;
                case "generate random orientation":
                    A(GenerateRandomOrientation);
                    break;
                case "generate balanced orientation":
                    A(GenerateBalancedOrientation);
                    break;
                case "springs":
                    A(DoSpringsLayout);
                    break;
                case "check f-choosable":
                    A(CheckfChoosable);
                    break;
                case "instructions":
                    A(OnInstructions);
                    break;
                case "about":
                    A(OnAbout);
                    break;
                case "compute sign sum":
                    A(ComputeSignSum);
                    break;
                case "clear orientation":
                    A(ClearOrientation);
                    break;
                case "analyze only near colorings":
                    A(AnalyzeOnlyNearColorings);
                    break;
                case "analyze only near colorings for selected edge":
                    A(AnalyzeOnlyNearColoringsForSelectedEdge);
                    break;
                case "check (f:g)-paintable":
                    A(CheckFGPaintable);
                    break;
                case "laplacian":
                    A(DoLaplacianLayout);
                    break;
                case "walk matrix":
                    A(DoWalkMatrixLayout);
                    break;
                case "find good 3-partition":
                    A(FindGood3Partition);
                    break;
                case "toggle grid":
                    A(DoGridToggle);
                    break;
                case "unit distance":
                    A(DoUnitDistanceLayout);
                    break;
                case "make hex grid":
                    A(DoMakeHexGrid);
                    break;
                case "list extra spindle edges":
                    A(DoListExtraSpindleEdges);
                    break;
                case "bases and tops weighting":
                    A(DoBasesAndTopsWeighting);
                    break;
                case "max fractional clique":
                    A(DoMaxFractionalClique);
                    break;
                case "generate LP":
                    A(DoSolveLP);
                    break;
                case "6-fold way":
                    A(DoSixFoldWay);
                    break;
                case "do tiling":
                    A(DoTiling);
                    break;
                case "spin":
                    A(DoSpin);
                    break;
                case "analyze superabundant only":
                    A(DoSuperabundantOnly);
                    break;
                case "analyze superabundant only near colorings":
                    A(DoSuperabundantOnlyNearColorings);
                    break;
                case "analyze superabundant only weakly":
                    A(DoSuperabundantOnlyWeakly);
                    break;
                case "generate proof":
                    A(DoGenerateProof);
                    break;
                case "generate proof only near colorings for selected edge":
                    A(DoGenerateProofSelectedEdge);
                    break;
                case "generate proof only near colorings for selected edge (use permutation magic)":
                    A(DoGenerateProofSelectedEdgeUsePermutations);
                    break;
                case "think harder":
                    ThinkHarderItem.Header = "think softer";
                    A(OnToggleFixerBreakerThinkHarder);
                    break;
                case "think softer":
                    ThinkHarderItem.Header = "think harder";
                    A(OnToggleFixerBreakerThinkHarder);
                    break;
                case "add clock spindle":
                    A(OnAddClockSpindle);
                    break;
                case "add cclock spindle":
                    A(OnAddCClockSpindle);
                    break;
                case "analyze current board":
                    A(OnAnalyzeCurrentBoard);
                    break;
                case "lookup isomorphism class":
                    A(LookupIsomorphismClass);
                    break;
                case "proof explorer":
                    A(LaunchProofExplorer);
                    break;
            }
        }
        static void A(Action a)
        {
            var b = a;
            if (b != null)
                b();
        }
    }
}
