using Microsoft.VisualStudio.TestTools.UnitTesting;
using TakEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakEngine.Tests
{
    [TestClass()]
    public class GameStateTests
    {
        [TestMethod()]
        public void LoadTPSTest()
        {
            //string tps = "[ 1,2,x,x,1/x,1,x,x,x/x,x,x,x,x/x,x,x,2,x/2,x,x,x,x 1 3 ]";
            //string ptn = "[Size \"5\"]\n1. b4 d2\n2. e5 a1\n3. e1 e2\n";
            //string tps = "[ x,x,x,x,x/2,2,x,1,x/x,2,21S,2C,1/1,1,x,1C,2/2,x,x,1,x 2 8 ]";
            //string ptn = "[Size \"5\"]\n\n1. a4 d1\n2.d4 c3\n3.b2 b3\n4.Cd2 Cd3\n5.Sc2 b4\n6.e3 e2\n7.c2+ a1\n8.a2";
            string tps = "[ 2221C,x,2,2,21/2,2122111112C,1,2,2/2,x,x,x,1/x,1,112S,x,1/1,112,x,x,x 1 27 ]";
            string ptn = "[Size \"5\"]\n1. d5 b4>\n2.d2 + e2\n3. 2c3- e4\n4. 2d3+ a1\n5.e5 c5\n6. 3d4< 5a2+113\n7.d4 c5<\n8.b4 b3+\n9. 5c4<14 2b5-\n10. 5a4> c4\n11.c5 e4+\n12.e4";
            var tps_game = TakEngine.GameState.LoadFromTPS(tps);
            var ptn_game = TakEngine.GameState.LoadFromPTN(ptn);
            if (tps_game.Board.GetHashCode() == ptn_game.Board.GetHashCode())
                return;
            Assert.Fail();
        }
    }
}