using SchoolV01.Client.Helpers;
using SchoolV01.Shared.ViewModels.Settings;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Shared.Components
{
    public partial class LanguageSelector
    {
        private IEnumerable<LanguageViewModel> _languages;


        [Parameter]
        public string Language { get; set; }

        [Parameter]
        public Variant VariantValue { get; set; } = Variant.Text;

        [Parameter]
        public EventCallback<string> OnLanguageSelection { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await LoadLanguages();
        }

        private async Task LoadLanguages()
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<LanguageViewModel>>(EndPoints.Languages);
            if (response != null)
            {
                _languages = response;
            }
            else
            {
                _snackBar.Add("Error retrieving data");
            }
        }


        protected async Task ChangeLanguage()
        {
            await OnLanguageSelection.InvokeAsync(Language);
        }


    }
}
