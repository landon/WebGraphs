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
using System.Windows.Shapes;
using GraphsCore;

namespace WebGraphs
{
    public class LayoutAnimation
    {
        const int Steps = 100;

        AlgorithmBlob _blob;
        Action _update;
        Action _finalUpdate;
        Storyboard _storyBoard;
        List<Tuple<double, double>> _layout;
        double[] _xstep;
        double[] _ystep;
        int _step = 0;

        public LayoutAnimation(AlgorithmBlob blob, Action update, Action finalUpdate, Layout.Algorithm layoutAlgorithm)
        {
            _blob = blob;
            _update = update;
            _finalUpdate = finalUpdate;
            try
            {
                _layout = layoutAlgorithm(blob.AlgorithmGraph);
            }
            catch { }

            if (_layout == null)
                return;

            _xstep = new double[_layout.Count];
            _ystep = new double[_layout.Count];
            for (int i = 0; i < _layout.Count; i++)
            {
                _xstep[i] = (_layout[i].Item1 - _blob.UIGraph.Vertices[i].X) / Steps;
                _ystep[i] = (_layout[i].Item2 - _blob.UIGraph.Vertices[i].Y) / Steps;
            }

            _storyBoard = new Storyboard();
            _storyBoard.Duration = TimeSpan.FromMilliseconds(1);
            _storyBoard.Completed += OnStoryBoardCompleted;
            _storyBoard.Begin();
        }

        void OnStoryBoardCompleted(object sender, EventArgs e)
        {
            for (int i = 0; i < _layout.Count; i++)
            {
                _blob.UIGraph.Vertices[i].X += _xstep[i];
                _blob.UIGraph.Vertices[i].Y += _ystep[i];
            }

            _update();
            _step++;
            if (_step >= Steps)
                OnFinish();
            else
                _storyBoard.Begin();
        }

        void OnFinish()
        {
            for (int i = 0; i < _layout.Count; i++)
            {
                _blob.UIGraph.Vertices[i].X = _layout[i].Item1;
                _blob.UIGraph.Vertices[i].Y = _layout[i].Item2;
            }
            _finalUpdate();
        }
    }
}
