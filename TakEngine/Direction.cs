using System;

namespace TakEngine
{
    public static class Direction
    {
        public const int East = 0;
        public const int North = 1;
        public const int West = 2;
        public const int South = 3;

        public static int[] DirX = new int[] { 1, 0, -1, 0 };
        public static int[] DirY = new int[] { 0, 1, 0, -1 };
        public static char[] DirName = new char[] { '>', '+', '<', '-' };

        public static BoardPosition Offset(BoardPosition pos, int dir)
        {
#if DEBUG
            if (dir < 0 || dir >= 4)
                throw new ArgumentException("Invalid direction");
#endif
            return new BoardPosition(pos.X + DirX[dir], pos.Y + DirY[dir]);
        }

        public static char DescribeDelta(BoardPosition delta)
        {
            return DirName[FromDelta(delta)];
        }

        public static int FromDelta(BoardPosition delta)
        {
            if (delta.X == 0)
            {
                if (delta.Y == 1)
                    return North;
                else if (delta.Y == -1)
                    return South;
            }
            else if (delta.Y == 0)
            {
                if (delta.X == 1)
                    return East;
                else if (delta.X == -1)
                    return West;
            }
            throw new ArgumentException("Delta must be a unit-length vector in a cardinal direction");
        }
    }
}
