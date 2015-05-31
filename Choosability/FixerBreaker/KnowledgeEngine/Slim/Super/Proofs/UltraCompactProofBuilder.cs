using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Choosability.Utility;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class UltraCompactProofBuilder : CompactProofBuilder
    {
        protected Dictionary<SuperSlimBoard, List<Tuple<Permutation, SuperSlimBoard>>> _permutationLinked;

        public UltraCompactProofBuilder(SuperSlimMind mind, string tikz = "")
            : base(mind, tikz)
        {
        }

        protected override void ExtractCases()
        {
            Cases = new List<ProofCase>();

            _permutationLinked = new Dictionary<SuperSlimBoard, List<Tuple<Permutation, SuperSlimBoard>>>();

            var indices = Mind.ColorableBoards[0].Stacks.Value.IndicesWhere(stack => stack.PopulationCount() == 2).ToList();
            var permutations = Permutation.EnumerateAll(indices.Count).ToList();

            var caseNumber = 0;

            var colorableCase = new ProofCase(Mind, 0, Mind.ColorableBoards);
            Cases.Add(colorableCase);
            caseNumber++;

            var remainingBoards = Mind.NonColorableBoards.ToList();
            var wonBoards = Mind.ColorableBoards.ToList();
            while (remainingBoards.Count > 0)
            {
                var proofCase = new ProofCase(Mind, caseNumber);
                Cases.Add(proofCase);

                var addedRootBoards = new List<SuperSlimBoard>();
                var addedBoards = new List<SuperSlimBoard>();
                foreach (var board in remainingBoards)
                {
                    if (addedBoards.Contains(board))
                        continue;

                    var treeInfo = Mind.GetWinTreeInfo(board);
                    var childBoards = treeInfo.Select(bc => new SuperSlimBoard(board._trace, bc.Alpha, bc.Beta, bc.Response, board._stackCount)).Distinct().ToList();

                    if (childBoards.SubsetEqual(wonBoards))
                    {
                        addedRootBoards.Add(board);
                        addedBoards.Add(board);
                        _permutationLinked[board] = new List<Tuple<Permutation, SuperSlimBoard>>();

                        foreach (var p in permutations)
                        {
                            var pb = board.Permute(p, indices);
                            if (wonBoards.Contains(pb) || addedBoards.Contains(pb))
                                continue;

                            var closed = true;
                            foreach (var cb in childBoards)
                            {
                                if (!wonBoards.Contains(cb.Permute(p, indices)))
                                {
                                    closed = false;
                                    break;
                                }
                            }

                            if (closed)
                            {
                                _permutationLinked[board].Add(new Tuple<Permutation, SuperSlimBoard>(p, pb));
                                addedBoards.Add(pb);
                            }
                        }
                    }
                }

                foreach (var board in addedRootBoards)
                {
                    proofCase.AddBoard(board);
                }

                foreach (var board in addedBoards)
                {
                    wonBoards.Add(board);
                    remainingBoards.Remove(board);
                }

                caseNumber++;
            }
        }

        protected override int GetHandledCaseNumber(SuperSlimBoard b, BreakerChoiceInfo bc)
        {
            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
            if (Cases[0].Boards.Contains(childBoard))
                return 1;

            return Cases.Skip(1).IndicesWhere(cc => cc.Boards.SelectMany(bb => new[] { bb }.Union(_permutationLinked[bb].Select(tup => tup.Item2))).Contains(childBoard)).First() + 2;
        }


        public override string WriteProof()
        {
            var length = Mind.ColorableBoards[0].ToXYZ().Length;
            var allBoards = Mind.ColorableBoards.Union(Mind.NonColorableBoards).ToList();
            
            var comparer = new SequenceGeneralizer<int>.VectorComparer();
            var sg = new SequenceGeneralizer<int>(length, new List<int> { 0, 1, 2 });

            var zot2 = allBoards.Select(b => b.To012()).ToList();
            var examples2 = allBoards.Select(b => b.To012()).ToList();

            var nonExamples2 = Enumerable.Repeat(Enumerable.Range(0, 3), length).CartesianProduct().Select(ll => ll.ToList()).Except(zot2.Distinct(comparer), comparer).ToList();

            var generalized2 = sg.Generalize(examples2, nonExamples2, false);
            var allBoardsXYZ = generalized2.Select(gg => "$" + string.Join("", gg.Select(_ => _.ToTex())) + "$").Listify("or");

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
            for (int i = 0; i < stringLength; i++)
                randomString += letters[rng.Next(3)];

            sb.Append("\\begin{proof}");
            sb.AppendLine("Let $X = \\{0,1\\}$, $Y = \\{0,2\\}$ and $Z = \\{1,2\\}$. Then with the vertex ordering in Figure \\ref{" + figureID + "}, a string such as " + randomString + ", ");
            sb.AppendLine("represents a possible list assignment on $V(H)$ arising from a $3$-edge-coloring of $G-E(H)$.");
            sb.AppendLine("By an $X$-Kempe change, we mean flipping colors $0$ and $1$ on a two-colored path in $G-E(H)$.  We call such a path an $X$-path. ");
            sb.AppendLine("Any endpoint of an $X$-path in $H$ must end at a $Y$ or $Z$ vertex.  The meanings of $Y$-Kempe change, $Z$-Kempe change, $Y$-path and $Z$-path are analogous.");
            sb.AppendLine("Note that if there are an odd number of $Y$'s and $Z$'s, then at least one $X$-path has only one endpoint in $H$.");
            sb.AppendLine();

            if (Mind.OnlyConsiderNearlyColorableBoards)
            {
                sb.AppendLine("We need to handle all boards that are nearly colorable for edge $e$ up to permutations of $\\{X,Y,Z\\}$, so it will suffice to handle all boards of the form " + allBoardsXYZ + ".");
            }
            else
            {
                sb.AppendLine("We need to handle all boards up to permutations of $\\{X,Y,Z\\}$, so it will suffice to handle all boards of the form " + allBoardsXYZ + ".");
            }

            sb.AppendLine();
            var wonBoards = new List<SuperSlimBoard>();
            for (int caseNumber = 1; caseNumber <= Cases.Count; caseNumber++)
            {
                var c = Cases[caseNumber - 1];

                var boards = c.Boards;
                List<SuperSlimBoard> thisClaimBoards;
                if (caseNumber > 1)
                    thisClaimBoards = boards.SelectMany(b => new[] { b }.Union(_permutationLinked[b].Select(tup => tup.Item2))).ToList();
                else
                    thisClaimBoards = boards;

                wonBoards.AddRange(thisClaimBoards);
                
                var zot = thisClaimBoards.Select(b => b.To012()).ToList();
                var examples = thisClaimBoards.Select(b => b.To012()).ToList();
                var nonExamples = Enumerable.Repeat(Enumerable.Range(0, 3), length).CartesianProduct().Select(ll => ll.ToList()).Except(zot.Distinct(comparer), comparer).ToList();

                var generalized = sg.Generalize(examples, nonExamples);
                var boardsXYZ = generalized.Select(gg => "$" + string.Join("", gg.Select(_ => _.ToTex())) + "$").Listify("or");

                var countModifier = boards.Count > 1 ? "one of " : "";
                sb.AppendLine(string.Format("\\case{{{0}}}{{$B$ is one of " + boardsXYZ + ".}}", caseNumber));

                if (caseNumber == 1)
                {
                    sb.AppendLine("In all these cases, $H$ is immediately colorable from the lists.");
                }
                else
                {
                    sb.AppendLine();
                    var fixGroups = boards.GroupBy(b =>
                    {
                        var treeInfo = Mind.GetWinTreeInfo(b);
                        var fixLetter = ((long)((1 << treeInfo.First().Alpha) | (1 << treeInfo.First().Beta))).ToXYZ();

                        return fixLetter;
                    });

                    foreach (var fixGroup in fixGroups)
                    {
                        var others = letters.ToList();
                        others.Remove(fixGroup.Key);

                        var swapCountGroups = fixGroup.GroupBy(b => Mind.GetWinTreeInfo(b).Max(ti => ti.SwapVertices.Count)).ToList();
                        foreach (var swapCountGroup in swapCountGroups)
                        {
                            if (swapCountGroup.Key == 1)
                            {
                                if (swapCountGroup.Count() > 1)
                                    sb.AppendLine("Each of " + swapCountGroup.Select(b => b.ToXYZ()).Listify() + " have an odd number of " + others[0] + "'s and " + others[1] + "'s, so there is " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-path with exactly one end in $H$.");
                                else
                                    sb.AppendLine("Since " + swapCountGroup.Select(b => b.ToXYZ()).Listify() + " has an odd number of " + others[0] + "'s and " + others[1] + "'s, there is " + fixGroup.Key.GetArticle() + " " + fixGroup.Key + "-path with exactly one end in $H$.");

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
                                    {
                                        var lll = treeInfo.Select(bc => GetHandledCaseNumber(b, bc)).Distinct().OrderBy(xx => xx).ToList();
                                        sb.AppendLine(" respectively, which are handled by Case" + (lll.Count > 1 ? "s " : " ") + lll.Listify() + ".");
                                    }
                                    else
                                        sb.AppendLine(", which is handled by Case " + GetHandledCaseNumber(b, treeInfo.First()) + ".");

                                    if (_permutationLinked[b].Count > 0)
                                    {
                                        sb.AppendLine("Since we already handled the permutation of all resulting boards by " + _permutationLinked[b].Select(ppp => ppp.Item1).Listify() + ", we have also handled " + _permutationLinked[b].Select(ppp => ppp.Item2.ToXYZ()).Listify() + ".");
                                        sb.AppendLine();
                                    }
                                }
                            }
                            else if (swapCountGroup.Key == 2)
                            {
                                foreach (var b in swapCountGroup)
                                {
                                    sb.AppendLine();
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
                                        {
                                            var lll = handled.Select(bc => GetHandledCaseNumber(b, bc)).Distinct().OrderBy(xx => xx).ToList();
                                            sb.AppendLine(" respectively, which are handled by Case" + (lll.Count > 1 ? "s " : " ") + lll.Listify() + ".");
                                        }
                                        else
                                            sb.AppendLine(", which is handled by Case " + GetHandledCaseNumber(b, handled.First()) + ".");

                                        foreach (var bc in handledAll)
                                            leftover.Remove(bc);
                                    }

                                    if (_permutationLinked[b].Count > 0)
                                    {
                                        sb.AppendLine("Since we already handled the permutation of all resulting boards by " + _permutationLinked[b].Select(ppp => ppp.Item1).Listify() + ", we have also handled " + _permutationLinked[b].Select(ppp => ppp.Item2.ToXYZ()).Listify() + ".");
                                        sb.AppendLine();
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
