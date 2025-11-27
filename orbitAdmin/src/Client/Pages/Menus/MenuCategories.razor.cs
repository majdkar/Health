using SchoolV01.Shared.ViewModels.Menus;
using SchoolV01.Shared.Wrapper;
using SchoolV01.Client.Helpers;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using SchoolV01.Shared.Constants.Role;
using System.Threading;

namespace SchoolV01.Client.Pages.Menus

{
    public partial class MenuCategories
    {
        private IEnumerable<MenuCategoryViewModel> elements;
        private MudTable<MenuCategoryViewModel> _table;
        private int totalItems;
        private string searchString = "";
        private bool loaded;
        private bool _isAdmin;
        private int clickedRowId = 0;
        private int selectedRowForTranslation = 0;

        private ClaimsPrincipal _currentUser;

        protected override async Task OnInitializedAsync()
        {
            loaded = true;
            _currentUser = await _authenticationManager.CurrentUser();
            _isAdmin = _currentUser.IsInRole(RoleConstants.AdministratorRole);

        }
        protected override async Task OnParametersSetAsync()
        {
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
        }

        private async Task<TableData<MenuCategoryViewModel>> ServerReload(TableState state, CancellationToken token)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                state.Page = 0;
            }
            await LoadData(state);

            return new TableData<MenuCategoryViewModel>() { Items = elements, TotalItems = totalItems };
        }

        private async Task LoadData(TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }
            var requestUri = EndPoints.GetAll(EndPoints.MenuCategories, searchString, orderings);
            var response = await _httpClient.GetFromJsonAsync<PagedResponse<MenuCategoryViewModel>>(requestUri);
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
        }

        private void OnSearch(string text)
        {
            searchString = text;
            _table.ReloadServerData();
        }

        private async Task InvokeAddEditModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0) // update operation
            {
                var selectedMenuCategory = elements.FirstOrDefault(c => c.Id == id);
                if (selectedMenuCategory != null)
                {
                    parameters.Add(nameof(AddEditMenuCategoryModal.MenuCategoryModel), new MenuCategoryUpdateModel
                    {
                        Id = selectedMenuCategory.Id,
                        NameAr = selectedMenuCategory.NameAr,
                        DescriptionAr = selectedMenuCategory.DescriptionAr,
                        NameEn = selectedMenuCategory.NameEn,
                        DescriptionEn = selectedMenuCategory.DescriptionEn,
                        NameGe = selectedMenuCategory.NameGe,
                        DescriptionGe = selectedMenuCategory.DescriptionGe,
                        IsActive = selectedMenuCategory.IsActive,
                        IsVisibleUser = selectedMenuCategory.IsVisableUser,
                    });
                }
            }
            //add operation
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = false };
            var dialog = await _dialogService.ShowAsync<AddEditMenuCategoryModal>(id == 0 ? "Create" : "Edit", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                OnSearch("");
            }
            loaded = false;
        }


        protected async Task SoftDeleteMenuCategory(int id)
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
                var result = await _httpClient.DeleteAsync($"{EndPoints.MenuCategories}/{id}");
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

        public void PageChanged()
        {
            _table.ReloadServerData();
        }

        private void RowClickEvent(TableRowClickEventArgs<MenuCategoryViewModel> e)
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
