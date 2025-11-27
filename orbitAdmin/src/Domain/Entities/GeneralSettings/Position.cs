using System;
using SchoolV01.Domain.Contracts;

namespace SchoolV01.Domain.Entities.GeneralSettings
{
    public class Position : AuditableEntity<int>
    {
		public string Name { get; set; }

        public string EnglishName { get; set; } = "";

    }
}