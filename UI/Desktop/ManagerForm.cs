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


    private readonly DataGridView grid =
        CreateGrid();


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


    // =================================================
    // GENEL BAKIŞ
    // =================================================

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
                    new Padding(20),

                BackColor =
                    UiTheme.Background
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
            "Ekipmanlar",
            _managerService
                .GetEquipment()
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


    // =================================================
    // ŞUBELER
    // =================================================

    private void ShowBranches()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni",
                AddBranch));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditBranch));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                DeleteBranch));


        LoadBranches();


        ShowPage(
            "Şubeler",
            panel);
    }


    private void LoadBranches()
    {
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

                        Adres =
                            b.Address,

                        Telefon =
                            b.Phone,

                        Acilis =
                            b.OpeningTime,

                        Kapanis =
                            b.ClosingTime
                    })
                .ToList();
    }


    private void AddBranch()
    {
        using var form =
            new BranchEditForm();


        if (form.ShowDialog(this) !=
            DialogResult.OK)
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
                form.Closing);


            ShowSuccess(
                "Şube eklendi.");

            LoadBranches();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditBranch()
    {
        try
        {
            int id =
                SelectedId();


            var branch =
                _managerService
                    .GetBranches()
                    .First(b =>
                        b.BranchId ==
                        id);


            using var form =
                new BranchEditForm(
                    branch);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _managerService.UpdateBranch(
                id,
                form.BranchName,
                form.City,
                form.District,
                form.Address,
                form.Phone,
                form.Opening,
                form.Closing);


            ShowSuccess(
                "Şube güncellendi.");

            LoadBranches();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteBranch()
    {
        try
        {
            int id =
                SelectedId();


            if (!ConfirmDelete())
                return;


            _managerService
                .DeleteBranch(id);


            LoadBranches();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =================================================
    // BÖLÜMLER
    // =================================================

    private void ShowDepartments()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni",
                AddDepartment));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditDepartment));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                DeleteDepartment));


        LoadDepartments();


        ShowPage(
            "Bölümler",
            panel);
    }


    private void LoadDepartments()
    {
        var branches =
            _managerService
                .GetBranches();


        grid.DataSource =
            branches
                .SelectMany(b =>
                    _managerService
                        .GetDepartments(
                            b.BranchId)
                        .Select(d =>
                            new
                            {
                                ID =
                                    d.DepartmentId,

                                Sube =
                                    b.Name,

                                Bolum =
                                    d.Name,

                                Aciklama =
                                    d.Description
                            }))
                .ToList();
    }


    private void AddDepartment()
    {
        var branches =
            _managerService
                .GetBranches();


        using var form =
            new DepartmentEditForm(
                branches);


        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }


        try
        {
            _managerService
                .AddDepartment(
                    form.SelectedBranch.BranchId,
                    form.DepartmentName,
                    form.Description);


            LoadDepartments();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditDepartment()
    {
        try
        {
            int id =
                SelectedId();


            var branches =
                _managerService
                    .GetBranches();


            Department department =
                branches
                    .SelectMany(b =>
                        _managerService
                            .GetDepartments(
                                b.BranchId))
                    .First(d =>
                        d.DepartmentId ==
                        id);


            using var form =
                new DepartmentEditForm(
                    branches,
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


            LoadDepartments();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteDepartment()
    {
        try
        {
            int id =
                SelectedId();


            if (!ConfirmDelete())
                return;


            _managerService
                .DeleteDepartment(id);


            LoadDepartments();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =================================================
    // HİZMETLER
    // =================================================

    private void ShowServices()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni",
                AddService));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditService));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                DeleteService));

        actions.Controls.Add(
            ActionButton(
                "Şubeye Ata",
                AssignService));


        LoadServices();


        ShowPage(
            "Hizmetler",
            panel);
    }


    private void LoadServices()
    {
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

                        Sure =
                            s.DurationMinutes,

                        Aciklama =
                            s.Description
                    })
                .ToList();
    }


    private void AddService()
    {
        using var form =
            new HealthServiceEditForm();


        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }


        try
        {
            _managerService
                .AddHealthService(
                    form.ServiceName,
                    form.Description,
                    form.Duration);


            LoadServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditService()
    {
        try
        {
            int id =
                SelectedId();


            var service =
                _managerService
                    .GetHealthServices()
                    .First(s =>
                        s.HealthServiceId ==
                        id);


            using var form =
                new HealthServiceEditForm(
                    service);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _managerService
                .UpdateHealthService(
                    id,
                    form.ServiceName,
                    form.Description,
                    form.Duration);


            LoadServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteService()
    {
        try
        {
            int id =
                SelectedId();


            if (!ConfirmDelete())
                return;


            _managerService
                .DeleteHealthService(id);


            LoadServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AssignService()
    {
        try
        {
            var branch =
                PromptDialog.Choose(
                    "Hizmet Ata",
                    "Şube seçiniz:",
                    _managerService
                        .GetBranches(),
                    b => b.Name);


            if (branch == null)
                return;


            var service =
                PromptDialog.Choose(
                    "Hizmet Ata",
                    "Hizmet seçiniz:",
                    _managerService
                        .GetHealthServices(),
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
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =================================================
    // EKİPMANLAR
    // =================================================

    private void ShowEquipment()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni",
                AddEquipment));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditEquipment));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                DeleteEquipment));

        actions.Controls.Add(
            ActionButton(
                "Şubeye Ata",
                AssignEquipment));


        LoadEquipment();


        ShowPage(
            "Ekipmanlar",
            panel);
    }


    private void LoadEquipment()
    {
        grid.DataSource =
            _managerService
                .GetEquipment()
                .Select(e =>
                    new
                    {
                        ID =
                            e.EquipmentId,

                        Ekipman =
                            e.Name,

                        Aciklama =
                            e.Description
                    })
                .ToList();
    }


    private void AddEquipment()
    {
        using var form =
            new EquipmentEditForm();


        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }


        try
        {
            _managerService
                .AddEquipment(
                    form.EquipmentName,
                    form.Description);


            LoadEquipment();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditEquipment()
    {
        try
        {
            int id =
                SelectedId();


            var equipment =
                _managerService
                    .GetEquipment()
                    .First(e =>
                        e.EquipmentId ==
                        id);


            using var form =
                new EquipmentEditForm(
                    equipment);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _managerService
                .UpdateEquipment(
                    id,
                    form.EquipmentName,
                    form.Description);


            LoadEquipment();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteEquipment()
    {
        try
        {
            int id =
                SelectedId();


            if (!ConfirmDelete())
                return;


            _managerService
                .DeleteEquipment(id);


            LoadEquipment();
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
                    "Şube:",
                    _managerService
                        .GetBranches(),
                    b => b.Name);


            if (branch == null)
                return;


            var equipment =
                PromptDialog.Choose(
                    "Ekipman Ata",
                    "Ekipman:",
                    _managerService
                        .GetEquipment(),
                    e => e.Name);


            if (equipment == null)
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
                    equipment.EquipmentId,
                    quantity.Value);


            ShowSuccess(
                "Ekipman şubeye atandı.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =================================================
    // DOKTOR / PERSONEL
    // =================================================

    private void ShowDoctors()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni Doktor",
                AddDoctor));


        LoadDoctors();


        ShowPage(
            "Doktorlar",
            panel);
    }


    private void LoadDoctors()
    {
        grid.DataSource =
            _employeeService
                .GetEmployees()
                .OfType<Doctor>()
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
    }


    private void AddDoctor()
    {
        MessageBox.Show(
            "Doktor için tek-form düzenleme ekranı bir sonraki adımda bağlanacak.");
    }


    private void ShowHealthStaff()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni Personel",
                AddHealthStaff));


        grid.DataSource =
            _employeeService
                .GetEmployees()
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
        MessageBox.Show(
            "Personel için tek-form düzenleme ekranı bir sonraki adımda bağlanacak.");
    }


    private void ShowEmployees()
    {
        var panel =
            CreateCrudPage(
                out _);


        grid.DataSource =
            _employeeService
                .GetEmployees()
                .Select(e =>
                    new
                    {
                        ID =
                            e.UserId,

                        Sube =
                            e.Branch.Name,

                        Ad =
                            e.FirstName,

                        Soyad =
                            e.LastName,

                        Eposta =
                            e.Email,

                        CalisanNo =
                            e.EmployeeNumber,

                        Bolum =
                            e.Department?.Name,

                        Tur =
                            EmployeeTypeText(e)
                    })
                .ToList();


        ShowPage(
            "Tüm Çalışanlar",
            panel);
    }


    // =================================================
    // UI
    // =================================================

    private Panel CreateCrudPage(
        out FlowLayoutPanel actions)
    {
        var panel =
            new Panel
            {
                BackColor =
                    Color.White,

                Padding =
                    new Padding(15)
            };


        actions =
            new FlowLayoutPanel
            {
                Dock =
                    DockStyle.Top,

                Height =
                    60,

                Padding =
                    new Padding(5),

                BackColor =
                    Color.White
            };


        grid.Dock =
            DockStyle.Fill;


        panel.Controls.Add(
            grid);

        panel.Controls.Add(
            actions);


        return panel;
    }


    private static DataGridView
        CreateGrid()
    {
        var result =
            new DataGridView
            {
                ReadOnly =
                    true,

                AllowUserToAddRows =
                    false,

                AllowUserToDeleteRows =
                    false,

                MultiSelect =
                    false,

                RowHeadersVisible =
                    false,

                SelectionMode =
                    DataGridViewSelectionMode
                        .FullRowSelect,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode
                        .Fill,

                AutoGenerateColumns =
                    true,

                BackgroundColor =
                    Color.White,

                BorderStyle =
                    BorderStyle.None,

                ColumnHeadersHeight =
                    38,

                RowTemplate =
                {
                    Height = 34
                }
            };


        result.DataBindingComplete +=
            (_, _) =>
            {
                foreach (DataGridViewColumn column
                         in result.Columns)
                {
                    column.SortMode =
                        DataGridViewColumnSortMode.Automatic;
                }
            };


        return result;
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


    private int SelectedId()
    {
        if (grid.CurrentRow ==
            null)
        {
            throw new Exception(
                "Önce bir satır seçiniz.");
        }


        return Convert.ToInt32(
            grid.CurrentRow
                .Cells["ID"]
                .Value);
    }


    private static bool ConfirmDelete()
    {
        return MessageBox.Show(
            "Seçili kayıt silinsin mi?",
            "Silme Onayı",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning)
            ==
            DialogResult.Yes;
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
                    220,

                Height =
                    130,

                BackColor =
                    Color.White,

                Margin =
                    new Padding(12),

                Padding =
                    new Padding(20)
            };


        var number =
            new Label
            {
                Text =
                    value.ToString(),

                Font =
                    new Font(
                        "Segoe UI",
                        25,
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
                    82
            };


        card.Controls.Add(
            number);

        card.Controls.Add(
            label);

        panel.Controls.Add(
            card);
    }


    private static string EmployeeTypeText(
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
}