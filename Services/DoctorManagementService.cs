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

    public DoctorManagementService(
        AppDbContext db)
    {
        _db = db;
    }


    public List<Appointment>
        GetAppointments(int doctorId)
    {
        return _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Branch)
            .Include(a => a.HealthService)

            .Where(a =>
                a.DoctorId == doctorId)

            .OrderBy(a =>
                a.StartDateTime)

            .ToList();
    }


    public List<DoctorSchedule>
        GetSchedules(int doctorId)
    {
        return _db.DoctorSchedules

            .Where(s =>
                s.DoctorId == doctorId)

            .OrderBy(s =>
                s.DayOfWeek)

            .ThenBy(s =>
                s.StartTime)

            .ToList();
    }


    public void ApproveAppointment(
        int doctorId,
        int appointmentId)
    {
        var appointment =
            FindDoctorAppointment(
                doctorId,
                appointmentId);

        if (appointment.Status !=
            AppointmentStatus.Pending)
        {
            throw new Exception(
                "Sadece bekleyen randevu onaylanabilir.");
        }

        appointment.Status =
            AppointmentStatus.Approved;

        _db.SaveChanges();
    }


    public void RejectAppointment(
        int doctorId,
        int appointmentId)
    {
        var appointment =
            FindDoctorAppointment(
                doctorId,
                appointmentId);

        if (appointment.Status !=
            AppointmentStatus.Pending)
        {
            throw new Exception(
                "Sadece bekleyen randevu reddedilebilir.");
        }

        appointment.Status =
            AppointmentStatus.Rejected;

        _db.SaveChanges();
    }


    public void CompleteAppointment(
        int doctorId,
        int appointmentId)
    {
        var appointment =
            FindDoctorAppointment(
                doctorId,
                appointmentId);

        if (appointment.Status !=
            AppointmentStatus.Approved)
        {
            throw new Exception(
                "Sadece onaylanmış randevu tamamlanabilir.");
        }

        appointment.Status =
            AppointmentStatus.Completed;

        _db.SaveChanges();
    }


    private Appointment
        FindDoctorAppointment(
            int doctorId,
            int appointmentId)
    {
        var appointment =
            _db.Appointments
                .FirstOrDefault(a =>
                    a.AppointmentId ==
                        appointmentId &&
                    a.DoctorId ==
                        doctorId);

        if (appointment == null)
        {
            throw new Exception(
                "Randevu bulunamadı veya bu doktora ait değil.");
        }

        return appointment;
    }
}