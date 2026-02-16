using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SchoolV01.Application.Features.FormCompanies.Queries;
using SchoolV01.Application.Features.FormCompanys.Commands;
using SchoolV01.Application.Requests.FormCompanies;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using SchoolV01.Shared.Constants.Permission;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolV01.Client.Pages.Forms
{
    public partial class Forms
    {
        [Inject] private IFormCompanyManager FormCompanyManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllFormCompaniesResponse> _pagedData;
        private MudTable<GetAllFormCompaniesResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = false;
        private bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar-");

        private ClaimsPrincipal _currentUser;
        private bool _canViewFormCompanys;
        private bool _canDeleteFormCompanys;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canViewFormCompanys = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Forms.View)).Succeeded;
            _canDeleteFormCompanys = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Forms.Delete)).Succeeded;
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }


        private void Details(int id = 0)
        {

            _navigationManager.NavigateTo($"/formcompany/details/{id}");

        }


        private async Task<TableData<GetAllFormCompaniesResponse>> ServerReload(TableState state, CancellationToken cancellation)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllFormCompaniesResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedFormCompaniesRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
            var response = await FormCompanyManager.GetAllPagedAsync(request);
            if (response.Succeeded)
            {
                _totalItems = response.TotalCount;
                _currentPage = response.CurrentPage;
                _pagedData = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task ExportToExcel()
        {
            if (!_pagedData.Any())
            {
                _snackBar.Add(_localizer["No Data To Export"], Severity.Normal);
                return;
            }
            var wb = new XLWorkbook();
            wb.Properties.Author = "FSIT";
            wb.Properties.Title = "Forms Report";
            wb.Properties.Subject = "Forms Report";

            var ws = wb.Worksheets.Add("Forms");

            ws.Cell(1, 1).Value = (IsArabic ? "أسم الشركة" : "Company Name");
            ws.Cell(1, 2).Value = (IsArabic ? "أسم الوكيل الانكليزي" : "Agent En-Name");
            ws.Cell(1, 3).Value = (IsArabic ? "نوع الجهاز" : "Device Type");
            ws.Cell(1, 4).Value = (IsArabic ? "ماركة الجهاز" : "Device Brand");
            ws.Cell(1, 5).Value = (IsArabic ? "موديل الصنع" : "Model");

          


            ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.SkyBlue;

            ws.Cell(1, 4).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 5).Style.Fill.BackgroundColor = XLColor.SkyBlue;

           



            int rowNumber = 1;
            foreach (var row in _pagedData.ToList())
            {
                ws.Cell(rowNumber + 1, 1).Value = row.CompanyName;
                ws.Cell(rowNumber + 1, 2).Value = row.AgentName;
                ws.Cell(rowNumber + 1, 3).Value = row.DeviceType ;
                ws.Cell(rowNumber + 1, 4).Value =row.DeviceBrand ;
                ws.Cell(rowNumber + 1, 5).Value =row.Model;
      
                rowNumber++;

            }
            ws.Columns().AdjustToContents();
            ws.Cells().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cells().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            MemoryStream XLSStream = new();
            wb.SaveAs(XLSStream);


            var data = Convert.ToBase64String(XLSStream.ToArray());

            await _jsRuntime.InvokeVoidAsync("Download", new
            {
                ByteArray = data,
                FileName = $"Forms_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                MimeType = ApplicationConstants.MimeTypes.OpenXml
            });
            _snackBar.Add(_localizer["Forms Report exported"], Severity.Success);
        }


   
        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog =await _dialogService.ShowAsync<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var response = await FormCompanyManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    OnSearch("");
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    OnSearch("");
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
    }
}