using AltunPital.Data;
using AltunPital.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using Font = System.Drawing.Font;
using Label = System.Windows.Forms.Label;

namespace AltunPital.UI.Desktop;

public class ManagerForm :
    DesktopShellForm
{
    private readonly IManagerService
        _managerService;

    private readonly IEmployeeService
        _employeeService;


    private readonly DataGridView
        grid =
            new()
            {
                ReadOnly = true,

                AllowUserToAddRows =
                    false,

                AllowUserToDeleteRows =
                    false,

                SelectionMode =
                    DataGridViewSelectionMode
                        .FullRowSelect,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode
                        .Fill,

                BackgroundColor =
                    Color.White
            };


    public ManagerForm(
        User user,
        IManagerService managerService,
        IEmployeeService employeeService,
        IUserAccountService accountService)
        : base(
            user,
            accountService,
            "Yönetici")
    {
        _managerService =
            managerService;

        _employeeService =
            employeeService;


        AddSidebarButton(
            "Genel Bakış",
            150,
            ShowOverview);

        AddSidebarButton(
            "Şubeler",
            195,
            ShowBranches);

        AddSidebarButton(
            "Bölümler",
            240,
            ShowDepartments);

        AddSidebarButton(
            "Hizmetler",
            285,
            ShowServices);

        AddSidebarButton(
            "Ekipmanlar",
            330,
            ShowEquipment);

        AddSidebarButton(
            "Doktorlar",
            375,
            ShowDoctors);

        AddSidebarButton(
            "Sağlık Personeli",
            420,
            ShowHealthStaff);

        AddSidebarButton(
            "Tüm Çalışanlar",
            465,
            ShowEmployees);

        AddSidebarButton(
            "Profilim",
            510,
            OpenProfile);

        AddLogoutButton();


        ShowOverview();
    }


    private void ShowOverview()
    {
        var panel =
            new FlowLayoutPanel
            {
                Dock =
                    DockStyle.Fill,

                AutoScroll =
                    true,

                Padding =
                    new Padding(15)
            };


        var employees =
            _employeeService
                .GetEmployees();


        AddInfoCard(
            panel,
            "Şubeler",
            _managerService
                .GetBranches()
                .Count);

        AddInfoCard(
            panel,
            "Hizmetler",
            _managerService
                .GetHealthServices()
                .Count);

        AddInfoCard(
            panel,
            "Doktorlar",
            employees
                .OfType<Doctor>()
                .Count());

        AddInfoCard(
            panel,
            "Sağlık Personeli",
            employees
                .OfType<HealthStaff>()
                .Count());


        ShowPage(
            "Genel Bakış",
            panel);
    }


    private void ShowBranches()
    {
        var panel =
            CreateGridPanel(
                out FlowLayoutPanel actions);


        actions.Controls.Add(
            CreateActionButton(
                "Yeni Şube",
                AddBranch));


        grid.DataSource =
            _managerService
                .GetBranches()
                .Select(b =>
                    new
                    {
                        ID =
                            b.BranchId,

                        Sube =
                            b.Name,

                        Sehir =
                            b.City,

                        Ilce =
                            b.District,

                        Telefon =
                            b.Phone,

                        Acilis =
                            b.OpeningTime,

                        Kapanis =
                            b.ClosingTime
                    })
                .ToList();


        ShowPage(
            "Şubeler",
            panel);
    }


    private void AddBranch()
    {
        try
        {
            int? hospitalId =
                PromptDialog.AskInt(
                    "Yeni Şube",
                    "Hastane ID:",
                    1);

            if (hospitalId == null)
                return;


            string? name =
                PromptDialog.Ask(
                    "Yeni Şube",
                    "Şube adı:");

            if (name == null)
                return;


            string? city =
                PromptDialog.Ask(
                    "Yeni Şube",
                    "Şehir:");

            if (city == null)
                return;


            string? district =
                PromptDialog.Ask(
                    "Yeni Şube",
                    "İlçe:");

            if (district == null)
                return;


            string address =
                PromptDialog.Ask(
                    "Yeni Şube",
                    "Adres:")
                ?? "";


            string phone =
                PromptDialog.Ask(
                    "Yeni Şube",
                    "Telefon:")
                ?? "";


            TimeOnly? opening =
                PromptDialog.AskTime(
                    "Yeni Şube",
                    "Açılış saati (HH:mm):");

            if (opening == null)
                return;


            TimeOnly? closing =
                PromptDialog.AskTime(
                    "Yeni Şube",
                    "Kapanış saati (HH:mm):");

            if (closing == null)
                return;


            _managerService
                .AddBranch(
                    hospitalId.Value,
                    name,
                    city,
                    district,
                    address,
                    phone,
                    opening.Value,
                    closing.Value);


            ShowSuccess(
                "Şube eklendi.");

            ShowBranches();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ShowDepartments()
    {
        try
        {
            var branch =
                PromptDialog.Choose(
                    "Bölümler",
                    "Şube seçiniz:",
                    _managerService.GetBranches(),
                    b =>
                        $"{b.Name} - {b.City}");

            if (branch == null)
                return;


            var panel =
                CreateGridPanel(
                    out FlowLayoutPanel actions);


            actions.Controls.Add(
                CreateActionButton(
                    "Yeni Bölüm",
                    () =>
                        AddDepartment(
                            branch)));


            grid.DataSource =
                _managerService
                    .GetDepartments(
                        branch.BranchId)
                    .Select(d =>
                        new
                        {
                            ID =
                                d.DepartmentId,

                            Bolum =
                                d.Name,

                            Aciklama =
                                d.Description
                        })
                    .ToList();


            ShowPage(
                $"Bölümler - {branch.Name}",
                panel);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AddDepartment(
        Branch branch)
    {
        try
        {
            string? name =
                PromptDialog.Ask(
                    "Yeni Bölüm",
                    "Bölüm adı:");

            if (name == null)
                return;


            string description =
                PromptDialog.Ask(
                    "Yeni Bölüm",
                    "Açıklama:")
                ?? "";


            _managerService
                .AddDepartment(
                    branch.BranchId,
                    name,
                    description);


            ShowSuccess(
                "Bölüm eklendi.");

            ShowDepartments();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ShowServices()
    {
        var panel =
            CreateGridPanel(
                out FlowLayoutPanel actions);


        actions.Controls.Add(
            CreateActionButton(
                "Yeni Hizmet",
                AddService));


        actions.Controls.Add(
            CreateActionButton(
                "Şubeye Ata",
                AssignServiceToBranch));


        grid.DataSource =
            _managerService
                .GetHealthServices()
                .Select(s =>
                    new
                    {
                        ID =
                            s.HealthServiceId,

                        Hizmet =
                            s.Name,

                        SureDakika =
                            s.DurationMinutes,

                        Aciklama =
                            s.Description
                    })
                .ToList();


        ShowPage(
            "Hizmetler",
            panel);
    }


    private void AddService()
    {
        try
        {
            string? name =
                PromptDialog.Ask(
                    "Yeni Hizmet",
                    "Hizmet adı:");

            if (name == null)
                return;


            string description =
                PromptDialog.Ask(
                    "Yeni Hizmet",
                    "Açıklama:")
                ?? "";


            int? duration =
                PromptDialog.AskInt(
                    "Yeni Hizmet",
                    "Süre (dakika):");

            if (duration == null)
                return;


            _managerService
                .AddHealthService(
                    name,
                    description,
                    duration.Value);


            ShowSuccess(
                "Hizmet eklendi.");

            ShowServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AssignServiceToBranch()
    {
        try
        {
            var branch =
                PromptDialog.Choose(
                    "Hizmet Ata",
                    "Şube seçiniz:",
                    _managerService.GetBranches(),
                    b => b.Name);

            if (branch == null)
                return;


            var service =
                PromptDialog.Choose(
                    "Hizmet Ata",
                    "Hizmet seçiniz:",
                    _managerService.GetHealthServices(),
                    s => s.Name);

            if (service == null)
                return;


            decimal? price =
                PromptDialog.AskDecimal(
                    "Hizmet Ata",
                    "Fiyat:");

            if (price == null)
                return;


            _managerService
                .AssignServiceToBranch(
                    branch.BranchId,
                    service.HealthServiceId,
                    price.Value);


            ShowSuccess(
                "Hizmet şubeye atandı.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ShowEquipment()
    {
        var panel =
            new FlowLayoutPanel
            {
                Dock =
                    DockStyle.Top,

                Height =
                    80,

                Padding =
                    new Padding(10)
            };


        panel.Controls.Add(
            CreateActionButton(
                "Yeni Ekipman",
                AddEquipment));

        panel.Controls.Add(
            CreateActionButton(
                "Şubeye Ekipman Ata",
                AssignEquipment));


        ShowPage(
            "Ekipman Yönetimi",
            panel);
    }


    private void AddEquipment()
    {
        try
        {
            string? name =
                PromptDialog.Ask(
                    "Yeni Ekipman",
                    "Ekipman adı:");

            if (name == null)
                return;


            string description =
                PromptDialog.Ask(
                    "Yeni Ekipman",
                    "Açıklama:")
                ?? "";


            var equipment =
                _managerService
                    .AddEquipment(
                        name,
                        description);


            ShowSuccess(
                $"Ekipman eklendi.\n" +
                $"Ekipman ID: {equipment.EquipmentId}");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AssignEquipment()
    {
        try
        {
            var branch =
                PromptDialog.Choose(
                    "Ekipman Ata",
                    "Şube seçiniz:",
                    _managerService.GetBranches(),
                    b => b.Name);

            if (branch == null)
                return;


            int? equipmentId =
                PromptDialog.AskInt(
                    "Ekipman Ata",
                    "Ekipman ID:");

            if (equipmentId == null)
                return;


            int? quantity =
                PromptDialog.AskInt(
                    "Ekipman Ata",
                    "Adet:");

            if (quantity == null)
                return;


            _managerService
                .AssignEquipmentToBranch(
                    branch.BranchId,
                    equipmentId.Value,
                    quantity.Value);


            ShowSuccess(
                "Ekipman şubeye atandı.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ShowDoctors()
    {
        var panel =
            CreateGridPanel(
                out FlowLayoutPanel actions);


        actions.Controls.Add(
            CreateActionButton(
                "Yeni Doktor",
                AddDoctor));

        actions.Controls.Add(
            CreateActionButton(
                "Hizmet Ata",
                AssignDoctorService));

        actions.Controls.Add(
            CreateActionButton(
                "Çalışma Saati Ata",
                AssignDoctorSchedule));


        var doctors =
            _employeeService
                .GetEmployees()
                .OfType<Doctor>()
                .ToList();


        grid.DataSource =
            doctors
                .Select(d =>
                    new
                    {
                        ID =
                            d.UserId,

                        Ad =
                            d.FirstName,

                        Soyad =
                            d.LastName,

                        Eposta =
                            d.Email,

                        Uzmanlik =
                            d.Specialization,

                        Sube =
                            d.Branch.Name,

                        Bolum =
                            d.Department?.Name
                    })
                .ToList();


        ShowPage(
            "Doktorlar",
            panel);
    }


    private void AddDoctor()
    {
        try
        {
            string? firstName =
                PromptDialog.Ask(
                    "Yeni Doktor",
                    "Ad:");

            if (firstName == null)
                return;


            string? lastName =
                PromptDialog.Ask(
                    "Yeni Doktor",
                    "Soyad:");

            if (lastName == null)
                return;


            string? email =
                PromptDialog.Ask(
                    "Yeni Doktor",
                    "E-posta:");

            if (email == null)
                return;


            string phone =
                PromptDialog.Ask(
                    "Yeni Doktor",
                    "Telefon:")
                ?? "";


            string? password =
                PromptDialog.Ask(
                    "Yeni Doktor",
                    "Şifre:",
                    "",
                    true);

            if (password == null)
                return;


            string? employeeNumber =
                PromptDialog.Ask(
                    "Yeni Doktor",
                    "Çalışan numarası:");

            if (employeeNumber == null)
                return;


            var branch =
                PromptDialog.Choose(
                    "Yeni Doktor",
                    "Şube:",
                    _managerService.GetBranches(),
                    b => b.Name);

            if (branch == null)
                return;


            var department =
                PromptDialog.Choose(
                    "Yeni Doktor",
                    "Bölüm:",
                    _managerService
                        .GetDepartments(
                            branch.BranchId),
                    d => d.Name);

            if (department == null)
                return;


            string? specialization =
                PromptDialog.Ask(
                    "Yeni Doktor",
                    "Uzmanlık:");

            if (specialization == null)
                return;


            _employeeService
                .AddDoctor(
                    firstName,
                    lastName,
                    email,
                    phone,
                    password,
                    employeeNumber,
                    DateTime.Today,
                    branch.BranchId,
                    department.DepartmentId,
                    specialization);


            ShowSuccess(
                "Doktor eklendi.");

            ShowDoctors();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AssignDoctorService()
    {
        try
        {
            var doctors =
                _employeeService
                    .GetEmployees()
                    .OfType<Doctor>()
                    .ToList();


            var doctor =
                PromptDialog.Choose(
                    "Doktora Hizmet Ata",
                    "Doktor:",
                    doctors,
                    d =>
                        $"{d.FirstName} {d.LastName}");

            if (doctor == null)
                return;


            var service =
                PromptDialog.Choose(
                    "Doktora Hizmet Ata",
                    "Hizmet:",
                    _managerService.GetHealthServices(),
                    s => s.Name);

            if (service == null)
                return;


            _employeeService
                .AssignService(
                    doctor.UserId,
                    service.HealthServiceId);


            ShowSuccess(
                "Hizmet doktora atandı.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AssignDoctorSchedule()
    {
        try
        {
            var doctors =
                _employeeService
                    .GetEmployees()
                    .OfType<Doctor>()
                    .ToList();


            var doctor =
                PromptDialog.Choose(
                    "Çalışma Saati",
                    "Doktor:",
                    doctors,
                    d =>
                        $"{d.FirstName} {d.LastName}");

            if (doctor == null)
                return;


            var days =
                new List<DayChoice>
                {
                    new(
                        DayOfWeek.Monday,
                        "Pazartesi"),

                    new(
                        DayOfWeek.Tuesday,
                        "Salı"),

                    new(
                        DayOfWeek.Wednesday,
                        "Çarşamba"),

                    new(
                        DayOfWeek.Thursday,
                        "Perşembe"),

                    new(
                        DayOfWeek.Friday,
                        "Cuma"),

                    new(
                        DayOfWeek.Saturday,
                        "Cumartesi"),

                    new(
                        DayOfWeek.Sunday,
                        "Pazar")
                };


            var day =
                PromptDialog.Choose(
                    "Çalışma Saati",
                    "Gün:",
                    days,
                    d => d.Text);

            if (day == null)
                return;


            TimeOnly? start =
                PromptDialog.AskTime(
                    "Çalışma Saati",
                    "Başlangıç (HH:mm):");

            if (start == null)
                return;


            TimeOnly? end =
                PromptDialog.AskTime(
                    "Çalışma Saati",
                    "Bitiş (HH:mm):");

            if (end == null)
                return;


            _employeeService
                .AddSchedule(
                    doctor.UserId,
                    day.Day,
                    start.Value,
                    end.Value);


            ShowSuccess(
                "Çalışma saati eklendi.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ShowHealthStaff()
    {
        var panel =
            CreateGridPanel(
                out FlowLayoutPanel actions);


        actions.Controls.Add(
            CreateActionButton(
                "Yeni Sağlık Personeli",
                AddHealthStaff));


        var staff =
            _employeeService
                .GetEmployees()
                .OfType<HealthStaff>()
                .ToList();


        grid.DataSource =
            staff
                .Select(s =>
                    new
                    {
                        ID =
                            s.UserId,

                        Ad =
                            s.FirstName,

                        Soyad =
                            s.LastName,

                        Eposta =
                            s.Email,

                        Pozisyon =
                            s.Position,

                        Sube =
                            s.Branch.Name,

                        Bolum =
                            s.Department?.Name
                    })
                .ToList();


        ShowPage(
            "Sağlık Personeli",
            panel);
    }


    private void AddHealthStaff()
    {
        try
        {
            string? firstName =
                PromptDialog.Ask(
                    "Yeni Sağlık Personeli",
                    "Ad:");

            if (firstName == null)
                return;


            string? lastName =
                PromptDialog.Ask(
                    "Yeni Sağlık Personeli",
                    "Soyad:");

            if (lastName == null)
                return;


            string? email =
                PromptDialog.Ask(
                    "Yeni Sağlık Personeli",
                    "E-posta:");

            if (email == null)
                return;


            string phone =
                PromptDialog.Ask(
                    "Yeni Sağlık Personeli",
                    "Telefon:")
                ?? "";


            string? password =
                PromptDialog.Ask(
                    "Yeni Sağlık Personeli",
                    "Şifre:",
                    "",
                    true);

            if (password == null)
                return;


            string? employeeNumber =
                PromptDialog.Ask(
                    "Yeni Sağlık Personeli",
                    "Çalışan numarası:");

            if (employeeNumber == null)
                return;


            var branch =
                PromptDialog.Choose(
                    "Yeni Sağlık Personeli",
                    "Şube:",
                    _managerService.GetBranches(),
                    b => b.Name);

            if (branch == null)
                return;


            var department =
                PromptDialog.Choose(
                    "Yeni Sağlık Personeli",
                    "Bölüm:",
                    _managerService
                        .GetDepartments(
                            branch.BranchId),
                    d => d.Name);


            string? position =
                PromptDialog.Ask(
                    "Yeni Sağlık Personeli",
                    "Görev / Pozisyon:");

            if (position == null)
                return;


            _employeeService
                .AddHealthStaff(
                    firstName,
                    lastName,
                    email,
                    phone,
                    password,
                    employeeNumber,
                    DateTime.Today,
                    branch.BranchId,
                    department?.DepartmentId,
                    position);


            ShowSuccess(
                "Sağlık personeli eklendi.");

            ShowHealthStaff();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ShowEmployees()
    {
        var panel =
            CreateGridPanel(
                out _);


        grid.DataSource =
            _employeeService
                .GetEmployees()
                .Select(e =>
                    new
                    {
                        ID =
                            e.UserId,

                        Ad =
                            e.FirstName,

                        Soyad =
                            e.LastName,

                        Eposta =
                            e.Email,

                        CalisanNo =
                            e.EmployeeNumber,

                        Sube =
                            e.Branch.Name,

                        Bolum =
                            e.Department?.Name,

                        Tur =
                            EmployeeTypeText(
                                e)
                    })
                .ToList();


        ShowPage(
            "Tüm Çalışanlar",
            panel);
    }


    private Panel CreateGridPanel(
        out FlowLayoutPanel actions)
    {
        var panel =
            new Panel();


        actions =
            new FlowLayoutPanel
            {
                Dock =
                    DockStyle.Top,

                Height =
                    65,

                Padding =
                    new Padding(5)
            };


        grid.Dock =
            DockStyle.Fill;


        panel.Controls.Add(
            grid);

        panel.Controls.Add(
            actions);


        return panel;
    }


    private static Button
        CreateActionButton(
            string text,
            Action action)
    {
        var button =
            UiTheme.CreateButton(
                text,
                190);


        button.Click +=
            (_, _) =>
                action();


        return button;
    }


    private static void AddInfoCard(
        FlowLayoutPanel panel,
        string title,
        int value)
    {
        var card =
            new Panel
            {
                Width =
                    230,

                Height =
                    130,

                BackColor =
                    Color.White,

                Margin =
                    new Padding(12)
            };


        var number =
            new Label
            {
                Text =
                    value.ToString(),

                Font =
                    new Font(
                        "Segoe UI",
                        26,
                        FontStyle.Bold),

                ForeColor =
                    UiTheme.Primary,

                AutoSize =
                    true,

                Left =
                    20,

                Top =
                    20
            };


        var label =
            new Label
            {
                Text =
                    title,

                AutoSize =
                    true,

                Left =
                    20,

                Top =
                    80
            };


        card.Controls.Add(
            number);

        card.Controls.Add(
            label);


        panel.Controls.Add(
            card);
    }


    private static string
        EmployeeTypeText(
            Employee employee)
    {
        return employee switch
        {
            Doctor =>
                "Doktor",

            Manager =>
                "Yönetici",

            HealthStaff staff =>
                $"Sağlık Personeli ({staff.Position})",

            _ =>
                "Çalışan"
        };
    }


    private class DayChoice
    {
        public DayOfWeek Day
        {
            get;
        }

        public string Text
        {
            get;
        }


        public DayChoice(
            DayOfWeek day,
            string text)
        {
            Day =
                day;

            Text =
                text;
        }
    }
}