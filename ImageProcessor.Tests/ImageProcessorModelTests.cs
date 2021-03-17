using ImageProcessor.UI.Models;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ImageProcessor.Tests
{
    public class ImageProcessorModelTests
    {
        [Fact]
        public void Should_Convert_Image_And_Set_Result_To_Class_Property_SYNC()
        {
            var model = new ImageProcessorModel();
            model.ImageSource = "./TestImages/addImageLogo.png";
            model.LoadImage();

            model.ConvertSync();

            Assert.NotNull(model.GetConvertedImage());
        }

        [Fact]
        public async Task Should_Convert_Image_And_Set_Result_To_Class_Property_ASYNC()
        {
            var model = new ImageProcessorModel();
            model.ImageSource = "./TestImages/addImageLogo.png";
            model.LoadImage();

            await model.ConvertAsync();

            Assert.NotNull(model.GetConvertedImage());
        }

        [Fact]
        public void Should_Save_Image_To_Specified_File_Path()
        {
            var model = new ImageProcessorModel();
            model.ImageSource = "./TestImages/addImageLogo.png";
            var savePath = "./TestImages/addImageLogoTest.png";

            model.LoadImage();
            model.ConvertSync();
            model.SaveConvertedImage(savePath, ImageFormat.Png);

            Assert.True(File.Exists(savePath));
        }

        [Fact]
        public void Should_Return_Whether_The_Image_Was_Selected_Or_Not()
        {
            var model = new ImageProcessorModel();
            model.ImageSource = "./TestImages/addImageLogo.png";
            
            model.LoadImage();

            var result = model.ImageSelected();

            Assert.True(result);
        }

    }
}
