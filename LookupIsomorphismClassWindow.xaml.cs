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
        public LookupIsomorphismClassWindow()
        {
            InitializeComponent();
        }

        void _termBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var s = _termBox.Text;

                var lists = s.Split('|').Select(a => Enumerable.Range(0, a.Length).Select(i => int.Parse(a[i].ToString())).ToList()).ToList();
                var board = SuperSlimBoard.FromLists(lists);
                _isomorphismClass.Text = board.ToListStringInLexOrder();
            }
            catch
            {
                _isomorphismClass.Text = "that is not a board";
            }
        }
    }
}

