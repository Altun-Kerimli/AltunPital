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

    private readonly IAppointmentService
        _appointmentService;

    private readonly IScheduleService
        _scheduleService;


    public PatientService(
        AppDbContext db,
        IAppointmentService appointmentService,
        IScheduleService scheduleService)
    {
        _db = db;

        _appointmentService =
            appointmentService;

        _scheduleService =
            scheduleService;
    }


    public List<Branch> GetBranches(string? preferredCity = null)
    {
        var query =
            _db.Branches
                .AsQueryable();

        if (string.IsNullOrWhiteSpace(
                preferredCity))
        {
            return query
                .OrderBy(b => b.City)
                .ThenBy(b => b.Name)
                .ToList();
        }

        string city =
            preferredCity.Trim();

        return query
            .OrderByDescending(b =>
                b.City.ToLower() ==
                city.ToLower())
            .ThenBy(b => b.City)
            .ThenBy(b => b.Name)
            .ToList();
    }


    public List<Department> GetDepartments(
        int branchId)
    {
        return _db.Departments
            .Where(d =>
                d.BranchId == branchId)

            .OrderBy(d => d.Name)

            .ToList();
    }


    public List<Doctor> GetDoctors(
        int branchId,
        int? departmentId = null)
    {
        var query =
            _db.Doctors
                .Include(d => d.Department)
                .Where(d =>
                    d.BranchId == branchId);

        if (departmentId.HasValue)
        {
            query = query.Where(d =>
                d.DepartmentId ==
                departmentId.Value);
        }

        return query
            .OrderBy(d => d.FirstName)
            .ThenBy(d => d.LastName)
            .ToList();
    }


    public List<HealthService> GetServices(
        int branchId,
        int? doctorId = null)
    {
        var query =
            _db.BranchServices
                .Include(bs =>
                    bs.HealthService)

                .Where(bs =>
                    bs.BranchId == branchId &&
                    bs.IsAvailable);


        if (doctorId.HasValue)
        {
            query = query.Where(bs =>
                _db.DoctorServices.Any(ds =>
                    ds.DoctorId ==
                        doctorId.Value &&

                    ds.HealthServiceId ==
                        bs.HealthServiceId));
        }


        return query
            .Select(bs =>
                bs.HealthService)

            .Distinct()

            .OrderBy(s => s.Name)

            .ToList();
    }


    public List<DateTime>
        GetAvailableSlots(
            int doctorId,
            int healthServiceId,
            DateOnly date)
    {
        return _scheduleService
            .GetAvailableSlots(
                doctorId,
                healthServiceId,
                date);
    }


    public Appointment CreateAppointment(
        int patientId,
        int doctorId,
        int branchId,
        int healthServiceId,
        DateTime startDateTime)
    {
        return _appointmentService
            .CreateAppointment(
                patientId,
                doctorId,
                branchId,
                healthServiceId,
                startDateTime);
    }


    public List<Appointment>
        GetAppointments(
            int patientId)
    {
        return _appointmentService
            .GetPatientAppointments(
                patientId);
    }


    public void CancelAppointment(
        int appointmentId)
    {
        _appointmentService
            .CancelAppointment(
                appointmentId);
    }
}