using EmployeeTracking.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
namespace EmployeeTracking.App_Codes
{
    public class _reports
    {

        public List<SiteLocation> GetAllSiteLocations()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    return db.SiteLocations.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<SiteLocation>();
            }
        }

        public List<InOut> ViewInOutReport(Reports rptmodel)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var model = (from att in db.AttendenceCorrections
                                 join us in db.UserProfiles
                                 on att.UserId equals us.Id
                                 join emp in db.EmployeementInfos
                                 on us.Id equals emp.Id
                                 where
                                   (rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0" && rptmodel.Status == "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To :
                                  (rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0" && rptmodel.Status != "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && att.IsApproved == rptmodel.Status :
                                  (rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && att.IsApproved == rptmodel.Status && emp.DivisionCode == rptmodel.DivisionCode :
                                  (rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && att.IsApproved == rptmodel.Status && emp.DivisionCode == rptmodel.DivisionCode && emp.PresentReportingLocation == rptmodel.LocationName :
                                  (rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && att.IsApproved == rptmodel.Status && emp.DivisionCode == rptmodel.DivisionCode && emp.PresentReportingLocation == rptmodel.LocationName && emp.Division == rptmodel.Division :
                                  (rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0" && rptmodel.Status != "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && att.IsApproved == rptmodel.Status && emp.PresentReportingLocation == rptmodel.LocationName :
                                  (rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0" && rptmodel.Status == "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode && emp.PresentReportingLocation == rptmodel.LocationName :
                                  (rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode :
                                  (rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && att.IsApproved == rptmodel.Status && emp.DivisionCode == rptmodel.DivisionCode && emp.Division == rptmodel.Division :
                                  (rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0" && rptmodel.Status == "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode && emp.Division == rptmodel.Division :
                                  (rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0" && rptmodel.Status != "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && att.IsApproved == rptmodel.Status && emp.Division == rptmodel.Division :
                                  (rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0" && rptmodel.Status == "All") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.PresentReportingLocation == rptmodel.LocationName && emp.Division == rptmodel.Division :
                                  att.Date >= rptmodel.From && att.Date <= rptmodel.To

                                 //(rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To : 
                                 //(rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode :
                                 //(rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.PresentReportingLocation==rptmodel.LocationName && emp.DivisionCode == rptmodel.DivisionCode :
                                 //(rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.Division == rptmodel.Division && emp.PresentReportingLocation == rptmodel.LocationName && emp.DivisionCode == rptmodel.DivisionCode :
                                 //(rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.Division == rptmodel.Division:
                                 //(rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.Division == rptmodel.Division && emp.DivisionCode == rptmodel.DivisionCode :
                                 //(rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.Division == rptmodel.Division && emp.PresentReportingLocation == rptmodel.LocationName :
                                 //(rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.PresentReportingLocation == rptmodel.LocationName :
                                 // att.Date >= rptmodel.From && att.Date <= rptmodel.To
                                 //(rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode  :
                                 //(rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To &&  emp.DivisionCode== rptmodel.DivisionCode:
                                 //(rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To :
                                 //(rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.Division == rptmodel.Division :
                                 //(rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.Division == rptmodel.Division :
                                 //(rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0") ? att.Date >= rptmodel.From && att.Date <= rptmodel.To && emp.Division == rptmodel.Division && emp.DivisionCode == rptmodel.DivisionCode :
                                 //att.Date >= rptmodel.From && att.Date <= rptmodel.To
                                 select new { att, us, emp }).ToList().OrderBy(m => m.emp.Id).ToList();
                    //OrderBy(m=>m.att.Date);

                    _usermanager um = new _usermanager();
                    List<InOut> InOutList = new List<InOut>();

                    if (rptmodel.Division > 0)
                    {
                        model = model.Where(x => x.emp.Division == rptmodel.Division).ToList();
                    }

                    if (rptmodel.DivisionCode != "0")
                    {
                        var emps = db.EmployeementInfos.Where(x => x.DivisionCode == rptmodel.DivisionCode).Select(x => x.Id).ToList();
                        model = model.Where(x => emps.Contains(x.us.Id)).ToList();
                    }

                    if (rptmodel.LocationName != "[All Locations]")
                    {
                        var emps = db.EmployeementInfos.Where(x => x.PresentReportingLocation == rptmodel.LocationName).Select(x => x.Id).ToList();
                        model = model.Where(x => emps.Contains(x.us.Id)).ToList();
                    }

                    //var filteredEmp = model.Where(x => x.emp.Division == rptmodel.Division).ToList();

                    foreach (var item in model)
                    {
                        try
                        {
                            InOut inout = new InOut();
                            inout.EmployeeName = item.us.FirstName;
                            inout.EmployeeNumber = item.us.EmployeeId;
                            inout.Designation = item.emp.Designation;
                            inout.Location = item.emp.PresentReportingLocation;
                            inout.SuperVisor = um.GetUser(item.att.SupervisorId).UserProfiles.FirstName;
                            inout.ImmediateSupper = um.GetUser(item.emp.SupervisorId).UserProfiles.FirstName;
                            inout.inoutdate = item.att.DateRequested.ToString("MM/dd/yyyy");

                            inout.DateStr = item.att.Date.ToString("MM/dd/yyyy");

                            inout.InTime = "-";
                            if (item.att.InTime.TimeOfDay.Ticks > 0)
                            {
                                inout.InTime = item.att.InTime.ToString("h:mm tt");
                            }

                            inout.InReason = item.att.InReason;

                            inout.OutTime = "-";
                            if (item.att.OutTime.TimeOfDay.Ticks > 0)
                            {
                                inout.OutTime = item.att.OutTime.ToString("h:mm tt");
                            }

                            //inout.OutTime = item.att.OutTime.ToString("h:mm tt");
                            inout.OutReason = item.att.OutReason;
                            inout.Remark = item.att.Remarks;
                            if (item.att.IsApproved == "true")
                            {
                                inout.Status = "Approved";
                            }
                            else if (item.att.IsApproved == "false")
                            {
                                inout.Status = "Rejected";
                            }
                            else if (item.att.IsApproved == "pending")
                            {
                                inout.Status = "Pending";
                            }

                            InOutList.Add(inout);
                        }
                        catch
                        {

                        }

                    }
                    return InOutList;
                }
            }
            catch (Exception ex)
            {
                return new List<InOut>();
            }
        }

        internal List<UsedLeavesByUser> GetUsedLeavesForUsers(int divisionId, int year, int month)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    //List<UsedLeavesByUser> lst = new List<UsedLeavesByUser>();

                    _leavs lv = new App_Codes._leavs();

                    var empInfo = new List<EmployeementInfo>();

                    if (divisionId > 0)
                    {
                        empInfo = db.EmployeementInfos.Where(x => x.Division == divisionId).ToList();
                    }
                    else
                    {
                        empInfo = db.EmployeementInfos.ToList();
                    }

                    var users = empInfo.ConvertAll(x => x.Id).ToList();
                    var fromDate = new DateTime(year, month, 1);
                    var toDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    var leaveHistory = lv.GetUsedLeaveHistory(fromDate, toDate, users);


                    return leaveHistory;
                }
            }
            catch(Exception er)
            {
                return null;
            }

           
        }

        //internal List<LeaveBalanceUser> GetLeaveBalanceSystemReport(int division, string location, int year)
        //{
        //    throw new NotImplementedException();
        //}

        public String ReportToExcel(Reports Results, String Path, String LogoPath)
        {
            try
            {


                using (ExcelPackage epackage = new ExcelPackage())
                {
                    ExcelWorksheet ws = epackage.Workbook.Worksheets.Add("Sheet 1");

                    ws.Cells["A1"].Value = "In/Out Correction Report";
                    ws.Cells["A1:C1"].Merge = true;

                    ws.Cells["B3"].Value = (Results.DivisionName == null) ? "All Divisions" : Results.DivisionName;// "Head Office";
                    ws.Cells["B4"].Value = (Results.LocationName == null) ? "All Locations" : Results.LocationName;// "Telecom Head Office";
                    ws.Cells["B5"].Value = (Results.DivisionCode == null || Results.DivisionCode == "0") ? "All Division Codes" : Results.DivisionCode; // "STL";
                    ws.Cells["B6"].Value = Results.From.ToString("MM/dd/yyyy") + " - " + Results.To.ToString("MM/dd/yyyy");// "3/1/2019-3/31/2019";

                    int currentrow = 9;
                    ws.Cells[8, 1].Value = "Employee No";
                    ws.Cells[8, 2].Value = "Employee Name";
                    ws.Cells[8, 3].Value = "Designation";


                    int coulcount = 4;

                    if (Results.Date)
                    {
                        ws.Cells[8, coulcount].Value = "Date";
                        coulcount++;
                    }


                    if (Results.InTime)
                    {
                        ws.Cells[8, coulcount].Value = "In Time";
                        coulcount++;

                    }

                    if (Results.InReason)
                    {
                        ws.Cells[8, coulcount].Value = "In Reason";
                        coulcount++;
                    }

                    if (Results.OutTime)
                    {
                        ws.Cells[8, coulcount].Value = "Out Timeate";
                        coulcount++;
                    }

                    if (Results.OutReason)
                    {
                        ws.Cells[8, coulcount].Value = "Out Reason";
                        coulcount++;

                    }
                    if (Results.inoutdate)
                    {
                        ws.Cells[8, coulcount].Value = "Request Date";
                        coulcount++;
                    }

                    if (Results.SuperVisor)
                    {
                        ws.Cells[8, coulcount].Value = "Approved By";
                        coulcount++;

                    }
                    if (Results.immediateSuperVisor)
                    {
                        ws.Cells[8, coulcount].Value = "Immediate supervisor";
                        coulcount++;

                    }
                    if (Results.BStatus)
                    {
                        ws.Cells[8, coulcount].Value = "Status";
                        coulcount++;

                    }
                    if (Results.Comment)
                    {
                        ws.Cells[8, coulcount].Value = "Remark";
                        coulcount++;

                    }

                    foreach (var item in Results.InOutResult)
                    {
                        ws.Cells[currentrow, 1].Value = item.EmployeeNumber;
                        ws.Cells[currentrow, 2].Value = item.EmployeeName;
                        ws.Cells[currentrow, 3].Value = item.Designation;

                        int datacolcount = 4;



                        if (Results.Date)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.DateStr;
                            datacolcount++;
                        }


                        if (Results.InTime)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.InTime;
                            datacolcount++;

                        }

                        if (Results.InReason)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.InReason;
                            datacolcount++;
                        }

                        if (Results.OutTime)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.OutTime;
                            datacolcount++;
                        }

                        if (Results.OutReason)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.OutReason;
                            datacolcount++;

                        }
                        if (Results.inoutdate)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.inoutdate;
                            datacolcount++;
                        }

                        if (Results.SuperVisor)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.SuperVisor;
                            datacolcount++;

                        }
                        if (Results.immediateSuperVisor)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.ImmediateSupper;
                            datacolcount++;

                        }

                        if (Results.BStatus)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.Status;
                            datacolcount++;

                        }

                        if (Results.Comment)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.Remark;
                            datacolcount++;

                        }

                        currentrow++;
                    }

                    ws.Row(currentrow + 5).Height = 50.00D;
                    ws.Cells[currentrow + 5, 1].Value = "This is a System Generated Document. No Signature is Required";
                    ws.Cells[currentrow + 5, 1, currentrow + 5, 3].Merge = true;
                    ws.Cells[currentrow + 5, 4].Value = "Sierra Construction Limited";
                    ws.Cells[currentrow + 6, 4].Value = DateTime.Now.ToShortDateString();
                    ws.Cells[currentrow + 6, 4, currentrow + 6, 5].Merge = true;
                    ws.Cells[currentrow + 5, 4, currentrow + 5, 5].Merge = true;
                    ws.Cells[currentrow + 5, 9].Value = "SEMS Powered by SIOT";
                    ws.Cells[currentrow + 5, 9, currentrow + 5, 11].Merge = true;
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    //ws.Row(5).Height = 39.00D;
                    //Image logo = Image.FromFile(LogoPath);
                    //var picture = ws.Drawings.AddPicture("img", logo);
                    //picture.SetPosition(5, 0, 2, 0);


                    Stream stream = File.Create(Path);
                    epackage.SaveAs(stream);

                    stream.Close();
                    byte[] data = File.ReadAllBytes(Path);
                }
                return Path;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<EmployeementInfo> GetdivisionCodes()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    return db.EmployeementInfos.Where(m => m.DivisionCode != null).ToList().GroupBy(m => m.DivisionCode).Select(m => m.FirstOrDefault()).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<EmployeementInfo>();
            }
        }

        public List<Leave> ViewLeaveReport(LeaveViewmodel rptmodel)
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    List<Leave> Leaves = new List<Leave>();
                    _usermanager um = new _usermanager();

                    var lvs = (from lv in db.LeaveHistories
                               join emp in db.EmployeementInfos
                               on lv.UserId equals emp.Id
                               where
                               (rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0" && rptmodel.Status == "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To :
                               (rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0" && rptmodel.Status != "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && (rptmodel.Status == "pending" ? lv.IsApproved == null || rptmodel.Status == "" : rptmodel.Status == lv.IsApproved) :
                               (rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && (rptmodel.Status == "pending" ? lv.IsApproved == null || rptmodel.Status == "" : rptmodel.Status == lv.IsApproved) && emp.DivisionCode == rptmodel.DivisionCode :
                               (rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && (rptmodel.Status == "pending" ? lv.IsApproved == null || rptmodel.Status == "" : rptmodel.Status == lv.IsApproved) && emp.DivisionCode == rptmodel.DivisionCode && emp.PresentReportingLocation == rptmodel.LocationName :
                               (rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && (rptmodel.Status == "pending" ? lv.IsApproved == null || rptmodel.Status == "" : rptmodel.Status == lv.IsApproved) && emp.DivisionCode == rptmodel.DivisionCode && emp.PresentReportingLocation == rptmodel.LocationName && emp.Division == rptmodel.Division :
                               (rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0" && rptmodel.Status != "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && (rptmodel.Status == "pending" ? lv.IsApproved == null || rptmodel.Status == "" : rptmodel.Status == lv.IsApproved) && emp.PresentReportingLocation == rptmodel.LocationName :
                               (rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0" && rptmodel.Status == "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode && emp.PresentReportingLocation == rptmodel.LocationName :
                               (rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && (rptmodel.Status == "pending" ? lv.IsApproved == null || rptmodel.Status == "" : rptmodel.Status == lv.IsApproved) && emp.DivisionCode == rptmodel.DivisionCode :
                               (rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0" && rptmodel.Status != "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && (rptmodel.Status == "pending" ? lv.IsApproved == null || rptmodel.Status == "" : rptmodel.Status == lv.IsApproved) && emp.DivisionCode == rptmodel.DivisionCode && emp.Division == rptmodel.Division :
                               (rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0" && rptmodel.Status == "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode && emp.Division == rptmodel.Division :
                               (rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0" && rptmodel.Status != "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && (rptmodel.Status == "pending" ? lv.IsApproved == null || rptmodel.Status == "" : rptmodel.Status == lv.IsApproved) && emp.Division == rptmodel.Division :
                               (rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0" && rptmodel.Status == "All") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.PresentReportingLocation == rptmodel.LocationName && emp.Division == rptmodel.Division :
                               lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To

                               //(rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To :
                               //(rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode :
                               //(rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.PresentReportingLocation == rptmodel.LocationName && emp.DivisionCode == rptmodel.DivisionCode :
                               //(rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.Division == rptmodel.Division && emp.PresentReportingLocation == rptmodel.LocationName && emp.DivisionCode == rptmodel.DivisionCode :
                               //(rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.Division == rptmodel.Division :
                               //(rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.Division == rptmodel.Division && emp.DivisionCode == rptmodel.DivisionCode :
                               //(rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.Division == rptmodel.Division && emp.PresentReportingLocation == rptmodel.LocationName :
                               //(rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.PresentReportingLocation == rptmodel.LocationName :
                               //lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To


                               //(rptmodel.Division == 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode :
                               //(rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode != "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.DivisionCode == rptmodel.DivisionCode :
                               //(rptmodel.Division == 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To :
                               //(rptmodel.Division != 0 && rptmodel.Location == 0 && rptmodel.DivisionCode == "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.Division == rptmodel.Division :
                               //(rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode == "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.Division == rptmodel.Division :
                               //(rptmodel.Division != 0 && rptmodel.Location != 0 && rptmodel.DivisionCode != "0") ? lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To && emp.Division == rptmodel.Division && emp.DivisionCode == rptmodel.DivisionCode :
                               // lv.FromDate >= rptmodel.From && lv.ToDate <= rptmodel.To
                               select lv
                              ).ToList().OrderBy(m => m.EMPNo).ToList();

                    //.OrderBy(m => m.RequestDate);

                    if (rptmodel.Division > 0)
                    {
                        var emps = db.EmployeementInfos.Where(x => x.Division == rptmodel.Division).Select(x => x.Id).ToList();
                        lvs = lvs.Where(x => emps.Contains(x.UserId)).ToList();
                    }

                    if (rptmodel.DivisionCode != "0")
                    {
                        var emps = db.EmployeementInfos.Where(x => x.DivisionCode == rptmodel.DivisionCode).Select(x => x.Id).ToList();
                        lvs = lvs.Where(x => emps.Contains(x.UserId)).ToList();
                    }

                    if (rptmodel.LocationName != "[All Locations]")
                    {
                        var emps = db.EmployeementInfos.Where(x => x.PresentReportingLocation == rptmodel.LocationName).Select(x => x.Id).ToList();
                        lvs = lvs.Where(x => emps.Contains(x.UserId)).ToList();
                    }

                    var leaveTypes = db.LeaveTypes.ToList();

                    if (lvs.Count() > 0)
                    {
                        foreach (var sub in lvs)
                        {
                            var approvals = db.LeaveApprovals.Where(x => x.RequestId == sub.Id).OrderByDescending(x => x.Id).ToList().FirstOrDefault();
                            var emp = um.GetUserEmployement(sub.UserId);
                            var uset = um.GetUserProfile(sub.UserId);
                            Leave lv = new Leave();
                            lv.EmployeeNumber = uset.EmployeeId;
                            lv.EmployeeName = uset.FirstName + " " + uset.LastName;
                            lv.Designation = emp.Designation;
                            lv.RequestDate = sub.RequestDate.ToString("MM/dd/yyyy");
                            lv.From = sub.FromDate.ToString("MM/dd/yyyy");
                            lv.To = sub.ToDate.ToString("MM/dd/yyyy");
                            lv.Days = Convert.ToString(sub.LeaveDays);
                            lv.ReasonforLeave = sub.Reason;
                            lv.Remark = sub.Remarks;
                            var sup = um.GetUserProfile(emp.SupervisorId);
                            lv.Supervisor = sup.FirstName + " " + sup.LastName;

                            if (approvals.Status == "true")
                            {
                                lv.IsApproved = "Approved";
                            }
                            else if (approvals.Status == "false")
                            {
                                lv.IsApproved = "Rejected";
                            }
                            else if (approvals.Status == "pending")
                            {
                                lv.IsApproved = "Pending";
                            }

                            if (sub.LeaveType == 0)
                            {
                                lv.Leavetype = "Short Leave";


                                lv.Approvedby = um.GetUserProfile(approvals.SupervisorId).FirstName;

                                if (sub.LeaveUnit == "HALFMOR")
                                {
                                    lv.LeaveUnit = "Half-Morning";
                                }
                                else if (sub.LeaveUnit == "HALFEVE")
                                {
                                    lv.LeaveUnit = "Half-Evening";
                                }
                                else
                                {
                                    lv.LeaveUnit = sub.LeaveUnit;
                                }
                            }
                            else
                            {
                                lv.Leavetype = leaveTypes.Where(x => x.Id == sub.LeaveType).FirstOrDefault().LeaveType1;

                                lv.Approvedby = um.GetUserProfile(approvals.SupervisorId).FirstName;

                                if (sub.LeaveUnit == "HALFMOR")
                                {
                                    lv.LeaveUnit = "Half-Morning";
                                }
                                else if (sub.LeaveUnit == "HALFEVE")
                                {
                                    lv.LeaveUnit = "Half-Evening";
                                }
                                else
                                {
                                    lv.LeaveUnit = sub.LeaveUnit;
                                }
                            }

                            Leaves.Add(lv);
                        }
                    }

                    return Leaves.OrderBy(x => x.EmployeeNumber).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<Leave>();
            }
        }

        public String LeaveExcel(LeaveViewmodel Results, String Path, String LogoPath)
        {
            try
            {
                using (ExcelPackage epackage = new ExcelPackage())
                {
                    ExcelWorksheet ws = epackage.Workbook.Worksheets.Add("Sheet 1");

                    ws.Cells["A1"].Value = "Monthly Leave Report";
                    ws.Cells["A1:C1"].Merge = true;

                    ws.Cells["B2"].Value = (Results.DivisionName == null) ? "[All Divisions]" : Results.DivisionName;// "Head Office";
                    ws.Cells["B3"].Value = (Results.LocationName == null) ? "[All Locations]" : Results.LocationName;// "Telecom Head Office";
                    ws.Cells["B4"].Value = (Results.DivisionCode == null || Results.DivisionCode == "0") ? "[All Division Codes]" : Results.DivisionCode; // "STL";
                    ws.Cells["B5"].Value = (Results.Status == null || Results.DivisionCode == "All") ? "[All Status]" : Results.StatusView; // "STL";
                    //ws.Cells["B2"].Value = (Results.DivisionName == null) ? "All": Results.DivisionName;// "Head Office";
                    //ws.Cells["B3"].Value = (Results.LocationName == null )? "All" : Results.LocationName ;// "Telecom Head Office";
                    //ws.Cells["B4"].Value = (Results.DivisionCode == null || Results.DivisionCode == "0") ? "All" : Results.DivisionCode; // "STL";
                    //ws.Cells["B5"].Value = (Results.Status == null || Results.DivisionCode == "All") ? "All" : Results.Status; // "STL";
                    ws.Cells["B6"].Value = Results.From.ToString("MM/dd/yyyy") + " - " + Results.To.ToString("MM/dd/yyyy");// "3/1/2019-3/31/2019";

                    int currentrow = 9;
                    ws.Cells[8, 1].Value = "Employee No";
                    ws.Cells[8, 2].Value = "Employee Name";
                    ws.Cells[8, 3].Value = "Designation";


                    int coulcount = 4;
                    if (Results.Leavetype)
                    {
                        ws.Cells[8, coulcount].Value = "Leave Type";
                        coulcount++;
                    }


                    if (Results.LeaveUnit)
                    {
                        ws.Cells[8, coulcount].Value = "Leave Unit";
                        coulcount++;

                    }

                    if (Results.RequestDate)
                    {
                        ws.Cells[8, coulcount].Value = "Request Date";
                        coulcount++;
                    }

                    if (Results.LFrom)
                    {
                        ws.Cells[8, coulcount].Value = "From";
                        coulcount++;
                    }

                    if (Results.LTo)
                    {
                        ws.Cells[8, coulcount].Value = "To";
                        coulcount++;

                    }

                    if (Results.Days)
                    {
                        ws.Cells[8, coulcount].Value = "Days";
                        coulcount++;

                    }

                    if (Results.ReasonforLeave)
                    {
                        ws.Cells[8, coulcount].Value = "Reason for Leave";
                        coulcount++;

                    }

                    if (Results.IsApproved)
                    {
                        ws.Cells[8, coulcount].Value = "Status";
                        coulcount++;

                    }

                    if (Results.Approvedby)
                    {
                        ws.Cells[8, coulcount].Value = "Approved by";
                        coulcount++;

                    }


                    if (Results.SuperVisor)
                    {
                        ws.Cells[8, coulcount].Value = "Supervisor";
                        coulcount++;
                    }

                    if (Results.Comment)
                    {
                        ws.Cells[8, coulcount].Value = "Remark";
                        coulcount++;
                    }






                    foreach (var item in Results.LeaveResult)
                    {
                        ws.Cells[currentrow, 1].Value = item.EmployeeNumber;
                        ws.Cells[currentrow, 2].Value = item.EmployeeName;
                        ws.Cells[currentrow, 3].Value = item.Designation;

                        int datacolcount = 4;

                        if (Results.Leavetype)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.Leavetype;
                            datacolcount++;
                        }


                        if (Results.LeaveUnit)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.LeaveUnit;
                            datacolcount++;

                        }

                        if (Results.RequestDate)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.RequestDate;
                            datacolcount++;
                        }

                        if (Results.LFrom)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.From;
                            datacolcount++;
                        }

                        if (Results.LTo)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.To;
                            datacolcount++;
                        }

                        if (Results.Days)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.Days;
                            datacolcount++;

                        }

                        if (Results.ReasonforLeave)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.ReasonforLeave;
                            datacolcount++;
                        }

                        if (Results.IsApproved)
                        {
                            ws.Cells[currentrow, datacolcount].Value = (item.IsApproved == "true") ? "Approved" : "Pending";
                            datacolcount++;
                        }

                        if (Results.Approvedby)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.Approvedby;
                            datacolcount++;
                        }

                        if (Results.SuperVisor)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.Supervisor;
                            datacolcount++;
                        }

                        if (Results.Comment)
                        {
                            ws.Cells[currentrow, datacolcount].Value = item.Remark;
                            datacolcount++;
                        }



                        currentrow++;
                    }

                    ws.Row(currentrow + 5).Height = 50.00D;
                    ws.Cells[currentrow + 5, 1].Value = "This is a System Generated Document. No Signature is Required";
                    ws.Cells[currentrow + 5, 1, currentrow + 5, 3].Merge = true;
                    ws.Cells[currentrow + 5, 4].Value = "Sierra Construction Limited";
                    ws.Cells[currentrow + 5, 4, currentrow + 5, 5].Merge = true;
                    ws.Cells[currentrow + 6, 4].Value = DateTime.Now.ToShortDateString();
                    ws.Cells[currentrow + 6, 4, currentrow + 6, 5].Merge = true;
                    ws.Cells[currentrow + 5, 7].Value = "SEMS Powered by SIOT";
                    ws.Cells[currentrow + 5, 7, currentrow + 5, 13].Merge = true;
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    //ws.Row(5).Height = 39.00D;
                    //Image logo = Image.FromFile(LogoPath);
                    //var picture = ws.Drawings.AddPicture("img", logo);
                    //picture.SetPosition(5, 0, 2, 0);


                    Stream stream = File.Create(Path);
                    epackage.SaveAs(stream);

                    stream.Close();
                    byte[] data = File.ReadAllBytes(Path);
                }
                return Path;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        internal List<LeaveBalanceUser> GetLeaveBalance(int divisionId, string location, int year)
        {
            using (ApplicationDbContext db = new Models.ApplicationDbContext())
            {
                List<LeaveBalanceUser> lst = new List<LeaveBalanceUser>();
                _usermanager um = new _usermanager();
                _leavs _leaves = new _leavs();

                var userLeaves = db.UserLeaves.Where(x => x.Year == year).ToList();

                var empInfo = new List<EmployeementInfo>();

                if (divisionId > 0 && location == "0")
                {
                    empInfo = db.EmployeementInfos.Where(x => x.Division == divisionId).ToList();
                }
                else if (divisionId == 0 && location != "0")
                {
                    empInfo = db.EmployeementInfos.Where(x => x.PresentReportingLocation == location).ToList();
                }
                else if (divisionId > 0 && location != "0")
                {
                    empInfo = db.EmployeementInfos.Where(x => x.Division == divisionId && x.PresentReportingLocation == location).ToList();
                }
                else
                {
                    empInfo = db.EmployeementInfos.ToList();
                }

                var leaves = _leaves.GetLeaves();
                var noPayLeaves = db.NoPayLeaves.ToList();

                foreach (var item in empInfo)
                {
                    LeaveBalanceUser model = new LeaveBalanceUser();
                    List<MonthlyBreackDown> userMonthlyUsedLeaves = new List<MonthlyBreackDown>();
                    List<UsedLeaves> yearlyUsedLeaves = new List<UsedLeaves>();

                    var lvInfo = (from user in db.UserProfiles
                                  join lvh in db.LeaveHistories on user.Id equals lvh.UserId
                                  where user.Id == item.Id && lvh.IsApproved == "true" && lvh.LeaveUnit != "SHORT"
                                  select new { user, lvh }).ToList();


                    var lvs = userLeaves.Where(x => x.UserId == item.Id).ToList();

                    if (lvInfo.Count() > 0)
                    {
                        var userProfile = lvInfo.FirstOrDefault().user;

                        model.EmployeeNumber = userProfile.EmployeeId;
                        model.EmployeeName = userProfile.FirstName;
                        model.AnnualAllocated = Convert.ToDouble(lvs.Where(x => x.LeaveType == 3000).Sum(x => x.AllocatedCount));
                        model.CassualAllocated = Convert.ToDouble(lvs.Where(x => x.LeaveType == 3001).Sum(x => x.AllocatedCount));

                        double totalUsedLeaves = 0;
                        //UsedLeaves totalUsedLeavesModel = new UsedLeaves();

                        for (int i = 1; i <= 12; i++)
                        {
                            MonthlyBreackDown monthLyBreackDownModel = new MonthlyBreackDown();
                            List<UsedLeaves> monthLyUsedLeaves = new List<UsedLeaves>();
                            double AdminEnforcedNoPay = 0;

                            monthLyBreackDownModel.Month_Id = i;
                            monthLyBreackDownModel.Month_Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);

                            var lvH = lvInfo.Where(x => x.lvh.FromDate.Year == year && x.lvh.FromDate.Month == i).ToList();
                            var noPayRequest = lvH.ConvertAll(x => x.lvh.Id).ToList();

                            var noPaylvH = noPayLeaves.Where(x => noPayRequest.Contains(x.Request_Id)).ToList();

                            foreach (var sub in leaves)
                            {
                                UsedLeaves usedLeavesModel = new UsedLeaves();

                                usedLeavesModel.LeaveType_Id = sub.Id;
                                usedLeavesModel.Leave_Name = sub.LeaveType1;
                                usedLeavesModel.Used_Count = Convert.ToDouble(lvH.Where(x => x.lvh.LeaveType == sub.Id).Sum(x => x.lvh.LeaveDays));

                                monthLyUsedLeaves.Add(usedLeavesModel);

                                if (sub.Id == 3014)
                                {
                                    AdminEnforcedNoPay = usedLeavesModel.Used_Count;
                                }
                            }

                            ///No Pay Leaves

                            UsedLeaves noPayLeavesModel = new UsedLeaves();

                            noPayLeavesModel.LeaveType_Id = -1;
                            noPayLeavesModel.Leave_Name = "No Pay";
                            noPayLeavesModel.Used_Count = noPaylvH.Sum(x => x.No_Pay_Count) + AdminEnforcedNoPay;

                            monthLyUsedLeaves.Add(noPayLeavesModel);
                            /////////////////////////////////////////////////////////////////////

                            totalUsedLeaves += monthLyUsedLeaves.Sum(x => x.Used_Count);
                            monthLyBreackDownModel.LeaveTypes = monthLyUsedLeaves;
                            userMonthlyUsedLeaves.Add(monthLyBreackDownModel);
                        }

                        foreach (var sub in leaves)
                        {
                            var lvH = lvInfo.Where(x => x.lvh.FromDate.Year == year && x.lvh.LeaveType == sub.Id).ToList();

                            UsedLeaves totalUsedLeavesModel = new UsedLeaves();

                            totalUsedLeavesModel.LeaveType_Id = sub.Id;
                            totalUsedLeavesModel.Leave_Name = sub.LeaveType1;
                            totalUsedLeavesModel.Used_Count = Convert.ToDouble(lvH.Sum(x => x.lvh.LeaveDays));

                            yearlyUsedLeaves.Add(totalUsedLeavesModel);
                        }

                        model.MonthlyUsedLeaves = userMonthlyUsedLeaves;
                        model.TotalUsed = yearlyUsedLeaves;

                        ///Yearly No Pay Leaves
                        var yearlylvH = lvInfo.Where(x => x.lvh.FromDate.Year == year).ToList();
                        var yearlyNoPayRequest = yearlylvH.ConvertAll(x => x.lvh.Id).ToList();
                        var yearlylNoPaylvH = noPayLeaves.Where(x => yearlyNoPayRequest.Contains(x.Request_Id)).ToList();

                        UsedLeaves yearlyNoPayLeavesModel = new UsedLeaves();

                        yearlyNoPayLeavesModel.LeaveType_Id = -1;
                        yearlyNoPayLeavesModel.Leave_Name = "No Pay";
                        yearlyNoPayLeavesModel.Used_Count = yearlylNoPaylvH.Sum(x => x.No_Pay_Count);

                        yearlyUsedLeaves.Add(yearlyNoPayLeavesModel);
                        /////////////////////////////////////////////////////////////////////

                        model.BalanceAnnualLeaves = Convert.ToDouble(lvs.Where(x => x.LeaveType == 3000).Sum(x => x.RemainingCount));
                        model.BalanceCassualLeaves = Convert.ToDouble(lvs.Where(x => x.LeaveType == 3001).Sum(x => x.RemainingCount));

                    }

                    lst.Add(model);
                }

                return lst;
            }
        }

        internal List<EmployeeLeaveSummery> GetLeaveBalance(int divisionId, int year, int month)
        {
            using (ApplicationDbContext db = new Models.ApplicationDbContext())
            {
                List<EmployeeLeaveSummery> lst = new List<EmployeeLeaveSummery>();
                _usermanager um = new _usermanager();
                _leavs _leaves = new _leavs();

                var userLeaves = db.UserLeaves.Where(x => x.Year == year).ToList();

                var empInfo = new List<EmployeementInfo>();

                if (divisionId > 0)
                {
                    empInfo = db.EmployeementInfos.Where(x => x.Division == divisionId).ToList();
                }
                else
                {
                    empInfo = db.EmployeementInfos.ToList();
                }

                var leaves = _leaves.GetLeaves();
                leaves.Add(new LeaveType { Id = 0, LeaveType1 = "SHORT", LeaveTypeCode = "SL"});
                leaves.Add(new LeaveType { Id = -1, LeaveType1 = "NOPAY", LeaveTypeCode = "NL" });

                var noPayLeaves = db.NoPayLeaves.ToList();

                var daysInMonth = DateTime.DaysInMonth(year, month);
                var empIds = empInfo.ConvertAll(x => x.Id).ToList();

                var fromDate = new DateTime(year, month, 1);
                var toDate = new DateTime(year, month, daysInMonth);

                var lvInfoAll = db.LeaveHistories.Where(x => empIds.Contains(x.UserId) && x.FromDate >= fromDate && x.FromDate <= toDate).ToList();
                var userProfiles = db.UserProfiles.Where(x => empIds.Contains(x.Id)).ToList();
                var holidaysInMonth = db.Holidays.Where(x => x.Date.Month == month).ToList();

                foreach (var item in empInfo)
                {
                    EmployeeLeaveSummery model = new EmployeeLeaveSummery();
                    List<UsedLeaves> usedLeaveTotal = new List<UsedLeaves>();
                    List<UsedLeaves> usedLeaves = new List<UsedLeaves>();
                    List<MonthlyLeaveDetails> userMonthlyUsedLeaves = new List<MonthlyLeaveDetails>();

                    var userInfo = userProfiles.Where(x => x.Id == item.Id).FirstOrDefault();
                    //List<UsedLeaves> yearlyUsedLeaves = new List<UsedLeaves>();

                    //var lvInfo = (from user in db.UserProfiles
                    //              join lvh in db.LeaveHistories on user.Id equals lvh.UserId
                    //              where user.Id == item.Id && lvh.IsApproved == "true" && lvh.LeaveUnit != "SHORT"
                    //              select new { user, lvh }).ToList();

                    //var lvInfo = lvInfoAll.Where(x => x.UserId == item.Id).ToList();
                    var lvs = userLeaves.Where(x => x.UserId == item.Id).ToList();
                    model.EmployeeName = userInfo.FirstName;
                    model.EmployeeNumber = userInfo.EmployeeId;

                    for (int i = 1; i <= daysInMonth; i++)
                    {
                        //MonthlyLeaveDetails detailsModel = new MonthlyLeaveDetails();

                        var leaveDate = new DateTime(year, month, i);
                        var lvInfo = lvInfoAll.Where(x => x.UserId == item.Id && x.FromDate == leaveDate).FirstOrDefault();

                        if(lvInfo != null)
                        {
                            if(lvInfo.LeaveType == 0)
                            {
                                userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = lvInfo.FromDate, AttendanceType = "SL" });
                                usedLeaves.Add(new UsedLeaves { LeaveType_Id = 0, Leave_Name = "SL", Used_Count = 1 });
                            }
                            else
                            {
                                if(lvInfo.LeaveUnit == "HALFMOR" || lvInfo.LeaveUnit == "HALFEVE")
                                {
                                    userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = lvInfo.FromDate, AttendanceType = "H" });
                                    usedLeaves.Add(new UsedLeaves { LeaveType_Id = lvInfo.LeaveType, Leave_Name = "H", Used_Count = 0.5 });
                                }
                                else
                                {
                                    var leaveTypeCode = leaves.Where(x => x.Id == lvInfo.LeaveType).FirstOrDefault().LeaveTypeCode;

                                    if (lvInfo.LeaveDays > 1)
                                    {
                                        int dayIndex = 0;

                                        for(int leaveDay = 1; leaveDay <= lvInfo.LeaveDays; leaveDay++)
                                        {
                                            DateTime leaveDayDate = lvInfo.FromDate.AddDays(dayIndex);

                                            //if (leaveDay == 1)
                                            //{
                                            //    leaveDayDate = lvInfo.FromDate;
                                            //}

                                            if (holidaysInMonth != null)
                                            {
                                                if (holidaysInMonth.Where(x => x.Date == leaveDayDate).FirstOrDefault() == null)
                                                {
                                                    if (leaveDayDate.DayOfWeek == DayOfWeek.Saturday)
                                                    {
                                                        userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = leaveDayDate, AttendanceType = "" });
                                                    }
                                                    else if (leaveDayDate.DayOfWeek == DayOfWeek.Sunday)
                                                    {
                                                        userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = leaveDayDate, AttendanceType = "" });

                                                        //if (leaveTypeCode == "ANL" || leaveTypeCode == "RNL")
                                                        //{
                                                        //    userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = leaveDayDate.AddDays(1), AttendanceType = leaveTypeCode });
                                                        //    usedLeaves.Add(new UsedLeaves { LeaveType_Id = -1, Leave_Name = "NL", Used_Count = 1 });
                                                        //}                                                        
                                                    }
                                                    else
                                                    {
                                                        userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = leaveDayDate, AttendanceType = leaveTypeCode });
                                                        //usedLeaves.Add(new UsedLeaves { LeaveType_Id = lvInfo.LeaveType, Leave_Name = leaveTypeCode, Used_Count = 1 };
                                                        if (leaveTypeCode == "ANL" || leaveTypeCode == "RNL")
                                                        {
                                                            usedLeaves.Add(new UsedLeaves { LeaveType_Id = -1, Leave_Name = "NL", Used_Count = 1 });
                                                        }
                                                        else
                                                        {
                                                            usedLeaves.Add(new UsedLeaves { LeaveType_Id = lvInfo.LeaveType, Leave_Name = leaveTypeCode, Used_Count = 1 });
                                                        }
                                                    }
                                                }
                                            }


                                            i = leaveDayDate.Day;
                                            dayIndex++;


                                        }
                                    }
                                    else
                                    {
                                        userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = lvInfo.FromDate, AttendanceType = leaveTypeCode });
                                        //usedLeaves.Add(new UsedLeaves { LeaveType_Id = lvInfo.LeaveType, Leave_Name = leaveTypeCode, Used_Count = 1 };
                                        if (leaveTypeCode == "ANL" || leaveTypeCode == "RNL")
                                        {
                                            usedLeaves.Add(new UsedLeaves { LeaveType_Id = -1, Leave_Name = "NL", Used_Count = 1 });
                                        }
                                        else
                                        {
                                            usedLeaves.Add(new UsedLeaves { LeaveType_Id = lvInfo.LeaveType, Leave_Name = leaveTypeCode, Used_Count = 1 });
                                        }
                                    }
                                    

                                }
                                    
                            }
                                
                        }
                        else
                        {
                            //userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = leaveDate, AttendanceType = "P" });
                            if (leaveDate.DayOfWeek == DayOfWeek.Sunday || leaveDate.DayOfWeek == DayOfWeek.Saturday)
                            {
                                userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = leaveDate, AttendanceType = "" });
                            }
                            else
                            {
                                if(leaveDate <= DateTime.Now.Date)
                                {
                                    userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = leaveDate, AttendanceType = "P" });
                                }
                                else
                                {
                                    userMonthlyUsedLeaves.Add(new MonthlyLeaveDetails { Day = leaveDate, AttendanceType = "" });
                                }
                                
                            }

                        }
                    }

                    foreach(var lv in leaves)
                    {
                        var usedCount = usedLeaves.Where(x => x.LeaveType_Id == lv.Id).ToList();
                        usedLeaveTotal.Add(new UsedLeaves { LeaveType_Id = lv.Id, Leave_Name = lv.LeaveTypeCode, Used_Count = usedCount.Sum(x => x.Used_Count) });
                        //usedLeaveTotal.Add(new UsedLeaves { LeaveType_Id = lv.Id, Leave_Name = lv.LeaveTypeCode, Used_Count = usedCount.Sum(x => x.Used_Count) });
                        //if (lv.LeaveTypeCode == "ANL" || lv.LeaveTypeCode == "RNL")
                        //{
                        //    usedLeaveTotal.Add(new UsedLeaves { LeaveType_Id = lv.Id, Leave_Name = lv.LeaveTypeCode, Used_Count = usedCount.Sum(x => x.Used_Count) });
                        //}
                        //else
                        //{
                        //    usedLeaveTotal.Add(new UsedLeaves { LeaveType_Id = lv.Id, Leave_Name = lv.LeaveTypeCode, Used_Count = usedCount.Sum(x => x.Used_Count) });
                        //}
                    }

                    model.LeaveTypes = usedLeaveTotal;
                    model.MonthlyLeaveDetails = userMonthlyUsedLeaves;

                    //if (lvInfo.Count() > 0)
                    //{
                    //    var userProfile = lvInfo.FirstOrDefault().user;

                    //    model.EmployeeNumber = userProfile.EmployeeId;
                    //    model.EmployeeName = userProfile.FirstName;


                    //    double totalUsedLeaves = 0;
                    //    //UsedLeaves totalUsedLeavesModel = new UsedLeaves();

                    //    for (int i = 1; i <= 12; i++)
                    //    {
                    //        MonthlyBreackDown monthLyBreackDownModel = new MonthlyBreackDown();
                    //        List<UsedLeaves> monthLyUsedLeaves = new List<UsedLeaves>();
                    //        double AdminEnforcedNoPay = 0;

                    //        monthLyBreackDownModel.Month_Id = i;
                    //        monthLyBreackDownModel.Month_Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);

                    //        var lvH = lvInfo.Where(x => x.lvh.FromDate.Year == year && x.lvh.FromDate.Month == i).ToList();
                    //        var noPayRequest = lvH.ConvertAll(x => x.lvh.Id).ToList();

                    //        var noPaylvH = noPayLeaves.Where(x => noPayRequest.Contains(x.Request_Id)).ToList();

                    //        foreach (var sub in leaves)
                    //        {
                    //            UsedLeaves usedLeavesModel = new UsedLeaves();

                    //            usedLeavesModel.LeaveType_Id = sub.Id;
                    //            usedLeavesModel.Leave_Name = sub.LeaveType1;
                    //            usedLeavesModel.Used_Count = Convert.ToDouble(lvH.Where(x => x.lvh.LeaveType == sub.Id).Sum(x => x.lvh.LeaveDays));

                    //            monthLyUsedLeaves.Add(usedLeavesModel);

                    //            if (sub.Id == 3014)
                    //            {
                    //                AdminEnforcedNoPay = usedLeavesModel.Used_Count;
                    //            }
                    //        }

                    //        ///No Pay Leaves

                    //        UsedLeaves noPayLeavesModel = new UsedLeaves();

                    //        noPayLeavesModel.LeaveType_Id = -1;
                    //        noPayLeavesModel.Leave_Name = "No Pay";
                    //        noPayLeavesModel.Used_Count = noPaylvH.Sum(x => x.No_Pay_Count) + AdminEnforcedNoPay;

                    //        monthLyUsedLeaves.Add(noPayLeavesModel);
                    //        /////////////////////////////////////////////////////////////////////

                    //        totalUsedLeaves += monthLyUsedLeaves.Sum(x => x.Used_Count);
                    //        monthLyBreackDownModel.LeaveTypes = monthLyUsedLeaves;
                    //        userMonthlyUsedLeaves.Add(monthLyBreackDownModel);
                    //    }

                    //    foreach (var sub in leaves)
                    //    {
                    //        var lvH = lvInfo.Where(x => x.lvh.FromDate.Year == year && x.lvh.LeaveType == sub.Id).ToList();

                    //        UsedLeaves totalUsedLeavesModel = new UsedLeaves();

                    //        totalUsedLeavesModel.LeaveType_Id = sub.Id;
                    //        totalUsedLeavesModel.Leave_Name = sub.LeaveType1;
                    //        totalUsedLeavesModel.Used_Count = Convert.ToDouble(lvH.Sum(x => x.lvh.LeaveDays));

                    //        yearlyUsedLeaves.Add(totalUsedLeavesModel);
                    //    }

                    //    model.MonthlyUsedLeaves = userMonthlyUsedLeaves;
                    //    model.TotalUsed = yearlyUsedLeaves;

                    //    ///Yearly No Pay Leaves
                    //    var yearlylvH = lvInfo.Where(x => x.lvh.FromDate.Year == year).ToList();
                    //    var yearlyNoPayRequest = yearlylvH.ConvertAll(x => x.lvh.Id).ToList();
                    //    var yearlylNoPaylvH = noPayLeaves.Where(x => yearlyNoPayRequest.Contains(x.Request_Id)).ToList();

                    //    UsedLeaves yearlyNoPayLeavesModel = new UsedLeaves();

                    //    yearlyNoPayLeavesModel.LeaveType_Id = -1;
                    //    yearlyNoPayLeavesModel.Leave_Name = "No Pay";
                    //    yearlyNoPayLeavesModel.Used_Count = yearlylNoPaylvH.Sum(x => x.No_Pay_Count);

                    //    yearlyUsedLeaves.Add(yearlyNoPayLeavesModel);
                    //    /////////////////////////////////////////////////////////////////////

                    //    model.BalanceAnnualLeaves = Convert.ToDouble(lvs.Where(x => x.LeaveType == 3000).Sum(x => x.RemainingCount));
                    //    model.BalanceCassualLeaves = Convert.ToDouble(lvs.Where(x => x.LeaveType == 3001).Sum(x => x.RemainingCount));

                    //}

                    lst.Add(model);
                }

                return lst;
            }
        }
    }
}