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

public abstract class DesktopShellForm : Form
{
    protected readonly User CurrentUser;

    protected readonly IUserAccountService
        AccountService;

    protected readonly Panel Sidebar =
        new();

    protected readonly Panel ContentPanel =
        new();

    private readonly Label
        pageTitle =
            new();


    protected DesktopShellForm(
        User user,
        IUserAccountService accountService,
        string roleName)
    {
        CurrentUser =
            user;

        AccountService =
            accountService;

        Text =
            $"AltunPital - {roleName}";

        WindowState =
            FormWindowState.Maximized;

        MinimumSize =
            new Size(
                1100,
                700);

        StartPosition =
            FormStartPosition.CenterScreen;

        BackColor =
            UiTheme.Background;

        Font =
            new Font(
                "Segoe UI",
                10);


        BuildSidebar(
            roleName);

        BuildContent();
    }


    private void BuildSidebar(
        string roleName)
    {
        Sidebar.Dock =
            DockStyle.Left;

        Sidebar.Width =
            245;

        Sidebar.BackColor =
            UiTheme.PrimaryDark;


        var logo =
            new Label
            {
                Text =
                    "ALTUNPITAL",

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Segoe UI",
                        18,
                        FontStyle.Bold),

                AutoSize =
                    true,

                Left =
                    28,

                Top =
                    28
            };


        var role =
            new Label
            {
                Text =
                    roleName,

                ForeColor =
                    Color.LightGray,

                Font =
                    new Font(
                        "Segoe UI",
                        9),

                AutoSize =
                    true,

                Left =
                    30,

                Top =
                    68
            };


        var userName =
            new Label
            {
                Text =
                    $"{CurrentUser.FirstName} " +
                    $"{CurrentUser.LastName}",

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Segoe UI",
                        11,
                        FontStyle.Bold),

                AutoSize =
                    true,

                Left =
                    30,

                Top =
                    95
            };


        Sidebar.Controls.Add(
            logo);

        Sidebar.Controls.Add(
            role);

        Sidebar.Controls.Add(
            userName);


        Controls.Add(
            Sidebar);
    }


    private void BuildContent()
    {
        var header =
            new Panel
            {
                Dock =
                    DockStyle.Top,

                Height =
                    85,

                BackColor =
                    Color.White
            };


        pageTitle.Text =
            "Ana Sayfa";

        pageTitle.AutoSize =
            true;

        pageTitle.Font =
            new Font(
                "Segoe UI",
                22,
                FontStyle.Bold);

        pageTitle.ForeColor =
            UiTheme.Text;

        pageTitle.Left =
            32;

        pageTitle.Top =
            25;


        header.Controls.Add(
            pageTitle);


        ContentPanel.Dock =
            DockStyle.Fill;

        ContentPanel.BackColor =
            UiTheme.Background;

        ContentPanel.Padding =
            new Padding(30);


        Controls.Add(
            ContentPanel);

        Controls.Add(
            header);
    }


    protected Button AddSidebarButton(
        string text,
        int top,
        Action action)
    {
        var button =
            new Button
            {
                Text =
                    text,

                Width =
                    195,

                Height =
                    44,

                Left =
                    25,

                Top =
                    top,

                FlatStyle =
                    FlatStyle.Flat,

                FlatAppearance =
                {
                    BorderSize = 0
                },

                BackColor =
                    UiTheme.PrimaryDark,

                ForeColor =
                    Color.White,

                TextAlign =
                    ContentAlignment.MiddleLeft,

                Padding =
                    new Padding(
                        10,
                        0,
                        0,
                        0),

                Cursor =
                    Cursors.Hand
            };


        button.Click +=
            (_, _) =>
                action();


        Sidebar.Controls.Add(
            button);

        return button;
    }


    protected void AddLogoutButton()
    {
        var button =
            new Button
            {
                Text =
                    "Oturumu Kapat",

                Width =
                    195,

                Height =
                    44,

                Left =
                    25,

                Top =
                    600,

                Anchor =
                    AnchorStyles.Left |
                    AnchorStyles.Bottom,

                FlatStyle =
                    FlatStyle.Flat,

                FlatAppearance =
                {
                    BorderSize = 1
                },

                BackColor =
                    UiTheme.PrimaryDark,

                ForeColor =
                    Color.White,

                Cursor =
                    Cursors.Hand
            };


        button.Click +=
            (_, _) =>
            {
                DialogResult result =
                    MessageBox.Show(
                        "Oturumu kapatmak istiyor musunuz?",
                        "Oturumu Kapat",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (result ==
                    DialogResult.Yes)
                {
                    Close();
                }
            };


        Sidebar.Controls.Add(
            button);
    }


    protected void ShowPage(
        string title,
        Control control)
    {
        pageTitle.Text =
            title;

        ContentPanel.Controls.Clear();

        control.Dock =
            DockStyle.Fill;

        ContentPanel.Controls.Add(
            control);
    }


    protected void OpenProfile()
    {
        using var form =
            new ProfileForm(
                CurrentUser,
                AccountService);

        form.ShowDialog(this);
    }


    protected static void ShowError(
        Exception ex)
    {
        MessageBox.Show(
            ex.Message,
            "Hata",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }


    protected static void ShowSuccess(
        string message)
    {
        MessageBox.Show(
            message,
            "Başarılı",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}