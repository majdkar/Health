using SchoolV01.Shared.ViewModels.Settings;
using SchoolV01.Client.Helpers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Countries.Commands;

namespace SchoolV01.Client.Pages.GeneralSettings
{
    public partial class AddEditCountryModal
    {
        [Parameter]public int Id { get; set; } 
        [Parameter]public AddEditCountryCommand CountryModel { get; set; } = new();

        [CascadingParameter]
        private IMudDialogInstance  MudDialog { get; set; }
        private bool _processing = false;

        public void Cancel()
        {
            MudDialog.Cancel();

        }

        private async Task SaveAsync()
        {
            _processing = true;
            var content = HelperMethods.ToJson(CountryModel);
            HttpResponseMessage response;
            if (CountryModel.Id == 0)
            {
                response = await _httpClient.PostAsync(EndPoints.Countries, content);
            }
            else
            {
                response = await _httpClient.PutAsync($"{EndPoints.Countries}/{CountryModel.Id}", content);
            }
            if (response.IsSuccessStatusCode)
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
