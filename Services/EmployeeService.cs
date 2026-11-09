using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;
using Microsoft.EntityFrameworkCore;

using AltunPital.Security;

namespace AltunPital.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;

    public EmployeeService(AppDbContext db)
    {
        _db = db;
    }

    public Doctor AddDoctor(
        string firstName,
        string lastName,
        string email,
        string phone,
        string password,
        string employeeNumber,
        DateTime hireDate,
        int branchId,
        int departmentId,
        string specialization)
    {
        ValidateBranchAndDepartment(
            branchId,
            departmentId);

        CheckEmailAndEmployeeNumber(
            email,
            employeeNumber);

        if (string.IsNullOrWhiteSpace(password))
            throw new Exception(
                "Şifre boş bırakılamaz.");

        var doctor = new Doctor
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email.Trim().ToLowerInvariant(),
            Phone = phone,

            PasswordHash =
                PasswordHasher.HashPassword(password),

            EmployeeNumber = employeeNumber,
            HireDate = hireDate,

            BranchId = branchId,
            DepartmentId = departmentId,

            Specialization = specialization
        };

        _db.Doctors.Add(doctor);
        _db.SaveChanges();

        return doctor;
    }


    public HealthStaff AddHealthStaff(
        string firstName,
        string lastName,
        string email,
        string phone,
        string password,
        string employeeNumber,
        DateTime hireDate,
        int branchId,
        int? departmentId,
        string position)
    {
        if (!_db.Branches.Any(
            b => b.BranchId == branchId))
        {
            throw new Exception(
                "Şube bulunamadı.");
        }

        if (departmentId.HasValue)
        {
            ValidateBranchAndDepartment(
                branchId,
                departmentId.Value);
        }

        CheckEmailAndEmployeeNumber(
            email,
            employeeNumber);

        if (string.IsNullOrWhiteSpace(password))
            throw new Exception(
                "Şifre boş bırakılamaz.");

        var staff = new HealthStaff
        {
            FirstName = firstName,
            LastName = lastName,

            Email = email.Trim().ToLowerInvariant(),
            Phone = phone,

            PasswordHash =
                PasswordHasher.HashPassword(password),

            EmployeeNumber = employeeNumber,
            HireDate = hireDate,

            BranchId = branchId,
            DepartmentId = departmentId,

            Position = position
        };

        _db.HealthStaff.Add(staff);
        _db.SaveChanges();

        return staff;
    }


    public Manager AddManager(
        string firstName,
        string lastName,
        string email,
        string phone,
        string employeeNumber,
        DateTime hireDate,
        int branchId)
    {
        if (!_db.Branches.Any(b =>
                b.BranchId == branchId))
        {
            throw new Exception(
                "Şube bulunamadı.");
        }

        CheckEmailAndEmployeeNumber(
            email,
            employeeNumber);

        var manager = new Manager
        {
            FirstName = firstName,
            LastName = lastName,

            Email = email,
            Phone = phone,

            PasswordHash = "NOT_IMPLEMENTED",

            EmployeeNumber = employeeNumber,
            HireDate = hireDate,

            BranchId = branchId
        };

        _db.Managers.Add(manager);
        _db.SaveChanges();

        return manager;
    }


    public void UpdateDoctor(
        int doctorId,
        int branchId,
        int departmentId,
        string specialization)
    {
        var doctor =
            _db.Doctors.Find(doctorId);

        if (doctor == null)
            throw new Exception(
                "Doktor bulunamadı.");

        ValidateBranchAndDepartment(
            branchId,
            departmentId);

        doctor.BranchId = branchId;
        doctor.DepartmentId = departmentId;
        doctor.Specialization = specialization;

        _db.SaveChanges();
    }


    public void AssignService(
        int doctorId,
        int healthServiceId)
    {
        var doctor =
            _db.Doctors.Find(doctorId);

        if (doctor == null)
            throw new Exception(
                "Doktor bulunamadı.");

        var service =
            _db.HealthServices.Find(
                healthServiceId);

        if (service == null)
            throw new Exception(
                "Hizmet bulunamadı.");

        bool serviceAvailableAtBranch =
            _db.BranchServices.Any(bs =>
                bs.BranchId == doctor.BranchId &&
                bs.HealthServiceId == healthServiceId &&
                bs.IsAvailable);

        if (!serviceAvailableAtBranch)
        {
            throw new Exception(
                "Bu hizmet doktorun şubesinde mevcut değil.");
        }

        bool alreadyAssigned =
            _db.DoctorServices.Any(ds =>
                ds.DoctorId == doctorId &&
                ds.HealthServiceId == healthServiceId);

        if (alreadyAssigned)
        {
            throw new Exception(
                "Bu hizmet doktora zaten atanmış.");
        }

        _db.DoctorServices.Add(
            new DoctorService
            {
                DoctorId = doctorId,
                HealthServiceId = healthServiceId
            });

        _db.SaveChanges();
    }


    public DoctorSchedule AddSchedule(
        int doctorId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (startTime >= endTime)
        {
            throw new Exception(
                "Başlangıç saati bitiş saatinden önce olmalıdır.");
        }

        if (!_db.Doctors.Any(d =>
                d.UserId == doctorId))
        {
            throw new Exception(
                "Doktor bulunamadı.");
        }

        bool conflict =
            _db.DoctorSchedules.Any(s =>
                s.DoctorId == doctorId &&
                s.DayOfWeek == dayOfWeek &&
                startTime < s.EndTime &&
                endTime > s.StartTime);

        if (conflict)
        {
            throw new Exception(
                "Doktorun bu saatlerle çakışan çalışma programı var.");
        }

        var schedule =
            new DoctorSchedule
            {
                DoctorId = doctorId,

                DayOfWeek = dayOfWeek,

                StartTime = startTime,
                EndTime = endTime
            };

        _db.DoctorSchedules.Add(schedule);
        _db.SaveChanges();

        return schedule;
    }


    public List<Employee> GetEmployees(
        int? branchId = null)
    {
        var query =
            _db.Employees
                .Include(e => e.Branch)
                .Include(e => e.Department)
                .AsQueryable();

        if (branchId.HasValue)
        {
            query = query.Where(e =>
                e.BranchId ==
                branchId.Value);
        }

        return query
            .OrderBy(e => e.FirstName)
            .ToList();
    }


    public void DeleteEmployee(
        int employeeId)
    {
        var doctor =
            _db.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefault(d =>
                    d.UserId == employeeId);

        if (doctor != null)
        {
            if (doctor.Appointments.Any())
            {
                throw new Exception(
                    "Randevu kaydı bulunan doktor silinemez.");
            }

            var services =
                _db.DoctorServices
                    .Where(ds =>
                        ds.DoctorId == employeeId);

            var schedules =
                _db.DoctorSchedules
                    .Where(s =>
                        s.DoctorId == employeeId);

            _db.DoctorServices.RemoveRange(
                services);

            _db.DoctorSchedules.RemoveRange(
                schedules);

            _db.Doctors.Remove(doctor);

            _db.SaveChanges();
            return;
        }


        var staff =
            _db.HealthStaff.Find(employeeId);

        if (staff != null)
        {
            _db.HealthStaff.Remove(staff);
            _db.SaveChanges();

            return;
        }


        var manager =
            _db.Managers.Find(employeeId);

        if (manager != null)
        {
            _db.Managers.Remove(manager);
            _db.SaveChanges();

            return;
        }

        throw new Exception(
            "Çalışan bulunamadı.");
    }


    private void ValidateBranchAndDepartment(
        int branchId,
        int departmentId)
    {
        if (!_db.Branches.Any(b =>
                b.BranchId == branchId))
        {
            throw new Exception(
                "Şube bulunamadı.");
        }

        bool validDepartment =
            _db.Departments.Any(d =>
                d.DepartmentId ==
                    departmentId &&
                d.BranchId ==
                    branchId);

        if (!validDepartment)
        {
            throw new Exception(
                "Bölüm seçilen şubeye ait değil.");
        }
    }


    private void CheckEmailAndEmployeeNumber(
        string email,
        string employeeNumber)
    {
        if (_db.Users.Any(u =>
                u.Email == email))
        {
            throw new Exception(
                "Bu e-posta adresi zaten kullanılıyor.");
        }

        if (_db.Employees.Any(e =>
                e.EmployeeNumber ==
                    employeeNumber))
        {
            throw new Exception(
                "Bu çalışan numarası zaten kullanılıyor.");
        }
    }

}