using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IAppointmentService
{
    Appointment CreateAppointment(
        int patientId,
        int doctorId,
        int branchId,
        int healthServiceId,
        DateTime startDateTime);

    void ApproveAppointment(int appointmentId);

    void RejectAppointment(int appointmentId);

    void CancelAppointment(int appointmentId);

    List<Appointment> GetPatientAppointments(int patientId);

    List<Appointment> GetPendingAppointments();
}