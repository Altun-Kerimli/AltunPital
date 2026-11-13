using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IEmployeeService
{
    Doctor AddDoctor(
        string firstName,
        string lastName,
        string email,
        string phone,
        string password,
        string employeeNumber,
        DateTime hireDate,
        int branchId,
        int departmentId,
        string specialization);


    HealthStaff AddHealthStaff(
        string firstName,
        string lastName,
        string email,
        string phone,
        string password,
        string employeeNumber,
        DateTime hireDate,
        int branchId,
        int? departmentId,
        string position);

    void UpdateDoctor(
        int doctorId,
        string firstName,
        string lastName,
        string email,
        string phone,
        string employeeNumber,
        DateTime hireDate,
        int branchId,
        int departmentId,
        string specialization);


    void UpdateHealthStaff(
        int staffId,
        string firstName,
        string lastName,
        string email,
        string phone,
        string employeeNumber,
        DateTime hireDate,
        int branchId,
        int? departmentId,
        string position);


    void AssignService(
        int doctorId,
        int healthServiceId);


    void RemoveService(
        int doctorId,
        int healthServiceId);


    List<AltunPital.Data.DoctorService>
        GetDoctorServices(
            int? doctorId = null);


    DoctorSchedule AddSchedule(
        int doctorId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime);


    void UpdateSchedule(
        int scheduleId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime);


    void DeleteSchedule(
        int scheduleId);


    List<DoctorSchedule>
        GetSchedules(
            int? doctorId = null);


    List<Employee> GetEmployees(
        int? branchId = null);


    void DeleteEmployee(
        int employeeId);
}