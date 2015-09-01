using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine
{
    public interface IMind
    {
        bool Analyze(Template template, Action<Tuple<string, int>> progress);
        int TotalBoards { get; }
        int MaxPot { set; }
    }
}
