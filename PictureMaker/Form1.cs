using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PictureMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _listBox.PreviewKeyDown += _listBox_PreviewKeyDown;
        }

        void _listBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Control)
            {
                MakePicture(Clipboard.GetText());
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                var text = _listBox.SelectedItem as string;
                if (text != null)
                    Clipboard.SetText(text);
            }
        }

        void MakePicture(string data)
        {
            var g = GraphsCore.CompactSerializer.Deserialize(data);
            var pm = new GraphPictureMaker(_skipLayoutBox.Checked);
            var path = pm.Draw(g, @"C:\Users\landon\Dropbox\Public\Web\GraphData\Exports\" + GetFileName(g));
            if (path != null)
                _listBox.Items.Add("https://dl.dropboxusercontent.com/u/8609833/Web/GraphData/Exports/" + System.IO.Path.GetFileName(path));
        }

        string GetFileName(Graphs.Graph g)
        {
            return Guid.NewGuid().ToString() + ".svg";
        }
    }
}
