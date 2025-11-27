using SchoolV01.Client.Extensions;
using SchoolV01.Client.Helpers;
using SchoolV01.Shared;
using SchoolV01.Shared.ViewModels.Blocks;
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

namespace SchoolV01.Client.Pages.Blocks
{
    public partial class BlockPhotoDetails
    {
        [Parameter]
        public int Id { get; set; }
        private BlockUpdateModel BlockModel { get; set; } = new();
        private IList<IBrowserFile> _blockphotos = [];
        public List<BlockPhotoUpdateModel> BlockPhotoUpdateModelList = [];

        private List<FileUploadModel> blockphotoUploadModelList = [];
        private string Title;
        private int CategoryId=0;
        private List<string> blockphotoUrlForPreviewList { get; set; } = [];
        private string noImageUrl = Constants.NOImagePath;
        private bool disableDeleteBlockPhoto { get; set; } = true;


        protected async override Task OnInitializedAsync()
        {

        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadBlock(Id);
        }

        private async Task LoadBlock(int id)
        {
       
                var requestUri = EndPoints.Blocks + $"/{id}";
                var response = await _httpClient.GetFromJsonAsync<BlockUpdateModel>(requestUri);
                if (response != null)
                {
                    BlockModel = response;
                    Title = BlockModel.NameAr;
                    CategoryId = BlockModel.CategoryId;
                    BlockPhotoUpdateModelList.Clear();
                    foreach (var selectedBlockPhoto in BlockModel.BlockPhotos)
                    {
                        BlockPhotoUpdateModelList.Add(
                                                    new BlockPhotoUpdateModel
                                                    {
                                                        Id = selectedBlockPhoto.Id,
                                                        Image = selectedBlockPhoto.Image,
                                                        BlockId = selectedBlockPhoto.BlockId,

                                                    }
                            );
                        if (!String.IsNullOrEmpty(selectedBlockPhoto.Image))
                        {
                            blockphotoUrlForPreviewList.Add( selectedBlockPhoto.Image);
                            disableDeleteBlockPhoto = false;
                        }


                    }

                    this.StateHasChanged();
                }
        }
        private async Task BlocksAsync()
        {
            if (CategoryId != 0)
                _navigationManager.NavigateTo($"/blocks/{CategoryId}");
            else
                _navigationManager.NavigateTo("/blocks");
        }

        private async Task SaveAsync()
        {
            bool error = false;

            foreach(var imageUploadModel in blockphotoUploadModelList)
            {
                var generatedImageName = await UploadFile(_blockphotos, imageUploadModel, (int)Enums.UploadFileTypeEnum.Image);
                var fullImagePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.BlocksFiles.ToString(), generatedImageName);
                var BlockPhotoInsertModel = new BlockPhotoInsertModel()
                {
                    BlockId = Id,
                    Image = !String.IsNullOrEmpty(generatedImageName) ? fullImagePath : "",
                };
                var content = HelperMethods.ToJson(BlockPhotoInsertModel);
                HttpResponseMessage response;
                response = await _httpClient.PostAsync(EndPoints.BlocksPhoto, content);
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
               // _navigationManager.NavigateTo("/blocks");
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }



        }

        public void Cancel()
        {
            if (CategoryId!=0)
                _navigationManager.NavigateTo($"/blocks/{CategoryId}");
            else
                _navigationManager.NavigateTo("/blocks");
        }

        private async void SelectImage(InputFileChangeEventArgs e1)
        {
            //_blockphotos.Clear();
            //blockphotoUploadModelList.Clear();
            _blockphotos.Add(e1.File);

            if (_blockphotos.Count > 0)
            {
                var blockphotoUploadModel = await HelperMethods.Save(e1.File);
                blockphotoUploadModelList.Add(blockphotoUploadModel);
                blockphotoUrlForPreviewList.Add(blockphotoUploadModel.Url);
                disableDeleteBlockPhoto = false;
                this.StateHasChanged();
            }
        }


        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            _blockphotos.Clear();
            foreach (var file in e.GetMultipleFiles())
            {
                this._blockphotos.Add(file);

                if (_blockphotos.Count > 0)
                {
                    var blockphotoUploadModel = await HelperMethods.Save(file);
                    blockphotoUploadModelList.Add(blockphotoUploadModel);
                    blockphotoUrlForPreviewList.Add(blockphotoUploadModel.Url);
                    disableDeleteBlockPhoto = false;
                    
                }


            }
            await SaveAsync();
            _blockphotos.Clear();
            blockphotoUrlForPreviewList.Clear();
            blockphotoUploadModelList.Clear();
            await LoadBlock(Id);
            this.StateHasChanged();
            // _navigationManager.NavigateTo($"/block-photo-details/{Id}",true);
            //TODO upload the files to the server
        }


        private async Task<string> UploadFile(IList<IBrowserFile> files, FileUploadModel fileModel, int uploadFileType)
        {
            if (files.Count > 0)
            {
                var content = new MultipartFormDataContent();
                content.Add
                (content: fileModel.Content, name: "\"file\"", fileName: fileModel.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.BlocksFiles}/{uploadFileType}", content);
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
            _blockphotos.Clear();
            
            blockphotoUrlForPreviewList.Clear();
            blockphotoUploadModelList.Clear();
            disableDeleteBlockPhoto = true;
            bool error = false;
            foreach (var delblockphoto in BlockPhotoUpdateModelList)
            {
                var result = await _httpClient.DeleteAsync($"{EndPoints.BlocksPhoto}/{delblockphoto.Id}");
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

            blockphotoUrlForPreviewList.Remove(imageUrlForPreview);
            var delblockphoto = BlockPhotoUpdateModelList.Find(x => x.Image == imageUrlForPreview);
            var result = await _httpClient.DeleteAsync($"{EndPoints.BlocksPhoto}/{delblockphoto.Id}");
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