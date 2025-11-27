using SchoolV01.Client.Extensions;
using SchoolV01.Client.Helpers;
using SchoolV01.Shared;
using SchoolV01.Shared.ViewModels.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SchoolV01.Shared.Constants;

namespace SchoolV01.Client.Pages.Events
{
    public partial class EventDetails
    {
        [Parameter]
        public string Id { get; set; }

        private EventUpdateModel eventModel { get; set; } = new();
        private IEnumerable<EventCategoryViewModel> categories;

        private IList<IBrowserFile> _images = new List<IBrowserFile>();
        private IList<IBrowserFile> _videos = new List<IBrowserFile>();
        private IList<IBrowserFile> _files = new List<IBrowserFile>();

        private FileUploadModel fileUploadModel;
        private FileUploadModel imageUploadModel;
        private FileUploadModel videoUploadModel;

        private DateTime? startDate { get; set; } = DateTime.Today;
        private DateTime? endDate { get; set; } = DateTime.Today;

        private string imageUrlForPreview { get; set; } = "";
        private string videoUrlForPreview { get; set; } = "";
        private bool videoisReadytobeuploaded { get; set; } = false;
        private string noImageUrl = Constants.NOImagePath;
        private string ready2Upload = Constants.ReadyToUploadPath;
        private bool disableDeleteImageButton { get; set; } = true;
        private bool disableDeleteVideoButton { get; set; } = true;
        private bool disableDeleteFileButton { get; set; } = true;
        

        protected async override Task OnInitializedAsync()
        {
            Id = Int32.TryParse(Id, out var _) ? Id : "0";
            await LoadCategories();
            
            await LoadEvent(Id);
        }

        protected override async Task OnParametersSetAsync()
        {
            Id = Int32.TryParse(Id, out var _) ? Id : "0";
            await LoadEvent(Id);
        }

        private async Task LoadEvent(string id)
        {
            if (id != "0")
            {
                var requestUri = EndPoints.Events + $"/{id}";
                var response = await _httpClient.GetFromJsonAsync<EventUpdateModel>(requestUri);
                if (response != null)
                {
                    eventModel = response;
                    if (!String.IsNullOrEmpty(response.Image))
                    {
                        imageUrlForPreview = response.Image;
                        disableDeleteImageButton = false;
                    }
                    if (!String.IsNullOrEmpty(response.Video))
                    {
                        videoUrlForPreview = response.Video;
                        disableDeleteVideoButton = false;
                    }
                    if (!String.IsNullOrEmpty(response.File))
                    {
                        disableDeleteFileButton = false;
                    }
                    this.StateHasChanged();
                }
            }
        }

        private async Task LoadCategories()
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<EventCategoryViewModel>>(EndPoints.EventCategoriesSelect);
            if (response != null)
            {
                categories = response;
            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
        }

        private async Task SaveAsync()
        {
            eventModel.StartDate = startDate ?? DateTime.Today;
            eventModel.EndDate = endDate ?? DateTime.Today;

            var generatedImageName = await UploadFile(_images, imageUploadModel, (int)Enums.UploadFileTypeEnum.Image);
            var fullImagePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.EventsFiles.ToString(), generatedImageName);


            var generatedVideoName = await UploadFile(_videos, videoUploadModel, (int)Enums.UploadFileTypeEnum.Video);
            var fullVideoPath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.EventsFiles.ToString(), generatedVideoName);

            var generatedFileName = await UploadFile(_files, fileUploadModel, (int)Enums.UploadFileTypeEnum.File);
            var fullFilePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.EventsFiles.ToString(), generatedFileName);
            
            if (eventModel.Id == 0)
            {
                eventModel.Image = !String.IsNullOrEmpty(generatedImageName) ? fullImagePath : "";
                eventModel.Video = !String.IsNullOrEmpty(generatedVideoName) ? fullImagePath : "";
                eventModel.File = !String.IsNullOrEmpty(generatedFileName) ? fullFilePath : "";
            }
            else
            {
                if (!String.IsNullOrEmpty(generatedImageName))
                    eventModel.Image = fullImagePath;
                if (!String.IsNullOrEmpty(generatedVideoName))
                    eventModel.Video = fullVideoPath;
                if (!String.IsNullOrEmpty(generatedFileName))
                    eventModel.File = fullFilePath;
            }

            var content = HelperMethods.ToJson(eventModel);
            HttpResponseMessage response;
            if (eventModel.Id == 0)
            {
                response = await _httpClient.PostAsync(EndPoints.Events, content);
            }
            else
            {
                response = await _httpClient.PutAsync($"{EndPoints.Events}/{eventModel.Id}", content);
            }
            if (response.IsSuccessStatusCode)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
                _navigationManager.NavigateTo("/Events");
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }

        }

        public void Cancel()
        {
            _navigationManager.NavigateTo("/events");
        }

        private async void SelectImage(InputFileChangeEventArgs e1)
        {
            _images.Clear();
            _images.Add(e1.File);

            if (_images.Count > 0)
            {
                imageUploadModel = await HelperMethods.Save(e1.File);
                imageUrlForPreview = imageUploadModel.Url;
                disableDeleteImageButton = false;
                this.StateHasChanged();
            }
        }

        private async void SelectVideo(InputFileChangeEventArgs e1)
        {
            _videos.Clear();
            _videos.Add(e1.File);

            if (_videos.Count > 0)
            {
                videoUploadModel = await HelperMethods.Save(e1.File);
                videoisReadytobeuploaded = true;
                videoUrlForPreview = "";// videoUploadModel.Url;
                disableDeleteVideoButton = false;
                this.StateHasChanged();
            }
        }

        private async void SelectFile(InputFileChangeEventArgs e2)
        {
            _files.Clear();
            _files.Add(e2.File);

            if (_files.Count > 0)
            {
                fileUploadModel = await HelperMethods.Save(e2.File);
                disableDeleteFileButton = false;
                this.StateHasChanged();
            }
        }

        private async Task<string> UploadFile(IList<IBrowserFile> files, FileUploadModel fileModel, int uploadFileType)
        {
            if (files.Count > 0)
            {
                using var content = new MultipartFormDataContent();
                content.Add
                (content: fileModel.Content, name: "\"file\"", fileName: fileModel.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.EventsFiles}/{uploadFileType}", content);
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

        private void DeleteImage()
        {
            _images.Clear();
            eventModel.Image = "";
            imageUrlForPreview = "";
            disableDeleteImageButton = true;
            this.StateHasChanged();
        }

        private void DeleteVideo()
        {
            _videos.Clear();
            eventModel.Video = "";
            videoUrlForPreview = "";
            videoisReadytobeuploaded = false;
            disableDeleteVideoButton = true;
            this.StateHasChanged();
        }

        private void DeleteFile()
        {
            _files.Clear();
            eventModel.File = "";
            disableDeleteFileButton = true;
            this.StateHasChanged();
        }

    }
}
