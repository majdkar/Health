using System;
using AutoMapper;
using SchoolV01.Application.Features.Maintenances.Commands;
using SchoolV01.Application.Features.Maintenances.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class MaintenanceProfile : Profile
    {
        public MaintenanceProfile()
        {
            CreateMap<AddEditMaintenanceCommand, Maintenance>().ReverseMap();
            CreateMap<GetAllMaintenancesResponse, Maintenance>().ReverseMap();
        }
    }
}