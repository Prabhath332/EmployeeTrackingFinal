namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PastExperiance
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public string Organization { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateTo { get; set; }

        public string Designation { get; set; }
    }
}
