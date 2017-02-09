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
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;

namespace WebGraphs
{
    public partial class LookupIsomorphismClassWindow : ChildWindow
    {
        string _delim = "|";
        public LookupIsomorphismClassWindow()
        {
            InitializeComponent();

            _isomorphismClass.KeyDown += _isomorphismClass_KeyDown;
        }

        void _isomorphismClass_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.D)
                {
                    if (_delim == "|")
                    {
                        _delim = ",";
                        _isomorphismClass.Text = _isomorphismClass.Text.Replace('|', ',');
                    }
                    else
                    {
                        _delim = "|";
                        _isomorphismClass.Text = _isomorphismClass.Text.Replace(',', '|');
                    }
                }
            }
        }

        void _termBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var s = _termBox.Text;

                var lists = s.Split(new[] { "|", "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => Enumerable.Range(0, a.Length).Select(i => int.Parse(a[i].ToString())).ToList()).ToList();
                var board = SuperSlimBoard.FromLists(lists);
                var ss = board.ToListStringInLexOrder();
                _isomorphismClass.Text = ss;
            }
            catch
            {
                _isomorphismClass.Text = "that is not a board";
            }
        }
    }
}

