using HealthApp.Infrastructure.Data;
using HealthApp.Domain.Entities;
using HotChocolate.Data;
using HotChocolate;

namespace Doctorly.Api.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Patient> GetPatients(HealthAppDbContext context) =>
        context.Patients;

    [UseProjection]
    public Patient? GetPatient(HealthAppDbContext context, Guid id) =>
        context.Patients.FirstOrDefault(p => p.Id == id);

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Doctor> GetDoctors(HealthAppDbContext context) =>
        context.Doctors;

    [UseProjection]
    public Doctor? GetDoctor(HealthAppDbContext context, Guid id) =>
        context.Doctors.FirstOrDefault(d => d.Id == id);

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Appointment> GetAppointments(HealthAppDbContext context) =>
        context.Appointments;

    [UseProjection]
    public Appointment? GetAppointment(HealthAppDbContext context, Guid id) =>
        context.Appointments.FirstOrDefault(a => a.Id == id);

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<MedicalRecord> GetMedicalRecords(HealthAppDbContext context) =>
        context.MedicalRecords;

    [UseProjection]
    public MedicalRecord? GetMedicalRecord(HealthAppDbContext context, Guid id) =>
        context.MedicalRecords.FirstOrDefault(m => m.Id == id);
}