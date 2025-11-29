using SchoolV01.Application.Features.DeviceStatuss.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class DeviceStatusFilterSpecification : HeroSpecification<DeviceStatus>
    {
        public DeviceStatusFilterSpecification(string searchString,int DeviceId)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Status.Contains(searchString) && p.DeviceId == DeviceId;
            }
            else
            {
                Criteria = p => true && p.DeviceId == DeviceId;
            }
        }

   
    }
    public static class DeviceStatusQueries
    {
        public static IQueryable<GetAllDeviceStatussResponse> SelectDeviceStatus(this IQueryable<DeviceStatus> source)
        {
            Expression<Func<DeviceStatus, GetAllDeviceStatussResponse>> expression = e => new GetAllDeviceStatussResponse
            {
                Id = e.Id,
                Status =e.Status,
                DeviceStatusDate =e.DeviceStatusDate,
                 DeviceId =e.DeviceId,
                 Device =e.Device,
            };
            return source.Select(expression);
        }
    }
}
