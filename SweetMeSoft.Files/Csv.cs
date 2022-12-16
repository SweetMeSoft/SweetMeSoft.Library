using CsvHelper.Configuration;
using CsvHelper;
using SweetMeSoft.Base;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace SweetMeSoft.Files
{
    public class Csv
    {
        public static async Task<IEnumerable<T>> Read<T>(StreamFile streamFile, bool hasHeader = true, string delimiter = "|")
        {
            return await Read<T>(streamFile.Stream, hasHeader, delimiter);
        }

        public static async Task<IEnumerable<T>> Read<T>(Stream stream, bool hasHeader = true, string delimiter = "|")
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

        public static async Task<IEnumerable<T>> Read<T, TMap>(StreamFile streamFile, bool hasHeader = true, string delimiter = "|") where TMap : ClassMap
        {
            return await Read<T, TMap>(streamFile.Stream, hasHeader, delimiter);
        }

        public static async Task<IEnumerable<T>> Read<T, TMap>(Stream stream, bool hasHeader = true, string delimiter = "|") where TMap : ClassMap
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
            csv.Context.RegisterClassMap<TMap>();
            return await csv.GetRecordsAsync<T>().ToListAsync();
        }

        public static async Task<MemoryStream> Create<T>(IEnumerable<T> list)
        {
            var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(list);

            var copiedStream = new MemoryStream();
            stream.Position = 0L;
            await stream.CopyToAsync(copiedStream);
            copiedStream.Position = 0L;

            return copiedStream;
        }
    }
}
