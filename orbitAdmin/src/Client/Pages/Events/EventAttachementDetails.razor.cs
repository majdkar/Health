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
    public partial class EventAttachementDetails
    {
        [Parameter]
        public string Id { get; set; }
        private EventUpdateModel EventModel { get; set; } = new();
        private IList<IBrowserFile> _eventAttachements = new List<IBrowserFile>();
        public List<EventAttachementUpdateModel> EventAttachementUpdateModelList = new List<EventAttachementUpdateModel>();

        private List<FileUploadModel> eventAttachementUploadModelList = new List<FileUploadModel>();

        private bool disableDeleteEventAttachement { get; set; } = true;


        protected async override Task OnInitializedAsync()
        {

        }

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
                    EventAttachementUpdateModelList.Clear();
                    foreach (var selectedEventAttachement in EventModel.EventAttachements)
                    {
                        EventAttachementUpdateModelList.Add(
                                                    new EventAttachementUpdateModel
                                                    {
                                                        Id = selectedEventAttachement.Id,
                                                        File = selectedEventAttachement.File,
                                                        Name = selectedEventAttachement.Name,
                                                        EventId = selectedEventAttachement.EventId,

                                                    }
                            );
                        if (!String.IsNullOrEmpty(selectedEventAttachement.File))
                        {
                           
                            disableDeleteEventAttachement = false;
                        }


                    }

                    this.StateHasChanged();
                }
            }
        }
        private void EventsAsync()
        {
            _navigationManager.NavigateTo($"/events");
        }

        private async Task SaveAsync()
        {
            bool error = false;

            foreach(var fileUploadModel in eventAttachementUploadModelList)
            {
                var generatedFileName = await UploadFile(_eventAttachements, fileUploadModel, (int)Enums.UploadFileTypeEnum.File);
                var fullFilePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.EventsFiles.ToString(), generatedFileName);
                var EventAttachementInsertModel = new EventAttachementInsertModel()
                {
                    EventId = int.Parse(Id),
                    File = !String.IsNullOrEmpty(generatedFileName) ? fullFilePath : "",
                    Name = fileUploadModel.Name,
                };
                var content = HelperMethods.ToJson(EventAttachementInsertModel);
                HttpResponseMessage response;
                response = await _httpClient.PostAsync(EndPoints.EventsAttachement, content);
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


        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            _eventAttachements.Clear();
            foreach (var file in e.GetMultipleFiles())
            {
                this._eventAttachements.Add(file);

                if (_eventAttachements.Count > 0)
                {
                    var eventAttachementUploadModel = await HelperMethods.Save(file);
                    eventAttachementUploadModelList.Add(eventAttachementUploadModel);
                    disableDeleteEventAttachement = false;
                    
                }


            }
            await SaveAsync();
            _eventAttachements.Clear();
            eventAttachementUploadModelList.Clear();
            await LoadEvent(Id);
            this.StateHasChanged();
            // _navigationManager.NavigateTo($"/event-Attachement-details/{Id}",true);
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

        private async void DeleteAllFiles()
        {
            _eventAttachements.Clear();
            
            

            disableDeleteEventAttachement = true;
            bool error = false;
            foreach (var deleventAttachement in EventAttachementUpdateModelList)
            {
                var result = await _httpClient.DeleteAsync($"{EndPoints.EventsAttachement}/{deleventAttachement.Id}");
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
                eventAttachementUploadModelList.Clear();
                EventAttachementUpdateModelList.Clear();
                _snackBar.Add("Deleted Successful!", Severity.Success);
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }

            this.StateHasChanged();
        }

        private async void DeleteFile(EventAttachementUpdateModel fileForPreview)
        {

            var deleventAttachement = EventAttachementUpdateModelList.Find(x => x.Id == fileForPreview.Id);
            EventAttachementUpdateModelList.Remove(deleventAttachement);
            var result = await _httpClient.DeleteAsync($"{EndPoints.EventsAttachement}/{deleventAttachement.Id}");
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