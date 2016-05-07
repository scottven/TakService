using TakEngine;
namespace TakGame_WinForms
{
    interface IBoardInteraction
    {
        bool TryPreviewAt(BoardStackPosition position);
        void CancelPreview();
        void Cancel();
        void AcceptPreview();
        bool Completed { get; }
        bool HasPreview { get; }
        IMove GetMove();
    }
}
