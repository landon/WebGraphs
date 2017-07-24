using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var sb = new StringBuilder();
            int i = 0;
            foreach (var f in Directory.GetFiles(@"C:\Users\landon\Documents\GitHub\books\colored graphs\graphs").OrderBy(f => f.ToCharArray().Count(c => c == '1')).OrderBy(f => f.Length))
            {
                sb.AppendLine("\\subfloat[]{\\includegraphics[width=0.2\\textwidth]{graphs/" + Path.GetFileName(f) + "}}");

                i++;
                if (i % 5 == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    if (i % 20 == 0)
                    {
                        sb.AppendLine("\\end{figure}");
                        sb.AppendLine("\\begin{figure}");
                    }
                }
            }

            var ss = sb.ToString();
            //using (var sw = new StreamWriter("diam3.txt"))
            //{
            //    foreach (var g in Enumerable.Range(4, 30).Select(n => string.Format(@"C:\Users\landon\Google Drive\research\Graph6\vertextransitive\cubic\cub{0:000}t.g6", n)).SelectMany(f => f.EnumerateGraph6File()))
            //    {
            //        var d = g.ComputeDiameter();
            //        var girth = g.FindShortestCycle().Count - 1;

            //        if (d <= 3 && girth >= 5)
            //            sw.WriteLine(g.ToGraph6());
            //    }

            //}

            // WeaklyFixableTester.Go();
            //  FixerBreakerTrees.Go();
            //   SuperAbundanceFinder.Go();
            //  FindFixerBreaker.Go();
           // MakePictures.Go();

            // MixedChoosables.Go();
            //EliminiteDoubleEdgeSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\Mixed spread 2 AT winners1.txt");

            //  FindTarpits.Go();
            // FindFixerBreaker.Go();
            //FindChoosables.Go();

            //   EliminateDoubleEdgeNotSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\not 10 vertex Mixed spread 2 max high 1 kappa2 winners1.txt");

            //ConAndNon.Go();
            System.Console.WriteLine();
            System.Console.WriteLine("done.");
            System.Console.ReadKey();
        }
    }
}
