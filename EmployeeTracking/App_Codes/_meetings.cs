using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EmployeeTracking.App_Codes
{
    public class _meetings {
        //public List<Meeting> GetMeetings() {
        //    try {
        //        using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
        //            return db.Meetings.ToList();
        //        }

        //    } catch (Exception er) {
        //        return null;
        //    }
        //}

        public List<Meeting> GetMeetingsForUser(string UserId) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var meetings = db.Meetings.ToList();
                    var users = db.MeetingUsers.Where(x => x.UserId == UserId).ToList();
                    List<Meeting> lst = new List<Models.Meeting>();

                    if (users.Count > 0) {
                        foreach (var item in users) {
                            foreach (var sub in meetings.Where(x => x.Id == item.MeetingId)) {
                                Meeting mt = new Meeting {
                                    ArrangedBy = sub.ArrangedBy,
                                    DateFrom = sub.DateFrom,
                                    DateTo = sub.DateTo,
                                    Venue = sub.Venue,
                                    Message = sub.Message
                                };

                                lst.Add(mt);
                            }

                        }
                    }


                    return lst;
                }

            } catch (Exception er) {
                return null;
            }
        }

        public bool AddMeeting(Meeting model) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    db.Meetings.Add(model);
                    db.SaveChanges();

                }

                return true;
            } catch (Exception er) {
                return false;
            }
        }

        public bool UpdateMeeting(Meeting model) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var meeting = db.Meetings.Where(x => x.Id == model.Id).FirstOrDefault();

                    if (meeting != null) {
                        db.Entry(meeting).CurrentValues.SetValues(model);
                        db.SaveChanges();
                    }
                }

                return true;
            } catch (Exception er) {
                return false;
            }
        }

        public String CreatenewMeeting(String CreateBy, DateTime SatartDate, DateTime EndDate, String Venue, String Message, String Employees) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    Meeting mt = new Meeting();
                    mt.ArrangedBy = CreateBy;
                    mt.DateFrom = SatartDate;
                    mt.DateTo = EndDate;
                    mt.Venue = Venue;
                    mt.Message = Message;
                    mt.Status = "Active";
                    db.Meetings.Add(mt);
                    db.SaveChanges();

                    MeetingUser mtc = new MeetingUser();
                    mtc.MeetingId = mt.Id;
                    mtc.UserId = CreateBy;
                    mtc.AttendinState = "Creator";
                    db.MeetingUsers.Add(mtc);
                    db.SaveChanges();

                    List<string> msglst = Employees.Split(',').ToList<string>();
                    foreach (var item in msglst) {
                        MeetingUser mtu = new MeetingUser();
                        mtu.MeetingId = mt.Id;
                        mtu.UserId = item;
                        mtu.AttendinState = "Pending";
                        db.MeetingUsers.Add(mtu);
                        db.SaveChanges();

                        new _notifications().AddNotifications(item, mt.Id, "meeting");
                    }
                    return "Meeting Created";
                }
            } catch (Exception ex) {
                return "Error";
            }
        }

        public String UpdateMeeting(int MeetingId,String CreateBy, DateTime SatartDate, DateTime EndDate, String Venue, String Message, String Employees) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var mt = db.Meetings.Where(m => m.Id == MeetingId).FirstOrDefault();       
                    mt.DateFrom = SatartDate;
                    mt.DateTo = EndDate;
                    mt.Venue = Venue;
                    mt.Message = Message; 
                    db.SaveChanges();

                    var mtuser = db.MeetingUsers.Where(m => m.MeetingId == MeetingId).ToList();
                    db.MeetingUsers.RemoveRange(mtuser);

                    MeetingUser mtc = new MeetingUser();
                    mtc.MeetingId = mt.Id;
                    mtc.UserId = CreateBy;
                    mtc.AttendinState = "Creator";
                    db.MeetingUsers.Add(mtc);
                    db.SaveChanges();

                    List<string> msglst = Employees.Split(',').ToList<string>();
                    foreach (var item in msglst) {
                        MeetingUser mtu = new MeetingUser();
                        mtu.MeetingId = mt.Id;
                        mtu.UserId = item;
                        mtu.AttendinState = "Pending";
                        db.MeetingUsers.Add(mtu);
                        db.SaveChanges();
                        new _notifications().AddNotifications(item, mt.Id, "meeting");
                    }
                    return "Meeting Updated";
                }
            } catch (Exception ex) {
                return "Error";
            }
        }

        public List<Meetings> GetMeetings(String UserId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var lst = (from meetinguser in db.MeetingUsers
                               join meetings in db.Meetings
                               on meetinguser.MeetingId equals meetings.Id
                               join user in db.UserProfiles
                               on meetinguser.UserId equals user.Id
                               where meetings.Status == "Active" && meetinguser.UserId == UserId
                               select new { meetinguser.Id, meetinguser.MeetingId, meetings.Venue, meetings.Message, meetinguser.UserId, meetinguser.AttendinState, meetinguser.Remark, meetings.Status, user.FirstName, meetings.DateFrom, meetings.DateTo }).ToList();
                    List<Meetings> meetingdlist = new List<Meetings>();
                    foreach (var item in lst) {
                        Meetings mt = new Meetings();
                        mt.Id = item.Id;
                        mt.MeetingId = item.MeetingId;
                        mt.Venue = item.Venue;
                        mt.Message = item.Message;
                        mt.UserId = item.UserId;
                        mt.AttendinState = item.AttendinState;
                        mt.Remark = item.Remark;
                        mt.Status = item.Status;
                        mt.UserName = item.FirstName;
                        mt.DateStr = item.DateFrom.ToString("dd/MM/yy");
                        mt.StatartTimeStr = item.DateFrom.ToString("HH:mm tt");
                        mt.EndTimeStr = item.DateTo.ToString("HH:mm tt");
                        meetingdlist.Add(mt);
                    }
                    return meetingdlist;
                }
            } catch (Exception ex) {
                return null;
            }

        }

        public String AcceptMeeting(int Id) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var meeting = db.MeetingUsers.Where(m => m.Id == Id).FirstOrDefault();
                    if (meeting != null) {
                        meeting.AttendinState = "Accept";
                        db.SaveChanges();
                        return "Meeting Acceppted";
                    } else {
                        return "Record Doesn't Match";
                    }
                }
            } catch (Exception ex) {
                return "Error";
            }
        }

        public String RejectMeeting(int Id) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var meeting = db.MeetingUsers.Where(m => m.Id == Id).FirstOrDefault();
                    if (meeting != null) {
                        meeting.AttendinState = "Reject";
                        db.SaveChanges();
                        return "Meeting Rejected";
                    } else {
                        return "Record Doesn't Match";
                    }
                }
            } catch (Exception ex) {
                return "Error";
            }
        }

        public String CancelMeeting(int MeetingId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var meeting = db.Meetings.Where(m => m.Id == MeetingId).FirstOrDefault();
                    if (meeting != null) {
                        meeting.Status = "Cancel";
                        db.SaveChanges();
                        return "Meeting Cancelled";
                    } else {
                        return "Record Doesn't Match";
                    }
                }
            } catch (Exception ex) {
                return "Error";
            }
        }

        public List<Meetings> ViewMeeting(int MeetingId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var lst = (from meeting in db.Meetings
                               join meetinguser in db.MeetingUsers
                               on meeting.Id equals meetinguser.MeetingId
                               join user in db.UserProfiles
                               on meetinguser.UserId equals user.Id
                               where meeting.Id== MeetingId
                               select new { meeting.Message, meeting.Venue, meeting.DateFrom, meeting.DateTo, meetinguser.AttendinState, meetinguser.Remark, user.FirstName, user.LastName ,user.Image}).ToList();
                    List<Meetings> MeetingList = new List<Meetings>();
                    foreach(var item in lst) {
                        Meetings mt = new Meetings();
                        mt.AttendinState = item.AttendinState;
                        mt.DateStr = item.DateFrom.ToString("dd/Mm/yy");
                        mt.StatartTimeStr = item.DateFrom.ToString("HH:mm tt");
                        mt.EndTimeStr = item.DateTo.ToString("HH:mm tt");
                        mt.Message = item.Message;
                        if (item.Remark == null) {
                            mt.Remark = " ";
                        } else {
                            mt.Remark = item.Remark;
                        }
                        mt.UserName = item.FirstName + " " + item.LastName;

                        if (item.Image != null) {
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            String p = item.Image.Replace("/", "\\");
                            mt.UserImage = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                        }else {
                            mt.UserImage = "images/av1.png";
                        }

                        MeetingList.Add(mt);
                    }
                    return MeetingList;
                }
            } catch (Exception) {
                return null;
            }
        }

        public Meetings ViewsigleMeeting(int MeetingId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var meetingmodel = db.Meetings.Where(m => m.Id == MeetingId).FirstOrDefault();
                    if (meetingmodel != null) {

                        
                        Meetings model = new Meetings();
                        model.Venue = meetingmodel.Venue;
                        model.Message = meetingmodel.Message;
                        model.DateStr = meetingmodel.DateFrom.ToString("MM/dd/yyyy");
                        model.StatartTimeStr = meetingmodel.DateFrom.ToString("HH:mm tt");
                        model.EndTimeStr = meetingmodel.DateTo.ToString("HH:mm tt");

                        var userlst = (from meeting in db.Meetings
                                   join meetinguser in db.MeetingUsers
                                   on meeting.Id equals meetinguser.MeetingId
                                   join user in db.UserProfiles
                                   on meetinguser.UserId equals user.Id
                                   where meeting.Id == MeetingId
                                   select new {user.Id, meeting.Message, meeting.Venue, meeting.DateFrom, meeting.DateTo, meetinguser.AttendinState, meetinguser.Remark, user.FirstName, user.LastName, user.Image }).ToList();
                        List<meetingusermodel> userlist = new List<meetingusermodel>();
                        if (userlst != null) {

                            foreach (var item in userlst) {
                                meetingusermodel mt = new meetingusermodel();                               
                                mt.UserName = item.FirstName + " " + item.LastName;
                                mt.UserId = item.Id;
                                userlist.Add(mt);
                            }
                        }

                        model.userlist = userlist;
                        return model;
                    }else {
                        return null;
                    }                  
                   
                }
            } catch (Exception) {
                return null;
            }
        }
    }
}