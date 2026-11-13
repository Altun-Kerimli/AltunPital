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

    private readonly DataGridView grid =
        UiTableHelper.CreateGrid();


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


    // =====================================================
    // GENEL BAKIŞ
    // =====================================================

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


    // =====================================================
    // ŞUBELER
    // =====================================================

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
                "Şubeyi Aç",
                OpenBranch));

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
        UiTableHelper.Bind(
            grid,
            _managerService
                .GetBranches()
                .Select(b =>
                    new
                    {
                        ID =
                            b.BranchId,

                        Şube =
                            b.Name,

                        Şehir =
                            b.City,

                        İlçe =
                            b.District,

                        Adres =
                            b.Address,

                        Telefon =
                            b.Phone,

                        Açılış =
                            b.OpeningTime,

                        Kapanış =
                            b.ClosingTime
                    }));
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
            _managerService
                .AddBranch(
                    1,
                    form.BranchName,
                    form.City,
                    form.District,
                    form.Address,
                    form.Phone,
                    form.Opening,
                    form.ClosingTime);


            ShowSuccess(
                "Şube eklendi.");

            LoadBranches();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void OpenBranch()
    {
        try
        {
            int branchId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            var branch =
                _managerService
                    .GetBranches()
                    .First(b =>
                        b.BranchId ==
                        branchId);


            using var form =
                new BranchDetailForm(
                    branch,
                    _managerService,
                    _employeeService);


            form.ShowDialog(this);


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


            _managerService
                .UpdateBranch(
                    id,
                    form.BranchName,
                    form.City,
                    form.District,
                    form.Address,
                    form.Phone,
                    form.Opening,
                    form.ClosingTime);


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
                .DeleteBranch(
                    id);


            LoadBranches();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // BÖLÜMLER
    // =====================================================

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


        UiTableHelper.Bind(
            grid,
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

                                Şube =
                                    b.Name,

                                Bölüm =
                                    d.Name,

                                Açıklama =
                                    d.Description
                            })));
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


            ShowSuccess(
                "Bölüm eklendi.");

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


            ShowSuccess(
                "Bölüm güncellendi.");

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
                .DeleteDepartment(
                    id);


            LoadDepartments();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // HİZMETLER
    // =====================================================

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

        actions.Controls.Add(
            ActionButton(
                "Şube Hizmetleri",
                ShowBranchServices));


        LoadServices();


        ShowPage(
            "Hizmetler",
            panel);
    }


    private void LoadServices()
    {
        UiTableHelper.Bind(
            grid,
            _managerService
                .GetHealthServices()
                .Select(s =>
                    new
                    {
                        ID =
                            s.HealthServiceId,

                        Hizmet =
                            s.Name,

                        Süre =
                            s.DurationMinutes,

                        Açıklama =
                            s.Description
                    }));
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


            ShowSuccess(
                "Hizmet eklendi.");

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


            ShowSuccess(
                "Hizmet güncellendi.");

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
                .DeleteHealthService(
                    id);


            LoadServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AssignService()
    {
        using var form =
            new Form
            {
                Text =
                    "Hizmeti Şubeye Ata",

                Width =
                    520,

                Height =
                    330,

                StartPosition =
                    FormStartPosition.CenterParent,

                FormBorderStyle =
                    FormBorderStyle.FixedDialog,

                MaximizeBox =
                    false,

                MinimizeBox =
                    false
            };


        var cmbBranch =
            EntityFormHelpers.ComboBox(
                150,
                45,
                310);


        var cmbService =
            EntityFormHelpers.ComboBox(
                150,
                105,
                310);


        var numPrice =
            new NumericUpDown
            {
                Left =
                    150,

                Top =
                    165,

                Width =
                    180,

                DecimalPlaces =
                    2,

                Minimum =
                    0,

                Maximum =
                    1000000,

                Value =
                    1000
            };


        cmbBranch.DataSource =
            _managerService
                .GetBranches();

        cmbBranch.DisplayMember =
            nameof(Branch.Name);


        cmbService.DataSource =
            _managerService
                .GetHealthServices();

        cmbService.DisplayMember =
            nameof(HealthService.Name);


        form.Controls.AddRange(
        [
            EntityFormHelpers.Label(
                "Şube",
                30,
                48),

            cmbBranch,

            EntityFormHelpers.Label(
                "Hizmet",
                30,
                108),

            cmbService,

            EntityFormHelpers.Label(
                "Fiyat",
                30,
                168),

            numPrice
        ]);


        var save =
            EntityFormHelpers.SaveButton(
                250,
                225);

        var cancel =
            EntityFormHelpers.CancelButton(
                370,
                225);


        form.Controls.Add(
            save);

        form.Controls.Add(
            cancel);


        form.AcceptButton =
            save;

        form.CancelButton =
            cancel;


        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }


        try
        {
            var branch =
                (Branch)cmbBranch.SelectedItem!;

            var service =
                (HealthService)cmbService.SelectedItem!;


            _managerService
                .AssignServiceToBranch(
                    branch.BranchId,
                    service.HealthServiceId,
                    numPrice.Value);


            ShowSuccess(
                "Hizmet şubeye atandı.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ShowBranchServices()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditBranchService));

        actions.Controls.Add(
            ActionButton(
                "Bağlantıyı Sil",
                DeleteBranchService));

        actions.Controls.Add(
            ActionButton(
                "Hizmetlere Dön",
                ShowServices));


        LoadBranchServices();


        ShowPage(
            "Şube Hizmetleri",
            panel);
    }


    private void LoadBranchServices()
    {
        UiTableHelper.Bind(
            grid,
            _managerService
                .GetBranchServices()
                .Select(bs =>
                    new
                    {
                        ŞubeID =
                            bs.BranchId,

                        HizmetID =
                            bs.HealthServiceId,

                        Şube =
                            bs.Branch.Name,

                        Hizmet =
                            bs.HealthService.Name,

                        Fiyat =
                            bs.Price,

                        Durum =
                            bs.IsAvailable
                                ? "Aktif"
                                : "Pasif"
                    }));


        UiTableHelper.HideColumns(
            grid,
            "ŞubeID",
            "HizmetID");
    }


    private void EditBranchService()
    {
        try
        {
            int branchId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ŞubeID");

            int serviceId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "HizmetID");


            var relation =
                _managerService
                    .GetBranchServices()
                    .First(bs =>
                        bs.BranchId ==
                            branchId &&
                        bs.HealthServiceId ==
                            serviceId);


            using var form =
                new Form
                {
                    Text =
                        "Şube Hizmeti Düzenle",

                    Width =
                        450,

                    Height =
                        280,

                    StartPosition =
                        FormStartPosition.CenterParent,

                    FormBorderStyle =
                        FormBorderStyle.FixedDialog,

                    MaximizeBox =
                        false,

                    MinimizeBox =
                        false
                };


            var numPrice =
                new NumericUpDown
                {
                    Left =
                        150,

                    Top =
                        50,

                    Width =
                        180,

                    DecimalPlaces =
                        2,

                    Minimum =
                        0,

                    Maximum =
                        1000000,

                    Value =
                        relation.Price
                };


            var chkActive =
                new CheckBox
                {
                    Text =
                        "Hizmet aktif",

                    Left =
                        150,

                    Top =
                        110,

                    AutoSize =
                        true,

                    Checked =
                        relation.IsAvailable
                };


            form.Controls.Add(
                EntityFormHelpers.Label(
                    "Fiyat",
                    35,
                    53));

            form.Controls.Add(
                numPrice);

            form.Controls.Add(
                chkActive);


            var save =
                EntityFormHelpers.SaveButton(
                    190,
                    165);

            var cancel =
                EntityFormHelpers.CancelButton(
                    310,
                    165);


            form.Controls.Add(
                save);

            form.Controls.Add(
                cancel);


            form.AcceptButton =
                save;

            form.CancelButton =
                cancel;


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _managerService
                .UpdateBranchService(
                    branchId,
                    serviceId,
                    numPrice.Value,
                    chkActive.Checked);


            LoadBranchServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteBranchService()
    {
        try
        {
            int branchId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ŞubeID");

            int serviceId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "HizmetID");


            if (!ConfirmDelete())
                return;


            _managerService
                .RemoveServiceFromBranch(
                    branchId,
                    serviceId);


            LoadBranchServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // EKİPMANLAR
    // =====================================================

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

        actions.Controls.Add(
            ActionButton(
                "Şube Ekipmanları",
                ShowBranchEquipment));


        LoadEquipment();


        ShowPage(
            "Ekipmanlar",
            panel);
    }


    private void LoadEquipment()
    {
        UiTableHelper.Bind(
            grid,
            _managerService
                .GetEquipment()
                .Select(e =>
                    new
                    {
                        ID =
                            e.EquipmentId,

                        Ekipman =
                            e.Name,

                        Açıklama =
                            e.Description
                    }));
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


            ShowSuccess(
                "Ekipman eklendi.");

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


            ShowSuccess(
                "Ekipman güncellendi.");

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
                .DeleteEquipment(
                    id);


            LoadEquipment();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void AssignEquipment()
    {
        using var form =
            new Form
            {
                Text =
                    "Ekipmanı Şubeye Ata",

                Width =
                    520,

                Height =
                    340,

                StartPosition =
                    FormStartPosition.CenterParent,

                FormBorderStyle =
                    FormBorderStyle.FixedDialog,

                MaximizeBox =
                    false,

                MinimizeBox =
                    false
            };


        var cmbBranch =
            EntityFormHelpers.ComboBox(
                160,
                45,
                300);

        var cmbEquipment =
            EntityFormHelpers.ComboBox(
                160,
                105,
                300);


        var numQuantity =
            new NumericUpDown
            {
                Left =
                    160,

                Top =
                    165,

                Width =
                    150,

                Minimum =
                    1,

                Maximum =
                    1000,

                Value =
                    1
            };


        cmbBranch.DataSource =
            _managerService
                .GetBranches();

        cmbBranch.DisplayMember =
            nameof(Branch.Name);


        cmbEquipment.DataSource =
            _managerService
                .GetEquipment();

        cmbEquipment.DisplayMember =
            nameof(Equipment.Name);


        form.Controls.AddRange(
        [
            EntityFormHelpers.Label(
                "Şube",
                30,
                48),

            cmbBranch,

            EntityFormHelpers.Label(
                "Ekipman",
                30,
                108),

            cmbEquipment,

            EntityFormHelpers.Label(
                "Adet",
                30,
                168),

            numQuantity
        ]);


        var save =
            EntityFormHelpers.SaveButton(
                250,
                225);

        var cancel =
            EntityFormHelpers.CancelButton(
                370,
                225);


        form.Controls.Add(
            save);

        form.Controls.Add(
            cancel);


        form.AcceptButton =
            save;

        form.CancelButton =
            cancel;


        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }


        try
        {
            var branch =
                (Branch)cmbBranch.SelectedItem!;

            var equipment =
                (Equipment)cmbEquipment.SelectedItem!;


            _managerService
                .AssignEquipmentToBranch(
                    branch.BranchId,
                    equipment.EquipmentId,
                    (int)numQuantity.Value);


            ShowSuccess(
                "Ekipman şubeye atandı.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void ShowBranchEquipment()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditBranchEquipment));

        actions.Controls.Add(
            ActionButton(
                "Bağlantıyı Sil",
                DeleteBranchEquipment));

        actions.Controls.Add(
            ActionButton(
                "Ekipmanlara Dön",
                ShowEquipment));


        LoadBranchEquipment();


        ShowPage(
            "Şube Ekipmanları",
            panel);
    }


    private void LoadBranchEquipment()
    {
        UiTableHelper.Bind(
            grid,
            _managerService
                .GetBranchEquipment()
                .Select(be =>
                    new
                    {
                        ŞubeID =
                            be.BranchId,

                        EkipmanID =
                            be.EquipmentId,

                        Şube =
                            be.Branch.Name,

                        Ekipman =
                            be.Equipment.Name,

                        Adet =
                            be.Quantity,

                        Durum =
                            be.IsOperational
                                ? "Çalışıyor"
                                : "Arızalı"
                    }));


        UiTableHelper.HideColumns(
            grid,
            "ŞubeID",
            "EkipmanID");
    }


    private void EditBranchEquipment()
    {
        try
        {
            int branchId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ŞubeID");

            int equipmentId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "EkipmanID");


            var relation =
                _managerService
                    .GetBranchEquipment()
                    .First(be =>
                        be.BranchId ==
                            branchId &&
                        be.EquipmentId ==
                            equipmentId);


            using var form =
                new Form
                {
                    Text =
                        "Şube Ekipmanı Düzenle",

                    Width =
                        450,

                    Height =
                        280,

                    StartPosition =
                        FormStartPosition.CenterParent,

                    FormBorderStyle =
                        FormBorderStyle.FixedDialog,

                    MaximizeBox =
                        false,

                    MinimizeBox =
                        false
                };


            var numQuantity =
                new NumericUpDown
                {
                    Left =
                        150,

                    Top =
                        50,

                    Width =
                        160,

                    Minimum =
                        0,

                    Maximum =
                        1000,

                    Value =
                        relation.Quantity
                };


            var chkOperational =
                new CheckBox
                {
                    Text =
                        "Ekipman çalışıyor",

                    Left =
                        150,

                    Top =
                        110,

                    AutoSize =
                        true,

                    Checked =
                        relation.IsOperational
                };


            form.Controls.Add(
                EntityFormHelpers.Label(
                    "Adet",
                    35,
                    53));

            form.Controls.Add(
                numQuantity);

            form.Controls.Add(
                chkOperational);


            var save =
                EntityFormHelpers.SaveButton(
                    190,
                    165);

            var cancel =
                EntityFormHelpers.CancelButton(
                    310,
                    165);


            form.Controls.Add(
                save);

            form.Controls.Add(
                cancel);


            form.AcceptButton =
                save;

            form.CancelButton =
                cancel;


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _managerService
                .UpdateBranchEquipment(
                    branchId,
                    equipmentId,
                    (int)numQuantity.Value,
                    chkOperational.Checked);


            LoadBranchEquipment();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteBranchEquipment()
    {
        try
        {
            int branchId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ŞubeID");

            int equipmentId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "EkipmanID");


            if (!ConfirmDelete())
                return;


            _managerService
                .RemoveEquipmentFromBranch(
                    branchId,
                    equipmentId);


            LoadBranchEquipment();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // DOKTORLAR
    // =====================================================

    private void ShowDoctors()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni",
                AddDoctor));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditDoctor));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                DeleteDoctor));

        actions.Controls.Add(
            ActionButton(
                "Hizmetler",
                ShowDoctorServices));

        actions.Controls.Add(
            ActionButton(
                "Çalışma Saatleri",
                ShowSchedules));


        LoadDoctors();


        ShowPage(
            "Doktorlar",
            panel);
    }


    private void LoadDoctors()
    {
        UiTableHelper.Bind(
            grid,
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

                        Telefon =
                            d.Phone,

                        ÇalışanNo =
                            d.EmployeeNumber,

                        Uzmanlık =
                            d.Specialization,

                        Şube =
                            d.Branch.Name,

                        Bölüm =
                            d.Department?.Name,

                        İşeGiriş =
                            d.HireDate
                    }));
    }


    private void AddDoctor()
    {
        using var form =
            new DoctorEditForm(
                _managerService);


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
                    form.SelectedBranch.BranchId,
                    form.SelectedDepartment.DepartmentId,
                    form.Specialization);


            ShowSuccess(
                "Doktor eklendi.");

            LoadDoctors();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditDoctor()
    {
        try
        {
            int id =
                SelectedId();


            var doctor =
                _employeeService
                    .GetEmployees()
                    .OfType<Doctor>()
                    .First(d =>
                        d.UserId ==
                        id);


            using var form =
                new DoctorEditForm(
                    _managerService,
                    doctor);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _employeeService
                .UpdateDoctor(
                    id,
                    form.FirstName,
                    form.LastName,
                    form.Email,
                    form.Phone,
                    form.EmployeeNumber,
                    form.HireDate,
                    form.SelectedBranch.BranchId,
                    form.SelectedDepartment.DepartmentId,
                    form.Specialization);


            ShowSuccess(
                "Doktor güncellendi.");

            LoadDoctors();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteDoctor()
    {
        try
        {
            int id =
                SelectedId();


            if (!ConfirmDelete())
                return;


            _employeeService
                .DeleteEmployee(
                    id);


            LoadDoctors();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // DOKTOR HİZMETLERİ
    // =====================================================

    private void ShowDoctorServices()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Hizmet Ata",
                AssignDoctorService));

        actions.Controls.Add(
            ActionButton(
                "Atamayı Sil",
                RemoveDoctorService));

        actions.Controls.Add(
            ActionButton(
                "Doktorlara Dön",
                ShowDoctors));


        LoadDoctorServices();


        ShowPage(
            "Doktor Hizmetleri",
            panel);
    }


    private void LoadDoctorServices()
    {
        UiTableHelper.Bind(
            grid,
            _employeeService
                .GetDoctorServices()
                .Select(ds =>
                    new
                    {
                        DoktorID =
                            ds.DoctorId,

                        HizmetID =
                            ds.HealthServiceId,

                        Doktor =
                            $"{ds.Doctor.FirstName} " +
                            $"{ds.Doctor.LastName}",

                        Hizmet =
                            ds.HealthService.Name
                    }));


        UiTableHelper.HideColumns(
            grid,
            "DoktorID",
            "HizmetID");
    }


    private void AssignDoctorService()
    {
        try
        {
            var doctor =
                PromptDialog.Choose(
                    "Doktora Hizmet Ata",
                    "Doktor:",
                    _employeeService
                        .GetEmployees()
                        .OfType<Doctor>()
                        .ToList(),
                    d =>
                        $"{d.FirstName} {d.LastName} - {d.Branch.Name}");


            if (doctor == null)
                return;


            var branchServiceIds =
                _managerService
                    .GetBranchServices(
                        doctor.BranchId)
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
                    "Hizmet:",
                    services,
                    s =>
                        s.Name);


            if (service == null)
                return;


            _employeeService
                .AssignService(
                    doctor.UserId,
                    service.HealthServiceId);


            ShowSuccess(
                "Hizmet doktora atandı.");

            LoadDoctorServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void RemoveDoctorService()
    {
        try
        {
            int doctorId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "DoktorID");

            int serviceId =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "HizmetID");


            if (!ConfirmDelete())
                return;


            _employeeService
                .RemoveService(
                    doctorId,
                    serviceId);


            LoadDoctorServices();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // ÇALIŞMA SAATLERİ
    // =====================================================

    private void ShowSchedules()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni",
                AddSchedule));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditSchedule));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                DeleteSchedule));

        actions.Controls.Add(
            ActionButton(
                "Doktorlara Dön",
                ShowDoctors));


        LoadSchedules();


        ShowPage(
            "Doktor Çalışma Saatleri",
            panel);
    }


    private void LoadSchedules()
    {
        UiTableHelper.Bind(
            grid,
            _employeeService
                .GetSchedules()
                .Select(s =>
                    new
                    {
                        ID =
                            s.DoctorScheduleId,

                        Doktor =
                            $"{s.Doctor.FirstName} " +
                            $"{s.Doctor.LastName}",

                        Gün =
                            DayText(
                                s.DayOfWeek),

                        Başlangıç =
                            s.StartTime,

                        Bitiş =
                            s.EndTime
                    }));
    }


    private void AddSchedule()
    {
        try
        {
            var doctors =
                _employeeService
                    .GetEmployees()
                    .OfType<Doctor>()
                    .ToList();


            using var form =
                new ScheduleEditForm(
                    doctors);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _employeeService
                .AddSchedule(
                    form.SelectedDoctor.UserId,
                    form.SelectedDay,
                    form.StartTime,
                    form.EndTime);


            ShowSuccess(
                "Çalışma saati eklendi.");

            LoadSchedules();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditSchedule()
    {
        try
        {
            int id =
                SelectedId();


            var schedule =
                _employeeService
                    .GetSchedules()
                    .First(s =>
                        s.DoctorScheduleId ==
                        id);


            var doctors =
                _employeeService
                    .GetEmployees()
                    .OfType<Doctor>()
                    .ToList();


            using var form =
                new ScheduleEditForm(
                    doctors,
                    schedule);


            if (form.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }


            _employeeService
                .UpdateSchedule(
                    id,
                    form.SelectedDay,
                    form.StartTime,
                    form.EndTime);


            ShowSuccess(
                "Çalışma saati güncellendi.");

            LoadSchedules();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteSchedule()
    {
        try
        {
            int id =
                SelectedId();


            if (!ConfirmDelete())
                return;


            _employeeService
                .DeleteSchedule(
                    id);


            LoadSchedules();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // SAĞLIK PERSONELİ
    // =====================================================

    private void ShowHealthStaff()
    {
        var panel =
            CreateCrudPage(
                out var actions);


        actions.Controls.Add(
            ActionButton(
                "Yeni",
                AddHealthStaff));

        actions.Controls.Add(
            ActionButton(
                "Düzenle",
                EditHealthStaff));

        actions.Controls.Add(
            ActionButton(
                "Sil",
                DeleteHealthStaff));


        LoadHealthStaff();


        ShowPage(
            "Sağlık Personeli",
            panel);
    }


    private void LoadHealthStaff()
    {
        UiTableHelper.Bind(
            grid,
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

                        Telefon =
                            s.Phone,

                        ÇalışanNo =
                            s.EmployeeNumber,

                        Pozisyon =
                            s.Position,

                        Şube =
                            s.Branch.Name,

                        Bölüm =
                            s.Department?.Name,

                        İşeGiriş =
                            s.HireDate
                    }));
    }


    private void AddHealthStaff()
    {
        using var form =
            new HealthStaffEditForm(
                _managerService);


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
                    form.SelectedBranch.BranchId,
                    form.SelectedDepartment?
                        .DepartmentId,
                    form.Position);


            ShowSuccess(
                "Sağlık personeli eklendi.");

            LoadHealthStaff();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void EditHealthStaff()
    {
        try
        {
            int id =
                SelectedId();


            var staff =
                _employeeService
                    .GetEmployees()
                    .OfType<HealthStaff>()
                    .First(s =>
                        s.UserId ==
                        id);


            using var form =
                new HealthStaffEditForm(
                    _managerService,
                    staff);


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
                    form.SelectedBranch.BranchId,
                    form.SelectedDepartment?
                        .DepartmentId,
                    form.Position);


            ShowSuccess(
                "Sağlık personeli güncellendi.");

            LoadHealthStaff();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    private void DeleteHealthStaff()
    {
        try
        {
            int id =
                SelectedId();


            if (!ConfirmDelete())
                return;


            _employeeService
                .DeleteEmployee(
                    id);


            LoadHealthStaff();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // TÜM ÇALIŞANLAR
    // =====================================================

    private void ShowEmployees()
    {
        var panel =
            CreateCrudPage(
                out _);


        UiTableHelper.Bind(
            grid,
            _employeeService
                .GetEmployees()
                .Select(e =>
                    new
                    {
                        ID =
                            e.UserId,

                        Şube =
                            e.Branch.Name,

                        Ad =
                            e.FirstName,

                        Soyad =
                            e.LastName,

                        Eposta =
                            e.Email,

                        Telefon =
                            e.Phone,

                        ÇalışanNo =
                            e.EmployeeNumber,

                        Bölüm =
                            e.Department?.Name,

                        Tür =
                            EmployeeTypeText(
                                e)
                    }));


        ShowPage(
            "Tüm Çalışanlar",
            panel);
    }


    // =====================================================
    // ORTAK UI
    // =====================================================

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


        var top =
            new Panel
            {
                Dock =
                    DockStyle.Top,

                Height =
                    105,

                BackColor =
                    Color.White
            };


        var searchLabel =
            new Label
            {
                Text =
                    "Ara",

                Left =
                    5,

                Top =
                    8,

                AutoSize =
                    true
            };


        var search =
            new TextBox
            {
                Left =
                    5,

                Top =
                    32,

                Width =
                    320,

                PlaceholderText =
                    "Tabloda ara..."
            };


        actions =
            new FlowLayoutPanel
            {
                Left =
                    350,

                Top =
                    20,

                Height =
                    60,

                Width =
                    900,

                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Left |
                    AnchorStyles.Right,

                FlowDirection =
                    FlowDirection.LeftToRight,

                WrapContents =
                    false,

                AutoScroll =
                    true
            };


        top.Controls.Add(
            searchLabel);

        top.Controls.Add(
            search);

        top.Controls.Add(
            actions);


        grid.Dock =
            DockStyle.Fill;


        panel.Controls.Add(
            grid);

        panel.Controls.Add(
            top);


        UiTableHelper.AttachSearch(
            search,
            grid);


        return panel;
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
        return UiTableHelper
            .SelectedInt(
                grid,
                "ID");
    }


    private static bool ConfirmDelete()
    {
        return MessageBox.Show(
            "Seçili kayıt silinsin mi?\n\n" +
            "Bu işlem geri alınamaz.",
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


    private static string DayText(
        DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday =>
                "Pazartesi",

            DayOfWeek.Tuesday =>
                "Salı",

            DayOfWeek.Wednesday =>
                "Çarşamba",

            DayOfWeek.Thursday =>
                "Perşembe",

            DayOfWeek.Friday =>
                "Cuma",

            DayOfWeek.Saturday =>
                "Cumartesi",

            DayOfWeek.Sunday =>
                "Pazar",

            _ =>
                "Bilinmiyor"
        };
    }
}