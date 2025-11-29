using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SchoolV01.Application.Features.Devices.Commands;
using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Application.Requests.Devices;
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

namespace SchoolV01.Client.Pages.Devices
{
    public partial class Devices
    {
        [Inject] private IDeviceManager DeviceManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllDevicesResponse> _pagedData;
        private MudTable<GetAllDevicesResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        private bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar-");

        private ClaimsPrincipal _currentUser;
        private bool _canCreateDevices;
        private bool _canEditDevices;
        private bool _canViewDevices;
        private bool _canDeleteDevices;
        private bool _canExportDevices;
        private bool _canSearchDevices;
        private bool _canPreviewDevicesCard;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.Create)).Succeeded;
            _canEditDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.Edit)).Succeeded;
            _canViewDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.View)).Succeeded;
            _canDeleteDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.Delete)).Succeeded;
            _canExportDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.Export)).Succeeded;
            _canSearchDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.Search)).Succeeded;
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task<TableData<GetAllDevicesResponse>> ServerReload(TableState state, CancellationToken cancellation)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllDevicesResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedDevicesRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
            var response = await DeviceManager.GetAllPagedAsync(request);
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
            wb.Properties.Title = "Devices Report";
            wb.Properties.Subject = "Devices Report";

            var ws = wb.Worksheets.Add("Devices");

            ws.Cell(1, 1).Value = (IsArabic ? "أسم الجهاز" : "Device Name");
            ws.Cell(1, 2).Value = (IsArabic ? "أسم الجهاز الانكليزي" : "Device En-Name");
            ws.Cell(1, 3).Value = (IsArabic ? "المورد" : "Supplier");
            ws.Cell(1, 4).Value = (IsArabic ? "فئة الجهاز" : "Device Category");
            ws.Cell(1, 5).Value = (IsArabic ? "فئة الجهاز الفرعية" : "Device Sub Category");
            ws.Cell(1, 6).Value = (IsArabic ? "النوع" : "Type");
            ws.Cell(1, 7).Value = (IsArabic ? "مستوصف" : "Clinic");
            ws.Cell(1, 8).Value = (IsArabic ? "مشتسفى" : "Hospital");
            ws.Cell(1, 9).Value = (IsArabic ? "موديل" : "Model");
            ws.Cell(1, 10).Value = (IsArabic ? "سيريال نمبر" : "SerialNumber");
            ws.Cell(1, 11).Value = (IsArabic ? "كود" : "Code");
            ws.Cell(1, 12).Value = (IsArabic ? "سنة الصنع" : "Year");
            ws.Cell(1, 13).Value = (IsArabic ? "تاريخ تشغيل" : "Start Run");
            ws.Cell(1, 14).Value = (IsArabic ? "حالة الجهاز" : "Device Status");
          


            ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.SkyBlue;

            ws.Cell(1, 4).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 5).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 6).Style.Fill.BackgroundColor = XLColor.SkyBlue;

            ws.Cell(1, 7).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 8).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 9).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 10).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 11).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 12).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 13).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 14).Style.Fill.BackgroundColor = XLColor.SkyBlue;
           



            int rowNumber = 1;
            foreach (var row in _pagedData.ToList())
            {
                ws.Cell(rowNumber + 1, 1).Value = row.Name;
                ws.Cell(rowNumber + 1, 2).Value = row.EnglishName;
                ws.Cell(rowNumber + 1, 3).Value =(IsArabic ?  row.Supplier?.Name : row.Supplier?.EnglishName) ;
                ws.Cell(rowNumber + 1, 4).Value =(IsArabic ?  row.ProjectType?.Name : row.ProjectType?.EnglishName) ;
                ws.Cell(rowNumber + 1, 5).Value =(IsArabic ?  row.SubProjectType?.Name : row.SubProjectType?.EnglishName) ;
                ws.Cell(rowNumber + 1, 6).Value = row.ByType;
                ws.Cell(rowNumber + 1, 7).Value = (IsArabic ? row.Clinic?.Name : row.Clinic?.EnglishName);
                ws.Cell(rowNumber + 1, 8).Value = (IsArabic ? row.Hospital?.Name : row.Hospital?.EnglishName);
                ws.Cell(rowNumber + 1, 9).Value = row.Model;
                ws.Cell(rowNumber + 1, 10).Value = row.SerialNumber;
                ws.Cell(rowNumber + 1, 11).Value = row.Code;
                ws.Cell(rowNumber + 1, 12).Value = row.Year;
                ws.Cell(rowNumber + 1, 13).Value = row.StartRun;
                ws.Cell(rowNumber + 1, 14).Value = row.DeviceStatus;
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
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var Device = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Device != null)
                {
                    parameters.Add(nameof(AddEditDeviceModal.AddEditDeviceModel), new AddEditDeviceCommand
                    {
                        Id = Device.Id,
                         Name =Device.Name,
                         EnglishName = Device.EnglishName,
                      
                        StartRun = Device.StartRun,
                        Year = Device.Year,
                        SupplierId = Device.SupplierId,
                        SubProjectTypeId = Device.SubProjectTypeId,
                        SubSubProjectTypeId = Device.SubSubProjectTypeId,
                        LicenseUrl = Device.LicenseUrl,
                        ByType = Device.ByType,
                        ClinicId = Device.ClinicId,
                        Code = Device.Code,
                        HospitalId = Device.HospitalId,
                        Model = Device.Model,
                        ProjectTypeId = Device.ProjectTypeId,
                        SerialNumber = Device.SerialNumber,
                        DeviceStatus = Device.DeviceStatus,
                    });
                }
            }
            var options = new DialogOptions { Position = DialogPosition.TopCenter, CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true };
            var dialog =await _dialogService.ShowAsync<AddEditDeviceModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                OnSearch("");
            }
        }

        private async Task InvokeDetailsModal(int id = 0)
        {
            //var parameters = new DialogParameters { { nameof(DeviceDetails.Id), id } };

            //if (id != 0)
            //{
            //    var Device = _pagedData.FirstOrDefault(c => c.Id == id);

            //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
            //    var dialog =await _dialogService.ShowAsync<AddEditDeviceModal>(_localizer["Details"], parameters, options);
            //    var result = await dialog.Result;
            //    if (!result.Canceled)
            //    {
            //        OnSearch("");
            //    }


            //}


        }

        private void InvokeMaintenances(int id = 0)
        {
          
                _navigationManager.NavigateTo($"/Maintenances/{id}");
            
        }

        private void InvokeStatus(int id = 0)
        {
          
                _navigationManager.NavigateTo($"/DeviceStatuss/{id}");
            
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
                var response = await DeviceManager.DeleteAsync(id);
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