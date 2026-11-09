using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;

namespace AltunPital.UI;

public class ConsoleMenu
{
    private readonly IAuthenticationService _authentication;
    private readonly IPatientService _patientService;
    private readonly IDoctorService _doctorService;
    private readonly IManagerService _managerService;
    private readonly IEmployeeService _employeeService;


    public ConsoleMenu(
        IAuthenticationService authentication,
        IPatientService patientService,
        IDoctorService doctorService,
        IManagerService managerService,
        IEmployeeService employeeService)
    {
        _authentication = authentication;
        _patientService = patientService;
        _doctorService = doctorService;
        _managerService = managerService;
        _employeeService = employeeService;
    }


    public void Start()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=== ALTUNPITAL ===");
            Console.WriteLine();
            Console.WriteLine("1 - Giriş");
            Console.WriteLine("2 - Hasta Kayıt");
            Console.WriteLine("0 - Çıkış");

            Console.Write("\nSeçim: ");

            string? choice =
                Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Login();
                        break;

                    case "2":
                        Register();
                        break;

                    case "0":
                        return;

                    default:
                        Message(
                            "Geçersiz seçim.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Message(
                    $"Hata: {ex.Message}");
            }
        }
    }


    // =================================================
    // AUTH
    // =================================================

    private void Login()
    {
        Console.Clear();

        Console.WriteLine(
            "=== GİRİŞ ===");

        Console.Write("E-posta: ");

        string email =
            Console.ReadLine() ?? "";

        Console.Write("Şifre: ");

        string password =
            Console.ReadLine() ?? "";

        User user =
            _authentication.Login(
                email,
                password);

        string role =
            _authentication.GetRole(user);

        switch (role)
        {
            case "Patient":
                PatientMenu(user);
                break;

            case "Doctor":
                DoctorMenu(user);
                break;

            case "Manager":
                ManagerMenu(user);
                break;

            case "HealthStaff":
                HealthStaffMenu(user);
                break;

            default:
                throw new Exception(
                    "Kullanıcı rolü tanımlanamadı.");
        }
    }


    private void Register()
    {
        Console.Clear();

        Console.WriteLine(
            "=== HASTA KAYIT ===");

        Console.Write("Ad: ");

        string firstName =
            Console.ReadLine() ?? "";

        Console.Write("Soyad: ");

        string lastName =
            Console.ReadLine() ?? "";

        Console.Write("E-posta: ");

        string email =
            Console.ReadLine() ?? "";

        Console.Write("Telefon: ");

        string phone =
            Console.ReadLine() ?? "";

        Console.Write("Şifre: ");

        string password =
            Console.ReadLine() ?? "";

        var patient =
            _authentication.RegisterPatient(
                firstName,
                lastName,
                email,
                phone,
                password);

        Message(
            $"Kayıt başarılı. Hasta ID: {patient.UserId}");
    }


    // =================================================
    // PATIENT
    // =================================================

    private void PatientMenu(User user)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine(
                $"=== HASTA: " +
                $"{user.FirstName} " +
                $"{user.LastName} ===");

            Console.WriteLine(
                "1 - Şubeleri Gör");

            Console.WriteLine(
                "2 - Randevu Al");

            Console.WriteLine(
                "3 - Randevularım");

            Console.WriteLine(
                "4 - Randevu İptal");

            Console.WriteLine(
                "0 - Çıkış Yap");

            Console.Write("\nSeçim: ");

            string? choice =
                Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":

                        ShowBranches();
                        break;

                    case "2":

                        CreatePatientAppointment(
                            user.UserId);

                        break;

                    case "3":

                        ShowPatientAppointments(
                            user.UserId);

                        break;

                    case "4":

                        CancelPatientAppointment(
                            user.UserId);

                        break;

                    case "0":

                        return;

                    default:

                        Message(
                            "Geçersiz seçim.");

                        break;
                }
            }
            catch (Exception ex)
            {
                Message(
                    $"Hata: {ex.Message}");
            }
        }
    }


    private void ShowBranches()
    {
        Console.Clear();

        Console.WriteLine(
            "=== ŞUBELER ===");

        foreach (var branch
                 in _patientService.GetBranches())
        {
            Console.WriteLine(
                $"{branch.BranchId} | " +
                $"{branch.Name} | " +
                $"{branch.City}");
        }

        Pause();
    }


    private void CreatePatientAppointment(
        int patientId)
    {
        Console.Clear();

        Console.WriteLine(
            "=== RANDEVU AL ===");


        var branches =
            _patientService.GetBranches();

        foreach (var branch in branches)
        {
            Console.WriteLine(
                $"{branch.BranchId} - " +
                $"{branch.Name}");
        }

        int branchId =
            ReadInt("Şube ID: ");


        var departments =
            _patientService
                .GetDepartments(branchId);

        Console.WriteLine(
            "\nBölümler:");

        foreach (var department
                 in departments)
        {
            Console.WriteLine(
                $"{department.DepartmentId} - " +
                $"{department.Name}");
        }

        int departmentId =
            ReadInt("Bölüm ID: ");


        var doctors =
            _patientService.GetDoctors(
                branchId,
                departmentId);

        if (doctors.Count == 0)
        {
            throw new Exception(
                "Bu bölümde doktor bulunamadı.");
        }

        Console.WriteLine(
            "\nDoktorlar:");

        foreach (var doctor
                 in doctors)
        {
            Console.WriteLine(
                $"{doctor.UserId} - " +
                $"{doctor.FirstName} " +
                $"{doctor.LastName} | " +
                $"{doctor.Specialization}");
        }

        int doctorId =
            ReadInt("Doktor ID: ");


        var services =
            _patientService.GetServices(
                branchId,
                doctorId);

        if (services.Count == 0)
        {
            throw new Exception(
                "Doktor için uygun hizmet bulunamadı.");
        }

        Console.WriteLine(
            "\nHizmetler:");

        foreach (var service
                 in services)
        {
            Console.WriteLine(
                $"{service.HealthServiceId} - " +
                $"{service.Name} | " +
                $"{service.DurationMinutes} dk");
        }

        int serviceId =
            ReadInt("Hizmet ID: ");


        Console.Write(
            "Tarih (yyyy-MM-dd): ");

        if (!DateOnly.TryParse(
                Console.ReadLine(),
                out DateOnly date))
        {
            throw new Exception(
                "Geçersiz tarih.");
        }


        var slots =
            _patientService
                .GetAvailableSlots(
                    doctorId,
                    serviceId,
                    date);

        if (slots.Count == 0)
        {
            throw new Exception(
                "Bu tarihte uygun saat bulunamadı.");
        }

        Console.WriteLine(
            "\nUygun Saatler:");

        for (int i = 0;
             i < slots.Count;
             i++)
        {
            Console.WriteLine(
                $"{i + 1} - " +
                $"{slots[i]:HH:mm}");
        }

        int slotChoice =
            ReadInt("Saat seçimi: ");

        if (slotChoice < 1 ||
            slotChoice > slots.Count)
        {
            throw new Exception(
                "Geçersiz saat seçimi.");
        }

        DateTime selected =
            slots[slotChoice - 1];


        var appointment =
            _patientService
                .CreateAppointment(
                    patientId,
                    doctorId,
                    branchId,
                    serviceId,
                    selected);

        Message(
            "Randevu oluşturuldu.\n" +
            $"ID: {appointment.AppointmentId}\n" +
            $"Durum: {appointment.Status}\n" +
            $"Fiyat: {appointment.Price} TL");
    }


    private void ShowPatientAppointments(
        int patientId)
    {
        Console.Clear();

        Console.WriteLine(
            "=== RANDEVULARIM ===");

        var appointments =
            _patientService
                .GetAppointments(
                    patientId);

        if (appointments.Count == 0)
        {
            Console.WriteLine(
                "Randevu bulunamadı.");

            Pause();

            return;
        }

        foreach (var a
                 in appointments)
        {
            Console.WriteLine(
                $"{a.AppointmentId} | " +
                $"{a.StartDateTime:g} | " +
                $"{a.Doctor.FirstName} " +
                $"{a.Doctor.LastName} | " +
                $"{a.HealthService.Name} | " +
                $"{a.Status} | " +
                $"{a.Price} TL");
        }

        Pause();
    }


    private void CancelPatientAppointment(
        int patientId)
    {
        Console.Clear();

        var appointments =
            _patientService
                .GetAppointments(
                    patientId);

        foreach (var a
                 in appointments)
        {
            Console.WriteLine(
                $"{a.AppointmentId} | " +
                $"{a.StartDateTime:g} | " +
                $"{a.Status}");
        }

        int id =
            ReadInt(
                "İptal edilecek ID: ");

        if (!appointments.Any(a =>
                a.AppointmentId == id))
        {
            throw new Exception(
                "Bu randevu size ait değil.");
        }

        _patientService
            .CancelAppointment(id);

        Message(
            "Randevu iptal edildi.");
    }


    // =================================================
    // DOCTOR
    // =================================================

    private void DoctorMenu(User user)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine(
                $"=== DOKTOR: " +
                $"{user.FirstName} " +
                $"{user.LastName} ===");

            Console.WriteLine(
                "1 - Randevularım");

            Console.WriteLine(
                "2 - Randevu Onayla");

            Console.WriteLine(
                "3 - Randevu Reddet");

            Console.WriteLine(
                "4 - Randevuyu Tamamla");

            Console.WriteLine(
                "5 - Çalışma Programım");

            Console.WriteLine(
                "0 - Çıkış Yap");

            Console.Write("\nSeçim: ");

            string? choice =
                Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":

                        ShowDoctorAppointments(
                            user.UserId);

                        break;

                    case "2":

                        ChangeDoctorAppointment(
                            user.UserId,
                            "approve");

                        break;

                    case "3":

                        ChangeDoctorAppointment(
                            user.UserId,
                            "reject");

                        break;

                    case "4":

                        ChangeDoctorAppointment(
                            user.UserId,
                            "complete");

                        break;

                    case "5":

                        ShowDoctorSchedule(
                            user.UserId);

                        break;

                    case "0":

                        return;

                    default:

                        Message(
                            "Geçersiz seçim.");

                        break;
                }
            }
            catch (Exception ex)
            {
                Message(
                    $"Hata: {ex.Message}");
            }
        }
    }


    private void ShowDoctorAppointments(
        int doctorId)
    {
        Console.Clear();

        Console.WriteLine(
            "=== RANDEVULAR ===");

        var appointments =
            _doctorService.GetAppointments(
                doctorId);

        if (appointments.Count == 0)
        {
            Console.WriteLine(
                "Randevu bulunamadı.");

            Pause();

            return;
        }

        foreach (var a
                 in appointments)
        {
            Console.WriteLine(
                $"{a.AppointmentId} | " +
                $"{a.StartDateTime:g} | " +
                $"{a.Patient.FirstName} " +
                $"{a.Patient.LastName} | " +
                $"{a.HealthService.Name} | " +
                $"{a.Status}");
        }

        Pause();
    }


    private void ChangeDoctorAppointment(
        int doctorId,
        string action)
    {
        Console.Clear();

        var appointments =
            _doctorService.GetAppointments(
                doctorId);

        foreach (var a
                 in appointments)
        {
            Console.WriteLine(
                $"{a.AppointmentId} | " +
                $"{a.StartDateTime:g} | " +
                $"{a.Patient.FirstName} " +
                $"{a.Patient.LastName} | " +
                $"{a.Status}");
        }

        int id =
            ReadInt("Randevu ID: ");

        switch (action)
        {
            case "approve":

                _doctorService
                    .ApproveAppointment(
                        doctorId,
                        id);

                Message(
                    "Randevu onaylandı.");

                break;


            case "reject":

                _doctorService
                    .RejectAppointment(
                        doctorId,
                        id);

                Message(
                    "Randevu reddedildi.");

                break;


            case "complete":

                _doctorService
                    .CompleteAppointment(
                        doctorId,
                        id);

                Message(
                    "Randevu tamamlandı.");

                break;
        }
    }


    private void ShowDoctorSchedule(
        int doctorId)
    {
        Console.Clear();

        Console.WriteLine(
            "=== ÇALIŞMA PROGRAMI ===");

        var schedules =
            _doctorService.GetSchedules(
                doctorId);

        foreach (var schedule
                 in schedules)
        {
            Console.WriteLine(
                $"{schedule.DayOfWeek} | " +
                $"{schedule.StartTime} - " +
                $"{schedule.EndTime}");
        }

        Pause();
    }


    // =================================================
    // MANAGER
    // =================================================

    private void ManagerMenu(User user)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine(
                $"=== YÖNETİCİ: " +
                $"{user.FirstName} " +
                $"{user.LastName} ===");

            Console.WriteLine();

            Console.WriteLine(
                "1 - Şubeleri Listele");

            Console.WriteLine(
                "2 - Şube Ekle");

            Console.WriteLine(
                "3 - Bölüm Ekle");

            Console.WriteLine(
                "4 - Hizmet Ekle");

            Console.WriteLine(
                "5 - Hizmeti Şubeye Ata");

            Console.WriteLine(
                "6 - Doktor Ekle");

            Console.WriteLine(
                "7 - Doktora Hizmet Ata");

            Console.WriteLine(
                "8 - Doktor Çalışma Saati Ata");

            Console.WriteLine(
                "9 - Ekipman Ekle");

            Console.WriteLine(
                "10 - Ekipmanı Şubeye Ata");

            Console.WriteLine(
                "11 - Çalışanları Listele");

            Console.WriteLine(
                "0 - Çıkış Yap");

            Console.Write("\nSeçim: ");

            string? choice =
                Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":

                        ShowManagerBranches();

                        break;

                    case "2":

                        ManagerAddBranch();

                        break;

                    case "3":

                        ManagerAddDepartment();

                        break;

                    case "4":

                        ManagerAddService();

                        break;

                    case "5":

                        ManagerAssignService();

                        break;

                    case "6":

                        ManagerAddDoctor();

                        break;

                    case "7":

                        ManagerAssignDoctorService();

                        break;

                    case "8":

                        ManagerAddDoctorSchedule();

                        break;

                    case "9":

                        ManagerAddEquipment();

                        break;

                    case "10":

                        ManagerAssignEquipment();

                        break;

                    case "11":

                        ManagerShowEmployees();

                        break;

                    case "0":

                        return;

                    default:

                        Message(
                            "Geçersiz seçim.");

                        break;
                }
            }
            catch (Exception ex)
            {
                Message(
                    $"Hata: {ex.Message}");
            }
        }
    }


    private void ShowManagerBranches()
    {
        Console.Clear();

        Console.WriteLine(
            "=== ŞUBELER ===");

        foreach (var branch
                 in _managerService
                     .GetBranches())
        {
            Console.WriteLine(
                $"{branch.BranchId} | " +
                $"{branch.Name} | " +
                $"{branch.City} / " +
                $"{branch.District}");
        }

        Pause();
    }


    private void ManagerAddBranch()
    {
        Console.Clear();

        int hospitalId =
            ReadInt("Hospital ID: ");

        Console.Write(
            "Şube adı: ");

        string name =
            Console.ReadLine() ?? "";

        Console.Write(
            "İl: ");

        string city =
            Console.ReadLine() ?? "";

        Console.Write(
            "İlçe: ");

        string district =
            Console.ReadLine() ?? "";

        Console.Write(
            "Adres: ");

        string address =
            Console.ReadLine() ?? "";

        Console.Write(
            "Telefon: ");

        string phone =
            Console.ReadLine() ?? "";

        TimeOnly opening =
            ReadTime(
                "Açılış (HH:mm): ");

        TimeOnly closing =
            ReadTime(
                "Kapanış (HH:mm): ");

        _managerService.AddBranch(
            hospitalId,
            name,
            city,
            district,
            address,
            phone,
            opening,
            closing);

        Message(
            "Şube eklendi.");
    }


    private void ManagerAddDepartment()
    {
        Console.Clear();

        ShowBranchesSimple();

        int branchId =
            ReadInt("Şube ID: ");

        Console.Write(
            "Bölüm adı: ");

        string name =
            Console.ReadLine() ?? "";

        Console.Write(
            "Açıklama: ");

        string description =
            Console.ReadLine() ?? "";

        _managerService.AddDepartment(
            branchId,
            name,
            description);

        Message(
            "Bölüm eklendi.");
    }


    private void ManagerAddService()
    {
        Console.Clear();

        Console.Write(
            "Hizmet adı: ");

        string name =
            Console.ReadLine() ?? "";

        Console.Write(
            "Açıklama: ");

        string description =
            Console.ReadLine() ?? "";

        int duration =
            ReadInt(
                "Süre (dakika): ");

        _managerService.AddHealthService(
            name,
            description,
            duration);

        Message(
            "Hizmet eklendi.");
    }


    private void ManagerAssignService()
    {
        Console.Clear();

        ShowBranchesSimple();

        int branchId =
            ReadInt("Şube ID: ");

        Console.WriteLine();
        Console.WriteLine(
            "Hizmetler:");

        foreach (var service
                 in _managerService
                     .GetHealthServices())
        {
            Console.WriteLine(
                $"{service.HealthServiceId} | " +
                $"{service.Name}");
        }

        int serviceId =
            ReadInt("Hizmet ID: ");

        decimal price =
            ReadDecimal("Fiyat: ");

        _managerService
            .AssignServiceToBranch(
                branchId,
                serviceId,
                price);

        Message(
            "Hizmet şubeye atandı.");
    }


    private void ManagerAddDoctor()
    {
        Console.Clear();

        Console.Write(
            "Ad: ");

        string firstName =
            Console.ReadLine() ?? "";

        Console.Write(
            "Soyad: ");

        string lastName =
            Console.ReadLine() ?? "";

        Console.Write(
            "E-posta: ");

        string email =
            Console.ReadLine() ?? "";

        Console.Write(
            "Telefon: ");

        string phone =
            Console.ReadLine() ?? "";

        Console.Write(
            "Çalışan no: ");

        string employeeNumber =
            Console.ReadLine() ?? "";


        ShowBranchesSimple();


        int branchId =
            ReadInt("Şube ID: ");


        Console.WriteLine();
        Console.WriteLine(
            "Bölümler:");

        foreach (var department
                 in _managerService
                     .GetDepartments(
                         branchId))
        {
            Console.WriteLine(
                $"{department.DepartmentId} | " +
                $"{department.Name}");
        }


        int departmentId =
            ReadInt("Bölüm ID: ");


        Console.Write(
            "Uzmanlık: ");

        string specialization =
            Console.ReadLine() ?? "";


        var doctor =
            _employeeService
                .AddDoctor(
                    firstName,
                    lastName,
                    email,
                    phone,
                    employeeNumber,
                    DateTime.Today,
                    branchId,
                    departmentId,
                    specialization);


        Message(
            $"Doktor eklendi. ID: {doctor.UserId}");
    }


    private void ManagerAssignDoctorService()
    {
        Console.Clear();

        var doctors =
            _employeeService
                .GetEmployees()
                .OfType<Doctor>()
                .ToList();

        Console.WriteLine(
            "Doktorlar:");

        foreach (var doctor
                 in doctors)
        {
            Console.WriteLine(
                $"{doctor.UserId} | " +
                $"{doctor.FirstName} " +
                $"{doctor.LastName}");
        }


        int doctorId =
            ReadInt("Doktor ID: ");


        Console.WriteLine();
        Console.WriteLine(
            "Hizmetler:");

        foreach (var service
                 in _managerService
                     .GetHealthServices())
        {
            Console.WriteLine(
                $"{service.HealthServiceId} | " +
                $"{service.Name}");
        }


        int serviceId =
            ReadInt("Hizmet ID: ");


        _employeeService.AssignService(
            doctorId,
            serviceId);


        Message(
            "Hizmet doktora atandı.");
    }


    private void ManagerAddDoctorSchedule()
    {
        Console.Clear();

        int doctorId =
            ReadInt("Doktor ID: ");

        Console.WriteLine(
            "0=Pazar 1=Pazartesi 2=Salı " +
            "3=Çarşamba 4=Perşembe " +
            "5=Cuma 6=Cumartesi");

        int day =
            ReadInt("Gün: ");

        if (day < 0 ||
            day > 6)
        {
            throw new Exception(
                "Geçersiz gün.");
        }


        TimeOnly start =
            ReadTime(
                "Başlangıç: ");

        TimeOnly end =
            ReadTime(
                "Bitiş: ");


        _employeeService.AddSchedule(
            doctorId,
            (DayOfWeek)day,
            start,
            end);


        Message(
            "Çalışma saati eklendi.");
    }


    private void ManagerAddEquipment()
    {
        Console.Clear();

        Console.Write(
            "Ekipman adı: ");

        string name =
            Console.ReadLine() ?? "";

        Console.Write(
            "Açıklama: ");

        string description =
            Console.ReadLine() ?? "";


        var equipment =
            _managerService.AddEquipment(
                name,
                description);


        Message(
            $"Ekipman eklendi. ID: " +
            $"{equipment.EquipmentId}");
    }


    private void ManagerAssignEquipment()
    {
        Console.Clear();

        ShowBranchesSimple();

        int branchId =
            ReadInt("Şube ID: ");

        int equipmentId =
            ReadInt("Ekipman ID: ");

        int quantity =
            ReadInt("Adet: ");


        _managerService
            .AssignEquipmentToBranch(
                branchId,
                equipmentId,
                quantity);


        Message(
            "Ekipman şubeye atandı.");
    }


    private void ManagerShowEmployees()
    {
        Console.Clear();

        Console.WriteLine(
            "=== ÇALIŞANLAR ===");

        Console.Write(
            "Şube ID (tümü için 0): ");

        int branchId =
            int.TryParse(
                Console.ReadLine(),
                out int value)
                    ? value
                    : 0;


        var employees =
            _employeeService
                .GetEmployees(
                    branchId == 0
                        ? null
                        : branchId);


        foreach (var employee
                 in employees)
        {
            Console.WriteLine(
                $"{employee.UserId} | " +
                $"{employee.FirstName} " +
                $"{employee.LastName} | " +
                $"{employee.EmployeeNumber} | " +
                $"{employee.Branch.Name}");
        }


        Pause();
    }


    private void ShowBranchesSimple()
    {
        Console.WriteLine();
        Console.WriteLine(
            "Şubeler:");

        foreach (var branch
                 in _managerService
                     .GetBranches())
        {
            Console.WriteLine(
                $"{branch.BranchId} | " +
                $"{branch.Name}");
        }
    }


    // =================================================
    // HEALTH STAFF
    // =================================================

    private void HealthStaffMenu(
        User user)
    {
        Console.Clear();

        Console.WriteLine(
            $"Hoş geldiniz " +
            $"{user.FirstName} " +
            $"{user.LastName}.");

        Console.WriteLine();

        Console.WriteLine(
            "Sağlık personeli özel işlemleri " +
            "henüz tanımlanmadı.");

        Pause();
    }


    // =================================================
    // HELPERS
    // =================================================

    private static int ReadInt(
        string message)
    {
        Console.Write(message);

        if (!int.TryParse(
                Console.ReadLine(),
                out int value))
        {
            throw new Exception(
                "Geçerli sayı giriniz.");
        }

        return value;
    }


    private static decimal ReadDecimal(
        string message)
    {
        Console.Write(message);

        if (!decimal.TryParse(
                Console.ReadLine(),
                out decimal value))
        {
            throw new Exception(
                "Geçerli sayı giriniz.");
        }

        return value;
    }


    private static TimeOnly ReadTime(
        string message)
    {
        Console.Write(message);

        if (!TimeOnly.TryParse(
                Console.ReadLine(),
                out TimeOnly value))
        {
            throw new Exception(
                "Geçerli saat giriniz.");
        }

        return value;
    }


    private static void Message(
        string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);

        Pause();
    }


    private static void Pause()
    {
        Console.WriteLine();

        Console.WriteLine(
            "Devam etmek için Enter...");

        Console.ReadLine();
    }
}