using GraphicsLayer;
using System;
using System.Collections.Generic;
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
using System.Linq;

namespace WebGraphs
{
    public class FastGraphics : IGraphics
    {
        static Dictionary<ARGB, Brush> _brushLookup = new Dictionary<ARGB, Brush>();
        Dictionary<Type, Tuple<List<FrameworkElement>, List<FrameworkElement>>> _typedChildren;

        Canvas _canvas;
        ARGB _background;
        int _lastZIndex;

        public FastGraphics(Canvas canvas, Dictionary<Type, Tuple<List<FrameworkElement>, List<FrameworkElement>>> typedChildren)
        {
            _canvas = canvas;
            _typedChildren = typedChildren;

            foreach (var kvp in _typedChildren)
            {
                kvp.Value.Item1.AddRange(kvp.Value.Item2);
                kvp.Value.Item2.Clear();
            }

            _lastZIndex = 0;
        }

        public void FinalizeGraphics()
        {
            foreach (var kvp in _typedChildren)
                foreach (var unused in kvp.Value.Item1)
                    unused.Visibility = Visibility.Collapsed;
        }

        public void Clear(ARGB argb)
        {
            if (argb.Equals(_background))
                return;

            _background = argb;
            _canvas.Background = GetBrush(_background);
        }

        public void DrawLine(ARGB argb, Box p1, Box p2, double width = 1)
        {
            DrawLine(argb, p1.X, p1.Y, p2.X, p2.Y, width);
        }

        public void DrawLine(ARGB argb, double x1, double y1, double x2, double y2, double width = 1)
        {
            var line = Create<Line>();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.StrokeThickness = width;
            line.Stroke = GetBrush(argb);
        }

        public void DrawLines(ARGB argb, System.Collections.Generic.IEnumerable<Box> points, double width = 1)
        {
            var brush = GetBrush(argb);

            var first = true;
            var previous = Box.Empty;
            foreach (var point in points)
            {
                if (first)
                {
                    previous = point;
                    first = false;
                    continue;
                }

                var line = Create<Line>();
                line.X1 = previous.X;
                line.Y1 = previous.Y;
                line.X2 = point.X;
                line.Y2 = point.Y;
                line.StrokeThickness = width;
                line.Stroke = brush;

                previous = point;
            }
        }

        public void FillPolygon(ARGB argb, System.Collections.Generic.IEnumerable<Box> points)
        {
            var poly = Create<Polygon>();
            poly.Points.Clear();

            foreach (var p in points)
                poly.Points.Add(new Point(p.X, p.Y));

            poly.Fill = GetBrush(argb);
        }

        public void DrawEllipse(ARGB argb, Box bounds, double width = 1)
        {
            var ellipse = Create<Ellipse>();
            ellipse.Fill = null;
            ellipse.Stroke = GetBrush(argb);
            ellipse.StrokeThickness = width;
            ellipse.Width = bounds.Width + 2 * width;
            ellipse.Height = bounds.Height + 2 * width;

            Canvas.SetLeft(ellipse, bounds.Left - width);
            Canvas.SetTop(ellipse, bounds.Top - width);
        }

        public void FillEllipse(ARGB argb, Box bounds)
        {
            var ellipse = Create<Ellipse>();
            ellipse.Stroke = null;
            ellipse.Fill = GetBrush(argb);
            ellipse.Width = bounds.Width;
            ellipse.Height = bounds.Height;

            Canvas.SetLeft(ellipse, bounds.Left);
            Canvas.SetTop(ellipse, bounds.Top);
        }

        public void DrawString(string s, Font font, ARGB argb, Box bounds)
        {
            var text = Create<TextBlock>();
            text.Text = s;
            text.FontFamily = new FontFamily(font.Name);
            text.FontSize = 1.3 * font.Size;
            text.Foreground = GetBrush(argb);
            text.FontWeight = FontWeights.ExtraBold;
            text.Measure(new Size(double.MaxValue, double.MaxValue));

            Canvas.SetLeft(text, bounds.Left + bounds.Width / 2 - text.ActualWidth / 2);
            Canvas.SetTop(text, bounds.Top + bounds.Height / 2 - text.ActualHeight / 2);
        }

        public void DrawRotatedString(string s, Font font, ARGB argb, Box bounds, double angle)
        {
            var text = Create<TextBlock>();
            text.Text = s;
            text.FontFamily = new FontFamily(font.Name);
            text.FontSize = 1.3 * font.Size;
            text.Foreground = GetBrush(argb);
            text.Measure(new Size(double.MaxValue, double.MaxValue));
            text.RenderTransform = new RotateTransform() { Angle = angle, CenterX = bounds.Width / 2, CenterY = bounds.Y / 2 };

            Canvas.SetLeft(text, bounds.Left + bounds.Width / 2 - text.ActualWidth / 2);
            Canvas.SetTop(text, bounds.Top + bounds.Height / 2 - text.ActualHeight / 2);
        }

        public Box MeasureString(string s, Font font)
        {
            var text = new TextBlock() { Text = s, FontFamily = new FontFamily(font.Name), FontSize = 1.3 * font.Size };
            text.Measure(new Size(double.MaxValue, double.MaxValue));

            return new Box(0, 0, text.ActualWidth, text.ActualHeight);
        }

        static Brush GetBrush(ARGB argb)
        {
            Brush brush;
            if (!_brushLookup.TryGetValue(argb, out brush))
            {
                _brushLookup[argb] = brush = new SolidColorBrush(argb.ToColor());
            }

            return brush;
        }

        T Create<T>()
            where T : FrameworkElement, new()
        {
            Tuple<List<FrameworkElement>, List<FrameworkElement>> children;
            if (!_typedChildren.TryGetValue(typeof(T), out children))
            {
                children = new Tuple<List<FrameworkElement>, List<FrameworkElement>>(new List<FrameworkElement>(), new List<FrameworkElement>());
                _typedChildren[typeof(T)] = children;
            }
            
            var t = children.Item1.FirstOrDefault() as T;

            if (t == null)
            {
                t = new T();
                _canvas.Children.Add(t);
            }
            else
            {
                children.Item1.Remove(t);
            }

            children.Item2.Add(t);

            _lastZIndex++;
            Canvas.SetZIndex(t, _lastZIndex);
            t.Visibility = Visibility.Visible;

            return t;
        }
    }
}
