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
            string tps = "[ x5/x2,121,x2/x5/x,2C,x3/x5 1 4 ]";
            var game = TakEngine.GameState.LoadTPS(tps);
            Assert.Fail();
        }
    }
}