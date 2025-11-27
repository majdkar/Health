using System;
using SchoolV01.Application.Requests.OwnersManagement;
using SchoolV01.Client.Extensions;
using SchoolV01.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Owners.Commands;
using SchoolV01.Client.Infrastructure.Managers.OwnersManagement.Owner;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using SchoolV01.Application.Features.Owners.Queries;
using System.Threading;

namespace SchoolV01.Client.Pages.OwnersManagement
{
    public partial class Owners
    {
        [Inject] private IOwnerManager OwnerManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllPagedOwnersResponse> _pagedData;
        private MudTable<GetAllPagedOwnersResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateOwners;
        private bool _canEditOwners;
        private bool _canDeleteOwners;
        private bool _canExportOwners;
        private bool _canSearchOwners;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateOwners = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Owners.Create)).Succeeded;
            _canEditOwners = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Owners.Edit)).Succeeded;
            _canDeleteOwners = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Owners.Delete)).Succeeded;
            _canExportOwners = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Owners.View)).Succeeded;
            _canSearchOwners = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Owners.View)).Succeeded;

            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task<TableData<GetAllPagedOwnersResponse>> ServerReload(TableState state, CancellationToken token)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllPagedOwnersResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] {$"{state.SortLabel} {state.SortDirection}"} : new[] {$"{state.SortLabel}"};
            }

            var request = new GetAllPagedOwnersRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
            var response = await OwnerManager.GetOwnersAsync(request);
            if (response.Succeeded)
            {
                _totalItems = response.TotalCount;
                _currentPage = response.CurrentPage;
                _pagedData = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task ExportToExcel()
        {
            var response = await OwnerManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Owners).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Owners exported"]
                    : _localizer["Filtered Owners exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var owner = _pagedData.FirstOrDefault(c => c.Id == id);
                if (owner != null)
                {
                    parameters.Add(nameof(AddEditOwnerModal.AddEditOwnerModel), new AddEditOwnerCommand
                    {
                        Id = owner.Id,
                        Name = owner.Name,
                        Description = owner.Description,
						
PassportId = owner.PassportId,
                        //Rate = owner.Rate,
                        //BrandId = owner.BrandId,
                        //Barcode = owner.Barcode
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await _dialogService.ShowAsync<AddEditOwnerModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                OnSearch("");
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = await _dialogService.ShowAsync<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var response = await OwnerManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    OnSearch("");
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    OnSearch("");
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
    }
}