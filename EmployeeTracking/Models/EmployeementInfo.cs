namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmployeementInfos")]
    public partial class EmployeementInfo
    {
        public string Id { get; set; }
        
        public string JobDescription { get; set; }

        [Column(TypeName = "date")]
        public DateTime? AppointmentDate { get; set; }

        [StringLength(128)]
        public string SupervisorId { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [ForeignKey("Project")]
        public int? Division { get; set; }

        [StringLength(50)]
        public string Section { get; set; }

        [StringLength(200)]
        public string PresentReportingLocation { get; set; }

        public string Designation { get; set; }

        public bool? WorkOnSaturday { get; set; }

        public string DivisionCode { get; set; }

        public virtual Project Project { get; set; }
    }
}
