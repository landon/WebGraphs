﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class GameTree : Tree<GameTree>
    {
        public SuperSlimBoard Board { get; set; }
        public bool IsColorable { get; set; }
        public BreakerChoiceInfo Info {get; set;}

        public void AddChild(GameTree tree, BreakerChoiceInfo info)
        {
            tree.Info = info;
            AddChild(tree);
        }
    }
}