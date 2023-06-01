using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using SweetMeSoft.Base.Files;
using SweetMeSoft.Base;

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

        public static StreamFile Create<T>(T obj, string fileName = "", bool indent = false)
        {
            fileName = string.IsNullOrEmpty(fileName) ? Guid.NewGuid().ToString("N") : fileName;
            return new StreamFile(fileName, new MemoryStream(Encoding.UTF8.GetBytes(CreateString(obj, indent))), Constants.ContentType.xml);
        }

        public static string CreateString<T>(T obj, bool indent = false)
        {
            using var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(T));
            var writer = indent ? new MyXmlTextWriter(stream)
            {
                Formatting = Formatting.Indented,
                IndentChar = '\t',
                Indentation = 1
            } : new MyXmlTextWriter(stream);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            serializer.Serialize(writer, obj, namespaces);
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
