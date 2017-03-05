using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability;
using System.Numerics;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Console
{
    public static class UnitDistance
    {
        const int MinN = 4;
        const int MaxN = 4;
        const string OutputFile = "unit_distance.txt";
        const string GraphFileRoot = @"C:\Users\landon\Google Drive\research\Graph6\VertexCritical\chi";
        public static void Go()
        {
            var gn = new GraphEnumerator(OutputFile, MinN, MaxN, false);
            gn.FileRoot = GraphFileRoot;
            foreach (var g in gn.EnumerateGraph6File())
            {
                if (IsUnitDistance(g))
                    gn.AddWinner(g);
 
            }
        }

        static bool IsUnitDistance(Graph g)
        {
            var sb = new StringBuilder();
            sb.Append("bash -lic \"singular -b -q -c '");
            sb.Append(string.Format("ring R = 0,({0}), dp;", string.Join(",", g.Vertices.Select(v => x(v) + "," + y(v)))));
            var polys = new List<string>();
            for (int i = 0; i < g.N; i++)
            {
                for (int j = i + 1; j < g.N; j++)
                {
                    if (g[i, j])
                    {
                        var pp = "f_" + i + "_" + j;
                        sb.Append("poly " + pp + " = (" + x(j) + "-" + x(i) + ")^2" + " + " + "(" + y(j) + "-" + y(i) + ")^2 - 1;");
                        polys.Add(pp);
                    }
                }
            }

            sb.Append("ideal I = " + string.Join(",", polys) + ";");
            sb.Append("ideal si = std(I);");
            sb.Append("si;");
            sb.Append("' >blah.txt\"");

            var sss = sb.ToString();

            using (var sw = new StreamWriter("daaa.bat"))
                sw.WriteLine(sss);

            var p = new Process();
            
            p.StartInfo = new ProcessStartInfo("daaa.bat");
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            while (true)
            {
                try
                {
                    using (var sr = new StreamReader(@"C:\cygwin64\home\landon\blah.txt"))
                    {
                        var blah = sr.ReadToEnd();

                        var xx = blah.StartsWith("si[1]=1");
                        return !xx;
                    }
                }
                catch { }
                Thread.Sleep(1000);
            }

            return false;
        }

        static string x(int v) { return "x_" + v; }
        static string y(int v) { return "y_" + v; }
    }
}
