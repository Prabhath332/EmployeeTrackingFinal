using EmployeeTracking.App_Codes;
using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    public class ApprovalsController : Controller
    {
        // GET: Approvals
        public ActionResult Index()
        {
            ViewBag.erMsg = TempData["erMsg"];
            _leavs lv = new _leavs();
            _attendence att = new App_Codes._attendence();
            _transfers tr = new _transfers();

            ApprovalsViewModel avm = new Models.ApprovalsViewModel();
            avm.LeaveApprovals = lv.GetUserLeaveRequest(Session["UserId"].ToString());
            avm.InOutApprovals = att.GetUserInOutRequests(Session["UserId"].ToString());
            avm.EmployeeTransfer = tr.GetRequestsToApprove(Session["UserId"].ToString());
            return View(avm);
        }

        public ActionResult ApproveLeave()
        {
            _leavs lv = new _leavs();
            LeaveHistoryViewModel model = new LeaveHistoryViewModel();

            model.Id = Convert.ToInt32(Request.Form["hfLeaveId"].ToString());

            if (Request.Form["hfIsApprove"].ToString() == "1")
            {
                model.IsApproved = "true";
            }
            else
            {
                model.IsApproved = "false";

                if (Request.Form["txtRemarks"].ToString().Length > 0)
                {
                    model.Remarks = Request.Form["txtRemarks"].ToString();
                }
            }


            if (lv.ApproveLeave(model))
            {
                if(model.IsApproved == "true")
                {
                    TempData["erMsg"] = "successMsg('Leave Request Approved')";
                }
                else
                {
                    TempData["erMsg"] = "successMsg('Leave Request Rejected')";
                }
                
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Cannot Complete Your Request')";
            }

            return RedirectToAction("Index");
        }

        public ActionResult ForwardLeave()
        {
            _leavs lv = new _leavs();

            var leaveId = Convert.ToInt32(Request.Form["hfForwardLeaveId"].ToString());
            var userId = Request.Form["hfForwardTo"].ToString();

            if (lv.ForwardLeave(leaveId, userId))
            {
                TempData["erMsg"] = "successMsg('Leave Request Submitted For Optional Approval')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Leave Request Could Not Be Submit')";
            }

            return RedirectToAction("Index");
        }

        public ActionResult ApproveInOut()
        {
            _attendence att = new App_Codes._attendence();
            ApproveInOutViewModel model = new Models.ApproveInOutViewModel();
            model.Id = Convert.ToInt32(Request.Form["hfInOutId"].ToString());

            if (Request.Form["hfInOutApprove"].ToString() == "1")
            {
                model.IsApproved = "true";
            }
            else
            {
                model.IsApproved = "false";

                if (Request.Form["txtInOutRemarks"].ToString().Length > 0)
                {
                    model.Remarks = Request.Form["txtInOutRemarks"].ToString();
                }
            }

            if (att.ApproveInOut(model))
            {
                if (model.IsApproved == "true")
                {
                    TempData["erMsg"] = "successMsg('Attendence Correction Request Approved')";
                }
                else
                {
                    TempData["erMsg"] = "successMsg('Attendence Correction Request Rejected')";
                }

            }
            else
            {
                TempData["erMsg"] = "errorMsg('Request Could Not Be Approved')";
            }

            return RedirectToAction("Index");
        }

        public ActionResult ApproveTransfer()
        {
            _transfers tr = new App_Codes._transfers();
            TransferApprovalViewModel model = new Models.TransferApprovalViewModel();
            model.RequestId = Convert.ToInt32(Request.Form["hfTransferId"].ToString());

            if (Request.Form["hfTransferApprove"].ToString() == "1")
            {
                model.Approval = "true";
            }
            else
            {
                model.Approval = "false";

                if (Request.Form["txtTransferRemarks"].ToString().Length > 0)
                {
                    model.Remarks = Request.Form["txtTransferRemarks"].ToString();
                }
            }

            if (tr.ApproveTransfer(model))
            {
                if (model.Approval == "true")
                {
                    TempData["erMsg"] = "successMsg('Transfer Request Approved')";
                }
                else
                {
                    TempData["erMsg"] = "successMsg('Transfer Request Rejected')";
                }

            }
            else
            {
                TempData["erMsg"] = "errorMsg('Request Could Not Be Approved')";
            }

            return RedirectToAction("Index");
        }
    }
}