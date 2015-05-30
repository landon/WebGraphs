﻿using Choosability.FixerBreaker.KnowledgeEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability.Utility;
using Choosability;

namespace Console
{
    public static class WeaklyFixableTester
    {
        const int Delta = int.MaxValue;
        public static void Go()
        {
            using (var graphEnumerator = new GraphEnumerator("superabundant weakly fixable test.txt", 2, 20))
            {
                graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";

                foreach (var g in graphEnumerator.EnumerateGraph6File(g => true, EnumerateWeightings))
                {
                    System.Console.Write("checking " + g.ToGraph6() + " with degrees [" + string.Join(",", g.VertexWeight) + "] ...");

                    var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g, false, false);
                    mind.MaxPot = Delta;
                    mind.SuperabundantOnly = true;

                    var weakMind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g, false, true);
                    weakMind.MaxPot = Delta;
                    weakMind.SuperabundantOnly = true;

                    var template = new Template(g.VertexWeight);

                    var win = mind.Analyze(template, null);
                    var winWeak = weakMind.Analyze(template, null);

                    if (win != winWeak)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine(" weakly is weaker");
                        System.Console.ForegroundColor = ConsoleColor.White;
                        graphEnumerator.AddWinner(g);
                    }
                    else
                    {
                        if (win)
                            System.Console.ForegroundColor = ConsoleColor.Blue;
                        else
                            System.Console.ForegroundColor = ConsoleColor.Green;
                        System.Console.WriteLine("  " + weakMind.BoardCountsList.Max(l => l.Count) + " - " + mind.BoardCountsList.Max(l => l.Count) + " = " + (weakMind.BoardCountsList.Max(l => l.Count) - mind.BoardCountsList.Max(l => l.Count)));
                        System.Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }

        static IEnumerable<Choosability.Graph> EnumerateWeightings(Choosability.Graph g)
        {
            if (g.MaxDegree > 4)
                yield break;

            foreach (var weighting in g.Vertices.Select(v => Enumerable.Range(g.Degree(v), 2).Reverse()).CartesianProduct())
            {
                var www = weighting.ToList();

                var gg = g.Clone();
                gg.VertexWeight = www;
                yield return gg;
            }
        }
    }
}
