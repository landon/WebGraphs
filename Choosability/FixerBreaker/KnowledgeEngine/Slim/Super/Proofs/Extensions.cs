using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public static class Extensions
    {
        const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string ToXYZ(this SuperSlimBoard board)
        {
            return string.Join("", board.Stacks.Value.Select(ToXYZ));
        }

        public static string ToCompactedPartitionId(this SuperSlimBoard board, List<List<int>> partition)
        {
            var xyz = board.Stacks.Value.Select(ToXYZ).ToList();
            return xyz.ToPartitionId(partition);
        }

        public static string ToPartitionId(this SuperSlimBoard board, List<List<int>> partition)
        {
            var xyz = board.Stacks.Value.Select(stack =>
                {
                    var s = stack.ToXYZ();
                    if (s.Length <= 0)
                        return "*";
                    return s;
                }).ToList();

            return xyz.ToPartitionId(partition);
        }

        public static string ToPartitionId(this List<string> xyz, List<List<int>> partition)
        {
            var pp = partition.OrderBy(part => part.Min()).ToList();
            for (int i = 0; i < pp.Count; i++)
            {
                var l = Alphabet[i].ToString();
                foreach (var j in pp[i])
                    xyz[j] = l;
            }

            return string.Join("", xyz);
        }

        public static string ToXYZ(this long stack)
        {
            switch (stack)
            {
                case 3:
                    return "X";
                case 5:
                    return "Y";
                case 6:
                    return "Z";
            }

            return "";
        }

        public static string GetArticle(this string letter)
        {
            switch (letter)
            {
                case "X":
                    return "an";
                case "Y":
                    return "a";
                case "Z":
                    return "a";
            }

            return "";
        }

        public static string Listify<T>(this IEnumerable<T> strings)
        {
            var ll = strings.ToList();
            if (ll.Count <= 0)
                return "";
            if (ll.Count == 1)
                return ll[0].ToString();

            return string.Join(", ", ll.Take(ll.Count - 1)) + " and " + ll.Last();
        }
    }
}
