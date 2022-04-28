namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NewsLike
    {
        public int Id { get; set; }

        public int NewsId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }
         
    }
}
