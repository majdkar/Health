using SchoolV01.Shared;
using SchoolV01.Shared.Constants;
using SchoolV01.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Helpers
{
    public static class HelperMethods
    {
        public static long maxFileSize = Constants.MaxFileSizeInByte;

        public static StringContent ToJson(object obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }

        public static async Task<PagedResponse<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<PagedResponse<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseObject;
        }

        public static async Task<FileUploadModel> Save(IBrowserFile file)
        {
            var fileUploadModel = new FileUploadModel();
            fileUploadModel.Content = new StreamContent(file.OpenReadStream(maxAllowedSize: maxFileSize));

            fileUploadModel.Content.Headers.ContentType =
                       new MediaTypeHeaderValue(file.ContentType);

            fileUploadModel.Name = file.Name;
            fileUploadModel.Size = new byte[file.Size];

            await file.OpenReadStream(maxAllowedSize: maxFileSize).ReadAsync(fileUploadModel.Size);
            fileUploadModel.Type = file.ContentType;
            fileUploadModel.Url = $"data:{fileUploadModel.Type};base64,{Convert.ToBase64String(fileUploadModel.Size)}";

            return fileUploadModel;
        }
     


    }
}
