using EmployeeTracking.App_Codes;
using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    [Authorize]
    [AdminAuthorization]
    public class CompaniesController : Controller
    {
        public ActionResult Index()
        {
            _companies obj = new App_Codes._companies();
            ViewBag.erMsg = TempData["erMsg"];
            return View(obj.GetCompanies());
        }

        public ActionResult NewCompany()
        {
            string CompanyName = Request.Form["txtCompanyName"].ToString();
            string Phone = Request.Form["txtCompanyPhone"].ToString();
            string Email = Request.Form["txtCompanyEmail"].ToString();
            string Fax = Request.Form["txtCompanyFax"].ToString();
            string CompanyId = Request.Form["hfCompanyId"].ToString();


            Company co = new Company();
            co.CompanyName = CompanyName;
            co.Email = Email;
            co.Fax = Fax;
            co.Phone = Phone;

            _companies obj = new App_Codes._companies();

            if(obj.AddCompany(Convert.ToInt32(CompanyId), co))
            {
                TempData["erMsg"] = "successMsg('Company <b>" + CompanyName + "</b> Created.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Company Could Not be Created.')";
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteCompany()
        {
            _companies com = new _companies();
            int CompanyId = Convert.ToInt32(Request.Form["hfDelCompany"].ToString());

            if(com.DeleteCompany(CompanyId))
            {
                TempData["erMsg"] = "successMsg('Company Deleted.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Company Could Not be Deleted.')";
            }

            return RedirectToAction("Index");
        }
    }
}