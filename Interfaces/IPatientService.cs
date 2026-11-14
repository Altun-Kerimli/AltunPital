using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IPatientService
{
    List<Branch> GetBranches(string? preferredCity = null);

    List<Department> GetDepartments(int branchId);

    List<Doctor> GetDoctors(int branchId, int departmentId);

    List<HealthService> GetServices(int branchId, int doctorId);

    List<DateTime> GetAvailableSlots(int doctorId, int healthServiceId, DateOnly date);

    Appointment CreateAppointment(
        int patientId,
        int doctorId,
        int branchId,
        int healthServiceId,
        DateTime startDateTime);

    List<Appointment> GetAppointments(int patientId);

    void CancelAppointment(int appointmentId);
}
