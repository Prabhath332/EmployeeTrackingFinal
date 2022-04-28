using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public class ModuleUsers
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string UserLevelId { get; set; }
    }
}