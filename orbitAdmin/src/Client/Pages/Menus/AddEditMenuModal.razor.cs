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

namespace SchoolV01.Client.Pages.Menus
{
    public partial class AddEditMenuModal
    {
        [Parameter]
        public MenuUpdateModel MenuModel { get; set; } = new();

        private IEnumerable<MenuCategoryViewModel> categories = [];
        private IEnumerable<MenuViewModel> _menus = [];
       
        private ClaimsPrincipal _currentUser;
        private bool _isAdmin;
        private bool _processing = false;

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
            _currentUser = await _authenticationManager.CurrentUser();
            _isAdmin = _currentUser.IsInRole("Administrator");

            await LoadCategories();
            await LoadMenus();
            parentId = MenuModel.ParentId.HasValue ? MenuModel.ParentId.Value : 0;

        }


        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            if (MenuModel.LevelOrder < 0)
            {
                _snackBar.Add("Order Must Be More Or Equal 0", Severity.Error);
                return;
            }
            
            _processing = true;

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
                MudDialog.Close();
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }
            _processing = false;

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
    }
}

