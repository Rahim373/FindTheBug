using System.Windows;
using FindTheBug.Desktop.Reception.ViewModels;

namespace FindTheBug.Desktop.Reception
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}