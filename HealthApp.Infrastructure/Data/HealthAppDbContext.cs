using Microsoft.EntityFrameworkCore;
using HealthApp.Domain.Entities;

namespace HealthApp.Infrastructure.Data;

public class HealthAppDbContext : DbContext
{
    public HealthAppDbContext(DbContextOptions<HealthAppDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Attendee> Attendees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.EmergencyContact).HasMaxLength(200);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
        });

        // Configure Doctor
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Specialization).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
            entity.Property(e => e.Department).HasMaxLength(200);
        });

        // Configure Appointment
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Appointments)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Doctor)
                  .WithMany(d => d.Appointments)
                  .HasForeignKey(e => e.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure MedicalRecord
        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Diagnosis).HasMaxLength(1000);
            entity.Property(e => e.Treatment).HasMaxLength(2000);
            entity.Property(e => e.Medications).HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.MedicalRecords)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Doctor)
                  .WithMany(d => d.MedicalRecords)
                  .HasForeignKey(e => e.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Event
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
        });

        // Configure Attendee
        modelBuilder.Entity<Attendee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => new { e.EventId, e.EmailAddress }).IsUnique();

            entity.HasOne(e => e.Event)
                  .WithMany(ev => ev.Attendees)
                  .HasForeignKey(e => e.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.GetType().GetProperty("CreatedAt") != null)
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            }

            if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }
    }
}