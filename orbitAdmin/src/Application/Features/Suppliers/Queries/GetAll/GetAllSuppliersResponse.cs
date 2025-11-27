using SchoolV01.Domain.Entities.GeneralSettings;
using System;
namespace SchoolV01.Application.Features.Suppliers.Queries
{
    public class GetAllSuppliersResponse
    {
        public int Id { get; set; }
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