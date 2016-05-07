namespace TakEngine
{
    public interface ITakAI
    {
        IMove FindGoodMove(GameState game);
        string EvalMethod { get; }
        int MaxDepth { get; set; }
        BoardPosition[] NormalPositions { get; }
        void Cancel();
        bool Canceled { get; }
    }
}
