using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class ClinicFilterSpecification : HeroSpecification<Clinic>
    {
        public ClinicFilterSpecification(string searchString)
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
    public static class ClinicQueries
    {
        public static IQueryable<GetAllClinicsResponse> SelectClinic(this IQueryable<Clinic> source)
        {
            Expression<Func<Clinic, GetAllClinicsResponse>> expression = e => new GetAllClinicsResponse
            {
                Id = e.Id,
                EnglishName =e.EnglishName,
                Name =e.Name,
                CityId =e.CityId,
                City =e.City,
                 DirectorateId = e.DirectorateId,
                 ByDirectorate =e.ByDirectorate,
                 Directorate =e.Directorate,
            };
            return source.Select(expression);
        }
    }
}
