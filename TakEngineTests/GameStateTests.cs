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
            string tps = "[ 1,2,x,x,1/x,1,x,x,x/x,x,x,x,x/x,x,x,2,x/2,x,x,x,x 1 3 ]";
            string ptn = "[Size \"5\"]\n1. b4 d2\n2. e5 a1\n3. e1 e2\n";
            var tps_game = TakEngine.GameState.LoadFromTPS(tps);
            var ptn_game = TakEngine.GameState.LoadFromPTN(ptn);
            if (tps_game.Board.GetHashCode() == ptn_game.Board.GetHashCode())
                return;
            Assert.Fail();
        }
    }
}