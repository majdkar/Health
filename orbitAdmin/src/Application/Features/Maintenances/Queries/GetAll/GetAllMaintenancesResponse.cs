using SchoolV01.Domain.Entities.GeneralSettings;
using System;
namespace SchoolV01.Application.Features.Maintenances.Queries
{
    public class GetAllMaintenancesResponse
    {
        public int Id { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string Description { get; set; } = "";

        public int DeviceId { get; set; }
        public Device Device { get; set; }
    }
}