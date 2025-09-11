using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GHB_D1.Code.BAL;
using System.Text;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Web.Configuration;
using GHB_D1.Models;
using System;
using GHB_D1.ReportFiles.DataSet;
using System.Data;
using GHB_D1.Code.Util;
using GHB_D1.Services;
using System.Globalization;

namespace GHB_D1.Controllers
{
    public class ADM_ONDEMANDReportController : BaseController
    {
        private AccountService _accService = new AccountService();
        iniConnection _iniCon = null;
        dsGenReport _ds = null;
        CommondBAL _comBal = null;
        //string ReportServer = WebConfigurationManager.AppSettings["ReportServer"].ToString();
        //string ReportDataBase = WebConfigurationManager.AppSettings["ReportDataBase"].ToString();
        //string ReportUserName = WebConfigurationManager.AppSettings["ReportUserName"].ToString();
        //string ReportPwd = WebConfigurationManager.AppSettings["ReportPwd"].ToString();
        string _strGroupNo = "";
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        public ADM_ONDEMANDReportController()
        {
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
            _accService = new AccountService();
        }

        [HttpGet]
        public ActionResult Index(string px, string py, string T_DATE, string cmdButton, string SEARCH_KEY, string pz)
        {

            ADMOnDemandViewModel admOnDemandVM = new ADMOnDemandViewModel();
            admOnDemandVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            admOnDemandVM.DISPLAY_FILTER = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            admOnDemandVM.SEARCH_KEY = SEARCH_KEY;

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            if (Session["UserId"] != null && Session["UserId"].ToString() == py)
            {
                _strGroupNo = _accService.GetGroupNoReportByName("ADM_ONDEMAND");
                list = GroupReportBAL.GetGroupDetailReport(px,py,_strGroupNo, SEARCH_KEY);
                ViewBag.GroupDetailReport = list;
                string searchValue = SEARCH_KEY;
                Session["TitleReport"] = pz;
                admOnDemandVM.TITLE_REPORT = pz;
                admOnDemandVM.BRANCH_ID = px;
                admOnDemandVM.USER_ID = py;
                admOnDemandVM.MESSAGE = "";

                return View("ADMOnDemand", admOnDemandVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }


        [HttpPost]
        public ActionResult GenReport(string px, string pd, string cmdButton, ADMOnDemandViewModel AdmVM)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            List<ADMModel> _admdata = new List<ADMModel>();
            DataTable _dt = new DataTable();
            _strGroupNo = _accService.GetGroupNoReportByName("ADM_ONDEMAND");
            _logSys.WriteProcessLogFile(_strPathFile, "GenReport T_Date : " + AdmVM.T_DATE);
            _logSys.WriteProcessLogFile(_strPathFile, "GenReport SEARCH_KEY : " + AdmVM.SEARCH_KEY);
            _logSys.WriteProcessLogFile(_strPathFile, "GetGroupDetailReport : " + _strGroupNo);

            AdmVM.T_DATE = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE;
            AdmVM.DISPLAY_FILTER = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE;
            AdmVM.SEARCH_KEY = AdmVM.SEARCH_KEY;

            list = GroupReportBAL.GetGroupDetailReport(px,pd,_strGroupNo, AdmVM.SEARCH_KEY);
            ViewBag.GroupDetailReport = list;
            _logSys.WriteProcessLogFile(_strPathFile, "GetGroupDetailReport_List : " + list.Count);

            //ADMOnDemandViewModel admOnDemandVM = new ADMOnDemandViewModel();
            //admOnDemandVM.T_DATE = (T_Date == null || T_Date == "") ? DateTime.Now.Date.ToShortDateString() : T_Date;
            //admOnDemandVM.DISPLAY_FILTER = (T_Date == null || T_Date == "") ? DateTime.Now.Date.ToShortDateString() : T_Date;
            //admOnDemandVM.SEARCH_KEY = Search;

            //list = GroupReportBAL.GetGroupDetailReport(_strGroupNo, admOnDemandVM.SEARCH_KEY);
            //ViewBag.GroupDetailReport = list;

            //branchID = (branchID != null && branchID != "")? branchID.PadLeft(4,'0').ToString() :"0001";
            switch (cmdButton)
            {
                case "Search":

                    list = GroupReportBAL.GetGroupDetailReport(px,pd,_strGroupNo, AdmVM.SEARCH_KEY);
                    ViewBag.GroupDetailReport = list;
                    if (list.Count <= 0)
                    {
                        AdmVM.MESSAGE = "ไม่มีข้อมูล";
                    }
                    break;
                default:
                    string[] _arrCon = null;
                    _arrCon = cmdButton.Split('|');                
                    string[] _arrDate = null;
                    _arrDate = AdmVM.T_DATE.Split('/');
                  
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
                        string strT_Date = AdmVM.T_DATE.Replace("/", "");

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
                        cryRpt.SetParameterValue("@T_BRANCH", AdmVM.BRANCH_ID);
                        cryRpt.Load(_strReportPath);
                                        
                                        
                        try
                        {
                            string contentType = "";
                         
                            switch (_strFormatType)
                            {
                                case "1":
                                    strReport = _strReport_Name + "_" + _tmpdate + ".pdf";

                                    _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
                                    contentType = "application/pdf";
                                    break;
                                case "2":
                                    strReport = _strReport_Name + "_" + _tmpdate + ".xlsx";

                                    _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);                     
                                    cryRpt.ExportToDisk(ExportFormatType.ExcelWorkbook, _strTempFile);
                                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    break;
                                case "3":                                   
                                    strReport = _strReport_Name + "-" + strT_Date + ".txt";
                                    _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
                                    cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
                                    contentType = "application/txt";
                                    break;

                            }

                            //return File(_strTempFile, "application/pdf", strReport);
                            return File(_strTempFile, contentType, strReport);
                        }
                        catch (Exception ex)
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, " catch (Exception ex) : line no 283 " + ex.Message.ToString());
                            ex.Message.ToString();
                            AdmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + AdmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                            return View("ADMOnDemand", AdmVM);                         
                        }


                    }
                    catch (Exception ex)
                    {

                        _logSys.WriteProcessLogFile(_strPathFile, " catch (Exception ex) : line no 297 " + ex.Message.ToString());
                        //_logSys.WriteErrLog(_strPathFile, ex.Message.ToString());
                        sb.Append("Can't generate Report File");
                        ex.Message.ToString();
                        AdmVM.MESSAGE = _strReport_Name + " ณ วันที่ " + AdmVM.DISPLAY_FILTER + " ไม่มีข้อมูล";
                        return View("ADMOnDemand", AdmVM);                       
                    }



                    // return File(_strTempFile, "application/pdf", strReport);

                    //return null;

            }

            //ADMOnDemandViewModel admOnDemandVM = new ADMOnDemandViewModel();
            //admOnDemandVM.T_DATE = (T_Date == null || T_Date == "") ? DateTime.Now.Date.ToShortDateString() : T_Date;
            //admOnDemandVM.DISPLAY_FILTER = (T_Date == null || T_Date == "") ? DateTime.Now.Date.ToShortDateString() : T_Date;
            //admOnDemandVM.SEARCH_KEY = Search;

            //string title = Session["TitleReport"].ToString();

            //admOnDemandVM.TITLE_REPORT = title;

            //list = GroupReportBAL.GetGroupDetailReport("2.00", Search);
            //ViewBag.GroupDetailReport = list;
            //string searchValue = Search;
            return View("ADMOnDemand", AdmVM);

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

    }
}