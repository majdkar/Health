using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Application.Responses.Identity;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class ClinicByDirectorateIdFilterSpecification : HeroSpecification<Clinic>
    {
        public ClinicByDirectorateIdFilterSpecification(string searchString,int DirectorateId)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.EnglishName.Contains(searchString) ||p.Name.Contains(searchString);
            }
            else
            {
                Criteria = p => true && p.DirectorateId == DirectorateId;
            }
        }

   
    }
}
