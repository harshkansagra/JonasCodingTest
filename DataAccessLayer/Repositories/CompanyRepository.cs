using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Serilog;
using AutoMapper;

namespace DataAccessLayer.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbWrapper<Company> _companyDbWrapper;
        private readonly IMapper _mapper;

        public CompanyRepository(IDbWrapper<Company> companyDbWrapper, IMapper mapper)
        {
            _companyDbWrapper = companyDbWrapper;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            try
            {
                Log.Information("Get all Company Records.");
                return await _companyDbWrapper.FindAllAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Company> GetByCodeAsync(string companyCode)
        {
            try
            {
                Log.Information($"Get Company Record with companyCode {companyCode}.");
                IEnumerable<Company> filteredCompanies = await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode));

                return filteredCompanies?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveCompanyAsync(Company company)
        {
            try
            {
                IEnumerable<Company> filteredCompanies = await _companyDbWrapper.FindAsync(t =>
                                    t.SiteId.Equals(company.SiteId) && t.CompanyCode.Equals(company.CompanyCode));
                Company existingCompany = filteredCompanies?.FirstOrDefault();
                
                if (existingCompany != null)
                {
                    Log.Information($"Update existing Company Record with companyCode {company.CompanyCode} and SiteId {company.SiteId}.");
                    existingCompany.CompanyName = company.CompanyName;
                    existingCompany.AddressLine1 = company.AddressLine1;
                    existingCompany.AddressLine2 = company.AddressLine2;
                    existingCompany.AddressLine3 = company.AddressLine3;
                    existingCompany.Country = company.Country;
                    existingCompany.EquipmentCompanyCode = company.EquipmentCompanyCode;
                    existingCompany.FaxNumber = company.FaxNumber;
                    existingCompany.PhoneNumber = company.PhoneNumber;
                    existingCompany.PostalZipCode = company.PostalZipCode;
                    existingCompany.LastModified = company.LastModified;
                    existingCompany.ArSubledgers = SaveSubLedger(existingCompany.ArSubledgers, company.ArSubledgers);
                    return await _companyDbWrapper.UpdateAsync(existingCompany);
                }

                Log.Information($"Insert new Company Record with companyCode {company.CompanyCode} and SiteId {company.SiteId}.");
                return await _companyDbWrapper.InsertAsync(company);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ArSubledger> SaveSubLedger(List<ArSubledger> existingArSugledgers, List<ArSubledger> newSublegers)
        {
            if (!existingArSugledgers.Any())
                return newSublegers;

            //if same ArSubledgerCode exist Select new subledgers else select existing subledger
            List<ArSubledger> UpdatedSubLedgers = (from existingRec in existingArSugledgers
                                                   join updatedRecord in newSublegers on existingRec.ArSubledgerCode equals updatedRecord.ArSubledgerCode into r
                                                   from updatedRec in r.DefaultIfEmpty()
                                                   select updatedRec != null ? _mapper.Map(updatedRec, existingRec) : existingRec).ToList();

            //Add new subledgers
            UpdatedSubLedgers.AddRange(newSublegers.Where(x => !existingArSugledgers.Select(rec => rec.ArSubledgerCode).Contains(x.ArSubledgerCode)));

            return UpdatedSubLedgers;
        }

        public async Task<bool> DeleteCompanyAsync(string companyCode)
        {
            try
            {
                Log.Information($"Deleting Company Record with companyCode {companyCode}.");
                return await _companyDbWrapper.DeleteAsync(t => t.CompanyCode.Equals(companyCode));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
