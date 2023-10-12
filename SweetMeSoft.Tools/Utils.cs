using SweetMeSoft.Base;

namespace SweetMeSoft.Tools
{
    public class Utils
    {
        public static string GetException(Exception e)
        {
            if (e.Message.Contains("An error occurred while updating the entries. See the inner exception for details."))
            {
                return GetException(e.InnerException);
            }

            return e.Message;
        }

        public static string GetRandomNumber(int lenght)
        {
            var result = "";
            var random = new Random();
            for (var i = 0; i <= lenght; i++)
            {
                result += random.Next(0, 9);
            }

            return result;
        }

        public static void WriteToAPath(StreamFile stream, string path)
        {
            stream.Stream.Position = 0;
            var fileStream = new FileStream(path + "/" + stream.FileName, FileMode.Create, FileAccess.Write);
            stream.Stream.CopyTo(fileStream);
            fileStream.Dispose();
        }

        public static void WriteToAPath(List<StreamFile> streams, string path)
        {
            foreach (var stream in streams)
            {
                WriteToAPath(stream, path);
            }
        }
    }
}
