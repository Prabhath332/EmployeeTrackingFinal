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
    [AdminAuthorization]
    [SessionExpireFilter]
    public class NewsController : Controller
    {
        // GET: News
        //public ActionResult Index()
        //{
        //    _news nw = new App_Codes._news();

        //    return View(nw.GetNews(Session["UserId"].ToString()));
        //}

        public ActionResult Latest()
        {
            ViewBag.erMsg = TempData["erMsg"];
            _news nw = new App_Codes._news();
            return View(nw.GetNews(Session["UserId"].ToString()));
        }

        public ActionResult NewsDetails(int NewsId = 0)
        {
            _news nw = new _news();
            ViewBag.erMsg = TempData["erMsg"];

            if (NewsId > 0)
            {
                return View(nw.GetNews(NewsId));
            }
            else
            {
                return View(new News());
            }

        }

        public ActionResult SaveNews(News model)
        {
            _news nw = new _news();

            var files = Request.Files.GetMultiple("fupNewsImages");
            
            if (nw.SaveNews(model, files, Session["UserId"].ToString()))
            {
                TempData["erMsg"] = "successMsg('New Details Saved')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('News Details Could Not be Saved')";
            }

            return RedirectToAction("Latest");
        }

        public ActionResult DeleteNews(int newsId)
        {
            _news nw = new _news();

            if (nw.DeleteNews(newsId))
            {
                TempData["erMsg"] = "successMsg('News Deleted')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('News Could Not be Delete')";
            }

            return Redirect("Latest");
        }

        public ActionResult DeleteNewsImage(int newsId)
        {
            _news nw = new _news();
            var imgName = Request.Form.AllKeys.FirstOrDefault().Replace("btnDelImg_", "");

            if (nw.DeleteNewsImage(newsId, imgName))
            {
                TempData["erMsg"] = "successMsg('Image Deleted')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Image Could Not be Delete')";
            }

            return RedirectToAction("NewsDetails", new { NewsId = newsId });
        }
    }
}