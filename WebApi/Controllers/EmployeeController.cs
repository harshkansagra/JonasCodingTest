using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    [RoutePrefix("api/employee")]
    public class EmployeeController : ApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeController(ICompanyService companyService, IEmployeeService employeeService, IMapper mapper)
        {
            _companyService = companyService;
            _employeeService = employeeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<EmployeeDetailDto>> GetAllAsync()
        {
            IEnumerable<EmployeeInfo> employees = await _employeeService.GetAllEmployeesAsync();
            IEnumerable<CompanyInfo> companies = await _companyService.GetAllCompaniesAsync();
            List<EmployeeDetailDto> employeeDetails = new List<EmployeeDetailDto>();

            foreach (var employee in employees)
            {
                CompanyInfo company = companies?.FirstOrDefault(x => x.CompanyCode == employee.CompanyCode);

                var employeeDetail = _mapper.Map<EmployeeDetailDto>(employee);
                if (company != null)
                    _mapper.Map(company, employeeDetail);

                employeeDetails.Add(employeeDetail);
            }

            return employeeDetails;
        }

        [HttpGet]
        [Route("{employeeCode}")]
        public async Task<IHttpActionResult> GetAsync(string employeeCode)
        {
            EmployeeInfo employee = await _employeeService.GetEmployeeByCodeAsync(employeeCode);
            if (employee == null)
                return NotFound();

            CompanyInfo company = await _companyService.GetCompanyByCodeAsync(employee.CompanyCode);
            if (company == null)
                throw new Exception($"No company found for an employee with employeeCode as {employeeCode}.");

            var employeeDetail = _mapper.Map<EmployeeDetailDto>(employee);
            _mapper.Map(company, employeeDetail);
            return Ok(employeeDetail);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IHttpActionResult> PostAsync([FromBody] EmployeeDto employeeDto)
        {

            if (employeeDto == null)
            {
                throw new Exception("employeeDto is empty. Try again with valid input.");
            }

            var existingCompany = await _companyService.GetCompanyByCodeAsync(employeeDto.CompanyCode);
            if (existingCompany == null)
            {
                throw new Exception($"No company found with companyCode as {employeeDto.CompanyCode} to register an employee.");
            }

            var employeeEntity = _mapper.Map<EmployeeInfo>(employeeDto);
            await _employeeService.CreateEmployeeAsync(employeeEntity);

            return Ok("Employee created successfully");
        }
    }
}