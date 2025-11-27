using System;
using SchoolV01.Application.Features.Positions.Queries;
using SchoolV01.Client.Extensions;
using SchoolV01.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Positions.Commands;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;

namespace SchoolV01.Client.Pages.GeneralSettings
{
    public partial class Positions
    {
        [Inject] private IPositionManager PositionManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllPositionsResponse> _PositionList = new();
        private GetAllPositionsResponse _Position = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreatePositions;
        private bool _canEditPositions;
        private bool _canDeletePositions;
        private bool _canExportPositions;
        private bool _canSearchPositions;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreatePositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Positions.Create)).Succeeded;
            _canEditPositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Positions.Edit)).Succeeded;
            _canDeletePositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Positions.Delete)).Succeeded;
            _canExportPositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Positions.Export)).Succeeded;
            _canSearchPositions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Positions.Search)).Succeeded;

            await GetPositionsAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetPositionsAsync()
        {
            var response = await PositionManager.GetAllAsync();
            if (response.Succeeded)
            {
                _PositionList = response.Data.ToList();
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
            var dialog =await _dialogService.ShowAsync<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var response = await PositionManager.DeleteAsync(id);
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
            //var response = await PositionManager.ExportToExcelAsync(_searchString);
            //if (response.Succeeded)
            //{
            //    await _jsRuntime.InvokeVoidAsync("Download", new
            //    {
            //        ByteArray = response.Data,
            //        FileName = $"{nameof(Positions).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
            //        MimeType = ApplicationConstants.MimeTypes.OpenXml
            //    });
            //    _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
            //        ? _localizer["Positions exported"]
            //        : _localizer["Filtered Positions exported"], Severity.Success);
            //}
            //else
            //{
            //    foreach (var message in response.Messages)
            //    {
            //        _snackBar.Add(message, Severity.Error);
            //    }
            //}
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                _Position = _PositionList.FirstOrDefault(c => c.Id == id);
                if (_Position != null)
                {
                    parameters.Add(nameof(AddEditPositionModal.AddEditPositionModel), new AddEditPositionCommand
                    {
                        Id = _Position.Id,
						Name = _Position.Name,
						EnglishName = _Position.EnglishName,

                        //Tax = _Position.Tax
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog =await _dialogService.ShowAsync<AddEditPositionModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _Position = new GetAllPositionsResponse();
            await GetPositionsAsync();
        }

        private bool Search(GetAllPositionsResponse Position)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
			if (Position.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            
			if (Position.EnglishName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
{
return true;
}

            /**/
            return false;
        }
    }
}