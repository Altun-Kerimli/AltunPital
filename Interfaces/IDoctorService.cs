using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IDoctorService
{
    List<Appointment> GetAppointments(
        int doctorId);

    List<DoctorSchedule> GetSchedules(
        int doctorId);

    void ApproveAppointment(
        int doctorId,
        int appointmentId);

    void RejectAppointment(
        int doctorId,
        int appointmentId);

    void CompleteAppointment(
        int doctorId,
        int appointmentId);
}