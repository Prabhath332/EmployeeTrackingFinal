using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public class LeaveApproval
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string SupervisorId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }

    public class ApprovalsViewModel
    {
        public List<LeaveHistory> LeaveApprovals { get; set; }
        public List<AttendenceCorrections> InOutApprovals { get; set; }
        public List<EmployeeTransfer> EmployeeTransfer { get; set; }
        public List<EmployeeTransfer> TransferRequests { get; set; }
        public AttendenceCorrections AttendenceCorrections { get; set; }
    }
}