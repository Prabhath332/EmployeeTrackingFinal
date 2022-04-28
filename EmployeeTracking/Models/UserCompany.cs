namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserCompany
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
