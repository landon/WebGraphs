using GraphicsLayer;
using Graphs;
using System.Linq;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SLPropertyGrid.MultiObject;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;

namespace WebGraphs
{
    public class TabCanvas : ICanvas
    {
        string _title;
        public Canvas Canvas { get; private set; }
        public GraphCanvas Operations { get; private set; }
        SLPropertyGrid.PropertyGrid PropertyGrid { get; set; }
        Dictionary<Type, Tuple<List<FrameworkElement>, List<FrameworkElement>>> _typedChildren = new Dictionary<Type, Tuple<List<FrameworkElement>, List<FrameworkElement>>>();

        public TabCanvas(Canvas canvas, GraphCanvas graphCanvas, SLPropertyGrid.PropertyGrid propertyGrid)
        {
            Operations = graphCanvas;
            Canvas = canvas;
            Operations.Canvas = this;
            PropertyGrid = propertyGrid;

            Canvas.MouseLeftButtonDown += OnMouseLeftButtonDown;
            Canvas.MouseRightButtonDown += OnMouseRightButtonDown;
            Canvas.MouseLeftButtonUp += OnMouseLeftButtonUp;
            Canvas.MouseRightButtonUp += OnMouseRightButtonUp;
            Canvas.MouseMove += image_MouseMove;

            Canvas.Loaded += delegate { Invalidate(); };
            Canvas.SizeChanged += delegate { Invalidate(); };

            Operations.GraphModified += OnGraphModified;
        }

        void OnGraphModified(Graph g)
        {
            Storage.Save(g, Title);
        }

        void image_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(Canvas);
            if (MouseMoved != null)
                MouseMoved(p.X, p.Y);
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.CaptureMouse();

            var p = e.GetPosition(Canvas);

            if (e.ClickCount == 2)
            {
                if (MouseButtonDoubleClicked != null)
                    MouseButtonDoubleClicked(p.X, p.Y, MouseButton.Left);
            }
            else
            {
                if (MouseButtonDown != null)
                    MouseButtonDown(p.X, p.Y, MouseButton.Left);
            }
        }
        void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (e.ClickCount == 2)
            {
                if (MouseButtonDoubleClicked != null)
                    MouseButtonDoubleClicked(p.X, p.Y, MouseButton.Right);
            }
            else
            {
                if (MouseButtonDown != null)
                    MouseButtonDown(p.X, p.Y, MouseButton.Right);
            }
        }
        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Canvas.ReleaseMouseCapture();

            var p = e.GetPosition(Canvas);

            if (MouseButtonUp != null)
                MouseButtonUp(p.X, p.Y, MouseButton.Left);
        }
        void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (MouseButtonUp != null)
                MouseButtonUp(p.X, p.Y, MouseButton.Right);
        }

        public void SetClipboardText(string text)
        {
            try
            {
                Clipboard.SetText(text);
            }
            catch { }
        }

        public string GetClipboardText()
        {
            try
            {
                return Clipboard.GetText();
            }
            catch { }

            return "";
        }

        public bool IsControlKeyDown
        {
            get { return Keyboard.Modifiers == ModifierKeys.Control; }
        }

        public System.Collections.Generic.IEnumerable<object> SelectedObjects
        {
            set 
            {
                PropertyGrid.SelectedObject = new MultiObject(value).Representative;
            }
        }

        public void Invalidate()
        {
            var g = new FastGraphics(Canvas, _typedChildren);
            Canvas.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, Canvas.ActualWidth, Canvas.ActualHeight) };

            Operations.Paint(g, (int)Canvas.ActualWidth, (int)Canvas.ActualHeight);

            g.FinalizeGraphics();
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Invalidate();
            }
        }

        public event Action<double, double> MouseMoved;
        public event Action<double, double, MouseButton> MouseButtonUp;
        public event Action<double, double, MouseButton> MouseButtonDown;
        public event Action<double, double, MouseButton> MouseButtonDoubleClicked;
    }
}
