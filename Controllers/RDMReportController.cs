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
using GHB_D1.Code.Util;
using System.Globalization;

namespace GHB_D1.Controllers
{
    public class RDMReportController : BaseController
    {
        string _strGroupNo = "";
        private AccountService _accService = new AccountService();
        iniConnection _iniCon = null;
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
      

        public RDMReportController()
        {
            _accService = new AccountService();
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            _iniCon.Server = ModConf.ReadIni(_iniCon.iniFile, "DB", "Server");
            _iniCon.User = ModConf.ReadIni(_iniCon.iniFile, "DB", "User");
            _iniCon.Password = ModConf.ReadIni(_iniCon.iniFile, "DB", "Password");
            _iniCon.DBAMS = ModConf.ReadIni(_iniCon.iniFile, "DB", "DatabaseAMS");
            _iniCon.DBTLF = ModConf.ReadIni(_iniCon.iniFile, "DB", "DatabaseTLF");
            _logSys = new Loger();
        }
        [HttpGet]
        public ActionResult Index(string px, string py, string T_DATE, string cmdButton, string SEARCH_KEY, string pz)
        {

            RDMViewModel rdmVM = new RDMViewModel();
            rdmVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            rdmVM.DISPLAY_FILTER = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
            rdmVM.SEARCH_KEY = SEARCH_KEY;

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            if (Session["UserId"] != null && Session["UserId"].ToString() == py)
            {
                _strGroupNo = _accService.GetGroupNoReportByName("RDM");
                list = _accService.AuthorizeGroupDetailReport(px, py, _strGroupNo, SEARCH_KEY);
                ViewBag.GroupDetailReport = list;
                string searchValue = SEARCH_KEY;
                Session["TitleReport"] = pz;
                rdmVM.TITLE_REPORT = pz;
                rdmVM.BRANCH_ID = px;
                rdmVM.USER_ID = py;
                rdmVM.MESSAGE = "";
                return View("RDM", rdmVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult GenReport(string px, string py, string cmdButton, RDMViewModel RdmVM)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            RdmVM.T_DATE = (RdmVM.T_DATE == null || RdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : RdmVM.T_DATE;
            RdmVM.DISPLAY_FILTER = (RdmVM.T_DATE == null || RdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : RdmVM.T_DATE;
            RdmVM.SEARCH_KEY = RdmVM.SEARCH_KEY;

            list = GroupReportBAL.GetGroupDetailReport(px,py,"6.00", RdmVM.SEARCH_KEY);
            ViewBag.GroupDetailReport = list;
            _strGroupNo = _accService.GetGroupNoReportByName("RDM");
            switch (cmdButton)
            {
                case "Search":

                    list = _accService.AuthorizeGroupDetailReport(RdmVM.BRANCH_ID, RdmVM.USER_ID, _strGroupNo, RdmVM.SEARCH_KEY);
                    ViewBag.GroupDetailReport = list;

                    if (list.Count <= 0)
                    {
                        RdmVM.MESSAGE = "ไม่มีข้อมูล";
                    }
                    break;
                default:
                    string _tmpdate = string.Empty;
                    string[] _arrCon = null;
                    _arrCon = cmdButton.Split('|');


                    string[] _arrDate = null;
                    _arrDate = RdmVM.T_DATE.Split('/');
                    _logSys.WriteProcessLogFile(_strPathFile, "arrDate[0] : " + _arrDate[0].ToString());
                    _logSys.WriteProcessLogFile(_strPathFile, "arrDate[1] : " + _arrDate[1].ToString());
                    _logSys.WriteProcessLogFile(_strPathFile, "arrDate[2] : " + _arrDate[2].ToString());

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
                                     

                    string strT_Date = RdmVM.T_DATE.Replace("/", "");
                    _logSys.WriteProcessLogFile(_strPathFile, "RdmVM.T_DATE.Replace : 117 line strT_Date = " + strT_Date);
                    if (strT_Date != "")
                    {
                        _tmpdate = strT_Date;
                        strT_Date = year + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
                    }
                                   
                    string _strReport_Name = _arrCon[0].ToString();
                    string _strFormatType = _arrCon[1].ToString();
                    StringBuilder sb = new StringBuilder();
                    List<string> RptParramList = new List<string>();

                    char[] _sps = new char[] { '|' };
                    char[] _spd = new char[] { '-' };
                 
                    string _depRecMOoney = string.Empty;
                    string _department = string.Empty;
                    string _pointRecMoney = string.Empty;

                    string strReport = string.Empty;
                    string _strTempFile = string.Empty;

                    try
                    {
                        
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
                            RdmVM.MESSAGE = "ไม่มีข้อมูล";
                            _logSys.WriteProcessLogFile(_strPathFile, "_strFormatType : " + ex.Message.ToString());
                            return View("RDM", RdmVM);
                        }


                    }
                    catch (Exception ex)
                    {
                        RdmVM.MESSAGE = "ไม่มีข้อมูล";
                        _logSys.WriteProcessLogFile(_strPathFile, "_strReportPath : " + ex.Message.ToString());
                        return View("RDM", RdmVM);
                    }




            }


            return View("RDM", RdmVM);

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