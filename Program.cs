using AltunPital.Data;
using AltunPital.Services;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();

db.Database.Migrate();
SeedData.Initialize(db);

var managerService =
    new ManagerService(db);


Console.WriteLine(
    "=== YÖNETİCİ CRUD TESTİ ===");


// ------------------------------------------------
// 1. Test şubesini eski testten kalmışsa temizle
// ------------------------------------------------

var oldBranch =
    db.Branches.FirstOrDefault(b =>
        b.Name == "AltunPital Test Şubesi");

if (oldBranch != null &&
    !db.Employees.Any(e =>
        e.BranchId == oldBranch.BranchId) &&
    !db.Appointments.Any(a =>
        a.BranchId == oldBranch.BranchId))
{
    managerService.DeleteBranch(
        oldBranch.BranchId);
}


// ------------------------------------------------
// 2. ŞUBE CREATE
// ------------------------------------------------

var branch =
    managerService.AddBranch(
        hospitalId: 1,

        name: "AltunPital Test Şubesi",

        city: "Bursa",
        district: "Nilüfer",

        address: "Test adresi",

        phone: "0224 000 00 00",

        openingTime:
            new TimeOnly(8, 0),

        closingTime:
            new TimeOnly(18, 0)
    );

Console.WriteLine(
    $"Şube eklendi: {branch.Name}");


// ------------------------------------------------
// 3. ŞUBE UPDATE
// ------------------------------------------------

managerService.UpdateBranch(
    branch.BranchId,

    "AltunPital Bursa",

    "Bursa",
    "Nilüfer",

    "Yeni test adresi",

    "0224 111 11 11",

    new TimeOnly(8, 30),
    new TimeOnly(19, 0)
);

Console.WriteLine(
    "Şube güncellendi.");


// ------------------------------------------------
// 4. DEPARTMENT CREATE
// ------------------------------------------------

var department =
    managerService.AddDepartment(
        branch.BranchId,

        "Nöroloji",

        "Sinir sistemi hastalıkları"
    );

Console.WriteLine(
    $"Bölüm eklendi: {department.Name}");


// ------------------------------------------------
// 5. HEALTH SERVICE CREATE
// ------------------------------------------------

var service =
    managerService.AddHealthService(

        "Nöroloji Muayenesi",

        "Nöroloji uzman muayenesi",

        durationMinutes: 40
    );

Console.WriteLine(
    $"Hizmet eklendi: {service.Name}");


// ------------------------------------------------
// 6. HİZMETİ ŞUBEYE ATA
// ------------------------------------------------

managerService.AssignServiceToBranch(
    branch.BranchId,
    service.HealthServiceId,
    price: 1750
);

Console.WriteLine(
    "Hizmet şubeye atandı.");


// ------------------------------------------------
// 7. FİYATI UPDATE
// ------------------------------------------------

managerService.UpdateBranchService(
    branch.BranchId,
    service.HealthServiceId,

    price: 1900,

    isAvailable: true
);

var branchService =
    db.BranchServices
        .Include(bs =>
            bs.HealthService)
        .First(bs =>
            bs.BranchId ==
                branch.BranchId &&
            bs.HealthServiceId ==
                service.HealthServiceId);

Console.WriteLine(
    $"Yeni fiyat: {branchService.Price} TL");


// ------------------------------------------------
// 8. EQUIPMENT CREATE
// ------------------------------------------------

var equipment =
    managerService.AddEquipment(

        "EEG Cihazı",

        "Elektroensefalografi cihazı"
    );

Console.WriteLine(
    $"Ekipman oluşturuldu: {equipment.Name}");


// ------------------------------------------------
// 9. EQUIPMENT BRANCH ASSIGN
// ------------------------------------------------

managerService.AssignEquipmentToBranch(
    branch.BranchId,
    equipment.EquipmentId,
    quantity: 2
);

Console.WriteLine(
    "Ekipman şubeye atandı.");


// ------------------------------------------------
// 10. EQUIPMENT UPDATE
// ------------------------------------------------

managerService.UpdateBranchEquipment(
    branch.BranchId,
    equipment.EquipmentId,

    quantity: 3,

    isOperational: true
);

Console.WriteLine(
    "Ekipman adedi güncellendi.");


// ------------------------------------------------
// 11. READ
// ------------------------------------------------

Console.WriteLine();
Console.WriteLine("=== ŞUBELER ===");

foreach (var item in
         managerService.GetBranches())
{
    Console.WriteLine(
        $"{item.BranchId} | " +
        $"{item.Name} | " +
        $"{item.City}");
}


// ------------------------------------------------
// 12. DELETE TEST
// ------------------------------------------------

managerService.RemoveEquipmentFromBranch(
    branch.BranchId,
    equipment.EquipmentId);

managerService.DeleteEquipment(
    equipment.EquipmentId);


managerService.RemoveServiceFromBranch(
    branch.BranchId,
    service.HealthServiceId);

managerService.DeleteHealthService(
    service.HealthServiceId);


managerService.DeleteDepartment(
    department.DepartmentId);


managerService.DeleteBranch(
    branch.BranchId);


Console.WriteLine();
Console.WriteLine(
    "Test verileri başarıyla silindi.");

Console.WriteLine(
    "=== YÖNETİCİ CRUD TESTİ TAMAMLANDI ===");