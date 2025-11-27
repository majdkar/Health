using System;
using AutoMapper;
using SchoolV01.Application.Features.Directorates.Commands;
using SchoolV01.Application.Features.Directorates.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class DirectorateProfile : Profile
    {
        public DirectorateProfile()
        {
            CreateMap<AddEditDirectorateCommand, Directorate>().ReverseMap();
            CreateMap<GetAllDirectoratesResponse, Directorate>().ReverseMap();
        }
    }
}