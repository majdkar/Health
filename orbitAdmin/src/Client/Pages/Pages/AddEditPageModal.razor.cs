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
using System.Threading.Tasks;
using SchoolV01.Shared.Constants;
using SchoolV01.Shared.ViewModels.Menus;
using SchoolV01.Shared.Wrapper;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;

namespace SchoolV01.Client.Pages.Pages
{
    public partial class AddEditPageModal
    {
        
        [Parameter]
        public PageUpdateModel PageModel { get; set; } = new();

        [CascadingParameter]
        private IMudDialogInstance  MudDialog { get; set; }
        private bool _processing = false;

        private IList<IBrowserFile> _images = new List<IBrowserFile>();
        private IList<IBrowserFile> _images1 = new List<IBrowserFile>();
        private IList<IBrowserFile> _images2 = new List<IBrowserFile>();
        private IList<IBrowserFile> _images3 = new List<IBrowserFile>();
        private IEnumerable<MenuViewModel> _menus;
        private FileUploadModel imageUploadModel;
        private FileUploadModel imageUploadModel1;
        private FileUploadModel imageUploadModel2;
        private FileUploadModel imageUploadModel3;


        List<string> PageTypes = new List<string> {
            "Basic Pages" ,
            "Contact Us" ,
        };
        private bool _isAdmin;
        private ClaimsPrincipal _currentUser;

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

        protected async override Task OnInitializedAsync()
        {
            
            _currentUser = await _authenticationManager.CurrentUser();

            
            _isAdmin = _currentUser.IsInRole("Administrator");
            await LoadMenus();
            
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

                    
                    if (!String.IsNullOrEmpty(response.Image))
                    {
                        PageModel.Image = response.Image;
                        //disableDeleteImageButton = false;
                    }
                    if (!String.IsNullOrEmpty(response.Image1))
                    {
                        PageModel.Image1 = response.Image1;
                        //disableDeleteImageButton = false;
                    }
                    if (!String.IsNullOrEmpty(response.Image2))
                    {
                        PageModel.Image2 = response.Image2;
                        //disableDeleteImageButton = false;
                    }
                    if (!String.IsNullOrEmpty(response.Image3))
                    {
                        PageModel.Image3 = response.Image3;
                        //disableDeleteImageButton = false;
                    }
                    this.StateHasChanged();
                }
            }
        }
        private async Task LoadMenus()
        {
            PagedResponse<MenuViewModel> response = null;
            try
            {
                var requestUri = EndPoints.GetAll(EndPoints.MenusNoCategory, "", null) ;
                response = await _httpClient.GetFromJsonAsync<PagedResponse<MenuViewModel>>(requestUri);
                if (response != null)
                {
                    _menus = response.Items;
                }
                else
                {
                    _snackBar.Add("Error retrieving data");
                }
            }
            catch (Exception)
            {
                _snackBar.Add("Error retrieving data: ");
            }
            finally
            {
            }
        }
        public void Cancel()
        {
            MudDialog.Cancel();
            
        }

        private async Task SaveAsync()
        {
            _processing = true;
            var generatedImageName = await UploadImage();
            var generatedImageName1 = await UploadImage1();
            var generatedImageName2 = await UploadImage2();
            var generatedImageName3 = await UploadImage3();
            var fullImagePath = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.PagesFiles.ToString(), generatedImageName);
            var fullImagePath1 = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.PagesFiles.ToString(), generatedImageName1);
            var fullImagePath2 = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.PagesFiles.ToString(), generatedImageName2);
            var fullImagePath3 = Path.Combine(Constants.UploadFolderName, Enums.FileLocation.PagesFiles.ToString(), generatedImageName3);

            if (PageModel.Id == 0)
            {
                PageModel.Image = !String.IsNullOrEmpty(generatedImageName) ? fullImagePath : "";
                PageModel.Image1 = !String.IsNullOrEmpty(generatedImageName1) ? fullImagePath1 : "";
                PageModel.Image2 = !String.IsNullOrEmpty(generatedImageName2) ? fullImagePath2 : "";
                PageModel.Image3 = !String.IsNullOrEmpty(generatedImageName3) ? fullImagePath3 : "";
            }
            else
            {
                if (!string.IsNullOrEmpty(generatedImageName))
                    PageModel.Image = fullImagePath;
                if (!string.IsNullOrEmpty(generatedImageName1))
                    PageModel.Image1 = fullImagePath1;
                if (!string.IsNullOrEmpty(generatedImageName2))
                    PageModel.Image2 = fullImagePath2;
                if (!string.IsNullOrEmpty(generatedImageName3))
                    PageModel.Image3 = fullImagePath3;
            }

            var content = HelperMethods.ToJson(PageModel);
            HttpResponseMessage response;
            if (PageModel.Id == 0)
            {
                
                response = await _httpClient.PostAsync(EndPoints.Pages, content);
                _snackBar.Add(response.ToString());
            }
            else
            {
                response = await _httpClient.PutAsync($"{EndPoints.Pages}/{PageModel.Id}", content);
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

        private async void SelectImage(InputFileChangeEventArgs e)
        {
            _images.Clear();
            _images.Add(e.File);

            if (_images.Count > 0)
            {
                imageUploadModel = await HelperMethods.Save(e.File);
                this.StateHasChanged();
            }
        }

        private async void SelectImage1(InputFileChangeEventArgs e)
        {
            _images1.Clear();
            _images1.Add(e.File);

            if (_images1.Count > 0)
            {
                imageUploadModel1 = await HelperMethods.Save(e.File);
                this.StateHasChanged();
            }
        }

        private async void SelectImage2(InputFileChangeEventArgs e)
        {
            _images2.Clear();
            _images2.Add(e.File);

            if (_images2.Count > 0)
            {
                imageUploadModel2 = await HelperMethods.Save(e.File);
                this.StateHasChanged();
            }
        }
        private async void SelectImage3(InputFileChangeEventArgs e)
        {
            _images3.Clear();
            _images3.Add(e.File);

            if (_images3.Count > 0)
            {
                imageUploadModel3 = await HelperMethods.Save(e.File);
                this.StateHasChanged();
            }
        }


        private async Task<string> UploadImage()
        {
            if (_images.Count > 0)
            {
                using var content = new MultipartFormDataContent();
                content.Add
                (content: imageUploadModel.Content, name: "\"file\"", fileName: imageUploadModel.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.PagesFiles}/{(int)Enums.UploadFileTypeEnum.Image}", content);
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

        private async Task<string> UploadImage1()
        {
            if (_images1.Count > 0)
            {
                using var content = new MultipartFormDataContent();
                content.Add
                (content: imageUploadModel1.Content, name: "\"file\"", fileName: imageUploadModel1.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.PagesFiles}/{(int)Enums.UploadFileTypeEnum.Image}", content);
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

        private async Task<string> UploadImage2()
        {
            if (_images2.Count > 0)
            {
                using var content = new MultipartFormDataContent();
                content.Add
                (content: imageUploadModel2.Content, name: "\"file\"", fileName: imageUploadModel2.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.PagesFiles}/{(int)Enums.UploadFileTypeEnum.Image}", content);
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

        private async Task<string> UploadImage3()
        {
            if (_images3.Count > 0)
            {
                using var content = new MultipartFormDataContent();
                content.Add
                (content: imageUploadModel3.Content, name: "\"file\"", fileName: imageUploadModel3.Name);

                var response = await _httpClient.PostAsync($"{EndPoints.FileUpload}/{(int)Enums.FileLocation.PagesFiles}/{(int)Enums.UploadFileTypeEnum.Image}", content);
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

    }

}


