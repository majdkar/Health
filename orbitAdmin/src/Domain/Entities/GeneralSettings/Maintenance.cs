using System;
using SchoolV01.Domain.Contracts;

namespace SchoolV01.Domain.Entities.GeneralSettings
{
    public class Maintenance : AuditableEntity<int>
    {


        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public DateTime? MaintenanceDate { get; set; }
        public string Description { get; set; } = "";
  

    }
}