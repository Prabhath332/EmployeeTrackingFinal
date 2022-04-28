namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Company()
        {
            //ProjectTeams = new HashSet<ProjectTeam>();
            //Projects = new HashSet<Project>();
            //UserCompanies = new HashSet<UserCompany>();
        }

        public int Id { get; set; }

        [StringLength(200)]
        public string CompanyName { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        public string COO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectTeam> ProjectTeams { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project> Projects { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserCompany> UserCompanies { get; set; }
    }

    public class CompaniesViewModel
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string CompanyName { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }
    }
}
