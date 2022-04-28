using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeTracking.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EmployeeTracking.App_Codes {
    public class _location {
        public UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        public RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        public String SaveMbileLocation(EmployeeLocation mobilelocation) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");
                    var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                    mobilelocation.Date = dateTime;
                    db.EmployeeLocations.Add(mobilelocation);
                    db.SaveChanges();
                    return "Saved";
                }
            } catch (Exception ex) {
                return "Faild";
            }
        }


        public List<messageusers> SearchUsersBy(String UserName, String MyUserId) {
            try {
                List<messageusers> lst = new List<messageusers>();

                using (ApplicationDbContext db = new Models.ApplicationDbContext()) {

                    var MyRol = UserManager.FindById(MyUserId).Roles.FirstOrDefault();
                    int MyLevel = db.RoleLevels.Where(m => m.RoleId == MyRol.RoleId).FirstOrDefault().SortOrder;

                    //var userlst = db.UserProfiles.Where(x => (x.FirstName + " " + x.LastName).Contains(UserName)).ToList();
                    var belowRoles = db.RoleLevels.Where(x => x.SortOrder > MyLevel).ToList();

                    foreach (var item in belowRoles)
                    {

                        var usersInRole = RoleManager.FindById(item.RoleId).Users;

                        foreach(var row in usersInRole)
                        {
                            var user = db.UserProfiles.Where(x => x.Id == row.UserId).FirstOrDefault();

                            messageusers msguser = new messageusers();
                            //if (item.Image != null) {
                            //    string path = AppDomain.CurrentDomain.BaseDirectory;
                            //    String p = item.Image.Replace("/", "\\");
                            //    msguser.Image = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                            //}
                            msguser.UserId = row.UserId;
                            msguser.Name = user.FirstName + " " + user.LastName;
                            msguser.RoalId = row.RoleId;
                            lst.Add(msguser);
                        }
                        

                    }
                    
                }
                return lst;
            } catch (Exception er) {
                return null;
            }
        }


        public LocationUserModel GetLocationByuserid(String UserId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var user = db.UserProfiles.Where(m => m.Id == UserId).FirstOrDefault();
                    if (user != null) {
                        LocationUserModel usermodel = new LocationUserModel();
                        var rol = UserManager.FindById(UserId);
                        var EmployeePromotions = db.EmployeePromotions.Where(m => m.UserId == user.Id).FirstOrDefault();
                        if (EmployeePromotions != null) {
                            usermodel.Designation = EmployeePromotions.Designation;
                            usermodel.ProjectLocation = EmployeePromotions.Location;
                        }
                   
                        usermodel.FullName = user.FirstName + " " + user.LastName;

                        if (user.Image != null) {
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            String p = user.Image.Replace("/", "\\");
                            try {
                                usermodel.profileImage = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                            } catch {
                                usermodel.profileImage = "images/av1.png";
                            }
                        } else {
                            usermodel.profileImage = "images/av1.png";
                        }

                        var location = db.EmployeeLocations.Where(m => m.UserId == user.Id).OrderByDescending(m => m.Id).FirstOrDefault();
                        if (location != null) {
                            var timegap = DateTime.Now.Minute - location.Date.Minute;
                            if (timegap < 20) {
                                usermodel.LocationStatus = "In";
                            } else {
                                usermodel.LocationStatus = "Out";
                            }

                            

                            List<LocationModel> lst = new List<LocationModel>();
                            LocationModel lcm = new LocationModel();
                            lcm.lati = location.Latitude;
                            lcm.longi = location.Longitude;
                            lcm.Description = location.MapLocation;
                            usermodel.LastLocationDate = location.Date.ToString("MM/dd/yy HH:mm tt");
                            lst.Add(lcm);
                            usermodel.LocationList = lst;
                            return usermodel;
                        } else {
                            usermodel.LocationStatus = "Out";
                            return usermodel;
                        }

                    } else {
                        return null;
                    }
                }
            } catch (Exception ex) {
                return null;
            }
        }

        public LocationUserModel GetLocationHistoryByuserid(DateTime startdate, DateTime enddate, String UserId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var user = db.UserProfiles.Where(m => m.Id == UserId).FirstOrDefault();
                    if (user != null) {
                        LocationUserModel usermodel = new LocationUserModel();
                        var rol= UserManager.FindById(UserId);
                   
                        usermodel.FullName = user.FirstName + " " + user.LastName;

                        var EmployeePromotions = db.EmployeePromotions.Where(m => m.UserId == user.Id).FirstOrDefault();
                        if (EmployeePromotions != null) {
                            usermodel.Designation = EmployeePromotions.Designation;
                            usermodel.ProjectLocation = EmployeePromotions.Location;
                        }

                        var islive = db.EmployeeLocations.Where(m => m.UserId == user.Id).OrderByDescending(m => m.Id).FirstOrDefault();
                        if (islive != null) {
                            var timegap = DateTime.Now.Minute - islive.Date.Minute;
                            if (timegap < 20) {
                                usermodel.LocationStatus = "In";
                            } else {
                                usermodel.LocationStatus = "Out";
                            }
                        } else {
                            usermodel.LocationStatus = "Out";
                        }
                        List<LocationModel> lst = new List<LocationModel>();
                        var location = db.EmployeeLocations.Where(m => m.UserId == user.Id && (m.Date >= startdate && m.Date <= enddate )).ToList();// m.Date >= startdate && m.Date <= enddate
                        foreach (var item in location) {  
                            LocationModel lcm = new LocationModel();
                            lcm.lati = item.Latitude;
                            lcm.longi = item.Longitude;
                            lcm.Description = item.MapLocation;
                            lst.Add(lcm);
                            usermodel.LocationList = lst;
                             
                        }
                        var latlocation = db.EmployeeLocations.Where(m => m.UserId == user.Id && (m.Date >= startdate && m.Date <= enddate)).OrderByDescending(m=>m.Id).FirstOrDefault();
                        if (latlocation != null) {
                            usermodel.LastLocationDate = latlocation.Date.ToString("MM/dd/yy HH:mm tt");
                        }

                        if (user.Image != null) {
                            string path = AppDomain.CurrentDomain.BaseDirectory;
                            String p = user.Image.Replace("/", "\\");
                           try {
                                usermodel.profileImage = "data:image/" + Path.GetExtension(path + p).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(path + p));
                            } catch {
                                usermodel.profileImage = "images/av1.png";
                            }
                        } else {
                            usermodel.profileImage = "images/av1.png";
                        }
                        return usermodel;
                    } else {
                        return null;
                    }
                }
            } catch (Exception ex) {
                return null;
            }
        }

        public bool AddSimLocation(List<RootObject> model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    List<EmployeeLocation> lst = new List<Models.EmployeeLocation>();

                    foreach(var item in model)
                    {
                        double lon = Convert.ToDouble(item.LONGITUDE);
                        double lat = Convert.ToDouble(item.LATITUDE);

                        var timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");
                        var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);

                        lst.Add(new Models.EmployeeLocation { UserId = item.User, Longitude = lon, Latitude = lat, Date = dateTime, MapLocation = item.SITE_NAME});
                    }

                    db.EmployeeLocations.AddRange(lst);
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