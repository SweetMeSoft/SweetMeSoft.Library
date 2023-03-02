using SweetMeSoft.Base;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using NPOI.HPSF;
using System.Text;
using System;

namespace SweetMeSoft.Files
{
    public class Zip
    {
        public static async Task<List<StreamFile>> Uncompress(Stream zipStream)
        {
            var streams = new List<StreamFile>();
            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
            foreach (var entry in zip.Entries)
            {
                var copiedStream = new MemoryStream();
                var stream = entry.Open();
                await stream.CopyToAsync(copiedStream);
                copiedStream.Position = 0;

                streams.Add(new StreamFile()
                {
                    FileName = entry.Name,
                    Stream = copiedStream,
                    ContentType = Constants.GetContentType(entry.Name.Substring(entry.Name.LastIndexOf(".") + 1))
                });
            }

            return streams;
        }

        public async static Task<StreamFile> Compress(string fileName = "", params StreamFile[] files)
        {
            return await Compress(files.ToList(), fileName);
        }

        public async static Task<StreamFile> Compress(List<StreamFile> fileStreams, string fileName = "")
        {
            using var stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);
            foreach (var fileWithStream in fileStreams)
            {
                var newInvoices = archive.CreateEntry(fileWithStream.FileName, CompressionLevel.Optimal);
                using var writer = new BinaryWriter(newInvoices.Open());
                fileWithStream.Stream.Seek(0, SeekOrigin.Begin);

                byte[] buffer = new byte[16 * 1024];
                using var ms = new MemoryStream();
                int read;
                while ((read = fileWithStream.Stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                writer.Write(ms.ToArray());
                writer.Close();
            }
            archive.Dispose();

            var copiedStream = new MemoryStream();
            stream.Position = 0L;
            await stream.CopyToAsync(copiedStream);
            copiedStream.Position = 0L;

            fileName = string.IsNullOrEmpty(fileName) ? Guid.NewGuid().ToString("N") : fileName;
            return new StreamFile(fileName, copiedStream, Constants.ContentType.zip);
        }

    }
}
