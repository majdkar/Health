using System.Collections.Generic;

namespace SchoolV01.Application.Features.Dashboards.Queries.GetData
{
    public class DashboardInfoDataResponse
    {
        public int Clinic { get; set; }
        public int Hospital { get; set; }
        public int Directorate { get; set; }
        public int DeviceWorked { get; set; }
        public int DeviceCalibration { get; set; }
        public int DeviceMaintenance { get; set; }
        public int Supplier { get; set; }

    }

}