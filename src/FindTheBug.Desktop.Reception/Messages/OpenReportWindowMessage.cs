using Microsoft.Reporting.WinForms;

namespace FindTheBug.Desktop.Reception.Messages;

public record OpenReportWindowMessage(string reportPath,
            List<ReportDataSource> dataSources,
            List<ReportParameter> parameters = null,
            string? windowTitle = null);
