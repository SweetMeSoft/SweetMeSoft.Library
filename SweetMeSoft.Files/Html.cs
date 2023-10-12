using SweetMeSoft.Base.Attributes;
using SweetMeSoft.Base;
using SweetMeSoft.Tools;
using HtmlAgilityPack;
using System.Globalization;

namespace SweetMeSoft.Files
{
    public class Html
    {
        public static async Task<List<T>> ReadTable<T>(StreamFile streamFile) where T : new()
        {
            try
            {
                var file = new StreamReader(streamFile.Stream);
                var html = await file.ReadToEndAsync();
                return ReadTable<T>(html);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<T> ReadTable<T>(string html) where T : new()
        {
            var list = new List<T>();
            html = html.Replace("&nbsp;", "");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var header = doc.DocumentNode.SelectSingleNode("//table")
                .Descendants("tr")
                .Take(1)
                .Where(tr => tr.Elements("td").Count() > 1)
                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                .ToList()[0];
            var rows = doc.DocumentNode.SelectSingleNode("//table")
                .Descendants("tr")
                .Skip(1)
                .Where(tr => tr.Elements("td").Count() > 1)
                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                .ToList();

            foreach (var row in rows)
            {
                var properties = typeof(T).GetProperties();
                var obj = new T();
                foreach (var property in properties)
                {
                    var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "ColumnExcelAttribute");
                    var columnAttr = attr == null ? new ColumnExcelAttribute(property.Name) : attr as ColumnExcelAttribute;
                    var headerCellIndex = header.FindIndex(a => a == columnAttr.Name);
                    if (headerCellIndex != -1)
                    {
                        var cell = row[headerCellIndex];

                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(obj, cell);
                        }

                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        {
                            cell = cell.Replace("a.m.", "AM").Replace("p.m.", "PM");
                            property.SetValue(obj, DateTime.ParseExact(cell, columnAttr.DateFormat, CultureInfo.InvariantCulture));
                        }

                        if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                        {
                            property.SetValue(obj, Converters.StringToDecimal(cell));
                        }

                        if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                        {
                            property.SetValue(obj, Converters.StringToDouble(cell));
                        }

                        if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                        {
                            property.SetValue(obj, Converters.StringToInt(cell));
                        }

                        if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                        {
                            if (cell == columnAttr.BoolTrueValue || cell == columnAttr.BoolFalseValue)
                            {
                                property.SetValue(obj, cell == columnAttr.BoolTrueValue);
                            }
                        }
                    }
                }

                list.Add(obj);
            }

            return list;
        }
    }
}
