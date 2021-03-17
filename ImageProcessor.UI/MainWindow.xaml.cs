using ImageProcessor.UI.ViewModels;
using System.Windows;

namespace ImageProcessor.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainPageViewModel viewModel;
        public MainWindow()
        {
            viewModel = new MainPageViewModel();
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
