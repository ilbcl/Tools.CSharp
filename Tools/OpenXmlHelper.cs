using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Linq;

namespace StraxeHelpers
{
    public class CellVal
    {
        public string ColumnName { get; set; }
        public uint RowIndex { get; set; }
        public string Text { get; set; }
    }

    public class OpenXmlHelper
    {
        public static byte[] GetExcelFileWithChanges(string filePath, List<CellVal> changesList, int sheetPart = 1)
        {
            byte[] filebytes = File.ReadAllBytes(filePath);
            using (MemoryStream stream = new MemoryStream(filebytes))
            {
                using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(stream, true))
                {
                    WorksheetPart worksheetPart = spreadSheet.WorkbookPart.WorksheetParts.ElementAt(sheetPart - 1);
                    if (worksheetPart != null)
                    {
                        var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>().Elements<Row>();
                        foreach (var cl in changesList)
                        {
                            Row row = rows.FirstOrDefault(r => r.RowIndex == cl.RowIndex);
                            Cell cell = row?.Elements<Cell>().FirstOrDefault(c => string.Compare(c.CellReference.Value, cl.ColumnName + cl.RowIndex, true) == 0);
                            if (cell != null)
                            {
                                cell.CellValue = new CellValue(cl.Text);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                            }
                        }
                        worksheetPart.Worksheet.Save();
                    }
                }
                return stream.ToArray();
            }
        }

    }
}
