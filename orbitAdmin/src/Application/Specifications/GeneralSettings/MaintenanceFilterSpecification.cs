using SchoolV01.Application.Features.Maintenances.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class MaintenanceFilterSpecification : HeroSpecification<Maintenance>
    {
        public MaintenanceFilterSpecification(string searchString,int DeviceId)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Description.Contains(searchString) && p.DeviceId == DeviceId;
            }
            else
            {
                Criteria = p => true && p.DeviceId == DeviceId;
            }
        }

   
    }
    public static class MaintenanceQueries
    {
        public static IQueryable<GetAllMaintenancesResponse> SelectMaintenance(this IQueryable<Maintenance> source)
        {
            Expression<Func<Maintenance, GetAllMaintenancesResponse>> expression = e => new GetAllMaintenancesResponse
            {
                Id = e.Id,
                Description =e.Description,
                MaintenanceDate =e.MaintenanceDate,
                 DeviceId =e.DeviceId,
                 Device =e.Device,
            };
            return source.Select(expression);
        }
    }
}
