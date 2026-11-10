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

    private readonly ComboBox cmbCity =
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
            "AltunPital - Yeni Hasta Kaydı";

        Width =
            520;

        Height =
            680;

        StartPosition =
            FormStartPosition.CenterParent;

        FormBorderStyle =
            FormBorderStyle.FixedDialog;

        MaximizeBox =
            false;

        MinimizeBox =
            false;

        BackColor =
            UiTheme.Background;

        Font =
            new Font(
                "Segoe UI",
                10);


        var title =
            UiTheme.CreateTitle(
                "Yeni Hasta Kaydı");

        title.Location =
            new Point(
                45,
                30);

        Controls.Add(
            title);


        var description =
            UiTheme.CreateSubtitle(
                "Randevu sistemini kullanmak için bilgilerinizi giriniz.");

        description.Location =
            new Point(
                47,
                72);

        Controls.Add(
            description);


        int y =
            120;


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


        var cityLabel =
            new Label
            {
                Text = "Şehir",
                AutoSize = true,
                Left = 45,
                Top = y
            };

        cmbCity.Left =
            45;

        cmbCity.Top =
            y + 25;

        cmbCity.Width =
            400;

        cmbCity.DropDownStyle =
            ComboBoxStyle.DropDownList;

        cmbCity.Items.AddRange(
            new object[]
            {
                "Sakarya",
                "İstanbul",
                "Kocaeli",
                "Ankara",
                "Bursa"
            });

        cmbCity.SelectedIndex =
            0;


        Controls.Add(
            cityLabel);

        Controls.Add(
            cmbCity);


        y +=
            75;


        AddField(
            "Şifre",
            txtPassword,
            ref y);

        txtPassword.UseSystemPasswordChar =
            true;


        var saveButton =
            UiTheme.CreateButton(
                "Kaydı Oluştur",
                400);

        saveButton.Left =
            45;

        saveButton.Top =
            y + 10;

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

                Left =
                    45,

                Top =
                    y
            };


        textBox.Left =
            45;

        textBox.Top =
            y + 25;

        textBox.Width =
            400;


        Controls.Add(
            label);

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
            string city =
                cmbCity.SelectedItem?.ToString()
                ?? "";


            var patient =
                _authentication
                    .RegisterPatient(
                        txtFirstName.Text,
                        txtLastName.Text,
                        txtEmail.Text,
                        txtPhone.Text,
                        city,
                        txtPassword.Text);


            MessageBox.Show(
                "Hasta kaydı başarıyla oluşturuldu.\n\n" +
                $"Hasta ID: {patient.UserId}",
                "Kayıt Başarılı",
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