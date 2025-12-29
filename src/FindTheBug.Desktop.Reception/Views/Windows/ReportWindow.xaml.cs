using CommunityToolkit.Mvvm.Messaging;
using FindTheBug.Desktop.Reception.Messages;
using FindTheBug.Desktop.Reception.ViewModels;
using Microsoft.Reporting.WinForms;
using System.Windows;

namespace FindTheBug.Desktop.Reception.Windows
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();

            var viewModel = new ReportViewerViewModel();
            
            // Set the ReportViewer control to the ViewModel
            viewModel.ReportViewer = ReportViewer;

            this.DataContext = viewModel;

            WeakReferenceMessenger.Default.Register<OpenReportWindowMessage>(this, (r, m) =>
            {
                ShowReportWindow(m.reportPath, m.dataSources, m.parameters, m.windowTitle);
                viewModel.Title = m.windowTitle;
            });
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Opens a report in a new window with the specified data
        /// </summary>
        /// <param name="reportPath">Path to the RDLC file</param>
        /// <param name="dataSourceName">Name of the data source in the report</param>
        /// <param name="dataTable">Data table containing the report data</param>
        /// <param name="parameters">Optional report parameters</param>
        /// <param name="windowTitle">Optional window title (default: "Report Viewer")</param>
        public void ShowReportWindow(
            string reportPath, 
            List<ReportDataSource> dataSources, 
            List<ReportParameter>parameters = null, 
            string? windowTitle = null)
        {

            if (!string.IsNullOrEmpty(windowTitle))
            {
                Title = windowTitle;
            }

            // Load the report when the window loads
            Loaded += (s, e) =>
            {
                (this.DataContext as ReportViewerViewModel).LoadReport(reportPath, dataSources, parameters);
            };

            this.Show();
        }
    }
}