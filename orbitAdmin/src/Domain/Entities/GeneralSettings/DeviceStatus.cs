using System;
using SchoolV01.Domain.Contracts;

namespace SchoolV01.Domain.Entities.GeneralSettings
{
    public class DeviceStatus : AuditableEntity<int>
    {


        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public DateTime? DeviceStatusDate { get; set; }
        public string Status { get; set; } = "";
  

    }
}