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


using Font = System.Drawing.Font;
using Label = System.Windows.Forms.Label;

namespace AltunPital.UI.Desktop;

public class PatientForm : DesktopShellForm
{
    private readonly IPatientService _patientService;

    private readonly DataGridView appointmentsGrid =
        UiTableHelper.CreateGrid();


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


    // =====================================================
    // HASTA
    // =====================================================

    private Patient GetPatient()
    {
        return AccountService
            .GetUser(
                CurrentUser.UserId)
            as Patient
            ?? throw new Exception(
                "Hasta bilgileri bulunamadı.");
    }


    // =====================================================
    // ANA SAYFA
    // =====================================================

    private void ShowHome()
    {
        try
        {
            Patient patient =
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
                        string.IsNullOrWhiteSpace(
                            patient.City)
                            ? "Şehir bilgisi girilmemiş."
                            : $"Şehriniz: {patient.City}",

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
                        "AltunPital üzerinden yeni randevu oluşturabilir, " +
                        "mevcut randevularınızı takip edebilir ve " +
                        "hesap bilgilerinizi yönetebilirsiniz.",

                    Width =
                        800,

                    Height =
                        70,

                    Left =
                        30,

                    Top =
                        135
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
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // RANDEVU AL
    // =====================================================

    private void ShowCreateAppointment()
    {
        try
        {
            Patient patient =
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
                new ListBox
                {
                    IntegralHeight =
                        false
                };


            int y =
                25;


            var cityInfo =
                new Label
                {
                    Text =
                        string.IsNullOrWhiteSpace(
                            patient.City)
                            ? "Tüm şubeler gösteriliyor."
                            : $"{patient.City} şehrindeki şubeler listenin başında gösterilir. " +
                              "Diğer şehirlerdeki şubeleri de seçebilirsiniz.",

                    ForeColor =
                        UiTheme.Primary,

                    AutoSize =
                        false,

                    Width =
                        750,

                    Height =
                        45,

                    Left =
                        25,

                    Top =
                        y
                };


            panel.Controls.Add(
                cityInfo);


            y +=
                55;


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
                190;


            panel.Controls.Add(
                slotLabel);

            panel.Controls.Add(
                slots);


            var loadSlotsButton =
                UiTheme.CreateButton(
                    "Uygun Saatleri Göster",
                    230);

            loadSlotsButton.Left =
                420;

            loadSlotsButton.Top =
                y + 28;


            var createButton =
                UiTheme.CreateButton(
                    "Randevu Oluştur",
                    230);

            createButton.Left =
                420;

            createButton.Top =
                y + 85;


            panel.Controls.Add(
                loadSlotsButton);

            panel.Controls.Add(
                createButton);


            List<DateTime> availableSlots =
                new();


            // -------------------------------------------------
            // ŞUBELER
            // -------------------------------------------------

            var branches =
                _patientService
                    .GetBranches(
                        patient.City);


            cbBranch.DataSource =
                branches;

            cbBranch.DisplayMember =
                nameof(Branch.Name);


            // -------------------------------------------------
            // BÖLÜMLER
            // -------------------------------------------------

            void LoadDepartments()
            {
                cbDepartment.DataSource =
                    null;

                cbDoctor.DataSource =
                    null;

                cbService.DataSource =
                    null;

                slots.Items.Clear();


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


            // -------------------------------------------------
            // DOKTORLAR
            // -------------------------------------------------

            void LoadDoctors()
            {
                cbDoctor.DataSource =
                    null;

                cbService.DataSource =
                    null;

                slots.Items.Clear();


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


                cbDoctor.Format -=
                    DoctorComboFormat;

                cbDoctor.Format +=
                    DoctorComboFormat;
            }


            void DoctorComboFormat(
                object? sender,
                ListControlConvertEventArgs e)
            {
                if (e.ListItem
                    is Doctor doctor)
                {
                    e.Value =
                        $"{doctor.FirstName} " +
                        $"{doctor.LastName} - " +
                        $"{doctor.Specialization}";
                }
            }


            // -------------------------------------------------
            // HİZMETLER
            // -------------------------------------------------

            void LoadServices()
            {
                cbService.DataSource =
                    null;

                slots.Items.Clear();


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


            // -------------------------------------------------
            // EVENTS
            // -------------------------------------------------

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
                {
                    LoadServices();
                };


            cbService.SelectedIndexChanged +=
                (_, _) =>
                {
                    slots.Items.Clear();
                    availableSlots.Clear();
                };


            datePicker.ValueChanged +=
                (_, _) =>
                {
                    slots.Items.Clear();
                    availableSlots.Clear();
                };


            // -------------------------------------------------
            // UYGUN SAATLER
            // -------------------------------------------------

            loadSlotsButton.Click +=
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


                        foreach (DateTime slot
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
                                "Seçilen tarihte uygun randevu saati bulunamadı.",
                                "Uygun Saat Bulunamadı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex);
                    }
                };


            // -------------------------------------------------
            // RANDEVU OLUŞTUR
            // -------------------------------------------------

            createButton.Click +=
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
                                "Uygun saatlerden birini seçiniz.");
                        }


                        if (slots.SelectedIndex >=
                            availableSlots.Count)
                        {
                            throw new Exception(
                                "Seçilen randevu saati geçersiz.");
                        }


                        DateTime selectedDateTime =
                            availableSlots[
                                slots.SelectedIndex];


                        DialogResult confirm =
                            MessageBox.Show(
                                $"{doctor.FirstName} {doctor.LastName}\n" +
                                $"{service.Name}\n" +
                                $"{selectedDateTime:dd.MM.yyyy HH:mm}\n\n" +
                                "Randevuyu oluşturmak istiyor musunuz?",
                                "Randevu Onayı",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);


                        if (confirm !=
                            DialogResult.Yes)
                        {
                            return;
                        }


                        var appointment =
                            _patientService
                                .CreateAppointment(
                                    CurrentUser.UserId,
                                    doctor.UserId,
                                    branch.BranchId,
                                    service.HealthServiceId,
                                    selectedDateTime);


                        ShowSuccess(
                            "Randevu başarıyla oluşturuldu.\n\n" +
                            $"Randevu No: {appointment.AppointmentId}");


                        ShowAppointments();
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex);
                    }
                };


            LoadDepartments();


            ShowPage(
                "Randevu Al",
                panel);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    // =====================================================
    // RANDEVULARIM
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


        var footer =
            new FlowLayoutPanel
            {
                Dock =
                    DockStyle.Bottom,

                Height =
                    65,

                Padding =
                    new Padding(
                        5,
                        10,
                        5,
                        5),

                FlowDirection =
                    FlowDirection.LeftToRight,

                BackColor =
                    Color.White
            };


        var refreshButton =
            UiTheme.CreateButton(
                "Yenile",
                140);


        var cancelButton =
            UiTheme.CreateButton(
                "Seçili Randevuyu İptal Et",
                250);


        refreshButton.Click +=
            (_, _) =>
                LoadAppointments();


        cancelButton.Click +=
            (_, _) =>
            {
                try
                {
                    int id =
                        UiTableHelper
                            .SelectedInt(
                                appointmentsGrid,
                                "ID");


                    DialogResult confirm =
                        MessageBox.Show(
                            "Seçili randevuyu iptal etmek istiyor musunuz?",
                            "Randevu İptali",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);


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
            refreshButton);

        footer.Controls.Add(
            cancelButton);


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
        UiTableHelper.Bind(
            appointmentsGrid,
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

                        Şube =
                            a.Branch.Name,

                        Doktor =
                            $"{a.Doctor.FirstName} " +
                            $"{a.Doctor.LastName}",

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


    // =====================================================
    // UI YARDIMCILARI
    // =====================================================

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