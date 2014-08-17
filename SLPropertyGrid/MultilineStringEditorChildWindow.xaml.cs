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

namespace SLPropertyGrid
{
    public partial class MultilineStringEditorChildWindow : ChildWindow
    {
        PropertyItem property;

        public MultilineStringEditorChildWindow(PropertyItem property)
        {
            InitializeComponent();

            this.property = property;
            var value = property.Value as String;
            if (value != null)
                MainTextBox.Text = value; 

            Title = "Edit " + property.Name;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            property.Value = MainTextBox.Text;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

