using AutoMapper;
using DentalClinic.Application.Features.TreatmentCategories.DTOs;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Treatments.Profiles;

public class TreatmentProfile : Profile
{
    public TreatmentProfile()
    {
        CreateMap<Treatment, TreatmentDto>();
        CreateMap<TreatmentCategory, TreatmentCategoryDto>();
        CreateMap<CreateTreatmentDto, Treatment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
    }
}