using GHB_D1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GHB_D1.Code.BAL;
using System.Text;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Web.Configuration;
using GHB_D1.Services;
using System.Data;
using GHB_D1.Code.Util;
using GHB_D1.ReportFiles.DataSet;
using System.Globalization;
using System.IO;

namespace GHB_D1.Controllers
{
    public class CDMReportController : BaseController
    {
        private AccountService _accService = new AccountService();
        iniConnection _iniCon = null;
        Loger _logSys = null;
        dsGenReport _ds = null;
        CommondBAL _comBal = null;
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        CultureInfo _cultureEnInfo = new CultureInfo("en-US");
        CultureInfo _cultureThInfo = new CultureInfo("th-TH");
        string _strGroupNo = "";
        //string ReportPathFiles = WebConfigurationManager.AppSettings["ReportPathFiles"].ToString();
        public CDMReportController()
        {
            _accService = new AccountService();
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            _iniCon.Server = ModConf.ReadIni(_iniCon.iniFile, "DB", "Server");
            _iniCon.User = ModConf.ReadIni(_iniCon.iniFile, "DB", "User");
            _iniCon.Password = ModConf.ReadIni(_iniCon.iniFile, "DB", "Password");
            _iniCon.DBAMS = ModConf.ReadIni(_iniCon.iniFile, "DB", "DatabaseAMS");
            _iniCon.DBTLF = ModConf.ReadIni(_iniCon.iniFile, "DB", "DatabaseTLF");
            _ds = new dsGenReport();
            _comBal = new CommondBAL();
            _logSys = new Loger();
        }

        [HttpGet]

        public ActionResult Index(string px, string py, string T_DATE, string F_DATE, string SEARCH_KEY, string pz, string message, string ToDate, string FrDate)
        {
            CDMViewModel cdmVM = new CDMViewModel();
            
            //cdmVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            cdmVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            cdmVM.F_DATE = (F_DATE == null || F_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : F_DATE;
            cdmVM.ToDate = (ToDate == null || ToDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ToDate;
            cdmVM.FrDate = (FrDate == null || FrDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : FrDate;
            cdmVM.DISPLAY_FILTER = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToString("dd-MM-yyyy", new CultureInfo("en-US")) : T_DATE;
            cdmVM.SEARCH_KEY = SEARCH_KEY;
            cdmVM.px = px;
            cdmVM.py = py;
            cdmVM.pz = pz;
        

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();

            if (Session["UserId"] != null && Session["UserId"].ToString() == py)
            {
                //_strGroupNo = _accService.GetGroupNoReportByName("CDM");
                //list = _accService.AuthorizeGroupDetailReport(px, py, _strGroupNo, SEARCH_KEY);
                var rolename = Session["RoleName"].ToString();
                _strGroupNo = _accService.GetGroupNoReportByNameND("CDM");
                list = _accService.AuthorizeGroupDetailReportND(rolename, py, _strGroupNo, SEARCH_KEY);
                ViewBag.GroupDetailReport = list;

                Session["TitleReport"] = pz;
                cdmVM.TITLE_REPORT = pz;
                cdmVM.BRANCH_ID = px;
                cdmVM.USER_ID = py;
                cdmVM.MESSAGE = message;
                cdmVM.px = px;
                cdmVM.py = py;
                cdmVM.pz = pz;
                if (Session["GroupReport"] != null)
                {
                    ViewBag.GroupReport = Session["GroupReport"];
                }
                return View("CDM", cdmVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult GenReport(string px, string py, string cmdButton, ADMViewModel AdmVM, string SOLCODE)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            List<ADMModel> _admdata = new List<ADMModel>();
            DataTable _dt = new DataTable();
            //_strGroupNo = _accService.GetGroupNoReportByName("CDM");
            _logSys.WriteProcessLogFile(_strPathFile, "GenReport T_Date : " + AdmVM.T_DATE);
            _logSys.WriteProcessLogFile(_strPathFile, "GenReport SEARCH_KEY : " + AdmVM.SEARCH_KEY);
            _logSys.WriteProcessLogFile(_strPathFile, "GetGroupDetailReport : " + _strGroupNo);
            AdmVM.T_DATE = AdmVM.ToDate;
            AdmVM.T_DATE = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE;
            AdmVM.DISPLAY_FILTER = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE;
            AdmVM.SEARCH_KEY = AdmVM.SEARCH_KEY;

            //list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, AdmVM.SEARCH_KEY);
            var rolename = Session["RoleName"].ToString();
            _strGroupNo = _accService.GetGroupNoReportByNameND("CDM");
            list = _accService.AuthorizeGroupDetailReportND(rolename, py, _strGroupNo, AdmVM.SEARCH_KEY);
            ViewBag.GroupDetailReport = list;
            _logSys.WriteProcessLogFile(_strPathFile, "GetGroupDetailReport_List : " + list.Count);
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["BranchId"] != null && Session["BranchId"].ToString() != "")
            {
                string bOndemand = getBranchOndemand(Session["BranchId"].ToString(), "", "ondemand");
                AdmVM.BRANCH_ID = Session["BranchId"].ToString().Substring(0, 3);
            }
            switch (cmdButton)
            {
                case "Search":
                    list = _accService.AuthorizeGroupDetailReport(AdmVM.BRANCH_ID, AdmVM.USER_ID, _strGroupNo, AdmVM.SEARCH_KEY);
                    ViewBag.GroupDetailReport = list;

                    if (list.Count <= 0)
                    {
                        AdmVM.MESSAGE = "ไม่มีข้อมูล";
                    }
                    break;
                default:

                    string[] _arrCon = null;
                    _arrCon = cmdButton.Split('|');
                    string _tmpdate = string.Empty;

                    string[] _arrDate = null;
                    _arrDate = AdmVM.T_DATE.Split('/');



                    int dt = Int32.Parse(_arrDate[2].ToString());

                    //DateTime dt = DateTime.Parse(AdmVM.T_DATE);


                    string year = "";
                    if (dt != 0)
                    {
                        year = (dt <= 2000) ? (dt + 543).ToString().Substring(2) : dt.ToString().Substring(2);
                    }
                    else
                    {
                        year = DateTime.Now.Year.ToString().Substring(2);
                    }


                   
                    string _strReport_Name = _arrCon[1].ToString();
                    string _strFormatType = _arrCon[2].ToString();

                    string strT_Date = AdmVM.T_DATE.Replace("/", "");

                    if (strT_Date != "")
                    {
                        _tmpdate = strT_Date;
                        strT_Date = year + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
                    }


                    StringBuilder sb = new StringBuilder();
                    List<string> RptParramList = new List<string>();
                    _iniCon.stp = ModConf.ReadIni(_iniCon.iniFile, "DS", _strReport_Name);

                    char[] _sps = new char[] { '|' };
                    char[] _spd = new char[] { '-' };
                   // string _tmpdate = string.Empty;
                    string _depRecMOoney = string.Empty;
                    string _department = string.Empty;
                    string _pointRecMoney = string.Empty;

                    string strReport = string.Empty;
                    string _strTempFile = string.Empty;

                    try
                    {
                       
                        if (_strFormatType == "2")
                        {
                            switch (_strReport_Name)
                            {
                                case "CDM9000_M":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                                case "CDM9000_M_TOTAL":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                                case "CDM9000_Q_TOTAL":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                                case "CDM9000_Y_TOTAL":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;

                            }
                        }

                        string _strReportPath = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + ".rpt");
                        _logSys.WriteProcessLogFile(_strPathFile, "_strReportPath1(176) : " + _strReportPath);

                        string _param = string.Empty;

                        DiskFileDestinationOptions dfdopt = new DiskFileDestinationOptions();
                        ReportDocument cryRpt = new ReportDocument();

                        _logSys.WriteProcessLogFile(_strPathFile, "check _iniCon.store : " + _iniCon.stp);
                    

                        TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                        TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                        ConnectionInfo crConnectionInfo = new ConnectionInfo();
                        Tables CrTables;

                        _logSys.WriteProcessLogFile(_strPathFile, "Server : " + _iniCon.Server);
                        _logSys.WriteProcessLogFile(_strPathFile, "DatabaseName : " + _iniCon.DBTLF);
                        _logSys.WriteProcessLogFile(_strPathFile, "UserID : " + _iniCon.User);
                        _logSys.WriteProcessLogFile(_strPathFile, "PWD : " + _iniCon.Password);

                        cryRpt.Load(_strReportPath);

                        crConnectionInfo.ServerName = _iniCon.Server;// ReportServer;
                        crConnectionInfo.DatabaseName = _iniCon.DBTLF;// ReportDataBase;
                        crConnectionInfo.UserID = _iniCon.User;// ReportUserName;
                        crConnectionInfo.Password = _iniCon.Password;// ReportPwd;

                        CrTables = cryRpt.Database.Tables;
                        foreach (Table CrTable in CrTables)
                        {
                            crtableLogoninfo = CrTable.LogOnInfo;
                            crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                            CrTable.ApplyLogOnInfo(crtableLogoninfo);
                        }
                        cryRpt.SetParameterValue("@T_DATE", strT_Date);
                        if (_strReport_Name.Contains("ONDEMAND"))
                        {
                            if (SOLCODE == "" || SOLCODE == null)
                            {
                                cryRpt.SetParameterValue("@T_BRANCH", AdmVM.BRANCH_ID);
                            }
                            else
                            {
                                cryRpt.SetParameterValue("@T_BRANCH", SOLCODE);
                            }
                        }
                        try
                        {
                            string contentType = "";
                            switch (_strFormatType)
                            {
                                case "1":
                                    strReport = _strReport_Name + "-" + strT_Date + ".pdf";
                                    _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
                                    contentType = "application/pdf";
                                    break;
                                case "2":
                                    strReport = _strReport_Name + "-" + strT_Date + ".xls";
                                    _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.Excel, _strTempFile);
                                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    break;
                                case "3":
                                    strReport = _strReport_Name + "-" + strT_Date + ".txt";
                                    _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
                                    contentType = "application/txt";
                                    break;

                            }

                            return File(_strTempFile, contentType, strReport);
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        
                            AdmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + AdmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                            _logSys.WriteProcessLogFile(_strPathFile, "Choose Report Type(ex) : " + ex.Message.ToString());
                            //return View("CDM", AdmVM);
                            string p = py;
                           
                            return RedirectToAction("Index", "CDMReport", new { px = px, py = py, T_DATE = AdmVM.T_DATE, SEARCH_KEY = "", pz = AdmVM.TITLE_REPORT, message = AdmVM.MESSAGE });
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                        AdmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + AdmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                        _logSys.WriteProcessLogFile(_strPathFile, "Switch case default(ex) : " + ex.Message.ToString());
                        // return View("CDM", AdmVM);
                        string p = py;
                       
                        return RedirectToAction("Index", "CDMReport", new { px = px, py = py, T_DATE = AdmVM.T_DATE, SEARCH_KEY = "", pz = AdmVM.TITLE_REPORT, message = AdmVM.MESSAGE });
                    }
            }
            return View("CDM", AdmVM);

        }

 

        [HttpGet]
        public ActionResult DownloadExportFile()
        {
            string fname = "";
            string tempPath = "";
            fname = Session["BrowseFilePRT"].ToString();
            tempPath = fname;
            return File(tempPath, "application/pdf", fname);
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

    }
}



