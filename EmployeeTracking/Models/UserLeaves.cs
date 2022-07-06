namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserLeaves
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("LeaveType1")]
        public int? LeaveType { get; set; }

        public double? AllocatedCount { get; set; }

        public double? RemainingCount { get; set; }

        public int Year { get; set; }        

        public virtual LeaveType LeaveType1 { get; set; }
    }

    //[Table(name: "NoPayLeaves")]
    public class Employee_NoPay_Leaves
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Request_Id { get; set; }
        public double No_Pay_Count { get; set; }
    }

    public class UserLeavesViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string LeaveType { get; set; }
        public int LeaveId { get; set; }
        public double? AllocatedCount { get; set; }
        public double? RemainingCount { get; set; }
        public int Year { get; set; }
    }

    public class MyLeavesViewModel
    {
        public List<UserLeavesViewModel> UserLeaves { get; set; }
        public List<LeaveHistory> MyLeaveHistory { get; set; }
        public LeaveHistory LeaveHistory { get; set; }
        public List<LeaveType> LeaveTypes { get; set; }
        public List<messageusers> Supervisors { get; set; }
        public List<RejectedLeave> RejectedLeaves { get; set; }
    }

    public class LeaveReportViewModel
    {
        public string EmpNo { get; set; }
        public string Username { get; set; }
        public int DivisionId { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
        public string Supervisor { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        //public string Section { get; set; }
        public List<UserLeavesViewModel> Leaves { get; set; }
    }

    public class LeaveReportDetailsViewModel
    {
        public string EmpNo { get; set; }
        public string Username { get; set; }
        public int DivisionId { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public List<LeaveHistory> Leaves { get; set; }
    }

    //public class EnforceLeavesViewModel
    //{
    //    public LeaveHistory LeaveHistory { get; set; }
    //    public List<EmployeeTransfer> TransferRequests { get; set; }
    //    public List<SelectListItem> Divisions { get; set; }
    //    public List<SelectListItem> SiteLocations { get; set; }
    //}

    //public class UserLeaveHistory
    //{
    //    public List<LeaveHistory> Leaves { get; set; }
    //    //public int Id { get; set; }
    //    //public DateTime From { get; set; }
    //    //public DateTime To { get; set; }
    //    //public string Reason { get; set; }
    //    //public string ApprovedBy { get; set; }
    //    //public string Status { get; set; }
    //    //public string LeaveType { get; set; }
    //    //public string LeaveUnit { get; set; }
    //    //public string CoveredBy { get; set; }
    //    //public DateTime ApprovedDate { get; set; }
    //}
}
