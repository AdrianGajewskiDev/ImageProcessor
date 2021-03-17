﻿using ImageProcessor.Library;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ImageProcessor.UI.Models
{
    public class ImageProcessorModel
    {
        public string ImageSource { get; set; }
        public string Message { get; set; }
        public double ElapsedTime { get; set; }

        private ImageProcessing _imageProcessing;

        private Bitmap _convertedImage;

        public ImageProcessorModel()
        {
            _imageProcessing = new ImageProcessing();
        }

        public void LoadImage()
        {
            if (File.Exists(ImageSource))
            {
                _imageProcessing.LoadImage(ImageSource);
            }
            else
                MessageBox.Show("Invalid file path", "Error", MessageBoxButton.OK);
        }

        public void ConvertSync()
        {
            _convertedImage = _imageProcessing.ToMainColorsSync();
        }

        public async Task ConvertAsync()
        {
            _convertedImage =  await _imageProcessing.ToMainColorsAsync();
        }

        public void SaveConvertedImage(string path, ImageFormat format)
        {
            _imageProcessing.SaveImage(_convertedImage, path, format);

        }
    }
}
