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
            int[] totalScore = new int[] { 0, 0 };
            var aitype1 = new TakAI_V3(BoardSize);
            aitype1.MaxDepth = 3;
            var aitype2 = new TakAI_V2(BoardSize);
            aitype2.MaxDepth = 3;
            var evaluator = new TakAI_V2.Evaluator(BoardSize);
            var movelog = new List<string>();
            var durationlog = new List<TimeSpan>();
            for (int gameCount = 0; ;gameCount++)
            {
                var guid = System.Guid.NewGuid();
                PrintTimeStampedMessage("Started new game");
                var game = GameState.NewGame(BoardSize);
                movelog.Clear();
                durationlog.Clear();
                bool gameOver;
                int eval;
                var starttime = DateTime.Now;
                ITakAI ai1, ai2;
                if (0 == (gameCount & 1))
                {
                    ai1 = aitype1;
                    ai2 = aitype2;
                }
                else
                {
                    ai1 = aitype2;
                    ai2 = aitype1;
                }
                do
                {
                    var movestart = DateTime.Now;
                    IMove move;
                    if (0 == (game.Ply & 1))
                        move = ai1.FindGoodMove(game);
                    else
                        move = ai2.FindGoodMove(game);
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
                    {
                        totalScore[gameCount & 1] += 1;
                        result = "First player wins (W: " + ai1.EvalMethod + ")";
                    }
                    else
                    {
                        totalScore[(gameCount + 1) & 1] += 1;
                        result = "Second player wins (W: " + ai2.EvalMethod + ")";
                    }

                    if (eval == Math.Abs(TakAI_V2.Evaluator.FlatWinEval))
                        result += " via flats";
                    else
                        result += " via road";
                }
                PrintTimeStampedMessage(result);
                PrintTimeStampedMessage(string.Format("{0}={1}, {2}={3}", aitype1.EvalMethod, totalScore[0], aitype2.EvalMethod, totalScore[1]));

                using (var writer = System.IO.File.AppendText(appendPath))
                {
                    writer.WriteLine("' Game ID {0}", guid);
                    writer.WriteLine("' AI1 difficulty {0}", ai1.MaxDepth);
                    writer.WriteLine("' AI2 difficulty {0}", ai2.MaxDepth);
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
