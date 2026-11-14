using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AltunPital.Services;

public class DoctorManagementService : IDoctorService
{
    private readonly AppDbContext _db;

    public DoctorManagementService(AppDbContext db, params object[] unusedDependencies)
    {
        _db = db;
    }

    public Doctor GetDoctor(int doctorId)
    {
        return _db.Doctors
            .Include(d => d.Branch)
            .Include(d => d.Department)
            .Include(d => d.DoctorServices)
                .ThenInclude(ds => ds.HealthService)
            .FirstOrDefault(d => d.UserId == doctorId)
            ?? throw new Exception("Doktor bulunamadı.");
    }

    public List<HealthService> GetServices(int doctorId)
    {
        return _db.DoctorServices
            .Where(ds => ds.DoctorId == doctorId)
            .Include(ds => ds.HealthService)
            .Select(ds => ds.HealthService)
            .OrderBy(s => s.HealthServiceId)
            .ToList();
    }

    public List<Appointment> GetAppointments(int doctorId)
    {
        return _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Branch)
            .Include(a => a.HealthService)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.StartDateTime)
            .ToList();
    }

    public List<DoctorSchedule> GetSchedules(int doctorId)
    {
        return _db.DoctorSchedules
            .Where(s => s.DoctorId == doctorId)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToList();
    }

    public void ApproveAppointment(int doctorId, int appointmentId)
    {
        Appointment appointment = GetDoctorAppointment(doctorId, appointmentId);

        if (appointment.Status != AppointmentStatus.Pending)
            throw new Exception("Yalnız bekleyen randevular onaylanabilir.");

        appointment.Status = AppointmentStatus.Approved;
        _db.SaveChanges();
    }

    public void RejectAppointment(int doctorId, int appointmentId)
    {
        Appointment appointment = GetDoctorAppointment(doctorId, appointmentId);

        if (appointment.Status != AppointmentStatus.Pending)
            throw new Exception("Yalnız bekleyen randevular reddedilebilir.");

        appointment.Status = AppointmentStatus.Rejected;
        _db.SaveChanges();
    }

    public void CompleteAppointment(int doctorId, int appointmentId)
    {
        Appointment appointment = GetDoctorAppointment(doctorId, appointmentId);

        if (appointment.Status != AppointmentStatus.Approved)
            throw new Exception("Yalnız onaylanmış randevular tamamlanabilir.");

        appointment.Status = AppointmentStatus.Completed;
        _db.SaveChanges();
    }

    private Appointment GetDoctorAppointment(int doctorId, int appointmentId)
    {
        return _db.Appointments
            .FirstOrDefault(a => a.AppointmentId == appointmentId && a.DoctorId == doctorId)
            ?? throw new Exception("Randevu bulunamadı veya bu doktora ait değil.");
    }
}
