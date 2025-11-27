using System;
using System.Collections.Generic;
using SchoolV01.Domain.Contracts;

namespace SchoolV01.Domain.Entities.GeneralSettings
{
    public class ProjectType : AuditableEntity<int>
    {
		public string Name { get; set; }

        public string EnglishName { get; set; } = "";

        public int? ParentId { get; set; }
        public ProjectType Parent { get; set; }

        public ICollection<ProjectType> SubProjectTypes { get; set; } = new List<ProjectType>();

    }
}