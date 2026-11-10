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

public class RegisterPatientForm : Form
{
    private readonly IAuthenticationService
        _authentication;


    private readonly TextBox txtFirstName =
        new();

    private readonly TextBox txtLastName =
        new();

    private readonly TextBox txtEmail =
        new();

    private readonly TextBox txtPhone =
        new();

    private readonly TextBox txtPassword =
        new();


    public RegisterPatientForm(
        IAuthenticationService authentication)
    {
        _authentication =
            authentication;

        InitializeUi();
    }


    private void InitializeUi()
    {
        Text =
            "Yeni Hasta Kaydı";

        Width =
            500;

        Height =
            610;

        StartPosition =
            FormStartPosition.CenterParent;

        BackColor =
            UiTheme.Background;

        Font =
            new Font(
                "Segoe UI",
                10);


        var title =
            UiTheme.CreateTitle(
                "Hasta Kaydı");

        title.Location =
            new Point(
                45,
                35);

        Controls.Add(
            title);


        int y =
            105;


        AddField(
            "Ad",
            txtFirstName,
            ref y);

        AddField(
            "Soyad",
            txtLastName,
            ref y);

        AddField(
            "E-posta",
            txtEmail,
            ref y);

        AddField(
            "Telefon",
            txtPhone,
            ref y);

        AddField(
            "Şifre",
            txtPassword,
            ref y);


        txtPassword.UseSystemPasswordChar =
            true;


        var saveButton =
            UiTheme.CreateButton(
                "Kaydı Oluştur",
                380);

        saveButton.Location =
            new Point(
                45,
                y + 15);

        saveButton.Click +=
            SaveButton_Click;


        Controls.Add(
            saveButton);
    }


    private void AddField(
        string labelText,
        TextBox textBox,
        ref int y)
    {
        var label =
            new Label
            {
                Text =
                    labelText,

                AutoSize =
                    true,

                Location =
                    new Point(
                        45,
                        y)
            };

        Controls.Add(
            label);


        textBox.Location =
            new Point(
                45,
                y + 25);

        textBox.Width =
            380;


        Controls.Add(
            textBox);


        y +=
            75;
    }


    private void SaveButton_Click(
        object? sender,
        EventArgs e)
    {
        try
        {
            var patient =
                _authentication
                    .RegisterPatient(
                        txtFirstName.Text,
                        txtLastName.Text,
                        txtEmail.Text,
                        txtPhone.Text,
                        txtPassword.Text);


            MessageBox.Show(
                $"Hasta kaydı oluşturuldu.\n\n" +
                $"Hasta ID: {patient.UserId}",
                "Başarılı",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);


            DialogResult =
                DialogResult.OK;

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
}