using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AltunPital.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _db;
    private readonly IScheduleService _scheduleService;

    public AppointmentService(
        AppDbContext db,
        IScheduleService scheduleService)
    {
        _db = db;
        _scheduleService = scheduleService;
    }


    public Appointment CreateAppointment(
        int patientId,
        int doctorId,
        int branchId,
        int healthServiceId,
        DateTime startDateTime)
    {
        var patient = _db.Patients
            .FirstOrDefault(p =>
                p.UserId == patientId);

        if (patient == null)
            throw new Exception("Hasta bulunamadı.");


        var doctor = _db.Doctors
            .Include(d => d.DoctorServices)
            .FirstOrDefault(d =>
                d.UserId == doctorId);

        if (doctor == null)
            throw new Exception("Doktor bulunamadı.");


        if (doctor.BranchId != branchId)
            throw new Exception(
                "Doktor seçilen şubede çalışmıyor."
            );


        var service = _db.HealthServices
            .FirstOrDefault(s =>
                s.HealthServiceId == healthServiceId);

        if (service == null)
            throw new Exception("Hizmet bulunamadı.");


        bool doctorProvidesService =
            doctor.DoctorServices.Any(ds =>
                ds.HealthServiceId == healthServiceId);

        if (!doctorProvidesService)
            throw new Exception(
                "Doktor seçilen hizmeti vermiyor."
            );


        var branchService = _db.BranchServices
            .FirstOrDefault(bs =>
                bs.BranchId == branchId &&
                bs.HealthServiceId == healthServiceId);

        if (branchService == null ||
            !branchService.IsAvailable)
        {
            throw new Exception(
                "Bu hizmet seçilen şubede mevcut değil."
            );
        }


        bool available =
            _scheduleService.IsDoctorAvailable(
                doctorId,
                startDateTime,
                service.DurationMinutes);

        if (!available)
            throw new Exception(
                "Doktor seçilen tarih ve saatte uygun değil."
            );


        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = doctorId,
            BranchId = branchId,
            HealthServiceId = healthServiceId,

            StartDateTime = startDateTime,

            EndDateTime =
                startDateTime.AddMinutes(
                    service.DurationMinutes),

            // O anki fiyatı saklıyoruz.
            Price = branchService.Price,

            Status = AppointmentStatus.Pending,

            CreatedAt = DateTime.Now
        };


        _db.Appointments.Add(appointment);

        _db.SaveChanges();

        return appointment;
    }


    public void ApproveAppointment(int appointmentId)
    {
        var appointment =
            _db.Appointments.Find(appointmentId);

        if (appointment == null)
            throw new Exception(
                "Randevu bulunamadı."
            );

        if (appointment.Status !=
            AppointmentStatus.Pending)
        {
            throw new Exception(
                "Sadece bekleyen randevular onaylanabilir."
            );
        }

        appointment.Status =
            AppointmentStatus.Approved;

        _db.SaveChanges();
    }


    public void CancelAppointment(int appointmentId)
    {
        var appointment =
            _db.Appointments.Find(appointmentId);

        if (appointment == null)
            throw new Exception(
                "Randevu bulunamadı."
            );

        appointment.Status =
            AppointmentStatus.Cancelled;

        _db.SaveChanges();
    }


    public List<Appointment> GetPatientAppointments(
        int patientId)
    {
        return _db.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Branch)
            .Include(a => a.HealthService)

            .Where(a =>
                a.PatientId == patientId)

            .OrderBy(a =>
                a.StartDateTime)

            .ToList();
    }
}