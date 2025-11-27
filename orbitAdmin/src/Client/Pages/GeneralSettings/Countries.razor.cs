using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using SchoolV01.Application.Features.Countries.Queries;
using SchoolV01.Application.Features.Countries.Commands;
using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using Microsoft.JSInterop;
using SchoolV01.Shared.Constants.Application;

namespace SchoolV01.Client.Pages.GeneralSettings
{
    public partial class Countries
    {
        [Inject] private ICountryManager CountryManager { get; set; }

        private List<GetAllCountriesResponse> Elements = new();
        private MudTable<GetAllCountriesResponse> _table;
        private ClaimsPrincipal _currentUser;
        private string searchString = "";
        private bool loading = true;
        private bool _canEditCountry;
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canEditCountry = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Countries.Edit)).Succeeded;
            await LoadData();
            loading = false;
        }


        private async Task LoadData()
        {
            loading = true;
            var response = await CountryManager.GetAllAsync();
            if (response.Succeeded)
            {
                Elements = response.Data;
            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
            loading = false;

        }


        private void InvokeAddEditModalAsync()
        {
            Elements.Insert(0, new GetAllCountriesResponse());
        }


        private async Task ItemHasBeenCommitted(GetAllCountriesResponse element)
        {
            loading = true;
            var addEditCountyModel = new AddEditCountryCommand
            {
                Id = element.Id,
                NameAr = element.NameAr,
                NameEn = element.NameEn,
                IsActive = element.IsActive
            };
            var response = await CountryManager.SaveAsync(addEditCountyModel);
            if (response.Succeeded)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }
            await LoadData();
            loading = false;
        }
        private bool FilterFunc(GetAllCountriesResponse element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.NameAr.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.NameEn.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
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
                var response = await CountryManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
                await LoadData();
            }
        }
        private async Task ExportToExcel()
        {
            var response = await CountryManager.ExportToExcelAsync(searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Countries).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(searchString)
                    ? _localizer["Countries exported"]
                    : _localizer["Filtered Countries exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}
