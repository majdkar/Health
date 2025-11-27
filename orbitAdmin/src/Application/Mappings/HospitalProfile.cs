using System;
using AutoMapper;
using SchoolV01.Application.Features.Hospitals.Commands;
using SchoolV01.Application.Features.Hospitals.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class HospitalProfile : Profile
    {
        public HospitalProfile()
        {
            CreateMap<AddEditHospitalCommand, Hospital>().ReverseMap();
            CreateMap<GetAllHospitalsResponse, Hospital>().ReverseMap();
        }
    }
}