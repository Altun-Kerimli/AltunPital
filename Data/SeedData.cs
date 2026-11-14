using AltunPital.Security;
using AltunPital.Services;

namespace AltunPital.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        // Seed sadece boş DB'de çalışır
        if (db.Hospitals.Any() || db.Users.Any() || db.Branches.Any())
        {
            return;
        }

        var hospital = new Hospital
        {
            Name = "AltunPital",
            Description = "AltunPital Hastaneler Grubu"
        };

        db.Hospitals.Add(hospital);
        db.SaveChanges();

        // Şubeler
        SeedBranch(
            db,
            hospital,
            "AltunPital Sakarya",
            "Sakarya",
            "Serdivan",
            "Serdivan, Sakarya",
            "0264 000 00 01",
            ["Genel Tıp", "Kardiyoloji", "Çocuk Sağlığı", "Radyoloji"],
            [
                new("Genel Muayene", "Genel sağlık muayenesi", 30, 900),
                new("Kardiyoloji Muayenesi", "Kardiyoloji uzman muayenesi", 30, 1300),
                new("Çocuk Sağlığı Muayenesi", "Çocuk sağlığı muayenesi", 30, 1100),
                new("MR", "Manyetik rezonans görüntüleme", 45, 2500)
            ],
            [
                new("MR Cihazı", "Radyoloji MR sistemi", 2),
                new("Ultrason Cihazı", "Ultrason görüntüleme cihazı", 2),
                new("EKG Cihazı", "Elektrokardiyografi cihazı", 3)
            ],
            [
                new(
                    "Mehmet",
                    "Kaya",
                    "mehmet.kaya@altunpital.com",
                    "05010000001",
                    "SAK-DOC-001",
                    "Kardiyoloji",
                    "Kardiyoloji",
                    ["Kardiyoloji Muayenesi"]),
                new(
                    "Selin",
                    "Yalçın",
                    "selin.yalcin@altunpital.com",
                    "05010000002",
                    "SAK-DOC-002",
                    "Genel Tıp",
                    "Genel Cerrahi",
                    ["Genel Muayene"]),
                new(
                    "Murat",
                    "Akın",
                    "murat.akin@altunpital.com",
                    "05010000003",
                    "SAK-DOC-003",
                    "Çocuk Sağlığı",
                    "Çocuk Sağlığı",
                    ["Çocuk Sağlığı Muayenesi"])
            ],
            [
                new(
                    "Ayşe",
                    "Koç",
                    "ayse.koc@altunpital.com",
                    "05011000001",
                    "SAK-STAFF-001",
                    "Kardiyoloji",
                    "Hemşire"),
                new(
                    "Emre",
                    "Şahin",
                    "emre.sahin@altunpital.com",
                    "05011000002",
                    "SAK-STAFF-002",
                    "Radyoloji",
                    "Radyoloji Teknisyeni")
            ],
            [
                new("Ali", "Demir", "ali.demir@example.com", "05012000001"),
                new("Deniz", "Yıldız", "deniz.yildiz@example.com", "05012000002"),
                new("Ece", "Acar", "ece.acar@example.com", "05012000003")
            ]);

        SeedBranch(
            db,
            hospital,
            "AltunPital İstanbul",
            "İstanbul",
            "Kadıköy",
            "Kadıköy, İstanbul",
            "0216 000 00 02",
            ["Nöroloji", "Dermatoloji", "KBB", "Göz", "Dahiliye"],
            [
                new("Nöroloji Muayenesi", "Nöroloji uzman muayenesi", 30, 1500),
                new("Dermatoloji Muayenesi", "Dermatoloji muayenesi", 30, 1400),
                new("KBB Muayenesi", "Kulak burun boğaz muayenesi", 30, 1250),
                new("Göz Muayenesi", "Göz hastalıkları muayenesi", 30, 1300)
            ],
            [
                new("EEG Cihazı", "EEG sistemi", 3),
                new("Göz Muayene Ünitesi", "Göz muayene sistemi", 4),
                new("KBB Muayene Ünitesi", "KBB muayene sistemi", 3)
            ],
            [
                new(
                    "Kerem",
                    "Arslan",
                    "kerem.arslan@altunpital.com",
                    "05020000001",
                    "IST-DOC-001",
                    "Nöroloji",
                    "Nöroloji",
                    ["Nöroloji Muayenesi"]),
                new(
                    "Derya",
                    "Tunç",
                    "derya.tunc@altunpital.com",
                    "05020000002",
                    "IST-DOC-002",
                    "Dermatoloji",
                    "Dermatoloji",
                    ["Dermatoloji Muayenesi"]),
                new(
                    "Onur",
                    "Korkmaz",
                    "onur.korkmaz@altunpital.com",
                    "05020000003",
                    "IST-DOC-003",
                    "KBB",
                    "Kulak Burun Boğaz",
                    ["KBB Muayenesi"])
            ],
            [
                new(
                    "Seda",
                    "Balcı",
                    "seda.balci@altunpital.com",
                    "05021000001",
                    "IST-STAFF-001",
                    "Nöroloji",
                    "Hemşire"),
                new(
                    "Kaan",
                    "Ateş",
                    "kaan.ates@altunpital.com",
                    "05021000002",
                    "IST-STAFF-002",
                    "KBB",
                    "Sağlık Teknikeri")
            ],
            [
                new("Naz", "Özkan", "naz.ozkan@example.com", "05022000001"),
                new("Serkan", "Bulut", "serkan.bulut@example.com", "05022000002"),
                new("Melis", "Avcı", "melis.avci@example.com", "05022000003")
            ]);

        SeedBranch(
            db,
            hospital,
            "AltunPital Kocaeli",
            "Kocaeli",
            "İzmit",
            "İzmit, Kocaeli",
            "0262 000 00 03",
            ["Ortopedi", "Kardiyoloji", "Genel Tıp", "Radyoloji"],
            [
                new("Ortopedi Muayenesi", "Ortopedi uzman muayenesi", 30, 1250),
                new("Kardiyoloji Muayenesi", "Kardiyoloji uzman muayenesi", 30, 1300),
                new("Genel Muayene", "Genel sağlık muayenesi", 30, 900),
                new("MR", "MR görüntüleme", 45, 2400)
            ],
            [
                new("Dijital Röntgen Cihazı", "Dijital röntgen sistemi", 3),
                new("MR Cihazı", "MR görüntüleme sistemi", 1),
                new("Defibrilatör", "Acil müdahale cihazı", 3)
            ],
            [
                new(
                    "Hakan",
                    "Duman",
                    "hakan.duman@altunpital.com",
                    "05030000001",
                    "KOC-DOC-001",
                    "Ortopedi",
                    "Ortopedi",
                    ["Ortopedi Muayenesi"]),
                new(
                    "Aslı",
                    "Özer",
                    "asli.ozer@altunpital.com",
                    "05030000002",
                    "KOC-DOC-002",
                    "Kardiyoloji",
                    "Kardiyoloji",
                    ["Kardiyoloji Muayenesi"]),
                new(
                    "Umut",
                    "Turan",
                    "umut.turan@altunpital.com",
                    "05030000003",
                    "KOC-DOC-003",
                    "Genel Tıp",
                    "Dahiliye",
                    ["Genel Muayene"])
            ],
            [
                new(
                    "Gamze",
                    "Doğan",
                    "gamze.dogan@altunpital.com",
                    "05031000001",
                    "KOC-STAFF-001",
                    "Ortopedi",
                    "Hemşire"),
                new(
                    "Oğuz",
                    "Çakır",
                    "oguz.cakir@altunpital.com",
                    "05031000002",
                    "KOC-STAFF-002",
                    "Radyoloji",
                    "Radyoloji Teknisyeni")
            ],
            [
                new("Burcu", "Sezer", "burcu.sezer@example.com", "05032000001"),
                new("Okan", "Işık", "okan.isik@example.com", "05032000002"),
                new("Leyla", "Kaplan", "leyla.kaplan@example.com", "05032000003")
            ]);

        SeedBranch(
            db,
            hospital,
            "AltunPital Ankara",
            "Ankara",
            "Çankaya",
            "Çankaya, Ankara",
            "0312 000 00 04",
            ["Dahiliye", "Nöroloji", "Kadın Hastalıkları", "Kardiyoloji"],
            [
                new("Dahiliye Muayenesi", "İç hastalıkları muayenesi", 30, 1100),
                new("Nöroloji Muayenesi", "Nöroloji uzman muayenesi", 30, 1450),
                new("Kadın Hastalıkları Muayenesi", "Kadın hastalıkları muayenesi", 30, 1400),
                new("Kardiyoloji Muayenesi", "Kardiyoloji uzman muayenesi", 30, 1350)
            ],
            [
                new("Ultrason Cihazı", "Ultrason sistemi", 4),
                new("EEG Cihazı", "EEG sistemi", 2),
                new("EKG Cihazı", "EKG sistemi", 3)
            ],
            [
                new(
                    "Esra",
                    "Bayram",
                    "esra.bayram@altunpital.com",
                    "05040000001",
                    "ANK-DOC-001",
                    "Dahiliye",
                    "Dahiliye",
                    ["Dahiliye Muayenesi"]),
                new(
                    "Volkan",
                    "Aksu",
                    "volkan.aksu@altunpital.com",
                    "05040000002",
                    "ANK-DOC-002",
                    "Nöroloji",
                    "Nöroloji",
                    ["Nöroloji Muayenesi"]),
                new(
                    "Şebnem",
                    "Güler",
                    "sebnem.guler@altunpital.com",
                    "05040000003",
                    "ANK-DOC-003",
                    "Kadın Hastalıkları",
                    "Kadın Hastalıkları",
                    ["Kadın Hastalıkları Muayenesi"])
            ],
            [
                new(
                    "Tuğba",
                    "Ekinci",
                    "tugba.ekinci@altunpital.com",
                    "05041000001",
                    "ANK-STAFF-001",
                    "Dahiliye",
                    "Hemşire"),
                new(
                    "Sinan",
                    "Gök",
                    "sinan.gok@altunpital.com",
                    "05041000002",
                    "ANK-STAFF-002",
                    "Nöroloji",
                    "Sağlık Teknikeri")
            ],
            [
                new("Gizem", "Bozkurt", "gizem.bozkurt@example.com", "05042000001"),
                new("Mert", "Ekin", "mert.ekin@example.com", "05042000002"),
                new("Zehra", "Yüksel", "zehra.yuksel@example.com", "05042000003")
            ]);

        SeedBranch(
            db,
            hospital,
            "AltunPital Bursa",
            "Bursa",
            "Nilüfer",
            "Nilüfer, Bursa",
            "0224 000 00 05",
            ["Ortopedi", "Fizik Tedavi", "Dermatoloji", "Genel Tıp"],
            [
                new("Ortopedi Muayenesi", "Ortopedi muayenesi", 30, 1200),
                new("Fizik Tedavi Değerlendirmesi", "Fizik tedavi değerlendirmesi", 45, 1150),
                new("Dermatoloji Muayenesi", "Dermatoloji muayenesi", 30, 1250),
                new("Genel Muayene", "Genel sağlık muayenesi", 30, 850)
            ],
            [
                new("Fizik Tedavi Ünitesi", "Fizik tedavi ekipmanı", 5),
                new("Dijital Röntgen Cihazı", "Röntgen sistemi", 2),
                new("Hasta Monitörü", "Hasta takip monitörü", 4)
            ],
            [
                new(
                    "Alper",
                    "Köse",
                    "alper.kose@altunpital.com",
                    "05050000001",
                    "BUR-DOC-001",
                    "Ortopedi",
                    "Ortopedi",
                    ["Ortopedi Muayenesi"]),
                new(
                    "İlayda",
                    "Ersoy",
                    "ilayda.ersoy@altunpital.com",
                    "05050000002",
                    "BUR-DOC-002",
                    "Fizik Tedavi",
                    "Fizik Tedavi",
                    ["Fizik Tedavi Değerlendirmesi"]),
                new(
                    "Batuhan",
                    "Oral",
                    "batuhan.oral@altunpital.com",
                    "05050000003",
                    "BUR-DOC-003",
                    "Dermatoloji",
                    "Dermatoloji",
                    ["Dermatoloji Muayenesi"])
            ],
            [
                new(
                    "Cansu",
                    "Yüce",
                    "cansu.yuce@altunpital.com",
                    "05051000001",
                    "BUR-STAFF-001",
                    "Ortopedi",
                    "Hemşire"),
                new(
                    "Recep",
                    "Kara",
                    "recep.kara@altunpital.com",
                    "05051000002",
                    "BUR-STAFF-002",
                    "Fizik Tedavi",
                    "Fizyoterapi Teknikeri")
            ],
            [
                new("Pınar", "Çolakoğlu", "pinar.colakoglu@example.com", "05052000001"),
                new("Erhan", "Mutlu", "erhan.mutlu@example.com", "05052000002"),
                new("Ada", "Keleş", "ada.keles@example.com", "05052000003")
            ]);

        SeedBranch(
            db,
            hospital,
            "AltunPital İzmir",
            "İzmir",
            "Konak",
            "Konak, İzmir",
            "0232 000 00 06",
            ["Kardiyoloji", "Ortopedi", "Dahiliye", "Radyoloji"],
            [
                new("Kardiyoloji Muayenesi", "Kardiyoloji uzman muayenesi", 30, 1350),
                new("Ortopedi Muayenesi", "Ortopedi uzman muayenesi", 30, 1250),
                new("Dahiliye Muayenesi", "Dahiliye uzman muayenesi", 30, 1050),
                new("Check-up Paketi", "Genel check-up değerlendirmesi", 60, 3000)
            ],
            [
                new("EKG Cihazı", "İzmir şubesi EKG cihazı", 3),
                new("Ultrason Cihazı", "İzmir şubesi ultrason cihazı", 2),
                new("Holter Cihazı", "İzmir şubesi Holter cihazı", 4)
            ],
            [
                new(
                    "Emre",
                    "Karahan",
                    "emre.karahan@altunpital.com",
                    "05551234567",
                    "IZM-DOC-001",
                    "Kardiyoloji",
                    "Kardiyoloji",
                    ["Kardiyoloji Muayenesi"]),
                new(
                    "Eylül",
                    "Aksoy",
                    "eylul.aksoy@altunpital.com",
                    "05060000002",
                    "IZM-DOC-002",
                    "Ortopedi",
                    "Ortopedi",
                    ["Ortopedi Muayenesi"]),
                new(
                    "Caner",
                    "Özdemir",
                    "caner.ozdemir@altunpital.com",
                    "05060000003",
                    "IZM-DOC-003",
                    "Dahiliye",
                    "Dahiliye",
                    ["Dahiliye Muayenesi", "Check-up Paketi"])
            ],
            [
                new(
                    "Selma",
                    "Aydın",
                    "selma.aydin@altunpital.com",
                    "05061000001",
                    "IZM-STAFF-001",
                    "Kardiyoloji",
                    "Hemşire"),
                new(
                    "Bora",
                    "Keser",
                    "bora.keser@altunpital.com",
                    "05061000002",
                    "IZM-STAFF-002",
                    "Radyoloji",
                    "Radyoloji Teknisyeni")
            ],
            [
                new("Cem", "Arı", "cem.ari@example.com", "05062000001"),
                new("Dila", "Eren", "dila.eren@example.com", "05062000002"),
                new("Sarp", "Yılmaz", "sarp.yilmaz@example.com", "05062000003")
            ]);

        // Yönetici
        Branch managerBranch = db.Branches
            .OrderBy(b => b.BranchId)
            .First();

        db.Managers.Add(new Manager
        {
            FirstName = "Altun",
            LastName = "Pital",
            Email = "manager@altunpital.com",
            Phone = "05000000000",
            PasswordHash = PasswordHasher.HashPassword("asdf1234!"),
            EmployeeNumber = "MGR-001",
            HireDate = DateTime.Today.AddYears(-3),
            BranchId = managerBranch.BranchId,
            DepartmentId = null
        });

        db.SaveChanges();
    }

    private static void SeedBranch(
        AppDbContext db,
        Hospital hospital,
        string branchName,
        string city,
        string district,
        string address,
        string phone,
        string[] departments,
        ServiceSeed[] services,
        EquipmentSeed[] equipment,
        DoctorSeed[] doctors,
        StaffSeed[] staff,
        PatientSeed[] patients)
    {
        var branch = new Branch
        {
            Name = branchName,
            City = city,
            District = district,
            Address = address,
            Phone = phone,
            OpeningTime = new TimeOnly(8, 0),
            ClosingTime = new TimeOnly(18, 0),
            HospitalId = hospital.HospitalId
        };

        db.Branches.Add(branch);
        db.SaveChanges();

        Dictionary<string, Department> departmentMap = new();

        foreach (string departmentName in departments)
        {
            var department = new Department
            {
                Name = departmentName,
                Description = $"{departmentName} bölümü",
                BranchId = branch.BranchId
            };

            db.Departments.Add(department);
            db.SaveChanges();

            departmentMap[departmentName] = department;
        }

        // Hizmetler — her şube kendi kaydını alır
        Dictionary<string, HealthService> serviceMap = new();
        Dictionary<int, decimal> servicePrices = new();

        foreach (ServiceSeed seed in services)
        {
            var service = new HealthService
            {
                Name = seed.Name,
                Description = seed.Description,
                DurationMinutes = seed.Duration
            };

            db.HealthServices.Add(service);
            db.SaveChanges();

            db.BranchServices.Add(new BranchService
            {
                BranchId = branch.BranchId,
                HealthServiceId = service.HealthServiceId,
                Price = seed.Price,
                IsAvailable = true
            });

            db.SaveChanges();

            serviceMap[seed.Name] = service;
            servicePrices[service.HealthServiceId] = seed.Price;
        }

        // Ekipmanlar — her şubenin kendi kaydı var
        foreach (EquipmentSeed seed in equipment)
        {
            var entity = new Equipment
            {
                Name = seed.Name,
                Description = seed.Description
            };

            db.Equipment.Add(entity);
            db.SaveChanges();

            db.BranchEquipment.Add(new BranchEquipment
            {
                BranchId = branch.BranchId,
                EquipmentId = entity.EquipmentId,
                Quantity = seed.Quantity,
                IsOperational = true
            });

            db.SaveChanges();
        }

        // Doktorlar
        List<Doctor> createdDoctors = new();

        foreach (DoctorSeed seed in doctors)
        {
            var doctor = new Doctor
            {
                FirstName = seed.FirstName,
                LastName = seed.LastName,
                Email = seed.Email,
                Phone = seed.Phone,
                PasswordHash = PasswordHasher.HashPassword("doktor123"),
                EmployeeNumber = seed.EmployeeNumber,
                HireDate = DateTime.Today.AddYears(-2),
                BranchId = branch.BranchId,
                DepartmentId = departmentMap[seed.Department].DepartmentId,
                Specialization = seed.Specialization
            };

            db.Doctors.Add(doctor);
            db.SaveChanges();

            createdDoctors.Add(doctor);

            foreach (string serviceName in seed.Services)
            {
                if (!serviceMap.TryGetValue(serviceName, out HealthService? service))
                {
                    continue;
                }

                db.DoctorServices.Add(new DoctorService
                {
                    DoctorId = doctor.UserId,
                    HealthServiceId = service.HealthServiceId
                });
            }

            for (int day = 1; day <= 5; day++)
            {
                db.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = doctor.UserId,
                    DayOfWeek = (DayOfWeek)day,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0)
                });
            }

            db.SaveChanges();
        }

        // Personel
        foreach (StaffSeed seed in staff)
        {
            db.HealthStaff.Add(new HealthStaff
            {
                FirstName = seed.FirstName,
                LastName = seed.LastName,
                Email = seed.Email,
                Phone = seed.Phone,
                PasswordHash = PasswordHasher.HashPassword("personel123"),
                EmployeeNumber = seed.EmployeeNumber,
                HireDate = DateTime.Today.AddYears(-1),
                BranchId = branch.BranchId,
                DepartmentId = departmentMap[seed.Department].DepartmentId,
                Position = seed.Position
            });
        }

        db.SaveChanges();

        // Hastalar
        List<Patient> createdPatients = new();

        foreach (PatientSeed seed in patients)
        {
            var patient = new Patient
            {
                FirstName = seed.FirstName,
                LastName = seed.LastName,
                Email = seed.Email,
                Phone = seed.Phone,
                City = city,
                PasswordHash = PasswordHasher.HashPassword("hasta123")
            };

            db.Patients.Add(patient);
            createdPatients.Add(patient);
        }

        db.SaveChanges();

        // Örnek randevular
        DateTime firstDate = NextWeekday(DateTime.Today.AddDays(1), DayOfWeek.Monday).Date.AddHours(10);
        int appointmentCount = Math.Min(2, Math.Min(createdDoctors.Count, createdPatients.Count));

        for (int i = 0; i < appointmentCount; i++)
        {
            Doctor doctor = createdDoctors[i];
            var doctorService = db.DoctorServices.FirstOrDefault(ds => ds.DoctorId == doctor.UserId);

            if (doctorService == null)
            {
                continue;
            }

            HealthService service = db.HealthServices.First(s => s.HealthServiceId == doctorService.HealthServiceId);
            DateTime start = firstDate.AddDays(i).AddHours(i);
            DateTime end = start.AddMinutes(service.DurationMinutes);

            db.Appointments.Add(new Appointment
            {
                PatientId = createdPatients[i].UserId,
                DoctorId = doctor.UserId,
                BranchId = branch.BranchId,
                HealthServiceId = service.HealthServiceId,
                StartDateTime = start,
                EndDateTime = end,
                Price = servicePrices[service.HealthServiceId],
                Status = i == 0 ? AppointmentStatus.Pending : AppointmentStatus.Approved,
                CreatedAt = DateTime.Now
            });
        }

        db.SaveChanges();
    }

    private static DateTime NextWeekday(DateTime date, DayOfWeek day)
    {
        while (date.DayOfWeek != day)
        {
            date = date.AddDays(1);
        }

        return date;
    }

    private sealed record ServiceSeed(string Name, string Description, int Duration, decimal Price);

    private sealed record EquipmentSeed(string Name, string Description, int Quantity);

    private sealed record DoctorSeed(
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string EmployeeNumber,
        string Department,
        string Specialization,
        string[] Services);

    private sealed record StaffSeed(
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string EmployeeNumber,
        string Department,
        string Position);

    private sealed record PatientSeed(string FirstName, string LastName, string Email, string Phone);
}
