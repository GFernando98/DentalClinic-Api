using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;

namespace DentalClinic.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
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

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Patient> Patients => _patients ??= new Repository<Patient>(_context);
    public IRepository<Doctor> Doctors => _doctors ??= new Repository<Doctor>(_context);
    public IRepository<Odontogram> Odontograms => _odontograms ??= new Repository<Odontogram>(_context);
    public IRepository<ToothRecord> ToothRecords => _toothRecords ??= new Repository<ToothRecord>(_context);
    public IRepository<ToothSurfaceRecord> ToothSurfaceRecords => _toothSurfaceRecords ??= new Repository<ToothSurfaceRecord>(_context);
    public IRepository<Treatment> Treatments => _treatments ??= new Repository<Treatment>(_context);
    public IRepository<TreatmentRecord> TreatmentRecords => _treatmentRecords ??= new Repository<TreatmentRecord>(_context);
    public IRepository<Appointment> Appointments => _appointments ??= new Repository<Appointment>(_context);
    public IRepository<MedicalRecord> MedicalRecords => _medicalRecords ??= new Repository<MedicalRecord>(_context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
