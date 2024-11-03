using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Paragraph = iTextSharp.text.Paragraph;
using Rectangle = iTextSharp.text.Rectangle;

namespace Ticketing.EndPoints.Reporting.Query.Export;

public interface IExport
{
    FileContentResult ToExcel(string title, List<string> header, List<ExcelDataType> dataType, List<CellInfo[]> data,
        List<string> SumColumnData, List<string> headerInfo);

    FileContentResult ToCSV(string title, List<string> header, List<object[]> data);

    FileContentResult ToPdf(string title, List<string> headerpdf, List<string> tableheader, List<CellInfo[]> data,
        List<PreTableHeader> PreTableHeader = null, bool rotate = false, List<string> SumColumnFooter = null);

    FileContentResult To(string exportType, string title, List<string> header, List<ExcelDataType> dataType,
        List<object[]> data, List<string>? tableheader, List<CellInfo[]>? tabledata);
}
public class ExportService : IExport
{
    public FileContentResult ToExcel(string title, List<string> header, List<ExcelDataType> dataType,
        List<CellInfo[]> data, List<string> SumColumnData, List<string> headerInfo)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(title);
        worksheet.PageSetup.Header.Left.AddText(title);
        var currentRow = 1;
        worksheet.Row(currentRow).Height = 21;
        if (headerInfo != null)
        {
            for (var i = 0; i < headerInfo.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = headerInfo[i];
                worksheet.Cell(currentRow, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, i + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                worksheet.Cell(currentRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(currentRow, i + 1).Style.Border.OutsideBorderColor = XLColor.BrightGreen;
            }

            currentRow++;
        }

        for (var i = 0; i < header.Count; i++)
        {
            worksheet.Cell(currentRow, i + 1).Value = header[i];
            worksheet.Cell(currentRow, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(currentRow, i + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Cell(currentRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, i + 1).Style.Border.OutsideBorderColor = XLColor.DimGray;
        }

        foreach (var d in data)
        {
            currentRow++;
            for (var i = 0; i < d.Length; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = d[i].Text.ToString();
                worksheet.Cell(currentRow, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, i + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(currentRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(currentRow, i + 1).Style.Border.OutsideBorderColor = XLColor.DimGray;
            }
        }

        currentRow++;
        if (SumColumnData != null)
        {
            for (var i = 0; i < SumColumnData.Count; i++)
            {
                worksheet.Cell(currentRow, i + 1).Value = SumColumnData[i];
                worksheet.Cell(currentRow, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, i + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(currentRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Cell(currentRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(currentRow, i + 1).Style.Border.OutsideBorderColor = XLColor.DimGray;
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();
        return new FileContentResult(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        { FileDownloadName = title + "-" + DateTime.Now.ToString("yyyy-M-d dddd") + ".xlsx" };
    }

    public FileContentResult ToCSV(string title, List<string> header, List<object[]> data)
    {
        var builder = new StringBuilder();
        builder.AppendLine(string.Join(",", header.ToArray()));
        foreach (var d in data)
        {
            builder.AppendLine(string.Join(",", d.Select(x => x?.ToString() ?? "").ToArray()));
        }

        return new FileContentResult(Encoding.UTF8.GetBytes(builder.ToString()),
                "text/csv")
        { FileDownloadName = title + ".csv" };
    }

    public FileContentResult ToPdf(string title, List<string> pdfheader, List<string> tableheader,
        List<CellInfo[]> data, List<PreTableHeader> PreTableHeader = null, bool rotate = false,
        List<string> SumColumnFooter = null)
    {
        Document document = new Document();
        try
        {
            if (rotate)
            {
                document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            }

            var pdfStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, pdfStream);

            var currentPath = Directory.GetCurrentDirectory();
            string logoPath = currentPath + "\\Content\\tazirat.png"; // Provide the path to your logo image
            Image logo = Image.GetInstance(logoPath);
            logo.ScaleToFit(100f, 100f); // Adjust the size of the logo as needed

            logo.SetAbsolutePosition(document.PageSize.Width / 2 - 60,
                document.PageSize.Height - 90f);

            document.Open();

            document.Add(logo);
            var headerTable = new PdfPTable(2);

            BaseFont baseFont =
                BaseFont.CreateFont("Content\\BNAZANIN.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont, 7);

            headerTable.DefaultCell.Border = Rectangle.NO_BORDER;
            headerTable.HorizontalAlignment = Element.ALIGN_CENTER;
            headerTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            headerTable.WidthPercentage = 100;
            foreach (var headerpdfItem in pdfheader)
            {
                headerTable.AddCell(CreateCell(headerpdfItem, 7, haveBorder: false));
            }

            var TitleTable = new PdfPTable(1);
            TitleTable.HorizontalAlignment = Element.ALIGN_CENTER;
            TitleTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            TitleTable.AddCell(CreateCell(title, 12, isbold: true, height: 20f, haveBorder: false));

            document.Add(headerTable);
            document.Add(new Paragraph("  ", font));
            document.Add(TitleTable);

            var table = new PdfPTable(tableheader.Count);
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

            document.Add(new Paragraph("  ", font));

            if (PreTableHeader != null)
            {
                foreach (var Item in PreTableHeader)
                {
                    table.AddCell(CreateCell(Item.Text, 8, isbold: true, height: 20f, colspan: Item.ColSpan));
                }
            }

            int[] cellWidths = new int[tableheader.Count - 1];
            int headerwitdthCounter = 0;
            float[] columnWidths = new float[tableheader.Count];

            foreach (var headerbodyItem in tableheader)
            {
                table.AddCell(CreateCell(headerbodyItem, 8, isbold: true, height: 20f));
                headerwitdthCounter++;

                columnWidths[tableheader.IndexOf(headerbodyItem)] = 45;
            }

            int counter = 1;

            table.WidthPercentage = 100;

            foreach (var item in data)
            {
                if (item.Length != tableheader.Count)
                    throw new Exception("تعداد ستون ها با تعداد ارسالی برای جدول برابر نمیباشد");

                for (int i = 0; i < item.Length; i++)
                {
                    if (item[i].LinkUrl != "")
                    {
                        var anchor = new Chunk(item[i].Text, font) { };
                        anchor.SetAnchor(item[i].LinkUrl);
                        Phrase p = new Phrase();
                        p.Add(anchor);
                        if (item[i].width > 1)
                        {
                            columnWidths[i] = 80;
                        }
                        var createdCell = new PdfPCell(p);
                        createdCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(createdCell);
                    }
                    else
                    {
                        if (item[i].RotateText)
                        {
                            var createdCell = CreateCell(item[i].Text?.ToString(), 9, colwidth: item[i].width ?? 2f, rotateverticaltext: true);
                            columnWidths[i] = 20;
                            table.AddCell(createdCell);
                        }
                        else
                        {
                            var createdCell = CreateCell(item[i].Text?.ToString(), 9, colwidth: item[i].width ?? 2f);
                            // Measure cell width
                            if (item[i].DynamicWidth)
                            {
                                var cellWidth = baseFont.GetWidthPoint(item[i].Text ?? "", 9);
                                if (cellWidth > 45)
                                {
                                    columnWidths[i] = 80;
                                }
                                else if (cellWidth < 10)
                                {
                                    columnWidths[i] = 30;
                                }
                            }

                            if (item[i].width > 1)
                            {
                                columnWidths[i] = 80;
                            }

                            table.AddCell(createdCell);
                        }

                    }
                }

                counter++;
            }

            if (SumColumnFooter != null)
            {
                foreach (var ColumnfooterItem in SumColumnFooter)
                {
                    table.AddCell(CreateCell(ColumnfooterItem, 9, isbold: true, height: 40f));
                }
            }

            Array.Reverse(columnWidths);
            table.SetTotalWidth(columnWidths);
            document.Add(table);
            document.Close();
            var content = pdfStream.ToArray();

            return new FileContentResult(
                    content,
                    "application/pdf")
            { FileDownloadName = Guid.NewGuid() + ".pdf" };
        }
        catch (Exception e)
        {
            throw new Exception("Create pdf -- " + e.Message);
        }
    }

    public FileContentResult To(string exportType, string title, List<string> header, List<ExcelDataType> dataType,
        List<object[]> data, List<string> headerdpf = null, List<CellInfo[]> datapdf = null)
    {
        switch (exportType)
        {
            case "excel":
                return ToExcel(title, header, dataType, datapdf, header, header);
            case "pdf":
                return ToPdf(title, header, headerdpf, datapdf);
            case "csv":
                return ToCSV(title, header, data);
        }

        return null;
    }

    public PdfPCell CreateCell(string text, int fontSize, bool haveBorder = true, bool isbold = false, float height = 0,
        int colspan = 1, float colwidth = 1f, bool rotateverticaltext = false)
    {
        try
        {
            BaseFont baseFont =
                BaseFont.CreateFont("Content\\BNAZANIN.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont, fontSize);
            if (isbold)
            {
                font = new Font(baseFont, fontSize, Font.BOLD);
            }

            var createdCell = new PdfPCell(new Phrase(text, font));
            createdCell.HorizontalAlignment = Element.ALIGN_CENTER;
            createdCell.VerticalAlignment = Element.ALIGN_CENTER;
            createdCell.Colspan = colspan;
            if (rotateverticaltext)
            {
                createdCell.Rotation = 90;
            }
            if (height != 0)
            {
                createdCell.FixedHeight = height;
            }

            if (!haveBorder)
            {
                createdCell.Border = Rectangle.NO_BORDER;
            }

            return createdCell;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return null;
    }


    private string GetImagePath(string fileName)
        => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "resources", "image", fileName);
}

public enum ExcelDataType
{
    Text = 0,
    Number = 1,
    Boolean = 2,
    DateTime = 3,
    TimeSpan = 4
}

public class CellInfo
{
    public string Text { get; set; }
    public float? width { get; set; }
    public string LinkUrl { get; set; } = "";
    public bool DynamicWidth { get; set; } = true;
    public bool RotateText { get; set; } = false;
}

public class PreTableHeader
{
    public string Text { get; set; }
    public int ColSpan { get; set; }
}
