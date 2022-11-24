using CsvHelper;
using CsvHelper.Configuration;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using OfficeOpenXml;

using SweetMeSoft.Base;
using SweetMeSoft.Base.Attributes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SweetMeSoft.Tools
{
    public class Excel
    {
        public static List<T> ReadExcel2003File<T>(StreamFile file, int headerRowIndex = 0) where T : new()
        {
            return ReadExcel2003File<T>(file.Stream, headerRowIndex);
        }

        public static List<T> ReadExcel2003File<T>(Stream stream, int headerRowIndex = 0) where T : new()
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
                                        property.SetValue(row, DateTime.ParseExact(cell.StringCellValue, columnAttr.Format, null));
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

        public static List<T> ReadExcelFile<T>(StreamFile file, int headerRow = 1) where T : new()
        {
            return ReadExcelFile<T>(file.Stream, headerRow);
        }

        public static List<T> ReadExcelFile<T>(Stream stream, int headerRow = 1) where T : new()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var list = new List<T>();
            using var excelFile = new ExcelPackage(stream);
            foreach (var sheet in excelFile.Workbook.Worksheets)
            {
                var headerFilled = false;
                var header = new List<string>();
                var start = sheet.Dimension.Start;
                var end = sheet.Dimension.End;
                headerRow = headerRow < start.Row ? start.Row : headerRow;
                for (int rowIndex = start.Row; rowIndex <= end.Row; rowIndex++)
                {
                    try
                    {
                        if (!headerFilled)
                        {
                            for (int columnIndex = start.Column; columnIndex <= end.Column; columnIndex++)
                            {
                                header.Add(sheet.Cells[headerRow, columnIndex].Text);
                            }

                            rowIndex = headerRow;
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
                                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                    {
                                        if (cell.Value is double)
                                        {
                                            property.SetValue(row, DateTime.FromOADate(cell.GetValue<double>()));
                                        }
                                        else
                                        {
                                            if (cell.Value is DateTime)
                                            {
                                                property.SetValue(row, cell.GetValue<DateTime>());
                                            }
                                            else
                                            {
                                                property.SetValue(row, DateTime.ParseExact(cell.GetValue<string>(), columnAttr.Format, null));
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

                            list.Add(row);
                        }
                    }
                    catch (Exception ex)
                    {
                        var a = ex.Message;
                    }
                }
            }

            return list;
        }

        public static MemoryStream GenerateExcelFile<T>(List<T> list, string sheetName)
        {
            return GenerateExcelFile(new List<ExcelSheet>()
            {
                new ExcelSheet
                {
                    Name = sheetName,
                    List = list,
                    Type = typeof(T)
                }
            });
        }

        public static MemoryStream GenerateExcelFile(List<ExcelSheet> sheets)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var book = new ExcelPackage();
            foreach (var sheet in sheets)
            {
                var rowIndex = 2;
                var realSheet = book.Workbook.Worksheets.Add(sheet.Name);
                WriteHeader(realSheet, sheet.Type);
                foreach (var item in sheet.List)
                {
                    rowIndex = WriteRow(realSheet, rowIndex, item, sheet.Type);
                }

                realSheet.Cells.AutoFitColumns();
            }

            return new MemoryStream(book.GetAsByteArray());
        }

        public static async Task<List<T>> ReadCsv<T>(Stream stream, bool hasHeader = true, string delimiter = "|")
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                IgnoreBlankLines = true,
                HasHeaderRecord = hasHeader,
                BadDataFound = null,
                ReadingExceptionOccurred = (ex) =>
                {
                    return false;
                }
            };

            var copiedStream = new MemoryStream();
            stream.Position = 0;
            await stream.CopyToAsync(copiedStream);
            copiedStream.Position = 0;

            using var reader = new StreamReader(copiedStream);
            using var csv = new CsvReader(reader, config);
            //csv.Context.RegisterClassMap<TMap>();
            return await csv.GetRecordsAsync<T>().ToListAsync();
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

        public static async Task<List<T>> ReadCsv<T>(StreamFile streamFile, bool hasHeader = true)
        {
            var list = new List<T>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                IgnoreBlankLines = true,
                IgnoreReferences = true,
                HasHeaderRecord = hasHeader,
                MissingFieldFound = (data) =>
                {
                    var a = data;
                },
                BadDataFound = (data) =>
                {
                    var a = data;
                },
                ReadingExceptionOccurred = (ex) =>
                {
                    return false;
                }
            };

            var copiedStream = new MemoryStream();
            streamFile.Stream.Position = 0;
            await streamFile.Stream.CopyToAsync(copiedStream);
            copiedStream.Position = 0;
            var file = new StreamReader(copiedStream);

            using var reader = new StreamReader(copiedStream);
            using var csv = new CsvReader(reader, config);
            //csv.Context.RegisterClassMap<PayUPaymentMap>();
            return await csv.GetRecordsAsync<T>().ToListAsync();
        }

        public static MemoryStream CreateCsv<T>(List<T> list)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(list);
            return memoryStream;
        }
    }

    public class ExcelSheet
    {

        public string Name { get; set; }

        public IList List { get; set; }

        public Type Type { get; set; }
    }
}