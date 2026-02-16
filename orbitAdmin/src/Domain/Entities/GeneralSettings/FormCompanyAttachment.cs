using SchoolV01.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchoolV01.Domain.Entities.GeneralSettings
{
    public class FormCompanyAttachment : AuditableEntity<int>
    {
        public int FormCompanyId { get; set; }
        [JsonIgnore]
        public FormCompany FormCompany { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }

    }
}
