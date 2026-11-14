using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AltunPital.Services;

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _db;

    public ScheduleService(AppDbContext db)
    {
        _db = db;
    }

    public bool IsDoctorAvailable(int doctorId, DateTime startDateTime, int durationMinutes)
    {
        DateTime endDateTime = startDateTime.AddMinutes(durationMinutes);

        // O gün çalışma saati var mı
        var schedules = _db.DoctorSchedules
            .Where(s => s.DoctorId == doctorId && s.DayOfWeek == startDateTime.DayOfWeek)
            .ToList();

        if (schedules.Count == 0)
            return false;

        TimeOnly requestedStart = TimeOnly.FromDateTime(startDateTime);
        TimeOnly requestedEnd = TimeOnly.FromDateTime(endDateTime);

        bool insideWorkingHours = schedules.Any(s =>
            requestedStart >= s.StartTime &&
            requestedEnd <= s.EndTime);

        if (!insideWorkingHours)
            return false;

        // Aynı saatte başka randevu olmasın
        bool conflict = _db.Appointments.Any(a =>
            a.DoctorId == doctorId &&
            a.Status != AppointmentStatus.Cancelled &&
            a.Status != AppointmentStatus.Rejected &&
            startDateTime < a.EndDateTime &&
            endDateTime > a.StartDateTime);

        return !conflict;
    }

    public List<DateTime> GetAvailableSlots(int doctorId, int healthServiceId, DateOnly date)
    {
        var service = _db.HealthServices
            .FirstOrDefault(s => s.HealthServiceId == healthServiceId);

        if (service == null)
            throw new Exception("Hizmet bulunamadı.");

        var schedules = _db.DoctorSchedules
            .Where(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek)
            .ToList();

        var result = new List<DateTime>();

        foreach (var schedule in schedules)
        {
            DateTime current = date.ToDateTime(schedule.StartTime);
            DateTime scheduleEnd = date.ToDateTime(schedule.EndTime);

            while (current.AddMinutes(service.DurationMinutes) <= scheduleEnd)
            {
                if (IsDoctorAvailable(doctorId, current, service.DurationMinutes))
                    result.Add(current);

                current = current.AddMinutes(service.DurationMinutes);
            }
        }

        return result;
    }
}
