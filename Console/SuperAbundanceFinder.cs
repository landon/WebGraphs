using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class SuperAbundanceFinder
    {
        public static void Go()
        {
            var g = Choosability.Graphs.P(5);
            var mind = new Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.SuperSlimMind(g);
            mind.SuperabundantOnly = true;
            mind.MaxPot = int.MaxValue;

            var sizes = g.Vertices.Select(v => g.Degree(v)).ToList();
            sizes[0] = 2;
            sizes[2] = 3;
            sizes[3] = 2;
            sizes[4] = 2;
            var win = mind.Analyze(new Choosability.FixerBreaker.KnowledgeEngine.Template(sizes), null);

            System.Console.WriteLine("win: " + win);
        }
    }
}
