namespace EmployeeTracking.Models {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Meeting {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Meeting() {
            MeetingUsers = new HashSet<MeetingUser>();
        }

        public int Id { get; set; }

        [StringLength(128)]
        public string ArrangedBy { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        [StringLength(200)]
        public string Venue { get; set; }

        [StringLength(255)]
        public string Message { get; set; }

        public String Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MeetingUser> MeetingUsers { get; set; }
    }

    public class Meetings {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string Venue { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public String DateStr { get; set; }
        public String StatartTimeStr { get; set; }
        public String EndTimeStr { get; set; }
        public String AttendinState { get; set; }
        public String Remark { get; set; }
        public String Status { get; set; }
        public Boolean IsCreator { get; set; }
        [NotMapped]
        public List<meetingusermodel> userlist { get; set; }

    }

    public class meetingusermodel {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
    }
}
