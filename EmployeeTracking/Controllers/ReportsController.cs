using EmployeeTracking.App_Codes;
using EmployeeTracking.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LeaveReports()
        {
            ViewBag.erMsg = TempData["erMsg"];
            return View();
        }

        public ActionResult AttendanceReports()
        {
            ViewBag.erMsg = TempData["erMsg"];
            return View();
        }

        public ActionResult TransferReports()
        {
            ViewBag.erMsg = TempData["erMsg"];
            return View();
        }

        public ActionResult LeaveSummery(FormCollection col)
        {
            var divisionId = Convert.ToInt32(col["ddlDivision"].ToString());
            var year = Convert.ToInt32(col["ddlYear"].ToString());

            _leavs lv = new _leavs();

            var rpt = lv.GetLeaveReport(divisionId, year);
            GetLeaveReport(rpt, divisionId, year);

            return RedirectToAction("LeaveReports");
        }

        public ActionResult LeaveDetails(FormCollection col)
        {
            var divisionId = Convert.ToInt32(col["ddlDivisionDetails"].ToString());
            var from = Convert.ToDateTime(col["txtLeaveFrom"].ToString());
            var to = Convert.ToDateTime(col["txtLeaveTo"].ToString());

            _leavs lv = new _leavs();

            var rpt = lv.GetLeaveDetails(divisionId, from, to);
            GetLeaveDetailsReport(rpt, divisionId, from.ToShortDateString() + " - " + to.ToShortDateString());
            return RedirectToAction("LeaveReports");
        }

        public ActionResult InOutDetails(FormCollection col)
        {
            _attendence att = new _attendence();

            var divisionId = Convert.ToInt32(col["ddlDivision"].ToString());
            //var year = Convert.ToInt32(col["ddlYear"].ToString());
            var dateFrom = Convert.ToDateTime(Request.Form["txtDateFrom"].ToString());
            var dateTo = Convert.ToDateTime(Request.Form["txtDateTo"].ToString());

            var lst = att.GetInOut(dateFrom, dateTo, divisionId);

            if (lst.Count > 0)
            {
                GetInOutReport(lst, divisionId, dateFrom, dateTo);
            }
            else
            {
                TempData["erMsg"] = "errorMsg('No Records Found For the Selected Parameterss')";
            }

            return RedirectToAction("AttendanceReports");
        }

        public ActionResult TranferDetails(FormCollection col)
        {
            _usermanager um = new App_Codes._usermanager();
            _transfers tr = new _transfers();

            //var divisionId = Convert.ToInt32(col["ddlDivision"].ToString());
            var year = Convert.ToInt32(col["ddlYear"].ToString());
            var com = um.GetUserCompany(Session["UserId"].ToString());

            if (User.IsInRole("SuperAdmin"))
            {
                var lst = tr.GetTransfers(0, year);

                GetTransferReport(lst, year);
                return RedirectToAction("TransferReports");
            }
            else
            {
                var lst = tr.GetTransfers(com.Id, year);

                GetTransferReport(lst, year, com.CompanyName);
                return RedirectToAction("TransferReports");
            }

        }

        public ActionResult MonthlyBreackdownSummery()
        {
            _projects pr = new _projects();
            _siteLocations sl = new _siteLocations();

            LeaveBalanceViewModel VM = new LeaveBalanceViewModel();
            VM.Divisions = pr.GetProjects();
            VM.Locations = sl.GetSiteLocations();

            return View(VM);
        }

        public ActionResult MonthlyLeaveSummery()
        {
            _projects pr = new _projects();

            var cult = System.Globalization.CultureInfo.CurrentCulture;
            List<MonthModel> months = new List<MonthModel>();

            for(int i = 1; i <= 12; i++)
            {
                //var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i);
                months.Add(new MonthModel { Id = i, Name = cult.DateTimeFormat.GetMonthName(i)});
            }


            MonthlyLeaveSummeryViewModel VM = new MonthlyLeaveSummeryViewModel();            
            VM.Divisions = pr.GetProjects();
            VM.MonthList = months;
            VM.DaysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            VM.Year = DateTime.Now.Year;
            VM.Month = DateTime.Now.Month;
            //VM.Locations = sl.GetSiteLocations();

            return View(VM);
        }

        [HttpPost]
        public ActionResult MonthlyLeaveSummery(MonthlyLeaveSummeryViewModel model)
        {
            _projects pr = new _projects();
            _reports rpt = new App_Codes._reports();

            var cult = System.Globalization.CultureInfo.CurrentCulture;
            List<MonthModel> months = new List<MonthModel>();

            var rptMode = Request.Form["system"] as string;

            //int toMonth = 12;
            //int currentMonth = DateTime.Now.Month;
            //int year = DateTime.Now.Year;

            for (int i = 1; i <= 12; i++)
            {
                //var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i);
                months.Add(new MonthModel { Id = i, Name = cult.DateTimeFormat.GetMonthName(i) });
            }

            string divisionName = "All Divisions";
            var division = Convert.ToInt32(Request.Form["Division"].ToString());

            var year = Convert.ToInt32(Request.Form["Year"].ToString());
            var month = Convert.ToInt32(Request.Form["Month"].ToString());

            if (division > 0)
            {
                divisionName = pr.GetProject(division).ProjectName;
            }

            var res = rpt.GetLeaveBalance(division, year, month);

            MonthlyLeaveSummeryViewModel VM = new MonthlyLeaveSummeryViewModel();
            VM.Divisions = pr.GetProjects();
            VM.EmployeeLeaveSummery = res;
            
            VM.MonthList = months;
            VM.DaysInMonth = DateTime.DaysInMonth(year, month);
            VM.Year = year;
            VM.Month = month;
            VM.TimePeriod = cult.DateTimeFormat.GetAbbreviatedMonthName(month) + "-" + year;

            ViewBag.Year = cult.DateTimeFormat.GetAbbreviatedMonthName(month) + "-" + year;
            ViewBag.Division = divisionName;

            if(Request.Form["view"] != null)
            {
                VM.ReportData = ViewMonthlyLeaveSummery(res, year, month, cult.DateTimeFormat.GetAbbreviatedMonthName(month) + "-" + year, divisionName);
            }
            else if (Request.Form["system"] != null)
            {
                var sysRes = rpt.GetUsedLeavesForUsers(division, year, month);
                GetSystemeFormatedReport(sysRes, year, month);
            }
            else
            {
                GenerateBalanceSummery(res, year, month, cult.DateTimeFormat.GetAbbreviatedMonthName(month) + "-" + year, divisionName);
            }

            return View(VM);
        }

        [HttpPost]
        public ActionResult MonthlyBreackdownSummery(LeaveBalanceViewModel model)
        {
            _projects pr = new _projects();
            _siteLocations sl = new _siteLocations();
            _reports rpt = new App_Codes._reports();

            
            var division = Convert.ToInt32(Request.Form["Division"].ToString());
            var location = Request.Form["Location"].ToString();
            var locationName = "All Locations";

            var year = Convert.ToInt32(Request.Form["Year"].ToString());

            string divisionName = "All Divisions";

            if (division > 0)
            {
                divisionName = pr.GetProject(division).ProjectName;
            }

            if (location != "0")
            {
                locationName = location;
            }

            LeaveBalanceViewModel VM = new LeaveBalanceViewModel();
            VM.Divisions = pr.GetProjects();
            VM.Locations = sl.GetSiteLocations();

            var res = rpt.GetLeaveBalance(division, location, year);

            if (Request.Form["view"] != null)
            {
                VM.ReportData = ViewBalanceSummery(res, year, divisionName, locationName);
            }
            else
            {
                GenerateBalanceSummery(res, year, divisionName, locationName);
            }
            ViewBag.Division = divisionName;
            ViewBag.Location = locationName;
            ViewBag.Year = year;

            return View(VM);

           
            
        }

        public void GetSystemeFormatedReport(List<UsedLeavesByUser> model, int Year, int Month)
        {
            var timePeriod = Month + "/" + Year;

            var fileName = "monthly_attendance_system" + DateTime.Now.Ticks + ".xlsx";
            var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
            string DirectoryPath = Server.MapPath("\\temp_files");
            string UrlPath = "temp_files/" + fileName;

            int intSheetIndex = 1;
            int intRowNumber = 2;
            int intColNumber = 1;
            //int intLastColumnNumber = 0;
            int headerIndex = 3;

            //int intEndColumn = 1;
            //int daysInMonth = model.FirstOrDefault().MonthlyLeaveDetails.Count();

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            System.IO.FileInfo objFile = new System.IO.FileInfo(path);
            ExcelPackage exlPac = new ExcelPackage(objFile);

            exlPac.Workbook.Worksheets.Add("sheet 1");
            ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

            exlWs.Row(1).Style.Font.Bold = true;
            exlWs.Row(2).Style.Font.Bold = true;
            exlWs.Row(3).Style.Font.Bold = true;
            exlWs.Row(4).Style.Font.Bold = true;
            exlWs.Row(5).Style.Font.Bold = true;
            exlWs.Row(6).Style.Font.Bold = true;
            exlWs.Row(7).Style.Font.Bold = true;

            exlWs.Cells[1, 1].Value = "EMP#";
            exlWs.Cells[1, 2].Value = "PERIOD";

            foreach(var lv in model.FirstOrDefault().LeaveTypes)
            {
                exlWs.Cells[1, headerIndex].Value = lv.LeaveTypeCode;
                headerIndex++;
            }

            foreach (var lv in model)
            {
                exlWs.Cells[intRowNumber, intColNumber].Value = lv.EmpNo;
                intColNumber++;

                exlWs.Cells[intRowNumber, intColNumber].Value = lv.Period;
                intColNumber++;

                foreach(var lvs in lv.UsedLeaves)
                {
                    exlWs.Cells[intRowNumber, intColNumber].Value = lvs.Used_Count;
                    intColNumber++;
                }

                intColNumber = 1;
                intRowNumber++;
            }

            exlPac.Save();

            WebClient req = new WebClient();
            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            byte[] data = req.DownloadData(path);
            response.BinaryWrite(data);
            System.IO.File.Delete(path);
            response.End();

        }

        private DataTable ViewBalanceSummery(List<LeaveBalanceUser> model, int Year, string Division, string Location)
        {

            var fileName = "monthly_leave_summery" + DateTime.Now.Ticks + ".xlsx";
            var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
            string DirectoryPath = Server.MapPath("\\temp_files");
            string UrlPath = "temp_files/" + fileName;
            string logoPath = Server.MapPath("~/Content/Images/sierrs_rpt.png");

            int intSheetIndex = 1;
            int intRowNumber = 8;
            int intLastColumnNumber = 0;
            int intEndColumn = 1;

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            System.IO.FileInfo objFile = new System.IO.FileInfo(path);
            ExcelPackage exlPac = new ExcelPackage(objFile);

            exlPac.Workbook.Worksheets.Add("sheet 1");
            ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

            exlWs.Row(1).Style.Font.Bold = true;
            exlWs.Row(2).Style.Font.Bold = true;
            exlWs.Row(3).Style.Font.Bold = true;
            exlWs.Row(4).Style.Font.Bold = true;
            exlWs.Row(5).Style.Font.Bold = true;
            exlWs.Row(6).Style.Font.Bold = true;
            exlWs.Row(7).Style.Font.Bold = true;

            exlWs.Column(1).Width = 15;
            exlWs.Column(2).Width = 25;
            //exlWs.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


            exlWs.Cells[1, 1, 4, 3].Merge = true;
            Image logo = Image.FromFile(logoPath);
            var picture = exlWs.Drawings.AddPicture("img", logo);
            picture.SetSize(70);
            picture.SetPosition(1, 0, 1, 0);

            exlWs.Cells[1, 4].Value = "Leave Summery Report For Year - " + Year;
            exlWs.Cells[2, 4].Value = Division;
            exlWs.Cells[3, 4].Value = Location;

            exlWs.Cells[6, 1].Value = "Employee No";
            exlWs.Cells[6, 2].Value = "Employee Name";
            exlWs.Cells[6, 3, 6, 4].Value = "Allocated";
            exlWs.Cells[6, 3, 6, 4].Merge = true;
            exlWs.Cells[7, 3].Value = "Annual";
            exlWs.Cells[7, 4].Value = "Cassual";

            foreach (var userMain in model)
            {
                if (!string.IsNullOrEmpty(userMain.EmployeeNumber))
                {
                    int monthColumn = 5;

                    exlWs.Cells[intRowNumber, 1].Value = userMain.EmployeeNumber;
                    exlWs.Cells[intRowNumber, 2].Value = userMain.EmployeeName;
                    exlWs.Cells[intRowNumber, 3].Value = userMain.AnnualAllocated;
                    exlWs.Cells[intRowNumber, 4].Value = userMain.CassualAllocated;

                    foreach (var months in userMain.MonthlyUsedLeaves)
                    {
                        exlWs.Cells[6, monthColumn, 6, monthColumn + 3].Merge = true;
                        exlWs.Cells[6, monthColumn].Value = months.Month_Name;
                        exlWs.Cells[7, monthColumn].Value = "AN";
                        exlWs.Cells[7, monthColumn + 1].Value = "CA";
                        exlWs.Cells[7, monthColumn + 2].Value = "SAN";
                        exlWs.Cells[7, monthColumn + 3].Value = "NOP";

                        if (months.LeaveTypes.Where(x => x.LeaveType_Id == 3000).FirstOrDefault() != null)
                        {
                            var count = months.LeaveTypes.Where(x => x.LeaveType_Id == 3000).FirstOrDefault().Used_Count;

                            if (count > 0 || count < 0)
                            {
                                exlWs.Cells[intRowNumber, monthColumn].Value = count;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, monthColumn].Value = "";
                            }
                        }

                        if (months.LeaveTypes.Where(x => x.LeaveType_Id == 3001).FirstOrDefault() != null)
                        {
                            var count = months.LeaveTypes.Where(x => x.LeaveType_Id == 3001).FirstOrDefault().Used_Count;

                            if (count > 0 || count < 0)
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 1].Value = count;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 1].Value = "";
                            }
                        }

                        if (months.LeaveTypes.Where(x => x.LeaveType_Id == 3010).FirstOrDefault() != null)
                        {
                            var count = months.LeaveTypes.Where(x => x.LeaveType_Id == 3010).FirstOrDefault().Used_Count;

                            if (count > 0 || count < 0)
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 2].Value = count;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 2].Value = "";
                            }
                        }

                        if (months.LeaveTypes.Where(x => x.LeaveType_Id == -1).FirstOrDefault() != null)
                        {
                            var count = months.LeaveTypes.Where(x => x.LeaveType_Id == -1).FirstOrDefault().Used_Count;

                            if (count > 0 || count < 0)
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 3].Value = count;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 3].Value = "";
                            }
                        }

                        monthColumn += 3;
                        monthColumn++;
                    }


                    foreach (var totalUsed in userMain.TotalUsed)
                    {

                        if (totalUsed.LeaveType_Id == 3000)
                        {
                            exlWs.Cells[intRowNumber, monthColumn].Value = totalUsed.Used_Count;
                        }

                        if (totalUsed.LeaveType_Id == 3001)
                        {
                            exlWs.Cells[intRowNumber, monthColumn + 1].Value = totalUsed.Used_Count;
                        }

                        if (totalUsed.LeaveType_Id == 3010)
                        {
                            exlWs.Cells[intRowNumber, monthColumn + 2].Value = totalUsed.Used_Count;
                        }

                        if (totalUsed.LeaveType_Id == -1)
                        {
                            exlWs.Cells[intRowNumber, monthColumn + 3].Value = totalUsed.Used_Count;
                        }


                    }

                    exlWs.Cells[intRowNumber, monthColumn + 4].Value = userMain.BalanceAnnualLeaves;
                    exlWs.Cells[intRowNumber, monthColumn + 5].Value = userMain.BalanceCassualLeaves;

                    intRowNumber++;
                    intLastColumnNumber = monthColumn;
                }

            }

            exlWs.Cells[6, intLastColumnNumber, 6, intLastColumnNumber + 3].Merge = true;
            exlWs.Cells[6, intLastColumnNumber].Value = "Total";
            exlWs.Cells[7, intLastColumnNumber].Value = "AN";
            exlWs.Cells[7, intLastColumnNumber + 1].Value = "CA";
            exlWs.Cells[7, intLastColumnNumber + 2].Value = "SAN";
            exlWs.Cells[7, intLastColumnNumber + 3].Value = "NOP";

            int balanceCol = intLastColumnNumber + 4;

            exlWs.Cells[6, balanceCol, 6, balanceCol + 3].Merge = true;
            exlWs.Cells[6, balanceCol].Value = "Balance";
            exlWs.Cells[7, balanceCol].Value = "AN";
            exlWs.Cells[7, balanceCol + 1].Value = "CA";
            exlWs.Cells[7, balanceCol + 2].Value = "SAN";
            exlWs.Cells[7, balanceCol + 3].Value = "NOP";

            //Row Styles
            var modelRows = intRowNumber - 1;
            string modelRange = "A6:BH" + modelRows.ToString();
            var modelTable = exlWs.Cells[modelRange];

            // Assign borders
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            ///Footer
            var footerRow = modelRows + 2;
            var prepColumn = "I" + footerRow + ":O" + footerRow;
            var checkedColumn = "AA" + footerRow + ":AG" + footerRow;
            var approvedColumn = "AU" + footerRow + ":AZ" + footerRow;


            DataTable dt = GetDataTableFromExcel(exlPac, true);

            return dt;
        }

        private DataTable ViewMonthlyLeaveSummery(List<EmployeeLeaveSummery> model, int Year, int Month, string TimePeriod, string Division)
        {

            var fileName = "monthly_leave_summery" + DateTime.Now.Ticks + ".xlsx";
            var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
            string DirectoryPath = Server.MapPath("\\temp_files");
            string UrlPath = "temp_files/" + fileName;
            string logoPath = Server.MapPath("~/Content/Images/sierrs_rpt.png");

            int intSheetIndex = 1;
            int intRowNumber = 8;
            int intLastColumnNumber = 0;
            int intEndColumn = 1;
            int daysInMonth = model.FirstOrDefault().MonthlyLeaveDetails.Count();

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            System.IO.FileInfo objFile = new System.IO.FileInfo(path);
            ExcelPackage exlPac = new ExcelPackage(objFile);

            exlPac.Workbook.Worksheets.Add("sheet 1");
            ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

            exlWs.Row(1).Style.Font.Bold = true;
            exlWs.Row(2).Style.Font.Bold = true;
            exlWs.Row(3).Style.Font.Bold = true;
            exlWs.Row(4).Style.Font.Bold = true;
            exlWs.Row(5).Style.Font.Bold = true;
            exlWs.Row(6).Style.Font.Bold = true;
            exlWs.Row(7).Style.Font.Bold = true;

            exlWs.Column(1).Width = 15;
            exlWs.Column(2).Width = 25;
            //exlWs.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


            exlWs.Cells[1, 1, 4, 3].Merge = true;
            Image logo = Image.FromFile(logoPath);
            var picture = exlWs.Drawings.AddPicture("img", logo);
            picture.SetSize(70);
            picture.SetPosition(1, 0, 1, 0);

            exlWs.Cells[1, 4].Value = "Monthly Attendance Summery Report For - " + TimePeriod;
            exlWs.Cells[2, 4].Value = Division;
            //exlWs.Cells[3, 4].Value = Location;

            exlWs.Cells[6, 1].Value = "Employee No";
            exlWs.Cells[6, 2].Value = "Employee Name";

            for(int i = 1; i <= daysInMonth; i++)
            {
                int dateStartIndex = 2 + i;
                exlWs.Cells[6, dateStartIndex].Value = i;

                var date = new DateTime(Year, Month, i);
                var dayOfWeek = date.DayOfWeek;

                if (dayOfWeek == DayOfWeek.Saturday)
                {
                    exlWs.Cells[7, dateStartIndex].Value = "SAT";
                }
                else if(dayOfWeek == DayOfWeek.Sunday)
                {
                    exlWs.Cells[7, dateStartIndex].Value = "SUN";
                }
            }

            //exlWs.Cells[6, 3, 6, 4].Value = "Allocated";
            //exlWs.Cells[6, 3, 6, 4].Merge = true;
            //exlWs.Cells[7, 3].Value = "Annual";
            //exlWs.Cells[7, 4].Value = "Cassual";

            foreach (var userMain in model)
            {
                if (!string.IsNullOrEmpty(userMain.EmployeeNumber))
                {
                    int dateColumn = 3;

                    exlWs.Cells[intRowNumber, 1].Value = userMain.EmployeeNumber;
                    exlWs.Cells[intRowNumber, 2].Value = userMain.EmployeeName;

                    foreach (var months in userMain.MonthlyLeaveDetails)
                    {
                        exlWs.Cells[intRowNumber, dateColumn].Value = months.AttendanceType;                        
                        dateColumn++;
                    }

                    exlWs.Cells[intRowNumber, dateColumn].Value = userMain.LeaveTypes.Where(x => x.LeaveType_Id == 3001).FirstOrDefault().Used_Count;
                    dateColumn++;

                    exlWs.Cells[intRowNumber, dateColumn].Value = userMain.LeaveTypes.Where(x => x.LeaveType_Id == 3000).FirstOrDefault().Used_Count;
                    dateColumn++;

                    double lv3010 = 0;
                    double lv3014 = 0;

                    if (userMain.LeaveTypes.Where(x => x.LeaveType_Id == 3014).FirstOrDefault() != null)
                    {
                        lv3014 = userMain.LeaveTypes.Where(x => x.LeaveType_Id == 3014).FirstOrDefault().Used_Count;
                    }

                    if (userMain.LeaveTypes.Where(x => x.LeaveType_Id == 3010).FirstOrDefault() != null)
                    {
                        lv3010 = userMain.LeaveTypes.Where(x => x.LeaveType_Id == 3010).FirstOrDefault().Used_Count;
                    }

                    exlWs.Cells[intRowNumber, dateColumn].Value = lv3010 + lv3014;
                    dateColumn++;

                    exlWs.Cells[intRowNumber, dateColumn].Value = 2;
                    dateColumn++;
                    //exlWs.Cells[intRowNumber, monthColumn + 4].Value = userMain.BalanceAnnualLeaves;
                    //exlWs.Cells[intRowNumber, monthColumn + 5].Value = userMain.BalanceCassualLeaves;

                    intRowNumber++;
                    intLastColumnNumber = dateColumn;
                }

            }

            exlWs.Cells[6, intLastColumnNumber, 6, intLastColumnNumber + 3].Merge = true;
            exlWs.Cells[6, intLastColumnNumber].Value = "Total";
            //exlWs.Cells[7, intLastColumnNumber].Value = "CA";
            //exlWs.Cells[7, intLastColumnNumber + 1].Value = "AN";
            //exlWs.Cells[7, intLastColumnNumber + 2].Value = "NL";
            //exlWs.Cells[7, intLastColumnNumber + 3].Value = "SL";

            int balanceCol = intLastColumnNumber + 4;

            exlWs.Cells[6, balanceCol, 6, balanceCol + 3].Merge = true;
            exlWs.Cells[6, balanceCol].Value = "Balance";
            //exlWs.Cells[7, balanceCol].Value = "CA";
            //exlWs.Cells[7, balanceCol + 1].Value = "AN";
            //exlWs.Cells[7, balanceCol + 2].Value = "NL";
            //exlWs.Cells[7, balanceCol + 3].Value = "SL";

            //Row Styles
            var modelRows = intRowNumber - 1;
            string modelRange = "A6:BH" + modelRows.ToString();
            var modelTable = exlWs.Cells[modelRange];

            // Assign borders
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            ///Footer
            var footerRow = modelRows + 2;
            var prepColumn = "I" + footerRow + ":O" + footerRow;
            var checkedColumn = "AA" + footerRow + ":AG" + footerRow;
            var approvedColumn = "AU" + footerRow + ":AZ" + footerRow;


            DataTable dt = GetDataTableFromExcel(exlPac, Year, Month, daysInMonth, intRowNumber, true);

            return dt;
        }

        public DataTable GetDataTableFromExcel(ExcelPackage exlPac, int Year, int Month, int DaysInMonth, int Rows, bool hasHeader = true)
        {
            using (var pck = exlPac)
            {
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();

                var daysInMonth = DaysInMonth;
                tbl.Columns.Add("Employee No");
                tbl.Columns.Add("Employee Name");

                for (int i = 1; i <= daysInMonth; i++)
                {
                    tbl.Columns.Add("Date_" + i);
                }

              
                var startRow = hasHeader ? 8 : 8;
                for (int rowNum = startRow; rowNum <= Rows; rowNum++)
                {
                    try
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                    catch
                    {

                    }
                   
                }
                return tbl;
            }
        }

        public DataTable GetDataTableFromExcel(ExcelPackage exlPac, bool hasHeader = true)
        {
            using (var pck = exlPac)
            {
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                //foreach (var firstRowCell in ws.Cells[6, 1, 6, ws.Dimension.End.Column])
                //{
                //    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                //}

                tbl.Columns.Add("Employee No");
                tbl.Columns.Add("Employee Name");
                tbl.Columns.Add("Annual");
                tbl.Columns.Add("Cassual");

                tbl.Columns.Add("jan-AN");
                tbl.Columns.Add("jan-CA");
                tbl.Columns.Add("jan-SAN");
                tbl.Columns.Add("jan-NOP");

                tbl.Columns.Add("feb-AN");
                tbl.Columns.Add("feb-CA");
                tbl.Columns.Add("feb-SAN");
                tbl.Columns.Add("feb-NOP");

                tbl.Columns.Add("mar-AN");
                tbl.Columns.Add("mar-CA");
                tbl.Columns.Add("mar-SAN");
                tbl.Columns.Add("mar-NOP");

                tbl.Columns.Add("apr-AN");
                tbl.Columns.Add("apr-CA");
                tbl.Columns.Add("apr-SAN");
                tbl.Columns.Add("apr-NOP");

                tbl.Columns.Add("may-AN");
                tbl.Columns.Add("may-CA");
                tbl.Columns.Add("may-SAN");
                tbl.Columns.Add("may-NOP");

                tbl.Columns.Add("Jun-AN");
                tbl.Columns.Add("Jun-CA");
                tbl.Columns.Add("Jun-SAN");
                tbl.Columns.Add("Jun-NOP");

                tbl.Columns.Add("jul-AN");
                tbl.Columns.Add("jul-CA");
                tbl.Columns.Add("jul-SAN");
                tbl.Columns.Add("jul-NOP");


                tbl.Columns.Add("aug-AN");
                tbl.Columns.Add("aug-CA");
                tbl.Columns.Add("aug-SAN");
                tbl.Columns.Add("aug-NOP");


                tbl.Columns.Add("sep-AN");
                tbl.Columns.Add("sep-CA");
                tbl.Columns.Add("sep-SAN");
                tbl.Columns.Add("sep-NOP");

                tbl.Columns.Add("oct-AN");
                tbl.Columns.Add("oct-CA");
                tbl.Columns.Add("oct-SAN");
                tbl.Columns.Add("oct-NOP");

                tbl.Columns.Add("nov-AN");
                tbl.Columns.Add("nov-CA");
                tbl.Columns.Add("nov-SAN");
                tbl.Columns.Add("nov-NOP");

                tbl.Columns.Add("dec-AN");
                tbl.Columns.Add("dec-CA");
                tbl.Columns.Add("dec-SAN");
                tbl.Columns.Add("dec-NOP");

                tbl.Columns.Add("tot-AN");
                tbl.Columns.Add("tot-CA");
                tbl.Columns.Add("tot-SAN");
                tbl.Columns.Add("tot-NOP");

                tbl.Columns.Add("bal-AN");
                tbl.Columns.Add("bal-CA");
                tbl.Columns.Add("bal-SAN");
                tbl.Columns.Add("bal-NOP");


                var startRow = hasHeader ? 8 : 8;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }

        private void GenerateBalanceSummery(List<LeaveBalanceUser> model, int Year, string Division, string Location)
        {
            var fileName = "monthly_leave_summery" + DateTime.Now.Ticks + ".xlsx";
            var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
            string DirectoryPath = Server.MapPath("\\temp_files");
            string UrlPath = "temp_files/" + fileName;
            string logoPath = Server.MapPath("~/Content/Images/sierrs_rpt.png");

            int intSheetIndex = 1;
            int intRowNumber = 8;
            int intLastColumnNumber = 0;
            int intEndColumn = 1;

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            System.IO.FileInfo objFile = new System.IO.FileInfo(path);
            ExcelPackage exlPac = new ExcelPackage(objFile);

            exlPac.Workbook.Worksheets.Add("sheet 1");
            ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

            exlWs.Row(1).Style.Font.Bold = true;
            exlWs.Row(2).Style.Font.Bold = true;
            exlWs.Row(3).Style.Font.Bold = true;
            exlWs.Row(4).Style.Font.Bold = true;
            exlWs.Row(5).Style.Font.Bold = true;
            exlWs.Row(6).Style.Font.Bold = true;
            exlWs.Row(7).Style.Font.Bold = true;

            exlWs.Column(1).Width = 15;
            exlWs.Column(2).Width = 25;
            //exlWs.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


            exlWs.Cells[1, 1, 4, 3].Merge = true;
            Image logo = Image.FromFile(logoPath);
            var picture = exlWs.Drawings.AddPicture("img", logo);
            picture.SetSize(70);
            picture.SetPosition(1, 0, 1, 0);

            exlWs.Cells[1, 4].Value = "Leave Summery Report For Year - " + Year;
            exlWs.Cells[2, 4].Value = Division;
            exlWs.Cells[3, 4].Value = Location;

            exlWs.Cells[6, 1].Value = "Employee No";
            exlWs.Cells[6, 2].Value = "Employee Name";
            exlWs.Cells[6, 3, 6, 4].Value = "Allocated";
            exlWs.Cells[6, 3, 6, 4].Merge = true;
            exlWs.Cells[7, 3].Value = "Annual";
            exlWs.Cells[7, 4].Value = "Cassual";

            foreach (var userMain in model)
            {
                if (!string.IsNullOrEmpty(userMain.EmployeeNumber))
                {
                    int monthColumn = 5;

                    exlWs.Cells[intRowNumber, 1].Value = userMain.EmployeeNumber;
                    exlWs.Cells[intRowNumber, 2].Value = userMain.EmployeeName;
                    exlWs.Cells[intRowNumber, 3].Value = userMain.AnnualAllocated;
                    exlWs.Cells[intRowNumber, 4].Value = userMain.CassualAllocated;

                    foreach (var months in userMain.MonthlyUsedLeaves)
                    {
                        exlWs.Cells[6, monthColumn, 6, monthColumn + 3].Merge = true;
                        exlWs.Cells[6, monthColumn].Value = months.Month_Name;
                        exlWs.Cells[7, monthColumn].Value = "AN";
                        exlWs.Cells[7, monthColumn + 1].Value = "CA";
                        exlWs.Cells[7, monthColumn + 2].Value = "SAN";
                        exlWs.Cells[7, monthColumn + 3].Value = "NOP";

                        if (months.LeaveTypes.Where(x => x.LeaveType_Id == 3000).FirstOrDefault() != null)
                        {
                            var count = months.LeaveTypes.Where(x => x.LeaveType_Id == 3000).FirstOrDefault().Used_Count;

                            if (count > 0 || count < 0)
                            {
                                exlWs.Cells[intRowNumber, monthColumn].Value = count;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, monthColumn].Value = "";
                            }
                        }

                        if (months.LeaveTypes.Where(x => x.LeaveType_Id == 3001).FirstOrDefault() != null)
                        {
                            var count = months.LeaveTypes.Where(x => x.LeaveType_Id == 3001).FirstOrDefault().Used_Count;

                            if (count > 0 || count < 0)
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 1].Value = count;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 1].Value = "";
                            }
                        }

                        if (months.LeaveTypes.Where(x => x.LeaveType_Id == 3010).FirstOrDefault() != null)
                        {
                            var count = months.LeaveTypes.Where(x => x.LeaveType_Id == 3010).FirstOrDefault().Used_Count;

                            if (count > 0 || count < 0)
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 2].Value = count;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 2].Value = "";
                            }
                        }

                        if (months.LeaveTypes.Where(x => x.LeaveType_Id == -1).FirstOrDefault() != null)
                        {
                            var count = months.LeaveTypes.Where(x => x.LeaveType_Id == -1).FirstOrDefault().Used_Count;

                            if (count > 0 || count < 0)
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 3].Value = count;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, monthColumn + 3].Value = "";
                            }
                        }

                        monthColumn += 3;
                        monthColumn++;
                    }


                    foreach (var totalUsed in userMain.TotalUsed)
                    {

                        if (totalUsed.LeaveType_Id == 3000)
                        {
                            exlWs.Cells[intRowNumber, monthColumn].Value = totalUsed.Used_Count;
                        }

                        if (totalUsed.LeaveType_Id == 3001)
                        {
                            exlWs.Cells[intRowNumber, monthColumn + 1].Value = totalUsed.Used_Count;
                        }

                        if (totalUsed.LeaveType_Id == 3010)
                        {
                            exlWs.Cells[intRowNumber, monthColumn + 2].Value = totalUsed.Used_Count;
                        }

                        if (totalUsed.LeaveType_Id == -1)
                        {
                            exlWs.Cells[intRowNumber, monthColumn + 3].Value = totalUsed.Used_Count;
                        }


                    }

                    exlWs.Cells[intRowNumber, monthColumn + 4].Value = userMain.BalanceAnnualLeaves;
                    exlWs.Cells[intRowNumber, monthColumn + 5].Value = userMain.BalanceCassualLeaves;

                    intRowNumber++;
                    intLastColumnNumber = monthColumn;
                }

            }

            exlWs.Cells[6, intLastColumnNumber, 6, intLastColumnNumber + 3].Merge = true;
            exlWs.Cells[6, intLastColumnNumber].Value = "Total";
            exlWs.Cells[7, intLastColumnNumber].Value = "AN";
            exlWs.Cells[7, intLastColumnNumber + 1].Value = "CA";
            exlWs.Cells[7, intLastColumnNumber + 2].Value = "SAN";
            exlWs.Cells[7, intLastColumnNumber + 3].Value = "NOP";

            int balanceCol = intLastColumnNumber + 4;

            exlWs.Cells[6, balanceCol, 6, balanceCol + 3].Merge = true;
            exlWs.Cells[6, balanceCol].Value = "Balance";
            exlWs.Cells[7, balanceCol].Value = "AN";
            exlWs.Cells[7, balanceCol + 1].Value = "CA";
            exlWs.Cells[7, balanceCol + 2].Value = "SAN";
            exlWs.Cells[7, balanceCol + 3].Value = "NOP";

            //Row Styles
            var modelRows = intRowNumber - 1;
            string modelRange = "A6:BH" + modelRows.ToString();
            var modelTable = exlWs.Cells[modelRange];

            // Assign borders
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            ///Footer
            var footerRow = modelRows + 2;
            var prepColumn = "I" + footerRow + ":O" + footerRow;
            var checkedColumn = "AA" + footerRow + ":AG" + footerRow;
            var approvedColumn = "AU" + footerRow + ":AZ" + footerRow;

            exlWs.Cells[prepColumn].Merge = true;
            exlWs.Cells[prepColumn].Style.Font.Bold = true;
            exlWs.Cells[prepColumn].Value = "Prepared By:.......................";

            exlWs.Cells[checkedColumn].Merge = true;
            exlWs.Cells[checkedColumn].Style.Font.Bold = true;
            exlWs.Cells[checkedColumn].Value = "Checked By:.....................";

            exlWs.Cells[approvedColumn].Merge = true;
            exlWs.Cells[approvedColumn].Style.Font.Bold = true;
            exlWs.Cells[approvedColumn].Value = "Approved By:......................";

            ///Datetime
            var footerDateRow = footerRow + 2;
            var msgColumn = "A" + footerDateRow + ":F" + footerDateRow;
            var dateColumn = "BC" + footerDateRow + ":BH" + footerDateRow;

            exlWs.Cells[msgColumn].Merge = true;
            exlWs.Cells[msgColumn].Style.Font.Bold = true;
            exlWs.Cells[msgColumn].Value = "This is a System Generated Report";

            exlWs.Cells[dateColumn].Merge = true;
            exlWs.Cells[dateColumn].Style.Font.Bold = true;
            exlWs.Cells[dateColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            exlWs.Cells[dateColumn].Value = DateTime.Now.ToLongDateString();

            exlPac.Save();

            WebClient req = new WebClient();
            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            byte[] data = req.DownloadData(path);
            response.BinaryWrite(data);
            System.IO.File.Delete(path);
            response.End();
        }

        private void GenerateBalanceSummery(List<EmployeeLeaveSummery> model, int Year, int Month, string TimePeriod, string Division)
        {
            var fileName = "monthly_attendance_summery" + DateTime.Now.Ticks + ".xlsx";
            var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
            string DirectoryPath = Server.MapPath("\\temp_files");
            string UrlPath = "temp_files/" + fileName;
            string logoPath = Server.MapPath("~/Content/Images/sierrs_rpt.png");

            int intSheetIndex = 1;
            int intRowNumber = 8;
            int intLastColumnNumber = 0;
            int intEndColumn = 1;
            int daysInMonth = model.FirstOrDefault().MonthlyLeaveDetails.Count();

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            System.IO.FileInfo objFile = new System.IO.FileInfo(path);
            ExcelPackage exlPac = new ExcelPackage(objFile);

            exlPac.Workbook.Worksheets.Add("sheet 1");
            ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

            exlWs.Row(1).Style.Font.Bold = true;
            exlWs.Row(2).Style.Font.Bold = true;
            exlWs.Row(3).Style.Font.Bold = true;
            exlWs.Row(4).Style.Font.Bold = true;
            exlWs.Row(5).Style.Font.Bold = true;
            exlWs.Row(6).Style.Font.Bold = true;
            exlWs.Row(7).Style.Font.Bold = true;

            exlWs.Column(1).Width = 15;
            exlWs.Column(2).Width = 25;

            exlWs.Cells[1, 1, 4, 3].Merge = true;
            Image logo = Image.FromFile(logoPath);
            var picture = exlWs.Drawings.AddPicture("img", logo);
            picture.SetSize(70);
            picture.SetPosition(1, 0, 1, 0);

            exlWs.Cells[1, 4].Value = "Leave Summery Report For " + TimePeriod;
            exlWs.Cells[2, 4].Value = Division;
            
            exlWs.Cells[6, 1].Value = "Employee No";
            exlWs.Cells[6, 2].Value = "Employee Name";

            for (int i = 1; i <= daysInMonth; i++)
            {
                int dateStartIndex = 2 + i;
                exlWs.Cells[6, dateStartIndex].Value = i;

                var date = new DateTime(Year, Month, i);
                var dayOfWeek = date.DayOfWeek;

                if (dayOfWeek == DayOfWeek.Saturday)
                {
                    exlWs.Cells[7, dateStartIndex].Value = "SAT";
                }
                else if (dayOfWeek == DayOfWeek.Sunday)
                {
                    exlWs.Cells[7, dateStartIndex].Value = "SUN";
                }
            }

            foreach (var userMain in model)
            {
                if (!string.IsNullOrEmpty(userMain.EmployeeNumber))
                {
                    int dateColumnNo = 3;

                    exlWs.Cells[intRowNumber, 1].Value = userMain.EmployeeNumber;
                    exlWs.Cells[intRowNumber, 2].Value = userMain.EmployeeName;

                    foreach (var months in userMain.MonthlyLeaveDetails)
                    {
                        exlWs.Cells[intRowNumber, dateColumnNo].Value = months.AttendanceType;
                        dateColumnNo++;
                    }

                    intRowNumber++;
                    intLastColumnNumber = dateColumnNo;
                }

            }

            exlWs.Cells[6, intLastColumnNumber, 6, intLastColumnNumber + 3].Merge = true;
            exlWs.Cells[6, intLastColumnNumber].Value = "Total";
            exlWs.Cells[7, intLastColumnNumber].Value = "AN";
            exlWs.Cells[7, intLastColumnNumber + 1].Value = "CA";
            exlWs.Cells[7, intLastColumnNumber + 2].Value = "SL";
            exlWs.Cells[7, intLastColumnNumber + 3].Value = "NL";
            

            int totalRowNumber = 8;

            foreach (var userMain in model)
            {
                if (!string.IsNullOrEmpty(userMain.EmployeeNumber))
                {
                    int dateColumnNo = intLastColumnNumber;                    

                    foreach (var months in userMain.LeaveTypes)
                    {
                        if(months.Leave_Name == "AN" || months.Leave_Name == "CA" || months.Leave_Name == "SL" || months.Leave_Name == "NL")
                        {
                            exlWs.Cells[totalRowNumber, dateColumnNo].Value = months.Used_Count;
                            dateColumnNo++;
                        }
                        
                    }

                    totalRowNumber++;
                    //intLastColumnNumber = dateColumnNo;
                }

            }

            //int balanceCol = intLastColumnNumber + 4;

            //exlWs.Cells[6, balanceCol, 6, balanceCol + 3].Merge = true;
            //exlWs.Cells[6, balanceCol].Value = "Balance";
            //exlWs.Cells[7, balanceCol].Value = "AN";
            //exlWs.Cells[7, balanceCol + 1].Value = "CA";
            //exlWs.Cells[7, balanceCol + 2].Value = "SAN";
            //exlWs.Cells[7, balanceCol + 3].Value = "NOP";

            //Row Styles
            var modelRows = intRowNumber - 1;
            string modelRange = "A6:AK" + modelRows.ToString();
            var modelTable = exlWs.Cells[modelRange];

            // Assign borders
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            ///Footer
            var footerRow = modelRows;
            //var footerRow = modelRows + 2;
            //var prepColumn = "F" + footerRow + ":K" + footerRow;
            //var checkedColumn = "U" + footerRow + ":Y" + footerRow;
            //var approvedColumn = "AF" + footerRow + ":AK" + footerRow;

            //exlWs.Cells[prepColumn].Merge = true;
            //exlWs.Cells[prepColumn].Style.Font.Bold = true;
            //exlWs.Cells[prepColumn].Value = "Prepared By:.......................";

            //exlWs.Cells[checkedColumn].Merge = true;
            //exlWs.Cells[checkedColumn].Style.Font.Bold = true;
            //exlWs.Cells[checkedColumn].Value = "Checked By:.....................";

            //exlWs.Cells[approvedColumn].Merge = true;
            //exlWs.Cells[approvedColumn].Style.Font.Bold = true;
            //exlWs.Cells[approvedColumn].Value = "Approved By:......................";

            ///Datetime
            var footerDateRow = footerRow + 2;
            var msgColumn = "A" + footerDateRow + ":H" + footerDateRow;
            var dateColumn = "Q" + footerDateRow + ":S" + footerDateRow;
            var dateColumn1 = "Q" + (footerDateRow +1) + ":S" + (footerDateRow + 1);

            exlWs.Cells[msgColumn].Merge = true;
            exlWs.Cells[msgColumn].Style.Font.Bold = true;
            exlWs.Cells[msgColumn].Value = "This is a System Generated Report.No Signature Required";

            exlWs.Cells[dateColumn].Merge = true;
            exlWs.Cells[dateColumn].Style.Font.Bold = true;
            exlWs.Cells[dateColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //exlWs.Cells[dateColumn].Value = DateTime.Now.ToLongDateString();
            exlWs.Cells[dateColumn].Value = "Sierra Construction Limited";

            exlWs.Cells[dateColumn1].Merge = true;
            exlWs.Cells[dateColumn1].Style.Font.Bold = true;
            exlWs.Cells[dateColumn1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            exlWs.Cells[dateColumn1].Value = DateTime.Now.ToLongDateString();

            ///Powered By Msg var dateColumn = "AH" + footerDateRow + ":AK" + footerDateRow;
            var semsMsgRow = footerDateRow;
            var semsMsgColumn = "AH" + semsMsgRow + ":AK" + semsMsgRow;
            exlWs.Cells[semsMsgColumn].Merge = true;
            exlWs.Cells[semsMsgColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            exlWs.Cells[semsMsgColumn].Style.Font.Bold = true;
            exlWs.Cells[semsMsgColumn].Value = "SEMS Powered by SIOT";

            exlPac.Save();

            WebClient req = new WebClient();
            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            byte[] data = req.DownloadData(path);
            response.BinaryWrite(data);
            System.IO.File.Delete(path);
            response.End();
        }

        public void GetTransferReport(List<EmployeeTransfer> lst, int Year, string ComapnyName = "")
        {
            if (!string.IsNullOrEmpty(ComapnyName))
            {
                if (ComapnyName.Length > 20)
                {
                    ComapnyName = ComapnyName.ToLower().Substring(0, 20);
                }

                var fileName = "transfer_report_" + ComapnyName.ToLower().Replace(" ", "_").Replace("-", "_") + "_" + DateTime.Now.Ticks + ".xlsx";
                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                string DirectoryPath = Server.MapPath("\\temp_files");
                string UrlPath = "temp_files/" + fileName;

                int intSheetIndex = 1;
                int intRowNumber = 4;
                int intColumnNumber = 1;
                int intEndColumn = 1;

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                System.IO.FileInfo objFile = new System.IO.FileInfo(path);
                ExcelPackage exlPac = new ExcelPackage(objFile);



                exlPac.Workbook.Worksheets.Add(ComapnyName);
                ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

                foreach (var item in lst)
                {
                    exlWs.Row(1).Style.Font.Bold = true;
                    exlWs.Row(2).Style.Font.Bold = true;
                    exlWs.Row(3).Style.Font.Bold = true;

                    exlWs.Column(1).Width = 15;
                    exlWs.Column(2).Width = 50;
                    exlWs.Column(3).Width = 30;
                    exlWs.Column(4).Width = 30;
                    exlWs.Column(5).Width = 60;
                    exlWs.Column(6).Width = 30;
                    exlWs.Column(7).Width = 60;
                    exlWs.Column(8).Width = 50;
                    exlWs.Column(9).Width = 30;
                    exlWs.Column(10).Width = 60;
                    exlWs.Column(11).Width = 30;
                    exlWs.Column(12).Width = 50;
                    exlWs.Column(14).Width = 50;
                    exlWs.Column(15).Width = 15;
                    exlWs.Column(16).Width = 15;

                    exlWs.Cells[1, 1].Value = "Employee Transfer Report For Year - " + Year;
                    exlWs.Cells[1, 1, 1, 5].Merge = true;
                    exlWs.Cells[3, 1].Value = "Employee No";
                    exlWs.Cells[3, 2].Value = "Employee Name";
                    exlWs.Cells[3, 3].Value = "Request Date";
                    exlWs.Cells[3, 4].Value = "Effective Date";
                    exlWs.Cells[3, 5].Value = "From Project";
                    exlWs.Cells[3, 6].Value = "From Supervisor";
                    exlWs.Cells[3, 7].Value = "Approval";
                    exlWs.Cells[3, 8].Value = "To Project";
                    exlWs.Cells[3, 9].Value = "To Supervisor";
                    exlWs.Cells[3, 10].Value = "Approval";
                    exlWs.Cells[3, 11].Value = "COO";
                    exlWs.Cells[3, 12].Value = "Approval";
                    exlWs.Cells[3, 13].Value = "Reason";
                    exlWs.Cells[3, 14].Value = "Remarks";
                    exlWs.Cells[3, 15].Value = "Division Code";
                    //exlWs.Cells[3, 16].Value = "Division";

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.RequestDate.ToShortDateString();
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EffectiveDate.ToShortDateString();
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.CurrentDivisionName;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.SupervisorFromName;
                    intColumnNumber += 1;

                    if (item.SupervisorFromApproval == "true")
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Approved";
                        intColumnNumber += 1;
                    }
                    else if (item.SupervisorFromApproval == "false")
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Rejected";
                        intColumnNumber += 1;
                    }
                    else
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Pending";
                        intColumnNumber += 1;
                    }


                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.ToDivisionName;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.SupervisorToName;
                    intColumnNumber += 1;

                    if (item.SupervisorToApproval == "true")
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Approved";
                        intColumnNumber += 1;
                    }
                    else if (item.SupervisorToApproval == "false")
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Rejected";
                        intColumnNumber += 1;
                    }
                    else
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Pending";
                        intColumnNumber += 1;
                    }

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.COOName;
                    intColumnNumber += 1;

                    if (item.COOApproval == "true")
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Approved";
                        intColumnNumber += 1;
                    }
                    else if (item.COOApproval == "false")
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Rejected";
                        intColumnNumber += 1;
                    }
                    else
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = "Pending";
                        intColumnNumber += 1;
                    }

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Reason;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Remarks;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                    intColumnNumber += 1;

                    //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                    //intColumnNumber += 1;

                    intEndColumn = intColumnNumber;
                    intColumnNumber = 1;
                    intRowNumber++;
                }

                ReportStamp(intRowNumber, intEndColumn, exlWs);

                exlPac.Save();

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                System.IO.File.Delete(path);
                response.End();
            }
            else
            {
                _projects pr = new _projects();
                var prLst = pr.GetProjects();

                var fileName = "transfer_report_all_" + DateTime.Now.Ticks + ".xlsx";
                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                string DirectoryPath = Server.MapPath("\\temp_files");
                string UrlPath = "temp_files/" + fileName;

                int intSheetIndex = 1;
                int intRowNumber = 4;
                int intColumnNumber = 1;

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                System.IO.FileInfo objFile = new System.IO.FileInfo(path);
                ExcelPackage exlPac = new ExcelPackage(objFile);

                foreach (var div in prLst)
                {
                    var prjName = div.ProjectName;

                    if (div.ProjectName.Length > 20)
                    {
                        prjName = ComapnyName.ToLower().Substring(0, 20);
                    }

                    exlPac.Workbook.Worksheets.Add(prjName);
                    ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

                    exlWs.Row(1).Style.Font.Bold = true;
                    exlWs.Row(2).Style.Font.Bold = true;
                    exlWs.Row(3).Style.Font.Bold = true;

                    exlWs.Column(1).Width = 15;
                    exlWs.Column(2).Width = 50;
                    exlWs.Column(3).Width = 30;
                    exlWs.Column(4).Width = 30;
                    exlWs.Column(5).Width = 60;
                    exlWs.Column(6).Width = 30;
                    exlWs.Column(7).Width = 60;
                    exlWs.Column(8).Width = 50;
                    exlWs.Column(9).Width = 30;
                    exlWs.Column(10).Width = 60;
                    exlWs.Column(11).Width = 30;
                    exlWs.Column(12).Width = 50;
                    exlWs.Column(14).Width = 50;
                    exlWs.Column(15).Width = 15;

                    exlWs.Cells[1, 1].Value = "Employee Transfer Report For Year - " + Year;
                    exlWs.Cells[1, 1, 1, 5].Merge = true;
                    exlWs.Cells[3, 1].Value = "Employee No";
                    exlWs.Cells[3, 2].Value = "Employee Name";
                    exlWs.Cells[3, 3].Value = "Request Date";
                    exlWs.Cells[3, 4].Value = "Effective Date";
                    exlWs.Cells[3, 5].Value = "From Project";
                    exlWs.Cells[3, 6].Value = "From Supervisor";
                    exlWs.Cells[3, 7].Value = "Approval";
                    exlWs.Cells[3, 8].Value = "To Project";
                    exlWs.Cells[3, 9].Value = "To Supervisor";
                    exlWs.Cells[3, 10].Value = "Approval";
                    exlWs.Cells[3, 11].Value = "COO";
                    exlWs.Cells[3, 12].Value = "Approval";
                    exlWs.Cells[3, 13].Value = "Reason";
                    exlWs.Cells[3, 14].Value = "Remarks";
                    exlWs.Cells[3, 15].Value = "Division Code";

                    foreach (var item in lst.Where(x => x.TransferTo == div.Id))
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.RequestDate.ToShortDateString();
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EffectiveDate.ToShortDateString();
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.CurrentDivisionName;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.SupervisorFromName;
                        intColumnNumber += 1;

                        if (item.SupervisorFromApproval == "true")
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Approved";
                            intColumnNumber += 1;
                        }
                        else if (item.SupervisorFromApproval == "false")
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Rejected";
                            intColumnNumber += 1;
                        }
                        else
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Pending";
                            intColumnNumber += 1;
                        }


                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.ToDivisionName;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.SupervisorToName;
                        intColumnNumber += 1;

                        if (item.SupervisorToApproval == "true")
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Approved";
                            intColumnNumber += 1;
                        }
                        else if (item.SupervisorToApproval == "false")
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Rejected";
                            intColumnNumber += 1;
                        }
                        else
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Pending";
                            intColumnNumber += 1;
                        }

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.COOName;
                        intColumnNumber += 1;

                        if (item.COOApproval == "true")
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Approved";
                            intColumnNumber += 1;
                        }
                        else if (item.COOApproval == "false")
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Rejected";
                            intColumnNumber += 1;
                        }
                        else
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = "Pending";
                            intColumnNumber += 1;
                        }

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Reason;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Remarks;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                        intColumnNumber += 1;

                        intColumnNumber = 1;
                        intRowNumber++;
                    }

                    ReportStamp(intRowNumber, intColumnNumber, exlWs);

                    intSheetIndex++;
                    intColumnNumber = 1;
                    intRowNumber = 4;
                }


                exlPac.Save();

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                System.IO.File.Delete(path);
                response.End();
            }

        }

        private static void ReportStamp(int intRowNumber, int columnNumber, ExcelWorksheet exlWs)
        {
            var stampRow = intRowNumber + 5;

            string msgStr1 = "This is a system generated document. No signature is required";

            string msgStr2 = "Sierra Construction Ltd, Telecom Engineering Division" + "\r\nPrinted on " + DateTime.Now.ToLongDateString();

            string msgStr3 = "SEMS powered by SIOT";

            columnNumber -= 1;
            int midCol = columnNumber / 3 * 2;
            int colLength = columnNumber / 3;
            int midColStart = colLength + 1;
            int lastColStart = midCol + 1;

            exlWs.Row(stampRow).Style.Font.Bold = true;
            exlWs.Row(stampRow).Style.Font.Italic = true;
            exlWs.Row(stampRow).Style.WrapText = true;
            exlWs.Row(stampRow).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            exlWs.Row(stampRow).Height = 50;


            exlWs.Cells[stampRow, 1, stampRow, colLength].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            exlWs.Cells[stampRow, midColStart, stampRow, midCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            exlWs.Cells[stampRow, lastColStart, stampRow, columnNumber].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

            exlWs.Cells[stampRow, 1, stampRow, colLength].Merge = true;
            exlWs.Cells[stampRow, midColStart, stampRow, midCol].Merge = true;
            exlWs.Cells[stampRow, lastColStart, stampRow, columnNumber].Merge = true;

            exlWs.Cells[stampRow, 1].Value = msgStr1;
            exlWs.Cells[stampRow, midColStart].Value = msgStr2;
            exlWs.Cells[stampRow, lastColStart].Value = msgStr3;
        }

        public void GetInOutReport(List<AttendenceCorrections> lst, int divisionId, DateTime dateFrom, DateTime dateTo)
        {
            _projects pr = new _projects();

            if (divisionId == 0)
            {
                var prLst = pr.GetProjects();

                var fileName = "inout_report_all_" + DateTime.Now.Ticks + ".xlsx";
                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                string DirectoryPath = Server.MapPath("\\temp_files");
                string UrlPath = "temp_files/" + fileName;

                int intSheetIndex = 1;
                int intRowNumber = 4;
                int intColumnNumber = 1;
                int intEndColumn = 1;

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                System.IO.FileInfo objFile = new System.IO.FileInfo(path);
                ExcelPackage exlPac = new ExcelPackage(objFile);

                foreach (var div in prLst)
                {
                    exlPac.Workbook.Worksheets.Add(div.ProjectName);
                    ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

                    exlWs.Row(1).Style.Font.Bold = true;
                    exlWs.Row(2).Style.Font.Bold = true;
                    exlWs.Row(3).Style.Font.Bold = true;

                    exlWs.Column(1).Width = 15;
                    exlWs.Column(2).Width = 50;
                    exlWs.Column(3).Width = 30;
                    exlWs.Column(4).Width = 30;
                    exlWs.Column(5).Width = 15;
                    exlWs.Column(6).Width = 30;
                    exlWs.Column(7).Width = 30;
                    exlWs.Column(8).Width = 50;
                    exlWs.Column(9).Width = 30;
                    exlWs.Column(10).Width = 60;
                    exlWs.Column(11).Width = 30;
                    exlWs.Column(12).Width = 15;
                    exlWs.Column(13).Width = 15;
                    exlWs.Column(14).Width = 15;
                    exlWs.Column(15).Width = 15;

                    exlWs.Cells[1, 1].Value = "In/Out Correction Report From " + dateFrom.ToShortDateString() + " to " + dateTo.ToShortDateString();
                    exlWs.Cells[1, 1, 1, 5].Merge = true;

                    exlWs.Cells[3, 1].Value = "Employee No";
                    exlWs.Cells[3, 2].Value = "Employee Name";
                    exlWs.Cells[3, 3].Value = "Designation";
                    exlWs.Cells[3, 4].Value = "Location";
                    exlWs.Cells[3, 5].Value = "Division Code";
                    exlWs.Cells[3, 6].Value = "Division Name";
                    exlWs.Cells[3, 7].Value = "Date";
                    exlWs.Cells[3, 8].Value = "In Time";
                    exlWs.Cells[3, 9].Value = "In Reason";
                    exlWs.Cells[3, 10].Value = "Out Time";
                    exlWs.Cells[3, 11].Value = "Out Reason";
                    exlWs.Cells[3, 12].Value = "Supervisor";
                    exlWs.Cells[3, 13].Value = "Status";
                    exlWs.Cells[3, 14].Value = "Remarks";
                    exlWs.Cells[3, 15].Value = "Date Requested";

                    foreach (var sub in lst.Where(x => x.DivisionId == div.Id))
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.EmpNo;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Username;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Designation ?? "";
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Location ?? "";
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.DivisionCode;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.DivisionName;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Date.ToShortDateString();
                        intColumnNumber += 1;

                        if (sub.InTime.TimeOfDay == TimeSpan.Zero)
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = string.Empty;
                            intColumnNumber += 1;
                        }
                        else
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.InTime.ToShortTimeString();
                            intColumnNumber += 1;
                        }

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.InReason;
                        intColumnNumber += 1;


                        if (sub.OutTime.TimeOfDay == TimeSpan.Zero)
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = string.Empty;
                            intColumnNumber += 1;
                        }
                        else
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.OutTime.ToShortTimeString();
                            intColumnNumber += 1;
                        }

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.OutReason;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.SupervisorName;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.IsApproved;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Remarks;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.DateRequested.ToShortDateString();

                        intRowNumber++;
                        intEndColumn = intColumnNumber;
                        intColumnNumber = 1;
                    }

                    ReportStamp(intRowNumber, intEndColumn, exlWs);

                    intColumnNumber = 1;
                    intRowNumber = 4;
                    intSheetIndex++;
                }

                exlPac.Save();

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                System.IO.File.Delete(path);
                response.End();
            }
            else
            {
                var prLst = pr.GetProject(divisionId);
                string fileName = "inout_report_" + prLst.ProjectName.ToLower().Replace(" ", "_") + "_" + DateTime.Now.Ticks.ToString().Substring(0, 4) + ".xlsx";

                if (prLst.ProjectName.ToLower().Length > 20)
                {
                    fileName = "inout_report_" + prLst.ProjectName.ToLower().Substring(0, 20).Replace(" ", "_") + "_" + DateTime.Now.Ticks.ToString().Substring(0, 4) + ".xlsx";
                }

                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                string DirectoryPath = Server.MapPath("\\temp_files");
                string UrlPath = "temp_files/" + fileName;

                int intSheetIndex = 1;
                int intRowNumber = 4;
                int intColumnNumber = 1;
                int intEndColumn = 1;

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                System.IO.FileInfo objFile = new System.IO.FileInfo(path);
                ExcelPackage exlPac = new ExcelPackage(objFile);

                exlPac.Workbook.Worksheets.Add(prLst.ProjectName);
                ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

                exlWs.Row(1).Style.Font.Bold = true;
                exlWs.Row(2).Style.Font.Bold = true;
                exlWs.Row(3).Style.Font.Bold = true;

                exlWs.Column(1).Width = 15;
                exlWs.Column(2).Width = 50;
                exlWs.Column(3).Width = 30;
                exlWs.Column(4).Width = 30;
                exlWs.Column(5).Width = 15;
                exlWs.Column(6).Width = 30;
                exlWs.Column(7).Width = 30;
                exlWs.Column(8).Width = 50;
                exlWs.Column(9).Width = 30;
                exlWs.Column(10).Width = 60;
                exlWs.Column(11).Width = 30;
                exlWs.Column(12).Width = 15;
                exlWs.Column(13).Width = 15;
                exlWs.Column(14).Width = 15;
                exlWs.Column(15).Width = 15;

                exlWs.Cells[1, 1].Value = "In/Out Correction Report From " + dateFrom.ToShortDateString() + " to " + dateTo.ToShortDateString();
                exlWs.Cells[1, 1, 1, 5].Merge = true;

                exlWs.Cells[3, 1].Value = "Employee No";
                exlWs.Cells[3, 2].Value = "Employee Name";
                exlWs.Cells[3, 3].Value = "Designation";
                exlWs.Cells[3, 4].Value = "Location";
                exlWs.Cells[3, 5].Value = "Division Code";
                exlWs.Cells[3, 6].Value = "Division Name";
                exlWs.Cells[3, 7].Value = "Date";
                exlWs.Cells[3, 8].Value = "In Time";
                exlWs.Cells[3, 9].Value = "In Reason";
                exlWs.Cells[3, 10].Value = "Out Time";
                exlWs.Cells[3, 11].Value = "Out Reason";
                exlWs.Cells[3, 12].Value = "Supervisor";
                exlWs.Cells[3, 13].Value = "Status";
                exlWs.Cells[3, 14].Value = "Remarks";
                exlWs.Cells[3, 15].Value = "Date Requested";

                foreach (var sub in lst)
                {
                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.EmpNo;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Username;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Designation ?? "";
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Location ?? "";
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.DivisionCode;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.DivisionName;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Date.ToShortDateString();
                    intColumnNumber += 1;

                    if (sub.InTime.TimeOfDay == TimeSpan.Zero)
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = string.Empty;
                        intColumnNumber += 1;
                    }
                    else
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.InTime.ToShortTimeString();
                        intColumnNumber += 1;
                    }

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.InReason;
                    intColumnNumber += 1;


                    if (sub.OutTime.TimeOfDay == TimeSpan.Zero)
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = string.Empty;
                        intColumnNumber += 1;
                    }
                    else
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.OutTime.ToShortTimeString();
                        intColumnNumber += 1;
                    }


                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.OutReason;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.SupervisorName;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.IsApproved;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Remarks;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.DateRequested.ToShortDateString();

                    intEndColumn = intColumnNumber;
                    intRowNumber++;
                    intColumnNumber = 1;
                }

                ReportStamp(intRowNumber, intEndColumn, exlWs);

                exlPac.Save();

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                System.IO.File.Delete(path);
                response.End();
            }
        }

        public void GetLeaveReport(List<LeaveReportViewModel> lst, int divisionId, int year)
        {
            _projects pr = new _projects();
            _leavs lv = new _leavs();

            if (divisionId == 0)
            {
                var prLst = pr.GetProjects();

                var leaves = lv.GetLeaves();

                var fileName = "leave_report_all_" + DateTime.Now.Ticks + ".xlsx";
                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                string DirectoryPath = Server.MapPath("\\temp_files");
                string UrlPath = "temp_files/" + fileName;

                int intSheetIndex = 1;
                int intRowNumber = 5;
                int intColumnNumber = 1;
                int intEndColumn = 1;

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                System.IO.FileInfo objFile = new System.IO.FileInfo(path);
                ExcelPackage exlPac = new ExcelPackage(objFile);

                foreach (var div in prLst)
                {
                    exlPac.Workbook.Worksheets.Add(div.ProjectName);
                    ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

                    exlWs.Row(1).Style.Font.Bold = true;
                    exlWs.Row(2).Style.Font.Bold = true;
                    exlWs.Row(3).Style.Font.Bold = true;
                    exlWs.Row(4).Style.Font.Bold = true;

                    exlWs.Column(1).Width = 15;
                    exlWs.Column(2).Width = 30;
                    exlWs.Column(3).Width = 30;
                    exlWs.Column(4).Width = 30;
                    exlWs.Column(5).Width = 30;
                    exlWs.Column(6).Width = 15;
                    exlWs.Column(7).Width = 30;

                    exlWs.Cells[1, 1].Value = "Leave Report For Year - " + year;
                    exlWs.Cells[1, 1, 1, 5].Merge = true;
                    exlWs.Cells[3, 1].Value = "Employee No";
                    exlWs.Cells[3, 2].Value = "Employee Name";
                    exlWs.Cells[3, 3].Value = "Designation";
                    exlWs.Cells[3, 4].Value = "Location";
                    exlWs.Cells[3, 5].Value = "Supervisor";
                    exlWs.Cells[3, 6].Value = "Division Code";
                    exlWs.Cells[3, 7].Value = "Division Name";

                    int leaveIndex = 8;

                    foreach (var item in leaves)
                    {
                        exlWs.Column(leaveIndex).Width = 10;
                        exlWs.Column(leaveIndex + 1).Width = 10;

                        if (item.Id == 3000 || item.Id == 3001)
                        {
                            try
                            {
                                exlWs.Cells[3, leaveIndex].Value = item.LeaveType1;
                                exlWs.Cells[3, leaveIndex, 3, leaveIndex + 1].Merge = true;
                            }
                            catch
                            {

                            }

                            exlWs.Cells[4, leaveIndex].Value = "Allocated";
                            exlWs.Cells[4, leaveIndex + 1].Value = "Remaining";

                            leaveIndex += 2;
                        }
                        else
                        {
                            exlWs.Cells[3, leaveIndex].Value = item.LeaveType1;
                            exlWs.Cells[4, leaveIndex].Value = "Day Count";
                            leaveIndex++;
                        }

                    }

                    exlWs.Cells[3, leaveIndex].Value = "Year";

                    foreach (var item in lst.Where(x => x.DivisionId == div.Id))
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Designation;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Location;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Supervisor;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionName;
                        intColumnNumber += 1;

                        foreach (var sub in leaves)
                        {
                            if (sub.Id == 3000 || sub.Id == 3001)
                            {
                                if (item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault() != null)
                                {
                                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().AllocatedCount;
                                    intColumnNumber += 1;

                                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().RemainingCount;
                                    intColumnNumber += 1;
                                }
                                else
                                {
                                    exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
                                    intColumnNumber += 1;

                                    exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
                                    intColumnNumber += 1;
                                }
                            }
                            else
                            {
                                if (item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault() != null)
                                {
                                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().RemainingCount;
                                    intColumnNumber += 1;
                                }
                                else
                                {
                                    exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
                                    intColumnNumber += 1;
                                }
                            }


                        }

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = year;
                        intColumnNumber += 1;

                        intRowNumber++;
                        intEndColumn = intColumnNumber;
                        intColumnNumber = 1;
                    }

                    ReportStamp(intRowNumber, intEndColumn, exlWs);

                    intSheetIndex++;
                    intRowNumber = 5;
                }

                exlPac.Save();

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                System.IO.File.Delete(path);
                response.End();

            }
            else
            {
                var prLst = pr.GetProject(divisionId);
                var leaves = lv.GetLeaves();

                var fileName = "leave_report_" + prLst.ProjectName.ToLower() + "_" + DateTime.Now.Ticks + ".xlsx";

                if (prLst.ProjectName.ToLower().Length > 20)
                {
                    fileName = "inout_report_" + prLst.ProjectName.ToLower().Substring(0, 20).Replace(" ", "_") + "_" + DateTime.Now.Ticks.ToString().Substring(0, 4) + ".xlsx";
                }

                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                string DirectoryPath = Server.MapPath("\\temp_files");
                string UrlPath = "temp_files/" + fileName;

                int intSheetIndex = 1;
                int intRowNumber = 5;
                int intColumnNumber = 1;
                int intEndColumn = 1;

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                System.IO.FileInfo objFile = new System.IO.FileInfo(path);
                ExcelPackage exlPac = new ExcelPackage(objFile);

                exlPac.Workbook.Worksheets.Add(prLst.ProjectName);
                ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

                exlWs.Row(1).Style.Font.Bold = true;
                exlWs.Row(2).Style.Font.Bold = true;
                exlWs.Row(3).Style.Font.Bold = true;
                exlWs.Row(4).Style.Font.Bold = true;

                exlWs.Column(1).Width = 15;
                exlWs.Column(2).Width = 30;
                exlWs.Column(3).Width = 30;
                exlWs.Column(4).Width = 30;
                exlWs.Column(5).Width = 30;
                exlWs.Column(6).Width = 15;
                exlWs.Column(7).Width = 30;

                exlWs.Cells[1, 1].Value = "Leave Report For Year - " + year;
                exlWs.Cells[1, 1, 1, 5].Merge = true;
                exlWs.Cells[3, 1].Value = "Employee No";
                exlWs.Cells[3, 2].Value = "Employee Name";
                exlWs.Cells[3, 3].Value = "Designation";
                exlWs.Cells[3, 4].Value = "Location";
                exlWs.Cells[3, 5].Value = "Supervisor";
                exlWs.Cells[3, 6].Value = "Division Code";
                exlWs.Cells[3, 7].Value = "Division Name";

                int leaveIndex = 8;

                foreach (var item in leaves)
                {
                    exlWs.Column(leaveIndex).Width = 10;
                    exlWs.Column(leaveIndex + 1).Width = 10;

                    if (item.Id == 3000 || item.Id == 3001)
                    {
                        try
                        {
                            exlWs.Cells[3, leaveIndex].Value = item.LeaveType1;
                            exlWs.Cells[3, leaveIndex, 3, leaveIndex + 1].Merge = true;
                        }
                        catch
                        {

                        }

                        exlWs.Cells[4, leaveIndex].Value = "Allocated";
                        exlWs.Cells[4, leaveIndex + 1].Value = "Remaining";

                        leaveIndex += 2;
                    }
                    else
                    {
                        exlWs.Cells[3, leaveIndex].Value = item.LeaveType1;
                        exlWs.Cells[4, leaveIndex].Value = "Day Count";
                        leaveIndex++;
                    }
                }

                exlWs.Cells[3, leaveIndex].Value = "Year";

                foreach (var item in lst)
                {
                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Designation;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Location;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Supervisor;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                    intColumnNumber += 1;

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionName;
                    intColumnNumber += 1;

                    foreach (var sub in leaves)
                    {


                        if (sub.Id == 3000 || sub.Id == 3001)
                        {
                            if (item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault() != null)
                            {
                                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().AllocatedCount;
                                intColumnNumber += 1;

                                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().RemainingCount;
                                intColumnNumber += 1;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
                                intColumnNumber += 1;

                                exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
                                intColumnNumber += 1;
                            }
                        }
                        else
                        {
                            if (item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault() != null)
                            {
                                exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Leaves.Where(x => x.LeaveId == sub.Id).FirstOrDefault().RemainingCount;
                                intColumnNumber += 1;
                            }
                            else
                            {
                                exlWs.Cells[intRowNumber, intColumnNumber].Value = 0;
                                intColumnNumber += 1;
                            }
                        }
                    }

                    exlWs.Cells[intRowNumber, intColumnNumber].Value = year;
                    intColumnNumber += 1;

                    intEndColumn = intColumnNumber;
                    intRowNumber++;
                    intColumnNumber = 1;
                }

                ReportStamp(intRowNumber, intEndColumn, exlWs);

                exlPac.Save();

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                System.IO.File.Delete(path);
                response.End();
            }
        }

        public void GetLeaveDetailsReport(List<LeaveReportDetailsViewModel> lst, int divisionId, string Date)
        {
            _projects pr = new _projects();
            _leavs lv = new _leavs();

            if (divisionId == 0)
            {
                var prLst = pr.GetProjects();

                var leaves = lv.GetLeaves();

                var fileName = "leave_report_details_all_" + DateTime.Now.Ticks + ".xlsx";

                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                string DirectoryPath = Server.MapPath("\\temp_files");
                string UrlPath = "temp_files/" + fileName;

                int intSheetIndex = 1;
                int intRowNumber = 5;
                int intColumnNumber = 1;
                int intEndColumn = 1;

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                System.IO.FileInfo objFile = new System.IO.FileInfo(path);
                ExcelPackage exlPac = new ExcelPackage(objFile);

                foreach (var prj in prLst)
                {
                    var sheetName = prj.ProjectName;

                    if (prj.ProjectName.ToLower().Length > 20)
                    {
                        sheetName = prj.ProjectName.ToLower().Substring(0, 20);
                    }

                    exlPac.Workbook.Worksheets.Add(sheetName);
                    ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

                    exlWs.Row(1).Style.Font.Bold = true;
                    exlWs.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    exlWs.Row(2).Style.Font.Bold = true;

                    exlWs.Column(1).Width = 15;
                    exlWs.Column(2).Width = 30;
                    exlWs.Column(3).Width = 30;
                    exlWs.Column(4).Width = 30;
                    exlWs.Column(5).Width = 30;
                    exlWs.Column(6).Width = 30;
                    exlWs.Column(7).Width = 30;
                    exlWs.Column(8).Width = 30;
                    exlWs.Column(9).Width = 30;
                    exlWs.Column(10).Width = 30;
                    exlWs.Column(11).Width = 30;
                    exlWs.Column(12).Width = 30;
                    exlWs.Column(13).Width = 30;
                    exlWs.Column(14).Width = 30;
                    exlWs.Column(15).Width = 30;
                    exlWs.Column(16).Width = 30;

                    //User Details
                    exlWs.Cells[1, 1].Value = "Employee";
                    exlWs.Cells[1, 1, 1, 5].Merge = true;
                    exlWs.Cells[2, 1].Value = "Employee No";
                    exlWs.Cells[2, 2].Value = "Employee Name";
                    exlWs.Cells[2, 3].Value = "Designation";
                    exlWs.Cells[2, 4].Value = "Location";
                    //exlWs.Cells[3, 5].Value = "Supervisor";
                    exlWs.Cells[2, 5].Value = "Division Code";
                    exlWs.Cells[2, 6].Value = "Division Name";

                    //Leave details
                    exlWs.Cells[1, 7].Value = "Leaves";
                    exlWs.Cells[1, 7, 1, 14].Merge = true;
                    exlWs.Cells[2, 7].Value = "Leave Type";
                    exlWs.Cells[2, 8].Value = "Leave Unit";
                    exlWs.Cells[2, 9].Value = "Request Date";
                    exlWs.Cells[2, 10].Value = "From";
                    exlWs.Cells[2, 11].Value = "To";
                    exlWs.Cells[2, 12].Value = "Days";
                    exlWs.Cells[2, 13].Value = "Reason for Leave";
                    exlWs.Cells[2, 14].Value = "Is Approved?";
                    exlWs.Cells[2, 15].Value = "Approved by";
                    exlWs.Cells[2, 16].Value = "Remark";

                    foreach (var item in lst.Where(x => x.DivisionId == prj.Id))
                    {
                        //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
                        //intColumnNumber += 1;

                        //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
                        //intColumnNumber += 1;

                        //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Designation;
                        //intColumnNumber += 1;

                        //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Location;
                        //intColumnNumber += 1;

                        ////exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Supervisor;
                        ////intColumnNumber += 1;

                        //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                        //intColumnNumber += 1;

                        foreach (var sub in item.Leaves)
                        {
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Designation;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Location;
                            intColumnNumber += 1;

                            //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Supervisor;
                            //intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionName;
                            intColumnNumber += 1;
                            ////////////////////////////////////////////////////////////////////////////////////////////////////////
                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.LeaveTypeStr;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.LeaveUnit;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.RequestDate.ToShortDateString();
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.FromDate.ToShortDateString();
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.ToDate.ToShortDateString();
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.LeaveDays;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Reason;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.IsApproved;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.ApprovedBy;
                            intColumnNumber += 1;

                            exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Remarks ?? "No Remarks";
                            intColumnNumber += 1;

                            intRowNumber++;
                            intEndColumn = intColumnNumber;
                            //intColumnNumber = 6;
                            intColumnNumber = 1;
                        }

                        //intEndColumn = intColumnNumber;
                        intColumnNumber = 1;
                        //intRowNumber = 3;

                    }

                    ReportStamp(intRowNumber, intEndColumn, exlWs);

                    intSheetIndex++;
                    intRowNumber = 3;
                }

                exlPac.Save();

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                System.IO.File.Delete(path);
                response.End();
            }
            else
            {
                var prLst = pr.GetProject(divisionId);
                var leaves = lv.GetLeaves();

                var fileName = "leave_report_details_" + prLst.ProjectName.ToLower() + "_" + DateTime.Now.Ticks + ".xlsx";

                if (prLst.ProjectName.ToLower().Length > 20)
                {
                    fileName = "inout_report_" + prLst.ProjectName.ToLower().Substring(0, 20).Replace(" ", "_") + "_" + DateTime.Now.Ticks.ToString().Substring(0, 4) + ".xlsx";
                }

                var path = Path.Combine(Server.MapPath("~/temp_files"), fileName);
                string DirectoryPath = Server.MapPath("\\temp_files");
                string UrlPath = "temp_files/" + fileName;

                int intSheetIndex = 1;
                int intRowNumber = 3;
                int intColumnNumber = 1;
                int intEndColumn = 1;

                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                System.IO.FileInfo objFile = new System.IO.FileInfo(path);
                ExcelPackage exlPac = new ExcelPackage(objFile);

                exlPac.Workbook.Worksheets.Add(prLst.ProjectName);
                ExcelWorksheet exlWs = exlPac.Workbook.Worksheets[intSheetIndex];

                exlWs.Row(1).Style.Font.Bold = true;
                exlWs.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                exlWs.Row(2).Style.Font.Bold = true;

                exlWs.Column(1).Width = 15;
                exlWs.Column(2).Width = 30;
                exlWs.Column(3).Width = 30;
                exlWs.Column(4).Width = 30;
                exlWs.Column(5).Width = 30;
                exlWs.Column(6).Width = 30;
                exlWs.Column(7).Width = 30;
                exlWs.Column(8).Width = 30;
                exlWs.Column(9).Width = 30;
                exlWs.Column(10).Width = 30;
                exlWs.Column(11).Width = 30;
                exlWs.Column(12).Width = 30;
                exlWs.Column(13).Width = 30;
                exlWs.Column(14).Width = 30;
                exlWs.Column(15).Width = 30;
                exlWs.Column(16).Width = 30;

                //User Details
                exlWs.Cells[1, 1].Value = "Employee";
                exlWs.Cells[1, 1, 1, 5].Merge = true;
                exlWs.Cells[2, 1].Value = "Employee No";
                exlWs.Cells[2, 2].Value = "Employee Name";
                exlWs.Cells[2, 3].Value = "Designation";
                exlWs.Cells[2, 4].Value = "Location";
                //exlWs.Cells[3, 5].Value = "Supervisor";
                exlWs.Cells[2, 5].Value = "Division Code";
                exlWs.Cells[2, 6].Value = "Division Name";

                //Leave details
                exlWs.Cells[1, 7].Value = "Leaves";
                exlWs.Cells[1, 7, 1, 14].Merge = true;
                exlWs.Cells[2, 7].Value = "Leave Type";
                exlWs.Cells[2, 8].Value = "Leave Unit";
                exlWs.Cells[2, 9].Value = "Request Date";
                exlWs.Cells[2, 10].Value = "From";
                exlWs.Cells[2, 11].Value = "To";
                exlWs.Cells[2, 12].Value = "Days";
                exlWs.Cells[2, 13].Value = "Reason for Leave";
                exlWs.Cells[2, 14].Value = "Is Approved?";
                exlWs.Cells[2, 15].Value = "Approved by";
                exlWs.Cells[2, 16].Value = "Remark";

                foreach (var item in lst)
                {
                    //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
                    //intColumnNumber += 1;

                    //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
                    //intColumnNumber += 1;

                    //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Designation;
                    //intColumnNumber += 1;

                    //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Location;
                    //intColumnNumber += 1;

                    ////exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Supervisor;
                    ////intColumnNumber += 1;

                    //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                    //intColumnNumber += 1;

                    foreach (var sub in item.Leaves)
                    {
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.EmpNo;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Username;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Designation;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Location;
                        intColumnNumber += 1;

                        //exlWs.Cells[intRowNumber, intColumnNumber].Value = item.Supervisor;
                        //intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionCode;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = item.DivisionName;
                        intColumnNumber += 1;
                        /////////////////////////////////////////////////////////////////////////////////////////
                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.LeaveTypeStr;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.LeaveUnit;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.RequestDate.ToShortDateString();
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.FromDate.ToShortDateString();
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.ToDate.ToShortDateString();
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.LeaveDays;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Reason;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.IsApproved;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.ApprovedBy;
                        intColumnNumber += 1;

                        exlWs.Cells[intRowNumber, intColumnNumber].Value = sub.Remarks ?? "No Remarks";
                        intColumnNumber += 1;

                        intRowNumber++;
                        intEndColumn = intColumnNumber;
                        //intColumnNumber = 6;
                        intColumnNumber = 1;
                    }

                    //intEndColumn = intColumnNumber;
                    intColumnNumber = 1;
                    //intRowNumber = 3;
                }

                ReportStamp(intRowNumber, intEndColumn, exlWs);

                exlPac.Save();

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                System.IO.File.Delete(path);
                response.End();
            }
        }

        [HttpGet]
        public ActionResult InOut()
        {
            Reports rptmodel = new Reports();
            LastResult = null;
            rptmodel.InOutResult = new List<Models.InOut>();
            return View(rptmodel);
        }
        public static Reports LastResult = new Reports();
        public static String LastExcelPath = "";
        [HttpPost]
        public ActionResult InOut(Reports rptmodel)
        {
            if (Request.Form["view"] != null)
            {
                rptmodel.InOutResult = new _reports().ViewInOutReport(rptmodel);
                LastResult = rptmodel;
            }
            else if (Request.Form["excel"] != null)
            {
                if (LastResult != null)
                {
                    Guid g = Guid.NewGuid();
                    string excelPath = Server.MapPath("~/Content/TempFile/AttendenceCorrections.xlsx");
                    string logoPath = Server.MapPath("~/Content/Images/login_main.png");

                    List<InOut> lst = LastResult.InOutResult;
                    rptmodel.InOutResult = LastResult.InOutResult;
                    LastExcelPath = new _reports().ReportToExcel(LastResult, excelPath, logoPath);

                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=\"" + Path.GetFileName(LastExcelPath) + "\"");
                    byte[] data = req.DownloadData(LastExcelPath);
                    response.BinaryWrite(data);
                    System.IO.File.Delete(LastExcelPath);
                    response.End();
                }
            }

            return View(rptmodel);
        }

        [HttpGet]
        public ActionResult Leave()
        {
            LeaveViewmodel rptmodel = new LeaveViewmodel();
            rptmodel.LeaveResult = new List<Models.Leave>();
            LeaveLastResult = null;
            return View(rptmodel);
        }

        private static LeaveViewmodel LeaveLastResult = new LeaveViewmodel();
        private static string LeaveLastExcelpatht = "";
        [HttpPost]
        public ActionResult Leave(LeaveViewmodel rptmode)
        {
            if (Request.Form["view"] != null)
            {
                rptmode.LeaveResult = new _reports().ViewLeaveReport(rptmode);
                LeaveLastResult = rptmode;
            }
            else if (Request.Form["excel"] != null)
            {
                if (LeaveLastResult != null)
                {
                    Guid g = Guid.NewGuid();
                    string excelPath = Server.MapPath("~/Content/TempFile/LeaveHistory.xlsx");
                    string logoPath = Server.MapPath("~/Content/Images/login_main.png");
                    LeaveLastExcelpatht = new _reports().LeaveExcel(LeaveLastResult, excelPath, logoPath);
                    rptmode.LeaveResult = LeaveLastResult.LeaveResult;

                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=\"" + Path.GetFileName(LeaveLastExcelpatht) + "\"");
                    byte[] data = req.DownloadData(LeaveLastExcelpatht);
                    response.BinaryWrite(data);
                    System.IO.File.Delete(LeaveLastExcelpatht);
                    response.End();
                }
            }
            return View(rptmode);
        }

        public ActionResult LeaveBalance(FormCollection col)
        {
            var divisionId = Convert.ToInt32(col["ddlDivision"].ToString());
            var year = Convert.ToInt32(col["ddlYear"].ToString());

            _leavs lv = new _leavs();
            //var rpt = lv.GetLeaveBalance(divisionId, year);
            return RedirectToAction("Index");
        }
    }
}