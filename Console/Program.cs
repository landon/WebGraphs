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
            FindFixerBreaker.Go();
            //MakePictures.Go();
            //var maker = new GraphPictureMaker(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\trees or trees plus edge only FixerBreaker winners Delta=3.txt");
            //maker.DrawAllAndMakeWebpage(@"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta3TreeOrTreePlusEdge");
            //var sss = string.Join(",", Directory.EnumerateFiles(@"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta3TriangleFree", "*.dot").Select(f => "'" + Path.GetFileName(f) + "'"));
            //maker.GenerateAllDots(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\savingdots");
            
            // FindTarpits.Go();
           // FindFixerBreaker.Go();
            //FindChoosables.Go();
            System.Console.WriteLine();
            System.Console.WriteLine("done.");
         //   System.Console.ReadKey();
        }
    }
}
