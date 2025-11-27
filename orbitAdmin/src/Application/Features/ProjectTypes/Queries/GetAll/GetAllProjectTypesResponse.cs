using System;
using System.Collections.Generic;
namespace SchoolV01.Application.Features.ProjectTypes.Queries
{
    public class GetAllProjectTypesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
		
		public string EnglishName { get; set; }

        public int? ParentId { get; set; }

        public List<GetAllProjectTypesResponse> SubProjectTypes { get; set; }
            = new List<GetAllProjectTypesResponse>();

    }
}