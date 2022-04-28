using EmployeeTracking.App_Codes;
using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    [Authorize]
    [SessionExpireFilter]
    public class AttendanceController : Controller
    {
        // GET: Attendance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LeaveRequests()
        {
            ViewBag.erMsg = TempData["erMsg"];
            var userId = Session["UserId"].ToString();

            _leavs lv = new App_Codes._leavs();

            MyLeavesViewModel vm = new Models.MyLeavesViewModel();
            vm.LeaveTypes = lv.GetLeaves();
            vm.UserLeaves = lv.GetUserLeaves(userId, DateTime.Now.Year);
            vm.Supervisors = lv.GetAboveUsers(userId);
            vm.MyLeaveHistory = lv.GetLeaveHistory(userId, "0", DateTime.Now.Year);
            vm.RejectedLeaves = lv.GetRejectedLeaves(userId);
            return View(vm);
        }

        public ActionResult SaveLeave(MyLeavesViewModel model)
        {
            var userId = Session["UserId"].ToString();
            _leavs lv = new App_Codes._leavs();
            
            if(Request.Form["ddlLeaveUnit"].ToString() == "SHORT")
            {
                model.LeaveHistory.LeaveType = 0;
            }   
            else
            {
                model.LeaveHistory.LeaveType = Convert.ToInt32(Request.Form["ddlLeaveType"].ToString());
            }     
            
            model.LeaveHistory.LeaveUnit = Request.Form["ddlLeaveUnit"].ToString();
            model.LeaveHistory.UserId = Request.Form["hfLeaveUserId"].ToString();

            if(Request.Form["ddlLeaveUnit"].ToString() != "FULL")
            {
                model.LeaveHistory.ToDate = model.LeaveHistory.FromDate;
            }

            //var leaveDays = Convert.ToDouble(Request.Form["txtLeaveDays"].ToString());
            //model.LeaveHistory.LeaveDays = leaveDays;

            var msg = lv.RequestLeave(model.LeaveHistory);

            if (msg == "You Don't Have Enough Days Remaining" || msg == "You Already Have Applied Leave For This Period" || msg == "Existing Record Overlaps with Current Request")
            {
                TempData["erMsg"] = "errorMsg('"+ msg + "')";
            }
            else
            {
                TempData["erMsg"] = "successMsg('Leave Request Sent')";                
            }

            return RedirectToAction("LeaveRequests");
        }

        //public ActionResult LeaveApprovals()
        //{
        //    ViewBag.erMsg = TempData["erMsg"];
        //    _leavs lv = new _leavs();
        //    _attendence att = new App_Codes._attendence();

        //    AttendenceApprovalViewModel avm = new Models.AttendenceApprovalViewModel();
        //    avm.LeaveApprovals = lv.GetUserLeaveRequest(Session["UserId"].ToString());
        //    avm.InOutApprovals = att.GetUserInOutRequests(Session["UserId"].ToString());

        //    return View(avm);
        //}

        public ActionResult UserCancelLeave()
        {
            _leavs lv = new _leavs();
            var leaveId = Convert.ToInt32(Request.Form["hfCancelLeaveId"].ToString());
                        
            if (lv.CancelUserLeave(leaveId, Session["UserId"].ToString()))
            {
                TempData["erMsg"] = "successMsg('Leave Request Canceled')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Leave Request Could Not Be Canceled')";
            }
            return RedirectToAction("LeaveRequests");
        }
                
        //public ActionResult ApproveLeave()
        //{
        //    _leavs lv = new _leavs();
        //    LeaveHistoryViewModel model = new LeaveHistoryViewModel();

        //    model.Id = Convert.ToInt32(Request.Form["hfLeaveId"].ToString());

        //    if (Request.Form["hfIsApprove"].ToString() == "1")
        //    {
        //        model.IsApproved = "true";
        //    }
        //    else
        //    {
        //        model.IsApproved = "false";                

        //        if (Request.Form["txtRemarks"].ToString().Length > 0)
        //        {
        //            model.Remarks = Request.Form["txtRemarks"].ToString();
        //        }
        //    }
           

        //    if (lv.ApproveLeave(model))
        //    {
        //        TempData["erMsg"] = "successMsg('Leave Request Rejected')";
        //    }
        //    else
        //    {
        //        TempData["erMsg"] = "errorMsg('Leave Request Could Not Be Reject')";
        //    }

        //    return RedirectToAction("LeaveApprovals");
        //}

        //public ActionResult ForwardLeave()
        //{            
        //    _leavs lv = new _leavs();

        //    var leaveId = Convert.ToInt32(Request.Form["hfForwardLeaveId"].ToString());
        //    var userId = Request.Form["hfForwardTo"].ToString();

        //    if (lv.ForwardLeave(leaveId, userId))
        //    {
        //        TempData["erMsg"] = "successMsg('Leave Request Submitted For Optional Approval')";
        //    }
        //    else
        //    {
        //        TempData["erMsg"] = "errorMsg('Leave Request Could Not Be Submit')";
        //    }
            
        //    return RedirectToAction("LeaveApprovals");
        //}

        public ActionResult In_Out_Corrections()
        {
            ViewBag.erMsg = TempData["erMsg"];
            ApprovalsViewModel avm = new Models.ApprovalsViewModel();
            _attendence att = new App_Codes._attendence();
            avm.InOutApprovals = att.GetMyInOutRequests(Session["UserId"].ToString());
            return View(avm);
        }

        public ActionResult CorrectInOut(ApprovalsViewModel model)
        {
            _attendence att = new App_Codes._attendence();
            model.AttendenceCorrections.UserId = Request.Form["hfInOutUserId"].ToString();
            if (att.SaveInOutCorrections(model.AttendenceCorrections))
            {
                TempData["erMsg"] = "successMsg('Attendence Correction Request Sent')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Request Could Not Be Sent')";
            }

            return RedirectToAction("In_Out_Corrections");
        }

        public ActionResult CancelInOut()
        {
            _attendence att = new App_Codes._attendence();
            ApproveInOutViewModel model = new Models.ApproveInOutViewModel();
            model.Id = Convert.ToInt32(Request.Form["hfInOutId"].ToString());

            if (Request.Form["hfInOutApprove"].ToString() == "-1")
            {
                model.IsApproved = "cancel";
            }

            if (att.ApproveInOut(model))
            {
                TempData["erMsg"] = "successMsg('Attendence Correction Request Canceled')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Request Could Not Be Canceled')";
            }

            return RedirectToAction("In_Out_Corrections");
        }
    }
}