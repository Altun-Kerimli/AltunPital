using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IScheduleService
{
    bool IsDoctorAvailable(int doctorId, DateTime startDateTime, int durationMinutes);

    List<DateTime> GetAvailableSlots(int doctorId, int healthServiceId, DateOnly date);
}
