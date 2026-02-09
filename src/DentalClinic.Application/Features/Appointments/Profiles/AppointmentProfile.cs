using AutoMapper;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Appointments.Profiles;

public class AppointmentProfile : Profile
{
    public AppointmentProfile()
    {
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty));
    }
}