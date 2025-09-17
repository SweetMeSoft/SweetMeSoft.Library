using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using OfficeOpenXml;

using SweetMeSoft.Base;
using SweetMeSoft.Base.Attributes;
using SweetMeSoft.Base.Files;
using SweetMeSoft.Tools;

using System.Globalization;

namespace SweetMeSoft.Files;

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
                                property.SetValue(row, cell.CellType == CellType.String ? DateTime.ParseExact(cell.StringCellValue, columnAttr.DateFormat, null) : cell.DateCellValue);
                            }

                            if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                            {
                                property.SetValue(row, cell.CellType == CellType.String ? Converters.StringToDecimal(cell.StringCellValue) : (decimal)cell.NumericCellValue);
                            }

                            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                            {
                                property.SetValue(row, cell.CellType == CellType.String ? Converters.StringToInt(cell.StringCellValue) : (int)cell.NumericCellValue);
                            }

                            if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                            {
                                property.SetValue(row, cell.CellType == CellType.String ? Converters.StringToBool(cell.StringCellValue) : cell.BooleanCellValue);
                            }

                            if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                            {
                                property.SetValue(row, Guid.Parse(cell.StringCellValue));
                            }

                            if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(row, cell.StringCellValue);
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
            for (int rowIndex = options.HeaderRow; rowIndex <= end.Row; rowIndex++)
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
                                        if (cell.Value is double && cell.Value.ToString() != "NaN")
                                        {
                                            property.SetValue(row, DateTime.FromOADate(cell.GetValue<double>()));
                                        }
                                        else
                                        {
                                            property.SetValue(row, cell.Value is DateTime ? cell.GetValue<DateTime>() : DateTime.ParseExact(cell.GetValue<string>(), columnAttr.DateFormat, null));
                                        }
                                    }

                                    if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                                    {
                                        var value = cell.GetValue<string>();
                                        if (value != null)
                                        {
                                            value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                                            property.SetValue(row, Convert.ToDecimal(value));
                                        }
                                    }

                                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                                    {
                                        property.SetValue(row, cell.GetValue<int>());
                                    }

                                    if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                                    {
                                        property.SetValue(row, cell.GetValue<bool>());
                                    }

                                    if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                                    {
                                        property.SetValue(row, Guid.Parse(cell.GetValue<string>()));
                                    }

                                    if (cell.Value is ExcelErrorValue)
                                    {
                                        property.SetValue(row, cell.GetValue<ExcelErrorValue>().ToString());
                                    }

                                    if (property.PropertyType == typeof(string))
                                    {
                                        property.SetValue(row, cell.GetValue<string>());
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

    public static List<List<T>> ReadAllSheets<T>(StreamFile file, int headerRow = 1) where T : new()
    {
        return ReadAllSheets<T>(file.Stream, headerRow);
    }

    public static List<List<T>> ReadAllSheets<T>(Stream file, int headerRow = 1) where T : new()
    {
        return ReadAllSheets<T>(new ExcelOptions(file, headerRow));
    }

    public static List<List<T>> ReadAllSheets<T>(ExcelOptions options) where T : new()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var allSheets = new List<List<T>>();
        using var excelFile = new ExcelPackage(options.Stream);
        
        foreach (var sheet in excelFile.Workbook.Worksheets)
        {
            var sheetList = new List<T>();
            var headerFilled = false;
            var header = new List<string>();
            var start = sheet.Dimension.Start;
            var end = sheet.Dimension.End;
            var currentHeaderRow = options.HeaderRow < start.Row ? start.Row : options.HeaderRow;
            
            for (int rowIndex = currentHeaderRow; rowIndex <= end.Row; rowIndex++)
            {
                try
                {
                    if (!headerFilled)
                    {
                        for (int columnIndex = start.Column; columnIndex <= end.Column; columnIndex++)
                        {
                            header.Add(sheet.Cells[currentHeaderRow, columnIndex].Text);
                        }

                        rowIndex = currentHeaderRow;
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
                                        if (cell.Value is double && cell.Value.ToString() != "NaN")
                                        {
                                            property.SetValue(row, DateTime.FromOADate(cell.GetValue<double>()));
                                        }
                                        else
                                        {
                                            property.SetValue(row, cell.Value is DateTime ? cell.GetValue<DateTime>() : DateTime.ParseExact(cell.GetValue<string>(), columnAttr.DateFormat, null));
                                        }
                                    }

                                    if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                                    {
                                        var value = cell.GetValue<string>();
                                        if (value != null)
                                        {
                                            value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                                            property.SetValue(row, Convert.ToDecimal(value));
                                        }
                                    }

                                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                                    {
                                        property.SetValue(row, cell.GetValue<int>());
                                    }

                                    if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                                    {
                                        property.SetValue(row, cell.GetValue<bool>());
                                    }

                                    if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                                    {
                                        property.SetValue(row, Guid.Parse(cell.GetValue<string>()));
                                    }

                                    if (cell.Value is ExcelErrorValue)
                                    {
                                        property.SetValue(row, cell.GetValue<ExcelErrorValue>().ToString());
                                    }

                                    if (property.PropertyType == typeof(string))
                                    {
                                        property.SetValue(row, cell.GetValue<string>());
                                    }
                                }
                            }
                        }

                        sheetList.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    options.ErrorCallback?.Invoke(rowIndex, ex);
                }
            }
            
            allSheets.Add(sheetList);
        }

        return allSheets;
    }

    public static StreamFile Generate<T>(IEnumerable<T> list, string sheetName, string fileName = "")
    {
        return Generate(
        [
            new ExcelSheet(sheetName, list)
        ], fileName);
    }

    public static StreamFile Generate(List<ExcelSheet> sheets, string fileName = "")
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

        fileName = string.IsNullOrEmpty(fileName) ? Guid.NewGuid().ToString("N") : fileName;
        return new StreamFile(fileName, new MemoryStream(book.GetAsByteArray()), Constants.ContentType.xlsx);
    }

    public static StreamFile GenerateTemplate<T>()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var book = new ExcelPackage();
        var realSheet = book.Workbook.Worksheets.Add("Template");
        WriteHeader(realSheet, typeof(T), true);
        realSheet.Cells.AutoFitColumns();
        return new StreamFile("Template", new MemoryStream(book.GetAsByteArray()), Constants.ContentType.xlsx);
    }

    public static string ValidateFormat<T>(StreamFile streamFile, int headerRow = 1)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var excelFile = new ExcelPackage(streamFile.Stream);
        var validations = "";
        var header = new List<string>();
        var sheet = excelFile.Workbook.Worksheets[0];
        var start = sheet.Dimension.Start;
        var end = sheet.Dimension.End;

        for (int columnIndex = start.Column; columnIndex <= end.Column; columnIndex++)
        {
            header.Add(sheet.Cells[headerRow, columnIndex].Text);
        }

        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "ColumnExcelAttribute");
            var columnAttr = attr == null ? new ColumnExcelAttribute(property.Name) : attr as ColumnExcelAttribute;
            var headerCell = header.FindIndex(a => a == columnAttr.Name);
            if (headerCell == -1)
            {
                validations += "El campo " + columnAttr.Name + " no está en el archivo.\n";
            }
        }

        return validations;
    }

    private static void WriteHeader(ExcelWorksheet sheet, Type type, bool writeExplanations = false)
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

                if (writeExplanations)
                {
                    var templateAttr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "TemplateAttribute");
                    if (templateAttr != null)
                    {
                        sheet.SetValue(rowIndex + 1, columnIndex, (templateAttr as TemplateAttribute).Explanation);
                    }
                }

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
            var ignoreAttr = property.GetCustomAttributes(true).Where(model => model.GetType().Name.Contains("Ignore"));
            if (!ignoreAttr.Any())
            {
                if (property.GetValue(obj, null) != null && property.GetValue(obj, null).ToString() != "")
                {
                    sheet.SetValue(rowIndex, columnIndex, property.GetValue(obj, null));
                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";//DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    }
                    else
                    {
                        if (property.PropertyType == typeof(TimeSpan) || property.PropertyType == typeof(TimeSpan?))
                        {
                            sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "HH:mm:ss";
                        }
                        else
                        {
                            var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "ColumnExcelAttribute");
                            var columnAttr = attr == null ? new ColumnExcelAttribute(property.Name) : attr as ColumnExcelAttribute;
                            if (attr != null && columnAttr.Type == ExcelColumnType.Currency)
                            {
                                sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "$ #,##0.00";
                            }

                            if (property.Name.Contains("FECHA")
                                || property.Name.Equals("CLIENTE_CODIGO_TRIBUTO")
                                || property.Name.Equals("LINEA_IMPUESTO1_TIPO")
                                || property.Name.Equals("LINEA_IMPUESTO1_PORCENTAJE"))
                            {
                                sheet.Cells[rowIndex, columnIndex].Style.QuotePrefix = true;
                            }
                        }
                    }
                }

                columnIndex++;
            }
        }

        return rowIndex + 1;
    }
}