namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TeamMember
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public bool? IsSupervisor { get; set; }

        public int? TeamId { get; set; }

        public int? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [ForeignKey("TeamId")]
        public virtual ProjectTeam ProjectTeam { get; set; }
    }

    public class TeamMemberViewModels
    {
        public string UserId { get; set; }
        public string MemberName { get; set; }
        public string EPFNo { get; set; }
        public string MobilePhone { get; set; }
        public bool? IsSupervisor { get; set; }
        public int? ProjectId { get; set; }
        public int? TeamId { get; set; }
    }

    public class ProjectUsersViewModels
    {
        public string UserId { get; set; }
        public string Username { get; set; }       
        public int? ProjectId { get; set; }
    }
}
