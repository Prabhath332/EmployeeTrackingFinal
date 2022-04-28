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
    public class DivisionsController : Controller
    {
        public ActionResult Index()
        {
            _projects pr = new App_Codes._projects();
            ViewBag.erMsg = TempData["erMsg"];
            return View(pr.GetProjects());
        }

        public ActionResult Teams(string id)
        {
            ACRYPTO.ACRYPTO obj = new ACRYPTO.ACRYPTO();
            _projects pr = new App_Codes._projects();
            ViewBag.erMsg = TempData["erMsg"];
            TempData.Remove("erMsg");

            if (id == null)
            {
                //id = TempData["DivisionId"].ToString();
                return View(pr.GetTeams());
                //return View(pr.GetTeams(Convert.ToInt32(id)));
            }
            else if(obj.Decrypt(id) == "0")
            {
                _usermanager um = new _usermanager();
                var user = um.GetUser(Session["UserId"].ToString());

                if(user.EmployeementInfos.Division == null)
                {
                    TempData["DivisionId"] = "0";
                    return View(pr.GetTeams());
                }
                else
                {
                    TempData["DivisionId"] = user.EmployeementInfos.Division;
                    return View(pr.GetTeams(Convert.ToInt32(TempData["DivisionId"].ToString())));
                }
                
            }
            else
            {
                id = obj.Decrypt(id);
                TempData["DivisionId"] = id;

                var str = Session["DivisionId"] as string;

                if (str == "0")
                {
                    Session["TempDivisionId"] = id;
                }

                return View(pr.GetTeams(Convert.ToInt32(id)));
            }
            
            
            
        }

        public ActionResult TeamMembers(string id)
        {
            ACRYPTO.ACRYPTO obj = new ACRYPTO.ACRYPTO();
            ViewBag.erMsg = TempData["erMsg"];
            TempData.Remove("erMsg");
            
            TempData["TeamId"] = obj.Decrypt(id);
            ViewBag.DivisionId = Session["TempDivisionId"].ToString();
            //TempData["DivisionId"] = Session["TempDivisionId"].ToString();
            _projects pr = new App_Codes._projects();
            return View(pr.GetTeamMemebers(Convert.ToInt32(obj.Decrypt(id))));
        }


        public ActionResult AddTeamMember()
        {
            ACRYPTO.ACRYPTO obj = new ACRYPTO.ACRYPTO();
            string UserId = Request.Form["hfUserId"].ToString();
            string TeamId = Request.Form["hfTeamId"].ToString();
            string DivisionId = Request.Form["hfDivisionId"].ToString();

            _projects pr = new App_Codes._projects();

            if(pr.AddTeamMember(UserId, Convert.ToInt32(TeamId), Convert.ToInt32(DivisionId)))
            {
                TempData["erMsg"] = "successMsg('Team Member Added.')";                
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Team Memeber Could Not be Added.')";
            }

            return RedirectToAction("TeamMembers", new { id = obj.Encrypt(TeamId) });
        }

        public ActionResult MakeSupervisor(string UserId, int TeamId)
        {
            ACRYPTO.ACRYPTO obj = new ACRYPTO.ACRYPTO();

            _projects pr = new App_Codes._projects();

            if (pr.AddSupervisor(UserId, TeamId))
            {
                TempData["erMsg"] = "successMsg('Team Supervisor Updated.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Team Supervisor Could Not be Updated.')";
            }

            return RedirectToAction("TeamMembers", new { id = obj.Encrypt(TeamId.ToString()) });
        }

        public ActionResult RemoveUserFromTeam(string UserId, int TeamId)
        {
            ACRYPTO.ACRYPTO obj = new ACRYPTO.ACRYPTO();

            _projects pr = new App_Codes._projects();

            if (pr.RemoveTeamMember(UserId, TeamId))
            {
                TempData["erMsg"] = "successMsg('Team Member Removed.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Team Member Cannot be Removed.')";
            }

            return RedirectToAction("TeamMembers", new { id = obj.Encrypt(TeamId.ToString()) });
        }

        public ActionResult NewTeam()
        {
            string TeamName = Request.Form["txtTeamName"].ToString(); 
            string TeamLocation = Request.Form["txtTeamLocation"].ToString();
            string TeamId = Request.Form["hfTeamId"].ToString();
            string DivisionId = Request.Form["hfDivisionId"].ToString();
            _projects pr = new App_Codes._projects();
            ProjectTeam pt = new Models.ProjectTeam();

            pt.TeamName = TeamName;
            pt.Location = TeamLocation;
            pt.ProjectId = Convert.ToInt32(DivisionId);

            if (pr.AddTeam(Convert.ToInt32(TeamId), pt))
            {
                TempData["erMsg"] = "successMsg('Team Details for <b>" + TeamName + "</b> Saved.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Team Could Not be Saved.')";
            }

            ACRYPTO.ACRYPTO obj = new ACRYPTO.ACRYPTO();
            //TempData["DivisionId"] = DivisionId;
            return RedirectToAction("Teams", new { id = obj.Encrypt(DivisionId)});
        }

        public ActionResult NewDivision()
        {
            string DivisionName = Request.Form["txtDivisionName"].ToString();
            string DivisionDescription = Request.Form["txtDivisionDescription"].ToString();
            string DivisionManager = Request.Form["txtDivisionManager"].ToString();
            string Company = "";

            try
            {
                Company = Request.Form["ddlCompanies"].ToString();
            }
            catch
            {

            }
            
            string DivisionId = Request.Form["hfDivisionId"].ToString();

            Project pr = new Models.Project();
            pr.ProjectName = DivisionName;
            pr.Description = DivisionDescription;
            pr.ProjectManager = DivisionManager;

            if(!string.IsNullOrEmpty(Company))
            {
                pr.CompanyId = Convert.ToInt32(Company);
            }
            

            _projects obj = new App_Codes._projects();

            if (obj.AddDivision(Convert.ToInt32(DivisionId), pr))
            {
                TempData["erMsg"] = "successMsg('Division Details for <b>" + DivisionName + "</b> Saved.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Division Could Not be Saved.')";
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteDivision()
        {
            int Id = Convert.ToInt32(Request.Form["hfDelDivision"].ToString());

            _projects obj = new App_Codes._projects();

            if (obj.DeleteDivision(Id))
            {
                TempData["erMsg"] = "successMsg('Division Deleted.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Division Could Not be Deleted.')";
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteTeam()
        {
            int Id = Convert.ToInt32(Request.Form["hfDelTeam"].ToString());
            string DivId = Request.Form["hfDelDiv"].ToString();
            _projects obj = new App_Codes._projects();

            if (obj.DeleteTeam(Id))
            {
                TempData["erMsg"] = "successMsg('Team Deleted.')";
            }
            else
            {
                TempData["erMsg"] = "errorMsg('Team Could Not be Deleted.')";
            }

            return RedirectToAction("Teams", new { id = DivId });
        }
    }
}