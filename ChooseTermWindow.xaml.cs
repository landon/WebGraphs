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
using GraphsCore;
using Choosability;
using GraphicsLayer;
using Choosability.Polynomials;

namespace WebGraphs
{
    public partial class ChooseTermWindow : ChildWindow
    {
        public Choosability.Graph G { get; set; }
        public int[] Powers { get; private set; }
        bool _doSign;

        public ChooseTermWindow(AlgorithmBlob blob, bool doSign = false)
        {
            InitializeComponent();
            G = blob.AlgorithmGraph;

            _doSign = doSign;
            Show();
        }

        async void OnOK(object sender, RoutedEventArgs e)
        {
            Powers = null;
            try
            {
                var p = _termBox.Text.Split(',').Select(s => int.Parse(s)).ToArray();
                if (p.Count() != G.N)
                    return;
                if (p.Sum() != G.E)
                    return;

                Powers = p;
            }
            catch { }

            if (Powers == null)
                return;

            DialogResult = true;
            var term = string.Join("", Powers.Select((p, v) => "x_" + v + "^" + p));
            using (var resultWindow = new ResultWindow())
            {
                if (_doSign)
                    resultWindow.AddChild(new TextBlock() { Text = "computing sign sum of " + term + "...", TextWrapping = TextWrapping.Wrap });
                else
                    resultWindow.AddChild(new TextBlock() { Text = "computing coefficient of " + term + "...", TextWrapping = TextWrapping.Wrap });
                var c = await Task.Factory.StartNew<int>(() =>
                {
                    if (_doSign)
                        return G.GetSignSum(Powers);
                    return G.GetCoefficient(Powers);
                });

                resultWindow.ClearChildren();

                if (_doSign)
                    resultWindow.AddChild(new TextBlock() { Text = "sign sum of " + term + " is " + c, TextWrapping = TextWrapping.Wrap });
                else
                    resultWindow.AddChild(new TextBlock() { Text = "coefficient of " + term + " is " + c, TextWrapping = TextWrapping.Wrap });
            }
        }

        void OnUseCurrentOrientation(object sender, RoutedEventArgs e)
        {
            _termBox.Text = string.Join(",", G.Vertices.Select(v => G.OutDegree(v)));
        }
    }
}

