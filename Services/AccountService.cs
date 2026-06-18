using GHB_D1.Code.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using GHB_D1.Models;
using System.Web.Configuration;
using GHB_D1.Code.Util;
using System.Text;
using GHB_D1.Code.BAL;

namespace GHB_D1.Services
{
    public class AccountService
    {
        #region Private Variable
        private DBAccess _objDB = null;
        iniConnection _iniCon = null;
        private string _sql = string.Empty;
        //string _enpwd = WebConfigurationManager.AppSettings["cryptoKey"].ToString();
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        private ASCIIEncoding textConverter = new ASCIIEncoding();
        private byte[] iv = null;
        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        #endregion


        public AccountService()
        {
            _objDB = new DBAccess();
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            _iniCon.cryptoKey = ModConf.ReadIni(_iniCon.iniFile, "appSetting", "cryptoKey");
            _iniCon.ivk = ModConf.ReadIni(_iniCon.iniFile, "appSetting", "ivk");
            _iniCon._strConnection = ModConf.ReadIni(_iniCon.iniFile, "DB", "conStr1");
            iv = textConverter.GetBytes(_iniCon.ivk);//new byte[16];
            _logSys = new Loger();
        }

        public LoginViewModel CheckUserLDAP(string username)
        {
            DataTable _result = new DataTable();
            LoginViewModel loginVM = new LoginViewModel();
            loginVM.Username = username;
            List<RoleModels> listRoleDetails = new List<RoleModels>();
            //string ReportServer = WebConfigurationManager.ConnectionStrings["strCon"].ToString();

            List<GroupReportViewModel> groupReportVMList = new List<GroupReportViewModel>();
            try
            {
                /*ตรวจสอบ user*/
                //OLD Logic 20250331
                //_sql = "SELECT DISTINCT  t2.ID_BRANCH, t3.[DESC] AS BRANCH_NAME,t1.USER_ID, t1.USER_LOGIN,";
                //_sql += "t1.USER_FIRSTNAME + ' ' + t1.USER_LASTNAME as FullName,";
                //_sql += "t1.USER_DEPARTMENT,t1.USER_EMAIL,t1.USER_LEVEL, t1.USER_LAST_LOGON, t1.IS_ACTIVE ";
                //_sql += " FROM TBL_USERS t1";
                //_sql += " LEFT JOIN TBL_MAP_USER_BRANCH t2 ON t1.USER_ID = t2.ID_USER";
                //_sql += " LEFT JOIN TBL_MAP_BRANCH_REPORTS t4 ON t2.ID_BRANCH = t4.ID_BRANCH";
                //_sql += " LEFT JOIN Table_Branch t3 ON t2.ID_BRANCH = t3.BRANCH";
                //_sql += " WHERE t1.USER_LOGIN = @username";

                _sql = " SELECT DISTINCT ";
                _sql += " std.sol_code as ID_BRANCH, std.sol_name as BRANCH_NAME, t1.USER_EMP_CODE, ";
                _sql += " t1.USER_ID, t1.USER_LOGIN, t1.Role_Name, ";
                _sql += " t1.USER_FIRSTNAME + ' ' + t1.USER_LASTNAME as FullName, ";
                _sql += " t1.USER_DEPARTMENT,t1.USER_EMAIL,t1.USER_LEVEL, t1.USER_LAST_LOGON, t1.IS_ACTIVE ";
                _sql += " FROM TBL_USERS t1 ";
                _sql += " LEFT JOIN SYSTEMOFFICER_Details std on std.emp_code = t1.USER_EMP_CODE ";
                _sql += " LEFT JOIN SYSTEMBRANCH stb on stb.SOL_CODE = t1.SOL_CODE ";
                _sql += " WHERE t1.USER_LOGIN = @username ";

                using (SqlConnection connection = new SqlConnection(_iniCon._strConnection))
                {
                    // Open the connection
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(_sql, connection))
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "_sql, connection : line 63 " + _sql.ToString());

                        // Add parameters to the command
                        command.Parameters.AddWithValue("@username", username);

                        // Execute the command (safe from SQL injection)
                        SqlDataReader reader = command.ExecuteReader();
                        _logSys.WriteProcessLogFile(_strPathFile, "command.ExecuteReader() : line 69 " + reader.ToString());
                        // Process the results

                        while (reader.Read())
                        {
                            loginVM.Login_State = true;
                            loginVM.BranchID = reader["ID_BRANCH"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "ID_BRANCH : line 75 " + reader["ID_BRANCH"].ToString());
                            loginVM.BranchName = reader["BRANCH_NAME"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "BRANCH_NAME : line 77 " + reader["BRANCH_NAME"].ToString());
                            loginVM.Username = reader["USER_LOGIN"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LOGIN : line 79 " + reader["USER_LOGIN"].ToString());
                            loginVM.UserID = reader["USER_ID"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_ID : line 81 " + reader["USER_ID"].ToString());
                            loginVM.FullName = reader["FullName"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "FullName : line 83 " + reader["FullName"].ToString());
                            loginVM.Level = Convert.ToInt32(reader["USER_LEVEL"]);
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LEVEL : line 885 " + reader["USER_LEVEL"].ToString());
                            loginVM.USER_LAST_LOGON = reader["USER_LAST_LOGON"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LAST_LOGON :  " + reader["USER_LAST_LOGON"].ToString());
                            _logSys.WriteProcessLogFile(_strPathFile, "IS_ACTIVE :  " + reader["IS_ACTIVE"].ToString());
                            loginVM.Is_Active = Convert.ToBoolean(reader["IS_ACTIVE"].ToString());
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_EMP_CODE :  " + reader["USER_EMP_CODE"].ToString());
                            loginVM.Emp_Code = reader["USER_EMP_CODE"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_EMP_CODE :  " + reader["Role_Name"].ToString());
                            loginVM.Role_Name = reader["Role_Name"].ToString();
                        }


                        // Close the reader
                        reader.Close();
                    }
                    if (loginVM.Role_Name != "")
                    {
                        using (SqlCommand command = new SqlCommand("AMS_SP_MAIN_GET_MS", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@code", "");
                            command.Parameters.AddWithValue("@name", loginVM.Role_Name);
                            command.Parameters.AddWithValue("@opt", "roleExport2");

                            SqlDataReader reader = command.ExecuteReader();
                            _logSys.WriteProcessLogFile(_strPathFile, "command.ExecuteReader() : line 69 " + reader.ToString());
                            // Process the results
                            while (reader.Read())
                            {
                                RoleModels obj = new RoleModels();

                                obj.ID_Role = Convert.ToInt64(reader["ID_Role"]);
                                obj.Position_Role = Convert.ToString(reader["Position_Role"] != DBNull.Value ? reader["Position_Role"] : string.Empty);
                                obj.Role_Name = Convert.ToString(reader["Role_Name"] != DBNull.Value ? reader["Role_Name"] : string.Empty);
                                obj.Module_No = Convert.ToString(reader["Module_No"] != DBNull.Value ? reader["Module_No"] : string.Empty);
                                obj.Module_Name = Convert.ToString(reader["Module_Name"] != DBNull.Value ? reader["Module_Name"] : string.Empty);
                                obj.Group_Module = Convert.ToDecimal(reader["Group_Module"]);
                                //obj.RoleReport_No = Convert.ToDecimal(dr["RoleReport_No"]);
                                obj.Group_No = Convert.ToString(reader["Group_No"] != DBNull.Value ? reader["Group_No"] : string.Empty);
                                obj.Group_Report = Convert.ToString(reader["Group_Report"] != DBNull.Value ? reader["Group_Report"] : string.Empty);
                                obj.RoleReport_Name = Convert.ToString(reader["RoleReport_Name"] != DBNull.Value ? reader["RoleReport_Name"] : string.Empty);
                                obj.List_Role = Convert.ToBoolean(reader["List_Role"] != DBNull.Value ? reader["List_Role"] : 0);
                                obj.Create_Role = Convert.ToBoolean(reader["Create_Role"] != DBNull.Value ? reader["Create_Role"] : 0);
                                obj.View_Role = Convert.ToBoolean(reader["View_Role"] != DBNull.Value ? reader["View_Role"] : 0);
                                obj.Update_Role = Convert.ToBoolean(reader["Update_Role"] != DBNull.Value ? reader["Update_Role"] : 0);
                                obj.Delete_Role = Convert.ToBoolean(reader["Delete_Role"] != DBNull.Value ? reader["Delete_Role"] : 0);
                                obj.Export_Role = Convert.ToBoolean(reader["Export_Role"] != DBNull.Value ? reader["Export_Role"] : 0);
                                obj.Is_Active = Convert.ToBoolean(reader["Is_Active"] != DBNull.Value ? reader["Is_Active"] : 0);
                                obj.Create_Date = Convert.ToDateTime(reader["Create_Date"]);
                                obj.Update_Date = Convert.ToDateTime(reader["Update_Date"]);
                                obj.Remark = Convert.ToString(reader["Remark"] != DBNull.Value ? reader["Remark"] : string.Empty);
                                listRoleDetails.Add(obj);
                            }
                            // Close the reader
                            reader.Close();
                        }
                        loginVM.roleList = new groupRole();
                        loginVM.roleList.role_name = loginVM.Role_Name;
                        loginVM.roleList.group_role = listRoleDetails;
                    }
                }



            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "CheckUserLDAP : " + ex.Message.ToString());
            }


            return loginVM;
        }

        public LoginViewModel CheckUserTransferLDAP(string emp_code)
        {
            DataTable _result = new DataTable();
            LoginViewModel loginVM = new LoginViewModel();
            loginVM.Login_State = false;

            List<GroupReportViewModel> groupReportVMList = new List<GroupReportViewModel>();
            try
            {
                /*ตรวจสอบ user*/

                _sql = " SELECT DISTINCT S.sol_code AS ID_BRANCH, ";
                _sql += " S.sol_name AS BRANCH_NAME,S.emp_code AS EMP_CODE, U.[USER_ID], ";
                _sql += " U.USER_LOGIN,U.USER_FIRSTNAME + ' ' + U.USER_LASTNAME as FullName, ";
                _sql += " S.dept_name AS USER_DEPARTMENT, U.USER_EMAIL, ";
                _sql += " U.USER_LEVEL, U.USER_LAST_LOGON, U.IS_ACTIVE, U.Role_Name, S.* ";
                _sql += " FROM [AMS].[dbo].[TBL_USERS] U";
                _sql += " INNER JOIN [AMS].[dbo].[SYSTEMOFFICER_Details] S ON S.emp_code = U.USER_EMP_CODE ";
                _sql += " WHERE S.emp_code = @empcode";

                using (SqlConnection connection = new SqlConnection(_iniCon._strConnection))
                {
                    // Open the connection
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(_sql, connection))
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "_sql, connection : line 63 " + _sql.ToString());

                        // Add parameters to the command
                        command.Parameters.AddWithValue("@empcode", emp_code);

                        // Execute the command (safe from SQL injection)
                        SqlDataReader reader = command.ExecuteReader();
                        _logSys.WriteProcessLogFile(_strPathFile, "command.ExecuteReader() : line 69 " + reader.ToString());
                        // Process the results

                        while (reader.Read())
                        {
                            loginVM.Login_State = true;
                            loginVM.BranchID = reader["ID_BRANCH"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "ID_BRANCH : line 75 " + reader["ID_BRANCH"].ToString());
                            loginVM.BranchName = reader["BRANCH_NAME"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "BRANCH_NAME : line 77 " + reader["BRANCH_NAME"].ToString());
                            loginVM.Username = reader["USER_LOGIN"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LOGIN : line 79 " + reader["USER_LOGIN"].ToString());
                            loginVM.UserID = reader["USER_ID"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_ID : line 81 " + reader["USER_ID"].ToString());
                            loginVM.FullName = reader["FullName"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "FullName : line 83 " + reader["FullName"].ToString());
                            loginVM.Level = Convert.ToInt32(reader["USER_LEVEL"]);
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LEVEL : line 885 " + reader["USER_LEVEL"].ToString());
                            loginVM.USER_LAST_LOGON = reader["USER_LAST_LOGON"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LAST_LOGON :  " + reader["USER_LAST_LOGON"].ToString());
                            loginVM.Emp_Code = reader["EMP_CODE"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "EMP_CODE :  " + reader["EMP_CODE"].ToString());
                            loginVM.Is_Active = Convert.ToBoolean(reader["IS_ACTIVE"].ToString());
                            loginVM.FName = reader["fname"].ToString();
                            loginVM.LName = reader["lname"].ToString();
                            loginVM.Zone_Name = reader["zone_name"].ToString();//เขต
                            loginVM.Zone_Code = reader["zone_code"].ToString();
                            loginVM.Dept_Name = reader["dept_name"].ToString();//ฝ่าย
                            loginVM.Dept_Code = reader["dept_code"].ToString();
                            loginVM.Role_Name = reader["role_name"].ToString();
                        }
                        // Close the reader
                        reader.Close();


                    }
                }
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "CheckUserTransferLDAP : " + ex.Message.ToString());
            }


            return loginVM;
        }

        public LoginViewModel CheckUser(string username, string password)
        {
            DataTable _result = new DataTable();
            LoginViewModel loginVM = new LoginViewModel();
            DataTable _exUser = userExist(username);
            if (_exUser.Rows.Count > 0)
            {
                loginVM.Username = username;
            }

            List<GroupReportViewModel> groupReportVMList = new List<GroupReportViewModel>();
            try
            {
                /*ตรวจสอบ user*/
                var encryptpwd = AesUtil.EncryptString(_iniCon.cryptoKey, password, iv);

                _sql = "SELECT DISTINCT  t2.ID_BRANCH, t3.[DESC] AS BRANCH_NAME,t1.USER_ID, t1.USER_LOGIN,";
                _sql += "t1.USER_FIRSTNAME + ' ' + t1.USER_LASTNAME as FullName,";
                _sql += "t1.USER_DEPARTMENT,t1.USER_EMAIL,t1.USER_LEVEL, t1.USER_FLAG";
                _sql += " FROM TBL_USERS t1";
                _sql += " LEFT JOIN TBL_MAP_USER_BRANCH t2 ON t1.USER_ID = t2.ID_USER";
                _sql += " LEFT JOIN TBL_MAP_BRANCH_REPORTS t4 ON t2.ID_BRANCH = t4.ID_BRANCH";
                _sql += " LEFT JOIN Table_Branch t3 ON t2.ID_BRANCH = t3.BRANCH";
                _sql += " WHERE t1.USER_LOGIN = @username AND t1.USER_PASSWORD = @password";

                using (SqlConnection connection = new SqlConnection(_iniCon._strConnection))
                {
                    // Open the connection
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(_sql, connection))
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "_sql, connection : line 63 " + _sql.ToString());

                        // Add parameters to the command
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", encryptpwd);

                        // Execute the command (safe from SQL injection)
                        SqlDataReader reader = command.ExecuteReader();
                        _logSys.WriteProcessLogFile(_strPathFile, "command.ExecuteReader() : line 69 " + reader.ToString());
                        // Process the results

                        while (reader.Read())
                        {
                            loginVM.Login_State = true;
                            loginVM.BranchID = reader["ID_BRANCH"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "ID_BRANCH : line 75 " + reader["ID_BRANCH"].ToString());
                            loginVM.BranchName = reader["BRANCH_NAME"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "BRANCH_NAME : line 77 " + reader["BRANCH_NAME"].ToString());
                            loginVM.Username = reader["USER_LOGIN"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LOGIN : line 79 " + reader["USER_LOGIN"].ToString());
                            loginVM.UserID = reader["USER_ID"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_ID : line 81 " + reader["USER_ID"].ToString());
                            loginVM.FullName = reader["FullName"].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "FullName : line 83 " + reader["FullName"].ToString());
                            loginVM.Level = Convert.ToInt32(reader["USER_LEVEL"]);
                            _logSys.WriteProcessLogFile(_strPathFile, "USER_LEVEL : line 885 " + reader["USER_LEVEL"].ToString());
                        }


                        // Close the reader
                        reader.Close();
                    }
                }

                //20240507
                //_result = _objDB.GetTbDataWithQSeluey(_sql, 1);

                //if (_result.Rows.Count > 0)
                //{
                //    loginVM.Login_State = true;
                //    loginVM.BranchID = _result.Rows[0]["ID_BRANCH"].ToString();
                //    loginVM.BranchName = _result.Rows[0]["BRANCH_NAME"].ToString();
                //    loginVM.Username = _result.Rows[0]["USER_LOGIN"].ToString();
                //    loginVM.UserID = _result.Rows[0]["USER_ID"].ToString();                    
                //    loginVM.FullName = _result.Rows[0]["FullName"].ToString();

                //    return loginVM;

                //}

            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "CheckUser : " + ex.Message.ToString());
            }


            return loginVM;
        }

        public LoginViewModel CheckUser2(string username, string password)
        {
            DataTable _result = new DataTable();
            LoginViewModel loginVM = new LoginViewModel();
            DataTable _exUser = userExist(username);
            List<RoleModels> listRoleDetails = new List<RoleModels>();
            List<GroupReportViewModel> groupReportVMList = new List<GroupReportViewModel>();
            try
            {
                /*ตรวจสอบ user*/
                var encryptpwd = AesUtil.EncryptString(_iniCon.cryptoKey, password, iv);
                if (_exUser.Rows.Count > 0)
                {
                    foreach (DataRow r in _exUser.Rows)
                    {
                        loginVM.Username = r["USER_LOGIN"].ToString();
                        loginVM.UserID = r["USER_ID"].ToString();
                        loginVM.Password = encryptpwd;
                    }
                }
                else
                {
                    loginVM.Username = username;
                    loginVM.UserID = "";
                    loginVM.Password = encryptpwd;
                }
                /* //Old logic 20241216
                _sql = "SELECT DISTINCT ";
                _sql += "case when t2.ID_BRANCH2 <> '' then t2.ID_BRANCH2 else t2.ID_BRANCH end as ID_BRANCH, ";
                _sql += "t3.[DESC] AS BRANCH_NAME,t1.USER_ID, t1.USER_LOGIN,";
                _sql += "t1.USER_FIRSTNAME + ' ' + t1.USER_LASTNAME as FullName,";
                _sql += "t1.USER_DEPARTMENT,t1.USER_EMAIL,t1.USER_LEVEL, t1.USER_FLAG";
                _sql += " FROM TBL_USERS t1";
                _sql += " LEFT JOIN TBL_MAP_USER_BRANCH t2 ON t1.USER_ID = t2.ID_USER";
                _sql += " LEFT JOIN TBL_MAP_BRANCH_REPORTS t4 ON t2.ID_BRANCH = t4.ID_BRANCH";
                _sql += " LEFT JOIN Table_Branch t3 ON t2.ID_BRANCH = t3.BRANCH";
                _sql += " WHERE t1.USER_LOGIN = @username AND t1.USER_PASSWORD = @password";
                */

                using (SqlConnection connection = new SqlConnection(_iniCon._strConnection))
                {
                    // Open the connection
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand("AMS_SP_MAP_USER_BRANCH", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@USERID", loginVM.UserID);
                            command.Parameters.AddWithValue("@PWD", encryptpwd);
                            command.Parameters.AddWithValue("@USERLOGIN", loginVM.Username);

                            SqlDataReader reader = command.ExecuteReader();
                            _logSys.WriteProcessLogFile(_strPathFile, "command.ExecuteReader() : line 69 " + reader.ToString());
                            // Process the results
                            while (reader.Read())
                            {
                                loginVM.Login_State = true;
                                loginVM.BranchID = reader["ID_BRANCH"].ToString();
                                _logSys.WriteProcessLogFile(_strPathFile, "ID_BRANCH : line 75 " + reader["ID_BRANCH"].ToString());
                                loginVM.BranchName = reader["BRANCH_NAME"].ToString();
                                _logSys.WriteProcessLogFile(_strPathFile, "BRANCH_NAME : line 77 " + reader["BRANCH_NAME"].ToString());
                                loginVM.Username = reader["USER_LOGIN"].ToString();
                                _logSys.WriteProcessLogFile(_strPathFile, "USER_LOGIN : line 79 " + reader["USER_LOGIN"].ToString());
                                loginVM.UserID = reader["USER_ID"].ToString();
                                _logSys.WriteProcessLogFile(_strPathFile, "USER_ID : line 81 " + reader["USER_ID"].ToString());
                                loginVM.FullName = reader["FullName"].ToString();
                                _logSys.WriteProcessLogFile(_strPathFile, "FullName : line 83 " + reader["FullName"].ToString());
                                loginVM.Level = Convert.ToInt32(reader["USER_LEVEL"]);
                                _logSys.WriteProcessLogFile(_strPathFile, "USER_LEVEL : line 885 " + reader["USER_LEVEL"].ToString());
                                loginVM.USER_LAST_LOGON = reader["USER_LAST_LOGON"].ToString();
                                loginVM.Is_Active = Convert.ToBoolean(reader["IS_ACTIVE"].ToString());
                                loginVM.Emp_Code = reader["USER_EMP_CODE"].ToString();
                                loginVM.Role_Name = reader["Role_Name"].ToString();
                            }
                            // Close the reader
                            reader.Close();
                        }

                        //if (_c == null) _c = "";
                        //if (_n == null) _n = "";
                        //List<RoleModels> listRoleDetails = AdministratorBAL.NdGetRoleManagementDetials(_c, _n, _o);//roleExport2
                        if (loginVM.Role_Name != "")
                        {
                            using (SqlCommand command = new SqlCommand("AMS_SP_MAIN_GET_MS", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                // Add parameters to the command
                                command.Parameters.AddWithValue("@code", "");
                                command.Parameters.AddWithValue("@name", loginVM.Role_Name);
                                command.Parameters.AddWithValue("@opt", "roleExport2");

                                SqlDataReader reader = command.ExecuteReader();
                                _logSys.WriteProcessLogFile(_strPathFile, "command.ExecuteReader() : line 69 " + reader.ToString());
                                // Process the results
                                while (reader.Read())
                                {
                                    RoleModels obj = new RoleModels();

                                    obj.ID_Role = Convert.ToInt64(reader["ID_Role"]);
                                    obj.Position_Role = Convert.ToString(reader["Position_Role"] != DBNull.Value ? reader["Position_Role"] : string.Empty);
                                    obj.Role_Name = Convert.ToString(reader["Role_Name"] != DBNull.Value ? reader["Role_Name"] : string.Empty);
                                    obj.Module_No = Convert.ToString(reader["Module_No"] != DBNull.Value ? reader["Module_No"] : string.Empty);
                                    obj.Module_Name = Convert.ToString(reader["Module_Name"] != DBNull.Value ? reader["Module_Name"] : string.Empty);
                                    obj.Group_Module = Convert.ToDecimal(reader["Group_Module"]);
                                    //obj.RoleReport_No = Convert.ToDecimal(dr["RoleReport_No"]);
                                    obj.Group_No = Convert.ToString(reader["Group_No"] != DBNull.Value ? reader["Group_No"] : string.Empty);
                                    obj.Group_Report = Convert.ToString(reader["Group_Report"] != DBNull.Value ? reader["Group_Report"] : string.Empty);
                                    obj.RoleReport_Name = Convert.ToString(reader["RoleReport_Name"] != DBNull.Value ? reader["RoleReport_Name"] : string.Empty);
                                    obj.List_Role = Convert.ToBoolean(reader["List_Role"] != DBNull.Value ? reader["List_Role"] : 0);
                                    obj.Create_Role = Convert.ToBoolean(reader["Create_Role"] != DBNull.Value ? reader["Create_Role"] : 0);
                                    obj.View_Role = Convert.ToBoolean(reader["View_Role"] != DBNull.Value ? reader["View_Role"] : 0);
                                    obj.Update_Role = Convert.ToBoolean(reader["Update_Role"] != DBNull.Value ? reader["Update_Role"] : 0);
                                    obj.Delete_Role = Convert.ToBoolean(reader["Delete_Role"] != DBNull.Value ? reader["Delete_Role"] : 0);
                                    obj.Export_Role = Convert.ToBoolean(reader["Export_Role"] != DBNull.Value ? reader["Export_Role"] : 0);
                                    obj.Is_Active = Convert.ToBoolean(reader["Is_Active"] != DBNull.Value ? reader["Is_Active"] : 0);
                                    obj.Create_Date = Convert.ToDateTime(reader["Create_Date"]);
                                    obj.Update_Date = Convert.ToDateTime(reader["Update_Date"]);
                                    obj.Remark = Convert.ToString(reader["Remark"] != DBNull.Value ? reader["Remark"] : string.Empty);
                                    listRoleDetails.Add(obj);
                                }
                                // Close the reader
                                reader.Close();
                            }
                            loginVM.roleList = new groupRole();
                            loginVM.roleList.role_name = loginVM.Role_Name;
                            loginVM.roleList.group_role = listRoleDetails;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "CheckUser-AMS_SP_MAP_USER_BRANCH : " + ex.Message.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "CheckUser : " + ex.Message.ToString());
            }


            return loginVM;
        }
        public bool GetUserLock(string username, string password)
        {
            bool result = false;

            try
            {
                /*ตรวจสอบ user*/
                var encryptpwd = AesUtil.EncryptString(_iniCon.cryptoKey, password, iv);


                using (SqlConnection connection = new SqlConnection(_iniCon._strConnection))
                {
                    // Open the connection
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AMS_SP_GET_USER_LOCK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@USER_LOGIN", username);
                        command.Parameters.AddWithValue("@USER_PASSWORD", encryptpwd);

                        DataTable dt = new DataTable();
                        SqlDataAdapter _adp = new SqlDataAdapter(command);
                        _adp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            result = Convert.ToBoolean(dt.Rows[0]["USER_LOCK"]);
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "GetUserLock : " + ex.Message.ToString());
            }


            return result;
        }
        public void EditUserLock(string username, string password)
        {


            try
            {
                /*lock user*/
                //var encryptpwd = AesUtil.EncryptString(_iniCon.cryptoKey, password, iv);


                using (SqlConnection connection = new SqlConnection(_iniCon._strConnection))
                {
                    // Open the connection
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AMS_SP_EDIT_USER_LOCK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@USER_LOGIN", username);

                        int result = command.ExecuteNonQuery();
                    }
                }



            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "EditUserLock : " + ex.Message.ToString());
            }


        }
        public MenuViewModel AuthorizeUserReport(string branchId, string userId, string Date)
        {
            DataTable _result = new DataTable();

            MenuViewModel mnVM = new MenuViewModel();
            string branchName = "";
            List<GroupReportViewModel> groupReportVMList = new List<GroupReportViewModel>();
            try
            {
                if (branchId != null && branchId != "" && userId != null && userId != "")
                {
                    DataTable _resultGroup = new DataTable();
                    /*ตรวจสอบสิทธิการแสดง report*/
                    //string _storeName = "AMS_SP_GET_USER_MAP_GROUP_REPORTS_v2";
                    string _storeName = "AMS_SP_GET_USER_MAP_GROUP_REPORTS_v3";
                    SqlParameter[] SqlParameter = new SqlParameter[] {
                                                  new SqlParameter("@ID_BRANCH", branchId),
                                                  new SqlParameter("@ID_USER", userId)
                    };

                    _resultGroup = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeName, SqlParameter, 1);

                    foreach (DataRow _drGroup in _resultGroup.Rows)
                    {
                        GroupReportViewModel groupReportVM = new GroupReportViewModel();

                        groupReportVM.GROUP_NO = _drGroup["GROUP_NO"].ToString();
                        groupReportVM.GROUP_NAME = _drGroup["GROUP_NAME"].ToString();

                        groupReportVMList.Add(groupReportVM);
                    }

                    mnVM._groupReportVMList = groupReportVMList;

                    /*ดึงข้อมูลรายละเอียด*/
                    List<GroupDetailReportViewModel> groupDetailReportVMList = new List<GroupDetailReportViewModel>();
                    /*ตรวจสอบสิทธิการแสดง report*/
                    if (branchId != null && branchId != "" && userId != null && userId != "")
                    {

                        DataTable _resultGroupDetail = new DataTable();
                        /*ตรวจสอบสิทธิการแสดง MAP Report*/
                        //string _storeNameDetail = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_v4";
                        string _storeNameDetail = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_v5";
                        SqlParameter[] SqlParameterDeatil = new SqlParameter[] {
                                                  new SqlParameter("@ID_USER", userId),
                                                  new SqlParameter("@ID_BRANCH", branchId)
                                                  };

                        _resultGroupDetail = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeNameDetail, SqlParameterDeatil, 1);

                        int jcount = 1;

                        foreach (DataRow _drGroupDetail in _resultGroupDetail.Rows)
                        {
                            GroupDetailReportViewModel groupReportVM = new GroupDetailReportViewModel();
                            groupReportVM.ID = jcount;
                            groupReportVM.REPORT_NO = _drGroupDetail["REPORT_NO"].ToString();
                            groupReportVM.REPORT_NAME = _drGroupDetail["REPORT_NAME"].ToString();
                            //  groupReportVM.T_TERM_T_TERM_ID = _drGroupDetail["T_TERM_T_TERM_ID"].ToString();
                            groupReportVM.BRANCH_NAME = _drGroupDetail["BRANCH_NAME"].ToString();
                            branchName = _drGroupDetail["BRANCH_NAME"].ToString();
                            groupDetailReportVMList.Add(groupReportVM);
                            jcount++;
                        }
                    }
                    mnVM._groupDetailReportVMList = new List<GroupDetailReportViewModel>();
                    mnVM._groupDetailReportVMList = groupDetailReportVMList;

                }
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "AuthorizeUserReport : " + ex.Message.ToString());
            }

            mnVM.BRANCH_ID = branchId;
            mnVM.BRANCH_NAME = branchName;// (mnVM._groupDetailReportVMList.Count > 0) ? mnVM._groupDetailReportVMList.FirstOrDefault().BRANCH_NAME : "";

            return mnVM;
        }

        public MenuViewModel AuthorizeUserReport2(string branchId, string userId, string Date)
        {
            DataTable _result = new DataTable();

            MenuViewModel mnVM = new MenuViewModel();
            string branchName = "";
            List<GroupReportViewModel> groupReportVMList = new List<GroupReportViewModel>();
            try
            {
                if (branchId != null && branchId != "" && userId != null && userId != "")
                {
                    DataTable _resultGroup = new DataTable();
                    /*ตรวจสอบสิทธิการแสดง report(Group Report)*/
                    //string _storeName = "AMS_SP_GET_USER_MAP_GROUP_REPORTS_v2";
                    //string _storeName = "AMS_SP_GET_USER_MAP_GROUP_REPORTS_v3"; //เปลี่ยนมาเป็นตัวล่าง เพราะเพิ่มกลุ่มพิเศษ
                    string _storeName = "AMS_SP_GET_USER_MAP_GROUP_REPORTS_add_branch_v1";
                    SqlParameter[] SqlParameter = new SqlParameter[] {
                                                  new SqlParameter("@ID_BRANCH", branchId),
                                                  new SqlParameter("@ID_USER", userId)
                    };

                    _resultGroup = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeName, SqlParameter, 1);

                    foreach (DataRow _drGroup in _resultGroup.Rows)
                    {
                        GroupReportViewModel groupReportVM = new GroupReportViewModel();

                        groupReportVM.GROUP_NO = _drGroup["GROUP_NO"].ToString();
                        groupReportVM.GROUP_NAME = _drGroup["GROUP_NAME"].ToString();

                        groupReportVMList.Add(groupReportVM);
                    }

                    mnVM._groupReportVMList = groupReportVMList;

                    /*ดึงข้อมูลรายละเอียด*/
                    List<GroupDetailReportViewModel> groupDetailReportVMList = new List<GroupDetailReportViewModel>();
                    /*ตรวจสอบสิทธิการแสดง report*/
                    if (branchId != null && branchId != "" && userId != null && userId != "")
                    {

                        DataTable _resultGroupDetail = new DataTable();
                        /*ตรวจสอบสิทธิการแสดง MAP Report*/
                        //string _storeNameDetail = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_v4";
                        //string _storeNameDetail = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_v5";
                        string _storeNameDetail = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_add_branch_v1";
                        SqlParameter[] SqlParameterDeatil = new SqlParameter[] {
                                                  new SqlParameter("@ID_USER", userId),
                                                  new SqlParameter("@ID_BRANCH", branchId)
                                                  };

                        _resultGroupDetail = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeNameDetail, SqlParameterDeatil, 1);

                        int jcount = 1;

                        foreach (DataRow _drGroupDetail in _resultGroupDetail.Rows)
                        {
                            GroupDetailReportViewModel groupReportVM = new GroupDetailReportViewModel();
                            groupReportVM.ID = jcount;
                            groupReportVM.REPORT_NO = _drGroupDetail["REPORT_NO"].ToString();
                            groupReportVM.REPORT_NAME = _drGroupDetail["REPORT_NAME"].ToString();
                            //  groupReportVM.T_TERM_T_TERM_ID = _drGroupDetail["T_TERM_T_TERM_ID"].ToString();
                            groupReportVM.BRANCH_NAME = _drGroupDetail["BRANCH_NAME"].ToString();
                            branchName = _drGroupDetail["BRANCH_NAME"].ToString();
                            groupDetailReportVMList.Add(groupReportVM);
                            jcount++;
                        }
                    }
                    mnVM._groupDetailReportVMList = new List<GroupDetailReportViewModel>();
                    mnVM._groupDetailReportVMList = groupDetailReportVMList;

                }
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "AuthorizeUserReport : " + ex.Message.ToString());
            }

            mnVM.BRANCH_ID = branchId;
            mnVM.BRANCH_NAME = branchName;// (mnVM._groupDetailReportVMList.Count > 0) ? mnVM._groupDetailReportVMList.FirstOrDefault().BRANCH_NAME : "";

            return mnVM;
        }

        public MenuViewModel AuthorizeUserReport2ND(string rolename)
        {
            _logSys.WriteErrLog(_strPathFile, "AccountService : AuthorizeUserReport2ND begin");
            DataTable _result = new DataTable();

            MenuViewModel mnVM = new MenuViewModel();
            string branchName = "";
            List<GroupReportViewModel> groupReportVMList = new List<GroupReportViewModel>();
            try
            {
                if (rolename != null)
                {
                    DataTable _resultGroup = new DataTable();
                    /*ตรวจสอบสิทธิการแสดง report(Group Report)*/
                    string _storeName = "AMS_SP_GET_GROUP_REPORT_ND";
                    SqlParameter[] SqlParameter = new SqlParameter[] {
                                                  new SqlParameter("@rolename", rolename),
                                                  new SqlParameter("@groupno", ""),
                                                  new SqlParameter("@mode", "1")
                    };

                    _resultGroup = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeName, SqlParameter, 1);

                    foreach (DataRow _drGroup in _resultGroup.Rows)
                    {
                        _logSys.WriteErrLog(_strPathFile, "AccountService : AMS_SP_GET_GROUP_REPORT_ND begin" + _resultGroup.Rows.Count);
                        GroupReportViewModel groupReportVM = new GroupReportViewModel();

                        groupReportVM.GROUP_NO = _drGroup["GROUP_NO"].ToString();
                        if (_drGroup["GROUP_NAME"].ToString() == "LOC")
                        {
                            groupReportVM.GROUP_NAME = _drGroup["GROUP_NAME"].ToString().Replace("LOC", "LOCATION");
                        }
                        else if (_drGroup["GROUP_NAME"].ToString() == "RAT")
                        {
                            groupReportVM.GROUP_NAME = _drGroup["GROUP_NAME"].ToString().Replace("RAT", "RATM");
                        }
                        else
                        {
                            groupReportVM.GROUP_NAME = _drGroup["GROUP_NAME"].ToString();
                        }
                        ;

                        groupReportVMList.Add(groupReportVM);
                    }

                    mnVM._groupReportVMList = groupReportVMList;

                    /*ดึงข้อมูลรายละเอียด*/
                    List<GroupDetailReportViewModel> groupDetailReportVMList = new List<GroupDetailReportViewModel>();
                    /*ตรวจสอบสิทธิการแสดง report*/
                    if (rolename != null)
                    {

                        DataTable _resultGroupDetail = new DataTable();
                        /*ตรวจสอบสิทธิการแสดง MAP Report*/
                        string _storeNameDetail = "AMS_SP_GET_GROUP_REPORT_ND";
                        SqlParameter[] SqlParameterDeatil = new SqlParameter[] {
                                                  new SqlParameter("@rolename", rolename),
                                                  new SqlParameter("@groupno", ""),
                                                  new SqlParameter("@mode", "2")
                                                  };

                        _resultGroupDetail = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeNameDetail, SqlParameterDeatil, 1);

                        int jcount = 1;

                        foreach (DataRow _drGroupDetail in _resultGroupDetail.Rows)
                        {
                            GroupDetailReportViewModel groupReportVM = new GroupDetailReportViewModel();
                            groupReportVM.ID = jcount;
                            groupReportVM.REPORT_NO = _drGroupDetail["REPORT_NO"].ToString();
                            groupReportVM.REPORT_NAME = _drGroupDetail["REPORT_NAME"].ToString();
                            //  groupReportVM.T_TERM_T_TERM_ID = _drGroupDetail["T_TERM_T_TERM_ID"].ToString();
                            groupReportVM.BRANCH_NAME = "000";// _drGroupDetail["BRANCH_NAME"].ToString();
                            branchName = "000";//_drGroupDetail["BRANCH_NAME"].ToString();
                            groupDetailReportVMList.Add(groupReportVM);
                            jcount++;
                        }
                    }
                    mnVM._groupDetailReportVMList = new List<GroupDetailReportViewModel>();
                    mnVM._groupDetailReportVMList = groupDetailReportVMList;

                }
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "AuthorizeUserReport : " + ex.Message.ToString());
            }

            mnVM.BRANCH_ID = "000";
            mnVM.BRANCH_NAME = branchName;// (mnVM._groupDetailReportVMList.Count > 0) ? mnVM._groupDetailReportVMList.FirstOrDefault().BRANCH_NAME : "";

            return mnVM;
        }

        public groupRole GetNewDesignRole1(string empcode, string rolename, string Date)
        {
            //string branchId = ""; string userId = "";
            DataTable _result = new DataTable();
            groupRole _gr = new groupRole();
            try
            {
                List<RoleModels> _rmList = AdministratorBAL.NdGetRoleManagementDetials(empcode, rolename, "roleExport2");
                if (_rmList.Count > 0)
                {
                    _gr.role_name = rolename;
                    _gr.group_role = _rmList;
                }
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "fn:-GetNewDesignRole1 : " + ex.Message.ToString());
            }
            return _gr;

            //MenuViewModel mnVM = new MenuViewModel();
            //string branchName = "";
            //List<GroupReportViewModel> groupReportVMList = new List<GroupReportViewModel>();
            //try
            //{
            //    if (branchId != null && branchId != "" && userId != null && userId != "")
            //    {
            //        DataTable _resultGroup = new DataTable();
            //        /*ตรวจสอบสิทธิการแสดง report*/
            //        //string _storeName = "AMS_SP_GET_USER_MAP_GROUP_REPORTS_v2";
            //        //string _storeName = "AMS_SP_GET_USER_MAP_GROUP_REPORTS_v3"; //เปลี่ยนมาเป็นตัวล่าง เพราะเพิ่มกลุ่มพิเศษ
            //        string _storeName = "AMS_SP_GET_USER_MAP_GROUP_REPORTS_add_branch_v1";
            //        SqlParameter[] SqlParameter = new SqlParameter[] {
            //                                      new SqlParameter("@ID_BRANCH", branchId),
            //                                      new SqlParameter("@ID_USER", userId)
            //        };

            //        _resultGroup = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeName, SqlParameter, 1);

            //        foreach (DataRow _drGroup in _resultGroup.Rows)
            //        {
            //            GroupReportViewModel groupReportVM = new GroupReportViewModel();

            //            groupReportVM.GROUP_NO = _drGroup["GROUP_NO"].ToString();
            //            groupReportVM.GROUP_NAME = _drGroup["GROUP_NAME"].ToString();

            //            groupReportVMList.Add(groupReportVM);
            //        }

            //        mnVM._groupReportVMList = groupReportVMList;

            //        /*ดึงข้อมูลรายละเอียด*/
            //        List<GroupDetailReportViewModel> groupDetailReportVMList = new List<GroupDetailReportViewModel>();
            //        /*ตรวจสอบสิทธิการแสดง report*/
            //        if (branchId != null && branchId != "" && userId != null && userId != "")
            //        {

            //            DataTable _resultGroupDetail = new DataTable();
            //            /*ตรวจสอบสิทธิการแสดง MAP Report*/
            //            //string _storeNameDetail = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_v4";
            //            //string _storeNameDetail = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_v5";
            //            string _storeNameDetail = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_add_branch_v1";
            //            SqlParameter[] SqlParameterDeatil = new SqlParameter[] {
            //                                      new SqlParameter("@ID_USER", userId),
            //                                      new SqlParameter("@ID_BRANCH", branchId)
            //                                      };

            //            _resultGroupDetail = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeNameDetail, SqlParameterDeatil, 1);

            //            int jcount = 1;

            //            foreach (DataRow _drGroupDetail in _resultGroupDetail.Rows)
            //            {
            //                GroupDetailReportViewModel groupReportVM = new GroupDetailReportViewModel();
            //                groupReportVM.ID = jcount;
            //                groupReportVM.REPORT_NO = _drGroupDetail["REPORT_NO"].ToString();
            //                groupReportVM.REPORT_NAME = _drGroupDetail["REPORT_NAME"].ToString();
            //                //  groupReportVM.T_TERM_T_TERM_ID = _drGroupDetail["T_TERM_T_TERM_ID"].ToString();
            //                groupReportVM.BRANCH_NAME = _drGroupDetail["BRANCH_NAME"].ToString();
            //                branchName = _drGroupDetail["BRANCH_NAME"].ToString();
            //                groupDetailReportVMList.Add(groupReportVM);
            //                jcount++;
            //            }
            //        }
            //        mnVM._groupDetailReportVMList = new List<GroupDetailReportViewModel>();
            //        mnVM._groupDetailReportVMList = groupDetailReportVMList;

            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logSys.WriteProcessLogFile(_strPathFile, "AuthorizeUserReport : " + ex.Message.ToString());
            //}

            //mnVM.BRANCH_ID = branchId;
            //mnVM.BRANCH_NAME = branchName;// (mnVM._groupDetailReportVMList.Count > 0) ? mnVM._groupDetailReportVMList.FirstOrDefault().BRANCH_NAME : "";

        }
        public List<GroupDetailReportViewModel> AuthorizeGroupDetailReport(string branchId, string userId, string groupNo, string strSearch)
        {
            DataTable _result = new DataTable();

            List<GroupDetailReportViewModel> groupDetailReportVMList = new List<GroupDetailReportViewModel>();
            try
            {
                if (branchId != null && branchId != "" && userId != null && userId != "")
                {
                    strSearch = (strSearch == null) ? "%" : strSearch;
                    DataTable _resultGroup = new DataTable();
                    /*ตรวจสอบสิทธิการแสดง MAP Report*/
                    //string _storeName = "AMS_SP_GET_USER_MAP_GROUP_DETAIL_REPORTS_v2";
                    //string _storeName = "AMS_SP_GET_GROUP_DETAIL_REPORTS_v1"; //change 20241217
                    string _storeName = "AMS_SP_GET_GROUP_DETAIL_REPORTS_add_branch_v1";
                    SqlParameter[] SqlParameter = new SqlParameter[] {
                                                  new SqlParameter("@ID_USER", userId),
                                                  new SqlParameter("@ID_BRANCH", branchId),
                                                  new SqlParameter("@ID_GROUP", groupNo),
                                                  //new SqlParameter("@KEY_SEARCH", strSearch),
                                                  };

                    _resultGroup = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeName, SqlParameter, 1);

                    int jcount = 1;
                    foreach (DataRow _drGroup in _resultGroup.Rows)
                    {
                        GroupDetailReportViewModel groupReportVM = new GroupDetailReportViewModel();
                        groupReportVM.ID = jcount;
                        groupReportVM.REPORT_NO = _drGroup["REPORT_NO"].ToString();
                        groupReportVM.REPORT_NAME = _drGroup["REPORT_NAME"].ToString();

                        groupDetailReportVMList.Add(groupReportVM);
                        jcount++;
                    }
                }

            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "AuthorizeGroupDetailReport : " + ex.Message.ToString());

            }


            return groupDetailReportVMList;
        }

        public List<GroupDetailReportViewModel> AuthorizeGroupDetailReportND(string rolename, string userId, string groupNo, string strSearch)
        {
            DataTable _result = new DataTable();

            List<GroupDetailReportViewModel> groupDetailReportVMList = new List<GroupDetailReportViewModel>();
            try
            {
                if (rolename != null)
                {
                    strSearch = (strSearch == null) ? "%" : strSearch;
                    DataTable _resultGroup = new DataTable();
                    /*ตรวจสอบสิทธิการแสดง MAP Report*/
                    string _storeName = "AMS_SP_GET_GROUP_REPORT_ND";
                    SqlParameter[] SqlParameter = new SqlParameter[] {
                                                  new SqlParameter("@rolename", rolename),
                                                  new SqlParameter("@groupno", groupNo),
                                                  new SqlParameter("@mode", "3")
                                                  };

                    _resultGroup = _objDB.ExecuteStoreProceduerMoreOneParameters(_storeName, SqlParameter, 1);

                    int jcount = 1;
                    foreach (DataRow _drGroup in _resultGroup.Rows)
                    {
                        GroupDetailReportViewModel groupReportVM = new GroupDetailReportViewModel();
                        groupReportVM.ID = jcount;
                        groupReportVM.REPORT_NO = _drGroup["REPORT_NO"].ToString();
                        groupReportVM.REPORT_NAME = _drGroup["REPORT_NAME"].ToString();

                        groupDetailReportVMList.Add(groupReportVM);
                        jcount++;
                    }
                }

            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "AuthorizeGroupDetailReport : " + ex.Message.ToString());

            }


            return groupDetailReportVMList;
        }
        public string GetGroupNoReportByName(string Name)
        {
            DataTable dt = new DataTable();
            string groupNo = "";
            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT GROUP_NO FROM TBL_GROUP_REPORT WHERE GROUP_NAME = @NAME_GROUP ORDER BY ID_GROUP DESC");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@NAME_GROUP", Name) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);


                if (dt != null && dt.Rows.Count > 0)
                {

                    groupNo = dt.Rows[0]["GROUP_NO"].ToString();

                }

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return groupNo;
        }

        public string GetGroupNoReportByNameND(string Name)
        {
            DataTable dt = new DataTable();
            string groupNo = "";
            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append(" Select Distinct Module_No " +
                    " FROM [AMS].[dbo].[TBL_ROLE_MODULE_MASTER] " +
                    " Where Len(Module_No) < 4 and Module_Name = @NAME_GROUP " +
                    "");
                //sb.Append("SELECT GROUP_NO FROM TBL_GROUP_REPORT WHERE GROUP_NAME = @NAME_GROUP ORDER BY ID_GROUP DESC");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@NAME_GROUP", Name) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);


                if (dt != null && dt.Rows.Count > 0)
                {

                    groupNo = dt.Rows[0]["Module_No"].ToString();

                }

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return groupNo;
        }

        public bool checkUserLdapinLocal(string ldapName)
        {
            DataTable dt = new DataTable();
            int userLevel = 0;
            bool r = false;
            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT COUNT(*) AS NUM, USER_LOGIN, USER_LEVEL FROM TBL_USERS WHERE USER_LOGIN = @ldapName GROUP BY USER_LOGIN, USER_LEVEL");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ldapName", ldapName) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
                if (dt.Rows.Count > 0)
                {
                    //update branch when user login
                    foreach (DataRow dr in dt.Rows)
                    {
                        userLevel = Convert.ToInt32(dr["USER_LEVEL"].ToString());
                    }

                    int[] levelAd = { 1, 4, 5 };//admin
                    if (levelAd.Contains(userLevel))
                    {
                        r = true;
                    }
                }

            }
            catch (Exception exc)
            {
                //throw exc;
                _logSys.WriteProcessLogFile(_strPathFile, "checkUserLdapinLocal : " + exc.Message.ToString());
            }

            return r;
        }

        public bool checkUserLdapinLocal2(string ldapName)
        {
            DataTable dt = new DataTable();
            bool ldap = false;
            bool r = false;
            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT COUNT(*) AS NUM, USER_LOGIN, USER_FLAG FROM TBL_USERS WHERE USER_LOGIN = @ldapName GROUP BY USER_LOGIN, USER_FLAG");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ldapName", ldapName) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ldap = Convert.ToBoolean(dr["USER_FLAG"].ToString() == "" ? false : dr["USER_FLAG"]);
                    }

                    if (ldap)
                    {
                        r = true;//ldap acc
                    }
                }

            }
            catch (Exception exc)
            {
                //throw exc;
                _logSys.WriteProcessLogFile(_strPathFile, "checkUserLdapinLocal : " + exc.Message.ToString());
            }

            return r;
        }

        public bool checkUserLdapinLocal_solCode(string ldapName, string solcode)
        {
            DataTable dt = new DataTable();
            string userId = string.Empty;
            bool r = false;
            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT COUNT(*) AS NUM, USER_LOGIN, USER_ID FROM TBL_USERS WHERE USER_LOGIN = @ldapName GROUP BY USER_LOGIN, USER_ID");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ldapName", ldapName) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
                if (dt.Rows.Count > 0)
                {
                    //update branch when user login
                    foreach (DataRow dr in dt.Rows)
                    {
                        userId = Convert.ToString(dr["USER_ID"]);
                    }

                    SqlParameter[] sqlParameter1 = new SqlParameter[] {
                                                    //@ID_USER AS bigint
                                                    new SqlParameter("@ID_USER", userId),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",solcode),
                                                 };

                    bool updateSolCode = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter1, 1);
                    //update user branch in tbl user
                    updateSolCode_login(userId, solcode);
                    r = true;
                }

            }
            catch (Exception exc)
            {
                //throw exc;
                _logSys.WriteProcessLogFile(_strPathFile, "checkUserLdapinLocal : " + exc.Message.ToString());
            }

            return r;
        }

        public bool checkUserLdapinLocal_solCode2(string ldapName, string solcode, string empCode)
        {
            DataTable dt = new DataTable();
            string userId = string.Empty;
            bool r = false;
            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT COUNT(*) AS NUM, USER_LOGIN, USER_ID FROM TBL_USERS WHERE USER_LOGIN = @ldapName Or USER_EMP_CODE = @empCode GROUP BY USER_LOGIN, USER_ID");
                SqlParameter[] sqlParameter = new SqlParameter[] {  new SqlParameter("@ldapName", ldapName),
                                                                    new SqlParameter("@empCode", empCode)
                                                };

                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
                if (dt.Rows.Count > 0 && dt.Rows.Count < 2)
                {
                    //update branch when user login
                    foreach (DataRow dr in dt.Rows)
                    {
                        userId = Convert.ToString(dr["USER_ID"]);
                    }

                    SqlParameter[] sqlParameter1 = new SqlParameter[] {
                                                    //@ID_USER AS bigint
                                                    new SqlParameter("@ID_USER", userId),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",solcode),
                                                 };

                    bool updateSolCode = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter1, 1);
                    //update user branch in tbl user
                    updateSolCode_login(userId, solcode);
                    r = true;
                }

            }
            catch (Exception exc)
            {
                //throw exc;
                _logSys.WriteProcessLogFile(_strPathFile, "checkUserLdapinLocal : " + exc.Message.ToString());
            }

            return r;
        }

        public bool checkUserLdapinLocal_solCode3(string ldapName, string solcode, string empCode, ApiPostResponse2 ldapUser2)
        {
            DataTable dt = new DataTable();
            string userId = string.Empty;
            //int level = 0;
            bool r = false;
            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT COUNT(*) AS NUM, USER_LOGIN, USER_ID FROM TBL_USERS WHERE USER_LOGIN = @ldapName and USER_EMP_CODE = @empCode GROUP BY USER_LOGIN, USER_ID");
                SqlParameter[] sqlParameter = new SqlParameter[] {  new SqlParameter("@ldapName", ldapName),
                                                                    new SqlParameter("@empCode", empCode)
                                                };

                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
                //อัพเดทตาราง และโลจิคเก่า
                if (dt.Rows.Count > 0 && dt.Rows.Count < 2)
                {
                    //update branch when user login
                    foreach (DataRow dr in dt.Rows)
                    {
                        userId = Convert.ToString(dr["USER_ID"]);
                    }

                    SqlParameter[] sqlParameter1 = new SqlParameter[] {
                                                    //@ID_USER AS bigint
                                                    new SqlParameter("@ID_USER", userId),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",solcode)
                                                 };

                    bool updateSolCode = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter1, 1);
                    //update user branch in tbl user
                    updateSolCode_login(userId, solcode);
                    r = true;
                }
                else if (dt.Rows.Count == 0)//new user login and insert
                {
                    UserModels obj2 = new UserModels();
                    obj2.USER_EMP_CODE = ldapUser2.emp_code;
                    obj2.USER_LOGIN = ldapName;
                    obj2.USER_FIRSTNAME = ldapUser2.fname;
                    obj2.USER_LASTNAME = ldapUser2.lname;
                    obj2.USER_EMAIL = ldapUser2.email;
                    obj2.USER_LEVEL = Convert.ToInt32(solcode != "000" ? 2 : 0);
                    //USER_LEVEL 0 Not role, 1 Admin, 2 Role Branch, 3 Role Head office 4 Admin1, 5 Amin2;
                    obj2.USER_FLAG = true;//true = AD Account
                    obj2.SOL_CODE = solcode;// cutSolCode(SOL_CODE, maxLength);
                    obj2.IS_ACTIVE = true;
                    obj2.Remark = "";
                    bool resultSave = AdministratorBAL.Nd_AddUserLDAP(obj2, ldapUser2);//AdministratorBAL.AddUserLDAPV4(obj2);
                    r = resultSave;
                }

            }
            catch (Exception exc)
            {
                //throw exc;
                _logSys.WriteProcessLogFile(_strPathFile, "checkUserLdapinLocal : " + exc.Message.ToString());
            }

            return r;
        }

        public void updateSolCode_login(string userId, string solcode)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    //@ID AS bigint,
                                                     new SqlParameter("@ID", userId),
                                                     new SqlParameter("@SOL_CODE", solcode)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_USER_UPDATE_SOLCODE", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            //return result;
        }

        public DataTable userExist(string username)
        {
            DBAccess dbAccess = new DBAccess();

            DataTable dt = new DataTable();
            string _sqlExist = string.Empty;
            try
            {
                //_sqlExist = "SELECT * FROM TBL_USERS WHERE USER_LOGIN = '" + model.Username + "' ";
                //dt = _objDBAcc.GetTbDataWithQSeluey(_sqlExist, null,1);
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM TBL_USERS WHERE USER_LOGIN = @uname");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@uname", username) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "fn_userExist : " + ex.Message.ToString());
            }
            return dt;
        }
    }
}