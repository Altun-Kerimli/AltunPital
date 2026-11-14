using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AltunPital.Services;

public class PatientService : IPatientService
{
    private readonly AppDbContext _db;

    public PatientService(AppDbContext db, params object[] unusedDependencies)
    {
        _db = db;
    }

    public List<Branch> GetBranches(string? preferredCity = null)
    {
        var branches = _db.Branches
            .OrderBy(b => b.BranchId)
            .ToList();

        if (string.IsNullOrWhiteSpace(preferredCity))
        {
            return branches;
        }

        string city = preferredCity.Trim();

        // Tercih edilen şehri üste al
        return branches
            .OrderByDescending(b =>
                string.Equals(b.City, city, StringComparison.OrdinalIgnoreCase))
            .ThenBy(b => b.City)
            .ThenBy(b => b.BranchId)
            .ToList();
    }

    public List<Department> GetDepartments(int branchId)
    {
        return _db.Departments
            .Where(d => d.BranchId == branchId)
            .OrderBy(d => d.DepartmentId)
            .ToList();
    }

    public List<Doctor> GetDoctors(int branchId, int departmentId)
    {
        // Şubede açık hizmeti olan doktorlar
        HashSet<int> activeServiceIds = _db.BranchServices
            .Where(bs => bs.BranchId == branchId && bs.IsAvailable)
            .Select(bs => bs.HealthServiceId)
            .ToHashSet();

        return _db.Doctors
            .Include(d => d.Branch)
            .Include(d => d.Department)
            .Include(d => d.DoctorServices)
            .Where(d => d.BranchId == branchId && d.DepartmentId == departmentId)
            .AsEnumerable()
            .Where(d => d.DoctorServices.Any(ds => activeServiceIds.Contains(ds.HealthServiceId)))
            .OrderBy(d => d.UserId)
            .ToList();
    }

    public List<HealthService> GetServices(int branchId, int doctorId)
    {
        HashSet<int> doctorServiceIds = _db.DoctorServices
            .Where(ds => ds.DoctorId == doctorId)
            .Select(ds => ds.HealthServiceId)
            .ToHashSet();

        List<int> activeServiceIds = _db.BranchServices
            .Where(bs =>
                bs.BranchId == branchId &&
                bs.IsAvailable &&
                doctorServiceIds.Contains(bs.HealthServiceId))
            .Select(bs => bs.HealthServiceId)
            .ToList();

        return _db.HealthServices
            .Where(s => activeServiceIds.Contains(s.HealthServiceId))
            .OrderBy(s => s.HealthServiceId)
            .ToList();
    }

    public List<DateTime> GetAvailableSlots(int doctorId, int healthServiceId, DateOnly date)
    {
        Doctor doctor = _db.Doctors.FirstOrDefault(d => d.UserId == doctorId)
            ?? throw new Exception("Doktor bulunamadı.");

        HealthService service = _db.HealthServices.FirstOrDefault(s => s.HealthServiceId == healthServiceId)
            ?? throw new Exception("Hizmet bulunamadı.");

        bool doctorOffersService = _db.DoctorServices.Any(ds =>
            ds.DoctorId == doctorId &&
            ds.HealthServiceId == healthServiceId);

        if (!doctorOffersService)
        {
            throw new Exception("Seçilen doktor bu hizmeti sunmuyor.");
        }

        bool branchOffersService = _db.BranchServices.Any(bs =>
            bs.BranchId == doctor.BranchId &&
            bs.HealthServiceId == healthServiceId &&
            bs.IsAvailable);

        if (!branchOffersService)
        {
            throw new Exception("Bu hizmet seçilen şubede aktif değil.");
        }

        DayOfWeek day = date.DayOfWeek;

        List<DoctorSchedule> schedules = _db.DoctorSchedules
            .Where(s => s.DoctorId == doctorId && s.DayOfWeek == day)
            .OrderBy(s => s.StartTime)
            .ToList();

        if (schedules.Count == 0)
        {
            return new List<DateTime>();
        }

        DateTime dayStart = date.ToDateTime(TimeOnly.MinValue);
        DateTime dayEnd = dayStart.AddDays(1);

        List<Appointment> appointments = _db.Appointments
            .Where(a =>
                a.DoctorId == doctorId &&
                a.StartDateTime >= dayStart &&
                a.StartDateTime < dayEnd &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Rejected)
            .ToList();

        var result = new List<DateTime>();

        // Mesai içinde, çakışmasız ve gelecek slotlar
        foreach (DoctorSchedule schedule in schedules)
        {
            DateTime current = date.ToDateTime(schedule.StartTime);
            DateTime scheduleEnd = date.ToDateTime(schedule.EndTime);

            while (current.AddMinutes(service.DurationMinutes) <= scheduleEnd)
            {
                DateTime slotEnd = current.AddMinutes(service.DurationMinutes);

                bool conflict = appointments.Any(a =>
                    current < a.EndDateTime &&
                    slotEnd > a.StartDateTime);

                bool isFuture = current > DateTime.Now;

                if (!conflict && isFuture)
                {
                    result.Add(current);
                }

                current = current.AddMinutes(service.DurationMinutes);
            }
        }

        return result
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public Appointment CreateAppointment(
        int patientId,
        int doctorId,
        int branchId,
        int healthServiceId,
        DateTime startDateTime)
    {
        Patient patient = _db.Patients.FirstOrDefault(p => p.UserId == patientId)
            ?? throw new Exception("Hasta bulunamadı.");

        Doctor doctor = _db.Doctors.FirstOrDefault(d => d.UserId == doctorId)
            ?? throw new Exception("Doktor bulunamadı.");

        if (doctor.BranchId != branchId)
        {
            throw new Exception("Seçilen doktor bu şubede çalışmıyor.");
        }

        HealthService service = _db.HealthServices.FirstOrDefault(s => s.HealthServiceId == healthServiceId)
            ?? throw new Exception("Hizmet bulunamadı.");

        BranchService branchService = _db.BranchServices.FirstOrDefault(bs =>
            bs.BranchId == branchId &&
            bs.HealthServiceId == healthServiceId &&
            bs.IsAvailable)
            ?? throw new Exception("Hizmet bu şubede aktif değil.");

        bool doctorOffersService = _db.DoctorServices.Any(ds =>
            ds.DoctorId == doctorId &&
            ds.HealthServiceId == healthServiceId);

        if (!doctorOffersService)
        {
            throw new Exception("Doktor seçilen hizmeti sunmuyor.");
        }

        DateOnly date = DateOnly.FromDateTime(startDateTime);
        List<DateTime> availableSlots = GetAvailableSlots(doctorId, healthServiceId, date);

        if (!availableSlots.Contains(startDateTime))
        {
            throw new Exception("Seçilen saat artık uygun değil.");
        }

        DateTime endDateTime = startDateTime.AddMinutes(service.DurationMinutes);

        // Hastanın aynı saatte başka randevusu olmasın
        bool patientConflict = _db.Appointments.Any(a =>
            a.PatientId == patientId &&
            a.Status != AppointmentStatus.Cancelled &&
            a.Status != AppointmentStatus.Rejected &&
            startDateTime < a.EndDateTime &&
            endDateTime > a.StartDateTime);

        if (patientConflict)
        {
            throw new Exception("Bu saat aralığında başka bir randevunuz bulunuyor.");
        }

        var appointment = new Appointment
        {
            PatientId = patient.UserId,
            DoctorId = doctor.UserId,
            BranchId = branchId,
            HealthServiceId = service.HealthServiceId,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            Price = branchService.Price,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.Now
        };

        _db.Appointments.Add(appointment);
        _db.SaveChanges();

        return appointment;
    }

    public List<Appointment> GetAppointments(int patientId)
    {
        return _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
            .Include(a => a.Branch)
            .Include(a => a.HealthService)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.StartDateTime)
            .ToList();
    }

    public void CancelAppointment(int appointmentId)
    {
        Appointment appointment = _db.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId)
            ?? throw new Exception("Randevu bulunamadı.");

        if (appointment.Status == AppointmentStatus.Cancelled ||
            appointment.Status == AppointmentStatus.Rejected ||
            appointment.Status == AppointmentStatus.Completed)
        {
            throw new Exception("Bu randevu iptal edilemez.");
        }

        appointment.Status = AppointmentStatus.Cancelled;

        _db.SaveChanges();
    }
}
