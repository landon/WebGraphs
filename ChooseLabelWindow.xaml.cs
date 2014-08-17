using System;
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
    public partial class ChooseLabelWindow : ChildWindow
    {
        public Action<string> Finished;
        public ChooseLabelWindow()
        {
            InitializeComponent();
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Finished != null)
                Finished(_labelBox.Text);
            DialogResult = true;
        }
    }
}

