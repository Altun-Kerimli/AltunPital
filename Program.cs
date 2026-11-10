using AltunPital.Data;
using AltunPital.Services;
using AltunPital.UI.Desktop;
using Microsoft.EntityFrameworkCore;

namespace AltunPital;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();


        using var db =
            new AppDbContext();


        db.Database.Migrate();

        SeedData.Initialize(
            db);


        var scheduleService =
            new ScheduleService(
                db);


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
            new AuthenticationService(
                db);


        var employeeService =
            new EmployeeService(
                db);


        var managerService =
            new ManagerService(
                db);


        var doctorService =
            new DoctorManagementService(
                db);


        var accountService =
            new UserAccountService(
                db);


        System.Windows.Forms.Application.Run(
            new LoginForm(
                authenticationService,
                patientService,
                doctorService,
                managerService,
                employeeService,
                accountService));
    }
}