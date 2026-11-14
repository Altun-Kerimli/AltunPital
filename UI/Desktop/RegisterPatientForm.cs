using AltunPital.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using AltunPital.Data;

using Font = System.Drawing.Font;
using Label = System.Windows.Forms.Label;

namespace AltunPital.UI.Desktop;

public class RegisterPatientForm : Form
{
    private readonly IAuthenticationService _authentication;
    private readonly IPatientService _patientService;

    private readonly TextBox txtFirstName;
    private readonly TextBox txtLastName;
    private readonly TextBox txtEmail;
    private readonly TextBox txtPhone;
    private readonly TextBox txtPassword;
    private readonly TextBox txtPasswordAgain;

    private readonly ComboBox cmbCity;
    private readonly ComboBox cmbBranch;

    private bool internalUpdate;

    public RegisterPatientForm(IAuthenticationService authentication, IPatientService patientService)
    {
        _authentication = authentication;
        _patientService = patientService;

        Text = "Hasta Kayıt";
        Width = 620;
        Height = 650;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(35, 25, 35, 25),
            BackColor = Color.White
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));

        Controls.Add(root);

        var header = new Panel
        {
            Dock = DockStyle.Fill
        };

        var title = new Label
        {
            Text = "Yeni Hasta Kaydı",
            AutoSize = true,
            Left = 0,
            Top = 0,
            Font = new Font("Segoe UI", 20, FontStyle.Bold)
        };

        var subtitle = new Label
        {
            Text = "Bilgilerinizi doldurarak hasta hesabınızı oluşturabilirsiniz.",
            AutoSize = true,
            Left = 2,
            Top = 42,
            ForeColor = Color.DimGray
        };

        header.Controls.Add(title);
        header.Controls.Add(subtitle);

        root.Controls.Add(header, 0, 0);

        var formPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 9,
            Padding = new Padding(0, 5, 0, 0)
        };

        formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (int i = 0; i < 9; i++)
        {
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        }

        txtFirstName = CreateTextBox();
        txtLastName = CreateTextBox();
        txtEmail = CreateTextBox();
        txtPhone = CreateTextBox();
        cmbCity = CreateComboBox();
        cmbBranch = CreateComboBox();
        txtPassword = CreateTextBox();
        txtPasswordAgain = CreateTextBox();

        txtPassword.UseSystemPasswordChar = true;
        txtPasswordAgain.UseSystemPasswordChar = true;

        AddRow(formPanel, 0, "Ad", txtFirstName);
        AddRow(formPanel, 1, "Soyad", txtLastName);
        AddRow(formPanel, 2, "E-posta", txtEmail);
        AddRow(formPanel, 3, "Telefon", txtPhone);
        AddRow(formPanel, 4, "Şehir", cmbCity);
        AddRow(formPanel, 5, "Tercih Edilen Şube", cmbBranch);
        AddRow(formPanel, 6, "Şifre", txtPassword);
        AddRow(formPanel, 7, "Şifre Tekrar", txtPasswordAgain);

        var info = new Label
        {
            Text =
                "Tercih edilen şube yalnızca başlangıç tercihinizdir. " +
                "Randevu alırken diğer şubeleri de seçebilirsiniz.",
            Dock = DockStyle.Fill,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.DimGray,
            Padding = new Padding(0, 3, 0, 0)
        };

        formPanel.Controls.Add(info, 1, 8);

        root.Controls.Add(formPanel, 0, 1);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0)
        };

        var register = UiTheme.CreateButton("Kayıt Ol", 120);

        var cancel = new Button
        {
            Text = "İptal",
            Width = 110,
            Height = 40,
            DialogResult = DialogResult.Cancel,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White
        };

        register.Height = 40;
        register.Click += (_, _) => Register();

        buttons.Controls.Add(register);
        buttons.Controls.Add(cancel);

        root.Controls.Add(buttons, 0, 2);

        AcceptButton = register;
        CancelButton = cancel;

        cmbBranch.FormattingEnabled = true;
        cmbBranch.Format += (_, e) =>
        {
            if (e.ListItem is Branch branch)
            {
                e.Value = $"{branch.Name} - " + $"{branch.District}";
            }
        };

        // Şehir değişince o şehrin şubelerini yükle
        cmbCity.SelectedIndexChanged += (_, _) =>
        {
            if (!internalUpdate)
            {
                LoadBranchesForCity();
            }
        };

        LoadLocations();
    }

    private void LoadLocations()
    {
        List<Branch> branches = _patientService.GetBranches();

        List<string> cities = branches
            .Where(b => !string.IsNullOrWhiteSpace(b.City))
            .Select(b => b.City.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(city => city)
            .ToList();

        internalUpdate = true;

        try
        {
            cmbCity.DataSource = null;
            cmbCity.DataSource = cities;
        }
        finally
        {
            internalUpdate = false;
        }

        LoadBranchesForCity();
    }

    private void LoadBranchesForCity()
    {
        string? selectedCity = cmbCity.SelectedItem?.ToString();

        List<Branch> branches = _patientService.GetBranches();

        if (!string.IsNullOrWhiteSpace(selectedCity))
        {
            branches = branches
                .Where(b => string.Equals(b.City, selectedCity, StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.BranchId)
                .ToList();
        }

        internalUpdate = true;

        try
        {
            cmbBranch.DataSource = null;
            cmbBranch.DisplayMember = nameof(Branch.Name);
            cmbBranch.ValueMember = nameof(Branch.BranchId);
            cmbBranch.DataSource = branches;
        }
        finally
        {
            internalUpdate = false;
        }
    }

    private void Register()
    {
        try
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text;
            string passwordAgain = txtPasswordAgain.Text;

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone))
            {
                throw new Exception("Ad, soyad, e-posta ve telefon alanlarını doldurunuz.");
            }

            if (cmbCity.SelectedItem == null)
            {
                throw new Exception("Şehir seçiniz.");
            }

            if (cmbBranch.SelectedItem == null)
            {
                throw new Exception("Tercih edilen şubeyi seçiniz.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Şifre giriniz.");
            }

            if (password != passwordAgain)
            {
                throw new Exception("Şifreler aynı değil.");
            }

            string city = cmbCity.SelectedItem.ToString()!;

            Patient patient = _authentication.RegisterPatient(
                firstName,
                lastName,
                email,
                phone,
                city,
                password);

            MessageBox.Show(
                $"Kayıt başarılı.\nHasta ID: {patient.UserId}",
                "Hasta Kayıt",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Kayıt Hatası",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static TextBox CreateTextBox()
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 7, 0, 7)
        };
    }

    private static ComboBox CreateComboBox()
    {
        return new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(0, 7, 0, 7)
        };
    }

    private static void AddRow(TableLayoutPanel panel, int row, string title, Control control)
    {
        var label = new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 9.5f)
        };

        panel.Controls.Add(label, 0, row);
        panel.Controls.Add(control, 1, row);
    }
}
