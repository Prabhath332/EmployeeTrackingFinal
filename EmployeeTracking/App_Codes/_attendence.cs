using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmployeeTracking.App_Codes
{
    public class _attendence
    {
        private static TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");

        public List<AttendenceCorrections> GetInOut(DateTime dateFrom, DateTime dateTo, int divisionId = 0)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<AttendenceCorrections> lst = new List<AttendenceCorrections>();
                    _usermanager um = new _usermanager();

                    if (divisionId == 0)
                    {
                        //var model = (from att in db.AttendenceCorrections
                        //             join us in db.UserProfiles
                        //             on att.UserId equals us.Id
                        //             join emp in db.EmployeementInfos
                        //             on us.Id equals emp.Id
                        //             where att.Date.Year == year
                        //             select new { att, us, emp }).ToList();

                        var model = (from att in db.AttendenceCorrections
                                     join us in db.UserProfiles
                                     on att.UserId equals us.Id
                                     join emp in db.EmployeementInfos
                                     on us.Id equals emp.Id
                                     where att.Date >= dateFrom && att.Date <= dateTo
                                     select new { att, us, emp }).ToList();

                        foreach (var item in model)
                        {
                            try
                            {
                                item.att.Username = item.us.FirstName;
                                item.att.EmpNo = item.us.EmployeeId;
                                item.att.Designation = item.emp.Designation;
                                item.att.Location = item.emp.PresentReportingLocation;
                                item.att.DivisionId = Convert.ToInt32(item.emp.Division);
                                item.att.DivisionName = item.emp.Project.ProjectName;
                                item.att.SupervisorName = um.GetUser(item.att.SupervisorId).UserProfiles.FirstName;
                                item.att.DivisionCode = um.GetUser(item.att.UserId).EmployeementInfos.DivisionCode;

                                if (item.att.IsApproved == "true")
                                {
                                    item.att.IsApproved = "Approved";
                                }
                                else if (item.att.IsApproved == "false")
                                {
                                    item.att.IsApproved = "Rejected";
                                }
                                else if (item.att.IsApproved == "pending")
                                {
                                    item.att.IsApproved = "Pending";
                                }

                                lst.Add(item.att);
                            }
                            catch
                            {

                            }

                        }
                    }
                    else
                    {
                        var dbs = db.AttendenceCorrections.ToList();

                        var model = (from att in db.AttendenceCorrections
                                     join us in db.UserProfiles
                                     on att.UserId equals us.Id
                                     join emp in db.EmployeementInfos
                                     on us.Id equals emp.Id
                                     where emp.Division == divisionId && att.Date >= dateFrom && att.Date <= dateTo
                                     select new { att, us, emp }).ToList();

                        foreach (var item in model)
                        {
                            try
                            {
                                item.att.Username = item.us.FirstName;
                                item.att.EmpNo = item.us.EmployeeId;
                                item.att.Designation = item.emp.Designation;
                                item.att.Location = item.emp.PresentReportingLocation;
                                item.att.DivisionId = Convert.ToInt32(item.emp.Division);
                                item.att.DivisionName = item.emp.Project.ProjectName;
                                item.att.SupervisorName = um.GetUser(item.att.SupervisorId).UserProfiles.FirstName;
                                item.att.DivisionCode = um.GetUser(item.att.UserId).EmployeementInfos.DivisionCode;

                                if (item.att.IsApproved == "true")
                                {
                                    item.att.IsApproved = "Approved";
                                }
                                else if (item.att.IsApproved == "false")
                                {
                                    item.att.IsApproved = "Rejected";
                                }
                                else if (item.att.IsApproved == "pending")
                                {
                                    item.att.IsApproved = "Pending";
                                }

                                lst.Add(item.att);
                            }
                            catch
                            {

                            }

                        }
                    }


                    return lst;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<AttendenceCorrections> GetUserInOutRequests(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<AttendenceCorrections> lst = new List<AttendenceCorrections>();

                    var model = (from att in db.AttendenceCorrections
                                 join us in db.UserProfiles
                                 on att.UserId equals us.Id
                                 join empInfo in db.EmployeementInfos on us.Id equals empInfo.Id
                                 where att.SupervisorId == UserId && att.IsApproved == "pending"
                                 select new { att, us, empInfo }).ToList();

                    foreach (var item in model)
                    {
                        AttendenceCorrections cm = new AttendenceCorrections();
                        cm.Id = item.att.Id;
                        cm.Username = item.us.FirstName;
                        cm.EmpNo = item.us.EmployeeId;
                        cm.DivisionCode = item.empInfo.DivisionCode;
                        cm.IsApproved = item.att.IsApproved;
                        cm.DateRequested = item.att.DateRequested;

                        //if(!string.IsNullOrEmpty(item.att.InReason))
                        //{

                        //}
                        //else
                        //{

                        //}

                        //if (!string.IsNullOrEmpty(item.att.InReason))
                        //{

                        //}
                        //else
                        //{

                        //}
                        cm.InTime = item.att.InTime;
                        cm.OutTime = item.att.OutTime;
                        cm.InReason = item.att.InReason ?? "-";
                        cm.OutReason = item.att.OutReason ?? "-";
                        cm.Date = item.att.Date;
                        lst.Add(cm);
                    }

                    return lst;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<AttendenceCorrections> GetMyInOutRequests(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<AttendenceCorrections> lst = new List<AttendenceCorrections>();

                    var model = (from att in db.AttendenceCorrections
                                 join us in db.UserProfiles
                                 on att.SupervisorId equals us.Id
                                 where att.UserId == UserId
                                 select new { att, us }).ToList();

                    foreach (var item in model)
                    {
                        item.att.SupervisorName = item.us.FirstName;

                        lst.Add(item.att);
                    }

                    return lst.OrderByDescending(x => x.Date).ToList();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public bool SaveInOutCorrections(AttendenceCorrections model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var att = db.AttendenceCorrections.Where(x => x.Id == model.Id).FirstOrDefault();

                    if (att != null)
                    {
                        return false;
                    }
                    else
                    {
                        //Boolean isdup = false;
                        //DateTime indate = new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, 0, 0, 0);
                        //var indup = db.AttendenceCorrections.Where(x => DbFunctions.CreateDateTime(x.InTime.Year, x.InTime.Month, x.InTime.Day, 0, 0, 0) == indate && x.UserId == model.UserId).FirstOrDefault();
                        //if (indup != null)
                        //{
                        //    if (indup.IsApproved != "cancel")
                        //    {
                        //        isdup = true;
                        //    }
                        //}
                        //DateTime oudate = new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, 0, 0, 0);
                        //var outdup = db.AttendenceCorrections.Where(x => DbFunctions.CreateDateTime(x.OutTime.Year, x.OutTime.Month, x.OutTime.Day, 0, 0, 0) == oudate && x.UserId == model.UserId).FirstOrDefault();
                        //if (outdup != null)
                        //{
                        //    if (indup.IsApproved != "cancel")
                        //    {
                        //        isdup = true;
                        //    }
                        //}
                        bool isdup = false;

                        if (isdup)
                        {
                            return false;
                        }
                        else
                        {
                            var requestTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                            //if (DateTime.pamodel.InTime)
                            DateTime inTime = new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, model.InTime.Hour, model.InTime.Minute, 0);
                            DateTime outTime = new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, model.OutTime.Hour, model.OutTime.Minute, 0);

                            model.DateRequested = requestTime;
                            model.InTime = inTime;
                            model.OutTime = outTime;
                            model.IsApproved = "pending";

                            if (model.InTime.Ticks == 0)
                            {
                                model.InReason = "";
                            }

                            if (model.OutTime.Ticks == 0)
                            {
                                model.OutReason = "";
                            }

                            db.AttendenceCorrections.Add(model);
                            db.SaveChanges();

                            new _notifications().AddNotifications(model.SupervisorId, model.Id, "inoutrequest");

                            return true;
                        }

                    }
                }


            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool ApproveInOut(ApproveInOutViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var att = db.AttendenceCorrections.Where(x => x.Id == model.Id).FirstOrDefault();

                    att.IsApproved = model.IsApproved;
                    att.Remarks = model.Remarks ?? "No Remarks";

                    db.SaveChanges();

                    if (model.IsApproved == "true")
                    {
                        new _notifications().AddNotifications(att.UserId, model.Id, "inoutapproved");
                    }
                    else if (model.IsApproved == "cancel")
                    {

                    }
                    else
                    {
                        new _notifications().AddNotifications(att.UserId, model.Id, "inoutreject");
                    }
                }

                return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        internal List<AttendenceCorrections> GetAttendanceHistory(string empId, int year)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<AttendenceCorrections> lst = new List<AttendenceCorrections>();

                    //var model = (from att in db.AttendenceCorrections
                    //             join us in db.UserProfiles
                    //             on att.UserId equals us.Id
                    //             join sup in db.UserProfiles
                    //             on att.SupervisorId equals sup.Id
                    //             join empInfo in db.EmployeementInfos on us.Id equals empInfo.Id
                    //             where us.EmployeeId == empId && att.IsApproved == "true" && att.Date.Year == year
                    //             select new { att, us, empInfo, sup }).ToList();

                    var model = (from att in db.AttendenceCorrections
                                 join us in db.UserProfiles
                                 on att.UserId equals us.Id
                                 join sup in db.UserProfiles
                                 on att.SupervisorId equals sup.Id
                                 join empInfo in db.EmployeementInfos on us.Id equals empInfo.Id
                                 where us.EmployeeId == empId && att.IsApproved == "true"
                                 select new { att, us, empInfo, sup }).ToList();

                    foreach (var item in model)
                    {
                        AttendenceCorrections cm = new AttendenceCorrections();
                        cm.Id = item.att.Id;
                        cm.Username = item.us.FirstName;
                        cm.EmpNo = item.us.EmployeeId;
                        cm.DivisionCode = item.empInfo.DivisionCode;
                        cm.IsApproved = item.att.IsApproved;
                        cm.SupervisorId = item.att.SupervisorId;
                        cm.SupervisorName = item.sup.FirstName;
                        cm.DateRequested = item.att.DateRequested;
                        cm.InTime = item.att.InTime;
                        cm.OutTime = item.att.OutTime;
                        cm.InReason = item.att.InReason ?? "-";
                        cm.OutReason = item.att.OutReason ?? "-";
                        cm.Date = item.att.Date;
                        lst.Add(cm);
                    }

                    return lst.OrderByDescending(x => x.Date).ToList();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        internal bool DeleteAttendance(int attId, string remark)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var extrecord = db.AttendenceCorrections.Where(x => x.Id == attId).FirstOrDefault();

                    if(extrecord != null)
                    {
                        extrecord.IsApproved = "deleted";
                        extrecord.Remarks = remark;
                        //db.AttendenceCorrections.Remove(extrecord);
                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception er)
            {
                return false;
            }
        }
    }
}