namespace TakEngine
{
    /// <summary>
    /// Move which places the specified piece on the top of the stack at the given board coordinates
    /// </summary>
    public class PlacePieceMove : IMove
    {
        /// <summary>
        /// Piece to place on top of the stack
        /// </summary>
        public int PieceID;

        /// <summary>
        /// Board position at which to place the piece
        /// </summary>
        public BoardPosition Pos;

        /// <summary>
        /// True if this stone is being placed from the player's reserve
        /// </summary>
        public bool FromReserve;

        /// <summary>
        /// True if this placement action is going to flatten a standing stone
        /// </summary>
        public bool Flatten;

        /// <summary>
        /// Create move for placing a stone on top of a stack
        /// </summary>
        /// <param name="piece">PieceID of the stone being placed</param>
        /// <param name="pos">Position at which the stone will be placed</param>
        /// <param name="fromReserve">True if the stone is being played from the player's reserve</param>
        /// <param name="flatten">True if this placement action is going to flatten a standing stone</param>
        public PlacePieceMove(int piece, BoardPosition pos, bool fromReserve, bool flatten)
        {
            PieceID = piece;
            Pos = pos;
            FromReserve = fromReserve;
            Flatten = flatten;
        }

        public void MakeMove(GameState game)
        {
            var stack = game.Board[Pos.X, Pos.Y];
            if (Flatten)
                stack[stack.Count - 1] = Piece.MakePieceID(Piece.Stone_Flat, Piece.GetPlayerID(stack[stack.Count - 1]));
            game.Board[Pos.X, Pos.Y].Add(PieceID);
            var stone = Piece.GetStone(PieceID);
            var player = Piece.GetPlayerID(PieceID);
            if (FromReserve)
            {
                if (stone == Piece.Stone_Cap)
                    game.CapRemaining[player]--;
                else
                    game.StonesRemaining[player]--;
            }
        }

        public void TakeBackMove(GameState game)
        {
            var stack = game.Board[Pos.X, Pos.Y];
            stack.RemoveAt(stack.Count - 1);
            var stone = Piece.GetStone(PieceID);
            var player = Piece.GetPlayerID(PieceID);
            if (FromReserve)
            {
                if (stone == Piece.Stone_Cap)
                    game.CapRemaining[player]++;
                else
                    game.StonesRemaining[player]++;
            }
            if (Flatten)
                stack[stack.Count - 1] = Piece.MakePieceID(Piece.Stone_Standing, Piece.GetPlayerID(stack[stack.Count - 1]));
        }

        public string Notate()
        {
            return string.Concat(Piece.Describe(PieceID), Pos.Describe());
        }

        public override string ToString()
        {
            return Notate();
        }
    }
}
