using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGT;

namespace Console
{
    public static class TestBGT
    {
        public static void Go()
        {
            var f = new Fron("aaa");
            var code = f.Encode();
            var f2 = new Fron(code);
        }
    }
}
