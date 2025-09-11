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
using GHB_D1.Models;
using GHB_D1.Code.BAL;
using GHB_D1.Code.DAL;
using GHB_D1.Services;
using System.Data;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.Configuration;
using GHB_D1.Code.Util;
using System.Web.SessionState;

namespace GHB_D1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        DBAccess _objDBAcc = null;
        private AccountService _accService = new AccountService();
        AccountBAL _accountbal = null;
        char[] _spd = new char[] { '@' };
        char[] _spd1 = new char[] { ' ' };
        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        private UserSessionManager sessionManager = new UserSessionManager();
        private LDAPAuthenticationService _authService;

        public AccountController()
        {
            _accountbal = new AccountBAL();
            _logSys = new Loger();
            _authService = new LDAPAuthenticationService(WebConfigurationManager.AppSettings["DomainName"].ToString());
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _objDBAcc = new DBAccess();

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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoginViewModel loginVM = new LoginViewModel();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Session["Username"] as string))
                {
                    if (loginVM.Level == 1)
                        return RedirectToAction("Index", "Administrator");
                    else
                        return RedirectToAction("Index", "Home");
                }
            }
            return View(loginVM);
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        // chk local -> AD
        // POST: /Account/Login
        //v3(AMLO)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string cmdButton)//for user 20250114, string system
        {
            AudiLogModel audilog = new AudiLogModel();
            //audilog.WorkFunction = "LoginSystem";
            //ProcessLogBAL.AddProcessLog2(model.Username, "Login", audilog);
            int maxLength = 0;//ขนาด รหัสโค้ดสาขา
            char[] _mail = new char[] { '@' };
            ApiPostResponse2 ldapUser2 = new ApiPostResponse2();
            LoginViewModel loginVM = new LoginViewModel();
            //HttpRequestBase request = null;
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            string username = model.Username;
            if (model.Username.Contains("@"))
            {
                var spUser = model.Username.Split(_mail);
                model.Username = spUser[0];
            }

            if (AuthenticationUtil.IsAccountLocked2(username, model.Password))
            {
                ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                return View(model);
            }

            //LDAP Account
            UserPostRequest userPostRequest = new UserPostRequest();

            if (model.Username != "" && model.Password != "")
            {
                userPostRequest.username = model.Username;
                userPostRequest.password = model.Password;
                if (model.Username.Contains("bypass"))//Bypass to local first
                {
                    var str = model.Username.Length;
                    model.Username = model.Username.Substring(0, str - 6);
                    return (await Loging(model, returnUrl, cmdButton));//, system
                }
                //ldap
                //2 Verify AD by user/passpord
                ldapUser2 = _authService.GetUserByUserName4(userPostRequest);
                if (ldapUser2.sol_code != null && ldapUser2.emp_code != null)
                {
                    string sub_solCode = ldapUser2.sol_code == "" ? "00000" : cutSolCode(ldapUser2.sol_code, maxLength);
                    if (_accService.checkUserLdapinLocal_solCode3(model.Username, sub_solCode, ldapUser2.emp_code, ldapUser2))//check and update branch
                    {

                        _logSys.WriteProcessLogFile(_strPathFile, "call function: _accService.CheckUserLDAP ");
                        var loginVMLdap = _accService.CheckUserLDAP(model.Username);
                        _logSys.WriteProcessLogFile(_strPathFile, "loginVMLdap : " + loginVMLdap.Login_State);
                        if (loginVMLdap.Login_State)//successfull case
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "UpdateLoginLogout : " + ldapUser2.emp_code + " " + loginVMLdap.UserID);
                            bool _result = AdministratorBAL.UpdateLoginLogout(ldapUser2.emp_code, loginVMLdap.UserID, 1);
                            SetAllSession(loginVMLdap, audilog, loginVMLdap.Username.ToString(), ldapUser2.sol_name);
                            if (Session["AudiLog"] != null)
                            {
                                _logSys.WriteProcessLogFile(_strPathFile, "Session['AudiLog'] : " + Session["AudiLog"]);
                                audilog = (AudiLogModel)Session["AudiLog"];
                                audilog.WorkFunction = "Login Process";
                                audilog.Message = "Login เข้าใช้งาน GHB ATM/CDM Report";
                                audilog.EmpCode = ldapUser2.emp_code;
                                if (Session["latitude"] != null && Session["longitude"] != null)
                                {
                                    audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                                    audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                                }
                                ProcessLogBAL.AddProcessLog2(audilog);
                            }
                            //Session.Clear();
                            //Session["BranchId"] = loginVMLdap.BranchID.ToString();
                            //Session["BranchName"] = loginVMLdap.BranchName.ToString() != "" ? loginVMLdap.BranchName.ToString().Replace("สาขา", "") : "";
                            //Session["Username"] = loginVMLdap.Username.ToString();
                            //Session["UserId"] = loginVMLdap.UserID.ToString();
                            //Session["Fullname"] = loginVMLdap.FullName.ToString();
                            //Session["Level"] = loginVMLdap.Level.ToString();
                            //Session["LastLogon"] = Convert.ToDateTime(loginVMLdap.USER_LAST_LOGON).ToString("yyyy-MM-dd");
                            //Session["IsActive"] = loginVMLdap.Is_Active.ToString();
                            //Session["EmpCode"] = loginVM.Emp_Code.ToString();
                            //Session["RoleName"] = loginVM.Role_Name.ToString();
                            //if (Session["AudiLog"] != null)
                            //{
                            //    Session["AudiLog"] = null;
                            //}
                            //audilog.Username = loginVMLdap.Username.ToString();
                            //audilog.Message = "LoginSystem";
                            //audilog.WorkFunction = "LoginSystem";
                            //audilog.EmpCode = ldapUser2.sol_code;
                            //audilog.EmpName = loginVMLdap.Username.ToString();
                            //audilog.MAC_Address = ProcessLogBAL.GetServerMacAddress();
                            //audilog.OS = ProcessLogBAL.GetServerOS();
                            //audilog.URL = "";
                            //audilog.Device = ProcessLogBAL.GetServerMachineName();
                            //audilog.IP_Address = ProcessLogBAL.GetServerIpAddress();
                            //audilog.Latitude = "";
                            //audilog.Longitude = "";
                            //Session["AudiLog"] = audilog;
                            //ProcessLogBAL.AddProcessLog2(audilog);
                            _logSys.WriteProcessLogFile(_strPathFile, "ModelState.IsValid : " + ModelState.IsValid);
                            if (ModelState.IsValid)
                            {
                                _logSys.WriteProcessLogFile(_strPathFile, "ChkandGetSessionId : " + loginVMLdap.UserID);
                                var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVMLdap.UserID));
                                var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                                if (value != null && chkSession != null)
                                {
                                    if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                                    {
                                        ViewBag.ShowPopup = true;
                                        ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                        _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                        return View(model);
                                    }
                                    else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")//ต้องเตะก่อนหน้าออก
                                    {
                                        sessionManager.HandleUserSession(Convert.ToInt64(loginVMLdap.UserID), HttpContext.Session.SessionID);
                                        _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                                    }
                                    else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                                    {
                                        _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                        return View(model);
                                    }
                                }
                                else
                                {
                                    sessionManager.HandleUserSession(Convert.ToInt64(loginVMLdap.UserID), HttpContext.Session.SessionID);
                                    _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                                }
                                //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                            }
                            _logSys.WriteProcessLogFile(_strPathFile, "ResetLoginAttempts : " + username);
                            AuthenticationUtil.ResetLoginAttempts(username);

                            int[] levelAd = { 1, 4, 5 };
                            _logSys.WriteProcessLogFile(_strPathFile, "loginVMLdap.Level : " + loginVMLdap.Level);
                            if (levelAd.Contains(loginVMLdap.Level))
                            {
                                _logSys.WriteProcessLogFile(_strPathFile, "Goto : Index, Administrator1");
                                return RedirectToAction("Index", "Administrator");
                            }
                            else if (loginVMLdap.Level == 0 && loginVMLdap.Role_Name != "")
                            {
                                _logSys.WriteProcessLogFile(_strPathFile, "Goto : Index, Administrator2");
                                return RedirectToAction("Index", "Administrator"); //RedirectToAction("Index", "Home");
                            }
                            else if (loginVMLdap.Level == 0 && loginVMLdap.BranchID != "")
                            {
                                ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role โปรดติดต่อ Admin");
                                return View(model);
                            }
                            else if (loginVMLdap.Level == 0 && loginVMLdap.BranchID == "")
                            {
                                ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                                return View(model);
                            }
                            else if (loginVMLdap.BranchID == "")
                            {
                                ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                                return View(model);
                            }
                        }
                        else
                        {
                            //ยังไม่ได้ลงทะเบียน ldap acc
                            // Authentication failed
                            _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                            // Login failed, increment login attempts

                            int time1 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                            int loginAttempts1 = AuthenticationUtil.IncrementLoginAttempts(username);

                            if (loginAttempts1 > time1)
                            {
                                // Lock the account
                                AuthenticationUtil.IsLockAccount(model.Username, model.Password);
                                AuthenticationUtil.LockAccount(username);
                                ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                                return View(model);
                            }
                            else
                            {
                                ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");/*ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง*/
                                return View(model);
                            }
                        }
                    }
                    else
                    {
                        //Go to reg in Ldap
                        //ModelState.AddModelError("", "โปรดลงทะเบียนบัญชีLdap");
                        ModelState.AddModelError("", "คุณยังไม่ได้รับสิทธิ์เข้าดูรายงาน");
                        _logSys.WriteProcessLogFile(_strPathFile, "คุณยังไม่ได้รับสิทธิ์เข้าดูรายงาน");
                    }
                }
                else//local
                {
                    loginVM = _accService.CheckUser2(model.Username, model.Password);

                    _logSys.WriteProcessLogFile(_strPathFile, "call function: _accService.CheckUser ");
                    if (loginVM.Login_State)
                    {
                        bool _result = AdministratorBAL.UpdateLoginLogout(loginVM.Emp_Code, loginVM.UserID, 1);
                        SetAllSession(loginVM, audilog, model.Username.ToString(), loginVM.BranchName);
                        if (Session["AudiLog"] != null)
                        {
                            audilog = (AudiLogModel)Session["AudiLog"];
                            audilog.WorkFunction = "Login Process";
                            audilog.Message = "Login เข้าใช้งาน GHB ATM/CDM Report";
                            audilog.EmpCode = loginVM.Emp_Code;
                            if (Session["latitude"] != null && Session["longitude"] != null)
                            {
                                audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                                audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                            }
                            ProcessLogBAL.AddProcessLog2(audilog);
                        }
                        //if (ModelState.IsValid)
                        {
                            var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVM.UserID));
                            var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                            if (value != null && chkSession != null)
                            {
                                if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                                {
                                    ViewBag.ShowPopup = true;
                                    ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                    return View(model);
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")//ต้องเตะก่อนหน้าออก
                                {
                                    sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                                {
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                    return View(model);
                                }
                            }
                            else
                            {
                                sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                            }
                            //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                        }

                        AuthenticationUtil.ResetLoginAttempts(username);

                        int[] levelAd = { 1, 4, 5 };//admin
                        if (levelAd.Contains(loginVM.Level))
                        {
                            return RedirectToAction("Index", "Administrator");
                        }
                        else if (loginVM.Role_Name != "")/* && loginVM.BranchID != ""*/
                        {
                            return RedirectToAction("Index", "Administrator"); //RedirectToAction("Index", "Home");
                        }
                        else if (loginVM.Level == 0 && loginVM.BranchID != "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVM.Level == 0 && loginVM.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVM.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                    }
                    else
                    {
                        // Authentication failed
                        _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");
                        if (loginVM.Username == "" || loginVM.Username == null)
                        {
                            ModelState.AddModelError("", "ไม่พบบัญชีของคุณในระบบ");
                            return View(model);
                        }
                        // Login failed, increment login attempts
                        int time2 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                        int loginAttempts2 = AuthenticationUtil.IncrementLoginAttempts(username);

                        if (loginAttempts2 > time2)
                        {
                            // Lock the account
                            AuthenticationUtil.IsLockAccount(model.Username, model.Password);
                            AuthenticationUtil.LockAccount(username);
                            ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");/*ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง*/
                            return View(model);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "ใส่ชื่อผู้ใช้ หรือรหัส ไม่ครบ");
            }



            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Loging(LoginViewModel model, string returnUrl, string cmdButton)//for test 20250114, string system
        {
            //ProcessLogBAL.AddProcessLog(model.Username, "Login");
            int maxLength = 0;//ขนาด รหัสโค้ดสาขา
            ApiPostResponse2 ldapUser2 = new ApiPostResponse2();
            LoginViewModel loginVM = new LoginViewModel();
            AudiLogModel audilog = new AudiLogModel();
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            string username = model.Username;

            if (AuthenticationUtil.IsAccountLocked2(username, model.Password))
            {
                ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                return View(model);
            }

            if (!_accService.checkUserLdapinLocal2(model.Username))
            {
                if (model.Username != "" && model.Password != "")
                {
                    loginVM = _accService.CheckUser2(model.Username, model.Password);

                    _logSys.WriteProcessLogFile(_strPathFile, "call function: _accService.CheckUser2 ");
                    if (loginVM.Login_State)
                    {
                        bool _result = AdministratorBAL.UpdateLoginLogout(loginVM.Emp_Code, loginVM.UserID, 1);
                        SetAllSession(loginVM, audilog, model.Username.ToString(), loginVM.BranchName);

                        if (Session["AudiLog"] != null)
                        {
                            audilog = (AudiLogModel)Session["AudiLog"];
                            audilog.WorkFunction = "Login Process";
                            audilog.Message = "Login เข้าใช้งาน GHB ATM/CDM Report";
                            audilog.EmpCode = loginVM.Emp_Code;
                            if (Session["latitude"] != null && Session["longitude"] != null)
                            {
                                audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                                audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                            }
                            ProcessLogBAL.AddProcessLog2(audilog);
                        }
                        //if (ModelState.IsValid)
                        {
                            var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVM.UserID));
                            var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                            if (value != null && chkSession != null)
                            {
                                if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                                {
                                    ViewBag.ShowPopup = true;
                                    ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                    return View(model);
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")//ต้องเตะก่อนหน้าออก
                                {
                                    sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                                {
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                    return View(model);
                                }
                            }
                            else
                            {
                                sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                            }
                            //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                        }

                        AuthenticationUtil.ResetLoginAttempts(username);

                        int[] levelAd = { 1, 4, 5 };//admin
                        if (levelAd.Contains(loginVM.Level))
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "Login : Go to Index/Administrator");
                            return RedirectToAction("Index", "Administrator");
                        }
                        else if (loginVM.Role_Name != "")/*&& loginVM.BranchID != ""*/
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "Login : Go to Index/Administrator");
                            return RedirectToAction("Index", "Administrator"); //RedirectToAction("Index", "Home");
                        }
                        else if (loginVM.Level == 0 && loginVM.BranchID != "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVM.Level == 0 && loginVM.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVM.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                    }
                    else
                    {
                        // Authentication failed
                        _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");
                        if (loginVM.Username == "" || loginVM.Username == null)
                        {
                            ModelState.AddModelError("", "ไม่พบบัญชีของคุณในระบบ");
                            return View(model);
                        }
                        // Login failed, increment login attempts
                        int time2 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                        int loginAttempts2 = AuthenticationUtil.IncrementLoginAttempts(username);

                        if (loginAttempts2 > time2)
                        {
                            // Lock the account
                            AuthenticationUtil.IsLockAccount(model.Username, model.Password);
                            AuthenticationUtil.LockAccount(username);
                            ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                            return View(model);
                        }
                        else
                        {
                            //ModelState.AddModelError("", "ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง");
                            ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");
                            return View(model);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "ใส่ชื่อผู้ใช้ หรือรหัส ไม่ครบ");
                }
            }
            else
            {
                //LDAP Account
                UserPostRequest userPostRequest = new UserPostRequest();
                if (model.Username != "" && model.Password != "")
                {
                    userPostRequest.username = model.Username;
                    userPostRequest.password = model.Password;
                    //2 Verify AD by user/passpord
                    ldapUser2 = _authService.GetUserByUserName4(userPostRequest);
                    if (ldapUser2.sol_code != null && ldapUser2.emp_code != null)
                    {
                        string sub_solCode = ldapUser2.sol_code == "" ? "00000" : cutSolCode(ldapUser2.sol_code, maxLength);
                        if (_accService.checkUserLdapinLocal_solCode3(model.Username, sub_solCode, ldapUser2.emp_code, ldapUser2))//check and update branch
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "call function: _accService.CheckUserLDAP ");
                            var loginVMLdap = _accService.CheckUserLDAP(model.Username);
                            if (loginVMLdap.Login_State)//successfull case
                            {
                                bool _result = AdministratorBAL.UpdateLoginLogout(ldapUser2.emp_code, loginVMLdap.UserID, 1);
                                SetAllSession(loginVM, audilog, model.Username.ToString(), ldapUser2.sol_name);
                                if (Session["AudiLog"] != null)
                                {
                                    audilog = (AudiLogModel)Session["AudiLog"];
                                    audilog.WorkFunction = "Login Process";
                                    audilog.Message = "Login เข้าใช้งาน GHB ATM/CDM Report";
                                    audilog.EmpCode = ldapUser2.emp_code;
                                    if (Session["latitude"] != null && Session["longitude"] != null)
                                    {
                                        audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                                        audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                                    }
                                    ProcessLogBAL.AddProcessLog2(audilog);
                                }
                                if (ModelState.IsValid)
                                {
                                    var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVMLdap.UserID));
                                    var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                                    if (value != null && chkSession != null)
                                    {
                                        if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                                        {
                                            ViewBag.ShowPopup = true;
                                            ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                            _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                            return View(model);
                                        }
                                        else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")//ต้องเตะก่อนหน้าออก
                                        {
                                            sessionManager.HandleUserSession(Convert.ToInt64(loginVMLdap.UserID), HttpContext.Session.SessionID);
                                            _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                                        }
                                        else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                                        {
                                            _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                            return View(model);
                                        }
                                    }
                                    else
                                    {
                                        sessionManager.HandleUserSession(Convert.ToInt64(loginVMLdap.UserID), HttpContext.Session.SessionID);
                                        _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                                    }
                                    //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                                }

                                AuthenticationUtil.ResetLoginAttempts(username);

                                int[] levelAd = { 1, 4, 5 };
                                if (levelAd.Contains(loginVMLdap.Level))
                                {
                                    return RedirectToAction("Index", "Administrator");
                                }
                                else if (loginVMLdap.Role_Name != "")/* && loginVMLdap.BranchID != ""*/
                                {
                                    return RedirectToAction("Index", "Administrator"); //RedirectToAction("Index", "Home");
                                }
                                else if (loginVMLdap.Level == 0 && loginVMLdap.BranchID != "")
                                {
                                    ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role โปรดติดต่อ Admin");
                                    return View(model);
                                }
                                else if (loginVMLdap.Level == 0 && loginVMLdap.BranchID == "")
                                {
                                    ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                                    return View(model);
                                }
                                else if (loginVMLdap.BranchID == "")
                                {
                                    ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                                    return View(model);
                                }
                            }
                            else
                            {
                                //ยังไม่ได้ลงทะเบียน ldap acc
                                // Authentication failed
                                _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                                // Login failed, increment login attempts

                                int time1 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                                int loginAttempts1 = AuthenticationUtil.IncrementLoginAttempts(username);

                                if (loginAttempts1 > time1)
                                {
                                    // Lock the account
                                    AuthenticationUtil.IsLockAccount(model.Username, model.Password);
                                    AuthenticationUtil.LockAccount(username);
                                    ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                                    return View(model);
                                }
                                else
                                {
                                    //ModelState.AddModelError("", "ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง"); 
                                    ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");
                                    return View(model);
                                }
                            }
                        }
                        else
                        {
                            //Go to reg in Ldap
                            //ModelState.AddModelError("", "โปรดลงทะเบียนบัญชีLdap");
                            ModelState.AddModelError("", "คุณยังไม่ได้รับสิทธิ์เข้าดูรายงาน");
                            _logSys.WriteProcessLogFile(_strPathFile, "คุณยังไม่ได้รับสิทธิ์เข้าดูรายงาน");
                        }
                    }
                    else
                    {
                        // Authentication failed
                        _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                        // Login failed, increment login attempts

                        int time1 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                        int loginAttempts1 = AuthenticationUtil.IncrementLoginAttempts(username);

                        if (loginAttempts1 > time1)
                        {
                            // Lock the account
                            AuthenticationUtil.IsLockAccount(model.Username, model.Password);
                            AuthenticationUtil.LockAccount(username);
                            ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");//ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง
                            return View(model);
                        }
                        //ModelState.AddModelError("", "ไม่พบบัญชีผู้ใช้ " + userPostRequest.username + " ในระบบ LDAP !");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "ใส่ชื่อผู้ใช้ หรือรหัส ไม่ครบ");
                }
            }
            return View(model);
        }

        #region old login api
        //
        // POST: /Account/Login
        //v3(AMLO)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login_chkADfirst_20241209(LoginViewModel model, string returnUrl, string system, string cmdButton)
        {
            int maxLength = 3;//ขนาด รหัสโค้ดสาขา
            ApiPostResponse2 ldapUser2 = new ApiPostResponse2();
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            string username = model.Username;

            // Check if the account is locked
            //if (AuthenticationUtil.IsAccountLocked(username))
            if (AuthenticationUtil.IsAccountLocked2(username, model.Password))
            {
                ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                return View(model);
            }

            //User ldap
            //check user ldap then check user local            
            //if (_authService.IsAuthenticated(model.Username, model.Password))
            UserPostRequest userPostRequest = new UserPostRequest();
            userPostRequest.username = model.Username;
            userPostRequest.password = model.Password;
            //bool result = _authService.GetUserByUserName2(userPostRequest);            
            //_logSys.WriteProcessLogFile(_strPathFile, "return _authService.GetUserByUserName2 (Row:133):" + result);
            ldapUser2 = _authService.GetUserByUserName4(userPostRequest);
            //var chkLdap = ldapUser2 ?? null;
            if (ldapUser2.sol_code != null && ldapUser2.emp_code != null)
            {
                string sub_solCode = ldapUser2.sol_code == "" ? "001" : cutSolCode(ldapUser2.sol_code, maxLength);
                if (_accService.checkUserLdapinLocal_solCode(model.Username, sub_solCode))
                {
                    _logSys.WriteProcessLogFile(_strPathFile, "call function: _accService.CheckUserLDAP ");
                    var loginVMLdap = _accService.CheckUserLDAP(model.Username);
                    if (loginVMLdap.Login_State)//successfull case
                    {
                        Session.Clear();
                        Session["BranchId"] = loginVMLdap.BranchID.ToString();
                        Session["BranchName"] = loginVMLdap.BranchName.ToString();
                        Session["Username"] = loginVMLdap.Username.ToString();
                        Session["UserId"] = loginVMLdap.UserID.ToString();
                        Session["Fullname"] = loginVMLdap.FullName.ToString();
                        Session["Level"] = loginVMLdap.Level.ToString();
                        if (ModelState.IsValid)
                        {
                            var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVMLdap.UserID));
                            var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                            if (value != null && chkSession != null)
                            {
                                if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                                {
                                    ViewBag.ShowPopup = true;
                                    ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                    return View(model);
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")
                                {
                                    sessionManager.HandleUserSession(Convert.ToInt64(loginVMLdap.UserID), HttpContext.Session.SessionID);
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                                {
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                    return View(model);
                                }
                            }
                            else
                            {
                                sessionManager.HandleUserSession(Convert.ToInt64(loginVMLdap.UserID), HttpContext.Session.SessionID);
                                _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                            }
                            //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                        }

                        AuthenticationUtil.ResetLoginAttempts(username);

                        int[] levelAd = { 1, 4, 5 };
                        if (levelAd.Contains(loginVMLdap.Level))
                        {
                            return RedirectToAction("Index", "Administrator");
                        }
                        else if (loginVMLdap.Level != 0 && loginVMLdap.BranchID != "")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (loginVMLdap.Level == 0 && loginVMLdap.BranchID != "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVMLdap.Level == 0 && loginVMLdap.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVMLdap.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                    }
                    else
                    {
                        //ยังไม่ได้ลงทะเบียน ldap acc
                        // Authentication failed
                        _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                        // Login failed, increment login attempts

                        int time1 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                        int loginAttempts1 = AuthenticationUtil.IncrementLoginAttempts(username);

                        if (loginAttempts1 > time1)
                        {
                            // Lock the account
                            AuthenticationUtil.IsLockAccount(model.Username, model.Password);
                            AuthenticationUtil.LockAccount(username);
                            ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");//ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง
                            return View(model);
                        }
                    }
                }
                else
                {
                    //Go to reg in Ldap
                    ModelState.AddModelError("", "โปรดลงทะเบียนบัญชีLdap");
                }
            }
            else //User Local
            {
                ModelState.AddModelError("", "ไม่พบบัญชีผู้ใช้ " + userPostRequest.username + " ในระบบ LDAP !");
                //DataTable _dt = new DataTable();
                //List<GroupReportViewModel> _groupReportVMList = new List<GroupReportViewModel>();
                var loginVM = _accService.CheckUser(model.Username, model.Password);
                if (_accService.checkUserLdapinLocal(model.Username) || loginVM.Login_State)
                {
                    _logSys.WriteProcessLogFile(_strPathFile, "call function: _accService.CheckUser ");
                    if (loginVM.Login_State)
                    {
                        Session.Clear();
                        Session["BranchId"] = loginVM.BranchID.ToString();
                        Session["BranchName"] = loginVM.BranchName.ToString();
                        Session["Username"] = model.Username.ToString();
                        Session["UserId"] = loginVM.UserID.ToString();
                        Session["Fullname"] = loginVM.FullName.ToString();
                        Session["Level"] = loginVM.Level.ToString();
                        //if (ModelState.IsValid)
                        {
                            var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVM.UserID));
                            var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                            if (value != null && chkSession != null)
                            {
                                if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                                {
                                    ViewBag.ShowPopup = true;
                                    ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                    return View(model);
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")
                                {
                                    sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                                {
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                    return View(model);
                                }
                            }
                            else
                            {
                                sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                            }
                            //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                        }

                        AuthenticationUtil.ResetLoginAttempts(username);

                        int[] levelAd = { 1, 4, 5 };
                        if (levelAd.Contains(loginVM.Level))
                        {
                            return RedirectToAction("Index", "Administrator");
                        }
                        else if (loginVM.Level != 0 && loginVM.BranchID != "")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (loginVM.Level == 0 && loginVM.BranchID != "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVM.Level == 0 && loginVM.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVM.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                    }
                    else
                    {
                        // Authentication failed
                        _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                        // Login failed, increment login attempts

                        int time2 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                        int loginAttempts2 = AuthenticationUtil.IncrementLoginAttempts(username);

                        if (loginAttempts2 > time2)
                        {
                            // Lock the account
                            AuthenticationUtil.IsLockAccount(model.Username, model.Password);
                            AuthenticationUtil.LockAccount(username);
                            ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");/*ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง*/
                            return View(model);
                        }
                    }
                }
                else
                {
                    //ยังไม่ได้ลงทะเบียน Local acc
                    ModelState.AddModelError("", "ไม่พบผู้ใช้ในระบบ Local");
                }
            }
            return View(model);
        }

        //v2(old api)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login_v2_20240819(LoginViewModel model, string returnUrl, string system, string cmdButton)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            string username = model.Username;

            // Check if the account is locked
            if (AuthenticationUtil.IsAccountLocked(username))
            {
                ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                return View(model);
            }

            //User ldap
            //check user ldap then check user local            
            if (_authService.IsAuthenticated(model.Username, model.Password))
            {
                if (_accService.checkUserLdapinLocal(model.Username))
                {
                    var loginVMLdap = _accService.CheckUserLDAP(model.Username);
                    if (loginVMLdap.Login_State)//successfull case
                    {
                        Session.Clear();
                        Session["BranchId"] = loginVMLdap.BranchID.ToString();
                        Session["BranchName"] = loginVMLdap.BranchName.ToString();
                        Session["Username"] = loginVMLdap.Username.ToString();
                        Session["UserId"] = loginVMLdap.UserID.ToString();
                        Session["Fullname"] = loginVMLdap.FullName.ToString();
                        Session["Level"] = loginVMLdap.Level.ToString();
                        if (ModelState.IsValid)
                        {
                            var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVMLdap.UserID));
                            var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                            if (value != null && chkSession != null)
                            {
                                if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                                {
                                    ViewBag.ShowPopup = true;
                                    ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                    return View(model);
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")
                                {
                                    sessionManager.HandleUserSession(Convert.ToInt64(loginVMLdap.UserID), HttpContext.Session.SessionID);
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                                {
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                    return View(model);
                                }
                            }
                            else
                            {
                                sessionManager.HandleUserSession(Convert.ToInt64(loginVMLdap.UserID), HttpContext.Session.SessionID);
                                _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                            }
                            //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                        }

                        AuthenticationUtil.ResetLoginAttempts(username);

                        if (loginVMLdap.Level == 1)
                        {
                            return RedirectToAction("Index", "Administrator");
                        }
                        else if (loginVMLdap.BranchID != "")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (loginVMLdap.Level == 0 && loginVMLdap.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVMLdap.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                    }
                    else
                    {
                        //ยังไม่ได้ลงทะเบียน ldap acc
                        // Authentication failed
                        _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                        // Login failed, increment login attempts

                        int time1 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                        int loginAttempts1 = AuthenticationUtil.IncrementLoginAttempts(username);

                        if (loginAttempts1 > time1)
                        {
                            // Lock the account
                            AuthenticationUtil.LockAccount(username);
                            ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");/*ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง*/
                            return View(model);
                        }
                    }
                }
                else
                {
                    //Go to reg in Ldap
                    ModelState.AddModelError("", "โปรดลงทะเบียนบัญชีLdap");
                }
            }
            else //User Local
            {
                DataTable _dt = new DataTable();

                List<GroupReportViewModel> _groupReportVMList = new List<GroupReportViewModel>();
                if (_accService.checkUserLdapinLocal(model.Username))
                {
                    var loginVM = _accService.CheckUser(model.Username, model.Password);

                    if (loginVM.Login_State)
                    {
                        Session.Clear();
                        Session["BranchId"] = loginVM.BranchID.ToString();
                        Session["BranchName"] = loginVM.BranchName.ToString();
                        Session["Username"] = model.Username.ToString();
                        Session["UserId"] = loginVM.UserID.ToString();
                        Session["Fullname"] = loginVM.FullName.ToString();
                        Session["Level"] = loginVM.Level.ToString();
                        if (ModelState.IsValid)
                        {
                            var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVM.UserID));
                            var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                            if (value != null && chkSession != null)
                            {
                                if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                                {
                                    ViewBag.ShowPopup = true;
                                    ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                    return View(model);
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")
                                {
                                    sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                                }
                                else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                                {
                                    _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                    return View(model);
                                }
                            }
                            else
                            {
                                sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                            }
                            //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                        }

                        AuthenticationUtil.ResetLoginAttempts(username);

                        if (loginVM.Level == 1)
                        {
                            return RedirectToAction("Index", "Administrator");
                        }
                        else if (loginVM.BranchID != "")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (loginVM.Level == 0 && loginVM.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                        else if (loginVM.BranchID == "")
                        {
                            ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                            return View(model);
                        }
                    }
                    else
                    {
                        // Authentication failed
                        _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                        // Login failed, increment login attempts

                        int time2 = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                        int loginAttempts2 = AuthenticationUtil.IncrementLoginAttempts(username);

                        if (loginAttempts2 > time2)
                        {
                            // Lock the account
                            AuthenticationUtil.LockAccount(username);
                            ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");/*ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง*/
                            return View(model);
                        }
                    }
                }
                else
                {
                    //ยังไม่ได้ลงทะเบียน Local acc
                    ModelState.AddModelError("", "โปรดลงทะเบียนบัญชีทั่วไป");
                }
            }
            return View(model);
        }

        //v1
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login_v1_20240920(LoginViewModel model, string returnUrl, string system, string cmdButton)
        {
            if (system == "1")
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }


                string username = model.Username;

                // Check if the account is locked
                if (AuthenticationUtil.IsAccountLocked(username))
                {
                    ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                    return View(model);
                }

                DataTable _dt = new DataTable();

                List<GroupReportViewModel> _groupReportVMList = new List<GroupReportViewModel>();

                var loginVM = _accService.CheckUserLDAP(model.Username);

                if (loginVM.Login_State && _authService.IsAuthenticated(model.Username, model.Password))
                {
                    Session.Clear();
                    //  _groupReportVMList = _accService.AuthorizeUserReport(loginVM.BranchID,loginVM.UserID);
                    Session["BranchId"] = loginVM.BranchID.ToString();
                    Session["BranchName"] = loginVM.BranchName.ToString();
                    Session["Username"] = model.Username.ToString();
                    Session["UserId"] = loginVM.UserID.ToString();
                    Session["Fullname"] = loginVM.FullName.ToString();
                    Session["Level"] = loginVM.Level.ToString();

                    //if (Response.Cookies.Count > 0)
                    //{
                    //    foreach (string s in Response.Cookies.AllKeys)
                    //    {
                    //        if (s == FormsAuthentication.FormsCookieName || "asp.net_sessionid".Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    //        {
                    //            Response.Cookies[s].Secure = true;
                    //        }
                    //    }
                    //}
                    // TempData["groupReportVMList"] = _groupReportVMList;
                    //return RedirectToAction("Index", "Home", _groupReportVMList);

                    if (ModelState.IsValid)
                    {
                        var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVM.UserID));
                        var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                        if (value != null && chkSession != null)
                        {
                            if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                            {
                                ViewBag.ShowPopup = true;
                                ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                return View(model);
                            }
                            else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")
                            {
                                sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                            }
                            else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                            {
                                _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                return View(model);
                            }
                        }
                        else
                        {
                            sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                            _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                        }
                        //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                    }

                    AuthenticationUtil.ResetLoginAttempts(username);

                    if (loginVM.Level == 1)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (loginVM.BranchID != "")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (loginVM.Level == 0 && loginVM.BranchID == "")
                    {
                        ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                        return View(model);
                    }
                    else if (loginVM.BranchID == "")
                    {
                        ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                        return View(model);
                    }
                }
                else
                {

                    // Authentication failed
                    _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                    // Login failed, increment login attempts

                    int time = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                    int loginAttempts = AuthenticationUtil.IncrementLoginAttempts(username);

                    if (loginAttempts > time)
                    {
                        // Lock the account
                        AuthenticationUtil.LockAccount(username);
                        ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");/*ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง*/
                        return View(model);
                    }


                }
            }
            else if (system == "2")
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }


                string username = model.Username;

                // Check if the account is locked
                if (AuthenticationUtil.IsAccountLocked(username))
                {
                    ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                    return View(model);
                }

                DataTable _dt = new DataTable();

                List<GroupReportViewModel> _groupReportVMList = new List<GroupReportViewModel>();

                var loginVM = _accService.CheckUser(model.Username, model.Password);

                if (loginVM.Login_State)
                {
                    Session.Clear();
                    //  _groupReportVMList = _accService.AuthorizeUserReport(loginVM.BranchID,loginVM.UserID);
                    Session["BranchId"] = loginVM.BranchID.ToString();
                    Session["BranchName"] = loginVM.BranchName.ToString();
                    Session["Username"] = model.Username.ToString();
                    Session["UserId"] = loginVM.UserID.ToString();
                    Session["Fullname"] = loginVM.FullName.ToString();
                    Session["Level"] = loginVM.Level.ToString();

                    //if (Response.Cookies.Count > 0)
                    //{
                    //    foreach (string s in Response.Cookies.AllKeys)
                    //    {
                    //        if (s == FormsAuthentication.FormsCookieName || "asp.net_sessionid".Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    //        {
                    //            Response.Cookies[s].Secure = true;
                    //        }
                    //    }
                    //}
                    // TempData["groupReportVMList"] = _groupReportVMList;
                    //return RedirectToAction("Index", "Home", _groupReportVMList);

                    if (ModelState.IsValid)
                    {
                        var chkSession = sessionManager.ChkandGetSessionId(Convert.ToInt64(loginVM.UserID));
                        var value = Request.Cookies["ASP.NET_SessionId"] ?? null;
                        if (value != null && chkSession != null)
                        {
                            if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == null)
                            {
                                ViewBag.ShowPopup = true;
                                ModelState.AddModelError("", "มีผู้ใช้ชื่อนี้ในระบบอยู่ก่อนแล้ว ต้องการใช้ชื่อนี้หรือไม่");
                                _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate worning");
                                return View(model);
                            }
                            else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Yes")
                            {
                                sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                                _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and rotate");
                            }
                            else if ((Request.Cookies["ASP.NET_SessionId"].Value != chkSession.ToString()) && cmdButton == "Cancel")
                            {
                                _logSys.WriteProcessLogFile(_strPathFile, "Login Duplicate and do nothing");
                                return View(model);
                            }
                        }
                        else
                        {
                            sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);
                            _logSys.WriteProcessLogFile(_strPathFile, "First Time Login");
                        }
                        //sessionManager.HandleUserSession(Convert.ToInt64(loginVM.UserID), HttpContext.Session.SessionID);

                    }

                    AuthenticationUtil.ResetLoginAttempts(username);

                    if (loginVM.Level == 1)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (loginVM.BranchID != "")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (loginVM.Level == 0 && loginVM.BranchID == "")
                    {
                        ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Role/Branch โปรดติดต่อ Admin");
                        return View(model);
                    }
                    else if (loginVM.BranchID == "")
                    {
                        ModelState.AddModelError("", "บัญชียังไม่ได้กำหนด Branch โปรดติดต่อ Admin");
                        return View(model);
                    }
                }
                else
                {

                    // Authentication failed
                    _logSys.WriteProcessLogFile(_strPathFile, "Login failed!");

                    // Login failed, increment login attempts

                    int time = AdministratorBAL.GetUSER_ATTRIB(0, "Failed_Attemps");
                    int loginAttempts = AuthenticationUtil.IncrementLoginAttempts(username);

                    if (loginAttempts > time)
                    {
                        // Lock the account
                        AuthenticationUtil.LockAccount(username);
                        ModelState.AddModelError("", "บัญชีของคุณถูกล็อค กรุณาลองใหม่อีกครั้งในภายหลัง");
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("", "ท่านใส่ user หรือ password ไม่ถูกต้อง");//ความพยายามในการเข้าสู่ระบบไม่ถูกต้อง
                        return View(model);
                    }


                }
            }
            else if (system == "0")
            {
                ModelState.AddModelError("", "คุณยังไม่ได้เลือกระบบ");
            }

            return View(model);
        }

        #endregion old login api

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

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            if (Session["UserId"] != null && Session["UserId"].ToString() != "")
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า Register");
            if (ModelState.IsValid)
            {
                var arrayUserid = model.Email.Split(_spd);
                var user = new ApplicationUser { UserName = arrayUserid[0], Email = model.Email };
                var _insreg = _accountbal.insertRegisDetailBiz(model);
                if (_insreg)
                {
                    return RedirectToAction("MainUser", "Administrator");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model); //RedirectToAction("Register", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register2()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
            //return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register2(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var arrayUserid = model.Email.Split(_spd);
                var user = new ApplicationUser { UserName = arrayUserid[0], Email = model.Email };
                bool _insreg = _accountbal.insertRegisDetailBiz(model);
                if (_insreg)
                {
                    return RedirectToAction("MainUser", "Administrator");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model); //RedirectToAction("Register", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult AddUserLDAP2()
        {
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.USER_LEVEL = 2;
            return View("AddUserLDAP2");
            //return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddUserLDAP2(string cmdButton, string USER_LOGIN, string USER_PASSWORD, string USER_FIRSTNAME, string USER_LASTNAME, string USER_EMAIL, int USER_LEVEL)
        {
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.USER_LEVEL = USER_LEVEL;
            UserPostRequest userReq = new UserPostRequest();
            ApiPostResponse objApi = new ApiPostResponse();
            userReq.username = USER_LOGIN;
            userReq.password = USER_PASSWORD;
            switch (cmdButton)
            {
                case "VerifyUser":
                    UserModels obj = new UserModels();//not use in this fn

                    try
                    {
                        if (USER_LOGIN == "" || USER_PASSWORD == "")
                        {
                            ViewBag.USER_LOGIN = USER_LOGIN;
                            ModelState.AddModelError("", "โปรดระบุชื่อผู้ใช้ และ รหัสบัญชี Ldap");
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LOGIN&USER_PASSWORD : โปรดระบุชื่อผู้ใช้ และ รหัสบัญชี Ldap");
                        }
                        else
                        {

                            //obj = _authService.GetUserByUserName(userReq.username);
                            objApi = _authService.GetUserByUserName1(userReq);

                            _logSys.WriteProcessLogFile(_strPathFile, "_authService.GetUserByUserName1(userReq) : " + objApi.fullName);
                            var value = objApi ?? null;
                            if (value is null)
                            {
                                ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName(object) : null");

                            }
                            else if (objApi.fullName == "")
                            {
                                ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName(ไม่พบชื่อนี้ในระบบ ldap) : Empty");
                            }
                            else
                            {
                                var userDetails = objApi.fullName.ToString().Split(_spd1);
                                var userLogin = objApi.email.ToString().Split(_spd);
                                ViewBag.USER_FIRSTNAME = userDetails[0];
                                ViewBag.USER_LASTNAME = userDetails[1];
                                ViewBag.USER_EMAIL = objApi.email;
                                ViewBag.USER_LEVEL = USER_LEVEL;
                                ViewBag.USER_LOGIN = userLogin[0];
                                ViewBag.Flag = true;
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_FIRSTNAME : " + userDetails[0]);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LASTNAME : " + userDetails[1]);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_EMAIL : " + objApi.email);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LOGIN : " + userLogin[0]);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LEVEL : " + USER_LEVEL);
                            }
                        }

                        return View("AddUserLDAP2");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                        ex.Message.ToString();
                        _logSys.WriteProcessLogFile(_strPathFile, "Error VerifyUser : " + ex.Message.ToString());
                    }
                    break;
                case "Save":
                    UserModels obj2 = new UserModels();
                    obj2.USER_LOGIN = USER_LOGIN;
                    obj2.USER_FIRSTNAME = USER_FIRSTNAME;
                    obj2.USER_LASTNAME = USER_LASTNAME;
                    obj2.USER_EMAIL = USER_EMAIL;
                    obj2.USER_LEVEL = USER_LEVEL;
                    obj2.USER_FLAG = true;
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LOGIN : " + obj2.USER_LOGIN);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_FIRSTNAME : " + obj2.USER_FIRSTNAME);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LASTNAME : " + obj2.USER_LASTNAME);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMAIL : " + obj2.USER_EMAIL);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LEVEL : " + obj2.USER_LEVEL);
                    bool resultSave = AdministratorBAL.AddUserLDAP(obj2);
                    if (resultSave == false)
                    {
                        ViewBag.USER_LOGIN = USER_LOGIN;
                        ModelState.AddModelError("", "ชื่อผู้ใช้ได้มีการเพิ่มแล้ว");
                        _logSys.WriteProcessLogFile(_strPathFile, "resultSave(ชื่อผู้ใช้ได้มีการเพิ่มแล้ว) : false");
                        return View("AddUserLDAP2");
                    }
                    else
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "resultSave(เพิ่มผู้ใช้ได้) :  true");
                        return RedirectToAction("MainUser");
                    }
                case "Cancel":
                    return RedirectToAction("MainUser");

            }

            return View("AddUserLDAP2");

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddUserLDAP3(string cmdButton, string USER_LOGIN, string USER_PASSWORD, string USER_FIRSTNAME, string USER_LASTNAME, string USER_EMAIL, int USER_LEVEL, string USER_EMP_CODE, string SOL_CODE)
        {
            _logSys.WriteProcessLogFile(_strPathFile, "Begin AddUserLDAP3");
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.USER_LEVEL = USER_LEVEL;
            UserPostRequest userReq = new UserPostRequest();
            ApiPostResponse2 objApi = new ApiPostResponse2();
            userReq.username = USER_LOGIN;
            userReq.password = USER_PASSWORD;
            int maxLength = 2;
            _logSys.WriteProcessLogFile(_strPathFile, "cmdButton(Row:1082): " + cmdButton);
            switch (cmdButton)
            {
                case "VerifyUser":
                    UserModels obj = new UserModels();//not use in this fn

                    try
                    {
                        if (USER_LOGIN == "" || USER_PASSWORD == "")
                        {
                            ViewBag.USER_LOGIN = USER_LOGIN;
                            ModelState.AddModelError("", "โปรดระบุชื่อผู้ใช้ และ รหัสบัญชี Ldap");
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LOGIN&USER_PASSWORD : โปรดระบุชื่อผู้ใช้ และ รหัสบัญชี Ldap");
                        }
                        else
                        {
                            objApi = _authService.GetUserByUserName3(userReq);
                            _logSys.WriteProcessLogFile(_strPathFile, "_authService.GetUserByUserName3(userReq) : " + objApi.fname + " " + objApi.lname);
                            var value = objApi ?? null;
                            if (value is null)
                            {
                                ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName3(object) : null");

                            }
                            else if (objApi.fname == "" && objApi.lname == "")
                            {
                                ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName3(ไม่พบชื่อนี้ในระบบ ldap) : Empty");
                            }
                            else
                            {

                                var userLogin = objApi.email.ToString().Split(_spd);
                                ViewBag.SOL_CODE = objApi.sol_code;
                                ViewBag.USER_EMP_CODE = objApi.emp_code;
                                ViewBag.USER_FIRSTNAME = objApi.fname;
                                ViewBag.USER_LASTNAME = objApi.lname;
                                ViewBag.USER_EMAIL = objApi.email;
                                ViewBag.USER_LEVEL = USER_LEVEL;
                                ViewBag.USER_LOGIN = userLogin[0];
                                ViewBag.Flag = true;
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_FIRSTNAME : " + objApi.fname);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LASTNAME : " + objApi.lname);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_EMAIL : " + objApi.email);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LOGIN : " + userLogin[0]);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LEVEL : " + USER_LEVEL);
                            }
                        }

                        return View("AddUserLDAP2");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                        ex.Message.ToString();
                        _logSys.WriteProcessLogFile(_strPathFile, "Error VerifyUser : " + ex.Message.ToString());
                    }
                    break;
                case "Save":
                    UserModels obj2 = new UserModels();
                    obj2.USER_LOGIN = USER_LOGIN;
                    obj2.USER_FIRSTNAME = USER_FIRSTNAME;
                    obj2.USER_LASTNAME = USER_LASTNAME;
                    obj2.USER_EMAIL = USER_EMAIL;
                    obj2.USER_LEVEL = 0;//USER_LEVEL 0 Not role, 1 Admin, 2 Role Branch, 3 Role Head office 4 Admin1, 5 Amin2;
                    obj2.USER_FLAG = true;
                    obj2.USER_EMP_CODE = USER_EMP_CODE;
                    obj2.SOL_CODE = SOL_CODE == "" ? "000" : cutSolCode(SOL_CODE, maxLength);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LOGIN : " + obj2.USER_LOGIN);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_FIRSTNAME : " + obj2.USER_FIRSTNAME);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LASTNAME : " + obj2.USER_LASTNAME);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMAIL : " + obj2.USER_EMAIL);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LEVEL : " + obj2.USER_LEVEL);
                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMP_CODE : " + obj2.USER_EMP_CODE);
                    bool resultSave = AdministratorBAL.AddUserLDAP(obj2);
                    if (resultSave == false)
                    {
                        ViewBag.USER_LOGIN = USER_LOGIN;
                        ModelState.AddModelError("", "ชื่อผู้ใช้ได้มีการเพิ่มแล้ว");
                        _logSys.WriteProcessLogFile(_strPathFile, "resultSave(ชื่อผู้ใช้ได้มีการเพิ่มแล้ว) : false");
                        return View("AddUserLDAP2");
                    }
                    else
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "resultSave(เพิ่มผู้ใช้ได้) :  true");
                        return RedirectToAction("MainUser");
                    }
                case "Cancel":
                    return RedirectToAction("MainUser");

            }

            return View("AddUserLDAP2");

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
                var _isforgot = _accountbal.forgotPwdBiz(model);
                if (_isforgot)
                {
                    return RedirectToAction("Login", "Account");
                }
                //var user = await UserManager.FindByNameAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return View("ForgotPasswordConfirmation");
                //}

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
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
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
        [AllowAnonymous]
        public ActionResult Logout()
        {
            //ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Logout");
            try
            {
                _logSys.WriteProcessLogFile(_strPathFile, "Begin Logout ");
                long oldSessionId = Convert.ToInt64(Session["UserId"]);
                string emp_code = Session["EmpCode"] == null ? "" : Session["EmpCode"].ToString();
                string userid = Session["UserId"] == null ? "" : Session["UserId"].ToString();
                string sessionId = HttpContext.Session.SessionID;
                sessionManager.SignOutUser(oldSessionId, sessionId);

                FormsAuthentication.SignOut();
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {

                    if (Session["AudiLog"] != null)
                    {
                        AudiLogModel audilog = (AudiLogModel)Session["AudiLog"];
                        audilog.WorkFunction = "Logout Process";
                        audilog.Message = "Logout ออกจากการใช้งาน GHB ATM/CDM Report";
                        audilog.EmpCode = emp_code;
                        if (Session["latitude"] != null && Session["longitude"] != null)
                        {
                            audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                            audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                        }
                        ProcessLogBAL.AddProcessLog2(audilog);
                    }
                    bool _result = AdministratorBAL.UpdateLoginLogout(emp_code, userid, 2);
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddYears(-1);//AddDays(-1);
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty));
                    _logSys.WriteProcessLogFile(_strPathFile, "ASP.NET_SessionId expires : " + Response.Cookies["ASP.NET_SessionId"].Expires);
                    _logSys.WriteProcessLogFile(_strPathFile, "ASP.NET_SessionId Value : " + Response.Cookies["ASP.NET_SessionId"].Value);
                    _logSys.WriteProcessLogFile(_strPathFile, "Go to Login page ");
                }
                //if (Request.Cookies["ASP.NET_SessionId"] != null)
                //{
                //    var cookie = new HttpCookie("ASP.NET_SessionId", "")
                //    {
                //        Expires = DateTime.Now.AddYears(-1),
                //        HttpOnly = true
                //    };
                //    Response.Cookies.Add(cookie);
                //}
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                _logSys.WriteProcessLogFile(_strPathFile, "Logout Error : " + ex.Message.ToString());
            }
            return RedirectToAction("Login");
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

        private string LoadMasterRole()
        {
            string _strData = string.Empty;
            List<RoleModels> _masterDataList = new List<RoleModels>();
            try
            {
                _masterDataList = AdministratorBAL.GetMasterRole();
                if (_masterDataList.Count > 0)
                {
                    foreach (RoleModels data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.ROLE_LEVEL + "," + data.ROLE_DEPARTMENT;
                        else
                            _strData += "|" + data.ROLE_LEVEL + "," + data.ROLE_DEPARTMENT;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }

        public string cutSolCode(string solcode, int maxLength)
        {
            string r = string.Empty;
            var l = solcode.Length;
            switch (l)
            {
                case (5):
                    r = solcode.Length > maxLength ? solcode.Substring(0, maxLength) : solcode;
                    break;
                case (4):
                    solcode = "0" + solcode;
                    r = solcode.Length > maxLength ? solcode.Substring(0, maxLength) : solcode;
                    break;
                case (3):
                    solcode = "00" + solcode;
                    r = solcode.Length > maxLength ? solcode.Substring(0, maxLength) : solcode;
                    break;
            }
            //r = solcode.Length > maxLength ? solcode.Substring(0, maxLength) : solcode;

            return r;
        }

        public void SetAllSession(LoginViewModel loginVM, AudiLogModel audilog, string modelUsername, string ldapSolcode)
        {
            Session.Clear();
            Session["BranchId"] = loginVM.BranchID.ToString();
            Session["BranchName"] = ldapSolcode != "" ? ldapSolcode : "";//loginVM.BranchName.ToString() != "" ? loginVM.BranchName.ToString().Replace("สาขา", "") : "";
            Session["Username"] = modelUsername;
            Session["UserId"] = loginVM.UserID.ToString();
            Session["Fullname"] = loginVM.FullName.ToString();
            Session["Level"] = loginVM.Level.ToString();
            Session["LastLogon"] = Convert.ToDateTime(loginVM.USER_LAST_LOGON != "" ? loginVM.USER_LAST_LOGON : null).ToString("yyyy-MM-dd");
            Session["IsActive"] = loginVM.Is_Active.ToString();
            Session["EmpCode"] = loginVM.Emp_Code.ToString();
            Session["RoleName"] = loginVM.Role_Name.ToString();
            audilog.Username = modelUsername;
            audilog.Message = "LoginSystem";
            audilog.WorkFunction = "LoginSystem";
            audilog.EmpCode = ldapSolcode;
            audilog.EmpName = loginVM.Username.ToString();
            audilog.MAC_Address = ProcessLogBAL.FormatMacAddress(ProcessLogBAL.GetServerMacAddress());
            audilog.OS = ProcessLogBAL.GetServerOS();
            audilog.URL = "";
            audilog.Device = ProcessLogBAL.GetServerMachineName();
            audilog.IP_Address = ProcessLogBAL.GetServerIpAddress();
            audilog.Latitude = "";
            audilog.Longitude = "";
            Session["AudiLog"] = audilog;
            if (loginVM.roleList != null)
            {
                Session["R0"] = loginVM.roleList.group_role.Where(x => x.Group_Report.Contains("R0") == true).ToList();
                Session["GroupReport"] = (from std in loginVM.roleList.group_role
                                          select std.Group_Report).Distinct().ToList();
                Session["Report"] = loginVM.roleList.group_role.Where(x => x.Group_Report.Contains("R0") == false).ToList();
                Session["R01"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "R01").ToList();//Role Management
                Session["R02"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "R02").ToList();//User Management
                Session["R03"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "R03").ToList();//User Transfer
                Session["R04"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "R04").ToList();//Audi Log
                Session["R05"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "R05").ToList();//Configuration
                Session["ALL"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "ALL").ToList();
                Session["IND"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "IND").ToList();
                Session["ADM"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "ADM").ToList();
                Session["CDM"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "CDM").ToList();
                Session["CRM"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "CRM").ToList();
                Session["LOC"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "LOC").ToList();
                Session["LRM"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "LRM").ToList();
                Session["QRP"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "QRP").ToList();
                Session["RAT"] = loginVM.roleList.group_role.Where(x => x.Group_Report == "RAT").ToList();
            }
        }

        #endregion
    }
}