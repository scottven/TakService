using System.Drawing;
using System.Drawing.Imaging;

namespace TakGame_WinForms
{
    /// <summary>
    /// Provides generic image processing functions
    /// </summary>
    static class ImageUtil
    {
        /// <summary>
        /// Load image file as a bitmap with the specified pixel format
        /// </summary>
        /// <param name="path">Image file name</param>
        /// <returns>Returns Bitmap object</returns>
        public static Bitmap LoadFormattedFromFile(string path, PixelFormat format = PixelFormat.Format32bppPArgb)
        {
            Bitmap converted;
            using (var unconverted = Image.FromFile(path))
            {
                converted = new Bitmap(unconverted.Width, unconverted.Height, format);
                using (var g = Graphics.FromImage(converted))
                {
                    var rect = new Rectangle(0, 0, unconverted.Width, unconverted.Height);
                    g.DrawImage(unconverted, rect, rect, GraphicsUnit.Pixel);
                }
            }
            return converted;
        }

        public static Bitmap DrawHighlighted(Bitmap bmp, PixelFormat format = PixelFormat.Format32bppPArgb)
        {
            var attr = new ImageAttributes();
            float[][] matElements = {
                new float[] { 0.75f, 0.0f, 0.0f, 0.0f, 0.0f },
                new float[] { 0.0f, 0.75f, 0.0f, 0.0f, 0.0f },
                new float[] { 0.0f, 0.0f, 0.75f, 0.0f, 0.0f },
                new float[] { 0.0f, 0.0f,  0.0f, 1.0f, 0.0f },
                new float[] { 0.25f, 0.25f, 0.25f, 0.0f, 1.0f }
            };

            attr.SetColorMatrix(new ColorMatrix(matElements), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            Bitmap dest = new Bitmap(bmp.Width, bmp.Height, format);
            using (var g = Graphics.FromImage(dest))
            {
                var destRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                g.DrawImage(
                    bmp,
                    destRect,
                    0,
                    0,
                    bmp.Width,
                    bmp.Height,
                    GraphicsUnit.Pixel,
                    attr);
            }
            return dest;
        }
    }
}
