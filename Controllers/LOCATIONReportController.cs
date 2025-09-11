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
    public class LOCATIONReportController : BaseController
    {
        // GET: LOCATIONReport
        private AccountService _accService = new AccountService();
        string _strGroupNo = "";     
        Loger _logSys = new Loger();
        CommonUtilies objCom = new CommonUtilies();
        iniConnection _iniCon = null;
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");

        public LOCATIONReportController()
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
        [HttpGet]
        public ActionResult Index(string px, string py, string T_DATE, string F_DATE, string SEARCH_KEY, string pz, string ToDate, string FrDate)
        {
           
            LocationViewModel locationVM = new LocationViewModel();
            locationVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            locationVM.F_DATE = (F_DATE == null || F_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : F_DATE;
            locationVM.ToDate = (ToDate == null || ToDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ToDate;
            locationVM.FrDate = (FrDate == null || FrDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : FrDate;
            locationVM.DISPLAY_FILTER = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            locationVM.SEARCH_KEY = SEARCH_KEY;

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            if (Session["UserId"] != null && Session["UserId"].ToString() == py)
            {
                //_strGroupNo = _accService.GetGroupNoReportByName("LOCATION");
                //list = _accService.AuthorizeGroupDetailReport(px, py, _strGroupNo, SEARCH_KEY);
                var rolename = Session["RoleName"].ToString();
                _strGroupNo = _accService.GetGroupNoReportByNameND("LOCATION");
                list = _accService.AuthorizeGroupDetailReportND(rolename, py, _strGroupNo, SEARCH_KEY);
                ViewBag.GroupDetailReport = list;

                Session["TitleReport"] = pz;
                locationVM.TITLE_REPORT = pz;
                locationVM.BRANCH_ID = px;
                locationVM.USER_ID = py;
                locationVM.MESSAGE = "";
                if (Session["GroupReport"] != null)
                {
                    ViewBag.GroupReport = Session["GroupReport"];
                }
                return View("LOCATION", locationVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult GenReport(string px, string py, string cmdButton, LocationViewModel locationVM)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            locationVM.T_DATE = locationVM.ToDate;
            locationVM.T_DATE = (locationVM.T_DATE == null || locationVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : locationVM.T_DATE;
            locationVM.DISPLAY_FILTER = (locationVM.T_DATE == null || locationVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : locationVM.T_DATE;
            locationVM.SEARCH_KEY = locationVM.SEARCH_KEY;
            //_strGroupNo = _accService.GetGroupNoReportByName("LOCATION");
            //list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, locationVM.SEARCH_KEY);
            var rolename = Session["RoleName"].ToString();
            _strGroupNo = _accService.GetGroupNoReportByNameND("LOCATION");
            list = _accService.AuthorizeGroupDetailReportND(rolename, py, _strGroupNo, locationVM.SEARCH_KEY);
            ViewBag.GroupDetailReport = list;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            switch (cmdButton)
            {
                case "Search":
                    list = _accService.AuthorizeGroupDetailReport(locationVM.BRANCH_ID, locationVM.USER_ID, _strGroupNo, locationVM.SEARCH_KEY);
                    ViewBag.GroupDetailReport = list;

                    if (list.Count <= 0)
                    {
                        locationVM.MESSAGE = "ไม่มีข้อมูล";
                    }
                    break;
                default:
                    _strPathFile = Server.MapPath(@"~\Logs\");
                    List<string> RptParramList = new List<string>();

                    char[] _sps = new char[] { '|' };
                    char[] _spd = new char[] { '-' };
                    string _tmpdate = string.Empty;
                    string _depRecMOoney = string.Empty;
                    string _department = string.Empty;
                    string _pointRecMoney = string.Empty;

                    string strReport = string.Empty;
                    string _strTempFile = string.Empty;
                    string contentType = "";
                    try
                    {
                        string[] _arrCon = null;
                        _arrCon = cmdButton.Split('|');
                        string _strT_Date = DateTime.Parse(locationVM.T_DATE).ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                        string _strReport_Name = _arrCon[1].ToString();
                        string _strFormatType = _arrCon[2].ToString();
                        string strT_Date = _strT_Date.Replace("/", "");

                        if (strT_Date != "")
                        {
                            _tmpdate = strT_Date;

                            strT_Date = _tmpdate.Substring(6, 2) + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
                        }



                        string _strReportPath = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + ".rpt");                       
                        string _param = string.Empty;

                        DiskFileDestinationOptions dfdopt = new DiskFileDestinationOptions();
                        ReportDocument cryRpt = new ReportDocument();
                        TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                        TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                        ConnectionInfo crConnectionInfo = new ConnectionInfo();
                        Tables CrTables;
                      

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
                                    contentType = "application/pdf";
                                    break;
                                case "2":
                                    strReport = _strReport_Name + "_" + _tmpdate + ".xls";
                                    objCom.deleteFileByReport(strReport, ".xls");
                                    _strTempFile = Server.MapPath(_strPathTempFiles + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.Excel, _strTempFile);
                                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    break;
                                case "3":
                                    strReport = _strReport_Name + "_" + _tmpdate + ".txt";
                                    objCom.deleteFileByReport(strReport, ".txt");
                                    _strTempFile = Server.MapPath(_strPathTempFiles + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
                                    contentType = "application/txt";
                                    break;

                            }

                     
                            return File(_strTempFile, contentType, strReport);
                        }
                        catch (Exception ex)
                        {
                            _logSys.WriteErrLog(_strPathFile, "Export Report File : " + ex.Message.ToString());


                            locationVM.MESSAGE = "ไม่มีข้อมูล";
                            return View("LOCATION", locationVM);

                        }


                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteErrLog(_strPathFile, ex.Message.ToString());


                        locationVM.MESSAGE = "ไม่มีข้อมูล";
                        return View("LOCATION", locationVM);
                    }



            }

            return View("LOCATION", locationVM);

        }
    }
}