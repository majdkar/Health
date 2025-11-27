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
using System.Net.Http.Json;
using System.Threading.Tasks;
using SchoolV01.Shared.Constants;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Shared.Components;

namespace SchoolV01.Client.Pages.Pages
{
    public partial class PageAttachementDetails
    {
        [Parameter]
        public string Id { get; set; }
        private PageUpdateModel PageModel { get; set; } = new();
        private IList<IBrowserFile> _pageAttachements = new List<IBrowserFile>();
        public List<PageAttachementUpdateModel> PageAttachementUpdateModelList = new List<PageAttachementUpdateModel>();

        private List<FileUploadModel> pageAttachementUploadModelList = new List<FileUploadModel>();

        private bool disableDeletePageAttachement { get; set; } = true;


        protected async override Task OnInitializedAsync()
        {

        }

        protected override async Task OnParametersSetAsync()
        {
            Id = Int32.TryParse(Id, out var idInteger) ? Id : "0";
            await LoadPage(Id);
        }

        private async Task LoadPage(string id)
        {
            if (id != "0")
            {
                var requestUri = EndPoints.Pages + $"/{id}";
                var response = await _httpClient.GetFromJsonAsync<PageUpdateModel>(requestUri);
                if (response != null)
                {
                    PageModel = response;
                    PageAttachementUpdateModelList.Clear();
                    foreach (var selectedPageAttachement in PageModel.PageAttachements)
                    {
                        PageAttachementUpdateModelList.Add(
                                                    new PageAttachementUpdateModel
                                                    {
                                                        Id = selectedPageAttachement.Id,
                                                        File = selectedPageAttachement.File,
                                                        Name = selectedPageAttachement.Name,
                                                        PageId = selectedPageAttachement.PageId,

                                                    }
                            );
                        if (!String.IsNullOrEmpty(selectedPageAttachement.File))
                        {
                           
                            disableDeletePageAttachement = false;
                        }


                    }

                    this.StateHasChanged();
                }
            }
        }
        private void PagesAsync()
        {
            _navigationManager.NavigateTo($"/pages");
        }

        private async Task SaveAsync()
        {
            bool error = false;

            foreach(var fileUploadModel in pageAttachementUploadModelList)
            {
                var generatedFileName = await UploadFile(_pageAttachements, fileUploadModel, (int)Enums.UploadFileTypeEnum.File);
                var fullFilePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.PagesFiles.ToString(), generatedFileName);
                var PageAttachementInsertModel = new PageAttachementInsertModel()
                {
                    PageId = int.Parse(Id),
                    File = !String.IsNullOrEmpty(generatedFileName) ? fullFilePath : "",
                    Name = fileUploadModel.Name,
                };
                var content = HelperMethods.ToJson(PageAttachementInsertModel);
                HttpResponseMessage response;
                response = await _httpClient.PostAsync(EndPoints.PagesAttachement, content);
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
               // _navigationManager.NavigateTo("/pages");
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }



        }

        public void Cancel()
        {
            _navigationManager.NavigateTo("/pages");
        }


        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            _pageAttachements.Clear();
            foreach (var file in e.GetMultipleFiles())
            {
                this._pageAttachements.Add(file);

                if (_pageAttachements.Count > 0)
                {
                    var pageAttachementUploadModel = await HelperMethods.Save(file);
                    pageAttachementUploadModelList.Add(pageAttachementUploadModel);
                    disableDeletePageAttachement = false;
                    
                }


            }
            await SaveAsync();
            _pageAttachements.Clear();
            pageAttachementUploadModelList.Clear();
            await LoadPage(Id);
            this.StateHasChanged();
            // _navigationManager.NavigateTo($"/page-Attachement-details/{Id}",true);
            //TODO upload the files to the server
        }


        private async Task<string> UploadFile(IList<IBrowserFile> files, FileUploadModel fileModel, int uploadFileType)
        {
            if (files.Count > 0)
            {
                var content = new MultipartFormDataContent();
                content.Add
                (content: fileModel.Content, name: "\"file\"", fileName: fileModel.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.PagesFiles}/{uploadFileType}", content);
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
            _pageAttachements.Clear();
            
            

            disableDeletePageAttachement = true;
            bool error = false;
            foreach (var delpageAttachement in PageAttachementUpdateModelList)
            {
                var result = await _httpClient.DeleteAsync($"{EndPoints.PagesAttachement}/{delpageAttachement.Id}");
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
                pageAttachementUploadModelList.Clear();
                PageAttachementUpdateModelList.Clear();
                _snackBar.Add("Deleted Successful!", Severity.Success);
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }

            this.StateHasChanged();
        }

        private async void DeleteFile(PageAttachementUpdateModel fileForPreview)
        {

            var delpageAttachement = PageAttachementUpdateModelList.Find(x => x.Id == fileForPreview.Id);
            PageAttachementUpdateModelList.Remove(delpageAttachement);
            var result = await _httpClient.DeleteAsync($"{EndPoints.PagesAttachement}/{delpageAttachement.Id}");
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