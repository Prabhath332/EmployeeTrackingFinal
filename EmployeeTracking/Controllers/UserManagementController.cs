using EmployeeTracking.App_Codes;
using EmployeeTracking.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace EmployeeTracking.Controllers
{
    //[Authorize]
    //[AdminAuthorization]
   // [SessionExpireFilter]
    public class UserManagementController : Controller
    {
        public ActionResult Index()
        {
            _usermanager um = new App_Codes._usermanager();
            ViewBag.erMsg = TempData["erMsg"];
            var userRole = Session["UserRole"] as string;
            if (userRole == "SuperAdmin")
            {
                return View(um.GetUsers());
            }
            else
            {
                return View(um.GetCompanyUsers(Convert.ToInt32(Session["CompanyId"].ToString())));
                //return View(um.GetUsers(Session["DivisionId"].ToString(), Session["UserId"].ToString()));
            }
            
        }

        public ActionResult UserProfile(string id)
        {
            _usermanager um = new App_Codes._usermanager();

            ViewBag.erMsg = TempData["erMsg"];

            if (id == null)
            {
                id = TempData["UserId"].ToString();
            }

            return View(um.GetUser(id));
        }
                
        public ActionResult MyProfile()
        {
            _usermanager um = new App_Codes._usermanager();

            //ViewBag.erMsg = TempData["erMsg"];

            //if (Session["UserId"] == null)
            //{
            //    id = TempData["UserId"].ToString();
            //}


            return View(um.GetUser(Session["UserId"].ToString()));
        }

        public ActionResult CreateNewUser()
        {
            _usermanager um = new _usermanager();
            _leavs lv = new App_Codes._leavs();

            string EpfNo = Request.Form["txtEpfNo"].ToString();
            string FirstName = Request.Form["txtFirstName"].ToString();
            string DivisionCode = Request.Form["ddlDivisionCode"].ToString();
            //string LastName = Request.Form["txtLastName"].ToString();
            string Role = Request.Form["ddlUserLevel"].ToString();

            var annualLeaves = Convert.ToDouble(Request.Form["txtAllocatedAnnual"].ToString());
            var cassualLeaves = Convert.ToDouble(Request.Form["txtAllocatedCassual"].ToString());

            var userId = um.AddNewUser(EpfNo, FirstName, Role, DivisionCode);

            if (userId != null)
            {
                ViewBag.erMsg = "successMsg('New User <b>" + FirstName + "</b> Created.')";
                
                List<UserLeaves> userLv = new List<UserLeaves>();
                userLv.Add(new UserLeaves { UserId = userId, LeaveType = 3000, AllocatedCount = annualLeaves, RemainingCount = annualLeaves, Year = Convert.ToInt32(DateTime.Now.Year) });
                userLv.Add(new UserLeaves { UserId = userId, LeaveType = 3001, AllocatedCount = cassualLeaves, RemainingCount = cassualLeaves, Year = Convert.ToInt32(DateTime.Now.Year) });

                lv.AddUserLeave(userLv);
            }
            else
            {
                ViewBag.erMsg = "errorMsg('User Could not be Created.')";
            }
            TempData["erMsg"] = ViewBag.erMsg;
            return RedirectToAction("UserProfile", new { id = userId});
        }
        
        private string ProccessUserFile(string FilePath)
        {
            string msg = "";

            using (ExcelPackage pck = new ExcelPackage())
            {
                try
                {
                    _projects pr = new _projects();
                    _usermanager um = new _usermanager();
                    //FilePath = Server.MapPath("~/temp_files/boo_new.xlsx");
                    //string original = FilePath;
                    //if (!original.StartsWith("http:"))
                    //{
                    //    original = "http://" + original;
                    //}
                        
                    //Uri uri;
                    //if (!Uri.TryCreate(original, UriKind.Absolute, out uri))
                    //{
                    //    //Bad bad bad!
                    //}

                    using (var stream = System.IO.File.OpenRead(FilePath))
                    {
                        pck.Load(stream);
                    }

                    ExcelWorksheet ws = pck.Workbook.Worksheets.First();
                    
                    int startRow = 3;
                    int totalCols = ws.Dimension.End.Column;
                    int totalRows = ws.Dimension.End.Row;

                    for (int rowNum = startRow; rowNum <= totalRows; rowNum++)
                    {
                        if(rowNum == 54)
                        {
                            var sd = "";
                        }
                        int colId = 1;

                        string FirstName = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Level = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string EmpId = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        if(!string.IsNullOrEmpty(EmpId))
                        {
                            if (EmpId.Contains(" ") || EmpId.Contains("_") || EmpId.Contains("-"))
                            {
                                EmpId = EmpId.Replace(" ", "").Replace("_", "").Replace("-", "");
                            }
                        }
                        else
                        {
                            //msg += "Empty Records Detected End of File";
                            break;
                        }
                        

                        string Designation = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Division = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        int DivisionId = 0;
                        var div = pr.GetProject(Convert.ToInt32(Division));
                        if (div != null)
                        {
                            DivisionId = div.Id;
                        }
                        else
                        {
                            if (msg == "")
                            {
                                msg += "The Following Records Excluded Due To Errors ==>";
                            }
                            msg += FirstName + " User's Division is Incorrect, ";
                        }

                        colId++;

                        string Section = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string divCode = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        int Age = Convert.ToInt32(ws.Cells[rowNum, colId].Value == null ? "0" : ws.Cells[rowNum, colId].Value.ToString().Substring(0, 2));
                        colId++;

                        string Gender = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Marital = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string NIC = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        DateTime DOB = DateTime.Now;
                        string dobstr = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        try
                        {
                            if(!string.IsNullOrEmpty(dobstr))
                            {
                                if(dobstr != "N/A")
                                {
                                    DOB = Convert.ToDateTime(dobstr);
                                }
                                
                            }                            
                        }
                        catch (Exception er)
                        {
                            msg += "Date of Birth : Incorrect Date Format, ";
                        }

                        colId++;

                        string Address = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string MobileNo = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string MobileAccount = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string FixedNo = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Email = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string SecnEdu = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string HighEdu = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string JobDes = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        DateTime AppoinDate = DateTime.Now;

                        try
                        {
                            var appDate = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();

                            if (!string.IsNullOrEmpty(appDate))
                            {
                                if(appDate != "N/A")
                                {
                                    AppoinDate = Convert.ToDateTime(ws.Cells[rowNum, colId].Value.ToString());
                                }
                                
                            }
                            
                        }
                        catch (Exception er)
                        {
                            msg += "Appointment Date : Incorrect Date Format, ";
                        }

                        colId++;

                        string SuperVisor = "";
                        string SupId = "";

                        try
                        {
                            SuperVisor = ws.Cells[rowNum, colId].Value.ToString();

                            if (SuperVisor == "")
                            {
                                if (msg == "")
                                {
                                    msg += "The Following Records Excluded Due To Errors ==>";
                                }

                                msg += FirstName + " User's Supervisor Name is Incorrect, ";
                            }
                            else
                            {
                                //SupId = um.GetUserIdByName(SuperVisor.Trim());
                                var _sup = um.GetUserByEmp(SuperVisor.Trim());
                                SupId = "";

                                if (_sup != null)
                                {
                                    SupId = _sup.Id;
                                }

                            }
                        }
                        catch (Exception er)
                        {

                        }

                        colId++;

                        string Location = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string WorkOnSaturday = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        bool wks = false;
                        if(WorkOnSaturday == "true")
                        {
                            wks = true;
                        }

                        colId++;

                        string remarks = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Username = EmpId;
                        var user = um.ApplicationUserProfile(EmpId);

                        if (DivisionId > 0)
                        {
                            string UserId = "";

                            if (user != null)
                            {
                                UserId = user.Id;

                                //var userRole = um.RoleManager.(user.Roles.FirstOrDefault().RoleId).Name
                            }
                            else
                            {
                                UserId = um.AddNewUser(EmpId, FirstName, Level, divCode);
                            }

                            if (UserId != null && !UserId.Contains("error_"))
                            {
                                EmployeeTracking.Models.UserProfile userpr = new Models.UserProfile
                                {
                                    Id = UserId,
                                    FirstName = FirstName,
                                    Age = Age,
                                    EmployeeId = EmpId,
                                    DateOfBirth = DOB,
                                    Gender = Gender,
                                    MaritalStatus = Marital,
                                    NICNo = NIC,
                                    Image = "Content/ProfileImages/" + EmpId + ".png",
                                    Remarks = remarks
                                };

                                um.UpdateUserProfile(UserId, Level, new UserProfileViewModel { UserProfiles = userpr });

                                ApplicationUser apu = new ApplicationUser();
                                apu.Id = UserId;
                                apu.MobileNumber = MobileNo;
                                apu.PhoneNumber = FixedNo;
                                apu.Email = Email;
                                apu.MobileAccount = MobileAccount;
                                apu.Address = Address;
                                um.UpdateContactInfo(new UserProfileViewModel { ApplicationUser = apu });

                                EducationalInfo edu = new EducationalInfo
                                {
                                    Id = UserId,
                                    Secondary = SecnEdu,
                                    Other = HighEdu,
                                };

                                um.UpdateEducationalInfo(UserId, new UserProfileViewModel { EducationalInfos = edu });

                                EmployeementInfo empl = new EmployeementInfo
                                {
                                    JobDescription = JobDes,
                                    SupervisorId = SupId,
                                    AppointmentDate = AppoinDate,
                                    Division = DivisionId,
                                    Section = Section,
                                    PresentReportingLocation = Location,
                                    Designation = Designation,
                                    WorkOnSaturday = wks,
                                    DivisionCode = divCode
                                };

                                um.UpdateEmployeement(UserId, new UserProfileViewModel { EmployeementInfos = empl });
                            }
                            else
                            {
                                msg += UserId.Replace("error_", "");                                                              
                            }

                        }



                    }

                    //for (int rowNum = startRow; rowNum <= totalRows; rowNum++)
                    //{
                    //    int colId = 1;

                    //    string FirstName = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                    //    colId++;

                    //    string Level = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                    //    colId++;

                    //    string EmpId = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                    //    colId++;

                    //    string Designation = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                    //    colId++;



                    //    string Username = EmpId;
                    //    var UserId = um.GetUsers().Where(x => x.EmployeeId == EmpId).FirstOrDefault();

                    //    if(UserId != null)
                    //    {
                    //        EmployeementInfo empl = new EmployeementInfo
                    //        {
                    //            Designation = Designation
                    //        };

                    //        um.UpdateEmployeement(UserId.Id, new UserProfileViewModel { EmployeementInfos = empl });
                    //    }



                    //}
                }
                catch(Exception er)
                {
                    msg = er.Message;
                }

            }

            if(msg == "")
            {
                TempData["FupError"] = "0";
            }
            else
            {
                TempData["FupError"] = msg;
            }
            
            return msg;
        }

        public ActionResult UpdateContactInfo(string UserId, UserProfileViewModel model)
        {
            model.ApplicationUser.Id = UserId;
            _usermanager um = new App_Codes._usermanager();
            if(um.UpdateContactInfo(model))
            {
                ViewBag.erMsg = "successMsg('Contact Details Updated')";
            }
            else
            {
                ViewBag.erMsg = "errorMsg('Contact Details Could not be Updated.')";
            }

            TempData["UserId"] = UserId;
            TempData["erMsg"] = ViewBag.erMsg;
            return RedirectToAction("UserProfile", new { id = UserId });
        }

        public ActionResult UpdateUserProfile(string UserId, UserProfileViewModel model, FormCollection collection)
        {
            _usermanager um = new App_Codes._usermanager();
            string RoleId = Request.Form["UserProfiles.UserRole"].ToString();
            
            if (um.UpdateUserProfile(UserId, RoleId, model))
            {
                ViewBag.erMsg = "successMsg('Profile Details Updated')";
            }
            else
            {
                ViewBag.erMsg = "errorMsg('Profile Details Could not be Updated.')";
            }

            TempData["UserId"] = UserId;
            TempData["erMsg"] = ViewBag.erMsg;
            return RedirectToAction("UserProfile", new { id = UserId });
        }

        public ActionResult UpdateEmployeementInfo(string UserId, UserProfileViewModel model)
        {
            _usermanager um = new App_Codes._usermanager();
            if (um.UpdateEmployeement(UserId, model))
            {
                ViewBag.erMsg = "successMsg('Employeement Details Updated')";
            }
            else
            {
                ViewBag.erMsg = "errorMsg('Employeement Details Could not be Updated.')";
            }

            TempData["UserId"] = UserId;
            TempData["erMsg"] = ViewBag.erMsg;
            return RedirectToAction("UserProfile", new { id = UserId });
        }

        public ActionResult UpdateEducationalInfo(string UserId, UserProfileViewModel model)
        {
            _usermanager um = new App_Codes._usermanager();
            if (um.UpdateEducationalInfo(UserId, model))
            {
                ViewBag.erMsg = "successMsg('Educational Details Updated')";
            }
            else
            {
                ViewBag.erMsg = "errorMsg('EducationalInfo Details Could not be Updated.')";
            }

            TempData["UserId"] = UserId;
            TempData["erMsg"] = ViewBag.erMsg;
            return RedirectToAction("UserProfile", new { id = UserId });
        }

        public ActionResult UpdateAwards(string UserId, UserProfileViewModel model)
        {
            _usermanager um = new App_Codes._usermanager();
            if (um.UpdateEducationalInfo(UserId, model))
            {
                ViewBag.erMsg = "successMsg('Educational Details Updated')";
            }
            else
            {
                ViewBag.erMsg = "errorMsg('EducationalInfo Details Could not be Updated.')";
            }

            TempData["UserId"] = UserId;
            TempData["erMsg"] = ViewBag.erMsg;
            return RedirectToAction("UserProfile", new { id = UserId });
        }

        public ActionResult UpdateUserImage(HttpPostedFileBase fupProfileImage, string UserId)
        {
            _usermanager um = new App_Codes._usermanager();

            if (fupProfileImage.ContentLength > 0)
            {
                var profile = um.GetUserProfile(UserId);
                var fileName = profile.EmployeeId + Path.GetExtension(fupProfileImage.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/ProfileImages"), fileName);
                var physicalPath = "Content/ProfileImages/" + fileName;

                if(!Directory.Exists(Server.MapPath("~/Content/ProfileImages")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/ProfileImages"));
                }

                fupProfileImage.SaveAs(path);

                if (um.UpdateProfileImage(UserId, physicalPath))
                {
                    ViewBag.erMsg = "successMsg('Profile Image Updated')";
                }
                else
                {
                    ViewBag.erMsg = "errorMsg('Profile Image Could not be Updated.')";
                }
            }


            TempData["UserId"] = UserId;
            TempData["erMsg"] = ViewBag.erMsg;
            return RedirectToAction("UserProfile", new { id = UserId});
        }

        public ActionResult UpdateUserFile(HttpPostedFileBase fupUserFile)
        {
            _usermanager um = new App_Codes._usermanager();

            if (fupUserFile.ContentLength > 0)
            {
                var fileName = fupUserFile.FileName;
                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                var physicalPath = "temp_files/" + fileName;

                if (!Directory.Exists(Server.MapPath("~/temp_files")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/temp_files"));
                }

                fupUserFile.SaveAs(path);

                var str = ProccessUserFile(path);
            }

            //TempData["erMsg"] = ViewBag.erMsg;            
            return RedirectToAction("Index");
        }

        public ActionResult NewUserLevel()
        {
            _usermanager um = new _usermanager();

            string RoleId = Request.Form["hfRoleId"].ToString();
            string UserRole = Request.Form["txtUserLevel"].ToString();
            int SortOrder = Convert.ToInt32(Request.Form["txtSortOrder"].ToString());
            bool canEdit = false;
            bool canViewAll = false;

            if (Request.Form.AllKeys.Contains("chkEdit"))
            {
                canEdit = true;
            }

            if(Request.Form.AllKeys.Contains("chkViewAll"))
            {
                canViewAll = true;
            }
            
            if(RoleId == "0")
            {
                if (um.AddNewRole(UserRole, SortOrder, canEdit, canViewAll))
                {
                    ViewBag.erMsg = "successMsg('New User Level <b>" + UserRole + "</b> Created.')";
                }
                else
                {
                    ViewBag.erMsg = "errorMsg('User Level Could not be Created.')";
                }

                TempData["erMsg"] = ViewBag.erMsg;
                return RedirectToAction("UserLevels");
            }
            else
            {
                if(um.UpdateRole(RoleId, UserRole, SortOrder, canEdit, canViewAll))
                {
                    ViewBag.erMsg = "successMsg('User Level <b>" + UserRole + "</b> Updated.')";
                }
                else
                {
                    ViewBag.erMsg = "errorMsg('User Level Could not be Updated.')";
                }

                TempData["erMsg"] = ViewBag.erMsg;
                return RedirectToAction("UserModules", new { id = RoleId });
            }
                        
        }

        public ActionResult UserLevels()
        {
            ViewBag.erMsg = TempData["erMsg"];
            
            return View();
        }

        public ActionResult UserModules(string id)
        {
            ViewBag.erMsg = TempData["erMsg"];

            _usermanager um = new _usermanager();

            UserAccessViewModel uma = new Models.UserAccessViewModel();
            uma.UserModules = um.GetUserModule(id);
            uma.UserRole = um.GetRole(id);

            return View(uma);
        }

        public ActionResult DeleteProfile()
        {
            string UserId = Request.Form["hfDelUser"].ToString();
            _usermanager um = new _usermanager();

            if (um.DeleteUser(UserId))
            {
                TempData["erMsg"] = "successMsg('User Removed from the System')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('User Could not be Deleted.')";
            }

            return RedirectToAction("Index");
        }

        public ActionResult Leaves()
        {
            ViewBag.erMsg = TempData["erMsg"];

            _leavs lv = new App_Codes._leavs();
            _holidays hl = new App_Codes._holidays();
            _usermanager um = new _usermanager();

            LeavesViewModel lvm = new Models.LeavesViewModel();

            lvm.LeaveTypes = lv.GetLeaves();
            lvm.UserLevelLeavs = lv.GetUserLevelLeaves();
            lvm.UserRoles = um.GetRoles().OrderBy(x => x.Name).ToList();
            lvm.UserLeaves = lv.GetUserLeaves(DateTime.Now.Year);
            lvm.UserProfile = um.GetUsers();
            lvm.HolidayDates = hl.GetHolidays(Year:DateTime.Now.Year);
            return View(lvm);
        }
        
        public ActionResult LeaveHistory()
        {
            ViewBag.erMsg = TempData["erMsg"];

            //if (!string.IsNullOrEmpty(TempData["empId"] as string))
            //{
            //    string userId = TempData["empId"].ToString();

            //    var col = Request.Form;
            //    var propInfo = col.GetType().GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
            //    propInfo.SetValue(col, false, new object[] { });
            //    col.Add("txtEmployeeId", userId);
            //}

            if (Request.Form.HasKeys())
            {
                _leavs lv = new App_Codes._leavs();
                var empId = Request.Form["txtEmployeeId"].ToString();

                //var col = Request.Form;
                //var propInfo = col.GetType().GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                //propInfo.SetValue(col, false, new object[] { });
                //col.Add("txtEmployeeId", empId);

                TempData["empId"] = empId;
                ViewBag.EmpId = empId;
                return View(lv.GetLeaveHistory(empId, DateTime.Now.Year));
            }
            else
            {
                ViewBag.EmpId = "";
                return View();
            }            
        }

        public ActionResult AttendanceHistory()
        {
            ViewBag.erMsg = TempData["erMsg"];

            if (Request.Form.HasKeys())
            {
                _attendence lv = new _attendence();
                var empId = Request.Form["txtEmployeeId"].ToString();

                TempData["empId"] = empId;
                ViewBag.EmpId = empId;
                return View(lv.GetAttendanceHistory(empId, DateTime.Now.Year));
            }
            else
            {
                ViewBag.EmpId = "";
                return View();
            }
        }

        public ActionResult DeleteAttendance()
        {
            _attendence lv = new _attendence();
            var attId = Convert.ToInt32(Request.Form["hfAttendanceId"].ToString());
            string userId = "";
            string remark = "";

            if (!string.IsNullOrEmpty(TempData["empId"] as string))
            {
                userId = TempData["empId"].ToString();
            }

            if (!string.IsNullOrEmpty(Request.Form["txtRemarks"].ToString()))
            {
                remark = Request.Form["txtRemarks"].ToString();
            }
            else
            {
                remark = "No Remark";
            }

            if (lv.DeleteAttendance(attId, remark))
            {
                TempData["erMsg"] = "successMsg('In/Out Record Delete.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('In/Out Record Cannot be Delete.')";
            }

            TempData["empId"] = userId;

            return RedirectToAction("AttendanceHistory");

        }

        public ActionResult EnforceUserLeave(LeaveHistory model = null)
        {
            ViewBag.erMsg = TempData["erMsg"];

            if (model.UserId == null)
            {
                return View();
            }
            else
            {
                _leavs lv = new _leavs();

                if(Request.Form.HasKeys())
                {
                    var lvType = Convert.ToInt32(Request.Form["ddlLeaveType"].ToString());
                    var lvUnit = Request.Form["ddlLeaveUnit"].ToString();

                    model.LeaveType = lvType;
                    model.LeaveUnit = lvUnit;
                    model.SupervisorId = Request.Form["hfUserId"].ToString();

                    if (Request.Form["ddlLeaveUnit"].ToString() != "FULL")
                    {
                        model.ToDate = model.FromDate;
                    }
                }
                
                var res = lv.AdminLeaveEnforcement(model);

                if (res == "0")
                {
                    TempData["erMsg"] = "successMsg('User Leave Has been Enforced')";
                }
                else if (res == "-1")
                {
                    TempData["erMsg"] = "errorMsg('Already Have Applied Leave For This Period')";
                }
                else if (res == "-2")
                {
                    TempData["erMsg"] = "errorMsg('Existing Record Overlaps with Current Request')";
                }

                return RedirectToAction("EnforceUserLeave");

            }
            
        }

        public ActionResult RejectLeave()
        {
            _leavs lv = new App_Codes._leavs();
            var lvId = Convert.ToInt32(Request.Form["hfLeaveId"].ToString());
            string userId = "";
            string remark = "";

            if (!string.IsNullOrEmpty(TempData["empId"] as string))
            {
                userId = TempData["empId"].ToString();
            }

            if (!string.IsNullOrEmpty(Request.Form["txtRemarks"].ToString()))
            {
                remark = Request.Form["txtRemarks"].ToString();
            }
            else
            {
                remark = "No Remark";
            }

            if (lv.CancelUserLeave(lvId, "", remark))
            {
                TempData["erMsg"] = "successMsg('Leave Canceled.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Leave Cannot be Canceled.')";
            }

            TempData["empId"] = userId;

            //var col = Request.Form;
            //var propInfo = col.GetType().GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
            //propInfo.SetValue(col, false, new object[] { });
            //col.Add("txtEmployeeId", userId);

            return RedirectToAction("LeaveHistory");

        }

        public ActionResult NewLeaveType()
        {
            string TypeName = Request.Form["txtType"].ToString();
            string LeaveId = Request.Form["hfLeaveId"].ToString();

            _leavs lv = new App_Codes._leavs();

            if(lv.AddLeaveType(TypeName, Convert.ToInt32(LeaveId)))
            {
                TempData["erMsg"] = "successMsg('Leave Type <b>" + TypeName + "</b> Saved.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Leave Type Could not be Added.')";
            }

            return RedirectToAction("Leaves");
        }

        public ActionResult LeaveAllocation(FormCollection col)
        {
            var RoleId = Request.Form["hfAllocatedRoleId"].ToString();
            _leavs lv = new _leavs();

            foreach (var key in col.AllKeys)
            {
                if(key.Contains("LeaveType_"))
                {
                    var LeaveId = key.Replace("LeaveType_", "");
                    var value = col[key];

                    lv.AddLevelLeaves(RoleId, Convert.ToInt32(LeaveId), float.Parse(value));
                }
            }

            TempData["erMsg"] = "successMsg('Leave Allocation Saved.')";
            return RedirectToAction("Leaves");
        }

        public ActionResult EditUserLeaves(FormCollection col)
        {
            var UserId = Request.Form["hfEditUser"].ToString();
            _leavs lv = new _leavs();

            foreach (var key in col.AllKeys)
            {
                if (key.Contains("LeaveType_Allocated_"))
                {
                    var LeaveId = key.Replace("LeaveType_Allocated_", "");
                    var value = col[key];

                    lv.UpdateUserLeave(UserId, Convert.ToInt32(LeaveId), float.Parse(value), DateTime.Now.Year);
                }
                //else if(key.Contains("LeaveType_Remaining_"))
                //{
                //    var LeaveId = key.Replace("LeaveType_Remaining_", "");
                //    var value = col[key];

                //    lv.UpdateUserLeave(UserId, Convert.ToInt32(LeaveId), float.Parse(value), DateTime.Now.Year);
                //}
            }

            TempData["erMsg"] = "successMsg('Leave Allocation Saved.')";
            return RedirectToAction("Leaves");
        }

        //public ActionResult LeaveReport(FormCollection col)
        //{
        //    var divisionId = Convert.ToInt32(col["ddlDivision"].ToString());
        //    var year = Convert.ToInt32(col["ddlYear"].ToString());

        //    _leavs lv = new _leavs();

        //    var rpt = lv.GetLeaveReport(divisionId, year);
        //    GetLeaveReport(rpt, divisionId, year);

        //    //_projects pr = new _projects();

        //    //if(divisionId == 0)
        //    //{
        //    //    var prLst = pr.GetProjects();
        //    //    var rpt = lv.GetLeaveReport(divisionId, year);

        //    //    GetLeaveReport(rpt, divisionId);
        //    //}
        //    //else
        //    //{                
        //    //    var rpt = lv.GetLeaveReport(divisionId, year);
        //    //    GetLeaveReport(rpt, divisionId);
        //    //}


        //    //TempData["erMsg"] = "successMsg('Leave Allocation Saved.')";
        //    return RedirectToAction("Leaves");
        //}

        public ActionResult AllocateUserLeaves(HttpPostedFileBase fupLeaveFile)
        {
            if (fupLeaveFile.ContentLength > 0)
            {
                var fileName = fupLeaveFile.FileName;
                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                var physicalPath = "temp_files/" + fileName;

                if (!Directory.Exists(Server.MapPath("~/temp_files")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/temp_files"));
                }

                fupLeaveFile.SaveAs(path);

                var str = UploadLeaveAllocation(path);

                if(str == "ok")
                {
                    TempData["erMsg"] = "successMsg('User Leave Allocation File Uploaded.')";
                }
                else
                {
                    TempData["erMsg"] = "errorMsg('Upload Failed.Please Check the File Format and Try Again')";
                }
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Please Select a File To Upload.')";
            }

            
            return RedirectToAction("Leaves");
        }

        public string UploadLeaveAllocation(string FilePath)
        {
            _leavs lv = new App_Codes._leavs();
            _usermanager um = new App_Codes._usermanager();
            Error_Log.er_log erMSG = new Error_Log.er_log();
                       List<UserLeaves> userLv = new List<Models.UserLeaves>();
            string msg = "";

            try
            {
                bool LeaveUpdated = false;

                using (ExcelPackage pck = new ExcelPackage())
                {
                    using (var stream = System.IO.File.OpenRead(FilePath))
                    {
                        pck.Load(stream);
                    }

                    ExcelWorksheet ws = pck.Workbook.Worksheets.First();

                    int startRow = 3;
                    int totalCols = ws.Dimension.End.Column;
                    int totalRows = ws.Dimension.End.Row;                    

                    var levelLeaves = lv.GetUserLevelLeaves();

                    for (int rowNum = startRow; rowNum <= totalRows; rowNum++)
                    {
                        int colId = 1;

                        string EmployeeId = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Leave = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Allocated = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Remainig = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string Year = ws.Cells[rowNum, colId].Value == null ? string.Empty : ws.Cells[rowNum, colId].Value.ToString();

                        //int LeaveId = lv.GetLeaveType(Leave).Id;

                        int LeaveId = 0;

                        if(Leave == "Annual")
                        {
                            LeaveId = 3000;
                        }
                        else if(Leave == "Casual")
                        {
                            LeaveId = 3001;
                        }

                        var UserId = um.GetUserByEmp(EmployeeId);
                        
                        if(UserId != null)
                        {
                            if (lv.GetUserLeaves(UserId.Id, Convert.ToInt32(Year)).Count == 0)
                            {
                                erMSG.WriteLog(EmployeeId + ", " + LeaveId + ", " + Allocated + ", " + Remainig + ", " + Year);
                                userLv.Add(new Models.UserLeaves { UserId = UserId.Id, LeaveType = LeaveId, AllocatedCount = float.Parse(Allocated), RemainingCount = float.Parse(Remainig), Year = Convert.ToInt32(Year) });
                            }
                            else
                            {
                                lv.UpdateUserLeave(UserId.Id, LeaveId, float.Parse(Allocated), float.Parse(Remainig), Convert.ToInt32(Year));
                            }
                        }
                        
                        
                    }
                }

                if(lv.AddUserLeave(userLv))
                {
                    return "ok";
                }
                else if(LeaveUpdated)
                {
                    return "ok";
                }
                else
                {
                    return "fail";
                }
                
                
            }
            catch(Exception er)
            {
                 erMSG.WriteLog(er.Message);
                return er.Message;
            }
            
        }

        public string UpdateHolidays(string FilePath)
        {
            _holidays lv = new _holidays();
            _usermanager um = new _usermanager();
            Error_Log.er_log erMSG = new Error_Log.er_log();
            List<Holiday> holidays = new List<Holiday>();
            string msg = "";

            try
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    using (var stream = System.IO.File.OpenRead(FilePath))
                    {
                        pck.Load(stream);
                    }

                    ExcelWorksheet ws = pck.Workbook.Worksheets.First();

                    int startRow = 2;
                    int totalCols = ws.Dimension.End.Column;
                    int totalRows = ws.Dimension.End.Row;                    

                    for (int rowNum = startRow; rowNum <= totalRows; rowNum++)
                    {
                        int colId = 1;
                        //var isEmpty = ws.Cells[rowNum, colId].Value.ToString();

                        string _date = ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        string _description = ws.Cells[rowNum, colId].Value.ToString();
                        colId++;

                        holidays.Add(new Holiday { Date = Convert.ToDateTime(_date), Description = _description });
                       
                    }

                    if(lv.SaveHolidays(holidays))
                    {
                        return "200";
                    }
                    else
                    {
                        return "Error Uploading File";
                    }
                    
                }
                
            }
            catch (Exception er)
            {
                erMSG.WriteLog(er.Message);
                return er.Message;
            }

        }

        public ActionResult UploadHolidays(HttpPostedFileBase fupHolidaysFile)
        {
            if(fupHolidaysFile.ContentLength > 0)
            {
                var fileName = fupHolidaysFile.FileName;
                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                var physicalPath = "temp_files/" + fileName;

                if (!Directory.Exists(Server.MapPath("~/temp_files")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/temp_files"));
                }

                fupHolidaysFile.SaveAs(path);

                var str = UpdateHolidays(path);

                if (str == "200")
                {
                    TempData["erMsg"] = "successMsg('Holiday Calender File Uploaded.')";
                }
                else
                {
                    TempData["erMsg"] = "errorMsg('Upload Failed.Please Check the File Format and Try Again')";
                }
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Please Select a File To Upload.')";
            }

            return RedirectToAction("Leaves");
        }

        public ActionResult DeleteHolidays()
        {
            _holidays lv = new _holidays();
            var holidayId = Convert.ToInt32(Request.Form["hfHolidayId"].ToString());

            if (lv.DeleteHoliday(holidayId))
            {
                TempData["erMsg"] = "successMsg('Selected Holiday Removed from the Calender.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Error Occured)";
            }
            
            return RedirectToAction("Leaves");
        }

        //public void GetLeaveReport(List<LeaveReportViewModel> lst, int divisionId, int year)
        //{
        //    _projects pr = new _projects();
        //    _leavs lv = new _leavs();

        //    if (divisionId == 0)
        //    {
        //        var prLst = pr.GetProjects();

        //        var leaves = lv.GetLeaves();

        //        var fileName = "leave_report_all_" + DateTime.Now.Ticks + ".xlsx";
        //        var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
        //        string DirectoryPath = Server.MapPath("\\temp_files");
        //        string UrlPath = "temp_files/" + fileName;

        //        int intSheetIndex = 1;
        //        int intRowNumber = 5;
        //        int intColumnNumber = 1;

        //        if (!Directory.Exists(DirectoryPath))
        //        {
        //            Directory.CreateDirectory(DirectoryPath);
        //        }

        //        System.IO.FileInfo objFile = new System.IO.FileInfo(path);
        //        ExcelPackage exlPac = new ExcelPackage(objFile);

        //        foreach (var div in prLst)
        //        {
        //            exlPac.Workbook.Worksheets.Add(div.ProjectName);
        //            ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

        //            exlWs.Row(1).Style.Font.Bold = true;
        //            exlWs.Row(2).Style.Font.Bold = true;
        //            exlWs.Row(3).Style.Font.Bold = true;
        //            exlWs.Row(4).Style.Font.Bold = true;

        //            exlWs.Column(1).Width = 15;
        //            exlWs.Column(2).Width = 30;
        //            exlWs.Column(3).Width = 30;
        //            exlWs.Column(4).Width = 30;
        //            exlWs.Column(5).Width = 30;

        //            exlWs.Cells[1, 1].Value = "Leave Report For Year - " + year;
        //            exlWs.Cells[1, 1, 1, 5].Merge = true;
        //            exlWs.Cells[3, 1].Value = "Employee No";
        //            exlWs.Cells[3, 2].Value = "Employee Name";
        //            exlWs.Cells[3, 3].Value = "Designation";
        //            exlWs.Cells[3, 4].Value = "Location";
        //            exlWs.Cells[3, 5].Value = "Supervisor";

        //            int leaveIndex = 6;

        //            foreach (var item in leaves)
        //            {
        //                exlWs.Column(leaveIndex).Width = 10;
        //                exlWs.Column(leaveIndex + 1).Width = 10;

        //                exlWs.Cells[3, leaveIndex].Value = item.LeaveType1;
        //                exlWs.Cells[3, leaveIndex, 3, leaveIndex + 1].Merge = true;

        //                exlWs.Cells[4, leaveIndex].Value = "Allocated";
        //                exlWs.Cells[4, leaveIndex + 1].Value = "Remaining";

        //                leaveIndex += 2;
        //            }

        //            exlWs.Cells[3, leaveIndex].Value = "Year";

        //            foreach (var item in lst.Where(x => x.DivisionId == div.Id))
        //            {
        //                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
        //                intColumnNumber += 1;

        //                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
        //                intColumnNumber += 1;

        //                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Designation;
        //                intColumnNumber += 1;

        //                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Location;
        //                intColumnNumber += 1;

        //                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Supervisor;
        //                intColumnNumber += 1;

        //                foreach (var sub in leaves)
        //                {
        //                    if (item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault() != null)
        //                    {
        //                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().AllocatedCount;
        //                        intColumnNumber += 1;

        //                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().RemainingCount;
        //                        intColumnNumber += 1;
        //                    }
        //                    else
        //                    {
        //                        exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
        //                        intColumnNumber += 1;

        //                        exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
        //                        intColumnNumber += 1;
        //                    }

        //                }

        //                exlWs.Cells[intRowNumber, intColumnNumber].Value = year;
        //                intColumnNumber += 1;

        //                intRowNumber++;
        //                intColumnNumber = 1;
        //            }

        //            intSheetIndex++;
        //            intRowNumber = 5;
        //        }

        //        exlPac.Save();

        //    }
        //    else
        //    {
        //        var prLst = pr.GetProject(divisionId);
        //        var leaves = lv.GetLeaves();

        //        var fileName = "leave_report_" + prLst.ProjectName.ToLower() + "_" + DateTime.Now.Ticks + ".xlsx";
        //        var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
        //        string DirectoryPath = Server.MapPath("\\temp_files");
        //        string UrlPath = "temp_files/" + fileName;

        //        int intSheetIndex = 1;
        //        int intRowNumber = 5;
        //        int intColumnNumber = 1;

        //        if (!Directory.Exists(DirectoryPath))
        //        {
        //            Directory.CreateDirectory(DirectoryPath);
        //        }

        //        System.IO.FileInfo objFile = new System.IO.FileInfo(path);
        //        ExcelPackage exlPac = new ExcelPackage(objFile);

        //        exlPac.Workbook.Worksheets.Add(prLst.ProjectName);
        //        ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

        //        exlWs.Row(1).Style.Font.Bold = true;
        //        exlWs.Row(2).Style.Font.Bold = true;
        //        exlWs.Row(3).Style.Font.Bold = true;
        //        exlWs.Row(4).Style.Font.Bold = true;

        //        exlWs.Column(1).Width = 15;
        //        exlWs.Column(2).Width = 30;
        //        exlWs.Column(3).Width = 30;
        //        exlWs.Column(4).Width = 30;
        //        exlWs.Column(5).Width = 30;

        //        exlWs.Cells[1, 1].Value = "Leave Report For Year - " + year;
        //        exlWs.Cells[1, 1, 1, 5].Merge = true;
        //        exlWs.Cells[3, 1].Value = "Employee No";
        //        exlWs.Cells[3, 2].Value = "Employee Name";
        //        exlWs.Cells[3, 3].Value = "Designation";
        //        exlWs.Cells[3, 4].Value = "Location";
        //        exlWs.Cells[3, 5].Value = "Supervisor";

        //        int leaveIndex = 6;

        //        foreach(var item in leaves)
        //        {
        //            exlWs.Column(leaveIndex).Width = 10;
        //            exlWs.Column(leaveIndex + 1).Width = 10;

        //            exlWs.Cells[3, leaveIndex].Value = item.LeaveType1;
        //            exlWs.Cells[3, leaveIndex, 3, leaveIndex + 1].Merge = true;

        //            exlWs.Cells[4, leaveIndex].Value = "Allocated";
        //            exlWs.Cells[4, leaveIndex + 1].Value = "Remaining";

        //            leaveIndex +=2;
        //        }

        //        exlWs.Cells[3, leaveIndex].Value = "Year";

        //        foreach (var item in lst)
        //        {
        //            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
        //            intColumnNumber += 1;

        //            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
        //            intColumnNumber += 1;

        //            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Designation;
        //            intColumnNumber += 1;

        //            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Location;
        //            intColumnNumber += 1;

        //            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Supervisor;
        //            intColumnNumber += 1;

        //            foreach (var sub in leaves)
        //            {
        //                if(item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault() != null)
        //                {
        //                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().AllocatedCount;
        //                    intColumnNumber += 1;

        //                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().RemainingCount;
        //                    intColumnNumber += 1;
        //                }
        //                else
        //                {
        //                    exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
        //                    intColumnNumber += 1;

        //                    exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
        //                    intColumnNumber += 1;
        //                }

        //            }

        //            exlWs.Cells[intRowNumber, intColumnNumber].Value = year;
        //            intColumnNumber += 1;

        //            intRowNumber++;
        //            intColumnNumber = 1;
        //        }

        //        exlPac.Save();
        //    }
        //}

        public ActionResult Transfers()
        {
            EmployeeTracking.App_Codes._projects pr = new EmployeeTracking.App_Codes._projects();
            _transfers tr = new _transfers();
            TransferViewModel vm = new Models.TransferViewModel();
            vm.Divisions = pr.ProjectsList();
            vm.TransferRequests = tr.GetTransferRequests(Session["UserId"].ToString());
            vm.SiteLocations = tr.GetSiteLocations();
            ViewBag.erMsg = TempData["erMsg"];
            return View(vm);
        }

        public ActionResult RequestTransfer(TransferViewModel model)
        {
            _transfers tr = new _transfers();

            model.RequestTransfer.RequestBy = Session["UserId"].ToString();

            if(tr.NewTransfer(model.RequestTransfer))
            {
                TempData["erMsg"] = "successMsg('Transfer Request Sent.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Transfer Request Could not be Sent.')";
            }

            return RedirectToAction("Transfers");
        }

        public ActionResult ResetPassword(string UserId)
        {
            _usermanager um = new App_Codes._usermanager();
            var user = um.GetUser(UserId);
            _emails em = new App_Codes._emails();

            if (um.ResetPassword(UserId))
            {                
                TempData["ErMsg"] = "successMsg('User Password Has Been Reset to Default 'abc@123'');";
                em.PasswordResetMail(user);
            }
            else
            {
                TempData["ErMsg"] = "errorMsg('Password Cannot be Reset');";
                
            }

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword()
        {
            var oldPassword = Request.Form["txtCurrentPassword"];
            var newPassword = Request.Form["txtNewPassword"];
            _usermanager um = new App_Codes._usermanager();
            
            if (um.ChangePassword(Session["UserId"].ToString(), oldPassword, newPassword))
            {
                TempData["ErMsg"] = "successMsg('New Password Saved.Please Login Using Your New Password');";
                //return RedirectToAction("LogOff", "UserManagement");
            }
            else
            {
                TempData["ErMsg"] = "errorMsg('Password Cannot be Saved');";
                //return RedirectToAction("Home", "Index");
            }

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Account");
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Account");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }

    
}