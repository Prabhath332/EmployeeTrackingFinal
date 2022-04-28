namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserProfile
    {
        public string Id { get; set; }

        [StringLength(200)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        public string EmployeeId { get; set; }

        public int? Age { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(15)]
        public string MaritalStatus { get; set; }

        [StringLength(50)]
        public string NICNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        public string Image { get; set; }

        public string Remarks { get; set; }

        public DateTime DateCreated { get; set; }

        [NotMapped]
        public string UserRole { get; set; }

        [NotMapped]
        public int RowNumber { get; set; }

        ///Checkin 08092019

    }

    public class EmployeeViewModel
    {
        public string Id { get; set; }        
        public string FirstName { get; set; }
        public string Designation { get; set; }
        public string EmployeeId { get; set; }
        public int CurrentDivisionId { get; set; }
        public string Division { get; set; }
        public string SupervisorId { get; set; }
        public string Supervisor { get; set; }
        public string Location { get; set; }
    }
}
