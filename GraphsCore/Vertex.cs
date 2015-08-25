using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Runtime.Serialization;
using GraphsCore.Fake;

namespace Graphs
{
    public class Vertex : GraphicsLayer.IPaintable, IHittable
    {
        static readonly GraphicsLayer.Font LabelFont = new GraphicsLayer.Font("Times New Roman", 12);
        static readonly GraphicsLayer.ARGB LabelBrushColor = new GraphicsLayer.ARGB(0, 0, 0);
        static readonly GraphicsLayer.ARGB BoundaryPenColor = new GraphicsLayer.ARGB(0, 0, 0);
        static readonly GraphicsLayer.ARGB BoundarySelectedPenColor = new GraphicsLayer.ARGB(120, 0, 255, 0);
        static readonly int BoundarySelectedPenWidth = 5;
        static readonly GraphicsLayer.ARGB DefaultFillBrushColor = new GraphicsLayer.ARGB(120, 0, 0, 0);
        static readonly GraphicsLayer.ARGB UniversalVertexFillBrushColor = new GraphicsLayer.ARGB(120, 0, 0, 255);
        static readonly GraphicsLayer.ARGB SelectedFillBrushColor = new GraphicsLayer.ARGB(255, 0, 255, 127);

        public Vertex(double x, double y)
            : this(x, y, "")
        {
        }

        public Vertex(Vector v)
            : this(v, "")
        {
        }

        public Vertex(Vector v, string label)
            : this(v.X, v.Y, label)
        {
        }

        public Vertex(double x, double y, string label)
        {
            _Location = new Vector(x, y);
            _label = label;
        }

        public Vertex(SerializationVertex v)
        {
            Location = new Vector(v.Location.X, v.Location.Y);
            Label = v.Label;
            Padding = v.Padding;
            Style = v.Style;
        }

        public virtual void Paint(GraphicsLayer.IGraphics g, int width, int height)
        {
            var bounds = ComputeBounds(g, width, height);

            if (!string.IsNullOrEmpty(_label))
            {
                if (Color.Equals(default(GraphicsLayer.ARGB)))
                {
                    if (IsUniversal)
                        g.FillEllipse(UniversalVertexFillBrushColor, bounds);
                }
                else
                {
                    g.FillEllipse(Color, bounds);
                }

                g.DrawEllipse(_IsSelected ? BoundarySelectedPenColor : BoundaryPenColor, bounds, _IsSelected ? BoundarySelectedPenWidth : 1);
                g.DrawString(_label, LabelFont, LabelBrushColor, bounds);
            }
            else
            {
                if (Color.Equals(default(GraphicsLayer.ARGB)))
                    g.FillEllipse(IsUniversal ? UniversalVertexFillBrushColor : DefaultFillBrushColor, bounds);
                else
                    g.FillEllipse(Color, bounds);

                g.DrawEllipse(_IsSelected ? BoundarySelectedPenColor : BoundaryPenColor, bounds, _IsSelected ? BoundarySelectedPenWidth : 1);
            }
        }

        public bool Hit(double x, double y)
        {
            return _LocalBounds.Contains(x, y);
        }

        public GraphicsLayer.Box ComputeBounds(GraphicsLayer.IGraphics g, int width, int height)
        {
            GraphicsLayer.Box bounds;

            if (!string.IsNullOrEmpty(_label))
            {
                var size = g.MeasureString(_label, LabelFont);

                bounds = new GraphicsLayer.Box(X * width - size.Width / 2, Y * height - size.Height / 2, size.Width, size.Height);
            }
            else
            {
                bounds = new GraphicsLayer.Box(X * width, Y * height, 0, 0);
            }

            bounds.Inflate(_padding * width, _padding * height);

            _LocalBounds = new GraphicsLayer.Box(bounds.X / width, bounds.Y / height, bounds.Width / width, bounds.Height / height);

            return bounds;
        }

        [Browsable(false)]
        public GraphicsLayer.ARGB Color { get; set; }

        [Browsable(false)]
        public int Modifier { get; set; }

        [Browsable(false)]
        public double X
        {
            get
            {
                return _Location.X;
            }
            set
            {
                _Location.X = value;
            }
        }

        [Browsable(false)]
        public double Y
        {
            get
            {
                return _Location.Y;
            }
            set
            {
                _Location.Y = value;
            }
        }

        [Browsable(false)]
        public Vector Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
            }
        }

        [Browsable(true)]
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                if (value == _label)
                    return;

                _label = value;
            }
        }

        [Browsable(true)]
        public float Padding
        {
            get
            {
                return _padding;
            }
            set
            {
                if (value == _padding)
                    return;

                _padding = value;
            }
        }

        [Browsable(false)]
        public GraphicsLayer.Box LocalBounds
        {
            get
            {
                return _LocalBounds;
            }
        }

        [Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (value == _IsSelected)
                    return;

                _IsSelected = value;
            }
        }

        [Browsable(false)]
        public GraphicsLayer.Box DragOffset
        {
            get
            {
                return _DragOffset;
            }
            set
            {
                _DragOffset = value;
            }
        }

        [Browsable(true)]
        public string Style
        {
            get
            {
                return _Style;
            }
            set
            {
                if (value.StartsWith("+") && !string.IsNullOrEmpty(_Style))
                    _Style += ", " + value.TrimStart('+');
                else
                    _Style = value.TrimStart('+');
            }
        }

        [Browsable(false)]
        public bool IsUniversal { get; set; }

        Vector _Location;
        GraphicsLayer.Box _DragOffset = GraphicsLayer.Box.Empty;
        string _label;
        string _Style;
        bool _IsSelected;

        float _padding = 1.0f / 100.0f;
        GraphicsLayer.Box _LocalBounds = GraphicsLayer.Box.Empty;
    }
}
