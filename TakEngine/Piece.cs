using System;
namespace TakEngine
{
    public static class Piece
    {
        public const int Stone_Flat = 0;
        public const int Stone_Standing = 1;
        public const int Stone_Cap = 2;
        public const int STONE_COUNT = 3;

        public const int Stone_Mask = 0x7;

        public static int GetStone(int pieceID)
        {
            return pieceID & Stone_Mask;
        }

        public static int GetPlayerID(int pieceID)
        {
            return pieceID >> 3;
        }

        public static int MakePieceID(int stone, int playerid)
        {
            return stone | (playerid << 3);
        }

        public static string Describe(int pieceID)
        {
            int stone = GetStone(pieceID);
            switch (stone)
            {
                case Stone_Flat:
                    return string.Empty;
                case Stone_Standing:
                    return "S";
                case Stone_Cap:
                    return "C";
                default:
                    throw new ArgumentException("Invalid piece ID");
            }
        }
    }
}
