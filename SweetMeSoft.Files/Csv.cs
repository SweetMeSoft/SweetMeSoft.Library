using CsvHelper.Configuration;
using CsvHelper;
using SweetMeSoft.Base;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SweetMeSoft.Files
{
    public class Csv
    {
        public static async Task<List<T>> ReadCsv<T>(StreamFile streamFile, bool hasHeader = true, string delimiter = "|")
        {
            return await ReadCsv<T>(streamFile.Stream, hasHeader, delimiter);
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
}
