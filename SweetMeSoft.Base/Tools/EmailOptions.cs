using System.Collections.Generic;

namespace SweetMeSoft.Base.Tools
{
    public class EmailOptions
    {
        public EmailOptions()
        {
            StreamFiles = new List<StreamFile>();
            AdditionalDestinataries = new List<string>();
            CC = new List<string>();
        }

        public EmailOptions(StreamFile file)
        {
            StreamFiles = new List<StreamFile>()
            {
                file
            };
            AdditionalDestinataries = new List<string>();
            CC = new List<string>();
        }

        public EmailOptions(List<StreamFile> files)
        {
            StreamFiles = files;
            AdditionalDestinataries = new List<string>();
            CC = new List<string>();
        }

        public string EmailFrom { get; set; }

        public string Password { get; set; }

        public List<string> AdditionalDestinataries { get; set; }

        public List<string> CC { get; set; }

        public List<StreamFile> StreamFiles { get; set; }
    }
}
