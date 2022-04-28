using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public class Attendence
    {
    }

    public class AttendenceCorrections
    {
        public int Id { get; set; }
        public int AttendenceId { get; set; }
        public string UserId { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        public string InReason { get; set; }
        public string OutReason { get; set; }
        public string SupervisorId { get; set; }
        public string IsApproved { get; set; }
        public string Remarks { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime Date { get; set; }

        [NotMapped]
        public string Username { get; set; }
        [NotMapped]
        public string EmpNo { get; set; }
        [NotMapped]
        public string SupervisorName { get; set; }
        
        
        [NotMapped]
        public int DivisionId { get; set; }
        [NotMapped]
        public string DivisionName { get; set; }
        [NotMapped]
        public string Intimestr { get; set; }
        [NotMapped]
        public string OutTimeStr { get; set; }
        [NotMapped]
        public string Datestr { get; set; }
        [NotMapped]
        public string DivisionCode { get; set; }
        [NotMapped]
        public string Designation { get; set; }
        [NotMapped]
        public string Location { get; set; }
        [NotMapped]
        public string InOutDate { get; set; }
    }

    public class ApproveInOutViewModel
    {
        public int Id { get; set; }
        public string IsApproved { get; set; }
        public string Remarks { get; set; }
    }
    
}