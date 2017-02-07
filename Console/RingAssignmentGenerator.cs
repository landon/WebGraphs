using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Choosability;

namespace Console
{
    public static class RingAssignmentGenerator
    {
        public static List<List<long>> GenerateAssignments(int ringSize, int potSize, int listSize, List<ulong> contractions = null)
        {
            var g = Choosability.Graphs.C(ringSize);
            var stable = g.IndependentSets.Where(s => s.Count > 0).Select(s => new Tuple<List<int>, ulong>(s, s.ToUInt64())).OrderByDescending(s => s.Item2).ToList();
            if (contractions != null)
                stable = stable.Where(s => contractions.All(c => (s.Item2 & c) == c || (s.Item2 & c) == 0)).ToList();

            var traces = new List<List<ulong>>();
            var current = new List<ulong>(potSize);
            var neededColors = Enumerable.Repeat(listSize, ringSize).ToArray();
            GenerateAssignments(current, potSize, neededColors, stable, 0, trace => traces.Add(trace));

            return traces.Select(trace => trace.ToStackLong(ringSize)).ToList();
        }

        static void GenerateAssignments(List<ulong> current, int colorsLeft, int[] neededColors, List<Tuple<List<int>, ulong>> classes, ulong lastBits, Action<List<ulong>> output)
        {
            if (neededColors.Max() <= 0)
            {
                output(current.ToList());
                return;
            }

            if (colorsLeft <= 0)
                return;

            var reducedClasses = classes.FindAll(c => c.Item2 >= lastBits && c.Item1.All(v => neededColors[v] > 0));

            foreach (var cc in reducedClasses)
            {
                foreach (var v in cc.Item1)
                    neededColors[v]--;

                current.Add(cc.Item2);
                GenerateAssignments(current, colorsLeft - 1, neededColors, reducedClasses, cc.Item2, output);
                current.RemoveAt(current.Count - 1);

                foreach (var v in cc.Item1)
                    neededColors[v]++;
            }
        }
    }
}


