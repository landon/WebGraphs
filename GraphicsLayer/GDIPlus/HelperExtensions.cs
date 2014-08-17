using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsLayer.GDIPlus
{
    public static class HelperExtensions
    {
        public static Color ToColor(this ARGB argb)
        {
            return Color.FromArgb(argb.A, argb.R, argb.G, argb.B);
        }
        public static RectangleF ToRectangleF(this Box box)
        {
            return new RectangleF((float)box.Left, (float)box.Top, (float)box.Width, (float)box.Height);
        }
        public static Box ToBox(this RectangleF rectangleF)
        {
            return new Box() { Top = rectangleF.Y, Left = rectangleF.X, Width = rectangleF.Width, Height = rectangleF.Height };
        }
        public static Box ToBox(this SizeF sizeF)
        {
            return new Box() { Width = sizeF.Width, Height = sizeF.Height };
        }
        public static PointF ToPointF(this Box box)
        {
            return new PointF((float)box.Left, (float)box.Top);
        }
        public static IEnumerable<PointF> ToPointF(this IEnumerable<Box> boxes)
        {
            return boxes.Select(box => box.ToPointF());
        }
    }
}
