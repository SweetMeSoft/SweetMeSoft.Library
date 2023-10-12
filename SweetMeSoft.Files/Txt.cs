using SweetMeSoft.Base;

using System.Text;

namespace SweetMeSoft.Files
{
    public class Txt
    {
        public async static Task<List<string>> ReadLines(Stream stream, bool hasHeader = true, bool removeEmptyLines = true)
        {
            var list = new List<string>();
            var headerReaded = !hasHeader;

            list.Clear();
            var copiedStream = new MemoryStream();
            stream.Position = 0;
            await stream.CopyToAsync(copiedStream);
            copiedStream.Position = 0;
            var file = new StreamReader(copiedStream);

            string? line;
            while ((line = file.ReadLine()) != null)
            {
                if (!headerReaded)
                {
                    headerReaded = true;
                }
                else
                {
                    if (!removeEmptyLines || line != string.Empty)
                    {
                        list.Add(line);
                    }
                }
            }

            file.Close();
            return list;
        }

        public async static Task<string> Read(Stream stream)
        {
            var copiedStream = new MemoryStream();
            stream.Position = 0;
            await stream.CopyToAsync(copiedStream);
            copiedStream.Position = 0;
            var file = new StreamReader(copiedStream);

            var text = await file.ReadToEndAsync();
            file.Close();
            return text;
        }

        public async static Task<StreamFile> Generate(List<string> lines, string fileName = "")
        {
            var stream = new MemoryStream();
            var info = new UTF8Encoding(true).GetBytes(string.Join("\n", lines));
            await stream.WriteAsync(info, 0, info.Length);
            stream.Position = 0;
            fileName = string.IsNullOrEmpty(fileName) ? Guid.NewGuid().ToString("N") : fileName;
            return new StreamFile(fileName, stream, Constants.ContentType.txt);
        }
    }
}
