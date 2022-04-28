using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.App_Codes
{
    public class _transfers
    {
        private static TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");

        public List<SelectListItem> GetSiteLocations()
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<SelectListItem> lst = new List<SelectListItem>();

                    var loc = db.SiteLocations.ToList();

                    lst.Add(new SelectListItem { Text = "[Select a Location]", Value = "0" });

                    foreach (var item in loc)
                    {
                        lst.Add(new SelectListItem { Text = item.Location, Value = item.Location});
                    }

                    return lst;
                }
            }
            catch
            {
                return null;
            }
        }
        
        public List<EmployeeTransfer> GetTransfers(int CompanyId, int Year)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    if(CompanyId == 0)
                    {
                        var trn = db.EmployeeTransfers.Where(x => x.RequestDate.Year >= Year && x.RequestDate.Year <= Year).ToList();

                        if (trn != null)
                        {
                            _usermanager um = new _usermanager();
                            _projects pro = new _projects();

                            foreach (var item in trn)
                            {
                                var em = db.EmployeementInfos.Where(x => x.Id == item.UserId).FirstOrDefault();
                                var user = um.GetUser(item.UserId);

                                item.EmpNo = user.UserProfiles.EmployeeId;
                                item.Username = user.UserProfiles.FirstName;
                                item.RequesterName = um.GetUser(item.RequestBy).UserProfiles.FirstName;
                                item.SupervisorToName = um.GetUser(item.SupervisorTo).UserProfiles.FirstName;
                                item.SupervisorFromName = um.GetUser(item.SupervisorFromApprovedBy).UserProfiles.FirstName;
                                item.COOName = um.GetUser(em.Project.Company.COO).UserProfiles.FirstName;
                                item.CurrentDivisionName = em.Project.ProjectName;
                                item.ToDivisionName = pro.GetProject(item.TransferTo).ProjectName;
                                item.DivisionCode = user.EmployeementInfos.DivisionCode;
                            }
                        }

                        return trn;
                    }
                    else
                    {
                        var trn = db.EmployeeTransfers.Where(x => x.CompanyId == CompanyId && x.RequestDate.Year >= Year && x.RequestDate.Year <= Year).ToList();

                        if (trn != null)
                        {
                            _usermanager um = new _usermanager();
                            _projects pro = new _projects();

                            foreach (var item in trn)
                            {
                                var em = db.EmployeementInfos.Where(x => x.Id == item.UserId).FirstOrDefault();
                                var user = um.GetUser(item.UserId);

                                item.EmpNo = user.UserProfiles.EmployeeId;
                                item.Username = user.UserProfiles.FirstName;
                                item.RequesterName = um.GetUser(item.RequestBy).UserProfiles.FirstName;
                                item.SupervisorToName = um.GetUser(item.SupervisorTo).UserProfiles.FirstName;
                                item.SupervisorFromName = um.GetUser(item.SupervisorFromApprovedBy).UserProfiles.FirstName;
                                item.COOName = um.GetUser(em.Project.Company.COO).UserProfiles.FirstName;
                                item.CurrentDivisionName = em.Project.ProjectName;
                                item.ToDivisionName = pro.GetProject(item.TransferTo).ProjectName;
                                item.DivisionCode = user.EmployeementInfos.DivisionCode;
                            }
                        }

                        return trn;
                    }
                   

                    
                }
                                
            }
            catch(Exception er)
            {                
                return null;
            }
        }

        public List<EmployeeTransfer> GetRequestsToApprove(string UserId)
        {
            var cult = System.Globalization.CultureInfo.CurrentCulture;
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new _usermanager();
                    var users = db.UserProfiles.ToList();                    
                    var divisions = db.Projects.ToList();
                    var req = db.EmployeeTransfers.Where(x => x.COOApprovedBy == UserId && x.COOApproval == "pending" || x.SupervisorTo == UserId && x.SupervisorToApproval == "pending" || x.SupervisorFromApprovedBy == UserId && x.SupervisorFromApproval == "pending").ToList();

                    foreach(var item in req)
                    {
                        var divId = db.EmployeementInfos.Where(x => x.Id == item.UserId).FirstOrDefault();

                        item.SupervisorToName = users.Where(x => x.Id == item.SupervisorTo).FirstOrDefault().FirstName;
                        item.SupervisorFromName = users.Where(x => x.Id == item.SupervisorFromApprovedBy).FirstOrDefault().FirstName;

                        if(!string.IsNullOrEmpty(item.COOApprovedBy))
                        {
                            item.COOName = users.Where(x => x.Id == item.COOApprovedBy).FirstOrDefault().FirstName;
                        }
                        
                        item.Username = users.Where(x => x.Id == item.UserId).FirstOrDefault().FirstName;
                        item.CurrentDivisionName = divisions.Where(x => x.Id == Convert.ToInt32(divId.Division)).FirstOrDefault().ProjectName;
                        item.ToDivisionName = divisions.Where(x => x.Id == item.TransferTo).FirstOrDefault().ProjectName;
                        item.ReqDateStr = item.EffectiveDate.Day + " - " + cult.DateTimeFormat.GetAbbreviatedMonthName(item.EffectiveDate.Month) + " - " + item.EffectiveDate.Year;
                        item.DateStr = item.RequestDate.ToString("MM/dd/yyyy hh:mm:ss");// + " - " + cult.DateTimeFormat.GetAbbreviatedMonthName(item.RequestDate.Month)+" - " + item.RequestDate.Year;
                        item.EmpNo = users.Where(x => x.Id == item.UserId).FirstOrDefault().EmployeeId;
                        item.Designation = um.GetUserEmployement(item.UserId).Designation ?? "Not Defined";

                    }

                    return req;
                }
            }
            catch(Exception er)
            {
                return new List<EmployeeTransfer>();
            }
        }

        public bool NewTransfer(EmployeeTransfer model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();

                    //var sup = um.GetUser(model.UserId);
                    var date = DateTime.Now;
                    var coo = um.GetCOO(model.UserId);
                    var employee = um.GetUser(model.UserId);

                    model.RequestDate = TimeZoneInfo.ConvertTime(date, timezone);
                    //model.SupervisorFromApprovedBy = sup.EmployeementInfos.SupervisorId;
                    model.SupervisorFromDate = date;
                    model.SupervisorFromApproval = "pending";
                    model.SupervisorToDate = date;
                    model.COOApprovedDate = date;
                    model.COOApprovedBy = coo.ApplicationUser.Id;
                    db.EmployeeTransfers.Add(model);
                    db.SaveChanges();

                    new _notifications().AddNotifications(model.SupervisorFromApprovedBy, model.Id, "Transfer Request", model.EffectiveDate.ToString());

                    //try
                    //{
                    //    var tf = db.Projects.Where(m => m.Id == model.TransferTo).FirstOrDefault();

                    //    var senduser = db.OneSignal.Where(m => m.UserId == model.UserId).FirstOrDefault();
                    //    String[] pid = { senduser.OnesignalId };
                    //    new _onesignal().PushNotification(pid, "", "You are transferred to ‘" + ((tf != null) ? tf.ProjectName : "") + "’ ‘" + model.RequestDate.ToString("yyyy-MM-dd") + "’");
                    //    db.Notifications.Add(new Notifications { TaskId = model.Id, TaskType = "User Transfer", UserId = model.RequestBy, Date = DateTime.Now, Status = "unread" });
                    //    db.SaveChanges();

                    //    //var supervisorFrom = db.OneSignal.Where(m => m.UserId == model.SupervisorFromApprovedBy).FirstOrDefault();
                    //    //string[] pidSupFrom = { supervisorFrom.OnesignalId };
                    //    //new _onesignal().PushNotification(pidSupFrom, "", "Employee Transfer Request for ‘" + ((employee.UserProfiles.FirstName != null) ? employee.UserProfiles.FirstName : "") + "’");

                    //    //db.Notifications.Add(new Notifications { TaskId = model.Id, TaskType = "User Transfer", UserId = model.RequestBy, Date = DateTime.Now, Status = "unread" });
                    //    //db.SaveChanges();
                    //}
                    //catch (Exception ex)
                    //{

                    //}


                    return true;
                }
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public bool ApproveTransfer(TransferApprovalViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new _usermanager();
                    _projects pro = new _projects();

                    var req = db.EmployeeTransfers.Where(x => x.Id == model.RequestId).FirstOrDefault();
                    

                    if(req.SupervisorFromApproval == "pending")
                    {
                        req.SupervisorFromApproval = model.Approval;
                        req.SupervisorFromDate = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                        if(model.Approval == "false")
                        {
                            var us = um.GetUserProfile(req.SupervisorFromApprovedBy);
                            req.Remarks = "Current Supervisor Rejected:" + model.Remarks + ";";
                            new _notifications().AddNotifications(req.RequestBy, req.Id, "Transfer Request Rejected", "Request Rejected by " + us.FirstName, req.EffectiveDate.ToString());
                        }
                        else
                        {
                            req.SupervisorToApproval = "pending";
                            new _notifications().AddNotifications(req.SupervisorTo, req.Id, "Transfer Request", Date:req.EffectiveDate.ToString());
                        }
                    }
                    else if(req.SupervisorToApproval == "pending")
                    {
                        req.SupervisorToApproval = model.Approval;
                        req.SupervisorToDate = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                        if (model.Approval == "false")
                        {
                            var us = um.GetUserProfile(req.SupervisorTo);
                            req.Remarks = "Supervisor To Be Rejected:" + model.Remarks + ";";
                            new _notifications().AddNotifications(req.RequestBy, req.Id, "Transfer Request Rejected", "Request Rejected by " + us.FirstName, req.EffectiveDate.ToString());
                        }
                        else
                        {                            
                            req.COOApproval = "pending";
                            new _notifications().AddNotifications(req.COOApprovedBy, req.Id, "Transfer Request", Date: req.EffectiveDate.ToString());
                        }
                    }
                    else if (req.COOApproval == "pending")
                    {
                        req.COOApproval = model.Approval;
                        req.COOApprovedDate = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                        if (model.Approval == "false")
                        {
                            var us = um.GetUserProfile(req.COOApprovedBy);
                            req.Remarks = "COO Rejected:" + model.Remarks + ";";
                            new _notifications().AddNotifications(req.RequestBy, req.Id, "Transfer Request Rejected", "Request Rejected by " + us.FirstName, Date: req.EffectiveDate.ToString());
                        }
                        else
                        {
                            var emp = db.EmployeementInfos.Where(x => x.Id == req.UserId).FirstOrDefault();
                            emp.Division = req.TransferTo;

                            try
                            {
                                var tf = db.Projects.Where(m => m.Id == req.TransferTo).FirstOrDefault();

                                var senduser = db.OneSignal.Where(m => m.UserId == req.UserId).FirstOrDefault();
                                String[] pid = { senduser.OnesignalId };
                                new _onesignal().PushNotification(pid, "", "You are transferred to ‘" + ((tf != null) ? tf.ProjectName : "") + "’ ‘" + req.EffectiveDate.ToString("yyyy-MM-dd") + "’");
                                db.Notifications.Add(new Notifications { TaskId = req.Id, TaskType = "User Transfer", UserId = req.UserId, Date = DateTime.Now, Status = "unread" });
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    db.SaveChanges();

                    
                }

                return true;
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public List<EmployeeTransfer> GetTransferRequests(string userId)
        {
            try
            {
                var cult = System.Globalization.CultureInfo.CurrentCulture;

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<EmployeeTransfer> req = new List<Models.EmployeeTransfer>();
                    _usermanager um = new _usermanager();

                    var users = db.UserProfiles.ToList();
                    var divisions = db.Projects.ToList();
                    req = db.EmployeeTransfers.Where(x => x.RequestBy == userId).ToList();
                    
                    foreach (var item in req)
                    {
                        var divId = db.EmployeementInfos.Where(x => x.Id == item.UserId).FirstOrDefault();

                        item.SupervisorToName = users.Where(x => x.Id == item.SupervisorTo).FirstOrDefault().FirstName;
                        item.SupervisorFromName = users.Where(x => x.Id == item.SupervisorFromApprovedBy).FirstOrDefault().FirstName;
                      
                        if (!string.IsNullOrEmpty(item.COOApprovedBy))
                        {
                            item.COOName = users.Where(x => x.Id == item.COOApprovedBy).FirstOrDefault().FirstName;
                        }
               
                        item.Username = users.Where(x => x.Id == item.UserId).FirstOrDefault().FirstName;
                        item.CurrentDivisionName = divisions.Where(x => x.Id == Convert.ToInt32(divId.Division)).FirstOrDefault().ProjectName;
                        item.ToDivisionName = divisions.Where(x => x.Id == item.TransferTo).FirstOrDefault().ProjectName;
                        item.ReqDateStr = item.EffectiveDate.Day + " - " + cult.DateTimeFormat.GetAbbreviatedMonthName(item.EffectiveDate.Month) + " - " + item.EffectiveDate.Year;
                        item.EmpNo = users.Where(x => x.Id == item.UserId).FirstOrDefault().EmployeeId;
                        item.Designation = um.GetUserEmployement(item.UserId).Designation ?? "Not Defined";
                    }

                    return req.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch(Exception er)
            {
                return null;
            }
        }
    }
}