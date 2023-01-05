using System.Collections.Generic;
using System.Linq;

namespace SweetMeSoft.Base
{
    public class Constants
    {

        public static readonly string[] Months = new string[]
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        public enum ContentType
        {
            xlsx,
            xls,
            csv,
            xml,
            zip,
            txt,
            dat,
            pagos
        }

        public static readonly Dictionary<ContentType, string> ContentTypesDict = new()
        {
            { ContentType.xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ContentType.xls, "application/vnd.ms-excel" },
            { ContentType.csv, "text/csv" },
            { ContentType.xml, "text/xml" },
            { ContentType.zip, "application/zip" },
            { ContentType.txt, "text/plain" },
            { ContentType.dat, "text/plain" },
            { ContentType.pagos, "text/plain" },
        };

        public static ContentType GetContentType(string extension)
        {
            extension = extension.ToLower();
            var key = ContentTypesDict.FirstOrDefault(model => model.Key.ToString().ToLower() == extension);
            return key.Value == null ? ContentType.txt : key.Key;
        }
    }
}