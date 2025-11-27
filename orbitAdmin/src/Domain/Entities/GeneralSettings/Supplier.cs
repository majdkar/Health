using System;
using SchoolV01.Domain.Contracts;

namespace SchoolV01.Domain.Entities.GeneralSettings
{
    public class Supplier : AuditableEntity<int>
    {
		public string Name { get; set; }

        public string EnglishName { get; set; } = "";
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string LicenseUrl { get; set; }

        public int? CountryId { get; set; }
        public Country Country { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public int? PositionId { get; set; }
        public Position Position { get; set; }


    }
}