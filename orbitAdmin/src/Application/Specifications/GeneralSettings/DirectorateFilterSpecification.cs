using SchoolV01.Application.Features.Directorates.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class DirectorateFilterSpecification : HeroSpecification<Directorate>
    {
        public DirectorateFilterSpecification(string searchString)
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
    public static class DirectorateQueries
    {
        public static IQueryable<GetAllDirectoratesResponse> SelectDirectorate(this IQueryable<Directorate> source)
        {
            Expression<Func<Directorate, GetAllDirectoratesResponse>> expression = e => new GetAllDirectoratesResponse
            {
                Id = e.Id,
                EnglishName =e.EnglishName,
                Name =e.Name,
                CityId =e.CityId,
                City =e.City,
            };
            return source.Select(expression);
        }
    }
}
