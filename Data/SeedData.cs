using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Security;

namespace AltunPital.Data;

public static class SeedData
{
    private record DoctorSeed(
        string FirstName,
        string LastName,
        string Department,
        string Specialization,
        string Service);

    private record StaffSeed(
        string FirstName,
        string LastName,
        string Department,
        string Position);

    private record PatientSeed(
        string FirstName,
        string LastName);


    public static void Initialize(
        AppDbContext db)
    {
        // =================================================
        // HASTANE
        // =================================================

        var hospital =
            db.Hospitals
                .FirstOrDefault(h =>
                    h.Name == "AltunPital");

        if (hospital == null)
        {
            hospital =
                new Hospital
                {
                    Name =
                        "AltunPital",

                    Description =
                        "AltunPital Sağlık Grubu"
                };


            db.Hospitals.Add(
                hospital);

            db.SaveChanges();
        }


        // =================================================
        // HİZMETLER
        // =================================================

        var services =
            new[]
            {
                new
                {
                    Name = "Genel Muayene",
                    Description = "Genel doktor muayenesi",
                    Duration = 30
                },

                new
                {
                    Name = "Kardiyoloji Muayenesi",
                    Description = "Kalp ve damar muayenesi",
                    Duration = 30
                },

                new
                {
                    Name = "Ortopedi Muayenesi",
                    Description = "Kas ve iskelet sistemi muayenesi",
                    Duration = 30
                },

                new
                {
                    Name = "Çocuk Sağlığı Muayenesi",
                    Description = "Çocuk hastalıkları muayenesi",
                    Duration = 30
                },

                new
                {
                    Name = "Dermatoloji Muayenesi",
                    Description = "Deri hastalıkları muayenesi",
                    Duration = 30
                },

                new
                {
                    Name = "MR",
                    Description = "Manyetik rezonans görüntüleme",
                    Duration = 45
                },

                new
                {
                    Name = "Laboratuvar",
                    Description = "Temel laboratuvar işlemleri",
                    Duration = 20
                }
            };


        foreach (var item
                 in services)
        {
            if (!db.HealthServices.Any(s =>
                    s.Name == item.Name))
            {
                db.HealthServices.Add(
                    new HealthService
                    {
                        Name =
                            item.Name,

                        Description =
                            item.Description,

                        DurationMinutes =
                            item.Duration
                    });
            }
        }

        db.SaveChanges();


        // =================================================
        // EKİPMANLAR
        // =================================================

        var equipmentSeeds =
            new[]
            {
                "MR Cihazı",
                "Röntgen Cihazı",
                "Ultrason Cihazı",
                "EKG Cihazı",
                "Defibrilatör",
                "Hasta Monitörü",
                "Laboratuvar Analizörü"
            };


        foreach (string name
                 in equipmentSeeds)
        {
            if (!db.Equipment.Any(e =>
                    e.Name == name))
            {
                db.Equipment.Add(
                    new Equipment
                    {
                        Name =
                            name,

                        Description =
                            $"{name} - AltunPital tıbbi ekipmanı"
                    });
            }
        }

        db.SaveChanges();


        // =================================================
        // ŞEHİRLER
        // =================================================

        var cities =
            new[]
            {
                new
                {
                    City = "Sakarya",
                    District = "Serdivan"
                },

                new
                {
                    City = "İstanbul",
                    District = "Kadıköy"
                },

                new
                {
                    City = "Kocaeli",
                    District = "İzmit"
                },

                new
                {
                    City = "Ankara",
                    District = "Çankaya"
                },

                new
                {
                    City = "Bursa",
                    District = "Nilüfer"
                }
            };


        var departmentNames =
            new[]
            {
                "Genel Tıp",
                "Kardiyoloji",
                "Ortopedi",
                "Çocuk Sağlığı",
                "Dermatoloji",
                "Radyoloji"
            };


        var doctorTemplates =
            new[]
            {
                new DoctorSeed(
                    "Ahmet",
                    "Yılmaz",
                    "Genel Tıp",
                    "Genel Pratisyen",
                    "Genel Muayene"),

                new DoctorSeed(
                    "Mehmet",
                    "Kaya",
                    "Kardiyoloji",
                    "Kardiyoloji",
                    "Kardiyoloji Muayenesi"),

                new DoctorSeed(
                    "Zeynep",
                    "Demir",
                    "Ortopedi",
                    "Ortopedi",
                    "Ortopedi Muayenesi"),

                new DoctorSeed(
                    "Elif",
                    "Çelik",
                    "Çocuk Sağlığı",
                    "Çocuk Sağlığı ve Hastalıkları",
                    "Çocuk Sağlığı Muayenesi"),

                new DoctorSeed(
                    "Burak",
                    "Aydın",
                    "Dermatoloji",
                    "Dermatoloji",
                    "Dermatoloji Muayenesi"),

                new DoctorSeed(
                    "Selin",
                    "Arslan",
                    "Radyoloji",
                    "Radyoloji",
                    "MR")
            };


        var staffTemplates =
            new[]
            {
                new StaffSeed(
                    "Ayşe",
                    "Koç",
                    "Kardiyoloji",
                    "Hemşire"),

                new StaffSeed(
                    "Emre",
                    "Şahin",
                    "Radyoloji",
                    "Radyoloji Teknisyeni"),

                new StaffSeed(
                    "Merve",
                    "Aksoy",
                    "Çocuk Sağlığı",
                    "Hemşire"),

                new StaffSeed(
                    "Can",
                    "Öztürk",
                    "Genel Tıp",
                    "Sağlık Teknikeri")
            };


        var patientTemplates =
            new[]
            {
                new PatientSeed(
                    "Ali",
                    "Demir"),

                new PatientSeed(
                    "Deniz",
                    "Yıldız"),

                new PatientSeed(
                    "Ece",
                    "Acar")
            };


        int cityIndex =
            1;


        foreach (var location
                 in cities)
        {
            // -----------------------------------------
            // ŞUBE
            // -----------------------------------------

            string branchName =
                $"AltunPital {location.City}";


            var branch =
                db.Branches
                    .FirstOrDefault(b =>
                        b.Name ==
                        branchName);


            if (branch == null)
            {
                branch =
                    new Branch
                    {
                        HospitalId =
                            hospital.HospitalId,

                        Name =
                            branchName,

                        City =
                            location.City,

                        District =
                            location.District,

                        Address =
                            $"{location.District}, {location.City}",

                        Phone =
                            $"0{260 + cityIndex} 000 00 0{cityIndex}",

                        OpeningTime =
                            new TimeOnly(
                                8,
                                0),

                        ClosingTime =
                            new TimeOnly(
                                18,
                                0)
                    };


                db.Branches.Add(
                    branch);

                db.SaveChanges();
            }


            // -----------------------------------------
            // BÖLÜMLER
            // -----------------------------------------

            foreach (string departmentName
                     in departmentNames)
            {
                if (!db.Departments.Any(d =>
                        d.BranchId ==
                            branch.BranchId &&
                        d.Name ==
                            departmentName))
                {
                    db.Departments.Add(
                        new Department
                        {
                            BranchId =
                                branch.BranchId,

                            Name =
                                departmentName,

                            Description =
                                $"{departmentName} Bölümü"
                        });
                }
            }

            db.SaveChanges();


            // -----------------------------------------
            // ŞUBE HİZMETLERİ
            // -----------------------------------------

            var allServices =
                db.HealthServices
                    .ToList();


            foreach (var service
                     in allServices)
            {
                bool exists =
                    db.BranchServices.Any(bs =>
                        bs.BranchId ==
                            branch.BranchId &&
                        bs.HealthServiceId ==
                            service.HealthServiceId);

                if (!exists)
                {
                    db.BranchServices.Add(
                        new BranchService
                        {
                            BranchId =
                                branch.BranchId,

                            HealthServiceId =
                                service.HealthServiceId,

                            Price =
                                700 +
                                service.HealthServiceId *
                                150 +
                                cityIndex *
                                50,

                            IsAvailable =
                                true
                        });
                }
            }

            db.SaveChanges();


            // -----------------------------------------
            // ŞUBE EKİPMANLARI
            // -----------------------------------------

            var equipments =
                db.Equipment
                    .ToList();


            foreach (var equipment
                     in equipments)
            {
                bool exists =
                    db.BranchEquipment.Any(be =>
                        be.BranchId ==
                            branch.BranchId &&
                        be.EquipmentId ==
                            equipment.EquipmentId);

                if (!exists)
                {
                    db.BranchEquipment.Add(
                        new BranchEquipment
                        {
                            BranchId =
                                branch.BranchId,

                            EquipmentId =
                                equipment.EquipmentId,

                            Quantity =
                                1 +
                                (
                                    equipment.EquipmentId +
                                    cityIndex
                                ) % 3,

                            IsOperational =
                                true
                        });
                }
            }

            db.SaveChanges();


            // -----------------------------------------
            // DOKTORLAR
            // -----------------------------------------

            int doctorNumber =
                1;


            foreach (var template
                     in doctorTemplates)
            {
                string email =
                    $"doktor{cityIndex}{doctorNumber}@altunpital.com";


                var doctor =
                    db.Doctors
                        .FirstOrDefault(d =>
                            d.Email ==
                            email);


                if (doctor == null)
                {
                    var department =
                        db.Departments
                            .First(d =>
                                d.BranchId ==
                                    branch.BranchId &&
                                d.Name ==
                                    template.Department);


                    doctor =
                        new Doctor
                        {
                            FirstName =
                                template.FirstName,

                            LastName =
                                $"{template.LastName} {location.City}",

                            Email =
                                email,

                            Phone =
                                $"0555{cityIndex}{doctorNumber}0001",

                            PasswordHash =
                                PasswordHasher.HashPassword(
                                    "doktor123"),

                            EmployeeNumber =
                                $"DOC-{cityIndex:00}-{doctorNumber:00}",

                            HireDate =
                                DateTime.Today
                                    .AddYears(
                                        -doctorNumber),

                            BranchId =
                                branch.BranchId,

                            DepartmentId =
                                department.DepartmentId,

                            Specialization =
                                template.Specialization
                        };


                    db.Doctors.Add(
                        doctor);

                    db.SaveChanges();
                }


                var healthService =
                    db.HealthServices
                        .First(s =>
                            s.Name ==
                            template.Service);


                bool doctorServiceExists =
                    db.DoctorServices.Any(ds =>
                        ds.DoctorId ==
                            doctor.UserId &&
                        ds.HealthServiceId ==
                            healthService.HealthServiceId);


                if (!doctorServiceExists)
                {
                    db.DoctorServices.Add(
                        new DoctorService
                        {
                            DoctorId =
                                doctor.UserId,

                            HealthServiceId =
                                healthService.HealthServiceId
                        });
                }


                for (int day = 1;
                     day <= 5;
                     day++)
                {
                    DayOfWeek weekDay =
                        (DayOfWeek)day;


                    bool scheduleExists =
                        db.DoctorSchedules.Any(s =>
                            s.DoctorId ==
                                doctor.UserId &&
                            s.DayOfWeek ==
                                weekDay);


                    if (!scheduleExists)
                    {
                        db.DoctorSchedules.Add(
                            new DoctorSchedule
                            {
                                DoctorId =
                                    doctor.UserId,

                                DayOfWeek =
                                    weekDay,

                                StartTime =
                                    new TimeOnly(
                                        9,
                                        0),

                                EndTime =
                                    new TimeOnly(
                                        17,
                                        0)
                            });
                    }
                }


                db.SaveChanges();

                doctorNumber++;
            }


            // -----------------------------------------
            // SAĞLIK PERSONELİ
            // -----------------------------------------

            int staffNumber =
                1;


            foreach (var template
                     in staffTemplates)
            {
                string email =
                    $"personel{cityIndex}{staffNumber}@altunpital.com";


                if (!db.HealthStaff.Any(s =>
                        s.Email ==
                        email))
                {
                    var department =
                        db.Departments
                            .First(d =>
                                d.BranchId ==
                                    branch.BranchId &&
                                d.Name ==
                                    template.Department);


                    db.HealthStaff.Add(
                        new HealthStaff
                        {
                            FirstName =
                                template.FirstName,

                            LastName =
                                $"{template.LastName} {location.City}",

                            Email =
                                email,

                            Phone =
                                $"0532{cityIndex}{staffNumber}0001",

                            PasswordHash =
                                PasswordHasher.HashPassword(
                                    "personel123"),

                            EmployeeNumber =
                                $"STAFF-{cityIndex:00}-{staffNumber:00}",

                            HireDate =
                                DateTime.Today
                                    .AddYears(
                                        -staffNumber),

                            BranchId =
                                branch.BranchId,

                            DepartmentId =
                                department.DepartmentId,

                            Position =
                                template.Position
                        });
                }


                staffNumber++;
            }

            db.SaveChanges();


            // -----------------------------------------
            // HASTALAR
            // -----------------------------------------

            int patientNumber =
                1;


            foreach (var template
                     in patientTemplates)
            {
                string email =
                    $"hasta{cityIndex}{patientNumber}@test.com";


                if (!db.Patients.Any(p =>
                        p.Email ==
                        email))
                {
                    db.Patients.Add(
                        new Patient
                        {
                            FirstName =
                                template.FirstName,

                            LastName =
                                $"{template.LastName} {location.City}",

                            Email =
                                email,

                            Phone =
                                $"0542{cityIndex}{patientNumber}0001",

                            City =
                                location.City,

                            PasswordHash =
                                PasswordHasher.HashPassword(
                                    "hasta123")
                        });
                }


                patientNumber++;
            }

            db.SaveChanges();


            // -----------------------------------------
            // RANDEVULAR
            // -----------------------------------------

            var branchPatients =
                db.Patients
                    .Where(p =>
                        p.City ==
                        location.City)
                    .Take(3)
                    .ToList();


            var branchDoctors =
                db.Doctors
                    .Where(d =>
                        d.BranchId ==
                        branch.BranchId)
                    .Take(3)
                    .ToList();


            var statuses =
                new[]
                {
                    AppointmentStatus.Pending,
                    AppointmentStatus.Approved,
                    AppointmentStatus.Completed
                };


            for (int i = 0;
                 i < Math.Min(
                     branchPatients.Count,
                     branchDoctors.Count);
                 i++)
            {
                var patient =
                    branchPatients[i];

                var doctor =
                    branchDoctors[i];


                var doctorService =
                    db.DoctorServices
                        .FirstOrDefault(ds =>
                            ds.DoctorId ==
                            doctor.UserId);


                if (doctorService == null)
                    continue;


                var service =
                    db.HealthServices
                        .First(s =>
                            s.HealthServiceId ==
                            doctorService.HealthServiceId);


                DateTime start =
                    DateTime.Today
                        .AddDays(
                            7 + cityIndex + i)
                        .AddHours(
                            10 + i);


                bool exists =
                    db.Appointments.Any(a =>
                        a.PatientId ==
                            patient.UserId &&
                        a.DoctorId ==
                            doctor.UserId &&
                        a.StartDateTime ==
                            start);


                if (exists)
                    continue;


                decimal price =
                    db.BranchServices
                        .Where(bs =>
                            bs.BranchId ==
                                branch.BranchId &&
                            bs.HealthServiceId ==
                                service.HealthServiceId)
                        .Select(bs =>
                            bs.Price)
                        .First();


                db.Appointments.Add(
                    new Appointment
                    {
                        PatientId =
                            patient.UserId,

                        DoctorId =
                            doctor.UserId,

                        BranchId =
                            branch.BranchId,

                        HealthServiceId =
                            service.HealthServiceId,

                        StartDateTime =
                            start,

                        EndDateTime =
                            start.AddMinutes(
                                service.DurationMinutes),

                        Price =
                            price,

                        Status =
                            statuses[
                                i %
                                statuses.Length],

                        CreatedAt =
                            DateTime.Now
                                .AddDays(
                                    -3)
                    });
            }


            db.SaveChanges();

            cityIndex++;
        }


        // =================================================
        // DEFAULT MANAGER
        // =================================================

        const string managerEmail =
            "manager@altunpital.com";


        if (!db.Managers.Any(m =>
                m.Email ==
                managerEmail))
        {
            var sakarya =
                db.Branches
                    .First(b =>
                        b.City ==
                        "Sakarya");


            db.Managers.Add(
                new Manager
                {
                    FirstName =
                        "Altun",

                    LastName =
                        "Pital",

                    Email =
                        managerEmail,

                    Phone =
                        "05000000000",

                    PasswordHash =
                        PasswordHasher.HashPassword(
                            "asdf1234!"),

                    EmployeeNumber =
                        "ALT-MNG-001",

                    HireDate =
                        DateTime.Today
                            .AddYears(-5),

                    BranchId =
                        sakarya.BranchId,

                    DepartmentId =
                        null
                });


            db.SaveChanges();
        }
    }
}