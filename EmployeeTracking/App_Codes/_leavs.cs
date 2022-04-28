using EmployeeTracking.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;

namespace EmployeeTracking.App_Codes
{
    public class _leavs
    {
        public UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        public RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

        private static TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");

        public List<LeaveType> GetLeaves()
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var leaveTypes = db.LeaveTypes.Where(x => x.Id != 3014).ToList(); 
                    return leaveTypes;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public LeaveType GetLeaveType(int LeaveId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    return db.LeaveTypes.Where(x => x.Id == LeaveId).FirstOrDefault();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public LeaveType GetLeaveType(string LeaveName)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    return db.LeaveTypes.Where(x => x.LeaveType1 == LeaveName).FirstOrDefault();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserLeavesViewModel> GetUserLeaves(string UserId, int Year)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();
                    List<UserLeavesViewModel> lst = new List<Models.UserLeavesViewModel>();
                    var userLeaves = db.UserLeaves.Where(x => x.UserId == UserId && x.Year == Year).ToList();
                    //var leaves = GetLeaves();
                    var users = um.GetUsers();

                    foreach (var item in userLeaves)
                    {
                        if (users.Where(x => x.Id == item.UserId).FirstOrDefault().FirstName != "siot0004")
                        {
                            lst.Add(new Models.UserLeavesViewModel { UserId = item.UserId, LeaveType = item.LeaveType1.LeaveType1, LeaveId = Convert.ToInt32(item.LeaveType), AllocatedCount = item.AllocatedCount, RemainingCount = item.RemainingCount, Year = Year, Username = users.Where(x => x.Id == item.UserId).FirstOrDefault().FirstName });
                        }
                        //var fname = users.Where(x => x.Id == "e54fc979-b48f-4692-ad94-3b7bddac8c49").FirstOrDefault();

                    }



                    return lst;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserLeavesViewModel> GetUserLeaves(int Year)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();
                    List<UserLeavesViewModel> lst = new List<Models.UserLeavesViewModel>();
                    var userLeaves = db.UserLeaves.Where(x => x.Year == Year).ToList();
                    var users = um.GetUsers();
                    var leaves = GetLeaves();

                    foreach (var item in userLeaves)
                    {
                        var leaveType = item.LeaveType1;
                        var userName = users.Where(x => x.Id == item.UserId).FirstOrDefault();
                        lst.Add(new Models.UserLeavesViewModel { UserId = item.UserId, Username = userName.FirstName, LeaveType = item.LeaveType1.LeaveType1, AllocatedCount = item.AllocatedCount, RemainingCount = item.RemainingCount, LeaveId = leaveType.Id });

                    }

                    return lst;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserLeavesViewModel> GetUserLeaves(int DivisionId, int Year)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();
                    List<UserLeavesViewModel> lst = new List<Models.UserLeavesViewModel>();
                    var userLeaves = db.UserLeaves.Where(x => x.Year == Year).ToList();

                    if (DivisionId == 0)
                    {
                        var users = um.GetUsers();
                        var leaves = GetLeaves();

                        foreach (var item in userLeaves)
                        {
                            var userName = users.Where(x => x.Id == item.UserId).FirstOrDefault();
                            var leaveType = item.LeaveType1;

                            if (userName != null)
                            {
                                lst.Add(new Models.UserLeavesViewModel { UserId = item.UserId, Username = userName.FirstName, LeaveType = leaveType.LeaveType1, AllocatedCount = item.AllocatedCount, RemainingCount = item.RemainingCount, Year = item.Year, LeaveId = leaveType.Id });
                            }

                        }

                        foreach (var item in users)
                        {
                            var lvH = db.LeaveHistories.Where(x => x.UserId == item.Id && x.RequestDate.Year == Year).ToList();

                            foreach (var sub in leaves)
                            {
                                //if(sub.Id != 3000 || sub.Id != 3001 || sub.Id != 0)
                                //{
                                //    var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                                //    lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCount, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });
                                //}

                                if (sub.Id == 3000 || sub.Id == 3001)
                                {
                                    //var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                                    //lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCount, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });

                                    var lvCountLst = lvH.Where(x => x.LeaveType == sub.Id).ToList();
                                    var lvCountsTotal = lvCountLst.Count();
                                    var specLv = lvCountLst.Where(x => x.LeaveUnit == "ADMIN ENFORCED").ToList();

                                    if (specLv.Count() > 0)
                                    {
                                        lvCountsTotal -= specLv.Count();
                                        lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCountsTotal, RemainingCount = lvCountsTotal, Year = Year, LeaveId = sub.Id });

                                    }
                                    else
                                    {
                                        lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCountsTotal, RemainingCount = lvCountsTotal, Year = Year, LeaveId = sub.Id });
                                    }
                                }
                                else if (sub.Id == 3014)
                                {
                                    var lvCountLst = lvH.Where(x => x.LeaveUnit == "ADMIN ENFORCED").ToList();

                                    if (lvCountLst.Count() > 0)
                                    {
                                        lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = 0, RemainingCount = lvCountLst.Count(), Year = Year, LeaveId = sub.Id });
                                    }
                                }
                                else
                                {
                                    var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                                    lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = 0, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });
                                }
                            }
                            //if(lvH.Where(x => x.LeaveType != 3000 || x.LeaveType != 3001 || x.LeaveType != 0))
                        }
                    }
                    else
                    {
                        var users = um.GetUsers(DivisionId.ToString());
                        var leaves = GetLeaves();

                        foreach (var item in userLeaves)
                        {
                            var userName = users.Where(x => x.Id == item.UserId).FirstOrDefault();
                            var leaveType = item.LeaveType1;

                            if (userName != null)
                            {
                                lst.Add(new Models.UserLeavesViewModel { UserId = item.UserId, Username = userName.FirstName, LeaveType = leaveType.LeaveType1, AllocatedCount = item.AllocatedCount, RemainingCount = item.RemainingCount, Year = item.Year, LeaveId = leaveType.Id });
                            }

                        }

                        foreach (var item in users)
                        {
                            var lvH = db.LeaveHistories.Where(x => x.UserId == item.Id && x.RequestDate.Year == Year).ToList();

                            foreach (var sub in leaves)
                            {
                                //if(sub.Id != 3000 || sub.Id != 3001 || sub.Id != 0)
                                //{
                                //    var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                                //    lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCount, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });
                                //}

                                if (sub.Id == 3000 || sub.Id == 3001)
                                {
                                    //var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                                    //lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCount, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });

                                    var lvCountLst = lvH.Where(x => x.LeaveType == sub.Id).ToList();
                                    var lvCountsTotal = lvCountLst.Count();
                                    var specLv = lvCountLst.Where(x => x.LeaveUnit == "ADMIN ENFORCED").ToList();

                                    if (specLv.Count() > 0)
                                    {
                                        lvCountsTotal -= specLv.Count();
                                        lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCountsTotal, RemainingCount = lvCountsTotal, Year = Year, LeaveId = sub.Id });

                                    }
                                    else
                                    {
                                        lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCountsTotal, RemainingCount = lvCountsTotal, Year = Year, LeaveId = sub.Id });
                                    }
                                }
                                else if (sub.Id == 3014)
                                {
                                    var lvCountLst = lvH.Where(x => x.LeaveUnit == "ADMIN ENFORCED").ToList();

                                    if (lvCountLst.Count() > 0)
                                    {
                                        lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = 0, RemainingCount = lvCountLst.Count(), Year = Year, LeaveId = sub.Id });
                                    }
                                }
                                else
                                {
                                    var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                                    lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = 0, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });
                                }
                            }
                            //if(lvH.Where(x => x.LeaveType != 3000 || x.LeaveType != 3001 || x.LeaveType != 0))
                        }
                    }

                    //var users = um.GetUsers(DivisionId.ToString());
                    //var leaves = GetLeaves();

                    //foreach (var item in userLeaves)
                    //{
                    //    var userName = users.Where(x => x.Id == item.UserId).FirstOrDefault();
                    //    var leaveType = item.LeaveType1;                        

                    //    if(userName != null)
                    //    {
                    //        lst.Add(new Models.UserLeavesViewModel { UserId = item.UserId, Username = userName.FirstName, LeaveType = leaveType.LeaveType1, AllocatedCount = item.AllocatedCount, RemainingCount = item.RemainingCount, Year = item.Year, LeaveId = leaveType.Id });
                    //    }

                    //}

                    //foreach(var item in users)
                    //{
                    //    var lvH = db.LeaveHistories.Where(x => x.UserId == item.Id && x.RequestDate.Year == Year).ToList();

                    //    foreach(var sub in leaves)
                    //    {
                    //        //if(sub.Id != 3000 || sub.Id != 3001 || sub.Id != 0)
                    //        //{
                    //        //    var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                    //        //    lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCount, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });
                    //        //}

                    //        if (sub.Id == 3000 || sub.Id == 3001)
                    //        {
                    //            //var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                    //            //lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCount, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });

                    //            var lvCountLst = lvH.Where(x => x.LeaveType == sub.Id).ToList();
                    //            var lvCountsTotal = lvCountLst.Count();
                    //            var specLv = lvCountLst.Where(x => x.LeaveUnit == "ADMIN ENFORCED").ToList();

                    //            if (specLv.Count() > 0)
                    //            {
                    //                lvCountsTotal -= specLv.Count();
                    //                lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCountsTotal, RemainingCount = lvCountsTotal, Year = Year, LeaveId = sub.Id });

                    //            }
                    //            else
                    //            {
                    //                lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = lvCountsTotal, RemainingCount = lvCountsTotal, Year = Year, LeaveId = sub.Id });
                    //            }
                    //        }
                    //        else if(sub.Id == 3014)
                    //        {
                    //            var lvCountLst = lvH.Where(x => x.LeaveUnit == "ADMIN ENFORCED").ToList();

                    //            if(lvCountLst.Count() > 0)
                    //            {
                    //                lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = 0, RemainingCount = lvCountLst.Count(), Year = Year, LeaveId = sub.Id });
                    //            }
                    //        }
                    //        else
                    //        {
                    //            var lvCount = lvH.Where(x => x.LeaveType == sub.Id).Count();

                    //            lst.Add(new Models.UserLeavesViewModel { UserId = item.Id, Username = item.FirstName, LeaveType = sub.LeaveType1, AllocatedCount = 0, RemainingCount = lvCount, Year = Year, LeaveId = sub.Id });
                    //        }
                    //    }
                    //    //if(lvH.Where(x => x.LeaveType != 3000 || x.LeaveType != 3001 || x.LeaveType != 0))
                    //}

                    return lst;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public UserLeavesViewModel GetOtherLeaveTypes(int LeaveType, string UserId, int Year)
        {
            try
            {
                List<UserLeavesViewModel> lst = new List<Models.UserLeavesViewModel>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new _usermanager();
                    var user = um.GetUserRoles(UserId);
                    var lvHis = GetLeaveHistory(UserId, "0");
                    var lvs = GetUserLevelLeaves().Where(x => x.LeaveType == LeaveType && x.UserLevelId == user.FirstOrDefault()).FirstOrDefault();

                    var lvVm = new UserLeavesViewModel { LeaveId = LeaveType, RemainingCount = lvHis.Where(x => x.LeaveType == LeaveType).Count(), AllocatedCount = 0 };

                    return lvVm;
                    //foreach(var item in lvHis)
                    //{
                    //    lst.Add(new UserLeavesViewModel { LeaveId = item.});
                    //}

                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserLevelLeaves> GetUserLevelLeaves()
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.UserLevelLeaves.ToList();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserLevelLeaves> GetUserLevelLeaves(string UserRole)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new _usermanager();
                    var roles = um.GetRoles();

                    var level = db.UserLevelLeaves.Where(x => x.UserLevelId == UserRole).ToList();
                    List<UserLevelLeaves> lst = new List<Models.UserLevelLeaves>();

                    foreach (var item in level)
                    {
                        lst.Add(new Models.UserLevelLeaves { Id = item.Id, LeaveType = item.LeaveType, LeaveCount = item.LeaveCount, UserLevelId = roles.Where(x => x.Id == item.UserLevelId).FirstOrDefault().Name });
                    }

                    return lst;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<LeaveHistory> GetLeaveHistory(string EmployeeId, int Year)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<LeaveHistory> lvh = new List<LeaveHistory>();
                    _usermanager um = new _usermanager();

                    var userEmp = um.GetUserByEmp(EmployeeId);

                    //var lvs = (from history in db.LeaveHistories
                    //           join levetype in db.LeaveTypes
                    //           on history.LeaveType equals levetype.Id
                    //           join empInfo in db.EmployeementInfos
                    //           on history.UserId equals empInfo.Id
                    //           where history.RequestDate.Year == Year && history.UserId == userEmp.Id
                    //           select new { history, levetype, empInfo }).ToList();

                    var lvs = (from history in db.LeaveHistories
                               join levetype in db.LeaveTypes
                               on history.LeaveType equals levetype.Id
                               join empInfo in db.EmployeementInfos
                               on history.UserId equals empInfo.Id
                               where history.UserId == userEmp.Id
                               select new { history, levetype, empInfo }).ToList();

                    foreach (var item in lvs)
                    {
                        var approvals = db.LeaveApprovals.Where(x => x.RequestId == item.history.Id).OrderByDescending(x => x.Id).ToList().FirstOrDefault();

                        if (approvals.Status == "true")
                        {
                            LeaveHistory model = new LeaveHistory();
                            model.Id = item.history.Id;
                            model.LeaveType = item.history.LeaveType;
                            model.LeaveTypeStr = item.levetype.LeaveType1;
                            model.Reason = item.history.Reason;
                            model.Remarks = approvals.Remarks;
                            model.IsApproved = approvals.Status;
                            model.ApprovedBy = um.GetUserProfile(approvals.SupervisorId).FirstName;
                            model.LeaveDays = item.history.LeaveDays;

                            if (item.history.LeaveUnit == "HALFMOR")
                            {
                                model.LeaveUnit = "Half-Morning";
                            }
                            else if (item.history.LeaveUnit == "HALFEVE")
                            {
                                model.LeaveUnit = "Half-Evening";
                            }


                            model.CoveredBy = item.history.CoveredBy;
                            model.FromDate = item.history.FromDate;
                            model.ToDate = item.history.ToDate;
                            model.Project = item.empInfo.Project.ProjectName;
                            model.Section = item.empInfo.Section;
                            model.Requester = um.GetUserProfile(item.history.UserId).FirstName;
                            model.RequestDate = item.history.RequestDate;
                            lvh.Add(model);
                        }

                    }

                    //lvh = db.LeaveHistories.Where(x => x.UserId == UserId && x.FromDate.Year == DateTime.Now.Year && x.ToDate.Year == DateTime.Now.Year).ToList();

                    var shortLeave = (from history in db.LeaveHistories
                                      join user in db.UserProfiles
                                      on history.UserId equals user.Id
                                      join empInfo in db.EmployeementInfos
                                      on history.UserId equals empInfo.Id
                                      where history.LeaveUnit == "SHORT" && history.UserId == userEmp.Id
                                      select new { history, user, empInfo }).ToList();

                    foreach (var item in shortLeave)
                    {
                        var approvals = db.LeaveApprovals.Where(x => x.RequestId == item.history.Id).OrderByDescending(x => x.Id).ToList().FirstOrDefault();

                        if (approvals.Status == "true")
                        {
                            LeaveHistory model = new LeaveHistory();
                            model.Id = item.history.Id;
                            model.LeaveType = item.history.LeaveType;
                            model.LeaveTypeStr = "SHORT";
                            model.Reason = item.history.Reason;
                            model.Remarks = approvals.Remarks;
                            model.IsApproved = approvals.Status;
                            model.ApprovedBy = um.GetUserProfile(approvals.SupervisorId).FirstName;
                            model.LeaveDays = item.history.LeaveDays;
                            model.LeaveUnit = item.history.LeaveUnit;
                            model.CoveredBy = item.history.CoveredBy;
                            model.FromDate = item.history.FromDate;
                            model.ToDate = item.history.ToDate;
                            model.Requester = um.GetUserProfile(item.history.UserId).FirstName;
                            model.RequestDate = item.history.RequestDate;
                            model.Project = item.empInfo.Project.ProjectName;
                            model.Section = item.empInfo.Section;

                            lvh.Add(model);
                        }

                    }
                    
                    return lvh.OrderByDescending(x => x.FromDate).ToList(); ;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<LeaveHistory> GetLeaveHistory(string UserId, string Status, int Year = 0)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<LeaveHistory> lvh = new List<LeaveHistory>();
                    _usermanager um = new _usermanager();

                    if (Status == "0")
                    {
                        //var lvs = (from history in db.LeaveHistories
                        //               //join approvals in db.LeaveApprovals.OrderByDescending(x => x.Status).ToList()
                        //               //on history.Id equals approvals.RequestId
                        //           join levetype in db.LeaveTypes
                        //              on history.LeaveType equals levetype.Id
                        //           where history.UserId == UserId && history.RequestDate.Year == Year
                        //           select new { history, levetype }).ToList();

                        var lvs = (from history in db.LeaveHistories
                                       //join approvals in db.LeaveApprovals.OrderByDescending(x => x.Status).ToList()
                                       //on history.Id equals approvals.RequestId
                                   join levetype in db.LeaveTypes
                                      on history.LeaveType equals levetype.Id
                                   where history.UserId == UserId
                                   select new { history, levetype }).ToList();

                        foreach (var item in lvs)
                        {
                            var approvals = db.LeaveApprovals.Where(x => x.RequestId == item.history.Id).OrderByDescending(x => x.Id).ToList().FirstOrDefault();

                            LeaveHistory model = new LeaveHistory();
                            model.Id = item.history.Id;
                            model.LeaveType = item.history.LeaveType;
                            model.LeaveTypeStr = item.levetype.LeaveType1;
                            model.Reason = item.history.Reason;
                            model.Remarks = approvals.Remarks;
                            model.IsApproved = approvals.Status;
                            model.ApprovedBy = um.GetUserProfile(approvals.SupervisorId).FirstName;
                            model.LeaveDays = item.history.LeaveDays;
                            //model.LeaveUnit = item.history.LeaveUnit;

                            if (item.history.LeaveUnit == "HALFMOR")
                            {
                                model.LeaveUnit = "Half-Morning";
                            }
                            else if (item.history.LeaveUnit == "HALFEVE")
                            {
                                model.LeaveUnit = "Half-Evening";
                            }
                            else
                            {
                                model.LeaveUnit = item.history.LeaveUnit;
                            }

                            model.CoveredBy = item.history.CoveredBy;
                            model.FromDate = item.history.FromDate;
                            model.ToDate = item.history.ToDate;
                            model.RequestDate = item.history.RequestDate;
                            lvh.Add(model);
                        }

                        //lvh = db.LeaveHistories.Where(x => x.UserId == UserId && x.FromDate.Year == DateTime.Now.Year && x.ToDate.Year == DateTime.Now.Year).ToList();

                        var shortLeave = (from history in db.LeaveHistories
                                              //join approvals in db.LeaveApprovals
                                              //on history.Id equals approvals.RequestId
                                          join user in db.UserProfiles
                                          on history.UserId equals user.Id
                                          join empInfo in db.EmployeementInfos
                                          on history.UserId equals empInfo.Id
                                          where history.LeaveUnit == "SHORT" && history.UserId == UserId
                                          select new { history, user, empInfo }).ToList();

                        foreach (var item in shortLeave)
                        {
                            var approvals = db.LeaveApprovals.Where(x => x.RequestId == item.history.Id).OrderByDescending(x => x.Id).ToList().FirstOrDefault();

                            LeaveHistory model = new LeaveHistory();
                            model.Id = item.history.Id;
                            model.LeaveType = item.history.LeaveType;
                            model.LeaveTypeStr = "Annual";//"SHORT";
                            model.Reason = item.history.Reason;
                            model.Remarks = approvals.Remarks;
                            model.IsApproved = approvals.Status;
                            model.ApprovedBy = um.GetUserProfile(approvals.SupervisorId).FirstName;
                            model.LeaveDays = item.history.LeaveDays;
                            model.LeaveUnit = item.history.LeaveUnit;
                            model.CoveredBy = item.history.CoveredBy;
                            model.FromDate = item.history.FromDate;
                            model.ToDate = item.history.ToDate;
                            model.RequestDate = item.history.RequestDate;

                            lvh.Add(model);
                        }
                    }
                    else
                    {
                        lvh = db.LeaveHistories.Where(x => x.UserId == UserId && x.IsApproved == Status && x.FromDate.Year == DateTime.Now.Year && x.ToDate.Year == DateTime.Now.Year).ToList();
                    }

                    //foreach(var item in lvh)
                    //{
                    //    if(item.LeaveUnit == "SHORT")
                    //    {
                    //        item.LeaveTypeStr = "N/A";
                    //    }
                    //    else
                    //    {
                    //        item.LeaveTypeStr = GetLeaveType(Convert.ToInt32(item.LeaveType)).LeaveType1;
                    //    }

                    //}

                    return lvh.OrderByDescending(x => x.FromDate).ToList();
                }
            }
            catch (Exception er)
            {
                return new List<Models.LeaveHistory>();
            }
        }

        public List<LeaveHistory> GetLeaveHistory(DateTime From, DateTime To, string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new _usermanager();

                    var lvs = db.LeaveHistories.Where(x => x.FromDate >= From && x.ToDate <= To && x.UserId == UserId).ToList();
                    var leaveTypes = db.LeaveTypes.ToList();

                    if (lvs.Count() > 0)
                    {
                        foreach (var sub in lvs)
                        {
                            var approvals = db.LeaveApprovals.Where(x => x.RequestId == sub.Id).OrderByDescending(x => x.Id).ToList().FirstOrDefault();
                            var emp = um.GetUserEmployement(sub.UserId);

                            if (sub.LeaveType == 0)
                            {
                                sub.LeaveTypeStr = "Short Leave";
                                sub.Remarks = approvals.Remarks;
                                sub.IsApproved = approvals.Status;
                                sub.ApprovedBy = um.GetUserProfile(approvals.SupervisorId).FirstName;

                                if (sub.LeaveUnit == "HALFMOR")
                                {
                                    sub.LeaveUnit = "Half-Morning";
                                }
                                else if (sub.LeaveUnit == "HALFEVE")
                                {
                                    sub.LeaveUnit = "Half-Evening";
                                }
                                else
                                {
                                    sub.LeaveUnit = sub.LeaveUnit;
                                }
                            }
                            else
                            {
                                sub.LeaveTypeStr = leaveTypes.Where(x => x.Id == sub.LeaveType).FirstOrDefault().LeaveType1;
                                sub.Remarks = approvals.Remarks;
                                sub.IsApproved = approvals.Status;
                                sub.ApprovedBy = um.GetUserProfile(approvals.SupervisorId).FirstName;

                                if (sub.LeaveUnit == "HALFMOR")
                                {
                                    sub.LeaveUnit = "Half-Morning";
                                }
                                else if (sub.LeaveUnit == "HALFEVE")
                                {
                                    sub.LeaveUnit = "Half-Evening";
                                }
                                else
                                {
                                    sub.LeaveUnit = sub.LeaveUnit;
                                }
                            }


                        }
                    }

                    return lvs;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UsedLeavesByUser> GetUsedLeaveHistory(DateTime From, DateTime To, List<string> Users)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new _usermanager();
                    List<UsedLeavesByUser> lst = new List<UsedLeavesByUser>();

                    var lvs = db.LeaveHistories.Where(x => x.FromDate >= From && x.ToDate <= To && Users.Contains(x.UserId) && x.IsApproved == "true").ToList();

                    var leaveTypes = db.LeaveTypes.ToList();
                    leaveTypes.Add(new LeaveType { Id = 0, LeaveType1 = "SHORT", LeaveTypeCode = "SL" });

                    if (lvs.Count() > 0)
                    {
                        foreach (var lvUser in Users)
                        {
                            UsedLeavesByUser model = new UsedLeavesByUser();
                            List<UsedLeaves> userLvs = new List<UsedLeaves>();

                            var emp = um.GetUserProfile(lvUser);
                            model.EmpNo = emp.EmployeeId;
                            model.Period = From.Month + "/" + From.Year;

                            foreach (var lvType in leaveTypes)
                            {
                                UsedLeaves lvModel = new UsedLeaves();
                                lvModel.LeaveType_Id = lvType.Id;
                                lvModel.Leave_Name = lvType.LeaveTypeCode;
                                lvModel.Used_Count = Convert.ToDouble(lvs.Where(x => x.LeaveType == lvType.Id && x.UserId == lvUser).Sum(x => x.LeaveDays));

                                userLvs.Add(lvModel);
                            }

                            model.UsedLeaves = userLvs;
                            model.LeaveTypes = leaveTypes;

                            lst.Add(model);
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

        public List<RejectedLeave> GetRejectedLeaves(string UserId = "")
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    if (string.IsNullOrEmpty(UserId))
                    {
                        return db.RejectedLeaves.ToList();
                    }
                    else
                    {
                        return db.RejectedLeaves.Where(x => x.UserId == UserId).ToList();
                    }
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public bool AddLeaveType(string LeaveName, int LeaveId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    if (LeaveId == 0)
                    {
                        var lvType = new Models.LeaveType { LeaveType1 = LeaveName };

                        db.LeaveTypes.Add(lvType);
                        db.SaveChanges();

                        _usermanager um = new App_Codes._usermanager();
                        var roles = um.GetRoles();
                        var userLv = new List<UserLevelLeaves>();

                        foreach (var item in roles)
                        {
                            userLv.Add(new Models.UserLevelLeaves { UserLevelId = item.Id, LeaveType = lvType.Id, LeaveCount = 0 });
                        }

                        db.UserLevelLeaves.AddRange(userLv);
                        db.SaveChanges();

                    }
                    else
                    {
                        var leave = db.LeaveTypes.Where(x => x.Id == LeaveId).FirstOrDefault();

                        db.Entry(leave).CurrentValues.SetValues(new Models.LeaveType { Id = LeaveId, LeaveType1 = LeaveName });
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

        public bool AddUserLeave(string UserId, string RoleId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<UserLeaves> lst = new List<Models.UserLeaves>();
                    var roleLeaves = db.UserLevelLeaves.Where(x => x.UserLevelId == RoleId).ToList();

                    foreach (var item in roleLeaves)
                    {
                        lst.Add(new Models.UserLeaves { UserId = UserId, LeaveType = item.LeaveType, AllocatedCount = item.LeaveCount, RemainingCount = item.LeaveCount, Year = DateTime.Now.Year });
                    }

                    db.UserLeaves.AddRange(lst);
                    db.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool AddUserLeave(List<UserLeaves> lst)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    db.UserLeaves.AddRange(lst);
                    db.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUserLeave(string UserId, int LeaveType, float AllocatedCount, int Year)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var leave = db.UserLeaves.Where(x => x.UserId == UserId && x.LeaveType == LeaveType && x.Year == Year).FirstOrDefault();

                    if (leave != null)
                    {
                        db.Entry(leave).CurrentValues.SetValues(new UserLeaves { UserId = UserId, LeaveType = LeaveType, AllocatedCount = AllocatedCount, RemainingCount = AllocatedCount, Year = Year });
                        db.SaveChanges();
                    }
                    else
                    {
                        db.UserLeaves.Add(new UserLeaves { UserId = UserId, LeaveType = LeaveType, AllocatedCount = AllocatedCount, RemainingCount = AllocatedCount, Year = Year });
                        db.SaveChanges();
                    }
                    //leave.AllocatedCount = AllocatedCount;
                    //db.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUserLeave(string UserId, int LeaveType, float AllocatedCount, float RemaningCount, int Year)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var leave = db.UserLeaves.Where(x => x.UserId == UserId && x.LeaveType == LeaveType && x.Year == Year).FirstOrDefault();

                    if (leave != null)
                    {
                        //var lvModel = new UserLeaves
                        //{
                        //    LeaveType = LeaveType,
                        //    AllocatedCount = AllocatedCount,
                        //    RemainingCount = RemaningCount,
                        //    Year = Year
                        //};

                        leave.LeaveType = LeaveType;
                        leave.AllocatedCount = AllocatedCount;
                        leave.RemainingCount = RemaningCount;
                        leave.Year = Year;

                        //db.Entry(leave).CurrentValues.SetValues(lvModel);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.UserLeaves.Add(new UserLeaves { UserId = UserId, LeaveType = LeaveType, AllocatedCount = AllocatedCount, RemainingCount = RemaningCount, Year = Year });
                        db.SaveChanges();
                    }
                    //leave.AllocatedCount = AllocatedCount;
                    //db.SaveChanges();

                    return true;
                }
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool AddLevelLeaves(string RoleId, int LeaveType, float Count)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    if (RoleId == "0")
                    {
                        _usermanager um = new App_Codes._usermanager();

                        var roles = um.GetRoles();

                        foreach (var item in roles)
                        {
                            var level = db.UserLevelLeaves.Where(x => x.UserLevelId == item.Id && x.LeaveType == LeaveType).FirstOrDefault();

                            if (level != null)
                            {
                                //db.Entry(level).CurrentValues.SetValues(new Models.UserLevelLeaves { UserLevelId = item.Id, LeaveType = LeaveType, LeaveCount = Count });
                                level.UserLevelId = item.Id;
                                level.LeaveType = LeaveType;
                                level.LeaveCount = Count;

                                db.SaveChanges();
                            }
                            else
                            {
                                db.UserLevelLeaves.Add(new Models.UserLevelLeaves { UserLevelId = item.Id, LeaveType = LeaveType, LeaveCount = Count });
                                db.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        _usermanager um = new _usermanager();
                        RoleId = um.GetRoles().Where(x => x.Name == RoleId).FirstOrDefault().Id;

                        var level = db.UserLevelLeaves.Where(x => x.UserLevelId == RoleId && x.LeaveType == LeaveType).FirstOrDefault();

                        if (level != null)
                        {
                            //db.Entry(level).CurrentValues.SetValues(new Models.UserLevelLeaves { UserLevelId = RoleId, LeaveType = LeaveType, LeaveCount = Count });
                            level.UserLevelId = RoleId;
                            level.LeaveType = LeaveType;
                            level.LeaveCount = Count;
                            db.SaveChanges();
                        }
                        else
                        {
                            db.UserLevelLeaves.Add(new Models.UserLevelLeaves { UserLevelId = RoleId, LeaveType = LeaveType, LeaveCount = Count });
                            db.SaveChanges();
                        }
                    }

                    return true;
                }
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool IsWorkOnSaturday(string userId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    bool res = Convert.ToBoolean(db.EmployeementInfos.Where(x => x.Id == userId).FirstOrDefault().WorkOnSaturday);
                    return res;
                }
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public string RequestLeave(LeaveHistory model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    //_holidays hl = new _holidays();
                    var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                    if (model.LeaveType == 0)
                    {
                        if (IsSaturday(model.FromDate, model.ToDate))
                        {
                            return "Your Not Allowed to Apply Short Leave on Saturday";
                        }
                        else
                        {
                            if (!IsInRange(model))
                            {
                                var lvHistoy = db.LeaveHistories.Where(x => x.UserId == model.UserId && x.FromDate == model.FromDate).FirstOrDefault();

                                if (lvHistoy == null)
                                {
                                    var lvModel = new LeaveHistory { UserId = model.UserId, FromDate = model.FromDate, ToDate = model.FromDate, LeaveType = model.LeaveType, LeaveUnit = model.LeaveUnit, CoveredBy = model.CoveredBy, LeaveDays = 0.25, Reason = model.Reason, RequestDate = dateTime };

                                    db.LeaveHistories.Add(lvModel);
                                    db.SaveChanges();

                                    db.LeaveApprovals.Add(new LeaveApproval { RequestId = lvModel.Id, Date = dateTime, Status = "pending", SupervisorId = model.SupervisorId });
                                    db.SaveChanges();

                                    new _notifications().AddNotifications(model.SupervisorId, model.Id, "Leave Request");

                                    return "Leave Request Sent";
                                }
                                else
                                {
                                    return "You Already Have Applied Leave For This Period";
                                }
                            }
                            else
                            {
                                return "Existing Record Overlaps with Current Request";
                            }
                        }


                    }
                    else if (model.LeaveType == 3000 || model.LeaveType == 3001)
                    {
                        if (!IsInRange(model))
                        {
                            var lvHistoy = db.LeaveHistories.Where(x => x.UserId == model.UserId && x.FromDate == model.FromDate && x.ToDate == model.ToDate).FirstOrDefault();

                            //var lvHis = (from lvh in db.LeaveHistories
                            //             join lva in db.LeaveApprovals on lvh.Id equals lva.RequestId
                            //             where lvh.UserId == model.UserId && lvh.FromDate == model.FromDate && lvh.ToDate == model.ToDate && lva.Status != "false" || lva.Status != "canceled"
                            //             select new { lvh }).OrderByDescending(x => x.lvh.Id).FirstOrDefault();

                            if (lvHistoy == null)
                            {
                                if (IsSaturday(model.FromDate, model.ToDate))
                                {
                                    model.LeaveUnit = "SATURDAY";
                                    model.LeaveDays = 0.5;
                                }

                                var userLeave = db.UserLeaves.Where(x => x.UserId == model.UserId && x.LeaveType == model.LeaveType && x.Year == DateTime.Now.Year).FirstOrDefault();

                                //var dayCount = (model.ToDate - model.FromDate).TotalDays + 1;
                                double dayCount = Convert.ToDouble(model.LeaveDays);

                                //var actualDayCount = hl.ActualDayCount(model.FromDate, model.ToDate);

                                //if(dayCount != actualDayCount)
                                //{
                                //    dayCount = actualDayCount;
                                //}

                                if (model.LeaveUnit == "HALFMOR" || model.LeaveUnit == "HALFEVE")
                                {
                                    dayCount = 0.5;
                                }

                                var lvModel = new LeaveHistory { UserId = model.UserId, FromDate = model.FromDate, ToDate = model.ToDate, LeaveType = model.LeaveType, LeaveUnit = model.LeaveUnit, CoveredBy = model.CoveredBy, LeaveDays = dayCount, Reason = model.Reason, RequestDate = dateTime };

                                db.LeaveHistories.Add(lvModel);
                                db.SaveChanges();

                                db.LeaveApprovals.Add(new LeaveApproval { RequestId = lvModel.Id, Date = dateTime, Status = "pending", SupervisorId = model.SupervisorId });
                                db.SaveChanges();

                                new _notifications().AddNotifications(model.SupervisorId, model.Id, "Leave Request");

                                return "Leave Request Sent";

                                //if (userLeave.RemainingCount >= dayCount)
                                //{
                                //    var lvModel = new LeaveHistory { UserId = model.UserId, FromDate = model.FromDate, ToDate = model.ToDate, LeaveType = model.LeaveType, LeaveUnit = model.LeaveUnit, CoveredBy = model.CoveredBy, LeaveDays = dayCount, Reason = model.Reason, RequestDate = dateTime };

                                //    db.LeaveHistories.Add(lvModel);
                                //    db.SaveChanges();

                                //    db.LeaveApprovals.Add(new LeaveApproval { RequestId = lvModel.Id, Date = dateTime, Status = "pending", SupervisorId = model.SupervisorId });
                                //    db.SaveChanges();

                                //    new _notifications().AddNotifications(model.SupervisorId, model.Id, "Leave Request");

                                //    return "Leave Request Sent";
                                //}
                                //else
                                //{
                                //    return "You Don't Have Enough Days Remaining";
                                //}
                            }
                            else
                            {
                                return "You Already Have Applied Leave For This Period";
                            }
                        }
                        else
                        {
                            return "Existing Record Overlaps with Current Request";
                        }

                    }
                    else
                    {
                        if (!IsInRange(model))
                        {
                            var lvHistoy = db.LeaveHistories.Where(x => x.UserId == model.UserId && x.FromDate == model.FromDate && x.ToDate == model.ToDate).FirstOrDefault();


                            if (model.LeaveUnit == "HALFMOR" || model.LeaveUnit == "HALFEVE")
                            {
                                lvHistoy = db.LeaveHistories.Where(x => x.UserId == model.UserId && x.FromDate == model.FromDate).FirstOrDefault();
                            }

                            if (lvHistoy == null)
                            {
                                if (IsSaturday(model.FromDate, model.ToDate))
                                {
                                    model.LeaveUnit = "SATURDAY";
                                    model.LeaveDays = 0.5;
                                }

                                //var dayCount = (model.ToDate - model.FromDate).TotalDays + 1;
                                double dayCount = Convert.ToDouble(model.LeaveDays);

                                if (model.LeaveUnit == "HALFMOR" || model.LeaveUnit == "HALFEVE")
                                {
                                    dayCount = 0.5;
                                }

                                var lvModel = new LeaveHistory { UserId = model.UserId, FromDate = model.FromDate, ToDate = model.ToDate, LeaveType = model.LeaveType, LeaveUnit = model.LeaveUnit, CoveredBy = model.CoveredBy, LeaveDays = dayCount, Reason = model.Reason, RequestDate = dateTime };

                                db.LeaveHistories.Add(lvModel);
                                db.SaveChanges();

                                db.LeaveApprovals.Add(new LeaveApproval { RequestId = lvModel.Id, Date = dateTime, Status = "pending", SupervisorId = model.SupervisorId });
                                db.SaveChanges();

                                new _notifications().AddNotifications(model.SupervisorId, model.Id, "Leave Request");
                                return "Leave Request Sent";
                            }
                            else
                            {
                                return "You Already Have Applied Leave For This Period";
                            }
                        }
                        else
                        {
                            return "Existing Record Overlaps with Current Request";
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool IsSaturday(DateTime from, DateTime to)
        {
            try
            {
                if (from.Date == to.Date && from.DayOfWeek == DayOfWeek.Saturday)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception er)
            {
                return false;
            }
        }

        private bool IsInRange(LeaveHistory model)
        {
            bool isExist = true;

            using (ApplicationDbContext db = new Models.ApplicationDbContext())
            {
                //var lvHistoy = db.LeaveHistories.Where(x => x.UserId == model.UserId).ToList();

                var lvs = (from lvh in db.LeaveHistories
                           join apr in db.LeaveApprovals on lvh.Id equals apr.RequestId
                           where lvh.UserId == model.UserId && apr.Status == "canceled" || apr.Status == "false"
                           select new { apr.RequestId }).Select(m => m.RequestId).ToList();

                var lvHistoy = (from lvh in db.LeaveHistories
                                join apr in db.LeaveApprovals on lvh.Id equals apr.RequestId
                                where lvh.UserId == model.UserId && !lvs.Contains(apr.RequestId)
                                select new { lvh, apr }).ToList();

                if (lvHistoy.Count > 0)
                {
                    foreach (var item in lvHistoy)
                    {
                        isExist = OverlappingPeriods(item.lvh.FromDate, item.lvh.ToDate, model.FromDate, model.ToDate);

                        if (isExist)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    isExist = false;
                }

                //if(lvHistoy.Count > 0)
                //{
                //    foreach (var item in lvHistoy)
                //    {
                //        isExist = OverlappingPeriods(item.FromDate, item.ToDate, model.FromDate, model.ToDate);

                //        if (isExist)
                //        {
                //            break;
                //        }
                //    }
                //}
                //else
                //{
                //    isExist = false;
                //}

            }

            return isExist;
        }

        public static bool OverlappingPeriods(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            if (aStart > aEnd)
                throw new ArgumentException("A start can not be after its end.");

            if (bStart > bEnd)
                throw new ArgumentException("B start can not be after its end.");

            return !((aEnd < bStart && aStart < bStart) ||
                     (bEnd < aStart && bStart < aStart));
        }

        //public bool CacelApprovedLeave(int leaveId, string Remarks = "")
        //{
        //    try
        //    {
        //        using (ApplicationDbContext db = new Models.ApplicationDbContext())
        //        {
        //            var leaveHistory = db.LeaveHistories.Where(x => x.Id == leaveId).FirstOrDefault();
        //            var leaves = db.LeaveApprovals.Where(x => x.RequestId == leaveId).OrderByDescending(x => x.Status).ToList().FirstOrDefault();
        //            var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
        //        }
        //    }
        //    catch(Exception er)
        //    {
        //        return false;
        //    }
        //}

        public bool CancelUserLeave(int leaveId, string UserId = "", string Remarks = "")
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var leaveHistory = db.LeaveHistories.Where(x => x.Id == leaveId).FirstOrDefault();
                    var leaves = db.LeaveApprovals.Where(x => x.RequestId == leaveId).OrderByDescending(x => x.Status).ToList().FirstOrDefault();
                    var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                    if (leaves.Status == "true")
                    {
                        UserId = HttpContext.Current.Session["UserId"].ToString();

                        if (leaveHistory.LeaveUnit == "SHORT")
                        {
                            db.LeaveApprovals.Add(new LeaveApproval { SupervisorId = leaveHistory.SupervisorId, Status = "canceled", Date = dateTime, Remarks = Remarks, RequestId = leaveId });

                            db.SaveChanges();

                            new _notifications().AddNotifications(leaveHistory.UserId, leaveId, "Leave Rejected");

                            MoveCanceledLeaves(leaveId, UserId, Remarks);
                        }
                        else if (leaveHistory.LeaveType == 3000 || leaveHistory.LeaveType == 3001)
                        {
                            var userLeave = db.UserLeaves.Where(x => x.UserId == leaveHistory.UserId && x.LeaveType == leaveHistory.LeaveType && x.Year == DateTime.Now.Year).FirstOrDefault();

                            var dayCount = leaveHistory.LeaveDays;

                            if (leaveHistory.LeaveUnit == "HALFMOR" || leaveHistory.LeaveUnit == "HALFEVE")
                            {
                                dayCount = 0.5;
                            }

                            leaveHistory.IsApproved = "canceled";
                            userLeave.RemainingCount += dayCount;
                            db.SaveChanges();

                            new _notifications().AddNotifications(leaveHistory.UserId, leaveId, "Leave Rejected");

                            MoveCanceledLeaves(leaveId, UserId, Remarks);
                        }
                        else
                        {
                            //var userLeave = db.UserLeaves.Where(x => x.UserId == leaveHistory.UserId && x.LeaveType == leaveHistory.LeaveType && x.Year == DateTime.Now.Year).FirstOrDefault();

                            leaveHistory.IsApproved = "canceled";
                            db.SaveChanges();

                            new _notifications().AddNotifications(leaveHistory.UserId, leaveId, "Leave Rejected");

                            MoveCanceledLeaves(leaveId, UserId, Remarks);

                            //if (userLeave != null)
                            //{
                            //    var dayCount = (leaveHistory.ToDate - leaveHistory.FromDate).TotalDays;
                            //    userLeave.RemainingCount += dayCount;

                            //}

                            //var remaining = userLeave.RemainingCount - dayCount;


                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(UserId))
                        {
                            db.LeaveApprovals.Add(new LeaveApproval { SupervisorId = leaves.SupervisorId, Status = "canceled", Date = dateTime, Remarks = Remarks, RequestId = leaveId });

                            db.SaveChanges();

                            MoveCanceledLeaves(leaveId, UserId, Remarks);
                        }
                        else
                        {
                            db.LeaveApprovals.Add(new LeaveApproval { SupervisorId = leaves.SupervisorId, Status = "canceled", Date = dateTime, Remarks = Remarks, RequestId = leaveId });

                            db.SaveChanges();

                            MoveCanceledLeaves(leaveId, leaveHistory.UserId, Remarks);
                        }


                    }

                    //if(string.IsNullOrEmpty(UserId))
                    //{
                    //    UserId = leaves.SupervisorId;
                    //}

                    //db.LeaveApprovals.Add(new LeaveApproval { SupervisorId = UserId, Status = "canceled", Date = dateTime, Remarks = Remarks, RequestId = leaveId });

                    //db.SaveChanges();

                    //MoveCanceledLeaves(leaveId, UserId);
                    //new _notifications().AddNotifications(leaveHistory.UserId, leaveId, "Leave Rejected");
                }

                return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool MoveCanceledLeaves(int RequestId, string UserId, string Remark = "")
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var cult = System.Globalization.CultureInfo.CurrentCulture;
                    var leaveHistory = db.LeaveHistories.Where(x => x.Id == RequestId).FirstOrDefault();
                    var leaves = db.LeaveApprovals.Where(x => x.RequestId == RequestId).OrderByDescending(x => x.Status).ToList().FirstOrDefault();
                    var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
                    var leaveTypes = db.LeaveTypes.ToList();

                    RejectedLeave mod = new Models.RejectedLeave();
                    mod.RequestId = leaveHistory.Id;
                    mod.Days = leaveHistory.LeaveDays.ToString();

                    if (leaveHistory.LeaveUnit == "HALFMOR" || leaveHistory.LeaveUnit == "HALFEVE" || leaveHistory.LeaveUnit == "SHORT")
                    {
                        mod.LeaveType = leaveHistory.LeaveUnit;
                    }
                    else
                    {
                        mod.LeaveType = leaveTypes.Where(x => x.Id == leaveHistory.LeaveType).FirstOrDefault().LeaveType1;
                    }

                    //mod.LeaveType = leaveTypes.Where(x => x.Id == leaveHistory.LeaveType).FirstOrDefault().LeaveType1;
                    mod.RequestedOn = leaveHistory.RequestDate.Day + " - " + cult.DateTimeFormat.GetAbbreviatedMonthName(leaveHistory.RequestDate.Month);
                    mod.Period = "From " + leaveHistory.FromDate.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(leaveHistory.FromDate.Month) + " To " + leaveHistory.ToDate.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(leaveHistory.ToDate.Month);
                    mod.RejectedBy = db.UserProfiles.Where(x => x.Id == UserId).FirstOrDefault().FirstName;
                    mod.Remarks = Remark;
                    mod.Unit = leaveHistory.LeaveUnit;
                    mod.UserId = leaveHistory.UserId;
                    mod.RejectedUserId = UserId;
                    mod.DateFrom = leaveHistory.FromDate;
                    mod.DateTo = leaveHistory.ToDate;
                    mod.Reason = leaveHistory.Reason;

                    mod.IsApproved = "canceled";

                    if (UserId == leaves.SupervisorId)
                    {
                        mod.IsApproved = "false";
                    }

                    var rej = db.RejectedLeaves.Where(x => x.RequestId == RequestId).FirstOrDefault();

                    if (rej == null)
                    {
                        db.RejectedLeaves.Add(mod);
                        db.SaveChanges();

                        db.LeaveHistories.Remove(leaveHistory);
                        db.SaveChanges();
                    }

                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string CheckUserLeaves(string UserId, int LeaveType, string Unit, double Days, DateTime date)
        {
            string msg = "0";

            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();

                    if (Unit == "SHORT")
                    {
                        var fromDate = new DateTime(date.Year, date.Month, 1);
                        var toDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

                        var leaveHistory = db.LeaveHistories.Where(x => x.UserId == UserId && x.LeaveUnit == Unit && x.FromDate >= fromDate && x.FromDate <= toDate).ToList();

                        if (leaveHistory.Count >= 2)
                        {
                            msg = "You Have Reached Your Monthly Short Leave Limit";
                        }
                    }
                    else
                    {
                        //if (LeaveType == 3000 || LeaveType == 3001)
                        //{
                        //    var userLeaves = db.UserLeaves.Where(x => x.UserId == UserId && x.Year == DateTime.Now.Year && x.LeaveType == LeaveType).FirstOrDefault();
                        //    var user = um.GetUser(UserId);

                        //    //if (userLeaves.RemainingCount < Days)
                        //    //{
                        //    //    msg = "You Don't Have Enough Leaves To Complete Your Request";
                        //    //}

                        //    //if (userLeaves.RemainingCount < Days)
                        //    //{
                        //    //    var noPayCount = Days - userLeaves.RemainingCount;
                        //    //    msg = "You Don't Have Enough Leaves To Complete Your Request." + noPayCount + " Days Will be Added as No Payleave";
                        //    //}
                        //}


                        //else
                        //{
                        //    var userLeaves = db.UserLeaves.Where(x => x.UserId == UserId && x.Year == DateTime.Now.Year && x.LeaveType == LeaveType).FirstOrDefault();
                        //    var user = um.GetUser(UserId);

                        //    if (userLeaves.RemainingCount < Days)
                        //    {
                        //        msg = "You Don't Have Enough Leaves To Complete Your Request";
                        //    }
                        //}

                    }

                }
            }
            catch
            {
                msg = "Error Occured During Operation";
            }

            return msg;
        }

        public bool ApproveLeave(LeaveHistoryViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var leaveHistory = db.LeaveHistories.Where(x => x.Id == model.Id).FirstOrDefault();
                    var leaves = db.LeaveApprovals.Where(x => x.RequestId == model.Id).OrderByDescending(x => x.Id).FirstOrDefault();

                    //leaves.Status = model.IsApproved;

                    if (!string.IsNullOrEmpty(model.Remarks))
                    {
                        leaves.Remarks = model.Remarks;
                    }
                    else
                    {
                        leaves.Remarks = "No Remarks";
                    }

                    if (model.IsApproved == "true")
                    {
                        if (leaveHistory.LeaveType == 3000 || leaveHistory.LeaveType == 3001)
                        {
                            var leaveYear = leaveHistory.FromDate.Year;
                            var userLeave = db.UserLeaves.Where(x => x.UserId == leaveHistory.UserId && x.LeaveType == leaveHistory.LeaveType && x.Year == leaveYear).FirstOrDefault();

                            //var dayCount = (leaveHistory.ToDate - leaveHistory.FromDate).TotalDays;
                            var dayCount = leaveHistory.LeaveDays;

                            if (leaveHistory.LeaveUnit == "HALFMOR" || leaveHistory.LeaveUnit == "HALFEVE")
                            {
                                dayCount = 0.5;
                            }

                            var remaining = userLeave.RemainingCount - dayCount;

                            //No Pay Leaves
                            if (userLeave.RemainingCount <= 0)
                            {
                                db.NoPayLeaves.Add(new Employee_NoPay_Leaves { Request_Id = leaveHistory.Id, No_Pay_Count = Convert.ToDouble(dayCount) });
                                db.SaveChanges();
                            }
                            else if (userLeave.RemainingCount < dayCount)
                            {
                                var noPayOnlyCount = dayCount - userLeave.RemainingCount;

                                db.NoPayLeaves.Add(new Employee_NoPay_Leaves { Request_Id = leaveHistory.Id, No_Pay_Count = Convert.ToDouble(noPayOnlyCount) });
                                db.SaveChanges();
                            }

                            userLeave.RemainingCount = remaining;
                            db.SaveChanges();

                            leaveHistory.IsApproved = model.IsApproved;
                            leaves.Status = model.IsApproved;
                            db.SaveChanges();

                            
                        }
                        else
                        {
                            leaveHistory.IsApproved = model.IsApproved;
                            leaves.Status = model.IsApproved;
                            db.SaveChanges();
                        }


                        new _notifications().AddNotifications(leaveHistory.UserId, model.Id, "Leave Approved");


                    }
                    else
                    {

                        leaveHistory.IsApproved = model.IsApproved;
                        leaves.Status = model.IsApproved;
                        db.SaveChanges();

                        MoveCanceledLeaves(model.Id, leaves.SupervisorId, model.Remarks);

                        new _notifications().AddNotifications(leaveHistory.UserId, model.Id, "Leave Rejected");
                    }

                }

                return true;
            }
            catch
            {
                return false;
            }
        }

       

        public string AdminLeaveEnforcement(LeaveHistory model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
                    var lvHistoy = db.LeaveHistories.Where(x => x.UserId == model.UserId && x.FromDate == model.FromDate && x.ToDate == model.ToDate).FirstOrDefault();

                    if (!IsInRange(model))
                    {
                        if (lvHistoy == null)
                        {
                            //if (IsSaturday(model.FromDate, model.ToDate))
                            //{
                            //    model.LeaveUnit = "SATURDAY";
                            //    model.LeaveDays = 0.5;
                            //}

                            var userLeave = db.UserLeaves.Where(x => x.UserId == model.UserId && x.LeaveType == model.LeaveType && x.Year == DateTime.Now.Year).FirstOrDefault();

                            double dayCount = Convert.ToDouble(model.LeaveDays);
                            var remaining = userLeave.RemainingCount - dayCount;
                            //if (model.LeaveUnit == "HALFMOR" || model.LeaveUnit == "HALFEVE")
                            //{
                            //    dayCount = 0.5;
                            //}

                            var lvModel = new LeaveHistory { UserId = model.UserId, FromDate = model.FromDate, ToDate = model.ToDate, LeaveType = model.LeaveType, LeaveUnit = "ADMIN ENFORCED", IsApproved = "true", CoveredBy = model.CoveredBy, LeaveDays = dayCount, Reason = model.Reason, RequestDate = dateTime };

                            db.LeaveHistories.Add(lvModel);
                            db.SaveChanges();

                            db.LeaveApprovals.Add(new LeaveApproval { RequestId = lvModel.Id, Date = dateTime, Status = "true", SupervisorId = model.SupervisorId });
                            db.SaveChanges();

                            userLeave.RemainingCount = remaining;
                            db.SaveChanges();
                            //new _notifications().AddNotifications(model.SupervisorId, model.Id, "Leave Request");

                            return "0";
                        }
                        else
                        {
                            return "-1";
                        }
                    }
                    else
                    {
                        return "-2";
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool ForwardLeave(int leaveId, string userId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var leaves = db.LeaveApprovals.Where(x => x.RequestId == leaveId && x.Status == "pending").FirstOrDefault();
                    var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                    leaves.Status = "forward";

                    db.LeaveApprovals.Add(new LeaveApproval { SupervisorId = userId, Status = "pending", Date = dateTime, Remarks = "", RequestId = leaveId });
                    db.SaveChanges();

                    new _notifications().AddNotifications(userId, leaveId, "Leave Request");

                    return true;
                }
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool DeleteLeaveType(int TypeId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var leave = db.LeaveTypes.Where(x => x.Id == TypeId).FirstOrDefault();

                    db.LeaveTypes.Remove(leave);
                    db.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<messageusers> GetAboveUsers(String UserId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var CurrentuserRole = UserManager.FindById(UserId).Roles.FirstOrDefault();

                    int RolLevel = db.RoleLevels.Where(m => m.RoleId == CurrentuserRole.RoleId).FirstOrDefault().SortOrder;
                    if (RolLevel != 0)
                    {
                        RolLevel--;
                    }
                    String NewRolLevelId = db.RoleLevels.Where(m => m.SortOrder == RolLevel).FirstOrDefault().RoleId;
                    var d = (from user in db.Users
                             join profile in db.UserProfiles
                             on user.Id equals profile.Id
                             where user.Roles.Any(r => r.RoleId == NewRolLevelId)
                             select profile).ToList();
                    List<messageusers> userlist = new List<messageusers>();
                    foreach (var item in d)
                    {
                        messageusers model = new messageusers();
                        model.UserId = item.Id;
                        model.Name = item.FirstName + " " + item.LastName;
                        userlist.Add(model);
                    }
                    return userlist;
                }

            }
            catch (Exception ex)
            {
                return new List<messageusers>();
            }
        }

        public List<messageusers> GetAboveUsers(string UserId, string Search)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var CurrentuserRole = UserManager.FindById(UserId).Roles.FirstOrDefault();

                    int RolLevel = db.RoleLevels.Where(m => m.RoleId == CurrentuserRole.RoleId).FirstOrDefault().SortOrder;
                    if (RolLevel != 0)
                    {
                        RolLevel--;
                    }
                    String NewRolLevelId = db.RoleLevels.Where(m => m.SortOrder == RolLevel).FirstOrDefault().RoleId;
                    var d = (from user in db.Users
                             join profile in db.UserProfiles
                             on user.Id equals profile.Id
                             where user.Roles.Any(r => r.RoleId == NewRolLevelId && profile.FirstName.Contains(Search))
                             select profile).ToList();
                    List<messageusers> userlist = new List<messageusers>();
                    foreach (var item in d)
                    {
                        messageusers model = new messageusers();
                        model.UserId = item.Id;
                        model.Name = item.FirstName + " " + item.LastName;
                        userlist.Add(model);
                    }
                    return userlist;
                }

            }
            catch (Exception ex)
            {
                return new List<messageusers>();
            }
        }

        public List<LeaveHistory> GetUserRequestLeave(String UserId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var usesrleave = (from history in db.LeaveHistories
                                      join levetype in db.LeaveTypes
                                      on history.LeaveType equals levetype.Id
                                      where history.UserId == UserId
                                      select new { history, levetype }).ToList();// db.LeaveHistories.Where(m => m.UserId == UserId).ToList();
                    List<LeaveHistory> hl = new List<LeaveHistory>();
                    foreach (var item in usesrleave)
                    {
                        LeaveHistory model = new LeaveHistory();
                        model.Id = item.history.Id;
                        model.LeaveTypeStr = item.levetype.LeaveType1;
                        model.FromDateStr = item.history.FromDate.ToString("dd/MM/yyy");
                        model.ToDateStr = item.history.ToDate.ToString("dd/MM/yyy");
                        model.Reason = item.history.Reason;
                        model.Remarks = item.history.Remarks;
                        model.IsApproved = item.history.IsApproved;
                        model.ReqDateStr = item.history.RequestDate.ToString("dd/MM/yyy");
                        hl.Add(model);
                    }
                    return hl;
                }
            }
            catch (Exception ex)
            {
                return new List<LeaveHistory>();
            }
        }

        public List<LeaveHistory> GetUserLeaveRequest(String UserId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var usesrleave = (from history in db.LeaveHistories
                                      join levetype in db.LeaveTypes
                                      on history.LeaveType equals levetype.Id
                                      join user in db.UserProfiles
                                      on history.UserId equals user.Id
                                      join empInfo in db.EmployeementInfos
                                      on history.UserId equals empInfo.Id
                                      join approvals in db.LeaveApprovals.Where(x => x.SupervisorId == UserId)
                                      on history.Id equals approvals.RequestId
                                      //where approvals.SupervisorId == UserId 
                                      select new { history, levetype, user, empInfo, approvals }).ToList();// db.LeaveHistories.Where(m => m.UserId == UserId).ToList && history.IsApproved == "pending"();
                    List<LeaveHistory> hl = new List<LeaveHistory>();
                    foreach (var item in usesrleave)
                    {
                        var approvals = db.LeaveApprovals.Where(x => x.RequestId == item.history.Id).OrderByDescending(x => x.Id).ToList().FirstOrDefault();

                        if (approvals.Status == "pending")
                        {
                            LeaveHistory model = new LeaveHistory();
                            model.Id = item.history.Id;
                            model.LeaveTypeStr = item.levetype.LeaveType1;
                            model.FromDateStr = item.history.FromDate.ToString("dd/MM/yyy");
                            model.ToDateStr = item.history.ToDate.ToString("dd/MM/yyy");
                            model.Reason = item.history.Reason;
                            model.Remarks = item.history.Remarks;
                            model.IsApproved = item.approvals.Status;
                            model.ReqDateStr = item.history.RequestDate.ToString("dd/MM/yyy");
                            model.Requester = item.user.FirstName + " " + item.user.LastName;
                            model.LeaveDays = item.history.LeaveDays;

                            if (item.history.LeaveUnit == "HALFMOR")
                            {
                                model.LeaveUnit = "Half-Morning";
                            }
                            else if (item.history.LeaveUnit == "HALFEVE")
                            {
                                model.LeaveUnit = "Half-Evening";
                            }
                            else
                            {
                                model.LeaveUnit = item.history.LeaveUnit;
                            }

                            //
                            model.CoveredBy = item.history.CoveredBy;
                            model.EMPNo = item.user.EmployeeId;
                            model.Project = item.empInfo.Project.ProjectName;
                            model.Section = item.empInfo.Section;
                            model.Location = item.empInfo.PresentReportingLocation;
                            model.FromDate = item.history.FromDate;
                            model.ToDate = item.history.ToDate;
                            model.DivisionCode = item.empInfo.DivisionCode;
                            hl.Add(model);
                        }

                    }

                    var shortLeave = (from history in db.LeaveHistories
                                      join approvals in db.LeaveApprovals
                                      on history.Id equals approvals.RequestId
                                      join user in db.UserProfiles
                                      on history.UserId equals user.Id
                                      join empInfo in db.EmployeementInfos
                                      on history.UserId equals empInfo.Id
                                      where history.LeaveUnit == "SHORT" && approvals.SupervisorId == UserId
                                      select new { history, approvals, user, empInfo }).ToList();

                    foreach (var item in shortLeave)
                    {
                        if (item.approvals.Status == "pending")
                        {
                            LeaveHistory model = new LeaveHistory();
                            model.Id = item.history.Id;
                            model.LeaveTypeStr = "Short Leave";
                            model.FromDateStr = item.history.FromDate.ToString("dd/MM/yyy");
                            model.ToDateStr = item.history.ToDate.ToString("dd/MM/yyy");
                            model.Reason = item.history.Reason;
                            model.Remarks = item.history.Remarks;
                            model.IsApproved = item.approvals.Status;
                            model.ReqDateStr = item.history.RequestDate.ToString("dd/MM/yyy");
                            model.Requester = item.user.FirstName + " " + item.user.LastName;
                            model.LeaveDays = item.history.LeaveDays;
                            model.LeaveUnit = item.history.LeaveUnit;
                            model.CoveredBy = item.history.CoveredBy;
                            model.EMPNo = item.user.EmployeeId;
                            model.Project = item.empInfo.Project.ProjectName;
                            model.Section = item.empInfo.Section;
                            model.Location = item.empInfo.PresentReportingLocation;
                            model.FromDate = item.history.FromDate;
                            model.ToDate = item.history.ToDate;
                            model.DivisionCode = item.empInfo.DivisionCode;
                            hl.Add(model);
                        }
                    }

                    return hl;
                }
            }
            catch (Exception ex)
            {
                return new List<LeaveHistory>();
            }
        }

        public void NewUserLeaves(string UserId, string RoleId)
        {
            try
            {
                var leaves = GetUserLevelLeaves(RoleId);
                List<UserLeaves> lst = new List<UserLeaves>();

                foreach (var item in leaves)
                {
                    lst.Add(new UserLeaves { UserId = UserId, LeaveType = item.LeaveType, AllocatedCount = item.LeaveCount, RemainingCount = item.LeaveCount, Year = DateTime.Now.Year });
                }

                AddUserLeave(lst);
            }
            catch (Exception er)
            {

            }
        }

        public List<LeaveReportViewModel> GetLeaveReport(int DivisionId, int Year)
        {
            try
            {
                List<LeaveReportViewModel> lst = new List<Models.LeaveReportViewModel>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();
                    _projects prj = new _projects();

                    if (DivisionId != 0)
                    {
                        var users = um.GetUsers(DivisionId.ToString());
                        var empInfo = um.GetUserEmployement();
                        var userLeaves = GetUserLeaves(DivisionId, Year);
                        var divisions = prj.GetProject(DivisionId);

                        //int index = 0;
                        foreach (var item in users)
                        {
                            //Error_Log.er_log lg = new Error_Log.er_log();
                            //lg.WriteLog(index + ". " + item.Id + " " + item.FirstName);
                            //index++;
                            var userEmp = empInfo.Where(x => x.Id == item.Id).FirstOrDefault();

                            if (empInfo.Where(x => x.Id == item.Id).FirstOrDefault() != null)
                            {
                                var supervisor = empInfo.Where(x => x.Id == item.Id).FirstOrDefault().SupervisorId;
                                var leaves = userLeaves.Where(x => x.UserId == item.Id).ToList();
                                var des = empInfo.Where(x => x.Id == item.Id).FirstOrDefault().Designation;
                                var loc = empInfo.Where(x => x.Id == item.Id).FirstOrDefault().PresentReportingLocation;
                                var sup = "";
                                var division = "Not Specified";

                                if (!string.IsNullOrEmpty(supervisor))
                                {
                                    sup = db.UserProfiles.Where(x => x.Id == supervisor).FirstOrDefault().FirstName;
                                }
                                else
                                {
                                    supervisor = "Not Specified";
                                }

                                if (des == null)
                                {
                                    des = "Not Specified";
                                }

                                if (loc == null)
                                {
                                    loc = "Not Specified";
                                }

                                if (!string.IsNullOrEmpty(userEmp.Section))
                                {
                                    division = divisions.ProjectName;
                                }

                                lst.Add(new LeaveReportViewModel { EmpNo = item.EmployeeId, Username = item.FirstName, DivisionId = DivisionId, Designation = des, Location = loc, Supervisor = sup, Leaves = leaves, DivisionCode = userEmp.DivisionCode, DivisionName = division });
                            }

                        }
                    }
                    else
                    {
                        var users = um.GetUsers();
                        var empInfo = um.GetUserEmployement();
                        var userLeaves = GetUserLeaves(0, Year);
                        var divisions = prj.GetProject(DivisionId);

                        foreach (var item in users)
                        {
                            var userEmp = empInfo.Where(x => x.Id == item.Id).FirstOrDefault();

                            if (empInfo.Where(x => x.Id == item.Id).FirstOrDefault() != null)
                            {
                                var supervisor = empInfo.Where(x => x.Id == item.Id).FirstOrDefault().SupervisorId;
                                var leaves = userLeaves.Where(x => x.UserId == item.Id).ToList();
                                var division = Convert.ToInt32(empInfo.Where(x => x.Id == item.Id).FirstOrDefault().Division);
                                var des = empInfo.Where(x => x.Id == item.Id).FirstOrDefault().Designation;
                                var loc = empInfo.Where(x => x.Id == item.Id).FirstOrDefault().PresentReportingLocation;
                                var sup = "";
                                var divisionName = "Not Specified";

                                if (!string.IsNullOrEmpty(supervisor))
                                {
                                    sup = users.Where(x => x.Id == supervisor).FirstOrDefault().FirstName;
                                }
                                else
                                {
                                    supervisor = "Not Specified";
                                }

                                if (des == null)
                                {
                                    des = "Not Specified";
                                }

                                if (loc == null)
                                {
                                    loc = "Not Specified";
                                }

                                if (!string.IsNullOrEmpty(userEmp.Section))
                                {
                                    divisionName = divisions.ProjectName;
                                }

                                lst.Add(new LeaveReportViewModel { EmpNo = item.EmployeeId, Username = item.FirstName, DivisionId = division, Designation = des, Location = loc, Supervisor = sup, Leaves = leaves, DivisionCode = userEmp.DivisionCode, DivisionName = divisionName });
                            }

                        }
                    }


                }

                return lst;
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<LeaveReportDetailsViewModel> GetLeaveDetails(int DivisionId, DateTime From, DateTime To)
        {
            try
            {
                List<LeaveReportDetailsViewModel> lst = new List<Models.LeaveReportDetailsViewModel>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();
                    _projects pro = new App_Codes._projects();

                    if (DivisionId == 0)
                    {
                        var users = um.GetUsers();
                        var empInfo = um.GetUserEmployement();
                        var proj = pro.GetAllProjects();

                        foreach (var item in users)
                        {
                            LeaveReportDetailsViewModel model = new Models.LeaveReportDetailsViewModel();

                            //var userLeaves = GetLeaveHistory(item.Id, "0", Year);
                            var userLeaves = GetLeaveHistory(From, To, item.Id);
                            var emp = empInfo.Where(x => x.Id == item.Id).FirstOrDefault();

                            if (userLeaves.Count > 0)
                            {
                                model.EmpNo = item.EmployeeId;
                                model.Username = item.FirstName;
                                model.Designation = emp.Designation ?? "Not Defined";
                                model.DivisionId = Convert.ToInt32(emp.Division);
                                model.DivisionCode = emp.DivisionCode ?? "Not Defined";
                                model.DivisionName = proj.Where(x => x.Id == emp.Division).FirstOrDefault().ProjectName;
                                model.Location = emp.PresentReportingLocation ?? "Not Defined";

                                //if(!string.IsNullOrEmpty(emp.Designation))
                                //{
                                //    model.Designation = emp.Designation;
                                //}
                                //else
                                //{
                                //    model.Designation = "Not Specified";
                                //}

                                model.Leaves = userLeaves;

                                lst.Add(model);
                            }

                        }

                    }
                    else
                    {
                        var users = um.GetUsers(DivisionId.ToString());
                        var empInfo = um.GetUserEmployement();
                        var proj = pro.GetProject(DivisionId);

                        foreach (var item in users)
                        {
                            LeaveReportDetailsViewModel model = new Models.LeaveReportDetailsViewModel();

                            //if(item.EmployeeId == "3526")
                            //{model.DivisionName = proj.Where(x => x.Id == emp.Division).FirstOrDefault().ProjectName;

                            //}

                            var userLeaves = GetLeaveHistory(From, To, item.Id);
                            var emp = empInfo.Where(x => x.Id == item.Id).FirstOrDefault();

                            if (userLeaves.Count > 0)
                            {
                                model.EmpNo = item.EmployeeId;
                                model.Username = item.FirstName;
                                model.Designation = emp.Designation ?? "Not Defined";
                                model.DivisionId = Convert.ToInt32(emp.Division);
                                model.DivisionCode = emp.DivisionCode ?? "Not Defined";
                                model.DivisionName = proj.ProjectName;
                                model.Location = emp.PresentReportingLocation ?? "Not Defined";

                                //if (!string.IsNullOrEmpty(emp.Designation))
                                //{
                                //    model.Designation = emp.Designation;
                                //}
                                //else
                                //{
                                //    model.Designation = "Not Specified";
                                //}

                                model.Leaves = userLeaves;

                                lst.Add(model);
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
    }
}