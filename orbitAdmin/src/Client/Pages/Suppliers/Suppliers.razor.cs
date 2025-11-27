using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SchoolV01.Application.Features.Suppliers.Commands;
using SchoolV01.Application.Features.Suppliers.Queries;
using SchoolV01.Application.Requests.Suppliers;
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

namespace SchoolV01.Client.Pages.Suppliers
{
    public partial class Suppliers
    {
        [Inject] private ISupplierManager SupplierManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllSuppliersResponse> _pagedData;
        private MudTable<GetAllSuppliersResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        private bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar-");

        private ClaimsPrincipal _currentUser;
        private bool _canCreateSuppliers;
        private bool _canEditSuppliers;
        private bool _canViewSuppliers;
        private bool _canDeleteSuppliers;
        private bool _canExportSuppliers;
        private bool _canSearchSuppliers;
        private bool _canPreviewSuppliersCard;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateSuppliers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Suppliers.Create)).Succeeded;
            _canEditSuppliers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Suppliers.Edit)).Succeeded;
            _canViewSuppliers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Suppliers.View)).Succeeded;
            _canDeleteSuppliers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Suppliers.Delete)).Succeeded;
            _canExportSuppliers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Suppliers.Export)).Succeeded;
            _canSearchSuppliers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Suppliers.Search)).Succeeded;
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task<TableData<GetAllSuppliersResponse>> ServerReload(TableState state, CancellationToken cancellation)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllSuppliersResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedSuppliersRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
            var response = await SupplierManager.GetAllPagedAsync(request);
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
            wb.Properties.Title = "Suppliers Report";
            wb.Properties.Subject = "Suppliers Report";

            var ws = wb.Worksheets.Add("Suppliers");

            ws.Cell(1, 1).Value = (IsArabic ? "أسم الشركة الموردة" : "Supplier Name");
            ws.Cell(1, 2).Value = (IsArabic ? "أسم الشركة المورد الانكليزي" : "Supplier En-Name");
            ws.Cell(1, 3).Value = (IsArabic ? "الماركة" : "Brand");
            ws.Cell(1, 4).Value = (IsArabic ? "البلد" : "Country");
            ws.Cell(1, 5).Value = (IsArabic ? "المدينة" : "City");
            ws.Cell(1, 6).Value = (IsArabic ? "الايميل" : "Email");
            ws.Cell(1, 7).Value = (IsArabic ? "الموبايل" : "Mobile");
            ws.Cell(1, 8).Value = (IsArabic ? "العنوان" : "Address");
          


            ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.SkyBlue;

            ws.Cell(1, 4).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 5).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 6).Style.Fill.BackgroundColor = XLColor.SkyBlue;

            ws.Cell(1, 7).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            ws.Cell(1, 8).Style.Fill.BackgroundColor = XLColor.SkyBlue;
           



            int rowNumber = 1;
            foreach (var row in _pagedData.ToList())
            {
                ws.Cell(rowNumber + 1, 1).Value = row.Name;
                ws.Cell(rowNumber + 1, 2).Value = row.EnglishName;
                ws.Cell(rowNumber + 1, 3).Value =(IsArabic ?  row.Position.Name : row.Position.EnglishName) ;
                ws.Cell(rowNumber + 1, 4).Value =(IsArabic ?  row.Country.NameAr : row.Country.NameEn) ;
                ws.Cell(rowNumber + 1, 5).Value =(IsArabic ?  row.City.NameAr : row.City.NameEn) ;
                ws.Cell(rowNumber + 1, 6).Value = row.Email;
                ws.Cell(rowNumber + 1, 7).Value = row.Mobile;
                ws.Cell(rowNumber + 1, 8).Value = row.Address;
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
                var Supplier = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Supplier != null)
                {
                    parameters.Add(nameof(AddEditSupplierModal.AddEditSupplierModel), new AddEditSupplierCommand
                    {
                        Id = Supplier.Id,
                         Name =Supplier.Name,
                         EnglishName = Supplier.EnglishName, 
                        CityId = Supplier.CityId,
                         Address =Supplier.Address,
                         Email =Supplier.Email,
                         Mobile =Supplier.Mobile,
                         CountryId =Supplier.CountryId,
                         PositionId =Supplier.PositionId,
                         LicenseUrl =Supplier.LicenseUrl,
                    });
                }
            }
            var options = new DialogOptions { Position = DialogPosition.TopCenter, CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true };
            var dialog =await _dialogService.ShowAsync<AddEditSupplierModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                OnSearch("");
            }
        }

        private async Task InvokeDetailsModal(int id = 0)
        {
            //var parameters = new DialogParameters { { nameof(SupplierDetails.Id), id } };

            //if (id != 0)
            //{
            //    var Supplier = _pagedData.FirstOrDefault(c => c.Id == id);

            //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
            //    var dialog =await _dialogService.ShowAsync<AddEditSupplierModal>(_localizer["Details"], parameters, options);
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
                var response = await SupplierManager.DeleteAsync(id);
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