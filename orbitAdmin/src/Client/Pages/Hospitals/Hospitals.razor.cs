using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SchoolV01.Application.Features.Hospitals.Commands;
using SchoolV01.Application.Features.Hospitals.Queries;
using SchoolV01.Application.Requests.Hospitals;
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

namespace SchoolV01.Client.Pages.Hospitals
{
    public partial class Hospitals
    {
        [Inject] private IHospitalManager HospitalManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllHospitalsResponse> _pagedData;
        private MudTable<GetAllHospitalsResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        private bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar-");

        private ClaimsPrincipal _currentUser;
        private bool _canCreateHospitals;
        private bool _canEditHospitals;
        private bool _canViewHospitals;
        private bool _canDeleteHospitals;
        private bool _canExportHospitals;
        private bool _canSearchHospitals;
        private bool _canPreviewHospitalsCard;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateHospitals = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Hospitals.Create)).Succeeded;
            _canEditHospitals = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Hospitals.Edit)).Succeeded;
            _canViewHospitals = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Hospitals.View)).Succeeded;
            _canDeleteHospitals = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Hospitals.Delete)).Succeeded;
            _canExportHospitals = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Hospitals.Export)).Succeeded;
            _canSearchHospitals = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Hospitals.Search)).Succeeded;
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task<TableData<GetAllHospitalsResponse>> ServerReload(TableState state, CancellationToken cancellation)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllHospitalsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedHospitalsRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
            var response = await HospitalManager.GetAllPagedAsync(request);
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
            wb.Properties.Title = "Hospitals Report";
            wb.Properties.Subject = "Hospitals Report";

            var ws = wb.Worksheets.Add("Hospitals");

            ws.Cell(1, 1).Value = (IsArabic ? "أسم المستشفى" : "Hospital Name");
            ws.Cell(1, 2).Value = (IsArabic ? "أسم المستشفى الانكليزي" : "Hospital En-Name");
            ws.Cell(1, 3).Value = (IsArabic ? "المدينة" : "Hospital En-Name");
          


            ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.SkyBlue;
           



            int rowNumber = 1;
            foreach (var row in _pagedData.ToList())
            {
                ws.Cell(rowNumber + 1, 1).Value = row.Name;
                ws.Cell(rowNumber + 1, 2).Value = row.EnglishName;
                ws.Cell(rowNumber + 1, 3).Value =(IsArabic ?  row.City.NameAr : row.City.NameEn) ;

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
                var Hospital = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Hospital != null)
                {
                    parameters.Add(nameof(AddEditHospitalModal.AddEditHospitalModel), new AddEditHospitalCommand
                    {
                        Id = Hospital.Id,
                         Name =Hospital.Name,
                         EnglishName = Hospital.EnglishName, 
                        CityId = Hospital.CityId,
                         ByDirectorate =Hospital.ByDirectorate,
                         DirectorateId =Hospital.DirectorateId,
                    });
                }
            }
            var options = new DialogOptions { Position = DialogPosition.TopCenter, CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog =await _dialogService.ShowAsync<AddEditHospitalModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                OnSearch("");
            }
        }

        private async Task InvokeDetailsModal(int id = 0)
        {
            //var parameters = new DialogParameters { { nameof(HospitalDetails.Id), id } };

            //if (id != 0)
            //{
            //    var Hospital = _pagedData.FirstOrDefault(c => c.Id == id);

            //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
            //    var dialog =await _dialogService.ShowAsync<AddEditHospitalModal>(_localizer["Details"], parameters, options);
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
                var response = await HospitalManager.DeleteAsync(id);
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