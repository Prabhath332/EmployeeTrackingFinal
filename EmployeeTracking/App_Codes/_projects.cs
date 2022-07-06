using EmployeeTracking.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.App_Codes {
    public class _projects {
        public UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        public RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        public List<SelectListItem> ProjectsList() {
            try {
                List<SelectListItem> lst = new List<SelectListItem>();
                var companyId = Convert.ToInt32(HttpContext.Current.Session["CompanyId"].ToString());
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    List<Project> prj = new List<Project>();

                    if(companyId > 0)
                    {
                        prj = db.Projects.Where(x => x.CompanyId == companyId).ToList();
                    }
                    else
                    {
                        prj = db.Projects.ToList();
                    }
                    //var projects = db.Projects.Where(x => x.CompanyId == companyId).ToList();

                    lst.Add(new SelectListItem { Text = "[Select a Project]", Value = "0" });

                    foreach (var item in prj) {
                        lst.Add(new SelectListItem { Text = item.ProjectName, Value = item.Id.ToString() });
                    }

                }

                return lst;
            } catch (Exception er) {
                return null;
            }
        }



        public List<Project> GetProjects() {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {

                    return db.Projects.ToList();
                }
            } catch (Exception er) {
                return null;
            }
        }



        public List<Project> GetProjects(int CompanyId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    if(CompanyId != 0)
                    {
                        return db.Projects.Where(x => x.CompanyId == CompanyId).ToList();                        
                    }
                    else
                    {
                        return db.Projects.ToList();
                    }                   
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }


        public List<Project> GetProjectsById(int ProjectId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    if (ProjectId != 0)
                    {
                        return db.Projects.Where(x => x.Id == ProjectId).ToList();
                    }
                    else
                    {
                        return db.Projects.ToList();
                    }
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }


        public List<SiteLocation> GetLocationsById(string Location)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    if (Location != null)
                    {
                        return db.SiteLocations.Where(x => x.Location.ToLower() == Location.ToLower()).ToList();
                    }
                    else
                    {
                        return db.SiteLocations.ToList();
                    }
                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<ProjectUsersViewModels> GetProjectUsers(int ProjectId, string EmployeeId, string search)
        {
            try
            {
                List<ProjectUsersViewModels> lst = new List<Models.ProjectUsersViewModels>();
                
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    _usermanager um = new App_Codes._usermanager();

                    if(string.IsNullOrEmpty(search))
                    {
                        var prousers = db.EmployeementInfos.Where(x => x.Division == ProjectId).ToList();
                        var team = db.TeamMembers.ToList();

                        foreach (var item in prousers)
                        {
                            var user = um.GetUser(item.Id);

                            if (team.Where(x => x.UserId == item.Id).Count() == 0)
                            {
                                ProjectUsersViewModels pu = new Models.ProjectUsersViewModels
                                {
                                    UserId = item.Id,
                                    Username = um.FullUserName(user.ApplicationUser.UserName),
                                    ProjectId = ProjectId
                                };

                                lst.Add(pu);
                            }
                        }
                    }
                    else
                    {
                        //var roleLevels = db.RoleLevels.OrderBy(x => x.SortOrder).ToList();
                        //var empRole = UserManager.FindById(EmployeeId).Roles.FirstOrDefault();
                        //var emRoleIndex = roleLevels.Where(x => x.RoleId == empRole.RoleId).FirstOrDefault().SortOrder;

                        //roleLevels = roleLevels.Where(x => x.SortOrder < emRoleIndex).ToList();

                        var divUsers = (from emp in db.EmployeementInfos
                                        join user in db.UserProfiles
                                        on emp.Id equals user.Id
                                        //join role in db.Roles                                        
                                        where emp.Division == ProjectId && user.FirstName.Contains(search) select new { emp, user}).ToList();

                        foreach(var item in divUsers)
                        {
                            //var roles = RoleManager.Roles.ToList();

                            //foreach(var rl in roleLevels)
                            //{
                            //    var usr = roles.Where(x => x.Id == rl.RoleId).FirstOrDefault().Users
                            //}

                            ProjectUsersViewModels pu = new Models.ProjectUsersViewModels
                            {
                                UserId = item.emp.Id,
                                Username = item.user.FirstName,
                                ProjectId = ProjectId
                            };

                            lst.Add(pu);
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

        public List<ProjectTeam> GetTeams() {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    return db.ProjectTeams.ToList();
                }

            } catch (Exception er) {
                return null;
            }
        }

        public List<ProjectTeam> GetTeams(int ProjectId) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    return db.ProjectTeams.Where(x => x.ProjectId == ProjectId).ToList();
                }

            } catch (Exception er) {
                return null;
            }
        }

        public List<TeamMemberViewModels> GetTeamMemebers(int TeamId) {
            try {
                List<TeamMemberViewModels> lst = new List<Models.TeamMemberViewModels>();
                _usermanager um = new App_Codes._usermanager();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var members = db.TeamMembers.Where(x => x.TeamId == TeamId).ToList();

                    foreach (var item in members) {
                        var user = um.GetUser(item.UserId).ApplicationUser;
                        bool IsSupervisor = false;

                        if (item.IsSupervisor == null) {
                            IsSupervisor = false;
                        } else {
                            IsSupervisor = Convert.ToBoolean(item.IsSupervisor);
                        }

                        TeamMemberViewModels tm = new Models.TeamMemberViewModels {
                            UserId = item.UserId,
                            MemberName = um.FullUserName(user.UserName),
                            ProjectId = item.ProjectId,
                            IsSupervisor = IsSupervisor,
                            TeamId = item.TeamId,
                            EPFNo = user.UserName,
                            MobilePhone = user.MobileNumber
                        };

                        lst.Add(tm);
                    }

                    return lst;

                }

            } catch (Exception er) {
                return null;
            }
        }

        public ProjectsViewModel GetProject(int DivisionId) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    ProjectsViewModel pvm = new Models.ProjectsViewModel();
                    var pro = db.Projects.Where(x => x.Id == DivisionId).FirstOrDefault();
                    pvm.CompanyId = pro.CompanyId;
                    pvm.Description = pro.Description;
                    pvm.ProjectManager = pro.ProjectManager;
                    pvm.ProjectName = pro.ProjectName;
                    pvm.Id = pro.Id;
                    return pvm;
                }

            } catch (Exception er) {
                return null;
            }
        }

        public ProjectsViewModel GetProject(string DivisionName) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    ProjectsViewModel pvm = new Models.ProjectsViewModel();
                    var pro = db.Projects.Where(x => x.ProjectName == DivisionName).FirstOrDefault();
                    pvm.CompanyId = pro.CompanyId;
                    pvm.Description = pro.Description;
                    pvm.ProjectManager = pro.ProjectManager;
                    pvm.ProjectName = pro.ProjectName;
                    pvm.Id = pro.Id;
                    return pvm;
                }

            } catch (Exception er) {
                return null;
            }
        }

        public TeamsViewModel GetTeam(int TeamId) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    TeamsViewModel tvm = new Models.TeamsViewModel();
                    var pro = db.ProjectTeams.Where(x => x.Id == TeamId).FirstOrDefault();
                    tvm.TeamName = pro.TeamName;
                    tvm.Location = pro.Location;
                    tvm.Id = pro.Id;
                    tvm.ProjectId = pro.ProjectId;
                    return tvm;
                }

            } catch (Exception er) {
                return null;
            }
        }

        public bool AddDivision(int DivisionId, Project model) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var division = db.Projects.Where(x => x.Id == DivisionId).FirstOrDefault();

                    if (division != null) {
                        model.Id = DivisionId;
                        db.Entry(division).CurrentValues.SetValues(model);
                        db.SaveChanges();
                    } else {
                        db.Projects.Add(model);
                        db.SaveChanges();
                    }


                }

                return true;
            } catch (Exception er) {
                return false;
            }
        }

        public bool AddSupervisor(string UserId, int TeamId) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var currentSupervisor = db.TeamMembers.Where(x => x.TeamId == TeamId && x.IsSupervisor == true).FirstOrDefault();
                    var team = db.TeamMembers.Where(x => x.UserId == UserId && x.TeamId == TeamId).FirstOrDefault();

                    if (team != null) {
                        if (currentSupervisor != null) {
                            currentSupervisor.IsSupervisor = false;
                        }

                        team.IsSupervisor = true;
                        db.SaveChanges();
                    }


                }

                return true;
            } catch (Exception er) {
                return false;
            }
        }

        public bool RemoveTeamMember(string UserId, int TeamId) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var team = db.TeamMembers.Where(x => x.UserId == UserId && x.TeamId == TeamId).FirstOrDefault();

                    if (team != null) {
                        db.TeamMembers.Remove(team);
                        db.SaveChanges();
                    }


                }

                return true;
            } catch (Exception er) {
                return false;
            }
        }

        public bool AddTeamMember(string UserId, int TeamId, int DivisionId) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var team = db.TeamMembers.Where(x => x.UserId == UserId).FirstOrDefault();

                    if (team == null) {
                        db.TeamMembers.Add(new Models.TeamMember { TeamId = TeamId, ProjectId = DivisionId, UserId = UserId, IsSupervisor = false });
                        db.SaveChanges();
                    }
                }

                return true;
            } catch (Exception er) {
                return false;
            }
        }

        public bool AddTeam(int TeamId, ProjectTeam model) {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    var team = db.ProjectTeams.Where(x => x.Id == TeamId).FirstOrDefault();

                    if (team != null) {
                        model.Id = TeamId;
                        db.Entry(team).CurrentValues.SetValues(model);
                        db.SaveChanges();
                    } else {
                        db.ProjectTeams.Add(model);
                        db.SaveChanges();
                    }
                }
                return true;
            } catch (Exception er) {
                return false;
            }
        }

        public List<ProjectsViewModel> GetAllProjects() {
            try {
                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {

                    var pr = db.Projects.ToList(); ;
                    List<ProjectsViewModel> list = new List<ProjectsViewModel>();
                    foreach (var item in pr) {
                        ProjectsViewModel model = new ProjectsViewModel();
                        model.Id = item.Id;
                        model.ProjectName = item.ProjectName;
                        list.Add(model);
                    }
                    return list;
                }

            } catch (Exception er) {
                return null;
            }
        }

        public List<messageusers> GetUsersBy(int ProjectId) {
            try {
                List<messageusers> lst = new List<messageusers>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    //messageusers msguser = new messageusers();

                    //var isinproject = db.Projects.Where(m => m.Id == ProjectId).FirstOrDefault();
                    //var d = (from pr in db.Projects
                    //         join prt in db.ProjectTeams
                    //         on pr.Id equals prt.ProjectId
                    //         join team in db.TeamMembers
                    //         on prt.Id equals team.TeamId
                    //         where pr.Id == ProjectId
                    //         select new { team.UserId }).ToList();

                    var d = db.EmployeementInfos.Where(x => x.Division == ProjectId).ToList();

                    foreach (var item in d) {
                        var user = db.UserProfiles.Where(x => x.Id == item.Id).FirstOrDefault();
                        messageusers msguser = new messageusers();
                        if (user != null) {                            
                            if (user.Image != null) {
                                string path = AppDomain.CurrentDomain.BaseDirectory;
                                String p = user.Image.Replace("/", "\\");

                                try
                                {
                                    msguser.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                                }
                                catch
                                {
                                    msguser.Image = "images/av1.png";
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

        public List<messageusers> SearchUsersBy(String UserName,int ProjectId,String MyuserId) {
            try {
                List<messageusers> lst = new List<messageusers>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {
                    //messageusers msguser = new messageusers();
                    //var isinproject = db.Projects.Where(m => m.Id == ProjectId).FirstOrDefault();
                    //var d = (from pr in db.Projects
                    //         join prt in db.ProjectTeams
                    //         on pr.Id equals prt.ProjectId
                    //         join team in db.TeamMembers
                    //         on prt.Id equals team.TeamId
                    //         where pr.Id == ProjectId
                    //         select new { team.UserId }).ToList();

                    var MyRol = UserManager.FindById(MyuserId).Roles.FirstOrDefault();
                    int MyLevel = db.RoleLevels.Where(m => m.RoleId == MyRol.RoleId).FirstOrDefault().SortOrder;

                    var belowRoles = db.RoleLevels.Where(x => x.SortOrder > MyLevel).ToList();

                    foreach (var item in belowRoles)
                    {

                        var usersInRole = RoleManager.FindById(item.RoleId).Users;

                        foreach (var row in usersInRole)
                        {
                            //var user = db.UserProfiles.Where(x => x.Id == row.UserId).FirstOrDefault();

                            var prUsers = (from up in db.UserProfiles
                                           join emp in db.EmployeementInfos
                                           on up.Id equals emp.Id where emp.Id == row.UserId && emp.Division == ProjectId && up.FirstName.Contains(UserName) select new { up.FirstName, up.LastName}).FirstOrDefault();

                            if(prUsers != null)
                            {
                                messageusers msguser = new messageusers();

                                msguser.UserId = row.UserId;
                                msguser.Name = prUsers.FirstName + " " + prUsers.LastName;
                                msguser.RoalId = row.RoleId;
                                lst.Add(msguser);
                            }
                           
                        }                        
                    }                    
                }
                return lst;
            } catch (Exception er) {
                return null;
            }
        }

        public bool DeleteDivision(int DivisionId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var prt = db.ProjectTeams.Where(x => x.ProjectId == DivisionId).ToList();

                    if(prt.Count > 0)
                    {
                        db.ProjectTeams.RemoveRange(prt);
                        db.SaveChanges();
                    }
                                       

                    var div = db.Projects.Where(x => x.Id == DivisionId).FirstOrDefault();

                    db.Projects.Remove(div);
                    db.SaveChanges();

                    return true;
                }
            }
            catch(Exception er)
            {
                return false;
            }
        }


        public bool DeleteTeam(int TeamId)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    var prt = db.ProjectTeams.Where(x => x.Id == TeamId).FirstOrDefault();

                    if (prt != null)
                    {
                        db.ProjectTeams.Remove(prt);
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