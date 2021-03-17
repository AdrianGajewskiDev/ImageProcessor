using System;
using System.Drawing;
using System.Drawing.Imaging;
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

        public Bitmap ToMainColorsSync()
        {
            if (_originalImage == null)
                throw new ArgumentNullException("Original Image was null");

            Bitmap newImage = new Bitmap(_originalImage, _originalImage.Width, _originalImage.Height);
            
            for(int x = 0; x < _originalImage.Width; x++)
            {
                for(int y = 0; y < _originalImage.Height; y++)
                {

                    newImage.SetPixel(x, y, SetNewColorForPixel(_originalImage.GetPixel(x, y)));
                }
            }

            return newImage;
        }

        public async Task<Bitmap> ToMainColorsAsync()
        {
            var result = await Task.Run(() => ToMainColorsSync());

            return result;
        }

        public Color SetNewColorForPixel(Color pixel)
        {
            var result = pixel.R;

            if(pixel.G > result)
            {
                result = pixel.G;
                if(pixel.B > result)
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
    }
}