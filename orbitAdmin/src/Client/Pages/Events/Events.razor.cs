using SchoolV01.Client.Extensions;
using SchoolV01.Client.Helpers;
using SchoolV01.Shared.ViewModels.Events;
using SchoolV01.Shared.Wrapper;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using System.Threading;
using Microsoft.AspNetCore.Components;

namespace SchoolV01.Client.Pages.Events
{
    public partial class Events
    {
       [SupplyParameterFromQuery] private string CategoryId { get; set; } = "N/A"; // Query string parameter

        private IEnumerable<EventViewModel> elements;
        private IEnumerable<EventCategoryViewModel> categories;
        private MudTable<EventViewModel> _table;
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
            await LoadCategories();
            await _table.ReloadServerData();
        }

        private async Task<TableData<EventViewModel>> ServerReload(TableState state, CancellationToken token)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                state.Page = 0;
            }
            await LoadData(state, state.Page, state.PageSize);

            return new TableData<EventViewModel>() { Items = elements, TotalItems = totalItems };
        }
        private void InvokeAddEditEventPhotos(int id = 0)
        {
            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                _navigationManager.NavigateTo($"/event-photo-details/{id}");
            }
        }
        private void InvokeAddEditEventAttachements(int id = 0)
        {
            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                _navigationManager.NavigateTo($"/event-attachement-details/{id}");
            }
        }
        private async Task LoadData(TableState state, int pageNumber, int pageSize)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }
            var requestUri = EndPoints.GetAllPaged(EndPoints.Events, pageNumber, pageSize, searchString, orderings) + $"&categoryId={CategoryId}";
            if (_canViewWebSiteManagement)
            {
                var response = await _httpClient.GetFromJsonAsync<PagedResponse<EventViewModel>>(requestUri);
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

        private async Task LoadCategories()
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<EventCategoryViewModel>>(EndPoints.EventCategoriesSelect);
            if (response != null)
            {
                categories = response;
            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
            loaded = false;
        }

        private void InvokeAddEditEvent(int id = 0)
        {
            if ((id == 0 && _canCreateWebSiteManagement) || (id != 0 && _canEditWebSiteManagement))
            {
                _navigationManager.NavigateTo($"/event-details/{id}");
            }
        }

 
        protected async Task SoftDeleteEvent(int id)
        {
            if (_canDeleteWebSiteManagement)
            {
                var result = await _httpClient.DeleteAsync($"{EndPoints.Events}/{id}");
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

        private void RowClickEvent(TableRowClickEventArgs<EventViewModel> e)
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
