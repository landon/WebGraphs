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

namespace WebGraphs
{
    public class TabCanvas : ICanvas
    {
        string _title;
        public Canvas Canvas { get; private set; }
        public GraphCanvas Operations { get; private set; }
        SLPropertyGrid.PropertyGrid PropertyGrid { get; set; }

        public TabCanvas(Canvas canvas, GraphCanvas graphCanvas, SLPropertyGrid.PropertyGrid propertyGrid)
        {
            Operations = graphCanvas;
            Canvas = canvas;
            Operations.Canvas = this;
            PropertyGrid = propertyGrid;

            Canvas.MouseLeftButtonDown += image_MouseLeftButtonDown;
            Canvas.MouseRightButtonDown += image_MouseRightButtonDown;
            Canvas.MouseLeftButtonUp += image_MouseLeftButtonUp;
            Canvas.MouseRightButtonUp += image_MouseRightButtonUp;
            Canvas.MouseMove += image_MouseMove;

            Canvas.Loaded += delegate { Invalidate(); };
            Canvas.SizeChanged += delegate { Invalidate(); };
        }

        void image_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(Canvas);
            if (MouseMoved != null)
                MouseMoved(p.X, p.Y);
        }

        void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
        void image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
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
        void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Canvas.ReleaseMouseCapture();

            var p = e.GetPosition(Canvas);

            if (MouseButtonUp != null)
                MouseButtonUp(p.X, p.Y, MouseButton.Left);
        }
        void image_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
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
                if (value != null && value.Count() == 1)
                    PropertyGrid.SelectedObject = value.FirstOrDefault();
                else
                    PropertyGrid.SelectedObject = null;
            }
        }

        public void Invalidate()
        {
            var g = new Graphics(Canvas);
            Canvas.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, Canvas.ActualWidth, Canvas.ActualHeight) };

            Operations.Paint(g, (int)Canvas.ActualWidth, (int)Canvas.ActualHeight);
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
