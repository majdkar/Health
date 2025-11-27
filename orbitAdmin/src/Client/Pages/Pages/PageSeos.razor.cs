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
using SchoolV01.Shared.ViewModels.Pages;
using SchoolV01.Client.Helpers;
using System.Net.Http.Json;
using System.Globalization;

namespace SchoolV01.Client.Pages.Pages
{
    public partial  class PageSeos
    {
        [Parameter] public int PageId { get; set; }
        [Parameter] public string Name { get; set; }


        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<PageSeoViewModel> _Seos = new();
        private PageSeoViewModel _Seo = new();

        private ClaimsPrincipal _currentUser;
        private bool _canCreatePageSeo;
        private bool _canEditPageSeo;
        private bool _canDeletePageSeo;

        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreatePageSeo = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Create)).Succeeded;
            _canEditPageSeo = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Edit)).Succeeded;
            _canDeletePageSeo = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.WebSiteManagement.Delete)).Succeeded;

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
            if (PageId != 0)
            {
                var requestUri = EndPoints.PagesSeo + $"/{PageId}";
                var response = await _httpClient.GetFromJsonAsync<List<PageSeoViewModel>>(requestUri); 

               if(response != null)
                    {
                    _Seos = response;
                }
           
            }
        }

        private async Task Delete(int id)
        {
            if (_canDeletePageSeo)
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
                    var result = await _httpClient.DeleteAsync($"{EndPoints.PagesSeoSave}/{id}");
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
            _Seos = new List<PageSeoViewModel>();
            await LoadSeosAsync();
        }

        private async Task InvokeModal(int id = 0,int PageId = 0)
        {
            _navigationManager.NavigateTo($"/seoPage-details/{id}/{PageId}");

        }

    }
}
