using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GHB_D1.Code.BAL;
using GHB_D1.Code.Util;
using GHB_D1.Models;
using GHB_D1.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GHB_D1.Controllers
{
    public class LRM_ONDEMANDReportController : BaseController
    {
        // GET: LRM_ONDEMANDReport
        string _strGroupNo = "";
        Loger _logSys = new Loger();
        iniConnection _iniCon = null;
        private AccountService _accService = new AccountService();
        CommonUtilies objCom = new CommonUtilies();
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
      

        public LRM_ONDEMANDReportController()
        {
            _accService = new AccountService();
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            _iniCon.Server = ModConf.ReadIni(_iniCon.iniFile, "DB", "Server");
            _iniCon.User = ModConf.ReadIni(_iniCon.iniFile, "DB", "User");
            _iniCon.Password = ModConf.ReadIni(_iniCon.iniFile, "DB", "Password");
            _iniCon.DBAMS = ModConf.ReadIni(_iniCon.iniFile, "DB", "DatabaseAMS");
            _iniCon.DBTLF = ModConf.ReadIni(_iniCon.iniFile, "DB", "DatabaseTLF");
        }

        // GET: LRMReport
        public ActionResult Index(string px, string py, string T_DATE, string cmdButton, string SEARCH_KEY, string pz)
        {                       
            LRMOnDemandViewModel lrmOnDemandVM = new LRMOnDemandViewModel();
            lrmOnDemandVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            lrmOnDemandVM.DISPLAY_FILTER = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            lrmOnDemandVM.SEARCH_KEY = SEARCH_KEY;

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            if (Session["UserId"] != null && Session["UserId"].ToString() == py)
            {
                _strGroupNo = _accService.GetGroupNoReportByName("LRM_ONDEMAND");
                list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, SEARCH_KEY);
                ViewBag.GroupDetailReport = list;
                string searchValue = SEARCH_KEY;
                Session["TitleReport"] = pz;
                lrmOnDemandVM.TITLE_REPORT = pz;
                lrmOnDemandVM.BRANCH_ID = px;
                lrmOnDemandVM.USER_ID = py;
                lrmOnDemandVM.MESSAGE = "";

                return View("LRM_ONDEMAND", lrmOnDemandVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult GenReport(string px, string py, string T_Date, string cmdButton, LRMOnDemandViewModel LrmVM)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            LRMOnDemandViewModel lrmOnDemandVM = new LRMOnDemandViewModel();
            lrmOnDemandVM.T_DATE = (T_Date == null || T_Date == "") ? DateTime.Now.Date.ToShortDateString() : T_Date;
            lrmOnDemandVM.DISPLAY_FILTER = (T_Date == null || T_Date == "") ? DateTime.Now.Date.ToShortDateString() : T_Date;
            lrmOnDemandVM.SEARCH_KEY = LrmVM.SEARCH_KEY;
            _strGroupNo = _accService.GetGroupNoReportByName("LRM_ONDEMAND");
            switch (cmdButton)
            {
                case "Search":
                  
                    list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, LrmVM.SEARCH_KEY);
                    ViewBag.GroupDetailReport = list;
                    break;
                default:
                    string[] _arrCon = null;
                    _arrCon = cmdButton.Split('|');
                    string[] _arrDate = null;
                    _arrDate = LrmVM.T_DATE.Split('/');

                    int dt = Int32.Parse(_arrDate[2].ToString());

                    string year = "";
                    if (dt != 0)
                    {
                        year = (dt <= 2000) ? (dt + 543).ToString().Substring(2) : dt.ToString().Substring(2);
                    }
                    else
                    {
                        year = DateTime.Now.Year.ToString().Substring(2);
                    }

                    string _strReport_Name = _arrCon[0].ToString();
                    string _strFormatType = _arrCon[1].ToString();
                    StringBuilder sb = new StringBuilder();
                    List<string> RptParramList = new List<string>();
                    _iniCon.stp = ModConf.ReadIni(_iniCon.iniFile, "DS", _strReport_Name);

                    

                    char[] _sps = new char[] { '|' };
                    char[] _spd = new char[] { '-' };
                    string _tmpdate = string.Empty;
                    string _depRecMOoney = string.Empty;
                    string _department = string.Empty;
                    string _pointRecMoney = string.Empty;

                    string strReport = string.Empty;
                    string _strTempFile = string.Empty;

                    try
                    {


                        string strT_Date = LrmVM.T_DATE.Replace("/", "");

                        if (strT_Date != "")
                        {
                            _tmpdate = strT_Date;
                            strT_Date = year + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
                        }


                        string _strReportPath = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + ".rpt");


                     
                        string _param = string.Empty;

                        DiskFileDestinationOptions dfdopt = new DiskFileDestinationOptions();
                        ReportDocument cryRpt = new ReportDocument();
                        TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                        TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                        ConnectionInfo crConnectionInfo = new ConnectionInfo();
                        Tables CrTables;

                        _logSys.WriteProcessLogFile(_strPathFile, "Server Name :" + _iniCon.Server);
                        _logSys.WriteProcessLogFile(_strPathFile, "DB Name :" + _iniCon.DBTLF);
                        _logSys.WriteProcessLogFile(_strPathFile, "User Name :" + _iniCon.User);
                        _logSys.WriteProcessLogFile(_strPathFile, "Password :" + _iniCon.Password);

                        cryRpt.Load(_strReportPath);

                        crConnectionInfo.ServerName = _iniCon.Server;
                        crConnectionInfo.DatabaseName = _iniCon.DBTLF;
                        crConnectionInfo.UserID = _iniCon.User;
                        crConnectionInfo.Password = _iniCon.Password;

                        CrTables = cryRpt.Database.Tables;
                        foreach (Table CrTable in CrTables)
                        {
                            crtableLogoninfo = CrTable.LogOnInfo;
                            crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                            CrTable.ApplyLogOnInfo(crtableLogoninfo);
                        }

                        _logSys.WriteProcessLogFile(_strPathFile, "Set Log on Database on Crystal Report");
                        _logSys.WriteProcessLogFile(_strPathFile, "Success Log on Database on Crystal Report");


                        _logSys.WriteProcessLogFile(_strPathFile, "Params Report : " + strT_Date);
                        cryRpt.SetParameterValue("@T_DATE", strT_Date);

                        string _strPathTempFiles = @"~\TempFiles\";

                        try
                        {
                            switch (_strFormatType)
                            {
                                case "1":
                                    strReport = _strReport_Name + "_" + _tmpdate + ".pdf";
                                    objCom.deleteFileByReport(strReport, ".pdf");
                                    _strTempFile = Server.MapPath(_strPathTempFiles + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
                                    break;
                                case "2":
                                    strReport = _strReport_Name + "_" + _tmpdate + ".xlsx";
                                    objCom.deleteFileByReport(strReport, ".xlsx");
                                    _strTempFile = Server.MapPath(_strPathTempFiles + strReport);

                                    cryRpt.ExportToDisk(ExportFormatType.XLSXPagebased, _strTempFile);
                                    break;
                                case "3":
                                    strReport = _strReport_Name + "_" + _tmpdate + ".txt";
                                    objCom.deleteFileByReport(strReport, ".txt");
                                    _strTempFile = Server.MapPath(_strPathTempFiles + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
                                    break;

                            }
                        }
                        catch (Exception ex)
                        {
                            _logSys.WriteErrLog(_strPathFile, "Export Report File : " + ex.Message.ToString());


                            list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, LrmVM.SEARCH_KEY);
                            ViewBag.GroupDetailReport = list;
                            lrmOnDemandVM.MESSAGE = _strReport_Name + " ณ วันที่ " + lrmOnDemandVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                            return View("LRM_ONDEMAND", lrmOnDemandVM);
                        }


                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteErrLog(_strPathFile, ex.Message.ToString());


                        list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, LrmVM.SEARCH_KEY);
                        ViewBag.GroupDetailReport = list;
                        lrmOnDemandVM.MESSAGE = _strReport_Name + " ณ วันที่ " + lrmOnDemandVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                        return View("LRM_ONDEMAND", lrmOnDemandVM);
                    }

                    return File(_strTempFile, "application/pdf", strReport);

            }

            return View("LRM_ONDEMAND", lrmOnDemandVM);

        }

    }
}