using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Models
{
    public class EmployeeTransfer
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TransferTo { get; set; }
        public string SupervisorTo { get; set; }
        public string RequestBy { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
        public string SupervisorFromApproval { get; set; }
        public string SupervisorFromApprovedBy { get; set; }
        public string SupervisorToApproval { get; set; }    
        public DateTime SupervisorFromDate { get; set; }
        public DateTime SupervisorToDate { get; set; }
        public string COOApproval { get; set; }
        public string COOApprovedBy { get; set; }
        public DateTime COOApprovedDate { get; set; }
        public string Remarks { get; set; }
        public int CompanyId { get; set; }
        public string NewLocation { get; set; }

        [NotMapped]
        public string EmpNo { get; set; }
        [NotMapped]
        public string Designation { get; set; }
        [NotMapped]
        public string Username { get; set; }
        [NotMapped]
        public string RequesterName { get; set; }
        [NotMapped]
        public string SupervisorToName { get; set; }
        [NotMapped]
        public string SupervisorFromName { get; set; }
        [NotMapped]
        public string COOName { get; set; }
        [NotMapped]
        public string CurrentDivisionName { get; set; }        
        [NotMapped]
        public string ToDivisionName { get; set; }
        [NotMapped]
        public string ReqDateStr { get; set; }
        [NotMapped]
        public string DateStr { get; set; }
        [NotMapped]
        public string DivisionCode { get; set; }
    }

    public class TransferViewModel
    {
        public EmployeeTransfer RequestTransfer { get; set; }
        public List<EmployeeTransfer> TransferRequests { get; set; }
        public List<SelectListItem> Divisions { get; set; }
        public List<SelectListItem> SiteLocations { get; set; }
    }
 

    public class TransferApprovalViewModel
    {
        public int RequestId { get; set; }
        public string ApproverId { get; set; }
        public string Approval { get; set; }
        public string ApproveType { get; set; }
        public string Remarks { get; set; }
    }
}