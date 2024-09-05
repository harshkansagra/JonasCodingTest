using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using Serilog;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class CompanyController : ApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;

        public CompanyController(ICompanyService companyService, IMapper mapper)
        {
            _companyService = companyService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            Log.Information("GetEmployee request received for employeeCode");
            var items = await _companyService.GetAllCompaniesAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(items);
        }

        public async Task<IHttpActionResult> GetAsync(string companyCode)
        {
            var item = await _companyService.GetCompanyByCodeAsync(companyCode);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CompanyDto>(item));
        }

        [ValidateModel]
        public async Task<IHttpActionResult> PostAsync([FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                throw new Exception("companyDto is empty. Try again with valid input.");
            }

            var companyEntity = _mapper.Map<CompanyInfo>(companyDto); // Assuming there's a Company entity class
            await _companyService.CreateCompanyAsync(companyEntity);

            return Ok("Company created successfully");
        }

        public async Task<IHttpActionResult> PutAsync(string companyCode, [FromBody] CompanyDto companyDto)
        {
            if (string.IsNullOrEmpty(companyCode) || companyDto == null)
            {
                throw new Exception("companyCode or companyDto is empty. Try again with valid input.");
            }

            var existingCompany = await _companyService.GetCompanyByCodeAsync(companyCode);
            if (existingCompany == null)
            {
                throw new Exception($"No company found with companyCode as {companyCode}.");
            }

            //take updated fields from companyDto and create new object
            CompanyInfo updatedCompany = _mapper.Map(companyDto, existingCompany);

            await _companyService.UpdateCompanyAsync(updatedCompany);

            return Ok("Company updated successfully");
        }

        public async Task<IHttpActionResult> DeleteAsync(string companyCode)
        {
            var existingCompany = await _companyService.GetCompanyByCodeAsync(companyCode);
            if (existingCompany == null)
            {
                throw new Exception($"No company found with companyCode as {companyCode}.");
            }

            await _companyService.DeleteCompanyAsync(companyCode);

            return Ok("Company and It's Employees deleted successfully");
        }
    }
}