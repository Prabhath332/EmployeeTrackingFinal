namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MeetingUser
    {
        public int Id { get; set; }

        public int  MeetingId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public String AttendinState { get; set; }

        public String Remark { get; set; }

    }
}
