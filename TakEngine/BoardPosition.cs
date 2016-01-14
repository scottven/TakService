namespace TakEngine
{
    /// <summary>
    /// Simple struct for holding X,Y board coordinates and performing basic vector
    /// arithmetic for convenience
    /// </summary>
    public struct BoardPosition
    {
        public int X;
        public int Y;

        public static readonly BoardPosition Zero = new BoardPosition(0, 0);

        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static BoardPosition operator +(BoardPosition p1, BoardPosition p2) {
            return new BoardPosition(p1.X + p2.X , p1.Y + p2.Y);
        }

        public static BoardPosition operator -(BoardPosition p1, BoardPosition p2) {
            return new BoardPosition(p1.X - p2.X, p1.Y - p2.Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoardPosition))
                return false;
            else
                return this == (BoardPosition)obj;
        }

        public static bool operator ==(BoardPosition p1, BoardPosition p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(BoardPosition p1, BoardPosition p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        public override int GetHashCode()
        {
            return ((X << 5) + X) ^ Y;
        }

        public string Describe() { return string.Format("{0}{1}", (char)('a' + X), Y + 1); }
    }
}
