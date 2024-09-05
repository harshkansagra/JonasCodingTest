using AutoMapper;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Models;

namespace WebApi
{
    public class AppServicesProfile : Profile
    {
        public AppServicesProfile()
        {
            CreateMapper();
        }

        private void CreateMapper()
        {
            CreateMap<BaseInfo, BaseDto>();
            CreateMap<CompanyInfo, CompanyDto>();
            CreateMap<ArSubledgerInfo, ArSubledgerDto>();

            CreateMap<CompanyDto, CompanyInfo>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && (!(srcMember is DateTime) || (DateTime)srcMember != DateTime.MinValue)));
            CreateMap<ArSubledger, ArSubledger>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<EmployeeInfo, EmployeeDto>();

            CreateMap<EmployeeInfo, EmployeeDetailDto>()
            .ForMember(dest => dest.OccupationName, opt => opt.MapFrom(src => src.Occupation))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.LastModifiedDateTime, opt => opt.MapFrom(src => src.LastModified.ToString("yyyy-MM-dd HH:mm:ss")));
            CreateMap<CompanyInfo, EmployeeDetailDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName));
        }
    }
}