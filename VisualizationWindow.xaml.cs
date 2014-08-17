using Choosability.Utility;
using GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WebGraphs
{
    public partial class VisualizationWindow : ChildWindow
    {
        const int GridWidth = 80;

        public Canvas Canvas { get { return _theCanvas; } }

        static readonly ARGB White = new ARGB(255, 255, 255);
        static readonly ARGB Black = new ARGB(0, 0, 0);

        SafeCollection<BoardInfo> _boardInfo = new SafeCollection<BoardInfo>();
        Box _nextLocation;
        double _verticalScale = -1;
        Font _infoFont = new Font("Arial", 26);
        string _parenthetical = "";
        
        TextBlock _remainingBlock;
        TextBlock _wonBlock;
        Line _dividerLine;

        System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();

        public VisualizationWindow()
        {
            InitializeComponent();
            HasCloseButton = false;
            Title = "thinking...";
            _timer.Tick += OnTimer;
        }

        public void AddChild(FrameworkElement c)
        {
            _layoutRoot.Children.Add(c);
        }

        public void Finish(string result)
        {
            Title = result;
            HasCloseButton = true;

            if (result.Contains("nearly colorable"))
            {
                _parenthetical = " (none nearly colorable)";
                Invalidate();
            }
        }

        public void Start()
        {
            _timer.Stop();
            _parenthetical = "";
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void ResetVisualization()
        {
            _nextLocation = Box.Empty;
            _verticalScale = -1;
            _boardInfo.Clear();

            Invalidate();
        }

        public async void UpdateVisualization(Choosability.FixerBreaker.KnowledgeEngine.ThoughtProgress p)
        {
            await Task.Factory.StartNew(() =>
            {
                if (p.BoardsAdded != null)
                {
                    foreach (var b in p.BoardsAdded)
                    {
                        var bi = new BoardInfo() { Board = b };
                        bi.InitialLocation = NextLocation();
                        bi.Color = Color.FromArgb(255, 225, 0, 0);

                        _boardInfo.Add(bi);
                    }
                }
                if (p.BoardsRemoved != null)
                {
                    foreach (var bi in _boardInfo)
                    {
                        if (p.BoardsRemoved.Contains(bi.Board))
                        {
                            bi.IsRemoved = true;
                            if (p.WinLength == 0)
                                bi.Color = Color.FromArgb(255, 0, 0, 255);
                            else
                                bi.Color = Color.FromArgb(255, 0, (byte)Math.Max(255 - p.WinLength * 25, 0), 0);
                        }
                    }
                }
            });

            Invalidate();
        }

        Box NextLocation()
        {
            var p = _nextLocation;

            if (_nextLocation.X >= GridWidth - 2)
                _nextLocation = new Box(0, _nextLocation.Y + 1);
            else
                _nextLocation = new Box(_nextLocation.X + 1, _nextLocation.Y);

            return p;
        }

        void DoPaint()
        {
            try
            {
                var padding = 30;
                var w = _dotCanvas.ActualWidth - 2 * padding;
                var h = _dotCanvas.ActualHeight - 2 * padding;
                var maxY = _nextLocation.Y;

                var vs = _verticalScale;

                if (vs < 0)
                    vs = (h / 3) / 10;

                if (vs * maxY > h / 3)
                    vs = (h / 3) / maxY;

                var dotWidth = Math.Max(5, w / GridWidth / 2);
                var dotHeight = Math.Max(5, vs / 2);

                dotWidth = Math.Min(dotWidth, dotHeight);
                dotHeight = dotWidth;

                var count = _boardInfo.Count;
                var wonCount = _boardInfo.Count(bi => bi.IsRemoved);

                foreach (var bi in _boardInfo)
                {
                    if (bi.Shape == null)
                    {
                        bi.Shape = new Ellipse() { Width = dotWidth, Height = dotHeight };
                        _dotCanvas.Children.Add(bi.Shape);
                    }

                    var x = padding + bi.InitialLocation.X * w / GridWidth - dotWidth / 2;
                    var y = padding + bi.InitialLocation.Y * vs - dotHeight / 2;

                    if (bi.IsRemoved)
                    {
                        var toY = h - y;
                        y = (float)(bi.T * toY + (1.0 - bi.T) * y);
                    }

                    if (bi.Shape.Fill == null || ((SolidColorBrush)bi.Shape.Fill).Color != bi.Color)
                        bi.Shape.Fill = new SolidColorBrush(bi.Color);
                    
                    Canvas.SetLeft(bi.Shape, x);
                    Canvas.SetTop(bi.Shape, y);
                }

                var s1 = string.Format("{0} boards remaining" + _parenthetical, count - wonCount);
                var s2 = string.Format("{0} boards won", wonCount);

                if (_remainingBlock == null)
                    _remainingBlock = DrawString(s1, _infoFont, White, new Box(w / 2, h / 2 - padding * 2 + 15), _dotCanvas);
                if (_wonBlock == null)
                    _wonBlock = DrawString(s2, _infoFont, White, new Box(w / 2, h / 2 + padding * 2 - 5), _dotCanvas);

                if (_dividerLine == null)
                {
                    _dividerLine = new Line() { X1 = 0, Y1 = _dotCanvas.ActualHeight / 2, X2 = _dotCanvas.ActualWidth, Y2 = _dotCanvas.ActualHeight / 2, StrokeThickness = 2, Stroke = new SolidColorBrush(White.ToColor()) };
                    _dotCanvas.Children.Add(_dividerLine);
                }

                _remainingBlock.Text = s1;
                _wonBlock.Text = s2;
                CenterTextBlock(_remainingBlock, new Box(w / 2, h / 2 - padding * 2 + 15));
                CenterTextBlock(_wonBlock, new Box(w / 2, h / 2 + padding * 2 - 5));
            }
            catch { }
        }

        TextBlock DrawString(string s, Font font, ARGB argb, Box bounds, Canvas canvas)
        {
            var t = new TextBlock() { Text = s, FontFamily = new FontFamily(font.Name), FontSize = 1.3 * font.Size, Foreground = new SolidColorBrush(argb.ToColor()) };
            CenterTextBlock(t, bounds);

            canvas.Children.Add(t);

            return t;
        }

        void CenterTextBlock(TextBlock t, Box bounds)
        {
            t.Measure(new Size(double.MaxValue, double.MaxValue));

            Canvas.SetLeft(t, bounds.Left + bounds.Width / 2 - t.ActualWidth / 2);
            Canvas.SetTop(t, bounds.Top + +bounds.Height / 2 - t.ActualHeight / 2);
        }

        void Invalidate()
        {
            Dispatcher.BeginInvoke(DoPaint);
        }

        void OnTimer(object sender, EventArgs e)
        {
            var changed = false;
            var count = _boardInfo.Count;
            foreach (var bi in _boardInfo)
            {
                if (bi.IsRemoved && bi.T < 1.0)
                {
                    bi.T = Math.Min(bi.T + 0.02, 1.0);
                    changed = true;
                }
            }

            if (changed)
                Invalidate();
        }
    }
}

