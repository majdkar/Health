using SchoolV01.Shared.ViewModels.Blocks;
using SchoolV01.Shared.Wrapper;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Helpers;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using SchoolV01.Core.Entities;
using System.Threading;
using SchoolV01.Shared.ViewModels.Menus;
using System.Globalization;


namespace SchoolV01.Client.Pages.Blocks
{
    public partial class Blocks
    {
        [Parameter]
        public int CategoryId { get; set; } = 0; // This is a queryString parameter

        public int? BlockId { get; set; }

        private static bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar");

        //[Parameter] public BlockCategoryViewModel _blockCategory { get; set; } = new();
        private IEnumerable<BlockViewModel> elements;
        private IEnumerable<BlockCategoryViewModel> categories;
        private MudTable<BlockViewModel> _table;
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
        private string _blockCategoryName = "";

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Create)).Succeeded;
            _canEditWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Edit)).Succeeded;
            _canDeleteWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Delete)).Succeeded;
            _canSearchWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.View)).Succeeded;
            _canViewWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.View)).Succeeded;

            _isAdmin = _currentUser.IsInRole("Administrator");
            loaded = true;

            await LoadCategories();
        }
        protected override async Task OnParametersSetAsync()
        {
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
            //await LoadCategoryDetails();
        }

        private async Task<TableData<BlockViewModel>> ServerReload(TableState state, CancellationToken token)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                state.Page = 0;
            }
            await LoadData(state, state.Page, state.PageSize);

            return new TableData<BlockViewModel>() { Items = elements, TotalItems = totalItems };
        }

        private async Task LoadData(TableState state, int pageNumber, int pageSize)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var requestUri = EndPoints.GetAllWithParameter(EndPoints.Blocks + $"/GetMaster?categoryId={CategoryId}&blockId={BlockId}", searchString, orderings);





            var response = await _httpClient.GetFromJsonAsync<PagedResponse<BlockViewModel>>(requestUri);
            if (response != null)
            {
                totalItems = response.TotalRecords;
                elements = response.Items;
                if (elements.FirstOrDefault(x => x.Id == selectedRowForTranslation) != null)
                    elements.FirstOrDefault(x => x.Id == selectedRowForTranslation).ShowTranslation = true;
            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
            loaded = false;
        }

        private void OnSearch(string text)
        {
            searchString = text;
            _table.ReloadServerData();
        }

        private async Task LoadCategories()
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<BlockCategoryViewModel>>(EndPoints.BlockCategoriesSelect);
            if (response != null)
            {
                categories = response;
                if (categories.FirstOrDefault(x => x.Id == Convert.ToInt32(CategoryId)) != null)
                    _blockCategoryName = categories.FirstOrDefault(x => x.Id == Convert.ToInt32(CategoryId)).NameAr;
            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
        }

        //private async Task LoadCategoryDetails()
        //{
        //    string[] orderings = null;
        //    var requestUri = "";
        //    if (CategoryId != 0)
        //    {
        //        requestUri = EndPoints.GetAllPaged(EndPoints.BlockCategories, 0, 25, "", orderings);
        //        var response = await _httpClient.GetFromJsonAsync<PagedResponse<BlockCategoryViewModel>>(requestUri);
        //        if (response != null)
        //        {

        //            _blockCategory = response.Items.Where(e => e.Id.ToString().Equals(CategoryId)).FirstOrDefault();

        //        }
        //    }
        //}
        private void InvokeAddEditBlock(int id = 0)
        {
            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                _navigationManager.NavigateTo($"/block-details/{CategoryId}/{id}");
            }
        }

        private void InvokeAddEditBlockPhotos(int id = 0)
        {
            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                _navigationManager.NavigateTo($"/block-photo-details/{id}");
            }
        }

        private async Task InvokeSeosModal(int id = 0, string name = "")
        {
            _navigationManager.NavigateTo($"/BlockSeo/{id}/{name}");
        }
        private async void InvokeBlockSubModal(int? id)
        {
            BlockId = id;
            searchString = string.Empty;
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
        }

        private async void InvokeBackModal(int id)
        {
            BlockId = null;
            searchString = string.Empty;
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
        }
        private void InvokeAddEditBlockAttachements(int id = 0)
        {
            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                _navigationManager.NavigateTo($"/block-attachement-details/{id}");
            }
        }



        protected async Task ChangeStatus(bool active, BlockViewModel viewModel)
        {
            if (_canEditWebSiteManagement)
            {
                var BlockModel = new BlockUpdateModel
                {
                    Id = viewModel.Id,
                    NameAr = viewModel.NameAr,
                    NameEn = viewModel.NameEn,
                    NameGe = viewModel.NameGe,
                    DescriptionAr = viewModel.DescriptionAr,
                    DescriptionEn = viewModel.DescriptionEn,
                    Date = viewModel.Date,
                    BlockAttachements = viewModel.BlockAttachements,
                    BlockPhotos = viewModel.BlockPhotos,
                    RecordOrder = viewModel.RecordOrder,
                    CategoryId = viewModel.CategoryId,
                    IsActive = active,
                    IsVisible = viewModel.IsVisible,
                    Url = viewModel.Url,
                     AuthorId = viewModel.AuthorId,
                      CreateAt = viewModel.CreateAt,
                };

                var content = HelperMethods.ToJson(BlockModel);

                var result = await _httpClient.PutAsync($"{EndPoints.Blocks}/{viewModel.Id}", content);
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
        }
        protected async Task SoftDeleteBlock(int id)
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
                    var result = await _httpClient.DeleteAsync($"{EndPoints.Blocks}/{id}");
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
            }
        }

        public void PageChanged()
        {
            _table.ReloadServerData();
        }

        private void RowClickEvent(TableRowClickEventArgs<BlockViewModel> e)
        {
            if (!e.Item.ShowTranslation)
            {
                if (clickedRowId != 0)
                    elements.FirstOrDefault(x => x.Id == clickedRowId).ShowTranslation = false;
                e.Item.ShowTranslation = true;
                clickedRowId = e.Item.Id;
            }
            else
            {
                e.Item.ShowTranslation = false;
            }
            loaded = false;
        }
    }

}
