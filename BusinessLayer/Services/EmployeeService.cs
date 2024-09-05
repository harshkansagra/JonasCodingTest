using BusinessLayer.Model.Interfaces;
using System.Collections.Generic;
using AutoMapper;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using System;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeInfo>> GetAllEmployeesAsync()
        {
            var res = await _EmployeeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeInfo>>(res);
        }

        public async Task<EmployeeInfo> GetEmployeeByCodeAsync(string EmployeeCode)
        {
            var result = await _EmployeeRepository.GetByCodeAsync(EmployeeCode);
            return _mapper.Map<EmployeeInfo>(result);
        }

        public async Task CreateEmployeeAsync(EmployeeInfo Employee)
        {
            if (Employee == null)
            {
                throw new ArgumentNullException(nameof(Employee));
            }

            var EmployeeEntity = _mapper.Map<Employee>(Employee); // Assuming there's a Employee entity class
            
            // Map Employee to entity class if necessary and add to repository
            await _EmployeeRepository.SaveEmployeeAsync(EmployeeEntity);
        }
    }
}
