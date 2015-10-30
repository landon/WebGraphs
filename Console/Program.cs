﻿using Choosability.FixerBreaker.KnowledgeEngine;
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
           // SinkEliminator.Go(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\ring size 11 min spread 0 spread 4 planar triangulationAT winners.txt");
            FindChoosablesAdvanced.Go();
          //  MicTester.Go();
           // GenerateWithExcludedSubgraphs.Go();
            //WeaklyFixableTester.Go();
          //  FixerBreakerTrees.Go();
         //   SuperAbundanceFinder.Go();
          //  FindFixerBreaker.Go();
         //   MakePictures.Go();

          //  MixedChoosables.Go();
            //EliminiteDoubleEdgeSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\Mixed spread 2 AT winners1.txt");
           
           //  FindTarpits.Go();
           // FindFixerBreaker.Go();
            //FindChoosables.Go();

         //   EliminateDoubleEdgeNotSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\not 10 vertex Mixed spread 2 max high 1 kappa2 winners1.txt");

            //ConAndNon.Go();

           // EliminateTriangleBlowups.Go("not 9 vertex Mixed spread 2 max high 2 kappa2 winners1.txt.not.eliminated.txt");
            System.Console.WriteLine();
            System.Console.WriteLine("done.");
            System.Console.ReadKey();
        }
    }
}
