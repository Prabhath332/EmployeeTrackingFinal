namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NewsImage
    {
        public int Id { get; set; }

        public int? NewsId { get; set; }

        public string ImagePath { get; set; }

        public virtual News News { get; set; }
    }
}
