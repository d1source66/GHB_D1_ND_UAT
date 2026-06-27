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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
// using Microsoft.AspNetCore.Mvc;

namespace GHB_D1.Controllers
{
    public class HomeController : BaseController
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
        string _tmpdate = string.Empty;
        public HomeController()
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
            _iniCon.filePath = ModConf.ReadIni(_iniCon.iniFile, "VirtualPathFile", "Files");
            _ds = new dsGenReport();
            _comBal = new CommondBAL();
            _logSys = new Loger();
        }

        public ActionResult Index(string px, string py, string T_DATE, string F_DATE, string SEARCH_KEY, string pz)
        {
            _logSys.WriteErrLog(_strPathFile, "HomeController.cs : Index begin");
            MenuViewModel mnVM = new MenuViewModel();
            mnVM._groupReportVMList = new List<GroupReportViewModel>();
            groupRole roleList = new groupRole();
            AudiLogModel audilog = new AudiLogModel();
            if (Session["GroupReport"] != null)
            {
                _logSys.WriteErrLog(_strPathFile, "Menu Nav Begin 'Home Page'");
                ViewBag.GroupReport = Session["GroupReport"];
            }
            if (Session["UserId"] != null)
            {
                _logSys.WriteErrLog(_strPathFile, "HomeController.cs : Session and get param begin");
                string branchId = (string)Session["BranchId"];
                string branchName = (string)Session["BranchName"];
                string userId = (string)Session["UserId"];
                string username = (string)Session["Username"];

                string empcode = (string)Session["EmpCode"];
                string rolename = (string)Session["RoleName"];

                string _tmpdate = string.Empty;
                string _strT_Date = DateTime.Now.Date.AddDays(-1).ToString();


                _logSys.WriteErrLog(_strPathFile, "HomeController.cs : AuthorizeUserReport2ND begin");
                mnVM = _accService.AuthorizeUserReport2ND(rolename);
                //mnVM.T_DATE = DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", _cultureEnInfo);
                _logSys.WriteErrLog(_strPathFile, "HomeController.cs : GetNewDesignRole1 begin");
                mnVM.roleList = _accService.GetNewDesignRole1(empcode, rolename);//New Design20250308
                mnVM.T_DATE = (T_DATE == null || T_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
                mnVM.F_DATE = (F_DATE == null || F_DATE == "") ? DateTime.Now.Date.AddDays(-1).ToString("dd/MM/yyyy", new CultureInfo("en-US")) : T_DATE;
                mnVM.BRANCH_ID = branchId;
                mnVM.USER_ID = userId;
                //mnVM.BRANCH_NAME = branchName != mnVM.BRANCH_NAME ? branchName + " (กลุ่มรายงาน: " + mnVM.BRANCH_NAME + ")" : branchName;
                mnVM.BRANCH_NAME = branchName;
                mnVM.DISPLAY_FILTER = (mnVM.T_DATE == null || mnVM.T_DATE == "") ? DateTime.Now.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : mnVM.T_DATE;
                mnVM.SEARCH_KEY = SEARCH_KEY;

                //if (branchId == "001" || branchId == "049")
                //{
                //    return View("NdMainReport", mnVM);
                //}
                //else
                //{
                //    return View("MenuBranch", mnVM);
                //}
                if (Session["AudiLog"] != null)
                {
                    _logSys.WriteErrLog(_strPathFile, "HomeController.cs : AudiLog begin");
                    audilog = (AudiLogModel)Session["AudiLog"];
                    if (Session["latitude"] != null && Session["longitude"] != null)
                    {
                        audilog.Latitude = Session["latitude"].ToString() == "" ? "" : Session["latitude"].ToString();
                        audilog.Longitude = Session["longitude"].ToString() == "" ? "" : Session["longitude"].ToString();
                    }
                    ProcessLogBAL.AddProcessLog2(audilog);
                }
                LocationModel model = new LocationModel();
                //var sl = SaveLocation(model);
                _logSys.WriteErrLog(_strPathFile, "HomeController.cs : Go to Main Report");
                return View("NdMainReport", mnVM);

            }
            else
            {
                _logSys.WriteErrLog(_strPathFile, "HomeController.cs : Go back to Login page");
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult Menu(List<GroupReportViewModel> groupVMList)
        {
            return View("Menu", groupVMList);
        }



        [HttpGet]
        public ActionResult AllDownload(string branchID, string T_DATE, string groupNo, string groupName, string cmdButton, MenuViewModel mnVM, string SOLCODE)
        {

            branchID = (branchID != null && branchID != "") ? branchID.PadLeft(3, '0').ToString() : "001";

            //mnVM.T_DATE = DateTime.Now.Date.AddDays(-1).ToShortDateString();

            //string _strT_Date = DateTime.Parse(mnVM.T_DATE).ToString("dd/MM/yyyy");

            string _strT_Date = (mnVM.T_DATE == null || mnVM.T_DATE == "") ? DateTime.Now.Date.ToString("MM/dd/yyyy") : mnVM.T_DATE.ToString();

            StringBuilder sb = new StringBuilder();
            List<string> RptParramList = new List<string>();
            List<ADMModel> _admdata = new List<ADMModel>();
            char[] _sps = new char[] { '|' };
            char[] _spd = new char[] { '-' };
            string _tmpdate = string.Empty;
            string _depRecMOoney = string.Empty;
            string _department = string.Empty;
            string _pointRecMoney = string.Empty;

            string strReport = string.Empty;
            string _strTempFile = string.Empty;
            string reportName = "";
            DataTable _dt = new DataTable();

            try
            {
                string branchId = (string)Session["BranchId"];
                string branchName = (string)Session["BranchName"];
                string userId = (string)Session["UserId"];
                //add 20250513
                string roleName = (string)Session["RoleName"];

                //*Delete All reports file before*//
                //bool emptyFile = DeleteFilesReport();

                //if (emptyFile)
                //{
                //    mnVM = DownloadAllFiles(branchID, T_DATE);
                //    if (mnVM.DownloadCompleted)
                //    {
                //        mnVM.MESSAGE = "Download All Reports เรียบร้อยแล้ว ";

                //        mnVM.T_DATE = DateTime.Now.Date.AddDays(-1).ToShortDateString();
                //        mnVM.BRANCH_ID = branchId;
                //        mnVM.USER_ID = userId;
                //        mnVM.BRANCH_NAME = branchName;
                //        //return View("MenuCenter", mnVM);
                //        return Json(mnVM.MESSAGE, JsonRequestBehavior.AllowGet);
                //    }                  
                //}

                mnVM = DownloadAllFiles(branchID, T_DATE, groupNo, roleName, groupName);

                List<string> files = new List<string>();
                files = mnVM.files;

                if (mnVM.DownloadCompleted)
                {
                    mnVM.MESSAGE = "Download All Reports เรียบร้อยแล้ว ";

                    //mnVM.T_DATE = DateTime.Now.Date.AddDays(-1).ToShortDateString();
                    mnVM.BRANCH_ID = branchId;
                    mnVM.USER_ID = userId;
                    mnVM.BRANCH_NAME = branchName;
                    //return View("MenuCenter", mnVM);
                    //return Json(mnVM.MESSAGE, JsonRequestBehavior.AllowGet);
                }
                /**
                mnVM.MESSAGE = "ไม่มีข้อมูล " + reportName;
                mnVM = Post(mnVM);
                mnVM.T_DATE = DateTime.Now.Date.AddDays(-1).ToShortDateString();
                mnVM.BRANCH_ID = branchId;
                mnVM.USER_ID = userId;
                mnVM.BRANCH_NAME = branchName;
                //return View("MenuCenter", mnVM);
                **/
                string[] arrDataDateForZip = T_DATE.Split('/');

                // ชื่อไฟล์ ZIP ที่จะส่งไปยังฝั่งไคลเอนต์
                string zipFileName = $"{groupName}_{arrDataDateForZip[2].Substring(2,2)}{arrDataDateForZip[1]}{arrDataDateForZip[0]}.zip";
                string pathFile = _iniCon.filePath + userId + "/";
                string virtualFilePath = pathFile + zipFileName;
                string physicalFilePath = Server.MapPath(pathFile);
                string zipFilePath = physicalFilePath + zipFileName;

                //DirectoryInfo directoryInfo = new DirectoryInfo(physicalFilePath);
                //directoryInfo.Create();
                //if (!Directory.Exists(physicalFilePath))
                //    Directory.CreateDirectory(physicalFilePath); //Create folder ~/Files/userId/ if not exist.

                // Delete all files in a memory-efficient way
                //Directory.EnumerateFiles(physicalFilePath).ToList().ForEach(file =>
                //{
                //    try
                //    {
                //        System.IO.File.Delete(file);
                //    }
                //    catch (Exception ex)
                //    {
                //        _logSys.WriteProcessLogFile(_strPathFile, $"HomeController.cs_AllDownload:Error before save .zip file with {ex.Message} ");
                //    }
                //});

                // Create an in-memory ZIP
                var memoryStream = new MemoryStream();
                
                using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in files)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            var fileContent = System.IO.File.ReadAllBytes(filePath);
                            var zipEntry = zip.CreateEntry(Path.GetFileName(filePath));
                            using (var zipStream = zipEntry.Open())
                            {
                                zipStream.Write(fileContent, 0, fileContent.Length);
                            }
                        }
                    }
                }//end using zip
                
                // รีเซ็ตตำแหน่งของ memoryStream
                memoryStream.Position = 0;

                // ส่งไฟล์ ZIP ไปยังไคลเอนต์
                //return File(memoryStream, "application/zip", zipFileName);
                // บันทึกไฟล์ ZIP ลงในโฟลเดอร์เซิร์ฟเวอร์
                //System.IO.File.WriteAllBytes(zipFilePath, memoryStream.ToArray());

                // Return the ZIP as a stream
                //return File(memoryStream, "application/zip", "download.zip");
                return File(memoryStream.ToArray(), "application/zip", "download.zip");
                //return Json(new { fileUrl = Url.Content(virtualFilePath) });
            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "HomeController.cs - AllDownload Error : " + ex.Message);
                mnVM.MESSAGE = "ไม่มีข้อมูล " + reportName;
                string branchId = (string)Session["BranchId"];
                string branchName = (string)Session["BranchName"];
                string userId = (string)Session["UserId"];
                mnVM.BRANCH_ID = branchId;
                mnVM.USER_ID = userId;
                mnVM.BRANCH_NAME = branchName;
                mnVM = Post(mnVM);
                return View("MenuCenter", mnVM);
            }

            //mnVM.MESSAGE = "Download All Reports เรียบร้อยแล้ว " + reportName;
            //return View("MenuCenter", mnVM);
        }

        private MenuViewModel Post(MenuViewModel mnVM)
        {

            string branchId = (string)Session["BranchId"];
            string branchName = (string)Session["BranchName"];
            string userId = (string)Session["UserId"];
            string _tmpdate = string.Empty;
            string _strT_Date = DateTime.Now.Date.AddDays(-1).ToString();

            //  mnVM.T_DATE = "09/06/2023";
            DateTime dt = DateTime.Parse(mnVM.T_DATE);
            string year = "";
            if (dt != null)
            {
                year = (dt.Year <= 2000) ? (dt.Year + 543).ToString().Substring(2) : dt.Year.ToString().Substring(2);
            }
            else
            {
                year = DateTime.Now.Year.ToString().Substring(2);
            }


            _strT_Date = mnVM.T_DATE;


            string strT_Date = _strT_Date.Replace("/", "");

            if (strT_Date != "")
            {
                _tmpdate = strT_Date;
                strT_Date = year + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
            }


            mnVM = _accService.AuthorizeUserReport(branchId, userId, strT_Date);
            mnVM.T_DATE = DateTime.Now.Date.AddDays(-1).ToShortDateString();
            mnVM.BRANCH_ID = branchId;
            mnVM.USER_ID = userId;
            mnVM.BRANCH_NAME = branchName;

            return mnVM;
        }

        private MenuViewModel DownloadAllFiles(string branchID, string T_DATE, string groupNo, string roleName, string groupName)
        {
            MenuViewModel mnVM = new MenuViewModel();
            //DataTable _dt = new DataTable();
            //string bOndemand = string.Empty;
            //string userId = (string)Session["UserId"];
            //List<ADMModel> _admdata = new List<ADMModel>();
            //List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            mnVM.T_DATE = T_DATE;
            string[] arrDate = T_DATE.Split('/');
            string yearFolder = arrDate[2]; //2026
            string monthFolder = "M" + arrDate[1]; //M01
            string dayFolder = arrDate[0]; //14
            string dayData = $"{arrDate[2].Substring(2, 2)}{arrDate[1]}{arrDate[0]}"; //260114
            List<string> listFiles = new List<string>(); //all file (with full path) for zip
            string subBrandID = string.Empty;
            //if (branchID.Length > 3)
            //{
            //    subBrandID = branchID.Substring(0, 3);
            //}
            //else
            //{
            //    subBrandID = branchID;
            //}
            //list = GroupReportBAL.GetGroupDetailReport(subBrandID, userId, groupNo, "%");
            ////add 20250513
            //if (list.Count == 0 && roleName != "")
            //{
            //    list = GroupReportBAL.GetGroupDetailReport2(roleName, groupNo);
            //}

            #region "Logic to check only branch (on-demand) and get files (no re-generate pdf)"
            bool isBranchOnly = false; //check branch only (on-demand)
            isBranchOnly = IsBranchOnly(roleName);
            var dirFileNames = new HashSet<string>();
            if (isBranchOnly)
            {
                dirFileNames = GetReportOnDemandByGroupNameAndBranchId(groupName, branchID, yearFolder, monthFolder, dayFolder);
            }
            else
            {
                dirFileNames = GetReportOnDemandByGroupName(groupName, yearFolder, monthFolder, dayFolder);
            }
            #endregion


            #region "unused new logic
            /**
            // check report new logic.
            // Get the list file from Dir.
            string report_dir = string.Empty; //report directory to check with Db.

            List<string> listFilesInDir = CreateFolderIfNotExist(_arrDate, list[0].GROUP_NAME);
            if ((listFilesInDir.Count == 1) && (listFilesInDir[0].EndsWith("dummy.txt")))
            {   // it is dummy data, use for, retrieve the report folder by Group Name.
                report_dir = Path.GetDirectoryName(listFilesInDir[0]);
                listFilesInDir.RemoveAt(0); //remove dummy data.
            }
            else
            {
                report_dir = Path.GetDirectoryName(listFilesInDir[0]);
            }
            // Get the list file from Db.
            List<ReportRegenerate> listFilesInDb = GetReportNameFromList(list, _arrDate, list[0].GROUP_NAME);

            // Find items in inDb that don’t exist in inDir (by filename).
            // Extract file names from full paths
            var dirFileNames = new HashSet<string>
                                (
                                    listFilesInDir
                                        .Select(p => Path.GetFileName(p))
                                                ,StringComparer.OrdinalIgnoreCase
                                );
            
            // Find items in inDb that are NOT in inDir with case-insensitive
            var resultNotInDir = listFilesInDb
                                    .Where(x => !dirFileNames.Contains(x.Report_File_Name))
                                    .ToList();

            // Generate new file from Crystal Report (.rpt) which is not in Dir.
            // Get the absolute full path to save the report.
            //string fullPath = Path.GetDirectoryName(report_dir);
            string _rptRegenerateFile = string.Empty;
            foreach (var item in resultNotInDir)
            {
                _rptRegenerateFile = GenenerateReportFile(
                                        Path.Combine(report_dir, item.Report_File_Name), 
                                        item.Report_Name_Db, 
                                        list[0].GROUP_NAME, _arrDate
                                     );
                //add the re-generated report file to files list a .zip file.
                if (!string.IsNullOrEmpty(_rptRegenerateFile))
                    files.Add(_rptRegenerateFile);
            }
            **/
            #endregion

            // add Dir File Names to list for generate a .zip file.
            //var tmpDirFileNames = new HashSet<string>();
            //foreach (var item in dirFileNames)
            //{
            //    //tmpDirFileNames.Add(Path.Combine(report_dir, item));
            //    tmpDirFileNames.Add( item);
            //}
            //files.AddRange(tmpDirFileNames);

            listFiles.AddRange(dirFileNames);
            mnVM.DownloadCompleted = true;

            #region "comment for old logic "
            /**** comment for old logic
            foreach (var rpt in list)
            {
                try
                {
                    _iniCon.stp = ModConf.ReadIni(_iniCon.iniFile, "DS", rpt.REPORT_NAME);

                    string _param = string.Empty;

                    DiskFileDestinationOptions dfdopt = new DiskFileDestinationOptions();
                    ReportDocument cryRpt = new ReportDocument();
                    TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    Tables CrTables;
                    string _strReport_Name = string.Empty;
                    _strReport_Name = rpt.REPORT_NAME;
                    switch (_strReport_Name)
                    {
                        case "RATM9000_M_TOTAL_B001":
                            _strReport_Name = _strReport_Name + "_EXCEL";
                            break;

                    }
                    string _strReportPath = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + ".rpt");


                    _logSys.WriteProcessLogFile(_strPathFile, "HomeController.cs_DownloadAllFiles : BranchId " + branchID + " วันที่ " + T_DATE + " Report Name " + rpt.REPORT_NAME);

                    //check report use dataset
                    if (_iniCon.stp == "" || _iniCon.stp == null)
                    {
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
                        
                        if (
                             _strReport_Name == "ADM1001_ONDEMAND" ||
                             _strReport_Name == "ADM1102_ONDEMAND" ||
                             _strReport_Name == "CDM1001_ONDEMAND" ||
                             _strReport_Name == "CDM1005_ONDEMAND" ||
                             _strReport_Name == "CDM1005_1_ONDEMAND" ||
                             _strReport_Name == "CDM1102_ONDEMAND" ||
                             _strReport_Name == "CRM1001_ONDEMAND" ||
                             _strReport_Name == "CRM1005_ONDEMAND" ||
                             _strReport_Name == "CRM1005_1_ONDEMAND" ||
                             _strReport_Name == "CRM1102_ONDEMAND" ||
                             _strReport_Name == "LRM1001_ONDEMAND" ||
                             _strReport_Name == "LRM1005_ONDEMAND" ||
                             _strReport_Name == "LRM1102_ONDEMAND" ||
                             _strReport_Name == "RATM1001_ONDEMAND" ||
                             _strReport_Name == "RATM1005_ONDEMAND" ||
                             _strReport_Name == "RATM1005_1_ONDEMAND" ||
                             _strReport_Name == "RATM1102_ONDEMAND"

                     )
                        {
                            bOndemand = getBranchOndemand(Session["BranchId"].ToString(), "", "ondemand");
                            if (bOndemand != "" || bOndemand != null)
                            {
                                cryRpt.SetParameterValue("@T_BRANCH", bOndemand);
                            }
                        }

                    }
                    else //(_iniCon.stp == "" || _iniCon.stp == null)
                    {
                        string _strReportPath_test = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + "_ds.rpt");
                        _dt = _ds.Tables["ADMrpt"];
                        _admdata = _comBal.GetADMDataByStoreBal(branchID, _iniCon.stp, strT_Date, "%");
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
                                _logSys.WriteProcessLogFile(_strPathFile, "foreach (ADMModel data in _admdata) line 346 : Report Name" + _strReportPath + "_" + ex.Message.ToString());
                            }
                        }

                        cryRpt.Load(_strReportPath_test);
                        cryRpt.SetDataSource(_dt);
                        cryRpt.SetParameterValue("@T_DATE", strT_Date);


                    }


                    try
                    {

                        / *** Check main folder **** /
                        string path = _iniCon.rptPath;

                        DirectoryInfo directoryInfo = new DirectoryInfo(path);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        if (directoryInfo.Exists)
                        {
                            string runDate = _tmpdate.Substring(4, 4) + _tmpdate.Substring(2, 2) + _tmpdate.Substring(0, 2);
                            string _pathReportFile = _iniCon.rptPath + "/" + userId + "/" + runDate + "/" + "D1-CBS-REPORT-" + rpt.GROUP_NAME;
                            DirectoryInfo directoryInfoFile = new DirectoryInfo(_pathReportFile);
                            directoryInfoFile.Create();

                            //string ReportFilePath = CreateForderTrue.FullName;
                            string contentType = string.Empty;
                            if (directoryInfoFile.Exists)
                            {
                                if (rpt.REPORT_NAME.Contains("EXCEL"))
                                {
                                    strReport = rpt.REPORT_NAME + ".xls";
                                    _strTempFile = _pathReportFile + @"\" + strReport;
                                    cryRpt.ExportToDisk(ExportFormatType.Excel, _strTempFile);
                                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    files.Add(_strTempFile);
                                }
                                else if (rpt.REPORT_NAME.Contains("TEXT"))
                                {
                                    strReport = rpt.REPORT_NAME + ".txt";
                                    _strTempFile = _pathReportFile + @"\" + strReport;
                                    cryRpt.ExportToDisk(ExportFormatType.Text, _strTempFile);
                                    contentType = "application/txt";
                                    files.Add(_strTempFile);
                                }
                                else
                                {
                                    strReport = rpt.REPORT_NAME + ".pdf";
                                    _strTempFile = _pathReportFile + @"\" + strReport;
                                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
                                    contentType = "application/pdf";
                                    files.Add(_strTempFile);
                                }
                                //strReport = rpt.REPORT_NAME + ".pdf";
                                //_strTempFile = _pathReportFile + @"\" + strReport;
                                //cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _strTempFile);
                                //contentType = "application/pdf";
                                //files.Add(_strTempFile);
                            }
                        }

                        cryRpt.Close();

                        mnVM.DownloadCompleted = true;
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "var CreateFolderMain = GetCreateMyFolder(_iniCon.rptPath); line 428 : Report Name" + _strReportPath + "_" + ex.Message.ToString());
                        mnVM.MESSAGE = "ไม่มีข้อมูล " + _strReport_Name;
                        mnVM.DownloadCompleted = false;

                    }

                }
                catch (Exception exc)
                {
                    _logSys.WriteProcessLogFile(_strPathFile, "HomeController.cs_DownloadAllFiles : Report Name " + rpt.REPORT_NAME + " Error : " + exc.Message);
                }

            }
            
            ***/
            #endregion

            mnVM.files = listFiles;
            return mnVM;
        }

        private HashSet<string> GetReportOnDemandByGroupName(string groupName, string yFolder, string mFolder, string dFolder)
        {
            HashSet<string> retval = new HashSet<string>();
            string reportRootDir = _iniCon.rptPath;
            string reportDir = string.Empty;
            switch (groupName)
            {
                case "ADM":
                    reportDir = Path.Combine(reportRootDir, yFolder, mFolder, dFolder, "D1-CBS-REPORT-ADM");
                    break;
                case "CDM":
                    reportDir = Path.Combine(reportRootDir, yFolder, mFolder, dFolder, "D1-CBS-REPORT-CDM");
                    break;
                case "CRM":
                    reportDir = Path.Combine(reportRootDir, yFolder, mFolder, dFolder, "D1-CBS-REPORT-CRM");
                    break;
                case "LRM":
                    reportDir = Path.Combine(reportRootDir, yFolder, mFolder, dFolder, "D1-CBS-REPORT-LRM");
                    break;
                case "RATM":
                    reportDir = Path.Combine(reportRootDir, yFolder, mFolder, dFolder, "D1-CBS-REPORT");
                    break;
                case "SETTLE":
                    reportDir = Path.Combine(reportRootDir, yFolder, mFolder, dFolder, "D1-CBS-SETTLE");
                    break;
                default:
                    break;
            }

            //file the files by folder
            var files = Directory.GetFiles(reportDir, "*.*");

            foreach (var file in files)
            {
                retval.Add(file);
            }
            return retval;
        }

        private HashSet<string> GetReportOnDemandByGroupNameAndBranchId(
            string groupName, string branchID, string yFolder, string mFolder, string dFolder)
        {
            HashSet<string> retval = new HashSet<string>();
            string reportRootDir = _iniCon.rptPath;
            string reportDir = Path.Combine(reportRootDir, yFolder, mFolder, dFolder, "OnDemandReport");

            //file the files by group (ADM, CDM, etc..) and branch
            var files = Directory.GetFiles(reportDir, $"{groupName}*_{branchID}_*");

            foreach (var file in files)
            {
                retval.Add(file);
            }
            return retval;

        }

        /// <summary>
        /// Check branch only (on-demand). 
        /// Group_Report = IND and List_Role = true, it is branch
        /// Group_Report = ALL and List_Role = true, it is HQ
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private bool IsBranchOnly(string roleName)
        {
            bool retval = false;
            DBAccess dbAccess = new DBAccess();
            DataTable dt = new DataTable();
            SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@Role_Name", roleName)
            };
            string sql = "SELECT Group_Report, List_Role ";
            sql += "FROM TBL_ROLE_MANAGEMENT "; 
            sql += "WHERE Role_Name = @Role_Name AND Group_Report IN ('IND','ALL') ";
            sql += "ORDER BY Group_Report";

            dt = dbAccess.ExecuteQueryMoreOneParameters(sql, sqlParameter, 1);

            if (dt.Rows.Count > 0) 
            {
                if ((dt.Rows[0]["Group_Report"].ToString() == "ALL") && 
                    ((bool)dt.Rows[0]["List_Role"] == false))
                {
                    retval = true; //it is branch only.
                }                
            }

            dbAccess = null;
            return retval;
        }

        #region "New logic check existing file if not re-generate file"

        /// <summary>
        /// Get the report name from the list in DB.
        /// </summary>
        /// <param name="list">List the report name from db.</param>
        /// <param name="arrDate">Array of the date that user select in the date-picker. index 0 = day, 1=month, 2=year(2026)</param>
        /// <param name="gROUP_NAME">Group name of the report. Ex. ADM, CDM, CRM, LOC, LRM, RAT, SETTLE</param>
        /// <returns>List string of the report name (just file name).</returns>
        private List<ReportRegenerate> GetReportNameFromList(List<GroupDetailReportViewModel> list, string[] arrDate, string gROUP_NAME)
        {
            List<ReportRegenerate> retval = new List<ReportRegenerate>();
            string tmp_report_name = string.Empty;
            string tmp_report_name_db = string.Empty;
            foreach (var item in list)
            {
                if (item.REPORT_NAME.Contains("EXCEL"))
                {
                    tmp_report_name = $"{item.REPORT_NAME.Replace("_EXCEL", "")}.xls";
                    tmp_report_name_db = $"{item.REPORT_NAME}";
                }
                else if (item.REPORT_NAME.Contains("TEXT"))
                {
                    tmp_report_name = $"{item.REPORT_NAME.Replace("_TEXT", "")}.txt";
                    tmp_report_name_db = $"{item.REPORT_NAME}";
                }
                else if (item.REPORT_NAME.StartsWith("PPD"))
                {
                    tmp_report_name = $"{item.REPORT_NAME}{arrDate[2].Substring(2,2)}{arrDate[1]}{arrDate[0]}.pdf";
                    tmp_report_name_db = $"{item.REPORT_NAME}";
                }
                else if (item.REPORT_NAME.EndsWith("ONDEMAND"))
                {
                    //not include ONDEMAND in the list. wait implement.
                }
                else if (gROUP_NAME.ToUpper() == "SETTLE")
                {
                    tmp_report_name = $"{item.REPORT_NAME}{arrDate[2].Substring(2,2)}{arrDate[1]}{arrDate[0]}";
                    tmp_report_name_db = $"{item.REPORT_NAME}";
                }
                else
                {
                    tmp_report_name = $"{item.REPORT_NAME}.pdf";
                    tmp_report_name_db = $"{item.REPORT_NAME}";
                }
                retval.Add(new ReportRegenerate(tmp_report_name_db, tmp_report_name));
            }//end foreach
            return retval;
        }

        /// <summary>
        /// regenerate the report from .rpt file
        /// </summary>
        /// <param name="reportNameWithFullPath">Report name with full path</param>
        /// <param name="rEPORT_NAME">Report name from Db for mapping</param>
        /// <param name="gROUP_NAME">Group name of report</param>
        /// <param name="arrDate">Array of the date that user select in the date-picker. index 0 = day, 1=month, 2=year(2026)</param>
        /// <returns></returns>
        private string GenenerateReportFile(string reportNameWithFullPath, string rEPORT_NAME, string gROUP_NAME, string[] arrDate)
        {
            string _strCrystRptName = string.Empty;
            string _rptFileToSave = reportNameWithFullPath;
            _strCrystRptName = rEPORT_NAME + ".rpt";            
            _strCrystRptName = Server.MapPath(@"~\ReportFiles\" + _strCrystRptName);

            try
            {
                DiskFileDestinationOptions dfdopt = new DiskFileDestinationOptions();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;
                ReportDocument cryRpt = new ReportDocument();

                cryRpt.Load(_strCrystRptName);
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
                //get only 260114
                cryRpt.SetParameterValue("@T_DATE", arrDate[2].Substring(2,2) + arrDate[1] + arrDate[0]);
                if (rEPORT_NAME.Contains("EXCEL"))
                {
                    cryRpt.ExportToDisk(ExportFormatType.Excel, _rptFileToSave);
                }
                else if (rEPORT_NAME.Contains("TEXT"))
                {
                    cryRpt.ExportToDisk(ExportFormatType.Text, _rptFileToSave);
                }
                else
                {
                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, _rptFileToSave);
                }
                cryRpt.Close();
                return _rptFileToSave; //return if generate success.
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, $"HomeController.cs_GenerateReportFile: Load crystal report file :{_strCrystRptName} with {ex.Message} ");
                return ""; // return if generate fail.
            }
            
        }

        /// <summary>
        /// Check the folder D:\ReportFile\yyyy\Mmm\dd\group_Name
        /// Ex: D:\ReportFile\2026\M01\14\D1-CBS-REPORT-ADM
        /// If not exist then create it and return the list of string full files name. 
        /// </summary>
        /// <param name="arr_Date">Array of the date that user select in the date-picker. index 0 = day, 1=month, 2=year(2026)</param>
        /// <param name="group_Name">Group name of the report. Ex. ADM, CDM, CRM, LOC, LRM, RAT, SETTLE</param>
        private List<string> CreateFolderIfNotExist(string[] arr_Date, string group_Name)
        {
            string reportFolder = _iniCon.rptPath;
            string folder_group = "";
            if (group_Name.ToUpper() == "RAT")
            {
                folder_group = "D1-CBS-REPORT";
            }
            else if (group_Name.ToUpper() == "SETTLE")
            {
                folder_group = "D1-CBS-SETTLE";
            }
            else
            {
                folder_group = $"D1-CBS-REPORT-{group_Name}";
            }
            reportFolder = Path.Combine(reportFolder, arr_Date[2], $"M{arr_Date[1]}", $"{arr_Date[0]}",folder_group);
            var listFiles = new List<string>();
            if (!Directory.Exists(reportFolder))
            {
                _logSys.WriteProcessLogFile(_strPathFile, $"HomeController.cs_CreateFolderIfNotExist : folder {reportFolder} does not exist then create it");
                Directory.CreateDirectory(reportFolder);
                listFiles.Add(Path.Combine(reportFolder,"dummy.txt"));
            }
            else
            {
                listFiles.AddRange( Directory.GetFiles(reportFolder, "*", SearchOption.TopDirectoryOnly).ToList());
            }
            return listFiles;
        }

        #endregion

        private bool DeleteFilesReport()
        {

            System.IO.DirectoryInfo di = new DirectoryInfo(_iniCon.rptPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            return true;
        }

        [HttpPost]
        public ActionResult GenReport(string branchID, string cmdButton, ADMViewModel AdmVM)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            List<HomeModel> _admdata = new List<HomeModel>();
            DataTable _dt = new DataTable();

            AdmVM.T_DATE = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE.ToString();
            AdmVM.DISPLAY_FILTER = (AdmVM.T_DATE == null || AdmVM.T_DATE == "") ? DateTime.Now.Date.ToShortDateString() : AdmVM.T_DATE;
            AdmVM.SEARCH_KEY = AdmVM.SEARCH_KEY;

            string[] _arrCon = null;
            _arrCon = cmdButton.Split('|');

            string[] _arrDate = null;
            _arrDate = AdmVM.T_DATE.Split('/');
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

            //string _strT_Date = DateTime.Parse(AdmVM.T_DATE).ToString("dd/MM/yyyy");
            //string _strTermId = _arrCon[0].ToString();
            //string _strReport_Name = _arrCon[1].ToString();
            //string _strFormatType = _arrCon[2].ToString();


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

                //check report use dataset
                if (_iniCon.stp == "" || _iniCon.stp == null)
                {
                    TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    Tables CrTables;


                    try
                    {
                        cryRpt.Load(_strReportPath);
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "GenReport_HomeController : ReportName : " + _strReportPath + "message_detail :" + ex.Message);
                    }



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


                    if (
                        _strReport_Name == "ADM1001_ONDEMAND" ||
                        _strReport_Name == "ADM1102_ONDEMAND" ||
                        _strReport_Name == "CDM1001_ONDEMAND" ||
                        _strReport_Name == "CDM1005_ONDEMAND" ||
                        _strReport_Name == "CDM1005_1_ONDEMAND" ||
                        _strReport_Name == "CDM1102_ONDEMAND" ||
                        _strReport_Name == "CRM1001_ONDEMAND" ||
                        _strReport_Name == "CRM1005_ONDEMAND" ||
                        _strReport_Name == "CRM1005_1_ONDEMAND" ||
                        _strReport_Name == "CRM1102_ONDEMAND" ||
                        _strReport_Name == "LRM1001_ONDEMAND" ||
                        _strReport_Name == "LRM1005_ONDEMAND" ||
                        //_strReport_Name == "LRM1005_1_ONDEMAND" ||
                        _strReport_Name == "LRM1102_ONDEMAND" ||
                        _strReport_Name == "RATM1001_ONDEMAND" ||
                        _strReport_Name == "RATM1005_ONDEMAND" ||
                        _strReport_Name == "RATM1005_1_ONDEMAND" ||
                        _strReport_Name == "RATM1102_ONDEMAND"


                        )
                    {
                        cryRpt.SetParameterValue("@T_BRANCH", AdmVM.BRANCH_ID);
                    }

                }
                else
                {
                    string _strReportPath_test = Server.MapPath(@"~\ReportFiles\" + _strReport_Name + "_ds.rpt");
                    _dt = _ds.Tables["ADMrpt"];
                    _admdata = _comBal.GetHomeDataByStoreBal(branchID, _iniCon.stp, strT_Date, "%");
                    foreach (HomeModel data in _admdata)
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
                            _logSys.WriteProcessLogFile(_strPathFile, "GenReport_HomeController_foreach (HomeModel data in _admdata) : ReportName : " + _strReportPath + "message_detail :" + ex.Message);
                        }
                    }
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

                    _logSys.WriteProcessLogFile(_strPathFile, "GenReport_HomeController__strFormatType : ReportName : " + _strReportPath + "message_detail :" + ex.Message);

                    AdmVM.MESSAGE = "ไม่มีข้อมูล";
                    return View("MenuBranch", AdmVM);
                }


            }
            catch (Exception ex)
            {
                //  ex.Message.ToString();
                _logSys.WriteProcessLogFile(_strPathFile, "GenReport_HomeController__T_DATE :" + "message_detail :" + ex.Message);

                AdmVM.MESSAGE = "ไม่มีข้อมูล";
                return View("MenuBranch", AdmVM);
            }


            //return View("MenuBranch", AdmVM);

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


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /*
        public static DirectoryInfo GetCreateMyFolder(string baseFolder)
        {
            return Directory.CreateDirectory(baseFolder);
        }

        public static DirectoryInfo chkinfo(string info)
        {
            return Directory.CreateDirectory(info);
        }
        */

        [HttpPost]
        public ActionResult SaveLocation(LocationModel model)
        {
            // Here, you can store the coordinates in a database or log them
            System.Diagnostics.Debug.WriteLine($"Received Location: {model.Latitude}, {model.Longitude}");

            return Content($"Received Location: {model.Latitude}, {model.Longitude}");
        }

        [HttpPost]
        public JsonResult SaveLocation2(double latitude, double longitude, double accuracy)
        {
            // Save or process the received coordinates
            //Console.WriteLine("Latitude: " + latitude + ", Longitude: " + longitude);
            if (Session["latitude"] != null && Session["longitude"] != null)
            {
                Session["latitude"] = null;
                Session["longitude"] = null;
            }
            Session["latitude"] = latitude == 0.00 ? 0.00 : latitude;
            Session["longitude"] = longitude == 0.00 ? 0.00 : longitude;
            // Return success response
            return Json(new { success = true, message = "Location received!", accuracy });
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