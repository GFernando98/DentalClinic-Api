using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;

namespace DentalClinic.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IRepository<Patient>? _patients;
    private IRepository<Doctor>? _doctors;
    private IRepository<Odontogram>? _odontograms;
    private IRepository<ToothRecord>? _toothRecords;
    private IRepository<ToothSurfaceRecord>? _toothSurfaceRecords;
    private IRepository<Treatment>? _treatments;
    private IRepository<TreatmentRecord>? _treatmentRecords;
    private IRepository<Appointment>? _appointments;
    private IRepository<MedicalRecord>? _medicalRecords;
    private IRepository<Notification>? _notifications;
    private IRepository<TreatmentCategory>? _treatmentCategories;
    private IRepository<ClinicInformation>? _clinicInformation;
    private IRepository<TaxInformation>? _taxInformation;
    private IRepository<Invoice>? _invoices;
    private IRepository<Payment>? _payments;

    public IRepository<Patient> Patients => _patients ??= new Repository<Patient>(context);
    public IRepository<Doctor> Doctors => _doctors ??= new Repository<Doctor>(context);
    public IRepository<Odontogram> Odontograms => _odontograms ??= new Repository<Odontogram>(context);
    public IRepository<ToothRecord> ToothRecords => _toothRecords ??= new Repository<ToothRecord>(context);
    public IRepository<ToothSurfaceRecord> ToothSurfaceRecords => _toothSurfaceRecords ??= new Repository<ToothSurfaceRecord>(context);
    public IRepository<Treatment> Treatments => _treatments ??= new Repository<Treatment>(context);
    public IRepository<TreatmentRecord> TreatmentRecords => _treatmentRecords ??= new Repository<TreatmentRecord>(context);
    public IRepository<Appointment> Appointments => _appointments ??= new Repository<Appointment>(context);
    public IRepository<MedicalRecord> MedicalRecords => _medicalRecords ??= new Repository<MedicalRecord>(context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(context);
    public IRepository<TreatmentCategory> TreatmentCategories => _treatmentCategories ??= new Repository<TreatmentCategory>(context);
    public IRepository<ClinicInformation> ClinicInformation => _clinicInformation ??= new Repository<ClinicInformation>(context);
    public IRepository<TaxInformation> TaxInformation => _taxInformation ??= new Repository<TaxInformation>(context);
    public IRepository<Invoice> Invoices => _invoices ??= new Repository<Invoice>(context);
    public IRepository<Payment> Payments => _payments ??= new Repository<Payment>(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    public void Dispose() => context.Dispose();
}
