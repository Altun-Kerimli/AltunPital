using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltunPital.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Hospitals.Any())
            return;


        // --------------------
        // HOSPITAL
        // --------------------

        var hospital = new Hospital
        {
            Name = "AltunPital",
            Description = "AltunPital Sağlık Grubu"
        };

        db.Hospitals.Add(hospital);
        db.SaveChanges();


        // --------------------
        // BRANCHES
        // --------------------

        var sakaryaBranch = new Branch
        {
            HospitalId = hospital.HospitalId,

            Name = "AltunPital Sakarya",

            City = "Sakarya",
            District = "Serdivan",

            Address = "Serdivan, Sakarya",

            Phone = "0264 000 00 01",

            OpeningTime = new TimeOnly(8, 0),
            ClosingTime = new TimeOnly(18, 0)
        };


        var kocaeliBranch = new Branch
        {
            HospitalId = hospital.HospitalId,

            Name = "AltunPital Kocaeli",

            City = "Kocaeli",
            District = "İzmit",

            Address = "İzmit, Kocaeli",

            Phone = "0262 000 00 01",

            OpeningTime = new TimeOnly(8, 30),
            ClosingTime = new TimeOnly(19, 0)
        };


        db.Branches.AddRange(
            sakaryaBranch,
            kocaeliBranch
        );

        db.SaveChanges();


        // --------------------
        // DEPARTMENTS
        // --------------------

        var sakaryaCardiology = new Department
        {
            BranchId = sakaryaBranch.BranchId,

            Name = "Kardiyoloji",

            Description = "Kalp ve damar hastalıkları"
        };


        var sakaryaRadiology = new Department
        {
            BranchId = sakaryaBranch.BranchId,

            Name = "Radyoloji",

            Description = "Tıbbi görüntüleme"
        };


        var kocaeliCardiology = new Department
        {
            BranchId = kocaeliBranch.BranchId,

            Name = "Kardiyoloji",

            Description = "Kalp ve damar hastalıkları"
        };


        db.Departments.AddRange(
            sakaryaCardiology,
            sakaryaRadiology,
            kocaeliCardiology
        );

        db.SaveChanges();


        // --------------------
        // SERVICES
        // --------------------

        var generalExamination = new HealthService
        {
            Name = "Genel Muayene",

            Description = "Standart doktor muayenesi",

            DurationMinutes = 30
        };


        var cardiologyExamination = new HealthService
        {
            Name = "Kardiyoloji Muayenesi",

            Description = "Kardiyoloji uzmanı muayenesi",

            DurationMinutes = 30
        };


        var mri = new HealthService
        {
            Name = "MR",

            Description = "Manyetik rezonans görüntüleme",

            DurationMinutes = 45
        };


        db.HealthServices.AddRange(
            generalExamination,
            cardiologyExamination,
            mri
        );

        db.SaveChanges();


        // --------------------
        // BRANCH SERVICE PRICES
        // --------------------

        db.BranchServices.AddRange(

            new BranchService
            {
                BranchId = sakaryaBranch.BranchId,

                HealthServiceId =
                    generalExamination.HealthServiceId,

                Price = 900,

                IsAvailable = true
            },

            new BranchService
            {
                BranchId = sakaryaBranch.BranchId,

                HealthServiceId =
                    cardiologyExamination.HealthServiceId,

                Price = 1400,

                IsAvailable = true
            },

            new BranchService
            {
                BranchId = sakaryaBranch.BranchId,

                HealthServiceId = mri.HealthServiceId,

                Price = 2500,

                IsAvailable = true
            },

            new BranchService
            {
                BranchId = kocaeliBranch.BranchId,

                HealthServiceId =
                    cardiologyExamination.HealthServiceId,

                Price = 1550,

                IsAvailable = true
            },

            new BranchService
            {
                BranchId = kocaeliBranch.BranchId,

                HealthServiceId = mri.HealthServiceId,

                Price = 2800,

                IsAvailable = true
            }
        );


        // --------------------
        // EQUIPMENT
        // --------------------

        var mriMachine = new Equipment
        {
            Name = "MR Cihazı",

            Description =
                "Manyetik rezonans görüntüleme cihazı"
        };


        var xrayMachine = new Equipment
        {
            Name = "Röntgen Cihazı",

            Description =
                "Dijital röntgen sistemi"
        };


        db.Equipment.AddRange(
            mriMachine,
            xrayMachine
        );

        db.SaveChanges();


        db.BranchEquipment.AddRange(

            new BranchEquipment
            {
                BranchId = sakaryaBranch.BranchId,

                EquipmentId = mriMachine.EquipmentId,

                Quantity = 2,

                IsOperational = true
            },

            new BranchEquipment
            {
                BranchId = sakaryaBranch.BranchId,

                EquipmentId = xrayMachine.EquipmentId,

                Quantity = 1,

                IsOperational = true
            },

            new BranchEquipment
            {
                BranchId = kocaeliBranch.BranchId,

                EquipmentId = mriMachine.EquipmentId,

                Quantity = 1,

                IsOperational = true
            }
        );


        // --------------------
        // DOCTOR
        // --------------------

        var doctor = new Doctor
        {
            FirstName = "Ahmet",

            LastName = "Yılmaz",

            Email = "ahmet.yilmaz@altunpital.local",

            Phone = "0500 000 00 01",

            PasswordHash = "NOT_IMPLEMENTED",

            EmployeeNumber = "ALT-DOC-001",

            HireDate = new DateTime(2025, 1, 10),

            BranchId = sakaryaBranch.BranchId,

            DepartmentId =
                sakaryaCardiology.DepartmentId,

            Specialization = "Kardiyoloji"
        };


        db.Doctors.Add(doctor);


        // --------------------
        // PATIENT
        // --------------------

        var patient = new Patient
        {
            FirstName = "Ali",

            LastName = "Demir",

            Email = "ali@example.com",

            Phone = "0555 000 00 01",

            PasswordHash = "NOT_IMPLEMENTED"
        };


        db.Patients.Add(patient);

        db.SaveChanges();


        // --------------------
        // DOCTOR SERVICES
        // --------------------

        db.DoctorServices.Add(

            new DoctorService
            {
                DoctorId = doctor.UserId,

                HealthServiceId =
                    cardiologyExamination.HealthServiceId
            }
        );


        // --------------------
        // SCHEDULE
        // --------------------

        db.DoctorSchedules.AddRange(

            new DoctorSchedule
            {
                DoctorId = doctor.UserId,

                DayOfWeek = DayOfWeek.Monday,

                StartTime = new TimeOnly(9, 0),

                EndTime = new TimeOnly(17, 0)
            },

            new DoctorSchedule
            {
                DoctorId = doctor.UserId,

                DayOfWeek = DayOfWeek.Tuesday,

                StartTime = new TimeOnly(9, 0),

                EndTime = new TimeOnly(17, 0)
            },

            new DoctorSchedule
            {
                DoctorId = doctor.UserId,

                DayOfWeek = DayOfWeek.Wednesday,

                StartTime = new TimeOnly(9, 0),

                EndTime = new TimeOnly(17, 0)
            }
        );


        db.SaveChanges();
    }
}