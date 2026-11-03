using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IManagerService
{
    // Branch
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


    // Department
    Department AddDepartment(
        int branchId,
        string name,
        string description);

    void UpdateDepartment(
        int departmentId,
        string name,
        string description);

    void DeleteDepartment(int departmentId);

    List<Department> GetDepartments(int branchId);


    // Health Service
    HealthService AddHealthService(
        string name,
        string description,
        int durationMinutes);

    void UpdateHealthService(
        int healthServiceId,
        string name,
        string description,
        int durationMinutes);

    void DeleteHealthService(int healthServiceId);

    List<HealthService> GetHealthServices();


    // Branch-Service
    void AssignServiceToBranch(
        int branchId,
        int healthServiceId,
        decimal price);

    void UpdateBranchService(
        int branchId,
        int healthServiceId,
        decimal price,
        bool isAvailable);

    void RemoveServiceFromBranch(
        int branchId,
        int healthServiceId);


    // Equipment
    Equipment AddEquipment(
        string name,
        string description);

    void UpdateEquipment(
        int equipmentId,
        string name,
        string description);

    void DeleteEquipment(int equipmentId);

    void AssignEquipmentToBranch(
        int branchId,
        int equipmentId,
        int quantity);

    void UpdateBranchEquipment(
        int branchId,
        int equipmentId,
        int quantity,
        bool isOperational);

    void RemoveEquipmentFromBranch(
        int branchId,
        int equipmentId);
}