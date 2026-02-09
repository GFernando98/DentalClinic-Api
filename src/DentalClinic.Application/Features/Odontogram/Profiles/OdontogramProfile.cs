using AutoMapper;
using DentalClinic.Application.Features.Odontogram.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Odontogram.Profiles;

public class OdontogramProfile : Profile
{
    public OdontogramProfile()
    {
        CreateMap<Domain.Entities.Odontogram, OdontogramDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty));
        
        CreateMap<ToothRecord, ToothRecordDto>();
    }
}