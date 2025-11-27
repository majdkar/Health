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
    public partial class EventPhotoDetails
    {
        [Parameter]
        public string Id { get; set; }
        private EventUpdateModel EventModel { get; set; } = new();
        private IList<IBrowserFile> _eventphotos = new List<IBrowserFile>();
        public List<EventPhotoUpdateModel> EventPhotoUpdateModelList = new List<EventPhotoUpdateModel>();

        private List<FileUploadModel> eventphotoUploadModelList = new List<FileUploadModel>();

        private List<string> eventphotoUrlForPreviewList { get; set; } = new();
        private string noImageUrl = Constants.NOImagePath;
        private bool disableDeleteEventPhoto { get; set; } = true;



        protected override async Task OnParametersSetAsync()
        {
            Id = Int32.TryParse(Id, out var idInteger) ? Id : "0";
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
                    EventModel = response;
                    EventPhotoUpdateModelList.Clear();
                    foreach (var selectedEventPhoto in EventModel.EventPhotos)
                    {
                        EventPhotoUpdateModelList.Add(
                                                    new EventPhotoUpdateModel
                                                    {
                                                        Id = selectedEventPhoto.Id,
                                                        Image = selectedEventPhoto.Image,
                                                        EventId = selectedEventPhoto.EventId,

                                                    }
                            );
                        if (!String.IsNullOrEmpty(selectedEventPhoto.Image))
                        {
                            eventphotoUrlForPreviewList.Add( selectedEventPhoto.Image);
                            disableDeleteEventPhoto = false;
                        }


                    }

                    this.StateHasChanged();
                }
            }
        }
        private async Task EventsAsync()
        {
            _navigationManager.NavigateTo($"/events");
        }

        private async Task SaveAsync()
        {
            bool error = false;

            foreach(var imageUploadModel in eventphotoUploadModelList)
            {
                var generatedImageName = await UploadFile(_eventphotos, imageUploadModel, (int)Enums.UploadFileTypeEnum.Image);
                var fullImagePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.EventsFiles.ToString(), generatedImageName);
                var EventPhotoInsertModel = new EventPhotoInsertModel()
                {
                    EventId = int.Parse(Id),
                    Image = !String.IsNullOrEmpty(generatedImageName) ? fullImagePath : "",
                };
                var content = HelperMethods.ToJson(EventPhotoInsertModel);
                HttpResponseMessage response;
                response = await _httpClient.PostAsync(EndPoints.EventsPhoto, content);
                if (response.IsSuccessStatusCode)
                {
                    _snackBar.Add("Completed Successful!", Severity.Success);
                }
                else
                {
                    _snackBar.Add("Something went wrong!", Severity.Error);
                    error = true;
                }

            }
            if (!error)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
               // _navigationManager.NavigateTo("/events");
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
            //_eventphotos.Clear();
            //eventphotoUploadModelList.Clear();
            _eventphotos.Add(e1.File);

            if (_eventphotos.Count > 0)
            {
                var eventphotoUploadModel = await HelperMethods.Save(e1.File);
                eventphotoUploadModelList.Add(eventphotoUploadModel);
                eventphotoUrlForPreviewList.Add(eventphotoUploadModel.Url);
                disableDeleteEventPhoto = false;
                this.StateHasChanged();
            }
        }


        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            _eventphotos.Clear();
            foreach (var file in e.GetMultipleFiles())
            {
                this._eventphotos.Add(file);

                if (_eventphotos.Count > 0)
                {
                    var eventphotoUploadModel = await HelperMethods.Save(file);
                    eventphotoUploadModelList.Add(eventphotoUploadModel);
                    eventphotoUrlForPreviewList.Add(eventphotoUploadModel.Url);
                    disableDeleteEventPhoto = false;
                    
                }


            }
            await SaveAsync();
            _eventphotos.Clear();
            eventphotoUrlForPreviewList.Clear();
            eventphotoUploadModelList.Clear();
            await LoadEvent(Id);
            this.StateHasChanged();
            // _navigationManager.NavigateTo($"/event-photo-details/{Id}",true);
            //TODO upload the files to the server
        }


        private async Task<string> UploadFile(IList<IBrowserFile> files, FileUploadModel fileModel, int uploadFileType)
        {
            if (files.Count > 0)
            {
                var content = new MultipartFormDataContent();
                content.Add
                (content: fileModel.Content, name: "\"file\"", fileName: fileModel.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.EventsFiles}/{uploadFileType}", content);
                if (response.IsSuccessStatusCode)
                {
                    return await response.getMessage();
                }
                else
                {
                    string messag = await response.getMessage();
                    _snackBar.Add(messag, Severity.Warning);
                }
            }
            return String.Empty;
        }

        private async void DeleteAllImages()
        {
            _eventphotos.Clear();
            
            eventphotoUrlForPreviewList.Clear();
            eventphotoUploadModelList.Clear();
            disableDeleteEventPhoto = true;
            bool error = false;
            foreach (var deleventphoto in EventPhotoUpdateModelList)
            {
                var result = await _httpClient.DeleteAsync($"{EndPoints.EventsPhoto}/{deleventphoto.Id}");
                if (result.IsSuccessStatusCode)
                {
                    _snackBar.Add("Complete Successful!", Severity.Success);
                }
                else
                {
                    _snackBar.Add("Something went wrong!", Severity.Error);
                    error = true;
                }

            }
            if (!error)
            {
                _snackBar.Add("Deleted Successful!", Severity.Success);
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }

            this.StateHasChanged();
        }

        private async void DeleteImage(string imageUrlForPreview)
        {

            eventphotoUrlForPreviewList.Remove(imageUrlForPreview);
            var deleventphoto = EventPhotoUpdateModelList.Find(x => x.Image == imageUrlForPreview);
            var result = await _httpClient.DeleteAsync($"{EndPoints.EventsPhoto}/{deleventphoto.Id}");
            if (result.IsSuccessStatusCode)
            {
                _snackBar.Add("Complete Successful!", Severity.Success);
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }

            this.StateHasChanged();
        }


    }
}