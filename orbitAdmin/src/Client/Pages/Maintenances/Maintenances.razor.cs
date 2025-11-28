using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Application.Features.Maintenances.Commands;
using SchoolV01.Application.Features.Maintenances.Queries;
using SchoolV01.Application.Requests.Maintenances;
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

namespace SchoolV01.Client.Pages.Maintenances
{
    public partial class Maintenances
    {
        [Inject] private IMaintenanceManager MaintenanceManager { get; set; }
        [Inject] private IDeviceManager DeviceManager { get; set; }


        [Parameter]
        public int DeviceId { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllMaintenancesResponse> _pagedData;
        private MudTable<GetAllMaintenancesResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        private bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar-");
        private GetByIdDevicesResponse DeviceInfo { get; set; } = new();

        private ClaimsPrincipal _currentUser;
        private bool _canCreateMaintenances;
        private bool _canEditMaintenances;
        private bool _canViewMaintenances;
        private bool _canDeleteMaintenances;
        private bool _canExportMaintenances;
        private bool _canSearchMaintenances;
        private bool _canPreviewMaintenancesCard;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateMaintenances = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Maintenances.Create)).Succeeded;
            _canEditMaintenances = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Maintenances.Edit)).Succeeded;
            _canViewMaintenances = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Maintenances.View)).Succeeded;
            _canDeleteMaintenances = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Maintenances.Delete)).Succeeded;
            _canExportMaintenances = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Maintenances.Export)).Succeeded;
            _canSearchMaintenances = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Maintenances.Search)).Succeeded;

            await LoadDeviceInfo();

            _loaded = true;

            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task<TableData<GetAllMaintenancesResponse>> ServerReload(TableState state, CancellationToken cancellation)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllMaintenancesResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedMaintenancesRequest { DeviceId = DeviceId, PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
            var response = await MaintenanceManager.GetAllPagedAsync(request);
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

        private async Task LoadDeviceInfo()
        {
            var response = await DeviceManager.GetAsync(DeviceId);

            if (response.Succeeded)
            {
                DeviceInfo = response.Data;
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
            wb.Properties.Title = "Maintenances Report";
            wb.Properties.Subject = "Maintenances Report";

            var ws = wb.Worksheets.Add("Maintenances");

            ws.Cell(1, 1).Value = (IsArabic ? "تاريخ الصيانة" : "Maintenance Date");
            ws.Cell(1, 2).Value = (IsArabic ? "الوصف" : "Description");
          


            ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.SkyBlue;
           



            int rowNumber = 1;
            foreach (var row in _pagedData.ToList())
            {
                ws.Cell(rowNumber + 1, 1).Value = row.MaintenanceDate?.ToShortDateString();
                ws.Cell(rowNumber + 1, 2).Value = row.Description;

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
                FileName = $"Employees_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                MimeType = ApplicationConstants.MimeTypes.OpenXml
            });
            _snackBar.Add(_localizer["Employees Report exported"], Severity.Success);
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters { };

            if (id != 0)
            {
                var Maintenance = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Maintenance != null)
                {
                    parameters.Add(nameof(AddEditMaintenanceModal.AddEditMaintenanceModel), new AddEditMaintenanceCommand
                    {
                        Id = Maintenance.Id,
                        Description = Maintenance.Description,
                        MaintenanceDate = Maintenance.MaintenanceDate,
                        DeviceId = Maintenance.DeviceId,
                    });
                }
            }
            else
            {
                parameters.Add(nameof(AddEditMaintenanceModal.AddEditMaintenanceModel), new AddEditMaintenanceCommand
                {
                    DeviceId = DeviceId
                });
            }
                var options = new DialogOptions { Position = DialogPosition.TopCenter, CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog =await _dialogService.ShowAsync<AddEditMaintenanceModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                OnSearch("");
            }
        }

        private async Task InvokeDetailsModal(int id = 0)
        {
            //var parameters = new DialogParameters { { nameof(MaintenanceDetails.Id), id } };

            //if (id != 0)
            //{
            //    var Maintenance = _pagedData.FirstOrDefault(c => c.Id == id);

            //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
            //    var dialog =await _dialogService.ShowAsync<AddEditMaintenanceModal>(_localizer["Details"], parameters, options);
            //    var result = await dialog.Result;
            //    if (!result.Canceled)
            //    {
            //        OnSearch("");
            //    }


            //}


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
                var response = await MaintenanceManager.DeleteAsync(id);
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