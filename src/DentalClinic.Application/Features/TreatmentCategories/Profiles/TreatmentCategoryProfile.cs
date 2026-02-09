using AutoMapper;
using DentalClinic.Application.Features.TreatmentCategories.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.TreatmentCategories.Profiles;

public class TreatmentCategoryProfile : Profile
{
    public TreatmentCategoryProfile()
    {
        CreateMap<TreatmentCategory, TreatmentCategoryDto>();
    }
}