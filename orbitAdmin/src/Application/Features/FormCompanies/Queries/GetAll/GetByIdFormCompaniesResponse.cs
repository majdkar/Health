using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Collections.Generic;
namespace SchoolV01.Application.Features.FormCompanies.Queries
{
    public class GetByIdFormCompaniesResponse
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