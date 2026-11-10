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

public class PatientForm :
    DesktopShellForm
{
    private readonly IPatientService
        _patientService;

    private readonly DataGridView
        appointmentsGrid =
            CreateGrid();


    public PatientForm(
        User user,
        IPatientService patientService,
        IUserAccountService accountService)
        : base(
            user,
            accountService,
            "Hasta")
    {
        _patientService =
            patientService;


        AddSidebarButton(
            "Ana Sayfa",
            160,
            ShowHome);

        AddSidebarButton(
            "Randevu Al",
            210,
            ShowCreateAppointment);

        AddSidebarButton(
            "Randevularım",
            260,
            ShowAppointments);

        AddSidebarButton(
            "Profilim",
            310,
            OpenProfile);

        AddLogoutButton();


        ShowHome();
    }


    private void ShowHome()
    {
        var panel =
            new Panel();


        var title =
            new Label
            {
                Text =
                    $"Hoş geldiniz, " +
                    $"{CurrentUser.FirstName} " +
                    $"{CurrentUser.LastName}",

                Font =
                    new Font(
                        "Segoe UI",
                        22,
                        FontStyle.Bold),

                AutoSize =
                    true,

                Left =
                    20,

                Top =
                    25
            };


        var info =
            new Label
            {
                Text =
                    "AltunPital üzerinden yeni randevu oluşturabilir, " +
                    "mevcut randevularınızı görüntüleyebilir ve " +
                    "profil bilgilerinizi yönetebilirsiniz.",

                Width =
                    750,

                Height =
                    80,

                Left =
                    20,

                Top =
                    90
            };


        panel.Controls.Add(
            title);

        panel.Controls.Add(
            info);


        ShowPage(
            "Ana Sayfa",
            panel);
    }


    private void ShowCreateAppointment()
    {
        var panel =
            new Panel
            {
                AutoScroll =
                    true
            };


        var cbBranch =
            new ComboBox();

        var cbDepartment =
            new ComboBox();

        var cbDoctor =
            new ComboBox();

        var cbService =
            new ComboBox();

        var datePicker =
            new DateTimePicker
            {
                Format =
                    DateTimePickerFormat.Short,

                MinDate =
                    DateTime.Today
            };

        var slots =
            new ListBox();


        int y =
            20;


        AddField(
            panel,
            "Şube",
            cbBranch,
            ref y);

        AddField(
            panel,
            "Bölüm",
            cbDepartment,
            ref y);

        AddField(
            panel,
            "Doktor",
            cbDoctor,
            ref y);

        AddField(
            panel,
            "Hizmet",
            cbService,
            ref y);

        AddField(
            panel,
            "Tarih",
            datePicker,
            ref y);


        var slotLabel =
            new Label
            {
                Text =
                    "Uygun Saatler",

                Left =
                    20,

                Top =
                    y,

                AutoSize =
                    true
            };


        slots.Left =
            20;

        slots.Top =
            y + 25;

        slots.Width =
            350;

        slots.Height =
            160;


        panel.Controls.Add(
            slotLabel);

        panel.Controls.Add(
            slots);


        var loadSlots =
            UiTheme.CreateButton(
                "Uygun Saatleri Göster",
                220);

        loadSlots.Left =
            400;

        loadSlots.Top =
            y + 25;


        var create =
            UiTheme.CreateButton(
                "Randevu Oluştur",
                220);

        create.Left =
            400;

        create.Top =
            y + 80;


        var branches =
            _patientService
                .GetBranches();


        cbBranch.DataSource =
            branches;

        cbBranch.DisplayMember =
            nameof(Branch.Name);


        void LoadDepartments()
        {
            if (cbBranch.SelectedItem
                is not Branch branch)
            {
                return;
            }


            var departments =
                _patientService
                    .GetDepartments(
                        branch.BranchId);


            cbDepartment.DataSource =
                departments;

            cbDepartment.DisplayMember =
                nameof(Department.Name);
        }


        void LoadDoctors()
        {
            if (cbBranch.SelectedItem
                    is not Branch branch ||
                cbDepartment.SelectedItem
                    is not Department department)
            {
                return;
            }


            var doctors =
                _patientService
                    .GetDoctors(
                        branch.BranchId,
                        department.DepartmentId);


            cbDoctor.DataSource =
                doctors;

            cbDoctor.DisplayMember =
                nameof(Doctor.LastName);
        }


        void LoadServices()
        {
            if (cbBranch.SelectedItem
                    is not Branch branch ||
                cbDoctor.SelectedItem
                    is not Doctor doctor)
            {
                return;
            }


            var services =
                _patientService
                    .GetServices(
                        branch.BranchId,
                        doctor.UserId);


            cbService.DataSource =
                services;

            cbService.DisplayMember =
                nameof(HealthService.Name);
        }


        cbBranch.SelectedIndexChanged +=
            (_, _) =>
            {
                LoadDepartments();
                LoadDoctors();
                LoadServices();
            };


        cbDepartment.SelectedIndexChanged +=
            (_, _) =>
            {
                LoadDoctors();
                LoadServices();
            };


        cbDoctor.SelectedIndexChanged +=
            (_, _) =>
                LoadServices();


        List<DateTime> availableSlots =
            new();


        loadSlots.Click +=
            (_, _) =>
            {
                try
                {
                    if (cbDoctor.SelectedItem
                            is not Doctor doctor ||
                        cbService.SelectedItem
                            is not HealthService service)
                    {
                        throw new Exception(
                            "Doktor ve hizmet seçiniz.");
                    }


                    DateOnly date =
                        DateOnly.FromDateTime(
                            datePicker.Value);


                    availableSlots =
                        _patientService
                            .GetAvailableSlots(
                                doctor.UserId,
                                service.HealthServiceId,
                                date);


                    slots.Items.Clear();


                    foreach (var slot
                             in availableSlots)
                    {
                        slots.Items.Add(
                            slot.ToString(
                                "HH:mm"));
                    }


                    if (availableSlots.Count ==
                        0)
                    {
                        MessageBox.Show(
                            "Bu tarihte uygun saat bulunamadı.",
                            "Bilgi");
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            };


        create.Click +=
            (_, _) =>
            {
                try
                {
                    if (cbBranch.SelectedItem
                            is not Branch branch ||
                        cbDoctor.SelectedItem
                            is not Doctor doctor ||
                        cbService.SelectedItem
                            is not HealthService service)
                    {
                        throw new Exception(
                            "Tüm alanları seçiniz.");
                    }


                    if (slots.SelectedIndex <
                        0)
                    {
                        throw new Exception(
                            "Bir randevu saati seçiniz.");
                    }


                    DateTime selected =
                        availableSlots[
                            slots.SelectedIndex];


                    var appointment =
                        _patientService
                            .CreateAppointment(
                                CurrentUser.UserId,
                                doctor.UserId,
                                branch.BranchId,
                                service.HealthServiceId,
                                selected);


                    ShowSuccess(
                        $"Randevu oluşturuldu.\n" +
                        $"Randevu ID: {appointment.AppointmentId}");


                    ShowAppointments();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            };


        panel.Controls.Add(
            loadSlots);

        panel.Controls.Add(
            create);


        LoadDepartments();
        LoadDoctors();
        LoadServices();


        ShowPage(
            "Randevu Al",
            panel);
    }


    private void ShowAppointments()
    {
        var panel =
            new Panel();


        appointmentsGrid.Dock =
            DockStyle.Fill;


        var footer =
            new Panel
            {
                Dock =
                    DockStyle.Bottom,

                Height =
                    70
            };


        var cancel =
            UiTheme.CreateButton(
                "Seçili Randevuyu İptal Et",
                240);

        cancel.Left =
            10;

        cancel.Top =
            12;


        cancel.Click +=
            (_, _) =>
            {
                try
                {
                    if (appointmentsGrid
                            .CurrentRow ==
                        null)
                    {
                        throw new Exception(
                            "Bir randevu seçiniz.");
                    }


                    int id =
                        Convert.ToInt32(
                            appointmentsGrid
                                .CurrentRow
                                .Cells["ID"]
                                .Value);


                    _patientService
                        .CancelAppointment(
                            id);


                    ShowSuccess(
                        "Randevu iptal edildi.");

                    LoadAppointments();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            };


        footer.Controls.Add(
            cancel);


        panel.Controls.Add(
            appointmentsGrid);

        panel.Controls.Add(
            footer);


        LoadAppointments();


        ShowPage(
            "Randevularım",
            panel);
    }


    private void LoadAppointments()
    {
        appointmentsGrid.DataSource =
            _patientService
                .GetAppointments(
                    CurrentUser.UserId)
                .Select(a =>
                    new
                    {
                        ID =
                            a.AppointmentId,

                        Tarih =
                            a.StartDateTime,

                        Doktor =
                            $"{a.Doctor.FirstName} " +
                            $"{a.Doctor.LastName}",

                        Hizmet =
                            a.HealthService.Name,

                        Durum =
                            AppointmentStatusText(
                                a.Status),

                        Fiyat =
                            a.Price
                    })
                .ToList();
    }


    private static void AddField(
        Control parent,
        string title,
        Control control,
        ref int y)
    {
        var label =
            new Label
            {
                Text =
                    title,

                Left =
                    20,

                Top =
                    y,

                Width =
                    180
            };


        control.Left =
            20;

        control.Top =
            y + 25;

        control.Width =
            350;


        parent.Controls.Add(
            label);

        parent.Controls.Add(
            control);


        y +=
            70;
    }


    private static DataGridView
        CreateGrid()
    {
        return new DataGridView
        {
            ReadOnly =
                true,

            AllowUserToAddRows =
                false,

            AllowUserToDeleteRows =
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
    }


    private static string
        AppointmentStatusText(
            AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Pending
                => "Beklemede",

            AppointmentStatus.Approved
                => "Onaylandı",

            AppointmentStatus.Rejected
                => "Reddedildi",

            AppointmentStatus.Cancelled
                => "İptal Edildi",

            AppointmentStatus.Completed
                => "Tamamlandı",

            _ => "Bilinmiyor"
        };
    }
}