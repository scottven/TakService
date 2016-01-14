using System;
using System.Collections.Generic;
using System.Text;

namespace TakEngine
{
    /// <summary>
    /// Delegate which checks if a given position matches the flood fill condition.  NOTE: To avoid infinite recursion by the
    /// flood fill algorithm, it's important that this delegate NOT match a position which has already been filled/painted by the algorithm!
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="y">y position</param>
    /// <returns>Returns true if the position matches the flood fill condition and should be painted/marked</returns>
    public delegate bool DoesLocationMatchDelegate(int x, int y);

    /// <summary>
    /// Delegate to mark / paint / (whatever you want to call it) a position which matches the flood fill condition
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public delegate void PaintLocationDelegate(int x, int y);

    /// <summary>
    /// Flood fill algorithm.
    /// NOTE: It is not safe for multiple threads to use this object at the same time.
    /// </summary>
    public class FloodFill
    {
        Queue<BoardPosition> _queue = new Queue<BoardPosition>();

        /// <summary>
        /// Execute flood fill algorithm, starting at the specified coordinates and only spreading in 4
        /// directions from each cell (i.e. no diagonals)
        /// </summary>
        public void Fill(int x, int y, DoesLocationMatchDelegate doesMatch, PaintLocationDelegate paint)
        {
            _queue.Clear();
            if (!doesMatch(x, y))
                return;
            _queue.Enqueue(new BoardPosition(x, y));

            while (_queue.Count > 0)
            {
                var p = _queue.Dequeue();
                if (doesMatch(p.X, p.Y))
                {
                    var xLeft = p.X;
                    var xRight = p.X;
                    while (doesMatch(xLeft - 1, p.Y))
                        xLeft--;
                    while (doesMatch(xRight + 1, p.Y))
                        xRight++;

                    for (int xCurrent = xLeft; xCurrent <= xRight; xCurrent++)
                    {
                        paint(xCurrent, p.Y);
                        if (doesMatch(xCurrent, p.Y - 1))
                            _queue.Enqueue(new BoardPosition(xCurrent, p.Y - 1));
                        if (doesMatch(xCurrent, p.Y + 1))
                            _queue.Enqueue(new BoardPosition(xCurrent, p.Y + 1));
                    }
                }
            }
        }

        /// <summary>
        /// Execute flood fill algorithm, starting at the specified coordinates and spreading in all 8
        /// directions from each cell (i.e. including diagonal directions)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="doesMatch"></param>
        /// <param name="paint"></param>
        public void FillDiag(int x, int y, DoesLocationMatchDelegate doesMatch, PaintLocationDelegate paint)
        {
            _queue.Clear();
            if (!doesMatch(x, y))
                return;
            _queue.Enqueue(new BoardPosition(x, y));

            while (_queue.Count > 0)
            {
                var p = _queue.Dequeue();
                if (doesMatch(p.X, p.Y))
                {
                    var xLeft = p.X;
                    var xRight = p.X;
                    while (doesMatch(xLeft - 1, p.Y))
                        xLeft--;
                    while (doesMatch(xRight + 1, p.Y))
                        xRight++;

                    for (int xCurrent = xLeft; xCurrent <= xRight; xCurrent++)
                    {
                        paint(xCurrent, p.Y);
                        if (doesMatch(xCurrent, p.Y - 1))
                            _queue.Enqueue(new BoardPosition(xCurrent, p.Y - 1));
                        if (doesMatch(xCurrent, p.Y + 1))
                            _queue.Enqueue(new BoardPosition(xCurrent, p.Y + 1));
                    }
                    if (doesMatch(xLeft - 1, p.Y - 1))
                        _queue.Enqueue(new BoardPosition(xLeft - 1, p.Y - 1));
                    if (doesMatch(xLeft - 1, p.Y + 1))
                        _queue.Enqueue(new BoardPosition(xLeft - 1, p.Y + 1));
                    if (doesMatch(xRight + 1, p.Y - 1))
                        _queue.Enqueue(new BoardPosition(xRight + 1, p.Y - 1));
                    if (doesMatch(xRight + 1, p.Y + 1))
                        _queue.Enqueue(new BoardPosition(xRight + 1, p.Y + 1));
                }
            }
        }
    }
}
