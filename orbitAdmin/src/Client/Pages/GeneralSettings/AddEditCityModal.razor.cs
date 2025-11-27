using Microsoft.AspNetCore.Components;
using MudBlazor;
using SchoolV01.Client.Helpers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using SchoolV01.Application.Features.Cities.Commands;
using System.Globalization;
using SchoolV01.Application.Features.Countries.Queries;

namespace SchoolV01.Client.Pages.GeneralSettings
{
    public partial class AddEditCityModal
    {
        [Parameter] public AddEditCityCommand AddEditCityModel { get; set; } = new();
        [Inject] private ICityManager CityManager { get; set; }
        [Inject] private ICountryManager CountryManager { get; set; }
   
        [CascadingParameter] private IMudDialogInstance  MudDialog { get; set; }
        private List<GetAllCountriesResponse> countries = new();
        private bool _processing = false;
        private static bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar");

        protected async override Task OnInitializedAsync()
        {
            await LoadCountryAsync();
        }

        public void Cancel()
        {
            MudDialog.Cancel();

        }
        public async Task LoadCountryAsync()
        {
            var response = await CountryManager.GetAllAsync();
            if (response.Succeeded)
            {
                countries = response.Data;
            }
        }

        private async Task SaveAsync()
        {
            _processing = true;
            var response = await CityManager.SaveAsync(AddEditCityModel);
            if (response.Succeeded)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
                MudDialog.Close();
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }
            _processing = false;
        }

    }
}
