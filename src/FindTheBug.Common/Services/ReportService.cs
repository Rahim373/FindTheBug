using BarcodeStandard;
using FindTheBug.Common.ReportModel;
using FindTheBug.Domain.Entities;
using Microsoft.Reporting.WinForms;
using System;
using System.IO.Packaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FindTheBug.Common.Services;

public enum ReportType
{
    LabTestInvoice
}

public class ReportService
{
    public string GetBarcodeAsBase64Image(string code)
    {
        // 1. Create an instance of the Barcode API
        Barcode barcodeApi = new Barcode();

        // 2. Generate the barcode image as an ImageSharp Image object
        // The BarcodeStandard library's Encode method now returns a SixLabors.ImageSharp.Image
        var barcodeImage = barcodeApi.Encode(BarcodeStandard.Type.Code128, code);

        // 3. Convert the ImageSharp Image to a byte array
        byte[] imageBytes;
        using (MemoryStream ms = new MemoryStream())
        {
            // Save the image to the MemoryStream in PNG format
            barcodeImage.Encode().SaveTo(ms);
            imageBytes = ms.ToArray();
        }

        // 4. Convert the byte array to a Base64 string
        string base64Image = Convert.ToBase64String(imageBytes);

        // Optional: Prepend the Data URI scheme for direct HTML embedding (e.g., in an <img> tag src attribute)
        // string dataUrl = $"data:image/png;base64,{base64Image}";

        return base64Image;

    }

    public string GetReportPath(ReportType reportType)
    {
        var reportName = reportType switch
        {
            ReportType.LabTestInvoice => "LabReceipt.rdlc",
            _ => string.Empty
        };

        return Path.Combine(Environment.CurrentDirectory, "Reports", reportName);
    }

    public (List<ReportParameter> ReportParameters, List<ReportDataSource> DataSources) GetInvoiceData(
        Patient patient,
        List<TestEntry> labTests)
    {
        var parameters = new List<ReportParameter>
        {
            new ReportParameter("Name", "Abdur Rahim"),
            new ReportParameter("PhoneNumber", "01734014433"),
            new ReportParameter("Age", "32Y"),
            new ReportParameter("Gender", "Male"),
            new ReportParameter("Address", "asdasd"),
            new ReportParameter("ReferredBy", "Dr. Shoaib Ali Khan (MBBS)"),
            new ReportParameter("InvoiceNumber", "MARC-2512-0000001"),
            new ReportParameter("SubTotal", "10"),
            new ReportParameter("Total", "10"),
            new ReportParameter("Discount", "0"),
            new ReportParameter("Due", "0"),
            new ReportParameter("Balance", "0"),
            new ReportParameter("PrintedBy", "Abdur Rahim"),
            new ReportParameter("Copy", "Customer Copy"),
            new ReportParameter("Barcode", GetBarcodeAsBase64Image("MARC-2512-0000001"))
        };

        var data = new List<LabTestDto>{new LabTestDto
        {
            Name = "X-Ray",
            Amount = 1000m,
            Discount = 0
        },
        new LabTestDto{
            Name = "Ultrasonogram",
            Amount = 1000m,
            Discount = 0
        } };

        var dataSources = new List<ReportDataSource>
        {
            new ReportDataSource { Name = "LabReceiptDataset", Value = data }
        };
        return new(parameters, dataSources);
    }
}

