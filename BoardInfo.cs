using Choosability.FixerBreaker;
using GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WebGraphs
{
    public class BoardInfo
    {
        public Board Board { get; set; }
        public Box InitialLocation { get; set; }
        public Color Color { get; set; }
        public bool IsRemoved { get; set; }
        public double T;
        public Shape Shape { get; set; }
    }
}
