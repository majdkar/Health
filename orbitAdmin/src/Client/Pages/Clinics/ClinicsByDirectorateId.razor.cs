using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SchoolV01.Application.Features.Clinics.Commands;
using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Application.Requests.Clinics;
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

namespace SchoolV01.Client.Pages.Clinics
{
    public partial class ClinicsByDirectorateId
    {
        [Inject] private IClinicManager ClinicManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        [Parameter]
        public int DirectorateId { get; set; }

        private IEnumerable<GetAllClinicsResponse> _pagedData;
        private MudTable<GetAllClinicsResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        private bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar-");

        private ClaimsPrincipal _currentUser;
        private bool _canCreateClinics;
        private bool _canEditClinics;
        private bool _canViewClinics;
        private bool _canDeleteClinics;
        private bool _canExportClinics;
        private bool _canSearchClinics;
        private bool _canPreviewClinicsCard;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateClinics = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Clinics.Create)).Succeeded;
            _canEditClinics = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Clinics.Edit)).Succeeded;
            _canViewClinics = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Clinics.View)).Succeeded;
            _canDeleteClinics = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Clinics.Delete)).Succeeded;
            _canExportClinics = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Clinics.Export)).Succeeded;
            _canSearchClinics = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Clinics.Search)).Succeeded;
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task<TableData<GetAllClinicsResponse>> ServerReload(TableState state, CancellationToken cancellation)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllClinicsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedClinicsRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
            var response = await ClinicManager.GetAllPagedByDirectorateIdAsync(request, DirectorateId);
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
            wb.Properties.Title = "Clinics Report";
            wb.Properties.Subject = "Clinics Report";

            var ws = wb.Worksheets.Add("Clinics");

            ws.Cell(1, 1).Value = (IsArabic ? "أسم المستشفى" : "Clinic Name");
            ws.Cell(1, 2).Value = (IsArabic ? "أسم المستشفى الانكليزي" : "Clinic En-Name");
            ws.Cell(1, 3).Value = (IsArabic ? "المدينة" : "Clinic En-Name");
          


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
                var Clinic = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Clinic != null)
                {
                    parameters.Add(nameof(AddEditClinicModal.AddEditClinicModel), new AddEditClinicCommand
                    {
                        Id = Clinic.Id,
                         Name =Clinic.Name,
                         EnglishName = Clinic.EnglishName, 
                        CityId = Clinic.CityId,
                         ByDirectorate =Clinic.ByDirectorate,
                         DirectorateId =Clinic.DirectorateId,
                    });
                }
            }
            var options = new DialogOptions { Position = DialogPosition.TopCenter, CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog =await _dialogService.ShowAsync<AddEditClinicModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                OnSearch("");
            }
        }

        private async Task InvokeDetailsModal(int id = 0)
        {
            //var parameters = new DialogParameters { { nameof(ClinicDetails.Id), id } };

            //if (id != 0)
            //{
            //    var Clinic = _pagedData.FirstOrDefault(c => c.Id == id);

            //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
            //    var dialog =await _dialogService.ShowAsync<AddEditClinicModal>(_localizer["Details"], parameters, options);
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
                var response = await ClinicManager.DeleteAsync(id);
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