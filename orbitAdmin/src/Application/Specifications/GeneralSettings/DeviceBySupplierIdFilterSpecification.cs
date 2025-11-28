using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class DeviceBySupplierIdFilterSpecification : HeroSpecification<Device>
    {
        public DeviceBySupplierIdFilterSpecification(string searchString,int SupplierId)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => (p.EnglishName.Contains(searchString) ||p.Name.Contains(searchString)) && p.SupplierId == SupplierId;
            }
            else
            {
                Criteria = p => true && p.SupplierId == SupplierId;
            }
        }

   
    }
}
