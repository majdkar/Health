using SchoolV01.Domain.Entities.GeneralSettings;
using System;
namespace SchoolV01.Application.Features.Devices.Queries
{
    public class GetAllDevicesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string EnglishName { get; set; } = "";
       
        public string LicenseUrl { get; set; }


        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Code { get; set; }
        public int? Year { get; set; }
        public DateTime? StartRun { get; set; }



        public int? ProjectTypeId { get; set; }
        public ProjectType ProjectType { get; set; }
        public int? SubProjectTypeId { get; set; }
        public ProjectType SubProjectType { get; set; }

        public int? SubSubProjectTypeId { get; set; }
        public ProjectType SubSubProjectType { get; set; }

        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public string ByType { get; set; }   // مشفى ولا عيادة
        public int? ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        public int? HospitalId { get; set; }
        public Hospital Hospital { get; set; }
    }
}