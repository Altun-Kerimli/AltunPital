using AltunPital.Data;
using AltunPital.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

using Label = System.Windows.Forms.Label;
using Font = System.Drawing.Font;

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


    private Patient GetPatient()
    {
        return AccountService
            .GetUser(
                CurrentUser.UserId)
            as Patient
            ?? throw new Exception(
                "Hasta bilgileri bulunamadı.");
    }


    private void ShowHome()
    {
        var patient =
            GetPatient();


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
                    $"Hoş geldiniz, " +
                    $"{CurrentUser.FirstName} " +
                    $"{CurrentUser.LastName}",

                AutoSize =
                    true,

                Font =
                    new Font(
                        "Segoe UI",
                        22,
                        FontStyle.Bold),

                Left =
                    30,

                Top =
                    30
            };


        var city =
            new Label
            {
                Text =
                    $"Şehriniz: " +
                    $"{patient.City}",

                AutoSize =
                    true,

                ForeColor =
                    UiTheme.Primary,

                Font =
                    new Font(
                        "Segoe UI",
                        11,
                        FontStyle.Bold),

                Left =
                    30,

                Top =
                    90
            };


        var description =
            new Label
            {
                Text =
                    "Yeni randevu oluşturabilir, " +
                    "randevularınızı takip edebilir ve " +
                    "hesap bilgilerinizi yönetebilirsiniz.",

                AutoSize =
                    true,

                Left =
                    30,

                Top =
                    130
            };


        panel.Controls.Add(
            title);

        panel.Controls.Add(
            city);

        panel.Controls.Add(
            description);


        ShowPage(
            "Ana Sayfa",
            panel);
    }


    private void ShowCreateAppointment()
    {
        var patient =
            GetPatient();


        var panel =
            new Panel
            {
                AutoScroll =
                    true,

                BackColor =
                    Color.White,

                Padding =
                    new Padding(25)
            };


        var cbBranch =
            new ComboBox
            {
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };

        var cbDepartment =
            new ComboBox
            {
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };

        var cbDoctor =
            new ComboBox
            {
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };

        var cbService =
            new ComboBox
            {
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };


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
            25;


        var cityInfo =
            new Label
            {
                Text =
                    string.IsNullOrWhiteSpace(
                        patient.City)
                        ? "Tüm şubeler gösteriliyor."
                        : $"{patient.City} şubeleri listenin başında gösteriliyor.",

                ForeColor =
                    UiTheme.Primary,

                AutoSize =
                    true,

                Left =
                    25,

                Top =
                    y
            };


        panel.Controls.Add(
            cityInfo);

        y +=
            45;


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
                    25,

                Top =
                    y,

                AutoSize =
                    true
            };


        slots.Left =
            25;

        slots.Top =
            y + 28;

        slots.Width =
            360;

        slots.Height =
            180;


        panel.Controls.Add(
            slotLabel);

        panel.Controls.Add(
            slots);


        var loadSlots =
            UiTheme.CreateButton(
                "Uygun Saatleri Göster",
                230);

        loadSlots.Left =
            420;

        loadSlots.Top =
            y + 28;


        var create =
            UiTheme.CreateButton(
                "Randevu Oluştur",
                230);

        create.Left =
            420;

        create.Top =
            y + 85;


        panel.Controls.Add(
            loadSlots);

        panel.Controls.Add(
            create);


        var branches =
            _patientService
                .GetBranches(
                    patient.City);


        cbBranch.DataSource =
            branches;

        cbBranch.DisplayMember =
            nameof(Branch.Name);


        void LoadDepartments()
        {
            if (cbBranch.SelectedItem
                is not Branch branch)
                return;


            cbDepartment.DataSource =
                _patientService
                    .GetDepartments(
                        branch.BranchId);

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
                cbDoctor.DataSource =
                    null;

                return;
            }


            cbDoctor.DataSource =
                _patientService
                    .GetDoctors(
                        branch.BranchId,
                        department.DepartmentId);

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
                cbService.DataSource =
                    null;

                return;
            }


            cbService.DataSource =
                _patientService
                    .GetServices(
                        branch.BranchId,
                        doctor.UserId);

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


                    availableSlots =
                        _patientService
                            .GetAvailableSlots(
                                doctor.UserId,
                                service.HealthServiceId,
                                DateOnly.FromDateTime(
                                    datePicker.Value));


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
                            "Bilgi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
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
                            "Şube, doktor ve hizmet seçiniz.");
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
                        "Randevu başarıyla oluşturuldu.\n\n" +
                        $"Randevu ID: {appointment.AppointmentId}");


                    ShowAppointments();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            };


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
            new Panel
            {
                BackColor =
                    Color.White,

                Padding =
                    new Padding(15)
            };


        var footer =
            new Panel
            {
                Dock =
                    DockStyle.Bottom,

                Height =
                    70,

                Padding =
                    new Padding(
                        5,
                        10,
                        5,
                        5)
            };


        var cancel =
            UiTheme.CreateButton(
                "Seçili Randevuyu İptal Et",
                250);


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


                    DialogResult confirm =
                        MessageBox.Show(
                            "Seçili randevuyu iptal etmek istiyor musunuz?",
                            "Randevu İptali",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);


                    if (confirm !=
                        DialogResult.Yes)
                    {
                        return;
                    }


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


        appointmentsGrid.Dock =
            DockStyle.Fill;


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
                            StatusText(
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
                    25,

                Top =
                    y,

                Width =
                    180
            };


        control.Left =
            25;

        control.Top =
            y + 25;

        control.Width =
            360;


        parent.Controls.Add(
            label);

        parent.Controls.Add(
            control);


        y +=
            75;
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

            RowHeadersVisible =
                false,

            SelectionMode =
                DataGridViewSelectionMode
                    .FullRowSelect,

            AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode
                    .Fill,

            BackgroundColor =
                Color.White,

            BorderStyle =
                BorderStyle.None,

            RowTemplate =
            {
                Height = 32
            }
        };
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
}