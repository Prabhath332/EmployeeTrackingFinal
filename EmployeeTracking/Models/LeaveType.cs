namespace EmployeeTracking.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    //public partial class LeaveType
    //{
    //    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    //    public LeaveType()
    //    {
    //        LeaveHistories = new HashSet<LeaveHistory>();
    //        UserLeaves = new HashSet<UserLeav>();
    //        UserLevelLeaves = new HashSet<UserLevelLeav>();
    //    }
    //    [Key]
    //    public int Id { get; set; }

    //    [Column("LeaveType")]
    //    [StringLength(200)]
    //    public string LeaveType1 { get; set; }

    //    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    public virtual ICollection<LeaveHistory> LeaveHistories { get; set; }

    //    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    public virtual ICollection<UserLeav> UserLeaves { get; set; }

    //    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    public virtual ICollection<UserLevelLeav> UserLevelLeaves { get; set; }
    //}
    [Table("LeaveTypes")]
    public partial class LeaveType {
         
        [Key]
        public int Id { get; set; }

        [Column("LeaveType")]
        [StringLength(200)]
        public string LeaveType1 { get; set; }

        public string LeaveTypeCode { get; set; }


    }

    public class LeavesViewModel
    {
        public List<LeaveType> LeaveTypes { get; set; }
        public List<UserLevelLeaves> UserLevelLeavs { get; set; }
        public List<IdentityRole> UserRoles { get; set; }
        public List<UserLeavesViewModel> UserLeaves { get; set; }
        public List<UserProfile> UserProfile { get; set; }
        public List<LeaveHistory> LeaveHistory { get; set; }
        public List<Holiday> HolidayDates { get; set; }
    }
}
