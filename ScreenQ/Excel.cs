using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace ScreenQ
{
    class Excel
    {

        public static string fileName = "ScreenQ - Report.xlsx";
        public static string outputDir = Properties.Settings.Default["ScreenShotSavePath"].ToString() + "\\";

        public static void CreateExcel()
        {
            // Set the file name and get the output directory

            // Create the file using the FileInfo object
            var file = new FileInfo(outputDir + fileName);

            // Create the package and make sure you wrap it in a using statement
            using (var package = new ExcelPackage(file))
            {
                // --------- Data and styling goes here -------------- //

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ScreenQ - Report");

                // Start adding the header
                // First of all the first row
                worksheet.Cells[1, 1].Value = "Screenshot";
                worksheet.Cells[1, 2].Value = "Error type";
                worksheet.Cells[1, 3].Value = "Error branch";
                worksheet.Cells[1, 4].Value = "Additional info";

                //Ok now format the first row of the header, but only the first two columns;
                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.YellowGreen);
                    range.Style.Font.Color.SetColor(Color.WhiteSmoke);
                    range.Style.ShrinkToFit = false;
                }

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(1).Width = 195;    //Debería calcularse con la captura de pantalla, pero bueno.
                package.Save();
            }

        }

        public static void WriteExcel(PngBitmapEncoder encoder, List<String> errorType, List<String> selectedErrorBranch, string additionalText)
        {
            // Create the file using the FileInfo object
            var file = new FileInfo(outputDir + fileName);

            // Create the package and make sure you wrap it in a using statement
            using (var package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                Image myImage = Image.FromFile(Properties.Settings.Default["ScreenShotSavePath"].ToString() + "\\screenshot - " + ((int)Properties.Settings.Default["ScreenID"] - 1) + ".png");
                var pic = worksheet.Drawings.AddPicture(((int)Properties.Settings.Default["ScreenID"] - 1 ).ToString(), myImage);
                pic.SetPosition((int)Properties.Settings.Default["ScreenID"] - 1, 0, 0, 0);
                //pic.SetSize(320,240);
                worksheet.Row((int)Properties.Settings.Default["ScreenID"]).Height = 575;

                if(errorType != null)
                {
                    errorType.Reverse();
                    worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 2].Value = string.Join("\r", errorType);
                    selectedErrorBranch.Reverse();
                    worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 3].Value = string.Join("", selectedErrorBranch);
                }

                worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 4].Value = additionalText;

                worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 2].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 3].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 2].Style.WrapText = true;
                worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 3].Style.WrapText = true;
                worksheet.Cells[(int)Properties.Settings.Default["ScreenID"], 4].Style.WrapText = true;
                package.Save();
            }
        }
        
    }
}
