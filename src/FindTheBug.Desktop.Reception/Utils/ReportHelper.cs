using CommunityToolkit.Mvvm.Messaging;
using FindTheBug.Desktop.Reception.Messages;
using FindTheBug.Desktop.Reception.Windows;
using Microsoft.Reporting.WinForms;

namespace FindTheBug.Desktop.Reception.Utils;

internal static class ReportHelper
{
    internal static void OpenReport(string reportPath, List<ReportDataSource> dataSources, List<ReportParameter> reportParameters, string windowTitle)
    {
        var window = new ReportWindow();
        window.Closed += (object? sender, EventArgs e) =>
        {
            WeakReferenceMessenger.Default.Unregister<OpenReportWindowMessage>(window);
        };
        WeakReferenceMessenger.Default.Send(new OpenReportWindowMessage(reportPath, dataSources, reportParameters, windowTitle));
    }
}
