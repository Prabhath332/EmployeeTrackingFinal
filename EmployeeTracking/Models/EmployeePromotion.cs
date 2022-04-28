namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EmployeePromotion
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(200)]
        public string Designation { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateTo { get; set; }

        [StringLength(200)]
        public string Division { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(400)]
        public string Remarks { get; set; }
    }
}
