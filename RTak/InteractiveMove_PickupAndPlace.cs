using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakEngine;

namespace TakGame_WinForms
{
    class InteractiveMove_PickupAndPlace : IBoardInteraction
    {
        GameState _game;
        BoardView _boardView;
        PickupAndPlaceMove _move = null;
        PlacePieceMove _previewPlace = null;
        PickUpMove _previewPickup = null;

        public InteractiveMove_PickupAndPlace(GameState game, BoardView boardView)
        {
            _game = game;
            _boardView = boardView;
            _boardView.CarryClear();
            _boardView.CarryVisible = false;
        }

        public bool Completed
        {
            get
            {
                if (_move != null)
                {
                    int pickupCount = _move.PickUpMove.PickUpCount;
                    int placeCount = _move.Count - 1 + (_previewPlace != null ? 1 : 0);
                    return pickupCount == placeCount;
                }
                return false;
            }
        }
        public bool HasPreview { get { return _previewPickup != null || _previewPlace != null; } }
        public IMove GetMove() { return _move; }

        public void AcceptPreview()
        {
            _boardView.ClearHighlights();
            if (_previewPickup != null)
            {
                if (_move != null)
                {
                    int oldCount = _move.PickUpMove.PickUpCount;
                    _move.TakeBackMove(_game);
                    _previewPickup = new PickUpMove(
                        _previewPickup.Position,
                        oldCount + _previewPickup.PickUpCount,
                        _game);
                    _boardView.CarryClear();
                }
                _move = new PickupAndPlaceMove(_previewPickup);
                _move.MakeMove(_game);
                _boardView.CarryVisible = true;
                foreach (var pieceID in _move.PickUpMove.PickUpPieces)
                    _boardView.CarryAdd(pieceID);
                _previewPickup = null;
            }
            else if (_previewPlace != null)
            {
                _move.AddToChain(_previewPlace);
                _previewPlace = null;
            }
        }

        public void Cancel()
        {
            CancelPreview();
            if (_move != null)
            {
                _move.TakeBackMove(_game);
                _boardView.InvalidateRender();
                _move = null;
            }
            _boardView.CarryClear();
        }

        public void CancelPreview()
        {
            if (_previewPlace != null)
            {
                _previewPlace.TakeBackMove(_game);
                _boardView.InvalidateRender();
                _boardView.CarryInsert(_previewPlace.PieceID);
                _previewPlace = null;
            }
            // NOTE: do not need to undo pick move as it doesn't really get applied until AcceptPreview
            _boardView.ClearHighlights();
        }

        public bool TryPreviewAt(BoardStackPosition mouseOver)
        {
            if (_game.Ply < 2)
                return false;
            _boardView.CarryVisible = true;
            var mouseOverPos = mouseOver.Position;
            if ((_move == null || (_move.PickUpMove.Position == mouseOver.Position && !_move.UnstackDirection.HasValue))
                && mouseOver.StackPos.HasValue)
            {
                int carryCount = 0;
                if (_move != null)
                    carryCount = _move.PickUpMove.PickUpCount;
                var stack = _game.Board[mouseOverPos.X, mouseOverPos.Y];
                if (stack.Count == 0)
                    return false;
                if (carryCount == 0 && Piece.GetPlayerID(stack[stack.Count - 1]) != (_game.Ply & 1))
                    return false;
                int pickUpCount = stack.Count - mouseOver.StackPos.Value;
                if (pickUpCount + carryCount <= _game.Size)
                {
                    _previewPickup = new PickUpMove(mouseOverPos, stack.Count - mouseOver.StackPos.Value, _game);
                    _boardView.SetHighlight(mouseOverPos, mouseOver.StackPos);
                    return true;
                }
            }
            else
            {
                // check for placing pieces
                if (_move == null)
                    return false;
                var placingPiece = _move.PickUpMove.PickUpPieces[_move.Count - 1];
                var covering = _game[mouseOverPos];
                bool flatten = false;
                if (covering.HasValue)
                {
                    // cannot put piece on top of cap stone
                    if (Piece.GetStone(covering.Value) == Piece.Stone_Cap)
                        return false;

                    // standing stone only gets flattened by cap stone
                    if (Piece.GetStone(covering.Value) == Piece.Stone_Standing)
                    {
                        if (Piece.GetStone(placingPiece) != Piece.Stone_Cap)
                            return false;
                        else
                            flatten = true;
                    }
                }

                // validate direction
                var unstackDir = _move.UnstackDirection;
                if (!unstackDir.HasValue)
                {
                    var delta = mouseOverPos - _move.PickUpMove.Position;
                    if (Math.Abs(delta.X) + Math.Abs(delta.Y) != 1)
                        return false;
                }
                else
                {
                    var lastPlacedAt = ((PlacePieceMove)_move[_move.Count - 1]).Pos;
                    var delta = mouseOverPos - lastPlacedAt;
                    if (delta != BoardPosition.Zero &&
                        mouseOverPos != Direction.Offset(lastPlacedAt, unstackDir.Value))
                        return false;
                }

                _previewPickup = null;
                _previewPlace = new PlacePieceMove(placingPiece, mouseOverPos, false, flatten);
                _previewPlace.MakeMove(_game);
                _boardView.CarryRemoveBottom();
                _boardView.SetHighlight(mouseOverPos, _game.Board[mouseOverPos.X, mouseOverPos.Y].Count - 1);
                _boardView.InvalidateRender();
                _boardView.CarryVisible = true;
                return true;
            }
            return false;
        }
    }
}
