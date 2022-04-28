using EmployeeTracking.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EmployeeTracking.App_Codes {
    public class _profile {
        public UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        public RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

        public mobileprofile GetUser(string UserId) {
            try {
               
                 

               // UserProfileViewModel uvm = new Models.UserProfileViewModel();
                ApplicationDbContext db = new Models.ApplicationDbContext();

                var ApplicationUser = UserManager.FindById(UserId);
                var UserProfiles = db.UserProfiles.Where(x => x.Id == UserId).FirstOrDefault();
                var rol = UserManager.FindById(UserId);
                UserProfiles.UserRole = RoleManager.FindById(rol.Roles.FirstOrDefault().RoleId).Name;
                                
                var EmployeePromotions = db.EmployeePromotions.Where(x => x.UserId == UserId).FirstOrDefault();
                var EducationalInfos = db.EducationalInfos.Where(x => x.Id == UserId).FirstOrDefault();
                var EmployeementInfos = db.EmployeementInfos.Where(x => x.Id == UserId).FirstOrDefault();
                var EmployeeAwards = db.EmployeeAwards.Where(x => x.UserId == UserId).ToList();
                var UserCompanies = db.UserCompanies.Where(x => x.UserId == UserId).ToList();

                var d = (from pr in db.Projects
                         join prt in db.ProjectTeams
                         on pr.Id equals prt.ProjectId
                         join team in db.TeamMembers
                         on prt.Id equals team.TeamId
                         where team.UserId == UserId
                         select new { pr.ProjectName,prt.Location }).FirstOrDefault();

                mobileprofile mp = new mobileprofile();
                if (UserProfiles != null) {
                    
                    mp.UsersId = UserProfiles.Id;
                    mp.Name = UserProfiles.FirstName + " " + UserProfiles.LastName;
                   
                    mp.EPFNo = UserProfiles.EmployeeId;
                    mp.Age = UserProfiles.Age.ToString();

                    if(UserProfiles.DateOfBirth != null)
                    {
                        var userAge = DateTime.Now.Year - UserProfiles.DateOfBirth.Value.Year;
                        mp.Age = userAge.ToString();
                    }

                    mp.Gender = UserProfiles.Gender;
                    mp.MaritalStatus = UserProfiles.MaritalStatus;
                    mp.NICNo = UserProfiles.NICNo;
                    if (UserProfiles.DateOfBirth != null) {
                        mp.DateOfBirth = Convert.ToDateTime(UserProfiles.DateOfBirth).ToString("MM-dd-yyyy");
                    }

                    mp.Remarks = UserProfiles.Remarks;


                    if (UserProfiles.Image != null) {
                        string path = AppDomain.CurrentDomain.BaseDirectory;
                        String p = UserProfiles.Image.Replace("/", "\\");

                        try {
                            mp.userimage = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                        } catch (Exception ex) {
                            mp.userimage = "images/av1.png";
                        }
                    } else {
                        mp.userimage = "images/av1.png";
                    }

                } else {
                    mp.userimage = "images/av1.png";
                }

                if (ApplicationUser != null) {
                    mp.PermanentAddress = ApplicationUser.Address;
                    mp.MobileNumbers = ApplicationUser.MobileNumber;
                    mp.FixedTelephoneNumber = ApplicationUser.PhoneNumber;
                    mp.Email = ApplicationUser.Email;
                  
                }

                //if (EmployeePromotions != null) {
                   
                //    mp.Division = EmployeePromotions.Division;
                   
                //}

                if (EmployeementInfos != null) {
                    mp.Section = EmployeementInfos.Section;

                    mp.Designation = EmployeementInfos.Designation;
                    // mp.Division= EmployeementInfos.Division;

                  var dev=  db.Projects.Where(m => m.Id == EmployeementInfos.Division).FirstOrDefault();
                    if (dev != null) {
                        mp.Division = dev.ProjectName;
                    }
                }



                if (EducationalInfos != null) {
                    mp.SecondaryEducation = EducationalInfos.Primary;
                    mp.HigherEducation = EducationalInfos.Secondary;
                    mp.OtherEducationalQualifications = EducationalInfos.Other;
                    mp.ExtraCurricularActivities = EducationalInfos.ExtraCurricular;

                }
                
                if (EmployeementInfos != null) {
                    mp.JobDescription = EmployeementInfos.JobDescription;
                    if (EmployeementInfos.AppointmentDate != null) {
                        mp.DateofAppointment = Convert.ToDateTime(EmployeementInfos.AppointmentDate).ToString("MM-dd-yyyy");
                    }
                    var super = db.UserProfiles.Where(x => x.Id == EmployeementInfos.SupervisorId).FirstOrDefault();
                    if (super != null) {
                        mp.ImmediateReportingPerson = super.FirstName+" "+super.LastName;
                    }
                    mp.PresentReportingLocation = EmployeementInfos.Location;
                                    

                }
                 
                return mp;
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
                                    if (message != null) {
                                        msguser.LastMessage = message.Message1;
                                        msguser.MessageTime = message.Date.ToString("dd/MM/yyyy");
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
                                if (message != null) {
                                    msguser.LastMessage = message.Message1;
                                    msguser.MessageTime = message.Date.ToString("dd/MM/yyyy");
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

    }
}