using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class ProofCase
    {
        public SuperSlimMind Mind { get; private set; }
        public List<SuperSlimBoard> Boards { get; private set; }

        public int Depth { get; private set; }

        public ProofCase(SuperSlimMind mind, int depth, List<SuperSlimBoard> boards = null)
        {
            Mind = mind;
            Depth = depth;
            Boards = boards;
            if (Boards == null)
                Boards = new List<SuperSlimBoard>();
        }

        public void AddBoard(SuperSlimBoard board)
        {
            Boards.Add(board);
        }
    }
}
