using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TakEngine;

namespace TakGame_WinForms
{
    public partial class BoardView : Control
    {
        public BoardView()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// Height at which each stack begins to be highlighted
        /// </summary>
        Dictionary<BoardPosition, int?> _highlights = new Dictionary<BoardPosition, int?>();

        GameState _game;
        public GameState Game
        {
            get { return _game; }
            set
            {
                if (value != _game)
                {
                    _game = value;
                    InvalidateRender();
                }
            }
        }

        /// <summary>
        /// Tracks the bounding rectangle of pieces / squares that have been drawn on the board
        /// so that we can perform hit detection more easily
        /// </summary>
        class RenderLayoutItem
        {
            public BoardStackPosition BoardStackPosition;
            public Rectangle RenderRect;
            public Bitmap Sprite;
            public RenderLayoutItem(BoardPosition pos, int? stackPos, Rectangle renderRect, Bitmap sprite)
            {
                this.BoardStackPosition.Position = pos;
                this.BoardStackPosition.StackPos = stackPos;
                RenderRect = renderRect;
                Sprite = sprite;
            }
        }

        /// <summary>
        /// Stores bounding rectangles and associated bitmaps for the pieces and squares that have
        /// been drawn on the board
        /// </summary>
        List<RenderLayoutItem> _layoutItems = new List<RenderLayoutItem>();

        /// <summary>
        /// Set to false whenever the game state changes and the board needs to be redrawn
        /// </summary>
        bool _renderValid = false;

        /// <summary>
        /// Off-screen bitmap onto which the game state is drawn
        /// </summary>
        Bitmap _render = null;

        /// <summary>
        /// Mark the current rendering of the game state as invalid so that the entire board will be redrawn the next time this control paints itself
        /// </summary>
        public void InvalidateRender()
        {
            _renderValid = false;
            Invalidate();
        }

        /// <summary>
        /// Gets a sprite rectangle centered at the specified coordinates
        /// </summary>
        /// <param name="center">Coordinates at which the sprite should be centered</param>
        /// <param name="spriteSize">Size of the sprite image</param>
        /// <returns></returns>
        static Rectangle GetCenteredRect(PointF center, Size spriteSize)
        {
            return new Rectangle((int)(center.X - 0.5f * spriteSize.Width), (int)(center.Y - 0.5f * spriteSize.Height), spriteSize.Width, spriteSize.Height);
        }

        /// <summary>
        /// Gets a sprite rectangle whose 3D bottom position (defined as 32 pixels from the bottom of the sprite in the context of our current isometric images)
        /// is located at the specified center point
        /// </summary>
        /// <param name="center">Coordinates at which the sprite's 3D bottom should be centered</param>
        /// <param name="spriteSize">Size of the sprite image</param>
        /// <returns></returns>
        static Rectangle GetBottomedRect(PointF center, Size spriteSize)
        {
            return new Rectangle((int)(center.X - 0.5f * spriteSize.Width), (int)(center.Y - (spriteSize.Height - 32)), spriteSize.Width, spriteSize.Height);
        }

        public Size RecommendedSize
        {
            get
            {
                EnsureRender();
                return _render.Size;
            }
        }

        /// <summary>
        /// Redraws the off-screen board render as needed
        /// </summary>
        void EnsureRender()
        {
            if (!_renderValid)
            {
                // Clear layout information every time we redraw
                _layoutItems.Clear();
                int gameSize = _game != null ? _game.Size : 5;

                // Prepare to draw game board
                var board = IsoImages.GetBoardImage(gameSize);

                if (_render != null && _render.Width != board.Width)
                {
                    _render.Dispose();
                    _render = null;
                }

                if (_render == null)
                    // allocate enough extra vertical space to hold some tall stacks of stones
                    _render = new Bitmap(board.Width, board.Height + 120, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                using (var g = Graphics.FromImage(_render))
                {
                    // Draw the game board on a transparent background
                    g.Clear(Color.Transparent);
                    g.DrawImage(board, 0, _render.Height - board.Height);

                    // Draw game pieces
                    if (_game != null)
                    {
                        var boardul = new PointF(0, _render.Height - board.Height);
                        var mask = IsoImages.GetSquareMaskImage();
                        BoardPosition viewPos;

                        // Draw board pieces from back to front, bottom to top
                        var blSquareCenter = IsoGeometry.Board.BottomLeftSquare(gameSize);
                        for (viewPos.Y = gameSize - 1; viewPos.Y >= 0; viewPos.Y--)
                        {
                            for (viewPos.X = 0; viewPos.X < gameSize; viewPos.X++)
                            {
                                // Calculate center position for this square
                                var squareCenter = new PointF(
                                    boardul.X + blSquareCenter.X + IsoGeometry.Board.Square.XDelta.X * viewPos.X + IsoGeometry.Board.Square.YDelta.X * viewPos.Y,
                                    boardul.Y + blSquareCenter.Y + IsoGeometry.Board.Square.XDelta.Y * viewPos.X + IsoGeometry.Board.Square.YDelta.Y * viewPos.Y);

                                var pos = GetRotated(gameSize, viewPos, _rot);

                                // square mask goes at the bottom of the stack
                                var rect = GetCenteredRect(squareCenter, mask.Size);
                                _layoutItems.Add(new RenderLayoutItem(pos, null, rect, mask));

                                bool highlighting = false;
                                int? height = null;
                                if (_highlights.TryGetValue(pos, out height))
                                    highlighting = true;

                                if (highlighting && !height.HasValue)
                                {
                                    // highlight square
                                    g.DrawImage(mask, rect.Left, rect.Top);
                                }

                                // draw pieces from bottom to top
                                var stack = _game.Board[pos.X, pos.Y];
                                for (int i = 0; i < stack.Count; i++)
                                {
                                    var piece = stack[i];
                                    var sprite = IsoImages.GetPieceImage(piece, highlighted: highlighting && height.HasValue && i >= height.Value);
                                    rect = GetBottomedRect(squareCenter, sprite.Size);
                                    g.DrawImage(sprite, rect.Left, rect.Top);
                                    _layoutItems.Add(new RenderLayoutItem(pos, i, rect, sprite)); // remember layout information for mouse hit test
                                    squareCenter.Y -= IsoGeometry.FlatStone.Height;
                                }
                            }
                        }
                    }
                }

                // Reverse the order of sprites because the last ones drawn are actually in front, and should be tested first
                // when trying to determine which piece the user has clicked on
                _layoutItems.Reverse();
                _renderValid = true;
            }
        }

        /// <summary>
        /// Gets the position at which the off-screen game board image will be drawn onto the screen
        /// </summary>
        Point RenderPos
        {
            get
            {
                if (_render == null)
                    return new Point(0, 0);
                else
                    return new Point((this.Width - _render.Width) / 2, this.Height - _render.Height - BottomMargin);
            }
        }

        const int BottomMargin = 8;
        protected override void OnPaint(PaintEventArgs pe)
        {
            EnsureRender();

            var g = pe.Graphics;
            g.Clear(Color.Gray);

            // draw the board in the center
            var renderPos = this.RenderPos;
            g.DrawImage(_render, renderPos.X, renderPos.Y);

            // draw carry stack
            if (_carryPos.HasValue && _carryVisible && _carryStack.Count > 0)
            {
                var stackBottom = new PointF(_carryPos.Value.X, _carryPos.Value.Y);
                foreach (var pieceID in _carryStack)
                {
                    var sprite = IsoImages.GetPieceImage(pieceID);
                    var rect = GetBottomedRect(stackBottom, sprite.Size);
                    g.DrawImage(sprite, rect.Left, rect.Top);
                    stackBottom.Y -= IsoGeometry.FlatStone.Height;
                }
            }

            base.OnPaint(pe);
        }

        public BoardStackPosition? HitTest(Point client)
        {
            var renderPos = this.RenderPos;
            var p = new Point(client.X - renderPos.X, client.Y - renderPos.Y);
            foreach (var item in _layoutItems)
            {
                var itemRect = item.RenderRect;
                if (!itemRect.Contains(p))
                    continue;
                var spritePixel = new Point(p.X - itemRect.Left, p.Y - itemRect.Top);
                if (item.Sprite.GetPixel(spritePixel.X, spritePixel.Y).A > 0)
                {
                    return item.BoardStackPosition;
                }
            }
            return null;
        }

        Point _lastMouseMove;
        public void InvalidateMouseOverSpot()
        {
            EnsureRender();
            this.MouseOverSpot = HitTest(_lastMouseMove);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _lastMouseMove = e.Location;
            this.MouseOverSpot = HitTest(_lastMouseMove);
            if (e.Location != _carryPos)
            {
                _carryPos = e.Location;
                if (_carryVisible && _carryStack.Count > 0)
                    Invalidate(true);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_carryPos.HasValue)
            {
                _carryPos = null;
                if (_carryVisible && _carryStack.Count > 0)
                    Invalidate(true);
            }
            base.OnMouseLeave(e);
        }

        BoardStackPosition? _mouseOverSpot;
        public BoardStackPosition? MouseOverSpot
        {
            get { return _mouseOverSpot; }
            set
            {
                if (value.HasValue != _mouseOverSpot.HasValue ||
                    (value.HasValue && (value.Value.Position != _mouseOverSpot.Value.Position || value.Value.StackPos != _mouseOverSpot.Value.StackPos)))
                {
                    _mouseOverSpot = value;
                    MouseOverSpotChanged(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler MouseOverSpotChanged = delegate { };

        public void ClearHighlights()
        {
            if (_highlights.Count > 0)
            {
                _highlights.Clear();
                InvalidateRender();
            }
        }

        public void SetHighlight(BoardPosition pos, int? height)
        {
            int? oldValue;
            if (!_highlights.TryGetValue(pos, out oldValue))
                InvalidateRender();
            else if (oldValue != height)
                InvalidateRender();
            _highlights[pos] = height;
        }

        List<int> _carryStack = new List<int>();
        public void CarryClear()
        {
            if (_carryStack.Count > 0)
            {
                _carryStack.Clear();
                if (_carryVisible)
                    Invalidate(true);
            }
        }

        public void CarryAdd(int pieceID)
        {
            _carryStack.Add(pieceID);
            if (_carryVisible)
                Invalidate(true);
        }

        public void CarryInsert(int pieceID)
        {
            _carryStack.Insert(0, pieceID);
            if (_carryVisible)
                Invalidate(true);
        }

        public void CarryRemoveBottom()
        {
            _carryStack.RemoveAt(0);
            if (_carryVisible)
                Invalidate(true);
        }

        bool _carryVisible = true;
        public bool CarryVisible
        {
            get { return _carryVisible; }
            set
            {
                if (value != _carryVisible)
                {
                    _carryVisible = value;
                    if (_carryStack.Count > 0)
                        Invalidate(true);
                }
            }
        }
        Point? _carryPos = null;

        int _rot;
        public void RotateView()
        {
            _rot = (_rot + 1) % 4;
            InvalidateRender();
        }

        static BoardPosition GetRotated(int gameSize, BoardPosition pos, int rot)
        {
            if (rot == 0)
                return pos;

            // all coordinates will be multiplied by 2 for this calculation in order to handle the center point for even-numbered board sizes more easily

            // define center position
            var center = new BoardPosition(gameSize - 1, gameSize - 1);
            pos += pos;
            pos -= center;
            for (int i = 0; i < rot; i++)
            {
                pos = new BoardPosition(pos.Y, -pos.X);
            }
            pos += center;
            pos.X /= 2;
            pos.Y /= 2;
            return pos;
        }
    }

    public struct BoardStackPosition
    {
        public BoardPosition Position;
        public int? StackPos;
    }
}
