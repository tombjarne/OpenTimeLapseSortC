using System;
using System.Drawing;

namespace OpenTimelapseSort.DataServices
{
    internal class ImageProcessingService
    {
        public byte[] ImageToByteArray(SImage image)
        {
            return System.IO.File.ReadAllBytes(image.Target);
        }

        public void SetImageMetaValues(SImage image)
        {
            var iBytes = ImageToByteArray(image);
            var iLumen = 0.0;
            long iMatrix = 0x00;

            for (var i = 4; i < iBytes.Length; i++)
            {
                if (i % 45000 == 0)
                {
                    var pixel = Color.FromArgb(iBytes[i], iBytes[i - 1], iBytes[i - 2], iBytes[i - 3]);
                    iMatrix += pixel.G;
                    iLumen += 0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B;
                }
            }

            image.Lumen = iLumen;
            image.Colors = iMatrix;

            //GC.Collect();
        }

        public void SetImageMetaValuesLite(SImage image)
        {
            var iBytes = ImageToByteArray(image);
            var iLumen = 0.0;
            long iMatrix = 0x00;

            for (var i = 4; i < iBytes.Length; i++)
            {
                if (i % 1000000 == 0)
                {
                    var pixel = Color.FromArgb(iBytes[i], iBytes[i - 1], iBytes[i - 2], iBytes[i - 3]);
                    iMatrix += pixel.G;
                    iLumen += 0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B;
                }
            }

            image.Lumen = iLumen;
            image.Colors = iMatrix;

            //GC.Collect();
        }
    }
}
