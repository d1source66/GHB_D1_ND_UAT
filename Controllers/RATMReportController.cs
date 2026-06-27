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
using System.Security.Claims;
using System.Globalization;
using System.Data;

namespace GHB_D1.Controllers
{
    public class RATMReportController : BaseController
    {
        private AccountService _accService = new AccountService();
        string _strGroupNo = "";
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        iniConnection _iniCon = null;
     
        public RATMReportController()
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

        [HttpGet]
        public ActionResult Index(string px, string py, string T_DATE, string F_DATE, string cmdButton, string SEARCH_KEY, string pz, string ToDate, string FrDate)
        {

            ATMViewModel atmVM = new ATMViewModel();
            atmVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            atmVM.F_DATE = (F_DATE == null || F_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : F_DATE;
            atmVM.ToDate = (ToDate == null || ToDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ToDate;
            atmVM.FrDate = (FrDate == null || FrDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : FrDate;
            atmVM.DISPLAY_FILTER = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            atmVM.SEARCH_KEY = SEARCH_KEY;

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();

            //if (Session["UserId"] != null && Session["UserId"].ToString() == py)
            if (Session["UserId"] != null)
            {
                //_strGroupNo = _accService.GetGroupNoReportByName("RATM");
                //list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, SEARCH_KEY);
                var rolename = Session["RoleName"].ToString();
                _strGroupNo = _accService.GetGroupNoReportByNameND("RATM");
                list = _accService.AuthorizeGroupDetailReportND(rolename, py, _strGroupNo, SEARCH_KEY);
                ViewBag.GroupDetailReport = list;
                string searchValue = SEARCH_KEY;
                Session["TitleReport"] = pz;
                atmVM.TITLE_REPORT = pz;
                atmVM.BRANCH_ID = px;
                atmVM.USER_ID = py;
                atmVM.MESSAGE = "";
                if (Session["GroupReport"] != null)
                {
                    ViewBag.GroupReport = Session["GroupReport"];
                }
                return View("RATM", atmVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult GenReport(string px, string py, string T_Date, string cmdButton, string Search, ATMViewModel AtmVM, string SOLCODE)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            AtmVM.T_DATE = AtmVM.ToDate;
            AtmVM.T_DATE = (AtmVM.T_DATE == null || AtmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AtmVM.T_DATE;
            AtmVM.DISPLAY_FILTER = (AtmVM.T_DATE == null || AtmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AtmVM.T_DATE;
            AtmVM.SEARCH_KEY = AtmVM.SEARCH_KEY;
            //_strGroupNo = _accService.GetGroupNoReportByName("RATM");
            //list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, AtmVM.SEARCH_KEY);
            var rolename = Session["RoleName"].ToString();
            _strGroupNo = _accService.GetGroupNoReportByNameND("RATM");
            list = _accService.AuthorizeGroupDetailReportND(rolename, py, _strGroupNo, AtmVM.SEARCH_KEY);
            ViewBag.GroupDetailReport = list;
            if (Session["GroupReport"] != null)
            {
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["BranchId"] != null && Session["BranchId"].ToString() != "")
            {
                string bOndemand = getBranchOndemand(Session["BranchId"].ToString(), "", "ondemand");
                AtmVM.BRANCH_ID = Session["BranchId"].ToString().Substring(0, 3);
            }
            switch (cmdButton)
            {
                case "Search":

                    list = _accService.AuthorizeGroupDetailReport(AtmVM.BRANCH_ID, AtmVM.USER_ID, _strGroupNo, AtmVM.SEARCH_KEY);
                    ViewBag.GroupDetailReport = list;

                    if (list.Count <= 0)
                    {
                        AtmVM.MESSAGE = "ไม่มีข้อมูล";
                    }
                    break;
                default:
                    string[] _arrCon = null;
                    _arrCon = cmdButton.Split('|');
                    string _tmpdate = string.Empty;

                    string[] _arrDate = null;
                    _arrDate = AtmVM.ToDate.Split('/');
                   


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

                    string strT_Date = AtmVM.ToDate.Replace("/", "");

                    if (strT_Date != "")
                    {
                        _tmpdate = strT_Date;
                        strT_Date = year + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
                    }
                
                    StringBuilder sb = new StringBuilder();
                    List<string> RptParramList = new List<string>();

                    char[] _sps = new char[] { '|' };
                    char[] _spd = new char[] { '-' };
                  
                    string _depRecMOoney = string.Empty;
                    string _department = string.Empty;
                    string _pointRecMoney = string.Empty;

                    string strReport = string.Empty;
                    string _strTempFile = string.Empty;
                    string contentType = "";
                    try
                    {
                        if (_strFormatType == "1")
                        {
                            switch (_strReport_Name)
                            {
                                case "RATM9000_M_TOTAL_B001":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;

                            }
                        }


                        if (_strFormatType == "2")
                        {
                            switch (_strReport_Name)
                            {
                                case "BOT001_M":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                                case "RATM9000_M_TOTAL_B001":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                            }
                        }


                        if (_strFormatType == "3")
                        {
                            switch (_strReport_Name)
                            {
                                case "BOT001_M":
                                    _strReport_Name = _strReport_Name + "_TEXT";
                                    break;
                                  
                            }
                        }

                        //==== check existing file
                        // if exist in D:\ReportFile\yyyy\Mmm\dd\D1-CBS-REPORT-CDM\tt.pdf then use it, otherwise regenerate.
                        string yearDir = "", monthDir = "", dayDir = "", t_Date_data = "";
                        yearDir = _arrDate[2]; //format 2026
                        monthDir = "M" + _arrDate[1]; //format M01
                        dayDir = _arrDate[0]; //format 14
                        t_Date_data = strT_Date; //format 260114
                        //folder report to check = D:\ReportFile\2026\M01\14\groupReport
                        string reportToCheckDir = System.IO.Path.Combine(_iniCon.rptPath, yearDir, monthDir, dayDir, "D1-CBS-REPORT");
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
                            //not include ONDEMAND in the list. wait implement.
                        }
                        else
                        {
                            reportNameToCheck = $"{_strReport_Name}.pdf";
                            downloadReportName = $"{_strReport_Name}_{t_Date_data}.pdf";
                            hdrContentType = "application/pdf";
                        }

                        string fullReportNameToCheck = System.IO.Path.Combine(reportToCheckDir, reportNameToCheck);
                        if (System.IO.File.Exists(fullReportNameToCheck))
                        {   //existing file no need generate report

                            return File(fullReportNameToCheck, hdrContentType, downloadReportName);
                        }

                        //===========
                        #region "generated pdf from .rpt"


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
                                cryRpt.SetParameterValue("@T_BRANCH", AtmVM.BRANCH_ID);
                            }
                            else
                            {
                                cryRpt.SetParameterValue("@T_BRANCH", SOLCODE);
                            }
                        }

                        try
                        {
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
                            AtmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + AtmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                            return View("RATM", AtmVM);
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                        AtmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + AtmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                        return View("RATM", AtmVM);
                    }


            }


            return View("RATM", AtmVM);

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