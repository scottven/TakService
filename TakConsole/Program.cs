using System;
using System.Collections.Generic;
using System.Linq;
using TakEngine;

namespace TakConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = TakEngine.Notation.TakPGN.LoadFromFile("example_notation.ptn");
            while (true)
            {
                Console.Write("(interactive, automated): ");
                var response = Console.ReadLine().Trim();
                if (response == "interactive")
                {
                    GameLoop();
                    break;
                }
                else if (response == "automated")
                    NonInteractiveTest.RunTest("c:\\temp\\tak_ai_test.txt");
                else
                    Console.WriteLine("Invalid response");
            }
        }

        static void GameLoop()
        {
            var fullauto = new bool[] { false, false };
            var game = GameState.NewGame(5);
            var ai = new TakAI(game.Size, maxDepth: 3);
            var evaluator = new TakAI.Evaluator(game.Size);
            bool gameOver;
            int eval;
            var recentMoves = new Stack<IMove>();
            var recentStates = new Stack<GameState>();
            while (true)
            {
                // print board
                PrintBoard(game, previous: recentStates.Count > 0 ? recentStates.Peek() : null);

                evaluator.Evaluate(game, out eval, out gameOver);
                if (gameOver)
                {
                    Console.Write("Game over, ");
                    if (eval == 0)
                        Console.WriteLine("Tie");
                    else if (eval > 0)
                        Console.WriteLine("X wins");
                    else
                        Console.WriteLine("O wins");
                }
                Console.Write("[T{0}, {1}]: ", game.Ply, (game.Ply & 1) == 0 ? 'X' : 'O');

                string cmd;
                if (fullauto[game.Ply & 1] && !gameOver)
                {
                    cmd = "ai";
                    Console.WriteLine(cmd);
                }
                else
                    cmd = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(cmd) || cmd == "q")
                    break;
                else if (cmd == "help")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("==Global commands");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("ai        AI will choose a move for this player");
                    Console.WriteLine("ai on     This player will become completely controlled by the AI");
                    Console.WriteLine("ai N      Set AI difficulty to N [2-9], default is 4.");
                    Console.WriteLine("ai off    Disable AI control for all players");
                    Console.WriteLine("undo      Undo last move, including the AI's response (if any)");
                    Console.WriteLine("list      List all legal moves in the current board position");
                    Console.WriteLine("          Warning: level N+1 is roughly 30 to 50 times slower than N!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("==Move notation");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("          NOTE: XY positions start from 0 in the top-left of the board");
                    Console.WriteLine("          NOTE: Directions are n,s,e,w (north, south, east, west)");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("fXY       Place flat stone from reserve at position X,Y");
                    Console.WriteLine("sXY       Place standing stone from reserve at position X,Y");
                    Console.WriteLine("cXY       Place cap stone from reserve at position X,Y");
                    Console.WriteLine("mXY,N,D N1 N2 N3 etc...");
                    Console.WriteLine("          Move stones by picking up N stones from the top of the stack");
                    Console.WriteLine("          at X,Y then then traveling in direction D.  Drop N1 stones");
                    Console.WriteLine("          in the first square, N2 in the second, etc...");
                    Console.WriteLine("              Example: m23,2,n 1 1");
                    Console.WriteLine("          This would pick up 2 stones from position 2,3 and travel north,");
                    Console.WriteLine("          dropping 1 stone at 2,2 and then another stone at 2,1");
                    Console.Write("<Press any key to continue>");
                    Console.ReadKey();
                    Console.WriteLine();
                }
                else if (cmd == "undo")
                {
                    while (recentMoves.Count > 0)
                    {
                        var undoing = recentMoves.Pop();
                        undoing.TakeBackMove(game);
                        game.Ply--;
                        recentStates.Pop();
                        if (!fullauto[game.Ply & 1])
                            break;
                    }
                }
                else if (cmd == "ai off")
                {
                    fullauto[0] = fullauto[1] = false;
                }
                else if (cmd.StartsWith("ai ") && cmd.Length > 3 && Char.IsDigit(cmd[3]))
                {
                    int diff = 0;
                    if (!int.TryParse(cmd.Substring(3), out diff))
                        diff = 0;
                    if (diff < 2 || diff > 9)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid difficulty level.  Legal values are 2 thru 9.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {

                        if (diff == ai.MaxDepth)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("AI difficulty is already set to " + diff.ToString());
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            ai.MaxDepth = diff;
                            Console.WriteLine("AI difficulty set to " + diff.ToString());
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }
                }
                else
                {
                    if (gameOver)
                        Console.WriteLine("Invalid command");
                    else if (cmd == "ai on" || cmd == "ai")
                    {
                        if (cmd == "ai on")
                            fullauto[game.Ply & 1] = true;
                        var move = ai.FindGoodMove(game);

                        var restoreColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("ai move => {0}", move.Notate());
                        Console.ForegroundColor = restoreColor;

                        recentStates.Push(game.DeepCopy());
                        recentMoves.Push(move);
                        move.MakeMove(game);
                        game.Ply++;
                    }
                    else if (cmd == "list")
                    {
                        var legalMoves = new List<IMove>();
                        TakAI.EnumerateMoves(legalMoves, game, ai.RandomPositions);
                        foreach (var move in legalMoves)
                            Console.WriteLine(move.Notate());
                    }
                    else
                    {
                        var legalMoves = new List<IMove>();
                        TakAI.EnumerateMoves(legalMoves, game, ai.RandomPositions);
                        var match = legalMoves.FirstOrDefault(x => x.Notate() == cmd);
                        if (match == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ILLEGAL MOVE OR INVALID MOVE NOTATION.  Type 'help' for more information.");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            recentStates.Push(game.DeepCopy());
                            recentMoves.Push(match);
                            match.MakeMove(game);
                            game.Ply++;
                        }
                    }
                }
            }

            Console.WriteLine("Exiting");
        }

        static void PrintBoard(GameState game, GameState previous = null)
        {
            var restoreColor = Console.ForegroundColor;
            BoardPosition pos;
            var maxWidth = new int[game.Size];
            for (pos.X = 0; pos.X < game.Size; pos.X++)
                for (pos.Y = 0; pos.Y < game.Size; pos.Y++)
                {
                    maxWidth[pos.X] = Math.Max(maxWidth[pos.X], game.Board[pos.X, pos.Y].Count);
                }

            if (maxWidth.Max() <= 1)
            {
                for (pos.Y = game.Size - 1; pos.Y >= 0; pos.Y--)
                {
                    for (pos.X = 0; pos.X < game.Size; pos.X++)
                    {
                        var piece = game[pos];
                        if (!piece.HasValue)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write('.');
                        }
                        else
                        {
                            if (previous != null && previous[pos] == piece)
                                Console.ForegroundColor = ConsoleColor.Gray;
                            else
                                Console.ForegroundColor = ConsoleColor.Cyan;
                            PrintPieceValue(piece.Value);
                        }
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                for (pos.Y = game.Size - 1; pos.Y >= 0; pos.Y--)
                {
                    for (pos.X = 0; pos.X < game.Size; pos.X++)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        var stack = game.Board[pos.X, pos.Y];
                        if (previous != null && previous.Board[pos.X, pos.Y].Count > stack.Count)
                            Console.ForegroundColor = ConsoleColor.Red;
                        for (int i = 0; i < Math.Max(1, maxWidth[pos.X]); i++)
                        {
                            if (i < stack.Count)
                            {
                                if (previous != null && i >= previous.Board[pos.X, pos.Y].Count)
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                PrintPieceValue(stack[i], lower: i < stack.Count - 1);
                            }
                            else
                            {
                                if (i == 0)
                                    Console.Write('.');
                                else
                                    Console.Write(' ');
                            }
                        }
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                }
            }
            Console.ForegroundColor = restoreColor;
        }

        private static void PrintPieceValue(int pieceValue, bool lower = false)
        {
            switch (Piece.GetStone(pieceValue))
            {
                case Piece.Stone_Cap:
                    if (Piece.GetPlayerID(pieceValue) == 0)
                        Console.Write('+');
                    else
                        Console.Write('*');
                    break;
                case Piece.Stone_Flat:
                    char c;
                    if (Piece.GetPlayerID(pieceValue) == 0)
                        c = 'X';
                    else
                        c = 'O';
                    if (lower)
                        c = Char.ToLower(c);
                    Console.Write(c);
                    break;
                case Piece.Stone_Standing:
                    if (Piece.GetPlayerID(pieceValue) == 0)
                        c = '|';
                    else
                        c = '#';
                    Console.Write(c);
                    break;
                default:
                    throw new ApplicationException();
            }
        }

        static void ReplayMoves(GameState game)
        {
            string movesText = @"NOT IN USE";

            var ai = new TakAI(game.Size);
            var lines = movesText.Split('\n');
            foreach (var line in lines)
            {
                var notation = line.Substring(line.IndexOf(':') + 1).Trim();
                var moves = new List<IMove>();
                TakAI.EnumerateMoves(moves, game, ai.NormalPositions);
                var move = moves.First(x => x.Notate() == notation);
                move.MakeMove(game);
                game.Ply++;
            }
        }

        static void LoadDebugBoard(GameState game)
        {
            string layout =
@"oo# . xx+ . .
  . . . . .
  . . . . .
  . . . . .
  . . . . .";
            int y = -1;
            foreach (var line in layout.Split('\n'))
            {
                y++;
                var cols = line.Trim().Split(' ');
                int x = -1;
                foreach (var col in cols)
                {
                    x++;
                    var stack = game.Board[x, y];
                    foreach (var c in col)
                    {
                        switch (Char.ToLower(c))
                        {
                            case 'x':
                                stack.Add(Piece.MakePieceID(Piece.Stone_Flat, 0));
                                break;
                            case 'o':
                                stack.Add(Piece.MakePieceID(Piece.Stone_Flat, 1));
                                break;
                            case '+':
                                stack.Add(Piece.MakePieceID(Piece.Stone_Cap, 0));
                                break;
                            case '*':
                                stack.Add(Piece.MakePieceID(Piece.Stone_Cap, 1));
                                break;
                            case '|':
                                stack.Add(Piece.MakePieceID(Piece.Stone_Standing, 0));
                                break;
                            case '#':
                                stack.Add(Piece.MakePieceID(Piece.Stone_Standing, 1));
                                break;
                        }
                    }
                }
            }
            game.Ply = 2;
        }
    }
}
