using System.Windows;
using FindTheBug.Desktop.Reception.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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
            
            // Use dependency injection to get ViewModel
            if (App.ServiceProvider is not null)
            {
                var viewModel = App.ServiceProvider.GetService<MainWindowViewModel>();
                DataContext = viewModel;
            }
        }
    }
}