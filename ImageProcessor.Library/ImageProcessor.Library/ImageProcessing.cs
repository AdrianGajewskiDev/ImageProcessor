using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ImageProcessor.Library
{
    public class ImageProcessing
    {
        private Bitmap _originalImage;

        public void LoadImage(string path)
        {
            _originalImage = (Bitmap)Image.FromFile(path);
        }

        public async Task<Bitmap> ToMainColorsAsync()
        {
            var result = await Task.Run(() => ToMainColorsSync());

            return result;
        }

        public Color SetNewColorForPixel(Color pixel)
        {
            var result = pixel.R;

            if (pixel.G > result)
            {
                result = pixel.G;
                if (pixel.B > result)
                    return ColorTranslator.FromHtml(ConvertedPixelValues.B);

                return ColorTranslator.FromHtml(ConvertedPixelValues.G);
            }
            else if (pixel.B > result)
                return ColorTranslator.FromHtml(ConvertedPixelValues.B);
            else
                return ColorTranslator.FromHtml(ConvertedPixelValues.R);

        }

        public void SaveImage(Bitmap image, string path, ImageFormat format)
        {
            image.Save(path, format);
        }

        public bool ImageLoaded() => _originalImage != null;
        public void Reset()
        {
            _originalImage = null;
        }
        public Bitmap ToMainColorsSync()
        {
            if (_originalImage == null)
                throw new ArgumentNullException("Original Image was null");

            Bitmap newImage = new Bitmap(_originalImage, _originalImage.Width, _originalImage.Height);

            BitmapData data = newImage.LockBits(new Rectangle(0, 0, newImage.Width, newImage.Height), ImageLockMode.ReadWrite, newImage.PixelFormat);

            //Calculates total bytes amount in bitmap
            //stride is total amount of pixels in bytes in single row
            var totalBytes = data.Stride * data.Height;

            byte[] allPixels = new byte[totalBytes];

            //Copies all pixels from bitmap to allPixels array
            Marshal.Copy(data.Scan0, allPixels, 0, allPixels.Length);

            //gets a single pixel size
            var pixelSize = Bitmap.GetPixelFormatSize(newImage.PixelFormat) / 8;

            ///calculates a total width of pixels in single row
            int totalWidth = data.Width * pixelSize;

            for (int y = 0; y < data.Height; y++)
            {
                //calculates the current line. 
                var currentLine = y * data.Stride;

                //loops throught all pixels in one row
                //x is calculated based on pixelSize,
                //each iteration adds a pixelSize value so the next iteration starts from the next pixel in the same row
                //for example: pixelSize is 4 then iteration will loop throught every 4 bytes
                //each pixel takes 4 bytes so 
                //    *first value corresponds to B 
                //    *second value corresponds to G
                //    *third value corresponds to R
                //    *fourth value corresponds to Alpha value of the color
                // it is because the Scan0 from BitmapData returns array in format: "BGRA", "BGRA" etc. How do i know this? I've searched this on the internet :)
                // we can use this because we copied the BitmapData.Scan0 to our allPixels array

                for (int x = 0; x < totalWidth; x = x + pixelSize)
                { 
                    int r = allPixels[currentLine + x + 2];
                    int g = allPixels[currentLine + x + 1];
                    int b = allPixels[currentLine + x];

                    var currentColor = Color.FromArgb(r, g, b);
                    var newColor = SetNewColorForPixel(currentColor);

                    allPixels[currentLine + x + 2] = newColor.R;
                    allPixels[currentLine + x + 1] = newColor.G;
                    allPixels[currentLine + x] = newColor.B;
                }
            }

            //copy our converted pixels back to
            Marshal.Copy(allPixels, 0, data.Scan0, allPixels.Length);
            newImage.UnlockBits(data);

            return newImage;
        }
    }
}

