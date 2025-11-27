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
    public partial class BlockAttachementDetails
    {
        [Parameter]
        public string Id { get; set; }
        private BlockUpdateModel BlockModel { get; set; } = new();
        private IList<IBrowserFile> _blockAttachements = new List<IBrowserFile>();
        public List<BlockAttachementUpdateModel> BlockAttachementUpdateModelList = new List<BlockAttachementUpdateModel>();

        private List<FileUploadModel> blockAttachementUploadModelList = new List<FileUploadModel>();

        private bool disableDeleteBlockAttachement { get; set; } = true;

       
        protected async override Task OnInitializedAsync()
        {

        }

        protected override async Task OnParametersSetAsync()
        {
            Id = Int32.TryParse(Id, out var idInteger) ? Id : "0";
            await LoadBlock(Id);
        }

        private async Task LoadBlock(string id)
        {
            if (id != "0")
            {
                var requestUri = EndPoints.Blocks + $"/{id}";
                var response = await _httpClient.GetFromJsonAsync<BlockUpdateModel>(requestUri);
                if (response != null)
                {
                    BlockModel = response;
                    BlockAttachementUpdateModelList.Clear();
                    foreach (var selectedBlockAttachement in BlockModel.BlockAttachements)
                    {
                        BlockAttachementUpdateModelList.Add(
                                                    new BlockAttachementUpdateModel
                                                    {
                                                        Id = selectedBlockAttachement.Id,
                                                        File = selectedBlockAttachement.File,
                                                        Name = selectedBlockAttachement.Name,
                                                        BlockId = selectedBlockAttachement.BlockId,

                                                    }
                            );
                        if (!String.IsNullOrEmpty(selectedBlockAttachement.File))
                        {
                           
                            disableDeleteBlockAttachement = false;
                        }


                    }

                    this.StateHasChanged();
                }
            }
        }
        private void BlocksAsync()
        {
            _navigationManager.NavigateTo($"/blocks/{BlockModel.CategoryId}");
            //_navigationManager.NavigateTo($"/blocks");
        }

        private async Task SaveAsync()
        {
            bool error = false;

            foreach(var fileUploadModel in blockAttachementUploadModelList)
            {
                var generatedFileName = await UploadFile(_blockAttachements, fileUploadModel, (int)Enums.UploadFileTypeEnum.File);
                var fullFilePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.BlocksFiles.ToString(), generatedFileName);
                var BlockAttachementInsertModel = new BlockAttachementInsertModel()
                {
                    BlockId = int.Parse(Id),
                    File = !String.IsNullOrEmpty(generatedFileName) ? fullFilePath : "",
                    Name = fileUploadModel.Name,
                };
                var content = HelperMethods.ToJson(BlockAttachementInsertModel);
                HttpResponseMessage response;
                response = await _httpClient.PostAsync(EndPoints.BlocksAttachement, content);
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
            _navigationManager.NavigateTo("/blocks");
        }


        private async void UploadFiles(InputFileChangeEventArgs e)
        {
            _blockAttachements.Clear();
            foreach (var file in e.GetMultipleFiles())
            {
                this._blockAttachements.Add(file);

                if (_blockAttachements.Count > 0)
                {
                    var blockAttachementUploadModel = await HelperMethods.Save(file);
                    blockAttachementUploadModelList.Add(blockAttachementUploadModel);
                    disableDeleteBlockAttachement = false;
                    
                }


            }
            await SaveAsync();
            _blockAttachements.Clear();
            blockAttachementUploadModelList.Clear();
            await LoadBlock(Id);
            this.StateHasChanged();
            // _navigationManager.NavigateTo($"/block-Attachement-details/{Id}",true);
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

        private async void DeleteAllFiles()
        {
            _blockAttachements.Clear();
            
            

            disableDeleteBlockAttachement = true;
            bool error = false;
            foreach (var delblockAttachement in BlockAttachementUpdateModelList)
            {
                var result = await _httpClient.DeleteAsync($"{EndPoints.BlocksAttachement}/{delblockAttachement.Id}");
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
                blockAttachementUploadModelList.Clear();
                BlockAttachementUpdateModelList.Clear();
                _snackBar.Add("Deleted Successful!", Severity.Success);
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }

            this.StateHasChanged();
        }

        private async void DeleteFile(BlockAttachementUpdateModel fileForPreview)
        {

            var delblockAttachement = BlockAttachementUpdateModelList.Find(x => x.Id == fileForPreview.Id);
            BlockAttachementUpdateModelList.Remove(delblockAttachement);
            var result = await _httpClient.DeleteAsync($"{EndPoints.BlocksAttachement}/{delblockAttachement.Id}");
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