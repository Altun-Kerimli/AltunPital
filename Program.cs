using AltunPital.Data;
using AltunPital.Services;
using AltunPital.UI;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();

db.Database.Migrate();

SeedData.Initialize(db);


// SERVICES

var scheduleService =
    new ScheduleService(db);

var appointmentService =
    new AppointmentService(
        db,
        scheduleService);

var patientService =
    new PatientService(
        db,
        appointmentService,
        scheduleService);

var authenticationService =
    new AuthenticationService(db);

var employeeService =
    new EmployeeService(db);

var managerService =
    new ManagerService(db);

var doctorService =
    new DoctorManagementService(db);


// UI

var menu =
    new ConsoleMenu(
        authenticationService,
        patientService,
        doctorService,
        managerService,
        employeeService);

menu.Start();