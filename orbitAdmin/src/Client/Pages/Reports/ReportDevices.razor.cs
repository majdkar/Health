using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using Polly;
using SchoolV01.Application.Features.Cities.Queries;
using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Application.Features.Directorates.Queries;
using SchoolV01.Application.Features.Hospitals.Queries;
using SchoolV01.Application.Features.ProjectTypes.Queries;
using SchoolV01.Application.Features.Reports;
using SchoolV01.Application.Features.Suppliers.Queries;
using SchoolV01.Application.Requests.Devices;
using SchoolV01.Application.Requests.Reports;
using SchoolV01.Client.Extensions;
using SchoolV01.Client.Infrastructure.Managers.GeneralSettings;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using SchoolV01.Shared.Constants.Permission;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolV01.Client.Pages.Reports
{
    public partial class ReportDevices
    {

        [Inject] private IReportManager ReportManager { get; set; }
        [Inject] private IClinicManager ClinicManager { get; set; }
        [Inject] private IHospitalManager HospitalManager { get; set; }
        [Inject] private IProjectTypeManager ProjectTypeManager { get; set; }
        [Inject] private IDirectorateManager DirectorateManager { get; set; }
        [Inject] private ISupplierManager SupplierManager { get; set; }
        [Inject] private ICityManager CityManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllCitiesResponse> _city;
        private List<GetAllProjectTypesResponse> _projectTypes = new();
        private List<GetAllClinicsResponse> _clinic = new();
        private List<GetAllHospitalsResponse> _hospital = new();
        private List<GetAllDirectoratesResponse> _directorate = new();
        private List<GetAllSuppliersResponse> _supplier = new();


        private IEnumerable<GetAllDeviceReportsResponse> _pagedData;
        private MudTable<GetAllDeviceReportsResponse> _table;
        private TableState tablestate = new TableState { Page = 1, PageSize = 500 };
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;


        public string DeviceNameAr { get; set; }
        public string ByType { get; set; }
        public string DeviceNameEn { get; set; }
        public string DeviceStatus { get; set; }
        public int Year { get; set; }
        public int ProjectTypeId { get; set; }
        public int SubProjectTypeId { get; set; }
        public int CityId { get; set; }
        public int ClinicId { get; set; }
        public int HospitalId { get; set; }
        public int DirectorateId { get; set; }
        public string SerialNumber { get; set; }
        public string Code { get; set; }

        public DateTime? runfromdate { get; set; }
        public DateTime? runtodate { get; set; }


        private bool IsArabic => CultureInfo.CurrentCulture.Name.Contains("ar-");

        public bool IsSearchAdvanced { get; set; } = true;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateDevices;
        private bool _canEditDevices;
        private bool _canDeleteDevices;
        private bool _canExportDevices;
        private bool _canSearchDevices;
        private bool _canViewDevices;
        private bool _canPreviewDevicesCard;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.Create)).Succeeded;
            _canEditDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.Edit)).Succeeded;
            _canDeleteDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.Delete)).Succeeded;
            _canExportDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.View)).Succeeded;
            _canSearchDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.View)).Succeeded;
            _canViewDevices = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.View)).Succeeded;
            _canPreviewDevicesCard = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Devices.View)).Succeeded;
            _loaded = true;
            await LoadDataAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }


        private async Task LoadDataAsync()
        {
            await LoadPrincedom();
            await LoadSuppliersAsync();


            await LoadProjectType();
            await LoadClinic();
            await LoadHospital();
            await LoadDirectorate();
            await LoadData();


        }

        protected async Task LoadDirectorate()
        {
            var response = await DirectorateManager.GetAllAsync();
            if (response.Succeeded)
            {
                _directorate = response.Data;
            }
        }
        

        protected async Task LoadHospital()
        {
            var response = await HospitalManager.GetAllAsync();
            if (response.Succeeded)
            {
                _hospital = response.Data;
            }
        }

        protected async Task LoadClinic()
        {
            var response = await ClinicManager.GetAllAsync();
            if (response.Succeeded)
            {
                _clinic = response.Data;
            }
        }

        protected async Task LoadPrincedom()
        {
            var response = await CityManager.GetAllAsync();
            if (response.Succeeded)
            {
                _city = response.Data;
            }
        }

        protected async Task SearchProjectType()
        {
            var response = await ProjectTypeManager.GetAllAsync();
            if (response.Succeeded)
            {
                _projectTypes = response.Data;
            }
        }

        protected async Task LoadProjectType()
        {
            var response = await ProjectTypeManager.GetAllAsync();
            if (response.Succeeded)
            {
                _projectTypes = response.Data;
            }
        }



        private async Task LoadSuppliersAsync()
        {
            var data = await SupplierManager.GetAllAsync();
            if (data.Succeeded)
            {
                _supplier = data.Data;
            }
        }



        protected override async Task OnParametersSetAsync()
        {
            StateHasChanged();
            if (_table != null)
                await LoadData();
            _loaded = true;
        }
        private async Task<TableData<GetAllDeviceReportsResponse>> ServerReload()
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                tablestate.Page = 0;
            }
            await LoadData();
            return new TableData<GetAllDeviceReportsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData()
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(tablestate.SortLabel))
            {
                orderings = tablestate.SortDirection != SortDirection.None ? new[] { $"{tablestate.SortLabel} {tablestate.SortDirection}" } : new[] { $"{tablestate.SortLabel}" };
            }

            var request = new GetAllPagedReportsRequest { Code = Code, DeviceNameAr = DeviceNameAr,DeviceNameEn = DeviceNameEn,
                DeviceStatus = DeviceStatus, CityId = CityId,ClinicId = ClinicId, DirectorateId = DirectorateId,
                HospitalId = HospitalId, ProjectTypeId = ProjectTypeId, SerialNumber = SerialNumber
                , SubProjectTypeId = SubProjectTypeId, Year = Year,   
                 RunFrom = runfromdate ,RunTo = runtodate , PageNumber = tablestate.Page, PageSize = tablestate.PageSize, SearchString = _searchString, Orderby = orderings };


            if (IsSearchAdvanced == true)
            {

                var data = await ReportManager.GetAllPagedReportDeviceAsync(request);

                if (data.Succeeded)
                {
                    _totalItems = data.TotalCount;
                    _currentPage = data.CurrentPage;
                    _pagedData = data.Data;
                }
                else
                {
                    foreach (var message in data.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }


            }
            else
            {
                //var response = new PaginatedResult<GetAllDeviceReportsResponse>(new List<GetAllDeviceReportsResponse>());
                //response = await ReportManager.GetDevicesReportMoreCompanyQuery(request);

                //if (response.Succeeded)
                //{
                //    _totalItems = response.TotalCount;
                //    _currentPage = response.CurrentPage;
                //    _pagedData = response.Data;
                //}
                //else
                //{
                //    foreach (var message in response.Messages)
                //    {
                //        _snackBar.Add(message, Severity.Error);
                //    }
                //}
            }
        }
        private async Task PrintMe()
        {
            //await _jsRuntime.InvokeVoidAsync("myPrint", _navigationManager.BaseUri);

            //var request = new GetAllPagedDevicesRequest { TypeDevice = TypeClient, PageNumber = tablestate.Page, PageSize = tablestate.PageSize, SearchString = _searchString, Orderby = null };

            //var response = await DeviceManager.GetAllByForDownloadReportAsync(request, fullname, identityNumber, nationalityId, jobTitle, employmentfromdate?.ToString("yyyy-MM-dd"), employmenttodate?.ToString("yyyy-MM-dd"), ClientId, false);

            //if (response.Succeeded)
            //{
            //    await _jsRuntime.InvokeVoidAsync("open", response.Data, "_blank");
            //}
        }
        private async Task OnSearch(string text)
        {
            IsSearchAdvanced = true;
             DeviceNameAr = null;
            DeviceNameAr = null;
            Code = null;
            SerialNumber = null;
            Year = 0;
            runtodate = null;
            runfromdate = null;
            CityId = 0;
            ClinicId = 0;
            HospitalId = 0;
            DirectorateId = 0;
            DeviceStatus = null;
            ProjectTypeId = 0;
            SubProjectTypeId = 0;
            _searchString = text;
            await LoadData();
        }



        private async Task<IEnumerable<int>> SearchProjectType(string value, CancellationToken cancellationToken)
        {
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _projectTypes.Select(x => x.Id);

            return _projectTypes.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.EnglishName.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }


        private async Task<IEnumerable<int>> SearchCities(string value, CancellationToken cancellationToken)
        {
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _city.Select(x => x.Id);

            return _city.Where(x => x.NameAr.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.NameEn.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }


        private async Task<IEnumerable<int>> SearchClinics(string value,CancellationToken cancellationToken)
        {
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _clinic.Select(x => x.Id);

            return _clinic.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.EnglishName.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }

       

        private async Task<IEnumerable<int>> SearchHospitals(string value, CancellationToken cancellationToken)
        {
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _hospital.Select(x => x.Id);

            return _hospital.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.EnglishName.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }

       

       

        private async Task SearchAdvance()
        {
            IsSearchAdvanced = true;
            StateHasChanged();
            await LoadData();
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
                ws.Cell(1, 2).Value = (IsArabic ? "أسم الجهاز الانكبزي" : "Company En-Name");
                ws.Cell(1, 3).Value = (IsArabic ? "فئة الجهاز" : "Device Category");
                ws.Cell(1, 4).Value = (IsArabic ? "فئة الجهاز الفرعية" : "Device Sub Category");
                ws.Cell(1, 5).Value = (IsArabic ? "حالة الجهاز" : "Device Status");
           
                ws.Cell(1, 6).Value = (IsArabic ? "تابع الى" : "ByType");
                ws.Cell(1, 7).Value = (IsArabic ? "المستوصف" : "Clinic");
                ws.Cell(1, 8).Value = (IsArabic ? "المشفى" : "Hospital");
                ws.Cell(1, 9).Value = (IsArabic ? "كود الوزارة" : "Code");

                ws.Cell(1, 10).Value = (IsArabic ? "سنة الصنع" : "Year");
                ws.Cell(1, 11).Value = (IsArabic ? "موديل" : "Model");
                ws.Cell(1, 12).Value = (IsArabic ? "سيريال نمبر" : "SerialNumber");


                ws.Cell(1, 13).Value = (IsArabic ? "تاريخ بداية التسغيل" : "Start Run");
                ws.Cell(1, 14).Value = (IsArabic ? "المورد" : "Supplier");


                


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
                    ws.Cell(rowNumber + 1, 2).Value =row.EnglishName;
                    ws.Cell(rowNumber + 1, 3).Value = row.ProjectTypeNameAr;
                    ws.Cell(rowNumber + 1, 4).Value = row.SubProjectTypeNameAr;
                    ws.Cell(rowNumber + 1, 5).Value = row.DeviceStatus;
                 
                    ws.Cell(rowNumber + 1, 6).Value = row.ByType;
                    ws.Cell(rowNumber + 1, 7).Value = row.ClinicNameAr;
                    ws.Cell(rowNumber + 1, 8).Value = row.HospitalNameAr;
                    ws.Cell(rowNumber + 1, 9).Value = row.Code;
                    ws.Cell(rowNumber + 1, 10).Value = row.Year;
             
                    ws.Cell(rowNumber + 1, 11).Value = row.Model;
                ws.Cell(rowNumber + 1, 12).Value = row.SerialNumber;
                ws.Cell(rowNumber + 1, 13).Value = row.StartRun?.ToShortDateString();
                ws.Cell(rowNumber + 1, 14).Value = row.SupplierNameAr;

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
                FileName = $"Devices_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                MimeType = ApplicationConstants.MimeTypes.OpenXml
            });
            _snackBar.Add(_localizer["Devices Report exported"], Severity.Success);
        }









    }
}
