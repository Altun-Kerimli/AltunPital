using AltunPital.Data;
using AltunPital.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AltunPital.UI.Desktop;

public class DoctorForm :
    DesktopShellForm
{
    private readonly IDoctorService
        _doctorService;

    private readonly DataGridView
        grid =
            new()
            {
                ReadOnly = true,

                AllowUserToAddRows =
                    false,

                SelectionMode =
                    DataGridViewSelectionMode
                        .FullRowSelect,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode
                        .Fill,

                BackgroundColor =
                    Color.White
            };


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
            "Randevularım",
            160,
            ShowAppointments);

        AddSidebarButton(
            "Çalışma Programım",
            210,
            ShowSchedule);

        AddSidebarButton(
            "Profilim",
            260,
            OpenProfile);

        AddLogoutButton();


        ShowAppointments();
    }


    private void ShowAppointments()
    {
        var panel =
            new Panel();


        grid.Dock =
            DockStyle.Fill;


        var buttons =
            new FlowLayoutPanel
            {
                Dock =
                    DockStyle.Bottom,

                Height =
                    70,

                FlowDirection =
                    FlowDirection.LeftToRight,

                Padding =
                    new Padding(10)
            };


        var approve =
            UiTheme.CreateButton(
                "Onayla");

        var reject =
            UiTheme.CreateButton(
                "Reddet");

        var complete =
            UiTheme.CreateButton(
                "Tamamla");


        approve.Click +=
            (_, _) =>
                ChangeStatus(
                    "onayla");

        reject.Click +=
            (_, _) =>
                ChangeStatus(
                    "reddet");

        complete.Click +=
            (_, _) =>
                ChangeStatus(
                    "tamamla");


        buttons.Controls.Add(
            approve);

        buttons.Controls.Add(
            reject);

        buttons.Controls.Add(
            complete);


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


        grid.DataSource =
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

                        Hizmet =
                            a.HealthService.Name,

                        Durum =
                            StatusText(
                                a.Status)
                    })
                .ToList();
    }


    private void ChangeStatus(
        string action)
    {
        try
        {
            if ((string?)grid.Tag !=
                "appointments")
            {
                throw new Exception(
                    "Önce randevularınızı görüntüleyiniz.");
            }


            if (grid.CurrentRow ==
                null)
            {
                throw new Exception(
                    "Bir randevu seçiniz.");
            }


            int id =
                Convert.ToInt32(
                    grid.CurrentRow
                        .Cells["ID"]
                        .Value);


            switch (action)
            {
                case "onayla":

                    _doctorService
                        .ApproveAppointment(
                            CurrentUser.UserId,
                            id);

                    break;


                case "reddet":

                    _doctorService
                        .RejectAppointment(
                            CurrentUser.UserId,
                            id);

                    break;


                case "tamamla":

                    _doctorService
                        .CompleteAppointment(
                            CurrentUser.UserId,
                            id);

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
        grid.Tag =
            "schedule";


        grid.DataSource =
            _doctorService
                .GetSchedules(
                    CurrentUser.UserId)
                .Select(s =>
                    new
                    {
                        Gun =
                            DayText(
                                s.DayOfWeek),

                        Baslangic =
                            s.StartTime,

                        Bitis =
                            s.EndTime
                    })
                .ToList();


        ShowPage(
            "Çalışma Programım",
            grid);
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
}