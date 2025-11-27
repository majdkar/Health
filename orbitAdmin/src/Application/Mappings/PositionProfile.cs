using System;
using AutoMapper;
using SchoolV01.Application.Features.Positions.Commands;
using SchoolV01.Application.Features.Positions.Queries;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Mappings
{
    public class PositionProfile : Profile
    {
        public PositionProfile()
        {
            CreateMap<AddEditPositionCommand, Position>().ReverseMap();
            CreateMap<GetAllPositionsResponse, Position>().ReverseMap();
        }
    }
}