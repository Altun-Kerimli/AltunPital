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
    private readonly IDoctorService
        _doctorService;


    private readonly DataGridView grid =
        UiTableHelper.CreateGrid();


    public DoctorForm(
        User user,
        IDoctorService doctorService,
        IUserAccountService accountService)
        : base(
            user,
            accountService,
            "Doktor")
    {
        _doctorService =
            doctorService;


        AddSidebarButton(
            "Ana Sayfa",
            160,
            ShowHome);

        AddSidebarButton(
            "Randevularım",
            210,
            ShowAppointments);

        AddSidebarButton(
            "Çalışma Programım",
            260,
            ShowSchedule);

        AddSidebarButton(
            "Profilim",
            310,
            OpenProfile);

        AddLogoutButton();


        ShowHome();
    }


    // =====================================================
    // ANA SAYFA
    // =====================================================

    private void ShowHome()
    {
        try
        {
            var doctor =
                CurrentUser
                    as Doctor;


            var panel =
                new Panel
                {
                    BackColor =
                        Color.White,

                    Padding =
                        new Padding(30)
                };


            var title =
                new Label
                {
                    Text =
                        $"Dr. {CurrentUser.FirstName} " +
                        $"{CurrentUser.LastName}",

                    Font =
                        new Font(
                            "Segoe UI",
                            22,
                            FontStyle.Bold),

                    AutoSize =
                        true,

                    Left =
                        30,

                    Top =
                        30
                };


            panel.Controls.Add(
                title);


            int y =
                95;


            if (doctor != null)
            {
                AddInfoLine(
                    panel,
                    "Uzmanlık",
                    doctor.Specialization,
                    ref y);


                AddInfoLine(
                    panel,
                    "Şube",
                    doctor.Branch?.Name
                    ?? "-",
                    ref y);


                AddInfoLine(
                    panel,
                    "Bölüm",
                    doctor.Department?.Name
                    ?? "-",
                    ref y);
            }


            var appointments =
                _doctorService
                    .GetAppointments(
                        CurrentUser.UserId);


            int pending =
                appointments.Count(a =>
                    a.Status ==
                    AppointmentStatus.Pending);


            int approved =
                appointments.Count(a =>
                    a.Status ==
                    AppointmentStatus.Approved);


            var stats =
                new Label
                {
                    Text =
                        $"Bekleyen randevu: {pending}\n" +
                        $"Onaylanmış randevu: {approved}",

                    Font =
                        new Font(
                            "Segoe UI",
                            11),

                    AutoSize =
                        true,

                    Left =
                        30,

                    Top =
                        y + 20
                };


            panel.Controls.Add(
                stats);


            ShowPage(
                "Ana Sayfa",
                panel);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // RANDEVULAR
    // =====================================================

    private void ShowAppointments()
    {
        var panel =
            new Panel
            {
                BackColor =
                    Color.White,

                Padding =
                    new Padding(15)
            };


        var buttons =
            new FlowLayoutPanel
            {
                Dock =
                    DockStyle.Bottom,

                Height =
                    65,

                FlowDirection =
                    FlowDirection.LeftToRight,

                Padding =
                    new Padding(
                        5,
                        10,
                        5,
                        5),

                WrapContents =
                    false,

                BackColor =
                    Color.White
            };


        var refreshButton =
            UiTheme.CreateButton(
                "Yenile",
                120);


        var approveButton =
            UiTheme.CreateButton(
                "Onayla",
                140);


        var rejectButton =
            UiTheme.CreateButton(
                "Reddet",
                140);


        var completeButton =
            UiTheme.CreateButton(
                "Tamamla",
                140);


        refreshButton.Click +=
            (_, _) =>
                LoadAppointments();


        approveButton.Click +=
            (_, _) =>
                ChangeAppointmentStatus(
                    AppointmentAction.Approve);


        rejectButton.Click +=
            (_, _) =>
                ChangeAppointmentStatus(
                    AppointmentAction.Reject);


        completeButton.Click +=
            (_, _) =>
                ChangeAppointmentStatus(
                    AppointmentAction.Complete);


        buttons.Controls.Add(
            refreshButton);

        buttons.Controls.Add(
            approveButton);

        buttons.Controls.Add(
            rejectButton);

        buttons.Controls.Add(
            completeButton);


        grid.Dock =
            DockStyle.Fill;


        panel.Controls.Add(
            grid);

        panel.Controls.Add(
            buttons);


        LoadAppointments();


        ShowPage(
            "Randevularım",
            panel);
    }


    private void LoadAppointments()
    {
        grid.Tag =
            "appointments";


        UiTableHelper.Bind(
            grid,
            _doctorService
                .GetAppointments(
                    CurrentUser.UserId)
                .Select(a =>
                    new
                    {
                        ID =
                            a.AppointmentId,

                        Tarih =
                            a.StartDateTime,

                        Hasta =
                            $"{a.Patient.FirstName} " +
                            $"{a.Patient.LastName}",

                        Telefon =
                            a.Patient.Phone,

                        Şube =
                            a.Branch.Name,

                        Hizmet =
                            a.HealthService.Name,

                        Başlangıç =
                            a.StartDateTime
                                .ToString(
                                    "HH:mm"),

                        Bitiş =
                            a.EndDateTime
                                .ToString(
                                    "HH:mm"),

                        Durum =
                            StatusText(
                                a.Status),

                        Fiyat =
                            a.Price
                    }));
    }


    private void ChangeAppointmentStatus(
        AppointmentAction action)
    {
        try
        {
            if ((string?)grid.Tag !=
                "appointments")
            {
                throw new Exception(
                    "Önce randevularınızı görüntüleyiniz.");
            }


            int id =
                UiTableHelper
                    .SelectedInt(
                        grid,
                        "ID");


            string question =
                action switch
                {
                    AppointmentAction.Approve =>
                        "Seçili randevu onaylansın mı?",

                    AppointmentAction.Reject =>
                        "Seçili randevu reddedilsin mi?",

                    AppointmentAction.Complete =>
                        "Seçili randevu tamamlandı olarak işaretlensin mi?",

                    _ =>
                        "İşlem uygulansın mı?"
                };


            DialogResult confirm =
                MessageBox.Show(
                    question,
                    "Randevu İşlemi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);


            if (confirm !=
                DialogResult.Yes)
            {
                return;
            }


            switch (action)
            {
                case AppointmentAction.Approve:

                    _doctorService
                        .ApproveAppointment(
                            CurrentUser.UserId,
                            id);

                    ShowSuccess(
                        "Randevu onaylandı.");

                    break;


                case AppointmentAction.Reject:

                    _doctorService
                        .RejectAppointment(
                            CurrentUser.UserId,
                            id);

                    ShowSuccess(
                        "Randevu reddedildi.");

                    break;


                case AppointmentAction.Complete:

                    _doctorService
                        .CompleteAppointment(
                            CurrentUser.UserId,
                            id);

                    ShowSuccess(
                        "Randevu tamamlandı.");

                    break;
            }


            LoadAppointments();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // ÇALIŞMA PROGRAMI
    // =====================================================

    private void ShowSchedule()
    {
        try
        {
            grid.Tag =
                "schedule";


            UiTableHelper.Bind(
                grid,
                _doctorService
                    .GetSchedules(
                        CurrentUser.UserId)
                    .Select(s =>
                        new
                        {
                            Gün =
                                DayText(
                                    s.DayOfWeek),

                            Başlangıç =
                                s.StartTime,

                            Bitiş =
                                s.EndTime
                        }));


            var panel =
                new Panel
                {
                    BackColor =
                        Color.White,

                    Padding =
                        new Padding(15)
                };


            var info =
                new Label
                {
                    Text =
                        "Çalışma programınız yönetici tarafından belirlenmektedir.",

                    Dock =
                        DockStyle.Top,

                    Height =
                        45,

                    ForeColor =
                        UiTheme.Primary,

                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold),

                    Padding =
                        new Padding(
                            5,
                            10,
                            5,
                            5)
                };


            grid.Dock =
                DockStyle.Fill;


            panel.Controls.Add(
                grid);

            panel.Controls.Add(
                info);


            ShowPage(
                "Çalışma Programım",
                panel);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // UI
    // =====================================================

    private static void AddInfoLine(
        Control parent,
        string title,
        string value,
        ref int y)
    {
        var titleLabel =
            new Label
            {
                Text =
                    $"{title}:",

                Font =
                    new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold),

                AutoSize =
                    true,

                Left =
                    30,

                Top =
                    y
            };


        var valueLabel =
            new Label
            {
                Text =
                    value,

                AutoSize =
                    true,

                Left =
                    150,

                Top =
                    y
            };


        parent.Controls.Add(
            titleLabel);

        parent.Controls.Add(
            valueLabel);


        y +=
            35;
    }


    private static string StatusText(
        AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Pending =>
                "Beklemede",

            AppointmentStatus.Approved =>
                "Onaylandı",

            AppointmentStatus.Rejected =>
                "Reddedildi",

            AppointmentStatus.Cancelled =>
                "İptal Edildi",

            AppointmentStatus.Completed =>
                "Tamamlandı",

            _ =>
                "Bilinmiyor"
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


    private enum AppointmentAction
    {
        Approve,
        Reject,
        Complete
    }
}