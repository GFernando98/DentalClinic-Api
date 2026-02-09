using DentalClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.IdentityNumber).HasMaxLength(20);
        builder.Property(p => p.Phone).HasMaxLength(20);
        builder.Property(p => p.WhatsAppNumber).HasMaxLength(20);
        builder.Property(p => p.Email).HasMaxLength(150);
        builder.Property(p => p.Address).HasMaxLength(300);
        builder.Property(p => p.City).HasMaxLength(100);
        builder.Property(p => p.Occupation).HasMaxLength(100);
        builder.Property(p => p.EmergencyContactName).HasMaxLength(150);
        builder.Property(p => p.EmergencyContactPhone).HasMaxLength(20);
        builder.Property(p => p.Allergies).HasMaxLength(500);
        builder.Property(p => p.MedicalConditions).HasMaxLength(1000);
        builder.Property(p => p.CurrentMedications).HasMaxLength(500);
        builder.Property(p => p.Notes).HasMaxLength(2000);

        builder.HasIndex(p => p.IdentityNumber).IsUnique().HasFilter("[IdentityNumber] IS NOT NULL AND [IsDeleted] = 0");
        builder.HasIndex(p => p.Email).HasFilter("[Email] IS NOT NULL AND [IsDeleted] = 0");
        builder.HasIndex(p => new { p.FirstName, p.LastName });

        builder.Ignore(p => p.FullName);
        builder.Ignore(p => p.Age);
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.LastName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.LicenseNumber).HasMaxLength(50);
        builder.Property(d => d.Specialty).HasMaxLength(100);
        builder.Property(d => d.Phone).HasMaxLength(20);
        builder.Property(d => d.Email).HasMaxLength(150);

        builder.HasOne(d => d.User)
            .WithOne(u => u.Doctor)
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(d => d.LicenseNumber).IsUnique().HasFilter("[LicenseNumber] IS NOT NULL AND [IsDeleted] = 0");

        builder.Ignore(d => d.FullName);
    }
}

public class OdontogramConfiguration : IEntityTypeConfiguration<Odontogram>
{
    public void Configure(EntityTypeBuilder<Odontogram> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasOne(o => o.Patient)
            .WithMany(p => p.Odontograms)
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Doctor)
            .WithMany()
            .HasForeignKey(o => o.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(o => o.Notes).HasMaxLength(2000);
        builder.HasIndex(o => o.PatientId);
        builder.HasIndex(o => o.ExaminationDate);
    }
}

public class ToothRecordConfiguration : IEntityTypeConfiguration<ToothRecord>
{
    public void Configure(EntityTypeBuilder<ToothRecord> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Odontogram)
            .WithMany(o => o.TeethRecords)
            .HasForeignKey(t => t.OdontogramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.Notes).HasMaxLength(1000);
        builder.HasIndex(t => new { t.OdontogramId, t.ToothNumber }).IsUnique();
    }
}

public class ToothSurfaceRecordConfiguration : IEntityTypeConfiguration<ToothSurfaceRecord>
{
    public void Configure(EntityTypeBuilder<ToothSurfaceRecord> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.ToothRecord)
            .WithMany(t => t.SurfaceRecords)
            .HasForeignKey(s => s.ToothRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.Notes).HasMaxLength(500);
        builder.HasIndex(s => new { s.ToothRecordId, s.Surface }).IsUnique();
    }
}

public class TreatmentConfiguration : IEntityTypeConfiguration<Treatment>
{
    public void Configure(EntityTypeBuilder<Treatment> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Code).HasMaxLength(20).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.DefaultPrice).HasColumnType("decimal(18,2)");
        builder.HasOne(t => t.Category)
            .WithMany(c => c.Treatments)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.Code).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}

public class TreatmentRecordConfiguration : IEntityTypeConfiguration<TreatmentRecord>
{
    public void Configure(EntityTypeBuilder<TreatmentRecord> builder)
    {
        builder.HasKey(tr => tr.Id);

        builder.HasOne(tr => tr.ToothRecord)
            .WithMany(t => t.Treatments)
            .HasForeignKey(tr => tr.ToothRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tr => tr.Treatment)
            .WithMany(t => t.TreatmentRecords)
            .HasForeignKey(tr => tr.TreatmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tr => tr.Doctor)
            .WithMany(d => d.TreatmentRecords)
            .HasForeignKey(tr => tr.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tr => tr.Appointment)
            .WithMany(a => a.TreatmentRecords)
            .HasForeignKey(tr => tr.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(tr => tr.Price).HasColumnType("decimal(18,2)");
        builder.Property(tr => tr.Notes).HasMaxLength(1000);
        builder.Property(tr => tr.SurfacesAffected).HasMaxLength(50);

        builder.HasIndex(tr => tr.PerformedDate);
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Reason).HasMaxLength(500);
        builder.Property(a => a.Notes).HasMaxLength(2000);
        builder.Property(a => a.CancellationReason).HasMaxLength(500);

        builder.HasIndex(a => a.ScheduledDate);
        builder.HasIndex(a => new { a.DoctorId, a.ScheduledDate });
        builder.HasIndex(a => new { a.PatientId, a.ScheduledDate });
    }
}

public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.HasKey(mr => mr.Id);

        builder.HasOne(mr => mr.Patient)
            .WithMany(p => p.MedicalRecords)
            .HasForeignKey(mr => mr.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mr => mr.Doctor)
            .WithMany()
            .HasForeignKey(mr => mr.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(mr => mr.Title).HasMaxLength(200).IsRequired();
        builder.Property(mr => mr.Description).HasMaxLength(5000);
        builder.Property(mr => mr.Diagnosis).HasMaxLength(1000);
        builder.Property(mr => mr.AttachmentUrl).HasMaxLength(500);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.HasOne(n => n.Patient)
            .WithMany(p => p.Notifications)
            .HasForeignKey(n => n.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.Appointment)
            .WithMany()
            .HasForeignKey(n => n.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(n => n.Subject).HasMaxLength(200);
        builder.Property(n => n.Message).HasMaxLength(2000);
        builder.Property(n => n.ErrorMessage).HasMaxLength(500);

        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.ScheduledAt);
    }
}

public class TreatmentCategoryConfiguration : IEntityTypeConfiguration<TreatmentCategory>
{
    public void Configure(EntityTypeBuilder<TreatmentCategory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Color).HasMaxLength(10);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
