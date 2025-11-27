using SchoolV01.Domain.Entities.GeneralSettings;
using System;
namespace SchoolV01.Application.Features.Directorates.Queries
{
    public class GetAllDirectoratesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
		
		public string EnglishName { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
    }
}