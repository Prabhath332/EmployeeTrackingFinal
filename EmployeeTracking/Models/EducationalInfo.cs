namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EducationalInfos")]
    public partial class EducationalInfo
    {
        public string Id { get; set; }

        public string Primary { get; set; }

        public string Secondary { get; set; }

        public string Other { get; set; }

        public string ExtraCurricular { get; set; }
    }
}
