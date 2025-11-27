using System;
using SchoolV01.Domain.Contracts;

namespace SchoolV01.Domain.Entities.GeneralSettings
{
    public class Directorate : AuditableEntity<int>
    {
		public string Name { get; set; }

        public string EnglishName { get; set; } = "";

        public int CityId { get; set; }
        public City City { get; set; }

    }
}