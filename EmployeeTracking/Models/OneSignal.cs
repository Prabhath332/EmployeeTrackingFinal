using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models {
    [Table("OnSignalUsers")]
    public class OneSignal {
        [Key]
        public int Id { get; set; }
        public String UserId { get; set; }
        public String OnesignalId { get; set; }
        public String Status { get; set; }
    }
}