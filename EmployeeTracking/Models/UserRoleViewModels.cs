using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public class UserRoleViewModels
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public int LevelId { get; set; }
        public int SortOrder { get; set; }
        public bool CanEdit { get; set; }
        public bool CanViewAll { get; set; }
    }
}