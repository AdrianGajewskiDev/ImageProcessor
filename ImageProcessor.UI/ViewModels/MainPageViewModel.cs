using ImageProcessor.UI.Commands;
using ImageProcessor.UI.Constants;
using ImageProcessor.UI.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
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
        public bool ShowLoadingSpinner
        {
            get
            {
                return model.ShowLoadingSpinner;
            }
            set
            {
                model.ShowLoadingSpinner = value;
                OnPropertyChanged(nameof(ShowLoadingSpinner));
            }
        }
        public bool Converting
        {
            get
            {
                return model.Converting;
            }
            set
            {
                model.Converting = value;
            }
        }

        private string defaultImagePath = "../Images/addImageLogo.png";

        public MainPageViewModel()
        {
            model = new ImageProcessorModel();

            Message = StaticMessages.SelectImage;
            ImageSource = defaultImagePath;

            OpenFileCommand = new Command(OpenFileCallback);
            RunSyncCommand = new Command(RunSync);
            RunAsyncCommand = new Command(async () => await RunAsync());
            RemoveImageCommand = new Command(RemoveImageCallback);
        }

        public ICommand OpenFileCommand { get; set; }
        public ICommand RunSyncCommand { get; set; }
        public ICommand RunAsyncCommand { get; set; }
        public ICommand RemoveImageCommand { get; set; }

        public void RemoveImageCallback()
        {
            if (Converting)
                return;

            ImageSource = defaultImagePath;
            model.Reset();
            UpdateUI(StaticMessages.SelectImage);
        }
        public void OpenFileCallback()
        {
            if (Converting)
                return;

            UpdateUI(StaticMessages.LoadingFile);

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image files (*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png";
            fileDialog.DefaultExt = "png";
            fileDialog.AddExtension = true;
            fileDialog.ShowDialog();


            if (!string.IsNullOrEmpty(fileDialog.FileName))
            {
                var fileName = Path.GetFullPath(fileDialog.FileName);

                ImageSource = fileName;
                model.LoadImage();

                UpdateUI(StaticMessages.FileLoaded);
            }
            else
            {
                UpdateUI(StaticMessages.Canceled);
                MessageBox.Show("Invalid File Path", "Error", MessageBoxButtons.OK);
            }
        }
        public void RunSync()
        {
            if (Converting)
                return;

            if (!model.ImageSelected())
            {
                MessageBox.Show("Please select an image.", "Error", MessageBoxButtons.OK);
                return;
            }

            try
            {
                Converting = true;
                stopwatch.Reset();
                stopwatch.Start();
                model.ConvertSync();
                stopwatch.Stop();

                Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong. Please try again", "Error", MessageBoxButtons.OK);
                return;
            }
        }
        public async Task RunAsync()
        {
            if (Converting)
                return;

            if (!model.ImageSelected())
            {
                MessageBox.Show("Please select an image.", "Error", MessageBoxButtons.OK);
                return;
            }

            try
            {
                EnableLoadingScreen();
                Converting = true;

                UpdateUI(StaticMessages.Converting);
                stopwatch.Reset();
                stopwatch.Start();
                await model.ConvertAsync();
                stopwatch.Stop();
                Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong. Please try again", "Error", MessageBoxButtons.OK);
                return;
            }
        }
        private void CheckFileExtensionCallback(object sender, CancelEventArgs e)
        {
            SaveFileDialog sv = (sender as SaveFileDialog);
            var extension = Path.GetExtension(sv.FileName).ToLower();
            var previousExtension = Path.GetExtension(ImageSource).ToLower();
            if (extension != ".png" && extension != ".jpg" && extension != ".bmp" && extension != ".jpeg")
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
        public void UpdateUI(string message)
        {
            Message = message;
        }
        private void Save()
        {
            DisableLoadingScreen();
            Converting = false;

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
            else
                UpdateUI(StaticMessages.Canceled);

        }

        private void EnableLoadingScreen() => ShowLoadingSpinner = true;
        private void DisableLoadingScreen() => ShowLoadingSpinner = false;
    }
}
