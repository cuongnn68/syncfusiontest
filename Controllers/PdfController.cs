using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.XlsIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIORenderer;

namespace syncfusiontest.Controllers;

[ApiController]
[Route("test")]
 public partial class PdfController : Controller
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    public PdfController(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet("/excel/1")]
    public ActionResult Excel() {
        Console.WriteLine("test excel 1111111111111");
        using ExcelEngine excelEngine = new ExcelEngine();
        
        IApplication application = excelEngine.Excel;
        FileStream fileStream = new FileStream("./file/Sample.xlsx", FileMode.Open, FileAccess.Read);
        IWorkbook workbook = application.Workbooks.Open(fileStream);
        IWorksheet worksheet = workbook.Worksheets[0];

        //Create Template Marker Processor
        ITemplateMarkersProcessor marker = workbook.CreateTemplateMarkersProcessor();

        //Insert Array Horizontally
        string[] names = new string[] { "Mickey", "Donald", "Tom", "Jerry" };
        string[] descriptions = new string[] { "Mouse", "Duck", "Cat", "Mouse" };
        string[] testtttt = new string[] { "testtttt 1", "testtttt 2", "testtttt 3", "testtttt 4", "testtttt 5" };

        //Add collections to the marker variables where the name should match with input template
        marker.AddVariable("Namess", names);
        marker.AddVariable("Descriptionss", descriptions);
        marker.AddVariable("testtttt", testtttt);

        //Process the markers in the template
        marker.ApplyMarkers();

        //Saving the workbook as stream
        workbook.Version = ExcelVersion.Excel2013;

        MemoryStream ms = new MemoryStream();
        workbook.SaveAs(ms);
        ms.Position = 0;

        return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SampleData.xlsx");;
    }

    [HttpGet("/word/1")]
    public ActionResult Word() {
        // throw new NotImplementedException();
        Console.WriteLine("test Word");

        FileStream fileStreamPath = new FileStream(@"./file/TestLabel.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using WordDocument document = new WordDocument(fileStreamPath, FormatType.Automatic);

        //Creates a clone of Input Template 
        // WordDocument document = d.Clone();

        MemoryStream stream = new MemoryStream();
        //Saves and closes the cloned document instance
        document.Save(stream, FormatType.Docx);


        string[] fieldNames = new string[] { "ConsigneeName", "ConsigneeAddress", "CourierName" };
        string[] fieldValues = new string[] { "Nancy Davolio", "Tét ddiajd chỉ", "JTTTTT" };
        //Performs the mail merge
        document.MailMerge.Execute(fieldNames, fieldValues);
        //Saves and closes the WordDocument instance

        //Save the PDF to the MemoryStream
        MemoryStream ms = new MemoryStream();

        document.Save(ms, FormatType.Docx);

        //If the position is not set to '0' then the PDF will be empty.
        ms.Position = 0;

        document.Close();

        //Download the PDF document in the browser.
        FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/msword");
        fileStreamResult.FileDownloadName = "LABELTESTTEST.docx";
        return fileStreamResult;
    }

    [HttpGet("/word2pdf/1")]
    public ActionResult Word2Pdf() {
        Console.WriteLine("test Word2Pdf");

        FileStream fileStreamPath = new FileStream(@"./file/TestLabel.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using WordDocument document = new WordDocument(fileStreamPath, FormatType.Automatic);

        //Creates a clone of Input Template 
        // WordDocument document = d.Clone();

        MemoryStream stream = new MemoryStream();
        //Saves and closes the cloned document instance
        document.Save(stream, FormatType.Docx);


        string[] fieldNames = new string[] { "OrderCode", "ConsigneeName", "ConsigneeAddress", "CourierName", "ConsigneeCity", "ConsigneeCountry", "ConsigneePhone" };
        string[] fieldValues = new string[] { "12345678", "Nancy Davolio", "Tét ddiajd chỉ", "EMS", "Hanoi", "Vietnam", "097654321" };
        //Performs the mail merge
        document.MailMerge.Execute(fieldNames, fieldValues);
        //Saves and closes the WordDocument instance

        DocIORenderer render = new DocIORenderer();
        //Sets Chart rendering Options.
        render.Settings.ChartRenderingOptions.ImageFormat = Syncfusion.OfficeChart.ExportImageFormat.Jpeg;
        
        //Converts Word document into PDF document
        PdfDocument pdfDocument = render.ConvertToPDF(document);

        //Save the PDF to the MemoryStream
        MemoryStream ms = new MemoryStream();

        pdfDocument.Save(ms);

        //If the position is not set to '0' then the PDF will be empty.
        ms.Position = 0;

        pdfDocument.Close(true);

        //Download the PDF document in the browser.
        FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/pdf");
        fileStreamResult.FileDownloadName = "LABELTESTTEST.pdf";
        return fileStreamResult;
    }

    [HttpGet("/pdf/1")]
    public ActionResult Barcode()
    {
        Console.WriteLine("test 222222222222222222222");


        string basePath = _hostingEnvironment.WebRootPath;
        string dataPath = string.Empty;
        dataPath = basePath + @"/PDF/";
        //Create a new instance of PdfDocument class.
        PdfDocument document = new PdfDocument();

        //Add a new page to the document.
        // PdfPage page = document.Pages.Add();

        PdfSection section1 = document.Sections.Add();
        section1.PageSettings.Size = PdfPageSize.A6;
        PdfPage page = section1.Pages.Add();

        //Create Pdf graphics for the page
        PdfGraphics g = page.Graphics;

        //Create a solid brush
        PdfBrush brush = PdfBrushes.Black;

        #region 2D Barcode
        //Set the font
        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 15f, PdfFontStyle.Bold); 
        PdfPen pen = new PdfPen(brush, 0.5f);
        float width = page.GetClientSize().Width;
        float xPos = page.GetClientSize().Width / 2; 
        PdfStringFormat format = new PdfStringFormat();
        format.Alignment = PdfTextAlignment.Center;
        // Draw String
        g.DrawString("2D Barcodes", font, brush, new PointF(xPos, 10), format);
        #region QR Barcode
        font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
        g.DrawString("QR Barcode", font, brush, new PointF(10, 65)); 
        PdfQRBarcode qrBarcode = new PdfQRBarcode(); 
        // Sets the Input mode to Binary mode
        qrBarcode.InputMode = InputMode.BinaryMode; 
        // Automatically select the Version
        qrBarcode.Version = QRCodeVersion.Auto; 
        // Set the Error correction level to high
        qrBarcode.ErrorCorrectionLevel = PdfErrorCorrectionLevel.High; 
        // Set dimension for each block
        qrBarcode.XDimension = 2;
        qrBarcode.Text = "Syncfusion Essential Studio Enterprise edition $995";
        // Draw the QR barcode
        qrBarcode.Draw(page, new PointF(25, 95));
        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);
        g.DrawString("Input Type :Eight Bit Binary", font, brush, new PointF(250, 130));
        g.DrawString("Encoded Data : Syncfusion Essential Studio Enterprise edition $995", font, brush, new PointF(250, 145));
        g.DrawLine(pen, new PointF(0, 205), new PointF(width, 205));
        #endregion
        #region QRCode with logo
        font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
        g.DrawString("QR Barcode with logo", font, brush, new PointF(10, 220));
        PdfQRBarcode qrbarcodelogo = new PdfQRBarcode();
        // Sets the Input mode to Binary mode
        qrbarcodelogo.InputMode = InputMode.BinaryMode; 
        // Automatically select the Version
        qrbarcodelogo.Version = QRCodeVersion.Auto; 
        // Set the Error correction level to high
        qrbarcodelogo.ErrorCorrectionLevel = PdfErrorCorrectionLevel.High; 
        // Set dimension for each block
        qrbarcodelogo.XDimension = 2;
        qrbarcodelogo.Text = "https://www.syncfusion.com"; 
        //Set the logo image to QR barcode.
        FileStream imageStream = new FileStream("./file/" + "dhl-logo.jpg", FileMode.Open, FileAccess.Read);            
        //Create QR Barcode logo.
        QRCodeLogo qRCodeLogo = new QRCodeLogo(imageStream);
        //Set the QR barcode logo.
        qrbarcodelogo.Logo = qRCodeLogo; 
        // Draw the QR barcode
        qrbarcodelogo.Draw(page, new PointF(25, 250));
        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular); 
        g.DrawString("Encoded Data : https://www.syncfusion.com", font, brush, new PointF(250, 270)); 
        g.DrawLine(pen, new PointF(0, 340), new PointF(width, 340));
        #endregion
        #region Datamatrix
        font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
        g.DrawString("DataMatrix Barcode", font, brush, new PointF(10, 355)); 
        PdfDataMatrixBarcode dataMatrixBarcode = new PdfDataMatrixBarcode("5575235 Win7 4GB 64bit 7Jun2010"); 
        // Set dimension for each block
        dataMatrixBarcode.XDimension = 4; 
        // Draw the barcode
        dataMatrixBarcode.Draw(page, new PointF(25, 385));
        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular); 
        g.DrawString("Symbol Type : Square", font, brush, new PointF(250, 405));
        g.DrawString("Encoded Data : 5575235 Win7 4GB 64bit 7Jun2010", font, brush, new PointF(250, 425)); 
        pen = new PdfPen(brush, 0.5f);
        g.DrawLine(pen, new PointF(0, 500), new PointF(width, 500));
        string text = "TYPE 3523 - ETWS/N FE- SDFHW 06/08"; 
        dataMatrixBarcode = new PdfDataMatrixBarcode(text); 
        // rectangular matrix
        dataMatrixBarcode.Size = PdfDataMatrixSize.Size16x48; 
        dataMatrixBarcode.XDimension = 4; 
        dataMatrixBarcode.Draw(page, new PointF(25, 520));
        g.DrawString("Symbol Type : Rectangle", font, brush, new PointF(250, 540));
        g.DrawString("Encoded Data : " + text, font, brush, new PointF(250, 560));
        pen = new PdfPen(brush, 0.5f); 
        g.DrawLine(pen, new PointF(0, 620), new PointF(width, 620));
        font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
        g.DrawString("PDF417 Barcode", font, brush, new PointF(10, 650));
        Pdf417Barcode pdf417Barcode = new Pdf417Barcode();
        pdf417Barcode.Text = "https://www.syncfusion.com/";
        pdf417Barcode.ErrorCorrectionLevel = Pdf417ErrorCorrectionLevel.Auto;
        pdf417Barcode.XDimension = 1f;
        pdf417Barcode.Size = new SizeF(200, 50);
        pdf417Barcode.Draw(page, new PointF(25, 680));
        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);
        g.DrawString("Encoded Data : https://www.syncfusion.com/", font, brush, new PointF(250, 700));
        #endregion
        #endregion
        
        #region 1D Barcode

        page = document.Pages.Add();
        g = page.Graphics;

        //Set the font
        font = new PdfStandardFont(PdfFontFamily.Helvetica, 15f, PdfFontStyle.Bold);

        // Draw String
        g.DrawString("1D/Linear Barcodes", font, brush, new PointF(150, 10));

        // Set string format.
        format = new PdfStringFormat();
        format.WordWrap = PdfWordWrapType.Word;

        #region Code39
        // Drawing Code39 barcode
        PdfCode39Barcode barcode = new PdfCode39Barcode();

        // Setting height of the barcode
        barcode.BarHeight = 45;
        barcode.Text = "CODE39$";

        //Printing barcode on to the Pdf.
        barcode.Draw(page, new PointF(25, 70));

        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);

        g.DrawString("Type : Code39", font, brush, new PointF(215, 80));
        g.DrawString("Allowed Characters : 0-9, A-Z,a dash(-),a dot(.),$,/,+,%, SPACE", font, brush, new PointF(215, 100));

        g.DrawLine(pen, new PointF(0, 150), new PointF(width, 150));
        #endregion

        #region Code39Extended
        // Drawing Code39Extended barcode
        PdfCode39ExtendedBarcode barcodeExt = new PdfCode39ExtendedBarcode();

        // Setting height of the barcode
        barcodeExt.BarHeight = 45;
        barcodeExt.Text = "CODE39Ext";

        //Printing barcode on to the Pdf.
        barcodeExt.Draw(page, new PointF(25, 200));

        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);

        g.DrawString("Type : Code39Ext", font, brush, new PointF(215, 210));
        g.DrawString("Allowed Characters : 0-9 A-Z a-z ", font, brush, new PointF(215, 230));

        g.DrawLine(pen, new PointF(0, 270), new PointF(width, 270));
        #endregion

        #region Code11Barcode
        // Drawing Code11  barcode
        PdfCode11Barcode barcode11 = new PdfCode11Barcode();

        // Setting height of the barcode
        barcode11.BarHeight = 45;
        barcode11.Text = "012345678";
        barcode11.EncodeStartStopSymbols = true;

        //Printing barcode on to the Pdf.
        barcode11.Draw(page, new PointF(25, 300));

        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);

        g.DrawString("Type : Code 11", font, brush, new PointF(215, 310));
        g.DrawString("Allowed Characters : 0-9, a dash(-) ", font, brush, new PointF(215, 330));

        g.DrawLine(pen, new PointF(0, 370), new PointF(width, 370));
        #endregion

        #region Codabar
        // Drawing CodaBarcode
        PdfCodabarBarcode codabar = new PdfCodabarBarcode();

        // Setting height of the barcode
        codabar.BarHeight = 45;
        codabar.Text = "0123";

        //Printing barcode on to the Pdf.
        codabar.Draw(page, new PointF(25, 400));

        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);

        g.DrawString("Type : Codabar", font, brush, new PointF(215, 410));
        g.DrawString("Allowed Characters : A,B,C,D,0-9,-$,:,/,a dot(.),+ ", font, brush, new PointF(215, 430));

        g.DrawLine(pen, new PointF(0, 470), new PointF(width, 470));
        #endregion

        #region Code32
        PdfCode32Barcode code32 = new PdfCode32Barcode();

        code32.Font = font;

        // Setting height of the barcode
        code32.BarHeight = 45;
        code32.Text = "01234567";
        code32.TextDisplayLocation = TextLocation.Bottom;
        code32.EnableCheckDigit = true;
        code32.ShowCheckDigit = true;

        //Printing barcode on to the Pdf.
        code32.Draw(page, new PointF(25, 500));

        g.DrawString("Type : Code32", font, brush, new PointF(215, 500));
        g.DrawString("Allowed Characters : 1 2 3 4 5 6 7 8 9 0 ", font, brush, new PointF(215, 520));

        g.DrawLine(pen, new PointF(0, 580), new PointF(width, 580));

        #endregion

        #region Code93
        PdfCode93Barcode code93 = new PdfCode93Barcode();

        // Setting height of the barcode
        code93.BarHeight = 45;
        code93.Text = "ABC 123456";

        //Printing barcode on to the Pdf.
        code93.Draw(page, new PointF(25, 600));

        g.DrawString("Type : Code93", font, brush, new PointF(215, 600));
        g.DrawString("Allowed Characters : 1 2 3 4 5 6 7 8 9 0 A B C D E F G H I J K L M N O P Q R S T U V W X Y Z - . $ / + % SPACE ", font, brush, new RectangleF(215, 620, 300, 200), format);

        g.DrawLine(pen, new PointF(0, 680), new PointF(width, 680));
        #endregion

        #region Code93Extended
        PdfCode93ExtendedBarcode code93ext = new PdfCode93ExtendedBarcode();

        //Setting height of the barcode
        code93ext.BarHeight = 45;
        code93ext.EncodeStartStopSymbols = true;
        code93ext.Text = "(abc) 123456";

        //Printing barcode on to the Pdf.
        page = document.Pages.Add();
        code93ext.Draw(page, new PointF(25, 50));

        g = page.Graphics;
        g.DrawString("Type : Code93 Extended", font, brush, new PointF(200, 50));
        g.DrawString("Allowed Characters : All 128 ASCII characters ", font, brush, new PointF(200, 70));

        g.DrawLine(pen, new PointF(0, 120), new PointF(width, 120));
        #endregion

        #region Code128
        PdfCode128ABarcode barcode128A = new PdfCode128ABarcode();

        // Setting height of the barcode
        barcode128A.BarHeight = 45;
        barcode128A.Text = "ABCD 12345";
        barcode128A.EnableCheckDigit = true;
        barcode128A.EncodeStartStopSymbols = true;
        barcode128A.ShowCheckDigit = true;

        //Printing barcode on to the Pdf.
        barcode128A.Draw(page, new PointF(25, 135));

        g.DrawString("Type : Code128 A", font, brush, new PointF(200, 135));
        g.DrawString("Allowed Characters : NUL (0x00) SOH (0x01) STX (0x02) ETX (0x03) EOT (0x04) ENQ (0x05) ACK (0x06) BEL (0x07) BS (0x08) HT (0x09) LF (0x0A) VT (0x0B) FF (0x0C) CR (0x0D) SO (0x0E) SI (0x0F) DLE (0x10) DC1 (0x11) DC2 (0x12) DC3 (0x13) DC4 (0x14) NAK (0x15) SYN (0x16) ETB (0x17) CAN (0x18) EM (0x19) SUB (0x1A) ESC (0x1B) FS (0x1C) GS (0x1D) RS (0x1E) US (0x1F) SPACE (0x20) \" ! # $ % & ' ( ) * + , - . / 0 1 2 3 4 5 6 7 8 9 : ; < = > ? @ A B C D E F G H I J K L M N O P Q R S T U V W X Y Z [ / ]^ _  ", font, brush, new RectangleF(200, 155, 300, 200), format);

        g.DrawLine(pen, new PointF(0, 250), new PointF(width, 250));

        PdfCode128BBarcode barcode128B = new PdfCode128BBarcode();

        // Setting height of the barcode
        barcode128B.BarHeight = 45;
        barcode128B.Text = "12345 abcd";
        barcode128B.EnableCheckDigit = true;
        barcode128B.EncodeStartStopSymbols = true;
        barcode128B.ShowCheckDigit = true;

        //Printing barcode on to the Pdf.
        barcode128B.Draw(page, new PointF(25, 280));

        g.DrawString("Type : Code128 B", font, brush, new PointF(200, 280));
        g.DrawString("Allowed Characters : SPACE (0x20) !  \" # $ % & ' ( ) * + , - . / 0 1 2 3 4 5 6 7 8 9 : ; < = > ? @ A B C D E F G H I J K L M N O P Q R S T U V W X Y Z [ / ]^ _ ` a b c d e f g h i j k l m n o p q r s t u v w x y z { | } ~ DEL (\x7F)  ", font, brush, new RectangleF(200, 300, 300, 200), format);

        g.DrawLine(pen, new PointF(0, 350), new PointF(width, 350));

        PdfCode128CBarcode barcode128C = new PdfCode128CBarcode();

        // Setting height of the barcode
        barcode128C.BarHeight = 45;
        barcode128C.Text = "001122334455";
        barcode128C.EnableCheckDigit = true;
        barcode128C.EncodeStartStopSymbols = true;
        barcode128C.ShowCheckDigit = true;

        //Printing barcode on to the Pdf.
        barcode128C.Draw(page, new PointF(25, 370));

        g.DrawString("Type : Code128 C", font, brush, new PointF(200, 370));
        g.DrawString("Allowed Characters : 0 1 2 3 4 5 6 7 8 9 ", font, brush, new PointF(200, 390));

        g.DrawLine(pen, new PointF(0, 440), new PointF(width, 440));

        #endregion            
        
        #region UPC-A
        // Drawing UPC-A barcode
        PdfCodeUpcBarcode upcBarcode = new PdfCodeUpcBarcode();

        // Setting height of the barcode
        upcBarcode.BarHeight = 45;
        upcBarcode.Text = "01234567890";

        //Printing barcode on to the Pdf.
        upcBarcode.Draw(page, new PointF(25, 460));

        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);

        g.DrawString("Type : UPC-A", font, brush, new PointF(200, 460));
        g.DrawString("Allowed Characters : 0-9", font, brush, new PointF(200, 480));

        g.DrawLine(pen, new PointF(0, 530), new PointF(width, 530));
        #endregion

        #region EAN13
        PdfEan13Barcode ean13barcode = new PdfEan13Barcode();
        ean13barcode.Text = "012345678910";
        ean13barcode.Draw(page, new RectangleF(15, 550,150,50));
        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);

        g.DrawString("Type : EAN-13", font, brush, new PointF(200, 550));
        g.DrawString("Allowed Characters : 0-9", font, brush, new PointF(200, 570));

        g.DrawLine(pen, new PointF(0, 620), new PointF(width, 620));
        #endregion

        #region EAN8
        PdfEan8Barcode ean8barcode = new PdfEan8Barcode();
        ean8barcode.Text = "0123456";
        ean8barcode.Draw(page, new PointF(25, 640));
        font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9, PdfFontStyle.Regular);

        g.DrawString("Type : EAN-8", font, brush, new PointF(200, 640));
        g.DrawString("Allowed Characters : 0-9", font, brush, new PointF(200, 660));

        g.DrawLine(pen, new PointF(0, 720), new PointF(width, 720));
        #endregion
        #endregion

        //Save the PDF to the MemoryStream
        MemoryStream ms = new MemoryStream();

        document.Save(ms);

        //If the position is not set to '0' then the PDF will be empty.
        ms.Position = 0;

        document.Close(true);

        //Download the PDF document in the browser.
        FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/pdf");
        fileStreamResult.FileDownloadName = "Barcode.pdf";
        return fileStreamResult;
    }
}