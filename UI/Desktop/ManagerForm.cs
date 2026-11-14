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

public class ManagerForm : DesktopShellForm
{
    private readonly IManagerService _managerService;
    private readonly IEmployeeService _employeeService;

    public ManagerForm(
        User user,
        IManagerService managerService,
        IEmployeeService employeeService,
        IUserAccountService accountService)
        : base(user, accountService, "Yönetici")
    {
        _managerService = managerService;
        _employeeService = employeeService;

        AddSidebarButton("Genel Bakış", 160, ShowOverview);
        AddSidebarButton("Şube Yönetimi", 210, ShowBranchManagement);
        AddSidebarButton("Veri Görüntüleme", 260, ShowDataExplorer);
        AddSidebarButton("Profilim", 310, OpenProfile);

        AddLogoutButton();

        ShowOverview();
    }

    // Genel bakış

    private void ShowOverview()
    {
        var employees = _employeeService.GetEmployees();
        var branches = _managerService.GetBranches();

        int appointmentCount = branches.Sum(branch =>
            _managerService.GetBranchAppointments(branch.BranchId).Count);

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(25),
            BackColor = UiTheme.Background
        };

        AddInfoCard(panel, "Şubeler", branches.Count);
        AddInfoCard(panel, "Doktorlar", employees.OfType<Doctor>().Count());
        AddInfoCard(panel, "Sağlık Personeli", employees.OfType<HealthStaff>().Count());
        AddInfoCard(panel, "Randevular", appointmentCount);

        ShowPage("Genel Bakış", panel);
    }

    // Şube yönetimi

    private void ShowBranchManagement()
    {
        var root = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            BackColor = Color.White
        };

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = Color.White
        };

        var branchLabel = new Label
        {
            Text = "Şube",
            AutoSize = true,
            Left = 5,
            Top = 8,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        var branchCombo = new ComboBox
        {
            Left = 5,
            Top = 32,
            Width = 330,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var newBranch = ActionButton("Yeni Şube", AddBranch);
        newBranch.Left = 365;
        newBranch.Top = 27;

        var editBranch = ActionButton("Şubeyi Düzenle", () =>
        {
            if (branchCombo.SelectedItem is Branch branch)
            {
                EditBranch(branch);
            }
        });
        editBranch.Left = 520;
        editBranch.Top = 27;

        var deleteBranch = ActionButton("Şubeyi Sil", () =>
        {
            if (branchCombo.SelectedItem is Branch branch)
            {
                DeleteBranch(branch);
            }
        });
        deleteBranch.Left = 675;
        deleteBranch.Top = 27;

        header.Controls.Add(branchLabel);
        header.Controls.Add(branchCombo);
        header.Controls.Add(newBranch);
        header.Controls.Add(editBranch);
        header.Controls.Add(deleteBranch);

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill
        };

        root.Controls.Add(tabs);
        root.Controls.Add(header);

        void LoadBranches()
        {
            var branches = _managerService
                .GetBranches()
                .OrderBy(b => b.BranchId)
                .ToList();

            branchCombo.DataSource = null;
            branchCombo.DataSource = branches;
            branchCombo.DisplayMember = nameof(Branch.Name);
            branchCombo.ValueMember = nameof(Branch.BranchId);

            if (branches.Count == 0)
            {
                tabs.TabPages.Clear();
            }
        }

        branchCombo.SelectedIndexChanged += (_, _) =>
        {
            if (branchCombo.SelectedItem is Branch branch)
            {
                BuildBranchTabs(tabs, branch);
            }
        };

        LoadBranches();

        ShowPage("Şube Yönetimi", root);
    }

    private void BuildBranchTabs(TabControl tabs, Branch branch)
    {
        tabs.TabPages.Clear();

        tabs.TabPages.Add(CreateBranchGeneralTab(branch));
        tabs.TabPages.Add(CreateDepartmentsTab(branch));
        tabs.TabPages.Add(CreateDoctorsTab(branch));
        tabs.TabPages.Add(CreateStaffTab(branch));
        tabs.TabPages.Add(CreateServicesTab(branch));
        tabs.TabPages.Add(CreateEquipmentTab(branch));
        tabs.TabPages.Add(CreateAppointmentsTab(branch));
    }

    // Şube - genel

    private TabPage CreateBranchGeneralTab(Branch branch)
    {
        var page = new TabPage("Genel");

        var root = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(35),
            BackColor = Color.White
        };

        var title = new Label
        {
            Text = branch.Name,
            Left = 35,
            Top = 30,
            AutoSize = true,
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = UiTheme.Primary
        };

        var location = new Label
        {
            Text = $"{branch.City} / {branch.District}",
            Left = 38,
            Top = 80,
            AutoSize = true,
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.DimGray
        };

        root.Controls.Add(title);
        root.Controls.Add(location);

        int y = 140;

        AddBranchInfo(root, "Adres", branch.Address, ref y);
        AddBranchInfo(root, "Telefon", branch.Phone, ref y);
        AddBranchInfo(root, "Çalışma Saatleri", $"{branch.OpeningTime:HH\\:mm} - {branch.ClosingTime:HH\\:mm}", ref y);

        y += 20;

        var divider = new Panel
        {
            Left = 35,
            Top = y,
            Width = 760,
            Height = 1,
            BackColor = Color.Gainsboro
        };

        root.Controls.Add(divider);

        y += 30;

        var employees = _employeeService.GetEmployees(branch.BranchId);

        AddBranchInfo(root, "Bölüm Sayısı", _managerService.GetDepartments(branch.BranchId).Count.ToString(), ref y);
        AddBranchInfo(root, "Doktor Sayısı", employees.OfType<Doctor>().Count().ToString(), ref y);
        AddBranchInfo(root, "Sağlık Personeli", employees.OfType<HealthStaff>().Count().ToString(), ref y);
        AddBranchInfo(root, "Aktif Hizmetler", _managerService.GetBranchServices(branch.BranchId).Count(bs => bs.IsAvailable).ToString(), ref y);
        AddBranchInfo(root, "Ekipman Türleri", _managerService.GetBranchEquipment(branch.BranchId).Count.ToString(), ref y);
        AddBranchInfo(root, "Toplam Randevu", _managerService.GetBranchAppointments(branch.BranchId).Count.ToString(), ref y);

        page.Controls.Add(root);

        return page;
    }

    // Şube - bölümler

    private TabPage CreateDepartmentsTab(Branch branch)
    {
        var grid = UiTableHelper.CreateGrid();
        var page = CreateSearchTab("Bölümler", grid, out var actions);

        void Load()
        {
            UiTableHelper.Bind(
                grid,
                _managerService
                    .GetDepartments(branch.BranchId)
                    .Select(d => new
                    {
                        ID = d.DepartmentId,
                        Bölüm = d.Name,
                        Açıklama = d.Description
                    }));
        }

        actions.Controls.Add(ActionButton("Yeni", () =>
        {
            using var form = new DepartmentEditForm(new List<Branch> { branch });

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _managerService.AddDepartment(
                    branch.BranchId,
                    form.DepartmentName,
                    form.Description);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Düzenle", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                var department = _managerService
                    .GetDepartments(branch.BranchId)
                    .First(d => d.DepartmentId == id);

                using var form = new DepartmentEditForm(
                    new List<Branch> { branch },
                    department);

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _managerService.UpdateDepartment(
                    id,
                    form.DepartmentName,
                    form.Description);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Sil", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                if (!ConfirmDelete())
                {
                    return;
                }

                _managerService.DeleteDepartment(id);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        Load();

        return page;
    }

    // Şube - doktorlar

    private TabPage CreateDoctorsTab(Branch branch)
    {
        var grid = UiTableHelper.CreateGrid();
        var page = CreateSearchTab("Doktorlar", grid, out var actions);

        void Load()
        {
            var doctors = _employeeService
                .GetEmployees(branch.BranchId)
                .OfType<Doctor>()
                .ToList();

            var doctorServices = _employeeService.GetDoctorServices();
            var schedules = _employeeService.GetSchedules();

            UiTableHelper.Bind(
                grid,
                doctors.Select(d => new
                {
                    ID = d.UserId,
                    Ad = d.FirstName,
                    Soyad = d.LastName,
                    Telefon = d.Phone,
                    Eposta = d.Email,
                    ÇalışanNo = d.EmployeeNumber,
                    Bölüm = d.Department?.Name ?? "-",
                    Uzmanlık = d.Specialization,
                    Hizmetler = string.Join(
                        ", ",
                        doctorServices
                            .Where(ds => ds.DoctorId == d.UserId)
                            .Select(ds => ds.HealthService.Name)),
                    ÇalışmaSaatleri = string.Join(
                        " | ",
                        schedules
                            .Where(s => s.DoctorId == d.UserId)
                            .OrderBy(s => s.DayOfWeek)
                            .ThenBy(s => s.StartTime)
                            .Select(s =>
                                $"{DayText(s.DayOfWeek)} {s.StartTime:HH\\:mm}-{s.EndTime:HH\\:mm}"))
                }));
        }

        actions.Controls.Add(ActionButton("Yeni Doktor", () =>
        {
            using var form = new DoctorEditForm(_managerService, null, branch);

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _employeeService.AddDoctor(
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.Password,
                    form.EmployeeNumber,
                    form.HireDate,
                    branch.BranchId,
                    form.SelectedDepartment.DepartmentId,
                    form.Specialization);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Düzenle", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                var doctor = _employeeService
                    .GetEmployees(branch.BranchId)
                    .OfType<Doctor>()
                    .First(d => d.UserId == id);

                using var form = new DoctorEditForm(_managerService, doctor, branch);

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _employeeService.UpdateDoctor(
                    id,
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.EmployeeNumber,
                    form.HireDate,
                    branch.BranchId,
                    form.SelectedDepartment.DepartmentId,
                    form.Specialization);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Sil", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                if (!ConfirmDelete())
                {
                    return;
                }

                _employeeService.DeleteEmployee(id);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Hizmetler", () =>
        {
            try
            {
                int doctorId = UiTableHelper.SelectedInt(grid, "ID");
                ManageDoctorServices(branch, doctorId, Load);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Çalışma Saatleri", () =>
        {
            try
            {
                int doctorId = UiTableHelper.SelectedInt(grid, "ID");
                ManageDoctorSchedules(branch, doctorId, Load);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        Load();

        return page;
    }

    // Doktor hizmetleri

    private void ManageDoctorServices(Branch branch, int doctorId, Action refreshParent)
    {
        var doctor = _employeeService
            .GetEmployees(branch.BranchId)
            .OfType<Doctor>()
            .First(d => d.UserId == doctorId);

        using var form = new Form
        {
            Text = $"Hizmetler - {doctor.FirstName} {doctor.LastName}",
            Width = 760,
            Height = 520,
            StartPosition = FormStartPosition.CenterParent
        };

        var grid = UiTableHelper.CreateGrid();

        var search = new TextBox
        {
            Dock = DockStyle.Top,
            Height = 35,
            PlaceholderText = "Ara..."
        };

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(5),
            FlowDirection = FlowDirection.LeftToRight
        };

        grid.Dock = DockStyle.Fill;

        form.Controls.Add(grid);
        form.Controls.Add(actions);
        form.Controls.Add(search);

        UiTableHelper.AttachSearch(search, grid);

        void Load()
        {
            UiTableHelper.Bind(
                grid,
                _employeeService
                    .GetDoctorServices(doctorId)
                    .Select(ds => new
                    {
                        ID = ds.HealthServiceId,
                        Hizmet = ds.HealthService.Name
                    }));
        }

        actions.Controls.Add(ActionButton("Hizmet Ekle", () =>
        {
            var assignedIds = _employeeService
                .GetDoctorServices(doctorId)
                .Select(ds => ds.HealthServiceId)
                .ToHashSet();

            var available = _managerService
                .GetBranchServices(branch.BranchId)
                .Where(bs => bs.IsAvailable && !assignedIds.Contains(bs.HealthServiceId))
                .Select(bs => bs.HealthService)
                .ToList();

            if (available.Count == 0)
            {
                MessageBox.Show("Bu doktor için atanabilecek başka aktif hizmet bulunmuyor.");

                return;
            }

            var service = PromptDialog.Choose(
                "Hizmet Ata",
                "Hizmet:",
                available,
                s => s.Name);

            if (service == null)
            {
                return;
            }

            try
            {
                _employeeService.AssignService(doctorId, service.HealthServiceId);

                Load();
                refreshParent();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Kaldır", () =>
        {
            try
            {
                int serviceId = UiTableHelper.SelectedInt(grid, "ID");

                if (!ConfirmDelete())
                {
                    return;
                }

                _employeeService.RemoveService(doctorId, serviceId);

                Load();
                refreshParent();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        Load();

        form.ShowDialog(this);
    }

    // Doktor çalışma saatleri

    private void ManageDoctorSchedules(Branch branch, int doctorId, Action refreshParent)
    {
        var doctor = _employeeService
            .GetEmployees(branch.BranchId)
            .OfType<Doctor>()
            .First(d => d.UserId == doctorId);

        using var form = new Form
        {
            Text = $"Çalışma Saatleri - {doctor.FirstName} {doctor.LastName}",
            Width = 850,
            Height = 550,
            StartPosition = FormStartPosition.CenterParent
        };

        var grid = UiTableHelper.CreateGrid();

        var search = new TextBox
        {
            Dock = DockStyle.Top,
            Height = 35,
            PlaceholderText = "Ara..."
        };

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(5),
            FlowDirection = FlowDirection.LeftToRight
        };

        grid.Dock = DockStyle.Fill;

        form.Controls.Add(grid);
        form.Controls.Add(actions);
        form.Controls.Add(search);

        UiTableHelper.AttachSearch(search, grid);

        void Load()
        {
            UiTableHelper.Bind(
                grid,
                _employeeService
                    .GetSchedules(doctorId)
                    .Select(s => new
                    {
                        ID = s.DoctorScheduleId,
                        Gün = DayText(s.DayOfWeek),
                        Başlangıç = s.StartTime,
                        Bitiş = s.EndTime
                    }));
        }

        actions.Controls.Add(ActionButton("Yeni", () =>
        {
            using var editor = new ScheduleEditForm(new List<Doctor> { doctor });

            if (editor.ShowDialog(form) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _employeeService.AddSchedule(
                    doctorId,
                    editor.SelectedDay,
                    editor.StartTime,
                    editor.EndTime);

                Load();
                refreshParent();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Düzenle", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                var schedule = _employeeService
                    .GetSchedules(doctorId)
                    .First(s => s.DoctorScheduleId == id);

                using var editor = new ScheduleEditForm(
                    new List<Doctor> { doctor },
                    schedule);

                if (editor.ShowDialog(form) != DialogResult.OK)
                {
                    return;
                }

                _employeeService.UpdateSchedule(
                    id,
                    editor.SelectedDay,
                    editor.StartTime,
                    editor.EndTime);

                Load();
                refreshParent();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Sil", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                if (!ConfirmDelete())
                {
                    return;
                }

                _employeeService.DeleteSchedule(id);

                Load();
                refreshParent();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        Load();

        form.ShowDialog(this);
    }

    // Şube - sağlık personeli

    private TabPage CreateStaffTab(Branch branch)
    {
        var grid = UiTableHelper.CreateGrid();
        var page = CreateSearchTab("Sağlık Personeli", grid, out var actions);

        void Load()
        {
            UiTableHelper.Bind(
                grid,
                _employeeService
                    .GetEmployees(branch.BranchId)
                    .OfType<HealthStaff>()
                    .Select(s => new
                    {
                        ID = s.UserId,
                        Ad = s.FirstName,
                        Soyad = s.LastName,
                        Telefon = s.Phone,
                        Eposta = s.Email,
                        ÇalışanNo = s.EmployeeNumber,
                        Bölüm = s.Department?.Name ?? "-",
                        Pozisyon = s.Position,
                        İşeGiriş = s.HireDate
                    }));
        }

        actions.Controls.Add(ActionButton("Yeni Personel", () =>
        {
            using var form = new HealthStaffEditForm(_managerService, null, branch);

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _employeeService.AddHealthStaff(
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.Password,
                    form.EmployeeNumber,
                    form.HireDate,
                    branch.BranchId,
                    form.SelectedDepartment?.DepartmentId,
                    form.Position);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Düzenle", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                var staff = _employeeService
                    .GetEmployees(branch.BranchId)
                    .OfType<HealthStaff>()
                    .First(s => s.UserId == id);

                using var form = new HealthStaffEditForm(_managerService, staff, branch);

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _employeeService.UpdateHealthStaff(
                    id,
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.EmployeeNumber,
                    form.HireDate,
                    branch.BranchId,
                    form.SelectedDepartment?.DepartmentId,
                    form.Position);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Sil", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                if (!ConfirmDelete())
                {
                    return;
                }

                _employeeService.DeleteEmployee(id);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        Load();

        return page;
    }

    // Şube - hizmetler

    private TabPage CreateServicesTab(Branch branch)
    {
        var grid = UiTableHelper.CreateGrid();
        var page = CreateSearchTab("Hizmetler", grid, out var actions);

        void Load()
        {
            UiTableHelper.Bind(
                grid,
                _managerService
                    .GetBranchServices(branch.BranchId)
                    .Select(bs => new
                    {
                        ID = bs.HealthServiceId,
                        Hizmet = bs.HealthService.Name,
                        Açıklama = bs.HealthService.Description,
                        Süre = bs.HealthService.DurationMinutes,
                        Fiyat = bs.Price,
                        Durum = bs.IsAvailable ? "Aktif" : "Pasif"
                    }));
        }

        actions.Controls.Add(ActionButton("Yeni Hizmet", () =>
        {
            using var form = new BranchServiceEditForm();

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _managerService.AddHealthServiceToBranch(
                    branch.BranchId,
                    form.ServiceName,
                    form.Description,
                    form.Duration,
                    form.Price,
                    form.IsActive);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Düzenle", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                var relation = _managerService
                    .GetBranchServices(branch.BranchId)
                    .First(bs => bs.HealthServiceId == id);

                using var form = new BranchServiceEditForm(
                    relation.HealthService,
                    relation.Price,
                    relation.IsAvailable);

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _managerService.UpdateBranchServiceDetails(
                    branch.BranchId,
                    id,
                    form.ServiceName,
                    form.Description,
                    form.Duration,
                    form.Price,
                    form.IsActive);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Kaldır", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                if (!ConfirmDelete())
                {
                    return;
                }

                _managerService.RemoveServiceFromBranch(branch.BranchId, id);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        Load();

        return page;
    }

    // Şube - ekipman

    private TabPage CreateEquipmentTab(Branch branch)
    {
        var grid = UiTableHelper.CreateGrid();
        var page = CreateSearchTab("Ekipmanlar", grid, out var actions);

        void Load()
        {
            UiTableHelper.Bind(
                grid,
                _managerService
                    .GetBranchEquipment(branch.BranchId)
                    .Select(be => new
                    {
                        ID = be.EquipmentId,
                        Ekipman = be.Equipment.Name,
                        Açıklama = be.Equipment.Description,
                        Adet = be.Quantity,
                        Durum = be.IsOperational ? "Çalışıyor" : "Arızalı"
                    }));
        }

        actions.Controls.Add(ActionButton("Yeni Ekipman", () =>
        {
            using var form = new BranchEquipmentEditForm();

            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                _managerService.AddEquipmentToBranch(
                    branch.BranchId,
                    form.EquipmentName,
                    form.Description,
                    form.Quantity,
                    form.IsOperational);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Düzenle", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                var relation = _managerService
                    .GetBranchEquipment(branch.BranchId)
                    .First(be => be.EquipmentId == id);

                using var form = new BranchEquipmentEditForm(
                    relation.Equipment,
                    relation.Quantity,
                    relation.IsOperational);

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _managerService.UpdateBranchEquipmentDetails(
                    branch.BranchId,
                    id,
                    form.EquipmentName,
                    form.Description,
                    form.Quantity,
                    form.IsOperational);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        actions.Controls.Add(ActionButton("Kaldır", () =>
        {
            try
            {
                int id = UiTableHelper.SelectedInt(grid, "ID");

                if (!ConfirmDelete())
                {
                    return;
                }

                _managerService.RemoveEquipmentFromBranch(branch.BranchId, id);

                Load();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }));

        Load();

        return page;
    }

    // Şube - randevular

    private TabPage CreateAppointmentsTab(Branch branch)
    {
        var grid = UiTableHelper.CreateGrid();
        var page = CreateSearchTab("Randevular", grid, out _);

        UiTableHelper.Bind(
            grid,
            _managerService
                .GetBranchAppointments(branch.BranchId)
                .Select(a => new
                {
                    ID = a.AppointmentId,
                    Tarih = a.StartDateTime,
                    Hasta = $"{a.Patient.FirstName} {a.Patient.LastName}",
                    HastaTelefon = a.Patient.Phone,
                    Doktor = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
                    Bölüm = a.Doctor.Department?.Name ?? "-",
                    Hizmet = a.HealthService.Name,
                    Başlangıç = a.StartDateTime,
                    Bitiş = a.EndDateTime,
                    Fiyat = a.Price,
                    Durum = AppointmentStatusText(a.Status)
                }));

        return page;
    }

    // Veri görüntüleme

    private void ShowDataExplorer()
    {
        var root = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            BackColor = Color.White
        };

        var top = new Panel
        {
            Dock = DockStyle.Top,
            Height = 125,
            BackColor = Color.White
        };

        var typeCombo = new ComboBox
        {
            Left = 5,
            Top = 28,
            Width = 210,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        typeCombo.Items.AddRange(
        [
            "Şubeler",
            "Bölümler",
            "Doktorlar",
            "Sağlık Personeli",
            "Tüm Çalışanlar",
            "Hizmetler",
            "Ekipmanlar",
            "Randevular"
        ]);

        var search = new TextBox
        {
            Left = 235,
            Top = 28,
            Width = 285,
            PlaceholderText = "Tüm kolonlarda ara..."
        };

        var filterColumn = new ComboBox
        {
            Left = 540,
            Top = 28,
            Width = 190,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var filterValue = new TextBox
        {
            Left = 750,
            Top = 28,
            Width = 235,
            PlaceholderText = "Filtre değeri..."
        };

        AddTopLabel(top, "Veri Türü", 5);
        AddTopLabel(top, "Arama", 235);
        AddTopLabel(top, "Filtre Kolonu", 540);
        AddTopLabel(top, "Filtre", 750);

        top.Controls.Add(typeCombo);
        top.Controls.Add(search);
        top.Controls.Add(filterColumn);
        top.Controls.Add(filterValue);

        var hint = new Label
        {
            Text = "Kolon başlıklarına tıklayarak artan / azalan sıralayabilirsiniz. Tablolar başlangıçta ID'ye göre artan sıralanır.",
            Left = 5,
            Top = 82,
            AutoSize = true,
            ForeColor = Color.DimGray
        };

        top.Controls.Add(hint);

        var grid = UiTableHelper.CreateGrid();
        grid.Dock = DockStyle.Fill;

        root.Controls.Add(grid);
        root.Controls.Add(top);

        UiTableHelper.AttachSearchAndFilter(
            search,
            filterColumn,
            filterValue,
            grid);

        void LoadData()
        {
            string selected = typeCombo.SelectedItem?.ToString() ?? "Şubeler";

            switch (selected)
            {
                case "Şubeler":
                    UiTableHelper.Bind(
                        grid,
                        _managerService
                            .GetBranches()
                            .Select(b => new
                            {
                                ID = b.BranchId,
                                Şube = b.Name,
                                Şehir = b.City,
                                İlçe = b.District,
                                Adres = b.Address,
                                Telefon = b.Phone,
                                Açılış = b.OpeningTime,
                                Kapanış = b.ClosingTime
                            }));
                    break;

                case "Bölümler":
                    UiTableHelper.Bind(
                        grid,
                        _managerService
                            .GetBranches()
                            .SelectMany(b =>
                                _managerService
                                    .GetDepartments(b.BranchId)
                                    .Select(d => new
                                    {
                                        ID = d.DepartmentId,
                                        Şehir = b.City,
                                        Şube = b.Name,
                                        Bölüm = d.Name,
                                        Açıklama = d.Description
                                    })));
                    break;

                case "Doktorlar":
                    var doctorServices = _employeeService.GetDoctorServices();
                    var schedules = _employeeService.GetSchedules();

                    UiTableHelper.Bind(
                        grid,
                        _employeeService
                            .GetEmployees()
                            .OfType<Doctor>()
                            .Select(d => new
                            {
                                ID = d.UserId,
                                Ad = d.FirstName,
                                Soyad = d.LastName,
                                Telefon = d.Phone,
                                Eposta = d.Email,
                                ÇalışanNo = d.EmployeeNumber,
                                Şehir = d.Branch.City,
                                Şube = d.Branch.Name,
                                Bölüm = d.Department?.Name ?? "-",
                                Uzmanlık = d.Specialization,
                                Hizmetler = string.Join(
                                    ", ",
                                    doctorServices
                                        .Where(ds => ds.DoctorId == d.UserId)
                                        .Select(ds => ds.HealthService.Name)),
                                ÇalışmaSaatleri = string.Join(
                                    " | ",
                                    schedules
                                        .Where(s => s.DoctorId == d.UserId)
                                        .OrderBy(s => s.DayOfWeek)
                                        .ThenBy(s => s.StartTime)
                                        .Select(s =>
                                            $"{DayText(s.DayOfWeek)} {s.StartTime:HH\\:mm}-{s.EndTime:HH\\:mm}"))
                            }));
                    break;

                case "Sağlık Personeli":
                    UiTableHelper.Bind(
                        grid,
                        _employeeService
                            .GetEmployees()
                            .OfType<HealthStaff>()
                            .Select(s => new
                            {
                                ID = s.UserId,
                                Ad = s.FirstName,
                                Soyad = s.LastName,
                                Telefon = s.Phone,
                                Eposta = s.Email,
                                ÇalışanNo = s.EmployeeNumber,
                                Şehir = s.Branch.City,
                                Şube = s.Branch.Name,
                                Bölüm = s.Department?.Name ?? "-",
                                Pozisyon = s.Position,
                                İşeGiriş = s.HireDate
                            }));
                    break;

                case "Tüm Çalışanlar":
                    UiTableHelper.Bind(
                        grid,
                        _employeeService
                            .GetEmployees()
                            .Select(e => new
                            {
                                ID = e.UserId,
                                Tür = EmployeeTypeText(e),
                                Ad = e.FirstName,
                                Soyad = e.LastName,
                                Telefon = e.Phone,
                                Eposta = e.Email,
                                ÇalışanNo = e.EmployeeNumber,
                                Şehir = e.Branch.City,
                                Şube = e.Branch.Name,
                                Bölüm = e.Department?.Name ?? "-",
                                İşeGiriş = e.HireDate
                            }));
                    break;

                case "Hizmetler":
                    UiTableHelper.Bind(
                        grid,
                        _managerService
                            .GetBranches()
                            .SelectMany(b =>
                                _managerService
                                    .GetBranchServices(b.BranchId)
                                    .Select(bs => new
                                    {
                                        ID = bs.HealthServiceId,
                                        Şehir = b.City,
                                        Şube = b.Name,
                                        Hizmet = bs.HealthService.Name,
                                        Açıklama = bs.HealthService.Description,
                                        Süre = bs.HealthService.DurationMinutes,
                                        Fiyat = bs.Price,
                                        Durum = bs.IsAvailable ? "Aktif" : "Pasif"
                                    })));
                    break;

                case "Ekipmanlar":
                    UiTableHelper.Bind(
                        grid,
                        _managerService
                            .GetBranches()
                            .SelectMany(b =>
                                _managerService
                                    .GetBranchEquipment(b.BranchId)
                                    .Select(be => new
                                    {
                                        ID = be.EquipmentId,
                                        Şehir = b.City,
                                        Şube = b.Name,
                                        Ekipman = be.Equipment.Name,
                                        Açıklama = be.Equipment.Description,
                                        Adet = be.Quantity,
                                        Durum = be.IsOperational ? "Çalışıyor" : "Arızalı"
                                    })));
                    break;

                case "Randevular":
                    UiTableHelper.Bind(
                        grid,
                        _managerService
                            .GetBranches()
                            .SelectMany(b =>
                                _managerService
                                    .GetBranchAppointments(b.BranchId)
                                    .Select(a => new
                                    {
                                        ID = a.AppointmentId,
                                        Şehir = b.City,
                                        Şube = b.Name,
                                        Hasta = $"{a.Patient.FirstName} {a.Patient.LastName}",
                                        HastaTelefon = a.Patient.Phone,
                                        Doktor = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
                                        Bölüm = a.Doctor.Department?.Name ?? "-",
                                        Hizmet = a.HealthService.Name,
                                        Başlangıç = a.StartDateTime,
                                        Bitiş = a.EndDateTime,
                                        Fiyat = a.Price,
                                        Durum = AppointmentStatusText(a.Status)
                                    })));
                    break;
            }

            UiTableHelper.RefreshFilterColumns(filterColumn, grid);

            search.Clear();
            filterValue.Clear();
        }

        typeCombo.SelectedIndexChanged += (_, _) => LoadData();
        typeCombo.SelectedIndex = 0;

        ShowPage("Veri Görüntüleme", root);
    }

    // Şube CRUD

    private void AddBranch()
    {
        using var form = new BranchEditForm();

        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _managerService.AddBranch(
                1,
                form.BranchName,
                form.City,
                form.District,
                form.Address,
                form.Phone,
                form.Opening,
                form.ClosingTime);

            ShowSuccess("Şube oluşturuldu.");
            ShowBranchManagement();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void EditBranch(Branch branch)
    {
        using var form = new BranchEditForm(branch);

        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _managerService.UpdateBranch(
                branch.BranchId,
                form.BranchName,
                form.City,
                form.District,
                form.Address,
                form.Phone,
                form.Opening,
                form.ClosingTime);

            ShowSuccess("Şube güncellendi.");
            ShowBranchManagement();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void DeleteBranch(Branch branch)
    {
        if (!ConfirmDelete())
        {
            return;
        }

        try
        {
            _managerService.DeleteBranch(branch.BranchId);

            ShowSuccess("Şube silindi.");
            ShowBranchManagement();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private TabPage CreateSearchTab(string title, DataGridView grid, out FlowLayoutPanel actions)
    {
        var page = new TabPage(title);

        var root = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            BackColor = Color.White
        };

        var top = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100,
            BackColor = Color.White
        };

        var search = new TextBox
        {
            Left = 5,
            Top = 32,
            Width = 300,
            PlaceholderText = "Ara..."
        };

        top.Controls.Add(new Label
        {
            Text = "Ara",
            Left = 5,
            Top = 8,
            AutoSize = true
        });

        top.Controls.Add(search);

        actions = new FlowLayoutPanel
        {
            Left = 330,
            Top = 20,
            Width = 850,
            Height = 65,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true
        };

        top.Controls.Add(actions);

        grid.Dock = DockStyle.Fill;

        root.Controls.Add(grid);
        root.Controls.Add(top);

        UiTableHelper.AttachSearch(search, grid);

        page.Controls.Add(root);

        return page;
    }

    private static Button ActionButton(string text, Action action)
    {
        var button = UiTheme.CreateButton(text, 145);
        button.Click += (_, _) => action();
        return button;
    }

    private static bool ConfirmDelete()
    {
        return MessageBox.Show(
            "Seçili kayıt silinsin/kaldırılsın mı?",
            "Onay",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning) == DialogResult.Yes;
    }

    private static void AddInfoCard(FlowLayoutPanel panel, string title, int value)
    {
        var card = new Panel
        {
            Width = 220,
            Height = 130,
            BackColor = Color.White,
            Margin = new Padding(12),
            Padding = new Padding(20)
        };

        var number = new Label
        {
            Text = value.ToString(),
            Font = new Font("Segoe UI", 25, FontStyle.Bold),
            ForeColor = UiTheme.Primary,
            AutoSize = true,
            Left = 20,
            Top = 20
        };

        var label = new Label
        {
            Text = title,
            AutoSize = true,
            Left = 20,
            Top = 82
        };

        card.Controls.Add(number);
        card.Controls.Add(label);
        panel.Controls.Add(card);
    }

    private static void AddBranchInfo(Control parent, string title, string value, ref int y)
    {
        var titleLabel = new Label
        {
            Text = title,
            Left = 38,
            Top = y,
            Width = 180,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.DimGray
        };

        var valueLabel = new Label
        {
            Text = string.IsNullOrWhiteSpace(value) ? "-" : value,
            Left = 235,
            Top = y,
            Width = 560,
            Height = 35,
            AutoSize = false,
            Font = new Font("Segoe UI", 11)
        };

        parent.Controls.Add(titleLabel);
        parent.Controls.Add(valueLabel);

        y += 42;
    }

    private static void AddTopLabel(Control parent, string text, int left)
    {
        parent.Controls.Add(new Label
        {
            Text = text,
            Left = left,
            Top = 5,
            AutoSize = true
        });
    }

    private static string EmployeeTypeText(Employee employee)
    {
        return employee switch
        {
            Doctor => "Doktor",
            Manager => "Yönetici",
            HealthStaff staff => $"Sağlık Personeli ({staff.Position})",
            _ => "Çalışan"
        };
    }

    private static string AppointmentStatusText(AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Pending => "Beklemede",
            AppointmentStatus.Approved => "Onaylandı",
            AppointmentStatus.Rejected => "Reddedildi",
            AppointmentStatus.Cancelled => "İptal Edildi",
            AppointmentStatus.Completed => "Tamamlandı",
            _ => "Bilinmiyor"
        };
    }

    private static string DayText(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "Pazartesi",
            DayOfWeek.Tuesday => "Salı",
            DayOfWeek.Wednesday => "Çarşamba",
            DayOfWeek.Thursday => "Perşembe",
            DayOfWeek.Friday => "Cuma",
            DayOfWeek.Saturday => "Cumartesi",
            DayOfWeek.Sunday => "Pazar",
            _ => "-"
        };
    }
}
