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

public class LoginForm : Form
{
    private readonly IAuthenticationService _authentication;
    private readonly IPatientService _patientService;
    private readonly IDoctorService _doctorService;
    private readonly IManagerService _managerService;
    private readonly IEmployeeService _employeeService;
    private readonly IUserAccountService _accountService;

    private readonly TextBox txtEmail = new();
    private readonly TextBox txtPassword = new();

    public LoginForm(
        IAuthenticationService authentication,
        IPatientService patientService,
        IDoctorService doctorService,
        IManagerService managerService,
        IEmployeeService employeeService,
        IUserAccountService accountService)
    {
        _authentication = authentication;
        _patientService = patientService;
        _doctorService = doctorService;
        _managerService = managerService;
        _employeeService = employeeService;
        _accountService = accountService;

        InitializeUi();
    }

    private void InitializeUi()
    {
        Text = "AltunPital";
        Width = 520;
        Height = 590;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = UiTheme.Background;
        Font = new Font("Segoe UI", 10);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        var panel = new Panel
        {
            Width = 390,
            Height = 460,
            BackColor = UiTheme.Surface,
            Left = 55,
            Top = 45,
            Padding = new Padding(35)
        };

        var title = UiTheme.CreateTitle("ALTUNPITAL");
        title.Location = new Point(100, 35);

        var subtitle = UiTheme.CreateSubtitle("Hastane Randevu Yönetim Sistemi");
        subtitle.Location = new Point(72, 82);

        var emailLabel = new Label
        {
            Text = "E-posta",
            AutoSize = true,
            Location = new Point(35, 145)
        };

        txtEmail.Location = new Point(35, 170);
        txtEmail.Width = 320;
        txtEmail.Height = 35;

        var passwordLabel = new Label
        {
            Text = "Şifre",
            AutoSize = true,
            Location = new Point(35, 225)
        };

        txtPassword.Location = new Point(35, 250);
        txtPassword.Width = 320;
        txtPassword.UseSystemPasswordChar = true;

        var loginButton = UiTheme.CreateButton("Giriş Yap", 320);
        loginButton.Location = new Point(35, 310);
        loginButton.Click += LoginButton_Click;

        var registerButton = new Button
        {
            Text = "Yeni Hasta Kaydı",
            Width = 320,
            Height = 42,
            Location = new Point(35, 365),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = UiTheme.Primary,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        registerButton.Click += RegisterButton_Click;

        panel.Controls.Add(title);
        panel.Controls.Add(subtitle);
        panel.Controls.Add(emailLabel);
        panel.Controls.Add(txtEmail);
        panel.Controls.Add(passwordLabel);
        panel.Controls.Add(txtPassword);
        panel.Controls.Add(loginButton);
        panel.Controls.Add(registerButton);

        Controls.Add(panel);

        AcceptButton = loginButton;
    }

    private void LoginButton_Click(object? sender, EventArgs e)
    {
        try
        {
            User user = _authentication.Login(txtEmail.Text, txtPassword.Text);
            string role = _authentication.GetRole(user);

            // Rolüne göre ilgili ekranı aç
            Form nextForm = role switch
            {
                "Patient" => new PatientForm(user, _patientService, _accountService),
                "Doctor" => new DoctorForm(user, _doctorService, _accountService),
                "Manager" => new ManagerForm(user, _managerService, _employeeService, _accountService),
                "HealthStaff" => new HealthStaffForm(user, _accountService),
                _ => throw new Exception("Kullanıcı rolü tanımlanamadı.")
            };

            Hide();
            nextForm.ShowDialog();
            txtPassword.Clear();
            Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Giriş Hatası",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void RegisterButton_Click(object? sender, EventArgs e)
    {
        using var form = new RegisterPatientForm(_authentication, _patientService);
        form.ShowDialog();
    }
}
