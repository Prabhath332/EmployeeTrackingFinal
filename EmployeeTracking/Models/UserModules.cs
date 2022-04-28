using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public class UserModules
    {
        public int Id { get; set; }
        public string Module { get; set; }
    }

    public class UserModulesViewModel
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public bool IsChecked { get; set; }
        public bool IsAdmin { get; set; }
        public string UserLevelId { get; set; }
    }

    public class UserAccessViewModel
    {
        public UserRoleViewModels UserRole { get; set; }
        public List<UserModulesViewModel> UserModules { get; set; }
    }
}