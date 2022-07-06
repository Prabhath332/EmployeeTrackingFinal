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
using Microsoft.Owin.Security.DataProtection;

namespace EmployeeTracking.App_Codes
{
    public class _usermanager
    {
        public UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        public RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

        private ApplicationSignInManager _signInManager;

        //public List<UserProfile> GetUsersBySupervisor(string userId)
        //{
        //    try
        //    {
        //        List<UserProfile> userLst = new List<UserProfile>();

        //        using (ApplicationDbContext db = new Models.ApplicationDbContext())
        //        {
        //            var users = (from user in db.UserProfiles
        //                         join emp in db.EmployeementInfos
        //                         on user.Id equals emp.Id
        //                         where emp.SupervisorId == userId select new { user }).ToList();

        //            foreach(var item in users)
        //            {

        //            }

        //            return userLst;
        //        }
        //    }
        //    catch(Exception er)
        //    {
        //        return null;
        //    }
        //    throw new NotImplementedException();
        //}

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public MobileUserModel MobileLogin(string Username, string Password)
        {
            try
            {
                MobileUserModel obj = new Models.MobileUserModel();
                var result = SignInManager.PasswordSignIn(Username, Password, true, shouldLockout: false);
                
                if (result == SignInStatus.Failure)
                {
                    return null;
                }
                else if (result == SignInStatus.Success)
                {
                    using (ApplicationDbContext db = new Models.ApplicationDbContext())
                    {
                        var userlevel = UserManager.FindByName(Username);
                        var roleId = userlevel.Roles.FirstOrDefault().RoleId;
                        obj.FullName = FullUserName(Username);                        
                        obj.Modules = GetModuleUsers(roleId);
                        obj.UserId= userlevel.Id;
                        var prof = db.UserProfiles.Where(m => m.Id == userlevel.Id).FirstOrDefault();
                        var emp = db.EmployeementInfos.Where(x => x.Id == obj.UserId).FirstOrDefault();
                        var proj = db.Projects.Where(x => x.Id == emp.Division).FirstOrDefault();
                        obj.DivisionId = proj.Id;
                        obj.ComapnyId = Convert.ToInt32(proj.CompanyId);

                        if (prof.Image != null) {
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            String p = prof.Image.Replace("/", "\\");

                            try
                            {
                                obj.UserImage = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                            }
                            catch
                            {
                                obj.UserImage = "Content/ProfileImage/def_profile.png";
                            }
                            
                        }
                        
                    }

                }

                return obj;
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<IdentityRole> GetRoles()
        {
            List<IdentityRole> roles = RoleManager.Roles.ToList();

            return roles;
        }

        public List<string> GetUserRoles(string UserId)
        {
            List<string> roles = UserManager.GetRoles(UserId).ToList();

            return roles;
        }

        public string FullUserName(string Username)
        {
            var userId = UserManager.FindByName(Username).Id;

            using (ApplicationDbContext db = new Models.ApplicationDbContext())
            {
                var user = db.UserProfiles.Where(x => x.Id == userId).FirstOrDefault();
                return user.FirstName + " " + user.LastName;
            }
        }

        public List<string> UserParm(string Username)
        {
            var userId = UserManager.FindByName(Username).Id;
            int DivisionId = 0;

            List<string> lst = new List<string>();

            using (ApplicationDbContext db = new Models.ApplicationDbContext())
            {
                //var user = db.UserProfiles.Where(x => x.Id == userId).FirstOrDefault();

                var  user = (from us in db.Users
                            join emp in db.EmployeementInfos on us.Id equals emp.Id
                            where us.UserName == Username
                            select new { us, emp}).FirstOrDefault();

                if(db.EmployeementInfos.Where(x => x.Id == userId).Count() > 0)
                {
                    var division = db.EmployeementInfos.Where(x => x.Id == userId).FirstOrDefault();
                    int divId = Convert.ToInt32(division.Division);
                    var company = db.Projects.Where(x => x.Id == divId).FirstOrDefault();

                    lst.Add(userId);
                    lst.Add(division.Division.ToString());
                    lst.Add(company.CompanyId.ToString());
                    lst.Add(user.emp.PresentReportingLocation);
                }
                else
                {
                    lst.Add(userId);
                }
               
                return lst;
            }
        }

        public List<UserProfile> GetUsers(string DivisionId)
        {
            var users = UserManager.Users.ToList();

            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var cId = Convert.ToInt32(DivisionId);
                    //var compayId = db.Projects.Where(x => x.Id == cId).FirstOrDefault().CompanyId;
                    var divisions = db.Projects.Where(x => x.Id == cId).ToList();

                    foreach(var item in divisions)
                    {
                        var diviUSers = db.EmployeementInfos.Where(x => x.Division == item.Id).ToList();

                        foreach(var sub in diviUSers)
                        {
                            var user = db.UserProfiles.Where(x => x.Id == sub.Id).FirstOrDefault();
                            lst.Add(user);
                        }
                    }

                    return lst;
                }
            }
            catch(Exception er)
            {
                return null;
            }
            
        }

        public List<UserProfile> GetUsers(string DivisionId, string UserId)
        {
            try
            {
                var users = GetUsersBelowList(UserId, Convert.ToInt32(DivisionId));
                return users;                
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public List<UserProfile> GetCompanyUsers(int CompanyId)
        {
            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var divisions = db.Projects.Where(x => x.CompanyId == CompanyId).ToList();
                    //var divisionIds = divisions.ConvertAll(x => x.Id).ToList();
                    //var divisionUsers = db.EmployeementInfos.Where(x => divisionIds.Contains(x.Division))

                    foreach (var item in divisions)
                    {
                        var diviUSers = db.EmployeementInfos.Where(x => x.Division == item.Id).ToList();

                        foreach (var sub in diviUSers)
                        {
                            var user = db.UserProfiles.Where(x => x.Id == sub.Id).FirstOrDefault();
                            lst.Add(user);
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

        public List<UserLocationsViewModel> GetUsersForLocator(string key)
        {
            try
            {
                if(key == "77b1e470de2545d7bbf188072784f1de")
                {
                    var users = UserManager.Users.ToList();
                    List<UserLocationsViewModel> lst = new List<UserLocationsViewModel>();

                    foreach (var item in users)
                    {
                        lst.Add(new UserLocationsViewModel { UserId = item.Id, MobileNo = item.MobileNumber, MobileAccount = item.MobileAccount});
                    }

                    return lst;
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public List<UserProfile> GetUsers()
        {
            var users = UserManager.Users.ToList();

            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    //var diviUSers = db.EmployeementInfos.ToList();

                    foreach (var sub in users)
                    {
                        var user = db.UserProfiles.Where(x => x.Id == sub.Id).FirstOrDefault();
                        lst.Add(user);
                    }

                    return lst;
                }
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public List<UserProfile> GetAllUsers(int PageIndex, int PageSize)
        {
            int skipCount = (PageIndex - 1) * PageSize;

            var users = UserManager.Users.OrderBy(x => x.Id).Skip(skipCount).Take(PageSize).ToList();
            //var sorted = users.Select((users, index) = > )
            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    //var diviUSers = db.EmployeementInfos.ToList();

                    foreach (var sub in users)
                    {
                        var user = db.UserProfiles.Where(x => x.Id == sub.Id).FirstOrDefault();
                        lst.Add(user);
                    }

                    return lst;
                }
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public List<EmployeementInfo> GetUserEmployement()
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.EmployeementInfos.ToList();
                }
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public EmployeementInfo GetUserEmployement(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();
                }
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public ApplicationUser ApplicationUserProfile(string EmpId)
        {
            try
            {
                return UserManager.FindByName(EmpId);
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public UserProfile GetUserProfile(string UserId)
        {
            var users = UserManager.Users.ToList();

            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.UserProfiles.Where(x => x.Id == UserId).FirstOrDefault();
                }
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public string GetUserImage(string Username)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var user = UserManager.FindByName(Username);
                    var img = db.UserProfiles.Where(x => x.Id == user.Id).FirstOrDefault().Image;
                    
                    if (!File.Exists(HttpContext.Current.Server.MapPath("~/" + img)))
                    {
                        img = "Content/ProfileImages/def_profile.png";
                    }

                    return img;
                }
            }
            catch (Exception er)
            {
                return null;
            }

        }

        public static bool UserCanEdit(string Username)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();
                    var user = um.UserManager.FindByName(Username);
                    var userRole = user.Roles.FirstOrDefault().RoleId;
                    return db.RoleLevels.Where(x => x.RoleId == userRole).FirstOrDefault().CanEdit;
                }
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public static bool UserCanViewAll(string Username)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();
                    var user = um.UserManager.FindByName(Username);
                    var userRole = user.Roles.FirstOrDefault().RoleId;
                    return db.RoleLevels.Where(x => x.RoleId == userRole).FirstOrDefault().CanViewAll;
                }
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public List<SelectListItem> UserRolesSelect()
        {
            var roles = RoleManager.Roles;
            List<SelectListItem> lst = new List<SelectListItem>();

            try
            {
                foreach(var item in roles)
                {
                    lst.Add(new SelectListItem { Text = item.Name, Value = item.Name});
                }
            }
            catch (Exception er)
            {
                return null;
            }

            return lst;
        }

        public UserProfileViewModel GetUser(string UserId)
        {
            try
            {
                UserProfileViewModel uvm = new Models.UserProfileViewModel();
                ApplicationDbContext db = new Models.ApplicationDbContext();

                List<SelectListItem> divisionCodes = new List<SelectListItem>();
                divisionCodes.Add(new SelectListItem { Text = "STL", Value = "STL"});
                divisionCodes.Add(new SelectListItem { Text = "SCON", Value = "SCON" });
                divisionCodes.Add(new SelectListItem { Text = "CASUAL", Value = "CASUAL" });


                var dateNow = DateTime.Now;
                int userAge = 0;

                if(db.UserProfiles.Where(x => x.Id == UserId).FirstOrDefault().DateOfBirth != null)
                {
                    var userBirthdate = db.UserProfiles.Where(x => x.Id == UserId).FirstOrDefault().DateOfBirth;
                    userAge = dateNow.Year - userBirthdate.Value.Year;
                    //userAge = Convert.ToInt32(Math.Round(realAge.TotalDays))/365;
                }
                                
                uvm.ApplicationUser = UserManager.FindById(UserId);
                uvm.UserProfiles = db.UserProfiles.Where(x => x.Id == UserId).FirstOrDefault();
                uvm.UserProfiles.Age = userAge;

                uvm.UserProfiles.UserRole = RoleManager.FindById(uvm.ApplicationUser.Roles.FirstOrDefault().RoleId).Name;
                uvm.EmployeePromotions = db.EmployeePromotions.Where(x => x.UserId == UserId).ToList();
                uvm.EducationalInfos = db.EducationalInfos.Where(x => x.Id == UserId).FirstOrDefault();
                uvm.EmployeementInfos = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();
                uvm.EmployeeAwards = db.EmployeeAwards.Where(x => x.UserId == UserId).ToList();
                uvm.UserCompanies = db.UserCompanies.Where(x => x.UserId == UserId).ToList();

                EmployeeTracking.App_Codes._projects pr = new EmployeeTracking.App_Codes._projects();
                uvm.UserDivisions = pr.ProjectsList();
                uvm.DivisionCodes = divisionCodes;

                return uvm;
            }
            catch (Exception er)
            {
                return null;
            }

        }        

        public Company GetUserCompany(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var str = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault().Project.Company;
                    return str;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public string GetUserIdByName(string Name)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var str = db.UserProfiles.Where(x => x.FirstName.Trim() == Name).FirstOrDefault();
                    return str.Id;
                }
            }
            catch(Exception er)
            {
                return null;
            }
        }

        public UserProfile GetUserByEmp(string Emp)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    //var emp = db.UserProfiles.Where(x => x.EmployeeId == Emp).Count();

                    //if()
                    return db.UserProfiles.Where(x => x.EmployeeId == Emp).FirstOrDefault();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<SelectListItem> GetSupervisorList(string UserId)
        {
            try
            {
                List<SelectListItem> lst = new List<SelectListItem>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault().RoleId;
                    var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                    var roleIndex = roleLevels.Where(x => x.RoleId == userRole).FirstOrDefault().SortOrder;

                    if(roleIndex != 1)
                    {
                        roleLevels = roleLevels.Where(x => x.SortOrder < roleIndex).ToList();

                        foreach (var item in roleLevels)
                        {
                            var roleUsers = db.Roles.Where(x => x.Id == item.RoleId).FirstOrDefault().Users;

                            foreach (var user in roleUsers)
                            {
                                var us = db.UserProfiles.Where(x => x.Id == user.UserId).FirstOrDefault();
                                string fullName = us.FirstName + " " + us.LastName;
                                lst.Add(new SelectListItem { Text = fullName, Value = us.Id });
                            }

                        }
                    }
                    
                }

                return lst;
            }
            catch(Exception er)
            {
                return null;
            }
        }


        //public List<SelectListItem> GetSupervisorList(string UserId)
        //{
        //    try
        //    {
        //        List<SelectListItem> lst = new List<SelectListItem>();

        //        using (ApplicationDbContext db = new Models.ApplicationDbContext())
        //        {
        //            var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault().RoleId;
        //            var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
        //            var roleIndex = roleLevels.Where(x => x.RoleId == userRole).FirstOrDefault().SortOrder;

        //            if (roleIndex != 1)
        //            {
        //                roleLevels = roleLevels.Where(x => x.SortOrder < roleIndex).ToList();

        //                foreach (var item in roleLevels)
        //                {
        //                    var roleUsers = db.Roles.Where(x => x.Id == item.RoleId).FirstOrDefault().Users;

        //                    foreach (var user in roleUsers)
        //                    {
        //                        var us = db.UserProfiles.Where(x => x.Id == user.UserId).FirstOrDefault();
        //                        string fullName = us.FirstName + " " + us.LastName;
        //                        lst.Add(new SelectListItem { Text = fullName, Value = us.Id });
        //                    }

        //                }
        //            }

        //        }

        //        return lst;
        //    }
        //    catch (Exception er)
        //    {
        //        return null;
        //    }
        //}

        public List<UserProfile> GetUsersBelowList(string UserId)
        {
            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault();
                    var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                    var roleIndex = roleLevels.Where(x => x.RoleId == userRole.RoleId).FirstOrDefault().SortOrder;
                    roleLevels = roleLevels.Where(x => x.SortOrder >= roleIndex).ToList();

                    if (roleIndex == 0)
                    {
                        return db.UserProfiles.ToList();
                    }
                    else
                    {
                        foreach (var item in roleLevels)
                        {
                            var roleUsers = db.Roles.Where(x => x.Id == item.RoleId).FirstOrDefault().Users;

                            foreach (var user in roleUsers)
                            {
                                var us = db.UserProfiles.Where(x => x.Id == user.UserId).FirstOrDefault();
                                var role = RoleManager.FindById(userRole.RoleId).Name;

                                string fullName = us.FirstName + " " + us.LastName;
                                lst.Add(new UserProfile { Id = us.Id, FirstName = us.FirstName, LastName = us.LastName, EmployeeId = us.EmployeeId, Image = us.Image, Age = us.Age, Gender = us.Gender, MaritalStatus = us.MaritalStatus, UserRole = role });
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

        public EmployeeViewModel GetEmployeesBelow(string Search, string UserId)
        {
            try
            {
                EmployeeViewModel lst = new EmployeeViewModel();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();

                    var user = db.UserProfiles.Where(x => x.EmployeeId == Search).FirstOrDefault();
                    var empRole = UserManager.FindById(user.Id).Roles.FirstOrDefault();                    
                    var emRoleIndex = roleLevels.Where(x => x.RoleId == empRole.RoleId).FirstOrDefault().SortOrder;

                    var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault();
                    var roleIndex = roleLevels.Where(x => x.RoleId == userRole.RoleId).FirstOrDefault().SortOrder;
                    var roleName = RoleManager.FindById(userRole.RoleId);

                    if(roleName.Name == "SuperAdmin" || roleName.Name == "Super Level")
                    {
                        var emp = (from em in db.EmployeementInfos
                                   join div in db.Projects on em.Division equals div.Id
                                   join usr in db.UserProfiles on em.Id equals usr.Id
                                   join sup in db.UserProfiles on em.SupervisorId equals sup.Id
                                   where em.Id == user.Id
                                   select new { em, div, usr, sup }).FirstOrDefault();

                        string fullName = user.FirstName + " " + user.LastName;
                        string division = emp.div.ProjectName;
                        string location = "Not Specified";
                        string supervisor = "Not Specified";
                        string designation = "Not Specified";
                        int currentDivId = emp.div.Id;

                        if (!string.IsNullOrEmpty(emp.em.PresentReportingLocation))
                        {
                            location = emp.em.PresentReportingLocation;
                        }

                        if (!string.IsNullOrEmpty(emp.usr.FirstName))
                        {
                            supervisor = emp.sup.FirstName;
                        }

                        if (!string.IsNullOrEmpty(emp.em.Designation))
                        {
                            designation = emp.em.Designation;
                        }

                        lst = new EmployeeViewModel { Id = user.Id, FirstName = fullName, Designation = designation, EmployeeId = user.EmployeeId, Division = division, Location = location, Supervisor = supervisor, SupervisorId = emp.sup.Id, CurrentDivisionId = currentDivId };
                    }

                    //if(emRoleIndex > roleIndex)
                    //{
                    //    var emp = (from em in db.EmployeementInfos
                    //               join div in db.Projects on em.Division equals div.Id
                    //               join usr in db.UserProfiles on em.Id equals usr.Id
                    //               join sup in db.UserProfiles on em.SupervisorId equals sup.Id
                    //               where em.Id == user.Id
                    //               select new { em, div, usr, sup }).FirstOrDefault();

                    //    string fullName = user.FirstName + " " + user.LastName;
                    //    string division = emp.div.ProjectName;
                    //    string location = "Not Specified";
                    //    string supervisor = "Not Specified";
                    //    string designation = "Not Specified";
                    //    int currentDivId = emp.div.Id;

                    //    if (!string.IsNullOrEmpty(emp.em.PresentReportingLocation))
                    //    {
                    //        location = emp.em.PresentReportingLocation;
                    //    }

                    //    if (!string.IsNullOrEmpty(emp.usr.FirstName))
                    //    {
                    //        supervisor = emp.sup.FirstName;
                    //    }

                    //    if(!string.IsNullOrEmpty(emp.em.Designation))
                    //    {
                    //        designation = emp.em.Designation;
                    //    }

                    //    lst = new EmployeeViewModel { Id = user.Id, FirstName = fullName, Designation = designation, EmployeeId = user.EmployeeId, Division = division, Location = location, Supervisor = supervisor, SupervisorId = emp.sup.Id, CurrentDivisionId = currentDivId };
                    //}

                    //var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault();
                    //var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                    //var roleIndex = roleLevels.Where(x => x.RoleId == userRole.RoleId).FirstOrDefault().SortOrder;
                    //roleLevels = roleLevels.Where(x => x.SortOrder > roleIndex).ToList();

                    //if (roleIndex == 0)
                    //{
                    //    //return db.UserProfiles.ToList();
                    //}
                    //else
                    //{
                    //    foreach (var item in roleLevels)
                    //    {
                    //        var roleUsers = db.Roles.Where(x => x.Id == item.RoleId).FirstOrDefault().Users;
                    //        var us = db.UserProfiles.Where(x => x.EmployeeId == Search).FirstOrDefault();

                    //        foreach (var user in roleUsers.Where(x => x.UserId == us.Id))
                    //        {                                
                    //            var role = RoleManager.FindById(userRole.RoleId).Name;
                    //            var emp = (from em in db.EmployeementInfos
                    //                       join div in db.Projects on em.Division equals div.Id
                    //                       join usr in db.UserProfiles on em.Id equals usr.Id
                    //                       where em.Id == user.UserId select new { em, div, usr }).FirstOrDefault();

                    //            string fullName = us.FirstName + " " + us.LastName;
                    //            string division = emp.div.ProjectName;
                    //            string location = "Not Specified";
                    //            string supervisor = "Not Specified";

                    //            if (!string.IsNullOrEmpty(emp.em.PresentReportingLocation))
                    //            {
                    //                location = emp.em.PresentReportingLocation;
                    //            }

                    //            if (!string.IsNullOrEmpty(emp.usr.FirstName))
                    //            {
                    //                supervisor = emp.usr.FirstName;
                    //            }

                    //            lst.Add(new EmployeeViewModel { Id = us.Id, FirstName = us.FirstName, EmployeeId = us.EmployeeId, Division = division, Location = location, Supervisor = supervisor });
                    //        }

                    //    }
                    //}

                }

                return lst;
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserProfile> GetUsersBelowList(string UserId, int DivisionId)
        {
            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var empIndo = db.EmployeementInfos.Where(x => x.Division == DivisionId).ToList();
                    var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault();
                    var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                    var roleIndex = roleLevels.Where(x => x.RoleId == userRole.RoleId).FirstOrDefault().SortOrder;
                    roleLevels = roleLevels.Where(x => x.SortOrder >= roleIndex).ToList();

                    if (roleIndex == 0)
                    {
                        return db.UserProfiles.ToList();
                    }
                    else
                    {
                        foreach (var item in roleLevels)
                        {
                            var roleUsers = db.Roles.Where(x => x.Id == item.RoleId).FirstOrDefault().Users;

                            foreach(var div in empIndo)
                            {
                                foreach (var user in roleUsers.Where(x => x.UserId == div.Id))
                                {
                                    var us = db.UserProfiles.Where(x => x.Id == user.UserId).FirstOrDefault();
                                    var role = RoleManager.FindById(userRole.RoleId).Name;

                                    string fullName = us.FirstName + " " + us.LastName;
                                    lst.Add(new UserProfile { Id = us.Id, FirstName = us.FirstName, LastName = us.LastName, EmployeeId = us.EmployeeId, Image = us.Image, Age = us.Age, Gender = us.Gender, MaritalStatus = us.MaritalStatus, UserRole = role, NICNo = us.NICNo, DateOfBirth = us.DateOfBirth });
                                }
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

        public List<UserRoleViewModels> GetRolesBelow(string UserId)
        {
            try
            {
                List<UserRoleViewModels> lst = new List<UserRoleViewModels>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var userRole = UserManager.FindById(UserId).Roles.FirstOrDefault();
                    var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                    var roleIndex = roleLevels.Where(x => x.RoleId == userRole.RoleId).FirstOrDefault().SortOrder;
                    roleLevels = roleLevels.Where(x => x.SortOrder >= roleIndex).ToList();

                    foreach (var item in roleLevels)
                    {
                        var role = RoleManager.FindById(item.RoleId);
                        lst.Add(new Models.UserRoleViewModels { Id = role.Id, RoleName = role.Name, SortOrder = item.SortOrder});
                    }
                    

                }

                return lst;
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserProfile> GetRoleUsers(string RoleId)
        {
            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var userRole = RoleManager.FindById(RoleId).Users;
                    var dateNow = DateTime.Now;

                    foreach (var item in userRole)
                    {
                        var role = db.UserProfiles.Where(x => x.Id == item.UserId).FirstOrDefault();

                        if(role.DateOfBirth.HasValue)
                        {
                            role.Age = dateNow.Year - role.DateOfBirth.Value.Year;
                        }
                        
                        lst.Add(role);
                    }

                }

                return lst;
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserProfile> SearchUsers(string RoleId, string Search, string SearchType = "")
        {
            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    if(SearchType == "")
                    {
                        var userRole = RoleManager.FindById(RoleId).Users;

                        string path = AppDomain.CurrentDomain.BaseDirectory;                        

                        foreach (var item in userRole)
                        {
                            var user = db.UserProfiles.Where(x => x.Id == item.UserId).FirstOrDefault();
                            
                            string p = user.Image.Replace("/", "\\");

                            try
                            {
                                user.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                            }
                            catch
                            {
                                user.Image = "images/av1.png";
                            }

                            lst.Add(user);
                        }

                        lst = lst.Where(x => x.FirstName.ToLower().Contains(Search.ToLower())).ToList();                        
                        return lst;
                    }
                    else
                    {
                        var users = GetUsersBelowList(RoleId);
                        users = users.Where(x => x.FirstName.ToLower().Contains(Search.ToLower())).ToList();

                        string path = AppDomain.CurrentDomain.BaseDirectory;

                        foreach (var item in users)
                        {
                            string p = item.Image.Replace("/", "\\");

                            try
                            {
                                item.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                            }
                            catch
                            {
                                item.Image = "images/av1.png";
                            }
                        }

                        return users;
                    }
                   
                }
                
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public UserProfileViewModel GetCOO(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    UserProfileViewModel user = new Models.UserProfileViewModel();

                    var userDiv = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();
                    var com = userDiv.Project.CompanyId;
                    var coo = db.Companies.Where(x => x.Id == com).FirstOrDefault();

                    user.ApplicationUser = UserManager.FindById(coo.COO);
                    user.EmployeementInfos = db.EmployeementInfos.Where(x => x.Id == coo.COO).FirstOrDefault();
                    user.UserProfiles = db.UserProfiles.Where(x => x.Id == coo.COO).FirstOrDefault();

                    //var proj = db.Projects.Where(x => x.CompanyId == com).ToList();
                    //var lv1 = RoleManager.Roles.ToList().Where(x => x.Id == "63edcefd-3072-4ec4-b0cd-f7ce8e3dec5c").FirstOrDefault().Users.ToList();

                    //foreach(var item in proj)
                    //{
                    //    foreach(var sub in lv1)
                    //    {
                    //        var projUser = db.EmployeementInfos.Where(x => x.Id == sub.UserId && x.Division == item.Id).FirstOrDefault();
                    //    }
                    //}

                    return user;
                }

            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserProfile> SearchUsers(string Search)
        {
            try
            {
                List<UserProfile> lst = new List<UserProfile>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var users = db.UserProfiles.Where(x => x.FirstName.ToLower().Contains(Search.ToLower())).ToList();

                    return users;

                }

            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<RoleLevels> GetRoleLevels()
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public UserRoleViewModels GetRole(string RoleId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var role = RoleManager.FindById(RoleId);
                    var roleLevel = db.RoleLevels.Where(x => x.RoleId == RoleId).FirstOrDefault();

                    UserRoleViewModels obj = new UserRoleViewModels();
                    obj.Id = RoleId;
                    obj.RoleName = role.Name;
                    obj.LevelId = roleLevel.Id;
                    obj.SortOrder = roleLevel.SortOrder;
                    obj.CanEdit = roleLevel.CanEdit;
                    obj.CanViewAll = roleLevel.CanViewAll;

                    return obj;
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<EmployeeAward> GetEmployeeAwards(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.EmployeeAwards.Where(x => x.UserId == UserId).ToList();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public EmployeeAward GetEmployeeAward(int AwardId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.EmployeeAwards.Where(x => x.Id == AwardId).FirstOrDefault();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<EmployeePromotion> GetEmployeePromotion(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.EmployeePromotions.Where(x => x.UserId == UserId).ToList();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public EmployeePromotion GetEmployeePromotion(int PromotionId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.EmployeePromotions.Where(x => x.Id == PromotionId).FirstOrDefault();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<PastExperiance> GetEmployeeExperiance(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.PastExperiances.Where(x => x.UserId == UserId).ToList();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public PastExperiance GetEmployeeExperiance(int PromotionId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.PastExperiances.Where(x => x.Id == PromotionId).FirstOrDefault();
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<UserModulesViewModel> GetUserModule(string LevelId)
        {
            try
            {
                List<UserModulesViewModel> lst = new List<UserModulesViewModel>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {                    
                    var modules = db.UserModules.ToList();
                    
                    foreach(var item in modules)
                    {
                        var userLevels = db.ModuleUsers.Where(x => x.UserLevelId == LevelId && x.ModuleId == item.Id).FirstOrDefault();

                        if(userLevels != null)
                        {
                            UserModulesViewModel obj = new UserModulesViewModel
                            {
                                Id = item.Id,
                                Module = item.Module,
                                IsChecked = true
                            };

                            lst.Add(obj);
                        }
                        else
                        {
                            UserModulesViewModel obj = new UserModulesViewModel
                            {
                                Id = item.Id,
                                Module = item.Module,
                                IsChecked = false
                            };

                            lst.Add(obj);
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

        public List<ModuleUsers> GetModuleUsers(string LevelId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var roleId = LevelId;
                    return db.ModuleUsers.Where(x => x.UserLevelId == roleId).ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public List<ModuleUsers> GetMenu(string Level)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var roleId = RoleManager.FindByName(Level).Id;
                    return db.ModuleUsers.Where(x => x.UserLevelId == roleId).ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public bool AddEmployeeAwards(EmployeeAward model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var award = db.EmployeeAwards.Where(x => x.Id == model.Id).FirstOrDefault();

                    if(award != null)
                    {
                        db.Entry(award).CurrentValues.SetValues(model);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.EmployeeAwards.Add(model);
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

        public bool AddEmployeePromotions(EmployeePromotion model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var award = db.EmployeePromotions.Where(x => x.Id == model.Id).FirstOrDefault();

                    if (award != null)
                    {
                        db.Entry(award).CurrentValues.SetValues(model);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.EmployeePromotions.Add(model);
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

        public bool AddEmployeePastExperiance(PastExperiance model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var award = db.PastExperiances.Where(x => x.Id == model.Id).FirstOrDefault();

                    if (award != null)
                    {
                        db.Entry(award).CurrentValues.SetValues(model);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.PastExperiances.Add(model);
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

        public int GetNewRoleIndex()
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var roleLevels = db.RoleLevels.OrderByDescending(x => x.SortOrder).ToList();
                    return roleLevels.FirstOrDefault().SortOrder + 1;
                }
            }
            catch(Exception er)
            {
                return 0;
            }
        }

        public bool IsHaveAccess(int ModuleId, string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var user = UserManager.FindById(UserId).Roles.FirstOrDefault();
                    var modules = db.ModuleUsers.Where(x => x.UserLevelId == user.RoleId && x.ModuleId == ModuleId).FirstOrDefault();

                    if(modules != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool AddNewRole(string RoleName, int SortOrder, bool Canedit, bool ViewAll)
        {
            bool ret = true;

            try
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = RoleName;
                RoleManager.Create(role);

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var currentLevel = db.RoleLevels.Where(x => x.SortOrder >= SortOrder).OrderBy(x => x.SortOrder).ToList();

                    if (currentLevel.Count > 0)
                    {
                        foreach(var item in currentLevel)
                        {
                            item.SortOrder += 1;
                        }

                        db.RoleLevels.Add(new Models.RoleLevels { RoleId = role.Id, SortOrder = SortOrder, CanEdit = Canedit, CanViewAll = ViewAll });
                        db.SaveChanges();
                    }
                    else
                    {
                        db.RoleLevels.Add(new Models.RoleLevels { RoleId = role.Id, SortOrder = SortOrder, CanEdit = Canedit, CanViewAll = ViewAll });
                        db.SaveChanges();
                    }
                    
                }
            }
            catch(Exception er)
            {
                ret = false;
            }

            return ret;
        }

        public bool ChangeRole(string RoleId, int ModuleId)
        {
            bool ret = false;
            
            try
            {  
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var role = db.ModuleUsers.Where(x => x.ModuleId == ModuleId && x.UserLevelId == RoleId).FirstOrDefault();
                    
                    if (role == null)
                    {
                        db.ModuleUsers.Add(new Models.ModuleUsers { ModuleId = ModuleId, UserLevelId = RoleId });
                        db.SaveChanges();

                        ret = true;
                    }
                    else
                    {
                        db.ModuleUsers.Remove(role);
                        db.SaveChanges();

                        ret = true;
                    }
                    
                }
            }
            catch (Exception er)
            {
                ret = false;
            }

            return ret;
        }
                
        public bool UpdateRole(string RoleId, string RoleName, int SortOrder, bool CanEdit, bool ViewAll)
        {
            bool ret = true;

            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var role = RoleManager.FindById(RoleId);
                    var roleLevel = db.RoleLevels.Where(x => x.RoleId == RoleId).FirstOrDefault();

                    if (role != null)
                    {
                        role.Name = RoleName;
                        RoleManager.Update(role);

                        roleLevel.SortOrder = SortOrder;
                        roleLevel.CanEdit = CanEdit;
                        roleLevel.CanViewAll = ViewAll;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception er)
            {
                ret = false;
            }

            return ret;
        }

        public bool UpdateProfileImage(string UserId, string FilePath)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var user = db.UserProfiles.Where(x => x.Id == UserId).FirstOrDefault();
                    user.Image = FilePath;

                    db.SaveChanges();

                    return true;
                }
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public string AddNewUser(string EPFNo, string FirstName, string Role, string nic,  string DisisionCode = "")
        {
            try
            {
                var user = new ApplicationUser { UserName = EPFNo };
                var res = UserManager.Create(user, "abc@123");

                if(res.Succeeded)
                {
                    _leavs lv = new _leavs();

                    if (user.Id != null)
                    {
                        if (!Role.Contains("Level") && Role.Length == 1)
                        {
                            Role = "Level " + Role;
                        }

                        UserManager.AddToRole(user.Id, Role);

                        using (ApplicationDbContext db = new Models.ApplicationDbContext())
                        {
                            db.UserProfiles.Add(new UserProfile { Id = user.Id, FirstName = FirstName, EmployeeId = EPFNo, NICNo = nic,   DateCreated = DateTime.Now });
                            db.SaveChanges(); 
                            db.EmployeementInfos.Add(new EmployeementInfo { Id = user.Id, DivisionCode = DisisionCode, AppointmentDate = DateTime.Now });
                            db.SaveChanges();

                            //db.EmployeementInfos.Add(new EmployeementInfo { Id = user.Id, DivisionCode = DisisionCode, AppointmentDate = DateTime.Now });
                            //db.SaveChanges();

                            //lv.NewUserLeaves(user.Id, RoleManager.FindByName(Role).Id);
                        }

                    }

                    return user.Id;
                }
                else
                {
                    return "error_" + res.Errors.FirstOrDefault();
                }
                                               
            }
            catch (Exception er)
            {
                return null;
            }
            
        }

        public bool UpdateContactInfo(UserProfileViewModel model)
        {
            try
            {
                var user = UserManager.FindById(model.ApplicationUser.Id);

                if(user != null)
                {
                    user.MobileNumber = model.ApplicationUser.MobileNumber;
                    user.FaxNumber = model.ApplicationUser.FaxNumber;
                    user.Address = model.ApplicationUser.Address;
                    user.MobileAccount = model.ApplicationUser.MobileAccount;
                    user.Email = model.ApplicationUser.Email;
                    user.PhoneNumber = model.ApplicationUser.PhoneNumber;

                    UserManager.Update(user);
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public bool UpdateUserProfile(string UserId, string Role, UserProfileViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {                    
                    var profile = db.UserProfiles.Where(x => x.Id == UserId).FirstOrDefault();
                    var user = UserManager.FindById(UserId);
                    string CurrentRoleId = "";

                    if(!Role.Contains("Level") && Role.Length == 1)
                    {
                        Role = "Level " + Role;
                    }
                    

                    if(profile != null)
                    {
                        model.UserProfiles.FirstName = model.UserProfiles.FirstName + " " + model.UserProfiles.LastName;
                        model.UserProfiles.Id = UserId;
                        model.UserProfiles.DateCreated = profile.DateCreated;

                        db.Entry(profile).CurrentValues.SetValues(model.UserProfiles);
                        db.SaveChanges();                        
                    }
                    else
                    {
                        model.UserProfiles.DateCreated = Convert.ToDateTime(DateTime.Now);
                        db.UserProfiles.Add(model.UserProfiles);
                        db.SaveChanges();                        
                    }

                    if (user.Roles.FirstOrDefault() != null)
                    {
                        CurrentRoleId = RoleManager.FindById(user.Roles.FirstOrDefault().RoleId).Name;

                        UserManager.RemoveFromRole(UserId, CurrentRoleId);
                        UserManager.AddToRole(UserId, Role);
                    }
                    else
                    {
                        UserManager.AddToRole(UserId, Role);
                    }

                    user.UserName = model.UserProfiles.EmployeeId;
                    UserManager.Update(user);                    
                                        
                    //UserManager.RemoveFromRole(UserId, CurrentRoleId.Name);
                    //UserManager.AddToRole(UserId, Role);
                }

                return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool UpdateEducationalInfo(string UserId, UserProfileViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var education = db.EducationalInfos.Where(x => x.Id == UserId).FirstOrDefault();
                    model.EducationalInfos.Id = UserId;

                    if (education == null)
                    {
                        db.EducationalInfos.Add(model.EducationalInfos);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(education).CurrentValues.SetValues(model.EducationalInfos);
                        db.SaveChanges();
                    }
                    
                }

                return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool UpdateEmployeement(string UserId, UserProfileViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var employee = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();                   
                    

                    if (employee == null)
                    {
                        int prid = 0;

                        if(model.EmployeementInfos.Division != null)
                        {
                            prid = Convert.ToInt32(model.EmployeementInfos.Division);
                        }
                        else
                        {
                            prid = model.EmployeementInfos.Project.Id;
                        }

                        model.EmployeementInfos.Id = UserId;
                        model.EmployeementInfos.Division = prid;
                        
                        db.EmployeementInfos.Add(new Models.EmployeementInfo { Id = UserId, Division = prid});
                        db.SaveChanges();

                        var emp = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();
                        db.Entry(emp).CurrentValues.SetValues(model.EmployeementInfos);
                        db.SaveChanges();
                    }
                    else
                    {
                        int prid = 0;

                        try
                        {
                            prid = model.EmployeementInfos.Division.Value;
                        }
                        catch
                        {
                            prid = Convert.ToInt32(employee.Division);
                        }

                        model.EmployeementInfos.Id = UserId;
                        model.EmployeementInfos.Division = prid;
                        employee.Division = model.EmployeementInfos.Division;
                        //employee.Location = model.EmployeementInfos.Location;
                        //employee.Section = model.EmployeementInfos.Section;
                        //employee.SupervisorId = model.EmployeementInfos.SupervisorId;
                        //employee.PresentReportingLocation = model.EmployeementInfos.PresentReportingLocation;
                        //employee.Designation = model.EmployeementInfos.Designation;
                        //employee.AppointmentDate = model.EmployeementInfos.AppointmentDate;

                        db.Entry(employee).CurrentValues.SetValues(model.EmployeementInfos);
                        db.SaveChanges();
                    }
                    
                }

                return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public List<EmployeeLocation> LocationHistory(DateTime From, DateTime To, string[] Users)
        {
            try
            {
                List<EmployeeLocation> lst = new List<Models.EmployeeLocation>();
                To = To.AddDays(1);

                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    foreach(var user in Users)
                    {
                        var loc = db.EmployeeLocations.Where(x => x.UserId == user & x.Date >= From & x.Date <= To).ToList();
                        lst.AddRange(loc);
                    }
                    
                    return lst;
                }
            }
            catch(Exception er)
            {
                return null;
            }
        }

        public List<UserModulesViewModel> GetMobileUserModule(string UserId) {
            try {
                List<UserModulesViewModel> lst = new List<UserModulesViewModel>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var modules = db.UserModules.ToList();

                    foreach (var item in modules) {
                        var rol = UserManager.FindById(UserId);
                        var role = rol.Roles.FirstOrDefault().RoleId;
                       //var rolid = RoleManager.FindById(rol.Roles.FirstOrDefault().RoleId);
                        var userLevels = db.ModuleUsers.Where(x => x.UserLevelId == role && x.ModuleId == item.Id).FirstOrDefault();

                        if (userLevels != null) {
                            UserModulesViewModel obj = new UserModulesViewModel {
                                Id = item.Id,
                                Module = item.Module,
                                IsChecked = true,
                                UserLevelId = role,
                                IsAdmin = (role == "9860cc6b-1888-4397-985c-79f04a03aaba" || role == "e9fb703b-4515-434e-be7e-085100f67535") ? true : false
                            };

                            lst.Add(obj);
                        } else {
                            UserModulesViewModel obj = new UserModulesViewModel {
                                Id = item.Id,
                                Module = item.Module,
                                IsChecked = false,
                                UserLevelId = role,
                                 IsAdmin = (role == "9860cc6b-1888-4397-985c-79f04a03aaba" || role == "e9fb703b-4515-434e-be7e-085100f67535") ? true : false
                            };

                            lst.Add(obj);
                        }

                    }

                }

                return lst;
            } catch (Exception er) {
                return null;
            }
        }

        public bool DeleteUser(string UserId)
        {
            try
            {
                var user = UserManager.FindById(UserId);
                UserManager.Delete(user);

                return true;
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public bool ChangePassword(string UserId, string CurrentPassword, string NewPassword)
        {
            try
            {
                var user = UserManager.ChangePassword(UserId, CurrentPassword, NewPassword);
                
                if(user.Errors.Count() == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception er)
            {
                return false;
            }
            
        }

        public bool ResetPassword(string UserId)
        {
            //var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("SEMS");
            var provider = new DpapiDataProtectionProvider("SEMS");
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));

            string token = UserManager.GeneratePasswordResetToken(UserId);
            IdentityResult result = UserManager.ResetPassword(UserId, token, "abc@123");

            if(result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class AdminAuthorization : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                EmployeeTracking.App_Codes._usermanager um = new EmployeeTracking.App_Codes._usermanager();
                var userRoles = um.GetUserRoles(HttpContext.Current.Session["UserId"].ToString());
                //var roleId = um.RoleManager.FindByName(userRoles.FirstOrDefault()).Id;
                //var roleLevels = um.GetRoleLevels().Where(x => x.RoleId == roleId).FirstOrDefault();
                var module = um.GetMenu(userRoles.FirstOrDefault());
                var controllerName = filterContext.Controller.ToString().Split('.')[2].Replace("Controller", "");
                var actionName = filterContext.ActionDescriptor.ActionName;

                var ctrId = 0;

                if (controllerName == "Companies")
                {
                    ctrId = 6;
                }
                else if (controllerName == "Divisions")
                {
                    if (actionName.Contains("Team"))
                    {
                        ctrId = 10;
                    }
                    else
                    {
                        ctrId = 9;
                    }
                }
                else if (controllerName == "UserManagement")
                {
                    if (actionName.Contains("UserProfile"))
                    {
                        ctrId = 11;
                    }
                    else if (actionName.Contains("Index"))
                    {
                        ctrId = 11;
                    }
                    else if (actionName.Contains("Leaves"))
                    {
                        ctrId = 12;
                    }
                    else if (actionName.Contains("UserLevels"))
                    {
                        ctrId = 7;
                    }
                    else if (actionName.Contains("Transfers"))
                    {
                        ctrId = 15;
                    }
                    else if (actionName.Contains("EnforceUserLeave"))
                    {
                        ctrId = 21;
                    }
                    else if (actionName.Contains("LeaveHistory"))
                    {
                        ctrId = 22;
                    }

                }
                else if (controllerName.Contains("News"))
                {
                    ctrId = 14;
                }
                else if (controllerName.Contains("Reports"))
                {
                    if (actionName.Contains("Leave"))
                    {
                        ctrId = 16;
                    }
                    else if (actionName.Contains("Attendance"))
                    {
                        ctrId = 17;
                    }
                    else if (actionName.Contains("Transfer"))
                    {
                        ctrId = 20;
                    }
                }

                if (userRoles.FirstOrDefault() != "SuperAdmin" || userRoles.FirstOrDefault() != "Super Level")
                {
                    if (module.Where(x => x.ModuleId == ctrId).Count() == 0)
                    {
                        if(actionName.Contains("MyProfile"))
                        {
                            //string unAuthorizedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl(new { controller = "UserManagement", action = "MyProfile" });
                            //filterContext.HttpContext.Response.Redirect(unAuthorizedUrl);
                        }
                        else if(actionName.Contains("Latest"))
                        {
                            //string unAuthorizedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl(new { controller = "News", action = "Latest" });
                            //filterContext.HttpContext.Response.Redirect(unAuthorizedUrl);
                        }
                        else if(actionName.Contains("ChangePassword"))
                        {
                            string unAuthorizedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl(new { controller = "Account", action = "Index" });
                            filterContext.HttpContext.Response.Redirect(unAuthorizedUrl);
                        }
                        else if (actionName.Contains("ResetPassword"))
                        {
                            string unAuthorizedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl(new { controller = "UserManagement", action = "Index" });
                            filterContext.HttpContext.Response.Redirect(unAuthorizedUrl);
                        }
                        else
                        {
                            //string unAuthorizedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl(new { controller = "Error", action = "Index" });
                            //filterContext.HttpContext.Response.Redirect(unAuthorizedUrl);
                        }

                    }
                }
            }
            catch
            {
                string unAuthorizedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl(new { controller = "Account", action = "Index" });
                filterContext.HttpContext.Response.Redirect(unAuthorizedUrl);
            }
            
            
        }
    }

    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Custom attribute for handling session timeout
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            
            // check if session is supported
            if (ctx.Session != null)
            {
                // check if a new session id was generated
                if (ctx.Session.IsNewSession)
                {
                    // If it says it is a new session, but an existing cookie exists, then it must
                    // have timed out
                    string sessionCookie = ctx.Request.Headers["Cookie"];
                    if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        ctx.Response.Redirect("~/Account/Index");
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}