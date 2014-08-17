using GraphicsLayer;
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
    public class Graphics : IGraphics
    {
        Canvas _canvas;

        public Graphics(Canvas canvas)
        {
            _canvas = canvas;
            _canvas.Children.Clear();
        }

        public void Clear(ARGB argb)
        {
            _canvas.Background = new SolidColorBrush(argb.ToColor());
        }

        public void DrawLine(ARGB argb, Box p1, Box p2, double width = 1)
        {
            var line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, StrokeThickness = width, Stroke = new SolidColorBrush(argb.ToColor()) };
            _canvas.Children.Add(line);
        }

        public void DrawLine(ARGB argb, double x1, double y1, double x2, double y2, double width = 1)
        {
            var line = new Line() { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, StrokeThickness = width, Stroke = new SolidColorBrush(argb.ToColor()) };
            _canvas.Children.Add(line);
        }

        public void DrawLines(ARGB argb, System.Collections.Generic.IEnumerable<Box> points, double width = 1)
        {
            var brush = new SolidColorBrush(argb.ToColor());

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

                var line = new Line() { X1 = previous.X, Y1 = previous.Y, X2 = point.X, Y2 = point.Y, StrokeThickness = width, Stroke = brush };
                _canvas.Children.Add(line);

                previous = point;
            }
        }

        public void FillPolygon(ARGB argb, System.Collections.Generic.IEnumerable<Box> points)
        {
            var poly = new Polygon();
            foreach (var p in points)
                poly.Points.Add(new Point(p.X, p.Y));

            poly.Fill = new SolidColorBrush(argb.ToColor());

            _canvas.Children.Add(poly);
        }

        public void DrawEllipse(ARGB argb, Box bounds, double width = 1)
        {
            var ellipse = new Ellipse() { Stroke = new SolidColorBrush(argb.ToColor()), StrokeThickness = width, Width = bounds.Width + 2 * width, Height = bounds.Height + 2 * width };
            Canvas.SetLeft(ellipse, bounds.Left - width);
            Canvas.SetTop(ellipse, bounds.Top - width);
            _canvas.Children.Add(ellipse);
        }

        public void FillEllipse(ARGB argb, Box bounds)
        {
            var ellipse = new Ellipse() { Fill = new SolidColorBrush(argb.ToColor()), Width = bounds.Width, Height = bounds.Height };
            Canvas.SetLeft(ellipse, bounds.Left);
            Canvas.SetTop(ellipse, bounds.Top);
            _canvas.Children.Add(ellipse);
        }

        public void DrawString(string s, Font font, ARGB argb, Box bounds)
        {
            var text = new TextBlock() { Text = s, FontFamily = new FontFamily(font.Name), FontSize = 1.3 * font.Size, Foreground = new SolidColorBrush(argb.ToColor()) };
            text.Measure(new Size(double.MaxValue, double.MaxValue));

            Canvas.SetLeft(text, bounds.Left + bounds.Width / 2 - text.ActualWidth / 2);
            Canvas.SetTop(text, bounds.Top +  + bounds.Height / 2 - text.ActualHeight / 2);

            _canvas.Children.Add(text);
        }

        public Box MeasureString(string s, Font font)
        {
            var text = new TextBlock() { Text = s, FontFamily = new FontFamily(font.Name), FontSize = 1.3 * font.Size };
            text.Measure(new Size(double.MaxValue, double.MaxValue));

            return new Box(0, 0, text.ActualWidth, text.ActualHeight);
        }
    }
}
