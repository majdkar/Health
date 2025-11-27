using SchoolV01.Client.Extensions;
using SchoolV01.Client.Helpers;
using SchoolV01.Shared;
using SchoolV01.Shared.ViewModels.Menus;
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
using SchoolV01.Shared.Wrapper;
using System.Linq;
using System.Security.Claims;
using Microsoft.JSInterop;

namespace SchoolV01.Client.Pages.Menus
{
    public partial class MenuDetails
    {
        [Parameter] public int Id { get; set; } = 0;
        [Parameter] public int? CategoryId { get; set; } = 0;
        public MenuUpdateModel MenuModel { get; set; } = new();

        private IEnumerable<MenuCategoryViewModel> categories;
        private IEnumerable<MenuViewModel> _menus;
        private IEnumerable<MenuViewModel> _parentMenus;
        private ClaimsPrincipal _currentUser;
        private bool _isAdmin;
        private int SocialMediaId;

        IList<IBrowserFile> _images = new List<IBrowserFile>();
        IList<IBrowserFile> _files = new List<IBrowserFile>();

        List<string> MenuTypes = new List<string> {
            "Pages" ,
            "Drop Down Menu" ,
            "Internal Link" ,
            "External Link" ,
            "Downloaded File" ,
        };
        private FileUploadModel fileUploadModel;
        private FileUploadModel imageUploadModel;

        public int parentId { get; set; } = 0;

        public long maxFileSize = Constants.MaxFileSizeInByte;

        [CascadingParameter]
        private IMudDialogInstance  MudDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {

            await Task.WhenAll(LoadCategories(),
             LoadMenus(),
              LoadMenu());
            //    await LoadMenu();
            parentId = MenuModel.ParentId.HasValue ? MenuModel.ParentId.Value : 0;
            _currentUser = await _authenticationManager.CurrentUser();
            _isAdmin = _currentUser.IsInRole("Administrator");

        }


        public async void Cancel()
        {
            //_navigationManager.NavigateTo($"/menus/{MenuModel.CategoryId}");
            await _jsRuntime.InvokeVoidAsync("history.back", -1);
        }

        private async Task SaveAsync()
        {
            if (MenuModel.LevelOrder < 0)
            {
                _snackBar.Add("Order Must Be More Or Equal 0", Severity.Error);
                return;
            }
            if (parentId == 0)
                MenuModel.ParentId = null;
            else
                MenuModel.ParentId = parentId;

            var generatedImageName = await UploadFile(_images, imageUploadModel, (int)Enums.UploadFileTypeEnum.Image);
            var fullImagePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.MenusFiles.ToString(), generatedImageName);

            var generatedFileName = await UploadFile(_files, fileUploadModel, (int)Enums.UploadFileTypeEnum.File);
            var fullFilePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.MenusFiles.ToString(), generatedFileName);

            if (MenuModel.Id == 0)
            {
                MenuModel.Image = !String.IsNullOrEmpty(generatedImageName) ? fullImagePath : "";
                MenuModel.File = !String.IsNullOrEmpty(generatedFileName) ? fullFilePath : "";
            }
            else
            {
                if (!String.IsNullOrEmpty(generatedImageName))
                    MenuModel.Image = fullImagePath;
                if (!String.IsNullOrEmpty(generatedFileName))
                    MenuModel.File = fullFilePath;
            }

            var content = HelperMethods.ToJson(MenuModel);
            HttpResponseMessage response;

            if (MenuModel.Id == 0)
            {
                response = await _httpClient.PostAsync(EndPoints.Menus, content);
            }
            else
            {
                response = await _httpClient.PutAsync($"{EndPoints.Menus}/{MenuModel.Id}", content);
            }
            if (response.IsSuccessStatusCode)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
                _navigationManager.NavigateTo($"/menus/{MenuModel.CategoryId}");

            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }

        }

        private async Task LoadCategories()
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<MenuCategoryViewModel>>(EndPoints.MenuCategoriesSelect);
            if (response != null)
            {
                categories = response;
                SocialMediaId = categories.Where(c => c.NameAr.Contains("قائمة السوشال ميديا") || c.NameEn.Contains("Social Media Menu")).Select(c => c.Id).FirstOrDefault();
            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
        }

        private async Task LoadMenus()
        {
            PagedResponse<MenuViewModel> response = null;
            //HttpResponseMessage response = null;
            try
            {

                var requestUri = EndPoints.GetAll(EndPoints.Menus, "", null) + $"?categoryId={MenuModel.CategoryId}";

                response = await _httpClient.GetFromJsonAsync<PagedResponse<MenuViewModel>>(requestUri);

                if (response != null)
                {
                    _menus = response.Items.Where(x => x.Id != MenuModel.Id);
                    _parentMenus = response.Items.Where(x => x.Id != MenuModel.Id && x.ParentId==null);
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
        private async Task LoadMenu()
        {
            MenuViewModel response = new();
            try
            {
                if (Id != 0)
                {
                    var requestUri = EndPoints.Menus + $"/{Id}";

                    response = await _httpClient.GetFromJsonAsync<MenuViewModel>(requestUri);

                    if (response != null)
                    {
                        MenuModel = new MenuUpdateModel
                        {
                            Id = response.Id,
                            NameAr = response.NameAr,
                            NameEn = response.NameEn,
                            NameGe = response.NameGe,
                            DescriptionAr = response.DescriptionAr,
                            DescriptionEn = response.DescriptionEn,
                            DescriptionGe = response.DescriptionGe,
                            CategoryId = response.CategoryId,
                            ParentId = response.ParentId,
                            IsActive = response.IsActive,
                            Image = response.Image,
                            File = response.File,
                            LevelOrder = response.LevelOrder,
                            Url = response.Url,
                            PageUrl = response.PageUrl,
                            Type = response.Type,
                            IsHome=response.IsHome,
                            IsFooter = response.IsFooter,
                            IsHomeFooter = response.IsHomeFooter,
                              
                        };
                    }
                    else
                    {
                        _snackBar.Add("Error retrieving data");
                    }
                }
                else
                {
                    MenuModel.CategoryId = CategoryId ?? 0;
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


        private async Task InvokeAddEditPage(string value)
        {

            MenuModel.Type = value;
            if (value == "Pages")
            {
                var parameters = new DialogParameters();

                //add operation
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = false };
                var dialog = await _dialogService.ShowAsync<AddEditPageModal>("Create", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {

                }
            }
        }



        private async void SelectImage(InputFileChangeEventArgs e1)
        {
            _images.Clear();
            _images.Add(e1.File);

            if (_images.Count > 0)
            {
                imageUploadModel = await HelperMethods.Save(e1.File);
                MenuModel.Image = imageUploadModel.Url;
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
                this.StateHasChanged();
            }
        }

        // return random generated file name
        private async Task<string> UploadFile(IList<IBrowserFile> files, FileUploadModel fileModel, int uploadFileType)
        {
            if (files.Count > 0)
            {
                // Provides a container for content encoded using multipart/form-data MIME type.
                using var content = new MultipartFormDataContent();
                content.Add
                (content: fileModel.Content, name: "\"file\"", fileName: fileModel.Name);
                try
                {
                    var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.MenusFiles}/{uploadFileType}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.getMessage();
                    }
                    else
                    {
                        return String.Empty;
                    }

                }
                catch (Exception e)
                {
                    return String.Empty;
                }

            }
            return String.Empty;
        }
        private void DeleteFileAsync()
        {
            _files.Clear();
            fileUploadModel = null;
            MenuModel.File = null;
          
        }
    }
}

