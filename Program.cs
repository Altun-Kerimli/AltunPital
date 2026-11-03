using AltunPital.Data;
using AltunPital.Services;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();

db.Database.Migrate();
SeedData.Initialize(db);

// Eski test randevularını temizle
db.Appointments.RemoveRange(db.Appointments);
db.SaveChanges();

var scheduleService =
    new ScheduleService(db);

var appointmentService =
    new AppointmentService(
        db,
        scheduleService
    );


// Sonraki pazartesi
DateTime today = DateTime.Today;

int daysUntilMonday =
    ((int)DayOfWeek.Monday -
     (int)today.DayOfWeek + 7) % 7;

if (daysUntilMonday == 0)
    daysUntilMonday = 7;

DateTime monday =
    today.AddDays(daysUntilMonday);


// ----------------------------
// 1. RANDEVU OLUŞTUR
// ----------------------------

var appointment =
    appointmentService.CreateAppointment(
        patientId: 2,
        doctorId: 1,
        branchId: 1,
        healthServiceId: 2,
        startDateTime: monday.AddHours(10)
    );

Console.WriteLine(
    $"Randevu oluşturuldu: {appointment.Status}"
);


// ----------------------------
// 2. PENDING LISTESİ
// ----------------------------

Console.WriteLine();
Console.WriteLine("=== BEKLEYEN RANDEVULAR ===");

var pending =
    appointmentService.GetPendingAppointments();

foreach (var a in pending)
{
    Console.WriteLine(
        $"ID: {a.AppointmentId} | " +
        $"{a.Patient.FirstName} {a.Patient.LastName} | " +
        $"{a.Doctor.FirstName} {a.Doctor.LastName} | " +
        $"{a.HealthService.Name} | " +
        $"{a.StartDateTime}"
    );
}


// ----------------------------
// 3. ONAYLA
// ----------------------------

appointmentService.ApproveAppointment(
    appointment.AppointmentId
);

var approved =
    db.Appointments.Find(
        appointment.AppointmentId
    );

Console.WriteLine();
Console.WriteLine(
    $"Onay sonrası durum: {approved?.Status}"
);


// ----------------------------
// 4. AYNI RANDEVUYU TEKRAR ONAYLA
// ----------------------------

try
{
    appointmentService.ApproveAppointment(
        appointment.AppointmentId
    );
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Beklenen hata: {ex.Message}"
    );
}


// ----------------------------
// 5. İPTAL ET
// ----------------------------

appointmentService.CancelAppointment(
    appointment.AppointmentId
);

var cancelled =
    db.Appointments.Find(
        appointment.AppointmentId
    );

Console.WriteLine(
    $"İptal sonrası durum: {cancelled?.Status}"
);


// ----------------------------
// 6. İPTAL SONRASI AYNI SAATİ TEKRAR AL
// ----------------------------

var newAppointment =
    appointmentService.CreateAppointment(
        patientId: 2,
        doctorId: 1,
        branchId: 1,
        healthServiceId: 2,
        startDateTime: monday.AddHours(10)
    );

Console.WriteLine(
    $"İptal sonrası aynı saat tekrar alındı: " +
    $"{newAppointment.Status}"
);


// ----------------------------
// 7. REDDET
// ----------------------------

appointmentService.RejectAppointment(
    newAppointment.AppointmentId
);

var rejected =
    db.Appointments.Find(
        newAppointment.AppointmentId
    );

Console.WriteLine(
    $"Red sonrası durum: {rejected?.Status}"
);