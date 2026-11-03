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
        string employeeNumber,
        DateTime hireDate,
        int branchId,
        int? departmentId,
        string position);

    Manager AddManager(
        string firstName,
        string lastName,
        string email,
        string phone,
        string employeeNumber,
        DateTime hireDate,
        int branchId);

    void UpdateDoctor(
        int doctorId,
        int branchId,
        int departmentId,
        string specialization);

    void AssignService(
        int doctorId,
        int healthServiceId);

    DoctorSchedule AddSchedule(
        int doctorId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime);

    List<Employee> GetEmployees(
        int? branchId = null);

    void DeleteEmployee(int employeeId);
}