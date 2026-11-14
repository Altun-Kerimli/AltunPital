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

    private readonly DataGridView appointmentsGrid = UiTableHelper.CreateGrid();

    public PatientForm(
        User user,
        IPatientService patientService,
        IUserAccountService accountService)
        : base(user, accountService, "Hasta")
    {
        _patientService = patientService;

        AddSidebarButton("Ana Sayfa", 160, ShowHome);
        AddSidebarButton("Randevu Al", 210, ShowCreateAppointment);
        AddSidebarButton("Randevularım", 260, ShowAppointments);
        AddSidebarButton("Profilim", 310, OpenProfile);
        AddLogoutButton();

        ShowHome();
    }

    private Patient GetPatient()
    {
        return AccountService
            .GetUser(CurrentUser.UserId)
            as Patient
            ?? throw new Exception("Hasta bilgileri bulunamadı.");
    }

    private void ShowHome()
    {
        try
        {
            Patient patient = GetPatient();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(35)
            };

            var title = new Label
            {
                Text = $"Hoş geldiniz, {patient.FirstName} {patient.LastName}",
                AutoSize = true,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                Left = 35,
                Top = 35
            };

            panel.Controls.Add(title);

            int y = 100;

            AddInfo(panel, "Şehir", string.IsNullOrWhiteSpace(patient.City) ? "-" : patient.City, ref y);
            AddInfo(panel, "Telefon", patient.Phone, ref y);
            AddInfo(panel, "E-posta", patient.Email, ref y);

            y += 15;

            var info = new Label
            {
                Text = "Randevu oluşturmak için şube, bölüm, doktor, hizmet, tarih ve uygun saat seçiniz.",
                Left = 35,
                Top = y,
                Width = 760,
                Height = 55,
                ForeColor = Color.DimGray
            };

            panel.Controls.Add(info);

            ShowPage("Ana Sayfa", panel);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ShowCreateAppointment()
    {
        try
        {
            Patient patient = GetPatient();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(30)
            };

            int y = 25;

            var info = new Label
            {
                Text = string.IsNullOrWhiteSpace(patient.City)
                    ? "Tüm şubeler gösteriliyor."
                    : $"{patient.City} şehrindeki şubeler öncelikli gösteriliyor.",
                Left = 30,
                Top = y,
                Width = 760,
                Height = 40,
                ForeColor = UiTheme.Primary
            };

            panel.Controls.Add(info);

            y += 55;

            ComboBox cmbBranch = CreateCombo();
            ComboBox cmbDepartment = CreateCombo();
            ComboBox cmbDoctor = CreateCombo();
            ComboBox cmbService = CreateCombo();

            DateTimePicker datePicker = new()
            {
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today,
                Value = DateTime.Today
            };

            ListBox slotList = new()
            {
                Width = 360,
                Height = 180,
                IntegralHeight = false
            };

            AddField(panel, "Şube", cmbBranch, ref y);
            AddField(panel, "Bölüm", cmbDepartment, ref y);
            AddField(panel, "Doktor", cmbDoctor, ref y);
            AddField(panel, "Hizmet", cmbService, ref y);
            AddField(panel, "Tarih", datePicker, ref y);

            var slotTitle = new Label
            {
                Text = "Uygun Saatler",
                Left = 30,
                Top = y,
                AutoSize = true
            };

            slotList.Left = 30;
            slotList.Top = y + 27;

            panel.Controls.Add(slotTitle);
            panel.Controls.Add(slotList);

            var loadSlots = UiTheme.CreateButton("Uygun Saatleri Göster", 220);
            loadSlots.Left = 425;
            loadSlots.Top = y + 27;

            var create = UiTheme.CreateButton("Randevu Oluştur", 220);
            create.Left = 425;
            create.Top = y + 82;

            panel.Controls.Add(loadSlots);
            panel.Controls.Add(create);

            List<DateTime> availableSlots = new();
            bool internalUpdate = false;

            cmbBranch.Format += (_, e) =>
            {
                if (e.ListItem is Branch branch)
                {
                    e.Value = $"{branch.Name} - {branch.City}";
                }
            };

            cmbDoctor.Format += (_, e) =>
            {
                if (e.ListItem is Doctor doctor)
                {
                    string city = doctor.Branch?.City ?? "";

                    string cityText = string.IsNullOrWhiteSpace(city)
                        ? ""
                        : $" {city}";

                    e.Value = $"{doctor.FirstName} " +
                        $"{doctor.LastName}" +
                        cityText +
                        $" - {doctor.Specialization}";
                }
            };

            void ClearSlots()
            {
                availableSlots.Clear();
                slotList.Items.Clear();
            }

            void ClearCombo(ComboBox combo)
            {
                combo.DataSource = null;
                combo.Items.Clear();
                combo.Text = "";
            }

            void LoadDepartments()
            {
                if (internalUpdate)
                {
                    return;
                }

                internalUpdate = true;

                try
                {
                    ClearCombo(cmbDepartment);
                    ClearCombo(cmbDoctor);
                    ClearCombo(cmbService);
                    ClearSlots();

                    if (cmbBranch.SelectedItem is not Branch branch)
                    {
                        return;
                    }

                    List<Department> departments = _patientService
                        .GetDepartments(branch.BranchId);

                    if (departments.Count == 0)
                    {
                        return;
                    }

                    cmbDepartment.DisplayMember = nameof(Department.Name);
                    cmbDepartment.ValueMember = nameof(Department.DepartmentId);
                    cmbDepartment.DataSource = departments;
                }
                finally
                {
                    internalUpdate = false;
                }

                LoadDoctors();
            }

            void LoadDoctors()
            {
                if (internalUpdate)
                {
                    return;
                }

                internalUpdate = true;

                try
                {
                    ClearCombo(cmbDoctor);
                    ClearCombo(cmbService);
                    ClearSlots();

                    if (cmbBranch.SelectedItem is not Branch branch ||
                        cmbDepartment.SelectedItem is not Department department)
                    {
                        return;
                    }

                    List<Doctor> doctors = _patientService
                        .GetDoctors(branch.BranchId, department.DepartmentId);

                    if (doctors.Count == 0)
                    {
                        return;
                    }

                    cmbDoctor.DisplayMember = "";
                    cmbDoctor.ValueMember = nameof(Doctor.UserId);
                    cmbDoctor.DataSource = doctors;
                }
                finally
                {
                    internalUpdate = false;
                }

                LoadServices();
            }

            void LoadServices()
            {
                if (internalUpdate)
                {
                    return;
                }

                internalUpdate = true;

                try
                {
                    ClearCombo(cmbService);
                    ClearSlots();

                    if (cmbBranch.SelectedItem is not Branch branch ||
                        cmbDoctor.SelectedItem is not Doctor doctor)
                    {
                        return;
                    }

                    List<HealthService> services = _patientService
                        .GetServices(branch.BranchId, doctor.UserId);

                    if (services.Count == 0)
                    {
                        return;
                    }

                    cmbService.DisplayMember = nameof(HealthService.Name);
                    cmbService.ValueMember = nameof(HealthService.HealthServiceId);
                    cmbService.DataSource = services;
                }
                finally
                {
                    internalUpdate = false;
                }
            }

            cmbBranch.SelectedIndexChanged += (_, _) =>
            {
                if (!internalUpdate)
                {
                    LoadDepartments();
                }
            };

            cmbDepartment.SelectedIndexChanged += (_, _) =>
            {
                if (!internalUpdate)
                {
                    LoadDoctors();
                }
            };

            cmbDoctor.SelectedIndexChanged += (_, _) =>
            {
                if (!internalUpdate)
                {
                    LoadServices();
                }
            };

            cmbService.SelectedIndexChanged += (_, _) =>
            {
                if (!internalUpdate)
                {
                    ClearSlots();
                }
            };

            datePicker.ValueChanged += (_, _) =>
                ClearSlots();

            loadSlots.Click += (_, _) =>
            {
                try
                {
                    if (cmbDoctor.SelectedItem is not Doctor doctor)
                    {
                        throw new Exception("Seçilen bölümde uygun doktor bulunmuyor.");
                    }

                    if (cmbService.SelectedItem is not HealthService service)
                    {
                        throw new Exception("Seçilen doktor için aktif hizmet bulunmuyor.");
                    }

                    availableSlots = _patientService
                        .GetAvailableSlots(
                            doctor.UserId,
                            service.HealthServiceId,
                            DateOnly.FromDateTime(datePicker.Value));

                    slotList.Items.Clear();

                    foreach (DateTime slot in availableSlots)
                    {
                        slotList.Items.Add(slot.ToString("HH:mm"));
                    }

                    if (availableSlots.Count == 0)
                    {
                        MessageBox.Show(
                            "Seçilen tarihte doktorun uygun çalışma saati bulunmuyor veya tüm saatler dolu.",
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

            create.Click += (_, _) =>
            {
                try
                {
                    if (cmbBranch.SelectedItem is not Branch branch)
                    {
                        throw new Exception("Şube seçiniz.");
                    }

                    if (cmbDepartment.SelectedItem is not Department)
                    {
                        throw new Exception("Bölüm seçiniz.");
                    }

                    if (cmbDoctor.SelectedItem is not Doctor doctor)
                    {
                        throw new Exception("Doktor seçiniz.");
                    }

                    if (cmbService.SelectedItem is not HealthService service)
                    {
                        throw new Exception("Hizmet seçiniz.");
                    }

                    if (slotList.SelectedIndex < 0 ||
                        slotList.SelectedIndex >= availableSlots.Count)
                    {
                        throw new Exception("Uygun saatlerden birini seçiniz.");
                    }

                    DateTime selectedTime = availableSlots[slotList.SelectedIndex];

                    DialogResult result = MessageBox.Show(
                        $"Şube: {branch.Name}\n" +
                        $"Doktor: {doctor.FirstName} {doctor.LastName}\n" +
                        $"Hizmet: {service.Name}\n" +
                        $"Tarih: {selectedTime:dd.MM.yyyy}\n" +
                        $"Saat: {selectedTime:HH:mm}\n\n" +
                        "Randevu oluşturulsun mu?",
                        "Randevu Onayı",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }

                    Appointment appointment = _patientService
                        .CreateAppointment(
                            patient.UserId,
                            doctor.UserId,
                            branch.BranchId,
                            service.HealthServiceId,
                            selectedTime);

                    ShowSuccess($"Randevu başarıyla oluşturuldu.\nRandevu No: {appointment.AppointmentId}");

                    ShowAppointments();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            };

            internalUpdate = true;

            try
            {
                List<Branch> branches = _patientService
                    .GetBranches(patient.City);

                if (branches.Count > 0)
                {
                    // Format çalışsın diye önce DisplayMember, sonra DataSource
                    cmbBranch.DisplayMember = nameof(Branch.Name);
                    cmbBranch.ValueMember = nameof(Branch.BranchId);
                    cmbBranch.DataSource = branches;
                }
            }
            finally
            {
                internalUpdate = false;
            }

            LoadDepartments();

            ShowPage("Randevu Al", panel);
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

        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            Padding = new Padding(5),
            FlowDirection = FlowDirection.LeftToRight
        };

        var refresh = UiTheme.CreateButton("Yenile", 130);
        var cancel = UiTheme.CreateButton("Randevuyu İptal Et", 220);

        refresh.Click += (_, _) =>
            LoadAppointments();

        cancel.Click += (_, _) =>
        {
            try
            {
                int id = UiTableHelper
                    .SelectedInt(appointmentsGrid, "ID");

                DialogResult result = MessageBox.Show(
                    "Seçili randevu iptal edilsin mi?",
                    "Randevu İptali",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                _patientService
                    .CancelAppointment(id);

                ShowSuccess("Randevu iptal edildi.");

                LoadAppointments();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        };

        footer.Controls.Add(refresh);
        footer.Controls.Add(cancel);

        appointmentsGrid.Dock = DockStyle.Fill;

        panel.Controls.Add(appointmentsGrid);
        panel.Controls.Add(footer);

        LoadAppointments();

        ShowPage("Randevularım", panel);
    }

    private void LoadAppointments()
    {
        UiTableHelper.Bind(
            appointmentsGrid,
            _patientService
                .GetAppointments(CurrentUser.UserId)
                .Select(a =>
                    new
                    {
                        ID = a.AppointmentId,
                        Tarih = a.StartDateTime,
                        Şube = a.Branch.Name,
                        Doktor = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
                        Bölüm = a.Doctor.Department?.Name ?? "-",
                        Hizmet = a.HealthService.Name,
                        Başlangıç = a.StartDateTime.ToString("HH:mm"),
                        Bitiş = a.EndDateTime.ToString("HH:mm"),
                        Süre = (int)(a.EndDateTime - a.StartDateTime).TotalMinutes,
                        Fiyat = a.Price,
                        Durum = StatusText(a.Status)
                    }));
    }

    private static ComboBox CreateCombo()
    {
        return new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            // Format eventinin çalışması için gerekli
            FormattingEnabled = true
        };
    }

    private static void AddField(Control parent, string title, Control control, ref int y)
    {
        parent.Controls.Add(new Label
        {
            Text = title,
            Left = 30,
            Top = y,
            Width = 180
        });

        control.Left = 30;
        control.Top = y + 24;
        control.Width = 360;

        parent.Controls.Add(control);

        y += 72;
    }

    private static void AddInfo(Control parent, string title, string value, ref int y)
    {
        parent.Controls.Add(new Label
        {
            Text = title,
            Left = 35,
            Top = y,
            Width = 150,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        });

        parent.Controls.Add(new Label
        {
            Text = string.IsNullOrWhiteSpace(value) ? "-" : value,
            Left = 200,
            Top = y,
            Width = 450
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
}
