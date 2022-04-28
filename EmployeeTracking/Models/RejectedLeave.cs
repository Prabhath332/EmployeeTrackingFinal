using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public class RejectedLeave
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string LeaveType { get; set; }
        public string RejectedBy { get; set; }
        public string RequestedOn { get; set; }
        public string Unit { get; set; }
        public string Period { get; set; }
        public string Days { get; set; }
        public string Remarks { get; set; }
        public string UserId { get; set; }
        public string RejectedUserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Reason { get; set; }
        public string IsApproved { get; set; }
    }
}