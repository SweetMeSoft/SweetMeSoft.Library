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
            bin,
            csv,
            dat,
            doc,
            docx,
            gif,
            jpg,
            msg,
            pagos,
            pdf,
            png,
            ppt,
            pptx,
            rar,
            txt,
            xlsx,
            xls,
            xml,
            zip
        }

        public static readonly Dictionary<ContentType, string> ContentTypesDict = new()
        {
            { ContentType.bin, "application/octet-stream" },
            { ContentType.csv, "text/csv" },
            { ContentType.doc, "application/msword" },
            { ContentType.docx, "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ContentType.dat, "text/plain" },
            { ContentType.gif, "image/gif" },
            { ContentType.jpg, "image/jpeg" },
            { ContentType.msg, "application/vnd.ms-outlook" },
            { ContentType.pagos, "text/plain" },
            { ContentType.png, "image/png" },
            { ContentType.pdf, "application/pdf" },
            { ContentType.ppt, "application/vnd.ms-powerpoint" },
            { ContentType.pptx, "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ContentType.rar, "application/vnd.rar" },
            { ContentType.txt, "text/plain" },
            { ContentType.xls, "application/vnd.ms-excel" },
            { ContentType.xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ContentType.xml, "text/xml" },
            { ContentType.zip, "application/zip" },
        };

        public static ContentType GetContentType(string extension)
        {
            extension = extension.ToLower();
            var key = ContentTypesDict.FirstOrDefault(model => model.Key.ToString().ToLower() == extension);
            return key.Value == null ? ContentType.txt : key.Key;
        }
    }
}