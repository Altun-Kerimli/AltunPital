using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;

namespace AltunPital.UI.Desktop;

public class HealthStaffEditForm : Form
{
    private readonly IManagerService
        _managerService;

    private readonly HealthStaff?
        _staff;

    private readonly Branch?
        _lockedBranch;


    private readonly TextBox
        txtFirstName;

    private readonly TextBox
        txtLastName;

    private readonly TextBox
        txtEmail;

    private readonly TextBox
        txtPhone;

    private readonly TextBox
        txtPassword;

    private readonly TextBox
        txtEmployeeNumber;

    private readonly TextBox
        txtPosition;


    private readonly ComboBox
        cmbBranch;

    private readonly ComboBox
        cmbDepartment;


    private readonly DateTimePicker
        dtHireDate;


    public string FirstName =>
        txtFirstName.Text.Trim();

    public string LastName =>
        txtLastName.Text.Trim();

    public string Email =>
        txtEmail.Text.Trim();

    public string Phone =>
        txtPhone.Text.Trim();

    public string Password =>
        txtPassword.Text;

    public string EmployeeNumber =>
        txtEmployeeNumber.Text.Trim();

    public string Position =>
        txtPosition.Text.Trim();

    public DateTime HireDate =>
        dtHireDate.Value.Date;


    public Branch SelectedBranch =>
        (Branch)cmbBranch.SelectedItem!;


    public Department?
        SelectedDepartment =>
            cmbDepartment.SelectedItem
                as Department;


    public HealthStaffEditForm(
        IManagerService managerService,
        HealthStaff? staff = null,
        Branch? lockedBranch = null)
    {
        _managerService =
            managerService;

        _staff =
            staff;

        _lockedBranch =
            lockedBranch;


        Text =
            staff == null
                ? "Yeni Sağlık Personeli"
                : "Sağlık Personeli Düzenle";


        Width =
            610;

        Height =
            720;

        StartPosition =
            FormStartPosition.CenterParent;

        FormBorderStyle =
            FormBorderStyle.FixedDialog;

        MaximizeBox =
            false;

        MinimizeBox =
            false;


        int labelLeft =
            35;

        int inputLeft =
            190;

        int y =
            35;


        txtFirstName =
            AddText(
                "Ad",
                ref y);

        txtLastName =
            AddText(
                "Soyad",
                ref y);

        txtEmail =
            AddText(
                "E-posta",
                ref y);

        txtPhone =
            AddText(
                "Telefon",
                ref y);

        txtPassword =
            AddText(
                "Şifre",
                ref y);


        txtPassword
            .UseSystemPasswordChar =
                true;


        if (_staff != null)
        {
            txtPassword.Enabled =
                false;

            txtPassword.Text =
                "********";
        }


        txtEmployeeNumber =
            AddText(
                "Çalışan No",
                ref y);


        Controls.Add(
            EntityFormHelpers.Label(
                "İşe Giriş",
                labelLeft,
                y + 3));


        dtHireDate =
            new DateTimePicker
            {
                Left =
                    inputLeft,

                Top =
                    y,

                Width =
                    320,

                Format =
                    DateTimePickerFormat.Short
            };


        Controls.Add(
            dtHireDate);


        y +=
            55;


        Controls.Add(
            EntityFormHelpers.Label(
                "Şube",
                labelLeft,
                y + 3));


        cmbBranch =
            EntityFormHelpers.ComboBox(
                inputLeft,
                y);


        Controls.Add(
            cmbBranch);


        y +=
            55;


        Controls.Add(
            EntityFormHelpers.Label(
                "Bölüm",
                labelLeft,
                y + 3));


        cmbDepartment =
            EntityFormHelpers.ComboBox(
                inputLeft,
                y);


        Controls.Add(
            cmbDepartment);


        y +=
            55;


        txtPosition =
            AddText(
                "Pozisyon",
                ref y);


        cmbBranch.SelectedIndexChanged +=
            (_, _) =>
                LoadDepartments();


        LoadBranches();


        if (_lockedBranch != null)
        {
            cmbBranch.SelectedValue =
                _lockedBranch.BranchId;

            cmbBranch.Enabled =
                false;

            LoadDepartments();
        }


        if (_staff != null)
        {
            txtFirstName.Text =
                _staff.FirstName;

            txtLastName.Text =
                _staff.LastName;

            txtEmail.Text =
                _staff.Email;

            txtPhone.Text =
                _staff.Phone;

            txtEmployeeNumber.Text =
                _staff.EmployeeNumber;

            txtPosition.Text =
                _staff.Position;

            dtHireDate.Value =
                _staff.HireDate;


            cmbBranch.SelectedValue =
                _staff.BranchId;


            LoadDepartments();


            if (_staff.DepartmentId.HasValue)
            {
                cmbDepartment.SelectedValue =
                    _staff.DepartmentId.Value;
            }
        }


        var save =
            EntityFormHelpers.SaveButton(
                330,
                y + 15);


        var cancel =
            EntityFormHelpers.CancelButton(
                450,
                y + 15);


        save.Click +=
            (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(
                        FirstName) ||
                    string.IsNullOrWhiteSpace(
                        LastName) ||
                    string.IsNullOrWhiteSpace(
                        Email) ||
                    string.IsNullOrWhiteSpace(
                        EmployeeNumber) ||
                    string.IsNullOrWhiteSpace(
                        Position))
                {
                    MessageBox.Show(
                        "Zorunlu alanları doldurunuz.");

                    DialogResult =
                        DialogResult.None;

                    return;
                }


                if (_staff == null &&
                    string.IsNullOrWhiteSpace(
                        Password))
                {
                    MessageBox.Show(
                        "Yeni personel için şifre zorunludur.");

                    DialogResult =
                        DialogResult.None;
                }
            };


        Controls.Add(save);
        Controls.Add(cancel);


        AcceptButton =
            save;

        CancelButton =
            cancel;


        TextBox AddText(
            string label,
            ref int rowY)
        {
            Controls.Add(
                EntityFormHelpers.Label(
                    label,
                    labelLeft,
                    rowY + 3));


            var box =
                EntityFormHelpers.TextBox(
                    inputLeft,
                    rowY);


            Controls.Add(
                box);


            rowY +=
                55;


            return box;
        }
    }


    private void LoadBranches()
    {
        cmbBranch.DataSource =
            _managerService
                .GetBranches();


        cmbBranch.DisplayMember =
            nameof(Branch.Name);

        cmbBranch.ValueMember =
            nameof(Branch.BranchId);
    }


    private void LoadDepartments()
    {
        if (cmbBranch.SelectedItem
            is not Branch branch)
        {
            cmbDepartment.DataSource =
                null;

            return;
        }


        cmbDepartment.DataSource =
            _managerService
                .GetDepartments(
                    branch.BranchId);

        cmbDepartment.DisplayMember =
            nameof(Department.Name);

        cmbDepartment.ValueMember =
            nameof(Department.DepartmentId);
    }
}