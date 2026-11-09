using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Security;

namespace AltunPital.Data;

public static class SeedData
{
    public static void Initialize(
        AppDbContext db)
    {
        // =========================================
        // HOSPITAL
        // =========================================

        var hospital =
            db.Hospitals.FirstOrDefault();

        if (hospital == null)
        {
            hospital =
                new Hospital
                {
                    Name = "AltunPital",

                    Description =
                        "AltunPital Sağlık Grubu"
                };

            db.Hospitals.Add(
                hospital);

            db.SaveChanges();
        }


        // =========================================
        // BRANCHES
        // =========================================

        var sakaryaBranch =
            db.Branches.FirstOrDefault(b =>
                b.Name ==
                "AltunPital Sakarya");

        if (sakaryaBranch == null)
        {
            sakaryaBranch =
                new Branch
                {
                    HospitalId =
                        hospital.HospitalId,

                    Name =
                        "AltunPital Sakarya",

                    City =
                        "Sakarya",

                    District =
                        "Serdivan",

                    Address =
                        "Serdivan, Sakarya",

                    Phone =
                        "0264 000 00 01",

                    OpeningTime =
                        new TimeOnly(8, 0),

                    ClosingTime =
                        new TimeOnly(18, 0)
                };

            db.Branches.Add(
                sakaryaBranch);

            db.SaveChanges();
        }


        var kocaeliBranch =
            db.Branches.FirstOrDefault(b =>
                b.Name ==
                "AltunPital Kocaeli");

        if (kocaeliBranch == null)
        {
            kocaeliBranch =
                new Branch
                {
                    HospitalId =
                        hospital.HospitalId,

                    Name =
                        "AltunPital Kocaeli",

                    City =
                        "Kocaeli",

                    District =
                        "İzmit",

                    Address =
                        "İzmit, Kocaeli",

                    Phone =
                        "0262 000 00 01",

                    OpeningTime =
                        new TimeOnly(8, 30),

                    ClosingTime =
                        new TimeOnly(19, 0)
                };

            db.Branches.Add(
                kocaeliBranch);

            db.SaveChanges();
        }


        // =========================================
        // DEPARTMENTS
        // =========================================

        AddDepartmentIfMissing(
            db,
            sakaryaBranch.BranchId,
            "Kardiyoloji",
            "Kalp ve damar hastalıkları");

        AddDepartmentIfMissing(
            db,
            sakaryaBranch.BranchId,
            "Radyoloji",
            "Tıbbi görüntüleme");

        AddDepartmentIfMissing(
            db,
            kocaeliBranch.BranchId,
            "Kardiyoloji",
            "Kalp ve damar hastalıkları");


        // =========================================
        // SERVICES
        // =========================================

        var general =
            GetOrCreateService(
                db,
                "Genel Muayene",
                "Standart doktor muayenesi",
                30);

        var cardiology =
            GetOrCreateService(
                db,
                "Kardiyoloji Muayenesi",
                "Kardiyoloji uzman muayenesi",
                30);

        var mri =
            GetOrCreateService(
                db,
                "MR",
                "Manyetik rezonans görüntüleme",
                45);


        AddBranchServiceIfMissing(
            db,
            sakaryaBranch.BranchId,
            general.HealthServiceId,
            900);

        AddBranchServiceIfMissing(
            db,
            sakaryaBranch.BranchId,
            cardiology.HealthServiceId,
            1400);

        AddBranchServiceIfMissing(
            db,
            sakaryaBranch.BranchId,
            mri.HealthServiceId,
            2500);


        AddBranchServiceIfMissing(
            db,
            kocaeliBranch.BranchId,
            cardiology.HealthServiceId,
            1550);

        AddBranchServiceIfMissing(
            db,
            kocaeliBranch.BranchId,
            mri.HealthServiceId,
            2800);


        // =========================================
        // EQUIPMENT
        // =========================================

        var mriMachine =
            GetOrCreateEquipment(
                db,
                "MR Cihazı",
                "Manyetik rezonans görüntüleme cihazı");

        var xrayMachine =
            GetOrCreateEquipment(
                db,
                "Röntgen Cihazı",
                "Dijital röntgen sistemi");


        AddBranchEquipmentIfMissing(
            db,
            sakaryaBranch.BranchId,
            mriMachine.EquipmentId,
            2);

        AddBranchEquipmentIfMissing(
            db,
            sakaryaBranch.BranchId,
            xrayMachine.EquipmentId,
            1);

        AddBranchEquipmentIfMissing(
            db,
            kocaeliBranch.BranchId,
            mriMachine.EquipmentId,
            1);


        // =========================================
        // DEFAULT MANAGER
        // =========================================

        const string managerEmail =
            "manager@altunpital.com";

        bool managerExists =
            db.Users.Any(u =>
                u.Email == managerEmail);

        if (!managerExists)
        {
            var manager =
                new Manager
                {
                    FirstName =
                        "Altun",

                    LastName =
                        "Pital",

                    Email =
                        managerEmail,

                    Phone =
                        "0500 000 00 00",

                    PasswordHash =
                        PasswordHasher
                            .HashPassword(
                                "asdf1234!"),

                    EmployeeNumber =
                        "ALT-MNG-001",

                    HireDate =
                        DateTime.Today,

                    BranchId =
                        sakaryaBranch.BranchId,

                    DepartmentId =
                        null
                };


            db.Managers.Add(
                manager);

            db.SaveChanges();
        }
    }


    // =================================================
    // HELPERS
    // =================================================

    private static void
        AddDepartmentIfMissing(
            AppDbContext db,
            int branchId,
            string name,
            string description)
    {
        bool exists =
            db.Departments.Any(d =>
                d.BranchId ==
                    branchId &&
                d.Name ==
                    name);

        if (exists)
            return;


        db.Departments.Add(
            new Department
            {
                BranchId =
                    branchId,

                Name =
                    name,

                Description =
                    description
            });

        db.SaveChanges();
    }


    private static HealthService
        GetOrCreateService(
            AppDbContext db,
            string name,
            string description,
            int duration)
    {
        var service =
            db.HealthServices
                .FirstOrDefault(s =>
                    s.Name == name);

        if (service != null)
            return service;


        service =
            new HealthService
            {
                Name =
                    name,

                Description =
                    description,

                DurationMinutes =
                    duration
            };


        db.HealthServices.Add(
            service);

        db.SaveChanges();

        return service;
    }


    private static void
        AddBranchServiceIfMissing(
            AppDbContext db,
            int branchId,
            int serviceId,
            decimal price)
    {
        bool exists =
            db.BranchServices.Any(bs =>
                bs.BranchId ==
                    branchId &&
                bs.HealthServiceId ==
                    serviceId);

        if (exists)
            return;


        db.BranchServices.Add(
            new BranchService
            {
                BranchId =
                    branchId,

                HealthServiceId =
                    serviceId,

                Price =
                    price,

                IsAvailable =
                    true
            });


        db.SaveChanges();
    }


    private static Equipment
        GetOrCreateEquipment(
            AppDbContext db,
            string name,
            string description)
    {
        var equipment =
            db.Equipment
                .FirstOrDefault(e =>
                    e.Name == name);

        if (equipment != null)
            return equipment;


        equipment =
            new Equipment
            {
                Name =
                    name,

                Description =
                    description
            };


        db.Equipment.Add(
            equipment);

        db.SaveChanges();

        return equipment;
    }


    private static void
        AddBranchEquipmentIfMissing(
            AppDbContext db,
            int branchId,
            int equipmentId,
            int quantity)
    {
        bool exists =
            db.BranchEquipment.Any(be =>
                be.BranchId ==
                    branchId &&
                be.EquipmentId ==
                    equipmentId);

        if (exists)
            return;


        db.BranchEquipment.Add(
            new BranchEquipment
            {
                BranchId =
                    branchId,

                EquipmentId =
                    equipmentId,

                Quantity =
                    quantity,

                IsOperational =
                    true
            });


        db.SaveChanges();
    }
}