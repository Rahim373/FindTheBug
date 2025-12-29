using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Reporting.WinForms;

namespace FindTheBug.Desktop.Reception.ViewModels;

public partial class ReportViewerViewModel : ObservableObject
{
    [ObservableProperty]
    private ReportViewer? _reportViewer;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    public string? _reportPath;

    /// <summary>
    /// Load a report with the specified RDLC file and data
    /// </summary>
    /// <param name="reportPath">Path to the RDLC file (relative to Reports folder or full path)</param>
    /// <param name="dataSources">List of the data source in the report</param>
    /// <param name="parameters">Optional report parameters</param>
    public void LoadReport(string reportPath,
        List<ReportDataSource> dataSources,
        List<ReportParameter> reportParameters)
    {
        ReportPath = reportPath;

        if (ReportViewer == null)
            return;

        try
        {
            // Clear previous report
            ReportViewer.Reset();

            // Load the RDLC report
            ReportViewer.LocalReport.ReportPath = reportPath;
            ReportViewer.LocalReport.EnableExternalImages = true;

            // Add data source
            if (dataSources != null && dataSources.Any())
            {
                ReportViewer.LocalReport.DataSources.Clear();
                dataSources.ForEach(ReportViewer.LocalReport.DataSources.Add);
            }

            // Add parameters if provided
            if (reportParameters != null && reportParameters.Count > 0)
            {
                ReportViewer.LocalReport.SetParameters(reportParameters);
            }

            // Refresh the report
            ReportViewer.RefreshReport();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error loading report: {ex.Message}", "Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Print the current report
    /// </summary>
    [RelayCommand]
    private void Print()
    {
        if (ReportViewer == null)
            return;

        try
        {
            ReportViewer.PrintDialog();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error printing report: {ex.Message}", "Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}