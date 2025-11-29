using SchoolV01.Domain.Entities.GeneralSettings;
using System;
namespace SchoolV01.Application.Features.DeviceStatuss.Queries
{
    public class GetAllDeviceStatussResponse
    {
        public int Id { get; set; }
        public DateTime? DeviceStatusDate { get; set; }
        public string Status { get; set; } = "";

        public int DeviceId { get; set; }
        public Device Device { get; set; }
    }
}