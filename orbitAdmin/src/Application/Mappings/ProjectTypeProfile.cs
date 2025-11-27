using System;
using AutoMapper;
using SchoolV01.Application.Features.ProjectTypes.Commands;
using SchoolV01.Application.Features.ProjectTypes.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class ProjectTypeProfile : Profile
    {
        public ProjectTypeProfile()
        {
            CreateMap<AddEditProjectTypeCommand, ProjectType>().ReverseMap();
            CreateMap<GetAllProjectTypesResponse, ProjectType>().ReverseMap();
        }
    }
}