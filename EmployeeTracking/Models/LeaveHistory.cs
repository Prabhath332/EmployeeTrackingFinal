namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeaveHistory")]
    public partial class LeaveHistory
    {
        [Key]
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public int LeaveType { get; set; }

        [Column(TypeName = "date")]
        public DateTime FromDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime ToDate { get; set; }

        public double? LeaveDays { get; set; }

        public string LeaveUnit { get; set; }

        [StringLength(128)]
        public string SupervisorId { get; set; }
        
        public string CoveredBy { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        public string IsApproved { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [Column(TypeName = "date")]
        public DateTime RequestDate { get; set; }

        [NotMapped]
        public String LeaveTypeStr { get; set; }

        [NotMapped]
        public String FromDateStr { get; set; }

        [NotMapped]
        public String ToDateStr { get; set; }
        [NotMapped]
        public String ReqDateStr { get; set; }
        [NotMapped]
        public String Requester { get; set; }
        [NotMapped]
        public string EMPNo { get; set; }
        [NotMapped]
        public string ApprovedBy { get; set; }
        [NotMapped]
        public string Project { get; set; }
        [NotMapped]
        public string Section { get; set; }
        [NotMapped]
        public string Location { get; set; }
        [NotMapped]
        public string DivisionCode { get; set; }

        //public virtual LeaveType LeaveType1 { get; set; }
    }

    public class LeaveHistoryViewModel
    {
        public int Id { get; set; }
        public string IsApproved { get; set; }
        public string Remarks { get; set; }
    }
}
