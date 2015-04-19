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
            var maker = new GraphPictureMaker(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\triangle-free FixerBreaker winners Delta=3.txt");
            maker.DrawAll(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\neatotest", DotRenderType.png);
            
            // FindTarpits.Go();
            //FindFixerBreaker.Go();
            //FindChoosables.Go();
            System.Console.WriteLine();
            System.Console.WriteLine("done.");
         //   System.Console.ReadKey();
        }
    }
}
