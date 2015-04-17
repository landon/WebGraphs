using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Choosability.Utility;

namespace Choosability.WordGame
{
    public static class Boxification
    {
        public static List<List<List<T>>> Boxify<T>(List<List<T>> points)
        {
            var boxes = points.Select(p => p.EnList()).ToList();
            while (true)
            {
                var boxContainers = boxes.CartesianProduct(boxes).Where(pair => !pair.Item1.SequenceEqual(pair.Item2)).Select(pair => new { Pair = pair, BoxedUnion = BoxContainer(pair.Item1.Concat(pair.Item2).ToList()) });

                var grown = boxContainers.FirstOrDefault(bc => ContainsBoxContainer(points, bc.BoxedUnion));
                if (grown == null)
                    break;

                var count = boxes.Count;
                boxes.Remove(grown.Pair.Item1);
                boxes.Remove(grown.Pair.Item2);
                boxes.Add(grown.BoxedUnion);

                if (boxes.Count == count)
                    break;
            }

            return boxes;
        }

        public static List<List<List<T>>> PrefixBoxify<T>(List<List<T>> points)
        {
            var allPoints = points.ToList();
            var boxes = new List<List<List<T>>>();

            while (true)
            {
                var t = Binary(1, points.Count - 1, i =>
                    {
                        return ContainsBox(allPoints, points.Take(i).ToList()) ? 1 : -1;
                    });

                boxes.Add(BoxContainer(points.Take(t).ToList()));
                points = points.Skip(t).ToList();
                if (points.Count <= 0)
                    break;
            }

            return boxes;
        }

        public static bool ContainsBox<T>(List<List<T>> allPoints, List<List<T>> points)
        {
            var box = BoxContainer(points);
            return ContainsBoxContainer(allPoints, box);
        }

        static bool ContainsBoxContainer<T>(List<List<T>> allPoints, List<List<T>> boxContainer)
        {
            return boxContainer.All(p => allPoints.Any(pp => pp.SequenceEqual(p)));
        }

        public static List<List<T>> BoxContainer<T>(List<List<T>> points)
        {
            return Enumerable.Range(0, points[0].Count).Select(i => points.Select(p => p[i]).Distinct().ToList())
                                                       .CartesianProduct()
                                                       .Select(e => e.ToList())
                                                       .ToList();
        }

        public static bool IsBox<T>(List<List<T>> points)
        {
            return points.Count == BoxContainer(points).Count;
        }

        public static string ToBoxString<T>(List<List<T>> points)
        {
            return string.Join(" * ", Enumerable.Range(0, points[0].Count).Select(i => "{" + string.Join(",", points.Select(p => p[i]).Distinct()) + "}"));
        }

        static int Binary(int first, int last, Func<int, int> targetDirection)
        {
            var left = first - 1;
            var right = last + 1;

            while (right - left >= 2)
            {
                int middle = left + (right - left) / 2;
                int direction = targetDirection(middle);

                if (direction == 0)
                    return middle;
                else if (direction < 0)
                    right = middle;
                else
                    left = middle;
            }

            if (left < first)
                return first - 1;
            if (right > last)
                return last + 1;

            return left;
        }
    }
}
