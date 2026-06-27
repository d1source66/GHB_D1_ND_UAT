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
    public class ADMReportController : BaseController
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
        CommonUtilies objCom = new CommonUtilies();
        public ADMReportController()
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

        public ActionResult Index(string px, string py, string T_DATE, string F_DATE, string SEARCH_KEY, string pz, string ToDate, string FrDate)
        {
            ADMViewModel admVM = new ADMViewModel();
            admVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            admVM.F_DATE = (F_DATE == null || F_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : F_DATE;
            admVM.ToDate = (ToDate == null || ToDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ToDate;
            admVM.FrDate = (FrDate == null || FrDate == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : FrDate;
            admVM.DISPLAY_FILTER = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : F_DATE;
            admVM.SEARCH_KEY = SEARCH_KEY;
            _logSys.WriteProcessLogFile(_strPathFile, "T_DATE : " + admVM.T_DATE);
            _logSys.WriteProcessLogFile(_strPathFile, "DISPLAY_FILTER : " + admVM.DISPLAY_FILTER);

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();

            if (Session["UserId"] != null && Session["UserId"].ToString() == py)
            {
                //_strGroupNo = _accService.GetGroupNoReportByName("ADM");
                //list = _accService.AuthorizeGroupDetailReport(px, py, _strGroupNo, SEARCH_KEY);
                var rolename = Session["RoleName"].ToString();
                _strGroupNo = _accService.GetGroupNoReportByNameND("ADM");
                list = _accService.AuthorizeGroupDetailReportND(rolename, py, _strGroupNo, SEARCH_KEY);
                ViewBag.GroupDetailReport = list;

                Session["TitleReport"] = pz;
                admVM.TITLE_REPORT = pz;
                admVM.BRANCH_ID = px;
                admVM.USER_ID = py;
                admVM.MESSAGE = "";
                if (Session["GroupReport"] != null)
                {
                    ViewBag.GroupReport = Session["GroupReport"];
                }
                return View("ADM", admVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult GenReport(string px, string pd, string cmdButton, ADMViewModel AdmVM, string SOLCODE)
        {
            _logSys.WriteProcessLogFile(_strPathFile, $"Begin ADM GenReport (branch Id):{AdmVM.BRANCH_ID} user id:{AdmVM.USER_ID}");
            string t_date = AdmVM.ToDate; //user select date in date-picker.

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            List<ADMModel> _admdata = new List<ADMModel>();
            DataTable _dt = new DataTable();

            AdmVM.DISPLAY_FILTER = t_date;
            AdmVM.SEARCH_KEY = AdmVM.SEARCH_KEY;
            var rolename = Session["RoleName"].ToString();
            _strGroupNo = _accService.GetGroupNoReportByNameND("ADM");
            list = _accService.AuthorizeGroupDetailReportND(rolename, pd, _strGroupNo, AdmVM.SEARCH_KEY);
            //list = GroupReportBAL.GetGroupDetailReport(px, pd, _strGroupNo, AdmVM.SEARCH_KEY);
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
                    _logSys.WriteProcessLogFile(_strPathFile, "cmdButton.Split('|') : " + _arrCon.Length);

                    string _strTermId = _arrCon[0].ToString();
                    string _strReport_Name = _arrCon[1].ToString();
                    string _strFormatType = _arrCon[2].ToString();
                    StringBuilder sb = new StringBuilder();
                    List<string> RptParramList = new List<string>();
                    _iniCon.stp = ModConf.ReadIni(_iniCon.iniFile, "DS", _strReport_Name);

                    string strReport = string.Empty;
                    string _strTempFile = string.Empty;

                    try
                    {
                        if (_strFormatType == "2")
                        {
                            switch (_strReport_Name)
                            {
                                case "ADM9000_M":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                                case "ADM9000_M_TOTAL":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                                case "ADM9000_Q_TOTAL":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;
                                case "ADM9000_Y_TOTAL":
                                    _strReport_Name = _strReport_Name + "_EXCEL";
                                    break;

                            }
                        }

                        //==== check existing file
                        // if exist in D:\ReportFile\yyyy\Mmm\dd\D1-CBS-REPORT-CDM\tt.pdf then use it, otherwise regenerate.
                        string yearDir = "", monthDir = "", dayDir = "", t_Date_data = "";
                        string[] _arrDate = t_date.Split('/');
                        yearDir = _arrDate[2]; //format 2026
                        monthDir = "M" + _arrDate[1]; //format M01
                        dayDir = _arrDate[0]; //format 14
                        t_Date_data = $"{_arrDate[2].Substring(2,2)}{_arrDate[1]}{_arrDate[0]}"; //format 260114
                                                 //folder report to check = D:\ReportFile\2026\M01\14\groupReport

                        _iniCon.rptPath = ModConf.ReadIni(_iniCon.iniFile, "PathFile", "PathReport");
                        string reportToCheckDir = System.IO.Path.Combine(_iniCon.rptPath, yearDir, monthDir, dayDir, "D1-CBS-REPORT-ADM");
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
                            reportNameToCheck = $"{_strReport_Name.Replace("_ONDEMAND", "")}_{AdmVM.BRANCH_ID}_{t_Date_data}.pdf";
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
                            AdmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + AdmVM.DISPLAY_FILTER + " ไม่พบข้อมูล";
                        }
                        #region "generated pdf from .rpt"
                        //===========
                        /**
                        string _strReportPath = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + ".rpt");
                        _logSys.WriteProcessLogFile(_strPathFile, "_strReportPath1(176) : " + _strReportPath);

                        string _param = string.Empty;

                        DiskFileDestinationOptions dfdopt = new DiskFileDestinationOptions();
                        ReportDocument cryRpt = new ReportDocument();

                        _logSys.WriteProcessLogFile(_strPathFile, "check _iniCon.store : " + _iniCon.stp);
                        //check report use dataset
                        if (_iniCon.stp == "" || _iniCon.stp == null)
                        {
                            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                            ConnectionInfo crConnectionInfo = new ConnectionInfo();
                            Tables CrTables;

                            _logSys.WriteProcessLogFile(_strPathFile, "Server : " + _iniCon.Server);
                            _logSys.WriteProcessLogFile(_strPathFile, "DatabaseName : " + _iniCon.DBTLF);
                            _logSys.WriteProcessLogFile(_strPathFile, "UserID : " + _iniCon.User);
                            _logSys.WriteProcessLogFile(_strPathFile, "PWD : " + _iniCon.Password);



                            crConnectionInfo.ServerName = _iniCon.Server;// ReportServer;
                            crConnectionInfo.DatabaseName = _iniCon.DBTLF;// ReportDataBase;
                            crConnectionInfo.UserID = _iniCon.User;// ReportUserName;
                            crConnectionInfo.Password = _iniCon.Password;// ReportPwd;
                            cryRpt.Load(_strReportPath);
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
                                    _logSys.WriteProcessLogFile(_strPathFile, "GetBranch detail(AdmVM.BRANCH_ID) : " + AdmVM.BRANCH_ID);
                                    cryRpt.SetParameterValue("@T_BRANCH", AdmVM.BRANCH_ID);
                                }
                                else
                                {
                                    _logSys.WriteProcessLogFile(_strPathFile, "GetBranch detail(SOLCODE) : " + SOLCODE);
                                    cryRpt.SetParameterValue("@T_BRANCH", SOLCODE);
                                }
                            }

                        }
                        else
                        {
                            string _strReportPath_test = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + "_ds.rpt");
                            _logSys.WriteProcessLogFile(_strPathFile, "_strReportPath_test(216) : " + _strReportPath_test);
                            _dt = _ds.Tables["ADMrpt"];
                            _logSys.WriteProcessLogFile(_strPathFile, "BRANCH_ID : " + AdmVM.BRANCH_ID);
                            _logSys.WriteProcessLogFile(_strPathFile, "store : " + _iniCon.stp);
                            _logSys.WriteProcessLogFile(_strPathFile, "T_DATE : " + strT_Date);
                            _admdata = _comBal.GetADMDataByStoreBal(AdmVM.BRANCH_ID, _iniCon.stp, strT_Date, _strTermId);
                            foreach (ADMModel data in _admdata)
                            {
                                try
                                {
                                    DataRow _dr = _dt.NewRow();
                                    _dr["T_BRCH_ID"] = data.T_BRCH_ID;
                                    _dr["BRCH_NAME"] = data.BRCH_NAME;
                                    _dr["WSID"] = data.WSID;
                                    _dr["T_TERM_T_TERM_ID"] = data.T_TERM_T_TERM_ID;
                                    _dr["T_TERM_NAME_LOC"] = data.T_TERM_NAME_LOC;
                                    _dr["T_TRAN_DAT"] = data.T_TRAN_DAT;
                                    _dr["T_TRAN_TIM"] = data.T_TRAN_TIM;
                                    _dr["T_CRD_T_PAN"] = data.T_CRD_T_PAN;
                                    _dr["T_T_CDE"] = data.T_T_CDE;
                                    _dr["T_T_FROM"] = data.T_T_FROM;
                                    _dr["T_T_TO"] = data.T_T_TO;
                                    _dr["T_FROM_ACCT"] = data.T_FROM_ACCT;
                                    _dr["T_TO_ACCT"] = data.T_TO_ACCT;
                                    _dr["T_MULT_ACCT"] = data.T_MULT_ACCT;
                                    _dr["T_DEP_TYP"] = data.T_DEP_TYP;
                                    _dr["REQ_AMT"] = data.REQ_AMT;
                                    _dr["ACT_AMT"] = data.ACT_AMT;
                                    _dr["FEE_AMT"] = data.FEE_AMT;
                                    _dr["RV"] = data.RV;
                                    _dr["RP"] = data.RP;
                                    _dr["T_SEQ_NUM"] = data.T_SEQ_NUM;
                                    _dr["BILL_CNT"] = data.BILL_CNT;
                                    _dr["BILL_CNT_CDM"] = data.BILL_CNT_CDM;
                                    _dr["T_CRD_T_FIID"] = data.T_CRD_T_FIID;
                                    _dr["T_RESP_BYTE_2"] = data.T_RESP_BYTE_2;
                                    _dr["FLAG_REVERSE"] = data.FLAG_REVERSE;
                                    _dr["BK_C"] = data.BK_C;
                                    _dr["CNT"] = data.CNT;
                                    _dr["ITEM05"] = data.ITEM05;
                                    _dr["HOPR_END"] = data.HOPR_END;
                                    _dr["TERM_ID05"] = data.TERM_ID05;
                                    _dr["ITEM03"] = data.ITEM03;
                                    _dr["HOPR1_INCR"] = data.HOPR1_INCR;
                                    _dr["HOPR2_INCR"] = data.HOPR2_INCR;
                                    _dr["HOPR3_INCR"] = data.HOPR3_INCR;
                                    _dr["HOPR4_INCR"] = data.HOPR4_INCR;
                                    _dr["TERM_ID03"] = data.TERM_ID03;
                                    _dr["ITEM07"] = data.ITEM07;
                                    _dr["HOPR1_DECR"] = data.HOPR1_DECR;
                                    _dr["HOPR2_DECR"] = data.HOPR2_DECR;
                                    _dr["HOPR3_DECR"] = data.HOPR3_DECR;
                                    _dr["HOPR4_DECR"] = data.HOPR4_DECR;
                                    _dr["TERM_ID07"] = data.TERM_ID07;
                                    _dr["ITEM09"] = data.ITEM09;
                                    _dr["HOPR1_END"] = data.HOPR1_END;
                                    _dr["HOPR2_END"] = data.HOPR2_END;
                                    _dr["HOPR3_END"] = data.HOPR3_END;
                                    _dr["HOPR4_END"] = data.HOPR4_END;
                                    _dr["TERM_ID09"] = data.TERM_ID09;
                                    _dr["spSP"] = data.spSP;
                                    _dr["spFL"] = data.spFL;
                                    _dt.Rows.Add(_dr);
                                }
                                catch (Exception ex)
                                {
                                    _logSys.WriteProcessLogFile(_strPathFile, "GenRptDS : " + ex.Message.ToString());
                                    throw ex;
                                }
                            }

                            _logSys.WriteProcessLogFile(_strPathFile, "check _dt(290) : " + _dt.Rows.Count);
                            _logSys.WriteProcessLogFile(_strPathFile, "@T_DATE(291) : " + strT_Date);

                            cryRpt.Load(_strReportPath_test);
                            cryRpt.SetDataSource(_dt);
                            cryRpt.SetParameterValue("@T_DATE", strT_Date);

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
                            return View("ADM", AdmVM);
                        }

                        **/
                        #endregion
                        break;
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                        AdmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + AdmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                        _logSys.WriteProcessLogFile(_strPathFile, "Switch case default(ex) : " + ex.Message.ToString());
                        return View("ADM", AdmVM);
                    }
            }
            return View("ADM", AdmVM);

        }

        #region oldlogic
        //[HttpPost]
        //public ActionResult GenReport(string cmdButton, ADMViewModel AdmVM)
        //{
        //    List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
        //    List<ADMModel> _admdata = new List<ADMModel>();
        //    DataTable _dt = new DataTable();

        //    AdmVM.T_DATE = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE.ToString();
        //    AdmVM.DISPLAY_FILTER = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE;
        //    AdmVM.SEARCH_KEY = AdmVM.SEARCH_KEY;

        //    list = GroupReportBAL.GetGroupDetailReport(_strGroupNo, AdmVM.SEARCH_KEY);
        //    ViewBag.GroupDetailReport = list;

        //    switch (cmdButton)
        //    {
        //        case "Search":
        //            list = _accService.AuthorizeGroupDetailReport(AdmVM.BRANCH_ID, AdmVM.USER_ID, _strGroupNo, AdmVM.SEARCH_KEY);
        //            ViewBag.GroupDetailReport = list;

        //            if (list.Count <= 0)
        //            {
        //                AdmVM.MESSAGE = "ไม่มีข้อมูล";
        //            }
        //            break;
        //        default:

        //            string[] _arrCon = null;
        //            _arrCon = cmdButton.Split('|');


        //            DateTime dt = DateTime.Parse(AdmVM.T_DATE);
        //            string year = "";
        //            if (dt != null)
        //            {
        //                year = (dt.Year <= 2000)? (dt.Year + 543).ToString().Substring(2) : dt.Year.ToString().Substring(2);
        //            }
        //            else
        //            {
        //                year = DateTime.Now.Year.ToString().Substring(2);
        //            }

        //            string _strT_Date = DateTime.Parse(AdmVM.T_DATE).ToString("dd/MM/yyyy");

        //            string _strTermId = _arrCon[0].ToString();
        //            string _strReport_Name = _arrCon[1].ToString();
        //            string _strFormatType = _arrCon[2].ToString();
        //            StringBuilder sb = new StringBuilder();
        //            List<string> RptParramList = new List<string>();
        //            _iniCon.stp = ModConf.ReadIni(_iniCon.iniFile, "DS", _strReport_Name);

        //            char[] _sps = new char[] { '|' };
        //            char[] _spd = new char[] { '-' };
        //            string _tmpdate = string.Empty;
        //            string _depRecMOoney = string.Empty;
        //            string _department = string.Empty;
        //            string _pointRecMoney = string.Empty;

        //            string strReport = string.Empty;
        //            string _strTempFile = string.Empty;

        //            try
        //            {
        //                string strT_Date = _strT_Date.Replace("/", "");

        //                if (strT_Date != "")
        //                {
        //                    _tmpdate = strT_Date;
        //                    strT_Date = year + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
        //                }

        //                string _strReportPath = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + ".rpt");

        //                string _param = string.Empty;

        //                DiskFileDestinationOptions dfdopt = new DiskFileDestinationOptions();
        //                ReportDocument cryRpt = new ReportDocument();

        //                //check report use dataset
        //                if (_iniCon.stp == "" || _iniCon.stp == null)
        //                {
        //                    TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
        //                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
        //                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
        //                    Tables CrTables;


        //                    cryRpt.Load(_strReportPath);

        //                    crConnectionInfo.ServerName = _iniCon.Server;// ReportServer;
        //                    crConnectionInfo.DatabaseName = _iniCon.DBTLF;// ReportDataBase;
        //                    crConnectionInfo.UserID = _iniCon.User;// ReportUserName;
        //                    crConnectionInfo.Password = _iniCon.Password;// ReportPwd;

        //                    CrTables = cryRpt.Database.Tables;
        //                    foreach (Table CrTable in CrTables)
        //                    {
        //                        crtableLogoninfo = CrTable.LogOnInfo;
        //                        crtableLogoninfo.ConnectionInfo = crConnectionInfo;
        //                        CrTable.ApplyLogOnInfo(crtableLogoninfo);
        //                    }


        //                    cryRpt.SetParameterValue("@T_DATE", strT_Date);
        //                }
        //                else
        //                {
        //                    string _strReportPath_test = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + "_ds.rpt");
        //                    _dt = _ds.Tables["ADMrpt"];
        //                    _admdata = _comBal.GetADMDataByStoreBal(AdmVM.BRANCH_ID,_iniCon.stp, strT_Date, _strTermId);
        //                    foreach (ADMModel data in _admdata)
        //                    {
        //                        try
        //                        {
        //                            DataRow _dr = _dt.NewRow();
        //                            _dr["T_BRCH_ID"] = data.T_BRCH_ID;
        //                            _dr["BRCH_NAME"] = data.BRCH_NAME;
        //                            _dr["WSID"] = data.WSID;
        //                            _dr["T_TERM_T_TERM_ID"] = data.T_TERM_T_TERM_ID;
        //                            _dr["T_TERM_NAME_LOC"] = data.T_TERM_NAME_LOC;
        //                            _dr["T_TRAN_DAT"] = data.T_TRAN_DAT;
        //                            _dr["T_TRAN_TIM"] = data.T_TRAN_TIM;
        //                            _dr["T_CRD_T_PAN"] = data.T_CRD_T_PAN;
        //                            _dr["T_T_CDE"] = data.T_T_CDE;
        //                            _dr["T_T_FROM"] = data.T_T_FROM;
        //                            _dr["T_T_TO"] = data.T_T_TO;
        //                            _dr["T_FROM_ACCT"] = data.T_FROM_ACCT;
        //                            _dr["T_TO_ACCT"] = data.T_TO_ACCT;
        //                            _dr["T_MULT_ACCT"] = data.T_MULT_ACCT;
        //                            _dr["T_DEP_TYP"] = data.T_DEP_TYP;
        //                            _dr["REQ_AMT"] = data.REQ_AMT;
        //                            _dr["ACT_AMT"] = data.ACT_AMT;
        //                            _dr["FEE_AMT"] = data.FEE_AMT;
        //                            _dr["RV"] = data.RV;
        //                            _dr["RP"] = data.RP;
        //                            _dr["T_SEQ_NUM"] = data.T_SEQ_NUM;
        //                            _dr["BILL_CNT"] = data.BILL_CNT;
        //                            _dr["BILL_CNT_CDM"] = data.BILL_CNT_CDM;
        //                            _dr["T_CRD_T_FIID"] = data.T_CRD_T_FIID;
        //                            _dr["T_RESP_BYTE_2"] = data.T_RESP_BYTE_2;
        //                            _dr["FLAG_REVERSE"] = data.FLAG_REVERSE;
        //                            _dr["BK_C"] = data.BK_C;
        //                            _dr["CNT"] = data.CNT;
        //                            _dr["ITEM05"] = data.ITEM05;
        //                            _dr["HOPR_END"] = data.HOPR_END;
        //                            _dr["TERM_ID05"] = data.TERM_ID05;
        //                            _dr["ITEM03"] = data.ITEM03;
        //                            _dr["HOPR1_INCR"] = data.HOPR1_INCR;
        //                            _dr["HOPR2_INCR"] = data.HOPR2_INCR;
        //                            _dr["HOPR3_INCR"] = data.HOPR3_INCR;
        //                            _dr["HOPR4_INCR"] = data.HOPR4_INCR;
        //                            _dr["TERM_ID03"] = data.TERM_ID03;
        //                            _dr["ITEM07"] = data.ITEM07;
        //                            _dr["HOPR1_DECR"] = data.HOPR1_DECR;
        //                            _dr["HOPR2_DECR"] = data.HOPR2_DECR;
        //                            _dr["HOPR3_DECR"] = data.HOPR3_DECR;
        //                            _dr["HOPR4_DECR"] = data.HOPR4_DECR;
        //                            _dr["TERM_ID07"] = data.TERM_ID07;
        //                            _dr["ITEM09"] = data.ITEM09;
        //                            _dr["HOPR1_END"] = data.HOPR1_END;
        //                            _dr["HOPR2_END"] = data.HOPR2_END;
        //                            _dr["HOPR3_END"] = data.HOPR3_END;
        //                            _dr["HOPR4_END"] = data.HOPR4_END;
        //                            _dr["TERM_ID09"] = data.TERM_ID09;
        //                            _dr["spSP"] = data.spSP;
        //                            _dr["spFL"] = data.spFL;
        //                            _dt.Rows.Add(_dr);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            throw ex;
        //                        }
        //                    }
        //                    cryRpt.Load(_strReportPath_test);
        //                    cryRpt.SetDataSource(_dt);
        //                    cryRpt.SetParameterValue("@T_DATE", strT_Date);
        //                }
        //                try
        //                {
        //                    string contentType = "";
        //                    //switch (_strFormatType)
        //                    //{
        //                    //    case "1":
        //                    //        strReport = _strReport_Name + "-" + strT_Date + ".pdf";
        //                    //        _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
        //                    //        cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
        //                    //        contentType = "application/pdf";
        //                    //        break;
        //                    //    case "2":
        //                    //        strReport = _strReport_Name + "-" + strT_Date + ".xls";
        //                    //        _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
        //                    //        cryRpt.ExportToDisk(ExportFormatType.Excel, _strTempFile);
        //                    //        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                    //        break;
        //                    //    case "3":
        //                    //        strReport = _strReport_Name + "-" + strT_Date + ".txt";
        //                    //        _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
        //                    //        cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
        //                    //        contentType = "application/txt";
        //                    //        break;

        //                    //}



        //                    /*Check main folder*/
        //                    var CreateFolderMain = GetCreateMyFolder(ReportPathFiles);
        //                    if (CreateFolderMain.Exists)
        //                    {
        //                        string runDate = _tmpdate.Substring(4, 4) + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
        //                        string _pathReportFile = ReportPathFiles + "/" + runDate + "/" + "D1-CBS-REPORT-" + "ADM";
        //                        var CreateForderTrue = GetCreateMyFolder(_pathReportFile);
        //                        string ReportFilePath = CreateForderTrue.FullName;
        //                     //   string contentType = "";
        //                        if (CreateForderTrue.Exists)
        //                        {                                    
        //                            switch (_strFormatType)
        //                            {
        //                                case "1":
        //                                    strReport = _strReport_Name + ".pdf";
        //                                    _strTempFile = _pathReportFile + @"\" + strReport;
        //                                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
        //                                    contentType = "application/pdf";
        //                                    break;
        //                                case "2":
        //                                    strReport = _strReport_Name + ".xls";
        //                                    _strTempFile = _pathReportFile + @"\" + strReport;
        //                                    cryRpt.ExportToDisk(ExportFormatType.Excel, _strTempFile);
        //                                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";                                          
        //                                    break;
        //                                case "3":
        //                                    strReport = _strReport_Name + ".txt";
        //                                    _strTempFile = _pathReportFile + @"\" + strReport;
        //                                    cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
        //                                    contentType = "application/txt";                                           
        //                                    break;

        //                            }
        //                        }
        //                    }


        //                    return View("ADM", AdmVM);
        //                    //return File(_strTempFile, contentType, strReport);
        //                }
        //                catch (Exception ex)
        //                {
        //                    ex.Message.ToString();
        //                    AdmVM.MESSAGE = "ไม่มีข้อมูล";
        //                    return View("ADM", AdmVM);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.Message.ToString();
        //                AdmVM.MESSAGE = "ไม่มีข้อมูล";
        //                return View("ADM", AdmVM);
        //            }
        //    }
        //    return View("ADM", AdmVM);

        //}

        //public static DirectoryInfo GetCreateMyFolder(string baseFolder)
        //{
        //    return Directory.CreateDirectory(baseFolder);
        //}
        #endregion

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



