using MudBlazor;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using SchoolV01.Application.Features.Cities.Queries;
using SchoolV01.Application.Features.Cities.Commands;
using Microsoft.AspNetCore.Components;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using System.Globalization;
using System;
using SchoolV01.Application.Features.Countries.Queries;
using Microsoft.JSInterop;
using SchoolV01.Shared.Constants.Application;

namespace SchoolV01.Client.Pages.GeneralSettings
{
    public partial class Cities
    {
        [Inject] public ICityManager CityManager { get; set; }
        [Inject] public ICountryManager CountryManager { get; set; }

        private List<GetAllCitiesResponse> Elements = [];
        private MudTable<GetAllCitiesResponse> _table;

        private ClaimsPrincipal _currentUser;
        private List<GetAllCountriesResponse> countries = [];
        private string searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        private bool loading = true;
        private static bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar");

        protected override async Task OnInitializedAsync()
        {
            await Task.WhenAll(LoadData(), LoadCountryAsync());

        }
        protected override async Task OnParametersSetAsync()
        {
            await LoadData();
            StateHasChanged();
        }

        private async Task LoadData()
        {
            loading = true;
            var response = await CityManager.GetAllAsync();
            if (response.Succeeded)
            {
                Elements = response.Data;

            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
            loading = false;
            StateHasChanged();

        }
        public async Task LoadCountryAsync()
        {
            var response = await CountryManager.GetAllAsync();
            if (response.Succeeded)
            {
                countries = response.Data;

            }
        }

        private void InvokeAddEditModalAsync()
        {
            Elements.Insert(0, new());
        }

        //add operation
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
                var response = await CityManager.DeleteAsync(id);
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
        private bool FilterFunc(GetAllCitiesResponse element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.NameAr.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.NameEn.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.CountryAr} {element.CountryEn}".Contains(searchString))
                return true;
            return false;
        }
        private async Task ItemHasBeenCommitted(GetAllCitiesResponse city)
        {
            loading = true;
            var request = new AddEditCityCommand
            {
                Id = city.Id,
                NameAr = city.NameAr,
                NameEn = city.NameEn,
                CountryId = city.CountryId,
                IsActive = city.IsActive
            };
            var response = await CityManager.SaveAsync(request);
            if (response.Succeeded)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }
            await LoadData();
        }

        private async Task ExportToExcel()
        {
            var response = await CityManager.ExportToExcelAsync(searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Cities).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(searchString)
                    ? _localizer["CheckTypes exported"]
                    : _localizer["Filtered CheckTypes exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        private IEnumerable<string> ValidCountry(int value)
        {
            if (value == 0)
            {
                yield return _localizer["Country is Required"];
            }
        }
    }
}