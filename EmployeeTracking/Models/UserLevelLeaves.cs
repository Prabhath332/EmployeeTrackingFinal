namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserLevelLeaves
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserLevelId { get; set; }

        [ForeignKey("LeaveType1")]
        public int? LeaveType { get; set; }

        public double? LeaveCount { get; set; }

        public virtual LeaveType LeaveType1 { get; set; }
    }
}
