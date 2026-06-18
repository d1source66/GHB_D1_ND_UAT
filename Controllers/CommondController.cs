using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GHB_D1.Code.BAL;
using GHB_D1.Code.DAL;
using GHB_D1.Code.Util;
using GHB_D1.Models;
using GHB_D1.ReportFiles.DataSet;
using GHB_D1.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GHB_D1.Controllers
{
    public class CommondController : BaseController
    {
        //string ReportServer = WebConfigurationManager.AppSettings["ReportServer"].ToString();
        //string ReportDataBase = WebConfigurationManager.AppSettings["ReportDataBase"].ToString();
        //string ReportUserName = WebConfigurationManager.AppSettings["ReportUserName"].ToString();
        //string ReportPwd = WebConfigurationManager.AppSettings["ReportPwd"].ToString();
        private AccountService _accService = null;
        DBAccess _objDBAcc = null;
        Loger _logSys = null;
        dsGenReport _ds = null;
        CommondBAL _comBal = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        string _strGroupNo = "";

        public CommondController()
        {
            _objDBAcc = new DBAccess();
            _accService = new AccountService();
            _ds = new dsGenReport();
            _logSys = new Loger();
            _comBal = new CommondBAL();
        }

        public ActionResult Index(string px, string py, string T_DATE, string SEARCH_KEY, string pz)
        {
            ADMViewModel admVM = new ADMViewModel();
            admVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : T_DATE;
            admVM.DISPLAY_FILTER = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : T_DATE;
            admVM.SEARCH_KEY = SEARCH_KEY;

            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            if (Session["UserId"] != null)
            {
                _strGroupNo = _accService.GetGroupNoReportByName("ADM");
                list = _accService.AuthorizeGroupDetailReport(px, py, _strGroupNo, SEARCH_KEY);
                ViewBag.GroupDetailReport = list;

                Session["TitleReport"] = pz;
                admVM.TITLE_REPORT = pz;
                admVM.BRANCH_ID = px;
                admVM.USER_ID = py;
                admVM.MESSAGE = "";

                return View("ADM", admVM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult GenReportDS(string branchId,string cmdButton, ADMViewModel AdmVM)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            List<ADMModel> _admdata = new List<ADMModel>();
            string strReport = string.Empty;
            string _strTempFile = string.Empty;
            var getDate = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE;
            var arrDate = getDate.Split('/');
            AdmVM.T_DATE = (Convert.ToInt32(arrDate[2].Substring(2, 2)) - 43).ToString() + (arrDate[1].Count() == 1 ? "0" + arrDate[1].ToString() : arrDate[1].ToString()) + arrDate[0].ToString();
            AdmVM.DISPLAY_FILTER = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE;
            AdmVM.SEARCH_KEY = AdmVM.SEARCH_KEY;
            _strGroupNo = _accService.GetGroupNoReportByName("ADM");
            list = GroupReportBAL.GetGroupDetailReport(branchId, "",_strGroupNo, AdmVM.SEARCH_KEY);
            ViewBag.GroupDetailReport = list;

            DataTable _dt = new DataTable();
            string[] _arrCon = null;
            _arrCon = cmdButton.Split('|');
            string _strReport_Name = _arrCon[0].ToString() + "_ds";
            string _strFormatType = _arrCon[1].ToString();

            try
            {
                string _strReportPath = Server.MapPath(@"~\ReportFiles\ADM1001_ds.rpt");
                _logSys.WriteProcessLogFile(_strPathFile, "File Source : " + _strReportPath);

                List<string> RptParramList = new List<string>();

                _dt = _ds.Tables["ADMrpt"];
                _admdata = _comBal.GetADMDataByStoreBal(branchId,"SP_RPT_ADM1001_4", AdmVM.T_DATE,"%");
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
                        throw ex;
                    }
                }

                DiskFileDestinationOptions dfdopt = new DiskFileDestinationOptions();
                ReportDocument cryRpt = new ReportDocument();

                cryRpt.Load(_strReportPath);
                cryRpt.SetDataSource(_dt);
                _logSys.WriteProcessLogFile(_strPathFile, "Set Log on Database on Crystal Report");
                _logSys.WriteProcessLogFile(_strPathFile, "Success Log on Database on Crystal Report");
                cryRpt.SetParameterValue("@T_DATE", "230506");

                string contentType = "";
                switch (_strFormatType)
                {
                    case "1":
                        strReport = _strReport_Name + "_" + AdmVM.T_DATE + ".pdf";
                        _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
                        cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
                        contentType = "application/pdf";
                        break;
                    case "2":
                        strReport = _strReport_Name + "_" + AdmVM.T_DATE + ".xls";
                        _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
                        cryRpt.ExportToDisk(ExportFormatType.Excel, _strTempFile);
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case "3":
                        strReport = _strReport_Name + "_" + AdmVM.T_DATE + ".txt";
                        _strTempFile = Server.MapPath(@"~\TempFiles\" + strReport);
                        cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
                        contentType = "application/txt";
                        break;

                }

                return File(_strTempFile, contentType, strReport);
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "fn_GenReportDS : " + ex.Message.ToString());
            }
            //Json(sb.ToString(), JsonRequestBehavior.AllowGet);
            return RedirectToAction("ADM", "ADMReport"); //RedirectToAction("Index", "Home"); 
        }
    }
}