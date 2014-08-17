using GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WebGraphs
{
    public static class HelperExtensions
    {
        public static Color ToColor(this ARGB argb)
        {
            return Color.FromArgb((byte)argb.A, (byte)argb.R, (byte)argb.G, (byte)argb.B);
        }
    }
}
