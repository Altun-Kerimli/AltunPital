using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IManagerService
{
    Branch AddBranch(
        int hospitalId,
        string name,
        string city,
        string district,
        string address,
        string phone,
        TimeOnly openingTime,
        TimeOnly closingTime);

    void UpdateBranch(
        int branchId,
        string name,
        string city,
        string district,
        string address,
        string phone,
        TimeOnly openingTime,
        TimeOnly closingTime);

    void DeleteBranch(int branchId);

    List<Branch> GetBranches();

    Department AddDepartment(int branchId, string name, string description);

    void UpdateDepartment(int departmentId, string name, string description);

    void DeleteDepartment(int departmentId);

    List<Department> GetDepartments(int branchId);

    HealthService AddHealthService(string name, string description, int durationMinutes);

    void UpdateHealthService(int healthServiceId, string name, string description, int durationMinutes);

    void DeleteHealthService(int healthServiceId);

    List<HealthService> GetHealthServices();

    void AssignServiceToBranch(int branchId, int healthServiceId, decimal price);

    List<BranchService> GetBranchServices(int? branchId = null);

    void UpdateBranchService(int branchId, int healthServiceId, decimal price, bool isAvailable);

    void RemoveServiceFromBranch(int branchId, int healthServiceId);

    // Şube hizmetini tek seferde ekler
    HealthService AddHealthServiceToBranch(
        int branchId,
        string name,
        string description,
        int duration,
        decimal price,
        bool isAvailable);

    void UpdateBranchServiceDetails(
        int branchId,
        int healthServiceId,
        string name,
        string description,
        int duration,
        decimal price,
        bool isAvailable);

    Equipment AddEquipment(string name, string description);

    void UpdateEquipment(int equipmentId, string name, string description);

    void DeleteEquipment(int equipmentId);

    List<Equipment> GetEquipment();

    void AssignEquipmentToBranch(int branchId, int equipmentId, int quantity);

    List<BranchEquipment> GetBranchEquipment(int? branchId = null);

    void UpdateBranchEquipment(int branchId, int equipmentId, int quantity, bool isOperational);

    void RemoveEquipmentFromBranch(int branchId, int equipmentId);

    // Şube ekipmanını tek seferde ekler
    Equipment AddEquipmentToBranch(
        int branchId,
        string name,
        string description,
        int quantity,
        bool isOperational);

    void UpdateBranchEquipmentDetails(
        int branchId,
        int equipmentId,
        string name,
        string description,
        int quantity,
        bool isOperational);

    List<Appointment> GetBranchAppointments(int branchId);
}
