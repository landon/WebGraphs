using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace PictureMaker
{
    public enum DotRenderType
    {
        pdf,
        png,
        svg,
        eps,
        ps
    }
    public class DotRenderer
    {
        string _dotPath;
        public bool SkipLayout { get; set; }

        public DotRenderer(string dotPath)
        {
            _dotPath = dotPath;
        }

        public string Render(string dot, string fileName, DotRenderType renderType)
        {
            var root = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(root);

            fileName = Path.Combine(root, Path.GetFileNameWithoutExtension(fileName) + "." + renderType);

            var tempFile = Path.GetTempFileName();
            using (var sw = new StreamWriter(tempFile))
                sw.Write(dot);

            string arguments;
            if (SkipLayout)
            {
                arguments = string.Format(@"-n -T{0} ""{2}"" -o ""{1}""", renderType, fileName, tempFile);
            }
            else
            {
                arguments = string.Format(@"-T{0} ""{2}"" -o ""{1}""", renderType, fileName, tempFile);
            }
            var info = new ProcessStartInfo(_dotPath, arguments);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            var process = new Process();
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();

            File.Delete(tempFile);

            return fileName;
        }
    }
}
