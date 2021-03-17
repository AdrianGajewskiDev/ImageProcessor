using ImageProcessor.Library;
using System.Drawing;
using Xunit;

namespace ImageProcessor.Tests
{
    public class ImageProcessingTests
    {
        [Theory]
        [InlineData(10, 20, 30, ConvertedPixelValues.B)]
        [InlineData(30, 20, 10, ConvertedPixelValues.R)]
        [InlineData(10, 30, 20, ConvertedPixelValues.G)]
        public void Should_Return_Correct_Color_For_Pixel(int r, int g, int b, string newColorExpected)
        {
            var imgProcessing = new ImageProcessing();

            Color color = Color.FromArgb(r, g, b);

            var convertedColor = imgProcessing.SetNewColorForPixel(color);
            var result = ColorTranslator.ToHtml(convertedColor);

            Assert.Equal(result, newColorExpected);
        }
    }
}