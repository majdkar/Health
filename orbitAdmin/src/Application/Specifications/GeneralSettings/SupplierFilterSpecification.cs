using SchoolV01.Application.Features.Suppliers.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class SupplierFilterSpecification : HeroSpecification<Supplier>
    {
        public SupplierFilterSpecification(string searchString)
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
    public static class SupplierQueries
    {
        public static IQueryable<GetAllSuppliersResponse> SelectSupplier(this IQueryable<Supplier> source)
        {
            Expression<Func<Supplier, GetAllSuppliersResponse>> expression = e => new GetAllSuppliersResponse
            {
                Id = e.Id,
                EnglishName =e.EnglishName,
                Name =e.Name,
                CityId =e.CityId,
                City =e.City,
                  Address =e.Address,
                  PositionId =e.PositionId,
                  Country =e.Country,
                  Mobile =e.Mobile,
                  Email =e.Email,
                  CountryId =e.CountryId,
                  LicenseUrl =e.LicenseUrl,
                  Position =e.Position,
            };
            return source.Select(expression);
        }
    }
}
