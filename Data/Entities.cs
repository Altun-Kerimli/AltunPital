using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltunPital.Data;

public enum AppointmentStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled,
    Completed
}

public abstract class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Phone { get; set; } = "";
}

public class Patient : User
{
    public List<Appointment> Appointments { get; set; } = new();
}

public abstract class Employee : User
{
    public string EmployeeNumber { get; set; } = "";

    public DateTime HireDate { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
}

public class Doctor : Employee
{
    public string Specialization { get; set; } = "";

    public List<DoctorService> DoctorServices { get; set; } = new();

    public List<DoctorSchedule> Schedules { get; set; } = new();

    public List<Appointment> Appointments { get; set; } = new();
}

public class HealthStaff : Employee
{
    public string Position { get; set; } = "";
}

public class Manager : Employee
{
}

public class Hospital
{
    public int HospitalId { get; set; }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public List<Branch> Branches { get; set; } = new();
}

public class Branch
{
    public int BranchId { get; set; }

    public string Name { get; set; } = "";

    public string City { get; set; } = "";
    public string District { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";

    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }

    public int HospitalId { get; set; }
    public Hospital Hospital { get; set; } = null!;

    public List<Department> Departments { get; set; } = new();

    public List<Employee> Employees { get; set; } = new();

    public List<BranchService> BranchServices { get; set; } = new();

    public List<BranchEquipment> BranchEquipments { get; set; } = new();

    public List<Appointment> Appointments { get; set; } = new();
}

public class Department
{
    public int DepartmentId { get; set; }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public List<Employee> Employees { get; set; } = new();
}

public class HealthService
{
    public int HealthServiceId { get; set; }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public int DurationMinutes { get; set; }

    public List<BranchService> BranchServices { get; set; } = new();

    public List<DoctorService> DoctorServices { get; set; } = new();

    public List<Appointment> Appointments { get; set; } = new();
}

public class BranchService
{
    public int BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public int HealthServiceId { get; set; }

    public HealthService HealthService { get; set; } = null!;

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }
}

public class Equipment
{
    public int EquipmentId { get; set; }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public List<BranchEquipment> BranchEquipments { get; set; } = new();
}

public class BranchEquipment
{
    public int BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public int EquipmentId { get; set; }

    public Equipment Equipment { get; set; } = null!;

    public int Quantity { get; set; }

    public bool IsOperational { get; set; }
}

public class DoctorService
{
    public int DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public int HealthServiceId { get; set; }

    public HealthService HealthService { get; set; } = null!;
}

public class DoctorSchedule
{
    public int DoctorScheduleId { get; set; }

    public int DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}

public class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public int HealthServiceId { get; set; }
    public HealthService HealthService { get; set; } = null!;

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public decimal Price { get; set; }

    public AppointmentStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
