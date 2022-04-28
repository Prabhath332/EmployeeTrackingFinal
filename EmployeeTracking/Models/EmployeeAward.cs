namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EmployeeAward
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        public string Description { get; set; }
    }
}
