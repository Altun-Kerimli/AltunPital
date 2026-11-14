using AltunPital.Data;
using AltunPital.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using Font = System.Drawing.Font;
using Label = System.Windows.Forms.Label;

namespace AltunPital.UI.Desktop;

public class DoctorForm : DesktopShellForm
{
    private readonly IDoctorService _doctorService;

    private readonly DataGridView grid = UiTableHelper.CreateGrid();

    public DoctorForm(
        User user,
        IDoctorService doctorService,
        IUserAccountService accountService)
        : base(user, accountService, "Doktor")
    {
        _doctorService = doctorService;

        AddSidebarButton("Ana Sayfa", 160, ShowHome);
        AddSidebarButton("Randevularım", 210, ShowAppointments);
        AddSidebarButton("Çalışma Programım", 260, ShowSchedule);
        AddSidebarButton("Profilim", 310, OpenProfile);
        AddLogoutButton();

        ShowHome();
    }

    private void ShowHome()
    {
        try
        {
            Doctor doctor = _doctorService
                .GetDoctor(CurrentUser.UserId);

            List<HealthService> services = _doctorService
                .GetServices(doctor.UserId);

            List<Appointment> appointments = _doctorService
                .GetAppointments(doctor.UserId);

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(35)
            };

            var title = new Label
            {
                Text = $"Dr. {doctor.FirstName} {doctor.LastName}",
                Left = 35,
                Top = 30,
                AutoSize = true,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = UiTheme.Primary
            };

            var location = new Label
            {
                Text = $"{doctor.Branch.City} / {doctor.Branch.Name}",
                Left = 38,
                Top = 80,
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.DimGray
            };

            panel.Controls.Add(title);
            panel.Controls.Add(location);

            int y = 135;

            AddInfo(panel, "Şube", doctor.Branch.Name, ref y);
            AddInfo(panel, "Şehir", doctor.Branch.City, ref y);
            AddInfo(panel, "Bölüm", doctor.Department?.Name ?? "-", ref y);
            AddInfo(panel, "Uzmanlık", doctor.Specialization, ref y);
            AddInfo(panel, "Çalışan No", doctor.EmployeeNumber, ref y);
            AddInfo(panel, "Telefon", doctor.Phone, ref y);
            AddInfo(panel, "E-posta", doctor.Email, ref y);
            AddInfo(
                panel,
                "Hizmetler",
                services.Count == 0
                    ? "Hizmet atanmamış"
                    : string.Join(", ", services.Select(s => s.Name)),
                ref y);

            y += 20;

            var divider = new Panel
            {
                Left = 35,
                Top = y,
                Width = 760,
                Height = 1,
                BackColor = Color.Gainsboro
            };

            panel.Controls.Add(divider);

            y += 30;

            AddInfo(panel, "Bekleyen Randevu", appointments.Count(a => a.Status == AppointmentStatus.Pending).ToString(), ref y);
            AddInfo(panel, "Onaylı Randevu", appointments.Count(a => a.Status == AppointmentStatus.Approved).ToString(), ref y);

            ShowPage("Ana Sayfa", panel);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ShowAppointments()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            Padding = new Padding(5),
            FlowDirection = FlowDirection.LeftToRight
        };

        var refresh = UiTheme.CreateButton("Yenile", 120);
        var approve = UiTheme.CreateButton("Onayla", 130);
        var reject = UiTheme.CreateButton("Reddet", 130);
        var complete = UiTheme.CreateButton("Tamamla", 130);

        refresh.Click += (_, _) =>
            LoadAppointments();

        approve.Click += (_, _) =>
            ChangeStatus(AppointmentAction.Approve);

        reject.Click += (_, _) =>
            ChangeStatus(AppointmentAction.Reject);

        complete.Click += (_, _) =>
            ChangeStatus(AppointmentAction.Complete);

        actions.Controls.Add(refresh);
        actions.Controls.Add(approve);
        actions.Controls.Add(reject);
        actions.Controls.Add(complete);

        grid.Dock = DockStyle.Fill;

        panel.Controls.Add(grid);
        panel.Controls.Add(actions);

        LoadAppointments();

        ShowPage("Randevularım", panel);
    }

    private void LoadAppointments()
    {
        grid.Tag = "appointments";

        UiTableHelper.Bind(
            grid,
            _doctorService
                .GetAppointments(CurrentUser.UserId)
                .Select(a =>
                    new
                    {
                        ID = a.AppointmentId,
                        Tarih = a.StartDateTime,
                        Hasta = $"{a.Patient.FirstName} {a.Patient.LastName}",
                        HastaTelefon = a.Patient.Phone,
                        Şube = a.Branch.Name,
                        Hizmet = a.HealthService.Name,
                        Süre = (int)(a.EndDateTime - a.StartDateTime).TotalMinutes,
                        Fiyat = a.Price,
                        Durum = StatusText(a.Status)
                    }));
    }

    private void ChangeStatus(AppointmentAction action)
    {
        try
        {
            int id = UiTableHelper
                .SelectedInt(grid, "ID");

            string question = action switch
            {
                AppointmentAction.Approve => "Randevu onaylansın mı?",
                AppointmentAction.Reject => "Randevu reddedilsin mi?",
                AppointmentAction.Complete => "Randevu tamamlandı olarak işaretlensin mi?",
                _ => "İşlem uygulansın mı?"
            };

            if (MessageBox.Show(
                    question,
                    "Randevu İşlemi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            switch (action)
            {
                case AppointmentAction.Approve:
                    _doctorService
                        .ApproveAppointment(CurrentUser.UserId, id);
                    break;

                case AppointmentAction.Reject:
                    _doctorService
                        .RejectAppointment(CurrentUser.UserId, id);
                    break;

                case AppointmentAction.Complete:
                    _doctorService
                        .CompleteAppointment(CurrentUser.UserId, id);
                    break;
            }

            LoadAppointments();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ShowSchedule()
    {
        try
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var info = new Label
            {
                Text = "Çalışma programınız yönetici tarafından belirlenmektedir.",
                Dock = DockStyle.Top,
                Height = 45,
                Padding = new Padding(5, 10, 5, 5),
                ForeColor = UiTheme.Primary,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            grid.Tag = "schedule";

            UiTableHelper.Bind(
                grid,
                _doctorService
                    .GetSchedules(CurrentUser.UserId)
                    .Select(s =>
                        new
                        {
                            ID = s.DoctorScheduleId,
                            Gün = DayText(s.DayOfWeek),
                            Başlangıç = s.StartTime,
                            Bitiş = s.EndTime
                        }));

            grid.Dock = DockStyle.Fill;

            panel.Controls.Add(grid);
            panel.Controls.Add(info);

            ShowPage("Çalışma Programım", panel);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private static void AddInfo(Control parent, string title, string value, ref int y)
    {
        parent.Controls.Add(new Label
        {
            Text = title,
            Left = 38,
            Top = y,
            Width = 170,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.DimGray
        });

        parent.Controls.Add(new Label
        {
            Text = string.IsNullOrWhiteSpace(value) ? "-" : value,
            Left = 220,
            Top = y,
            Width = 600,
            Height = 32
        });

        y += 38;
    }

    private static string StatusText(AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Pending => "Beklemede",
            AppointmentStatus.Approved => "Onaylandı",
            AppointmentStatus.Rejected => "Reddedildi",
            AppointmentStatus.Cancelled => "İptal Edildi",
            AppointmentStatus.Completed => "Tamamlandı",
            _ => "Bilinmiyor"
        };
    }

    private static string DayText(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "Pazartesi",
            DayOfWeek.Tuesday => "Salı",
            DayOfWeek.Wednesday => "Çarşamba",
            DayOfWeek.Thursday => "Perşembe",
            DayOfWeek.Friday => "Cuma",
            DayOfWeek.Saturday => "Cumartesi",
            DayOfWeek.Sunday => "Pazar",
            _ => "-"
        };
    }

    private enum AppointmentAction
    {
        Approve,
        Reject,
        Complete
    }
}
