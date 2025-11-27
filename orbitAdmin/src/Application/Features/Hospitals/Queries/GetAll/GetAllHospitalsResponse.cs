using SchoolV01.Domain.Entities.GeneralSettings;
using System;
namespace SchoolV01.Application.Features.Hospitals.Queries
{
    public class GetAllHospitalsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
		
		public string EnglishName { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }

        public bool ByDirectorate { get; set; }

        public int? DirectorateId { get; set; }
        public Directorate Directorate { get; set; }
    }
}