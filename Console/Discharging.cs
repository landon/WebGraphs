using Choosability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class Discharging
    {
        public static void BuildNeighborhoods(int d, int dmin, int dmax, int rad, int maxHigh = int.MaxValue)
        {
            var excluded = @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test.txt".EnumerateWeightedGraphs(removeOrientation: true, weightAdjustment: 5)
                                                                                                     //.Where(g => g.VertexWeight.Max() <= dmax && g.VertexWeight.Count(ww => ww == dmax) <= maxHigh)
                                                                                                     .ToList();

            NewGenerator.GenerateWeightedNeighborhoods(d, dmin, dmax, rad, excluded, maxHigh).WriteToWeightFile(string.Format("dcharge_test_{0}_{1}_{2}_{3}_{4}.txt", d, dmin, dmax, rad, maxHigh)); ;
        }
    }
}
