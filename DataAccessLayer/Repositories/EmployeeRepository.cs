using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Serilog;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbWrapper<Employee> _employeeDbWrapper;

        public EmployeeRepository(IDbWrapper<Employee> employeeDbWrapper)
        {
            _employeeDbWrapper = employeeDbWrapper;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                Log.Information("Get all Employee Records.");
                return await _employeeDbWrapper.FindAllAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Employee> GetByCodeAsync(string employeeCode)
        {
            try
            {
                Log.Information($"Get Employee Record with EmployeeCode {employeeCode}.");
                IEnumerable<Employee> filteredEmployees = await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode));
                return filteredEmployees?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveEmployeeAsync(Employee employee)
        {
            try
            {
                IEnumerable<Employee> filteredEmployees = await _employeeDbWrapper.FindAsync(t =>
                t.SiteId.Equals(employee.SiteId) && t.EmployeeCode.Equals(employee.EmployeeCode));

                var itemRepo = filteredEmployees?.FirstOrDefault();
                if (itemRepo != null)
                    //return error existing record with same code and name exist
                    throw new Exception($"Cannot insert new employee with same EmployeeCode: {employee.EmployeeCode} and same SiteId (company): {employee.SiteId}.");

                Log.Information($"Create new Employee Record with EmployeeCode {employee.EmployeeCode} for company {employee.CompanyCode}.");
                return await _employeeDbWrapper.InsertAsync(employee);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
