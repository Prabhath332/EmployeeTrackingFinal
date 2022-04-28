using EmployeeTracking.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.App_Codes {
    public class _message {
        public UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        public RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

        private ApplicationSignInManager _signInManager;

        private static TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");

        public List<messageusers> GetRoleUsers(string RoleId) {
            try {
                List<messageusers> lst = new List<messageusers>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var userRole = RoleManager.FindById(RoleId).Users;

                    //var timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");                    

                    foreach (var item in userRole) {
                        messageusers msguser = new messageusers();
                        var role = db.UserProfiles.Where(x => x.Id == item.UserId).FirstOrDefault();
                        var lastuserMessage = db.MessageRecipients.Where(m => m.MsgTo == item.UserId).FirstOrDefault();
                        if (lastuserMessage != null) {
                            var message = db.Messages.Where(m => m.Id == lastuserMessage.MessageId).OrderByDescending(m => m.Id).FirstOrDefault();
                            var dateTime = TimeZoneInfo.ConvertTime(message.Date, timezone);

                            if (message != null) {
                                msguser.LastMessage = message.Message1;
                                //msguser.MessageTime = message.Date.ToString("dd/MM/yyyy");
                                msguser.MessageTime = dateTime.Date.ToString("dd/MM/yyyy");
                            } else {
                                msguser.LastMessage = "No Message";
                                msguser.MessageTime = " ";
                            }
                        } else {
                            msguser.LastMessage = "No Message";
                            msguser.MessageTime = " ";
                        }
                        msguser.UserId = role.Id;
                        msguser.Name = role.FirstName + " " + role.LastName;
                        msguser.RoalId = role.UserRole;
                        lst.Add(msguser);
                    }

                }
                return lst;
            } catch (Exception er) {
                return null;
            }
        }

        public List<messageusers> GetRoleUsersSearchByName(String Name) {
            try {
                List<messageusers> lst = new List<messageusers>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    messageusers msguser = new messageusers();
                    var users = db.UserProfiles.Where(x => (x.FirstName + " " + x.LastName).Contains(Name)).ToList();
                    foreach (var item in users) {
                        var lastuserMessage = db.MessageRecipients.Where(m => m.MsgTo == item.Id).FirstOrDefault();
                        if (lastuserMessage != null) {
                            var message = db.Messages.Where(m => m.Id == lastuserMessage.MessageId).OrderByDescending(m => m.Id).FirstOrDefault();
                            var dateTime = TimeZoneInfo.ConvertTime(message.Date, timezone);

                            if (message != null) {
                                msguser.LastMessage = message.Message1;
                                //msguser.MessageTime = message.Date.ToString("dd/MM/yyyy");
                                msguser.MessageTime = dateTime.Date.ToString("dd/MM/yyyy");
                            } else {
                                msguser.LastMessage = "No Message";
                                msguser.MessageTime = " ";
                            }
                        } else {
                            msguser.LastMessage = "No Message";
                            msguser.MessageTime = " ";
                        }
                        msguser.UserId = item.Id;
                        msguser.Name = item.FirstName + " " + item.LastName;
                        msguser.RoalId = item.UserRole;
                        lst.Add(msguser);
                    }


                }
                return lst;
            } catch (Exception er) {
                return null;
            }
        }

        public List<messageusers> GetAllUsers(String UserId) {
            try {
                List<messageusers> lst = new List<messageusers>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    List<IdentityRole> idtlst = new List<IdentityRole>();
                    var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault();
                    var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                    var roleIndex = roleLevels.Where(x => x.RoleId == userRole.RoleId).FirstOrDefault().SortOrder;
                    roleLevels = roleLevels.Where(x => x.SortOrder > roleIndex).ToList();
                    foreach (var item in roleLevels) {
                        var role = RoleManager.FindById(item.RoleId);
                        idtlst.Add(role);
                    }

                    foreach (var rollitem in idtlst) {
                        var url = RoleManager.FindById(rollitem.Id).Users;
                        foreach (var useritem in url) {
                            messageusers msguser = new messageusers();
                            var user = db.UserProfiles.Where(x => x.Id == useritem.UserId).FirstOrDefault();
                            if (user != null) {
                                var lastuserMessage = db.MessageRecipients.Where(m => m.MsgTo == user.Id).OrderByDescending(m => m.MessageId).FirstOrDefault();
                                if (lastuserMessage != null) {
                                    var message = db.Messages.Where(m => m.Id == lastuserMessage.MessageId).FirstOrDefault();
                                    var dateTime = TimeZoneInfo.ConvertTime(message.Date, timezone);

                                    if (message != null) {
                                        msguser.LastMessage = message.Message1;
                                        //msguser.MessageTime = message.Date.ToString("dd/MM/yyyy");
                                        msguser.MessageTime = dateTime.Date.ToString("dd/MM/yyyy");
                                    } else {
                                        msguser.LastMessage = "No Message";
                                        msguser.MessageTime = " ";
                                    }
                                } else {
                                    msguser.LastMessage = "No Message";
                                    msguser.MessageTime = " ";
                                }

                                if (user.Image != null) {
                                    string path = AppDomain.CurrentDomain.BaseDirectory;
                                    String p = user.Image.Replace("/", "\\");

                                    try {
                                        msguser.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                                    } catch (Exception ex) {

                                    }
                                }

                                msguser.UserId = user.Id;
                                msguser.Name = user.FirstName + " " + user.LastName;
                                msguser.RoalId = user.UserRole;
                                lst.Add(msguser);
                            }

                        }
                    }

                    var imm = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();
                    if (imm != null) {
                        messageusers msguser = new messageusers();
                        var user = db.UserProfiles.Where(x => x.Id == imm.Id).FirstOrDefault();
                        if (user != null) {
                            var lastuserMessage = db.MessageRecipients.Where(m => m.MsgTo == user.Id).OrderByDescending(m => m.MessageId).FirstOrDefault();
                            if (lastuserMessage != null) {
                                var message = db.Messages.Where(m => m.Id == lastuserMessage.MessageId).FirstOrDefault();
                                var dateTime = TimeZoneInfo.ConvertTime(message.Date, timezone);

                                if (message != null) {
                                    msguser.LastMessage = message.Message1;
                                    //msguser.MessageTime = message.Date.ToString("dd/MM/yyyy");
                                    msguser.MessageTime = dateTime.Date.ToString("dd/MM/yyyy");
                                } else {
                                    msguser.LastMessage = "No Message";
                                    msguser.MessageTime = " ";
                                }
                            } else {
                                msguser.LastMessage = "No Message";
                                msguser.MessageTime = " ";
                            }

                            if (user.Image != null) {
                                string path = AppDomain.CurrentDomain.BaseDirectory;
                                String p = user.Image.Replace("/", "\\");

                                try {
                                    msguser.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                                } catch (Exception ex) {

                                }
                            }
                            msguser.UserId = user.Id;
                            msguser.Name = user.FirstName + " " + user.LastName;
                            msguser.RoalId = user.UserRole;
                            lst.Add(msguser);
                        }
                    }
                }
                return lst;
            } catch (Exception er) {
                return null;
            }
        }

        public List<messageusers> GetSearchUsers(String UserId, String UserName) {
            try {
                List<messageusers> lst = new List<messageusers>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    List<IdentityRole> idtlst = new List<IdentityRole>();
                    var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault();
                    var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                    var roleIndex = roleLevels.Where(x => x.RoleId == userRole.RoleId).FirstOrDefault().SortOrder;
                    roleLevels = roleLevels.Where(x => x.SortOrder > roleIndex).ToList();
                    foreach (var item in roleLevels) {
                        var role = RoleManager.FindById(item.RoleId);
                        idtlst.Add(role);
                    }

                    foreach (var rollitem in idtlst) {
                        var url = RoleManager.FindById(rollitem.Id).Users;
                        foreach (var useritem in url) {
                            messageusers msguser = new messageusers();
                            var user = db.UserProfiles.Where(x => x.Id == useritem.UserId && (x.FirstName + " " + x.LastName).Contains(UserName)).FirstOrDefault();
                            if (user != null) {
                                var lastuserMessage = db.MessageRecipients.Where(m => m.MsgTo == user.Id).OrderByDescending(m => m.MessageId).FirstOrDefault();
                                if (lastuserMessage != null) {
                                    var message = db.Messages.Where(m => m.Id == lastuserMessage.MessageId).FirstOrDefault();
                                    var dateTime = TimeZoneInfo.ConvertTime(message.Date, timezone);

                                    if (message != null) {
                                        msguser.LastMessage = message.Message1;
                                        //msguser.MessageTime = message.Date.ToString("dd/MM/yyyy");
                                        msguser.MessageTime = dateTime.Date.ToString("dd/MM/yyyy");
                                    } else {
                                        msguser.LastMessage = "No Message";
                                        msguser.MessageTime = " ";
                                    }
                                } else {
                                    msguser.LastMessage = "No Message";
                                    msguser.MessageTime = " ";
                                }

                                if (user.Image != null) {
                                    string path = AppDomain.CurrentDomain.BaseDirectory;
                                    String p = user.Image.Replace("/", "\\");

                                    try {
                                        msguser.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                                    } catch (Exception ex) { }
                                }
                                msguser.UserId = user.Id;
                                msguser.Name = user.FirstName + " " + user.LastName;
                                msguser.RoalId = user.UserRole;
                                lst.Add(msguser);
                            }

                        }
                    }

                    var imm = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();
                    if (imm != null) {
                        messageusers msguser = new messageusers();
                        var user = db.UserProfiles.Where(x => x.Id == imm.SupervisorId).FirstOrDefault();
                        if (user != null) {
                            var lastuserMessage = db.MessageRecipients.Where(m => m.MsgTo == user.Id).OrderByDescending(m => m.MessageId).FirstOrDefault();
                            if (lastuserMessage != null) {
                                var message = db.Messages.Where(m => m.Id == lastuserMessage.MessageId).FirstOrDefault();
                                var dateTime = TimeZoneInfo.ConvertTime(message.Date, timezone);

                                if (message != null) {
                                    msguser.LastMessage = message.Message1;
                                    //msguser.MessageTime = message.Date.ToString("dd/MM/yyyy");
                                    msguser.MessageTime = dateTime.Date.ToString("dd/MM/yyyy");
                                } else {
                                    msguser.LastMessage = "No Message";
                                    msguser.MessageTime = " ";
                                }
                            } else {
                                msguser.LastMessage = "No Message";
                                msguser.MessageTime = " ";
                            }

                            if (user.Image != null) {
                                string path = AppDomain.CurrentDomain.BaseDirectory;
                                String p = user.Image.Replace("/", "\\");

                                try {
                                    msguser.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                                } catch (Exception ex) {

                                }
                            }
                            msguser.UserId = user.Id;
                            msguser.Name = user.FirstName + " " + user.LastName;
                            msguser.RoalId = user.UserRole;
                            lst.Add(msguser);
                        }
                    }
                }
                return lst;
            } catch (Exception er) {
                return null;
            }
        }

        public List<Messages> GetMessageList(String UserId, String Receip) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    List<Messages> msglist = new List<Messages>();

                    //var msglst = (from mr in db.MessageRecipients
                    //           join ms in db.Messages on mr.MessageId equals ms.Id
                    //           where mr.MsgTo == UserId || ms.MsgFrom == UserId
                    //           select new { ms.Id, ms.Message1, ms.MsgFrom, mr.MsgTo, ms.Date }).ToList();

                    var msglstTo = (from mr in db.MessageRecipients
                                    join ms in db.Messages on mr.MessageId equals ms.Id
                                    where mr.MsgTo == UserId && ms.MsgFrom == Receip
                                    select new { ms.Id, ms.Message1, ms.MsgFrom, mr.MsgTo, ms.Date }).ToList();

                    //var msglstFrom = (from message in db.Messages
                    //                  join msrec in db.MessageRecipients on message.Id equals msrec.MessageId
                    //                  where ms.MsgFrom == UserId && mr.MsgTo == Receip
                    //                  select new { ms.Id, ms.Message1, ms.MsgFrom, mr.MsgTo, ms.Date }).ToList();

                    var msglstFrom = (from mr in db.MessageRecipients
                                    join ms in db.Messages on mr.MessageId equals ms.Id
                                    where mr.MsgTo == Receip && ms.MsgFrom == UserId
                                    select new { ms.Id, ms.Message1, ms.MsgFrom, mr.MsgTo, ms.Date }).ToList();

                    List<Messages> msglst = new List<Messages>();
                    foreach (var item in msglstTo) {
                        Messages msgm = new Messages();
                        msgm.Id = item.Id;
                        msgm.message = item.Message1;
                        msgm.Date = item.Date;
                        msgm.msgtype = "To";
                        msglst.Add(msgm);
                    }


                    foreach (var item in msglstFrom) {
                        Messages msgm = new Messages();
                        msgm.Id = item.Id;
                        msgm.message = item.Message1;
                        msgm.Date = item.Date;
                        msgm.msgtype = "From";
                        msglst.Add(msgm);
                    }

                    msglst = msglst.OrderBy(m => m.Id).ToList();
                    //var msglst = (from msg in db.Messages
                    //              join rec in db.MessageRecipients
                    //              on msg.Id equals rec.MessageId
                    //              where msg.MsgFrom == User || rec.MsgTo == Receip
                    //              select new { msg.Message1, msg.Date, rec.MsgTo }).ToList();

                    foreach (var item in msglst) {
                        //var dateTime = TimeZoneInfo.ConvertTime(item.Date, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));

                        Messages mm = new Messages();
                        mm.message = item.message;
                        mm.time = item.Date.ToString("HH:mm dd/MM");
                        mm.msgtype = item.msgtype;
                        msglist.Add(mm);

                    }

                    return msglist;
                }
            } catch (Exception ex) {
                return null;
            }
        }

        public String SendMsg(Messages msg) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                    Message message = new Message();
                    message.Message1 = msg.message;
                    message.MsgFrom = msg.userid;
                    //message.Date = DateTime.Now;
                    message.Date = dateTime;
                    db.Messages.Add(message);
                    db.SaveChanges();
                    MessageRecipient re = new MessageRecipient();
                    re.MessageId = message.Id;
                    re.MsgTo = msg.reid;
                    db.MessageRecipients.Add(re);
                    db.SaveChanges();
                    new _notifications().AddNotifications(msg.reid, message.Id, "message");
                    return "Save";
                }
            } catch (Exception ex) {
                return "Error";
            }
        }

        public Boolean SendGroupMsgs(String Mesg, String UserList, String From) {
            List<string> msglst = UserList.Split(',').ToList<string>();

            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                    Message message = new Message();
                    message.Message1 = Mesg;
                    message.MsgFrom = From;
                    //message.Date = DateTime.Now;
                    message.Date = dateTime;
                    db.Messages.Add(message);
                    db.SaveChanges();
                    foreach (var item in msglst) {
                        MessageRecipient re = new MessageRecipient();
                        re.MessageId = message.Id;
                        re.MsgTo = item;
                        db.MessageRecipients.Add(re);
                        db.SaveChanges();

                        new _notifications().AddNotifications(item, message.Id, "message");
                    }
                    return true;
                }
            } catch (Exception ex) {
                return false;
            }
        }

        public String SendMessagesToImmed(String UserId, String Msg) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var EmployeementInfos = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();
                    if (EmployeementInfos != null) {
                        if (EmployeementInfos.SupervisorId == null) {
                            return "Sorry, Immediate Reporting Person Dosn't Exist";
                        } else {
                            var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                            Message message = new Message();
                            message.Message1 = Msg;
                            message.MsgFrom = UserId;
                            //message.Date = DateTime.Now;
                            message.Date = dateTime;
                            db.Messages.Add(message);
                            db.SaveChanges();
                            MessageRecipient re = new MessageRecipient();
                            re.MessageId = message.Id;
                            re.MsgTo = EmployeementInfos.SupervisorId;
                            db.MessageRecipients.Add(re);
                            db.SaveChanges();
                            new _notifications().AddNotifications(EmployeementInfos.SupervisorId, message.Id, "message");
                            return "Message Sent";
                        }
                    } else {
                        return "Sorry, Message Sending Faild";
                    }
                }
            } catch (Exception ex) {
                return "Sorry, Message Sending Faild";
            }
        }

        public List<Messages> GetMessageallList(String User) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    List<Messages> msglist = new List<Messages>();
                    var msglst = (from msg in db.Messages
                                  join rec in db.MessageRecipients
                                  on msg.Id equals rec.MessageId
                                  where rec.MsgTo == User
                                  select new { msg.Message1, msg.Date, rec.MsgTo, msg.MsgFrom }).ToList();
                    foreach (var item in msglst) {
                        var dateTime = TimeZoneInfo.ConvertTime(item.Date, timezone);
                        Messages mm = new Messages();
                        mm.message = item.Message1;
                        //mm.time = item.Date.ToString("HH:mm dd/MM");
                        mm.time = dateTime.Date.ToString("HH:mm dd/MM");
                        mm.userid = item.MsgFrom;
                        var up = db.UserProfiles.Where(m => m.Id == item.MsgFrom).FirstOrDefault();
                        if (up != null) {
                            mm.fromid = up.FirstName;
                            if (up.Image != null) {
                                string path = AppDomain.CurrentDomain.BaseDirectory;
                                String p = up.Image.Replace("/", "\\");
                                try {
                                    mm.msgtype = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                                } catch (Exception ex) {
                                    mm.msgtype = "images/av1.png";
                                }
                            }
                        }
                        msglist.Add(mm);
                    }
                    return msglist;
                }
            } catch (Exception ex) {
                return null;
            }
        }

        public List<MsgThread> GetMsgThread(string UserId, string From) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    List<MsgThread> lst = new List<Models.MsgThread>();

                    string path = AppDomain.CurrentDomain.BaseDirectory;

                    var msg = (from mr in db.MessageRecipients
                               join ms in db.Messages on mr.MessageId equals ms.Id
                               where mr.MsgTo == UserId || ms.MsgFrom == UserId
                               select new { ms.Id, ms.Message1, ms.MsgFrom, mr.MsgTo, ms.Date }).ToList();

                    foreach (var item in msg)
                    {
                        var dateTime = TimeZoneInfo.ConvertTime(item.Date, timezone);

                        if (item.MsgTo == UserId)
                        {
                            var user = db.UserProfiles.Where(x => x.Id == item.MsgFrom).FirstOrDefault();
                            string p = "";

                            string userImage = "";

                            try {
                                p = user.Image.Replace("/", "\\");
                                userImage = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                            } catch {
                                userImage = "images/av1.png";
                            }

                            //lst.Add(new Models.MsgThread { Id = item.Id, Date = item.Date, Msg = item.Message1, MsgUser = user.FirstName, UserImage = userImage, IsRecieved = true });
                            lst.Add(new Models.MsgThread { Id = item.Id, Date = dateTime.Date, Msg = item.Message1, MsgUser = user.FirstName, UserImage = userImage, IsRecieved = true });
                        }
                        else
                        {
                            var user = db.UserProfiles.Where(x => x.Id == item.MsgTo).FirstOrDefault();
                            string p = "";
                            string userImage = "";

                            try {
                                p = user.Image.Replace("/", "\\");
                                userImage = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                            } catch {
                                userImage = "images/av1.png";
                            }

                            //lst.Add(new Models.MsgThread { Id = item.Id, Date = item.Date, Msg = item.Message1, MsgUser = user.FirstName, UserImage = userImage, IsRecieved = false });
                            lst.Add(new Models.MsgThread { Id = item.Id, Date = dateTime.Date, Msg = item.Message1, MsgUser = user.FirstName, UserImage = userImage, IsRecieved = false });
                        }

                    }

                    return lst.OrderByDescending(x => x.Date).ToList();
                }
            } catch (Exception er) {
                return null;
            }
        }

        public List<MsgIn> GetMsgIn(string UserId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    List<MsgIn> lst = new List<Models.MsgIn>();
                    string path = AppDomain.CurrentDomain.BaseDirectory;

                    var msgIn = db.MessageRecipients.Where(x => x.MsgTo == UserId).ToList();

                    foreach (var item in msgIn)
                    {                        

                        var msg = db.Messages.Where(x => x.Id == item.MessageId).FirstOrDefault();
                        var user = db.UserProfiles.Where(x => x.Id == msg.MsgFrom).FirstOrDefault();

                        var dateTime = TimeZoneInfo.ConvertTime(msg.Date, timezone);

                        string p = "";

                        try {
                            p = user.Image.Replace("/", "\\");
                            user.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                        } catch {
                            user.Image = "images/av1.png";
                        }

                        //lst.Add(new Models.MsgIn { Id = item.MessageId, Date = msg.Date, Msg = msg.Message1, UserId = user.Id, Username = user.FirstName, FromImage = user.Image });
                        lst.Add(new Models.MsgIn { Id = item.MessageId, Date = dateTime.Date, Msg = msg.Message1, UserId = user.Id, Username = user.FirstName, FromImage = user.Image });
                    }

                    return lst.OrderByDescending(x => x.Date).ToList();
                }
            } catch (Exception er) {
                return null;
            }
        }
    }
}