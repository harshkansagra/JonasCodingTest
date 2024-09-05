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
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CompanyInfo>> GetAllCompaniesAsync()
        {
            var res = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyInfo>>(res);
        }

        public async Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode)
        {
            var result = await _companyRepository.GetByCodeAsync(companyCode);
            return _mapper.Map<CompanyInfo>(result);
        }

        public async Task CreateCompanyAsync(CompanyInfo company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            var companyEntity = _mapper.Map<Company>(company); // Assuming there's a Company entity class

            // Map Company to entity class if necessary and add to repository
            await _companyRepository.SaveCompanyAsync(companyEntity);
        }

        public async Task UpdateCompanyAsync(CompanyInfo company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            var existingCompany = await _companyRepository.GetByCodeAsync(company.CompanyCode);
            if (existingCompany == null)
            {
                throw new KeyNotFoundException($"Company with code {company.CompanyCode} not found.");
            }

            // Update existing company with new values
            _mapper.Map(company, existingCompany);

            // Update the repository
            await _companyRepository.SaveCompanyAsync(existingCompany);
        }

        public async Task DeleteCompanyAsync(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
            {
                throw new ArgumentNullException(nameof(companyCode));
            }

            var company = await _companyRepository.GetByCodeAsync(companyCode);
            if (company == null)
            {
                throw new KeyNotFoundException($"Company with code {companyCode} not found.");
            }

            // Remove the company from the repository
            await _companyRepository.DeleteCompanyAsync(companyCode);
        }
    }
}
