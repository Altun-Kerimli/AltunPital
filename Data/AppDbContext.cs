using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace AltunPital.Data;

public class AppDbContext : DbContext
{
    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();

    public DbSet<HealthService> HealthServices => Set<HealthService>();
    public DbSet<BranchService> BranchServices => Set<BranchService>();

    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<BranchEquipment> BranchEquipment => Set<BranchEquipment>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<HealthStaff> HealthStaff => Set<HealthStaff>();
    public DbSet<Manager> Managers => Set<Manager>();

    public DbSet<DoctorService> DoctorServices => Set<DoctorService>();
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=altunpital.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Kullanıcı tipleri ayrı tablolarda (TPT)
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Patient>().ToTable("Patients");
        modelBuilder.Entity<Employee>().ToTable("Employees");
        modelBuilder.Entity<Doctor>().ToTable("Doctors");
        modelBuilder.Entity<HealthStaff>().ToTable("HealthStaff");
        modelBuilder.Entity<Manager>().ToTable("Managers");

        modelBuilder.Entity<Hospital>()
            .HasMany(h => h.Branches)
            .WithOne(b => b.Hospital)
            .HasForeignKey(b => b.HospitalId);

        modelBuilder.Entity<Branch>()
            .HasMany(b => b.Departments)
            .WithOne(d => d.Branch)
            .HasForeignKey(d => d.BranchId);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Branch)
            .WithMany(b => b.Employees)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BranchService>()
            .HasKey(bs => new { bs.BranchId, bs.HealthServiceId });

        modelBuilder.Entity<BranchService>()
            .HasOne(bs => bs.Branch)
            .WithMany(b => b.BranchServices)
            .HasForeignKey(bs => bs.BranchId);

        modelBuilder.Entity<BranchService>()
            .HasOne(bs => bs.HealthService)
            .WithMany(s => s.BranchServices)
            .HasForeignKey(bs => bs.HealthServiceId);

        modelBuilder.Entity<BranchEquipment>()
            .HasKey(be => new { be.BranchId, be.EquipmentId });

        modelBuilder.Entity<BranchEquipment>()
            .HasOne(be => be.Branch)
            .WithMany(b => b.BranchEquipments)
            .HasForeignKey(be => be.BranchId);

        modelBuilder.Entity<BranchEquipment>()
            .HasOne(be => be.Equipment)
            .WithMany(e => e.BranchEquipments)
            .HasForeignKey(be => be.EquipmentId);

        modelBuilder.Entity<DoctorService>()
            .HasKey(ds => new { ds.DoctorId, ds.HealthServiceId });

        modelBuilder.Entity<DoctorService>()
            .HasOne(ds => ds.Doctor)
            .WithMany(d => d.DoctorServices)
            .HasForeignKey(ds => ds.DoctorId);

        modelBuilder.Entity<DoctorService>()
            .HasOne(ds => ds.HealthService)
            .WithMany(s => s.DoctorServices)
            .HasForeignKey(ds => ds.HealthServiceId);

        modelBuilder.Entity<DoctorSchedule>()
            .HasOne(s => s.Doctor)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DoctorId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Branch)
            .WithMany(b => b.Appointments)
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.HealthService)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.HealthServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enum'u metin olarak tutuyoruz
        modelBuilder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasConversion<string>();

        modelBuilder.Entity<BranchService>()
            .Property(bs => bs.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Price)
            .HasPrecision(10, 2);
    }
}
