using System;
using SchoolV01.Application.Features.Passports.Queries;
using SchoolV01.Client.Extensions;
using SchoolV01.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Passports.Commands;
using SchoolV01.Client.Infrastructure.Managers.OwnersManagement.Passport;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;

namespace SchoolV01.Client.Pages.OwnersManagement
{
    public partial class Passports
    {
        [Inject] private IPassportManager PassportManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllPassportsResponse> _passportList = new();
        private GetAllPassportsResponse _passport = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreatePassports;
        private bool _canEditPassports;
        private bool _canDeletePassports;
        private bool _canExportPassports;
        private bool _canSearchPassports;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreatePassports = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Passports.Create)).Succeeded;
            _canEditPassports = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Passports.Edit)).Succeeded;
            _canDeletePassports = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Passports.Delete)).Succeeded;
            _canExportPassports = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Passports.View)).Succeeded;
            _canSearchPassports = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Passports.View)).Succeeded;

            await GetPassportsAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetPassportsAsync()
        {
            var response = await PassportManager.GetAllAsync();
            if (response.Succeeded)
            {
                _passportList = response.Data.ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
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
                var response = await PassportManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await Reset();
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    await Reset();
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private async Task ExportToExcel()
        {
            var response = await PassportManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Passports).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Passports exported"]
                    : _localizer["Filtered Passports exported"], Severity.Success);
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
                _passport = _passportList.FirstOrDefault(c => c.Id == id);
                if (_passport != null)
                {
                    parameters.Add(nameof(AddEditPassportModal.AddEditPassportModel), new AddEditPassportCommand
                    {
                        Id = _passport.Id,
						Name = _passport.Name,
                        Description = _passport.Description,
						

                        //Tax = _passport.Tax
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = await _dialogService.ShowAsync<AddEditPassportModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _passport = new GetAllPassportsResponse();
            await GetPassportsAsync();
        }

        private bool Search(GetAllPassportsResponse passport)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
			if (passport.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (passport.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
			
            /**/
            return false;
        }
    }
}