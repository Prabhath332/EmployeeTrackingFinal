namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmployeeLocation")]
    public partial class EmployeeLocation
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string MapLocation { get; set; }

        public DateTime Date { get; set; }
    }

    public class LocationModel {
        public Double lati { get; set; }
        public Double longi { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public String LastLocationDate { get; set; }
    }

    public class LocationUserModel {
        public String UserId { get; set; }
        public String FullName { get; set; }
        public String Designation { get; set; }
        public String profileImage { get; set; }
        public String ProjectLocation { get; set; }
        public String LocationStatus { get; set; }
        [NotMapped]
        public String LastLocationDate { get; set; }
        public List<LocationModel> LocationList { get; set; }
    }
}
