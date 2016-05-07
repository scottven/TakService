using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TakGame_WinForms
{
    public static class IsoGeometry
    {
        public static class Board
        {
            /// <summary>
            /// Pixel coordinates of the center of the bottom-left square
            /// </summary>
            public static PointF BottomLeftSquare(int gameSize)
            {
                switch (gameSize)
                {
                    case 4:
                        return new PointF(61, 101);
                    case 5:
                        return new PointF(62, 127);
                    case 6:
                        return new PointF(60, 153);
                    case 7:
                        return new PointF(60, 178);
                    case 8:
                        return new PointF(62, 203);
                    default:
                        throw new ArgumentException();
                }
            }

            public static class Square
            {
                public readonly static PointF XDelta = new PointF(203.0f * 0.25f, 60.0f * 0.25f);
                public readonly static PointF YDelta = new PointF(118.0f * 0.25f, -103.0f * 0.25f);
            }
        }

        public static class FlatStone
        {
            public const float Height = 10.0f;
        }
    }
}
