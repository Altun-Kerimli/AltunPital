using AltunPital.Data;
using AltunPital.Services;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();

db.Database.Migrate();

SeedData.Initialize(db);


// TEST sırasında eski randevuları temizle.
db.Appointments.RemoveRange(db.Appointments);
db.SaveChanges();


var scheduleService =
    new ScheduleService(db);

var appointmentService =
    new AppointmentService(
        db,
        scheduleService);


// Bir sonraki pazartesiyi bul.
DateTime today = DateTime.Today;

int daysUntilMonday =
    ((int)DayOfWeek.Monday -
     (int)today.DayOfWeek + 7) % 7;

if (daysUntilMonday == 0)
    daysUntilMonday = 7;

DateTime monday =
    today.AddDays(daysUntilMonday);


// 10:00 randevusu
DateTime appointmentTime =
    monday.AddHours(10);


Console.WriteLine(
    "=== RANDEVU TESTİ ==="
);

Console.WriteLine(
    $"Test tarihi: {appointmentTime}"
);


try
{
    var appointment =
        appointmentService.CreateAppointment(
            patientId: 2,
            doctorId: 1,
            branchId: 1,
            healthServiceId: 2,
            startDateTime: appointmentTime
        );

    Console.WriteLine();
    Console.WriteLine("RANDEVU OLUŞTURULDU");

    Console.WriteLine(
        $"ID: {appointment.AppointmentId}"
    );

    Console.WriteLine(
        $"Saat: {appointment.StartDateTime}"
    );

    Console.WriteLine(
        $"Bitiş: {appointment.EndDateTime}"
    );

    Console.WriteLine(
        $"Fiyat: {appointment.Price} TL"
    );

    Console.WriteLine(
        $"Durum: {appointment.Status}"
    );


    Console.WriteLine();
    Console.WriteLine(
        "Aynı saate ikinci randevu deneniyor..."
    );


    appointmentService.CreateAppointment(
        patientId: 2,
        doctorId: 1,
        branchId: 1,
        healthServiceId: 2,
        startDateTime: appointmentTime
    );
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine(
        $"BEKLENEN RED: {ex.Message}"
    );
}


Console.WriteLine();
Console.WriteLine(
    "=== UYGUN SAATLER ==="
);

var slots =
    scheduleService.GetAvailableSlots(
        doctorId: 1,
        healthServiceId: 2,
        DateOnly.FromDateTime(monday)
    );

foreach (var slot in slots)
{
    Console.WriteLine(
        slot.ToString("HH:mm")
    );
}