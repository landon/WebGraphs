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
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;

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
        public event Action OnAddClockSpindle;
        public event Action OnAddCClockSpindle;
        public event Action OnNextDeepestBoard;
        public event Action ExtendTriangulation;

        public event Action<bool, int, FixerBreakerSwapMode, bool, bool, bool, FixerBreakeReductionMode> Analyze;
        public event Action<bool, int, FixerBreakerSwapMode, bool, bool, bool, FixerBreakeReductionMode> AnalyzeCurrentBoard;
        public event Action<bool, int, FixerBreakerSwapMode, bool, bool, bool, FixerBreakeReductionMode> GenenerateBoard;

        public MenuBar()
        {
            InitializeComponent();
            LoadSettings();
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
                case "add clock spindle":
                    A(OnAddClockSpindle);
                    break;
                case "add cclock spindle":
                    A(OnAddCClockSpindle);
                    break;
                case "analyze fixed Δ":
                    A(Analyze, GetNearColoring(), GetExtraPsi(), GetSwapMode(), AllowAllIntermediateMode(), false, false, GetReductionMode());
                    break;
                case "analyze superabundant only":
                    A(Analyze, GetNearColoring(), GetExtraPsi(), GetSwapMode(), AllowAllIntermediateMode(), true, false, GetReductionMode());
                    break;
                case "generate proof fixed Δ":
                    A(Analyze, GetNearColoring(), GetExtraPsi(), GetSwapMode(), AllowAllIntermediateMode(), false, true, GetReductionMode());
                    break;
                case "analyze current board":
                    A(AnalyzeCurrentBoard, GetNearColoring(), GetExtraPsi(), GetSwapMode(), AllowAllIntermediateMode(), true, false, GetReductionMode());
                    break;
                case "generate deepest board":
                    A(GenenerateBoard, GetNearColoring(), GetExtraPsi(), GetSwapMode(), AllowAllIntermediateMode(), true, false, GetReductionMode());
                    break;
                case "original mode":
                    _fixerBreakerModeItem.Header = "single swap mode";
                    SaveSettings();
                    break;
                case "single swap mode":
                    _fixerBreakerModeItem.Header = "multi swap mode";
                    SaveSettings();
                    break;
                case "multi swap mode":
                    _fixerBreakerModeItem.Header = "original mode";
                    SaveSettings();
                    break;
                case "restrict intermediate boards mode":
                    _fixerBreakerIntermediateModeItem.Header = "allow all intermediate boards mode";
                    SaveSettings();
                    break;
                case "allow all intermediate boards mode":
                    _fixerBreakerIntermediateModeItem.Header = "restrict intermediate boards mode";
                    SaveSettings();
                    break;
                case "next deepest board":
                    A(OnNextDeepestBoard);
                    break;
                case "no reductions":
                    _fixerBreakerReductionModeItem.Header = "superabundant reductions";
                    SaveSettings();
                    break;
                case "superabundant reductions":
                    _fixerBreakerReductionModeItem.Header = "definite reductions";
                    SaveSettings();
                    break;
                case "definite reductions":
                    _fixerBreakerReductionModeItem.Header = "no reductions";
                    SaveSettings();
                    break;
                case "extra psi 0":
                    _extraPsiItem.Header = "extra psi 1";
                    SaveSettings();
                    break;
                case "extra psi 1":
                    _extraPsiItem.Header = "extra psi 0";
                    SaveSettings();
                    break;
                case "near colorings only":
                    _nearColoringItem.Header = "all assignments";
                    SaveSettings();
                    break;
                case "all assignments":
                    _nearColoringItem.Header = "near colorings only";
                    SaveSettings();
                    break;
                case "extend triangulation":
                    A(ExtendTriangulation);
                    break;
            }
        }

        void SaveSettings()
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_fixerBreakerReductionModeItem.Header"] = _fixerBreakerReductionModeItem.Header;
            System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_extraPsiItem.Header"] = _extraPsiItem.Header;
            System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_nearColoringItem.Header"] = _nearColoringItem.Header;
            System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_fixerBreakerModeItem.Header"] = _fixerBreakerModeItem.Header;
            System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_fixerBreakerIntermediateModeItem.Header"] = _fixerBreakerIntermediateModeItem.Header;
        }

        void LoadSettings()
        {
            if (System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings.Contains("_fixerBreakerReductionModeItem.Header"))
                _fixerBreakerReductionModeItem.Header = (string)System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_fixerBreakerReductionModeItem.Header"];
            if (System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings.Contains("_extraPsiItem.Header"))
                _extraPsiItem.Header = (string)System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_extraPsiItem.Header"];
            if (System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings.Contains("_nearColoringItem.Header"))
                _nearColoringItem.Header = (string)System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_nearColoringItem.Header"];
            if (System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings.Contains("_fixerBreakerModeItem.Header"))
                _fixerBreakerModeItem.Header = (string)System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_fixerBreakerModeItem.Header"];
            if (System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings.Contains("_fixerBreakerIntermediateModeItem.Header"))
                _fixerBreakerIntermediateModeItem.Header = (string)System.IO.IsolatedStorage.IsolatedStorageSettings.SiteSettings["_fixerBreakerIntermediateModeItem.Header"];
        }

        bool GetNearColoring()
        {
            switch ((string)_nearColoringItem.Header)
            {
                case "near colorings only":
                    return true;
                case "all assignments":
                    return false;
            }

            return false;
        }

        int GetExtraPsi()
        {
            switch ((string)_extraPsiItem.Header)
            {
                case "extra psi 0":
                    return 0;
                case "extra psi 1":
                    return 1;
            }

            return 0;
        }

        FixerBreakeReductionMode GetReductionMode()
        {
            switch ((string)_fixerBreakerReductionModeItem.Header)
            {
                case "no reductions":
                    return FixerBreakeReductionMode.None;
                case "superabundant reductions":
                    return FixerBreakeReductionMode.Superabundant;
                case "definite reductions":
                    return FixerBreakeReductionMode.Definite;
            }

            return FixerBreakeReductionMode.None;
        }

        FixerBreakerSwapMode GetSwapMode()
        {
            switch ((string)_fixerBreakerModeItem.Header)
            {
                case "original mode":
                    return FixerBreakerSwapMode.Original;
                case "single swap mode":
                    return FixerBreakerSwapMode.SingleSwap;
                case "multi swap mode":
                    return FixerBreakerSwapMode.MultiSwap;
            }

            return FixerBreakerSwapMode.SingleSwap;
        }

        bool AllowAllIntermediateMode()
        {
            return (string)_fixerBreakerIntermediateModeItem.Header == "allow all intermediate boards mode";
        }

        static void A(Action a)
        {
            var b = a;
            if (b != null)
                b();
        }
        static void A<T>(Action<T> a, T t)
        {
            var b = a;
            if (b != null)
                b(t);
        }
        static void A<T,S>(Action<T,S> a, T t, S s)
        {
            var b = a;
            if (b != null)
                b(t,s);
        }
        static void A<T, S, R>(Action<T, S, R> a, T t, S s, R r)
        {
            var b = a;
            if (b != null)
                b(t, s, r);
        }
        static void A<T, S, R, P>(Action<T, S, R, P> a, T t, S s, R r, P p)
        {
            var b = a;
            if (b != null)
                b(t, s, r, p);
        }
        static void A<T, S, R, P, Q, Z>(Action<T, S, R, P, Q, Z> a, T t, S s, R r, P p, Q q, Z z)
        {
            var b = a;
            if (b != null)
                b(t, s, r, p, q, z);
        }
        static void A<T, S, R, P, Q, Z, M>(Action<T, S, R, P, Q, Z, M> a, T t, S s, R r, P p, Q q, Z z, M m)
        {
            var b = a;
            if (b != null)
                b(t, s, r, p, q, z, m);
        }
    }
}
