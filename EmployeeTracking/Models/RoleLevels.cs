using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public class RoleLevels
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public int SortOrder { get; set; }
        public bool CanEdit { get; set; }
        public bool CanViewAll { get; set; }
    }
}