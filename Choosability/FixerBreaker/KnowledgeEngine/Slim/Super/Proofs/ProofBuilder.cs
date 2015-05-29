using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class ProofBuilder
    {
        public SuperSlimMind Mind { get; private set; }
        public List<ProofCase> Cases { get; protected set; }

        public ProofBuilder(SuperSlimMind mind)
        {
            Mind = mind;
            ExtractCases();
        }

        protected virtual void ExtractCases()
        {
            Cases = new List<ProofCase>();

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

                var addedBoards = new List<SuperSlimBoard>();
                foreach (var board in remainingBoards)
                {
                    var treeInfo = Mind.GetWinTreeInfo(board);

                    if (treeInfo.All(bc => wonBoards.Contains(new SuperSlimBoard(board._trace, bc.Alpha, bc.Beta, bc.Response, board._stackCount))))
                        addedBoards.Add(board);
                }

                foreach (var board in addedBoards)
                {
                    proofCase.AddBoard(board);
                    wonBoards.Add(board);
                    remainingBoards.Remove(board);
                }

                caseNumber++;
            }
        }

        protected virtual int GetHandledCaseNumber(SuperSlimBoard b, BreakerChoiceInfo bc)
        {
            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
            return Cases.IndicesWhere(cc => cc.Boards.Contains(childBoard)).First() + 1;
        }

        protected string GetChildBoardName(SuperSlimBoard b, BreakerChoiceInfo bc)
        {
            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
            return childBoard.ToXYZ();
        }

        public virtual string WriteProof()
        {
            var sb = new StringBuilder();
            
            for(int caseNumber = 1; caseNumber <= Cases.Count; caseNumber++)
            {
                var c = Cases[caseNumber - 1];

                var boardsXYZ = string.Join(", ", c.Boards.Select(b => b.ToXYZ()));
                var countModifier = c.Boards.Count > 1 ? "one of " : "";
                sb.AppendLine(string.Format("\\case{{{0}}}{{$B$ is " + countModifier + boardsXYZ + ".}}", caseNumber));

                if (caseNumber == 1)
                {
                    sb.AppendLine("In all these cases, $G$ is immediately colorable from the lists. (JUSTIFICATION NEEDED?)");
                }
                else
                {
                    if (c.Boards.Count > 1)
                        sb.AppendLine("We show that for each board, one of X-fix, Y-fix, or Z-fix gets to a board in a previous case.");
                    foreach (var b in c.Boards)
                    {
                        sb.AppendLine();
                        if (c.Boards.Count > 1)
                            sb.Append("For " + b.ToXYZ() + ", ");
                        
                        var treeInfo = Mind.GetWinTreeInfo(b);
                        var fixLetter = ((long)((1 << treeInfo.First().Alpha) | (1 << treeInfo.First().Beta))).ToXYZ();
                        var others = new List<string>() {"X", "Y", "Z"};
                        others.Remove(fixLetter);

                        if (c.Boards.Count > 1)
                            sb.Append("we");
                        else
                            sb.Append("We");
                        sb.AppendLine(" perform " + fixLetter.GetArticle() + " " + fixLetter + "-fix.");

                        foreach (var bc in treeInfo)
                        {
                            var partitionId = b.ToPartitionId(bc.Partition);
                            sb.Append("If the partition is " + b.ToCompactedPartitionId(bc.Partition) + ", then swapping " + others[0] + " and " + others[1] + " at " + bc.SwapVertices.Select(v => partitionId[v]).Distinct().Listify());
                            
                            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
                            sb.Append(" yields " + childBoard.ToXYZ() + " which we already handled in Case ");
                            var handledCaseNumber = Cases.IndicesWhere(cc => cc.Boards.Contains(childBoard)).First() + 1;
                            sb.AppendLine(handledCaseNumber + ".");
                        }
                    }
                }

                sb.AppendLine();
            }


            return sb.ToString();
        }
    }
}
