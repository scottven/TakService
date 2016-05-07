using System.Collections.Generic;
using System.Drawing;
using TakEngine;
using System.IO;

namespace TakGame_WinForms
{
    static class IsoImages
    {
        static Dictionary<int, Bitmap> _images;
        static Dictionary<int, Bitmap> _himages;
        static Bitmap _board;
        static Bitmap _squaremask;
        static int _boardSize = -1;

        public static Bitmap GetPieceImage(int pieceID, bool highlighted = false)
        {
            EnsureTilesLoaded();
            if (highlighted)
                return _himages[pieceID];
            else
                return _images[pieceID];
        }

        public static Bitmap GetBoardImage(int boardSize)
        {
            if (_boardSize != boardSize)
            {
                if (_board != null)
                    _board.Dispose();
                _board = null;
            }
            if (_board == null)
                _board = ImageUtil.LoadFormattedFromFile(Path.Combine(BinRoot, string.Format("Iso/board{0}.png", boardSize)));
            return _board;
        }

        public static Bitmap GetSquareMaskImage()
        {
            EnsureTilesLoaded();
            return _squaremask;
        }

        static string BinRoot { get { return Path.GetDirectoryName(typeof(IsoImages).Assembly.Location); } }

        static void EnsureTilesLoaded()
        {
            if (_images != null)
                return;
            _images = new Dictionary<int, Bitmap>();
            var binRoot = BinRoot;
            _images[Piece.MakePieceID(Piece.Stone_Cap, 0)] = ImageUtil.LoadFormattedFromFile(Path.Combine(binRoot, "Iso/cap0.png"));
            _images[Piece.MakePieceID(Piece.Stone_Cap, 1)] = ImageUtil.LoadFormattedFromFile(Path.Combine(binRoot, "Iso/cap1.png"));
            _images[Piece.MakePieceID(Piece.Stone_Flat, 0)] = ImageUtil.LoadFormattedFromFile(Path.Combine(binRoot, "Iso/flat0.png"));
            _images[Piece.MakePieceID(Piece.Stone_Flat, 1)] = ImageUtil.LoadFormattedFromFile(Path.Combine(binRoot, "Iso/flat1.png"));
            _images[Piece.MakePieceID(Piece.Stone_Standing, 0)] = ImageUtil.LoadFormattedFromFile(Path.Combine(binRoot, "Iso/stand0.png"));
            _images[Piece.MakePieceID(Piece.Stone_Standing, 1)] = ImageUtil.LoadFormattedFromFile(Path.Combine(binRoot, "Iso/stand1.png"));
            _squaremask = ImageUtil.LoadFormattedFromFile(Path.Combine(binRoot, "Iso/squaremask.png"));

            _himages = new Dictionary<int, Bitmap>();
            foreach (var key in _images.Keys)
                _himages[key] = ImageUtil.DrawHighlighted(_images[key]);
        }
    }
}
