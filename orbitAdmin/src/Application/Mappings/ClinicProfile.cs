using System;
using AutoMapper;
using SchoolV01.Application.Features.Clinics.Commands;
using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class ClinicProfile : Profile
    {
        public ClinicProfile()
        {
            CreateMap<AddEditClinicCommand, Clinic>().ReverseMap();
            CreateMap<GetAllClinicsResponse, Clinic>().ReverseMap();
        }
    }
}