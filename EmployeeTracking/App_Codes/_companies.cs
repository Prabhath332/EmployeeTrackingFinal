using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EmployeeTracking.App_Codes
{
    public class _companies
    {
        public List<Company> GetCompanies()
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.Companies.ToList();
                }
                
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public CompaniesViewModel GetCompany(int CompanyId)
        {
            try
            {
                ApplicationDbContext db = new Models.ApplicationDbContext();   
                var com = db.Companies.Where(x => x.Id == CompanyId).FirstOrDefault();
                CompaniesViewModel vm = new Models.CompaniesViewModel();
                vm.CompanyName = com.CompanyName;
                vm.Email = com.Email;
                vm.Fax = com.Fax;
                vm.Id = com.Id;
                vm.Phone = com.Phone;

                return vm;
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public bool AddCompany(int ComapnyId, Company model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var company = db.Companies.Where(x => x.Id == ComapnyId).FirstOrDefault();

                    if(company != null)
                    {
                        model.Id = ComapnyId;
                        db.Entry(company).CurrentValues.SetValues(model);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Companies.Add(model);
                        db.SaveChanges();
                    }

                    
                }

                return true;
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public bool DeleteCompany(int CompanyId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var com = db.Companies.Where(x => x.Id == CompanyId).FirstOrDefault();
                    db.Companies.Remove(com);
                    db.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}