using GHB_D1.Code.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using GHB_D1.Models;
using GHB_D1.Code.Util;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GHB_D1.Code.BAL
{
    public class AdministratorBAL
    {
        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");

        public AdministratorBAL()
        {
            _logSys = new Loger();
        }

        public static List<GroupReportModel> GetGroupReport()
        {
            List<GroupReportModel> list = new List<GroupReportModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetGroupReport();

                foreach (DataRow dr in dt.Rows)
                {
                    GroupReportModel obj = new GroupReportModel();
                    obj.ID = Convert.ToInt32(dr["ID_GROUP"]);
                    obj.GroupCode = Convert.ToDecimal(dr["GROUP_NO"]);
                    obj.GroupName = dr["GROUP_NAME"].ToString();
                    obj.Active = Convert.ToBoolean(dr["ACTIVE"]);
                    obj.Created = Convert.ToDateTime(dr["CREATE_DATE"]);
                    obj.Updated = Convert.ToDateTime(dr["UPDATE_DATE"]);
                    list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }



        public static GroupReportModel GetGroupReportByID(int ID)
        {
            GroupReportModel obj = new GroupReportModel();

            try
            {
                DataTable dt = AdministratorDAL.GetGroupReportByID(ID);
                if (dt != null && dt.Rows.Count > 0)
                {
                    obj.ID = Convert.ToInt32(dt.Rows[0]["ID_GROUP"]);
                    obj.GroupCode = Convert.ToDecimal(dt.Rows[0]["GROUP_NO"]);
                    obj.GroupName = dt.Rows[0]["GROUP_NAME"].ToString();
                    obj.Active = Convert.ToBoolean(dt.Rows[0]["ACTIVE"]);
                    obj.Created = Convert.ToDateTime(dt.Rows[0]["CREATE_DATE"]);
                    obj.Updated = Convert.ToDateTime(dt.Rows[0]["UPDATE_DATE"]);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }

        public static decimal GenGroupReportNo()
        {
            decimal dcmGroupNo = 0;
            try
            {
                DataTable dt = new DataTable();
                dt = AdministratorDAL.GenGroupNo();
                if (dt.Rows[0]["GENERATE_GROUP_NO"] != DBNull.Value)
                {
                    dcmGroupNo = Convert.ToDecimal(dt.Rows[0]["GENERATE_GROUP_NO"]);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dcmGroupNo;
        }
        public static bool AddGroupReport(GroupReportModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddGroupReport(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool EditGroupReport(GroupReportModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditGroupReport(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool DeleteGroupReportByID(int ID)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteGroupReportByID(ID);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static List<UserAttribModel> GetUserAttrib()
        {
            List<UserAttribModel> list = new List<UserAttribModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetUSER_ATTRIB();

                foreach (DataRow dr in dt.Rows)
                {
                    UserAttribModel obj = new UserAttribModel();

                    obj.NCFGVARIABLEID = Convert.ToInt16(dr["NCFGVARIABLEID"]);

                    obj.SCFGVARIABLE = Convert.ToString(dr["SCFGVARIABLE"]);

                    obj.SCFGVALUE = Convert.ToString(dr["SCFGVALUE"]);

                    obj.SDESCRIPTION = Convert.ToString(dr["SDESCRIPTION"]);

                    obj.Category = Convert.ToString(dr["Category"]);

                    list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static bool AddUserAttrib(UserAttribModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddUserAttrib(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static List<AudiLogModel> GetAudiLogDetails1()
        {
            List<AudiLogModel> list = new List<AudiLogModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetAudiLogDetails1();

                foreach (DataRow dr in dt.Rows)
                {
                    AudiLogModel obj = new AudiLogModel();
                    obj.Audi_Id = Convert.ToInt16(dr["ID"]);
                    obj.Username = Convert.ToString(dr["Username"]);
                    obj.Message = Convert.ToString(dr["Message"]);
                    obj.WorkFunction = Convert.ToString(dr["WorkFunction"]);
                    obj.Details01 = Convert.ToString(dr["Details01"]);
                    obj.Details02 = Convert.ToString(dr["Details02"]);
                    obj.EmpCode = Convert.ToString(dr["EmpCode"]);
                    obj.EmpName = Convert.ToString(dr["EmpName"]);
                    obj.MAC_Address = Convert.ToString(dr["MAC_Address"]);
                    obj.OS = Convert.ToString(dr["OS"]);
                    obj.URL = Convert.ToString(dr["URL"]);
                    obj.Device = Convert.ToString(dr["Device"]);
                    obj.IP_Address = Convert.ToString(dr["IP_Address"]);
                    obj.Created = DateTime.Parse(dr["Created"].ToString());
                    obj.Updated = DateTime.Parse(dr["Updated"].ToString());
                    obj.Latitude = Convert.ToString(dr["Latitude"]);
                    obj.Longitude = Convert.ToString(dr["Longitude"]);
                    list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<AudiLogModel> GetAudiLogDetails(AudiLogModel audi)
        {
            List<AudiLogModel> list = new List<AudiLogModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetAudiLogDetails(audi);

                foreach (DataRow dr in dt.Rows)
                {
                    AudiLogModel obj = new AudiLogModel();
                    obj.Audi_Id = Convert.ToInt16(dr["ID"]);
                    obj.Username = Convert.ToString(dr["Username"]);
                    obj.Message = Convert.ToString(dr["Message"]);
                    obj.WorkFunction = Convert.ToString(dr["WorkFunction"]);
                    obj.Details01 = Convert.ToString(dr["Details01"]);
                    obj.Details02 = Convert.ToString(dr["Details02"]);
                    obj.EmpCode = Convert.ToString(dr["EmpCode"]);
                    obj.EmpName = Convert.ToString(dr["EmpName"]);
                    obj.MAC_Address = Convert.ToString(dr["MAC_Address"]);
                    obj.OS = Convert.ToString(dr["OS"]);
                    obj.URL = Convert.ToString(dr["URL"]);
                    obj.Device = Convert.ToString(dr["Device"]);
                    obj.IP_Address = Convert.ToString(dr["IP_Address"]);
                    obj.Created = DateTime.Parse(dr["Created"].ToString());
                    obj.Updated = DateTime.Parse(dr["Created"].ToString());
                    obj.Latitude = Convert.ToString(dr["Latitude"]);
                    obj.Longitude = Convert.ToString(dr["Longitude"]);
                    list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static AudiLogModel GetAudiLogDetails2(AudiLogModel audi)
        {
            AudiLogModel obj = new AudiLogModel();

            try
            {
                DataTable dt = AdministratorDAL.GetAudiLogDetails(audi);

                foreach (DataRow dr in dt.Rows)
                {
                    //AudiLogModel obj = new AudiLogModel();
                    obj.Audi_Id = Convert.ToInt16(dr["ID"]);
                    obj.Username = Convert.ToString(dr["Username"]);
                    obj.Message = Convert.ToString(dr["Message"]);
                    obj.WorkFunction = Convert.ToString(dr["WorkFunction"]);
                    obj.Details01 = Convert.ToString(dr["Details01"]);
                    obj.Details02 = Convert.ToString(dr["Details02"]);
                    obj.EmpCode = Convert.ToString(dr["EmpCode"]);
                    obj.EmpName = Convert.ToString(dr["EmpName"]);
                    obj.MAC_Address = Convert.ToString(dr["MAC_Address"]);
                    obj.OS = Convert.ToString(dr["OS"]);
                    obj.URL = Convert.ToString(dr["URL"]);
                    obj.Device = Convert.ToString(dr["Device"]);
                    obj.IP_Address = Convert.ToString(dr["IP_Address"]);
                    obj.Created = DateTime.Parse(dr["Created"].ToString());
                    obj.Updated = DateTime.Parse(dr["Created"].ToString());
                    obj.Latitude = Convert.ToString(dr["Latitude"]);
                    obj.Longitude = Convert.ToString(dr["Longitude"]);
                    //list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }

        public static List<RoleModels> GetRole()
        {
            List<RoleModels> list = new List<RoleModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetRole();

                foreach (DataRow dr in dt.Rows)
                {
                    RoleModels obj = new RoleModels();

                    obj.ROLE_LEVEL = Convert.ToInt32(dr["ROLE_LEVEL"]);

                    obj.ROLE_DEPARTMENT = Convert.ToString(dr["ROLE_DEPARTMENT"]);

                    obj.ROLE_ACTIVE = Convert.ToBoolean(dr["ROLE_ACTIVE"]);

                    obj.ROLE_CREATED = Convert.ToDateTime(dr["ROLE_CREATED"]);

                    obj.ROLE_UPDATED = Convert.ToDateTime(dr["ROLE_UPDATED"]);

                    list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static bool AddRole(RoleModels obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddRole(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool NdAddRole(string roleName, string role, string[] chkGranted, string[] chkDenied, RoleModels obj)
        {
            bool result = false;
            //DataTable dtR01 = new DataTable();
            //check rolename first
            RoleModels m = new RoleModels();
            try
            {
                m.Role_Name = roleName;
                m.Position_Role = role;//
                if (chkGranted[0] != null)
                {
                    //get list check boxes
                    var R01 = chkGranted.Where(x => x.Contains("R01")).ToList();
                    var R02 = chkGranted.Where(x => x.Contains("R02")).ToList();
                    var R03 = chkGranted.Where(x => x.Contains("R03")).ToList();
                    var R04 = chkGranted.Where(x => x.Contains("R04")).ToList();
                    var R05 = chkGranted.Where(x => x.Contains("R05")).ToList();

                    var ALL = chkGranted.Where(x => x.Contains("ALL")).ToList();
                    var IND = chkGranted.Where(x => x.Contains("IND")).ToList();

                    var ADM = chkGranted.Where(x => x.Contains("6.1")).ToList();
                    var CDM = chkGranted.Where(x => x.Contains("6.2")).ToList();
                    var CRM = chkGranted.Where(x => x.Contains("6.3")).ToList();
                    var LOC = chkGranted.Where(x => x.Contains("6.4")).ToList();
                    var LRM = chkGranted.Where(x => x.Contains("6.5")).ToList();
                    var QRP = chkGranted.Where(x => x.Contains("6.6")).ToList();
                    var RAT = chkGranted.Where(x => x.Contains("6.7")).ToList();

                    //insert process
                    if (R01.Count() > -1) result = addRoleDetails(m, R01, "R01");
                    if (R02.Count() > -1) result = addRoleDetails(m, R02, "R02");
                    if (R03.Count() > -1) result = addRoleDetails(m, R03, "R03");
                    if (R04.Count() > -1) result = addRoleDetails(m, R04, "R04");
                    if (R05.Count() > -1) result = addRoleDetails(m, R05, "R05");

                    if (ALL.Count() > -1) result = addRoleDetails(m, ALL, "ALL");
                    if (IND.Count() > -1) result = addRoleDetails(m, IND, "IND");

                    if (ADM.Count() > -1) result = addRoleDetails(m, ADM, "ADM");
                    if (CDM.Count() > -1) result = addRoleDetails(m, CDM, "CDM");
                    if (CRM.Count() > -1) result = addRoleDetails(m, CRM, "CRM");
                    if (LOC.Count() > -1) result = addRoleDetails(m, LOC, "LOC");
                    if (LRM.Count() > -1) result = addRoleDetails(m, LRM, "LRM");
                    if (QRP.Count() > -1) result = addRoleDetails(m, QRP, "QRP");
                    if (RAT.Count() > -1) result = addRoleDetails(m, RAT, "RAT");
                }
                //result = true;

            }
            catch (Exception exc)
            {
                //throw exc;
                exc.Message.ToString();
            }
            return result;
        }

        public static bool addRoleDetails(RoleModels m, List<string> d, string groupReport)
        {
            bool result = false;
            DataTable dt = new DataTable();
            dt = getMS(groupReport, "", "rolem");
            string[] _groupreport = { "ADM", "CDM", "CRM", "LOC", "LRM", "QRP", "RAT" };
            char[] _space = new char[] { ' ' };
            int k = 0;

            foreach (DataRow dr in dt.Rows)
            {

                m.Module_No = dr["Module_No"].ToString();
                m.Module_Name = dr["Module_Name"].ToString();
                m.Group_Module = Convert.ToDecimal(dr["Group_Module"]);
                m.Is_Active = true;
                //m.Create_Date = DateTime.Now;
                //m.Update_Date = DateTime.Now;
                m.Remark = "";
                m.Group_Report = groupReport;

                if (d.Count != 0)
                {
                    if (_groupreport.Contains(groupReport))//สิทธิของกลุ่มรายงาน 
                    {
                        var head1 = d.Where(x => x.Contains(m.Module_Name) && x.Contains("column")).ToList();
                        var details1 = d.Where(x => x.Contains(m.Module_Name) && x.Contains(m.Group_Module.ToString()) && x.Contains("column") == false && x.Contains("IsSelected") == false).ToList();

                        if (head1.Count != 0 && k==0)
                        {
                            //loop
                            for (var j1 = 0; j1 < head1.Count; j1++)//Header Report
                            {
                                var RR = string.Empty;
                                var SS = head1[j1].Split(_space);
                                if (SS.Count() > 2)
                                {
                                    RR = SS[0].Replace("column", "");
                                    //m.Module_Name = SS[2].ToString();
                                    m.Group_No = SS[1].ToString();
                                }
                                switch (RR)
                                {
                                    case "List_Role": m.List_Role = true; break;
                                    case "Create_Role": m.Create_Role = true; break;
                                    case "View_Role": m.View_Role = true; break;
                                    case "Update_Role": m.Update_Role = true; break;
                                    case "Delete_Role": m.Delete_Role = true; break;
                                    case "Export_Role": m.Export_Role = true; break;
                                }
                            }

                        }
                        else if (head1.Count == 0 && k == 0)
                        {
                            m.List_Role = false;
                            m.Create_Role = false;
                            m.View_Role = false;
                            m.Update_Role = false;
                            m.Delete_Role = false;
                            m.Export_Role = false;
                        }
                        else if (head1.Count == 0 && details1.Count != 0)//Details Report
                        {
                            //loop
                            for (var j2 = 0; j2 < details1.Count; j2++)
                            {
                                var RR = "";// details1[j2].Substring(4);
                                if (_groupreport.Contains(groupReport))
                                {
                                    var SS = details1[j2].Split(_space);
                                    if (SS.Count() > 2)
                                    {
                                        RR = SS[1];
                                        //m.Module_Name = SS[2].ToString();
                                        m.Group_No = SS[2].ToString();
                                    }

                                }
                                switch (RR)
                                {
                                    case "List_Role": m.List_Role = true; break;
                                    case "Create_Role": m.Create_Role = true; break;
                                    case "View_Role": m.View_Role = true; break;
                                    case "Update_Role": m.Update_Role = true; break;
                                    case "Delete_Role": m.Delete_Role = true; break;
                                    case "Export_Role": m.Export_Role = true; break;
                                }
                            }
                        }
                        else if (head1.Count == 0 && details1.Count == 0)
                        {
                            m.List_Role = false;
                            m.Create_Role = false;
                            m.View_Role = false;
                            m.Update_Role = false;
                            m.Delete_Role = false;
                            m.Export_Role = false;
                        }
                    }
                    else if (!_groupreport.Contains(groupReport))//กลุ่มอื่นที่ไม่ใช่กลุ่มรายงาน
                    {
                        //clear for setting new checkbox seleted
                        m.List_Role = false;
                        m.Create_Role = false;
                        m.View_Role = false;
                        m.Update_Role = false;
                        m.Delete_Role = false;
                        m.Export_Role = false;
                        //begin set new checkbox seleted
                        for (var i = 0; i < d.Count(); i++)
                        {
                            var RR = d[i].Substring(4);
                            var SS = d[i].Split(_space);
                            if (SS.Count() > 2)
                            {
                                RR = SS[2];
                                //m.Module_Name = SS[2].ToString();
                                m.Group_No = SS[1].ToString();
                            }
                            switch (RR)
                            {
                                case "List_Role": m.List_Role = true; break;
                                case "Create_Role": m.Create_Role = true; break;
                                case "View_Role": m.View_Role = true; break;
                                case "Update_Role": m.Update_Role = true; break;
                                case "Delete_Role": m.Delete_Role = true; break;
                                case "Export_Role": m.Export_Role = true; break;
                            }
                        }
                    }

                    //********************************* OLD LOGIC
                    #region old logic
                    //if (_groupreport.Contains(groupReport) && d[k].Contains("column"))// Role Report Headers
                    //{
                    //    var RR = string.Empty;
                    //    var SS = d[k].Split(_space);
                    //    if (SS.Count() > 2)
                    //    {
                    //        RR = SS[0].Replace("column", "");
                    //        //m.Module_Name = SS[2].ToString();
                    //        m.Group_No = SS[1].ToString();
                    //    }

                    //    switch (RR)
                    //    {
                    //        case "List_Role": m.List_Role = true; break;
                    //        case "Create_Role": m.Create_Role = true; break;
                    //        case "View_Role": m.View_Role = true; break;
                    //        case "Update_Role": m.Update_Role = true; break;
                    //        case "Delete_Role": m.Delete_Role = true; break;
                    //        case "Export_Role": m.Export_Role = true; break;
                    //        default:
                    //            if (d[k].Substring(4) == "List_Role")
                    //            {
                    //                m.List_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "Create_Role")
                    //            {
                    //                m.Create_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "View_Role")
                    //            {
                    //                m.View_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "Update_Role")
                    //            {
                    //                m.Update_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "Delete_Role")
                    //            {
                    //                m.Delete_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "Export_Role")
                    //            {
                    //                m.Export_Role = false;
                    //            }
                    //            break;
                    //    }
                    //}
                    //else if (_groupreport.Contains(groupReport))// Role Report Details
                    //{
                    //    var RR = d[k].Substring(4);
                    //    if (_groupreport.Contains(groupReport))
                    //    {
                    //        var SS = d[k].Split(_space);
                    //        if (SS.Count() > 2)
                    //        {
                    //            RR = SS[1];
                    //            //m.Module_Name = SS[2].ToString();
                    //            m.Group_No = SS[2].ToString();
                    //        }

                    //    }

                    //    switch (RR)
                    //    {
                    //        case "List_Role": m.List_Role = true; break;
                    //        case "Create_Role": m.Create_Role = true; break;
                    //        case "View_Role": m.View_Role = true; break;
                    //        case "Update_Role": m.Update_Role = true; break;
                    //        case "Delete_Role": m.Delete_Role = true; break;
                    //        case "Export_Role": m.Export_Role = true; break;
                    //        default:
                    //            if (d[k].Substring(4) == "List_Role")
                    //            {
                    //                m.List_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "Create_Role")
                    //            {
                    //                m.Create_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "View_Role")
                    //            {
                    //                m.View_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "Update_Role")
                    //            {
                    //                m.Update_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "Delete_Role")
                    //            {
                    //                m.Delete_Role = false;
                    //            }
                    //            else if (d[k].Substring(4) == "Export_Role")
                    //            {
                    //                m.Export_Role = false;
                    //            }
                    //            break;
                    //    }
                    //}
                    //else if (!_groupreport.Contains(groupReport))// Role ที่ไม่เกี่ยวกับ Report
                    //{
                    //    for (var i = 0; i < d.Count(); i++)
                    //    {
                    //        var RR = d[i].Substring(4);
                    //        var SS = d[i].Split(_space);
                    //        if (SS.Count() > 2)
                    //        {
                    //            RR = SS[2];
                    //            //m.Module_Name = SS[2].ToString();
                    //            m.Group_No = SS[1].ToString();
                    //        }
                    //        switch (RR)
                    //        {
                    //            case "List_Role": m.List_Role = true; break;
                    //            case "Create_Role": m.Create_Role = true; break;
                    //            case "View_Role": m.View_Role = true; break;
                    //            case "Update_Role": m.Update_Role = true; break;
                    //            case "Delete_Role": m.Delete_Role = true; break;
                    //            case "Export_Role": m.Export_Role = true; break;
                    //            default:
                    //                if (d[i].Substring(4) == "List_Role")
                    //                {
                    //                    m.List_Role = false;
                    //                }
                    //                else if (d[i].Substring(4) == "Create_Role")
                    //                {
                    //                    m.Create_Role = false;
                    //                }
                    //                else if (d[i].Substring(4) == "View_Role")
                    //                {
                    //                    m.View_Role = false;
                    //                }
                    //                else if (d[i].Substring(4) == "Update_Role")
                    //                {
                    //                    m.Update_Role = false;
                    //                }
                    //                else if (d[i].Substring(4) == "Delete_Role")
                    //                {
                    //                    m.Delete_Role = false;
                    //                }
                    //                else if (d[i].Substring(4) == "Export_Role")
                    //                {
                    //                    m.Export_Role = false;
                    //                }
                    //                break;
                    //        }
                    //    }

                    //}
                    #endregion
                }
                else
                {
                    m.Group_No = m.Module_No.Length > 3 ? m.Module_No.Substring(0, 3) : m.Module_No;//20250421
                    m.List_Role = false;
                    m.Create_Role = false;
                    m.View_Role = false;
                    m.Update_Role = false;
                    m.Delete_Role = false;
                    m.Export_Role = false;
                }

                try
                {
                    result = AdministratorDAL.NdInsertRole(m);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                k++;
            }
            return result;
        }

        public static bool NdEditRole(string roleName, string role, string[] chkGranted, string[] chkDenied, RoleModels obj)
        {
            bool result = false;
            //DataTable dtR01 = new DataTable();
            //check rolename first
            RoleModels m = new RoleModels();
            try
            {
                m.Role_Name = roleName;
                m.Position_Role = role;//
                if (chkGranted[0] != null)
                {
                    //get list check boxes
                    var R01 = chkGranted.Where(x => x.Contains("R01")).ToList();
                    var R02 = chkGranted.Where(x => x.Contains("R02")).ToList();
                    var R03 = chkGranted.Where(x => x.Contains("R03")).ToList();
                    var R04 = chkGranted.Where(x => x.Contains("R04")).ToList();
                    var R05 = chkGranted.Where(x => x.Contains("R05")).ToList();

                    var ALL = chkGranted.Where(x => x.Contains("ALL")).ToList();
                    var IND = chkGranted.Where(x => x.Contains("IND")).ToList();

                    var ADM = chkGranted.Where(x => x.Contains("6.1")).ToList();
                    var CDM = chkGranted.Where(x => x.Contains("6.2")).ToList();
                    var CRM = chkGranted.Where(x => x.Contains("6.3")).ToList();
                    var LOC = chkGranted.Where(x => x.Contains("6.4")).ToList();
                    var LRM = chkGranted.Where(x => x.Contains("6.5")).ToList();
                    var QRP = chkGranted.Where(x => x.Contains("6.6")).ToList();
                    var RAT = chkGranted.Where(x => x.Contains("6.7")).ToList();

                    //insert process
                    if (R01.Count() > -1) result = EditRoleDetails(m, R01, "R01");
                    if (R02.Count() > -1) result = EditRoleDetails(m, R02, "R02");
                    if (R03.Count() > -1) result = EditRoleDetails(m, R03, "R03");
                    if (R04.Count() > -1) result = EditRoleDetails(m, R04, "R04");
                    if (R05.Count() > -1) result = EditRoleDetails(m, R05, "R05");

                    if (ALL.Count() > -1) result = EditRoleDetails(m, ALL, "ALL");
                    if (IND.Count() > -1) result = EditRoleDetails(m, IND, "IND");

                    if (ADM.Count() > -1) result = EditRoleDetails(m, ADM, "ADM");
                    if (CDM.Count() > -1) result = EditRoleDetails(m, CDM, "CDM");
                    if (CRM.Count() > -1) result = EditRoleDetails(m, CRM, "CRM");
                    if (LOC.Count() > -1) result = EditRoleDetails(m, LOC, "LOC");
                    if (LRM.Count() > -1) result = EditRoleDetails(m, LRM, "LRM");
                    if (QRP.Count() > -1) result = EditRoleDetails(m, QRP, "QRP");
                    if (RAT.Count() > -1) result = EditRoleDetails(m, RAT, "RAT");
                }
                //result = true;

            }
            catch (Exception exc)
            {
                //throw exc;
                exc.Message.ToString();
            }
            return result;
        }

        public static bool EditRoleDetails(RoleModels m, List<string> d, string groupReport)
        {
            bool result = false;
            DataTable dt = new DataTable();
            dt = getMS(groupReport, "", "rolem");
            string[] _groupreport = { "ADM", "CDM", "CRM", "LOC", "LRM", "QRP", "RAT" };
            char[] _space = new char[] { ' ' };
            int k = 0;

            foreach (DataRow dr in dt.Rows)
            {

                m.Module_No = dr["Module_No"].ToString();
                m.Module_Name = dr["Module_Name"].ToString();
                m.Group_Module = Convert.ToDecimal(dr["Group_Module"]);
                m.Is_Active = true;
                //m.Create_Date = DateTime.Now;
                //m.Update_Date = DateTime.Now;
                m.Remark = "";
                m.Group_Report = groupReport;

                if (d.Count != 0)
                {
                    if (_groupreport.Contains(groupReport))//สิทธิของกลุ่มรายงาน 
                    {
                        var head1 = d.Where(x => x.Contains(m.Module_Name) && x.Contains("column")).ToList();
                        var details1 = d.Where(x => x.Contains(m.Module_Name) && x.Contains("column") == false && x.Contains("IsSelected") == false).ToList();

                        if (head1.Count != 0 && k == 0)
                        {
                            //loop
                            for (var j1 = 0; j1 < head1.Count; j1++)//Header Report
                            {
                                var RR = string.Empty;
                                var SS = head1[j1].Split(_space);
                                if (SS.Count() > 2)
                                {
                                    RR = SS[0].Replace("column", "");
                                    //m.Module_Name = SS[2].ToString();
                                    m.Group_No = SS[1].ToString();
                                }
                                switch (RR)
                                {
                                    case "List_Role": m.List_Role = true; break;
                                    case "Create_Role": m.Create_Role = true; break;
                                    case "View_Role": m.View_Role = true; break;
                                    case "Update_Role": m.Update_Role = true; break;
                                    case "Delete_Role": m.Delete_Role = true; break;
                                    case "Export_Role": m.Export_Role = true; break;
                                }
                            }

                        }
                        else if (head1.Count == 0 && k == 0)
                        {
                            m.List_Role = false;
                            m.Create_Role = false;
                            m.View_Role = false;
                            m.Update_Role = false;
                            m.Delete_Role = false;
                            m.Export_Role = false;
                        }
                        else if (head1.Count == 0 && details1.Count != 0)//Details Report
                        {
                            //loop
                            for (var j2 = 0; j2 < details1.Count; j2++)
                            {
                                var RR = "";// details1[j2].Substring(4);
                                if (_groupreport.Contains(groupReport))
                                {
                                    var SS = details1[j2].Split(_space);
                                    if (SS.Count() > 2)
                                    {
                                        RR = SS[1];
                                        //m.Module_Name = SS[2].ToString();
                                        m.Group_No = SS[2].ToString();
                                    }

                                }
                                switch (RR)
                                {
                                    case "List_Role": m.List_Role = true; break;
                                    case "Create_Role": m.Create_Role = true; break;
                                    case "View_Role": m.View_Role = true; break;
                                    case "Update_Role": m.Update_Role = true; break;
                                    case "Delete_Role": m.Delete_Role = true; break;
                                    case "Export_Role": m.Export_Role = true; break;
                                }
                            }
                        }
                        else if (head1.Count == 0 && details1.Count == 0)
                        {
                            m.List_Role = false;
                            m.Create_Role = false;
                            m.View_Role = false;
                            m.Update_Role = false;
                            m.Delete_Role = false;
                            m.Export_Role = false;
                        }
                    }
                    else if (!_groupreport.Contains(groupReport))//กลุ่มอื่นที่ไม่ใช่กลุ่มรายงาน
                    {
                        //clear for setting new checkbox seleted
                        m.List_Role = false;
                        m.Create_Role = false;
                        m.View_Role = false;
                        m.Update_Role = false;
                        m.Delete_Role = false;
                        m.Export_Role = false;
                        //begin set new checkbox seleted
                        for (var i = 0; i < d.Count(); i++)
                        {
                            var RR = d[i].Substring(4);
                            var SS = d[i].Split(_space);
                            if (SS.Count() > 2)
                            {
                                RR = SS[2];
                                //m.Module_Name = SS[2].ToString();
                                m.Group_No = SS[1].ToString();
                            }
                            switch (RR)
                            {
                                case "List_Role": m.List_Role = true; break;
                                case "Create_Role": m.Create_Role = true; break;
                                case "View_Role": m.View_Role = true; break;
                                case "Update_Role": m.Update_Role = true; break;
                                case "Delete_Role": m.Delete_Role = true; break;
                                case "Export_Role": m.Export_Role = true; break;
                            }
                        }
                    }
                }
                else
                {
                    m.Group_No = m.Module_No.Length > 3 ? m.Module_No.Substring(0, 3) : m.Module_No;
                    m.List_Role = false;
                    m.Create_Role = false;
                    m.View_Role = false;
                    m.Update_Role = false;
                    m.Delete_Role = false;
                    m.Export_Role = false;
                }

                try
                {
                    result = AdministratorDAL.NdEditRole(m);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                k++;
            }
            return result;
        }

        public static List<RoleModuleMasterModel> GetAlldRole()
        {
            List<RoleModuleMasterModel> list = new List<RoleModuleMasterModel>();
            try
            {
                DataTable dt = AdministratorDAL.GetAlldRole();
                foreach (DataRow r in dt.Rows)
                {
                    RoleModuleMasterModel dr = new RoleModuleMasterModel();
                    dr.Id = Convert.ToInt64(r["ID"].ToString());
                    dr.Module_No = r["Module_No"].ToString();
                    dr.Group_Module = Convert.ToDecimal(r["Group_Module"].ToString());
                    dr.Module_Name = r["Module_Name"].ToString();
                    dr.Description = r["Description"].ToString();
                    dr.Create_Date = DateTime.Parse(r["Create_Date"].ToString());
                    dr.Update_Date = DateTime.Parse(r["Update_Date"].ToString());
                    dr.Group_Report = r["Group_Report"].ToString();
                    dr.Group_No = r["Group_No"].ToString();
                    list.Add(dr);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return list;
        }

        public static RoleModels GetRoleByID(long ID)
        {
            RoleModels obj = new RoleModels();

            try
            {
                DataTable dt = AdministratorDAL.GetRoleByID(ID);
                if (dt != null && dt.Rows.Count > 0)
                {

                    obj.ROLE_LEVEL = Convert.ToInt64(dt.Rows[0]["ROLE_LEVEL"]);

                    obj.ROLE_DEPARTMENT = Convert.ToString(dt.Rows[0]["ROLE_DEPARTMENT"]);

                    obj.ROLE_ACTIVE = Convert.ToBoolean(dt.Rows[0]["ROLE_ACTIVE"]);

                    obj.ROLE_CREATED = Convert.ToDateTime(dt.Rows[0]["ROLE_CREATED"]);

                    obj.ROLE_UPDATED = Convert.ToDateTime(dt.Rows[0]["ROLE_UPDATED"]);

                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }



        public static bool EditRole(RoleModels obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditRole(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool DeleteRoleByID(long lngID)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteRoleByID(lngID);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static List<BranchModel> GetBranch()
        {
            List<BranchModel> list = new List<BranchModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetBranch();

                foreach (DataRow dr in dt.Rows)
                {
                    BranchModel obj = new BranchModel();
                    //[BRANCH]
                    obj.BRANCH = Convert.ToString(dr["BRANCH"]);
                    //,[DESC]
                    obj.DESC = Convert.ToString(dr["DESC"]);
                    //,[DESC_ENG]
                    obj.DESC_ENG = Convert.ToString(dr["DESC_ENG"]);
                    //,[ZONE]
                    obj.ZONE = Convert.ToString(dr["ZONE"]);
                    //,[REGION]
                    obj.REGION = Convert.ToString(dr["REGION"]);
                    //,[EMAIL]
                    obj.EMAIL = Convert.ToString(dr["EMAIL"]);
                    //,[EXCEPTREGION]
                    obj.EXCEPTREGION = Convert.ToString(dr["EXCEPTREGION"]);
                    //,[BNREGION]
                    obj.BNREGION = Convert.ToString(dr["BNREGION"]);
                    //,[FLAGCLOSE]
                    obj.FLAGCLOSE = Convert.ToString(dr["FLAGCLOSE"]);
                    //,[Alt1]
                    obj.Alt1 = Convert.ToString(dr["Alt1"]);
                    //,[Alt2]
                    obj.Alt2 = Convert.ToString(dr["Alt2"]);
                    //,[Alt3]
                    obj.Alt3 = Convert.ToString(dr["Alt3"]);
                    list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static BranchModel GetBranchByID(string BRANCH)
        {
            BranchModel obj = new BranchModel();

            try
            {
                DataTable dt = AdministratorDAL.GetBranchByID(BRANCH);
                if (dt != null && dt.Rows.Count > 0)
                {
                    //[BRANCH]
                    obj.BRANCH = Convert.ToString(dt.Rows[0]["BRANCH"]);
                    //,[DESC]
                    obj.DESC = Convert.ToString(dt.Rows[0]["DESC"]);
                    //,[DESC_ENG]
                    obj.DESC_ENG = Convert.ToString(dt.Rows[0]["DESC_ENG"]);
                    //,[ZONE]
                    obj.ZONE = Convert.ToString(dt.Rows[0]["ZONE"]);
                    //,[REGION]
                    obj.REGION = Convert.ToString(dt.Rows[0]["REGION"]);
                    //,[EMAIL]
                    obj.EMAIL = Convert.ToString(dt.Rows[0]["EMAIL"]);
                    //,[EXCEPTREGION]
                    obj.EXCEPTREGION = Convert.ToString(dt.Rows[0]["EXCEPTREGION"]);
                    //,[BNREGION]
                    obj.BNREGION = Convert.ToString(dt.Rows[0]["BNREGION"]);
                    //,[FLAGCLOSE]
                    obj.FLAGCLOSE = Convert.ToString(dt.Rows[0]["FLAGCLOSE"]);
                    //,[Alt1]
                    obj.Alt1 = Convert.ToString(dt.Rows[0]["Alt1"]);
                    //,[Alt2]
                    obj.Alt2 = Convert.ToString(dt.Rows[0]["Alt2"]);
                    //,[Alt3]
                    obj.Alt3 = Convert.ToString(dt.Rows[0]["Alt3"]);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }
        public static bool AddBranch(BranchModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddBranch(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool EditBranch(BranchModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditBranch(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool DeleteBranchByID(string BRANCH)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteBranchByID(BRANCH);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static List<GroupDetailReportModel> GetGroupDetailReport()
        {
            List<GroupDetailReportModel> list = new List<GroupDetailReportModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetGroupDetailReport();

                foreach (DataRow dr in dt.Rows)
                {
                    GroupDetailReportModel obj = new GroupDetailReportModel();
                    //[ID_REPORT]
                    obj.ID_REPORT = Convert.ToInt32(dr["ID_REPORT"]);
                    //,[GROUP_NO]
                    obj.GROUP_NO = Convert.ToDecimal(dr["GROUP_NO"]);
                    obj.GROUP_NAME = Convert.ToString(dr["GROUP_NAME"]);
                    //,[REPORT_NO]
                    obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"]);
                    //,[REPORT_NAME]
                    obj.REPORT_NAME = Convert.ToString(dr["REPORT_NAME"]);
                    //,[ACTIVE]
                    obj.ACTIVE = Convert.ToBoolean(dr["ACTIVE"]);
                    //,[CREATE_DATE]
                    obj.CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]);
                    //,[UPDATE_DATE]
                    obj.UPDATE_DATE = Convert.ToDateTime(dr["UPDATE_DATE"]);
                    list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static GroupDetailReportModel GetGroupDetailReportByID(int intID)
        {
            GroupDetailReportModel obj = new GroupDetailReportModel();

            try
            {
                DataTable dt = AdministratorDAL.GetGroupDetailReportByID(intID);

                if (dt.Rows.Count > 0)
                {

                    //[ID_REPORT]
                    obj.ID_REPORT = Convert.ToInt32(dt.Rows[0]["ID_REPORT"]);
                    //,[GROUP_NO]
                    obj.GROUP_NO = Convert.ToDecimal(dt.Rows[0]["GROUP_NO"]);
                    //,[REPORT_NO]
                    obj.REPORT_NO = Convert.ToDecimal(dt.Rows[0]["REPORT_NO"]);
                    //,[REPORT_NAME]
                    obj.REPORT_NAME = Convert.ToString(dt.Rows[0]["REPORT_NAME"]);
                    //,[ACTIVE]
                    obj.ACTIVE = Convert.ToBoolean(dt.Rows[0]["ACTIVE"]);
                    //,[CREATE_DATE]
                    obj.CREATE_DATE = Convert.ToDateTime(dt.Rows[0]["CREATE_DATE"]);
                    //,[UPDATE_DATE]
                    obj.UPDATE_DATE = Convert.ToDateTime(dt.Rows[0]["UPDATE_DATE"]);

                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }
        public static bool AddGroupDetailReport(GroupDetailReportModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddGroupDetailReport(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool EditGroupDetailReport(GroupDetailReportModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditGroupDetailReport(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool DeleteGroupDetailReportByID(int ID)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteGroupDetailReportByID(ID);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static List<UserModels> GetUser()
        {
            Loger _logSys = new Loger();
            string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
            List<UserModels> list = new List<UserModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetUser();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        UserModels obj = new UserModels();
                        ApiPostResponse2 obj2 = new ApiPostResponse2();
                        obj.USER_ID = Convert.ToInt64(dr["USER_ID"]);
                        obj.USER_LOGIN = Convert.ToString(dr["USER_LOGIN"] != DBNull.Value ? dr["USER_LOGIN"] : string.Empty);
                        obj.USER_PASSWORD = Convert.ToString(dr["USER_PASSWORD"] != DBNull.Value ? dr["USER_PASSWORD"] : string.Empty);
                        obj.USER_PASSWORD_UPDATE_TIME = Convert.ToDateTime(dr["USER_PASSWORD_UPDATE_TIME"]);
                        obj.USER_FIRSTNAME = Convert.ToString(dr["USER_FIRSTNAME"] != DBNull.Value ? dr["USER_FIRSTNAME"] : string.Empty);
                        obj.USER_LASTNAME = Convert.ToString(dr["USER_LASTNAME"] != DBNull.Value ? dr["USER_LASTNAME"] : string.Empty);
                        obj.USER_DEPARTMENT = Convert.ToString(dr["USER_DEPARTMENT"] != DBNull.Value ? dr["USER_DEPARTMENT"] : string.Empty);
                        obj.USER_POSITION = Convert.ToString(dr["USER_POSITION"] != DBNull.Value ? dr["USER_POSITION"] : string.Empty);
                        obj.USER_LEVEL = Convert.ToInt32(dr["USER_LEVEL"] != DBNull.Value ? dr["USER_LEVEL"] : 0);
                        obj.USER_EMAIL = Convert.ToString(dr["USER_EMAIL"] != DBNull.Value ? dr["USER_EMAIL"] : string.Empty);
                        obj.USER_LOCK = Convert.ToBoolean(dr["USER_LOCK"] != DBNull.Value ? dr["USER_LOCK"] : 0);
                        obj.IS_ACTIVE = Convert.ToBoolean(dr["IS_ACTIVE"] != DBNull.Value ? dr["IS_ACTIVE"] : 0);
                        obj.User_Branch = Convert.ToString(dr["SOL_CODE"] != DBNull.Value ? dr["SOL_CODE"] : string.Empty);
                        obj.User_Role_Level = Convert.ToString(dr["ROLE_NAME"] != DBNull.Value ? dr["ROLE_NAME"] : "รอดำเนินการ");
                        obj.USER_EMP_CODE = Convert.ToString(dr["USER_EMP_CODE"] != DBNull.Value ? dr["USER_EMP_CODE"] : "");
                        obj.SOL_NAME = Convert.ToString(dr["SOL_NAME"] != DBNull.Value ? dr["SOL_NAME"] : string.Empty);
                        obj.Dept_Code = Convert.ToString(dr["Dept_Code"] != DBNull.Value ? dr["Dept_Code"] : string.Empty);
                        obj.Dept_Name = Convert.ToString(dr["Dept_Name"] != DBNull.Value ? dr["Dept_Name"] : string.Empty);
                        obj.Hub_Code = Convert.ToString(dr["Zone_Code"] != DBNull.Value ? dr["Zone_Code"] : string.Empty);
                        obj.Hub_Name = Convert.ToString(dr["Zone_Name"] != DBNull.Value ? dr["Zone_Name"] : string.Empty);
                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        //throw exc;
                        exc.Message.ToString();
                        _logSys.WriteProcessLogFile(_strPathFile, "GetUser :  " + exc.Message.ToString());
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<RoleModels> NdGetRoleManagement()
        {
            List<RoleModels> list = new List<RoleModels>();

            try
            {
                DataTable dt = AdministratorDAL.NdGetRoleManagement();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        RoleModels obj = new RoleModels();

                        //obj.ID_Role = Convert.ToInt64(dr["ID_Role"]);
                        //obj.Position_Role = Convert.ToString(dr["Position_Role"] != DBNull.Value ? dr["Position_Role"] : string.Empty);
                        obj.Role_Name = Convert.ToString(dr["Role_Name"] != DBNull.Value ? dr["Role_Name"] : string.Empty);
                        //obj.Module_No = Convert.ToString(dr["Module_No"] != DBNull.Value ? dr["Module_No"] : string.Empty);
                        //obj.Module_Name = Convert.ToString(dr["Module_Name"] != DBNull.Value ? dr["Module_Name"] : string.Empty);
                        //obj.Group_Module = Convert.ToDecimal(dr["Group_Module"]);
                        ////obj.RoleReport_No = Convert.ToDecimal(dr["RoleReport_No"]);
                        //obj.Group_No = Convert.ToString(dr["Group_No"] != DBNull.Value ? dr["Group_No"] : string.Empty);
                        //obj.Group_Report = Convert.ToString(dr["Group_Report"] != DBNull.Value ? dr["Group_Report"] : string.Empty);
                        //obj.RoleReport_Name = Convert.ToString(dr["RoleReport_Name"] != DBNull.Value ? dr["RoleReport_Name"] : string.Empty);
                        //obj.List_Role = Convert.ToBoolean(dr["List_Role"] != DBNull.Value ? dr["List_Role"] : 0);
                        //obj.Create_Role = Convert.ToBoolean(dr["Create_Role"] != DBNull.Value ? dr["Create_Role"] : 0);
                        //obj.View_Role = Convert.ToBoolean(dr["View_Role"] != DBNull.Value ? dr["View_Role"] : 0);
                        //obj.Update_Role = Convert.ToBoolean(dr["Update_Role"] != DBNull.Value ? dr["Update_Role"] : 0);
                        //obj.Delete_Role = Convert.ToBoolean(dr["Delete_Role"] != DBNull.Value ? dr["Delete_Role"] : 0);
                        //obj.Export_Role = Convert.ToBoolean(dr["Export_Role"] != DBNull.Value ? dr["Export_Role"] : 0);
                        //obj.Is_Active = Convert.ToBoolean(dr["Is_Active"] != DBNull.Value ? dr["Is_Active"] : 0);
                        //obj.Create_Date = Convert.ToDateTime(dr["Create_Date"]);
                        //obj.Update_Date = Convert.ToDateTime(dr["Update_Date"]);
                        //obj.Remark = Convert.ToString(dr["Remark"] != DBNull.Value ? dr["Remark"] : string.Empty);
                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        exc.Message.ToString();
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static RoleModels NdCheckDupRoleName(string _c, string _n , string _o )
        {
            RoleModels chkDup = new RoleModels();
            try
            {
                DataTable dt = AdministratorDAL.NdCheckDupRoleName(_c, _n , _o);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {

                        chkDup.Role_Name = Convert.ToString(dr["Role_Name"] != DBNull.Value ? dr["Role_Name"] : string.Empty);                        
                        
                    }
                    catch (Exception exc)
                    {
                        exc.Message.ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return chkDup;
        }

        public static List<RoleModels> NdCheckDupRoleName_lst(string _c, string _n, string _o)
        {
            List<RoleModels> lst_chkDup = new List<RoleModels>();
            try
            {
                DataTable dt = AdministratorDAL.NdCheckDupRoleName(_c, _n, _o);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        RoleModels chkDup = new RoleModels();
                        chkDup.Role_Name = Convert.ToString(dr["Role_Name"] != DBNull.Value ? dr["Role_Name"] : string.Empty);
                        lst_chkDup.Add(chkDup);
                    }
                    catch (Exception exc)
                    {
                        exc.Message.ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return lst_chkDup;
        }

        public static List<RoleModels> NdGetRoleManagementDetials(string _c, string _n, string _o)
        {
            List<RoleModels> list = new List<RoleModels>();

            try
            {
                DataTable dt = AdministratorDAL.NdGetRoleManagementDetials(_c, _n, _o);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        RoleModels obj = new RoleModels();

                        obj.ID_Role = Convert.ToInt64(dr["ID_Role"]);
                        obj.Position_Role = Convert.ToString(dr["Position_Role"] != DBNull.Value ? dr["Position_Role"] : string.Empty);
                        obj.Role_Name = Convert.ToString(dr["Role_Name"] != DBNull.Value ? dr["Role_Name"] : string.Empty);
                        obj.Module_No = Convert.ToString(dr["Module_No"] != DBNull.Value ? dr["Module_No"] : string.Empty);
                        obj.Module_Name = Convert.ToString(dr["Module_Name"] != DBNull.Value ? dr["Module_Name"] : string.Empty);
                        obj.Group_Module = Convert.ToDecimal(dr["Group_Module"]);
                        //obj.RoleReport_No = Convert.ToDecimal(dr["RoleReport_No"]);
                        obj.Group_No = Convert.ToString(dr["Group_No"] != DBNull.Value ? dr["Group_No"] : string.Empty);
                        obj.Group_Report = Convert.ToString(dr["Group_Report"] != DBNull.Value ? dr["Group_Report"] : string.Empty);
                        obj.RoleReport_Name = Convert.ToString(dr["RoleReport_Name"] != DBNull.Value ? dr["RoleReport_Name"] : string.Empty);
                        obj.List_Role = Convert.ToBoolean(dr["List_Role"] != DBNull.Value ? dr["List_Role"] : 0);
                        obj.Create_Role = Convert.ToBoolean(dr["Create_Role"] != DBNull.Value ? dr["Create_Role"] : 0);
                        obj.View_Role = Convert.ToBoolean(dr["View_Role"] != DBNull.Value ? dr["View_Role"] : 0);
                        obj.Update_Role = Convert.ToBoolean(dr["Update_Role"] != DBNull.Value ? dr["Update_Role"] : 0);
                        obj.Delete_Role = Convert.ToBoolean(dr["Delete_Role"] != DBNull.Value ? dr["Delete_Role"] : 0);
                        obj.Export_Role = Convert.ToBoolean(dr["Export_Role"] != DBNull.Value ? dr["Export_Role"] : 0);
                        obj.Is_Active = Convert.ToBoolean(dr["Is_Active"] != DBNull.Value ? dr["Is_Active"] : 0);
                        obj.Create_Date = Convert.ToDateTime(dr["Create_Date"]);
                        obj.Update_Date = Convert.ToDateTime(dr["Update_Date"]);
                        obj.Remark = Convert.ToString(dr["Remark"] != DBNull.Value ? dr["Remark"] : string.Empty);
                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        exc.Message.ToString();
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<UserModels> NdGetUserTransfer()
        {
            List<UserModels> list = new List<UserModels>();

            try
            {
                DataTable dt = AdministratorDAL.NdGetUserTransfer();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        UserModels obj = new UserModels();
                        obj.USER_ID = Convert.ToInt64(dr["USER_ID"]);
                        obj.USER_LOGIN = Convert.ToString(dr["USER_LOGIN"] != DBNull.Value ? dr["USER_LOGIN"] : string.Empty);
                        obj.USER_PASSWORD = Convert.ToString(dr["USER_PASSWORD"] != DBNull.Value ? dr["USER_PASSWORD"] : string.Empty);
                        obj.USER_PASSWORD_UPDATE_TIME = Convert.ToDateTime(dr["USER_PASSWORD_UPDATE_TIME"]);
                        obj.USER_FIRSTNAME = Convert.ToString(dr["USER_FIRSTNAME"] != DBNull.Value ? dr["USER_FIRSTNAME"] : string.Empty);
                        obj.USER_LASTNAME = Convert.ToString(dr["USER_LASTNAME"] != DBNull.Value ? dr["USER_LASTNAME"] : string.Empty);
                        obj.USER_DEPARTMENT = Convert.ToString(dr["USER_DEPARTMENT"] != DBNull.Value ? dr["USER_DEPARTMENT"] : string.Empty);
                        obj.USER_POSITION = Convert.ToString(dr["USER_POSITION"] != DBNull.Value ? dr["USER_POSITION"] : string.Empty);
                        obj.USER_LEVEL = Convert.ToInt32(dr["USER_LEVEL"] != DBNull.Value ? dr["USER_LEVEL"] : 0);
                        obj.USER_EMAIL = Convert.ToString(dr["USER_EMAIL"] != DBNull.Value ? dr["USER_EMAIL"] : string.Empty);
                        obj.USER_LOCK = Convert.ToBoolean(dr["USER_LOCK"] != DBNull.Value ? dr["USER_LOCK"] : 0);
                        obj.IS_ACTIVE = Convert.ToBoolean(dr["IS_ACTIVE"] != DBNull.Value ? dr["IS_ACTIVE"] : 0);
                        obj.User_Branch = Convert.ToString(dr["SOL_CODE"] != DBNull.Value ? dr["SOL_CODE"] : string.Empty);
                        obj.User_Role_Level = Convert.ToString(dr["ROLE_NAME"] != DBNull.Value ? dr["ROLE_NAME"] : "รอดำเนินการ");
                        obj.USER_EMP_CODE = Convert.ToString(dr["USER_EMP_CODE"] != DBNull.Value ? dr["USER_EMP_CODE"] : "");
                        obj.SOL_NAME = Convert.ToString(dr["SOL_NAME"] != DBNull.Value ? dr["SOL_NAME"] : string.Empty);
                        obj.Dept_Code = Convert.ToString(dr["Dept_Code"] != DBNull.Value ? dr["Dept_Code"] : string.Empty);
                        obj.Dept_Name = Convert.ToString(dr["Dept_Name"] != DBNull.Value ? dr["Dept_Name"] : string.Empty);
                        obj.Hub_Code = Convert.ToString(dr["Zone_Code"] != DBNull.Value ? dr["Zone_Code"] : string.Empty);
                        obj.Hub_Name = Convert.ToString(dr["Zone_Name"] != DBNull.Value ? dr["Zone_Name"] : string.Empty);
                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static UserModels NdGetEditUserTransfer()
        {
            UserModels obj = new UserModels();

            try
            {
                DataTable dt = AdministratorDAL.NdGetUserTransfer();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        //UserModels obj = new UserModels();
                        obj.USER_ID = Convert.ToInt64(dr["USER_ID"]);
                        obj.USER_LOGIN = Convert.ToString(dr["USER_LOGIN"] != DBNull.Value ? dr["USER_LOGIN"] : string.Empty);
                        obj.USER_PASSWORD = Convert.ToString(dr["USER_PASSWORD"] != DBNull.Value ? dr["USER_PASSWORD"] : string.Empty);
                        obj.USER_PASSWORD_UPDATE_TIME = Convert.ToDateTime(dr["USER_PASSWORD_UPDATE_TIME"]);
                        obj.USER_FIRSTNAME = Convert.ToString(dr["USER_FIRSTNAME"] != DBNull.Value ? dr["USER_FIRSTNAME"] : string.Empty);
                        obj.USER_LASTNAME = Convert.ToString(dr["USER_LASTNAME"] != DBNull.Value ? dr["USER_LASTNAME"] : string.Empty);
                        obj.USER_DEPARTMENT = Convert.ToString(dr["USER_DEPARTMENT"] != DBNull.Value ? dr["USER_DEPARTMENT"] : string.Empty);
                        obj.USER_POSITION = Convert.ToString(dr["USER_POSITION"] != DBNull.Value ? dr["USER_POSITION"] : string.Empty);
                        obj.USER_LEVEL = Convert.ToInt32(dr["USER_LEVEL"] != DBNull.Value ? dr["USER_LEVEL"] : 0);
                        obj.USER_EMAIL = Convert.ToString(dr["USER_EMAIL"] != DBNull.Value ? dr["USER_EMAIL"] : string.Empty);
                        obj.USER_LOCK = Convert.ToBoolean(dr["USER_LOCK"] != DBNull.Value ? dr["USER_LOCK"] : 0);
                        obj.IS_ACTIVE = Convert.ToBoolean(dr["IS_ACTIVE"] != DBNull.Value ? dr["IS_ACTIVE"] : 0);
                        obj.User_Branch = Convert.ToString(dr["SOL_CODE"] != DBNull.Value ? dr["SOL_CODE"] : string.Empty);
                        obj.User_Role_Level = Convert.ToString(dr["ROLE_NAME"] != DBNull.Value ? dr["ROLE_NAME"] : "รอดำเนินการ");
                        obj.USER_EMP_CODE = Convert.ToString(dr["USER_EMP_CODE"] != DBNull.Value ? dr["USER_EMP_CODE"] : "");
                        obj.SOL_NAME = Convert.ToString(dr["SOL_NAME"] != DBNull.Value ? dr["SOL_NAME"] : string.Empty);
                        obj.Dept_Code = Convert.ToString(dr["Dept_Code"] != DBNull.Value ? dr["Dept_Code"] : string.Empty);
                        obj.Dept_Name = Convert.ToString(dr["Dept_Name"] != DBNull.Value ? dr["Dept_Name"] : string.Empty);
                        obj.Hub_Code = Convert.ToString(dr["Zone_Code"] != DBNull.Value ? dr["Zone_Code"] : string.Empty);
                        obj.Hub_Name = Convert.ToString(dr["Zone_Name"] != DBNull.Value ? dr["Zone_Name"] : string.Empty);

                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }

        public static List<UserTransferScheduleModel> NdGetUserTransfer2()
        {
            List<UserTransferScheduleModel> list = new List<UserTransferScheduleModel>();

            try
            {
                DataTable dt = AdministratorDAL.NdGetUserTransfer();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        UserTransferScheduleModel obj = new UserTransferScheduleModel();
                        //UserTransferScheduleModel
                        obj.Transfer_Id = Convert.ToInt16(dr["transfer_id"]);
                        obj.emp_code = Convert.ToString(dr["emp_code"] != DBNull.Value ? dr["emp_code"] : string.Empty);
                        obj.fname = Convert.ToString(dr["fname"] != DBNull.Value ? dr["fname"] : string.Empty);
                        obj.lname = Convert.ToString(dr["lname"] != DBNull.Value ? dr["lname"] : string.Empty);
                        obj.is_active = Convert.ToBoolean(dr["is_active"]);
                        obj.sch_time = Convert.ToDateTime(dr["sch_time"]);
                        obj.start_date = Convert.ToDateTime(dr["start_date"]);
                        obj.end_date = Convert.ToDateTime(dr["end_date"]);
                        obj.branch_code = Convert.ToString(dr["branch_code"] != DBNull.Value ? dr["branch_code"] : string.Empty);
                        obj.branch_name = Convert.ToString(dr["branch_name"] != DBNull.Value ? dr["branch_name"] : string.Empty);
                        obj.hub_code = Convert.ToString(dr["hub_code"] != DBNull.Value ? dr["hub_code"] : string.Empty);
                        obj.hub_name = Convert.ToString(dr["hub_name"] != DBNull.Value ? dr["hub_name"] : string.Empty);
                        obj.dept_code = Convert.ToString(dr["dept_code"] != DBNull.Value ? dr["dept_code"] : string.Empty);
                        obj.dept_name = Convert.ToString(dr["dept_name"] != DBNull.Value ? dr["dept_name"] : string.Empty);
                        obj.user_level = Convert.ToInt32(dr["user_level"]);
                        obj.transfer_branch = Convert.ToString(dr["transfer_branch"] != DBNull.Value ? dr["transfer_branch"] : string.Empty);
                        obj.remark = Convert.ToString(dr["remark"] != DBNull.Value ? dr["remark"] : string.Empty);
                        obj.diff = Convert.ToInt32(dr["Diff"]);
                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<UserTransferScheduleModel> NdSearchUserTransfer(UserTransferScheduleModel obj)
        {
            List<UserTransferScheduleModel> list = new List<UserTransferScheduleModel>();

            try
            {
                DataTable dt = AdministratorDAL.NdSearchUserTransfer(obj);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        UserTransferScheduleModel um = new UserTransferScheduleModel();
                        um.Transfer_Id = Convert.ToInt16(dr["transfer_id"]);
                        um.emp_code = Convert.ToString(dr["emp_code"] != DBNull.Value ? dr["emp_code"] : string.Empty);
                        um.fname = Convert.ToString(dr["fname"] != DBNull.Value ? dr["fname"] : string.Empty);
                        um.lname = Convert.ToString(dr["lname"] != DBNull.Value ? dr["lname"] : string.Empty);
                        um.is_active = Convert.ToBoolean(dr["is_active"]);
                        um.sch_time = Convert.ToDateTime(dr["sch_time"]);
                        um.start_date = Convert.ToDateTime(dr["start_date"]);
                        um.end_date = Convert.ToDateTime(dr["end_date"]);
                        um.branch_code = Convert.ToString(dr["branch_code"] != DBNull.Value ? dr["branch_code"] : string.Empty);
                        um.branch_name = Convert.ToString(dr["branch_name"] != DBNull.Value ? dr["branch_name"] : string.Empty);
                        um.hub_code = Convert.ToString(dr["hub_code"] != DBNull.Value ? dr["hub_code"] : string.Empty);
                        um.hub_name = Convert.ToString(dr["hub_name"] != DBNull.Value ? dr["hub_name"] : string.Empty);
                        um.dept_code = Convert.ToString(dr["dept_code"] != DBNull.Value ? dr["dept_code"] : string.Empty);
                        um.dept_name = Convert.ToString(dr["dept_name"] != DBNull.Value ? dr["dept_name"] : string.Empty);
                        um.user_level = Convert.ToInt32(dr["user_level"]);
                        um.transfer_branch = Convert.ToString(dr["transfer_branch"] != DBNull.Value ? dr["transfer_branch"] : string.Empty);
                        um.remark = Convert.ToString(dr["remark"] != DBNull.Value ? dr["remark"] : string.Empty);
                        um.role_name1 = Convert.ToString(dr["role_name1"] != DBNull.Value ? dr["role_name1"] : string.Empty);
                        um.role_name2 = Convert.ToString(dr["role_name2"] != DBNull.Value ? dr["role_name2"] : string.Empty);
                        list.Add(um);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static UserTransferScheduleModel NdGetUserTransfer_param(string empcode, long id)
        {
            UserTransferScheduleModel obj = new UserTransferScheduleModel();

            try
            {
                DataTable dt = AdministratorDAL.NdGetUserTransfer_param(empcode, id);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        obj.emp_code = Convert.ToString(dr["emp_code"] != DBNull.Value ? dr["emp_code"] : string.Empty);
                        obj.fname = Convert.ToString(dr["fname"] != DBNull.Value ? dr["fname"] : string.Empty);
                        obj.lname = Convert.ToString(dr["lname"] != DBNull.Value ? dr["lname"] : string.Empty);
                        obj.is_active = Convert.ToBoolean(dr["is_active"]);
                        obj.sch_time = Convert.ToDateTime(dr["sch_time"]);
                        obj.start_date = Convert.ToDateTime(dr["start_date"]);
                        obj.end_date = Convert.ToDateTime(dr["end_date"]);
                        obj.branch_code = Convert.ToString(dr["branch_code"] != DBNull.Value ? dr["branch_code"] : string.Empty);
                        obj.branch_name = Convert.ToString(dr["branch_name"] != DBNull.Value ? dr["branch_name"] : string.Empty);
                        obj.hub_code = Convert.ToString(dr["hub_code"] != DBNull.Value ? dr["hub_code"] : string.Empty);
                        obj.hub_name = Convert.ToString(dr["hub_name"] != DBNull.Value ? dr["hub_name"] : string.Empty);
                        obj.dept_code = Convert.ToString(dr["dept_code"] != DBNull.Value ? dr["dept_code"] : string.Empty);
                        obj.dept_name = Convert.ToString(dr["dept_name"] != DBNull.Value ? dr["dept_name"] : string.Empty);
                        obj.user_level = Convert.ToInt32(dr["user_level"]);
                        obj.branch_code2 = Convert.ToString(dr["branch_code2"] != DBNull.Value ? dr["branch_code2"] : string.Empty);
                        obj.branch_name2 = Convert.ToString(dr["branch_name2"] != DBNull.Value ? dr["branch_name2"] : string.Empty);
                        obj.hub_code2 = Convert.ToString(dr["hub_code2"] != DBNull.Value ? dr["hub_code2"] : string.Empty);
                        obj.hub_name2 = Convert.ToString(dr["hub_name2"] != DBNull.Value ? dr["hub_name2"] : string.Empty);
                        obj.dept_code2 = Convert.ToString(dr["dept_code2"] != DBNull.Value ? dr["dept_code2"] : string.Empty);
                        obj.dept_name2 = Convert.ToString(dr["dept_name2"] != DBNull.Value ? dr["dept_name2"] : string.Empty);
                        obj.user_level2 = Convert.ToInt32(dr["user_level2"]);
                        obj.transfer_branch = Convert.ToString(dr["transfer_branch"] != DBNull.Value ? dr["transfer_branch"] : string.Empty);
                        obj.remark = Convert.ToString(dr["remark"] != DBNull.Value ? dr["remark"] : string.Empty);
                        obj.role_name1 = Convert.ToString(dr["role_name1"] != DBNull.Value ? dr["role_name1"] : string.Empty);
                        obj.role_name2 = Convert.ToString(dr["role_name2"] != DBNull.Value ? dr["role_name2"] : string.Empty);
                        obj.type = Convert.ToString(dr["type"] != DBNull.Value ? dr["type"] : string.Empty);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }

        //
        public static UserTransferScheduleModel NdCheckTblTransfer(string _c, string _n, string _o)
        {
            UserTransferScheduleModel obj = new UserTransferScheduleModel();
            try
            {
                DataTable dt = AdministratorDAL.NdCheckTblTransfer(_c, _n, _o);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        obj.Transfer_Id = Convert.ToInt64(dr["transfer_id"]);
                        obj.emp_code = Convert.ToString(dr["emp_code"] != DBNull.Value ? dr["emp_code"] : string.Empty);
                        obj.fname = Convert.ToString(dr["fname"] != DBNull.Value ? dr["fname"] : string.Empty);
                        obj.lname = Convert.ToString(dr["lname"] != DBNull.Value ? dr["lname"] : string.Empty);
                        obj.is_active = Convert.ToBoolean(dr["is_active"]);
                        obj.sch_time = Convert.ToDateTime(dr["sch_time"]);
                        obj.start_date = Convert.ToDateTime(dr["start_date"]);
                        obj.end_date = Convert.ToDateTime(dr["end_date"]);
                        obj.branch_code = Convert.ToString(dr["branch_code"] != DBNull.Value ? dr["branch_code"] : string.Empty);
                        obj.branch_name = Convert.ToString(dr["branch_name"] != DBNull.Value ? dr["branch_name"] : string.Empty);
                        obj.hub_code = Convert.ToString(dr["hub_code"] != DBNull.Value ? dr["hub_code"] : string.Empty);
                        obj.hub_name = Convert.ToString(dr["hub_name"] != DBNull.Value ? dr["hub_name"] : string.Empty);
                        obj.dept_code = Convert.ToString(dr["dept_code"] != DBNull.Value ? dr["dept_code"] : string.Empty);
                        obj.dept_name = Convert.ToString(dr["dept_name"] != DBNull.Value ? dr["dept_name"] : string.Empty);
                        obj.user_level = Convert.ToInt32(dr["user_level"]);
                        obj.branch_code2 = Convert.ToString(dr["branch_code2"] != DBNull.Value ? dr["branch_code2"] : string.Empty);
                        obj.branch_name2 = Convert.ToString(dr["branch_name2"] != DBNull.Value ? dr["branch_name2"] : string.Empty);
                        obj.hub_code2 = Convert.ToString(dr["hub_code2"] != DBNull.Value ? dr["hub_code2"] : string.Empty);
                        obj.hub_name2 = Convert.ToString(dr["hub_name2"] != DBNull.Value ? dr["hub_name2"] : string.Empty);
                        obj.dept_code2 = Convert.ToString(dr["dept_code2"] != DBNull.Value ? dr["dept_code2"] : string.Empty);
                        obj.dept_name2 = Convert.ToString(dr["dept_name2"] != DBNull.Value ? dr["dept_name2"] : string.Empty);
                        obj.user_level2 = Convert.ToInt32(dr["user_level2"]);
                        obj.transfer_branch = Convert.ToString(dr["transfer_branch"] != DBNull.Value ? dr["transfer_branch"] : string.Empty);
                        obj.remark = Convert.ToString(dr["remark"] != DBNull.Value ? dr["remark"] : string.Empty);
                        obj.role_name1 = Convert.ToString(dr["role_name1"] != DBNull.Value ? dr["role_name1"] : string.Empty);
                        obj.role_name2 = Convert.ToString(dr["role_name2"] != DBNull.Value ? dr["role_name2"] : string.Empty);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }
            return obj;
        }

        public static UserTransferScheduleModel NdGetUserSysTransfer(string _c, string _n, string _o)
        {
            UserTransferScheduleModel obj = new UserTransferScheduleModel();
            try
            {
                DataTable dt = AdministratorDAL.NdGetUserSysTransfer(_c, _n, _o);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        //obj.Transfer_Id = Convert.ToInt64(dr["transfer_id"]);
                        obj.emp_code = Convert.ToString(dr["emp_code"] != DBNull.Value ? dr["emp_code"] : string.Empty);
                        obj.fname = Convert.ToString(dr["fname"] != DBNull.Value ? dr["fname"] : string.Empty);
                        obj.lname = Convert.ToString(dr["lname"] != DBNull.Value ? dr["lname"] : string.Empty);
                        //obj.is_active = Convert.ToBoolean(dr["is_active"]);
                        //obj.sch_time = Convert.ToDateTime(dr["sch_time"]);
                        //obj.start_date = Convert.ToDateTime(dr["start_date"]);
                        //obj.end_date = Convert.ToDateTime(dr["end_date"]);
                        obj.branch_code = Convert.ToString(dr["sol_code"] != DBNull.Value ? dr["sol_code"] : string.Empty);
                        obj.branch_name = Convert.ToString(dr["sol_name"] != DBNull.Value ? dr["sol_name"] : string.Empty);
                        obj.hub_code = Convert.ToString(dr["zone_code"] != DBNull.Value ? dr["zone_code"] : string.Empty);
                        obj.hub_name = Convert.ToString(dr["zone_name"] != DBNull.Value ? dr["zone_name"] : string.Empty);
                        obj.dept_code = Convert.ToString(dr["dept_code"] != DBNull.Value ? dr["dept_code"] : string.Empty);
                        obj.dept_name = Convert.ToString(dr["dept_name"] != DBNull.Value ? dr["dept_name"] : string.Empty);
                        //obj.user_level = Convert.ToInt32(dr["user_level"]);
                        //obj.branch_code2 = Convert.ToString(dr["branch_code2"] != DBNull.Value ? dr["branch_code2"] : string.Empty);
                        //obj.branch_name2 = Convert.ToString(dr["branch_name2"] != DBNull.Value ? dr["branch_name2"] : string.Empty);
                        //obj.hub_code2 = Convert.ToString(dr["hub_code2"] != DBNull.Value ? dr["hub_code2"] : string.Empty);
                        //obj.hub_name2 = Convert.ToString(dr["hub_name2"] != DBNull.Value ? dr["hub_name2"] : string.Empty);
                        //obj.dept_code2 = Convert.ToString(dr["dept_code2"] != DBNull.Value ? dr["dept_code2"] : string.Empty);
                        //obj.dept_name2 = Convert.ToString(dr["dept_name2"] != DBNull.Value ? dr["dept_name2"] : string.Empty);
                        //obj.user_level2 = Convert.ToInt32(dr["user_level2"]);
                        //obj.transfer_branch = Convert.ToString(dr["transfer_branch"] != DBNull.Value ? dr["transfer_branch"] : string.Empty);
                        //obj.remark = Convert.ToString(dr["remark"] != DBNull.Value ? dr["remark"] : string.Empty);
                        //obj.role_name1 = Convert.ToString(dr["role_name1"] != DBNull.Value ? dr["role_name1"] : string.Empty);
                        //obj.role_name2 = Convert.ToString(dr["role_name2"] != DBNull.Value ? dr["role_name2"] : string.Empty);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }
            return obj;
        }

        public static List<UserModels> GetUserProfile()
        {
            Loger _logSys = new Loger();
            string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
            List<UserModels> list = new List<UserModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetUserProfile();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        UserModels obj = new UserModels();
                        obj.USER_FIRSTNAME = Convert.ToString(dr["USER_FIRSTNAME"] != DBNull.Value ? dr["USER_FIRSTNAME"] : string.Empty);
                        obj.USER_LASTNAME = Convert.ToString(dr["USER_LASTNAME"] != DBNull.Value ? dr["USER_LASTNAME"] : string.Empty);
                        obj.User_Role_Level = Convert.ToString(dr["Role_Level"] != DBNull.Value ? dr["Role_Level"] : string.Empty);
                        obj.User_Branch = Convert.ToString(dr["Branch"] != DBNull.Value ? dr["Branch"] : string.Empty);
                        obj.USER_ID = Convert.ToInt64(dr["USER_ID"]);
                        obj.USER_LOGIN = Convert.ToString(dr["USER_LOGIN"] != DBNull.Value ? dr["USER_LOGIN"] : string.Empty);
                        obj.IS_ACTIVE = Convert.ToBoolean(dr["IS_ACTIVE"]);
                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {                        
                        _logSys.WriteProcessLogFile(_strPathFile, "GetUserProfile : line 591 " + exc.Message.ToString());
                        throw exc;
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static UserModels GetUserByID(long ID)
        {
            UserModels obj = new UserModels();

            try
            {
                DataTable dt = AdministratorDAL.GetUserByID(ID);

                if (dt != null && dt.Rows.Count > 0)
                {

                    obj.USER_ID = Convert.ToInt64(dt.Rows[0]["USER_ID"]);
                    obj.USER_LOGIN = Convert.ToString(dt.Rows[0]["USER_LOGIN"] != DBNull.Value ? dt.Rows[0]["USER_LOGIN"] : string.Empty);
                    obj.USER_PASSWORD = Convert.ToString(dt.Rows[0]["USER_PASSWORD"] != DBNull.Value ? dt.Rows[0]["USER_PASSWORD"] : string.Empty);
                    obj.USER_PASSWORD_UPDATE_TIME = Convert.ToDateTime(dt.Rows[0]["USER_PASSWORD_UPDATE_TIME"]);
                    obj.USER_FIRSTNAME = Convert.ToString(dt.Rows[0]["USER_FIRSTNAME"] != DBNull.Value ? dt.Rows[0]["USER_FIRSTNAME"] : string.Empty);
                    obj.USER_LASTNAME = Convert.ToString(dt.Rows[0]["USER_LASTNAME"] != DBNull.Value ? dt.Rows[0]["USER_LASTNAME"] : string.Empty);
                    obj.USER_DEPARTMENT = Convert.ToString(dt.Rows[0]["USER_DEPARTMENT"] != DBNull.Value ? dt.Rows[0]["USER_DEPARTMENT"] : string.Empty);
                    obj.USER_POSITION = Convert.ToString(dt.Rows[0]["USER_POSITION"] != DBNull.Value ? dt.Rows[0]["USER_POSITION"] : string.Empty);
                    obj.USER_LEVEL = Convert.ToInt32(dt.Rows[0]["USER_LEVEL"] != DBNull.Value ? dt.Rows[0]["USER_LEVEL"] : 0);
                    obj.USER_EMP_CODE = Convert.ToString(dt.Rows[0]["USER_EMP_CODE"] != DBNull.Value ? dt.Rows[0]["USER_EMP_CODE"] : string.Empty);
                    //@USER_LOGIN_FLAG as bit,
                    obj.USER_LOGIN_FLAG = Convert.ToBoolean(dt.Rows[0]["USER_LOGIN_FLAG"] != DBNull.Value ? dt.Rows[0]["USER_LOGIN_FLAG"] : 0);
                    //@USER_FONT_NAME as varchar(50),
                    obj.USER_FONT_NAME = Convert.ToString(dt.Rows[0]["USER_FONT_NAME"] != DBNull.Value ? dt.Rows[0]["USER_FONT_NAME"] : string.Empty);
                    //@USER_FONT_SIZE as smallint,
                    obj.USER_FONT_SIZE = Convert.ToInt16(dt.Rows[0]["USER_FONT_SIZE"] != DBNull.Value ? dt.Rows[0]["USER_FONT_SIZE"] : 0);
                    //@USER_FONT_BOLD as bit,
                    obj.USER_FONT_BOLD = Convert.ToBoolean(dt.Rows[0]["USER_FONT_BOLD"] != DBNull.Value ? dt.Rows[0]["USER_FONT_BOLD"] : 0);
                    //@USER_FONT_ITALIC as bit,
                    obj.USER_FONT_ITALIC = Convert.ToBoolean(dt.Rows[0]["USER_FONT_ITALIC"] != DBNull.Value ? dt.Rows[0]["USER_FONT_ITALIC"] : 0);
                    //@USER_RUN_OUTSIDEVIEW as bit,
                    obj.USER_RUN_OUTSIDEVIEW = Convert.ToBoolean(dt.Rows[0]["USER_RUN_OUTSIDEVIEW"] != DBNull.Value ? dt.Rows[0]["USER_RUN_OUTSIDEVIEW"] : 0);
                    //@USER_DEFAULT_PAGE as varchar(10),
                    obj.USER_DEFAULT_PAGE = Convert.ToString(dt.Rows[0]["USER_DEFAULT_PAGE"] != DBNull.Value ? dt.Rows[0]["USER_DEFAULT_PAGE"] : string.Empty);
                    //@USER_CONDITION as nvarchar(500),
                    obj.USER_CONDITION = Convert.ToString(dt.Rows[0]["USER_CONDITION"] != DBNull.Value ? dt.Rows[0]["USER_CONDITION"] : string.Empty);
                    //@ADMIN_LAST_MA nvarchar(20),
                    obj.ADMIN_LAST_MA = Convert.ToString(dt.Rows[0]["ADMIN_LAST_MA"] != DBNull.Value ? dt.Rows[0]["ADMIN_LAST_MA"] : string.Empty);
                    //@ADMIN_BY as nvarchar(20),
                    obj.ADMIN_BY = Convert.ToString(dt.Rows[0]["ADMIN_BY"] != DBNull.Value ? dt.Rows[0]["ADMIN_BY"] : string.Empty);
                    //@ADMIN_CREATE_BY nvarchar(20),
                    obj.ADMIN_CREATE_BY = Convert.ToString(dt.Rows[0]["ADMIN_CREATE_BY"] != DBNull.Value ? dt.Rows[0]["ADMIN_CREATE_BY"] : string.Empty);
                    //@USER_WRONG_PWD as int,
                    obj.USER_WRONG_PWD = Convert.ToInt32(dt.Rows[0]["USER_WRONG_PWD"] != DBNull.Value ? dt.Rows[0]["USER_WRONG_PWD"] : 0);
                    //@USER_EMAIL as nvarchar(50)
                    obj.USER_EMAIL = Convert.ToString(dt.Rows[0]["USER_EMAIL"] != DBNull.Value ? dt.Rows[0]["USER_EMAIL"] : string.Empty);

                    //@USER_FLAG as bit,
                    obj.USER_FLAG = Convert.ToBoolean(dt.Rows[0]["USER_FLAG"] != DBNull.Value ? dt.Rows[0]["USER_FLAG"] : 0);
                    obj.USER_LOCK = Convert.ToBoolean(dt.Rows[0]["USER_LOCK"] != DBNull.Value ? dt.Rows[0]["USER_LOCK"] : 0);
                    obj.IS_ACTIVE = Convert.ToBoolean(dt.Rows[0]["IS_ACTIVE"] != DBNull.Value ? dt.Rows[0]["IS_ACTIVE"] : 0);

                    obj.SOL_NAME = Convert.ToString(dt.Rows[0]["SOL_NAME"] != DBNull.Value ? dt.Rows[0]["SOL_NAME"] : string.Empty);
                    obj.Hub_Code = Convert.ToString(dt.Rows[0]["Zone_Code"] != DBNull.Value ? dt.Rows[0]["Zone_Code"] : string.Empty);
                    obj.Hub_Name = Convert.ToString(dt.Rows[0]["Zone_Name"] != DBNull.Value ? dt.Rows[0]["Zone_Name"] : string.Empty);
                    obj.Dept_Code = Convert.ToString(dt.Rows[0]["Dept_Code"] != DBNull.Value ? dt.Rows[0]["Dept_Code"] : string.Empty);
                    obj.Dept_Name = Convert.ToString(dt.Rows[0]["Dept_Name"] != DBNull.Value ? dt.Rows[0]["Dept_Name"] : string.Empty);
                    obj.Remark = Convert.ToString(dt.Rows[0]["Remark"] != DBNull.Value ? dt.Rows[0]["Remark"] : string.Empty);
                    obj.Role_Name = Convert.ToString(dt.Rows[0]["Role_Name"] != DBNull.Value ? dt.Rows[0]["Role_Name"] : string.Empty);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }

        public static List<UserModels> GetUserByParam(List<UserModels> model, UserModels param)
        {
            List<UserModels> list = new List<UserModels>();

            try
            {
                //DataTable dt = AdministratorDAL.GetUser();
                DataTable dt = AdministratorDAL.GetUserByParam(model, param);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        UserModels obj = new UserModels();
                        ApiPostResponse2 obj2 = new ApiPostResponse2();
                        obj.USER_ID = Convert.ToInt64(dr["USER_ID"]);
                        obj.USER_LOGIN = Convert.ToString(dr["USER_LOGIN"] != DBNull.Value ? dr["USER_LOGIN"] : string.Empty);
                        obj.USER_PASSWORD = Convert.ToString(dr["USER_PASSWORD"] != DBNull.Value ? dr["USER_PASSWORD"] : string.Empty);
                        obj.USER_PASSWORD_UPDATE_TIME = Convert.ToDateTime(dr["USER_PASSWORD_UPDATE_TIME"]);
                        obj.USER_FIRSTNAME = Convert.ToString(dr["USER_FIRSTNAME"] != DBNull.Value ? dr["USER_FIRSTNAME"] : string.Empty);
                        obj.USER_LASTNAME = Convert.ToString(dr["USER_LASTNAME"] != DBNull.Value ? dr["USER_LASTNAME"] : string.Empty);
                        obj.USER_DEPARTMENT = Convert.ToString(dr["USER_DEPARTMENT"] != DBNull.Value ? dr["USER_DEPARTMENT"] : string.Empty);
                        obj.USER_POSITION = Convert.ToString(dr["USER_POSITION"] != DBNull.Value ? dr["USER_POSITION"] : string.Empty);
                        obj.USER_LEVEL = Convert.ToInt32(dr["USER_LEVEL"] != DBNull.Value ? dr["USER_LEVEL"] : 0);
                        obj.USER_EMAIL = Convert.ToString(dr["USER_EMAIL"] != DBNull.Value ? dr["USER_EMAIL"] : string.Empty);
                        obj.USER_LOCK = Convert.ToBoolean(dr["USER_LOCK"] != DBNull.Value ? dr["USER_LOCK"] : 0);
                        obj.IS_ACTIVE = Convert.ToBoolean(dr["IS_ACTIVE"] != DBNull.Value ? dr["IS_ACTIVE"] : 0);
                        obj.User_Branch = Convert.ToString(dr["SOL_CODE"] != DBNull.Value ? dr["SOL_CODE"] : string.Empty);
                        obj.User_Role_Level = Convert.ToString(dr["ROLE_NAME"] != DBNull.Value ? dr["ROLE_NAME"] : "รอดำเนินการ");
                        obj.USER_EMP_CODE = Convert.ToString(dr["USER_EMP_CODE"] != DBNull.Value ? dr["USER_EMP_CODE"] : "");
                        obj.SOL_NAME = Convert.ToString(dr["SOL_NAME"] != DBNull.Value ? dr["SOL_NAME"] : string.Empty);
                        obj.Dept_Code = Convert.ToString(dr["Dept_Code"] != DBNull.Value ? dr["Dept_Code"] : string.Empty);
                        obj.Dept_Name = Convert.ToString(dr["Dept_Name"] != DBNull.Value ? dr["Dept_Name"] : string.Empty);
                        obj.Hub_Code = Convert.ToString(dr["Zone_Code"] != DBNull.Value ? dr["Zone_Code"] : string.Empty);
                        obj.Hub_Name = Convert.ToString(dr["Zone_Name"] != DBNull.Value ? dr["Zone_Name"] : string.Empty);
                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<UserModels> GetUserByParam1(List<UserModels> model, UserModels searchDetails)
        {
            List<UserModels> list = new List<UserModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetUserByParam(model, searchDetails);
                if (dt != null && dt.Rows.Count > 0)
                {
                    UserModels obj = new UserModels();
                    obj.USER_ID = Convert.ToInt64(dt.Rows[0]["USER_ID"]);
                    obj.USER_LOGIN = Convert.ToString(dt.Rows[0]["USER_LOGIN"] != DBNull.Value ? dt.Rows[0]["USER_LOGIN"] : string.Empty);
                    obj.USER_PASSWORD = Convert.ToString(dt.Rows[0]["USER_PASSWORD"] != DBNull.Value ? dt.Rows[0]["USER_PASSWORD"] : string.Empty);
                    obj.USER_PASSWORD_UPDATE_TIME = Convert.ToDateTime(dt.Rows[0]["USER_PASSWORD_UPDATE_TIME"]);
                    obj.USER_FIRSTNAME = Convert.ToString(dt.Rows[0]["USER_FIRSTNAME"] != DBNull.Value ? dt.Rows[0]["USER_FIRSTNAME"] : string.Empty);
                    obj.USER_LASTNAME = Convert.ToString(dt.Rows[0]["USER_LASTNAME"] != DBNull.Value ? dt.Rows[0]["USER_LASTNAME"] : string.Empty);
                    obj.USER_DEPARTMENT = Convert.ToString(dt.Rows[0]["USER_DEPARTMENT"] != DBNull.Value ? dt.Rows[0]["USER_DEPARTMENT"] : string.Empty);
                    obj.USER_POSITION = Convert.ToString(dt.Rows[0]["USER_POSITION"] != DBNull.Value ? dt.Rows[0]["USER_POSITION"] : string.Empty);
                    obj.USER_LEVEL = Convert.ToInt32(dt.Rows[0]["USER_LEVEL"] != DBNull.Value ? dt.Rows[0]["USER_LEVEL"] : 0);

                    //@USER_LOGIN_FLAG as bit,
                    obj.USER_LOGIN_FLAG = Convert.ToBoolean(dt.Rows[0]["USER_LOGIN_FLAG"] != DBNull.Value ? dt.Rows[0]["USER_LOGIN_FLAG"] : 0);
                    //@USER_FONT_NAME as varchar(50),
                    obj.USER_FONT_NAME = Convert.ToString(dt.Rows[0]["USER_FONT_NAME"] != DBNull.Value ? dt.Rows[0]["USER_FONT_NAME"] : string.Empty);
                    //@USER_FONT_SIZE as smallint,
                    obj.USER_FONT_SIZE = Convert.ToInt16(dt.Rows[0]["USER_FONT_SIZE"] != DBNull.Value ? dt.Rows[0]["USER_FONT_SIZE"] : 0);
                    //@USER_FONT_BOLD as bit,
                    obj.USER_FONT_BOLD = Convert.ToBoolean(dt.Rows[0]["USER_FONT_BOLD"] != DBNull.Value ? dt.Rows[0]["USER_FONT_BOLD"] : 0);
                    //@USER_FONT_ITALIC as bit,
                    obj.USER_FONT_ITALIC = Convert.ToBoolean(dt.Rows[0]["USER_FONT_ITALIC"] != DBNull.Value ? dt.Rows[0]["USER_FONT_ITALIC"] : 0);
                    //@USER_RUN_OUTSIDEVIEW as bit,
                    obj.USER_RUN_OUTSIDEVIEW = Convert.ToBoolean(dt.Rows[0]["USER_RUN_OUTSIDEVIEW"] != DBNull.Value ? dt.Rows[0]["USER_RUN_OUTSIDEVIEW"] : 0);
                    //@USER_DEFAULT_PAGE as varchar(10),
                    obj.USER_DEFAULT_PAGE = Convert.ToString(dt.Rows[0]["USER_DEFAULT_PAGE"] != DBNull.Value ? dt.Rows[0]["USER_DEFAULT_PAGE"] : string.Empty);
                    //@USER_CONDITION as nvarchar(500),
                    obj.USER_CONDITION = Convert.ToString(dt.Rows[0]["USER_CONDITION"] != DBNull.Value ? dt.Rows[0]["USER_CONDITION"] : string.Empty);
                    //@ADMIN_LAST_MA nvarchar(20),
                    obj.ADMIN_LAST_MA = Convert.ToString(dt.Rows[0]["ADMIN_LAST_MA"] != DBNull.Value ? dt.Rows[0]["ADMIN_LAST_MA"] : string.Empty);
                    //@ADMIN_BY as nvarchar(20),
                    obj.ADMIN_BY = Convert.ToString(dt.Rows[0]["ADMIN_BY"] != DBNull.Value ? dt.Rows[0]["ADMIN_BY"] : string.Empty);
                    //@ADMIN_CREATE_BY nvarchar(20),
                    obj.ADMIN_CREATE_BY = Convert.ToString(dt.Rows[0]["ADMIN_CREATE_BY"] != DBNull.Value ? dt.Rows[0]["ADMIN_CREATE_BY"] : string.Empty);
                    //@USER_WRONG_PWD as int,
                    obj.USER_WRONG_PWD = Convert.ToInt32(dt.Rows[0]["USER_WRONG_PWD"] != DBNull.Value ? dt.Rows[0]["USER_WRONG_PWD"] : 0);
                    //@USER_EMAIL as nvarchar(50)
                    obj.USER_EMAIL = Convert.ToString(dt.Rows[0]["USER_EMAIL"] != DBNull.Value ? dt.Rows[0]["USER_EMAIL"] : string.Empty);

                    //@USER_FLAG as bit,
                    obj.USER_FLAG = Convert.ToBoolean(dt.Rows[0]["USER_FLAG"] != DBNull.Value ? dt.Rows[0]["USER_FLAG"] : 0);
                    obj.USER_LOCK = Convert.ToBoolean(dt.Rows[0]["USER_LOCK"] != DBNull.Value ? dt.Rows[0]["USER_LOCK"] : 0);
                    obj.IS_ACTIVE = Convert.ToBoolean(dt.Rows[0]["IS_ACTIVE"] != DBNull.Value ? dt.Rows[0]["IS_ACTIVE"] : 0);

                    obj.SOL_NAME = Convert.ToString(dt.Rows[0]["SOL_NAME"] != DBNull.Value ? dt.Rows[0]["SOL_NAME"] : string.Empty);
                    obj.Hub_Code = Convert.ToString(dt.Rows[0]["Zone_Code"] != DBNull.Value ? dt.Rows[0]["Zone_Code"] : string.Empty);
                    obj.Hub_Name = Convert.ToString(dt.Rows[0]["Zone_Name"] != DBNull.Value ? dt.Rows[0]["Zone_Name"] : string.Empty);
                    obj.Dept_Code = Convert.ToString(dt.Rows[0]["Dept_Code"] != DBNull.Value ? dt.Rows[0]["Dept_Code"] : string.Empty);
                    obj.Dept_Name = Convert.ToString(dt.Rows[0]["Dept_Name"] != DBNull.Value ? dt.Rows[0]["Dept_Name"] : string.Empty);
                    list.Add(obj);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static int GetCountUserByUserName(string UserName)
        {
            int intCount = 0;
            try
            {
                intCount = AdministratorDAL.GetCountUserByUserName(UserName);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return intCount;
        }
        public static bool AddUserLDAP(UserModels obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddUserLDAP(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool AddUserLDAPV3(UserModels obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddUserLDAPV3(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool AddUserLDAPV4(UserModels obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddUserLDAPV4(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }


        public static bool Nd_AddUserLDAP(UserModels obj, ApiPostResponse2 ldapUser)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.Nd_AddUserLDAP(obj, ldapUser);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool InsertDataFromAPI(ApiPostResponse2 ldapUser, DataTable _dt, UserModels obj, string tableName)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.InsertDataFromAPI(ldapUser, _dt, obj, tableName);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool EditUser(UserModels obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditUser(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool EditUser2(UserModels obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditUser2(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool MainUser_UdateRole(UserModels obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.UPDATEROLE(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool DeleteUserByID(long ID)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteUserByID(ID);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static List<MAP_USER_BRANCHModel> GetMap_User_Branch()
        {
            List<MAP_USER_BRANCHModel> list = new List<MAP_USER_BRANCHModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMap_User_Branch();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        MAP_USER_BRANCHModel obj = new MAP_USER_BRANCHModel();
                        obj.ID = Convert.ToInt64(dr["ID"]);
                        obj.ID_USER = Convert.ToInt64(dr["ID_USER"]);
                        obj.USER_NAME = Convert.ToString(dr["UserName"]);
                        obj.ID_BRANCH = Convert.ToString(dr["ID_BRANCH"] != DBNull.Value ? dr["ID_BRANCH"] : string.Empty);//,< ID_BRANCH, nvarchar(5),>
                        obj.BRANCH_NAME = Convert.ToString(dr["BranchName"]);
                        obj.CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"] != DBNull.Value ? dr["CREATE_DATE"] : DateTime.MinValue);//,< CREATE_DATE, datetime,>                       
                        obj.UPDATE_DATE = Convert.ToDateTime(dr["UPDATE_DATE"] != DBNull.Value ? dr["UPDATE_DATE"] : DateTime.MinValue);//,< UPDATE_DATE, datetime,>

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static List<MAP_BRANCH_REPORTSModel> GetMap_Branch_Report()
        {
            List<MAP_BRANCH_REPORTSModel> list = new List<MAP_BRANCH_REPORTSModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMap_Branch_Report();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        MAP_BRANCH_REPORTSModel obj = new MAP_BRANCH_REPORTSModel();
                        obj.ID = Convert.ToInt64(dr["ID"]);
                        obj.GROUP_NO = Convert.ToDecimal(dr["GROUP_NO"] != DBNull.Value ? dr["GROUP_NO"] : 0);//,< GROUP_NO, numeric(18, 2),>
                        obj.GROUP_NAME = Convert.ToString(dr["GroupName"]);
                        obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"] != DBNull.Value ? dr["REPORT_NO"] : 0);//,< REPORT_NO, numeric(18, 2),>
                        obj.REPORT_NAME = Convert.ToString(dr["ReportName"]);
                        obj.ID_BRANCH = Convert.ToString(dr["ID_BRANCH"] != DBNull.Value ? dr["ID_BRANCH"] : string.Empty);//,< ID_BRANCH, nvarchar(5),>
                        obj.BRANCH_NAME = Convert.ToString(dr["BranchName"]);
                        obj.CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"] != DBNull.Value ? dr["CREATE_DATE"] : DateTime.MinValue);//,< CREATE_DATE, datetime,>                       
                        obj.UPDATE_DATE = Convert.ToDateTime(dr["UPDATE_DATE"] != DBNull.Value ? dr["UPDATE_DATE"] : DateTime.MinValue);//,< UPDATE_DATE, datetime,>

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<MAP_USER_REPORTModel> GetMap_User_Report()
        {
            List<MAP_USER_REPORTModel> list = new List<MAP_USER_REPORTModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMap_User_Report();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        MAP_USER_REPORTModel obj = new MAP_USER_REPORTModel();
                        obj.ID_MENU = Convert.ToInt64(dr["ID_MENU"]);
                        // < ID_USER, bigint,>
                        obj.ID_USER = Convert.ToInt64(dr["ID_USER"] != DBNull.Value ? dr["ID_USER"] : 0);
                        obj.UserName = Convert.ToString(dr["UserName"]);
                        //,< GROUP_NO, numeric(18, 2),>
                        obj.GROUP_NO = Convert.ToDecimal(dr["GROUP_NO"] != DBNull.Value ? dr["GROUP_NO"] : 0);
                        obj.GroupName = Convert.ToString(dr["GroupName"]);
                        //,< REPORT_NO, numeric(18, 2),>
                        obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"] != DBNull.Value ? dr["REPORT_NO"] : 0);
                        obj.ReportName = Convert.ToString(dr["ReportName"]);
                        //,< ID_BRANCH, nvarchar(5),>
                        obj.ID_BRANCH = Convert.ToString(dr["ID_BRANCH"] != DBNull.Value ? dr["ID_BRANCH"] : string.Empty);
                        obj.BranchName = Convert.ToString(dr["BranchName"]);
                        //,< CREATE_DATE, datetime,>
                        obj.CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"] != DBNull.Value ? dr["CREATE_DATE"] : DateTime.MinValue);
                        //,< UPDATE_DATE, datetime,>
                        obj.UPDATE_DATE = Convert.ToDateTime(dr["UPDATE_DATE"] != DBNull.Value ? dr["UPDATE_DATE"] : DateTime.MinValue);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static MAP_BRANCH_REPORTSModel GetMap_Branch_ReportByID(long ID)
        {
            MAP_BRANCH_REPORTSModel obj = new MAP_BRANCH_REPORTSModel();

            try
            {
                DataTable dt = AdministratorDAL.GetMap_Branch_ReportByID(ID);

                foreach (DataRow dr in dt.Rows)
                {

                    obj.ID = Convert.ToInt64(dr["ID"]);

                    //,< GROUP_NO, numeric(18, 2),>
                    obj.GROUP_NO = Convert.ToDecimal(dr["GROUP_NO"]);
                    //,< REPORT_NO, numeric(18, 2),>
                    obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"]);
                    //,< ID_BRANCH, nvarchar(5),>
                    obj.ID_BRANCH = Convert.ToString(dr["ID_BRANCH"]);
                    //,< CREATE_DATE, datetime,>
                    obj.CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]);
                    //,< UPDATE_DATE, datetime,>
                    obj.UPDATE_DATE = Convert.ToDateTime(dr["UPDATE_DATE"]);


                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }
        public static MAP_USER_BRANCHModel GetMap_User_BranchByID(long ID)
        {
            MAP_USER_BRANCHModel obj = new MAP_USER_BRANCHModel();

            try
            {
                DataTable dt = AdministratorDAL.GetMap_User_BranchByID(ID);

                foreach (DataRow dr in dt.Rows)
                {

                    obj.ID = Convert.ToInt64(dr["ID"]);
                    // < ID_USER, bigint,>
                    obj.ID_USER = Convert.ToInt64(dr["ID_USER"]);
                    //,< ID_BRANCH, nvarchar(5),>
                    obj.ID_BRANCH = Convert.ToString(dr["ID_BRANCH"]);
                    //,< CREATE_DATE, datetime,>
                    obj.CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]);
                    //,< UPDATE_DATE, datetime,>
                    obj.UPDATE_DATE = Convert.ToDateTime(dr["UPDATE_DATE"]);
                    //,< ID_BRANCH2, nvarchar(5),>
                    obj.ID_BRANCH2 = Convert.ToString(dr["ID_BRANCH2"]);
                    //,< B_DATE, datetime,>
                    obj.B_DATE = dr["B_DATE"].ToString() == string.Empty ? DateTime.Now.ToString() : dr["B_DATE"].ToString();
                    //,< E_DATE, datetime,>
                    obj.E_DATE = dr["E_DATE"].ToString() == string.Empty ? DateTime.Now.ToString() : dr["E_DATE"].ToString();
                    //,< PERMA, bit,>
                    obj.PERMA = Convert.ToBoolean(dr["PERMA"].ToString() == string.Empty ? false : dr["PERMA"]);
                    //,< OPEN_OPT, bit,>
                    obj.OPEN_OPT = Convert.ToBoolean(dr["OPEN_OPT"].ToString() == string.Empty ? false : dr["OPEN_OPT"]);

                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }
        public static MAP_USER_REPORTModel GetMap_User_ReportByID(long ID)
        {
            MAP_USER_REPORTModel obj = new MAP_USER_REPORTModel();

            try
            {
                DataTable dt = AdministratorDAL.GetMap_User_ReportByID(ID);

                foreach (DataRow dr in dt.Rows)
                {

                    obj.ID_MENU = Convert.ToInt64(dr["ID_MENU"]);
                    // < ID_USER, bigint,>
                    obj.ID_USER = Convert.ToInt64(dr["ID_USER"]);
                    //,< GROUP_NO, numeric(18, 2),>
                    obj.GROUP_NO = Convert.ToDecimal(dr["GROUP_NO"]);
                    //,< REPORT_NO, numeric(18, 2),>
                    obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"]);
                    //,< ID_BRANCH, nvarchar(5),>
                    obj.ID_BRANCH = Convert.ToString(dr["ID_BRANCH"]);
                    //,< CREATE_DATE, datetime,>
                    obj.CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]);
                    //,< UPDATE_DATE, datetime,>
                    obj.UPDATE_DATE = Convert.ToDateTime(dr["UPDATE_DATE"]);


                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }
        public static bool AddMap_User_Branch(MAP_USER_BRANCHModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddMap_User_Branch(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool AddMap_Branch_Report(MAP_BRANCH_REPORTSModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddMap_Branch_Report(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool AddMap_Branch_Report2(MAP_BRANCH_REPORTSModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddMap_Branch_Report2(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool AddMap_User_Report(MAP_USER_REPORTModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.AddMap_User_Report(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool EditMap_Branch_Report(MAP_BRANCH_REPORTSModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditMap_Branch_Report(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool EditMap_User_Branch(MAP_USER_BRANCHModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditMap_User_Branch(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool EditMap_User_Branch2(MAP_USER_BRANCHModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditMap_User_Branch2(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool EditMap_User_Report(MAP_USER_REPORTModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditMap_User_Report(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool DeleteMapUserReportByID(long ID)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteMapUserReportByID(ID);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool DeleteMapUserBranchByID(long ID)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteMapUserBranchByID(ID);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool DeleteMapBranchReportByID(long ID)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteMapBranchReportByID(ID);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static MAP_USER_REPORTModel GetBranchByUserID(string UserID)
        {
            MAP_USER_REPORTModel obj = new MAP_USER_REPORTModel();

            try
            {
                DataTable dt = AdministratorDAL.GetBranchByUserID(UserID);
                if (dt != null && dt.Rows.Count > 0)
                {
                    obj.ID_BRANCH = Convert.ToString(dt.Rows[0]["ID_BRANCH"]);

                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }


        public static SysBranch_RelationModel NdGetBrandRelation(string brandcode)
        {
            SysBranch_RelationModel obj = new SysBranch_RelationModel();
            try
            {
                DataTable dt = AdministratorDAL.NdGetBrandRelation(brandcode);

                foreach (DataRow dr in dt.Rows)
                {

                    obj.brand_code = dr["BRANCH_CODE"].ToString();
                    obj.department_code = dr["DEPARTMENT_CODE"].ToString();
                    obj.hub_code = dr["HUB_CODE"].ToString();
                    obj.brand_name = dr["BRANCH_NAME"].ToString();
                    obj.department_name = dr["DEPARTMENT_NAME"].ToString();
                    obj.hub_name = dr["HUB_NAME"].ToString();
                    obj.last_update = Convert.ToDateTime(dr["LAST_UPDATE"]);
                }
            }
            catch (Exception exc)
            {
                //throw exc;
                exc.Message.ToString();
            }

            return obj;
        }

        public static List<UserTransferScheduleModel> NdGetBrandRelation2(string emp_code)
        {
            List<UserTransferScheduleModel> list = new List<UserTransferScheduleModel>();
            try
            {
                DataTable dt = AdministratorDAL.NdGetBrandRelation2(emp_code);

                foreach (DataRow dr in dt.Rows)
                {

                    UserTransferScheduleModel obj = new UserTransferScheduleModel();
                    //UserTransferScheduleModel
                    obj.emp_code = Convert.ToString(dr["emp_code"] != DBNull.Value ? dr["emp_code"] : string.Empty);
                    obj.fname = Convert.ToString(dr["fname"] != DBNull.Value ? dr["fname"] : string.Empty);
                    obj.lname = Convert.ToString(dr["lname"] != DBNull.Value ? dr["lname"] : string.Empty);
                    obj.is_active = Convert.ToBoolean(dr["is_active"]);
                    obj.sch_time = Convert.ToDateTime(dr["sch_time"]);
                    obj.start_date = Convert.ToDateTime(dr["start_date"]);
                    obj.end_date = Convert.ToDateTime(dr["end_date"]);
                    obj.branch_code = Convert.ToString(dr["branch_code"] != DBNull.Value ? dr["branch_code"] : string.Empty);
                    obj.branch_name = Convert.ToString(dr["branch_name"] != DBNull.Value ? dr["branch_name"] : string.Empty);
                    obj.hub_code = Convert.ToString(dr["hub_code"] != DBNull.Value ? dr["hub_code"] : string.Empty);
                    obj.hub_name = Convert.ToString(dr["hub_name"] != DBNull.Value ? dr["hub_name"] : string.Empty);
                    obj.dept_code = Convert.ToString(dr["dept_code"] != DBNull.Value ? dr["dept_code"] : string.Empty);
                    obj.dept_name = Convert.ToString(dr["dept_name"] != DBNull.Value ? dr["dept_name"] : string.Empty);
                    obj.user_level = Convert.ToInt32(dr["user_level"]);
                    obj.transfer_branch = Convert.ToString(dr["transfer_branch"] != DBNull.Value ? dr["transfer_branch"] : string.Empty);
                    obj.remark = Convert.ToString(dr["remark"] != DBNull.Value ? dr["remark"] : string.Empty);
                    list.Add(obj);
                }
            }
            catch (Exception exc)
            {
                //throw exc;
                exc.Message.ToString();
            }

            return list;
        }

        public static bool NdInsertUserTransfer(UserTransferScheduleModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.NdInsertUserTransfer(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        //
        public static bool NdUpdateUserTransfer(UserTransferScheduleModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.NdUpdateUserTransfer(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        #region Master data
        public static List<RoleModels> GetMasterRole()
        {
            List<RoleModels> list = new List<RoleModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetMasterRole();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        RoleModels obj = new RoleModels();
                        obj.ROLE_LEVEL = Convert.ToInt64(dr["ROLE_LEVEL"]);
                        obj.ROLE_DEPARTMENT = Convert.ToString(dr["ROLE_DEPARTMENT"] != DBNull.Value ? dr["ROLE_DEPARTMENT"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static List<UserModels> GetMasterUser()
        {
            List<UserModels> list = new List<UserModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetMasterUser();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        UserModels obj = new UserModels();
                        obj.USER_ID = Convert.ToInt64(dr["USER_ID"]);
                        obj.USER_LOGIN = Convert.ToString(dr["USER_LOGIN"] != DBNull.Value ? dr["USER_LOGIN"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static List<GroupReportModel> GetMasterGroupReport()
        {
            List<GroupReportModel> list = new List<GroupReportModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMasterGroupReport();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        GroupReportModel obj = new GroupReportModel();
                        obj.GroupCode = Convert.ToDecimal(dr["GROUP_NO"]);
                        obj.GroupName = Convert.ToString(dr["GROUP_NAME"] != DBNull.Value ? dr["GROUP_NAME"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static List<ReportModel> GetMasterReport()
        {
            List<ReportModel> list = new List<ReportModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMasterGroupDetailReport();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        ReportModel obj = new ReportModel();
                        obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"]);
                        obj.REPORT_NAME = Convert.ToString(dr["REPORT_NAME"] != DBNull.Value ? dr["REPORT_NAME"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static List<ReportModel> GetMasterReportByGroupNo(decimal? GroupNo)
        {
            List<ReportModel> list = new List<ReportModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMasterGroupDetailReportByGroupNo(GroupNo);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        ReportModel obj = new ReportModel();
                        obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"]);
                        obj.REPORT_NAME = Convert.ToString(dr["REPORT_NAME"] != DBNull.Value ? dr["REPORT_NAME"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<ReportModel> GetMasterReportByGroupNo2(decimal? GroupNo, string branchId)
        {
            List<ReportModel> list = new List<ReportModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMasterGroupDetailReportByGroupNo2(GroupNo, branchId);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        ReportModel obj = new ReportModel();
                        obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"]);
                        obj.REPORT_NAME = Convert.ToString(dr["REPORT_NAME"] != DBNull.Value ? dr["REPORT_NAME"] : string.Empty);
                        obj.IsSelected = dr["Selected"].ToString() == "Y" ? true : false;

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }
        public static decimal GenerateReportNo(decimal? GroupNo)
        {
            decimal dcmReportNo = 0;
            try
            {
                DataTable dt = new DataTable();
                dt = AdministratorDAL.GenerateReportNo(GroupNo);
                if (dt.Rows[0]["GENERATE_REPORT_NO"] != DBNull.Value)
                {
                    dcmReportNo = Convert.ToDecimal(dt.Rows[0]["GENERATE_REPORT_NO"]);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dcmReportNo;
        }
        public static List<GroupDetailReportModel> GetMasterGroupDetailReport()
        {
            List<GroupDetailReportModel> list = new List<GroupDetailReportModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMasterGroupDetailReport();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        GroupDetailReportModel obj = new GroupDetailReportModel();
                        obj.REPORT_NO = Convert.ToDecimal(dr["REPORT_NO"]);
                        obj.REPORT_NAME = Convert.ToString(dr["REPORT_NAME"] != DBNull.Value ? dr["REPORT_NAME"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<BranchModel> GetSystemBranch()
        {
            List<BranchModel> list = new List<BranchModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetSystemBranch();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        BranchModel obj = new BranchModel();
                        obj.SOL_CODE = Convert.ToString(dr["SOL_CODE"] != DBNull.Value ? dr["SOL_CODE"] : string.Empty);
                        obj.CATEGORY = Convert.ToString(dr["BRANCH_CATEGORY_TITLE_TH"] != DBNull.Value ? dr["BRANCH_CATEGORY_TITLE_TH"] : string.Empty);
                        obj.CATEGORY_DESC = Convert.ToString(dr["DESC"] != DBNull.Value ? dr["DESC"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<HubModels> GetSystemHub()
        {
            List<HubModels> list = new List<HubModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetSystemHub();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        HubModels obj = new HubModels();
                        obj.HUB_CODE = Convert.ToString(dr["HUB_CODE"] != DBNull.Value ? dr["HUB_CODE"] : string.Empty);
                        obj.HUB_NAME = Convert.ToString(dr["HUB_NAME"] != DBNull.Value ? dr["HUB_NAME"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<DistrictModels> GetSystemDistrict()
        {
            List<DistrictModels> list = new List<DistrictModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetSystemDistrict();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        DistrictModels obj = new DistrictModels();
                        obj.DISTRICT_CODE = Convert.ToString(dr["DISTRICT_CODE"] != DBNull.Value ? dr["DISTRICT_CODE"] : string.Empty);
                        obj.DISTRICT_NAME = Convert.ToString(dr["DISTRICT_NAME"] != DBNull.Value ? dr["DISTRICT_NAME"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<AudiLogModel> LoadWorkFunction()
        {
            List<AudiLogModel> list = new List<AudiLogModel>();

            try
            {
                DataTable dt = AdministratorDAL.LoadWorkFunction();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        AudiLogModel obj = new AudiLogModel();
                        obj.WorkFunction = dr["Message"].ToString() != "" ? dr["Message"].ToString() : "";

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {                        
                        exc.Message.ToString();
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<DepartmentModels> GetSystemDepartment()
        {
            List<DepartmentModels> list = new List<DepartmentModels>();

            try
            {
                DataTable dt = AdministratorDAL.GetSystemDepartment();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        DepartmentModels obj = new DepartmentModels();
                        obj.DEPT_CODE = Convert.ToString(dr["DEPT_CODE"] != DBNull.Value ? dr["DEPT_CODE"] : string.Empty);
                        obj.DEPT_NAME = Convert.ToString(dr["DEPT_NAME"] != DBNull.Value ? dr["DEPT_NAME"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static List<BranchModel> GetMasterBranch()
        {
            List<BranchModel> list = new List<BranchModel>();

            try
            {
                DataTable dt = AdministratorDAL.GetMasterBranch();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        BranchModel obj = new BranchModel();
                        obj.BRANCH = Convert.ToString(dr["BRANCH"] != DBNull.Value ? dr["BRANCH"] : string.Empty);
                        obj.DESC = Convert.ToString(dr["DESC"] != DBNull.Value ? dr["DESC"] : string.Empty);

                        list.Add(obj);
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return list;
        }

        public static string GenBranchNo()
        {
            string dcmBranchNo = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                dt = AdministratorDAL.GenBranchNo();
                if (dt.Rows[0]["BranchNo"] != DBNull.Value)
                {
                    dcmBranchNo = dt.Rows[0]["BranchNo"].ToString();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dcmBranchNo;
        }

        public static DataTable getMS(string _c, string _n, string _o)
        {
            DataTable dt = new DataTable();
            try
            {
                _c = _c == "" ? "" : _c;
                _n = _n == "" ? "" : _n;
                _o = _o == "" ? "" : _o;
                dt = AdministratorDAL.getMS(_c, _n, _o);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        public static DataTable getBranchOndemand(string _c, string _n, string _o)
        {
            DataTable dt = new DataTable();
            try
            {
                _c = _c == "" ? "" : _c;
                _n = _n == "" ? "" : _n;
                _o = _o == "" ? "" : _o;
                dt = AdministratorDAL.getBranchOndemand(_c, _n, _o);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        #endregion

        #region Get Config

        public static int GetUSER_ATTRIB(short NCFGVARIABLEID, string SCFGVARIABLE)
        {
            DataTable dt = new DataTable();

            Loger _logSys = new Loger();
            string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");


            int result = 0;
            try
            {
                _logSys.WriteProcessLogFile(_strPathFile, "NCFGVARIABLEID : line 956 " + NCFGVARIABLEID.ToString());
                dt = AdministratorDAL.GetUSER_ATTRIB(NCFGVARIABLEID, SCFGVARIABLE);
                _logSys.WriteProcessLogFile(_strPathFile, "dt : line 958 " + dt.ToString());
                if (dt != null && dt.Rows.Count > 0)
                    result = Convert.ToInt32(dt.Rows[0]["SCFGVALUE"]);
                _logSys.WriteProcessLogFile(_strPathFile, "dt.Rows[0]['SCFGVALUE'] : line 961 " + dt.Rows[0]["SCFGVALUE"].ToString());
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return result;
        }
        public static UserAttribModel GetUserAttribByIDAndVariable(short NCFGVARIABLEID, string SCFGVARIABLE)
        {
            DataTable dt = new DataTable();
            UserAttribModel obj = new UserAttribModel();
            try
            {
                dt = AdministratorDAL.GetUSER_ATTRIB(NCFGVARIABLEID, SCFGVARIABLE);
                if (dt != null && dt.Rows.Count > 0)
                {
                    obj.NCFGVARIABLEID = Convert.ToInt16(dt.Rows[0]["NCFGVARIABLEID"]);
                    obj.SCFGVARIABLE = Convert.ToString(dt.Rows[0]["SCFGVARIABLE"]);
                    obj.SCFGVALUE = Convert.ToString(dt.Rows[0]["SCFGVALUE"]);
                    obj.SDESCRIPTION = Convert.ToString(dt.Rows[0]["SDESCRIPTION"]);
                    obj.Category = Convert.ToString(dt.Rows[0]["Category"]);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }
        //SearchUSER_ATTRIB(short NCFGVARIABLEID, string Category)
        public static UserAttribModel SearchUSER_ATTRIB(string SCFGVARIABLE, string Category)
        {
            DataTable dt = new DataTable();
            UserAttribModel obj = new UserAttribModel();
            try
            {
                dt = AdministratorDAL.SearchUSER_ATTRIB(SCFGVARIABLE, Category);
                if (dt != null && dt.Rows.Count > 0)
                {                    
                    obj.NCFGVARIABLEID = Convert.ToInt16(dt.Rows[0]["NCFGVARIABLEID"]);
                    obj.SCFGVARIABLE = Convert.ToString(dt.Rows[0]["SCFGVARIABLE"]);
                    obj.SCFGVALUE = Convert.ToString(dt.Rows[0]["SCFGVALUE"]);
                    obj.SDESCRIPTION = Convert.ToString(dt.Rows[0]["SDESCRIPTION"]);
                    obj.Category = Convert.ToString(dt.Rows[0]["Category"]);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return obj;
        }

        public static List<UserAttribModel> SearchUSER_ATTRIB2(string SCFGVARIABLE, string Category)
        {
            DataTable dt = new DataTable();
            List<UserAttribModel> lstobj = new List<UserAttribModel>();
            try
            {
                dt = AdministratorDAL.SearchUSER_ATTRIB(SCFGVARIABLE, Category);
                if (dt != null && dt.Rows.Count > 0)
                {
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        UserAttribModel obj = new UserAttribModel();
                        obj.NCFGVARIABLEID = Convert.ToInt16(dr["NCFGVARIABLEID"]);
                        obj.SCFGVARIABLE = Convert.ToString(dr["SCFGVARIABLE"]);
                        obj.SCFGVALUE = Convert.ToString(dr["SCFGVALUE"]);
                        obj.SDESCRIPTION = Convert.ToString(dr["SDESCRIPTION"]);
                        obj.Category = Convert.ToString(dr["Category"]);
                        lstobj.Add(obj);
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return lstobj;
        }
        //foreach (DataRow dr in dt.Rows)
                
        public static bool EditUserAttrib(UserAttribModel obj)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.EditUserAttrib(obj);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
        public static bool DeleteUserAttribByIDAndVariable(short shtID, string strVariable)
        {
            bool result = false;
            try
            {
                result = AdministratorDAL.DeleteUserAttribByIDAndVariable(shtID, strVariable);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool UpdateLoginLogout(string emp_code, string userid, int mode)
        {
            Loger _logSys = new Loger();
            string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
            bool result = false;
            try
            {
                result = AdministratorDAL.UpdateLoginLogout(emp_code,userid, mode);
            }
            catch (Exception exc)
            {
                //throw exc;
                _logSys.WriteProcessLogFile(_strPathFile, "UpdateLoginLogout : " + exc.Message.ToString());
            }
            return result;
        }

        #endregion
        #region Get Count

        //public static int GetCount(string strQuery)
        //{
        //    int result = 0;
        //    try
        //    {

        //        result = AdministratorDAL.GetCount(strQuery);

        //    }
        //    catch (Exception exc)
        //    {
        //        throw exc;
        //    }
        //    return result;
        //}

        public static int GetCount(string strName, string typefn)
        {
            int result = 0;
            try
            {

                result = AdministratorDAL.GetCount(strName, typefn);

            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        #endregion
    }
}