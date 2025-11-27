using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using SchoolV01.Application.Features.Cities.Queries;
using SchoolV01.Application.Features.Countries.Queries;
using SchoolV01.Application.Features.Directorates.Commands;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SchoolV01.Client.Pages.Directorates
{
    public partial class AddEditDirectorateModal
    {
        [Inject] private IDirectorateManager DirectorateManager { get; set; }

        [Parameter] public AddEditDirectorateCommand AddEditDirectorateModel { get; set; } = new();
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Inject] private ICityManager CityManager { get; set; }


        private List<GetAllCitiesResponse> cities = new();


        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private bool _processing = false;
        private bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar-");


        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            _processing = true;
            var response = await DirectorateManager.SaveAsync(AddEditDirectorateModel);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            _processing = false;
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadCityAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        public async Task LoadCityAsync()
        {
            var response = await CityManager.GetAllAsync();
            if (response.Succeeded)
            {
                cities = response.Data;
            }

            StateHasChanged();
        }
    }
}