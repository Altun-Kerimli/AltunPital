using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AltunPital.Services;

public class ManagerService : IManagerService
{
    private readonly AppDbContext _db;

    public ManagerService(AppDbContext db)
    {
        _db = db;
    }


    // =====================================================
    // BRANCH
    // =====================================================

    public Branch AddBranch(
        int hospitalId,
        string name,
        string city,
        string district,
        string address,
        string phone,
        TimeOnly openingTime,
        TimeOnly closingTime)
    {
        if (!_db.Hospitals.Any(h =>
                h.HospitalId == hospitalId))
        {
            throw new Exception("Hastane bulunamadı.");
        }

        if (openingTime >= closingTime)
        {
            throw new Exception(
                "Açılış saati kapanış saatinden önce olmalıdır.");
        }

        var branch = new Branch
        {
            HospitalId = hospitalId,
            Name = name,
            City = city,
            District = district,
            Address = address,
            Phone = phone,
            OpeningTime = openingTime,
            ClosingTime = closingTime
        };

        _db.Branches.Add(branch);
        _db.SaveChanges();

        return branch;
    }


    public void UpdateBranch(
        int branchId,
        string name,
        string city,
        string district,
        string address,
        string phone,
        TimeOnly openingTime,
        TimeOnly closingTime)
    {
        var branch =
            _db.Branches.Find(branchId);

        if (branch == null)
            throw new Exception("Şube bulunamadı.");

        if (openingTime >= closingTime)
        {
            throw new Exception(
                "Açılış saati kapanış saatinden önce olmalıdır.");
        }

        branch.Name = name;
        branch.City = city;
        branch.District = district;
        branch.Address = address;
        branch.Phone = phone;
        branch.OpeningTime = openingTime;
        branch.ClosingTime = closingTime;

        _db.SaveChanges();
    }


    public void DeleteBranch(int branchId)
    {
        var branch =
            _db.Branches
                .Include(b => b.Employees)
                .Include(b => b.Appointments)
                .FirstOrDefault(b =>
                    b.BranchId == branchId);

        if (branch == null)
            throw new Exception("Şube bulunamadı.");

        if (branch.Employees.Any())
        {
            throw new Exception(
                "Çalışanı bulunan şube silinemez.");
        }

        if (branch.Appointments.Any())
        {
            throw new Exception(
                "Randevu kaydı bulunan şube silinemez.");
        }

        var departments =
            _db.Departments
                .Where(d =>
                    d.BranchId == branchId);

        var services =
            _db.BranchServices
                .Where(bs =>
                    bs.BranchId == branchId);

        var equipment =
            _db.BranchEquipment
                .Where(be =>
                    be.BranchId == branchId);

        _db.BranchServices.RemoveRange(services);
        _db.BranchEquipment.RemoveRange(equipment);
        _db.Departments.RemoveRange(departments);

        _db.Branches.Remove(branch);

        _db.SaveChanges();
    }


    public List<Branch> GetBranches()
    {
        return _db.Branches
            .OrderBy(b => b.City)
            .ThenBy(b => b.Name)
            .ToList();
    }


    // =====================================================
    // DEPARTMENT
    // =====================================================

    public Department AddDepartment(
        int branchId,
        string name,
        string description)
    {
        if (!_db.Branches.Any(b =>
                b.BranchId == branchId))
        {
            throw new Exception("Şube bulunamadı.");
        }

        bool exists =
            _db.Departments.Any(d =>
                d.BranchId == branchId &&
                d.Name == name);

        if (exists)
        {
            throw new Exception(
                "Bu bölüm seçilen şubede zaten mevcut.");
        }

        var department = new Department
        {
            BranchId = branchId,
            Name = name,
            Description = description
        };

        _db.Departments.Add(department);
        _db.SaveChanges();

        return department;
    }


    public void UpdateDepartment(
        int departmentId,
        string name,
        string description)
    {
        var department =
            _db.Departments.Find(departmentId);

        if (department == null)
            throw new Exception("Bölüm bulunamadı.");

        department.Name = name;
        department.Description = description;

        _db.SaveChanges();
    }


    public void DeleteDepartment(int departmentId)
    {
        var department =
            _db.Departments
                .Include(d => d.Employees)
                .FirstOrDefault(d =>
                    d.DepartmentId == departmentId);

        if (department == null)
            throw new Exception("Bölüm bulunamadı.");

        if (department.Employees.Any())
        {
            throw new Exception(
                "Çalışanı bulunan bölüm silinemez.");
        }

        _db.Departments.Remove(department);
        _db.SaveChanges();
    }


    public List<Department> GetDepartments(
        int branchId)
    {
        return _db.Departments
            .Where(d => d.BranchId == branchId)
            .OrderBy(d => d.Name)
            .ToList();
    }


    // =====================================================
    // HEALTH SERVICE
    // =====================================================

    public HealthService AddHealthService(
        string name,
        string description,
        int durationMinutes)
    {
        if (durationMinutes <= 0)
        {
            throw new Exception(
                "Hizmet süresi sıfırdan büyük olmalıdır.");
        }

        var service = new HealthService
        {
            Name = name,
            Description = description,
            DurationMinutes = durationMinutes
        };

        _db.HealthServices.Add(service);
        _db.SaveChanges();

        return service;
    }


    public void UpdateHealthService(
        int healthServiceId,
        string name,
        string description,
        int durationMinutes)
    {
        var service =
            _db.HealthServices.Find(
                healthServiceId);

        if (service == null)
            throw new Exception("Hizmet bulunamadı.");

        if (durationMinutes <= 0)
        {
            throw new Exception(
                "Hizmet süresi sıfırdan büyük olmalıdır.");
        }

        service.Name = name;
        service.Description = description;
        service.DurationMinutes = durationMinutes;

        _db.SaveChanges();
    }


    public void DeleteHealthService(
        int healthServiceId)
    {
        bool usedInAppointment =
            _db.Appointments.Any(a =>
                a.HealthServiceId ==
                healthServiceId);

        if (usedInAppointment)
        {
            throw new Exception(
                "Randevu kaydı bulunan hizmet silinemez.");
        }

        var service =
            _db.HealthServices.Find(
                healthServiceId);

        if (service == null)
            throw new Exception("Hizmet bulunamadı.");

        var branchServices =
            _db.BranchServices
                .Where(bs =>
                    bs.HealthServiceId ==
                    healthServiceId);

        var doctorServices =
            _db.DoctorServices
                .Where(ds =>
                    ds.HealthServiceId ==
                    healthServiceId);

        _db.BranchServices.RemoveRange(
            branchServices);

        _db.DoctorServices.RemoveRange(
            doctorServices);

        _db.HealthServices.Remove(service);

        _db.SaveChanges();
    }


    public List<HealthService>
        GetHealthServices()
    {
        return _db.HealthServices
            .OrderBy(s => s.Name)
            .ToList();
    }


    // =====================================================
    // BRANCH SERVICE
    // =====================================================

    public void AssignServiceToBranch(
        int branchId,
        int healthServiceId,
        decimal price)
    {
        if (price < 0)
            throw new Exception(
                "Fiyat negatif olamaz.");

        if (!_db.Branches.Any(b =>
                b.BranchId == branchId))
        {
            throw new Exception("Şube bulunamadı.");
        }

        if (!_db.HealthServices.Any(s =>
                s.HealthServiceId ==
                healthServiceId))
        {
            throw new Exception("Hizmet bulunamadı.");
        }

        bool exists =
            _db.BranchServices.Any(bs =>
                bs.BranchId == branchId &&
                bs.HealthServiceId ==
                    healthServiceId);

        if (exists)
        {
            throw new Exception(
                "Hizmet bu şubeye zaten atanmış.");
        }

        _db.BranchServices.Add(
            new BranchService
            {
                BranchId = branchId,
                HealthServiceId = healthServiceId,
                Price = price,
                IsAvailable = true
            });

        _db.SaveChanges();
    }


    public void UpdateBranchService(
        int branchId,
        int healthServiceId,
        decimal price,
        bool isAvailable)
    {
        var branchService =
            _db.BranchServices.Find(
                branchId,
                healthServiceId);

        if (branchService == null)
            throw new Exception(
                "Şube hizmeti bulunamadı.");

        if (price < 0)
            throw new Exception(
                "Fiyat negatif olamaz.");

        branchService.Price = price;
        branchService.IsAvailable = isAvailable;

        _db.SaveChanges();
    }


    public void RemoveServiceFromBranch(
        int branchId,
        int healthServiceId)
    {
        bool appointmentExists =
            _db.Appointments.Any(a =>
                a.BranchId == branchId &&
                a.HealthServiceId ==
                    healthServiceId);

        if (appointmentExists)
        {
            throw new Exception(
                "Randevu geçmişi bulunan hizmet şubeden silinemez. Pasif hale getirin.");
        }

        var branchService =
            _db.BranchServices.Find(
                branchId,
                healthServiceId);

        if (branchService == null)
            throw new Exception(
                "Şube hizmeti bulunamadı.");

        _db.BranchServices.Remove(
            branchService);

        _db.SaveChanges();
    }


    // =====================================================
    // EQUIPMENT
    // =====================================================

    public Equipment AddEquipment(
        string name,
        string description)
    {
        var equipment = new Equipment
        {
            Name = name,
            Description = description
        };

        _db.Equipment.Add(equipment);
        _db.SaveChanges();

        return equipment;
    }


    public void UpdateEquipment(
        int equipmentId,
        string name,
        string description)
    {
        var equipment =
            _db.Equipment.Find(equipmentId);

        if (equipment == null)
            throw new Exception(
                "Ekipman bulunamadı.");

        equipment.Name = name;
        equipment.Description = description;

        _db.SaveChanges();
    }


    public void DeleteEquipment(
        int equipmentId)
    {
        var equipment =
            _db.Equipment.Find(equipmentId);

        if (equipment == null)
            throw new Exception(
                "Ekipman bulunamadı.");

        var branchEquipment =
            _db.BranchEquipment
                .Where(be =>
                    be.EquipmentId ==
                    equipmentId);

        _db.BranchEquipment.RemoveRange(
            branchEquipment);

        _db.Equipment.Remove(equipment);

        _db.SaveChanges();
    }


    public void AssignEquipmentToBranch(
        int branchId,
        int equipmentId,
        int quantity)
    {
        if (quantity <= 0)
        {
            throw new Exception(
                "Ekipman adedi sıfırdan büyük olmalıdır.");
        }

        if (!_db.Branches.Any(b =>
                b.BranchId == branchId))
        {
            throw new Exception("Şube bulunamadı.");
        }

        if (!_db.Equipment.Any(e =>
                e.EquipmentId == equipmentId))
        {
            throw new Exception(
                "Ekipman bulunamadı.");
        }

        bool exists =
            _db.BranchEquipment.Any(be =>
                be.BranchId == branchId &&
                be.EquipmentId ==
                    equipmentId);

        if (exists)
        {
            throw new Exception(
                "Ekipman bu şubede zaten tanımlı.");
        }

        _db.BranchEquipment.Add(
            new BranchEquipment
            {
                BranchId = branchId,
                EquipmentId = equipmentId,
                Quantity = quantity,
                IsOperational = true
            });

        _db.SaveChanges();
    }


    public void UpdateBranchEquipment(
        int branchId,
        int equipmentId,
        int quantity,
        bool isOperational)
    {
        var branchEquipment =
            _db.BranchEquipment.Find(
                branchId,
                equipmentId);

        if (branchEquipment == null)
        {
            throw new Exception(
                "Şube ekipmanı bulunamadı.");
        }

        if (quantity < 0)
        {
            throw new Exception(
                "Ekipman adedi negatif olamaz.");
        }

        branchEquipment.Quantity = quantity;
        branchEquipment.IsOperational =
            isOperational;

        _db.SaveChanges();
    }

    public void RemoveEquipmentFromBranch(
        int branchId,
        int equipmentId)
    {
        var branchEquipment =
            _db.BranchEquipment.Find(
                branchId,
                equipmentId);

        if (branchEquipment == null)
        {
            throw new Exception(
                "Şube ekipmanı bulunamadı.");
        }

        _db.BranchEquipment.Remove(
            branchEquipment);

        _db.SaveChanges();
    }

    public List<Equipment> GetEquipment()
    {
        return _db.Equipment
            .OrderBy(e => e.Name)
            .ToList();
    }


    public List<BranchEquipment>
        GetBranchEquipment(
            int? branchId = null)
    {
        var query =
            _db.BranchEquipment
                .Include(be =>
                    be.Branch)
                .Include(be =>
                    be.Equipment)
                .AsQueryable();


        if (branchId.HasValue)
        {
            query =
                query.Where(be =>
                    be.BranchId ==
                    branchId.Value);
        }


        return query
            .OrderBy(be =>
                be.Branch.Name)
            .ThenBy(be =>
                be.Equipment.Name)
            .ToList();
    }

    public List<BranchService> GetBranchServices(int? branchId = null)
    {
        var query =
            _db.BranchServices
                .Include(bs =>
                    bs.Branch)
                .Include(bs =>
                    bs.HealthService)
                .AsQueryable();


        if (branchId.HasValue)
        {
            query =
                query.Where(bs =>
                    bs.BranchId ==
                    branchId.Value);
        }


        return query.ToList();
    }

    public List<Appointment> GetBranchAppointments(int branchId)
    {
        return _db.Appointments
            .Include(a =>
                a.Patient)
            .Include(a =>
                a.Doctor)
            .Include(a =>
                a.HealthService)
            .Include(a =>
                a.Branch)
            .Where(a =>
                a.BranchId ==
                branchId)
            .OrderBy(a =>
                a.AppointmentId)
            .ToList();
    }

}