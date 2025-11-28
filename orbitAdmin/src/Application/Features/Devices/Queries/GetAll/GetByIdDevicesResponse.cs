using SchoolV01.Domain.Entities.GeneralSettings;
using System;
namespace SchoolV01.Application.Features.Devices.Queries
{
    public class GetByIdDevicesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string LicenseUrl { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Code { get; set; }
        public string DeviceStatus { get; set; }
        public string ByType { get; set; }
        public int? Year { get; set; }
        public DateTime? StartRun { get; set; }

        public ProjectTypeLiteDto ProjectType { get; set; }
        public ProjectTypeLiteDto SubProjectType { get; set; }
        public ProjectTypeLiteDto SubSubProjectType { get; set; }

        public SupplierLiteDto Supplier { get; set; }
        public ClinicLiteDto Clinic { get; set; }
        public HospitalLiteDto Hospital { get; set; }
    }


    public class ProjectTypeLiteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }
    public class SupplierLiteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }
    public class ClinicLiteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }
    public class HospitalLiteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }

}