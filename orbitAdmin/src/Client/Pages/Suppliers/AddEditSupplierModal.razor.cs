using Blazored.FluentValidation;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using SchoolV01.Application.Features.Cities.Queries;
using SchoolV01.Application.Features.Countries.Queries;
using SchoolV01.Application.Features.Directorates.Queries;
using SchoolV01.Application.Features.Positions.Queries;
using SchoolV01.Application.Features.Suppliers.Commands;
using SchoolV01.Application.Requests;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SchoolV01.Client.Pages.Suppliers
{
    public partial class AddEditSupplierModal
    {
        [Inject] private ISupplierManager SupplierManager { get; set; }
        [Inject] private IDirectorateManager DirectorateManager { get; set; }

        [Parameter] public AddEditSupplierCommand AddEditSupplierModel { get; set; } = new();
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Inject] private ICityManager CityManager { get; set; }
        [Inject] private ICountryManager CountryManager { get; set; }
        [Inject] private IPositionManager PositionManager { get; set; }


        private List<GetAllCitiesResponse> cities = new();
        private List<GetAllCountriesResponse> countries = new();
        private List<GetAllPositionsResponse> positions = new();
        private List<GetAllDirectoratesResponse> Directorates = new();


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
            var response = await SupplierManager.SaveAsync(AddEditSupplierModel);
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
            await LoadDirectorateAsync();
            await LoadCountryAsync();
            await LoadPositionAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        public async Task LoadDirectorateAsync()
        {
            var response = await DirectorateManager.GetAllAsync();
            if (response.Succeeded)
            {
                Directorates = response.Data;
            }

            StateHasChanged();
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

        public async Task LoadCountryAsync()
        {
            var response = await CountryManager.GetAllAsync();
            if (response.Succeeded)
            {
                countries = response.Data;
            }

            StateHasChanged();
        }

        public async Task LoadPositionAsync()
        {
            var response = await PositionManager.GetAllAsync();
            if (response.Succeeded)
            {
                positions = response.Data;
            }

            StateHasChanged();
        }


        private IBrowserFile _imageFile;

        private async Task UploadLicenseImage(InputFileChangeEventArgs e)
        {
            _imageFile = e.File;
            if (_imageFile != null)
            {
                var extension = System.IO.Path.GetExtension(_imageFile.Name);
                var format = "image/png";
                var imageFile = await e.File.RequestImageFileAsync(format, 600, 600);
                var buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream().ReadAsync(buffer);
                AddEditSupplierModel.LicenseUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                AddEditSupplierModel.IicenseUrlUploadRequest = new UploadRequest { Data = buffer, UploadType = Application.Enums.UploadType.Student, Extension = extension };
            }
        }


        private void DeleteLicenseImageAsync()
        {
            AddEditSupplierModel.LicenseUrl = null;
            AddEditSupplierModel.IicenseUrlUploadRequest = new UploadRequest();
        }

    }
}