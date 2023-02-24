using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using OfficeOpenXml;

using SweetMeSoft.Base;
using SweetMeSoft.Base.Attributes;
using SweetMeSoft.Base.Files;
using SweetMeSoft.Tools;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SweetMeSoft.Files
{
    public class Excel
    {
        public static IEnumerable<T> Read2003File<T>(StreamFile file, int headerRowIndex = 0) where T : new()
        {
            return Read2003File<T>(file.Stream, headerRowIndex);
        }

        public static IEnumerable<T> Read2003File<T>(Stream stream, int headerRowIndex = 0) where T : new()
        {
            var list = new List<T>();
            var workbook = new HSSFWorkbook(stream);
            var sheet = workbook.GetSheetAt(0);

            var headerRow = sheet.GetRow(headerRowIndex);
            var header = new List<string>();
            foreach (var cell in headerRow.Cells)
            {
                header.Add(cell.ToString());
            }
            headerRowIndex++;

            for (var indexRow = headerRowIndex; indexRow <= sheet.LastRowNum; indexRow++)
            {
                if (sheet.GetRow(indexRow) != null) //null is when the row only contains empty cells 
                {
                    var properties = typeof(T).GetProperties();
                    var row = new T();
                    foreach (var property in properties)
                    {
                        try
                        {
                            var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "ColumnExcelAttribute");
                            var columnAttr = attr == null ? new ColumnExcelAttribute(property.Name) : attr as ColumnExcelAttribute;
                            var headerCell = header.FindIndex(a => a == columnAttr.Name);
                            if (headerCell != -1)
                            {
                                var cell = sheet.GetRow(indexRow).Cells[headerCell];
                                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                {
                                    if (cell.CellType == CellType.String)
                                    {
                                        property.SetValue(row, DateTime.ParseExact(cell.StringCellValue, columnAttr.DateFormat, null));
                                    }
                                    else
                                    {
                                        property.SetValue(row, cell.DateCellValue);
                                    }
                                }
                                else
                                {
                                    if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                                    {
                                        if (cell.CellType == CellType.String)
                                        {
                                            property.SetValue(row, Converters.StringToDecimal(cell.StringCellValue));
                                        }
                                        else
                                        {
                                            property.SetValue(row, (decimal)cell.NumericCellValue);
                                        }
                                    }
                                    else
                                    {
                                        if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                                        {
                                            if (cell.CellType == CellType.String)
                                            {
                                                property.SetValue(row, Converters.StringToInt(cell.StringCellValue));
                                            }
                                            else
                                            {
                                                property.SetValue(row, (int)cell.NumericCellValue);
                                            }
                                        }
                                        else
                                        {
                                            property.SetValue(row, cell.StringCellValue);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var a = ex;
                        }
                    }

                    list.Add(row);
                }
            }

            return list;
        }

        public static IEnumerable<T> Read<T>(StreamFile file, int headerRow = 1) where T : new()
        {
            return Read<T>(file.Stream, headerRow);
        }

        public static IEnumerable<T> Read<T>(Stream file, int headerRow = 1) where T : new()
        {
            return Read<T>(new ExcelOptions(file, headerRow));
        }

        public static IEnumerable<T> Read<T>(ExcelOptions options) where T : new()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var list = new List<T>();
            using var excelFile = new ExcelPackage(options.Stream);
            foreach (var sheet in excelFile.Workbook.Worksheets)
            {
                var headerFilled = false;
                var header = new List<string>();
                var start = sheet.Dimension.Start;
                var end = sheet.Dimension.End;
                options.HeaderRow = options.HeaderRow < start.Row ? start.Row : options.HeaderRow;
                for (int rowIndex = start.Row; rowIndex <= end.Row; rowIndex++)
                {
                    try
                    {
                        if (!headerFilled)
                        {
                            for (int columnIndex = start.Column; columnIndex <= end.Column; columnIndex++)
                            {
                                header.Add(sheet.Cells[options.HeaderRow, columnIndex].Text);
                            }

                            rowIndex = options.HeaderRow;
                            headerFilled = true;
                        }
                        else
                        {
                            var properties = typeof(T).GetProperties();
                            var row = new T();
                            foreach (var property in properties)
                            {
                                var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "ColumnExcelAttribute");
                                var columnAttr = attr == null ? new ColumnExcelAttribute(property.Name) : attr as ColumnExcelAttribute;
                                var headerCell = header.FindIndex(a => a == columnAttr.Name);
                                if (headerCell != -1)
                                {
                                    var cell = sheet.Cells[rowIndex, headerCell + start.Column];
                                    if (cell.Value != null)
                                    {
                                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                        {
                                            if (cell.Value is double)
                                            {
                                                if (cell.Value.ToString() != "NaN")
                                                {
                                                    property.SetValue(row, DateTime.FromOADate(cell.GetValue<double>()));
                                                }
                                            }
                                            else
                                            {
                                                if (cell.Value is DateTime)
                                                {
                                                    property.SetValue(row, cell.GetValue<DateTime>());
                                                }
                                                else
                                                {
                                                    property.SetValue(row, DateTime.ParseExact(cell.GetValue<string>(), columnAttr.DateFormat, null));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                                            {
                                                var value = cell.GetValue<string>();
                                                if (value != null)
                                                {
                                                    value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                                                    property.SetValue(row, Convert.ToDecimal(value));
                                                }
                                            }
                                            else
                                            {
                                                if (cell.Value is ExcelErrorValue)
                                                {
                                                    property.SetValue(row, cell.GetValue<ExcelErrorValue>().ToString());
                                                }
                                                else
                                                {
                                                    property.SetValue(row, cell.GetValue<string>());
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            list.Add(row);
                        }
                    }
                    catch (Exception ex)
                    {
                        options.ErrorCallback?.Invoke(rowIndex, ex);
                    }
                }
            }

            return list;
        }

        public static MemoryStream Generate<T>(IEnumerable<T> list, string sheetName)
        {
            return Generate(new List<ExcelSheet>()
            {
                new ExcelSheet
                {
                    Name = sheetName,
                    List = list
                }
            });
        }

        public static MemoryStream Generate(List<ExcelSheet> sheets)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var book = new ExcelPackage();
            foreach (var sheet in sheets)
            {
                var rowIndex = 2;
                var realSheet = book.Workbook.Worksheets.Add(sheet.Name);
                var type = sheet.List.GetType().GetGenericArguments()[0];
                WriteHeader(realSheet, type);
                foreach (var item in sheet.List)
                {
                    rowIndex = WriteRow(realSheet, rowIndex, item, type);
                }

                realSheet.Cells.AutoFitColumns();
            }

            return new MemoryStream(book.GetAsByteArray());
        }

        private static void WriteHeader(ExcelWorksheet sheet, Type type)
        {
            var rowIndex = 1;
            var columnIndex = 1;
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attrs = property.GetCustomAttributes(true).Where(model => model.GetType().Name.Contains("Ignore"));
                if (!attrs.Any())
                {
                    var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "ColumnExcelAttribute");
                    var columnAttr = attr == null ? new ColumnExcelAttribute(property.Name) : attr as ColumnExcelAttribute;

                    sheet.SetValue(rowIndex, columnIndex, columnAttr.Name);
                    columnIndex++;
                }
            }
        }

        private static int WriteRow(ExcelWorksheet sheet, int rowIndex, object obj, Type type)
        {
            var columnIndex = 1;
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attrs = property.GetCustomAttributes(true).Where(model => model.GetType().Name.Contains("Ignore"));
                if (!attrs.Any())
                {
                    if (property.GetValue(obj, null) != null && property.GetValue(obj, null).ToString() != "")
                    {
                        sheet.SetValue(rowIndex, columnIndex, property.GetValue(obj, null));
                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        {
                            sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        }
                        else
                        {
                            if (property.Name.Contains("FECHA")
                                || property.Name.Equals("CLIENTE_CODIGO_TRIBUTO")
                                || property.Name.Equals("LINEA_IMPUESTO1_TIPO")
                                || property.Name.Equals("LINEA_IMPUESTO1_PORCENTAJE"))
                            {
                                sheet.Cells[rowIndex, columnIndex].Style.QuotePrefix = true;
                            }
                        }
                    }

                    columnIndex++;
                }
            }

            return rowIndex + 1;
        }
    }
}