using System.Net.Http;

namespace SchoolV01.Client.Helpers
{
    public class FileUploadModel
    {
        public StreamContent Content { get; set; }
        public string Name { get; set; }
        public byte[] Size { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
    }
}
