using Blazored.FluentValidation;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using SchoolV01.Application.Features.Cities.Queries;
using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Application.Features.Countries.Queries;
using SchoolV01.Application.Features.Devices.Commands;
using SchoolV01.Application.Features.Directorates.Queries;
using SchoolV01.Application.Features.Hospitals.Queries;
using SchoolV01.Application.Features.Positions.Queries;
using SchoolV01.Application.Features.ProjectTypes.Queries;
using SchoolV01.Application.Features.Suppliers.Queries;
using SchoolV01.Application.Requests;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolV01.Client.Pages.Devices
{
    public partial class AddEditDeviceModal
    {
        [Inject] private IDeviceManager DeviceManager { get; set; }
        //[Inject] private IDirectorateManager DirectorateManager { get; set; }

        [Parameter] public AddEditDeviceCommand AddEditDeviceModel { get; set; } = new();
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Inject] private ISupplierManager SupplierManager { get; set; }
        [Inject] private IProjectTypeManager ProjectTypeManager { get; set; }
        [Inject] private IPositionManager PositionManager { get; set; }
        [Inject] private IClinicManager ClinicManager { get; set; }
        [Inject] private IHospitalManager HospitalManager { get; set; }


        private List<GetAllSuppliersResponse> Suppliers = new();
        private List<GetAllProjectTypesResponse> ProjectTypes = new();
        private List<GetAllProjectTypesResponse> SubProjectTypes = new();
        private List<GetAllProjectTypesResponse> SubSubProjectTypes = new();
        private List<GetAllDirectoratesResponse> Directorates = new();
        private List<GetAllClinicsResponse> Clinics = new();
        private List<GetAllHospitalsResponse> Hospitals = new();


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
            var response = await DeviceManager.SaveAsync(AddEditDeviceModel);
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
            await LoadSupplierAsync();
            await LoadClinicAsync();
            await LoadHospitalAsync();
           // await LoadDirectorateAsync();
            await LoadProjectTypeAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        //public async Task LoadDirectorateAsync()
        //{
        //    var response = await DirectorateManager.GetAllAsync();
        //    if (response.Succeeded)
        //    {
        //        Directorates = response.Data;
        //    }

        //    StateHasChanged();
        //}

        public async Task LoadSupplierAsync()
        {
            var response = await SupplierManager.GetAllAsync();
            if (response.Succeeded)
            {
                Suppliers = response.Data;
            }

            StateHasChanged();
        }

        public async Task LoadClinicAsync()
        {
            var response = await ClinicManager.GetAllAsync();
            if (response.Succeeded)
            {
                Clinics = response.Data;
            }

            StateHasChanged();
        }

        public async Task LoadHospitalAsync()
        {
            var response = await HospitalManager.GetAllAsync();
            if (response.Succeeded)
            {
                Hospitals = response.Data;
            }

            StateHasChanged();
        }

        public async Task LoadProjectTypeAsync()
        {
            var response = await ProjectTypeManager.GetAllAsync();
            if (response.Succeeded)
            {
                ProjectTypes = response.Data;
            }

            StateHasChanged();
        }

        private async Task OnProjectTypeChanged(int? value)
        {
            AddEditDeviceModel.ProjectTypeId = value;

            SubProjectTypes = ProjectTypes
                .FirstOrDefault(x => x.Id == value)?
                .SubProjectTypes.ToList() ?? new();

            // لازم نفرّغ المستوى الثالث
            SubSubProjectTypes = new();
            AddEditDeviceModel.SubProjectTypeId = null;

            StateHasChanged();
        }

        private async Task OnSubProjectTypeChanged(int? value)
        {
            AddEditDeviceModel.SubProjectTypeId = value;

            SubSubProjectTypes = SubProjectTypes
                .FirstOrDefault(x => x.Id == value)?
                .SubProjectTypes.ToList() ?? new();

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
                AddEditDeviceModel.LicenseUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                AddEditDeviceModel.IicenseUrlUploadRequest = new UploadRequest { Data = buffer, UploadType = Application.Enums.UploadType.Student, Extension = extension };
            }
        }


        private void DeleteLicenseImageAsync()
        {
            AddEditDeviceModel.LicenseUrl = null;
            AddEditDeviceModel.IicenseUrlUploadRequest = new UploadRequest();
        }

    }
}