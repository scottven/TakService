using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakEngine;

namespace TakConsole
{
    static class NonInteractiveTest
    {
        const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        const int BoardSize = 5;
        public static void RunTest(string appendPath)
        {
            var ai = new TakAI(BoardSize);
            ai.MaxDepth = 4;
            var evaluator = new TakAI.Evaluator(BoardSize);
            var movelog = new List<string>();
            var durationlog = new List<TimeSpan>();
            while (true)
            {
                var guid = System.Guid.NewGuid();
                PrintTimeStampedMessage("Started new game");
                var game = GameState.NewGame(BoardSize);
                movelog.Clear();
                durationlog.Clear();
                bool gameOver;
                int eval;
                var starttime = DateTime.Now;
                do
                {
                    var movestart = DateTime.Now;
                    var move = ai.FindGoodMove(game);
                    var duration = DateTime.Now.Subtract(movestart);
                    var notation = move.Notate();
                    PrintTimeStampedMessage(string.Concat(game.Ply, ": ", notation));
                    movelog.Add(notation);
                    durationlog.Add(duration);
                    move.MakeMove(game);
                    game.Ply++;
                    evaluator.Evaluate(game, out eval, out gameOver);
                } while (!gameOver);

                string result;
                if (eval == 0)
                    result = "Tie";
                else
                {
                    if (eval > 0)
                        result = "First player wins";
                    else
                        result = "Second player wins";

                    if (eval == Math.Abs(TakAI.Evaluator.FlatWinEval))
                        result += " via flats";
                    else
                        result += " via road";
                }
                PrintTimeStampedMessage(result);

                using (var writer = System.IO.File.AppendText(appendPath))
                {
                    writer.WriteLine("' Game ID {0}", guid);
                    writer.WriteLine("' AI difficulty {0}", ai.MaxDepth);
                    writer.WriteLine("' Started {0}", starttime.ToString(DateFormat));
                    writer.WriteLine("' Duration {0}", DateTime.Now.Subtract(starttime));
                    writer.WriteLine("' Moves {0}", movelog.Count);
                    writer.WriteLine("' Result {0}", result);
                    for (int i = 0; i < movelog.Count; i++)
                        writer.WriteLine("{0}\t{1}\t{2}", i + 1, movelog[i], durationlog[i]);
                }
            }
        }

        static void PrintTimeStampedMessage(string message)
        {
            Console.WriteLine("[{0}] {1}", DateTime.Now.ToString(DateFormat), message);
        }
    }
}
