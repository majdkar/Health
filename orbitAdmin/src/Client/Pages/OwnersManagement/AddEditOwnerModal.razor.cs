using System;
using SchoolV01.Application.Features.Owners.Commands;
using SchoolV01.Application.Requests;
using SchoolV01.Client.Extensions;
using SchoolV01.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazored.FluentValidation;

using SchoolV01.Client.Infrastructure.Managers.OwnersManagement.Passport;
using SchoolV01.Client.Infrastructure.Managers.OwnersManagement.Owner;
using SchoolV01.Application.Features.Passports.Queries;
using System.Threading;

namespace SchoolV01.Client.Pages.OwnersManagement
{
    public partial class AddEditOwnerModal
    {
        [Inject] private IOwnerManager OwnerManager { get; set; }

        [Inject] private IPassportManager PassportManager { get; set; }
        //[Inject] private IBrandManager BrandManager { get; set; }

        [Parameter] public AddEditOwnerCommand AddEditOwnerModel { get; set; } = new();
        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private List<GetAllPassportsResponse> _passports = new();
        //private List<GetAllBrandsResponse> _brands = new();

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            var response = await OwnerManager.SaveAsync(AddEditOwnerModel);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            await LoadImageAsync();

            await LoadPassportsAsync();

        }


        private async Task LoadPassportsAsync()
        {
            var data = await PassportManager.GetAllAsync();
            if (data.Succeeded)
            {
                _passports = data.Data;
            }
        }

        /*private async Task LoadBrandsAsync()
        {
            var data = await BrandManager.GetAllAsync();
            if (data.Succeeded)
            {
                _brands = data.Data;
            }
        }*/

        private async Task LoadImageAsync()
        {
            var data = await OwnerManager.GetOwnerImageAsync(AddEditOwnerModel.Id);
            if (data.Succeeded)
            {
                var imageData = data.Data;
                if (!string.IsNullOrEmpty(imageData))
                {
                    AddEditOwnerModel.ImageDataURL = imageData;
                }
            }
        }

        private void DeleteAsync()
        {
            AddEditOwnerModel.ImageDataURL = null;
            AddEditOwnerModel.UploadRequest = new UploadRequest();
        }

        private IBrowserFile _file;

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                var extension = Path.GetExtension(_file.Name);
                var format = "image/png";
                var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
                var buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream().ReadAsync(buffer);
                AddEditOwnerModel.ImageDataURL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                AddEditOwnerModel.UploadRequest = new UploadRequest { Data = buffer, UploadType = Application.Enums.UploadType.Owner, Extension = extension };
            }
        }

        private async Task<IEnumerable<int>> SearchPassports(string value, CancellationToken token)
        {
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _passports.Select(x => x.Id);

            return _passports.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }

        /* private async Task<IEnumerable<int>> SearchBrands(string value)
         {
             // In real life use an asynchronous function for fetching data from an api.
             await Task.Delay(5);

             // if text is null or empty, show complete list
             if (string.IsNullOrEmpty(value))
                 return _brands.Select(x => x.Id);

             return _brands.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                 .Select(x => x.Id);
         }*/
    }
}