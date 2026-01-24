using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace Kafaa.API.Extention;
public static class DevExpressExtentions
{
    public static byte[] GenerateReportPdf(this XtraReport report)
    {
        using (var stream = new MemoryStream())
        {
            report.ExportToPdf(stream);
            return stream.ToArray();
        }
    }
    public static TimeSpan Fill(this XRTable table, List<List<string>> values)
    {
        var timeSpan = Stopwatch.StartNew();

        var cellsCount = table.Rows.FirstRow.Cells.Count;
        var cellWidth = new float[cellsCount];

        if (values.Count > 0)
            if (values[0].Count != cellsCount)
                throw new Exception("عدد البيانات المرسلة غير مطابق لعدد الاعمدة بالجدول");

        for (int i = 0; i < cellsCount; i++)
            cellWidth[i] = table.Rows.FirstRow.Cells[i].WidthF;

        table.BeginInit();
        for (int j = 0; j < values.Count; j++)
        {
            XRTableRow xrRow = new();
            for (int i = 0; i < values[j].Count; i++)
            {
                XRTableCell xRCell = new()
                {
                    BorderWidth = 1,
                    Text = values[j][i],
                    WidthF = cellWidth[i],
                    BorderColor = Color.Black,
                    BorderDashStyle = BorderDashStyle.Double,
                    Borders = BorderSide.All,
                    TextAlignment = TextAlignment.MiddleCenter,
                };
                xrRow.Cells.Add(xRCell);
            }
            table.Rows.Add(xrRow);
        }
        table.EndInit();

        timeSpan.Stop();
        return TimeSpan.FromMilliseconds(timeSpan.ElapsedMilliseconds);
    }
    public static Image SetImageTransparency(this XtraReport report, Image image, float opacity)
    {
        Bitmap bmp = new Bitmap(image.Width, image.Height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            ColorMatrix matrix = new ColorMatrix
            {
                Matrix33 = opacity // الشفافية من 0.0 (شفاف تمامًا) إلى 1.0 (واضح)
            };

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                        0, 0, image.Width, image.Height,
                        GraphicsUnit.Pixel, attributes);
        }

        return bmp;
    }

}
