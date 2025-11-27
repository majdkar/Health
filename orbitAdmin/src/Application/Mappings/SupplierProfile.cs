using System;
using AutoMapper;
using SchoolV01.Application.Features.Suppliers.Commands;
using SchoolV01.Application.Features.Suppliers.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<AddEditSupplierCommand, Supplier>().ReverseMap();
            CreateMap<GetAllSuppliersResponse, Supplier>().ReverseMap();
        }
    }
}