using System.Collections.Generic;

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
            txt
        }

        public static readonly Dictionary<ContentType, string> ContentTypesDict = new Dictionary<ContentType, string>
        {
            { ContentType.xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ContentType.xls, "application/vnd.ms-excel" },
            { ContentType.csv, "text/csv" },
            { ContentType.xml, "text/xml" },
            { ContentType.zip, "application/zip" },
            { ContentType.txt, "text/plain" },
        };

    }
}