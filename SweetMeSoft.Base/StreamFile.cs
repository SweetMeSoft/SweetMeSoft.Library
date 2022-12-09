using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Http;

namespace SweetMeSoft.Base
{
    public class StreamFile
    {
        public StreamFile() { }

        public StreamFile(string fileName, Stream stream, Constants.ContentType contentType)
        {
            FileName = fileName;
            Stream = stream;
            ContentType = contentType;
        }

        public StreamFile(IFormFile file)
        {
            FileName = file.FileName;
            ContentType = Constants.ContentTypesDict.FirstOrDefault(model => model.Value == file.ContentType).Key;
            Stream = file.OpenReadStream();
        }

        private string fileName;

        public string FileName
        {
            get
            {
                return fileName.ToLower().EndsWith(ContentType.ToString()) ? fileName : fileName + "." + ContentType.ToString();
            }
            set
            {
                fileName = value;
            }
        }

        public Stream Stream { get; set; }

        public Constants.ContentType ContentType { get; set; }

        public string GetContentType()
        {
            return Constants.ContentTypesDict[ContentType];
        }
    }
}
