using GHB_D1.Code.BAL;
using GHB_D1.Code.DAL;
using GHB_D1.Code.Util;
using GHB_D1.Models;
using GHB_D1.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Globalization;
using System.Data;
using System.Reflection;
using GHB_D1.ReportFiles.DataSet;
using System.Text.RegularExpressions;

namespace GHB_D1.Controllers
{
    public class AdministratorController : BaseController
    {
        iniConnection _iniCon = null;
        private byte[] iv = null;
        DBAccess dbAccess = null;
        dsGenReport _ds = null;
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        private ASCIIEncoding textConverter = new ASCIIEncoding();
        private LDAPAuthenticationService _authService;
        private AccountService _accService = new AccountService();
        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        char[] _spd = new char[] { ' ' };
        char[] _spd2 = new char[] { '@' };
        public AdministratorController()
        {
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            _iniCon.cryptoKey = ModConf.ReadIni(_iniCon.iniFile, "appSetting", "cryptoKey");
            _iniCon.ivk = ModConf.ReadIni(_iniCon.iniFile, "appSetting", "ivk");
            iv = textConverter.GetBytes(_iniCon.ivk);
            dbAccess = new DBAccess();
            _authService = new LDAPAuthenticationService(WebConfigurationManager.AppSettings["DomainName"].ToString());
            _logSys = new Loger();
            _ds = new dsGenReport();
        }

        // GET: Administrator
        [HttpGet]
        public ActionResult Index()
        {
            /*
            ViewBag.UserAttribCount = AdministratorBAL.GetCount("SELECT count(*) as CountRow FROM TBL_USER_ATTRIB");
            ViewBag.RoleCount = AdministratorBAL.GetCount("SELECT count(*) as CountRow FROM TBL_ROLES");
            ViewBag.UserCount = AdministratorBAL.GetCount("SELECT count(*) as CountRow FROM TBL_USERS");
            ViewBag.GroupReportCount = AdministratorBAL.GetCount("SELECT count(*) as CountRow FROM TBL_GROUP_REPORT");
            ViewBag.GroupDetailReportCount = AdministratorBAL.GetCount("SELECT count(*) as CountRow FROM TBL_GROUP_DETAIL_REPORT");
            ViewBag.BranchCount = AdministratorBAL.GetCount("SELECT count(*) as CountRow FROM Table_Branch");
            */
            //GetCount(string strName, string typefn)
            _logSys.WriteProcessLogFile(_strPathFile, "Begin : Index/Administrator");
            ViewBag.UserAttribCount = AdministratorBAL.GetCount("AMS_SP_MAIN_APP", "A12");
            ViewBag.RoleCount = AdministratorBAL.GetCount("AMS_SP_MAIN_APP", "A13");
            ViewBag.UserCount = AdministratorBAL.GetCount("AMS_SP_MAIN_APP", "A14");
            ViewBag.GroupReportCount = AdministratorBAL.GetCount("AMS_SP_MAIN_APP", "A15");
            ViewBag.GroupDetailReportCount = AdministratorBAL.GetCount("AMS_SP_MAIN_APP", "A16");
            ViewBag.BranchCount = AdministratorBAL.GetCount("AMS_SP_MAIN_APP", "A17");
            ViewBag.MapBranchReportCount = AdministratorBAL.GetCount("AMS_SP_MAIN_APP", "A20");
            if (null != Session["Level"])
            {
                ViewBag.Level = Session["Level"].ToString();
                _logSys.WriteProcessLogFile(_strPathFile, "Begin : Index/Administrator - Session['Level'']");
            }
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
                _logSys.WriteProcessLogFile(_strPathFile, "Begin : Index/Administrator - Session['GroupReport'']");
            }
            if (Session["AudiLog"] != null)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "Begin : Index/Administrator - Session['AudiLog'']");
                AudiLogModel audilog = (AudiLogModel)Session["AudiLog"];
                audilog.WorkFunction = "ไปที่หน้าหลัก UserManagement";
                audilog.Message = "ไปที่หน้า UserManagement";
                if (Session["latitude"] != null && Session["longitude"] != null)
                {
                    audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                    audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                }
                ProcessLogBAL.AddProcessLog2(audilog);
            }
            //ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า UserManagement");
            _logSys.WriteProcessLogFile(_strPathFile, "Begin : Index/Administrator - GetUser()");
            List<UserModels> list = AdministratorBAL.GetUser();
            //HomeController abc = new HomeController();
            //LocationModel model = new LocationModel();

            //GroupUserModel groupUser = new GroupUserModel();
            //groupUser.lst_m1 = list;
            //groupUser.lst_role = (List<RoleModels>)Session["R01"];
            //groupUser.groupRole = new groupRole();
            //groupUser.groupRole.group_role = (List<RoleModels>)Session["R0"];
            _logSys.WriteProcessLogFile(_strPathFile, "Begin : Index/Administrator - Default page list count = " + list.Count);
            return View("Default", list);

        }
        [HttpGet]
        public ActionResult SearchGroupReport(string groupReportName)
        {

            var data = from m in AdministratorBAL.GetGroupReport()
                       select m;
            if (!string.IsNullOrEmpty(groupReportName))
            {
                data = data.Where(s => s.GroupName.Contains(groupReportName));
            }
            return View("GroupReport", data.ToList());

        }
        public ActionResult GroupReport()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า GroupReport");
            List<GroupReportModel> list = AdministratorBAL.GetGroupReport();
            return View("GroupReport", list);

        }
        [HttpPost]
        public ActionResult GroupReport(string cmdButton, List<GroupReportModel> model)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            int intID = 0;
            if (arrStr.Length > 1)
                intID = Convert.ToInt32(arrStr[1]);
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddGroupReport");
                    return RedirectToAction("AddGroupReport");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditGroupReport");
                    return RedirectToAction("EditGroupReport", new { ID = intID });
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล GroupReport");
                    var items = model.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteGroupReportByID(i.ID);
                    }
                    return RedirectToAction("GroupReport");
            }
            return null;
        }
        public ActionResult AddGroupReport()
        {
            ViewBag.GenGroupNo = AdministratorBAL.GenGroupReportNo();
            return View("AddGroupReport");

        }
        [HttpPost]
        public ActionResult AddGroupReport(string cmdButton, decimal? GroupCode, decimal? GroupNo, string GroupName, bool? Active)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddGroupReport");
                    GroupReportModel obj = new GroupReportModel();
                    obj.GroupCode = GroupNo;
                    obj.GroupName = GroupName;
                    obj.Active = Active;
                    bool resultSave = AdministratorBAL.AddGroupReport(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddGroupReport");
                    return RedirectToAction("GroupReport");
            }
            return RedirectToAction("GroupReport");
        }
        public ActionResult EditGroupReport(int ID)
        {

            GroupReportModel obj = AdministratorBAL.GetGroupReportByID(ID);
            ViewBag.ID = obj.ID;
            ViewBag.GroupCode = obj.GroupCode;
            ViewBag.GroupName = obj.GroupName;
            ViewBag.Active = obj.Active;

            return View("EditGroupReport");

        }
        [HttpPost]
        public ActionResult EditGroupReport(string cmdButton, int ID, decimal GroupCode, string GroupName, bool Active)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditGroupReport");
                    GroupReportModel obj = new GroupReportModel();
                    obj.ID = ID;
                    obj.GroupCode = GroupCode;
                    obj.GroupName = GroupName;
                    obj.Active = Active;
                    bool result = AdministratorBAL.EditGroupReport(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditGroupReport");
                    return RedirectToAction("GroupReport");

            }
            return RedirectToAction("GroupReport");
        }
        [HttpPost]
        public ActionResult AddUserAttrib(string cmdButton, UserAttribModel obj)
        {

            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddUserAttrib");
                    bool resultSave = AdministratorBAL.AddUserAttrib(obj);

                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddUserAttrib");
                    return RedirectToAction("MainUserAttrib");
            }
            return RedirectToAction("MainUserAttrib");
        }
        public ActionResult AddUserAttrib()
        {
            UserAttribModel obj = new UserAttribModel();
            return View("AddUserAttrib", obj);

        }
        [HttpGet]
        public ActionResult SearchUserAttrib(string SCFGVARIABLE)
        {

            var data = from m in AdministratorBAL.GetUserAttrib()
                       select m;

            if (!string.IsNullOrEmpty(SCFGVARIABLE))
            {
                data = data.Where(s => s.SCFGVARIABLE.Contains(SCFGVARIABLE));
            }



            return View("MainUserAttrib", data.ToList());

        }
        public ActionResult MainUserAttrib()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า MainUserAttrib");
            List<UserAttribModel> list = AdministratorBAL.GetUserAttrib();

            return View("MainUserAttrib", list);

        }
        [HttpPost]
        public ActionResult UserAttrib(string cmdButton, List<UserAttribModel> model)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            short shtID = 0;
            string strVariable = string.Empty;
            if (arrStr.Length > 1)
            {
                shtID = Convert.ToInt16(arrStr[1]);
                strVariable = Convert.ToString(arrStr[2]);
            }
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddUserAttrib");
                    return RedirectToAction("AddUserAttrib");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditUserAttrib");
                    return RedirectToAction("GetUserAttribByIDAndVariable", new { ID = shtID, Variable = strVariable });
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล UserAttrib");
                    var items = model.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteUserAttribByIDAndVariable(i.NCFGVARIABLEID, i.SCFGVARIABLE);
                    }
                    return RedirectToAction("MainUserAttrib");

            }

            return null;
        }
        public ActionResult GetUserAttribByIDAndVariable(short ID, string Variable)
        {

            UserAttribModel obj = AdministratorBAL.GetUserAttribByIDAndVariable(ID, Variable);

            return View("EditUserAttrib", obj);

        }
        [HttpPost]
        public ActionResult EditUserAttrib(string cmdButton, UserAttribModel obj)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditUserAttrib");
                    bool result = AdministratorBAL.EditUserAttrib(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditUserAttrib");
                    return RedirectToAction("MainUserAttrib");

            }
            return RedirectToAction("MainUserAttrib");
        }
        [HttpGet]
        public ActionResult SearchRole(string roleName)
        {

            var data = from m in AdministratorBAL.GetRole()
                       select m;

            if (!string.IsNullOrEmpty(roleName))
            {
                data = data.Where(s => s.ROLE_DEPARTMENT.Contains(roleName));
            }



            return View("MainRole", data.ToList());

        }
        public ActionResult MainRole()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า MainRole");

            List<RoleModels> list = AdministratorBAL.GetRole();

            return View("MainRole", list);

        }
        [HttpPost]
        public ActionResult Role(string cmdButton, List<RoleModels> model)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            long lngID = 0;
            if (arrStr.Length > 1)
                lngID = Convert.ToInt64(arrStr[1]);
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddRole");
                    return RedirectToAction("AddRole");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditRole");
                    return RedirectToAction("GetRoleByID", new { ID = lngID });
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล Role");
                    var items = model.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteRoleByID(i.ROLE_LEVEL);
                    }
                    return RedirectToAction("MainRole");

            }

            return null;
        }
        public ActionResult AddRole()
        {
            RoleModels obj = new RoleModels();
            return View("AddRole", obj);

        }
        [HttpPost]
        public ActionResult AddRole(string cmdButton, RoleModels obj)
        {

            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddRole");
                    bool resultSave = AdministratorBAL.AddRole(obj);

                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddRole");
                    return RedirectToAction("MainRole");
            }
            return RedirectToAction("MainRole");
        }
        public ActionResult GetRoleByID(long ID)
        {

            RoleModels obj = AdministratorBAL.GetRoleByID(ID);

            return View("EditRole", obj);

        }
        [HttpPost]
        public ActionResult EditRole(string cmdButton, RoleModels obj)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditRole");
                    bool result = AdministratorBAL.EditRole(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditRole");
                    return RedirectToAction("MainRole");

            }
            return RedirectToAction("MainRole");
        }


        ////////////////////////////////////////////////////////////////////////*****
        #region RoleManagement

        //User
        #region User
        [HttpGet]
        public ActionResult NdUserManagement()
        {
            ViewBag.SystemBranch = LoadSystemBranch();
            ViewBag.SystemHub = LoadSystemHub();
            ViewBag.SystemDistrict = LoadSystemDistrict();
            ViewBag.SystemDepartment = LoadSystemDepartment();
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.CurrentStatusDDL = "All";
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า UserManagement");
            List<UserModels> list = AdministratorBAL.GetUser();
            List<RoleModels> R02 = new List<RoleModels>();
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["R02"] != null)
            {
                R02 = (List<RoleModels>)Session["R02"];
                ViewBag.List_Role = R02[0].List_Role;
                ViewBag.View_Role = R02[0].View_Role;
                ViewBag.Update_Role = R02[0].Create_Role;
                ViewBag.Delete_Role = R02[0].Delete_Role;
                ViewBag.Create_Role = R02[0].Create_Role;
                ViewBag.Export_Role = R02[0].Export_Role;
            }
            var rolename = Session["RoleName"].ToString();
            if (rolename != "superadmin")
            {
                list = list.Where(x => x.User_Role_Level != "superadmin").ToList();
            }
            return View("NdUserManagement", list);

        }

        [HttpPost]
        public ActionResult NdUserManage(string cmdButton, List<UserModels> model, string usernameInput, string empcodeInput, string firstInput, string lastInput, string departInput, string disInput, string branchInput, string statusInput)
        {
            string[] arrStr = null;
            string strValue = string.Empty;
            long lngID = 0;
            if (cmdButton != "" && cmdButton.Contains('|'))
            {
                arrStr = cmdButton.Split('|');
                strValue = Convert.ToString(arrStr[0]);
                lngID = Convert.ToInt32(arrStr[1]);
            }
            else
            {
                strValue = cmdButton;
            }
            List<UserModels> list = new List<UserModels>();
            //***for audi log
            UserModels param = new UserModels();
            param.USER_LOGIN = usernameInput;
            param.USER_EMP_CODE = empcodeInput;
            param.USER_FIRSTNAME = firstInput;
            param.USER_LASTNAME = lastInput;
            param.Dept_Code = departInput == "All" ? "" : departInput;
            param.Hub_Code = disInput == "All" ? "" : disInput;
            param.SOL_CODE = branchInput == "All" ? "" : branchInput;
            param.IS_ACTIVE2 = statusInput;// == "1" ? true : false;

            ViewBag.SystemBranch = LoadSystemBranch();
            ViewBag.SystemHub = LoadSystemHub();
            ViewBag.SystemDistrict = LoadSystemDistrict();
            ViewBag.SystemDepartment = LoadSystemDepartment();
            ViewBag.StatusDDL = LoadStatusActive();
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            switch (strValue)
            {
                case "ExportFile":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Export File Excel");
                    return RedirectToAction("ExportExcelUser");
                case "ExportCSV":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Export CSV");
                    return RedirectToAction("ExportCSVUser");
                case "ImportFile":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Import File");
                    return RedirectToAction("ImportFile");
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า Register");
                    return RedirectToAction("Register", "Account");
                case "AddUserLDAP":
                    return RedirectToAction("NdCreateUser", new { USER_LEVEL = 0 });
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า NdEditUser");
                    return RedirectToAction("NdEditUser", new { ID = lngID });
                case "Search":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า Search");
                    list = AdministratorBAL.GetUserByParam(model, param);
                    ViewBag.CurrentStatusDDL = statusInput;
                    //if (list.Count > 0)
                    //{
                    //    foreach (UserModels r in list)
                    //    {
                    //        ViewBag.CurrentStatusDDL = statusInput;// r.IS_ACTIVE == true ? "1" : "0";
                    //    }
                    //}
                    //else
                    //{
                    //    ViewBag.CurrentStatusDDL = statusInput;
                    //}
                    ViewBag.CurrentBranch = param.SOL_CODE;
                    ViewBag.CurrentDepartment = param.Dept_Code;
                    ViewBag.CurrentDistrict = param.Hub_Code;
                    ViewBag.usernameInput = usernameInput;
                    ViewBag.empcodeInput = empcodeInput;
                    ViewBag.firstInput = firstInput;
                    ViewBag.lastInput = lastInput;
                    List<RoleModels> R02 = new List<RoleModels>();
                    if (Session["R02"] != null)
                    {
                        R02 = (List<RoleModels>)Session["R02"];
                        ViewBag.List_Role = R02[0].List_Role;
                        ViewBag.View_Role = R02[0].View_Role;
                        ViewBag.Update_Role = R02[0].Create_Role;
                        ViewBag.Delete_Role = R02[0].Delete_Role;
                        ViewBag.Create_Role = R02[0].Create_Role;
                        ViewBag.Export_Role = R02[0].Export_Role;
                    }
                    //ViewBag.CurrentStatusDDL = param.IS_ACTIVE;
                    return View("NdUserManagement", list);
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ขั้นตอน Approve Role");
                    var itemsSV = model.Where(r => r.IsSelected).ToList();
                    foreach (var i in itemsSV)
                    {
                        bool _r = false;
                        if (i.USER_LEVEL == 0 && i.User_Branch != null)
                        {
                            i.ADMIN_BY = Convert.ToString(Session["Username"]) != "" ? Convert.ToString(Session["Username"]) : "";
                            i.ADMIN_CREATE_DATE = DateTime.ParseExact(DateTime.Now.ToString(), "dd/MM/yyyy", null);//Convert.ToDateTime(DateTime.Now.ToString());//
                            i.USER_LEVEL = (i.User_Branch == "" || i.User_Branch == "001") ? 3 : 2; //2 = branch, 3 = head office
                            _r = AdministratorBAL.MainUser_UdateRole(i);
                        }
                    }
                    return RedirectToAction("NdUserManagement");
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล User");
                    //var items = model.Where(m => m.IsSelected).ToList();
                    //foreach (var i in items)
                    //{
                    //    bool resultDelete = AdministratorBAL.DeleteUserByID(i.USER_ID);
                    //}
                    return RedirectToAction("NdUserManagement");
                //break;
                case "reset":
                    list = AdministratorBAL.GetUser();
                    //return Json(list, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("NdUserManagement", list);

            }

            return null;
        }

        #endregion User

        //Role
        #region Role
        public ActionResult NdRoleManagement()
        {
            //ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า MainRole");
            //List<RoleModels> list = AdministratorBAL.GetRole();
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า Role");
            //List<UserModels> list = AdministratorBAL.GetUser();
            List<RoleModels> list = AdministratorBAL.NdGetRoleManagement();
            List<RoleModels> R01 = new List<RoleModels>();
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["R01"] != null)
            {
                R01 = (List<RoleModels>)Session["R01"];
                ViewBag.List_Role = R01[0].List_Role;
                ViewBag.View_Role = R01[0].View_Role;
                ViewBag.Update_Role = R01[0].Create_Role;
                ViewBag.Delete_Role = R01[0].Delete_Role;
                ViewBag.Create_Role = R01[0].Create_Role;
                ViewBag.Export_Role = R01[0].Export_Role;
            }
            return View("NdRoleManagement", list);

        }

        [HttpPost]
        public ActionResult NdRoleManage(string cmdButton, List<RoleModels> model, string searchInput)
        {
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            string _name = string.Empty;
            if (arrStr.Length > 1)
                _name = arrStr[1].ToString();
            //long lngID = 0;
            //if (arrStr.Length > 1)
            //    lngID = Convert.ToInt64(arrStr[1]);
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddRole");
                    return RedirectToAction("NdAddRole");

                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditRole");
                    return RedirectToAction("NdEditRole", new { name = _name });

                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล Role");
                    return RedirectToAction("NdRoleDelete", new { _c = "", _n = _name, _o = "deleteRole" });
                case "ExportExcel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddRole");
                    List<RoleModels> list = AdministratorBAL.NdGetRoleManagement();
                    var rolename = (from std in list
                                    select std.Role_Name)
                      .Distinct().ToList();
                    return RedirectToAction("RoleExportExcel", new { _c = "", _n = "", _o = "roleExport" });
                case "ExportCSV":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddRole");
                    return RedirectToAction("RoleExportCSV", new { _c = "", _n = "", _o = "roleExport" });
                case "roleSearch":
                    return RedirectToAction("NdRoleSearch", new { _c = "", _n = searchInput, _o = "dup" });
                case "NO":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล Role");
                    return RedirectToAction("NdRoleManagement");
                default:
                    break;
            }

            return null;
        }

        public ActionResult NdEditRole(string name)
        {
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["AudiLog"] != null && Session["EmpCode"] != null)
            {
                string emp = Session["EmpCode"].ToString();
                audiLog("EditRole", "แก้ไข Role: " + name, emp);
            }
            ViewBag.StatusRadioLst = LoadStatusRadio();
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า Role");
            string _c = string.Empty; string _n = name; string _o = "paramrm";
            List<RoleModels> list = AdministratorBAL.NdGetRoleManagementDetials(_c, _n, _o);
            var Seleted = list.Select(x => x.Role_Name).Distinct().ToList();
            ViewBag.roleName = Seleted[0].ToString();
            var QS = (from std in list
                      select std.Position_Role)
                      .Distinct().ToList();
            ViewBag.Seleted = QS[0].ToString();
            return View("NdEditRole", list);
        }

        public ActionResult NdRoleSearch(string _c, string _n, string _o)
        {
            List<RoleModels> R01 = new List<RoleModels>();
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["AudiLog"] != null && Session["EmpCode"] != null)
            {
                string emp = Session["EmpCode"].ToString();
                audiLog("RoleSearch", "ค้าหา Role: " + _n, emp);
            }
            //RoleModels chkDup = AdministratorBAL.NdCheckDupRoleName(_c, _n, _o);
            List<RoleModels> lst_chkDup = AdministratorBAL.NdCheckDupRoleName_lst(_c, _n, _o);
            //List<RoleModels> list = new List<RoleModels>();
            //if (chkDup.Role_Name != null)
            //{
            //    list.Add(chkDup);
            //}
            if (Session["R01"] != null)
            {
                R01 = (List<RoleModels>)Session["R01"];
                ViewBag.List_Role = R01[0].List_Role;
                ViewBag.View_Role = R01[0].View_Role;
                ViewBag.Update_Role = R01[0].Create_Role;
                ViewBag.Delete_Role = R01[0].Delete_Role;
                ViewBag.Create_Role = R01[0].Create_Role;
                ViewBag.Export_Role = R01[0].Export_Role;
            }
            ViewBag.searchInput = _n;
            return View("NdRoleManagement", lst_chkDup);
        }

        public ActionResult NdRoleDelete(string _c, string _n, string _o)
        {
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["AudiLog"] != null && Session["EmpCode"] != null)
            {
                string emp = Session["EmpCode"].ToString();
                audiLog("RoleDelete", "Delete Role: " + _n, emp);
            }
            RoleModels chkDup = AdministratorBAL.NdCheckDupRoleName(_c, _n, _o);
            List<RoleModels> list = new List<RoleModels>();
            if (chkDup.Role_Name != null)
            {
                list.Add(chkDup);
            }
            return RedirectToAction("NdRoleManagement");
        }

        public ActionResult GetRoleByID2(long ID)
        {

            RoleModels obj = AdministratorBAL.GetRoleByID(ID);

            return View("NdEditRole", obj);

        }

        public ActionResult NdAddRole()
        {
            List<RoleModuleMasterModel> list = new List<RoleModuleMasterModel>();
            ViewBag.StatusRadioLst = LoadStatusRadio();
            list = AdministratorBAL.GetAlldRole();
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return View("NdAddRole", list);

        }
        [HttpPost]
        public ActionResult NdAddRoleSelected(string cmdButton, RoleModels obj, string roleName, string role, string[] chkGranted, string[] chkDenied)
        {
            bool resultSave = false;
            var _rolename = string.Empty;

            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddRole");
                    //check roleName dup                   
                    string _c = string.Empty; string _n = roleName; string _o = "dup";
                    RoleModels chkDup = AdministratorBAL.NdCheckDupRoleName(_c, _n, _o);
                    if (chkDup.Role_Name != "" && chkDup.Role_Name != null)
                    {
                        ViewModel result = new ViewModel();
                        result.Result = "Duplicate RoleName";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else if (roleName != null)/*&& role != null*/
                    {
                        if (role == null)
                        {
                            role = "";
                        }
                        resultSave = AdministratorBAL.NdAddRole(roleName, role, chkGranted, chkDenied, obj);
                    }
                    var model = new { Result = resultSave };
                    return Json(model, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("NdRoleManagement");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddRole");
                    if (roleName != null)/* && role != null*/
                    {
                        if (role == null)
                        {
                            role = "";
                        }
                        resultSave = AdministratorBAL.NdEditRole(roleName, role, chkGranted, chkDenied, obj);
                    }
                    var model1 = new { Result = resultSave };
                    return Json(model1, JsonRequestBehavior.AllowGet);
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddRole");
                    return RedirectToAction("NdRoleManagement");

            }
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return null;//RedirectToAction("NdRoleManagement");
        }

        public ActionResult NdAddUserLDAP(int USER_LEVEL)
        {
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.USER_LEVEL = USER_LEVEL;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return View("NdAddUserLDAP");
        }

        public ActionResult NdCreateUser(int USER_LEVEL)
        {
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.CurrentStatusDDL = "1";
            ViewBag.USER_LEVEL = USER_LEVEL;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return View("NdCreateUser");
        }

        #endregion Role

        //AddUserLDap
        #region Add LDap
        [HttpPost]
        [AllowAnonymous]
        public ActionResult NdAddUserLDAPAd1(string cmdButton, string USER_LOGIN, string USER_FIRSTNAME, string USER_LASTNAME, string USER_EMAIL, int USER_LEVEL, string USER_EMP_CODE, string SOL_CODE, string ZONE_NAME, string DEPT_NAME, string statusInput, string Role_Name, string REMARK)
        {
            _logSys.WriteProcessLogFile(_strPathFile, "Begin AddUserLDAPAd1");
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.RoleList = LoadRoleListNd();
            ViewBag.USER_LEVEL = 0;// USER_LEVEL;
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.CurrentStatusDDL = "1";
            UserPostRequest userReq = new UserPostRequest();
            ApiPostResponse2 objApi = new ApiPostResponse2();
            ApiPostResponse2 Session_ObjApi = new ApiPostResponse2();
            //userReq.username = USER_LOGIN;
            //objApi.emp_code = USER_EMP_CODE;
            //userReq.password = USER_PASSWORD;
            DataTable _dt = new DataTable();
            int maxLength = 0;
            _logSys.WriteProcessLogFile(_strPathFile, "cmdButton(Row:1082): " + cmdButton);
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            switch (cmdButton)
            {
                case "VerifyUser":
                    //UserModels obj = new UserModels();

                    try
                    {
                        if (USER_EMP_CODE == "")
                        {
                            ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                            ModelState.AddModelError("", "โปรดระบุ รหัสพนักงาน");
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LOGIN&USER_PASSWORD : โปรดระบุชื่อผู้ใช้ และ รหัสบัญชี Ldap");
                            return View("NdCreateUser");
                        }
                        else
                        {
                            objApi = _authService.GetUserByUserNameV3(userReq, USER_EMP_CODE);
                            _logSys.WriteProcessLogFile(_strPathFile, "_authService.GetUserByUserName3(userReq) : " + objApi.fname + " " + objApi.lname);
                            if (objApi.emp_code != "")
                            {
                                Session["TempDataLdapUser"] = objApi;
                            }
                            //var value = objApi ?? null;
                            if (objApi.fname == null && objApi.lname == null)
                            {
                                ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName3(object) : null");
                                return View("NdCreateUser");

                            }
                            else if (objApi.fname == "" && objApi.lname == "")
                            {
                                ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName3(ไม่พบชื่อนี้ในระบบ ldap) : Empty");
                                return View("NdCreateUser");
                            }
                            else
                            {
                                ViewBag.SOL_CODE = objApi.sol_code == "" ? "" : cutSolCode(objApi.sol_code, maxLength);// objApi.sol_code;
                                ViewBag.USER_EMP_CODE = objApi.emp_code;
                                ViewBag.USER_FIRSTNAME = objApi.fname;
                                ViewBag.USER_LASTNAME = objApi.lname;
                                ViewBag.USER_EMAIL = objApi.email;
                                ViewBag.USER_LEVEL = objApi.sol_code != "" ? 2 : USER_LEVEL;//แก้ไข 20250218
                                ViewBag.ZONE_NAME = objApi.zone_name;
                                ViewBag.DEPT_NAME = objApi.dept_name;
                                ViewBag.CurrentStatusDDL = "1";
                                if (objApi.email != null && objApi.email != "")
                                {
                                    var userLogin = objApi.email.ToString().Split(_spd2);
                                    ViewBag.USER_LOGIN = userLogin[0];
                                }
                                else
                                {
                                    ViewBag.USER_LOGIN = "-";
                                }

                                ViewBag.Flag = true;
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_FIRSTNAME : " + objApi.fname);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LASTNAME : " + objApi.lname);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_EMAIL : " + objApi.email);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LOGIN : " + ViewBag.USER_LOGIN);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LEVEL : " + USER_LEVEL);
                            }
                        }

                        return View("NdAddUserLDAP");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                        ex.Message.ToString();
                        _logSys.WriteProcessLogFile(_strPathFile, "Error VerifyUser : " + ex.Message.ToString());
                    }
                    break;
                case "Save":
                    try
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2 (846)");
                        if (Role_Name == "0")
                        {
                            ModelState.AddModelError("", "โปรดระบุ Role******");
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LEVEL : โปรดระบุ Role");
                            ViewBag.SOL_CODE = SOL_CODE;
                            ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                            ViewBag.USER_FIRSTNAME = USER_FIRSTNAME;
                            ViewBag.USER_LASTNAME = USER_LASTNAME;
                            ViewBag.USER_EMAIL = USER_EMAIL;
                            ViewBag.USER_LEVEL = 0;// USER_LEVEL;
                            ViewBag.USER_LOGIN = USER_LOGIN;
                            ViewBag.ZONE_NAME = ZONE_NAME;
                            ViewBag.DEPT_NAME = DEPT_NAME;
                            ViewBag.Role_Name = Role_Name;
                            ViewBag.CurrentStatusDDL = statusInput;
                            ViewBag.Flag = true;
                            ViewBag.REMARK = REMARK;
                        }
                        else
                        {
                            if (Session["TempDataLdapUser"] != null)
                            {
                                _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2 : Session['empDataLdapUser'] != null");
                                Session_ObjApi = (ApiPostResponse2)Session["TempDataLdapUser"];
                                _dt = _ds.Tables["Officer_Details"];
                                DataRow _dr = _dt.NewRow();
                                _dr["emp_code"] = Session_ObjApi.emp_code;
                                _dr["fname"] = Session_ObjApi.fname;
                                _dr["lname"] = Session_ObjApi.lname;
                                _dr["id_card"] = Session_ObjApi.id_card;
                                _dr["birth_date"] = Session_ObjApi.birth_date;
                                _dr["grade"] = Session_ObjApi.grade;
                                _dr["emp_type"] = Session_ObjApi.emp_type;
                                _dr["position_id"] = Session_ObjApi.position_id;
                                _dr["position_name"] = Session_ObjApi.position_name;
                                _dr["email"] = Session_ObjApi.email;
                                _dr["tel"] = Session_ObjApi.tel;
                                _dr["org_level"] = Session_ObjApi.org_level;
                                _dr["comp_objid"] = Session_ObjApi.comp_objid;
                                _dr["comp_code"] = Session_ObjApi.comp_code;
                                _dr["comp_short_name"] = Session_ObjApi.comp_short_name;
                                _dr["comp_name"] = Session_ObjApi.comp_name;
                                _dr["group_objid"] = Session_ObjApi.group_objid;
                                _dr["group_code"] = Session_ObjApi.group_code;
                                _dr["group_short_name"] = Session_ObjApi.group_short_name;
                                _dr["group_name"] = Session_ObjApi.group_name;
                                _dr["field_objid"] = Session_ObjApi.field_objid;
                                _dr["field_code"] = Session_ObjApi.field_code;
                                _dr["field_short_name"] = Session_ObjApi.field_short_name;
                                _dr["field_name"] = Session_ObjApi.field_name;
                                _dr["dept_objid"] = Session_ObjApi.dept_objid;
                                _dr["dept_code"] = Session_ObjApi.dept_code;
                                _dr["dept_short_name"] = Session_ObjApi.dept_short_name;
                                _dr["dept_name"] = Session_ObjApi.dept_name;
                                _dr["center_objid"] = Session_ObjApi.center_objid;
                                _dr["center_code"] = Session_ObjApi.center_code;
                                _dr["center_short_name"] = Session_ObjApi.center_short_name;
                                _dr["center_name"] = Session_ObjApi.center_name;
                                _dr["zone_objid"] = Session_ObjApi.zone_objid;
                                _dr["zone_code"] = Session_ObjApi.zone_code;
                                _dr["zone_short_name"] = Session_ObjApi.zone_short_name;
                                _dr["zone_name"] = Session_ObjApi.zone_name;
                                _dr["division_objid"] = Session_ObjApi.division_objid;
                                _dr["division_code"] = Session_ObjApi.division_code;
                                _dr["division_short_name"] = Session_ObjApi.division_short_name;
                                _dr["division_name"] = Session_ObjApi.division_name;
                                _dr["sol_objid"] = Session_ObjApi.sol_objid;
                                _dr["sol_code"] = Session_ObjApi.sol_code;
                                _dr["sol_short_name"] = Session_ObjApi.sol_short_name;
                                _dr["sol_name"] = Session_ObjApi.sol_name;
                                _dr["sub_sol_objid"] = Session_ObjApi.sub_sol_objid;
                                _dr["sub_sol_code"] = Session_ObjApi.sub_sol_code;
                                _dr["sub_sol_short_name"] = Session_ObjApi.sub_sol_short_name;
                                _dr["sub_sol_name"] = Session_ObjApi.sub_sol_name;
                                _dr["start_date"] = DateTime.Now;// DateTime.ParseExact(Session_ObjApi.start_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                                _dr["stop_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.stop_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                                _dr["last_action"] = Session_ObjApi.last_action;
                                _dr["sol_instead_code"] = Session_ObjApi.sol_instead_code;
                                _dr["sol_instead_name"] = Session_ObjApi.sol_instead_name;
                                _dr["sol_hq"] = Session_ObjApi.sol_hq;
                                _dr["plans_000"] = Session_ObjApi.plans_000;
                                _dr["plansname_000"] = Session_ObjApi.plansname_000;
                                _dr["org_000"] = Session_ObjApi.org_000;
                                _dr["solid_000"] = Session_ObjApi.solid_000;
                                _dr["orgname_000"] = Session_ObjApi.orgname_000;
                                _dr["org_748"] = Session_ObjApi.org_748;
                                _dr["solid_748"] = Session_ObjApi.solid_748;
                                _dr["orgname_748"] = Session_ObjApi.orgname_748;
                                _dr["create_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.create_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                                _dr["update_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.update_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));

                                _dt.Rows.Add(_dr);

                                _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2 : add AD to DataSet ");
                            }
                            _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2 : obj2 add details");
                            UserModels obj2 = new UserModels();
                            //var userLogin2 = USER_LOGIN.Split(_spd);
                            obj2.USER_EMP_CODE = USER_EMP_CODE;
                            obj2.USER_LOGIN = USER_LOGIN;
                            obj2.USER_FIRSTNAME = USER_FIRSTNAME;
                            obj2.USER_LASTNAME = USER_LASTNAME;
                            obj2.USER_EMAIL = USER_EMAIL;
                            obj2.USER_LEVEL = 0;// Convert.ToInt32(USER_LEVEL);
                            //USER_LEVEL 0 Not role, 1 Admin, 2 Role Branch, 3 Role Head office 4 Admin1, 5 Amin2;
                            obj2.USER_FLAG = true;//true = AD Account
                            obj2.USER_EMP_CODE = USER_EMP_CODE;
                            obj2.SOL_CODE = Session_ObjApi.sol_code == "" ? "" : Session_ObjApi.sol_code;// cutSolCode(SOL_CODE, maxLength);
                            obj2.IS_ACTIVE = true;
                            obj2.Role_Name = Role_Name;
                            obj2.Remark = REMARK;
                            _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LOGIN : " + obj2.USER_LOGIN);
                            _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_FIRSTNAME : " + obj2.USER_FIRSTNAME);
                            _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LASTNAME : " + obj2.USER_LASTNAME);
                            _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMAIL : " + obj2.USER_EMAIL);
                            _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LEVEL : " + obj2.USER_LEVEL);
                            _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMP_CODE : " + obj2.USER_EMP_CODE);
                            //bool resultSave = AdministratorBAL.AddUserLDAPV3(obj2);
                            _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2 : InsertDataFromAPI");
                            bool resultSave = AdministratorBAL.InsertDataFromAPI(Session_ObjApi, _dt, obj2, "SYSTEMOFFICER_Details");
                            if (resultSave == false)
                            {
                                ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                                ViewBag.USER_LEVEL = obj2.USER_LEVEL;
                                ViewBag.SOL_CODE = SOL_CODE;
                                ViewBag.USER_FIRSTNAME = USER_FIRSTNAME;
                                ViewBag.USER_LASTNAME = USER_LASTNAME;
                                ViewBag.USER_EMAIL = USER_EMAIL;
                                ViewBag.USER_LOGIN = USER_LOGIN;
                                ViewBag.ZONE_NAME = ZONE_NAME;
                                ViewBag.DEPT_NAME = DEPT_NAME;
                                ViewBag.CurrentStatusDDL = statusInput;
                                ViewBag.Flag = true;
                                ModelState.AddModelError("", "ชื่อผู้ใช้ได้มีการเพิ่มแล้ว");
                                _logSys.WriteProcessLogFile(_strPathFile, "resultSave(ชื่อผู้ใช้ได้มีการเพิ่มแล้ว) : false");
                                return View("NdAddUserLDAP");
                            }
                            else
                            {
                                _logSys.WriteProcessLogFile(_strPathFile, "resultSave(เพิ่มผู้ใช้ได้) :  true");
                                return RedirectToAction("NdUserManagement");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "AddUserLDAP2 : " + ex.Message.ToString());
                    }
                    break;
                case "Cancel":
                    return RedirectToAction("NdCreateUser", new { USER_LEVEL = 0 }); //RedirectToAction("NdUserManagement");
            }
            return View("NdAddUserLDAP");

        }

        #endregion Add LDap

        //EditUser
        #region EditUser
        public ActionResult NdEditUser(long ID)
        {
            string DecryptPWD = string.Empty;
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.RoleList = LoadRoleListNd();
            ViewBag.CurrentLockAccountList = "0,Unlock|1,Lock";
            UserModels obj = AdministratorBAL.GetUserByID(ID);
            ViewBag.ID = obj.USER_ID;
            ViewBag.USER_LOGIN = obj.USER_LOGIN;//@USER_LOGIN as varchar(50),
            if (obj.USER_ID > 28)
            {
                DecryptPWD = AesUtil.DecryptString(_iniCon.cryptoKey, obj.USER_PASSWORD, iv);//@USER_PASSWORD as varchar(50),
            }
            else
            {
                DecryptPWD = obj.USER_PASSWORD.ToString();
            }
            ViewBag.USER_EMP_CODE = obj.USER_EMP_CODE;
            ViewBag.USER_PASSWORD = DecryptPWD;
            ViewBag.USER_FIRSTNAME = obj.USER_FIRSTNAME;//@USER_FIRSTNAME as varchar(150),            
            ViewBag.USER_LASTNAME = obj.USER_LASTNAME;//@USER_LASTNAME as varchar(150),            
            ViewBag.USER_LEVEL = obj.USER_LEVEL;//@USER_LEVEL as int,            
            ViewBag.USER_EMAIL = obj.USER_EMAIL;//@USER_EMAIL as nvarchar(50)            
            ViewBag.USER_FLAG = obj.USER_FLAG;//@USER_FLAG as bit,
            ViewBag.USER_LOCK = obj.USER_LOCK;
            ViewBag.IS_ACTIVE = obj.IS_ACTIVE;
            ViewBag.HUB_CODE = obj.Hub_Code;
            ViewBag.HUB_NAME = obj.Hub_Name;
            ViewBag.DEPT_CODE = obj.Dept_Code;
            ViewBag.DEPT_NAME = obj.Dept_Name;
            ViewBag.SOL_NAME = obj.SOL_NAME;
            ViewBag.Remark = obj.Remark;
            ViewBag.CurrentStatusDDL = obj.IS_ACTIVE == true ? "1" : "2";
            ViewBag.CurrentRole = obj.Role_Name;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return View("NdEditUser");

        }
        [HttpPost]
        public ActionResult NdEditUser(string cmdButton, long ID, string USER_EMP_CODE, string USER_LOGIN, string USER_PASSWORD, string USER_FIRSTNAME, string USER_LASTNAME, int USER_LEVEL, string USER_EMAIL, bool? isChecked, string Role_Name, string SOL_NAME, string HUB_NAME, string DEPT_NAME, string Remark, string statusInput)
        {
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            switch (cmdButton)
            {
                case "Save":
                    if (Role_Name == "0")
                    {
                        if (Session["AudiLog"] != null && Session["EmpCode"] != null)
                        {
                            string emp = Session["EmpCode"].ToString();
                            audiLog("EditUser", "ไปที่หน้า Edit User", emp);
                        }
                        ModelState.AddModelError("", "โปรดระบุ Role******");
                        _logSys.WriteProcessLogFile(_strPathFile, "USER_LEVEL : โปรดระบุ Role");
                        ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                        ViewBag.CurrentRoleList = LoadMasterRole();
                        ViewBag.StatusDDL = LoadStatusActive();
                        ViewBag.RoleList = LoadRoleListNd();
                        ViewBag.CurrentLockAccountList = "0,Unlock|1,Lock";
                        ViewBag.ID = ID; ;
                        ViewBag.USER_LOGIN = USER_LOGIN;
                        ViewBag.USER_FIRSTNAME = USER_FIRSTNAME;//@USER_FIRSTNAME as varchar(150),            
                        ViewBag.USER_LASTNAME = USER_LASTNAME;//@USER_LASTNAME as varchar(150),            
                        ViewBag.USER_LEVEL = 0;//@USER_LEVEL as int,            
                        ViewBag.USER_EMAIL = USER_EMAIL;//@USER_EMAIL as nvarchar(50)   
                        //ViewBag.USER_LOCK = Convert.ToBoolean(USER_LOCK);
                        ViewBag.CurrentStatusDDL = statusInput;// == true ? "1" : "2";
                        ViewBag.HUB_NAME = HUB_NAME;
                        ViewBag.DEPT_NAME = DEPT_NAME;
                        ViewBag.SOL_NAME = SOL_NAME;
                        ViewBag.Remark = Remark;
                        return View("NdEditUser");
                    }
                    else
                    {
                        //ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditUser");
                        if (Session["AudiLog"] != null && Session["EmpCode"] != null)
                        {
                            string emp = Session["EmpCode"].ToString();
                            audiLog("EditUser", "ไปที่หน้า Edit User", emp);
                        }
                        UserModels obj = new UserModels();
                        obj.USER_ID = ID;
                        obj.USER_LOGIN = USER_LOGIN;
                        var ENCRYPTPWD = AesUtil.EncryptString(_iniCon.cryptoKey, USER_PASSWORD, iv);
                        obj.USER_PASSWORD = ENCRYPTPWD;
                        obj.USER_FIRSTNAME = USER_FIRSTNAME;
                        obj.USER_LASTNAME = USER_LASTNAME;
                        obj.USER_LEVEL = 0;// USER_LEVEL;
                        obj.Role_Name = Role_Name;
                        obj.USER_EMAIL = USER_EMAIL;
                        //obj.USER_LOCK = Convert.ToBoolean(USER_LOCK);
                        obj.IS_ACTIVE = statusInput == "1" ? true : false;// Convert.ToBoolean(statusInput);
                        obj.Remark = Remark;
                        bool resultSave = AdministratorBAL.EditUser2(obj);
                    }
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditUser");
                    return RedirectToAction("NdUserManagement");

            }

            return RedirectToAction("NdUserManagement");
        }
        #endregion EditUser

        //User Transfer
        #region UserTransfer
        [HttpGet]
        public ActionResult NdUserTransfer()
        {
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }

            if (Session["AudiLog"] != null && Session["EmpCode"] != null)
            {
                string emp = Session["EmpCode"].ToString();
                audiLog("UserTransfer ScheduleManagement", "ไปที่หน้า User Transfer ScheduleManagement", emp);
            }
            ViewBag.SystemBranch = LoadSystemBranch();
            ViewBag.SystemHub = LoadSystemHub();
            ViewBag.SystemDistrict = LoadSystemDistrict();
            ViewBag.SystemDepartment = LoadSystemDepartment();
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.RoleList = LoadRoleListNd();
            ViewBag.CurrentStatusDDL = "1";
            //ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า User Transfer ScheduleManagement");
            List<UserModels> list = AdministratorBAL.NdGetUserTransfer();
            List<UserTransferScheduleModel> list1 = AdministratorBAL.NdGetUserTransfer2();
            GroupUserModel m = new GroupUserModel();
            m.lst_m1 = list;
            m.lst_m2 = list1;
            List<RoleModels> R03 = new List<RoleModels>();
            if (Session["R03"] != null)
            {
                R03 = (List<RoleModels>)Session["R03"];
                ViewBag.List_Role = R03[0].List_Role;
                ViewBag.View_Role = R03[0].View_Role;
                ViewBag.Update_Role = R03[0].Create_Role;
                ViewBag.Delete_Role = R03[0].Delete_Role;
                ViewBag.Create_Role = R03[0].Create_Role;
                ViewBag.Export_Role = R03[0].Export_Role;
            }
            return View("NdUserTransfer", m);

        }

        [HttpPost]
        public ActionResult NdUserTrans(string cmdButton, string emp_code, string fname, string lname, string SchDate, string FrDate, string ToDate, string statusInput, string branchInput1, string branchInput2, string disInputx1, string disInputx1_code, string disInputx2, string disInputx2_code, string departInputx1, string departInputx1_code, string departInputx2, string departInputx2_code, string USER_LEVEL1, string USER_LEVEL2, string Role_Name1, string Role_Name2, string _trans)
        {
            string strValue = string.Empty;
            string empcode = string.Empty;
            string strVariable = string.Empty;
            if (cmdButton.Contains("|"))
            {
                string[] arrStr = cmdButton.Split('|');
                strValue = Convert.ToString(arrStr[0]);
                empcode = Convert.ToString(arrStr[1]);
                if (arrStr.Length > 2)
                {
                    strVariable = Convert.ToString(arrStr[2]);
                }
            }
            else
            {
                strValue = cmdButton;
            }
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            UserTransferScheduleModel obj = new UserTransferScheduleModel();

            switch (strValue)
            {

                case "AddUserTransfer":
                    //return RedirectToAction("NdAddUserTransfer");
                    return RedirectToAction("NdCreateUserTrans");
                case "Edit":
                    if (Session["AudiLog"] != null && Session["EmpCode"] != null)
                    {
                        string emp = Session["EmpCode"].ToString();
                        audiLog("EditUser", "ไปที่หน้า Edit User", emp);
                    }
                    return RedirectToAction("NdEditUserTransfer", new { code = empcode, id = strVariable });
                case "View":
                    if (Session["AudiLog"] != null && Session["EmpCode"] != null)
                    {
                        string emp = Session["EmpCode"].ToString();
                        audiLog("EditUser", "ไปที่หน้า View User", emp);
                    }
                    return RedirectToAction("NdEditUserTransfer", new { code = empcode, id = strVariable });
                case "Search":
                    if (Session["AudiLog"] != null && Session["EmpCode"] != null)
                    {
                        string emp = Session["EmpCode"].ToString();
                        audiLog("ค้นหาบนหน้า User Transfer", "ไปที่หน้า User Transfer", emp);
                    }
                    obj.emp_code = emp_code != "" ? emp_code : "";
                    obj.fname = fname != "" ? fname : "";
                    obj.lname = lname != "" ? lname : "";
                    if (SchDate != "") { obj.sch_time = DateTime.ParseExact(SchDate, "dd/MM/yyyy", null); }
                    if (FrDate != "") { obj.start_date = DateTime.ParseExact(FrDate, "dd/MM/yyyy", null); }
                    if (ToDate != "") { obj.end_date = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null); }
                    return RedirectToAction("NdSearchUserTransfer", obj);
                case "reset":
                    if (Session["AudiLog"] != null && Session["EmpCode"] != null)
                    {
                        string emp = Session["EmpCode"].ToString();
                        audiLog("reset บนหน้า User Transfer", "ไปที่หน้า User Transfer", emp);
                    }
                    return RedirectToAction("NdUserTransfer");
                case "Save":
                    if (Session["AudiLog"] != null && Session["EmpCode"] != null)
                    {
                        string emp = Session["EmpCode"].ToString();
                        audiLog("SaveUserTransfer", "ขั้นตอน Save User Transfer", emp);
                    }
                    obj.emp_code = emp_code != "" ? emp_code : "";
                    obj.fname = fname != "" ? fname : "";
                    obj.lname = lname != "" ? lname : "";
                    _logSys.WriteProcessLogFile(_strPathFile, "SchDate : " + DateTime.ParseExact(SchDate, "dd/MM/yyyy", null));
                    _logSys.WriteProcessLogFile(_strPathFile, "FrDate : " + DateTime.ParseExact(FrDate, "dd/MM/yyyy", null));
                    _logSys.WriteProcessLogFile(_strPathFile, "ToDate : " + DateTime.ParseExact(ToDate, "dd/MM/yyyy", null));
                    if (SchDate != "") { obj.sch_time = DateTime.ParseExact(SchDate, "dd/MM/yyyy", null); }
                    if (FrDate != "") { obj.start_date = DateTime.ParseExact(FrDate, "dd/MM/yyyy", null); }
                    if (ToDate != "") { obj.end_date = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null); }
                    obj.branch_name = branchInput1 == "" ? "" : getMS(branchInput1, "", "branch");
                    obj.branch_code = branchInput1 == "" ? "" : branchInput1;
                    obj.dept_name = departInputx1 == "" ? "" : departInputx1;
                    obj.dept_code = departInputx1_code == "" ? "" : departInputx1_code;
                    obj.hub_name = disInputx1 == "" ? "" : disInputx1;
                    obj.hub_code = disInputx1_code == "" ? "" : disInputx1_code;
                    obj.user_level = Convert.ToInt32(USER_LEVEL1);
                    obj.branch_name2 = branchInput2 == "" ? "" : getMS(branchInput2, "", "branch"); ;
                    obj.branch_code2 = branchInput2 == "" ? "" : branchInput2;
                    obj.dept_name2 = departInputx2 == "" ? "" : departInputx2;
                    obj.dept_code2 = departInputx2_code == "" ? "" : departInputx2_code;
                    obj.hub_name2 = disInputx2 == "" ? "" : disInputx2;
                    obj.hub_code2 = disInputx2_code == "" ? "" : disInputx2_code;
                    obj.user_level2 = Convert.ToInt32(USER_LEVEL2);
                    obj.is_active = statusInput == "1" ? true : false;
                    obj.role_name1 = Role_Name1;
                    obj.role_name2 = Role_Name2;
                    obj.type = _trans == "" ? "" : _trans;
                    return RedirectToAction("NdInsertUserTransfer", obj);
            }

            return null;
        }

        public ActionResult NdCreateUserTrans()
        {
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["AudiLog"] != null && Session["EmpCode"] != null)
            {
                string emp = Session["EmpCode"].ToString();
                audiLog("UserTransfer ScheduleManagement", "ไปที่หน้า User Transfer ScheduleManagement", emp);
            }
            List<RoleModels> R03 = new List<RoleModels>();
            if (Session["R03"] != null)
            {
                R03 = (List<RoleModels>)Session["R03"];
                ViewBag.List_Role = R03[0].List_Role;
                ViewBag.View_Role = R03[0].View_Role;
                ViewBag.Update_Role = R03[0].Create_Role;
                ViewBag.Delete_Role = R03[0].Delete_Role;
                ViewBag.Create_Role = R03[0].Create_Role;
                ViewBag.Export_Role = R03[0].Export_Role;
            }
            return View("NdCreateUserTrans");
        }

        public ActionResult NdCreateUserTrasnLdap(string cmdButton, string USER_EMP_CODE)
        {
            _logSys.WriteProcessLogFile(_strPathFile, "Begin NdCreateUserTrasnLdap");
            ApiPostResponse2 objApi = new ApiPostResponse2();
            ApiPostResponse2 Session_ObjApi = new ApiPostResponse2();
            UserPostRequest userReq = new UserPostRequest();
            DataTable _dt = new DataTable();
            int maxLength = 0;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            switch (cmdButton)
            {
                case "Reset":
                    ViewBag.USER_EMP_CODE = "";
                    _logSys.WriteProcessLogFile(_strPathFile, "NdCreateUserTrasnLdap : Reset");
                    return View("NdCreateUserTrans");
                case "VerifyUser":
                    try
                    {
                        if (USER_EMP_CODE == "")
                        {
                            ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                            ModelState.AddModelError("", "โปรดระบุ รหัสพนักงาน");
                            _logSys.WriteProcessLogFile(_strPathFile, "NdCreateUserTrasnLdap : โปรดระบุรหัสพนักงาน");
                            return View("NdCreateUserTrans");
                        }
                        else
                        {
                            //check user in the App
                            LoginViewModel loginVMLdap = _accService.CheckUserTransferLDAP(USER_EMP_CODE);
                            if (loginVMLdap.Login_State)//มีในระบบแล้ว
                            {
                                //check tble TBL_USER_TRANSFER_SCHEDULE_MG ล่าสุด
                                UserTransferScheduleModel SchDetails = AdministratorBAL.NdCheckTblTransfer(USER_EMP_CODE, "", "chKSch");
                                //1.ไม่เคยสร้าง schedule หรือเคยสร้างมีแต่เวลาสิ้นสุดแล้ว สร้าง schedule ได้
                                if (SchDetails.emp_code == "" || SchDetails.emp_code == null)
                                {
                                    return RedirectToAction("NdAddUserTrans_withEmpcode", loginVMLdap);/*new { loginVMLdap, code = USER_EMP_CODE }*/
                                }
                                else//2.เวลายังไม่สิ้นสุด สร้างไม่ได้ ต้องเป็นแก้ไขได้แทน
                                {
                                    ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                                    ModelState.AddModelError("", "*ได้มีการสร้าง User Transfer ไว้แล้ว โปรดไปที่หน้า User Transfer เพื่อแก้ไข");
                                    //return RedirectToAction("NdEditUserTransfer", new { code = SchDetails.emp_code, id = SchDetails.Transfer_Id });
                                    return View("NdCreateUserTrans");
                                }
                            }
                            else//ไม่มีในระบบ บันทึกลง TBL_USERS, SYSTEMOFFICER_Details และไปสร้าง schedule ต่อ
                            {
                                //get ldap from GHB
                                objApi = _authService.GetUserByUserNameV3_2(userReq, USER_EMP_CODE);
                                _logSys.WriteProcessLogFile(_strPathFile, "_authService.GetUserByUserName3_2(userReq) : " + objApi.fname + " " + objApi.lname);
                                if (objApi.emp_code != "" && objApi.emp_code != null)
                                {
                                    Session["TempDataLdapUser"] = objApi;
                                }
                                //var value = objApi ?? null;
                                if (objApi.fname == null && objApi.lname == null)
                                {
                                    ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                    _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName3(object) : null");
                                    return View("NdCreateUserTrans");

                                }
                                else if (objApi.fname == "" && objApi.lname == "")
                                {
                                    ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                    _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName3(ไม่พบชื่อนี้ในระบบ ldap) : Empty");
                                    return View("NdCreateUserTrans");
                                }
                                else
                                {
                                    //1.Begin Insert process TBL_USERS, SYSTEMOFFICER_Details
                                    if (Session["TempDataLdapUser"] != null)
                                    {
                                        _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2 : Session['empDataLdapUser'] != null");
                                        Session_ObjApi = (ApiPostResponse2)Session["TempDataLdapUser"];
                                        _dt = _ds.Tables["Officer_Details"];
                                        DataRow _dr = _dt.NewRow();
                                        _dr["emp_code"] = Session_ObjApi.emp_code;
                                        _dr["fname"] = Session_ObjApi.fname;
                                        _dr["lname"] = Session_ObjApi.lname;
                                        _dr["id_card"] = Session_ObjApi.id_card;
                                        _dr["birth_date"] = Session_ObjApi.birth_date;
                                        _dr["grade"] = Session_ObjApi.grade;
                                        _dr["emp_type"] = Session_ObjApi.emp_type;
                                        _dr["position_id"] = Session_ObjApi.position_id;
                                        _dr["position_name"] = Session_ObjApi.position_name;
                                        _dr["email"] = Session_ObjApi.email;
                                        _dr["tel"] = Session_ObjApi.tel;
                                        _dr["org_level"] = Session_ObjApi.org_level;
                                        _dr["comp_objid"] = Session_ObjApi.comp_objid;
                                        _dr["comp_code"] = Session_ObjApi.comp_code;
                                        _dr["comp_short_name"] = Session_ObjApi.comp_short_name;
                                        _dr["comp_name"] = Session_ObjApi.comp_name;
                                        _dr["group_objid"] = Session_ObjApi.group_objid;
                                        _dr["group_code"] = Session_ObjApi.group_code;
                                        _dr["group_short_name"] = Session_ObjApi.group_short_name;
                                        _dr["group_name"] = Session_ObjApi.group_name;
                                        _dr["field_objid"] = Session_ObjApi.field_objid;
                                        _dr["field_code"] = Session_ObjApi.field_code;
                                        _dr["field_short_name"] = Session_ObjApi.field_short_name;
                                        _dr["field_name"] = Session_ObjApi.field_name;
                                        _dr["dept_objid"] = Session_ObjApi.dept_objid;
                                        _dr["dept_code"] = Session_ObjApi.dept_code;
                                        _dr["dept_short_name"] = Session_ObjApi.dept_short_name;
                                        _dr["dept_name"] = Session_ObjApi.dept_name;
                                        _dr["center_objid"] = Session_ObjApi.center_objid;
                                        _dr["center_code"] = Session_ObjApi.center_code;
                                        _dr["center_short_name"] = Session_ObjApi.center_short_name;
                                        _dr["center_name"] = Session_ObjApi.center_name;
                                        _dr["zone_objid"] = Session_ObjApi.zone_objid;
                                        _dr["zone_code"] = Session_ObjApi.zone_code;
                                        _dr["zone_short_name"] = Session_ObjApi.zone_short_name;
                                        _dr["zone_name"] = Session_ObjApi.zone_name;
                                        _dr["division_objid"] = Session_ObjApi.division_objid;
                                        _dr["division_code"] = Session_ObjApi.division_code;
                                        _dr["division_short_name"] = Session_ObjApi.division_short_name;
                                        _dr["division_name"] = Session_ObjApi.division_name;
                                        _dr["sol_objid"] = Session_ObjApi.sol_objid;
                                        _dr["sol_code"] = Session_ObjApi.sol_code;
                                        _dr["sol_short_name"] = Session_ObjApi.sol_short_name;
                                        _dr["sol_name"] = Session_ObjApi.sol_name;
                                        _dr["sub_sol_objid"] = Session_ObjApi.sub_sol_objid;
                                        _dr["sub_sol_code"] = Session_ObjApi.sub_sol_code;
                                        _dr["sub_sol_short_name"] = Session_ObjApi.sub_sol_short_name;
                                        _dr["sub_sol_name"] = Session_ObjApi.sub_sol_name;
                                        _dr["start_date"] = DateTime.Now;// DateTime.ParseExact(Session_ObjApi.start_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                                        _dr["stop_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.stop_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                                        _dr["last_action"] = Session_ObjApi.last_action;
                                        _dr["sol_instead_code"] = Session_ObjApi.sol_instead_code;
                                        _dr["sol_instead_name"] = Session_ObjApi.sol_instead_name;
                                        _dr["sol_hq"] = Session_ObjApi.sol_hq;
                                        _dr["plans_000"] = Session_ObjApi.plans_000;
                                        _dr["plansname_000"] = Session_ObjApi.plansname_000;
                                        _dr["org_000"] = Session_ObjApi.org_000;
                                        _dr["solid_000"] = Session_ObjApi.solid_000;
                                        _dr["orgname_000"] = Session_ObjApi.orgname_000;
                                        _dr["org_748"] = Session_ObjApi.org_748;
                                        _dr["solid_748"] = Session_ObjApi.solid_748;
                                        _dr["orgname_748"] = Session_ObjApi.orgname_748;
                                        _dr["create_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.create_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                                        _dr["update_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.update_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));

                                        _dt.Rows.Add(_dr);

                                        _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2 : add AD to DataSet ");
                                    }
                                    _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2 : obj2 add details");
                                    UserModels obj2 = new UserModels();
                                    //var userLogin2 = USER_LOGIN.Split(_spd);
                                    obj2.USER_EMP_CODE = USER_EMP_CODE;
                                    var arremail = Session_ObjApi.email.Split('@');
                                    obj2.USER_LOGIN = arremail[0];
                                    obj2.USER_FIRSTNAME = Session_ObjApi.fname;
                                    obj2.USER_LASTNAME = Session_ObjApi.lname;
                                    obj2.USER_EMAIL = Session_ObjApi.email;
                                    obj2.USER_LEVEL = 0;// Convert.ToInt32(USER_LEVEL);
                                                        //USER_LEVEL 0 Not role, 1 Admin, 2 Role Branch, 3 Role Head office 4 Admin1, 5 Amin2;
                                    obj2.USER_FLAG = true;//true = AD Account
                                    obj2.USER_EMP_CODE = USER_EMP_CODE;
                                    obj2.SOL_CODE = Session_ObjApi.sol_code == "" ? "" : Session_ObjApi.sol_code;// cutSolCode(SOL_CODE, maxLength);
                                    obj2.IS_ACTIVE = true;
                                    obj2.Role_Name = "";
                                    obj2.Remark = "";
                                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LOGIN : " + obj2.USER_LOGIN);
                                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_FIRSTNAME : " + obj2.USER_FIRSTNAME);
                                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LASTNAME : " + obj2.USER_LASTNAME);
                                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMAIL : " + obj2.USER_EMAIL);
                                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LEVEL : " + obj2.USER_LEVEL);
                                    _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMP_CODE : " + obj2.USER_EMP_CODE);
                                    //bool resultSave = AdministratorBAL.AddUserLDAPV3(obj2);
                                    _logSys.WriteProcessLogFile(_strPathFile, "Begin Save UserLdap2Transfer : InsertDataFromAPI");
                                    bool resultSave = AdministratorBAL.InsertDataFromAPI(Session_ObjApi, _dt, obj2, "SYSTEMOFFICER_Details");

                                    //2.ไปสร้าง schedule ต่อ


                                    ViewBag.SOL_CODE = objApi.sol_code == "" ? "" : cutSolCode(objApi.sol_code, maxLength);// objApi.sol_code;
                                    ViewBag.USER_EMP_CODE = objApi.emp_code;
                                    ViewBag.USER_FIRSTNAME = objApi.fname;
                                    ViewBag.USER_LASTNAME = objApi.lname;
                                    ViewBag.USER_EMAIL = objApi.email;
                                    ViewBag.USER_LEVEL = 0;// objApi.sol_code != "" ? 2 : USER_LEVEL;//แก้ไข 20250218
                                    ViewBag.ZONE_NAME = objApi.zone_name;
                                    ViewBag.DEPT_NAME = objApi.dept_name;
                                    ViewBag.CurrentStatusDDL = "1";
                                    if (objApi.email != null && objApi.email != "")
                                    {
                                        var userLogin = objApi.email.ToString().Split(_spd2);
                                        ViewBag.USER_LOGIN = userLogin[0];
                                    }
                                    else
                                    {
                                        ViewBag.USER_LOGIN = "-";
                                    }

                                    ViewBag.Flag = true;
                                    _logSys.WriteProcessLogFile(_strPathFile, "V.USER_FIRSTNAME : " + objApi.fname);
                                    _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LASTNAME : " + objApi.lname);
                                    _logSys.WriteProcessLogFile(_strPathFile, "V.USER_EMAIL : " + objApi.email);
                                    _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LOGIN : " + ViewBag.USER_LOGIN);
                                    _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LEVEL : " + 0);
                                    return RedirectToAction("NdAddUserTrans_withEmpcode", new { userDetails = "", code = objApi.emp_code });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "NdCreateUserTrasnLdap : " + ex.Message.ToString());
                    }
                    break;


            }
            return View("NdCreateUserTrans");
        }

        [HttpGet]
        public ActionResult NdAddUserTransfer()
        {
            ViewBag.SystemBranch = LoadSystemBranch();
            ViewBag.SystemHub = LoadSystemHub();
            ViewBag.SystemDistrict = LoadSystemDistrict();
            ViewBag.SystemDepartment = LoadSystemDepartment();
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.RoleList = LoadRoleListNd();
            ViewBag.TransferStatus = LoadTransferRadio();
            ViewBag.CurrentStatusDDL = "1";
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            //ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า User Transfer ScheduleManagement");
            if (Session["AudiLog"] != null)
            {
                AudiLogModel audilog = (AudiLogModel)Session["AudiLog"];
                audilog.WorkFunction = "AddUserTransfer";
                audilog.Message = "ไปที่หน้า Add User Transfer";
                if (Session["latitude"] != null && Session["longitude"] != null)
                {
                    audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                    audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                }
                ProcessLogBAL.AddProcessLog2(audilog);
            }
            //List<UserModels> list = AdministratorBAL.GetUser();
            UserTransferScheduleModel m = new UserTransferScheduleModel();

            return View("NdAddUserTransfer", m);

        }


        public ActionResult NdAddUserTrans_withEmpcode(LoginViewModel userDetails, string code)
        {
            UserTransferScheduleModel m = new UserTransferScheduleModel();
            ViewBag.SystemBranch = LoadSystemBranch();
            ViewBag.SystemHub = LoadSystemHub();
            ViewBag.SystemDistrict = LoadSystemDistrict();
            ViewBag.SystemDepartment = LoadSystemDepartment();
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.RoleList = LoadRoleListNd();
            ViewBag.TransferStatus = LoadTransferRadio();
            ViewBag.CurrentStatusDDL = "1";
            //ViewBag.TransferSeleted = "1";
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["AudiLog"] != null)
            {
                AudiLogModel audilog = (AudiLogModel)Session["AudiLog"];
                audilog.WorkFunction = "AddUserTransfer";
                audilog.Message = "ไปที่หน้า Add User Transfer";
                if (Session["latitude"] != null && Session["longitude"] != null)
                {
                    audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                    audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                }
                ProcessLogBAL.AddProcessLog2(audilog);
            }
            if (userDetails.Emp_Code != "")
            {
                UserTransferScheduleModel SchDetails = AdministratorBAL.NdGetUserSysTransfer(userDetails.Emp_Code, "", "usersys");
                m.emp_code = SchDetails.emp_code;
                m.fname = SchDetails.fname;
                m.lname = SchDetails.lname;
                m.is_active = SchDetails.is_active;
                m.branch_code = SchDetails.branch_code;
                m.branch_name = SchDetails.branch_name;
                m.hub_code = SchDetails.hub_code;
                m.hub_name = SchDetails.hub_name;
                m.dept_code = SchDetails.dept_code;
                m.dept_name = SchDetails.dept_name;
                m.user_level = SchDetails.user_level;
                m.remark = "";
                ViewBag.CurrentBranch = SchDetails.branch_code;
                ViewBag.disInputx1 = SchDetails.hub_name;
                ViewBag.departInputx1 = SchDetails.dept_name;
                ViewBag.disInputx1_code = SchDetails.hub_code;
                ViewBag.departInputx1_code = SchDetails.dept_code;
            }
            //var QS = (from std in list
            //          select std.Position_Role)
            //          .Distinct().ToList();
            //ViewBag.Seleted = QS[0].ToString();
            return View("NdAddUserTransfer", m);
        }

        //[HttpPost]
        public ActionResult NdEditUserTransfer(string code, long id)
        {
            GroupUserModel m = new GroupUserModel();
            ViewBag.SystemBranch = LoadSystemBranch();
            ViewBag.SystemHub = LoadSystemHub();
            ViewBag.SystemDistrict = LoadSystemDistrict();
            ViewBag.SystemDepartment = LoadSystemDepartment();
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.RoleList = LoadRoleListNd();
            ViewBag.TransferStatus = LoadTransferRadio();
            ViewBag.CurrentStatusDDL = "1";
            //ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า User Transfer ScheduleManagement");
            if (Session["AudiLog"] != null)
            {
                AudiLogModel audilog = (AudiLogModel)Session["AudiLog"];
                audilog.WorkFunction = "EditUserTransfer";
                audilog.Message = "ไปที่หน้า Edit User Transfer";
                if (Session["latitude"] != null && Session["longitude"] != null)
                {
                    audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                    audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                }
                ProcessLogBAL.AddProcessLog2(audilog);
            }
            UserModels _u1 = AdministratorBAL.NdGetEditUserTransfer();
            UserTransferScheduleModel _u2 = AdministratorBAL.NdGetUserTransfer_param(code, id);
            m.m1 = _u1;
            m.m2 = _u2;
            ViewBag.CurrentBranch = _u2.branch_code;
            ViewBag.CurrentBranch2 = _u2.branch_code2;
            ViewBag.USER_LEVEL = _u2.user_level;
            ViewBag.USER_LEVEL2 = _u2.user_level2;
            ViewBag.CurrentSch = _u2.sch_time.ToString("dd/MM/yyyy");
            ViewBag.CurrentFr = _u2.start_date.ToString("dd/MM/yyyy");
            ViewBag.CurrentTo = _u2.end_date.ToString("dd/MM/yyyy");
            ViewBag.CurrentRole1 = "";
            ViewBag.CurrentRole2 = "";
            ViewBag.CurrentRole1 = _u2.role_name1;
            ViewBag.CurrentRole2 = _u2.role_name2;
            ViewBag.TransferSeleted = _u2.type;
            ViewBag.TransferId = id;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return View("NdEditUserTransfer", _u2);

        }

        [HttpPost]
        public ActionResult NdEditUserTrans(string cmdButton, string emp_code, string fname, string lname, string SchDate, string FrDate, string ToDate, string statusInput, string branchInput1, string branchInput2, string hub_name, string hub_code, string hub_name2, string hub_code2, string dept_name, string dept_code, string dept_name2, string dept_code2, string USER_LEVEL1, string USER_LEVEL2, string Role_Name1, string Role_Name2, string _trans, string transferId)
        {
            string strValue = string.Empty;
            string empcode = string.Empty;
            string strVariable = string.Empty;
            if (cmdButton.Contains("|"))
            {
                string[] arrStr = cmdButton.Split('|');
                strValue = Convert.ToString(arrStr[0]);
                empcode = Convert.ToString(arrStr[1]);
            }
            else
            {
                strValue = cmdButton;
            }

            UserTransferScheduleModel obj = new UserTransferScheduleModel();

            switch (strValue)
            {
                case "reset":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditUser");
                    return RedirectToAction("NdUserTransfer");
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ขั้นตอน Save User Transfer");
                    obj.emp_code = emp_code != "" ? emp_code : "";
                    obj.fname = fname != "" ? fname : "";
                    obj.lname = lname != "" ? lname : "";
                    if (SchDate != "") { obj.sch_time = DateTime.ParseExact(SchDate, "dd/MM/yyyy", null); }/*Convert.ToDateTime(SchDate);*/
                    if (FrDate != "") { obj.start_date = DateTime.ParseExact(FrDate, "dd/MM/yyyy", null); }/*Convert.ToDateTime(FrDate);*/
                    if (ToDate != "") { obj.end_date = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null); }/*Convert.ToDateTime(ToDate);*/
                    obj.branch_name = branchInput1 == "" ? "" : getMS(branchInput1, "", "branch");
                    obj.branch_code = branchInput1 == "" ? "" : branchInput1;
                    obj.dept_name = dept_name == "" ? "" : dept_name;
                    obj.dept_code = dept_code == "" ? "" : dept_code;
                    obj.hub_name = hub_name == "" ? "" : hub_name;
                    obj.hub_code = hub_code == "" ? "" : hub_code;
                    obj.user_level = Convert.ToInt32(USER_LEVEL1);
                    obj.branch_name2 = branchInput2 == "" ? "" : getMS(branchInput2, "", "branch"); ;
                    obj.branch_code2 = branchInput2 == "" ? "" : branchInput2;
                    obj.dept_name2 = dept_name2 == "" ? "" : dept_name2;
                    obj.dept_code2 = dept_code2 == "" ? "" : dept_code2;
                    obj.hub_name2 = hub_name2 == "" ? "" : hub_name2;
                    obj.hub_code2 = hub_code2 == "" ? "" : hub_code2;
                    obj.user_level2 = Convert.ToInt32(USER_LEVEL2);
                    obj.is_active = statusInput == "1" ? true : false;
                    obj.role_name1 = Role_Name1;
                    obj.role_name2 = Role_Name2;
                    obj.type = _trans == "" ? "" : _trans;
                    obj.Transfer_Id = Convert.ToInt64(transferId);
                    return RedirectToAction("NdUpdateUserTransfer", obj);
            }
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult NdGetBrandRelation(string brandcode)
        {
            SysBranch_RelationModel obj = AdministratorBAL.NdGetBrandRelation(brandcode);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        public ActionResult NdInsertUserTransfer(UserTransferScheduleModel obj)
        {
            bool _r = AdministratorBAL.NdInsertUserTransfer(obj);
            return RedirectToAction("NdUserTransfer");
        }

        public ActionResult NdUpdateUserTransfer(UserTransferScheduleModel obj)
        {
            bool _r = AdministratorBAL.NdUpdateUserTransfer(obj);
            return RedirectToAction("NdUserTransfer");
        }

        public ActionResult NdSearchUserTransfer(UserTransferScheduleModel obj)
        {
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            ViewBag.SystemBranch = LoadSystemBranch();
            ViewBag.SystemHub = LoadSystemHub();
            ViewBag.SystemDistrict = LoadSystemDistrict();
            ViewBag.SystemDepartment = LoadSystemDepartment();
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.StatusDDL = LoadStatusActive();
            ViewBag.RoleList = LoadRoleListNd();
            ViewBag.CurrentStatusDDL = "1";
            List<UserModels> list = AdministratorBAL.NdGetUserTransfer();
            List<UserTransferScheduleModel> list1 = AdministratorBAL.NdSearchUserTransfer(obj);
            GroupUserModel m = new GroupUserModel();
            m.lst_m1 = list;
            m.lst_m2 = list1;
            if (obj.sch_time != null && obj.sch_time.ToString("dd/MM/yyyy") != "01/01/0001")
            {
                ViewBag.CurrentSch = obj.sch_time.ToString("dd/MM/yyyy");
            }
            if (obj.start_date != null && obj.start_date.ToString("dd/MM/yyyy") != "01/01/0001")
            {
                ViewBag.CurrentFr = obj.start_date.ToString("dd/MM/yyyy");
            }
            if (obj.end_date != null && obj.end_date.ToString("dd/MM/yyyy") != "01/01/0001")
            {
                ViewBag.CurrentTo = obj.end_date.ToString("dd/MM/yyyy");
            }
            ViewBag.empcode = obj.emp_code;
            //ViewBag.CurrentSch = obj.sch_time.ToString("dd/MM/yyyy");
            //admVM.ToDate = (ToDate == null || ToDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ToDate;
            List<RoleModels> R03 = new List<RoleModels>();
            if (Session["R03"] != null)
            {
                R03 = (List<RoleModels>)Session["R03"];
                ViewBag.List_Role = R03[0].List_Role;
                ViewBag.View_Role = R03[0].View_Role;
                ViewBag.Update_Role = R03[0].Create_Role;
                ViewBag.Delete_Role = R03[0].Delete_Role;
                ViewBag.Create_Role = R03[0].Create_Role;
                ViewBag.Export_Role = R03[0].Export_Role;
            }
            return View("NdUserTransfer", m);
        }

        #endregion UserTransfer

        //Config
        #region Configuration
        public ActionResult NdMainUserAttrib()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า MainUserAttrib");
            List<UserAttribModel> list = AdministratorBAL.GetUserAttrib();
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            List<RoleModels> R05 = new List<RoleModels>();
            if (Session["R05"] != null)
            {
                R05 = (List<RoleModels>)Session["R05"];
                ViewBag.List_Role = R05[0].List_Role;
                ViewBag.View_Role = R05[0].View_Role;
                ViewBag.Update_Role = R05[0].Create_Role;
                ViewBag.Delete_Role = R05[0].Delete_Role;
                ViewBag.Create_Role = R05[0].Create_Role;
                ViewBag.Export_Role = R05[0].Export_Role;
            }
            return View("NdMainUserAttrib", list);

        }

        [HttpPost]
        public ActionResult NdUserAttrib(string cmdButton, List<UserAttribModel> model, string searchInput, string searchInputCat)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            short shtID = 0;
            string strVariable = string.Empty;
            if (arrStr.Length > 1)
            {
                shtID = Convert.ToInt16(arrStr[1]);
                strVariable = Convert.ToString(arrStr[2]);
            }
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddUserAttrib");
                    return RedirectToAction("NdAddUserAttrib");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditUserAttrib");
                    return RedirectToAction("NdGetUserAttribByIDAndVariable", new { ID = shtID, Variable = strVariable });
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล UserAttrib");
                    //var items = model.Where(m => m.IsSelected).ToList();
                    //foreach (var i in items)
                    //{
                    bool resultDelete = AdministratorBAL.DeleteUserAttribByIDAndVariable(shtID, strVariable);
                    //}
                    return RedirectToAction("NdMainUserAttrib");
                case "NO":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditUserAttrib");
                    return RedirectToAction("NdAddUserAttrib");
                case "Search":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditUserAttrib");
                    //UserAttribModel obj = AdministratorBAL.SearchUSER_ATTRIB(searchInput, searchInputCat);
                    List<UserAttribModel> list = AdministratorBAL.SearchUSER_ATTRIB2(searchInput, searchInputCat); //new List<UserAttribModel>();
                    //if (obj.Category != null && obj.SCFGVARIABLE != null)
                    //{
                    //    list.Add(obj);
                    //}
                    if (Session["GroupReport"] != null)
                    {
                        ViewBag.GroupReport = Session["GroupReport"];
                    }
                    List<RoleModels> R05 = new List<RoleModels>();
                    if (Session["R05"] != null)
                    {
                        R05 = (List<RoleModels>)Session["R05"];
                        ViewBag.List_Role = R05[0].List_Role;
                        ViewBag.View_Role = R05[0].View_Role;
                        ViewBag.Update_Role = R05[0].Create_Role;
                        ViewBag.Delete_Role = R05[0].Delete_Role;
                        ViewBag.Create_Role = R05[0].Create_Role;
                        ViewBag.Export_Role = R05[0].Export_Role;
                    }
                    return View("NdMainUserAttrib", list);
                case "Reset":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditUserAttrib");
                    return RedirectToAction("NdMainUserAttrib");

            }
            return null;
        }

        public ActionResult NdGetUserAttribByIDAndVariable(short ID, string Variable)
        {
            UserAttribModel obj = AdministratorBAL.GetUserAttribByIDAndVariable(ID, Variable);
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return View("NdEditUserAttrib", obj);
        }


        [HttpPost]
        public ActionResult NdAddUserAttrib(string cmdButton, UserAttribModel obj)
        {

            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddUserAttrib");
                    bool resultSave = AdministratorBAL.AddUserAttrib(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddUserAttrib");
                    return RedirectToAction("NdMainUserAttrib");
            }
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return RedirectToAction("NdMainUserAttrib");
        }
        public ActionResult NdAddUserAttrib()
        {
            UserAttribModel obj = new UserAttribModel();
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return View("NdAddUserAttrib", obj);

        }

        [HttpPost]
        public ActionResult NdEditUserAttrib(string cmdButton, UserAttribModel obj)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditUserAttrib");
                    bool result = AdministratorBAL.EditUserAttrib(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditUserAttrib");
                    return RedirectToAction("NdMainUserAttrib");

            }
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            return RedirectToAction("NdMainUserAttrib");
        }
        #endregion Configuration

        //Audi Log
        #region Audi Log
        public ActionResult NdAudiLog()
        {
            ViewBag.SystemBranch = LoadSystemBranch();
            ViewBag.SystemHub = LoadSystemHub();
            ViewBag.SystemDistrict = LoadSystemDistrict();
            ViewBag.SystemDepartment = LoadSystemDepartment();
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.WorkFunction = LoadWorkFunction();
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            //ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า Audi Log");
            if (Session["AudiLog"] != null)
            {
                AudiLogModel audilog = (AudiLogModel)Session["AudiLog"];
                audilog.WorkFunction = "ไปที่หน้า Audi Log";
                audilog.Message = "ไปที่หน้า Audi Log";
                if (Session["latitude"] != null && Session["longitude"] != null)
                {
                    audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                    audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                }
                ProcessLogBAL.AddProcessLog2(audilog);
            }
            List<AudiLogModel> listAudi = AdministratorBAL.GetAudiLogDetails1();
            List<RoleModels> R04 = new List<RoleModels>();
            if (Session["R04"] != null)
            {
                R04 = (List<RoleModels>)Session["R04"];
                ViewBag.List_Role = R04[0].List_Role;
                ViewBag.View_Role = R04[0].View_Role;
                ViewBag.Update_Role = R04[0].Create_Role;
                ViewBag.Delete_Role = R04[0].Delete_Role;
                ViewBag.Create_Role = R04[0].Create_Role;
                ViewBag.Export_Role = R04[0].Export_Role;
            }
            return View("NdAudiLog", listAudi);

        }

        public ActionResult NdAudiLogSearch(string cmdButton, AudiLogModel model, string workfunction, string FrDate, string ToDate, string empcode, string nameinput)
        {
            ViewBag.WorkFunction = LoadWorkFunction();
            _logSys.WriteProcessLogFile(_strPathFile, "Begin AuditLog Search");
            model.Audi_Id = 0;
            _logSys.WriteProcessLogFile(_strPathFile, "model.Audi_Id : 0");
            model.EmpCode = empcode != null ? empcode : "";
            _logSys.WriteProcessLogFile(_strPathFile, "model.EmpCode : " + model.EmpCode);
            model.Username = nameinput != null ? nameinput : "";
            _logSys.WriteProcessLogFile(_strPathFile, "model.Username : " + model.Username);
            model.WorkFunction = workfunction != null ? workfunction : "";
            _logSys.WriteProcessLogFile(_strPathFile, "model.WorkFunction : " + model.WorkFunction);
            _logSys.WriteProcessLogFile(_strPathFile, "model.FrDate : " + FrDate);
            _logSys.WriteProcessLogFile(_strPathFile, "model.ToDate : " + ToDate);
            if (FrDate != null && FrDate != "")
            {
                var arrFrdate2 = FrDate.Split('/');
                model.frdate2 = arrFrdate2[2] + "-" + arrFrdate2[1] + "-" + arrFrdate2[0];
            }
            else
            {
                model.frdate2 = "";
            }
            if (ToDate != null && ToDate != "")
            {
                var arrTodate2 = ToDate.Split('/');
                model.todate2 = arrTodate2[2] + "-" + arrTodate2[1] + "-" + arrTodate2[0];
            }
            else
            {
                model.todate2 = "";
            }
            _logSys.WriteProcessLogFile(_strPathFile, "model.FrDate : " + model.frdate2);
            _logSys.WriteProcessLogFile(_strPathFile, "model.ToDate : " + model.todate2);
            //model.FrDate = (FrDate != null && FrDate != "") ? Convert.ToDateTime(FrDate) : Convert.ToDateTime("0001-01-01");
            //model.ToDate = (ToDate != null && ToDate != "") ? Convert.ToDateTime(ToDate) : Convert.ToDateTime("0001-01-01");

            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            switch (cmdButton)
            {
                case "Search":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า Audi Log");
                    _logSys.WriteProcessLogFile(_strPathFile, "Search : Begin ");
                    List<AudiLogModel> listAudi = AdministratorBAL.GetAudiLogDetails(model);
                    _logSys.WriteProcessLogFile(_strPathFile, "Search result : " + listAudi.Count);
                    ViewBag.CurrentWorkFunction = workfunction;
                    ViewBag.EmpCode = empcode;
                    ViewBag.NameInput = nameinput;
                    return View("NdAudiLog", listAudi);
                case "ExportExcel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Export File");
                    return RedirectToAction("audiExportExcel");
                case "ExportCSV":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Export File");
                    return RedirectToAction("audiExportCSV");
                case "reset":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Export File");
                    return RedirectToAction("NdAudiLog");
            }

            //model.WorkFunction = workfunction != null ? workfunction : "";
            return View("NdAudiLog", model);
        }


        public ActionResult NdAudiDetails(string cmdButton, AudiLogModel model, long id, string EmpCode, string Username, string WorkFunction, string Details01, string Created, string MAC_Address)
        {
            model.Audi_Id = id;
            model.EmpCode = EmpCode != null ? EmpCode : "";
            model.Username = Username != null ? Username : "";
            model.WorkFunction = WorkFunction != null ? WorkFunction : "";
            //model.Created = DateTime.Parse(Created != null ? Created : "");
            //model.Updated = DateTime.Parse(Created != null ? Created : "");
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            switch (cmdButton)
            {
                case "Detail":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddUserAttrib");
                    //List<AudiLogModel> listAudi = AdministratorBAL.GetAudiLogDetails(model);
                    return RedirectToAction("NdAudiLogDetails", model);
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddUserAttrib");
                    return RedirectToAction("NdAudiLog");
            }

            return RedirectToAction("NdAudiLog");

        }

        public ActionResult NdAudiDetails1(string cmdButton)
        {
            switch (cmdButton)
            {

                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddUserAttrib");
                    return RedirectToAction("NdAudiLog");
            }
            return RedirectToAction("NdAudiLog");

        }

        public ActionResult NdAudiLogDetails(AudiLogModel model)
        {

            //UserAttribModel obj = AdministratorBAL.GetUserAttribByIDAndVariable(ID, "");

            AudiLogModel Audi = AdministratorBAL.GetAudiLogDetails2(model);
            ViewBag.WorkFunction = Audi.WorkFunction;
            ViewBag.Details01 = Audi.Details01;
            ViewBag.Details02 = Audi.Details02;
            ViewBag.Created = Audi.Created;
            ViewBag.EmpCode = Audi.EmpCode;
            ViewBag.EmpName = Audi.EmpName;

            ViewBag.OS = Audi.OS;
            ViewBag.URL = Audi.URL;
            ViewBag.Device = Audi.Device;
            ViewBag.IP_Address = Audi.IP_Address;
            ViewBag.Latitude = Audi.Latitude;
            ViewBag.Longitude = Audi.Longitude;
            string formattedMac = Regex.Replace(Audi.MAC_Address, "(.{2})", "$1:").TrimEnd(':');
            ViewBag.MAC_Address = formattedMac;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }

            return View("NdAudiLogDetails", Audi);

        }

        #endregion Audo Log

        #endregion RoleManagement
        ////////////////////////////////////////////////////////////////////////*****

        public ActionResult SearchBranch(string branchName)
        {

            var data = from m in AdministratorBAL.GetBranch()
                       select m;

            if (!string.IsNullOrEmpty(branchName))
            {
                data = data.Where(s => s.DESC.Contains(branchName));
            }



            return View("Branch", data.ToList());

        }
        public ActionResult Branch()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า Branch");
            List<BranchModel> list = AdministratorBAL.GetBranch();

            return View("Branch", list);

        }
        [HttpPost]
        public ActionResult Branch(string cmdButton, List<BranchModel> model)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            string strID = string.Empty;
            if (arrStr.Length > 1)
                strID = Convert.ToString(arrStr[1]);
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddBranch");
                    return RedirectToAction("AddBranch");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditBranch");
                    return RedirectToAction("EditBranch", new { BRANCH = strID });
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล Branch");
                    var items = model.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteBranchByID(i.BRANCH);
                    }
                    return RedirectToAction("Branch");
            }

            return null;
        }
        public ActionResult AddBranch()
        {
            ViewBag.GenBranchNo = AdministratorBAL.GenBranchNo();
            return View("AddBranch");

        }
        [HttpPost]
        public ActionResult AddBranch(string cmdButton, string BRANCH, string DESC, string DESC_ENG, string ZONE, string REGION, string EMAIL,
            string EXCEPTREGION, string BNREGION, string FLAGCLOSE, string Alt1, string Alt2, string Alt3)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddBranch");
                    BranchModel obj = new BranchModel();
                    obj.BRANCH = BRANCH;
                    obj.DESC = DESC;
                    obj.DESC_ENG = DESC_ENG;
                    obj.ZONE = ZONE;
                    obj.REGION = REGION;
                    obj.EMAIL = EMAIL;
                    obj.EXCEPTREGION = EXCEPTREGION;
                    obj.BNREGION = BNREGION;
                    obj.FLAGCLOSE = FLAGCLOSE;
                    obj.Alt1 = Alt1;
                    obj.Alt2 = Alt2;
                    obj.Alt3 = Alt3;
                    bool resultSave = AdministratorBAL.AddBranch(obj);

                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddBranch");
                    return RedirectToAction("Branch");
            }
            return RedirectToAction("Branch");
        }
        [HttpPost]
        public ActionResult EditBranch(string cmdButton, string BRANCH, string DESC, string DESC_ENG, string ZONE, string REGION, string EMAIL,
            string EXCEPTREGION, string BNREGION, string FLAGCLOSE, string Alt1, string Alt2, string Alt3)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditBranch");
                    BranchModel obj = new BranchModel();
                    obj.BRANCH = BRANCH;
                    obj.DESC = DESC;
                    obj.DESC_ENG = DESC_ENG;
                    obj.ZONE = ZONE;
                    obj.REGION = REGION;
                    obj.EMAIL = EMAIL;
                    obj.EXCEPTREGION = EXCEPTREGION;
                    obj.BNREGION = BNREGION;
                    obj.FLAGCLOSE = FLAGCLOSE;
                    obj.Alt1 = Alt1;
                    obj.Alt2 = Alt2;
                    obj.Alt3 = Alt3;
                    bool result = AdministratorBAL.EditBranch(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditBranch");
                    return RedirectToAction("Branch");

            }
            return RedirectToAction("Branch");
        }
        public ActionResult EditBranch(string BRANCH)
        {

            BranchModel obj = AdministratorBAL.GetBranchByID(BRANCH);
            //[BRANCH]
            ViewBag.BRANCH = obj.BRANCH;
            //,[DESC]
            ViewBag.DESC = obj.DESC;
            //,[DESC_ENG]
            ViewBag.DESC_ENG = obj.DESC_ENG;
            //,[ZONE]
            ViewBag.ZONE = obj.ZONE;
            //,[REGION]
            ViewBag.REGION = obj.REGION;
            //,[EMAIL]
            ViewBag.EMAIL = obj.EMAIL;
            //,[EXCEPTREGION]
            ViewBag.EXCEPTREGION = obj.EXCEPTREGION;
            //,[BNREGION]
            ViewBag.BNREGION = obj.BNREGION;
            //,[FLAGCLOSE]
            ViewBag.FLAGCLOSE = obj.FLAGCLOSE;
            //,[Alt1]
            ViewBag.Alt1 = obj.Alt1;
            //,[Alt2]
            ViewBag.Alt2 = obj.Alt2;
            //,[Alt3]
            ViewBag.Alt3 = obj.Alt3;
            return View("EditBranch");

        }
        [HttpGet]
        public ActionResult SearchGroupDetailReport(string reportName)
        {

            var data = from m in AdministratorBAL.GetGroupDetailReport()
                       select m;

            if (!string.IsNullOrEmpty(reportName))
            {
                data = data.Where(s => s.REPORT_NAME.Contains(reportName));
            }



            return View("GroupDetailReport", data.ToList());

        }
        public ActionResult GroupDetailReport()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า GroupDetailReport");
            List<GroupDetailReportModel> list = AdministratorBAL.GetGroupDetailReport();

            return View("GroupDetailReport", list);

        }
        [HttpPost]
        public ActionResult GroupDetailReport(string cmdButton, List<GroupDetailReportModel> model)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            int intID = 0;
            if (arrStr.Length > 1)
                intID = Convert.ToInt32(arrStr[1]);
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddGroupDetailReport");
                    return RedirectToAction("AddGroupDetailReport");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditGroupDetailReport");
                    return RedirectToAction("EditGroupDetailReport", new { ID = intID });
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล GroupDetailReport");
                    var items = model.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteGroupDetailReportByID(i.ID_REPORT);
                    }
                    return RedirectToAction("GroupDetailReport");

            }

            return null;
        }
        public ActionResult AddGroupDetailReport()
        {

            ViewBag.CurrentGroupReportList = LoadMasterGroupReport();
            return View("AddGroupDetailReport");

        }
        [HttpPost]
        public ActionResult AddGroupDetailReport(string cmdButton, decimal? GroupNo, decimal? ReportNo, string ReportName, bool? Active)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddGroupDetailReport");
                    GroupDetailReportModel obj = new GroupDetailReportModel();
                    obj.GROUP_NO = GroupNo;
                    obj.REPORT_NO = ReportNo;
                    obj.REPORT_NAME = ReportName;
                    obj.ACTIVE = Active;
                    bool resultSave = AdministratorBAL.AddGroupDetailReport(obj);
                    return RedirectToAction("GroupDetailReport");
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddGroupDetailReport");
                    return RedirectToAction("GroupDetailReport");
                case null:
                    ViewBag.CurrentGroupReportList = LoadMasterGroupReport();
                    ViewBag.GroupNo = Convert.ToString(GroupNo);
                    string strGenerateReportNo = string.Empty;
                    if (GroupNo != null)
                    {
                        strGenerateReportNo = Convert.ToString(AdministratorBAL.GenerateReportNo(GroupNo));
                        if (strGenerateReportNo != "0")
                        {
                            ViewBag.GenerateReportNo = strGenerateReportNo;
                        }
                        else
                        {
                            ViewBag.GenerateReportNo = Convert.ToString(GroupNo) + "00";
                        }

                    }
                    return View("AddGroupDetailReport");

            }
            return null;
        }
        public ActionResult EditGroupDetailReport(int ID)
        {

            ViewBag.CurrentGroupReportList = LoadMasterGroupReport();
            GroupDetailReportModel obj = AdministratorBAL.GetGroupDetailReportByID(ID);
            ViewBag.ID = obj.ID_REPORT;
            ViewBag.GroupNo = Convert.ToString(obj.GROUP_NO);
            ViewBag.ReportNo = obj.REPORT_NO;
            ViewBag.ReportName = obj.REPORT_NAME;
            ViewBag.Active = obj.ACTIVE;

            return View("EditGroupDetailReport");

        }
        [HttpPost]
        public ActionResult EditGroupDetailReport(string cmdButton, int ID, decimal? GroupNo, decimal? ReportNo, string ReportName, bool? Active)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditGroupDetailReport");
                    GroupDetailReportModel obj = new GroupDetailReportModel();
                    obj.ID_REPORT = ID;
                    obj.GROUP_NO = GroupNo;
                    obj.REPORT_NO = ReportNo;
                    obj.REPORT_NAME = ReportName;
                    obj.ACTIVE = Active;
                    bool resultSave = AdministratorBAL.EditGroupDetailReport(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditGroupDetailReport");
                    return RedirectToAction("GroupDetailReport");

            }
            return RedirectToAction("GroupDetailReport");
        }
        [HttpGet]
        public ActionResult SearchUser(string userName)
        {

            var data = from m in AdministratorBAL.GetUser()
                       select m;

            if (!string.IsNullOrEmpty(userName))
            {
                data = data.Where(s => s.USER_LOGIN.Contains(userName));
            }



            return View("MainUser", data.ToList());

        }
        public ActionResult MainUser()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า MainUser");
            List<UserModels> list = AdministratorBAL.GetUser();

            return View("MainUser", list);

        }
        public ActionResult AddUserLDAP(int USER_LEVEL)
        {
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.USER_LEVEL = USER_LEVEL;
            return View("AddUserLDAP");

        }
        [HttpPost]
        public ActionResult AddUserLDAP1(string cmdButton, string USER_LOGIN, string USER_FIRSTNAME, string USER_LASTNAME, string USER_EMAIL, int USER_LEVEL)
        {
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.USER_LEVEL = USER_LEVEL;

            switch (cmdButton)
            {
                case "VerifyUser":
                    UserModels obj = new UserModels();
                    try
                    {
                        if (USER_LOGIN == "")
                        {
                            ModelState.AddModelError("", "โปรดระบุชื่อผู้ใช้");
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LEVEL : โปรดระบุชื่อผู้ใช้");
                        }
                        else
                        {
                            obj = _authService.GetUserByUserName(USER_LOGIN);
                            var value = obj ?? null;
                            if (value is null)
                            {
                                ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName(object) : null");

                            }
                            else if (obj.USER_LOGIN == "")
                            {
                                ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                                _logSys.WriteProcessLogFile(_strPathFile, "result fn_GetUserByUserName(ไม่พบชื่อนี้ในระบบ ldap) : Empty");
                            }
                            else
                            {
                                var uerDetails = obj.USER_FIRSTNAME.ToString().Split(_spd);
                                ViewBag.USER_FIRSTNAME = uerDetails[0];
                                ViewBag.USER_LASTNAME = uerDetails[1];
                                ViewBag.USER_EMAIL = obj.USER_EMAIL;
                                ViewBag.USER_LEVEL = USER_LEVEL;
                                ViewBag.USER_LOGIN = obj.USER_LOGIN;
                                ViewBag.Flag = true;
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_FIRSTNAME : " + obj.USER_FIRSTNAME);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LASTNAME : " + obj.USER_LASTNAME);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_EMAIL : " + obj.USER_EMAIL);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LOGIN : " + obj.USER_LOGIN);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LEVEL : " + USER_LEVEL);
                            }
                        }

                        return View("AddUserLDAP");
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
                    bool resultSave = AdministratorBAL.AddUserLDAP(obj2);
                    if (resultSave == false)
                    {
                        ViewBag.USER_LOGIN = USER_LOGIN;
                        ModelState.AddModelError("", "ชื่อผู้ใช้ได้มีการเพิ่มแล้ว");
                        _logSys.WriteProcessLogFile(_strPathFile, "resultSave(ชื่อผู้ใช้ได้มีการเพิ่มแล้ว) : false");
                        return View("AddUserLDAP");
                    }
                    else
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "resultSave(เพิ่มผู้ใช้ได้) :  true");
                        return RedirectToAction("MainUser");
                    }
                case "Cancel":
                    return RedirectToAction("MainUser");

            }

            return View("AddUserLDAP");

        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddUserLDAPAd1(string cmdButton, string USER_LOGIN, string USER_FIRSTNAME, string USER_LASTNAME, string USER_EMAIL, int USER_LEVEL, string USER_EMP_CODE, string SOL_CODE, string ZONE_NAME, string REMARK)
        {
            _logSys.WriteProcessLogFile(_strPathFile, "Begin AddUserLDAPAd1");
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.USER_LEVEL = USER_LEVEL;
            UserPostRequest userReq = new UserPostRequest();
            ApiPostResponse2 objApi = new ApiPostResponse2();
            ApiPostResponse2 Session_ObjApi = new ApiPostResponse2();
            DataTable _dt = new DataTable();
            //userReq.username = USER_LOGIN;
            //objApi.emp_code = USER_EMP_CODE;
            //userReq.password = USER_PASSWORD;
            int maxLength = 3;
            _logSys.WriteProcessLogFile(_strPathFile, "cmdButton(Row:1082): " + cmdButton);
            switch (cmdButton)
            {
                case "VerifyUser":
                    UserModels obj = new UserModels();//not use in this fn

                    try
                    {
                        if (USER_EMP_CODE == "")
                        {
                            ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                            ModelState.AddModelError("", "โปรดระบุ รหัสพนักงาน");
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LOGIN&USER_PASSWORD : โปรดระบุชื่อผู้ใช้ และ รหัสบัญชี Ldap");
                        }
                        else
                        {
                            objApi = _authService.GetUserByUserNameV3(userReq, USER_EMP_CODE);
                            if (objApi.emp_code != "")
                            {
                                Session["TempDataLdapUser"] = objApi;
                            }
                            _logSys.WriteProcessLogFile(_strPathFile, "_authService.GetUserByUserName3(userReq) : " + objApi.fname + " " + objApi.lname);
                            //var value = objApi ?? null;
                            if (objApi.fname == null && objApi.lname == null)
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
                                ViewBag.SOL_CODE = objApi.sol_code == "" ? "" : cutSolCode(objApi.sol_code, maxLength);// objApi.sol_code;
                                ViewBag.USER_EMP_CODE = objApi.emp_code;
                                ViewBag.USER_FIRSTNAME = objApi.fname;
                                ViewBag.USER_LASTNAME = objApi.lname;
                                ViewBag.USER_EMAIL = objApi.email;
                                ViewBag.USER_LEVEL = objApi.sol_code != "" ? 2 : USER_LEVEL;
                                ViewBag.ZONE_NAME = objApi.zone_name;
                                ViewBag.DEPT_NAME = objApi.dept_name;
                                if (objApi.email != null && objApi.email != "")
                                {
                                    var userLogin = objApi.email.ToString().Split(_spd2);
                                    ViewBag.USER_LOGIN = userLogin[0];
                                }
                                else
                                {
                                    ViewBag.USER_LOGIN = "-";
                                }

                                ViewBag.Flag = true;
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_FIRSTNAME : " + objApi.fname);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LASTNAME : " + objApi.lname);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_EMAIL : " + objApi.email);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LOGIN : " + ViewBag.USER_LOGIN);
                                _logSys.WriteProcessLogFile(_strPathFile, "V.USER_LEVEL : " + USER_LEVEL);
                            }
                        }

                        return View("AddUserLDAP");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "ไม่พบชื่อนี้ในระบบ ldap");
                        ex.Message.ToString();
                        _logSys.WriteProcessLogFile(_strPathFile, "Error VerifyUser : " + ex.Message.ToString());
                    }
                    break;
                case "Save":
                    if (USER_LEVEL == 0)
                    {
                        ModelState.AddModelError("", "โปรดระบุ ระดับ");
                        _logSys.WriteProcessLogFile(_strPathFile, "USER_LEVEL : โปรดระบุ ระดับ");
                        ViewBag.SOL_CODE = SOL_CODE;
                        ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                        ViewBag.USER_FIRSTNAME = USER_FIRSTNAME;
                        ViewBag.USER_LASTNAME = USER_LASTNAME;
                        ViewBag.USER_EMAIL = USER_EMAIL;
                        ViewBag.USER_LEVEL = USER_LEVEL;
                        ViewBag.USER_LOGIN = USER_LOGIN;
                        ViewBag.ZONE_NAME = objApi.zone_name;
                        ViewBag.Flag = true;
                    }
                    else
                    {
                        if (Session["TempDataLdapUser"] != null)
                        {

                            Session_ObjApi = (ApiPostResponse2)Session["TempDataLdapUser"];
                            _dt = _ds.Tables["Officer_Details"];
                            DataRow _dr = _dt.NewRow();
                            _dr["emp_code"] = Session_ObjApi.emp_code;
                            _dr["fname"] = Session_ObjApi.fname;
                            _dr["lname"] = Session_ObjApi.lname;
                            _dr["id_card"] = Session_ObjApi.id_card;
                            _dr["birth_date"] = Session_ObjApi.birth_date;
                            _dr["grade"] = Session_ObjApi.grade;
                            _dr["emp_type"] = Session_ObjApi.emp_type;
                            _dr["position_id"] = Session_ObjApi.position_id;
                            _dr["position_name"] = Session_ObjApi.position_name;
                            _dr["email"] = Session_ObjApi.email;
                            _dr["tel"] = Session_ObjApi.tel;
                            _dr["org_level"] = Session_ObjApi.org_level;
                            _dr["comp_objid"] = Session_ObjApi.comp_objid;
                            _dr["comp_code"] = Session_ObjApi.comp_code;
                            _dr["comp_short_name"] = Session_ObjApi.comp_short_name;
                            _dr["comp_name"] = Session_ObjApi.comp_name;
                            _dr["group_objid"] = Session_ObjApi.group_objid;
                            _dr["group_code"] = Session_ObjApi.group_code;
                            _dr["group_short_name"] = Session_ObjApi.group_short_name;
                            _dr["group_name"] = Session_ObjApi.group_name;
                            _dr["field_objid"] = Session_ObjApi.field_objid;
                            _dr["field_code"] = Session_ObjApi.field_code;
                            _dr["field_short_name"] = Session_ObjApi.field_short_name;
                            _dr["field_name"] = Session_ObjApi.field_name;
                            _dr["dept_objid"] = Session_ObjApi.dept_objid;
                            _dr["dept_code"] = Session_ObjApi.dept_code;
                            _dr["dept_short_name"] = Session_ObjApi.dept_short_name;
                            _dr["dept_name"] = Session_ObjApi.dept_name;
                            _dr["center_objid"] = Session_ObjApi.center_objid;
                            _dr["center_code"] = Session_ObjApi.center_code;
                            _dr["center_short_name"] = Session_ObjApi.center_short_name;
                            _dr["center_name"] = Session_ObjApi.center_name;
                            _dr["zone_objid"] = Session_ObjApi.zone_objid;
                            _dr["zone_code"] = Session_ObjApi.zone_code;
                            _dr["zone_short_name"] = Session_ObjApi.zone_short_name;
                            _dr["zone_name"] = Session_ObjApi.zone_name;
                            _dr["division_objid"] = Session_ObjApi.division_objid;
                            _dr["division_code"] = Session_ObjApi.division_code;
                            _dr["division_short_name"] = Session_ObjApi.division_short_name;
                            _dr["division_name"] = Session_ObjApi.division_name;
                            _dr["sol_objid"] = Session_ObjApi.sol_objid;
                            _dr["sol_code"] = Session_ObjApi.sol_code;
                            _dr["sol_short_name"] = Session_ObjApi.sol_short_name;
                            _dr["sol_name"] = Session_ObjApi.sol_name;
                            _dr["sub_sol_objid"] = Session_ObjApi.sub_sol_objid;
                            _dr["sub_sol_code"] = Session_ObjApi.sub_sol_code;
                            _dr["sub_sol_short_name"] = Session_ObjApi.sub_sol_short_name;
                            _dr["sub_sol_name"] = Session_ObjApi.sub_sol_name;
                            _dr["start_date"] = DateTime.Now;// DateTime.ParseExact(Session_ObjApi.start_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                            _dr["stop_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.stop_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                            _dr["last_action"] = Session_ObjApi.last_action;
                            _dr["sol_instead_code"] = Session_ObjApi.sol_instead_code;
                            _dr["sol_instead_name"] = Session_ObjApi.sol_instead_name;
                            _dr["sol_hq"] = Session_ObjApi.sol_hq;
                            _dr["plans_000"] = Session_ObjApi.plans_000;
                            _dr["plansname_000"] = Session_ObjApi.plansname_000;
                            _dr["org_000"] = Session_ObjApi.org_000;
                            _dr["solid_000"] = Session_ObjApi.solid_000;
                            _dr["orgname_000"] = Session_ObjApi.orgname_000;
                            _dr["org_748"] = Session_ObjApi.org_748;
                            _dr["solid_748"] = Session_ObjApi.solid_748;
                            _dr["orgname_748"] = Session_ObjApi.orgname_748;
                            _dr["create_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.create_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                            _dr["update_date"] = DateTime.Now;//DateTime.ParseExact(Session_ObjApi.update_date.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));

                            _dt.Rows.Add(_dr);
                        }
                        UserModels obj2 = new UserModels();
                        //var userLogin2 = USER_LOGIN.Split(_spd);
                        obj2.USER_EMP_CODE = USER_EMP_CODE;
                        obj2.USER_LOGIN = USER_LOGIN;
                        obj2.USER_FIRSTNAME = USER_FIRSTNAME;
                        obj2.USER_LASTNAME = USER_LASTNAME;
                        obj2.USER_EMAIL = USER_EMAIL;
                        obj2.USER_LEVEL = Convert.ToInt32(USER_LEVEL);
                        //USER_LEVEL 0 Not role, 1 Admin, 2 Role Branch, 3 Role Head office 4 Admin1, 5 Amin2;
                        obj2.USER_FLAG = true;//true = AD Account
                        obj2.USER_EMP_CODE = USER_EMP_CODE;
                        obj2.SOL_CODE = SOL_CODE == "" ? "000" : SOL_CODE;// cutSolCode(SOL_CODE, maxLength);
                        objApi.zone_name = ZONE_NAME;
                        obj2.IS_ACTIVE = true;
                        obj2.Remark = REMARK;
                        _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LOGIN : " + obj2.USER_LOGIN);
                        _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_FIRSTNAME : " + obj2.USER_FIRSTNAME);
                        _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LASTNAME : " + obj2.USER_LASTNAME);
                        _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMAIL : " + obj2.USER_EMAIL);
                        _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_LEVEL : " + obj2.USER_LEVEL);
                        _logSys.WriteProcessLogFile(_strPathFile, "obj2.USER_EMP_CODE : " + obj2.USER_EMP_CODE);
                        bool resultSave = false;// AdministratorBAL.AddUserLDAPV3(obj2);
                        //Table name = SYSTEMOFFICER_Details
                        bool resultSave2 = AdministratorBAL.InsertDataFromAPI(Session_ObjApi, _dt, obj2, "SYSTEMOFFICER_Details");
                        if (resultSave == false)
                        {
                            ViewBag.USER_EMP_CODE = USER_EMP_CODE;
                            ViewBag.USER_LEVEL = 0;
                            ModelState.AddModelError("", "ชื่อผู้ใช้ได้มีการเพิ่มแล้ว");
                            _logSys.WriteProcessLogFile(_strPathFile, "resultSave(ชื่อผู้ใช้ได้มีการเพิ่มแล้ว) : false");
                            return View("NdAddUserLDAP");
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "resultSave(เพิ่มผู้ใช้ได้) :  true");
                            return RedirectToAction("NdUserManagement");
                        }
                    }
                    break;
                case "Cancel":
                    return RedirectToAction("MainUser");

            }

            return View("AddUserLDAP");

        }


        [HttpPost]
        public ActionResult ImportFile(string cmdButton, UploadViewModel model)
        {
            switch (cmdButton)
            {
                case "UploadFile":
                    if (model.File != null && model.File.ContentLength > 0)
                    {
                        var users = new List<UserModels>();
                        using (var reader = new StreamReader(model.File.InputStream))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                char delimiter = Convert.ToChar(WebConfigurationManager.AppSettings["Delimiter"]);
                                var values = line.Split(delimiter);
                                var user = new UserModels
                                {
                                    USER_LOGIN = values[0],
                                    USER_FIRSTNAME = values[1],
                                    USER_LASTNAME = values[2],
                                    USER_EMAIL = values[3]
                                };
                                users.Add(user);
                            }
                        }

                        foreach (UserModels user in users)
                        {
                            int intCount = AdministratorBAL.GetCountUserByUserName(user.USER_LOGIN);
                            if (intCount == 0)
                            {
                                user.USER_FLAG = true;
                                bool resultSave = AdministratorBAL.AddUserLDAP(user);
                            }
                        }

                        //return View("MainUser", users);
                        return RedirectToAction("MainUser");
                    }

                    return View();
                case "Cancel":
                    return RedirectToAction("MainUser");
            }
            return null;
        }
        public ActionResult ImportFile()
        {


            return View("ImportFile");

        }

        #region export

        public ActionResult ExportExcelUser()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //List<UserModels> users1 = AdministratorBAL.GetUserProfile();
            List<UserModels> users = AdministratorBAL.GetUser();
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "First Name";
                worksheet.Cells[1, 2].Value = "Last Name";
                worksheet.Cells[1, 3].Value = "Role";
                worksheet.Cells[1, 4].Value = "Group Report";
                worksheet.Cells[1, 5].Value = "User Login";
                worksheet.Cells[1, 6].Value = "Active";

                worksheet.Cells["A1:F1"].AutoFitColumns();

                // Add more than 10 rows of data
                int j = 1;
                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cells[j + 1, 1].Value = $"{users[i].USER_FIRSTNAME}";
                    worksheet.Cells[j + 1, 2].Value = $"{users[i].USER_LASTNAME}";
                    worksheet.Cells[j + 1, 3].Value = $"{users[i].User_Role_Level}";
                    worksheet.Cells[j + 1, 4].Value = $"{users[i].User_Branch}";
                    worksheet.Cells[j + 1, 5].Value = $"{users[i].USER_LOGIN}";
                    worksheet.Cells[j + 1, 6].Value = $"{users[i].IS_ACTIVE}";
                    j++;
                }

                // Convert to byte array
                byte[] fileBytes = package.GetAsByteArray();

                // Return the file with an appropriate MIME type for Excel
                return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "user_profiles_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx"
                };
            }
        }

        public ActionResult ExportExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<UserModels> users = AdministratorBAL.GetUserProfile();
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "First Name";
                worksheet.Cells[1, 2].Value = "Last Name";
                worksheet.Cells[1, 3].Value = "Role";
                worksheet.Cells[1, 4].Value = "Group Report";
                worksheet.Cells[1, 5].Value = "User Login";
                worksheet.Cells[1, 6].Value = "Active";

                worksheet.Cells["A1:F1"].AutoFitColumns();

                // Add more than 10 rows of data
                int j = 1;
                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cells[j + 1, 1].Value = $"{users[i].USER_FIRSTNAME}";
                    worksheet.Cells[j + 1, 2].Value = $"{users[i].USER_LASTNAME}";
                    worksheet.Cells[j + 1, 3].Value = $"{users[i].User_Role_Level}";
                    worksheet.Cells[j + 1, 4].Value = $"{users[i].User_Branch}";
                    worksheet.Cells[j + 1, 5].Value = $"{users[i].USER_LOGIN}";
                    worksheet.Cells[j + 1, 6].Value = $"{users[i].IS_ACTIVE}";
                    j++;
                }

                // Convert to byte array
                byte[] fileBytes = package.GetAsByteArray();

                // Return the file with an appropriate MIME type for Excel
                return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "user_profiles_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx"
                };
            }
        }

        public ActionResult ExportCSVUser()
        {
            List<UserModels> users = AdministratorBAL.GetUser();

            var delimiter = WebConfigurationManager.AppSettings["Delimiter"].ToString();
            var csv = new StringBuilder();
            csv.AppendLine($"UserName{delimiter}FirstName{delimiter}LastName{delimiter}Email");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.USER_LOGIN}{delimiter}{user.USER_FIRSTNAME}{delimiter}{user.USER_LASTNAME}{delimiter}{user.USER_EMAIL}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var output = new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = "usersprofiles_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".csv"
            };

            return output;
        }

        public ActionResult ExportCSV()
        {
            List<UserModels> users = AdministratorBAL.GetUser();

            var delimiter = WebConfigurationManager.AppSettings["Delimiter"].ToString();
            var csv = new StringBuilder();
            csv.AppendLine($"UserName{delimiter}FirstName{delimiter}LastName{delimiter}Email");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.USER_LOGIN}{delimiter}{user.USER_FIRSTNAME}{delimiter}{user.USER_LASTNAME}{delimiter}{user.USER_EMAIL}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var output = new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = "users.csv"
            };

            return output;
        }


        public ActionResult audiExportExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //List<UserModels> users = AdministratorBAL.GetUserProfile();

            List<AudiLogModel> listAudi = AdministratorBAL.GetAudiLogDetails1();
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "Audi_Id";
                worksheet.Cells[1, 2].Value = "Username";
                worksheet.Cells[1, 3].Value = "Message";
                worksheet.Cells[1, 4].Value = "Created";
                worksheet.Cells[1, 5].Value = "Updated";
                worksheet.Cells[1, 6].Value = "WorkFunction";
                worksheet.Cells[1, 7].Value = "Details01";
                worksheet.Cells[1, 8].Value = "Details02";
                worksheet.Cells[1, 9].Value = "EmpCode";
                worksheet.Cells[1, 10].Value = "EmpName";
                worksheet.Cells[1, 11].Value = "MAC_Address";
                worksheet.Cells[1, 12].Value = "OS";
                worksheet.Cells[1, 13].Value = "URL";
                worksheet.Cells[1, 14].Value = "Device";
                worksheet.Cells[1, 15].Value = "IP_Address";
                worksheet.Cells[1, 16].Value = "Latitude";
                worksheet.Cells[1, 17].Value = "Longitude";

                worksheet.Cells["A1:F1"].AutoFitColumns();

                // Add more than 10 rows of data
                int j = 1;
                for (int i = 0; i < listAudi.Count; i++)
                {
                    worksheet.Cells[j + 1, 1].Value = $"{listAudi[i].Audi_Id}";
                    worksheet.Cells[j + 1, 2].Value = $"{listAudi[i].Username}";
                    worksheet.Cells[j + 1, 3].Value = $"{listAudi[i].Message}";
                    worksheet.Cells[j + 1, 4].Value = $"{listAudi[i].Created}";
                    worksheet.Cells[j + 1, 5].Value = $"{listAudi[i].Updated}";
                    worksheet.Cells[j + 1, 6].Value = $"{listAudi[i].WorkFunction}";
                    worksheet.Cells[j + 1, 7].Value = $"{listAudi[i].Details01}";
                    worksheet.Cells[j + 1, 8].Value = $"{listAudi[i].Details02}";
                    worksheet.Cells[j + 1, 9].Value = $"{listAudi[i].EmpCode}";
                    worksheet.Cells[j + 1, 10].Value = $"{listAudi[i].EmpName}";
                    worksheet.Cells[j + 1, 11].Value = $"{listAudi[i].MAC_Address}";
                    worksheet.Cells[j + 1, 12].Value = $"{listAudi[i].OS}";
                    worksheet.Cells[j + 1, 13].Value = $"{listAudi[i].URL}";
                    worksheet.Cells[j + 1, 14].Value = $"{listAudi[i].Device}";
                    worksheet.Cells[j + 1, 15].Value = $"{listAudi[i].IP_Address}";
                    worksheet.Cells[j + 1, 16].Value = $"{listAudi[i].Latitude}";
                    worksheet.Cells[j + 1, 17].Value = $"{listAudi[i].Longitude}";
                    j++;
                }

                // Convert to byte array
                byte[] fileBytes = package.GetAsByteArray();

                // Return the file with an appropriate MIME type for Excel
                return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "audi_profiles_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx"
                };
            }
        }

        public ActionResult audiExportCSV()
        {
            //List<UserModels> users = AdministratorBAL.GetUser();
            List<AudiLogModel> listAudi = AdministratorBAL.GetAudiLogDetails1();

            var delimiter = WebConfigurationManager.AppSettings["Delimiter"].ToString();
            var csv = new StringBuilder();
            csv.AppendLine($"Audi_Id{delimiter}Username{delimiter}Message{delimiter}Created{delimiter}Update{delimiter}WorkFunction{delimiter}Details01{delimiter}Details02{delimiter}EmpCode{delimiter}EmpName{delimiter}MAC_Address{delimiter}OS{delimiter}URL{delimiter}Device{delimiter}IP_Address{delimiter}Latitude{delimiter}Longitude");

            foreach (var user in listAudi)
            {
                csv.AppendLine($"{user.Audi_Id}{delimiter}{user.Username}{delimiter}{user.Message}{delimiter}{user.Created}{delimiter}{user.Updated}{delimiter}{user.WorkFunction}{delimiter}{user.Details01}{delimiter}{user.Details02}{delimiter}{user.EmpCode}{delimiter}{user.EmpName}{delimiter}{user.MAC_Address}{delimiter}{user.OS}{delimiter}{user.URL}{delimiter}{user.Device}{delimiter}{user.IP_Address}{delimiter}{user.Latitude}{delimiter}{user.Longitude}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var output = new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = "users.csv"
            };

            return output;
        }
        #endregion

        public ActionResult RoleExportExcel(string _c, string _n, string _o)
        {
            if (_c == null) _c = "";
            if (_n == null) _n = "";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<RoleModels> listRoleDetails = AdministratorBAL.NdGetRoleManagementDetials(_c, _n, _o);
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "Role Name";
                worksheet.Cells[1, 2].Value = "Position Role";
                worksheet.Cells[1, 3].Value = "Module No";
                worksheet.Cells[1, 4].Value = "Module Name";
                worksheet.Cells[1, 5].Value = "Group No";
                worksheet.Cells[1, 6].Value = "Group Module";
                worksheet.Cells[1, 7].Value = "Group Report";
                worksheet.Cells[1, 8].Value = "RoleReport Name";
                worksheet.Cells[1, 9].Value = "List Role";
                worksheet.Cells[1, 10].Value = "Create Role";
                worksheet.Cells[1, 11].Value = "View Role";
                worksheet.Cells[1, 12].Value = "Update Role";
                worksheet.Cells[1, 13].Value = "Delete Role";
                worksheet.Cells[1, 14].Value = "Export Role";
                worksheet.Cells[1, 15].Value = "Is Active";
                worksheet.Cells[1, 16].Value = "Create Date";
                worksheet.Cells[1, 17].Value = "Update Date";

                worksheet.Cells["A1:F1"].AutoFitColumns();

                // Add more than 10 rows of data
                int j = 1;
                for (int i = 0; i < listRoleDetails.Count; i++)
                {
                    worksheet.Cells[j + 1, 1].Value = $"{listRoleDetails[i].Role_Name}";
                    worksheet.Cells[j + 1, 2].Value = $"{listRoleDetails[i].Position_Role}";
                    worksheet.Cells[j + 1, 3].Value = $"{listRoleDetails[i].Module_No}";
                    worksheet.Cells[j + 1, 4].Value = $"{listRoleDetails[i].Module_Name}";
                    worksheet.Cells[j + 1, 5].Value = $"{listRoleDetails[i].Group_No}";
                    worksheet.Cells[j + 1, 6].Value = $"{listRoleDetails[i].Group_Module}";
                    worksheet.Cells[j + 1, 7].Value = $"{listRoleDetails[i].Group_Report}";
                    worksheet.Cells[j + 1, 8].Value = $"{listRoleDetails[i].RoleReport_Name}";
                    worksheet.Cells[j + 1, 9].Value = $"{listRoleDetails[i].List_Role}";
                    worksheet.Cells[j + 1, 10].Value = $"{listRoleDetails[i].Create_Role}";
                    worksheet.Cells[j + 1, 11].Value = $"{listRoleDetails[i].View_Role}";
                    worksheet.Cells[j + 1, 12].Value = $"{listRoleDetails[i].Update_Role}";
                    worksheet.Cells[j + 1, 13].Value = $"{listRoleDetails[i].Delete_Role}";
                    worksheet.Cells[j + 1, 14].Value = $"{listRoleDetails[i].Export_Role}";
                    worksheet.Cells[j + 1, 15].Value = $"{listRoleDetails[i].Is_Active}";
                    worksheet.Cells[j + 1, 16].Value = $"{listRoleDetails[i].Create_Date}";
                    worksheet.Cells[j + 1, 17].Value = $"{listRoleDetails[i].Update_Date}";
                    j++;
                }

                // Convert to byte array
                byte[] fileBytes = package.GetAsByteArray();

                // Return the file with an appropriate MIME type for Excel
                return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "Role_Details_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx"
                };
            }
        }

        public ActionResult RoleExportCSV(string _c, string _n, string _o)
        {

            if (_c == null) _c = "";
            if (_n == null) _n = "";
            List<RoleModels> listRoleDetails = AdministratorBAL.NdGetRoleManagementDetials(_c, _n, _o);

            var delimiter = WebConfigurationManager.AppSettings["Delimiter"].ToString();
            var csv = new StringBuilder();
            csv.AppendLine($"Role Name{delimiter}Position Role{delimiter}Module No{delimiter}Module Name{delimiter}Group No{delimiter}Group Module{delimiter}Group Report{delimiter}RoleReport Name{delimiter}List Role{delimiter}Create Role{delimiter}View Role{delimiter}Update Role{delimiter}Delete Role{delimiter}Export Role{delimiter}Is Active{delimiter}Create Date{delimiter}Update Date");

            foreach (var user in listRoleDetails)
            {
                csv.AppendLine($"{user.Role_Name}{delimiter}{user.Position_Role}{delimiter}{user.Module_No}{delimiter}{user.Module_Name}{delimiter}{user.Group_No}{delimiter}{user.Group_Module}{delimiter}{user.Group_Report}{delimiter}{user.RoleReport_Name}{delimiter}{user.List_Role}{delimiter}{user.Create_Role}{delimiter}{user.View_Role}{delimiter}{user.Update_Role}{delimiter}{user.Delete_Role}{delimiter}{user.Export_Role}{delimiter}{user.Is_Active}{delimiter}{user.Create_Date}{delimiter}{user.Update_Date}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var output = new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = "Role_Details_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".csv"
            };

            return output;
        }

        [HttpPost]
        public ActionResult MainUser(string cmdButton, List<UserModels> model)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            long lngID = 0;
            if (arrStr.Length > 1)
                lngID = Convert.ToInt32(arrStr[1]);
            switch (strValue)
            {
                case "ExportFile":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Export File");
                    return RedirectToAction("ExportExcel");
                case "ImportFile":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "Import File");
                    return RedirectToAction("ImportFile");
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า Register");
                    return RedirectToAction("Register", "Account");
                case "AddUserLDAP":
                    return RedirectToAction("AddUserLDAP", new { USER_LEVEL = 0 });
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditUser");
                    return RedirectToAction("EditUser", new { ID = lngID });
                //break;
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ขั้นตอน Approve Role");
                    var itemsSV = model.Where(r => r.IsSelected).ToList();
                    foreach (var i in itemsSV)
                    {
                        bool _r = false;
                        if (i.USER_LEVEL == 0 && i.User_Branch != null)
                        {
                            i.ADMIN_BY = Convert.ToString(Session["Username"]) != "" ? Convert.ToString(Session["Username"]) : "";
                            i.ADMIN_CREATE_DATE = DateTime.ParseExact(DateTime.Now.ToString(), "dd/MM/yyyy", null);//Convert.ToDateTime(DateTime.Now.ToString());//
                            i.USER_LEVEL = (i.User_Branch == "" || i.User_Branch == "001") ? 3 : 2; //2 = branch, 3 = head office
                            _r = AdministratorBAL.MainUser_UdateRole(i);
                        }
                    }
                    return RedirectToAction("MainUser");
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล User");
                    var items = model.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteUserByID(i.USER_ID);
                    }
                    return RedirectToAction("MainUser");
                    //break;

            }

            return null;
        }

        public ActionResult EditUser(long ID)
        {
            string DecryptPWD = string.Empty;
            ViewBag.CurrentRoleList = LoadMasterRole();
            ViewBag.CurrentLockAccountList = "0,Unlock|1,Lock";
            UserModels obj = AdministratorBAL.GetUserByID(ID);
            ViewBag.ID = obj.USER_ID;
            //@USER_LOGIN as varchar(50),
            ViewBag.USER_LOGIN = obj.USER_LOGIN;
            //@USER_PASSWORD as varchar(50),
            //var DecryptPWD = AesUtil.DecryptString(_iniCon.cryptoKey, obj.USER_PASSWORD, iv);
            if (obj.USER_ID > 28)
            {
                DecryptPWD = AesUtil.DecryptString(_iniCon.cryptoKey, obj.USER_PASSWORD, iv);
            }
            else
            {
                DecryptPWD = obj.USER_PASSWORD.ToString();
            }
            ViewBag.USER_PASSWORD = DecryptPWD;
            //@USER_FIRSTNAME as varchar(150),
            ViewBag.USER_FIRSTNAME = obj.USER_FIRSTNAME;
            //@USER_LASTNAME as varchar(150),
            ViewBag.USER_LASTNAME = obj.USER_LASTNAME;
            //@USER_LEVEL as int,
            ViewBag.USER_LEVEL = obj.USER_LEVEL;
            //@USER_EMAIL as nvarchar(50)
            ViewBag.USER_EMAIL = obj.USER_EMAIL;
            //@USER_FLAG as bit,
            ViewBag.USER_FLAG = obj.USER_FLAG;
            ViewBag.USER_LOCK = obj.USER_LOCK;
            ViewBag.IS_ACTIVE = obj.IS_ACTIVE;

            return View("EditUser");

        }
        [HttpPost]
        public ActionResult EditUser(string cmdButton, long ID, string USER_LOGIN, string USER_PASSWORD, string USER_FIRSTNAME, string USER_LASTNAME, int USER_LEVEL, string USER_EMAIL, int USER_LOCK, bool? isChecked)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditUser");
                    UserModels obj = new UserModels();
                    obj.USER_ID = ID;
                    obj.USER_LOGIN = USER_LOGIN;
                    var ENCRYPTPWD = AesUtil.EncryptString(_iniCon.cryptoKey, USER_PASSWORD, iv);
                    obj.USER_PASSWORD = ENCRYPTPWD;
                    obj.USER_FIRSTNAME = USER_FIRSTNAME;
                    obj.USER_LASTNAME = USER_LASTNAME;
                    obj.USER_LEVEL = USER_LEVEL;
                    obj.USER_EMAIL = USER_EMAIL;
                    obj.USER_LOCK = Convert.ToBoolean(USER_LOCK);
                    obj.IS_ACTIVE = Convert.ToBoolean(isChecked);
                    bool resultSave = AdministratorBAL.EditUser2(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditUser");
                    return RedirectToAction("MainUser");

            }
            return RedirectToAction("MainUser");
        }
        [HttpPost]
        public ActionResult AddMapUserBranch(string cmdButton, string ID_BRANCH, List<UserModels> model)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูล AddMapUserBranch");
                    var items = model.Where(r => r.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        MAP_USER_BRANCHModel obj = new MAP_USER_BRANCHModel();
                        obj.ID_USER = i.USER_ID;
                        obj.ID_BRANCH = ID_BRANCH;
                        bool resultSave = AdministratorBAL.AddMap_User_Branch(obj);
                    }
                    return RedirectToAction("MainMapUserBranch");
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูล AddMapUserBranch");
                    return RedirectToAction("MainMapUserBranch");

            }

            return null;
        }
        public ActionResult AddMapUserBranch()
        {


            ViewBag.CurrentBranchList = LoadMasterBranch();

            List<UserModels> list = new List<UserModels>();
            list = AdministratorBAL.GetUser();

            return View("AddMapUserBranch", list);

        }
        [HttpPost]
        public ActionResult MainMapUserBranch(List<MAP_USER_BRANCHModel> model, string cmdButton)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            long lngID = 0;
            if (arrStr.Length > 1)
                lngID = Convert.ToInt32(arrStr[1]);
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddMapUserBranch");
                    return RedirectToAction("AddMapUserBranch");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditMapUserBranch");
                    return RedirectToAction("EditMapUserBranch", new { ID = lngID });
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล MapUserBranch");
                    var items = model.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteMapUserBranchByID(i.ID);
                    }
                    return RedirectToAction("MainMapUserBranch");

            }

            return null;
        }
        public ActionResult EditMapUserBranch(long ID)
        {

            ViewBag.CurrentUserList = LoadMasterUser();
            ViewBag.CurrentBranchList = LoadMasterBranch();

            MAP_USER_BRANCHModel obj = AdministratorBAL.GetMap_User_BranchByID(ID);
            ViewBag.ID = obj.ID;
            ViewBag.ID_USER = Convert.ToString(obj.ID_USER);
            ViewBag.ID_BRANCH = obj.ID_BRANCH;
            ViewBag.ID_BRANCH2 = obj.ID_BRANCH2 == "" ? "0" : obj.ID_BRANCH2;
            ViewBag.B_Date = (obj.B_DATE != null || obj.B_DATE != "") ? DateTime.Parse(obj.B_DATE).ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
            ViewBag.E_Date = (obj.E_DATE != null || obj.E_DATE != "") ? DateTime.Parse(obj.E_DATE).ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
            ViewBag.PERMA = obj.PERMA;
            ViewBag.OPEN_OPT = obj.OPEN_OPT;
            return View("EditMapUserBranch");

        }
        [HttpPost]
        public ActionResult EditMapUserBranch_bk(string cmdButton, long ID, long ID_USER, string ID_BRANCH)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูล EditMapUserBranch");
                    MAP_USER_BRANCHModel obj = new MAP_USER_BRANCHModel();
                    obj.ID = ID;
                    obj.ID_USER = ID_USER;
                    obj.ID_BRANCH = ID_BRANCH;
                    bool resultSave = AdministratorBAL.EditMap_User_Branch(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูล EditMapUserBranch");
                    return RedirectToAction("MainMapUserBranch");
            }
            return RedirectToAction("MainMapUserBranch");
        }

        [HttpPost]
        public ActionResult EditMapUserBranch(string cmdButton, long ID, string _iduser, string ID_BRANCH, string ID_BRANCH1, string ID_BRANCH2, string B_DATE, string E_DATE, bool? PERMA, bool? OPEN_OPT, string _idbranch2, string _bdate, string _edate)
        {
            switch (cmdButton)
            {
                case "Save":
                    MAP_USER_BRANCHModel obj = new MAP_USER_BRANCHModel();
                    obj.ID = ID;
                    obj.ID_USER = Convert.ToInt64(_iduser);//ID_USER;
                    obj.ID_BRANCH = Convert.ToString(ID_BRANCH != null ? ID_BRANCH : ID_BRANCH1);
                    obj.ID_BRANCH2 = Convert.ToString((ID_BRANCH2 != null) ? ID_BRANCH2 : _idbranch2);
                    obj.B_DATE = (B_DATE != null) ? DateTime.Parse(B_DATE).ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")) : DateTime.Parse(_bdate).ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"));
                    obj.E_DATE = (E_DATE != null) ? DateTime.Parse(E_DATE).ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")) : DateTime.Parse(_edate).ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"));
                    obj.PERMA = Convert.ToBoolean(PERMA.ToString() == string.Empty ? false : PERMA);
                    obj.OPEN_OPT = Convert.ToBoolean(OPEN_OPT.ToString() == string.Empty ? false : OPEN_OPT);
                    bool resultSave = AdministratorBAL.EditMap_User_Branch2(obj);
                    break;
                case "Cancel":
                    return RedirectToAction("MainMapUserBranch");
            }
            return RedirectToAction("MainMapUserBranch");
        }

        public ActionResult SearchMapUserBranch(string userName, string branchName)
        {

            var data = from m in AdministratorBAL.GetMap_User_Branch()
                       select m;
            if (!string.IsNullOrEmpty(userName))
            {
                data = data.Where(s => s.USER_NAME.Contains(userName));
            }
            if (!string.IsNullOrEmpty(branchName))
            {
                data = data.Where(s => s.BRANCH_NAME.Contains(branchName));
            }

            return View("MainMapUserBranch", data.ToList());

        }
        public ActionResult MainMapUserBranch()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า MainMapUserBranch");
            List<MAP_USER_BRANCHModel> list = AdministratorBAL.GetMap_User_Branch();
            if (null != Session["Level"])
            {
                ViewBag.Level = Session["Level"].ToString();
            }
            return View("MainMapUserBranch", list);

        }
        public ActionResult SearchMapBranchReport(string branchName)
        {

            var data = from m in AdministratorBAL.GetMap_Branch_Report()
                       select m;

            if (!string.IsNullOrEmpty(branchName))
            {
                data = data.Where(s => s.BRANCH_NAME.Contains(branchName));
            }



            return View("MainMapBranchReport", data.ToList());

        }
        public ActionResult MainMapBranchReport()
        {
            ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า MainMapBranchReport");
            List<MAP_BRANCH_REPORTSModel> list = AdministratorBAL.GetMap_Branch_Report();

            return View("MainMapBranchReport", list);

        }
        public ActionResult EditMapBranchReport(long ID)
        {

            ViewBag.CurrentGroupReportList = LoadMasterGroupReport();
            ViewBag.CurrentGroupDetailReportList = LoadMasterGroupDetailReport();
            ViewBag.CurrentBranchList = LoadMasterBranch();

            MAP_BRANCH_REPORTSModel obj = AdministratorBAL.GetMap_Branch_ReportByID(ID);
            ViewBag.ID = obj.ID;
            ViewBag.GROUP_NO = Convert.ToString(obj.GROUP_NO);
            ViewBag.REPORT_NO = Convert.ToString(obj.REPORT_NO);
            ViewBag.ID_BRANCH = obj.ID_BRANCH;
            return View("EditMapBranchReport");

        }
        [HttpPost]
        public ActionResult EditMapBranchReport(string cmdButton, long ID, decimal? GROUP_NO, decimal? REPORT_NO, string ID_BRANCH)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า EditMapBranchReport");
                    MAP_BRANCH_REPORTSModel obj = new MAP_BRANCH_REPORTSModel();
                    obj.ID = ID;
                    obj.GROUP_NO = GROUP_NO;
                    obj.REPORT_NO = REPORT_NO;
                    obj.ID_BRANCH = ID_BRANCH;
                    bool resultSave = AdministratorBAL.EditMap_Branch_Report(obj);
                    break;
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า EditMapBranchReport");
                    return RedirectToAction("MainMapBranchReport");
            }
            return RedirectToAction("MainMapBranchReport");
        }
        [HttpPost]
        public ActionResult MainMapBranchReport(List<MAP_BRANCH_REPORTSModel> model, string cmdButton)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            long lngID = 0;
            if (arrStr.Length > 1)
                lngID = Convert.ToInt32(arrStr[1]);
            switch (strValue)
            {
                case "Add":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า AddMapBranchReport");
                    return RedirectToAction("AddMapBranchReport");
                case "Edit":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ไปที่หน้า EditMapBranchReport");
                    return RedirectToAction("EditMapBranchReport", new { ID = lngID });
                case "Delete":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ลบข้อมูล MapBranchReport");
                    var items = model.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteMapBranchReportByID(i.ID);
                    }
                    return RedirectToAction("MainMapBranchReport");

            }

            return null;
        }
        public ActionResult AddMapBranchReport()
        {

            ViewBag.CurrentGroupReportList = LoadMasterGroupReport();

            ViewBag.CurrentBranchList = LoadMasterBranch();


            var viewModel = new ReportViewModel
            {
                Reports = null
            };
            return View("AddMapBranchReport", viewModel);

        }
        [HttpPost]
        public ActionResult AddMapBranchReport(string cmdButton, string ID_BRANCH, decimal? GROUP_NO, ReportViewModel model)
        {
            switch (cmdButton)
            {
                case "Save":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "บันทึกข้อมูลหน้า AddMapBranchReport");
                    var selectedReports = model.Reports.Where(r => r.IsSelected).ToList();
                    //foreach (var i in selectedReports)
                    //{
                    //    MAP_BRANCH_REPORTSModel obj = new MAP_BRANCH_REPORTSModel();

                    //    obj.GROUP_NO = GROUP_NO;
                    //    obj.REPORT_NO = i.REPORT_NO;
                    //    obj.ID_BRANCH = ID_BRANCH;
                    //    bool resultSave = AdministratorBAL.AddMap_Branch_Report(obj);
                    //}
                    foreach (var i in model.Reports)//Fn2 20240802
                    {
                        MAP_BRANCH_REPORTSModel obj = new MAP_BRANCH_REPORTSModel();

                        obj.GROUP_NO = GROUP_NO;
                        obj.REPORT_NO = i.REPORT_NO;
                        obj.ID_BRANCH = ID_BRANCH;
                        obj.IsSelected = i.IsSelected;
                        bool resultSave2 = AdministratorBAL.AddMap_Branch_Report2(obj);
                    }
                    if (model.Reports != null)//Prepare for the new
                    {
                        model.Reports = null;
                    }
                    return RedirectToAction("MainMapBranchReport");
                case "Cancel":
                    ProcessLogBAL.AddProcessLog(Convert.ToString(Session["Username"]), "ยกเลิกข้อมูลหน้า AddMapBranchReport");
                    return RedirectToAction("MainMapBranchReport");
                case null:

                    ViewBag.CurrentGroupReportList = LoadMasterGroupReport();
                    ViewBag.CurrentGroupReport = Convert.ToString(GROUP_NO);

                    ViewBag.CurrentBranchList = LoadMasterBranch();
                    ViewBag.CurrentBranch = ID_BRANCH;
                    //var Reports = AdministratorBAL.GetMasterReportByGroupNo(GROUP_NO);
                    var Reports2 = AdministratorBAL.GetMasterReportByGroupNo2(GROUP_NO, ID_BRANCH);//add 20270802
                    var viewModel = new ReportViewModel
                    {
                        Reports = Reports2
                    };
                    return View("AddMapBranchReport", viewModel);
            }

            return null;
        }
        public ActionResult SearchMapUserReport(string userName)
        {

            var data = from m in AdministratorBAL.GetMap_User_Report()
                       select m;

            if (!string.IsNullOrEmpty(userName))
            {
                data = data.Where(s => s.UserName.Contains(userName));
            }

            var viewModel = new MAP_USER_REPORTViewModel { MapUserReports = data.ToList() };

            return View("MainMapUserReport", viewModel);

        }
        public ActionResult MainMapUserReport()
        {

            List<MAP_USER_REPORTModel> list = AdministratorBAL.GetMap_User_Report();
            var viewModel = new MAP_USER_REPORTViewModel
            {
                MapUserReports = list
            };
            return View("MainMapUserReport", viewModel);

        }

        [HttpPost]
        public ActionResult MainMapUserReport(MAP_USER_REPORTViewModel model, string cmdButton)
        {
            string[] arrStr = cmdButton.Split('|');
            string strValue = Convert.ToString(arrStr[0]);
            long lngID = 0;
            if (arrStr.Length > 1)
                lngID = Convert.ToInt32(arrStr[1]);
            switch (strValue)
            {
                case "Add":
                    return RedirectToAction("AddMapUserReport");
                case "Edit":
                    return RedirectToAction("EditMapUserReport", new { ID = lngID });
                case "Delete":
                    var items = model.MapUserReports.Where(m => m.IsSelected).ToList();
                    foreach (var i in items)
                    {
                        bool resultDelete = AdministratorBAL.DeleteMapUserReportByID(i.ID_MENU);
                    }
                    return RedirectToAction("MainMapUserReport");

            }

            return null;
        }
        public ActionResult AddMapUserReport()
        {

            ViewBag.CurrentUserList = LoadMasterUser();
            ViewBag.CurrentGroupReportList = LoadMasterGroupReport();

            ViewBag.CurrentBranchList = LoadMasterBranch();


            var viewModel = new ReportViewModel
            {
                Reports = null
            };
            return View("AddMapUserReport", viewModel);

        }

        [HttpPost]
        public ActionResult AddMapUserReport(string cmdButton, long? ID_USER, string ID_BRANCH, decimal? GROUP_NO, ReportViewModel model)
        {
            switch (cmdButton)
            {
                case "Save":

                    var selectedReports = model.Reports.Where(r => r.IsSelected).ToList();
                    foreach (var i in selectedReports)
                    {
                        MAP_USER_REPORTModel obj = new MAP_USER_REPORTModel();
                        obj.ID_USER = ID_USER;
                        obj.GROUP_NO = GROUP_NO;
                        obj.REPORT_NO = i.REPORT_NO;
                        obj.ID_BRANCH = ID_BRANCH;
                        bool resultSave = AdministratorBAL.AddMap_User_Report(obj);
                    }
                    return RedirectToAction("MainMapUserReport");
                case "Cancel":
                    return RedirectToAction("MainMapUserReport");
                case null:
                    ViewBag.CurrentUserList = LoadMasterUser();
                    ViewBag.CurrentUser = Convert.ToString(ID_USER);
                    ViewBag.CurrentGroupReportList = LoadMasterGroupReport();
                    ViewBag.CurrentGroupReport = Convert.ToString(GROUP_NO);

                    ViewBag.CurrentBranchList = LoadMasterBranch();
                    ViewBag.CurrentBranch = ID_BRANCH;
                    var Reports = AdministratorBAL.GetMasterReportByGroupNo(GROUP_NO);
                    var viewModel = new ReportViewModel
                    {
                        Reports = Reports
                    };
                    return View("AddMapUserReport", viewModel);
            }

            return null;
        }
        public ActionResult EditMapUserReport(long ID)
        {

            ViewBag.CurrentUserList = LoadMasterUser();
            ViewBag.CurrentGroupReportList = LoadMasterGroupReport();
            ViewBag.CurrentGroupDetailReportList = LoadMasterGroupDetailReport();
            ViewBag.CurrentBranchList = LoadMasterBranch();

            MAP_USER_REPORTModel obj = AdministratorBAL.GetMap_User_ReportByID(ID);
            ViewBag.ID = obj.ID_MENU;
            ViewBag.ID_USER = Convert.ToString(obj.ID_USER);
            ViewBag.GROUP_NO = Convert.ToString(obj.GROUP_NO);
            ViewBag.REPORT_NO = Convert.ToString(obj.REPORT_NO);
            ViewBag.ID_BRANCH = obj.ID_BRANCH;
            return View("EditMapUserReport");

        }
        [HttpPost]
        public ActionResult EditMapUserReport(string cmdButton, long ID, long? ID_USER, decimal? GROUP_NO, decimal? REPORT_NO, string ID_BRANCH)
        {
            switch (cmdButton)
            {
                case "Save":
                    MAP_USER_REPORTModel obj = new MAP_USER_REPORTModel();
                    obj.ID_MENU = ID;
                    obj.ID_USER = ID_USER;
                    obj.GROUP_NO = GROUP_NO;
                    obj.REPORT_NO = REPORT_NO;
                    obj.ID_BRANCH = ID_BRANCH;
                    bool resultSave = AdministratorBAL.EditMap_User_Report(obj);
                    break;
                case "Cancel":
                    return RedirectToAction("MainMapUserReport");
            }
            return RedirectToAction("MainMapUserReport");
        }

        #region Master data
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
        private string LoadMasterUser()
        {
            string _strData = string.Empty;
            List<UserModels> _masterDataList = new List<UserModels>();
            try
            {
                _masterDataList = AdministratorBAL.GetMasterUser();
                if (_masterDataList.Count > 0)
                {
                    foreach (UserModels data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.USER_ID + "," + data.USER_LOGIN;
                        else
                            _strData += "|" + data.USER_ID + "," + data.USER_LOGIN;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }
        private string LoadMasterGroupReport()
        {
            string _strData = string.Empty;
            List<GroupReportModel> _masterDataList = new List<GroupReportModel>();
            try
            {
                _masterDataList = AdministratorBAL.GetMasterGroupReport();
                if (_masterDataList.Count > 0)
                {
                    foreach (GroupReportModel data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.GroupCode + "," + data.GroupName;
                        else
                            _strData += "|" + data.GroupCode + "," + data.GroupName;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }
        private string LoadMasterGroupDetailReport()
        {
            string _strData = string.Empty;
            List<GroupDetailReportModel> _masterDataList = new List<GroupDetailReportModel>();
            try
            {
                _masterDataList = AdministratorBAL.GetMasterGroupDetailReport();
                if (_masterDataList.Count > 0)
                {
                    foreach (GroupDetailReportModel data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.REPORT_NO + "," + data.REPORT_NAME;
                        else
                            _strData += "|" + data.REPORT_NO + "," + data.REPORT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }
        private string LoadSystemBranch()
        {
            string _strData = string.Empty;
            List<BranchModel> _masterDataList = new List<BranchModel>();
            try
            {
                _masterDataList = AdministratorBAL.GetSystemBranch();
                if (_masterDataList.Count > 0)
                {
                    foreach (BranchModel data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.SOL_CODE + "," + data.CATEGORY_DESC;
                        else
                            _strData += "|" + data.SOL_CODE + "," + data.CATEGORY_DESC;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }

        private string LoadSystemHub()
        {
            string _strData = string.Empty;
            List<HubModels> _masterDataList = new List<HubModels>();
            try
            {
                _masterDataList = AdministratorBAL.GetSystemHub();
                if (_masterDataList.Count > 0)
                {
                    foreach (HubModels data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.HUB_CODE + "," + data.HUB_NAME;
                        else
                            _strData += "|" + data.HUB_CODE + "," + data.HUB_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }

        private string LoadSystemDistrict()
        {
            string _strData = string.Empty;
            List<DistrictModels> _masterDataList = new List<DistrictModels>();
            try
            {
                _masterDataList = AdministratorBAL.GetSystemDistrict();
                if (_masterDataList.Count > 0)
                {
                    foreach (DistrictModels data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.DISTRICT_CODE + "," + data.DISTRICT_NAME;
                        else
                            _strData += "|" + data.DISTRICT_CODE + "," + data.DISTRICT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }

        private string LoadSystemDepartment()
        {
            string _strData = string.Empty;
            List<DepartmentModels> _masterDataList = new List<DepartmentModels>();
            try
            {
                _masterDataList = AdministratorBAL.GetSystemDepartment();
                if (_masterDataList.Count > 0)
                {
                    foreach (DepartmentModels data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.DEPT_CODE + "," + data.DEPT_NAME;
                        else
                            _strData += "|" + data.DEPT_CODE + "," + data.DEPT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }

        private string LoadMasterBranch()
        {
            string _strData = string.Empty;
            List<BranchModel> _masterDataList = new List<BranchModel>();
            try
            {
                _masterDataList = AdministratorBAL.GetMasterBranch();
                if (_masterDataList.Count > 0)
                {
                    foreach (BranchModel data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.BRANCH + "," + data.DESC;
                        else
                            _strData += "|" + data.BRANCH + "," + data.DESC;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }

        private string LoadWorkFunction()
        {
            string _strData = string.Empty;
            List<AudiLogModel> _masterDataList = new List<AudiLogModel>();
            try
            {
                _masterDataList = AdministratorBAL.LoadWorkFunction();
                if (_masterDataList.Count > 0)
                {
                    foreach (AudiLogModel data in _masterDataList)
                    {
                        if (_strData == "")
                            _strData = data.WorkFunction + "," + data.WorkFunction;
                        else
                            _strData += "|" + data.WorkFunction + "," + data.WorkFunction;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _strData;
        }

        [HttpPost]
        public ActionResult GetBranchByUserID(string UserID)
        {
            MAP_USER_REPORTModel obj = new MAP_USER_REPORTModel();
            try
            {
                obj = AdministratorBAL.GetBranchByUserID(UserID);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Content(obj.ID_BRANCH);
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

        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(T).FullName;
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable GetDataTableFromObjects(object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                Type t = objects[0].GetType();
                DataTable dt = new DataTable(t.Name);
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    dt.Columns.Add(new DataColumn(pi.Name));
                }
                foreach (var o in objects)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr[dc.ColumnName] = o.GetType().GetProperty(dc.ColumnName).GetValue(o, null);
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return null;

        }

        private string LoadStatusActive()
        {
            string _strData = string.Empty;
            List<StatusActiveModel> statusActives = new List<StatusActiveModel>()
            {
                new StatusActiveModel { activeValue = 1, activeName = "Active" },
                new StatusActiveModel { activeValue = 0, activeName = "InActive" }
            };

            if (statusActives.Count > 0)
            {
                foreach (StatusActiveModel data in statusActives)
                {
                    if (_strData == "")
                    {
                        _strData = data.activeValue + "," + data.activeName;
                    }
                    else
                    {
                        _strData += "|" + data.activeValue + "," + data.activeName;
                    }
                }
            }
            return _strData;

        }

        private string LoadStatusRadio()
        {
            string _strData = string.Empty;
            List<StatusRadioModel> statusRadio = new List<StatusRadioModel>()
            {
                new StatusRadioModel { rdoValue = 4, rdoName = "HQ Admin" },
                new StatusRadioModel { rdoValue = 5, rdoName = "Branch Admin" },
                new StatusRadioModel { rdoValue = 6, rdoName = "HQ" },
                new StatusRadioModel { rdoValue = 2, rdoName = "Branch" }
            };

            if (statusRadio.Count > 0)
            {
                foreach (StatusRadioModel data in statusRadio)
                {
                    if (_strData == "")
                    {
                        _strData = data.rdoValue + "," + data.rdoName;
                    }
                    else
                    {
                        _strData += "|" + data.rdoValue + "," + data.rdoName;
                    }
                }
            }
            return _strData;

        }

        private string LoadTransferRadio()
        {
            string _strData = string.Empty;
            List<StatusRadioModel> transRadio = new List<StatusRadioModel>()
            {
                new StatusRadioModel { rdoValue = 1, rdoName = "ย้ายสาขา" },
                new StatusRadioModel { rdoValue = 2, rdoName = "ย้าย Role" },
                new StatusRadioModel { rdoValue = 3, rdoName = "ย้าย Role และสาขา" }
            };

            if (transRadio.Count > 0)
            {
                foreach (StatusRadioModel data in transRadio)
                {
                    if (_strData == "")
                    {
                        _strData = data.rdoValue + "," + data.rdoName;
                    }
                    else
                    {
                        _strData += "|" + data.rdoValue + "," + data.rdoName;
                    }
                }
            }
            return _strData;

        }

        //get branch,hub,dept,dist -> code, name
        public string getMS(string _c, string _n, string _o)
        {
            string _r = string.Empty;

            try
            {
                DataTable dt = AdministratorBAL.getMS(_c, _n, _o);
                if (dt.Rows.Count > 0)
                {
                    if (_c != "")
                    {
                        _r = dt.Rows[0]["Name"].ToString();
                    }
                    else if (_n != "")
                    {
                        _r = dt.Rows[0]["Code"].ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return _r;
        }

        //get Branch Group for RPT_Ondemand
        public string getBranchOndemand(string code, string name, string obt)
        {
            string result = string.Empty;

            try
            {
                DataTable dt = AdministratorBAL.getBranchOndemand(code, name, obt);
                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[0]["Branch_group"].ToString();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return result;
        }

        public string LoadRoleListNd()
        {
            string _strData = string.Empty;
            try
            {
                var rolename = Session["RoleName"].ToString();
                List<RoleModels> rolelist = AdministratorBAL.NdGetRoleManagement();
                List<RoleModels> rolelist1 = AdministratorBAL.NdGetRoleManagement();
                if (rolelist.Count > 0)
                {
                    if (rolename != "superadmin")
                    {
                        rolelist1 = rolelist.Where(x => x.Role_Name != "superadmin").ToList();
                    }
                    foreach (RoleModels data in rolelist1)
                    {
                        if (_strData == "")
                        {
                            _strData = data.Role_Name + "," + data.Role_Name;
                        }
                        else
                        {
                            _strData += "|" + data.Role_Name + "," + data.Role_Name;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return _strData;
        }

        #endregion

        #region Functions

        public void audiLog(string wf, string msg, string emp)
        {
            if (Session["AudiLog"] != null)
            {
                AudiLogModel audilog = (AudiLogModel)Session["AudiLog"];
                audilog.WorkFunction = wf;
                audilog.Message = msg;
                audilog.EmpCode = emp;
                if (Session["latitude"] != null && Session["longitude"] != null)
                {
                    audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                    audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                }
                ProcessLogBAL.AddProcessLog2(audilog);
            }
        }

        #endregion
        public class ViewModel
        {
            public string Result { get; set; }
        }
    }
}