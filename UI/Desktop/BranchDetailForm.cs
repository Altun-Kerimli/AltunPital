using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;

namespace AltunPital.UI.Desktop;

public class BranchDetailForm : Form
{
    private readonly Branch
        _branch;

    private readonly IManagerService
        _managerService;

    private readonly IEmployeeService
        _employeeService;


    private readonly TabControl
        tabs =
            new();


    public BranchDetailForm(
        Branch branch,
        IManagerService managerService,
        IEmployeeService employeeService)
    {
        _branch =
            branch;

        _managerService =
            managerService;

        _employeeService =
            employeeService;


        Text =
            $"{branch.Name} - Şube Yönetimi";


        WindowState =
            FormWindowState.Maximized;

        MinimumSize =
            new Size(
                1100,
                700);

        StartPosition =
            FormStartPosition.CenterParent;


        BuildUi();
    }


    private void BuildUi()
    {
        var header =
            new Panel
            {
                Dock =
                    DockStyle.Top,

                Height =
                    95,

                BackColor =
                    UiTheme.PrimaryDark
            };


        var title =
            new Label
            {
                Text =
                    _branch.Name,

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Segoe UI",
                        20,
                        FontStyle.Bold),

                AutoSize =
                    true,

                Left =
                    30,

                Top =
                    20
            };


        var subtitle =
            new Label
            {
                Text =
                    $"{_branch.City} / {_branch.District}",

                ForeColor =
                    Color.LightGray,

                AutoSize =
                    true,

                Left =
                    32,

                Top =
                    58
            };


        var close =
            new Button
            {
                Text =
                    "Şubelere Dön",

                Width =
                    140,

                Height =
                    40,

                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Right,

                Left =
                    900,

                Top =
                    27,

                BackColor =
                    Color.White,

                FlatStyle =
                    FlatStyle.Flat
            };


        close.Click +=
            (_, _) =>
                Close();


        header.Controls.Add(
            title);

        header.Controls.Add(
            subtitle);

        header.Controls.Add(
            close);


        header.Resize +=
            (_, _) =>
            {
                close.Left =
                    header.ClientSize.Width -
                    close.Width -
                    30;
            };


        tabs.Dock =
            DockStyle.Fill;


        tabs.TabPages.Add(
            CreateGeneralTab());

        tabs.TabPages.Add(
            CreateDepartmentsTab());

        tabs.TabPages.Add(
            CreateDoctorsTab());

        tabs.TabPages.Add(
            CreateStaffTab());

        tabs.TabPages.Add(
            CreateServicesTab());

        tabs.TabPages.Add(
            CreateEquipmentTab());

        tabs.TabPages.Add(
            CreateAppointmentsTab());


        Controls.Add(
            tabs);

        Controls.Add(
            header);
    }


    // =====================================================
    // GENEL BİLGİLER
    // =====================================================

    private TabPage CreateGeneralTab()
    {
        var tab =
            new TabPage(
                "Genel Bilgiler");


        var panel =
            new Panel
            {
                Dock =
                    DockStyle.Fill,

                Padding =
                    new Padding(35),

                BackColor =
                    Color.White
            };


        int y =
            35;


        AddInfo(
            panel,
            "Şube",
            _branch.Name,
            ref y);

        AddInfo(
            panel,
            "Şehir",
            _branch.City,
            ref y);

        AddInfo(
            panel,
            "İlçe",
            _branch.District,
            ref y);

        AddInfo(
            panel,
            "Adres",
            _branch.Address,
            ref y);

        AddInfo(
            panel,
            "Telefon",
            _branch.Phone,
            ref y);

        AddInfo(
            panel,
            "Çalışma Saatleri",
            $"{_branch.OpeningTime} - {_branch.ClosingTime}",
            ref y);


        tab.Controls.Add(
            panel);


        return tab;
    }


    // =====================================================
    // BÖLÜMLER
    // =====================================================

    private TabPage CreateDepartmentsTab()
    {
        var grid =
            UiTableHelper.CreateGrid();


        var page =
            CreateSearchPage(
                "Bölümler",
                grid,
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni",
                () =>
                    AddDepartment(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                () =>
                    EditDepartment(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                () =>
                    DeleteDepartment(
                        grid)));


        LoadDepartments(
            grid);


        return page;
    }


    private void LoadDepartments(
        DataGridView grid)
    {
        UiTableHelper.Bind(
            grid,
            _managerService
                .GetDepartments(
                    _branch.BranchId)
                .Select(d =>
                    new
                    {
                        ID =
                            d.DepartmentId,

                        Bölüm =
                            d.Name,

                        Açıklama =
                            d.Description
                    }));
    }


    private void AddDepartment(
        DataGridView grid)
    {
        using var form =
            new DepartmentEditForm(
                new List<Branch>
                {
                    _branch
                });


        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }


        try
        {
            _managerService
                .AddDepartment(
                    _branch.BranchId,
                    form.DepartmentName,
                    form.Description);


            LoadDepartments(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditDepartment(
        DataGridView grid)
    {
        try
        {
            int id =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            var department =
                _managerService
                    .GetDepartments(
                        _branch.BranchId)
                    .First(d =>
                        d.DepartmentId ==
                        id);


            using var form =
                new DepartmentEditForm(
                    new List<Branch>
                    {
                        _branch
                    },
                    department);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _managerService
                .UpdateDepartment(
                    id,
                    form.DepartmentName,
                    form.Description);


            LoadDepartments(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteDepartment(
        DataGridView grid)
    {
        try
        {
            int id =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            if (!ConfirmDelete())
                return;


            _managerService
                .DeleteDepartment(
                    id);


            LoadDepartments(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // DOKTORLAR
    // =====================================================

    private TabPage CreateDoctorsTab()
    {
        var grid =
            UiTableHelper.CreateGrid();


        var page =
            CreateSearchPage(
                "Doktorlar",
                grid,
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni Doktor",
                () =>
                    AddDoctor(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                () =>
                    EditDoctor(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                () =>
                    DeleteDoctor(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Hizmet Ata",
                () =>
                    AssignDoctorService(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Çalışma Saati",
                () =>
                    ManageDoctorSchedule(
                        grid)));


        LoadDoctors(
            grid);


        return page;
    }


    private void LoadDoctors(
        DataGridView grid)
    {
        var doctors =
            _employeeService
                .GetEmployees(
                    _branch.BranchId)
                .OfType<Doctor>()
                .ToList();


        var services =
            _employeeService
                .GetDoctorServices();


        var schedules =
            _employeeService
                .GetSchedules();


        UiTableHelper.Bind(
            grid,
            doctors.Select(d =>
                new
                {
                    ID =
                        d.UserId,

                    Ad =
                        d.FirstName,

                    Soyad =
                        d.LastName,

                    Bölüm =
                        d.Department?.Name,

                    Uzmanlık =
                        d.Specialization,

                    Hizmet =
                        services
                            .Where(ds =>
                                ds.DoctorId ==
                                d.UserId)
                            .Select(ds =>
                                ds.HealthService.Name)
                            .DefaultIfEmpty(
                                "Atanmamış")
                            .Aggregate(
                                (a, b) =>
                                    $"{a}, {b}"),

                    Program =
                        schedules.Any(s =>
                            s.DoctorId ==
                            d.UserId)
                            ? "Var"
                            : "Atanmamış",

                    Eposta =
                        d.Email
                }));
    }


    private void AddDoctor(
        DataGridView grid)
    {
        using var form =
            new DoctorEditForm(
                _managerService,
                null,
                _branch);


        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }


        try
        {
            _employeeService
                .AddDoctor(
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.Password,
                    form.EmployeeNumber,
                    form.HireDate,
                    _branch.BranchId,
                    form.SelectedDepartment.DepartmentId,
                    form.Specialization);


            LoadDoctors(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditDoctor(
        DataGridView grid)
    {
        try
        {
            int doctorId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            var doctor =
                _employeeService
                    .GetEmployees(
                        _branch.BranchId)
                    .OfType<Doctor>()
                    .First(d =>
                        d.UserId ==
                        doctorId);


            using var form =
                new DoctorEditForm(
                    _managerService,
                    doctor,
                    _branch);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _employeeService
                .UpdateDoctor(
                    doctorId,
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.EmployeeNumber,
                    form.HireDate,
                    _branch.BranchId,
                    form.SelectedDepartment.DepartmentId,
                    form.Specialization);


            LoadDoctors(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteDoctor(
        DataGridView grid)
    {
        try
        {
            int doctorId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            if (!ConfirmDelete())
                return;


            _employeeService
                .DeleteEmployee(
                    doctorId);


            LoadDoctors(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AssignDoctorService(
        DataGridView grid)
    {
        try
        {
            int doctorId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            var branchServiceIds =
                _managerService
                    .GetBranchServices(
                        _branch.BranchId)
                    .Where(bs =>
                        bs.IsAvailable)
                    .Select(bs =>
                        bs.HealthServiceId)
                    .ToHashSet();


            var services =
                _managerService
                    .GetHealthServices()
                    .Where(s =>
                        branchServiceIds.Contains(
                            s.HealthServiceId))
                    .ToList();


            var service =
                PromptDialog.Choose(
                    "Doktora Hizmet Ata",
                    "Bu şubedeki hizmet:",
                    services,
                    s => s.Name);


            if (service == null)
                return;


            _employeeService
                .AssignService(
                    doctorId,
                    service.HealthServiceId);


            LoadDoctors(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ManageDoctorSchedule(
        DataGridView grid)
    {
        try
        {
            int doctorId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            var doctor =
                _employeeService
                    .GetEmployees(
                        _branch.BranchId)
                    .OfType<Doctor>()
                    .First(d =>
                        d.UserId ==
                        doctorId);


            using var form =
                new ScheduleEditForm(
                    new List<Doctor>
                    {
                        doctor
                    });


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _employeeService
                .AddSchedule(
                    doctorId,
                    form.SelectedDay,
                    form.StartTime,
                    form.EndTime);


            LoadDoctors(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // PERSONEL
    // =====================================================

    private TabPage CreateStaffTab()
    {
        var grid =
            UiTableHelper.CreateGrid();


        var page =
            CreateSearchPage(
                "Sağlık Personeli",
                grid,
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni Personel",
                () =>
                    AddStaff(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                () =>
                    EditStaff(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                () =>
                    DeleteStaff(
                        grid)));


        LoadStaff(
            grid);


        return page;
    }


    private void LoadStaff(
        DataGridView grid)
    {
        UiTableHelper.Bind(
            grid,
            _employeeService
                .GetEmployees(
                    _branch.BranchId)
                .OfType<HealthStaff>()
                .Select(s =>
                    new
                    {
                        ID =
                            s.UserId,

                        Ad =
                            s.FirstName,

                        Soyad =
                            s.LastName,

                        Bölüm =
                            s.Department?.Name,

                        Pozisyon =
                            s.Position,

                        Eposta =
                            s.Email,

                        Telefon =
                            s.Phone
                    }));
    }


    private void AddStaff(
        DataGridView grid)
    {
        using var form =
            new HealthStaffEditForm(
                _managerService,
                null,
                _branch);


        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }


        try
        {
            _employeeService
                .AddHealthStaff(
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.Password,
                    form.EmployeeNumber,
                    form.HireDate,
                    _branch.BranchId,
                    form.SelectedDepartment?
                        .DepartmentId,
                    form.Position);


            LoadStaff(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditStaff(
        DataGridView grid)
    {
        try
        {
            int id =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            var staff =
                _employeeService
                    .GetEmployees(
                        _branch.BranchId)
                    .OfType<HealthStaff>()
                    .First(s =>
                        s.UserId ==
                        id);


            using var form =
                new HealthStaffEditForm(
                    _managerService,
                    staff,
                    _branch);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _employeeService
                .UpdateHealthStaff(
                    id,
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.EmployeeNumber,
                    form.HireDate,
                    _branch.BranchId,
                    form.SelectedDepartment?
                        .DepartmentId,
                    form.Position);


            LoadStaff(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteStaff(
        DataGridView grid)
    {
        try
        {
            int id =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            if (!ConfirmDelete())
                return;


            _employeeService
                .DeleteEmployee(
                    id);


            LoadStaff(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // HİZMETLER
    // =====================================================

    private TabPage CreateServicesTab()
    {
        var grid =
            UiTableHelper.CreateGrid();


        var page =
            CreateSearchPage(
                "Şube Hizmetleri",
                grid,
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Hizmet Ata",
                () =>
                    AddBranchService(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                () =>
                    EditBranchService(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Kaldır",
                () =>
                    DeleteBranchService(
                        grid)));


        LoadBranchServices(
            grid);


        return page;
    }


    private void LoadBranchServices(
        DataGridView grid)
    {
        UiTableHelper.Bind(
            grid,
            _managerService
                .GetBranchServices(
                    _branch.BranchId)
                .Select(bs =>
                    new
                    {
                        ID =
                            bs.HealthServiceId,

                        Hizmet =
                            bs.HealthService.Name,

                        Süre =
                            bs.HealthService
                                .DurationMinutes,

                        Fiyat =
                            bs.Price,

                        Durum =
                            bs.IsAvailable
                                ? "Aktif"
                                : "Pasif"
                    }));
    }


    private void AddBranchService(
        DataGridView grid)
    {
        try
        {
            var currentIds =
                _managerService
                    .GetBranchServices(
                        _branch.BranchId)
                    .Select(bs =>
                        bs.HealthServiceId)
                    .ToHashSet();


            var available =
                _managerService
                    .GetHealthServices()
                    .Where(s =>
                        !currentIds.Contains(
                            s.HealthServiceId))
                    .ToList();


            var service =
                PromptDialog.Choose(
                    "Şubeye Hizmet Ata",
                    "Hizmet:",
                    available,
                    s => s.Name);


            if (service == null)
                return;


            decimal? price =
                PromptDialog.AskDecimal(
                    "Hizmet Fiyatı",
                    "Fiyat:");


            if (price == null)
                return;


            _managerService
                .AssignServiceToBranch(
                    _branch.BranchId,
                    service.HealthServiceId,
                    price.Value);


            LoadBranchServices(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditBranchService(
        DataGridView grid)
    {
        try
        {
            int serviceId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            var relation =
                _managerService
                    .GetBranchServices(
                        _branch.BranchId)
                    .First(bs =>
                        bs.HealthServiceId ==
                        serviceId);


            decimal? price =
                PromptDialog.AskDecimal(
                    "Hizmet Fiyatı",
                    "Yeni fiyat:");


            if (price == null)
                return;


            bool active =
                MessageBox.Show(
                    "Hizmet aktif olsun mu?",
                    "Hizmet Durumu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                ==
                DialogResult.Yes;


            _managerService
                .UpdateBranchService(
                    _branch.BranchId,
                    serviceId,
                    price.Value,
                    active);


            LoadBranchServices(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteBranchService(
        DataGridView grid)
    {
        try
        {
            int serviceId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            if (!ConfirmDelete())
                return;


            _managerService
                .RemoveServiceFromBranch(
                    _branch.BranchId,
                    serviceId);


            LoadBranchServices(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // EKİPMANLAR
    // =====================================================

    private TabPage CreateEquipmentTab()
    {
        var grid =
            UiTableHelper.CreateGrid();


        var page =
            CreateSearchPage(
                "Şube Ekipmanları",
                grid,
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Ekipman Ekle",
                () =>
                    AddBranchEquipment(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                () =>
                    EditBranchEquipment(
                        grid)));

        actions.Controls.Add(
            ActionButton(
                "Kaldır",
                () =>
                    DeleteBranchEquipment(
                        grid)));


        LoadBranchEquipment(
            grid);


        return page;
    }


    private void LoadBranchEquipment(
        DataGridView grid)
    {
        UiTableHelper.Bind(
            grid,
            _managerService
                .GetBranchEquipment(
                    _branch.BranchId)
                .Select(be =>
                    new
                    {
                        ID =
                            be.EquipmentId,

                        Ekipman =
                            be.Equipment.Name,

                        Adet =
                            be.Quantity,

                        Durum =
                            be.IsOperational
                                ? "Çalışıyor"
                                : "Arızalı"
                    }));
    }


    private void AddBranchEquipment(
        DataGridView grid)
    {
        try
        {
            var assignedIds =
                _managerService
                    .GetBranchEquipment(
                        _branch.BranchId)
                    .Select(be =>
                        be.EquipmentId)
                    .ToHashSet();


            var equipment =
                PromptDialog.Choose(
                    "Şubeye Ekipman Ekle",
                    "Ekipman türü:",
                    _managerService
                        .GetEquipment()
                        .Where(e =>
                            !assignedIds.Contains(
                                e.EquipmentId))
                        .ToList(),
                    e => e.Name);


            if (equipment == null)
                return;


            int? quantity =
                PromptDialog.AskInt(
                    "Ekipman Adedi",
                    "Adet:");


            if (quantity == null)
                return;


            _managerService
                .AssignEquipmentToBranch(
                    _branch.BranchId,
                    equipment.EquipmentId,
                    quantity.Value);


            LoadBranchEquipment(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditBranchEquipment(
        DataGridView grid)
    {
        try
        {
            int equipmentId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            int? quantity =
                PromptDialog.AskInt(
                    "Ekipman Adedi",
                    "Yeni adet:");


            if (quantity == null)
                return;


            bool operational =
                MessageBox.Show(
                    "Ekipman çalışır durumda mı?",
                    "Ekipman Durumu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                ==
                DialogResult.Yes;


            _managerService
                .UpdateBranchEquipment(
                    _branch.BranchId,
                    equipmentId,
                    quantity.Value,
                    operational);


            LoadBranchEquipment(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteBranchEquipment(
        DataGridView grid)
    {
        try
        {
            int equipmentId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            if (!ConfirmDelete())
                return;


            _managerService
                .RemoveEquipmentFromBranch(
                    _branch.BranchId,
                    equipmentId);


            LoadBranchEquipment(
                grid);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // RANDEVULAR
    // =====================================================

    private TabPage CreateAppointmentsTab()
    {
        var grid =
            UiTableHelper.CreateGrid();


        var page =
            CreateSearchPage(
                "Randevular",
                grid,
                out _);


        UiTableHelper.Bind(
            grid,
            _managerService
                .GetBranchAppointments(
                    _branch.BranchId)
                .Select(a =>
                    new
                    {
                        ID =
                            a.AppointmentId,

                        Tarih =
                            a.StartDateTime,

                        Hasta =
                            $"{a.Patient.FirstName} " +
                            $"{a.Patient.LastName}",

                        Doktor =
                            $"{a.Doctor.FirstName} " +
                            $"{a.Doctor.LastName}",

                        Hizmet =
                            a.HealthService.Name,

                        Durum =
                            StatusText(
                                a.Status),

                        Fiyat =
                            a.Price
                    }));


        return page;
    }


    // =====================================================
    // ORTAK UI
    // =====================================================

    private TabPage CreateSearchPage(
        string title,
        DataGridView grid,
        out FlowLayoutPanel actions)
    {
        var page =
            new TabPage(
                title);


        var root =
            new Panel
            {
                Dock =
                    DockStyle.Fill,

                Padding =
                    new Padding(20),

                BackColor =
                    Color.White
            };


        var top =
            new Panel
            {
                Dock =
                    DockStyle.Top,

                Height =
                    100,

                BackColor =
                    Color.White
            };


        var searchLabel =
            new Label
            {
                Text =
                    "Ara",

                AutoSize =
                    true,

                Left =
                    5,

                Top =
                    8
            };


        var search =
            new TextBox
            {
                Left =
                    5,

                Top =
                    32,

                Width =
                    350,

                PlaceholderText =
                    "Tabloda ara..."
            };


        actions =
            new FlowLayoutPanel
            {
                Left =
                    380,

                Top =
                    20,

                Height =
                    55,

                Width =
                    850,

                Anchor =
                    AnchorStyles.Left |
                    AnchorStyles.Right |
                    AnchorStyles.Top,

                FlowDirection =
                    FlowDirection.LeftToRight,

                WrapContents =
                    false
            };


        top.Controls.Add(
            searchLabel);

        top.Controls.Add(
            search);

        top.Controls.Add(
            actions);


        grid.Dock =
            DockStyle.Fill;


        root.Controls.Add(
            grid);

        root.Controls.Add(
            top);


        UiTableHelper.AttachSearch(
            search,
            grid);


        page.Controls.Add(
            root);


        return page;
    }


    private static Button ActionButton(
        string text,
        Action action)
    {
        var button =
            UiTheme.CreateButton(
                text,
                145);


        button.Click +=
            (_, _) =>
                action();


        return button;
    }


    private static bool ConfirmDelete()
    {
        return MessageBox.Show(
            "Seçili kayıt kaldırılsın mı?",
            "Onay",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning)
            ==
            DialogResult.Yes;
    }


    private static void AddInfo(
        Control parent,
        string title,
        string value,
        ref int y)
    {
        var titleLabel =
            new Label
            {
                Text =
                    $"{title}:",

                Font =
                    new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold),

                AutoSize =
                    true,

                Left =
                    25,

                Top =
                    y
            };


        var valueLabel =
            new Label
            {
                Text =
                    value,

                AutoSize =
                    true,

                Left =
                    180,

                Top =
                    y
            };


        parent.Controls.Add(
            titleLabel);

        parent.Controls.Add(
            valueLabel);


        y +=
            42;
    }


    private static string StatusText(
        AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Pending =>
                "Beklemede",

            AppointmentStatus.Approved =>
                "Onaylandı",

            AppointmentStatus.Rejected =>
                "Reddedildi",

            AppointmentStatus.Cancelled =>
                "İptal Edildi",

            AppointmentStatus.Completed =>
                "Tamamlandı",

            _ =>
                "Bilinmiyor"
        };
    }


    private static void ShowError(
        Exception ex)
    {
        MessageBox.Show(
            ex.Message,
            "Hata",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}