using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.UI.Desktop;

public class ScheduleEditForm : Form
{
    private readonly ComboBox cmbDoctor;
    private readonly ComboBox cmbDay;
    private readonly DateTimePicker dtStart;
    private readonly DateTimePicker dtEnd;

    public Doctor SelectedDoctor => (Doctor)cmbDoctor.SelectedItem!;
    public DayOfWeek SelectedDay => ((DayItem)cmbDay.SelectedItem!).Day;
    public TimeOnly StartTime => TimeOnly.FromDateTime(dtStart.Value);
    public TimeOnly EndTime => TimeOnly.FromDateTime(dtEnd.Value);

    public ScheduleEditForm(List<Doctor> doctors, DoctorSchedule? schedule = null)
    {
        Text = schedule == null ? "Çalışma Saati Ekle" : "Çalışma Saati Düzenle";

        Width = 500;
        Height = 350;

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        cmbDoctor = EntityFormHelpers.ComboBox(150, 40);
        cmbDoctor.DataSource = doctors;
        cmbDoctor.DisplayMember = nameof(Doctor.Email);

        cmbDay = EntityFormHelpers.ComboBox(150, 100);
        cmbDay.DataSource = new List<DayItem>
        {
            new(DayOfWeek.Monday, "Pazartesi"),
            new(DayOfWeek.Tuesday, "Salı"),
            new(DayOfWeek.Wednesday, "Çarşamba"),
            new(DayOfWeek.Thursday, "Perşembe"),
            new(DayOfWeek.Friday, "Cuma"),
            new(DayOfWeek.Saturday, "Cumartesi"),
            new(DayOfWeek.Sunday, "Pazar")
        };

        dtStart = CreateTimePicker(150, 160);
        dtEnd = CreateTimePicker(150, 215);

        Controls.AddRange(
        [
            EntityFormHelpers.Label("Doktor", 30, 43),
            cmbDoctor,
            EntityFormHelpers.Label("Gün", 30, 103),
            cmbDay,
            EntityFormHelpers.Label("Başlangıç", 30, 163),
            dtStart,
            EntityFormHelpers.Label("Bitiş", 30, 218),
            dtEnd
        ]);

        var save = EntityFormHelpers.SaveButton(230, 265);
        var cancel = EntityFormHelpers.CancelButton(350, 265);

        Controls.Add(save);
        Controls.Add(cancel);

        AcceptButton = save;
        CancelButton = cancel;

        if (schedule != null)
        {
            cmbDoctor.SelectedItem = doctors.FirstOrDefault(d =>
                d.UserId == schedule.DoctorId);

            cmbDoctor.Enabled = false;

            cmbDay.SelectedItem = ((List<DayItem>)cmbDay.DataSource)
                .First(d => d.Day == schedule.DayOfWeek);

            dtStart.Value = DateTime.Today.Add(schedule.StartTime.ToTimeSpan());
            dtEnd.Value = DateTime.Today.Add(schedule.EndTime.ToTimeSpan());
        }
        else
        {
            dtStart.Value = DateTime.Today.AddHours(9);
            dtEnd.Value = DateTime.Today.AddHours(17);
        }
    }

    private static DateTimePicker CreateTimePicker(int left, int top)
    {
        return new DateTimePicker
        {
            Left = left,
            Top = top,
            Width = 150,
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true
        };
    }

    private class DayItem
    {
        public DayOfWeek Day { get; }
        private readonly string _text;

        public DayItem(DayOfWeek day, string text)
        {
            Day = day;
            _text = text;
        }

        public override string ToString()
        {
            return _text;
        }
    }
}
