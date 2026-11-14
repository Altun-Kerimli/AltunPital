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
    protected readonly IUserAccountService AccountService;

    protected readonly Panel Sidebar = new();
    protected readonly Panel ContentPanel = new();

    private readonly Label pageTitle = new();
    private readonly Label sidebarUserName = new();

    protected DesktopShellForm(User user, IUserAccountService accountService, string roleName)
    {
        CurrentUser = user;
        AccountService = accountService;

        Text = $"AltunPital - {roleName}";
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1200, 750);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = UiTheme.Background;
        Font = new Font("Segoe UI", 10);

        BuildLayout(roleName);
    }

    private void BuildLayout(string roleName)
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 245));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Sidebar.Dock = DockStyle.Fill;
        Sidebar.Margin = Padding.Empty;
        Sidebar.Padding = new Padding(24, 26, 24, 20);
        Sidebar.BackColor = UiTheme.PrimaryDark;

        var logo = new Label
        {
            Text = "ALTUNPITAL",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            AutoSize = true,
            Left = 24,
            Top = 30
        };

        var role = new Label
        {
            Text = roleName,
            ForeColor = Color.LightGray,
            Font = new Font("Segoe UI", 9),
            AutoSize = true,
            Left = 24,
            Top = 68
        };

        sidebarUserName.Text = $"{CurrentUser.FirstName} " + $"{CurrentUser.LastName}";
        sidebarUserName.ForeColor = Color.White;
        sidebarUserName.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        sidebarUserName.AutoSize = true;
        sidebarUserName.Left = 24;
        sidebarUserName.Top = 95;

        Sidebar.Controls.Add(logo);
        Sidebar.Controls.Add(role);
        Sidebar.Controls.Add(sidebarUserName);

        root.Controls.Add(Sidebar, 0, 0);
        root.SetRowSpan(Sidebar, 2);

        var header = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = Color.White,
            Padding = new Padding(32, 0, 32, 0)
        };

        pageTitle.Text = "Ana Sayfa";
        pageTitle.AutoSize = true;
        pageTitle.Font = new Font("Segoe UI", 21, FontStyle.Bold);
        pageTitle.ForeColor = UiTheme.Text;
        pageTitle.Left = 32;
        pageTitle.Top = 26;

        header.Controls.Add(pageTitle);

        root.Controls.Add(header, 1, 0);

        ContentPanel.Dock = DockStyle.Fill;
        ContentPanel.Margin = Padding.Empty;
        ContentPanel.Padding = new Padding(32, 30, 32, 30);
        ContentPanel.BackColor = UiTheme.Background;

        root.Controls.Add(ContentPanel, 1, 1);

        Controls.Add(root);
    }

    protected Button AddSidebarButton(string text, int top, Action action)
    {
        var button = new Button
        {
            Text = text,
            Width = 195,
            Height = 42,
            Left = 24,
            Top = top,
            FlatStyle = FlatStyle.Flat,
            BackColor = UiTheme.PrimaryDark,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0),
            Cursor = Cursors.Hand
        };

        button.FlatAppearance.BorderSize = 0;

        button.MouseEnter += (_, _) =>
        {
            button.BackColor = UiTheme.Primary;
        };

        button.MouseLeave += (_, _) =>
        {
            button.BackColor = UiTheme.PrimaryDark;
        };

        button.Click += (_, _) => action();

        Sidebar.Controls.Add(button);

        return button;
    }

    protected void AddLogoutButton()
    {
        var button = new Button
        {
            Text = "Oturumu Kapat",
            Width = 195,
            Height = 42,
            Left = 24,
            Top = 650,
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
            FlatStyle = FlatStyle.Flat,
            BackColor = UiTheme.PrimaryDark,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };

        button.FlatAppearance.BorderColor = Color.FromArgb(220, 230, 235);

        button.Click += (_, _) =>
        {
            DialogResult result = MessageBox.Show(
                "Oturumu kapatmak istiyor musunuz?",
                "Oturumu Kapat",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Close();
            }
        };

        Sidebar.Controls.Add(button);
    }

    protected void ShowPage(string title, Control control)
    {
        pageTitle.Text = title;
        ContentPanel.Controls.Clear();

        var wrapper = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Background,
            Padding = new Padding(12)
        };

        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(8);

        wrapper.Controls.Add(control);
        ContentPanel.Controls.Add(wrapper);
    }

    protected void OpenProfile()
    {
        using var form = new ProfileForm(CurrentUser, AccountService);

        form.ShowDialog(this);

        var refreshed = AccountService.GetUser(CurrentUser.UserId);

        CurrentUser.FirstName = refreshed.FirstName;
        CurrentUser.LastName = refreshed.LastName;
        CurrentUser.Email = refreshed.Email;
        CurrentUser.Phone = refreshed.Phone;

        // Profil kaydından sonra yan menüdeki adı yenile
        sidebarUserName.Text = $"{CurrentUser.FirstName} " + $"{CurrentUser.LastName}";
    }

    protected static void ShowError(Exception ex)
    {
        MessageBox.Show(
            ex.Message,
            "Hata",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    protected static void ShowSuccess(string message)
    {
        MessageBox.Show(
            message,
            "Başarılı",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
