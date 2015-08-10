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
           // WeaklyFixableTester.Go();
          //  FixerBreakerTrees.Go();
         //   SuperAbundanceFinder.Go();
          //  FindFixerBreaker.Go();
           // MakePictures.Go();

            MixedChoosables.Go();
            //EliminiteDoubleEdgeSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\Mixed spread 2 AT winners1.txt");
           
           //  FindTarpits.Go();
           // FindFixerBreaker.Go();
            //FindChoosables.Go();
            System.Console.WriteLine();
            System.Console.WriteLine("done.");
            System.Console.ReadKey();
        }
    }
}
