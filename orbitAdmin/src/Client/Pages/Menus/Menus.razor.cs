using SchoolV01.Client.Helpers;
using SchoolV01.Shared.ViewModels.Menus;
using SchoolV01.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;

namespace SchoolV01.Client.Pages.Menus
{
    public partial class Menus
    {
        [Parameter]
        public string CategoryId { get; set; } = "0";
        public int? MenuId { get; set; }

        private IEnumerable<MenuViewModel> elements;
        private ClaimsPrincipal _currentUser;

        private MudTable<MenuViewModel> _table;
        private int totalItems;
        private string searchString = "";
        private bool _isAdmin;
        private bool loaded;
        private int clickedRowId = 0;
        private int selectedRowForTranslation = 0;

        protected override async void OnInitialized()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _isAdmin = _currentUser.IsInRole("Administrator");
            loaded = true;
        }

        protected override async Task OnParametersSetAsync()
        {
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
        }

        private async Task<TableData<MenuViewModel>> ServerReload(TableState state, CancellationToken token)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                state.Page = 0;
            }
            await LoadData(state);

            return new TableData<MenuViewModel>() { Items = elements, TotalItems = totalItems };
        }

        private async Task LoadData(TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var requestUri = EndPoints.GetAllWithParameter(EndPoints.Menus + $"/GetMaster?categoryId={CategoryId}&menuId={MenuId}", searchString, orderings);





            var response = await _httpClient.GetFromJsonAsync<PagedResponse<MenuViewModel>>(requestUri);
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

        private async Task InvokeAddEditModal(int id = 0)
        {
            _navigationManager.NavigateTo($"/menu-details/{id}/{CategoryId}");
            //var parameters = new DialogParameters();
            //if (id != 0) // update operation
            //{
            //    var selectedMenu = elements.FirstOrDefault(c => c.Id == id);
            //    if (selectedMenu != null)
            //    {
            //        parameters.Add(nameof(AddEditMenuModal.MenuModel), new MenuUpdateModel
            //        {
            //            Id = selectedMenu.Id,
            //            Name = selectedMenu.Name,
            //            EnglishName = selectedMenu.EnglishName,
            //            Description = selectedMenu.Description,
            //            EnglishDescription = selectedMenu.EnglishDescription,
            //            Type = selectedMenu.Type,
            //            File = selectedMenu.File,
            //            Image = selectedMenu.Image,
            //            PageUrl = selectedMenu.PageUrl,
            //            CategoryId = selectedMenu.CategoryId,
            //            ParentId = selectedMenu.ParentId,
            //            LevelOrder = selectedMenu.LevelOrder,
            //            IsActive = selectedMenu.IsActive
            //        });
            //    }
            //}
            //else
            //{
            //    parameters.Add(nameof(AddEditMenuModal.MenuModel), new MenuUpdateModel
            //    {
            //        CategoryId = int.Parse(CategoryId),
            //    });
            //}
            ////add operation
            //var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
            //var dialog = await _dialogService.ShowAsync<AddEditMenuModal>(id == 0 ? "Create" : "Edit", parameters, options);
            //var result = await dialog.Result;
            //if (!result.Canceled)
            //{
            //    OnSearch("");
            //}
            //loaded = false;
        }
        private async void InvokeMenuSubModal(int? id)
        {
            MenuId = id;
            searchString = string.Empty;
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
        }

        private async void InvokeBackModal(int id)
        {
            MenuId = null;
            searchString = string.Empty;
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
        }

        protected async Task SoftDeleteMenu(int id)
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
                var result = await _httpClient.DeleteAsync($"{EndPoints.Menus}/{id}");
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

        private void RowClickEvent(TableRowClickEventArgs<MenuViewModel> e)
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

