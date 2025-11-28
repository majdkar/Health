using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class DeviceFilterSpecification : HeroSpecification<Device>
    {
        public DeviceFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.EnglishName.Contains(searchString) ||p.Name.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }

   
    }
    public static class DeviceQueries
    {
        public static IQueryable<GetAllDevicesResponse> SelectDevice(this IQueryable<Device> source)
        {
            Expression<Func<Device, GetAllDevicesResponse>> expression = e => new GetAllDevicesResponse
            {
                Id = e.Id,
                EnglishName =e.EnglishName,
                Name =e.Name,
                StartRun =e.StartRun,
                Year =e.Year,
                SupplierId =e.SupplierId,
                SubProjectTypeId =e.SubProjectTypeId,
                SubSubProjectTypeId =e.SubSubProjectTypeId,
                Supplier =e.Supplier,
                Clinic =e.Clinic,
                 LicenseUrl =e.LicenseUrl,
                 Hospital =e.Hospital,
                 ByType =e.ByType,
                 ClinicId =e.ClinicId,
                 Code =e.Code,
                 HospitalId =e.HospitalId,
                 Model=e.Model,
                 ProjectType =e.ProjectType,
                 ProjectTypeId =e.ProjectTypeId,
                 SerialNumber =e.SerialNumber,
                 SubProjectType= e.SubProjectType,
                 SubSubProjectType =e.SubSubProjectType,
                 DeviceStatus =e.DeviceStatus
            };
            return source.Select(expression);
        }
    }
}
