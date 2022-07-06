using EmployeeTracking.App_Codes;
using EmployeeTracking.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EmployeeTracking.Controllers {
    public class trackerController : ApiController {
        [HttpGet]
        public List<EmployeeTracking.Models.EmployeeAward> GetAwards(string UserId) {
            _usermanager um = new _usermanager();
            return um.GetEmployeeAwards(UserId);
        }

        [HttpGet]
        public EmployeeTracking.Models.EmployeeAward GetAward(string AwardId) {
            _usermanager um = new _usermanager();
            return um.GetEmployeeAward(Convert.ToInt32(AwardId));
        }

        [HttpGet]
        public List<EmployeeTracking.Models.EmployeePromotion> GetPromotions(string UserId) {
            _usermanager um = new _usermanager();
            return um.GetEmployeePromotion(UserId);
        }

        [HttpGet]
        public EmployeeTracking.Models.EmployeePromotion GetPromotion(string PromotionId) {
            _usermanager um = new _usermanager();
            return um.GetEmployeePromotion(Convert.ToInt32(PromotionId));
        }

        [HttpGet]
        public List<EmployeeTracking.Models.PastExperiance> GetExperiances(string UserId) {
            _usermanager um = new _usermanager();
            return um.GetEmployeeExperiance(UserId);
        }

        [HttpGet]
        public EmployeeTracking.Models.PastExperiance GetExperiance(string ExperianceId) {
            _usermanager um = new _usermanager();
            return um.GetEmployeeExperiance(Convert.ToInt32(ExperianceId));
        }

        [HttpPost]
        public bool AddAwards(EmployeeTracking.Models.EmployeeAward model) {
            _usermanager um = new _usermanager();
            return um.AddEmployeeAwards(model);
        }

        [HttpPost]
        public bool AddExperiance(EmployeeTracking.Models.PastExperiance model) {
            _usermanager um = new _usermanager();
            return um.AddEmployeePastExperiance(model);
        }

        [HttpPost]
        public bool AddPromotions(EmployeeTracking.Models.EmployeePromotion model) {
            _usermanager um = new _usermanager();
            return um.AddEmployeePromotions(model);
        }

        [HttpGet]
        public List<EmployeeTracking.Models.UserModulesViewModel> GetModules(string LevelId) {
            _usermanager um = new _usermanager();
            return um.GetUserModule(LevelId);
        }

        [HttpGet]
        public UserRoleViewModels GetRole(string LevelId) {
            _usermanager um = new _usermanager();
            return um.GetRole(LevelId);
        }

        [HttpPost]
        public bool ChangeModule(string RoleId, string Module) {
            _usermanager um = new _usermanager();

            if (um.ChangeRole(RoleId, Convert.ToInt32(Module))) {
                return true;
            } else {
                return false;
            }
        }

        [HttpGet]
        public CompaniesViewModel GetCompany(string CompanyId) {
            _companies um = new _companies();
            return um.GetCompany(Convert.ToInt32(CompanyId));
        }

        [HttpGet]
        public ProjectsViewModel GetDivision(string DivisionId) {
            _projects um = new _projects();
            return um.GetProject(Convert.ToInt32(DivisionId));
        }

        [HttpGet]
        public List<Project> GetDivisions(string DivisionId) {
            _projects um = new _projects();
            return um.GetProjects();
        }

        [HttpGet]
        public TeamsViewModel GetTeam(string TeamId) {
            _projects um = new _projects();
            return um.GetTeam(Convert.ToInt32(TeamId));
        }

        [HttpGet]
        public List<ProjectUsersViewModels> GetProjectUsers(string ProjectId, string EmployeeId = "", string search = "") {
            _projects um = new _projects();
            return um.GetProjectUsers(Convert.ToInt32(ProjectId), EmployeeId, search);
        }

        [HttpPost]
        public MobileUserModel MobileLogin(string Username, string Password) {
            _usermanager um = new _usermanager();
            return um.MobileLogin(Username, Password);
        }

        [HttpGet]
        public List<Meeting> GetUserMeetings(string UserId) {
            _meetings um = new _meetings();
            return um.GetMeetingsForUser(UserId);
        }

        [HttpPost]
        public bool AddMeetings(Meeting model) {
            _meetings um = new _meetings();
            return um.AddMeeting(model);
        }

        [HttpPost]
        public bool UpdateMeetings(string MeetingId, Meeting model) {
            _meetings um = new _meetings();
            model.Id = Convert.ToInt32(MeetingId);
            return um.UpdateMeeting(model);
        }

        [HttpGet]
        public List<UserRoleViewModels> GetRolesBelow(string UserId) {
            _usermanager um = new _usermanager();
            return um.GetRolesBelow(UserId);
        }

        [HttpGet]
        public List<UserProfile> GetRoleUsers(string RoleId) {
            _usermanager um = new _usermanager();
            return um.GetRoleUsers(RoleId);
        }

        [HttpGet]
        public EmployeeViewModel GetEmployeesBelow(string search, string userId) {
            _usermanager um = new _usermanager();
            return um.GetEmployeesBelow(search, userId);
        }

        [HttpGet]
        public List<messageusers> GetRoleByUsers(string RoleId) {
            return new _message().GetRoleUsers(RoleId);
        }

        [HttpGet]
        public List<messageusers> GetRoleUsersSearch(String Name) {
            return new _message().GetRoleUsersSearchByName(Name);
        }

        [HttpGet]
        public List<messageusers> GetAllUsers(String UserId) {
            return new _message().GetAllUsers(UserId);
        }
        [HttpGet]
        public List<messageusers> GetrolSearchUsers(String UserId, String Name) {
            return new _message().GetSearchUsers(UserId, Name);
        }

        [HttpPost]
        public Boolean SendGroupMsg(String Mesg, String UserList, String From) {
            return new _message().SendGroupMsgs(Mesg, UserList, From);
        }


        [HttpGet]
        public string GetLocationHistory(string from, string to, string Users) {
            String[] userarray = Users.Split(',');
            _usermanager um = new _usermanager();
            var lst = um.LocationHistory(Convert.ToDateTime(from), Convert.ToDateTime(to), userarray);
            return LocationExcel(lst, from, to);
        }


        [HttpPost]
        public int UploadFile(String UserId, String Title, String Description, String Venue) {
            try {
                if (HttpContext.Current.Request.Files.AllKeys.Any()) {
                    var files = HttpContext.Current.Request.Files;
                    String NewsFolder = "News" + DateTime.Now.Ticks + Guid.NewGuid();
                    bool exists = System.IO.Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath("~/Content/NewsUpload/" + NewsFolder)));
                    Boolean isFolderCreate = false;
                    if (!exists) {
                        System.IO.Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath("~/Content/NewsUpload/" + NewsFolder)));
                        isFolderCreate = true;
                    } else {
                        isFolderCreate = true;
                    }
                    if (isFolderCreate) {
                        if (files != null) {
                            for (int i = 0; i < files.Count; i++) {
                                string imagename = "UploadedImage" + i;
                                var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/NewsUpload/" + NewsFolder), "NewsImg" + i + ".jpg");
                                try {
                                    files[imagename].SaveAs(fileSavePath);
                                } catch (Exception ex) {

                                }
                            }
                            using (ApplicationDbContext db = new ApplicationDbContext()) {
                                News ns = new News();
                                ns.FolderPath = "~/Content/NewsUpload/" + NewsFolder;
                                ns.Date = DateTime.Now;
                                ns.UserId = UserId;
                                ns.Description = Description;
                                ns.Title = Title;
                                ns.Venu = Venue;
                                ns.LikeCount = 0;
                                db.News.Add(ns);
                                db.SaveChanges();
                                new _notifications().AddNotifications(UserId, ns.Id, "news");
                                return ns.Id;
                            }

                        } else {
                            return 0;
                        }
                    } else {
                        //not saved
                        return 0;
                    }

                } else {
                    return 0;
                }
            } catch (Exception ex) {
                return 0;
            }
        }

        [HttpGet]
        public List<News> GetIncidentImages(int CurrentNewsCount, String UserId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var newslst = db.News.OrderBy(m => m.Id).ToList().Skip(CurrentNewsCount * 5).Take(5).ToList();
                    List<News> newsList = new List<News>();
                    foreach (var item in newslst) {
                        News nw = new News();
                        nw.Id = item.Id;
                        nw.DateStr = item.Date.ToString("dd/MM/yy");
                        nw.Date = item.Date;
                        nw.Title = item.Title;
                        nw.Description = item.Description;
                        nw.Venu = item.Venu;
                        nw.LikeCount = item.LikeCount;
                        nw.UserId = item.UserId;
                        var islike = db.NewsLikes.Where(m => m.NewsId == item.Id && m.UserId == UserId).FirstOrDefault();
                        if (islike != null) {
                            nw.isLike = "like";
                        } else {
                            nw.isLike = "unlike";
                        }

                        List<imagesliest> imglst = new List<imagesliest>();
                        DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(item.FolderPath));
                        FileInfo[] rgFiles = di.GetFiles();
                        if (rgFiles != null) {
                            int imagecout = 0;
                            foreach (var imageitem in rgFiles) {
                                if (imagecout < 3) {
                                    imagecout++;
                                    if (imageitem.Length > 0) {
                                        byte[] img = File.ReadAllBytes(imageitem.FullName);
                                        String imagestr = Convert.ToBase64String(img);
                                        imagesliest imgstr = new imagesliest();
                                        imgstr.Imageb64 = imagestr;
                                        imglst.Add(imgstr);
                                    }
                                }

                            }
                            nw.Image64list = imglst;
                        }
                        newsList.Add(nw);
                    }
                    return newsList.OrderBy(m=>m.Id).ToList();
                }
            } catch (Exception ex) {
                return null;
            }
        }

        [HttpPost]
        public Boolean Like(String UserId, int NewsId) {
            return new _news().Like(UserId, NewsId);
        }

        [HttpGet]
        public News LoadImegebyId(int NewsId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var newsmodel = db.News.Where(m => m.Id == NewsId).FirstOrDefault();
                    if (newsmodel != null) {
                        newsmodel.DateStr = newsmodel.Date.ToString("dd/MM/yyyy");
                        List<imagesliest> imglst = new List<imagesliest>();
                        DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(newsmodel.FolderPath));
                        FileInfo[] rgFiles = di.GetFiles();
                        if (rgFiles != null) {
                            int imagecout = 0;
                            foreach (var imageitem in rgFiles) {

                                imagecout++;
                                if (imageitem.Length > 0) {
                                    byte[] img = File.ReadAllBytes(imageitem.FullName);
                                    String imagestr = Convert.ToBase64String(img);
                                    imagesliest imgstr = new imagesliest();
                                    imgstr.Imageb64 = imagestr;
                                    imgstr.ImageName = imageitem.Name;
                                    imglst.Add(imgstr);
                                }

                            }
                            newsmodel.Image64list = imglst;
                        }
                    }
                    return newsmodel;
                }
            } catch (Exception ex) {
                return null;
            }
        }


        private string LocationExcel(List<EmployeeLocation> lst, string from, string to) {
            _usermanager um = new _usermanager();
            var fileName = "location_history_" + DateTime.Now.Ticks + ".xlsx";
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/temp_files"), fileName);
            string DirectoryPath = HttpContext.Current.Server.MapPath("\\temp_files");
            string UrlPath = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/temp_files/" + fileName;

            int intSheetIndex = 1;
            int intRowNumber = 3;
            int intColumnNumber = 1;

            if (!Directory.Exists(DirectoryPath)) {
                Directory.CreateDirectory(DirectoryPath);
            }

            System.IO.FileInfo objFile = new System.IO.FileInfo(path);
            ExcelPackage exlPac = new ExcelPackage(objFile);

            exlPac.Workbook.Worksheets.Add("History");
            ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

            exlWs.Row(1).Style.Font.Bold = true;
            exlWs.Row(2).Style.Font.Bold = true;
            exlWs.Column(1).Width = 30;
            exlWs.Column(2).Width = 50;
            exlWs.Column(3).Width = 50;

            exlWs.Cells[1, 1].Value = "Date From : " + from + " - Date To : " + to;
            exlWs.Cells[2, 1].Value = "Employee Name";
            exlWs.Cells[2, 2].Value = "Date";
            exlWs.Cells[2, 3].Value = "Location";

            foreach (var item in lst) {
                exlWs.Cells[intRowNumber, intColumnNumber].Value = um.GetUserProfile(item.UserId.ToString()).FirstName;
                intColumnNumber += 1;

                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Date.ToLongDateString();
                intColumnNumber += 1;

                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.MapLocation;
                intColumnNumber += 1;

                intRowNumber++;
                intColumnNumber = 1;
            }

            exlPac.Save();

            return UrlPath;
        }

        [HttpPost]
        public String SendMsg(Messages msg) {
            return new _message().SendMsg(msg);
        }
        [HttpGet]
        public List<Messages> LoadMsg(String user, string receip) {
            return new _message().GetMessageList(user, receip);
        }

        [HttpPost]
        public String CreateMeeting(String CreateBy, DateTime SatartDate, DateTime EndDate, String Venue, String Message, String Employees) {
            return new _meetings().CreatenewMeeting(CreateBy, SatartDate, EndDate, Venue, Message, Employees);
        }

        [HttpPost]
        public String UpdateMeeting(int MeetingId, String CreateBy, DateTime SatartDate, DateTime EndDate, String Venue, String Message, String Employees) {
            return new _meetings().UpdateMeeting(MeetingId, CreateBy, SatartDate, EndDate, Venue, Message, Employees);
        }

        [HttpGet]
        public List<Meetings> GetMeetingList(String userId) {
            return new _meetings().GetMeetings(userId);
        }

        [HttpPost]
        public String AcceptMeetings(int id) {
            return new _meetings().AcceptMeeting(id);
        }

        [HttpPost]
        public String RejectMeetings(int id) {
            return new _meetings().RejectMeeting(id);
        }

        [HttpPost]
        public String CencelMeetings(int Meetingid) {
            return new _meetings().CancelMeeting(Meetingid);
        }

        [HttpGet]
        public List<Meetings> ViewMeetings(int Meetingid) {
            return new _meetings().ViewMeeting(Meetingid);
        }

        [HttpGet]
        public Meetings ViewSingleMeeting(int Meetingid) {
            return new _meetings().ViewsigleMeeting(Meetingid);
        }

        [HttpGet]
        public List<ProjectsViewModel> GetProject() {
            _projects um = new _projects();
            return um.GetAllProjects();
        }

        [HttpPost]
        public String SaveMobileLocation(EmployeeLocation location) {
            return new _location().SaveMbileLocation(location);
        }

        [HttpGet]
        public List<messageusers> GetUsersByProject(int ProjectId) {
            return new _projects().GetUsersBy(ProjectId);
        }

        [HttpGet]
        public List<messageusers> SearchUsersByProject(int ProjectId, String UserName, String MyUserId) {
            return new _projects().SearchUsersBy(UserName, ProjectId, MyUserId);
        }

        [HttpGet]
        public List<messageusers> SearchUsers(String UserName, String MyUserId) {
            return new _location().SearchUsersBy(UserName, MyUserId);
        }

        [HttpGet]
        public LocationUserModel GetSinleUserLocation(String UserId) {
            return new _location().GetLocationByuserid(UserId);
        }

        [HttpGet]
        public LocationUserModel GetSinleUserHistory(String UserId, DateTime startdate, DateTime enddate) {
            return new _location().GetLocationHistoryByuserid(startdate, enddate, UserId);
        }

        [HttpGet]
        public mobileprofile GetUserProfile(String UserId) {
            return new _profile().GetUser(UserId);
        }

        [HttpGet]
        public List<EmployeeTracking.Models.UserModulesViewModel> GetmobileModules(string UserId) {
            _usermanager um = new _usermanager();
            return um.GetMobileUserModule(UserId);
        }

        [HttpGet]
        public List<messageusers> GetSearchProUser(String UserId, string UserName) {
            return new _profile().GetSearchUsers(UserId, UserName);
        }

        //Change LONG LAT
        [HttpPost]
        public bool GetSimLocations(List<RootObject> model) {
            _location lc = new _location();

            if (!lc.AddSimLocation(model)) {
                return false;
            } else {
                return true;
            }

        }

        [HttpGet]
        public List<UserLocationsViewModel> GetSimLocationUsers(string key) {
            _usermanager um = new _usermanager();
            return um.GetUsersForLocator(key);
        }

        [HttpGet]
        public List<UserProfile> SearchUser(string RoleId, string Search, string SearchType = "") {
            _usermanager um = new _usermanager();
            return um.SearchUsers(RoleId, Search, SearchType);
        }

        [HttpGet]
        public List<UserProfile> SearchUser(string Search) {
            _usermanager um = new _usermanager();
            return um.SearchUsers(Search);
        }

        [HttpPost]
        public String SendImmidiatetomsg(String UserId, String Msg) {
            return new _message().SendMessagesToImmed(UserId, Msg);
        }

        [HttpGet]
        public List<Messages> GetallMsg(String UserId) {
            return new _message().GetMessageallList(UserId);
        }

        [HttpGet]
        public List<NotificationViewModel> NotificationCount(string UserId) {
            _notifications not = new App_Codes._notifications();
            return not.GetNotifications(UserId);
        }

        [HttpGet]
        public bool NotificationStatus(string UserId, string Type) {
            _notifications not = new App_Codes._notifications();
            return not.UpdateNotifications(UserId, Type);
        }

        [HttpGet]
        public List<MsgThread> MsgThread(string UserId, string From) {
            _message not = new App_Codes._message();
            return not.GetMsgThread(UserId, From);
        }

        [HttpGet]
        public List<MsgIn> MsgInbox(string UserId) {
            _message not = new App_Codes._message();
            return not.GetMsgIn(UserId);
        }

        [HttpGet]
        public List<UserLeavesViewModel> GetUserLeaves(string UserId, int Year) {
            _leavs lv = new App_Codes._leavs();
            _usermanager um = new App_Codes._usermanager();
            var users = um.GetUsers();
            var lvl = lv.GetUserLeaves(UserId, Year);
            

            return lvl;
        }

        [HttpPost]
        public string RequestLeaves(LeaveHistory model) {
            _leavs lv = new App_Codes._leavs();
            return lv.RequestLeave(model);
        }

        [HttpPost]
        public bool ApproveLeaves(LeaveHistoryViewModel model) {
            _leavs lv = new App_Codes._leavs();
            return lv.ApproveLeave(model);
        }

        [HttpGet]
        public LeaveType GetLeaveType(int LeaveId) {
            _leavs lv = new App_Codes._leavs();
            return lv.GetLeaveType(LeaveId);
        }

        [HttpGet]
        public List<UserLevelLeaves> GetLevelLeave(string UserRole) {
            _leavs lv = new App_Codes._leavs();
            return lv.GetUserLevelLeaves(UserRole);
        }

        [HttpGet]
        public List<UserLeavesViewModel> GetUsersLeave(string UserId, int Year) {
            _leavs lv = new App_Codes._leavs();
            return lv.GetUserLeaves(UserId, Year);
        }

        [HttpPost]
        public Boolean SaveNewOneSignalUser(String UserId, String OnesignalId) {
            return new _onesignal().SaveNewUser(UserId, OnesignalId);
        }

        [HttpPost]
        public Boolean RemoveOneSignalUser(String UserId, String OnesignalId) {
            return new _onesignal().Removeuser(UserId, OnesignalId);
        }

        [HttpGet]
        public List<LeaveType> GetLeavTypes() {
            return new _leavs().GetLeaves();
        }

        [HttpGet]
        public List<messageusers> GetAboveUsers(String UserId) {
            return new _leavs().GetAboveUsers(UserId);
        }

        [HttpGet]
        public List<messageusers> GetAboveUsers(string userId, string search) {
            return new _leavs().GetAboveUsers(userId, search);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////


        [HttpGet]
        public List<messageusers> GetDivision(string userId, string search)
        {
            return new _leavs().GetDivision(userId, search);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////


        [HttpGet]
        public List<LeaveHistory> getUsersLeaves(String UserId) {
            return new _leavs().GetUserRequestLeave(UserId);
        }

        [HttpGet]
        public string CheckUserLeaves(string userId, int leaveType, string unit, double days, DateTime date) {
            _leavs lv = new App_Codes._leavs();
            return lv.CheckUserLeaves(userId, leaveType, unit, days, date);
        }

        [HttpGet]
        public List<LeaveHistory> GetUserLeaveRequest(String UserId) {
            return new _leavs().GetUserLeaveRequest(UserId);
        }

        [HttpGet]
        public String testapi() {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var lst = (from ones in db.OneSignal
                               join user in db.Users
                               on ones.UserId equals user.Id
                               select new { ones.OnesignalId }).ToList();
                    String[] df = new String[lst.Count];
                    for (int i = 0; i < lst.Count; i++) {
                        df[i] = lst[i].OnesignalId;
                    }
                    new _onesignal().PushNotification(df, "", "You Have New News");
                    return df[0];
                }
            } catch (Exception ex) {
                return ex.InnerException.ToString();
            }
            // String[] pid = { "d050372c-363e-47b8-835f-3da657db94a1", "d050372c-363e-47b8-835f-3da657db94a1" };

            //new _onesignal().PushNotification(pid, "", "You Have New Meaage");
        }

        [HttpPost]
        public bool ChangePassword(string UserId, string CurrentPassword, string NewPassword) {
            _usermanager um = new App_Codes._usermanager();

            if (um.ChangePassword(UserId, CurrentPassword, NewPassword)) {
                return true;
            } else {
                return false;
            }
        }

        [HttpGet]
        public List<UserProfile> GetAllUsers(int pageIndex, int pageSize) {
            _usermanager um = new App_Codes._usermanager();
            return um.GetAllUsers(pageIndex, pageSize);
        }

        [HttpPost]
        public int EditNews(String UserId, String Title, String Description, String Venue, String DeleteImageList, int newid) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var existnews = db.News.Where(m => m.Id == newid).FirstOrDefault();
                    if (existnews != null) {
                        existnews.UpdateOn = DateTime.Now;
                        existnews.UpdateBy = UserId;
                        existnews.Description = Description;
                        existnews.Title = Title;
                        existnews.Venu = Venue;
                        db.SaveChanges();
                        if (DeleteImageList != null) {
                            String[] deletefiimagelist = DeleteImageList.Split(',');
                            foreach (var diemge in deletefiimagelist) {
                                var filePath = HttpContext.Current.Server.MapPath(existnews.FolderPath + "/" + diemge);
                                if (File.Exists(filePath)) {
                                    File.Delete(filePath);
                                }
                            }
                        }

                        if (HttpContext.Current.Request.Files.AllKeys.Any()) {
                            var files = HttpContext.Current.Request.Files;
                            if (files != null) {
                                System.IO.DirectoryInfo ndir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath(existnews.FolderPath));
                                int imagecount = ndir.GetFiles().Length;
                                for (int i = 0; i < files.Count; i++) {
                                    string imagename = "UploadedImage" + i;
                                    var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath(existnews.FolderPath), "NewsImg" + i + "_" + (DateTime.Now.Ticks) + ".jpg");
                                    try {
                                        files[imagename].SaveAs(fileSavePath);
                                    } catch (Exception ex) {
                                        Console.WriteLine(ex);
                                    }
                                }
                            }
                        }
                        return 1;
                    } else {
                        return 0;//news does't exist
                    }
                }

            } catch (Exception ex) {
                return 0;
            }
        }

        [HttpPost]
        public int DeleteNews(int newid) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var existnews = db.News.Where(m => m.Id == newid).FirstOrDefault();
                    if (existnews != null) {
                        var newslike = db.NewsLikes.Where(m => m.NewsId == newid).ToList();
                        db.NewsLikes.RemoveRange(newslike);
                        db.SaveChanges();
                        db.News.Remove(existnews);
                        db.SaveChanges();
                        System.IO.DirectoryInfo ndir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath(existnews.FolderPath));
                        try {
                            ndir.Delete();
                        } catch (Exception ex) {

                        }
                        return 1;
                    } else {
                        return 0;
                    }
                }
            } catch (Exception ex) {
                return -1;
            }
        }


        [HttpGet]
        public List<LeaveHistory> GetleaveHistory(String userId) {
            _leavs lv = new App_Codes._leavs();
            var leaves = lv.GetLeaveHistory(userId, "0", DateTime.Now.Year);
            List<LeaveHistory> retrnlist = new List<LeaveHistory>();
            var cult = System.Globalization.CultureInfo.CurrentCulture;
            foreach (var item in leaves) {
                LeaveHistory model = new LeaveHistory();
                model.Id = item.Id;
                model.LeaveTypeStr = item.LeaveTypeStr;
                model.ReqDateStr = item.RequestDate.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(item.RequestDate.Month);
                model.LeaveUnit = item.LeaveUnit;
                model.FromDate = item.FromDate;
                model.FromDateStr = item.FromDate.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(item.FromDate.Month);
                model.ToDateStr =  item.ToDate.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(item.ToDate.Month);
                model.LeaveDays = item.LeaveDays;
                model.Remarks = item.Remarks; 
                model.Reason = item.Reason;
                model.IsApproved = item.IsApproved;
                model.ApprovedBy = item.ApprovedBy;               
                retrnlist.Add(model);
            }
          
            var rejwcted_list = lv.GetRejectedLeaves(userId);           
            foreach (var item in rejwcted_list) {
                var dtFrom = Convert.ToDateTime(item.DateFrom);
                var dtTo = Convert.ToDateTime(item.DateTo);

                LeaveHistory model = new LeaveHistory();
                model.Id = item.Id;
                model.LeaveTypeStr = item.LeaveType;
                model.ReqDateStr = item.RequestedOn;
                model.LeaveUnit = item.Unit;             
                model.FromDateStr = dtFrom.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(dtFrom.Month);
                model.ToDateStr = dtTo.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(dtTo.Month);
                model.LeaveDays = Convert.ToDouble(item.Days);
                model.Remarks = item.Remarks;              
                model.Reason = item.Reason;
                model.IsApproved = item.IsApproved;
                model.ApprovedBy = item.RejectedBy;

                retrnlist.Add(model);
            }
            //retrnlist = retrnlist.OrderByDescending(m => m.FromDate).ToList();

            return retrnlist.OrderByDescending(m => m.FromDate).ToList(); ;
        }

        [HttpGet]
        public String CancelLeave(String Leaveid) {
            _leavs lv = new _leavs();
            var leaveId = Convert.ToInt32(Leaveid);
            if (lv.CancelUserLeave(leaveId)) {
                return "Leave Request Canceled";
            } else {
                return "Leave Request Could Not Be Canceled";
            }
        }

        [HttpPost]
        public String SaveLeave(LeaveHistory model) {
            _leavs lv = new App_Codes._leavs();
            
            if (model.LeaveUnit == "SHORT") {
                model.LeaveType = 0;
            } else {
                model.LeaveType = model.LeaveType;
            }
            if (model.LeaveUnit != "FULL") {
                model.ToDate = model.FromDate;
            }

            var msg = lv.RequestLeave(model);
            return msg;
            //if (lv.RequestLeave(model)) {
            //    return "Leave Request Sent";
            //} else {
            //    return "Leave Request Could Not Be Sent";
            //}

        }

        [HttpGet]
        public List<LeaveHistory> GetLeaveRequest(String UserId) {
            _leavs lv = new _leavs();
            return lv.GetUserLeaveRequest(UserId);
        }

        [HttpPost]
        public String ForwardLeave(int leaveId, String forwardto) {
            _leavs lv = new _leavs();

            if (lv.ForwardLeave(leaveId, forwardto)) {
                return "Leave Request Submitted For Optional Approval";
            } else {
                return "Leave Request Could Not Be Submit";
            }
        }

        [HttpPost]
        public String InOutRequest(AttendenceCorrections model) {
            _attendence att = new App_Codes._attendence();
            if (att.SaveInOutCorrections(model)) {
                return "Attendence Correction Request Sent";
            } else {
                return "Request Could Not Be Sent";
            }
        }

        [HttpGet]
        public List<AttendenceCorrections> GetInout(String UserId) {
            var cult = System.Globalization.CultureInfo.CurrentCulture;
            _attendence att = new App_Codes._attendence();
            var lst = att.GetMyInOutRequests(UserId);
            lst.Select(c => { c.Datestr = c.DateRequested.Date.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(c.DateRequested.Date.Month);
                        c.Intimestr = (c.InTime.ToString("hh:mm tt")== "12:00 AM" ? "-": c.InTime.ToString("hh:mm tt"));
                        c.OutTimeStr = (c.OutTime.ToString("hh:mm tt")=="12:00 AM")? "-": c.OutTime.ToString("hh:mm tt");
                        c.InOutDate = c.Date.ToString("yyyy-MM-dd"); return c; }).ToList().OrderBy(m=>m.DateRequested).ToList();
            return lst;
        }

        [HttpGet]
        public List<AttendenceCorrections> GetInoutApproval(String UserId) {
            _attendence att = new App_Codes._attendence();
            var lst = att.GetUserInOutRequests(UserId);
            var cult = System.Globalization.CultureInfo.CurrentCulture;
                        
            lst.Select(c => {
                c.Datestr = c.DateRequested.Date.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(c.DateRequested.Date.Month);
                if(c.InTime.TimeOfDay.Ticks != 0)
                {
                    c.Intimestr = c.InTime.ToString("hh:mm tt");
                }
                else
                {
                    c.Intimestr = "-";
                }

                if (c.OutTime.TimeOfDay.Ticks != 0)
                {
                    c.OutTimeStr = c.OutTime.ToString("hh:mm tt");
                }
                else
                {
                    c.OutTimeStr = "-";
                }

                c.InOutDate = c.Date.ToString("yyyy-MM-dd");


                return c;
            }).ToList().OrderBy(m => m.DateRequested).ToList();
            return lst;
        }

        [HttpPost]
        public String AcceptRejectInOut(ApproveInOutViewModel model) {
            _attendence att = new App_Codes._attendence();
            if (att.ApproveInOut(model)) {
                if (model.IsApproved == "true") {
                   
                    return "Attendence Correction Request Approved";
                } else if (model.IsApproved == "false") {
                    return "Attendence Correction Request Rejected";
                }else {
                    return "Attendence Correction Request Canceled";
                }

            } else {
                return "Request Could Not Be Approved";
            }
        }

        [HttpGet]
        public List<ProjectsViewModel> GetDevisionList() {
            EmployeeTracking.App_Codes._projects pr = new EmployeeTracking.App_Codes._projects();
            List<ProjectsViewModel> lst = new List<ProjectsViewModel>();
            foreach (var item in pr.GetProjects()) {
                ProjectsViewModel mdl = new ProjectsViewModel();
                mdl.Id = item.Id;
                mdl.ProjectName = item.ProjectName;
                lst.Add(mdl);
            }
            return lst;
        }
        [HttpPost]
        public String RequestTransfer(EmployeeTransfer model) {

            _transfers tr = new _transfers();
            if (tr.NewTransfer(model)) {
                return "Transfer Request Sent";
            } else {
                return "Transfer Request Could not be Sent";
            }
        }
        [HttpGet]
        public List<EmployeeTransfer> TransferList(String UserId) {
            _transfers tr = new _transfers();
            return tr.GetTransferRequests(UserId);
        }

        [HttpGet]
        public List<EmployeeTransfer> GetAppTransferList(String UserId) {
            _transfers tr = new _transfers();
            return tr.GetRequestsToApprove(UserId);
        }

        [HttpPost]
        public String ApproveRejectTrans(ApproveInOutViewModel appmodel) {
            _transfers tr = new App_Codes._transfers();
            TransferApprovalViewModel model = new Models.TransferApprovalViewModel();
            model.RequestId = appmodel.Id;

            model.Approval = appmodel.IsApproved;
            if (appmodel.IsApproved.Equals("false")) {
                model.Remarks = appmodel.Remarks;
            }

            if (tr.ApproveTransfer(model)) {
                if (model.Approval == "true") {
                    return "Transfer Request Approved";
                } else {
                    return "Transfer Request Rejected";
                }

            } else {
                return "Request Could Not Be Approved";
            }
        }

        [HttpGet]
        public List<LeaveHistory> LeaveHistory(String empId) {
            _leavs lv = new App_Codes._leavs();
            var cult = System.Globalization.CultureInfo.CurrentCulture;
            var lst= lv.GetLeaveHistory(empId, DateTime.Now.Year);
            lst.Select(m => { m.FromDateStr = m.FromDate.Date.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(m.FromDate.Date.Month);
                              m.ToDateStr = m.ToDate.Date.Day + "-" + cult.DateTimeFormat.GetAbbreviatedMonthName(m.ToDate.Date.Month); return m; }).ToList().OrderBy(m => m.RequestDate).ToList();
            return lst;
        }

        [HttpPost]
        public String RejectLeaveAdmin(int LeaveId,String Remark) {
            _leavs lv = new App_Codes._leavs();         
            string remark = "";

            if (!string.IsNullOrEmpty(Remark)) {
                remark = Remark;
            } else {
                remark = "No Remark";
            }

            if (lv.CancelUserLeave(LeaveId, "", remark)) {
                return "Leave Canceled.";
            } else {
                return "Leave Cannot be Canceled.";
            }
        }

        [HttpGet]
        public double ValidateWeekends(string userId, DateTime from, DateTime to)
        {
            var days = to.Subtract(from);
            _leavs lv = new _leavs();
            _holidays hl = new _holidays();

            bool IsOnSaturday = lv.IsWorkOnSaturday(userId);
            double daysCount = 0;

            //var days = hl.ActualDayCount(from, to);

            if (lv.IsSaturday(from, to))
            {
                daysCount = 0.5;
            }
            else
            {
                for (int i = 0; i <= days.Days; i++)
                {
                    var day = from.AddDays(i);
                    var IsHoliday = hl.IsHoliday(day);

                    if(!IsHoliday)
                    {
                        if (IsOnSaturday)
                        {
                            if (day.DayOfWeek == DayOfWeek.Saturday)
                            {
                                daysCount += 0.5;
                            }
                            else if (day.DayOfWeek != DayOfWeek.Sunday)
                            {
                                daysCount++;
                            }
                        }
                        else
                        {
                            if (day.DayOfWeek != DayOfWeek.Sunday && day.DayOfWeek != DayOfWeek.Saturday)
                            {
                                daysCount++;
                            }
                            //else
                            //{
                            //    daysCount--;
                            //}
                        }
                    }
                    

                }
            }
            

            return daysCount;
        }
    }


}