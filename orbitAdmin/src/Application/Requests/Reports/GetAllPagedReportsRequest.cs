using System;

namespace SchoolV01.Application.Requests.Reports
{
    public class GetAllPagedReportsRequest : PagedRequest
    {
        public string DeviceNameAr { get; set; }
        public string DeviceNameEn { get; set; }
        public string DeviceStatus { get; set; }
        public int ProjectTypeId { get; set; }
        public int SubProjectTypeId { get; set; }
        public int CityId { get; set; }
        public int ClinicId { get; set; }
        public int HospitalId { get; set; }
        public int DirectorateId { get; set; }
        public int Year { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? RunFrom { get; set; }
        public DateTime? RunTo { get; set; }
        public string Code { get; set; }


        public string SearchString { get; set; }

    }
}