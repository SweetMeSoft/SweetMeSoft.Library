using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
    }
}
