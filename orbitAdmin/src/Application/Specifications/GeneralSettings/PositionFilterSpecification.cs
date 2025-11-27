using System;
using SchoolV01.Application.Specifications.Base;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Specifications.GeneralSettings
{
    public class PositionFilterSpecification : HeroSpecification<Position>
    {
        public PositionFilterSpecification(string searchString)
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
}
