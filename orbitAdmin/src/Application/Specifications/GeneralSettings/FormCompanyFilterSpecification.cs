using SchoolV01.Application.Features.FormCompanies.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class FormCompanyFilterSpecification : HeroSpecification<FormCompany>
    {
        public FormCompanyFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Model.Contains(searchString) ||p.DeviceType.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }

   
    }
    public static class FormCompanyQueries
    {
        public static IQueryable<GetAllFormCompaniesResponse> SelectFormCompany(this IQueryable<FormCompany> source)
        {
            Expression<Func<FormCompany, GetAllFormCompaniesResponse>> expression = e => new GetAllFormCompaniesResponse
            {
                Id = e.Id,
                  AgentName =e.AgentName,
                  DeviceType = e.DeviceType,
                  Model =e.Model,
                  Attachments = e.Attachments,
                  CompanyName =e.CompanyName,
                   FormNumber = e.FormNumber,
                  DeviceBrand = e.DeviceBrand,
            };
            return source.Select(expression);
        }
    }
}
