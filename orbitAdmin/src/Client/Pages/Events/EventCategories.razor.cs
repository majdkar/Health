using SchoolV01.Shared.ViewModels.Events;
using SchoolV01.Shared.Wrapper;
using SchoolV01.Client.Helpers;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using SchoolV01.Client.Shared.Dialogs;
using System.Threading;

namespace SchoolV01.Client.Pages.Events
{
    public partial class EventCategories
    {
        private IEnumerable<EventCategoryViewModel> elements;
        private MudTable<EventCategoryViewModel> _table;
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

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Create)).Succeeded;
            _canEditWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Edit)).Succeeded;
            _canDeleteWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Delete)).Succeeded;
            _canSearchWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.View)).Succeeded;
            _canViewWebSiteManagement = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.View)).Succeeded;

            loaded = true;
        }
        protected override async Task OnParametersSetAsync()
        {
            StateHasChanged();
            if (_table != null)
                await _table.ReloadServerData();
            loaded = false;
        }

        private async Task<TableData<EventCategoryViewModel>> ServerReload(TableState state, CancellationToken token)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                state.Page = 0;
            }
            await LoadData(state, state.Page, state.PageSize);

            return new TableData<EventCategoryViewModel>() { Items = elements, TotalItems = totalItems };

        }

        private async Task LoadData(TableState state, int pageNumber, int pageSize)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }
            var requestUri = EndPoints.GetAllPaged(EndPoints.EventCategories, pageNumber, pageSize, searchString, orderings);
            if (_canViewWebSiteManagement)
            {
                var response = await _httpClient.GetFromJsonAsync<PagedResponse<EventCategoryViewModel>>(requestUri);
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
                    var selectedEventCategory = elements.FirstOrDefault(c => c.Id == id);
                    if (selectedEventCategory != null)
                    {
                        parameters.Add(nameof(AddEditEventCategoryModal.EventCategoryModel), new EventCategoryUpdateModel
                        {
                            Id = selectedEventCategory.Id,
                            Name = selectedEventCategory.Name,
                            Description = selectedEventCategory.Description,
                            EnglishName = selectedEventCategory.EnglishName,
                            EnglishDescription = selectedEventCategory.EnglishDescription,
                            Image = selectedEventCategory.Image,
                            RecordOrder = selectedEventCategory.RecordOrder,
                            IsActive = selectedEventCategory.IsActive
                        });
                    }
                }
                //add operation
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = false };
                var dialog = await _dialogService.ShowAsync<AddEditEventCategoryModal>(id == 0 ? "Create" : "Edit", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    OnSearch("");
                }
            }
            loaded = false;
        }

        private async Task InvokeImageModal(string ImagePath)
        {
            if (!string.IsNullOrWhiteSpace(ImagePath))
            {
                var parameters = new DialogParameters();
                parameters.Add(nameof(ImageDialog.ImagePath), ImagePath);
                //add operation
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
                var dialog = await _dialogService.ShowAsync<ImageDialog>("View Image", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {

                }
            }
            loaded = false;
        }

        

        protected async Task SoftDeleteEventCategory(int id)
        {
            if (_canDeleteWebSiteManagement)
            {
                var result = await _httpClient.DeleteAsync($"{EndPoints.EventCategories}/{id}");
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

        private void RowClickEvent(TableRowClickEventArgs<EventCategoryViewModel> e)
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