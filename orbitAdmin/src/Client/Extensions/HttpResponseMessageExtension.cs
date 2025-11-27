using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SchoolV01.Client.Extensions
{
    public static class HttpResponseMessageExtension
    {
        public async static Task<string> getMessage(this HttpResponseMessage response)
        {
            var stream = response.Content.ReadAsStream();
            var readStream = new StreamReader(stream, Encoding.UTF8);
            return await readStream.ReadToEndAsync();
        }
    }
}
