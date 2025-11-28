using SchoolV01.Domain.Entities.GeneralSettings;
using System;
namespace SchoolV01.Application.Features.Reports
{
    public class GetAllDeviceReportsResponse
    {
        public string Name { get; set; }

        public string EnglishName { get; set; } = "";

        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Code { get; set; }
        public string DeviceStatus { get; set; }
        public int? Year { get; set; }
        public DateTime? StartRun { get; set; }


        public string ProjectTypeNameAr { get; set; }
        public string ProjectTypeNameEn { get; set; }

        public string SubProjectTypeNameAr { get; set; }
        public string SubProjectTypeNameEn { get; set; }

        public string SupplierNameAr { get; set; }
        public string SupplierNameEn { get; set; }

        public string ByType { get; set; }   // مشفى ولا عيادة


        public string ClinicNameAr { get; set; }
        public string ClinicNameEn { get; set; }
        public string HospitalNameAr { get; set; }
        public string HospitalNameEn { get; set; }

    }
}