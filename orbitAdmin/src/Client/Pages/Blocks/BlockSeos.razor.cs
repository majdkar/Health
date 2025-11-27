using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

using SchoolV01.Client.Extensions;
using SchoolV01.Shared.Constants.Application;
using SchoolV01.Shared.Constants.Permission;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using SchoolV01.Shared.ViewModels.Blocks;
using SchoolV01.Client.Helpers;
using System.Net.Http.Json;
using System.Globalization;

namespace SchoolV01.Client.Pages.Blocks
{
    public partial  class BlockSeos
    {
        [Parameter] public int BlockId { get; set; }
        [Parameter] public string Name { get; set; }


        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<BlockSeoViewModel> _Seos = new();
        private BlockSeoViewModel _Seo = new();

        private ClaimsPrincipal _currentUser;
        private bool _canCreateBlockSeo;
        private bool _canEditBlockSeo;
        private bool _canDeleteBlockSeo;

        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateBlockSeo = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Create)).Succeeded;
            _canEditBlockSeo = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Edit)).Succeeded;
            _canDeleteBlockSeo = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Delete)).Succeeded;

            await LoadSeosAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task LoadSeosAsync()
        {
            if (BlockId != 0)
            {
                var requestUri = EndPoints.BlocksSeo + $"/{BlockId}";
                var response = await _httpClient.GetFromJsonAsync<List<BlockSeoViewModel>>(requestUri); 

               if(response != null)
                    {
                    _Seos = response;
                }
           
            }
        }

        private async Task Delete(int id)
        {
            if (_canDeleteBlockSeo)
            {
                string deleteContent = _localizer["Delete Content"];
                var parameters = new DialogParameters
                {
                    {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
                var dialog = await _dialogService.ShowAsync<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
                var result1 = await dialog.Result;
                if (!result1.Canceled)
                {
                    var result = await _httpClient.DeleteAsync($"{EndPoints.BlocksSeoSave}/{id}");
                    if (result.IsSuccessStatusCode)
                    {
                        _snackBar.Add("Complete Successful!", Severity.Success);
                    }
                    else
                    {
                        _snackBar.Add("Something went wrong!", Severity.Error);
                    }
                    await LoadSeosAsync();
                }
            }
        }

        private async Task Reset()
        {
            _Seos = new List<BlockSeoViewModel>();
            await LoadSeosAsync();
        }

        private async Task InvokeModal(int id = 0,int BlockId = 0)
        {
            _navigationManager.NavigateTo($"/seoBlock-details/{id}/{BlockId}");

        }

    }
}
