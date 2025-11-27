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
    public partial class PagePhotoDetails
    {
        [Parameter]
        public string Id { get; set; }
        private PageUpdateModel PageModel { get; set; } = new();
        private IList<IBrowserFile> _pagephotos = new List<IBrowserFile>();
        public List<PagePhotoUpdateModel> PagePhotoUpdateModelList = new List<PagePhotoUpdateModel>();

        private List<FileUploadModel> pagephotoUploadModelList = new List<FileUploadModel>();

        private List<string> pagephotoUrlForPreviewList { get; set; } = new();
        private string noImageUrl = Constants.NOImagePath;
        private bool disableDeletePagePhoto { get; set; } = true;


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
                    PagePhotoUpdateModelList.Clear();
                    foreach (var selectedPagePhoto in PageModel.PagePhotos)
                    {
                        PagePhotoUpdateModelList.Add(
                                                    new PagePhotoUpdateModel
                                                    {
                                                        Id = selectedPagePhoto.Id,
                                                        Image = selectedPagePhoto.Image,
                                                        PageId = selectedPagePhoto.PageId,

                                                    }
                            );
                        if (!String.IsNullOrEmpty(selectedPagePhoto.Image))
                        {
                            pagephotoUrlForPreviewList.Add( selectedPagePhoto.Image);
                            disableDeletePagePhoto = false;
                        }


                    }

                    this.StateHasChanged();
                }
            }
        }
        private async Task PagesAsync()
        {
            _navigationManager.NavigateTo($"/pages");
        }

        private async Task SaveAsync()
        {
            bool error = false;

            foreach(var imageUploadModel in pagephotoUploadModelList)
            {
                var generatedImageName = await UploadFile(_pagephotos, imageUploadModel, (int)Enums.UploadFileTypeEnum.Image);
                var fullImagePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.PagesFiles.ToString(), generatedImageName);
                var PagePhotoInsertModel = new PagePhotoInsertModel()
                {
                    PageId = int.Parse(Id),
                    Image = !String.IsNullOrEmpty(generatedImageName) ? fullImagePath : "",
                };
                var content = HelperMethods.ToJson(PagePhotoInsertModel);
                HttpResponseMessage response;
                response = await _httpClient.PostAsync(EndPoints.PagesPhoto, content);
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

        private async void SelectImage(InputFileChangeEventArgs e1)
        {
            //_pagephotos.Clear();
            //pagephotoUploadModelList.Clear();
            _pagephotos.Add(e1.File);

            if (_pagephotos.Count > 0)
            {
                var pagephotoUploadModel = await HelperMethods.Save(e1.File);
                pagephotoUploadModelList.Add(pagephotoUploadModel);
                pagephotoUrlForPreviewList.Add(pagephotoUploadModel.Url);
                disableDeletePagePhoto = false;
                this.StateHasChanged();
            }
        }


        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            _pagephotos.Clear();
            foreach (var file in e.GetMultipleFiles())
            {
                this._pagephotos.Add(file);

                if (_pagephotos.Count > 0)
                {
                    var pagephotoUploadModel = await HelperMethods.Save(file);
                    pagephotoUploadModelList.Add(pagephotoUploadModel);
                    pagephotoUrlForPreviewList.Add(pagephotoUploadModel.Url);
                    disableDeletePagePhoto = false;
                    
                }


            }
            await SaveAsync();
            _pagephotos.Clear();
            pagephotoUrlForPreviewList.Clear();
            pagephotoUploadModelList.Clear();
            await LoadPage(Id);
            this.StateHasChanged();
            // _navigationManager.NavigateTo($"/page-photo-details/{Id}",true);
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

        private async void DeleteAllImages()
        {
            _pagephotos.Clear();
            
            pagephotoUrlForPreviewList.Clear();
            pagephotoUploadModelList.Clear();
            disableDeletePagePhoto = true;
            bool error = false;
            foreach (var delpagephoto in PagePhotoUpdateModelList)
            {
                var result = await _httpClient.DeleteAsync($"{EndPoints.PagesPhoto}/{delpagephoto.Id}");
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

            pagephotoUrlForPreviewList.Remove(imageUrlForPreview);
            var delpagephoto = PagePhotoUpdateModelList.Find(x => x.Image == imageUrlForPreview);
            var result = await _httpClient.DeleteAsync($"{EndPoints.PagesPhoto}/{delpagephoto.Id}");
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