using ImageProcessor.Library;
using ImageProcessor.UI.Commands;
using ImageProcessor.UI.Constants;
using ImageProcessor.UI.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ImageProcessor.UI.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private ImageProcessorModel model;

        private Stopwatch stopwatch = new Stopwatch();

        public string ImageSource
        {
            get
            {
                return model.ImageSource;
            }
            set
            {
                model.ImageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public string Message
        {
            get
            {
                return model.Message;
            }
            set
            {
                model.Message = value;
                OnPropertyChanged(nameof(Message));
            }
        }


        public MainPageViewModel()
        {
            model = new ImageProcessorModel();
            Message = StaticMessages.SelectImage;

            OpenFileCommand = new Command(OpenFileCallback);
            RunSyncCommand = new Command(RunSync);
            RunAsyncCommand = new Command(async () => await RunAsync());
        }

        public ICommand OpenFileCommand { get; set; }
        public ICommand RunSyncCommand { get; set; }
        public ICommand RunAsyncCommand { get; set; }


        public void OpenFileCallback()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();

            if(!string.IsNullOrEmpty(fileDialog.FileName))
            {
                UpdateUI(StaticMessages.LoadingFile);
                var fileName = Path.GetFullPath(fileDialog.FileName);
                ImageSource = fileName;

                model.LoadImage();
                UpdateUI(StaticMessages.FileLoaded);

            }
            else
                MessageBox.Show("Invalid File Path", "Error", MessageBoxButtons.OK);
        }

        public void RunSync()
        {
            stopwatch.Reset();
            stopwatch.Start();
            model.ConvertSync();
            stopwatch.Stop();

            try
            {
                Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong.", "Error", MessageBoxButtons.OK);
                return;
            }
        }

        public async Task RunAsync()
        {
            UpdateUI(StaticMessages.Converting);
            stopwatch.Reset();
            stopwatch.Start();
            await model.ConvertAsync();
            stopwatch.Stop();

            try
            {
                Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong.", "Error", MessageBoxButtons.OK);
                return;
            }
        }

        private void CheckFileExtensionCallback(object sender, CancelEventArgs e)
        {
            SaveFileDialog sv = (sender as SaveFileDialog);
            var extension = Path.GetExtension(sv.FileName).ToLower();
            var previousExtension = Path.GetExtension(ImageSource);
            if (extension != ".png" && extension != ".jpg" && extension != ".bmp")
            {
               

                e.Cancel = true;
                MessageBox.Show("Only PNG, JPG and BMP extensions are supported");
                return;
            }
            else if (extension != previousExtension)
            {
                e.Cancel = true;
                MessageBox.Show($"Use the same extension as the original image has [{previousExtension}]");
                return;
            }
        }

        private void DisplayConvertedImage(string newImagePath)
        {
            ImageSource = newImagePath;
        }

        private void UpdateUI(string message)
        {
            Message = message;
        }
        private void Save()
        {
            var ext = Path.GetExtension(ImageSource);
            var defaultName = Path.GetFileName(ImageSource).Replace(ext, string.Empty) + "_converted";

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileOk += CheckFileExtensionCallback;
            fileDialog.Filter = $"Image files (*{ext})|*{ext}";
            fileDialog.DefaultExt = ext;
            fileDialog.AddExtension = true;
            fileDialog.FileName = defaultName;

            var result = fileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(fileDialog.FileName))
                {
                    ImageFormat imageFormat = ImageFormat.Png;

                    switch (ext)
                    {
                        case AllowedExtensions.JPEG: imageFormat = ImageFormat.Jpeg; break;
                        case AllowedExtensions.JPG: imageFormat = ImageFormat.Jpeg; break;
                        case AllowedExtensions.BMP: imageFormat = ImageFormat.Bmp; break;
                        case AllowedExtensions.PNG: imageFormat = ImageFormat.Png; break;
                    }

                    model.SaveConvertedImage(fileDialog.FileName, imageFormat);

                    DisplayConvertedImage(fileDialog.FileName);
                    UpdateUI(StaticMessages.TimeElapsed(stopwatch.ElapsedMilliseconds));
                }
            }

        }
    }
}
