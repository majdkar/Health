using SchoolV01.Application.Features.Reports;
using SchoolV01.Application.Features.Suppliers.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class ReportFilterSpecification : HeroSpecification<Device>
    {
        public ReportFilterSpecification(GetAllPagedDeviceReportsQuery request)
        {
            Expression<Func<Device, bool>> criteria = p => true;

            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                criteria = criteria.AndAlso(p =>
                    p.Name.Contains(request.SearchString) ||
                    p.EnglishName.Contains(request.SearchString) ||
                    p.SerialNumber.Contains(request.SearchString)
                );
            }

            if (!string.IsNullOrWhiteSpace(request.DeviceNameAr))
                criteria = criteria.AndAlso(p => p.Name.Contains(request.DeviceNameAr));

            if (!string.IsNullOrWhiteSpace(request.DeviceNameEn))
                criteria = criteria.AndAlso(p => p.EnglishName.Contains(request.DeviceNameEn));

            if (!string.IsNullOrWhiteSpace(request.DeviceStatus))
                criteria = criteria.AndAlso(p => p.DeviceStatus.ToString() == request.DeviceStatus);

            if (request.ProjectTypeId > 0)
                criteria = criteria.AndAlso(p => p.ProjectTypeId == request.ProjectTypeId);

            if (request.SubProjectTypeId > 0)
                criteria = criteria.AndAlso(p => p.SubProjectTypeId == request.SubProjectTypeId);

            
            if (request.CityId > 0)
                criteria = criteria.AndAlso(p => p.Clinic.CityId == request.CityId || p.Hospital.CityId == request.CityId);
           
            
            if (request.ClinicId > 0)
                criteria = criteria.AndAlso(p => p.ClinicId == request.ClinicId);

            if (request.HospitalId > 0)
                criteria = criteria.AndAlso(p => p.HospitalId == request.HospitalId);

            if (request.DirectorateId > 0)
                criteria = criteria.AndAlso(p => p.Clinic.DirectorateId == request.DirectorateId || p.Hospital.DirectorateId == request.DirectorateId);

            if (request.Year > 0)
                criteria = criteria.AndAlso(p => p.Year == request.Year);

            if (!string.IsNullOrWhiteSpace(request.SerialNumber))
                criteria = criteria.AndAlso(p => p.SerialNumber.Contains(request.SerialNumber));

            if (request.RunFrom.HasValue)
                criteria = criteria.AndAlso(p => p.StartRun >= request.RunFrom.Value);

            if (request.RunTo.HasValue)
                criteria = criteria.AndAlso(p => p.StartRun <= request.RunTo.Value);

            Criteria = criteria;
        }
                }

   
    
    
    public static class ExpressionExtensions
                {
                    public static Expression<Func<T, bool>> AndAlso<T>(
                        this Expression<Func<T, bool>> expr1,
                        Expression<Func<T, bool>> expr2)
                    {
                        var parameter = Expression.Parameter(typeof(T));

                        var combined = Expression.Lambda<Func<T, bool>>(
                            Expression.AndAlso(
                                Expression.Invoke(expr1, parameter),
                                Expression.Invoke(expr2, parameter)
                            ),
                            parameter
                        );

                        return combined;
                    }
    }



    public static class ReportQueries
    {
        public static IQueryable<GetAllDeviceReportsResponse> SelectReport(this IQueryable<Device> source)
        {
            Expression<Func<Device, GetAllDeviceReportsResponse>> expression = e => new GetAllDeviceReportsResponse
            {
                EnglishName = e.EnglishName,
                Name = e.Name,
                 DeviceStatus =e.DeviceStatus,
                 ByType =e.ByType,
                 ClinicNameAr =e.Clinic.Name,
                 ClinicNameEn = e.Clinic.EnglishName,
                 Code =e.Code,
                 HospitalNameAr =e.Hospital.Name,
                 HospitalNameEn =e.Hospital.EnglishName,
                 Model =e.Model,
                 ProjectTypeNameAr = e.ProjectType.Name,
                 ProjectTypeNameEn =e.ProjectType.EnglishName,
                 SerialNumber =e.SerialNumber,
                 StartRun =e.StartRun,
                 SubProjectTypeNameAr =e.SubProjectType.Name,
                 SubProjectTypeNameEn = e.SubProjectType.EnglishName,
                 SupplierNameAr =e.Supplier.Name,
                 SupplierNameEn =e.Supplier.EnglishName,
                 Year =e.Year,
                 

            };
            return source.Select(expression);
        }
    }
}
