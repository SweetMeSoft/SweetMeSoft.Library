using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using SweetMeSoft.Base.Files;

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
            using var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(T));
            var writer = new MyXmlTextWriter(stream)
            {
                Formatting = Formatting.Indented,
                IndentChar = '\t',
                Indentation = 1
            };
            serializer.Serialize(writer, obj);
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
