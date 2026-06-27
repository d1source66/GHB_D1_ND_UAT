using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GHB_D1.Code.BAL;
using GHB_D1.Code.Util;
using GHB_D1.Models;
using GHB_D1.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GHB_D1.Controllers
{
    public class LRMReportController : BaseController
    {
        private AccountService _accService = new AccountService();
        string _strGroupNo = "";
        
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        Loger _logSys = new Loger();
        CommonUtilies objCom = new CommonUtilies();
        iniConnection _iniCon = null;
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
      

        public LRMReportController()
        {
            _accService = new AccountService();
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            _iniCon.Server = ModConf.ReadIni(_iniCon.iniFile, "DB", "Server");
            _iniCon.User = ModConf.ReadIni(_iniCon.iniFile, "DB", "User");
            _iniCon.Password = ModConf.ReadIni(_iniCon.iniFile, "DB", "Password");
            _iniCon.DBAMS = ModConf.ReadIni(_iniCon.iniFile, "DB", "DatabaseAMS");
            _iniCon.DBTLF = ModConf.ReadIni(_iniCon.iniFile, "DB", "DatabaseTLF");
            _iniCon.rptPath = ModConf.ReadIni(_iniCon.iniFile, "PathFile", "PathReport");
        }

        // GET: LRMReport
        [HttpGet]
        public ActionResult Index(string px, string py, string T_DATE, string F_DATE, string SEARCH_KEY, string pz, string ToDate, string FrDate)
        {            
            LRMViewModel lrmVM = new LRMViewModel();
            lrmVM.T_DATE = (lrmVM.T_DATE == null || lrmVM.T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : lrmVM.T_DATE;
            lrmVM.F_DATE = (lrmVM.F_DATE == null || lrmVM.F_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : lrmVM.F_DATE;
            lrmVM.ToDate = (ToDate == null || ToDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ToDate;
            lrmVM.FrDate = (FrDate == null || FrDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : FrDate;
            lrmVM.DISPLAY_FILTER = (lrmVM.T_DATE == null || lrmVM.T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : lrmVM.T_DATE;
            lrmVM.SEARCH_KEY = lrmVM.SEARCH_KEY;

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            //if (Session["UserId"] != null && Session["UserId"].ToString() == py && Session["RoleName"].ToString() != null)
            if (Session["UserId"] != null)
            {
                //_strGroupNo = _accService.GetGroupNoReportByName("LRM");
                var rolename = Session["RoleName"].ToString();
                _strGroupNo = _accService.GetGroupNoReportByNameND("LRM");
                list = _accService.AuthorizeGroupDetailReportND(rolename, py, _strGroupNo, lrmVM.SEARCH_KEY);
                //_accService.AuthorizeGroupDetailReport(px, py, _strGroupNo, lrmVM.SEARCH_KEY);
                ViewBag.GroupDetailReport = list;

                lrmVM._lrmVMList = list;

                lrmVM.TITLE_REPORT = pz;
                lrmVM.BRANCH_ID = px;
                lrmVM.USER_ID = py;
                lrmVM.MESSAGE = "";
                if (Session["GroupReport"] != null)
                {
                    ViewBag.GroupReport = Session["GroupReport"];
                }
                return View("LRM", lrmVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult GenReport(string px, string pd, string cmdButton, LRMViewModel lrmVM, string SOLCODE)
        {
            _logSys.WriteProcessLogFile(_strPathFile, $"Begin LRM GenReport (branch Id):{lrmVM.BRANCH_ID} user id:{lrmVM.USER_ID}, T_Date:{lrmVM.ToDate}");
            lrmVM.T_DATE = lrmVM.ToDate;

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            lrmVM.T_DATE = (lrmVM.T_DATE == null || lrmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : lrmVM.T_DATE;
            lrmVM.ToDate = (lrmVM.ToDate == null || lrmVM.ToDate == "") ? DateTime.Now.Date.ToShortDateString() : lrmVM.ToDate;
            lrmVM.FrDate = (lrmVM.FrDate == null || lrmVM.FrDate == "") ? DateTime.Now.Date.ToShortDateString() : lrmVM.FrDate;
            lrmVM.DISPLAY_FILTER = (lrmVM.T_DATE == null || lrmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : lrmVM.T_DATE;
            lrmVM.SEARCH_KEY = lrmVM.SEARCH_KEY;
            //_strGroupNo = _accService.GetGroupNoReportByName("LRM");
            //list = GroupReportBAL.GetGroupDetailReport(px,pd,_strGroupNo, lrmVM.SEARCH_KEY);
            var rolename = Session["RoleName"].ToString();
            _strGroupNo = _accService.GetGroupNoReportByNameND("LRM");
            list = _accService.AuthorizeGroupDetailReportND(rolename, pd, _strGroupNo, lrmVM.SEARCH_KEY);
            ViewBag.GroupDetailReport = list;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["BranchId"] != null && Session["BranchId"].ToString() != "")
            {
                string bOndemand = getBranchOndemand(Session["BranchId"].ToString(), "", "ondemand");
                lrmVM.BRANCH_ID = Session["BranchId"].ToString().Substring(0, 3);
            }
            switch (cmdButton)
            {
                case "Search":
                    list = _accService.AuthorizeGroupDetailReport(lrmVM.BRANCH_ID, lrmVM.USER_ID, _strGroupNo, lrmVM.SEARCH_KEY);
                    ViewBag.GroupDetailReport = list;

                    if (list.Count <= 0)
                    {
                        lrmVM.MESSAGE = "ไม่มีข้อมูล";
                    }
                    break;
                default:

                    string[] _arrCon = null;
                    _arrCon = cmdButton.Split('|');

                    string[] _arrDate = null;
                    _arrDate = lrmVM.ToDate.Split('/');

                    string _strReport_Name = _arrCon[1].ToString(); //_arrCon[0].ToString();
                    string _strFormatType = _arrCon[2].ToString();

                    try
                    {
                        if (_strFormatType == "2")
                        {
                            switch (_strReport_Name)
                            {
                                case "LRM9000_M":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                                case "LRM9000_M_TOTAL":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;                              

                            }
                        }

                        //==== check existing file
                        // if exist in D:\ReportFile\yyyy\Mmm\dd\D1-CBS-REPORT-CDM\tt.pdf then use it, otherwise regenerate.
                        string yearDir = "", monthDir = "", dayDir = "", t_Date_data = "";
                        yearDir = _arrDate[2]; //format 2026
                        monthDir = "M" + _arrDate[1]; //format M01
                        dayDir = _arrDate[0]; //format 14
                        t_Date_data = $"{_arrDate[2].Substring(2, 2)}{_arrDate[1]}{_arrDate[0]}"; //format 260114
                        //folder report to check = D:\ReportFile\2026\M01\14\groupReport
                        string reportToCheckDir = System.IO.Path.Combine(_iniCon.rptPath, yearDir, monthDir, dayDir, "D1-CBS-REPORT-LRM");
                        string reportNameToCheck = "", hdrContentType = "", downloadReportName = "";
                        if (_strReport_Name.Contains("_EXCEL"))
                        {
                            reportNameToCheck = $"{_strReport_Name.Replace("_EXCEL", "")}.xls";
                            downloadReportName = $"{_strReport_Name.Replace("_EXCEL", "")}_{t_Date_data}.xls";
                            hdrContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        }
                        else if (_strReport_Name.Contains("TEXT"))
                        {
                            reportNameToCheck = $"{_strReport_Name.Replace("_TEXT", "")}.txt";
                            downloadReportName = $"{_strReport_Name.Replace("_TEXT", "")}_{t_Date_data}.txt";
                            hdrContentType = "application/txt";
                        }
                        else if (_strReport_Name.StartsWith("PPD"))
                        {
                            reportNameToCheck = $"{_strReport_Name}{_arrDate[2].Substring(2, 2)}{_arrDate[1]}{_arrDate[0]}.pdf";
                            downloadReportName = reportNameToCheck;
                            hdrContentType = "application/pdf";
                        }
                        else if (_strReport_Name.EndsWith("ONDEMAND"))
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "ONDEMAND _strReport_Name: " + _strReport_Name);
                            reportToCheckDir = System.IO.Path.Combine(_iniCon.rptPath, yearDir, monthDir, dayDir, "OnDemandReport");
                            reportNameToCheck = $"{_strReport_Name.Replace("_ONDEMAND", "")}_{lrmVM.BRANCH_ID}_{t_Date_data}.pdf";
                            downloadReportName = reportNameToCheck;
                            hdrContentType = "application/pdf";
                        }
                        else
                        {
                            reportNameToCheck = $"{_strReport_Name}.pdf";
                            downloadReportName = $"{_strReport_Name}_{t_Date_data}.pdf";
                            hdrContentType = "application/pdf";
                        }

                        string fullReportNameToCheck = Path.Combine(reportToCheckDir, reportNameToCheck);
                        if (System.IO.File.Exists(fullReportNameToCheck))
                        {   //existing file no need generate report
                            _logSys.WriteProcessLogFile(_strPathFile, $"found find to download : {fullReportNameToCheck} ");
                            return File(fullReportNameToCheck, hdrContentType, downloadReportName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, $"File {fullReportNameToCheck} not found.");
                            lrmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + lrmVM.DISPLAY_FILTER + " ไม่พบข้อมูล";
                        }
                        break; //break case default

                        #region "generated pdf from .rpt"
                        /**
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
                        if (_strReport_Name.Contains("ONDEMAND"))
                        {
                            if (SOLCODE == "" || SOLCODE == null)
                            {
                                cryRpt.SetParameterValue("@T_BRANCH", lrmVM.BRANCH_ID);
                            }
                            else
                            {
                                cryRpt.SetParameterValue("@T_BRANCH", SOLCODE);
                            }
                        }

                        string _strPathTempFiles = @"~\TempFiles\";

                        try
                        {

                            switch (_strFormatType)
                            {
                                case "1":
                                    strReport = _strReport_Name + "-" + strT_Date + ".pdf";
                                    objCom.deleteFileByReport(strReport, ".pdf");
                                    _strTempFile = Server.MapPath(_strPathTempFiles + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
                                    contentType = "application/pdf";
                                    break;
                                case "2":
                                    strReport = _strReport_Name + "-" + strT_Date + ".xls";
                                    objCom.deleteFileByReport(strReport, ".xls");
                                    _strTempFile = Server.MapPath(_strPathTempFiles + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.Excel, _strTempFile);
                                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    break;
                                case "3":
                                    strReport = _strReport_Name + "-" + strT_Date + ".txt";
                                    objCom.deleteFileByReport(strReport, ".txt");
                                    _strTempFile = Server.MapPath(_strPathTempFiles + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
                                    contentType = "application/txt";
                                    break;

                            }
                        
                        }
                        catch (Exception ex)
                        {
                            _logSys.WriteErrLog(_strPathFile, "Export Report File : " + ex.Message.ToString());
                            lrmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + lrmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                            return View("LRM", lrmVM);
                        }
                        **/
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteErrLog(_strPathFile, ex.Message.ToString());
                        lrmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + lrmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                        return View("LRM", lrmVM);
                    }
            }

            return View("LRM", lrmVM);

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