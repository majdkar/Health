using Microsoft.AspNetCore.Components;
using MudBlazor;
using SchoolV01.Application.Features.Dashboards.Queries.GetData;
using SchoolV01.Client.Infrastructure.Managers.Dashboard;
using System.Threading.Tasks;

namespace SchoolV01.Client.Pages.Content
{
    public partial class Dashboard
    {
        [Inject] private IDashboardManager DashboardManager { get; set; }

        private DashboardDataResponse _data;


        private bool _isProcessingDashboardDataResponse = false;
        protected override async Task OnInitializedAsync()
        {
            _isProcessingDashboardDataResponse = true;

            await Task.WhenAll(LoadDashboardData());

            _isProcessingDashboardDataResponse = false;

        }
        private async Task LoadDashboardData()
        {

            var response = await DashboardManager.GetDataAsync(1);
            if (response.Succeeded)
            {
                _data = response.Data;
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