namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProjectTeam
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProjectTeam()
        {
            TeamMembers = new HashSet<TeamMember>();
        }

        public int Id { get; set; }

        public int? ProjectId { get; set; }

        [StringLength(200)]
        public string TeamName { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual Project Project { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeamMember> TeamMembers { get; set; }
    }

    public class TeamsViewModel
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string TeamName { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        public int? ProjectId { get; set; }
    }
}
