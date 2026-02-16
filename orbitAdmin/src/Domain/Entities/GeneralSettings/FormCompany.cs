using SchoolV01.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolV01.Domain.Entities.GeneralSettings
{
    public class FormCompany : AuditableEntity<int>
    {
        public string CompanyName { get; set; }
        public string FormNumber { get; set; }
        public string AgentName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceBrand { get; set; }
        public string Model { get; set; }


        public ICollection<FormCompanyAttachment> Attachments { get; set; } = new List<FormCompanyAttachment>();
    }
}
