using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.App_Codes
{
    public class _notifications
    {
        public List<NotificationViewModel> GetNotifications(string UserId)
        {
            try
            {
                List<NotificationViewModel> lst = new List<Models.NotificationViewModel>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var msgCount = db.Notifications.Where(x => x.UserId == UserId && x.TaskType == "message" && x.Status == "unread").Count();
                    var meetCount = db.Notifications.Where(x => x.UserId == UserId && x.TaskType == "meeting" && x.Status == "unread").Count();
                    var newsCount = db.Notifications.Where(x => x.UserId == UserId && x.TaskType == "news" && x.Status == "unread").Count();
                    var leaverequwstcount = db.Notifications.Where(x => x.UserId == UserId && x.TaskType == "Leave Request" && x.Status == "unread").Count();
                    var leaveapproved = db.Notifications.Where(x => x.UserId == UserId && x.TaskType == "Leave Approved" && x.Status == "unread").Count();
                    var leaverejected = db.Notifications.Where(x => x.UserId == UserId && x.TaskType == "Leave Rejected" && x.Status == "unread").Count();
                  
                    lst.Add(new Models.NotificationViewModel { TaskType = "message", TaskCount = msgCount});
                    lst.Add(new Models.NotificationViewModel { TaskType = "meeting", TaskCount = meetCount });
                    lst.Add(new Models.NotificationViewModel { TaskType = "news", TaskCount = newsCount });
                    lst.Add(new Models.NotificationViewModel { TaskType = "Leave Request", TaskCount = leaverequwstcount });
                    lst.Add(new Models.NotificationViewModel { TaskType = "Leave Approved", TaskCount = leaveapproved });
                    lst.Add(new Models.NotificationViewModel { TaskType = "Leave Rejected", TaskCount = leaverejected });

                    return lst;
                }
            }
            catch
            {
                return null;
            }
            
        }

        public bool AddNotifications(string UserId, int TaskId, string TaskType, string NotiType = "",String Date="")
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    if (TaskType == "news")
                    {
                        var users = db.Users.ToList();

                        for (int i = 0; i < users.Count; i++)
                        {
                            if (users[i].Id != UserId)
                            {

                                db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = users[i].Id, Date = DateTime.Now, Status = "unread" });
                                db.SaveChanges();
                            }
                        }

                        var lst = (from ones in db.OneSignal
                                   join user in db.Users
                                   on ones.UserId equals user.Id
                                   where ones.UserId != UserId
                                   select new { ones.OnesignalId }).ToList();
                        String[] df = new String[lst.Count];
                        for (int i = 0; i < lst.Count; i++)
                        {
                            df[i] = lst[i].OnesignalId;
                        }
                        new _onesignal().PushNotification(df, "News", "New News alert received");
                    }
                    else if (TaskType == "Leave Request")
                    {
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "Leave Request", "You have New Leave Request ");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = DateTime.Now, Status = "unread" });
                        db.SaveChanges();
                    }
                    else if (TaskType == "Leave Approved")
                    {
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "Leave Approved", "Your Leave Request has been Approved");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = DateTime.Now, Status = "unread" });
                        db.SaveChanges();
                    }
                    else if (TaskType == "Leave Rejected")
                    {
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "Leave Rejected", "Your Leave Request has been Rejected");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = DateTime.Now, Status = "unread" });
                        db.SaveChanges();
                    }
                    else if (TaskType == "meeting")
                    {
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "Meeting", "New Meeting alert received");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = DateTime.Now, Status = "unread" });
                        db.SaveChanges();
                    }
                    else if (TaskType == "inoutrequest")
                    {
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "In Out Request", "You have New In/Out Correction Request");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = DateTime.Now, Status = "unread" });
                        db.SaveChanges();
                    }
                    else if (TaskType == "inoutapproved")
                    {
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "In Out Approved", "In/out correction has been approved");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = DateTime.Now, Status = "unread" });
                        db.SaveChanges();
                    }
                    else if (TaskType == "inoutreject")
                    {
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "In Out Reject", "Your In/Out Correction Request has been Rejected");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = DateTime.Now, Status = "unread" });
                        db.SaveChanges();
                    }
                    else if (TaskType == "Transfer Request")
                    {
                        DateTime effdate = Convert.ToDateTime(Date);
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "Transfer Request", "You Have a New Employee Transfer Request");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = effdate, Status = "unread" });
                        db.SaveChanges();
                    }
                    else if (TaskType == "Transfer Request Rejected")
                    {
                        DateTime effdate = Convert.ToDateTime(Date);
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "Transfer Request Rejected", NotiType);
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = effdate, Status = "unread" });
                        db.SaveChanges();
                    }
                    else
                    {
                        var senduser = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                        String[] pid = { senduser.OnesignalId };
                        new _onesignal().PushNotification(pid, "Message", "New SMS alert received");
                        db.Notifications.Add(new Notifications { TaskId = TaskId, TaskType = TaskType, UserId = UserId, Date = DateTime.Now, Status = "unread" });
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch(Exception er)
            {
                return false;
            }

        }

        public bool UpdateNotifications(string UserId, string Type)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var noti = db.Notifications.Where(x => x.UserId == UserId && x.TaskType == Type && x.Status == "unread").ToList();
                    
                    foreach (var item in noti)
                    {
                        item.Status = "read";
                    }
                    
                    db.SaveChanges();

                    return true;
                }
            }
            catch(Exception er)
            {
                return false;
            }

        }
    }
}