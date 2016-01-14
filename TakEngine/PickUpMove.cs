using System;
using System.Collections.Generic;

namespace TakEngine
{
    /// <summary>
    /// IMove implementation for picking up stones from the top of a stack and putting them in our hand.
    /// This is NOT a completely legal move in the game, but this object is part of the aggregate PickUpAndPlaceMove
    /// </summary>
    public class PickUpMove : IMove
    {
        /// <summary>
        /// Board position at which pieces will be picked up
        /// </summary>
        BoardPosition _pos;

        /// <summary>
        /// Array of pieces in our hand after picking up pieces.
        /// Element 0 is the bottom, and should be placed first when dropping pieces in adjacent cells
        /// </summary>
        int[] _pickupPieces;

        public readonly int Remaining;

        /// <summary>
        /// Creates the PickUpMove and also inspects the game state to determine which pieces will be picked up
        /// </summary>
        /// <param name="pos">Stack position</param>
        /// <param name="pickUpCount">Number of pieces to pick up from the top of the stack</param>
        /// <param name="game">Game state</param>
        public PickUpMove(BoardPosition pos, int pickUpCount, GameState game)
        {
            _pos = pos;
            _pickupPieces = new int[pickUpCount];
            var stack = game.Board[_pos.X, _pos.Y];
            for (int i = 0; i < _pickupPieces.Length; i++)
                _pickupPieces[i] = stack[stack.Count - _pickupPieces.Length + i];
            Remaining = stack.Count - pickUpCount;
        }

        public int PickUpCount { get { return _pickupPieces.Length; } }

        /// <summary>
        /// Gets the list of pieces to be picked up.  Element 0 is the bottom which should be placed first when dropping pieces.
        /// </summary>
        public IReadOnlyList<int> PickUpPieces {  get { return _pickupPieces; } }

        /// <summary>
        /// Gets whether the stone at the specified position in the hand is a cap stone
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool IsCapStone(int offset) { return Piece.GetStone(_pickupPieces[offset]) == Piece.Stone_Cap; }

        public void MakeMove(GameState game)
        {
            var stack = game.Board[_pos.X, _pos.Y];
            for (int i = 0; i < _pickupPieces.Length; i++)
                stack.RemoveAt(stack.Count - 1);
        }

        public void TakeBackMove(GameState game)
        {
            var stack = game.Board[_pos.X, _pos.Y];
            for (int i = 0; i < _pickupPieces.Length; i++)
                stack.Add(_pickupPieces[i]);
        }

        /// <summary>
        /// Create a PlacePieceMove for the piece in the specified position of our hand
        /// </summary>
        /// <param name="offset">Hand position, 0 = bottom which is placed first</param>
        /// <param name="pos">Board position at which the piece will be placed</param>
        /// <param name="flatten">True if the placement action will result in flattening a standing stone</param>
        /// <returns></returns>
        public PlacePieceMove GeneratePlaceFromStack(int offset, BoardPosition pos, bool flatten)
        {
            return new PlacePieceMove(_pickupPieces[offset], pos, false, flatten);
        }
        public BoardPosition Position { get { return _pos; } }

        /// <summary>
        /// INVALID OPERATION because this isn't a legal move by itself
        /// </summary>
        public string Notate()
        {
            throw new InvalidOperationException();
        }
    }
}
