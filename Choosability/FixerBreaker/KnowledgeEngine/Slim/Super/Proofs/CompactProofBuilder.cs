using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class CompactProofBuilder : ProofBuilder
    {
        string _tikz;

        public CompactProofBuilder(SuperSlimMind mind, string tikz = "")
            : base(mind)
        {
            _tikz = tikz;
        }

        public override string WriteProof()
        {
            var sb = new StringBuilder();
            var figureID = "fig:" + Guid.NewGuid().ToString();

            sb.AppendLine("\\begin{figure}");
            sb.AppendLine("\\centering");
            sb.AppendLine(_tikz);
            sb.AppendLine("\\caption{Solid vertices have lists of size 3 and the labeled vertices have lists of size 2.}\\label{" + figureID + "}");
            sb.AppendLine("\\end{figure}");

            sb.AppendLine("\\begin{lem}");
            sb.AppendLine("The graph in Figure \\ref{" + figureID + "} is reducible.");
            sb.AppendLine("\\end{lem}");
            
            var letters = new List<string>() { "X", "Y", "Z" };
            var stringLength = Mind.ColorableBoards[0].Stacks.Value.Count(ss => ss.PopulationCount() == 2);
            var rng = new Random(DateTime.Now.Millisecond);
            var randomString = "";
            for(int i = 0; i < stringLength; i++)
                randomString += letters[rng.Next(3)];

            sb.Append("\\begin{proof}");
            sb.AppendLine("Let $X = \\{0,1\\}$, $Y = \\{0,2\\}$ and $Z = \\{1,2\\}$. Then with the vertex ordering in Figure \\ref{" + figureID + "}, a string such as " + randomString + ", ");
            sb.AppendLine("represents a possible list assignment on $V(H)$ arising from a $3$-edge-coloring of $G-E(H)$.");
            sb.AppendLine("By an $X$-Kempe change, we mean flipping colors $0$ and $1$ on a two-colored path in $G-E(H)$.  We call such a path an $X$-path. ");
            sb.AppendLine("Any endpoint of an $X$-path in $H$ must end at a $Y$ or $Z$ vertex.  The meanings of $Y$-Kempe change, $Z$-Kempe change, $Y$-path and $Z$-path are analogous.");
            sb.AppendLine("Note that if there are an odd number of $Y$'s and $Z$'s, then at least one $X$-Kempe change has only one endpoint in $H$.");
            sb.AppendLine();

            for (int caseNumber = 1; caseNumber <= Cases.Count; caseNumber++)
            {
                var c = Cases[caseNumber - 1];

                var boardsXYZ = string.Join(", ", c.Boards.Select(b => b.ToXYZ()));
                var countModifier = c.Boards.Count > 1 ? "one of " : "";
                sb.AppendLine(string.Format("\\case{{{0}}}{{$B$ is " + countModifier + boardsXYZ + ".}}", caseNumber));

                if (caseNumber == 1)
                {
                    sb.AppendLine("In all these cases, $G$ is immediately colorable from the lists.");
                }
                else
                {
                    var fixGroups = c.Boards.GroupBy(b =>
                        {
                            var treeInfo = Mind.GetWinTreeInfo(b);
                            var fixLetter = ((long)((1 << treeInfo.First().Alpha) | (1 << treeInfo.First().Beta))).ToXYZ();

                            return fixLetter;
                        });

                    foreach (var fixGroup in fixGroups)
                    {
                        var others = letters.ToList();
                        others.Remove(fixGroup.Key);
                        sb.AppendLine();
                        sb.Append("For ");
                        if (fixGroup.Count() > 1)
                            sb.Append("each of ");
                        sb.Append(fixGroup.Select(b => b.ToXYZ()).Listify());
                        sb.Append(string.Format(", we do {0} {1}-Kempe change.", fixGroup.Key.GetArticle(), fixGroup.Key));

                        var swapCountGroups = fixGroup.GroupBy(b => Mind.GetWinTreeInfo(b).Max(ti => ti.SwapVertices.Count)).ToList();
                        foreach (var swapCountGroup in swapCountGroups)
                        {
                            if (swapCountGroup.Key == 1)
                            {
                                if (swapCountGroups.Count == 1)
                                {
                                    if (swapCountGroups.First().Count() > 1)
                                        sb.AppendLine(" Each of these have an odd number of " + others[0] + "'s and " + others[1] + "'s, so there is " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-path with exactly one vertex in $H$.");
                                    else
                                        sb.AppendLine(" This has an odd number of " + others[0] + "'s and " + others[1] + "'s, so there is " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-path with exactly one vertex in $H$.");
                                }
                                else
                                {
                                    if (swapCountGroup.Count() > 1)
                                        sb.AppendLine(" Each of " + swapCountGroup.Select(b => b.ToXYZ()).Listify() + " have an odd number of " + others[0] + "'s and " + others[1] + "'s, so there is " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-path with exactly one edge in $H$.");
                                    else
                                        sb.AppendLine(" Now " + swapCountGroup.Select(b => b.ToXYZ()).Listify() + " has an odd number of " + others[0] + "'s and " + others[1] + "'s, so there is " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-path with exactly one edge in $H$.");
                                }

                                foreach (var b in swapCountGroup)
                                {
                                    if (swapCountGroup.Count() == 1)
                                        sb.Append("If ");
                                    else
                                        sb.Append("For " + b.ToXYZ() + ", if ");
                                    var treeInfo = Mind.GetWinTreeInfo(b);
                                    var groups = treeInfo.GroupBy(ss => ss.SwapVertices[0]);

                                    sb.Append("this is the " + groups.OrderBy(gg => gg.Key).Select(gg => gg.Key.GetXYZIndex(b).Wordify()).Listify("or") + " vertex of $H$, ");

                                    sb.Append("then doing " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-Kempe change there yields " + groups.OrderBy(gg => gg.Key).Select(gg => GetChildBoardName(b, gg.First())).Listify());
                                    if (treeInfo.Count > 1)
                                        sb.AppendLine(" respectively, each of which we already handled.");
                                    else
                                        sb.AppendLine(", which we already handled.");
                                }
                            }
                            else if (swapCountGroup.Key == 2)
                            {
                                foreach (var b in swapCountGroup)
                                {
                                    sb.AppendLine();
                                    if (swapCountGroup.Count() == 1)
                                        sb.Append("If ");
                                    else
                                        sb.Append("For " + b.ToXYZ() + ", if ");

                                    var treeInfo = Mind.GetWinTreeInfo(b);
                                    var leftover = treeInfo.ToList();

                                    var first = true;
                                    while (leftover.Count > 0)
                                    {
                                        var commonestSwapper = Enumerable.Range(0, b._stackCount).MaxIndex(v => leftover.Count(bc => bc.SwapVertices.Contains(v)));
                                        var handledAll = leftover.Where(bc => bc.SwapVertices.Contains(commonestSwapper)).ToList();
                                        var handled = handledAll.Distinct(bc => bc.SwapVertices.Count == 1 ? -1 : bc.SwapVertices.Except(commonestSwapper).First()).ToList();

                                        if (!first)
                                            sb.Append("If ");
                                        
                                        first = false;
                                        sb.Append("the " + fixGroup.Key + "-path starting at the " + commonestSwapper.GetXYZIndex(b).Wordify() + " vertex");
                                        var single = handled.FirstOrDefault(bc => bc.SwapVertices.Count == 1);
                                        if (single != null)
                                            sb.Append(" doesn't end in $H$");

                                        if (handled.Where(bc => bc.SwapVertices.Count > 1).Count() > 0)
                                        {
                                            if (single != null)
                                                sb.Append(" or");

                                            sb.Append(" ends at the " + handled.Where(bc => bc.SwapVertices.Count > 1).OrderBy(bc => bc.SwapVertices.Except(commonestSwapper).First()).Select(bc => bc.SwapVertices.Except(commonestSwapper).First().GetXYZIndex(b).Wordify()).Listify("or") + " vertex of $H$, ");
                                            sb.Append("then doing " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-Kempe change there yields " +
                                                handled.OrderBy(bc => bc.SwapVertices.Count == 1 ? -1 : bc.SwapVertices.Except(commonestSwapper).First()).Select(bc => GetChildBoardName(b, bc)).Listify());
                                        }
                                        else if (single != null)
                                        {
                                            sb.Append(" then doing " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-Kempe change there yields " + GetChildBoardName(b, single));
                                        }

                                        if (handled.Count > 1)
                                            sb.AppendLine(" respectively, each of which we already handled.");
                                        else
                                            sb.AppendLine(", which we already handled.");

                                        foreach (var bc in handledAll)
                                            leftover.Remove(bc);
                                    }
                                }
                            }
                            else
                            {
                                sb.AppendLine();
                                sb.AppendLine("NEED TWO SWAPS, FIX ME!");
                                sb.AppendLine();
                            }
                        }
                    }
                }

                sb.AppendLine();
            }

            sb.AppendLine("\\end{proof}");
            return sb.ToString();
        }
    }
}
