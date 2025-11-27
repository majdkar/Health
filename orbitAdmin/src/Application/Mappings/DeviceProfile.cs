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
        }
    }
}