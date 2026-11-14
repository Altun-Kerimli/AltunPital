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

    // Şubeler

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
        if (!_db.Hospitals.Any(h => h.HospitalId == hospitalId))
        {
            throw new Exception("Hastane bulunamadı.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Şube adı zorunludur.");
        }

        if (openingTime >= closingTime)
        {
            throw new Exception("Açılış saati kapanış saatinden önce olmalıdır.");
        }

        if (_db.Branches.Any(b => b.Name == name.Trim()))
        {
            throw new Exception("Bu isimde bir şube zaten mevcut.");
        }

        var branch = new Branch
        {
            HospitalId = hospitalId,
            Name = name.Trim(),
            City = city.Trim(),
            District = district.Trim(),
            Address = address.Trim(),
            Phone = phone.Trim(),
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
        var branch = _db.Branches.FirstOrDefault(b => b.BranchId == branchId)
            ?? throw new Exception("Şube bulunamadı.");

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Şube adı zorunludur.");
        }

        if (openingTime >= closingTime)
        {
            throw new Exception("Açılış saati kapanış saatinden önce olmalıdır.");
        }

        if (_db.Branches.Any(b => b.BranchId != branchId && b.Name == name.Trim()))
        {
            throw new Exception("Bu isimde başka bir şube zaten mevcut.");
        }

        branch.Name = name.Trim();
        branch.City = city.Trim();
        branch.District = district.Trim();
        branch.Address = address.Trim();
        branch.Phone = phone.Trim();
        branch.OpeningTime = openingTime;
        branch.ClosingTime = closingTime;

        _db.SaveChanges();
    }

    public void DeleteBranch(int branchId)
    {
        var branch = _db.Branches.FirstOrDefault(b => b.BranchId == branchId)
            ?? throw new Exception("Şube bulunamadı.");

        // Bağlı kayıt varsa silinemez
        bool hasRelatedData =
            _db.Departments.Any(d => d.BranchId == branchId)
            || _db.Employees.Any(e => e.BranchId == branchId)
            || _db.BranchServices.Any(bs => bs.BranchId == branchId)
            || _db.BranchEquipment.Any(be => be.BranchId == branchId)
            || _db.Appointments.Any(a => a.BranchId == branchId);

        if (hasRelatedData)
        {
            throw new Exception("Bu şubeye bağlı bölüm, çalışan, hizmet, ekipman veya randevu bulunduğu için şube silinemez.");
        }

        _db.Branches.Remove(branch);
        _db.SaveChanges();
    }

    public List<Branch> GetBranches()
    {
        return _db.Branches
            .OrderBy(b => b.BranchId)
            .ToList();
    }

    // Bölümler

    public Department AddDepartment(int branchId, string name, string description)
    {
        if (!_db.Branches.Any(b => b.BranchId == branchId))
        {
            throw new Exception("Şube bulunamadı.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Bölüm adı zorunludur.");
        }

        if (_db.Departments.Any(d => d.BranchId == branchId && d.Name == name.Trim()))
        {
            throw new Exception("Bu şubede aynı isimde bir bölüm zaten mevcut.");
        }

        var department = new Department
        {
            BranchId = branchId,
            Name = name.Trim(),
            Description = description.Trim()
        };

        _db.Departments.Add(department);
        _db.SaveChanges();

        return department;
    }

    public void UpdateDepartment(int departmentId, string name, string description)
    {
        var department = _db.Departments.FirstOrDefault(d => d.DepartmentId == departmentId)
            ?? throw new Exception("Bölüm bulunamadı.");

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Bölüm adı zorunludur.");
        }

        if (_db.Departments.Any(d =>
                d.DepartmentId != departmentId &&
                d.BranchId == department.BranchId &&
                d.Name == name.Trim()))
        {
            throw new Exception("Bu şubede aynı isimde başka bir bölüm mevcut.");
        }

        department.Name = name.Trim();
        department.Description = description.Trim();

        _db.SaveChanges();
    }

    public void DeleteDepartment(int departmentId)
    {
        var department = _db.Departments.FirstOrDefault(d => d.DepartmentId == departmentId)
            ?? throw new Exception("Bölüm bulunamadı.");

        if (_db.Employees.Any(e => e.DepartmentId == departmentId))
        {
            throw new Exception("Bu bölüme bağlı çalışanlar bulunduğu için bölüm silinemez.");
        }

        _db.Departments.Remove(department);
        _db.SaveChanges();
    }

    public List<Department> GetDepartments(int branchId)
    {
        return _db.Departments
            .Where(d => d.BranchId == branchId)
            .OrderBy(d => d.DepartmentId)
            .ToList();
    }

    // Hizmetler

    public HealthService AddHealthService(string name, string description, int durationMinutes)
    {
        ValidateService(name, durationMinutes);

        var service = new HealthService
        {
            Name = name.Trim(),
            Description = description.Trim(),
            DurationMinutes = durationMinutes
        };

        _db.HealthServices.Add(service);
        _db.SaveChanges();

        return service;
    }

    public void UpdateHealthService(int healthServiceId, string name, string description, int durationMinutes)
    {
        var service = _db.HealthServices.FirstOrDefault(s => s.HealthServiceId == healthServiceId)
            ?? throw new Exception("Hizmet bulunamadı.");

        ValidateService(name, durationMinutes);

        service.Name = name.Trim();
        service.Description = description.Trim();
        service.DurationMinutes = durationMinutes;

        _db.SaveChanges();
    }

    public void DeleteHealthService(int healthServiceId)
    {
        var service = _db.HealthServices.FirstOrDefault(s => s.HealthServiceId == healthServiceId)
            ?? throw new Exception("Hizmet bulunamadı.");

        bool used =
            _db.BranchServices.Any(bs => bs.HealthServiceId == healthServiceId)
            || _db.DoctorServices.Any(ds => ds.HealthServiceId == healthServiceId)
            || _db.Appointments.Any(a => a.HealthServiceId == healthServiceId);

        if (used)
        {
            throw new Exception("Bu hizmet aktif ilişkilerde veya randevularda kullanıldığı için silinemez.");
        }

        _db.HealthServices.Remove(service);
        _db.SaveChanges();
    }

    public List<HealthService> GetHealthServices()
    {
        return _db.HealthServices
            .OrderBy(s => s.HealthServiceId)
            .ToList();
    }

    // Şube hizmetleri

    public void AssignServiceToBranch(int branchId, int healthServiceId, decimal price)
    {
        if (price < 0)
        {
            throw new Exception("Fiyat negatif olamaz.");
        }

        if (!_db.Branches.Any(b => b.BranchId == branchId))
        {
            throw new Exception("Şube bulunamadı.");
        }

        if (!_db.HealthServices.Any(s => s.HealthServiceId == healthServiceId))
        {
            throw new Exception("Hizmet bulunamadı.");
        }

        var existing = _db.BranchServices.FirstOrDefault(bs =>
            bs.BranchId == branchId &&
            bs.HealthServiceId == healthServiceId);

        // Varsa güncelle, yoksa ekle
        if (existing != null)
        {
            existing.Price = price;
            existing.IsAvailable = true;
        }
        else
        {
            _db.BranchServices.Add(new BranchService
            {
                BranchId = branchId,
                HealthServiceId = healthServiceId,
                Price = price,
                IsAvailable = true
            });
        }

        _db.SaveChanges();
    }

    public List<BranchService> GetBranchServices(int? branchId = null)
    {
        IQueryable<BranchService> query = _db.BranchServices
            .Include(bs => bs.Branch)
            .Include(bs => bs.HealthService);

        if (branchId.HasValue)
        {
            query = query.Where(bs => bs.BranchId == branchId.Value);
        }

        return query
            .OrderBy(bs => bs.BranchId)
            .ThenBy(bs => bs.HealthServiceId)
            .ToList();
    }

    public void UpdateBranchService(int branchId, int healthServiceId, decimal price, bool isAvailable)
    {
        if (price < 0)
        {
            throw new Exception("Fiyat negatif olamaz.");
        }

        var relation = _db.BranchServices.FirstOrDefault(bs =>
                bs.BranchId == branchId &&
                bs.HealthServiceId == healthServiceId)
            ?? throw new Exception("Şube hizmet kaydı bulunamadı.");

        relation.Price = price;
        relation.IsAvailable = isAvailable;

        _db.SaveChanges();
    }

    public void RemoveServiceFromBranch(int branchId, int healthServiceId)
    {
        var relation = _db.BranchServices.FirstOrDefault(bs =>
                bs.BranchId == branchId &&
                bs.HealthServiceId == healthServiceId)
            ?? throw new Exception("Şube-hizmet bağlantısı bulunamadı.");

        if (_db.Appointments.Any(a => a.BranchId == branchId && a.HealthServiceId == healthServiceId))
        {
            throw new Exception("Bu hizmete ait randevular bulunduğu için hizmet şubeden kaldırılamaz. Hizmeti pasif yapabilirsiniz.");
        }

        if (_db.DoctorServices.Any(ds => ds.HealthServiceId == healthServiceId))
        {
            throw new Exception("Bu hizmet doktorlara atanmış durumda. Önce doktor-hizmet bağlantılarını kaldırınız.");
        }

        _db.BranchServices.Remove(relation);
        _db.SaveChanges();
    }

    public HealthService AddHealthServiceToBranch(
        int branchId,
        string name,
        string description,
        int duration,
        decimal price,
        bool isAvailable)
    {
        ValidateService(name, duration);

        if (price < 0)
        {
            throw new Exception("Fiyat negatif olamaz.");
        }

        if (!_db.Branches.Any(b => b.BranchId == branchId))
        {
            throw new Exception("Şube bulunamadı.");
        }

        using var transaction = _db.Database.BeginTransaction();

        try
        {
            var service = new HealthService
            {
                Name = name.Trim(),
                Description = description.Trim(),
                DurationMinutes = duration
            };

            _db.HealthServices.Add(service);
            _db.SaveChanges();

            _db.BranchServices.Add(new BranchService
            {
                BranchId = branchId,
                HealthServiceId = service.HealthServiceId,
                Price = price,
                IsAvailable = isAvailable
            });

            _db.SaveChanges();
            transaction.Commit();

            return service;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateBranchServiceDetails(
        int branchId,
        int healthServiceId,
        string name,
        string description,
        int duration,
        decimal price,
        bool isAvailable)
    {
        ValidateService(name, duration);

        if (price < 0)
        {
            throw new Exception("Fiyat negatif olamaz.");
        }

        using var transaction = _db.Database.BeginTransaction();

        try
        {
            var service = _db.HealthServices.FirstOrDefault(s => s.HealthServiceId == healthServiceId)
                ?? throw new Exception("Hizmet bulunamadı.");

            var relation = _db.BranchServices.FirstOrDefault(bs =>
                    bs.BranchId == branchId &&
                    bs.HealthServiceId == healthServiceId)
                ?? throw new Exception("Şube hizmet kaydı bulunamadı.");

            service.Name = name.Trim();
            service.Description = description.Trim();
            service.DurationMinutes = duration;

            relation.Price = price;
            relation.IsAvailable = isAvailable;

            _db.SaveChanges();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // Ekipman

    public Equipment AddEquipment(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Ekipman adı zorunludur.");
        }

        var equipment = new Equipment
        {
            Name = name.Trim(),
            Description = description.Trim()
        };

        _db.Equipment.Add(equipment);
        _db.SaveChanges();

        return equipment;
    }

    public void UpdateEquipment(int equipmentId, string name, string description)
    {
        var equipment = _db.Equipment.FirstOrDefault(e => e.EquipmentId == equipmentId)
            ?? throw new Exception("Ekipman bulunamadı.");

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Ekipman adı zorunludur.");
        }

        equipment.Name = name.Trim();
        equipment.Description = description.Trim();

        _db.SaveChanges();
    }

    public void DeleteEquipment(int equipmentId)
    {
        var equipment = _db.Equipment.FirstOrDefault(e => e.EquipmentId == equipmentId)
            ?? throw new Exception("Ekipman bulunamadı.");

        if (_db.BranchEquipment.Any(be => be.EquipmentId == equipmentId))
        {
            throw new Exception("Bu ekipman bir şubeye bağlı olduğu için silinemez.");
        }

        _db.Equipment.Remove(equipment);
        _db.SaveChanges();
    }

    // Şube ekipman

    public void AssignEquipmentToBranch(int branchId, int equipmentId, int quantity)
    {
        if (quantity < 0)
        {
            throw new Exception("Ekipman adedi negatif olamaz.");
        }

        if (!_db.Branches.Any(b => b.BranchId == branchId))
        {
            throw new Exception("Şube bulunamadı.");
        }

        if (!_db.Equipment.Any(e => e.EquipmentId == equipmentId))
        {
            throw new Exception("Ekipman bulunamadı.");
        }

        var existing = _db.BranchEquipment.FirstOrDefault(be =>
            be.BranchId == branchId &&
            be.EquipmentId == equipmentId);

        if (existing != null)
        {
            existing.Quantity = quantity;
            existing.IsOperational = true;
        }
        else
        {
            _db.BranchEquipment.Add(new BranchEquipment
            {
                BranchId = branchId,
                EquipmentId = equipmentId,
                Quantity = quantity,
                IsOperational = true
            });
        }

        _db.SaveChanges();
    }

    public List<Equipment> GetEquipment()
    {
        return _db.Equipment
            .OrderBy(e => e.EquipmentId)
            .ToList();
    }

    public List<BranchEquipment> GetBranchEquipment(int? branchId = null)
    {
        IQueryable<BranchEquipment> query = _db.BranchEquipment
            .Include(be => be.Branch)
            .Include(be => be.Equipment);

        if (branchId.HasValue)
        {
            query = query.Where(be => be.BranchId == branchId.Value);
        }

        return query
            .OrderBy(be => be.BranchId)
            .ThenBy(be => be.EquipmentId)
            .ToList();
    }

    public void UpdateBranchEquipment(int branchId, int equipmentId, int quantity, bool isOperational)
    {
        if (quantity < 0)
        {
            throw new Exception("Ekipman adedi negatif olamaz.");
        }

        var relation = _db.BranchEquipment.FirstOrDefault(be =>
                be.BranchId == branchId &&
                be.EquipmentId == equipmentId)
            ?? throw new Exception("Şube ekipman kaydı bulunamadı.");

        relation.Quantity = quantity;
        relation.IsOperational = isOperational;

        _db.SaveChanges();
    }

    public void RemoveEquipmentFromBranch(int branchId, int equipmentId)
    {
        var relation = _db.BranchEquipment.FirstOrDefault(be =>
                be.BranchId == branchId &&
                be.EquipmentId == equipmentId)
            ?? throw new Exception("Şube-ekipman bağlantısı bulunamadı.");

        _db.BranchEquipment.Remove(relation);
        _db.SaveChanges();
    }

    public Equipment AddEquipmentToBranch(
        int branchId,
        string name,
        string description,
        int quantity,
        bool isOperational)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Ekipman adı zorunludur.");
        }

        if (quantity < 0)
        {
            throw new Exception("Ekipman adedi negatif olamaz.");
        }

        if (!_db.Branches.Any(b => b.BranchId == branchId))
        {
            throw new Exception("Şube bulunamadı.");
        }

        using var transaction = _db.Database.BeginTransaction();

        try
        {
            var equipment = new Equipment
            {
                Name = name.Trim(),
                Description = description.Trim()
            };

            _db.Equipment.Add(equipment);
            _db.SaveChanges();

            _db.BranchEquipment.Add(new BranchEquipment
            {
                BranchId = branchId,
                EquipmentId = equipment.EquipmentId,
                Quantity = quantity,
                IsOperational = isOperational
            });

            _db.SaveChanges();
            transaction.Commit();

            return equipment;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateBranchEquipmentDetails(
        int branchId,
        int equipmentId,
        string name,
        string description,
        int quantity,
        bool isOperational)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Ekipman adı zorunludur.");
        }

        if (quantity < 0)
        {
            throw new Exception("Ekipman adedi negatif olamaz.");
        }

        using var transaction = _db.Database.BeginTransaction();

        try
        {
            var equipment = _db.Equipment.FirstOrDefault(e => e.EquipmentId == equipmentId)
                ?? throw new Exception("Ekipman bulunamadı.");

            var relation = _db.BranchEquipment.FirstOrDefault(be =>
                    be.BranchId == branchId &&
                    be.EquipmentId == equipmentId)
                ?? throw new Exception("Şube ekipman kaydı bulunamadı.");

            equipment.Name = name.Trim();
            equipment.Description = description.Trim();

            relation.Quantity = quantity;
            relation.IsOperational = isOperational;

            _db.SaveChanges();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // Randevular

    public List<Appointment> GetBranchAppointments(int branchId)
    {
        return _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
            .Include(a => a.HealthService)
            .Include(a => a.Branch)
            .Where(a => a.BranchId == branchId)
            .OrderBy(a => a.AppointmentId)
            .ToList();
    }

    private static void ValidateService(string name, int duration)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Hizmet adı zorunludur.");
        }

        if (duration <= 0)
        {
            throw new Exception("Hizmet süresi sıfırdan büyük olmalıdır.");
        }
    }
}
