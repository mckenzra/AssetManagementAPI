﻿using AssetManagementAPI.DTO;
using AssetManagementAPI.Models;
using AssetManagementAPI.Services.Helpers;

namespace AssetManagementAPI.Interfaces
{
    public interface IEmployeeRepository : IDisposable
    {
        Task<ICollection<Employee>> GetAllAsync(QueryObject? queryObject);
        Task<Employee?> CreateAsync(CreateEmployeeDTO employee);
        Task<Employee?> GetByIdAsync(string id);
        Task<Employee?> UpdateAsync(string id, UpdateEmployeeDTO employee);
        Task<Employee?> DeleteAsync(string id);
    }
}
