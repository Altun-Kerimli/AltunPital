using AltunPital.Data;
using AltunPital.Services;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();

db.Database.Migrate();
SeedData.Initialize(db);

var employeeService =
    new EmployeeService(db);


Console.WriteLine(
    "=== ÇALIŞAN YÖNETİMİ TESTİ ===");


// Test doktoru önce varsa temizle
var oldDoctor =
    db.Doctors.FirstOrDefault(d =>
        d.Email ==
        "mehmet.test@altunpital.local");

if (oldDoctor != null &&
    !db.Appointments.Any(a =>
        a.DoctorId == oldDoctor.UserId))
{
    employeeService.DeleteEmployee(
        oldDoctor.UserId);
}


// Doktor oluştur
var doctor =
    employeeService.AddDoctor(
        "Mehmet",
        "Kaya",

        "mehmet.test@altunpital.local",
        "05000000010",

        "ALT-DOC-TEST",

        DateTime.Today,

        branchId: 1,
        departmentId: 1,

        specialization: "Kardiyoloji"
    );


Console.WriteLine(
    $"Doktor eklendi: " +
    $"{doctor.FirstName} {doctor.LastName}");


// Hizmet ata
employeeService.AssignService(
    doctor.UserId,
    healthServiceId: 2
);

Console.WriteLine(
    "Kardiyoloji hizmeti atandı.");


// Çalışma saati ata
employeeService.AddSchedule(
    doctor.UserId,

    DayOfWeek.Friday,

    new TimeOnly(10, 0),
    new TimeOnly(16, 0)
);

Console.WriteLine(
    "Cuma 10:00-16:00 çalışma saati atandı.");


// Çalışanları listele
Console.WriteLine();
Console.WriteLine(
    "=== SAKARYA ÇALIŞANLARI ===");

var employees =
    employeeService.GetEmployees(1);

foreach (var employee in employees)
{
    Console.WriteLine(
        $"{employee.UserId} | " +
        $"{employee.FirstName} " +
        $"{employee.LastName} | " +
        $"{employee.EmployeeNumber}");
}


// Test silme
employeeService.DeleteEmployee(
    doctor.UserId);

Console.WriteLine();
Console.WriteLine(
    "Test doktoru başarıyla silindi.");