using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace SweetMeSoft.Files
{
    public class Xml
    {
        public static async Task<T> Read<T>(Stream stream)
        {
            try
            {
                var copiedStream = new MemoryStream();
                stream.Position = 0;
                await stream.CopyToAsync(copiedStream);
                copiedStream.Position = 0;

                var serializer = new XmlSerializer(typeof(T));
                using var xmlStreamReader = new StreamReader(copiedStream, Encoding.UTF8);
                var deserialized = serializer.Deserialize(xmlStreamReader);
                copiedStream.Close();
                return deserialized != null ? (T)deserialized : default;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Stream Create<T>(T obj)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(CreateString(obj)));
        }

        public static string CreateString<T>(T obj)
        {
            XmlSerializer xsSubmit = new(typeof(T));
            using var sww = new StringWriter();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using XmlWriter writer = XmlWriter.Create(sww, settings);
            xsSubmit.Serialize(writer, obj);
            return sww.ToString();
        }
    }
}
