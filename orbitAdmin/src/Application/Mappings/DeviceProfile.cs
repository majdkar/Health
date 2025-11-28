using System;
using AutoMapper;
using SchoolV01.Application.Features.Devices.Commands;
using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<AddEditDeviceCommand, Device>().ReverseMap();
            CreateMap<GetAllDevicesResponse, Device>().ReverseMap();


            CreateMap<Device, GetByIdDevicesResponse>()
           .ForMember(dest => dest.ProjectType,
               opt => opt.MapFrom(src =>
                   src.ProjectType == null ? null :
                   new ProjectTypeLiteDto
                   {
                       Id = src.ProjectType.Id,
                       Name = src.ProjectType.Name,
                       EnglishName = src.ProjectType.EnglishName
                   }))

           .ForMember(dest => dest.SubProjectType,
               opt => opt.MapFrom(src =>
                   src.SubProjectType == null ? null :
                   new ProjectTypeLiteDto
                   {
                       Id = src.SubProjectType.Id,
                       Name = src.SubProjectType.Name,
                       EnglishName = src.SubProjectType.EnglishName
                   }))

           .ForMember(dest => dest.SubSubProjectType,
               opt => opt.MapFrom(src =>
                   src.SubSubProjectType == null ? null :
                   new ProjectTypeLiteDto
                   {
                       Id = src.SubSubProjectType.Id,
                       Name = src.SubSubProjectType.Name,
                       EnglishName = src.SubSubProjectType.EnglishName
                   }))

           .ForMember(dest => dest.Supplier,
               opt => opt.MapFrom(src =>
                   src.Supplier == null ? null :
                   new SupplierLiteDto
                   {
                       Id = src.Supplier.Id,
                       Name = src.Supplier.Name
                   }))

           .ForMember(dest => dest.Clinic,
               opt => opt.MapFrom(src =>
                   src.Clinic == null ? null :
                   new ClinicLiteDto
                   {
                       Id = src.Clinic.Id,
                       Name = src.Clinic.Name
                   }))

           .ForMember(dest => dest.Hospital,
               opt => opt.MapFrom(src =>
                   src.Hospital == null ? null :
                   new HospitalLiteDto
                   {
                       Id = src.Hospital.Id,
                       Name = src.Hospital.Name
                   }));
        }
    }
}