using App.DataAccess;
using App.DataAccess.Identity;
using App.Web.Models;
using App.Web.Services.Contracts;
using App.Web.Utilities;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace App.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AccountController));
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext _context;
        private IAccountServices _services;
        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) {
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager, IAccountServices services
            ) {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            _services = services;
            
        }

        public ApplicationSignInManager SignInManager { get; private set; }

        public ApplicationUserManager UserManager { get; private set; }
        public ApplicationRoleManager RoleManager { get; private set; }

        [HttpPost]
        [AppAuth(Action = AppActions.User_Management)]
        public ActionResult UserList(DttRequestWithDate form) {
            return Json(_services.GetList(form));
        }

        [HttpPost]
        [AppAuth(Action = AppActions.User_Management_Roles)]
        public ActionResult RoleList(DttRequestWithDate form) {
            return Json(_services.GetRoleList(form));
        }

        [HttpGet]
        [AppAuth(Action = AppActions.User_Management)]
        public ActionResult Users() {
            ViewBag.Roles = Newtonsoft.Json.JsonConvert.SerializeObject(RoleManager.Roles.ToList());
            return View();
        }

        [HttpGet]
        [AppAuth(Action = AppActions.User_Management_Roles)]
        public ActionResult Roles() {
            return View();
        }

        [HttpGet]
        [AppAuth(Action = AppActions.User_Management_Roles)]
        public ActionResult RoleEntry(string id) {

            var vm = _services.GetRoleViewModel(id);

            vm.ActionsOpt = _services.GetActionList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AppAuth(Action = AppActions.User_Management_Roles)]
        public ActionResult RoleEntry(RoleViewModel model) {
            if (!ModelState.IsValid)
            {
                model.ActionsOpt = _services.GetActionList();
                return View(model);
            }

            var vm = _services.InsertRole(model);

            vm.ActionsOpt = _services.GetActionList();

            return RedirectToAction("Roles").Success("Menambahkan role " + vm.Name);
        }

        [HttpPost]
        [AppAuth(Action = AppActions.User_Management)]
        public ActionResult DeleteUser(string id) {
            _services.DeleteUser(id, User.Identity.Name);
            return RedirectToAction("Users").Success("Menghapus user");
        }

        [HttpPost]
        [AppAuth(Action = AppActions.User_Management_Roles)]
        public ActionResult DeleteRole(string id) {
            _services.DeleteRole(id, User.Identity.Name);
            return RedirectToAction("Roles").Success("Mengahpus role");
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            return View();
        }

        //
        // POST: /Account/Login
        //[HttpPost]
        //[Log(action: "Login")]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        // public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return View(model);
        //     }

        //     var user = UserManager.FindByEmail(model.Email);

        //     if (user == null)
        //     {
        //         //ModelState.AddModelError("valerror", "Invalid login attempt.");
        //         //ViewBag.
        //         SetActivityLogData("Invalid login attempt " + model.Email);
        //         return View(model).Error("Invalid Email or Password");
        //     }

        //     if (!user.IsActive)
        //     {
        //         //ModelState.AddModelError("valerror", "Invalid login attempt.");
        //         SetActivityLogData("Invalid login attempt " + model.Email);
        //         return View(model).Error("User Inactive");
        //     }

        //     //var url = Request.Url.AbsoluteUri;
        //     //var ip = Request.UserHostAddress;
        //     //var hostname = Request.UserHostName;
        //     //var userName = model.Email;

        //     //var context = new ApplicationDbContext();

        //     //var log = new ActivityLog();

        //     //log.Action = url;
        //     //log.IPAddress = ip;
        //     //log.HostName = hostname;
        //     //log.Username = userName;
        //     //log.CreatedDate = DateTime.Now;

        //     var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);


        //     //log.Data = result.ToString();
        //     //context.ActivityLogs.Add(log);

        //     //context.SaveChanges();

        //     switch (result)
        //     {
        //         case SignInStatus.Success:
        //             SetActivityLogData("Login Success " + model.Email);
        //             return RedirectToLocal(returnUrl).ShowReminder();
        //         case SignInStatus.LockedOut:
        //             return View("Lockout");
        //         case SignInStatus.RequiresVerification:
        //             return RedirectToAction("SendCode", new { ReturnUrl = returnUrl ?? "/", model.RememberMe });
        //         case SignInStatus.Failure:
        //         default:
        //             SetActivityLogData("Invalid login attempt " + model.Email);
        //             //ModelState.AddModelError("valerror", "Invalid login attempt.");
        //             return View(model).Error("Invalid Email or Password");
        //     }
        // }

        // LDAP Login
        [HttpPost]
        [Log(action: "Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Info("Invalid");
                    return View(model);
                }

                var result = new SignInStatus();

                var user = UserManager.FindByName(model.UserName);
                if (user == null) {
                    SetActivityLogData("Invalid login attempt " + model.UserName);
                    return View(model).Error("Invalid Email or Password");
                }

                if (!user.IsActive) {
                    SetActivityLogData("Invalid login attempt " + model.UserName);
                    return View(model).Error("User Inactive");
                }

                result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                switch (result)
                {
                    case SignInStatus.Success:
                        SetActivityLogData("Login Success " + model.UserName);
                        return RedirectToLocal(returnUrl).ShowReminder();
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl ?? "/", model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        SetActivityLogData("Invalid login attempt " + model.UserName);
                        //ModelState.AddModelError("valerror", "Invalid login attempt.");
                        return View(model).Error("Invalid Email or Password");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
                Log.Info("Error LDAP Login = " +  TempData["ErrorMessage"]);
                return View("Error", TempData["ErrorMessage"]);
            }
        }

        static SearchResult CreateDirectoryEntry(string sAMAccountName, string[] requiredProperties)
        {
            DirectoryEntry ldapConnection = null;

            try
            {
                //DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
                //var defaultNamingContext = rootDSE.Properties["defaultNamingContext"].Value;
                // Create LDAP connection object  
                //ldapConnection = new DirectoryEntry("alpha.company.com");
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["LDAPUserName"]))
                {
                    ldapConnection = new DirectoryEntry(ConfigurationManager.AppSettings["LDAPConnectionString"].ToString());
                }
                else {
                    ldapConnection = new DirectoryEntry(ConfigurationManager.AppSettings["LDAPConnectionString"].ToString(), ConfigurationManager.AppSettings["LDAPUserName"].ToString(), ConfigurationManager.AppSettings["LDAPPassword"].ToString());
                }
                //ldapConnection.Path = connectionPath;
                ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

                DirectorySearcher search = new DirectorySearcher(ldapConnection);
                search.Filter = String.Format("(sAMAccountName={0})", sAMAccountName);

                foreach (String property in requiredProperties)
                    search.PropertiesToLoad.Add(property);

                SearchResult result = search.FindOne();
                //SearchResultCollection searchResultCollection = search.FindAll();

                if (result != null)
                {
                    //foreach (String property in requiredProperties)
                    //    foreach (Object myCollection in result.Properties[property])
                    //        Console.WriteLine(String.Format("{0,-20} : {1}",
                    //                      property, myCollection.ToString()));
                    // return searchResultCollection;
                    return result;
                }
                else
                {
                    return null;
                    //Console.WriteLine("User not found!");
                }
                //return ldapConnection;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
            return null;
        }

        public SearchResult ValidateUserCredentialsViaActiveDirectory(string username, string password)
        {
            var directoryEntry = new DirectoryEntry(ConfigurationManager.AppSettings["LDAPConnectionString"],
                username,
                password,
                AuthenticationTypes.Secure);
            //AuthenticationTypes.Secure);
            
            try
            {
                var directorySearcher = new DirectorySearcher(directoryEntry)
                {
                    Filter = $"(samaccountname={username})"
                };

                try
                {
                    Log.Info("Try to get user");

                    return directorySearcher.FindOne();

                }
                catch (DirectoryServicesCOMException dsce)
                {
                    Log.Info("Error get user data = " + dsce.Message);
                    Log.Info("Stack Trace = " + dsce.StackTrace);
                    if (dsce.Message.Contains("user name or password") || dsce.Message.Contains("user name or bad password"))
                    {
                        Log.Info($"wrong account = {username}, password = {password}");
                        return null;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Log.Info($"LDAP Domain Error : {e.Message}");

                return null;
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe) {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync()) {
                return View("Error");
            }

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe,
                model.RememberBrowser);
            switch (result) {
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

        //
        // GET: /Account/Register
        [AppAuth(Action = AppActions.usermanagement_registrasi)]
        public ActionResult Register(string id) {

            RegisterViewModel vm = new RegisterViewModel();
            if (id != null) {
                var current = UserManager.FindById(id);

                vm.Email = current.Email;
                vm.FullName = current.FullName;
                vm.Id = current.Id;
                vm.Roles = current.Roles;
                vm.IsActive = current.IsActive;
            } else {
                vm = new RegisterViewModel();

                vm.Password = ConfigurationManager.AppSettings["defaultpassword"].ToString();

            }

            vm.RolesOpt = RoleManager.Roles.ToList();
            return View(vm);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AppAuth(Action = AppActions.usermanagement_registrasi)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model) {
            IdentityResult result = null;
            if (model.cmd == "save") {
                if (ModelState.IsValid) {
                    ApplicationUser user = null;

                    var allroles = RoleManager.Roles.ToList();

                    if (model?.Id == null) {
                        user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
                        user.IsActive = model.IsActive;
                        user.Init();
                        user.SetCreated();

                        await UserManager.CreateAsync(user, model.Password);
                        model.SelectedRoles = model.SelectedRoles ?? new List<string>();
                        result = UserManager.AddToRoles(user.Id, RoleManager.Roles.Where(t => model.SelectedRoles.Any(r => r == t.Id)).Select(t => t.Name).ToArray());

                    } else {
                        user = UserManager.FindById(model?.Id);
                        user.SetUpdated();
                        user.IsActive = model.IsActive;
                        user.Email = model.Email;
                        user.FullName = model.FullName;

                        var currentroles = allroles.Where(t => user.Roles.Any(r => r.RoleId == t.Id)).ToList();

                        UserManager.Update(user);

                        UserManager.RemoveFromRoles(user.Id, currentroles.Select(t => t.Name).ToArray());

                        model.SelectedRoles = model.SelectedRoles ?? new List<string>();

                        result = UserManager.AddToRoles(user.Id, RoleManager.Roles.Where(t => model.SelectedRoles.Any(r => r == t.Id)).Select(t => t.Name).ToArray());
                    }



                    //if ((user.CreatedDate.Date == user.UpdatedDate?.Date || user.UpdatedDate == null) && !user.IsDraft) {

                    //    FsmForm fsmform = new FsmForm(user.UserName, user.FullName, "", FsmType.EnableActive);

                    //    _fsmSvc.InsertFsm(fsmform);
                    //}


                    if (result?.Succeeded != true) {
                        AddErrors(result);
                    } else {
                        return RedirectToAction("Users").Success("Menyimpan User " + model.FullName);
                    }
                } else {

                    //var current = UserManager.FindById(model.Id);

                    //model.Email = current.Email;
                    //model.FullName = current.FullName;
                    //model.Id = current.Id;
                    //model.Roles = current.Roles;
                    model.RolesOpt = RoleManager.Roles.ToList();
                    return View(model);
                }
            } else if (model.cmd == "resetpassword") {

                if (await UserManager.FindByIdAsync(model.Id) != null) {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(model.Id);

                    model.ResetPassword = true;
                    model.Code = code;

                    string url = Url.Action("ResetPassword", "Account", new { userId = model.Id, code = code }, protocol: Request.Url.Scheme);


                    model.ResetLink = url;
                } else {
                    ModelState.AddModelError("", "User tidak ditemukan");
                }
            }

            model.RolesOpt = RoleManager.Roles.ToList();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code) {
            if (userId == null || code == null) {
                return View("Error");
            }

            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword() {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model) {
            if (ModelState.IsValid) {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !await UserManager.IsEmailConfirmedAsync(user.Id)) {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
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
        public ActionResult ForgotPasswordConfirmation() {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword(string code = null) {
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            var currentuser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            var user = await UserManager.FindByNameAsync(currentuser.Email);
            if (user == null) {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var hasher = new PasswordHasher();
            if (hasher.VerifyHashedPassword(user.PasswordHash, model.OldPassword) != PasswordVerificationResult.Failed) {

                if (model.Password == model.ConfirmPassword) {
                    
                    var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var result = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
                    if (result.Succeeded) {
                        return RedirectToAction("Index", "Home");
                    }

                    AddErrors(result);
                } else {

                    ModelState.AddModelError("", "Password Need to Match");
                }
            } else {

                ModelState.AddModelError("", "Invalid Old Password");
            }
            
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation() {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl) {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe) {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null) {
                return View("Error");
            }

            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose })
                .ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model) {
            if (!ModelState.IsValid) {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider)) {
                return View("Error");
            }

            return RedirectToAction("VerifyCode",
                new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl) {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, false);
            switch (result) {
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
                    return View("ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl) {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid) {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null) {
                    return View("ExternalLoginFailure");
                }

                var user = new DataAccess.Identity.ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded) {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded) {
                        await SignInManager.SignInAsync(user, false, false);
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
        public ActionResult LogOff() {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //FormsAuthentication.SignOut();
            Response.Cookies.Clear();
            Request.GetOwinContext().Authentication.SignOut();
            Session.Clear();

            HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", "");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);


            HttpCookie cookie2 = new HttpCookie("__RequestVerificationToken", "");
            cookie2.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie2);

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure() {
            return View();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (_userManager != null) {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null) {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null) { }

            public ChallengeResult(string provider, string redirectUri, string userId) {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context) {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null) {
                    properties.Dictionary[XsrfKey] = UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion

    }
}