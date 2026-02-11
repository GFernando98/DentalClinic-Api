using DentalClinic.Domain.Entities;

namespace DentalClinic.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Patient> Patients { get; }
    IRepository<Doctor> Doctors { get; }
    IRepository<Odontogram> Odontograms { get; }
    IRepository<ToothRecord> ToothRecords { get; }
    IRepository<ToothSurfaceRecord> ToothSurfaceRecords { get; }
    IRepository<Treatment> Treatments { get; }
    IRepository<TreatmentRecord> TreatmentRecords { get; }
    IRepository<Appointment> Appointments { get; }
    IRepository<MedicalRecord> MedicalRecords { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<TreatmentCategory> TreatmentCategories { get; }
    IRepository<ClinicInformation> ClinicInformation { get; }
    IRepository<TaxInformation> TaxInformation { get; }
    IRepository<Invoice> Invoices { get; }
    IRepository<Payment> Payments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
