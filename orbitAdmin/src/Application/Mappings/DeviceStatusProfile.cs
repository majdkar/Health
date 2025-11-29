using System;
using AutoMapper;
using SchoolV01.Application.Features.DeviceStatuss.Commands;
using SchoolV01.Application.Features.DeviceStatuss.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class DeviceStatusProfile : Profile
    {
        public DeviceStatusProfile()
        {
            CreateMap<AddEditDeviceStatusCommand, DeviceStatus>().ReverseMap();
            CreateMap<GetAllDeviceStatussResponse, DeviceStatus>().ReverseMap();
        }
    }
}