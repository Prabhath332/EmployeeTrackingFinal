using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public class Notifications
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string TaskType { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }

    public class NotificationViewModel
    {
        public string TaskType { get; set; }
        public int TaskCount { get; set; }
    }
}