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


    // =====================================================
    // ANA MENÜ
    // =====================================================

    public void Start()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            Console.Clear();

            AnaBaslik();

            MenuSecenegi(1, "Giriş Yap");
            MenuSecenegi(2, "Hasta Kaydı Oluştur");
            MenuCikis("Programdan Çık");

            string? secim =
                SecimOku();

            try
            {
                switch (secim)
                {
                    case "1":
                        GirisYap();
                        break;

                    case "2":
                        HastaKaydiOlustur();
                        break;

                    case "0":
                        return;

                    default:
                        UyariMesaji(
                            "Geçersiz seçim yaptınız.");
                        break;
                }
            }
            catch (Exception ex)
            {
                HataMesaji(
                    ex.Message);
            }
        }
    }


    // =====================================================
    // GİRİŞ / KAYIT
    // =====================================================

    private void GirisYap()
    {
        Console.Clear();

        SayfaBasligi(
            "SİSTEME GİRİŞ");

        Console.Write(
            "E-posta : ");

        string email =
            Console.ReadLine() ?? "";

        Console.Write(
            "Şifre   : ");

        string password =
            Console.ReadLine() ?? "";

        User user =
            _authentication.Login(
                email,
                password);

        string role =
            _authentication.GetRole(user);

        BasariMesaji(
            $"Hoş geldiniz, {user.FirstName} {user.LastName}.");

        switch (role)
        {
            case "Patient":
                HastaMenusu(user);
                break;

            case "Doctor":
                DoktorMenusu(user);
                break;

            case "Manager":
                YoneticiMenusu(user);
                break;

            case "HealthStaff":
                SaglikPersoneliMenusu(user);
                break;

            default:
                throw new Exception(
                    "Kullanıcı rolü tanımlanamadı.");
        }
    }


    private void HastaKaydiOlustur()
    {
        Console.Clear();

        SayfaBasligi(
            "YENİ HASTA KAYDI");

        Console.Write(
            "Ad       : ");

        string firstName =
            Console.ReadLine() ?? "";

        Console.Write(
            "Soyad    : ");

        string lastName =
            Console.ReadLine() ?? "";

        Console.Write(
            "E-posta  : ");

        string email =
            Console.ReadLine() ?? "";

        Console.Write(
            "Telefon  : ");

        string phone =
            Console.ReadLine() ?? "";

        Console.Write(
            "Şifre    : ");

        string password =
            Console.ReadLine() ?? "";

        var patient =
            _authentication.RegisterPatient(
                firstName,
                lastName,
                email,
                phone,
                password);

        BasariMesaji(
            $"Hasta kaydı oluşturuldu. Hasta ID: {patient.UserId}");
    }


    // =====================================================
    // HASTA
    // =====================================================

    private void HastaMenusu(
        User user)
    {
        while (true)
        {
            Console.Clear();

            RolBasligi(
                "HASTA",
                user);

            MenuSecenegi(
                1,
                "Şubeleri Görüntüle");

            MenuSecenegi(
                2,
                "Yeni Randevu Al");

            MenuSecenegi(
                3,
                "Randevularımı Görüntüle");

            MenuSecenegi(
                4,
                "Randevu İptal Et");

            MenuCikis(
                "Çıkış Yap");

            string? secim =
                SecimOku();

            try
            {
                switch (secim)
                {
                    case "1":
                        SubeleriGoster();
                        break;

                    case "2":
                        HastaRandevuOlustur(
                            user.UserId);
                        break;

                    case "3":
                        HastaRandevulariniGoster(
                            user.UserId);
                        break;

                    case "4":
                        HastaRandevuIptal(
                            user.UserId);
                        break;

                    case "0":
                        return;

                    default:
                        UyariMesaji(
                            "Geçersiz seçim yaptınız.");
                        break;
                }
            }
            catch (Exception ex)
            {
                HataMesaji(
                    ex.Message);
            }
        }
    }


    private void SubeleriGoster()
    {
        Console.Clear();

        SayfaBasligi(
            "HASTANE ŞUBELERİ");

        var branches =
            _patientService
                .GetBranches();

        if (branches.Count == 0)
        {
            BilgiMesaji(
                "Kayıtlı şube bulunamadı.");

            return;
        }

        Console.WriteLine(
            $"{"ID",-5} " +
            $"{"Şube",-28} " +
            $"{"Şehir",-15}");

        Ayirici();

        foreach (var branch
                 in branches)
        {
            Console.WriteLine(
                $"{branch.BranchId,-5} " +
                $"{branch.Name,-28} " +
                $"{branch.City,-15}");
        }

        Bekle();
    }


    private void HastaRandevuOlustur(
        int patientId)
    {
        Console.Clear();

        SayfaBasligi(
            "YENİ RANDEVU");

        AltBaslik(
            "1. Şube Seçimi");

        var branches =
            _patientService
                .GetBranches();

        foreach (var branch
                 in branches)
        {
            Console.WriteLine(
                $"{branch.BranchId} - {branch.Name}");
        }

        int branchId =
            SayiOku(
                "\nŞube ID: ");


        AltBaslik(
            "2. Bölüm Seçimi");

        var departments =
            _patientService
                .GetDepartments(
                    branchId);

        if (departments.Count == 0)
        {
            throw new Exception(
                "Bu şubede kayıtlı bölüm bulunamadı.");
        }

        foreach (var department
                 in departments)
        {
            Console.WriteLine(
                $"{department.DepartmentId} - " +
                $"{department.Name}");
        }

        int departmentId =
            SayiOku(
                "\nBölüm ID: ");


        AltBaslik(
            "3. Doktor Seçimi");

        var doctors =
            _patientService
                .GetDoctors(
                    branchId,
                    departmentId);

        if (doctors.Count == 0)
        {
            throw new Exception(
                "Bu bölümde kayıtlı doktor bulunamadı.");
        }

        foreach (var doctor
                 in doctors)
        {
            Console.WriteLine(
                $"{doctor.UserId} - " +
                $"{doctor.FirstName} " +
                $"{doctor.LastName} " +
                $"({doctor.Specialization})");
        }

        int doctorId =
            SayiOku(
                "\nDoktor ID: ");


        AltBaslik(
            "4. Hizmet Seçimi");

        var services =
            _patientService
                .GetServices(
                    branchId,
                    doctorId);

        if (services.Count == 0)
        {
            throw new Exception(
                "Doktor için tanımlı hizmet bulunamadı.");
        }

        foreach (var service
                 in services)
        {
            Console.WriteLine(
                $"{service.HealthServiceId} - " +
                $"{service.Name} " +
                $"({service.DurationMinutes} dakika)");
        }

        int serviceId =
            SayiOku(
                "\nHizmet ID: ");


        AltBaslik(
            "5. Tarih Seçimi");

        Console.Write(
            "Tarih (yyyy-MM-dd): ");

        if (!DateOnly.TryParse(
                Console.ReadLine(),
                out DateOnly date))
        {
            throw new Exception(
                "Geçersiz tarih.");
        }


        AltBaslik(
            "6. Saat Seçimi");

        var slots =
            _patientService
                .GetAvailableSlots(
                    doctorId,
                    serviceId,
                    date);

        if (slots.Count == 0)
        {
            throw new Exception(
                "Seçilen tarihte uygun randevu saati bulunamadı.");
        }

        for (int i = 0;
             i < slots.Count;
             i++)
        {
            Console.WriteLine(
                $"{i + 1} - {slots[i]:HH:mm}");
        }

        int slotChoice =
            SayiOku(
                "\nSaat seçimi: ");

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

        Console.WriteLine();

        BasariMesaji(
            "Randevu başarıyla oluşturuldu.");

        Console.WriteLine(
            $"Randevu ID : {appointment.AppointmentId}");

        Console.WriteLine(
            $"Tarih      : {appointment.StartDateTime:g}");

        Console.WriteLine(
            $"Durum      : {RandevuDurumuTurkce(appointment.Status)}");

        Console.WriteLine(
            $"Fiyat      : {appointment.Price:N2} TL");

        Bekle();
    }


    private void HastaRandevulariniGoster(
        int patientId)
    {
        Console.Clear();

        SayfaBasligi(
            "RANDEVULARIM");

        var appointments =
            _patientService
                .GetAppointments(
                    patientId);

        if (appointments.Count == 0)
        {
            BilgiMesaji(
                "Henüz randevunuz bulunmamaktadır.");

            return;
        }

        foreach (var appointment
                 in appointments)
        {
            Console.WriteLine(
                $"Randevu No : {appointment.AppointmentId}");

            Console.WriteLine(
                $"Tarih      : {appointment.StartDateTime:g}");

            Console.WriteLine(
                $"Doktor     : " +
                $"{appointment.Doctor.FirstName} " +
                $"{appointment.Doctor.LastName}");

            Console.WriteLine(
                $"Hizmet     : {appointment.HealthService.Name}");

            Console.WriteLine(
                $"Durum      : " +
                $"{RandevuDurumuTurkce(appointment.Status)}");

            Console.WriteLine(
                $"Fiyat      : {appointment.Price:N2} TL");

            Ayirici();
        }

        Bekle();
    }


    private void HastaRandevuIptal(
        int patientId)
    {
        Console.Clear();

        SayfaBasligi(
            "RANDEVU İPTAL");

        var appointments =
            _patientService
                .GetAppointments(
                    patientId);

        if (appointments.Count == 0)
        {
            BilgiMesaji(
                "Randevunuz bulunmamaktadır.");

            return;
        }

        Console.WriteLine(
            $"{"ID",-6} " +
            $"{"Tarih",-20} " +
            $"{"Durum",-15}");

        Ayirici();

        foreach (var appointment
                 in appointments)
        {
            Console.WriteLine(
                $"{appointment.AppointmentId,-6} " +
                $"{appointment.StartDateTime,-20:g} " +
                $"{RandevuDurumuTurkce(appointment.Status),-15}");
        }

        int id =
            SayiOku(
                "\nİptal edilecek randevu ID: ");

        if (!appointments.Any(a =>
                a.AppointmentId == id))
        {
            throw new Exception(
                "Seçilen randevu size ait değildir.");
        }

        _patientService
            .CancelAppointment(id);

        BasariMesaji(
            "Randevu iptal edildi.");
    }


    // =====================================================
    // DOKTOR
    // =====================================================

    private void DoktorMenusu(
        User user)
    {
        while (true)
        {
            Console.Clear();

            RolBasligi(
                "DOKTOR",
                user);

            MenuSecenegi(
                1,
                "Randevularımı Görüntüle");

            MenuSecenegi(
                2,
                "Randevu Onayla");

            MenuSecenegi(
                3,
                "Randevu Reddet");

            MenuSecenegi(
                4,
                "Randevuyu Tamamla");

            MenuSecenegi(
                5,
                "Çalışma Programımı Görüntüle");

            MenuCikis(
                "Çıkış Yap");

            string? secim =
                SecimOku();

            try
            {
                switch (secim)
                {
                    case "1":
                        DoktorRandevulariniGoster(
                            user.UserId);
                        break;

                    case "2":
                        DoktorRandevuDurumuDegistir(
                            user.UserId,
                            "onayla");
                        break;

                    case "3":
                        DoktorRandevuDurumuDegistir(
                            user.UserId,
                            "reddet");
                        break;

                    case "4":
                        DoktorRandevuDurumuDegistir(
                            user.UserId,
                            "tamamla");
                        break;

                    case "5":
                        DoktorCalismaPrograminiGoster(
                            user.UserId);
                        break;

                    case "0":
                        return;

                    default:
                        UyariMesaji(
                            "Geçersiz seçim yaptınız.");
                        break;
                }
            }
            catch (Exception ex)
            {
                HataMesaji(
                    ex.Message);
            }
        }
    }


    private void DoktorRandevulariniGoster(
        int doctorId)
    {
        Console.Clear();

        SayfaBasligi(
            "RANDEVULARIM");

        var appointments =
            _doctorService
                .GetAppointments(
                    doctorId);

        if (appointments.Count == 0)
        {
            BilgiMesaji(
                "Randevu bulunamadı.");

            return;
        }

        foreach (var appointment
                 in appointments)
        {
            Console.WriteLine(
                $"Randevu No : {appointment.AppointmentId}");

            Console.WriteLine(
                $"Tarih      : {appointment.StartDateTime:g}");

            Console.WriteLine(
                $"Hasta      : " +
                $"{appointment.Patient.FirstName} " +
                $"{appointment.Patient.LastName}");

            Console.WriteLine(
                $"Hizmet     : {appointment.HealthService.Name}");

            Console.WriteLine(
                $"Durum      : " +
                $"{RandevuDurumuTurkce(appointment.Status)}");

            Ayirici();
        }

        Bekle();
    }


    private void DoktorRandevuDurumuDegistir(
        int doctorId,
        string islem)
    {
        Console.Clear();

        string baslik =
            islem switch
            {
                "onayla" => "RANDEVU ONAYLA",
                "reddet" => "RANDEVU REDDET",
                "tamamla" => "RANDEVUYU TAMAMLA",
                _ => "RANDEVU İŞLEMİ"
            };

        SayfaBasligi(
            baslik);

        var appointments =
            _doctorService
                .GetAppointments(
                    doctorId);

        if (appointments.Count == 0)
        {
            BilgiMesaji(
                "Randevu bulunamadı.");

            return;
        }

        foreach (var appointment
                 in appointments)
        {
            Console.WriteLine(
                $"{appointment.AppointmentId} | " +
                $"{appointment.StartDateTime:g} | " +
                $"{appointment.Patient.FirstName} " +
                $"{appointment.Patient.LastName} | " +
                $"{RandevuDurumuTurkce(appointment.Status)}");
        }

        int id =
            SayiOku(
                "\nRandevu ID: ");

        switch (islem)
        {
            case "onayla":

                _doctorService
                    .ApproveAppointment(
                        doctorId,
                        id);

                BasariMesaji(
                    "Randevu onaylandı.");
                break;


            case "reddet":

                _doctorService
                    .RejectAppointment(
                        doctorId,
                        id);

                BasariMesaji(
                    "Randevu reddedildi.");
                break;


            case "tamamla":

                _doctorService
                    .CompleteAppointment(
                        doctorId,
                        id);

                BasariMesaji(
                    "Randevu tamamlandı.");
                break;
        }
    }


    private void DoktorCalismaPrograminiGoster(
        int doctorId)
    {
        Console.Clear();

        SayfaBasligi(
            "ÇALIŞMA PROGRAMIM");

        var schedules =
            _doctorService
                .GetSchedules(
                    doctorId);

        if (schedules.Count == 0)
        {
            BilgiMesaji(
                "Çalışma programı bulunamadı.");

            return;
        }

        Console.WriteLine(
            $"{"Gün",-15} {"Başlangıç",-12} {"Bitiş",-12}");

        Ayirici();

        foreach (var schedule
                 in schedules)
        {
            Console.WriteLine(
                $"{GunTurkce(schedule.DayOfWeek),-15} " +
                $"{schedule.StartTime:HH\\:mm,-12} " +
                $"{schedule.EndTime:HH\\:mm,-12}");
        }

        Bekle();
    }


    // =====================================================
    // YÖNETİCİ
    // =====================================================

    private void YoneticiMenusu(
        User user)
    {
        while (true)
        {
            Console.Clear();

            RolBasligi(
                "YÖNETİCİ",
                user);

            AltBaslik(
                "Kurum Yönetimi");

            MenuSecenegi(
                1,
                "Şubeleri Listele");

            MenuSecenegi(
                2,
                "Yeni Şube Ekle");

            MenuSecenegi(
                3,
                "Yeni Bölüm Ekle");

            MenuSecenegi(
                4,
                "Yeni Hizmet Ekle");

            MenuSecenegi(
                5,
                "Hizmeti Şubeye Ata");

            MenuSecenegi(
                6,
                "Yeni Ekipman Ekle");

            MenuSecenegi(
                7,
                "Ekipmanı Şubeye Ata");


            AltBaslik(
                "Personel Yönetimi");

            MenuSecenegi(
                8,
                "Yeni Doktor Ekle");

            MenuSecenegi(
                9,
                "Yeni Sağlık Personeli Ekle");

            MenuSecenegi(
                10,
                "Doktora Hizmet Ata");

            MenuSecenegi(
                11,
                "Doktor Çalışma Saati Ata");

            MenuSecenegi(
                12,
                "Çalışanları Listele");

            MenuCikis(
                "Çıkış Yap");

            string? secim =
                SecimOku();

            try
            {
                switch (secim)
                {
                    case "1":
                        YoneticiSubeleriGoster();
                        break;

                    case "2":
                        YoneticiSubeEkle();
                        break;

                    case "3":
                        YoneticiBolumEkle();
                        break;

                    case "4":
                        YoneticiHizmetEkle();
                        break;

                    case "5":
                        YoneticiHizmetiSubeyeAta();
                        break;

                    case "6":
                        YoneticiEkipmanEkle();
                        break;

                    case "7":
                        YoneticiEkipmaniSubeyeAta();
                        break;

                    case "8":
                        YoneticiDoktorEkle();
                        break;

                    case "9":
                        YoneticiSaglikPersoneliEkle();
                        break;

                    case "10":
                        YoneticiDoktoraHizmetAta();
                        break;

                    case "11":
                        YoneticiDoktorCalismaSaatiAta();
                        break;

                    case "12":
                        YoneticiCalisanlariGoster();
                        break;

                    case "0":
                        return;

                    default:
                        UyariMesaji(
                            "Geçersiz seçim yaptınız.");
                        break;
                }
            }
            catch (Exception ex)
            {
                HataMesaji(
                    ex.Message);
            }
        }
    }


    private void YoneticiSubeleriGoster()
    {
        Console.Clear();

        SayfaBasligi(
            "ŞUBELER");

        var branches =
            _managerService
                .GetBranches();

        if (branches.Count == 0)
        {
            BilgiMesaji(
                "Kayıtlı şube bulunamadı.");

            return;
        }

        Console.WriteLine(
            $"{"ID",-5} " +
            $"{"Şube",-25} " +
            $"{"Şehir",-15} " +
            $"{"İlçe",-15}");

        Ayirici();

        foreach (var branch
                 in branches)
        {
            Console.WriteLine(
                $"{branch.BranchId,-5} " +
                $"{branch.Name,-25} " +
                $"{branch.City,-15} " +
                $"{branch.District,-15}");
        }

        Bekle();
    }


    private void YoneticiSubeEkle()
    {
        Console.Clear();

        SayfaBasligi(
            "YENİ ŞUBE EKLE");

        int hospitalId =
            SayiOku(
                "Hastane ID     : ");

        Console.Write(
            "Şube adı        : ");

        string name =
            Console.ReadLine() ?? "";

        Console.Write(
            "Şehir           : ");

        string city =
            Console.ReadLine() ?? "";

        Console.Write(
            "İlçe            : ");

        string district =
            Console.ReadLine() ?? "";

        Console.Write(
            "Adres           : ");

        string address =
            Console.ReadLine() ?? "";

        Console.Write(
            "Telefon         : ");

        string phone =
            Console.ReadLine() ?? "";

        TimeOnly opening =
            SaatOku(
                "Açılış (HH:mm) : ");

        TimeOnly closing =
            SaatOku(
                "Kapanış(HH:mm) : ");

        _managerService
            .AddBranch(
                hospitalId,
                name,
                city,
                district,
                address,
                phone,
                opening,
                closing);

        BasariMesaji(
            "Yeni şube başarıyla oluşturuldu.");
    }


    private void YoneticiBolumEkle()
    {
        Console.Clear();

        SayfaBasligi(
            "YENİ BÖLÜM EKLE");

        SubeleriKisaGoster();

        int branchId =
            SayiOku(
                "\nŞube ID: ");

        Console.Write(
            "Bölüm adı : ");

        string name =
            Console.ReadLine() ?? "";

        Console.Write(
            "Açıklama  : ");

        string description =
            Console.ReadLine() ?? "";

        _managerService
            .AddDepartment(
                branchId,
                name,
                description);

        BasariMesaji(
            "Bölüm başarıyla oluşturuldu.");
    }


    private void YoneticiHizmetEkle()
    {
        Console.Clear();

        SayfaBasligi(
            "YENİ HİZMET EKLE");

        Console.Write(
            "Hizmet adı               : ");

        string name =
            Console.ReadLine() ?? "";

        Console.Write(
            "Açıklama                  : ");

        string description =
            Console.ReadLine() ?? "";

        int duration =
            SayiOku(
                "Hizmet süresi (dakika)   : ");

        _managerService
            .AddHealthService(
                name,
                description,
                duration);

        BasariMesaji(
            "Hizmet başarıyla oluşturuldu.");
    }


    private void YoneticiHizmetiSubeyeAta()
    {
        Console.Clear();

        SayfaBasligi(
            "HİZMETİ ŞUBEYE ATA");

        SubeleriKisaGoster();

        int branchId =
            SayiOku(
                "\nŞube ID: ");

        Console.WriteLine();
        Console.WriteLine(
            "Hizmetler:");

        foreach (var service
                 in _managerService
                     .GetHealthServices())
        {
            Console.WriteLine(
                $"{service.HealthServiceId} - {service.Name}");
        }

        int serviceId =
            SayiOku(
                "\nHizmet ID: ");

        decimal price =
            OndalikSayiOku(
                "Fiyat     : ");

        _managerService
            .AssignServiceToBranch(
                branchId,
                serviceId,
                price);

        BasariMesaji(
            "Hizmet şubeye başarıyla atandı.");
    }


    private void YoneticiEkipmanEkle()
    {
        Console.Clear();

        SayfaBasligi(
            "YENİ EKİPMAN EKLE");

        Console.Write(
            "Ekipman adı : ");

        string name =
            Console.ReadLine() ?? "";

        Console.Write(
            "Açıklama    : ");

        string description =
            Console.ReadLine() ?? "";

        var equipment =
            _managerService
                .AddEquipment(
                    name,
                    description);

        BasariMesaji(
            $"Ekipman oluşturuldu. Ekipman ID: {equipment.EquipmentId}");
    }


    private void YoneticiEkipmaniSubeyeAta()
    {
        Console.Clear();

        SayfaBasligi(
            "EKİPMANI ŞUBEYE ATA");

        SubeleriKisaGoster();

        int branchId =
            SayiOku(
                "\nŞube ID    : ");

        int equipmentId =
            SayiOku(
                "Ekipman ID  : ");

        int quantity =
            SayiOku(
                "Adet        : ");

        _managerService
            .AssignEquipmentToBranch(
                branchId,
                equipmentId,
                quantity);

        BasariMesaji(
            "Ekipman şubeye başarıyla atandı.");
    }


    private void YoneticiDoktorEkle()
    {
        Console.Clear();

        SayfaBasligi(
            "YENİ DOKTOR EKLE");

        Console.Write(
            "Ad                : ");

        string firstName =
            Console.ReadLine() ?? "";

        Console.Write(
            "Soyad             : ");

        string lastName =
            Console.ReadLine() ?? "";

        Console.Write(
            "E-posta           : ");

        string email =
            Console.ReadLine() ?? "";

        Console.Write(
            "Telefon           : ");

        string phone =
            Console.ReadLine() ?? "";

        Console.Write(
            "Şifre             : ");

        string password =
            Console.ReadLine() ?? "";

        Console.Write(
            "Çalışan numarası  : ");

        string employeeNumber =
            Console.ReadLine() ?? "";

        SubeleriKisaGoster();

        int branchId =
            SayiOku(
                "\nŞube ID: ");

        Console.WriteLine();
        Console.WriteLine(
            "Bölümler:");

        var departments =
            _managerService
                .GetDepartments(
                    branchId);

        foreach (var department
                 in departments)
        {
            Console.WriteLine(
                $"{department.DepartmentId} - " +
                $"{department.Name}");
        }

        int departmentId =
            SayiOku(
                "\nBölüm ID: ");

        Console.Write(
            "Uzmanlık alanı     : ");

        string specialization =
            Console.ReadLine() ?? "";

        var doctor =
            _employeeService
                .AddDoctor(
                    firstName,
                    lastName,
                    email,
                    phone,
                    password,
                    employeeNumber,
                    DateTime.Today,
                    branchId,
                    departmentId,
                    specialization);

        BasariMesaji(
            $"Doktor başarıyla eklendi. Doktor ID: {doctor.UserId}");
    }


    private void YoneticiSaglikPersoneliEkle()
    {
        Console.Clear();

        SayfaBasligi(
            "YENİ SAĞLIK PERSONELİ EKLE");

        Console.Write(
            "Ad                : ");

        string firstName =
            Console.ReadLine() ?? "";

        Console.Write(
            "Soyad             : ");

        string lastName =
            Console.ReadLine() ?? "";

        Console.Write(
            "E-posta           : ");

        string email =
            Console.ReadLine() ?? "";

        Console.Write(
            "Telefon           : ");

        string phone =
            Console.ReadLine() ?? "";

        Console.Write(
            "Şifre             : ");

        string password =
            Console.ReadLine() ?? "";

        Console.Write(
            "Çalışan numarası  : ");

        string employeeNumber =
            Console.ReadLine() ?? "";

        SubeleriKisaGoster();

        int branchId =
            SayiOku(
                "\nŞube ID: ");

        Console.WriteLine();
        Console.WriteLine(
            "Bölümler:");

        var departments =
            _managerService
                .GetDepartments(
                    branchId);

        foreach (var department
                 in departments)
        {
            Console.WriteLine(
                $"{department.DepartmentId} - " +
                $"{department.Name}");
        }

        Console.WriteLine(
            "0 - Bölümsüz");

        int departmentInput =
            SayiOku(
                "\nBölüm ID: ");

        int? departmentId =
            departmentInput == 0
                ? null
                : departmentInput;

        Console.Write(
            "Görev / Pozisyon  : ");

        string position =
            Console.ReadLine() ?? "";

        var staff =
            _employeeService
                .AddHealthStaff(
                    firstName,
                    lastName,
                    email,
                    phone,
                    password,
                    employeeNumber,
                    DateTime.Today,
                    branchId,
                    departmentId,
                    position);

        BasariMesaji(
            $"Sağlık personeli eklendi. Personel ID: {staff.UserId}");
    }


    private void YoneticiDoktoraHizmetAta()
    {
        Console.Clear();

        SayfaBasligi(
            "DOKTORA HİZMET ATA");

        DoktorlariKisaGoster();

        int doctorId =
            SayiOku(
                "\nDoktor ID: ");

        Console.WriteLine();
        Console.WriteLine(
            "Hizmetler:");

        foreach (var service
                 in _managerService
                     .GetHealthServices())
        {
            Console.WriteLine(
                $"{service.HealthServiceId} - " +
                $"{service.Name}");
        }

        int serviceId =
            SayiOku(
                "\nHizmet ID: ");

        _employeeService
            .AssignService(
                doctorId,
                serviceId);

        BasariMesaji(
            "Hizmet doktora başarıyla atandı.");
    }


    private void YoneticiDoktorCalismaSaatiAta()
    {
        Console.Clear();

        SayfaBasligi(
            "DOKTOR ÇALIŞMA SAATİ ATA");

        DoktorlariKisaGoster();

        int doctorId =
            SayiOku(
                "\nDoktor ID: ");

        Console.WriteLine();

        MenuSecenegi(
            1,
            "Pazartesi");

        MenuSecenegi(
            2,
            "Salı");

        MenuSecenegi(
            3,
            "Çarşamba");

        MenuSecenegi(
            4,
            "Perşembe");

        MenuSecenegi(
            5,
            "Cuma");

        MenuSecenegi(
            6,
            "Cumartesi");

        MenuSecenegi(
            7,
            "Pazar");

        int gunSecimi =
            SayiOku(
                "\nGün seçimi: ");

        DayOfWeek day =
            gunSecimi switch
            {
                1 => DayOfWeek.Monday,
                2 => DayOfWeek.Tuesday,
                3 => DayOfWeek.Wednesday,
                4 => DayOfWeek.Thursday,
                5 => DayOfWeek.Friday,
                6 => DayOfWeek.Saturday,
                7 => DayOfWeek.Sunday,

                _ => throw new Exception(
                    "Geçersiz gün seçimi.")
            };

        TimeOnly start =
            SaatOku(
                "Başlangıç saati (HH:mm): ");

        TimeOnly end =
            SaatOku(
                "Bitiş saati (HH:mm)     : ");

        _employeeService
            .AddSchedule(
                doctorId,
                day,
                start,
                end);

        BasariMesaji(
            "Doktor çalışma programı güncellendi.");
    }


    private void YoneticiCalisanlariGoster()
    {
        Console.Clear();

        SayfaBasligi(
            "ÇALIŞANLAR");

        Console.WriteLine(
            "Şube filtresi:");

        Console.WriteLine(
            "0 - Tüm Şubeler");

        foreach (var branch
                 in _managerService
                     .GetBranches())
        {
            Console.WriteLine(
                $"{branch.BranchId} - {branch.Name}");
        }

        int branchId =
            SayiOku(
                "\nŞube ID: ");

        var employees =
            _employeeService
                .GetEmployees(
                    branchId == 0
                        ? null
                        : branchId);

        if (employees.Count == 0)
        {
            BilgiMesaji(
                "Çalışan bulunamadı.");

            return;
        }

        Console.WriteLine();

        Console.WriteLine(
            $"{"ID",-5} " +
            $"{"Ad Soyad",-25} " +
            $"{"Görev",-30} " +
            $"{"Şube",-25}");

        Ayirici();

        foreach (var employee
                 in employees)
        {
            string fullName =
                $"{employee.FirstName} " +
                $"{employee.LastName}";

            Console.WriteLine(
                $"{employee.UserId,-5} " +
                $"{fullName,-25} " +
                $"{CalisanTuruTurkce(employee),-30} " +
                $"{employee.Branch.Name,-25}");
        }

        Bekle();
    }


    // =====================================================
    // SAĞLIK PERSONELİ
    // =====================================================

    private void SaglikPersoneliMenusu(
        User user)
    {
        while (true)
        {
            Console.Clear();

            RolBasligi(
                "SAĞLIK PERSONELİ",
                user);

            MenuSecenegi(
                1,
                "Hesap Bilgilerimi Görüntüle");

            MenuCikis(
                "Çıkış Yap");

            string? secim =
                SecimOku();

            switch (secim)
            {
                case "1":

                    Console.Clear();

                    SayfaBasligi(
                        "HESAP BİLGİLERİ");

                    Console.WriteLine(
                        $"Ad Soyad : " +
                        $"{user.FirstName} " +
                        $"{user.LastName}");

                    Console.WriteLine(
                        $"E-posta  : {user.Email}");

                    Console.WriteLine(
                        $"Telefon  : {user.Phone}");

                    Bekle();
                    break;


                case "0":

                    return;


                default:

                    UyariMesaji(
                        "Geçersiz seçim yaptınız.");
                    break;
            }
        }
    }


    // =====================================================
    // KISA LİSTELER
    // =====================================================

    private void SubeleriKisaGoster()
    {
        Console.WriteLine();
        Console.WriteLine(
            "Şubeler:");

        foreach (var branch
                 in _managerService
                     .GetBranches())
        {
            Console.WriteLine(
                $"{branch.BranchId} - " +
                $"{branch.Name}");
        }
    }


    private void DoktorlariKisaGoster()
    {
        Console.WriteLine();
        Console.WriteLine(
            "Doktorlar:");

        var doctors =
            _employeeService
                .GetEmployees()
                .OfType<Doctor>()
                .ToList();

        if (doctors.Count == 0)
        {
            Console.WriteLine(
                "Kayıtlı doktor bulunamadı.");

            return;
        }

        foreach (var doctor
                 in doctors)
        {
            Console.WriteLine(
                $"{doctor.UserId} - " +
                $"{doctor.FirstName} " +
                $"{doctor.LastName} | " +
                $"{doctor.Specialization}");
        }
    }


    // =====================================================
    // TÜRKÇE DÖNÜŞÜMLER
    // =====================================================

    private static string RandevuDurumuTurkce(
        AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Pending
                => "Beklemede",

            AppointmentStatus.Approved
                => "Onaylandı",

            AppointmentStatus.Rejected
                => "Reddedildi",

            AppointmentStatus.Cancelled
                => "İptal Edildi",

            AppointmentStatus.Completed
                => "Tamamlandı",

            _ => "Bilinmiyor"
        };
    }


    private static string GunTurkce(
        DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday
                => "Pazartesi",

            DayOfWeek.Tuesday
                => "Salı",

            DayOfWeek.Wednesday
                => "Çarşamba",

            DayOfWeek.Thursday
                => "Perşembe",

            DayOfWeek.Friday
                => "Cuma",

            DayOfWeek.Saturday
                => "Cumartesi",

            DayOfWeek.Sunday
                => "Pazar",

            _ => "Bilinmiyor"
        };
    }


    private static string CalisanTuruTurkce(
        Employee employee)
    {
        return employee switch
        {
            Doctor
                => "Doktor",

            Manager
                => "Yönetici",

            HealthStaff staff
                => $"Sağlık Personeli / {staff.Position}",

            _
                => "Çalışan"
        };
    }


    // =====================================================
    // UI YARDIMCILARI
    // =====================================================

    private static void AnaBaslik()
    {
        Console.ForegroundColor =
            ConsoleColor.Cyan;

        Console.WriteLine(
            "╔════════════════════════════════════════════╗");

        Console.WriteLine(
            "║             ALTUNPITAL                    ║");

        Console.WriteLine(
            "║       Hastane Randevu Sistemi             ║");

        Console.WriteLine(
            "╚════════════════════════════════════════════╝");

        Console.ResetColor();

        Console.WriteLine();
    }


    private static void SayfaBasligi(
        string title)
    {
        Console.ForegroundColor =
            ConsoleColor.Cyan;

        Console.WriteLine(
            "══════════════════════════════════════════════");

        Console.WriteLine(
            $"  {title}");

        Console.WriteLine(
            "══════════════════════════════════════════════");

        Console.ResetColor();

        Console.WriteLine();
    }


    private static void RolBasligi(
        string role,
        User user)
    {
        Console.ForegroundColor =
            ConsoleColor.Cyan;

        Console.WriteLine(
            "══════════════════════════════════════════════");

        Console.WriteLine(
            $"  {role}");

        Console.WriteLine(
            $"  {user.FirstName} {user.LastName}");

        Console.WriteLine(
            "══════════════════════════════════════════════");

        Console.ResetColor();

        Console.WriteLine();
    }


    private static void AltBaslik(
        string title)
    {
        Console.WriteLine();

        Console.ForegroundColor =
            ConsoleColor.DarkCyan;

        Console.WriteLine(
            $"--- {title} ---");

        Console.ResetColor();

        Console.WriteLine();
    }


    private static void MenuSecenegi(
        int number,
        string text)
    {
        Console.ForegroundColor =
            ConsoleColor.White;

        Console.Write(
            $"{number,2}");

        Console.ResetColor();

        Console.WriteLine(
            $" - {text}");
    }


    private static void MenuCikis(
        string text)
    {
        Console.ForegroundColor =
            ConsoleColor.DarkGray;

        Console.Write(
            " 0");

        Console.ResetColor();

        Console.WriteLine(
            $" - {text}");
    }


    private static string? SecimOku()
    {
        Console.WriteLine();

        Console.ForegroundColor =
            ConsoleColor.Yellow;

        Console.Write(
            "Seçiminiz: ");

        Console.ResetColor();

        return Console.ReadLine();
    }


    private static void BasariMesaji(
        string message)
    {
        Console.WriteLine();

        Console.ForegroundColor =
            ConsoleColor.Green;

        Console.WriteLine(
            $"[BAŞARILI] {message}");

        Console.ResetColor();

        Bekle();
    }


    private static void HataMesaji(
        string message)
    {
        Console.WriteLine();

        Console.ForegroundColor =
            ConsoleColor.Red;

        Console.WriteLine(
            $"[HATA] {message}");

        Console.ResetColor();

        Bekle();
    }


    private static void UyariMesaji(
        string message)
    {
        Console.WriteLine();

        Console.ForegroundColor =
            ConsoleColor.Yellow;

        Console.WriteLine(
            $"[UYARI] {message}");

        Console.ResetColor();

        Bekle();
    }


    private static void BilgiMesaji(
        string message)
    {
        Console.WriteLine();

        Console.ForegroundColor =
            ConsoleColor.Cyan;

        Console.WriteLine(
            $"[BİLGİ] {message}");

        Console.ResetColor();

        Bekle();
    }


    private static void Ayirici()
    {
        Console.WriteLine(
            "────────────────────────────────────────────────────────────");
    }


    // =====================================================
    // INPUT YARDIMCILARI
    // =====================================================

    private static int SayiOku(
        string message)
    {
        Console.Write(
            message);

        if (!int.TryParse(
                Console.ReadLine(),
                out int value))
        {
            throw new Exception(
                "Geçerli bir sayı giriniz.");
        }

        return value;
    }


    private static decimal OndalikSayiOku(
        string message)
    {
        Console.Write(
            message);

        if (!decimal.TryParse(
                Console.ReadLine(),
                out decimal value))
        {
            throw new Exception(
                "Geçerli bir sayı giriniz.");
        }

        return value;
    }


    private static TimeOnly SaatOku(
        string message)
    {
        Console.Write(
            message);

        if (!TimeOnly.TryParse(
                Console.ReadLine(),
                out TimeOnly value))
        {
            throw new Exception(
                "Geçerli bir saat giriniz.");
        }

        return value;
    }


    private static void Bekle()
    {
        Console.WriteLine();

        Console.ForegroundColor =
            ConsoleColor.DarkGray;

        Console.Write(
            "Devam etmek için Enter tuşuna basın...");

        Console.ResetColor();

        Console.ReadLine();
    }
}