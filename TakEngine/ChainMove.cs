using System.Collections.Generic;

namespace TakEngine
{
    /// <summary>
    /// Abstract class for representing a move as a series of moves that are performed in sequence (and taken back in reverse order)
    /// </summary>
    public abstract class ChainMove : IMove
    {
        protected List<IMove> Moves = new List<IMove>(4);

        public ChainMove()
        {
        }

        public ChainMove(IEnumerable<IMove> moves)
        {
            Moves.AddRange(moves);
        }

        public void AddToChain(IMove move)
        {
            Moves.Add(move);
        }

        /// <summary>
        /// Perform the contained moves in order
        /// </summary>
        /// <param name="game"></param>
        public void MakeMove(GameState game)
        {
            for (int i = 0; i < Moves.Count; i++)
                Moves[i].MakeMove(game);
        }

        /// <summary>
        /// Take back the contained moves in reverse order
        /// </summary>
        /// <param name="game"></param>
        public void TakeBackMove(GameState game)
        {
            for (int i = Moves.Count - 1; i >= 0; i--)
                Moves[i].TakeBackMove(game);
        }
        public int Count { get { return Moves.Count; } }
        public IMove this[int index] { get { return Moves[index]; } }

        public void RemoveFromEnd(int count)
        {
            Moves.RemoveRange(Moves.Count - count, count);
        }

        /// <summary>
        /// Notation is only exists for full-defined moves, so this must be marked abstract
        /// </summary>
        /// <returns></returns>
        public abstract string Notate();
    }
}
