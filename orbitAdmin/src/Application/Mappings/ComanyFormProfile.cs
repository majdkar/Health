using System;
using AutoMapper;
using SchoolV01.Application.Features.FormCompanys.Commands;
using SchoolV01.Application.Features.FormCompanies.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class FormCompanyProfile : Profile
    {
        public FormCompanyProfile()
        {
            CreateMap<AddEditFormCompanyCommand, FormCompany>()
                .ForMember(dest => dest.Attachments, opt => opt.Ignore());
            
            CreateMap<GetAllFormCompaniesResponse, FormCompany>().ReverseMap();


            CreateMap<FormCompany, GetByIdFormCompaniesResponse>();


         
        }
    }
}