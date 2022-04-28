using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Models
{
    public class Reports
    {
        public String DivisionName { get; set; }
        public String LocationName { get; set; }
        
        public int Division { get; set; }
        public int Location { get; set; }
        public string DivisionCode {get;set;}
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public Boolean Date { get; set; }
        public Boolean InTime { get; set; }
        public Boolean InReason { get; set; }
        public Boolean OutTime { get; set; }
        public Boolean OutReason { get; set; }
        public Boolean SuperVisor { get; set; }
        public Boolean immediateSuperVisor { get; set; }
        public Boolean inoutdate { get; set; }
        public Boolean BStatus { get; set; }
        public Boolean Comment { get; set; }
        public String StatusView { get; set; }
        public String Status { get; set; }
        [AllowHtml]
        public string HTMLContent { get; set; }

        public List<InOut> InOutResult { get; set; }
    }

    public class InOut
    {
        public String EmployeeNumber { get; set; }
        public String EmployeeName { get; set; }
        public String Designation { get; set; }
        public String Location { get; set; }
        public String DateStr { get; set; }
        public String InTime { get; set; }
        public String InReason { get; set; }
        public String OutTime { get; set; }
        public String OutReason { get; set; }
        public String SuperVisor { get; set; }
        public String ImmediateSupper { get; set; }
        public String inoutdate { get; set; }
        public String Status { get; set; }
        public String Remark { get; set; }
    }
    public class LeaveViewmodel
    {
        public String DivisionName { get; set; }
        public String LocationName { get; set; }

        public int Division { get; set; }
        public int Location { get; set; }
        public string DivisionCode { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Status { get; set; }
        public string  StatusView { get; set; }
        public Boolean Leavetype { get; set; }
        public Boolean LeaveUnit { get; set; }
        public Boolean RequestDate { get; set; }
        public Boolean LFrom { get; set; }
        public Boolean LTo { get; set; }
        public Boolean Days { get; set; }
        public Boolean ReasonforLeave { get; set; }
        public Boolean IsApproved { get; set; }
        public Boolean Approvedby { get; set; }
        public Boolean SuperVisor { get; set; }
        public Boolean Comment { get; set; }

        public List<Leave> LeaveResult { get; set; }
    }

    public class Leave
    {
        public String EmployeeNumber { get; set; }
        public String EmployeeName { get; set; }
        public String Designation { get; set; }
        public String Leavetype { get; set; }
        public String LeaveUnit { get; set; }
        public String RequestDate { get; set; }
        public String From { get; set; }
        public String To { get; set; }
        public String Days { get; set; }
        public String ReasonforLeave { get; set; }
        public String IsApproved { get; set; }
        public String Approvedby { get; set; }
        public String Supervisor { get; set; }
        public String Remark { get; set; }
    }

    public class LeaveBalanceViewModel
    {
        public List<LeaveBalanceUser> UserLeaves { get; set; }
        public string DivisionName { get; set; }
        public string LocationName { get; set; }
        public List<Project> Divisions { get; set; }
        public List<SiteLocation> Locations { get; set; }
        public DataTable ReportData { get; set; }
    }

    public class LeaveBalanceUser
    {
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public double AnnualAllocated { get; set; }
        public double CassualAllocated { get; set; }
        //public UsedLeaves TotalUsed { get; set; }
        public double BalanceAnnualLeaves { get; set; }
        public double BalanceCassualLeaves { get; set; }
        public List<MonthlyBreackDown> MonthlyUsedLeaves { get; set; }
        public List<UsedLeaves> TotalUsed { get; set; }
        public List<UsedLeaves> BalanceLeaves { get; set; }
    }

    public class MonthlyBreackDown
    {
        public int Month_Id { get; set; }
        public string Month_Name { get; set; }
        public List<UsedLeaves> LeaveTypes { get; set; }
    }

    public class UsedLeaves
    {
        public int LeaveType_Id { get; set; }
        public string Leave_Name { get; set; }
        public double Used_Count { get; set; }
    }

    public class UsedLeavesByUser
    {
        public string EmpNo { get; set; }
        public string Period { get; set; }
        public List<UsedLeaves> UsedLeaves { get; set; }
        public List<LeaveType> LeaveTypes { get; set; }
    }

    /// <summary>
    /// Monthly Attendance Summery
    /// </summary>
    public class MonthlyLeaveSummeryViewModel
    {
        public string DivisionName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string TimePeriod { get; set; }
        public int DaysInMonth { get; set; }
        //public int MyProperty { get; set; }
        public List<Project> Divisions { get; set; }
        public List<EmployeeLeaveSummery> EmployeeLeaveSummery { get; set; }
        public List<MonthModel> MonthList { get; set; }
        public DataTable ReportData { get; set; }
    }

    public class EmployeeLeaveSummery
    {
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public List<MonthlyLeaveDetails> MonthlyLeaveDetails { get; set; }
        public List<UsedLeaves> LeaveTypes { get; set; }
    }

    public class MonthlyLeaveDetails
    {
        public DateTime Day { get; set; }
        public string AttendanceType { get; set; }
    }

    public class MonthModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public DayOfWeek DayOfWeek { get; set; }
    }
}