using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using NUnit.Framework;

namespace Ara3D.IfcParser.Test
{
    public static class ExcelReader
    {
       
        [Test]
        public static void TestExcelRead()
        {
            // Example usage
            var filePath = @"C:\Users\cdigg\dev\impraria\test.xlsx";
            var sheetName = "Model Content Matrix";
            using (var workbook = new XLWorkbook(filePath))
            {
                // Access the specified sheet
                var worksheet = workbook.Worksheet(sheetName);

                for (var rowNumber = 3; rowNumber < 465; ++rowNumber)
                {

                    var row = worksheet.Row(rowNumber);
                    var rowData = new List<string>();

                    // Iterate through the cells in the row
                    foreach (var cell in row.Cells())
                    {
                        // Add the cell value to the rowData list
                        rowData.Add(cell.Value.ToString());
                    }

                    // Print the row data
                    foreach (var cellValue in rowData)
                    {
                        Console.Write(cellValue + ",");
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
