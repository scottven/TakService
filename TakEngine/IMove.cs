namespace TakEngine
{
    /// <summary>
    /// Common interface implemented by objects that can modify the game's state to make a move or reverse it
    /// </summary>
    public interface IMove
    {
        /// <summary>
        /// Make a move by altering the game's state.
        /// NOTE: this method should NOT alter the game's turn number, only the pieces
        /// </summary>
        /// <param name="game"></param>
        void MakeMove(GameState game);

        /// <summary>
        /// Undo the game state changes that were applied by the MakeMove() method
        /// </summary>
        /// <param name="game"></param>
        void TakeBackMove(GameState game);

        /// <summary>
        /// Get the official PTN representation of this move
        /// </summary>
        /// <returns>Move notation as a string</returns>
        string Notate();
    }
}
