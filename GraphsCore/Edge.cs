using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using System.ComponentModel;
using System.Runtime.Serialization;
using GraphsCore.Fake;

namespace Graphs
{
    public class Edge : GraphicsLayer.IPaintable, IHittable
    {
        public enum Orientations
        {
            None,
            Forward,
            Backward
        }
        public Edge(Vertex v1, Vertex v2)
            : this(v1, v2, Orientations.None)
        {
        }

        public Edge(Vertex v1, Vertex v2, Orientations orientation)
        {
            _V1 = v1;
            _V2 = v2;
            _Orientation = orientation;
            _Multiplicity = 1;
        }

        public Edge(SerializationEdge e, List<Vertex> vertices)
        {
            V1 = vertices[e.IndexV1];
            V2 = vertices[e.IndexV2];
            Orientation = e.Orientation;
            Thickness = e.Thickness;
            Multiplicity = e.Multiplicity;
            Style = e.Style;
        }

        public void Paint(GraphicsLayer.IGraphics g, int width, int height)
        {
            if (_V1 == _V2)
            {
                // Draw self loop.
            }
            else
            {
                var maxRadius1 = Math.Max(_V1.LocalBounds.Width, _V1.LocalBounds.Height) / 2;
                var maxRadius2 = Math.Max(_V2.LocalBounds.Width, _V2.LocalBounds.Height) / 2;

                var cos = (_V2.X - _V1.X) / Math.Sqrt((_V2.X - _V1.X) * (_V2.X - _V1.X) + (_V2.Y - _V1.Y) * (_V2.Y - _V1.Y));
                cos = Math.Min(Math.Max(cos, -1), 1);

                var angle = Math.Acos(cos);
                if (_V1.Y > _V2.Y)
                    angle = -angle;

                if (double.IsNaN(angle))
                    return;

                var angleStep = Math.PI / 2.0 / _Multiplicity;
                var evenModifier = (_Multiplicity % 2 == 0) ? 0.5 : 0;

                for (int i = -(_Multiplicity - 1) / 2; i < Math.Ceiling((_Multiplicity + 1) / 2.0); i++)
                {
                    var startAngle = angle + angleStep * (i - evenModifier);
                    var finishAngle = angle - angleStep * (i - evenModifier);

                    var start = LocalToGlobal(_V1.Location + Utility.PolarToRectangular(maxRadius1 + 0.01, startAngle), width, height);
                    var finish = LocalToGlobal(_V2.Location - Utility.PolarToRectangular(maxRadius2 + 0.01, finishAngle), width, height);

                    g.DrawLine(_penColor, start, finish, _Thickness);
                }

                if (_Orientation != Orientations.None)
                {
                    var points = new GraphicsLayer.Box[3];
                    if (_Orientation == Orientations.Forward)
                    {
                        var sweep = _V2.Location - Utility.PolarToRectangular(maxRadius2 + 0.025f, angle);

                        points[0] = LocalToGlobal(Utility.RotateAroundPoint(sweep, _V2.Location, Math.PI / 180.0 * 10.0), width, height);
                        points[1] = LocalToGlobal(_V2.Location - Utility.PolarToRectangular(maxRadius2 + 0.005f, angle), width, height);
                        points[2] = LocalToGlobal(Utility.RotateAroundPoint(sweep, _V2.Location, -Math.PI / 180.0 * 10.0), width, height);
                    }
                    else if (_Orientation == Orientations.Backward)
                    {
                        var sweep = _V1.Location + Utility.PolarToRectangular(maxRadius1 + 0.025f, angle);

                        points[0] = LocalToGlobal(Utility.RotateAroundPoint(sweep, _V1.Location, Math.PI / 180.0 * 10.0), width, height);
                        points[1] = LocalToGlobal(_V1.Location + Utility.PolarToRectangular(maxRadius1 + 0.005f, angle), width, height);
                        points[2] = LocalToGlobal(Utility.RotateAroundPoint(sweep, _V1.Location, -Math.PI / 180.0 * 10.0), width, height);
                    }

                    g.FillPolygon(new GraphicsLayer.ARGB(0, 0, 0), points);
                }
            }
        }

        GraphicsLayer.Box LocalToGlobal(Vector local, int width, int height)
        {
            return new GraphicsLayer.Box(width * local.X, height * local.Y);
        }

        public bool Hit(double x, double y)
        {
            return false;
        }

        public bool Meets(Edge other)
        {
            return _V1 == other.V1 ||
                   _V2 == other.V1 ||
                   _V1 == other.V2 ||
                   _V2 == other.V2;
        }

        [Browsable(false)]
        public Vertex V1
        {
            get
            {
                return _V1;
            }
            set
            {
                _V1 = value;
            }
        }

        [Browsable(false)]
        public Vertex V2
        {
            get
            {
                return _V2;
            }
            set
            {
                _V2 = value;
            }
        }

        [Browsable(true)]
        public Orientations Orientation
        {
            get
            {
                return _Orientation;
            }
            set
            {
                _Orientation = value;

                UpdateStyle();
            }
        }

        void UpdateStyle()
        {
            if (Style == null)
                Style = "";

            var parts = Style.Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            parts = parts.Where(p => p != "pre" && p != "post").ToList();

            if (_Orientation == Orientations.Forward)
            {
                parts.Add("post");
            }
            else if (_Orientation == Orientations.Backward)
            {
                parts.Add("pre");
            }

            var sb = new StringBuilder();

            bool isFirst = true;
            foreach (var p in parts)
            {
                if (!isFirst)
                    sb.Append(", ");

                sb.Append(p);

                isFirst = false;
            }

            Style = sb.ToString();
        }

        [Browsable(true)]
        public float Thickness
        {
            get
            {
                return _Thickness;
            }
            set
            {
                _Thickness = value;
            }
        }

        [Browsable(false)]
        public GraphicsLayer.ARGB Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
                _penColor = _Color;
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

                Color = _IsSelected ? new GraphicsLayer.ARGB(50, 205, 50) : new GraphicsLayer.ARGB(0, 0, 0);
            }
        }

        [Browsable(true)]
        public int Multiplicity
        {
            get
            {
                return _Multiplicity;
            }
            set
            {
                _Multiplicity = Math.Max(1, value);
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
                if (value == null)
                    return;

                if (value.StartsWith("+") && !string.IsNullOrEmpty(_Style))
                    _Style += ", " + value.TrimStart('+');
                else
                    _Style = value.TrimStart('+');
            }
        }

        Vertex _V1;
        Vertex _V2;
        Orientations _Orientation;
        float _Thickness = 4;
        GraphicsLayer.ARGB _Color = new GraphicsLayer.ARGB(0, 0, 0);
        GraphicsLayer.ARGB _penColor = new GraphicsLayer.ARGB(0, 0, 0);
        bool _IsSelected;
        int _Multiplicity;
        string _Style;
    }
}

