using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakEngine;

namespace TakGame_WinForms
{
    class InteractiveMove_PlaceFromReserve : IBoardInteraction
    {
        GameState _game;
        BoardView _boardView;
        IMove _move;
        int _pieceID;

        public InteractiveMove_PlaceFromReserve(GameState game, BoardView boardView, int pieceID)
        {
            _game = game;
            _boardView = boardView;
            _pieceID = pieceID;
            _boardView.CarryClear();
            _boardView.CarryAdd(pieceID);
            _boardView.CarryVisible = true;
        }

        public bool Completed { get { return _move != null; } }
        public bool HasPreview {  get { return _move != null; } }
        public IMove GetMove() { return _move; }

        public void AcceptPreview()
        {
            _boardView.CarryClear();
            _boardView.ClearHighlights();
        }

        public void Cancel()
        {
            CancelPreview();
            _boardView.CarryClear();
        }

        public void CancelPreview()
        {
            if (_move != null)
            {
                _boardView.ClearHighlights();
                _move.TakeBackMove(_game);
                _boardView.InvalidateRender();
                _move = null;
            }
            _boardView.CarryVisible = true;
        }

        public bool TryPreviewAt(BoardStackPosition mouseOver)
        {
            if (!_game[mouseOver.Position].HasValue)
            {
                _boardView.CarryVisible = false;
                _move = new PlacePieceMove(_pieceID, mouseOver.Position, true, false);
                _move.MakeMove(_game);
                _boardView.InvalidateRender();
                _boardView.SetHighlight(mouseOver.Position, 0);
                return true;
            }
            return false;
        }
    }
}
