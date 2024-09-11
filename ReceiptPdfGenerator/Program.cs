using ceTe.DynamicPDF;
using ceTe.DynamicPDF.Merger;
using ceTe.DynamicPDF.PageElements;

namespace ReceiptPdfGenerator;

class Program
{
    static void Main(string[] args)
    {
        string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName!;

        // Step 1: Create a receipt PDF with a placeholder
        string pdfFilePath = System.IO.Path.Combine(projectDirectory, "receipt_placeholder.pdf");
        CreateReceiptPdf(pdfFilePath);

        // Step 2: Convert the PDF to a Base64 string
        string base64Content = ConvertPdfToBase64(pdfFilePath);

        // Step 3: Update the PDF with a QR Code
        string qrCodeFilePath = System.IO.Path.Combine(projectDirectory, "CodeQR.Transaction640f87f9-8.PNG");
        string outputPdfFilePath = System.IO.Path.Combine(projectDirectory, "output.pdf");

        UpdatePdfWithQrCode(base64Content, qrCodeFilePath, outputPdfFilePath);
    }

    static void CreateReceiptPdf(string filePath)
    {
        Document document = new();
        Page page = new(PageSize.Postcard, PageOrientation.Portrait, 54.0f);
        document.Pages.Add(page);
        float pageWidth = page.Dimensions.Width;

        // Add text elements to the page
        page.Elements.Add(new Label("Restaurant Receipt", 10, 0, 280, 30, Font.Helvetica, 18, TextAlign.Center));
        page.Elements.Add(new Label("Date: " + DateTime.Now.ToString("MM/dd/yyyy"), 0, 40, pageWidth, 12, Font.Helvetica, 12, TextAlign.Left));
        page.Elements.Add(new Label("Item", 0, 70, 200, 12, Font.Helvetica, 12, TextAlign.Left));
        page.Elements.Add(new Label("Price", 0, 70, 200, 12, Font.Helvetica, 12, TextAlign.Right));

        // Add sample items
        page.Elements.Add(new Label("Burger", 0, 90, 200, 12, Font.Helvetica, 12, TextAlign.Left));
        page.Elements.Add(new Label("$5.99", 0, 90, 200, 12, Font.Helvetica, 12, TextAlign.Right));
        page.Elements.Add(new Label("Fries", 0, 110, 200, 12, Font.Helvetica, 12, TextAlign.Left));
        page.Elements.Add(new Label("$2.99", 0, 110, 200, 12, Font.Helvetica, 12, TextAlign.Right));
        page.Elements.Add(new Label("Soda", 0, 130, 200, 12, Font.Helvetica, 12, TextAlign.Left));
        page.Elements.Add(new Label("$1.50", 0, 130, 200, 12, Font.Helvetica, 12, TextAlign.Right));

        page.Elements.Add(new Label("============================ ", 0, 150, pageWidth, 50, Font.Helvetica, 12, TextAlign.Left));

        // Add total
        page.Elements.Add(new Label("Total", 0, 170, 200, 12, Font.Helvetica, 12, TextAlign.Left));
        page.Elements.Add(new Label("$10.48", 0, 170, 200, 12, Font.TimesBold, 12, TextAlign.Right));

        // Add empty space for QR code
        page.Elements.Add(new Label(" ", 0, 280, 0, 50, Font.Helvetica, 12, TextAlign.Center));

        page.Elements.Add(new Label("============================ ", 0, 300, pageWidth, 50, Font.Helvetica, 12, TextAlign.Left));


        // Save the document to a file
        using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
        document.Draw(fileStream);
    }

    static string ConvertPdfToBase64(string filePath)
    {
        byte[] pdfBytes = File.ReadAllBytes(filePath);
        return Convert.ToBase64String(pdfBytes);
    }

    static void UpdatePdfWithQrCode(string base64Content, string qrCodeFilePath, string outputPdfFilePath)
    {
        // Load the existing PDF from the Base64 string
        byte[] pdfBytes = Convert.FromBase64String(base64Content);
        MergeDocument pdfDocument = new(pdfBytes);


        // Add the QR code image to the PDF
        Image qrCodeImage = new(qrCodeFilePath, 150, 290, 0.4f) { Align = Align.Center, VAlign = VAlign.Center };

        // Add the QR code to the first page of the PDF
        Page firstPage = pdfDocument.Pages[0];
        firstPage.Elements.Add(qrCodeImage);

        // Save the modified PDF back to disk
        using (FileStream outputStream = new(outputPdfFilePath, FileMode.Create, FileAccess.Write))
        {
            pdfDocument.Draw(outputStream);
        }

        Console.WriteLine($"PDF with QR code has been saved to {outputPdfFilePath}");
    }
}