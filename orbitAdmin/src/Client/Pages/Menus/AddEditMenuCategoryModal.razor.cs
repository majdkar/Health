using SchoolV01.Shared.ViewModels.Menus;
using SchoolV01.Client.Helpers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SchoolV01.Shared.Constants.Application;
using SchoolV01.Client.Extensions;

namespace SchoolV01.Client.Pages.Menus
{
    public partial class AddEditMenuCategoryModal
    {

        [Parameter]
        public MenuCategoryUpdateModel MenuCategoryModel { get; set; } = new();


        [CascadingParameter]
        private IMudDialogInstance  MudDialog { get; set; }
        private bool _processing = false;

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {

            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();

        }

        private async Task SaveAsync()
        {
            _processing = true;
            var content = HelperMethods.ToJson(MenuCategoryModel);
            HttpResponseMessage response;
            if (MenuCategoryModel.Id == 0)
            {
                response = await _httpClient.PostAsync(EndPoints.MenuCategories, content);
            }
            else
            {
                response = await _httpClient.PutAsync($"{EndPoints.MenuCategories}/{MenuCategoryModel.Id}", content);
            }
            if (response.IsSuccessStatusCode)
            {
                _snackBar.Add("Completed Successful!", Severity.Success);
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
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

