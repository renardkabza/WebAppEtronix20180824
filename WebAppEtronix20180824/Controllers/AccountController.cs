using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebAppEtronix20180824.Models;
using WebAppEtronix20180824.Models.Entity;
using WebAppEtronix20180824;
using WebAppEtronix20180824.App_Start.Other;
using PagedList;
using WebAppEtronix20180824.Models.DTO;
using System.Security.Cryptography.Pkcs;



using System.Data.SqlClient;
using System.Security.Cryptography;

namespace WebAppEtronix20180824.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private Entities _contextEntities;
      

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;

            if (!ModelState.IsValid)
            {
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList("User Name & Pasword are required");
                model.EtronixValidation = EV;
                return View(model);
            }
            

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    //ModelState.AddModelError("", "Invalid login attempt.");
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList("Invalid login attempt");
                    model.EtronixValidation = EV;
                    return View(model);
            }

            /*
            if (ModelState.IsValid)
            {
                using (var context = new Entities())
                {
                    //verify whether this user exists
                    //Linq 
                    //Method Syntax
                    try
                    {
                        Models.Entity.AspNetUsers query = context.AspNetUsers.Where(a => a.Email == model.Email)
                            .FirstOrDefault();
                    }
                    catch (Exception ex1)
                    {
                        string a = ex1.ToString();
                    }
                    //Ling Synstax the entire row
                    IQueryable<AspNetUsers> vUser =context.AspNetUsers.Where(r => r.Email == model.Email).Select(item => item);

                    //LINQ query
                    //Query syntax
                     var  query2   = (from c in context.AspNetUsers
                                      where c.Email==model.Email
                                      select c).ToList();


                    


                    return RedirectToAction("Index", "Home");
                }
                
            }
            */

            return RedirectToAction("Login", new {returnUrl=""});
        }


        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
      
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        
        //GET: /Account/Delete
        [Authorize]
        public ActionResult Delete()
        {
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {   
            // !User.Identity.IsAuthenticated
            string vLoggedUserId = User.Identity.GetUserId();

            //controller name
            string vActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string vControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var vAccess=false;
            if (vLoggedUserId != null)
            {
                _contextEntities = new Entities();

                var vClientIdParameter = new SqlParameter("@UserID", vLoggedUserId);
                var vControllerNameParameter = new SqlParameter("@ControllerName", vControllerName);
                var vActionNameParameter = new SqlParameter("@ActionName", vActionName);

                vAccess = Convert.ToBoolean(_contextEntities.Database
                    .SqlQuery<System.String>("CheckUserAccess " +
                                             "@UserID, " +
                                             "@ControllerName, " +
                                             "@ActionName",
                        vClientIdParameter,
                        vControllerNameParameter,
                        vActionNameParameter).ToList()[0]);
            }
            else
            {
                vAccess = false;
            }
            if (vAccess == false)
            {
                return PartialView("Error");
            }
            
            

            return View();
        }
        
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;
            _contextEntities = new Entities();
            try
            {

                
            
            if (ModelState.IsValid)
            {
                

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    //RK - send an email
                    EtronixEmail vEmail = new EtronixEmail();
                    vEmail.VEmailBody = "User:" + model.Email.ToString() + Environment.NewLine +
                                        "Password:" + model.Password.ToString();
                    vEmail.VSendTo = model.Email;
                    vEmail.SendEmail();

                    alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_success;
                    vMessage = "User:" + model.Email + " was created successfully";
                               
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    EV.AddToList("The password was sent to:" + model.Email);
                    model.EtronixValidation = EV;


                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //return RedirectToAction("Index", "Home");


                    //2020-11-16 Update the Points table with no access to all points
                    var vUserId = new SqlParameter("@UserId", user.Id);
                    
                    var vUpdatePointsAccess = _contextEntities.Database.SqlQuery<int>(
                        "UpdatePointsAccess @UserId ",
                        vUserId
                        ).ToList();

                    if (vUpdatePointsAccess[0] == 0)
                    {
                        alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_success;
                        EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                        EV.AddToList("The access to all points for the new user was updated");
                        foreach (var resultError in result.Errors)
                        {
                            EV.AddToList(resultError);
                        }
                        model.EtronixValidation = EV;
                        return View(model);
                    }
                    else
                    {
                        alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                        EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                        EV.AddToList("The UpdatePointsAccess procedure created an error");
                        foreach (var resultError in result.Errors)
                        {
                            EV.AddToList(resultError);
                        }
                        model.EtronixValidation = EV;
                        return View(model);
                    }
                    return View(model);
                }

                //The usser is not added        
                else
                {
                

                    //1st ver
                    //AddErrors(result);

                    //2nd ver
                    alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    foreach (var resultError in result.Errors)
                    {
                        EV.AddToList(resultError);
                    }
                    model.EtronixValidation = EV;
                    return View(model);
                }

            }
            else //model is invalid
            {
                List<string> mE = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                foreach (var msg in mE)
                {
                   EV.AddToList(msg);
                }
                model.EtronixValidation = EV;
               return View(model);
            }

            }
            catch (Exception e)
            {
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(e.Message);
                model.EtronixValidation = EV;
                return View(model);
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> UpdateUserPassword(UpdateUserPasswordViewModel model)
        {


            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
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
            return RedirectToAction("Index", "Home");
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

        //Firsl Load
        public ActionResult Index(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {
            return View();
        }

        //Get: Return a tablet as a partial view
        public ActionResult IndexTableActionResult(string userName, string email, string sortColumn,string sortOrder, int? currentpage, int? ppageSize)
        {
            var vUsers = AspNetUsers(userName, email, sortColumn, sortOrder, currentpage, ppageSize);


            return PartialView("_Account_IndexTable", vUsers);
        }

        //Users
        private IPagedList<AspNetUsers> AspNetUsers(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {
            Entities db = new Entities();
            int pageIndex = Convert.ToInt32(currentpage);
            int? vsalary;
            
            bool pagehaschange = false;

            int pageSize = Convert.ToInt32(ppageSize);
            if (pageSize == 0)
            {
                pageSize = 1;
            }
            if (currentpage.Equals(null))
            {
                //pageIndex = currentpage.HasValue ? Convert.ToInt32(page) : 1;
                pageIndex = 1;
                ViewBag.CurrentPage = 1;
                ViewBag.PreviousPage = 1;
                pagehaschange = false;
            }

            if (currentpage <= 0)
            {
                pageIndex = 1;
                ViewBag.CurrentPage = 1;
                ViewBag.PreviousPage = 1;
                pagehaschange = false;
            }


            //ViewBag.CurrentSort = currentSort;
            //ViewBag.PreviousSort = previousSort;


            string vsortColumn = String.IsNullOrEmpty(sortColumn) ? "User Name" : sortColumn;
            IPagedList<AspNetUsers> vAspNetUsers = null;
            List<AspNetUsers> vAspNetUsersMasters = null;
            ViewBag.SortColumn = vsortColumn;


            ViewBag.CurrentPage = pageIndex;

            var flag = 1;
        takedata:
            switch (vsortColumn)
            {
                case "User Name":
                    if (sortOrder == "Desc")
                    {
                        //employees = db.Employees.Where(m=>m.Name.Contains("10")).OrderByDescending(m => m.Name).ToPagedList(pageIndex, pageSize);
                        //or
                        //employees = db.Employees.OrderByDescending(m => m.Name).ToPagedList(pageIndex, pageSize);
                        //or some exmple from the internet
                        /*_db.EMPLOYEEs.Where(x => x.EMAIL == givenInfo || x.USER_NAME == givenInfo)
                        .Select(x => new { x.EMAIL, x.ID }); */
                        vAspNetUsersMasters = db.AspNetUsers.OrderByDescending(m => m.UserName).ToList();
                        
                    }
                    else
                    {
                        vAspNetUsersMasters = db.AspNetUsers.OrderBy(m => m.UserName).ToList();
                    }

                    break;
                case "Email":
                    if (sortOrder == "Desc")
                        vAspNetUsersMasters = db.AspNetUsers.OrderByDescending(m => m.Email).ToList();
                    else
                        vAspNetUsersMasters = db.AspNetUsers.OrderBy(m => m.Email).ToList();
                    break;
                default: //by Employee Name and ASC
                    vAspNetUsers = db.AspNetUsers.OrderBy(m => m.UserName).ToPagedList(pageIndex, pageSize);
                    break;
            }

            if (userName != "")
            {
                vAspNetUsersMasters = vAspNetUsersMasters.Where(m => m.UserName.Contains(userName)).ToList();
            }
            if (email != "")
            {
                vAspNetUsersMasters = vAspNetUsersMasters.Where(m => m.Email.Contains(email)).ToList();
            }
            
            vAspNetUsers = vAspNetUsersMasters.ToPagedList(pageIndex, pageSize);


            int rowsquantity = vAspNetUsers.Count;


            if (flag == 1)
            {
                flag = 0;
                if (rowsquantity == 0)
                {
                    currentpage = 1;
                    ViewBag.CurrentPage = 1;
                    pageIndex = 1;

                    goto takedata;
                }
            }
            return vAspNetUsers;
        }

        //Goto Previous Page
        public ActionResult PreviousPageActionResult(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {

            int? v_currentpage = currentpage;
            v_currentpage--;
            var employees = AspNetUsers(userName, email, sortColumn, sortOrder, v_currentpage, ppageSize);


            return PartialView("_Account_IndexTable", employees);



        }

        //Goto Previous Page
        public ActionResult NextPageActionResult(string userName, string email, string sortColumn, string sortOrder, int? currentpage, int? ppageSize)
        {

            int? v_currentpage = currentpage;
            v_currentpage++;
            var employees = AspNetUsers(userName, email, sortColumn, sortOrder, v_currentpage, ppageSize);


            return PartialView("_Account_IndexTable", employees);



        }
        

        //delete User
        [HttpGet]
        public ActionResult DeleteUser(string userId)
        {
            Procedures.ResultDeleteUser  modelDeleteUser = new Procedures.ResultDeleteUser();
            _contextEntities = new Entities();
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;

            try
            {
                if (userId != null)
                {

                    var clientIdParameter = new SqlParameter("@UserID", userId);
                    var vMainMenu = _contextEntities.Database
                        .SqlQuery<Procedures.ResultDeleteUser>("DeleteUser @UserId", clientIdParameter).ToList();

                    alert = -1;
                    vMessage = null;

                    //alert success
                    alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_success;
                    vMessage = "User:" + userId + " was successfully deleted" + Environment.NewLine;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    modelDeleteUser.EtronixValidation = EV;

                    //delete all points of this users. When a user is deleted points are not needed
                    //not used
                    //Delete Rule was set to FK_Access_ASPNetUserId on the Access table
                    /*
                    var vDeleteAccessPoint = _contextEntities.Database
                        .SqlQuery<int>("DeletePointsAccess @UserId", clientIdParameter).ToList();

                    EV.AddToList("All user's points were deleted");
                                          * */
                    EV.AddToList("Related points to this user were deleted");
                }
            }
            catch (Exception es)
            {
                //Console.WriteLine(exception.Message);
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vMessage = es.Message + Environment.NewLine;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                EV.AddToList(vMessage);
            }

            return PartialView("_Account_DeleteUser", modelDeleteUser);
        }

        public ActionResult UsersRoles(string userID)
        {
            _contextEntities = new Entities();
            var clientIdParameter = new SqlParameter("@UserID", userID);
            var vUserName = _contextEntities.Database
                .SqlQuery<string>("GetUserName @UserID", clientIdParameter).FirstOrDefault();


            UserRoles vUserRoles = new UserRoles();
            vUserRoles.ID = userID;
            vUserRoles.UserName = vUserName;
            return View(vUserRoles);
        }

        public ActionResult GetUserRoles(string userID)
        {
            //UserRoles vUserRoles = new UserRoles();
            //vUserRoles.ID = userID;

            _contextEntities = new Entities();
            List<Procedures.ResultGetAllRoles> vAllRoles = _contextEntities.Database
                .SqlQuery<Procedures.ResultGetAllRoles>("GetAllRoles").ToList();


            var clientIdParameter = new SqlParameter("@UserID", userID);
            List<Procedures.ResultGetUserRoles> vUserRoles = _contextEntities.Database
                .SqlQuery<Procedures.ResultGetUserRoles>("GetUserRoles @UserID", clientIdParameter).ToList();

            foreach (Procedures.ResultGetAllRoles itemAllRoles in vAllRoles)
            {

                var item = vUserRoles.Find(x => x.RoleId == itemAllRoles.Id);
                if (item != null)
                {
                    itemAllRoles.Access = true;
                }
                else
                {
                    itemAllRoles.Access = false;
                }
            }

            return PartialView("_Account_GetUserRoles", vAllRoles);
        }

        public ActionResult UpdateUserRoles(LinkedList<UserRolesParameters> pUserRolesParameters)
        {
            UserRolesParameters vUrRolesParameters;
            EtronixValidation vEtronixValidation;
            
            try
            {

                //throw new System.ArgumentException("Error Test","someparameter");
                
                vUrRolesParameters = pUserRolesParameters.ElementAt(0);

                _contextEntities = new Entities();

                string vUserId = "";
                bool vFlag = false;
                string vSubMenu = "";

                foreach (var item in pUserRolesParameters)
                {
                    vUserId = item.SUserID;
                    vFlag = bool.Parse(item.SValue);
                    vSubMenu = item.SID;

                    //delete one role by one
                    var vClientIdParameter = new SqlParameter("@UserID", vUserId);
                    var vSubMenuParameter = new SqlParameter("@SubMenuID", vSubMenu);
                    var vMainMenu = _contextEntities.Database
                        .SqlQuery<Procedures.ResultDeleteUser>("DeleteUserRole " +
                                                               "@UserID, " +
                                                               "@SubMenuID",
                            vClientIdParameter,
                            vSubMenuParameter).ToList();

                    //if true insert it again.
                    if (vFlag == true)
                    {
                        var vClientIdParameter2 = new SqlParameter("@UserID", vUserId);
                        var vSubMenuParameter2 = new SqlParameter("@SubMenuID", vSubMenu);
                        var vMainMenu2 = _contextEntities.Database
                            .SqlQuery<Procedures.ResultDeleteUser>("InsertUserRole " +
                                                                   "@UserID, " +
                                                                   "@SubMenuID",
                                vClientIdParameter2,
                                vSubMenuParameter2).ToList();
                    }

                }

                vEtronixValidation = new EtronixValidation();
                vUrRolesParameters.EtronixValidation = vEtronixValidation;
                vUrRolesParameters.EtronixValidation.ValidationMessage.Add("User:" + vUserId.ToString() + "was modified");
                int vCode = (int)EtronixValidationCode.ValidationCodeEnum.alert_success;
                vUrRolesParameters.EtronixValidation.ValidationCode = EtronixValidationCode.ValidationDic[vCode];

            }
            catch (Exception eErrorException)
            {
                vEtronixValidation = new EtronixValidation();
                vUrRolesParameters= new UserRolesParameters();
                vUrRolesParameters.EtronixValidation = vEtronixValidation;
                vUrRolesParameters.EtronixValidation.ValidationMessage.Add(eErrorException.Message);
                int vCode = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                vUrRolesParameters.EtronixValidation.ValidationCode = EtronixValidationCode.ValidationDic[vCode];
                
            }
            


            

            return PartialView("_Account_SetUserRoles", vUrRolesParameters);


            //return Content("User name which will be modified:"+vUrRolesParameters.SID);
        }

        // GET: /Account/Register
        [AllowAnonymous]
        [HttpGet]
        public ActionResult EditUser(string userID)
        {


            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;

                ApplicationUser user1 = UserManager.FindById(userID);

                UpdateUserPasswordViewModel vUpdateUserPassword = new UpdateUserPasswordViewModel();



                try
                {
                    _contextEntities = new Entities();
                    var vUserParameter = new SqlParameter("@userID", userID);
                    Procedures.ResultGetUsersDetails vUserDetails =
                        _contextEntities.Database.SqlQuery<Procedures.ResultGetUsersDetails>(
                            "GetUsersDetails " + "@UserID ",
                            vUserParameter).SingleOrDefault();

                    vUpdateUserPassword.UserId = vUserDetails.Id;
                    vUpdateUserPassword.UserName = vUserDetails.UserName;
                    vUpdateUserPassword.Email = vUserDetails.Email;
                    vUpdateUserPassword.EtronixValidation = EV;
                    vUpdateUserPassword.Password = "";
                    vUpdateUserPassword.ConfirmPassword = "";


                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception.Message);
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    vMessage = exception.Message + Environment.NewLine;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);

                }
            

            return View(vUpdateUserPassword);
        }

        // POST: /Account/EditUser
        //Update Details
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(UpdateUserPasswordViewModel model)
        {
            EtronixValidation EV = new EtronixValidation();
            int alert = -1;
            string vMessage = null;

            //check if all fields are correctly filled with data - data validation
            if (ModelState.IsValid)
            {

                ApplicationUser user1 = UserManager.FindById(model.UserId);

                UpdateUserPasswordViewModel vUpdateUserPassword = new UpdateUserPasswordViewModel();


                try
                {
                    _contextEntities = new Entities();
                    var vUserParameter1 = new SqlParameter("@userID", model.UserId);
                    var vUserParameter2 = new SqlParameter("@userName", model.UserName);
                    var vResult = _contextEntities.Database.SqlQuery<List<string>>(
                            "UpdateUsersDetails " + 
                            "@userID, "+
                            "@userName",
                            vUserParameter1,
                            vUserParameter2).SingleOrDefault();

                    

                    //CreateTempUser
                    string vTempUser = Guid.NewGuid().ToString() + "@gmail.com";
                    ApplicationUser user = new ApplicationUser { UserName = vTempUser, Email = vTempUser };
                    //genretae automaticaly a new password
                    string vPassword = model.Password;
                    var vIdentityResult = UserManager.Create(user, vPassword);
                    var vIdentityResult2 = vIdentityResult;

                    if (vIdentityResult.Succeeded == true)
                    {
                        string vNewUserId = user.Id;
                        string vOriginUserId = model.UserId;
                        //swapp hashes

                        var vUserParameter3 = new SqlParameter("@OldUserID", model.UserId);
                        var vUserParameter4 = new SqlParameter("@NewUserId", user.Id);

                        var vResult2 = _contextEntities.Database.SqlQuery<List<string>>(
                            "UpdateUsersHashPassword " +
                            "@OldUserID, " +
                            "@NewUserId",
                            vUserParameter3,
                            vUserParameter4).SingleOrDefault();

                        //RK - send an email
                        EtronixEmail vEmail = new EtronixEmail();
                        vEmail.VEmailBody = "User:" + model.Email.ToString() + Environment.NewLine +
                                            "Password:" + model.Password.ToString();
                        vEmail.VSendTo = model.Email;
                        vEmail.SendEmail();

                        alert = (int) EtronixValidationCode.ValidationCodeEnum.alert_success;
                        vMessage = "UserName & Email - updated";

                        EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                        EV.AddToList(vMessage);
                        EV.AddToList("Password - a new password was generated and sent via email");

                        model.EtronixValidation = EV;
                    }
                    else
                    {
                        alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                        EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                        foreach (var msg in vIdentityResult.Errors)
                        {
                            EV.AddToList(msg);
                        }
                        model.EtronixValidation = EV;
                        return View(model);
                    }
                    


                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception.Message);
                    alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                    vMessage = exception.Message + Environment.NewLine;
                    EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                    EV.AddToList(vMessage);
                    model.EtronixValidation = EV;

                }
            }
            else
            {
                List<string> mE = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                alert = (int)EtronixValidationCode.ValidationCodeEnum.alert_danger;
                EV.ValidationCode = EtronixValidationCode.ValidationDic[alert];
                foreach (var msg in mE)
                {
                    EV.AddToList(msg);
                }
                model.EtronixValidation = EV;
                return View(model);
              
                
            }

            return View(model);
        }


        public ApplicationUser UpdateUsersPassword(string userID)
        {
            //take the ID of the logged user
            ApplicationUser user1 = UserManager.FindById(userID);

            //get details of the changing person
            _contextEntities = new Entities();
            var vUserParameter = new SqlParameter("@userID", userID);
            Procedures.ResultGetUsersDetails vUserDetails =
                _contextEntities.Database.SqlQuery<Procedures.ResultGetUsersDetails>(
                    "GetUsersDetails " + "@UserID ",
                    vUserParameter).SingleOrDefault();

            Procedures.ResultGetUsersDetails vUserDetails2 = vUserDetails;
            //create a temp new user
            
            string vTempUser = Guid.NewGuid().ToString() + "@gmail.com";
            ApplicationUser user = new ApplicationUser { UserName = vTempUser, Email = vTempUser };
            //genretae automaticaly a new password
            string vPassword = GenerateRandomPassword().ToString();
            var vIdentityResult = UserManager.Create(user, vPassword );
            var vIdentityResult2 = vIdentityResult;
            if (vIdentityResult.Succeeded == true)
            {
                string vNewUserId = user.Id;
                string vOriginUserId = userID;
                //swapp hashes

            }

            Console.WriteLine("Koniec");

            return user;
        }

        public ActionResult UpdateUsersDetails(string userID)
        {
            //take the ID of the logged user
            ApplicationUser user1 = UserManager.FindById(userID);

            _contextEntities = new Entities();
            var vUserParameter = new SqlParameter("@userID", userID);
            Procedures.ResultGetUsersDetails vUserDetails =
                _contextEntities.Database.SqlQuery<Procedures.ResultGetUsersDetails>(
                    "GetUsersDetails " + "@UserID ",
                    vUserParameter).SingleOrDefault();

            Procedures.ResultGetUsersDetails vUserDetails2 = vUserDetails;
            //create a temp new user
            string vTempUser = Guid.NewGuid().ToString() + "@gmail.com";
            ApplicationUser user = new ApplicationUser { UserName = vTempUser, Email = vTempUser };
            var vIdentityResult = UserManager.Create(user, "Z@sada2020");
            var vIdentityResult2 = vIdentityResult;
            if (vIdentityResult.Succeeded == true)
            {
                string vNewUserId = user.Id;
            }

            Console.WriteLine("Koniec");

            return View();
        }


        public ActionResult GenerateRandomPassword()
        {

            int RequiredLength = 8;
            int RequiredUniqueChars = 4;
            bool RequireDigit = true;
            bool RequireLowercase = true;
            bool RequireNonAlphanumeric = true;
            bool RequireUppercase = true;
            

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-"                        // non-alphanumeric
            };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < RequiredLength
                                      || chars.Distinct().Count() < RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return Content(string.Join("", chars.ToArray()));
        }

        

       
    }
}