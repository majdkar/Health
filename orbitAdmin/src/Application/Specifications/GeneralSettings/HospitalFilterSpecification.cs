using SchoolV01.Application.Features.Hospitals.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class HospitalFilterSpecification : HeroSpecification<Hospital>
    {
        public HospitalFilterSpecification(string searchString)
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
    public static class HospitalQueries
    {
        public static IQueryable<GetAllHospitalsResponse> SelectHospital(this IQueryable<Hospital> source)
        {
            Expression<Func<Hospital, GetAllHospitalsResponse>> expression = e => new GetAllHospitalsResponse
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
