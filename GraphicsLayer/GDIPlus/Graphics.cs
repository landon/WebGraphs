using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsLayer.GDIPlus
{
    public class Graphics : IGraphics
    {
        System.Drawing.Graphics _graphics;

        public Graphics(System.Drawing.Graphics graphics)
        {
            _graphics = graphics;
        }

        public void Clear(ARGB argb)
        {
            _graphics.Clear(argb.ToColor());
        }
        public void DrawLine(ARGB argb, Box p1, Box p2, double width = 1)
        {
            using (var pen = new Pen(argb.ToColor(), (float)width))
                _graphics.DrawLine(pen, p1.ToPointF(), p2.ToPointF());
        }
        public void DrawLine(ARGB argb, double x1, double y1, double x2, double y2, double width = 1)
        {
            DrawLine(argb, new Box(x1, y1), new Box(x2, y2), width);
        }
        public void DrawLines(ARGB argb, IEnumerable<Box> points, double width = 1)
        {
            using (var pen = new Pen(argb.ToColor(), (float)width))
                _graphics.DrawLines(pen, points.ToPointF().ToArray());
        }
        public void FillPolygon(ARGB argb, IEnumerable<Box> points)
        {
            using (var brush = new SolidBrush(argb.ToColor()))
                _graphics.FillPolygon(brush, points.ToPointF().ToArray());
        }
        public void DrawEllipse(ARGB argb, Box bounds, double width = 1)
        {
            using (var pen = new Pen(argb.ToColor(), (float)width))
                _graphics.DrawEllipse(pen, bounds.ToRectangleF());
        }
        public void FillEllipse(ARGB argb, Box bounds)
        {
            using (var brush = new SolidBrush(argb.ToColor()))
                _graphics.FillEllipse(brush, bounds.ToRectangleF());
        }
        public void DrawString(string s, Font font, ARGB argb, Box bounds)
        {
            using (var brush = new SolidBrush(argb.ToColor()))
            using (var gdiFont = new System.Drawing.Font(font.Name, font.Size))
            {
                var format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                _graphics.DrawString(s, gdiFont, brush, bounds.ToRectangleF(), format);
            }
        }
        public Box MeasureString(string s, Font font)
        {
            using (var gdiFont = new System.Drawing.Font(font.Name, font.Size))
                return _graphics.MeasureString(s, gdiFont).ToBox();
        }
    }
}
