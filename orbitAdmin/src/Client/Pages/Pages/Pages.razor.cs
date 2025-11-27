using SchoolV01.Client.Helpers;
using SchoolV01.Shared.ViewModels.Pages;
using SchoolV01.Shared.Wrapper;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using SchoolV01.Shared.Constants.Role;
using System.Threading;
using System.Globalization;

namespace SchoolV01.Client.Pages.Pages
{
    public partial class Pages
    {
        private IEnumerable<PageViewModel> elements = new List<PageViewModel>();
        private MudTable<PageViewModel> _table;
        private int totalItems;
        private string searchString = "";

        private bool loaded;
        private int clickedRowId = 0;
        private int selectedRowForTranslation = 0;
        private ClaimsPrincipal _currentUser;
        private bool _canCreateWebSiteManagement;
        private bool _canEditWebSiteManagement;
        private bool _canDeleteWebSiteManagement;
        private bool _canSearchWebSiteManagement;
        private bool _canViewWebSiteManagement;
        private bool _isAdmin;
        private static bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar");

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Create)).Succeeded;
            _canEditWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Edit)).Succeeded;
            _canDeleteWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Delete)).Succeeded;
            _canSearchWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.View)).Succeeded;
            _canViewWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.View)).Succeeded;
            _isAdmin = _currentUser.IsInRole(RoleConstants.AdministratorRole);

            loaded = true;
        }
        protected override async Task OnParametersSetAsync()
        {
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
        }

        private async Task<TableData<PageViewModel>> ServerReload(TableState state, CancellationToken token)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                state.Page = 0;
            }
            await LoadData(state, state.Page, state.PageSize);

            return new TableData<PageViewModel>() { Items = elements, TotalItems = totalItems };
        }

        private async Task LoadData(TableState state, int pageNumber, int pageSize)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }
            var requestUri = EndPoints.GetAllPaged(EndPoints.Pages, pageNumber, pageSize, searchString, orderings);
            if (_canViewWebSiteManagement)
            {
                var response = await _httpClient.GetFromJsonAsync<PagedResponse<PageViewModel>>(requestUri);
                if (response != null)
                {
                    totalItems = response.TotalRecords;
                    elements = response.Items;
                    //if (elements.FirstOrDefault(x => x.Id == selectedRowForTranslation) != null)
                    //    elements.FirstOrDefault(x => x.Id == selectedRowForTranslation).ShowTranslation = true;

                }
                else
                {
                    totalItems = 0;
                    elements = new List<PageViewModel>();
                    _snackBar.Add("Error retrieving data");
                }

            }

        }

        private void OnSearch(string text)
        {
            if (_canSearchWebSiteManagement)
            {
                searchString = text;
                _table.ReloadServerData();
            }
        }

        private async Task InvokeAddEditModal(int id = 0)
        {

            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                var parameters = new DialogParameters();
                if (id != 0) // update operation
                {
                    var selectedPage = elements.FirstOrDefault(c => c.Id == id);
                    if (selectedPage != null)
                    {
                        parameters.Add(nameof(AddEditPageModal.PageModel), new PageUpdateModel
                        {
                            Id = selectedPage.Id,
                            NameAr = selectedPage.NameAr,
                            NameEn = selectedPage.NameEn,
                            NameGe = selectedPage.NameGe,
                            DescriptionAr = selectedPage.DescriptionAr,
                            DescriptionEn = selectedPage.DescriptionEn,
                            DescriptionGe = selectedPage.DescriptionGe,
                            DescriptionAr1 = selectedPage.DescriptionAr1,
                            DescriptionEn1 = selectedPage.DescriptionEn1,
                            DescriptionGe1 = selectedPage.DescriptionGe1,
                            DescriptionAr2 = selectedPage.DescriptionAr2,
                            DescriptionEn2 = selectedPage.DescriptionEn2,
                            DescriptionGe2 = selectedPage.DescriptionGe2,
                            DescriptionAr3 = selectedPage.DescriptionAr3,
                            DescriptionEn3 = selectedPage.DescriptionEn3,
                            DescriptionGe3 = selectedPage.DescriptionGe3,
                            Type = selectedPage.Type,
                            Image = selectedPage.Image,
                            Image1 = selectedPage.Image1,
                            Image2 = selectedPage.Image2,
                            Image3 = selectedPage.Image3,
                            GeoLink = selectedPage.GeoLink,
                            RecordOrder = selectedPage.RecordOrder,
                            IsActive = selectedPage.IsActive,
                            MenuId = selectedPage.MenuId,
                            Url = selectedPage.Url,

                        });
                    }
                }
                //add operation
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
                var dialog = await _dialogService.ShowAsync<AddEditPageModal>(id == 0 ? "Create" : "Edit", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    OnSearch("");
                }
            }
            loaded = false;
        }
        private void InvokeAddEditPagePhotos(int id = 0)
        {
            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                _navigationManager.NavigateTo($"/page-photo-details/{id}");
            }
        }
        private void InvokeAddEditPageAttachements(int id = 0)
        {
            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                _navigationManager.NavigateTo($"/page-attachement-details/{id}");
            }
        }

        private async Task InvokeSeosModal(int id = 0, string name = "")
        {
            _navigationManager.NavigateTo($"/PageSeo/{id}/{name}");
        }

        protected async Task SoftDeletePage(int id)
        {
            if (_canDeleteWebSiteManagement)
            {
                string deleteContent = localizer["Delete Content"];
                var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
                var dialog = await _dialogService.ShowAsync<Shared.Dialogs.DeleteConfirmation>(localizer["Delete"], parameters, options);
                var result1 = await dialog.Result;
                if (!result1.Canceled)
                {
                    var result = await _httpClient.DeleteAsync($"{EndPoints.Pages}/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        _snackBar.Add("Complete Successful!", Severity.Success);
                    }
                    else
                    {
                        _snackBar.Add("Something went wrong!", Severity.Error);
                    }
                    await _table.ReloadServerData();
                }
                loaded = false;
            }
        }

        public void PageChanged()
        {
            _table.ReloadServerData();
        }

        private void RowClickEvent(TableRowClickEventArgs<PageViewModel> e)
        {
            //if (!e.Item.ShowTranslation)
            //{
            //    if (clickedRowId != 0)
            //       elements.FirstOrDefault(x => x.Id == clickedRowId).ShowTranslation = false;
            //    e.Item.ShowTranslation = true;
            //    clickedRowId = e.Item.Id;
            //}
            //else
            //{
            //    e.Item.ShowTranslation = false;
            //}
            loaded = false;
        }
    }

}
