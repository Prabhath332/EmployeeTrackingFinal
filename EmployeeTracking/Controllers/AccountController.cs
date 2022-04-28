using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EmployeeTracking.Models;
using EmployeeTracking.App_Codes;

namespace EmployeeTracking.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult PassReset()
        {
            _usermanager um = new App_Codes._usermanager();
            var user = um.GetUser("f5c16088-d978-4775-b16e-b938ec1d57bf");
            _emails em = new App_Codes._emails();

            if (um.ResetPassword("f5c16088-d978-4775-b16e-b938ec1d57bf"))
            {
                TempData["ErMsg"] = "successMsg('User Password Has Been Reset to Default 'abc@123'');";
                //em.PasswordResetMail(user);
            }
            else
            {
                TempData["ErMsg"] = "errorMsg('Password Cannot be Reset');";

            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
                        
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, true, shouldLockout: false);
            if (result == SignInStatus.Failure)
            {
                TempData["ErMsg"] = "errorMsg('Wrong Username or Password')";
                return View("Index");
            }

            _usermanager um = new _usermanager();
            var parm = um.UserParm(model.UserName);

            switch (result)
            {
                case SignInStatus.Success:
                    Session["FullName"] = um.FullUserName(model.UserName);
                    Session["UserId"] = parm[0];
                    Session["UserRole"] = um.GetUserRoles(Session["UserId"].ToString()).FirstOrDefault();
                    Session["UserImage"] = um.GetUserImage(model.UserName);

                    if (Session["UserRole"].ToString() == "SuperAdmin")
                    {
                        Session["DivisionId"] = "0";
                        Session["CompanyId"] = "0";
                    }
                    else
                    {
                        if(parm.Count > 1)
                        {
                            Session["DivisionId"] = um.UserParm(model.UserName)[1];
                            Session["CompanyId"] = um.UserParm(model.UserName)[2];
                        }
                        else
                        {
                            Session["DivisionId"] = 0;
                            Session["CompanyId"] = 0;
                        }
                                                
                    }
                    
                    
                    return RedirectToAction("Index", "Home");
                case SignInStatus.Failure:
                default: return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.RemoveAll();
            return RedirectToAction("Index", "Account");
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Account");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}