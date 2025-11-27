using SchoolV01.Client.Extensions;
using SchoolV01.Client.Helpers;
using SchoolV01.Shared;
using SchoolV01.Shared.ViewModels.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolV01.Shared.Constants;

namespace SchoolV01.Client.Pages.Menus
{
    public partial class AddEditPageModal
    {
        [Parameter]
        public PageUpdateModel PageModel { get; set; } = new();

        [CascadingParameter]
        private IMudDialogInstance  MudDialog { get; set; }
        private bool _processing = false;

        private IList<IBrowserFile> _images = new List<IBrowserFile>();
        private FileUploadModel imageUploadModel;




        public void Cancel()
        {
              MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            _processing = true;
            var generatedImageName = await UploadImage();
            var fullImagePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.PagesFiles.ToString(), generatedImageName);

            if (PageModel.Id == 0)
            {
                PageModel.Image = !String.IsNullOrEmpty(generatedImageName) ? fullImagePath : "";
            }
            else
            {
                if (!string.IsNullOrEmpty(generatedImageName))
                    PageModel.Image = fullImagePath;
            }

            var content = HelperMethods.ToJson(PageModel);
            HttpResponseMessage response;
            if (PageModel.Id == 0)
            {
                response = await _httpClient.PostAsync(EndPoints.Pages, content);
            }
            else
            {
                response = await _httpClient.PutAsync($"{EndPoints.Pages}/{PageModel.Id}", content);
            }
            if (response.IsSuccessStatusCode)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
                MudDialog.Close();
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }
            _processing = false;
        }

        private async void SelectImage(InputFileChangeEventArgs e)
        {
            _images.Clear();
            _images.Add(e.File);

            if (_images.Count > 0)
            {
                imageUploadModel = await HelperMethods.Save(e.File);
                this.StateHasChanged();
            }
        }

        private async Task<string> UploadImage()
        {
            if (_images.Count > 0)
            {
                using var content = new MultipartFormDataContent();
                content.Add
                (content: imageUploadModel.Content, name: "\"file\"", fileName: imageUploadModel.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.PagesFiles}/{(int)Enums.UploadFileTypeEnum.Image}", content);
                if (response.IsSuccessStatusCode)
                {
                    return await response.getMessage();
                }
                else
                {
                    return String.Empty;
                }
            }
            return String.Empty;
        }

    }
}

