using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;

using Font = System.Drawing.Font;
using Label = System.Windows.Forms.Label;

namespace AltunPital.UI.Desktop;

public class ProfileForm : Form
{
    private readonly User
        _user;

    private readonly IUserAccountService
        _accountService;


    private readonly TextBox txtFirstName =
        new();

    private readonly TextBox txtLastName =
        new();

    private readonly TextBox txtEmail =
        new();

    private readonly TextBox txtPhone =
        new();

    private readonly TextBox txtCurrentPassword =
        new();

    private readonly TextBox txtNewPassword =
        new();


    public ProfileForm(
        User user,
        IUserAccountService accountService)
    {
        _user =
            user;

        _accountService =
            accountService;


        Text =
            "Profilim";

        Width =
            520;

        Height =
            680;

        StartPosition =
            FormStartPosition.CenterParent;

        BackColor =
            UiTheme.Background;

        Font =
            new Font(
                "Segoe UI",
                10);


        BuildUi();
        LoadUser();
    }


    private void BuildUi()
    {
        var title =
            UiTheme.CreateTitle(
                "Profilim");

        title.Left =
            40;

        title.Top =
            25;

        Controls.Add(
            title);


        int y =
            95;


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


        var updateButton =
            UiTheme.CreateButton(
                "Bilgileri Güncelle",
                400);

        updateButton.Left =
            40;

        updateButton.Top =
            y + 5;

        updateButton.Click +=
            UpdateProfile;


        Controls.Add(
            updateButton);


        y +=
            80;


        var passwordTitle =
            new Label
            {
                Text =
                    "Şifre Değiştir",

                Font =
                    new Font(
                        "Segoe UI",
                        14,
                        FontStyle.Bold),

                AutoSize =
                    true,

                Left =
                    40,

                Top =
                    y
            };


        Controls.Add(
            passwordTitle);

        y +=
            45;


        AddField(
            "Mevcut Şifre",
            txtCurrentPassword,
            ref y);

        AddField(
            "Yeni Şifre",
            txtNewPassword,
            ref y);


        txtCurrentPassword
            .UseSystemPasswordChar =
                true;

        txtNewPassword
            .UseSystemPasswordChar =
                true;


        var changePassword =
            UiTheme.CreateButton(
                "Şifreyi Değiştir",
                400);

        changePassword.Left =
            40;

        changePassword.Top =
            y + 5;

        changePassword.Click +=
            ChangePassword;


        Controls.Add(
            changePassword);
    }


    private void AddField(
        string title,
        TextBox box,
        ref int y)
    {
        var label =
            new Label
            {
                Text =
                    title,

                Left =
                    40,

                Top =
                    y,

                AutoSize =
                    true
            };


        box.Left =
            40;

        box.Top =
            y + 24;

        box.Width =
            400;


        Controls.Add(
            label);

        Controls.Add(
            box);


        y +=
            70;
    }


    private void LoadUser()
    {
        var user =
            _accountService.GetUser(
                _user.UserId);


        txtFirstName.Text =
            user.FirstName;

        txtLastName.Text =
            user.LastName;

        txtEmail.Text =
            user.Email;

        txtPhone.Text =
            user.Phone;
    }


    private void UpdateProfile(
        object? sender,
        EventArgs e)
    {
        try
        {
            _accountService
                .UpdateProfile(
                    _user.UserId,
                    txtFirstName.Text,
                    txtLastName.Text,
                    txtEmail.Text,
                    txtPhone.Text);


            MessageBox.Show(
                "Profil bilgileri güncellendi.",
                "Başarılı",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Hata",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }


    private void ChangePassword(
        object? sender,
        EventArgs e)
    {
        try
        {
            _accountService
                .ChangePassword(
                    _user.UserId,
                    txtCurrentPassword.Text,
                    txtNewPassword.Text);


            txtCurrentPassword.Clear();
            txtNewPassword.Clear();


            MessageBox.Show(
                "Şifreniz değiştirildi.",
                "Başarılı",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Hata",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}