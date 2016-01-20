using System;
using System.Collections.Generic;

namespace TakEngine
{
    public class GameState
    {
        /// <summary>
        /// Size of the board (4,5,6,7 or 8)
        /// </summary>
        public int Size;

        /// <summary>
        /// Current ply number; starts at 0 for the first player's turn, then goes to 1 for the opposing player's response, etc...
        /// </summary>
        public int Ply;
        
        /// <summary>
        /// 2-element array containing the number of regular stones (standing or flat) remaining for each player
        /// </summary>
        public int[] StonesRemaining;

        /// <summary>
        /// 2-element array containing the number of cap stones remaining for each player
        /// </summary>
        public int[] CapRemaining;

        /// <summary>
        /// Holds the contents of each cell on the board.  A separate list of pieces is contained in each cell.
        /// Element 0 of the list corresponds to the bottom of the stack.
        /// </summary>
        public List<int>[,] Board;

        static int[] EmptyStack = new int[] { };

        private GameState()
        {
        }

        public void Clear()
        {
            Ply = 0;
            InitReserve();
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Board[i, j].Clear();
        }

        void InitReserve()
        {
            switch (Size)
            {
                case 4:
                    StonesRemaining[0] = StonesRemaining[1] = 15;
                    CapRemaining[0] = CapRemaining[1] = 0;
                    break;
                case 5:
                    StonesRemaining[0] = StonesRemaining[1] = 20;
                    CapRemaining[0] = CapRemaining[1] = 1;
                    break;
                case 6:
                    StonesRemaining[0] = StonesRemaining[1] = 30;
                    CapRemaining[0] = CapRemaining[1] = 1;
                    break;
                case 7:
                    StonesRemaining[0] = StonesRemaining[1] = 40;
                    CapRemaining[0] = CapRemaining[1] = 2; // technically should be 1 or 2
                    break;
                case 8:
                    StonesRemaining[0] = StonesRemaining[1] = 50;
                    CapRemaining[0] = CapRemaining[1] = 2;
                    break;
                default:
                    throw new ArgumentException("Invalid board size");
            }
        }

        public static GameState NewGame(int size)
        {
            var game = new GameState();
            game.Size = size;
            game.Ply = 0;
            game.Board = new List<int>[size, size];
            game.StonesRemaining = new int[2];
            game.CapRemaining = new int[2];
            game.InitReserve();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    game.Board[i, j] = new List<int>(size);
            return game;
        }

        public static GameState LoadTPS(string tps)
        {
            if (tps[0] != '[')
                throw new ApplicationException("tps doesn't start with [");
            int start = 1;
            if (tps[1] == ' ')
                start = 2;
            int end = tps.Substring(start).IndexOf(' ');
            string[] rows = tps.Substring(start, end).Split('/');
            start += end + 1;
            end = tps.Substring(start).IndexOf(' ');
            int current_player = int.Parse(tps.Substring(start, end));
            int current_turn = int.Parse(tps.Substring(start + end + 1, 1));
            if (current_player != 1 && current_player != 2)
                throw new ApplicationException("player should be 1 or 2");
            if (tps.Substring(start + end + 2).IndexOf(']') < 0)
                throw new ApplicationException("tps doesn't end with ]");

            var game = GameState.NewGame(rows.Length);
            game.Ply = (current_turn - 1) * 2 + (current_player - 1); //silly TPS counts up from 1
            for(int i = 0; i < game.Size; i++)
            {
                string[] spaces = rows[i].Split(',');
                int space = 0;
                for(int j = 0; j < spaces.Length; j++)
                {
                    if (spaces[j][0] == 'x')
                    {
                        int reps = 1;
                        if (spaces[j].Length == 2)
                        {
                            reps = int.Parse(spaces[j][1].ToString());
                            if (reps < 2 || reps > game.Size)
                                throw new ApplicationException("repetition out of range: " + spaces[j]);
                        }
                        else if (spaces[j].Length != 1)
                            throw new ApplicationException("incorrect empty space notation: " + spaces[j]);
                        space += reps;
                    }
                    else
                    {
                        int stack_height = spaces[j].Length;
                        int top = Piece.Stone_Flat;
                        if (spaces[j][stack_height - 1] == 'S' || spaces[j][stack_height - 1] == 'C')
                        {
                            stack_height--;
                            top = spaces[j][stack_height] == 'S' ? Piece.Stone_Standing : Piece.Stone_Cap;
                        }
                        for (int k = 0; k < stack_height; k++)
                        {
                            int owner = spaces[j][k] == '1' ? 0 : 1;
                            if (owner == 1 && spaces[j][k] != '2')
                                throw new ApplicationException("stones in a stack must be owned by player 1 or player 2: " + spaces[j]);

                            if (k == stack_height - 1)
                            {
                                game.Board[i, j].Add(Piece.MakePieceID(top, owner));
                                if(top == Piece.Stone_Cap)
                                {
                                    game.CapRemaining[owner]--;
                                }
                                else
                                {
                                    game.StonesRemaining[owner]--;
                                }
                            }
                            else
                            {
                                game.Board[i, j].Add(Piece.MakePieceID(Piece.Stone_Flat, owner));
                                game.StonesRemaining[owner]--;
                            }
                        }
                    }
                }
            }
            return game;
        }

        public GameState DeepCopy()
        {
            var clone = new GameState();
            clone.Size = Size;
            clone.Ply = Ply;
            clone.StonesRemaining = new int[] { StonesRemaining[0], StonesRemaining[1] };
            clone.CapRemaining = new int[] { CapRemaining[0], CapRemaining[1] };
            clone.Board = new List<int>[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    clone.Board[i, j] = new List<int>(Board[i, j]);
            return clone;
        }

        /// <summary>
        /// Gets the Piece ID for the top of the stack only.  If no stack exists at the specified coordinates then returns null.
        /// Coordinate (0,0) is the bottom-left corner of the board
        /// </summary>
        /// <param name="x">X coordinate (0 to board size - 1)</param>
        /// <param name="y">Y coordinate (0 to board size - 1)</param>
        /// <returns>Piece ID of the top of the stack, or null if stack is empty</returns>
        public int? this[int x, int y]
        {
            get
            {
                int? result = null;
                var stack = Board[x, y];
                if (stack.Count > 0)
                    result = stack[stack.Count - 1];
                return result;
            }
        }

        /// <summary>
        /// Gets the Piece ID for the top of the stack only.  If no stack exists at the specified coordinates then returns null.
        /// Coordinate (0,0) is the bottom-left corner of the board
        /// </summary>
        /// <param name="p">Board coordinates.  (0,0) is the bottom left corner of the board</param>
        /// <returns>Piece ID of the top of the stack, or null if stack is empty</returns>
        public int? this[BoardPosition p] { get { return this[p.X, p.Y]; } }

        public bool IsPositionLegal(BoardPosition p)
        {
            return p.X >= 0 && p.Y >= 0 && p.X < Size && p.Y < Size;
        }
    }
}
