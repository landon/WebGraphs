﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WebGraphs
{
    public partial class ResultWindow : ChildWindow, IDisposable
    {
        bool _closeOnFinish;
        public ResultWindow(bool realProgress = false, bool closeOnFinish = false)
        {
            InitializeComponent();
            HasCloseButton = false;

            if (realProgress)
            {
                _progressIndicator.IsIndeterminate = false;
                _progressIndicator.Minimum = 0;
                _progressIndicator.Maximum = 100;
                _progressIndicator.Value = 0;
            }

            Title = "thinking...";
            _closeOnFinish = closeOnFinish;
            Show();
        }

        public void OnProgress(Tuple<string, int> p)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                _progressIndicator.Value = p.Item2;
                Title = p.Item1;
            }));
        }

        public void ClearChildren()
        {
            _layoutRoot.Children.Clear();
        }

        public FrameworkElement AddChild(FrameworkElement c)
        {
            var tt = c as TextBox;
            if (tt != null)
            {
                tt.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                tt.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                tt.IsReadOnly = true;
                tt.Height = 450;
            }

            _layoutRoot.Children.Add(c);

            return c;
        }

        public void Dispose()
        {
            _layoutRoot.Children.Remove(_progressIndicator);
            Title = ResultTitle ?? "results";
            HasCloseButton = true;

            if (_closeOnFinish)
            {
                DialogResult = true;
                Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
            }
        }

        public string ResultTitle { get; set; }
    }
}

