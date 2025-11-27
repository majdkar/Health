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

using SchoolV01.Client.Shared.Components;
using System.Linq;
using SchoolV01.Shared.ViewModels.Menus;
using SchoolV01.Shared.Wrapper;
using System.Security.Claims;

namespace SchoolV01.Client.Pages.Blocks
{
    public partial class BlockDetails
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public int CategoryId { get; set; } = 0;
        private int NewsId { get; set; } = 0;
        private int EventsId { get; set; } = 0;

        private BlockUpdateModel BlockModel { get; set; } = new();
        private IEnumerable<BlockCategoryViewModel> categories = new List<BlockCategoryViewModel>();
        private IEnumerable<BlockViewModel> _parentBlocks;

        private IList<IBrowserFile> _images = new List<IBrowserFile>();
        private IList<IBrowserFile> _images1 = new List<IBrowserFile>();
        private IList<IBrowserFile> _images2 = new List<IBrowserFile>();
        private IList<IBrowserFile> _images3 = new List<IBrowserFile>();

        private IList<IBrowserFile> _files = new List<IBrowserFile>();
        private ClaimsPrincipal _currentUser;

        public int parentId { get; set; } = 0;

        private FileUploadModel fileUploadModel;
        private FileUploadModel imageUploadModel;
        private FileUploadModel imageUploadModel1;
        private FileUploadModel imageUploadModel2;
        private FileUploadModel imageUploadModel3;


        private LanguageSelector languageSelector { get; set; }
        private string imageUrlForPreview { get; set; } = "";
        private string imageUrlForPreview1 { get; set; } = "";
        private string imageUrlForPreview2 { get; set; } = "";
        private string imageUrlForPreview3 { get; set; } = "";
        private string noImageUrl = Constants.NOImagePath;
        private bool disableDeleteImageButton { get; set; } = true;
        private bool disableDeleteImageButton1 { get; set; } = true;
        private bool disableDeleteImageButton2 { get; set; } = true;
        private bool disableDeleteImageButton3 { get; set; } = true;
        private bool disableDeleteFileButton { get; set; } = true;
        private bool _processing = false;

        readonly TextEditorConfig editorAr = new("#editorAr");
        readonly TextEditorConfig editorEn = new("#editorEn");
        readonly TextEditorConfig editorSe = new("#editorSe");
        readonly TextEditorConfig editorAr1 = new("#editorAr1");
        readonly TextEditorConfig editorEn1 = new("#editorEn1");
        readonly TextEditorConfig editorSe1 = new("#editorSe1");
        readonly TextEditorConfig editorAr2 = new("#editorAr2");
        readonly TextEditorConfig editorEn2 = new("#editorEn2");
        readonly TextEditorConfig editorSe2 = new("#editorSe2");
        readonly TextEditorConfig editorAr3 = new("#editorAr3");
        readonly TextEditorConfig editorEn3 = new("#editorEn3");
        readonly TextEditorConfig editorSe3 = new("#editorSe3");
        private bool _isAdmin;

        protected async override Task OnInitializedAsync()
        {
            await LoadCategories();
            await LoadBlocks();
            BlockModel.CategoryId = CategoryId;
            parentId = BlockModel.ParentId.HasValue ? BlockModel.ParentId.Value : 0;
            _currentUser = await _authenticationManager.CurrentUser();
            _isAdmin = _currentUser.IsInRole("Administrator");
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


                    if (!String.IsNullOrEmpty(response.Image))
                    {
                        imageUrlForPreview = response.Image;
                        disableDeleteImageButton = false;
                    }
                    if (!String.IsNullOrEmpty(response.Image1))
                    {
                        imageUrlForPreview = response.Image1;
                        disableDeleteImageButton = false;
                    }
                    if (!String.IsNullOrEmpty(response.Image2))
                    {
                        imageUrlForPreview2 = response.Image2;
                        disableDeleteImageButton2 = false;
                    }
                    if (!String.IsNullOrEmpty(response.Image3))
                    {
                        imageUrlForPreview3 = response.Image3;
                        disableDeleteImageButton3 = false;
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
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<BlockCategoryViewModel>>(EndPoints.BlockCategoriesSelect);
            if (response != null)
            {
                categories = response;
                NewsId = categories.Where(c => c.NameAr.Contains("الأخبار") || c.NameEn.Contains("News")).FirstOrDefault()?.Id ?? 0;
                EventsId = categories.Where(c => c.NameAr == "الفعاليات" || c.NameEn == "Events").FirstOrDefault()?.Id ?? 0;
            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
        }


        private async Task LoadBlocks()
        {
            PagedResponse<BlockViewModel> response = null;
            //HttpResponseMessage response = null;
            try
            {

                var requestUri = EndPoints.GetAll(EndPoints.Blocks, "", null) + $"?categoryId={BlockModel.CategoryId}";

                response = await _httpClient.GetFromJsonAsync<PagedResponse<BlockViewModel>>(requestUri);

                if (response != null)
                {
                    _parentBlocks = response.Items.Where(x => x.Id != BlockModel.Id && x.ParentId == null);
                }
                else
                {
                    _snackBar.Add("Error retrieving data");
                }

                //response = (List<MenuViewModel>)await _httpClient.GetFromJsonAsync<List<MenuViewModel>>(EndPoints.MenuSelect);
                //response = await _httpClient.GetAsync(EndPoints.MenuSelect);

            }
            catch (Exception)
            {
                _snackBar.Add("Error retrieving data: ");
            }
            finally
            {

            }


        }

        private async Task SaveAsync()
        {

            _processing = true;

            if (parentId == 0)
                BlockModel.ParentId = null;
            else
                BlockModel.ParentId = parentId;



            var generatedImageName = await UploadFile(_images, imageUploadModel, (int)Enums.UploadFileTypeEnum.Image);
            var fullImagePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.BlocksFiles.ToString(), generatedImageName);
            var generatedImageName1 = await UploadFile(_images1, imageUploadModel1, (int)Enums.UploadFileTypeEnum.Image);
            var fullImagePath1 = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.BlocksFiles.ToString(), generatedImageName1);

            var generatedImageName2 = await UploadFile(_images2, imageUploadModel2, (int)Enums.UploadFileTypeEnum.Image);
            var fullImagePath2 = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.BlocksFiles.ToString(), generatedImageName2);

            var generatedImageName3 = await UploadFile(_images3, imageUploadModel3, (int)Enums.UploadFileTypeEnum.Image);
            var fullImagePath3 = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.BlocksFiles.ToString(), generatedImageName3);

            var generatedFileName = await UploadFile(_files, fileUploadModel, (int)Enums.UploadFileTypeEnum.File);
            var fullFilePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.BlocksFiles.ToString(), generatedFileName);


            if (BlockModel.Id == 0)
            {
                BlockModel.Image = !String.IsNullOrEmpty(generatedImageName) ? fullImagePath : "";
                BlockModel.Image1 = !String.IsNullOrEmpty(generatedImageName1) ? fullImagePath1 : "";
                BlockModel.Image2 = !String.IsNullOrEmpty(generatedImageName2) ? fullImagePath2 : "";
                BlockModel.Image3 = !String.IsNullOrEmpty(generatedImageName3) ? fullImagePath3 : "";
                BlockModel.File = !String.IsNullOrEmpty(generatedFileName) ? fullFilePath : "";
            }
            else
            {
                if (!String.IsNullOrEmpty(generatedImageName))
                    BlockModel.Image = fullImagePath;
                if (!String.IsNullOrEmpty(generatedImageName1))
                    BlockModel.Image1 = fullImagePath1;
                if (!String.IsNullOrEmpty(generatedImageName2))
                    BlockModel.Image2 = fullImagePath2;
                if (!String.IsNullOrEmpty(generatedImageName3))
                    BlockModel.Image3 = fullImagePath3;
                if (!String.IsNullOrEmpty(generatedFileName))
                    BlockModel.File = fullFilePath;
            }

            var content = HelperMethods.ToJson(BlockModel);
            HttpResponseMessage response;
            if (BlockModel.Id == 0)
            {
                response = await _httpClient.PostAsync(EndPoints.Blocks, content);
            }
            else
            {
                response = await _httpClient.PutAsync($"{EndPoints.Blocks}/{BlockModel.Id}", content);
            }
            if (response.IsSuccessStatusCode)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
                _navigationManager.NavigateTo($"/blocks/{BlockModel.CategoryId}");
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }
            _processing = false;
        }

        public void Cancel()
        {
            _navigationManager.NavigateTo($"/blocks/{BlockModel.CategoryId}");
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
        private async void SelectImage1(InputFileChangeEventArgs e1)
        {
            _images1.Clear();
            _images1.Add(e1.File);

            if (_images1.Count > 0)
            {
                imageUploadModel1 = await HelperMethods.Save(e1.File);
                imageUrlForPreview1 = imageUploadModel1.Url;
                disableDeleteImageButton1 = false;
                this.StateHasChanged();
            }
        }

        private async void SelectImage2(InputFileChangeEventArgs e1)
        {
            _images2.Clear();
            _images2.Add(e1.File);

            if (_images2.Count > 0)
            {
                imageUploadModel2 = await HelperMethods.Save(e1.File);
                imageUrlForPreview2 = imageUploadModel2.Url;
                disableDeleteImageButton2 = false;
                this.StateHasChanged();
            }
        }

        private async void SelectImage3(InputFileChangeEventArgs e1)
        {
            _images3.Clear();
            _images3.Add(e1.File);

            if (_images3.Count > 0)
            {
                imageUploadModel3 = await HelperMethods.Save(e1.File);
                imageUrlForPreview3 = imageUploadModel3.Url;
                disableDeleteImageButton3 = false;
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

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.BlocksFiles}/{uploadFileType}", content);
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
            BlockModel.Image = "";
            imageUrlForPreview = "";
            disableDeleteImageButton = true;
            this.StateHasChanged();
        }
        private void DeleteImage1()
        {
            _images1.Clear();
            BlockModel.Image1 = "";
            imageUrlForPreview1 = "";
            disableDeleteImageButton1 = true;
            this.StateHasChanged();
        }

        private void DeleteImage2()
        {
            _images2.Clear();
            BlockModel.Image2 = "";
            imageUrlForPreview2 = "";
            disableDeleteImageButton2 = true;
            this.StateHasChanged();
        }

        private void DeleteImage3()
        {
            _images3.Clear();
            BlockModel.Image3 = "";
            imageUrlForPreview3 = "";
            disableDeleteImageButton3 = true;
            this.StateHasChanged();
        }
        private void DeleteFile()
        {
            _files.Clear();
            BlockModel.File = "";
            disableDeleteFileButton = true;
            this.StateHasChanged();
        }



    }
}