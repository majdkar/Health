using SchoolV01.Application.Requests.Reports;
using System;
using System.Linq;

namespace SchoolV01.Client.Infrastructure.Routes
{
    public static class ReportsEndpoints
    {
        public static string Endpoints = "api/v1/Reports";

        public static string GetAllPaged(GetAllPagedReportsRequest request)
        {
            var url = $"{Endpoints}?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

            void AddParam(string name, object value)
            {
                if (value == null) return;
                if (value is string s && string.IsNullOrWhiteSpace(s)) return;
                if (value is int i && i <= 0) return;

                // التاريخ nullable
                url += $"&{name}={value}";
            }

            AddParam("searchString", request.SearchString);
            AddParam("DeviceNameAr", request.DeviceNameAr);
            AddParam("DeviceNameEn", request.DeviceNameEn);
            AddParam("DeviceStatus", request.DeviceStatus);
            AddParam("ProjectTypeId", request.ProjectTypeId);
            AddParam("SubProjectTypeId", request.SubProjectTypeId);
            AddParam("CityId", request.CityId);
            AddParam("ClinicId", request.ClinicId);
            AddParam("HospitalId", request.HospitalId);
            AddParam("DirectorateId", request.DirectorateId);
            AddParam("Year", request.Year);
            AddParam("SerialNumber", request.SerialNumber);

            if (request.RunFrom.HasValue)
                AddParam("RunFrom", request.RunFrom.Value.ToString("yyyy-MM-dd"));

            if (request.RunTo.HasValue)
                AddParam("RunTo", request.RunTo.Value.ToString("yyyy-MM-dd"));

            // OrderBy
            if (request.Orderby?.Any() == true)
            {
                var order = string.Join(",", request.Orderby);
                url += $"&orderBy={order}";
            }

            return url;
        }
    }

}