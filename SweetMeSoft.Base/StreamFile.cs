using System;
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
            Created = DateTime.Now;
        }

        public StreamFile(string fileName, Stream stream, Constants.ContentType contentType, DateTime created)
        {
            FileName = fileName;
            Stream = stream;
            ContentType = contentType;
            Created = created;
        }

        public StreamFile(IFormFile file)
        {
            FileName = file.FileName;
            ContentType = Constants.ContentTypesDict.FirstOrDefault(model => model.Value == file.ContentType).Key;
            Stream = file.OpenReadStream();
            Created = DateTime.Now;
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

        public DateTime Created { get; set; }

        public string GetContentType()
        {
            return Constants.ContentTypesDict[ContentType];
        }
    }
}
