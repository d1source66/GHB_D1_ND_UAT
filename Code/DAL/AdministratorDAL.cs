using GHB_D1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace GHB_D1.Code.DAL
{
    public class AdministratorDAL
    {
        public static DataTable GetGroupReport()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT * FROM TBL_GROUP_REPORT ORDER BY ID_GROUP DESC", null,1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A1") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetGroupReportByID(int ID)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM TBL_GROUP_REPORT WHERE ID_GROUP = @ID_GROUP ORDER BY ID_GROUP DESC");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID_GROUP", ID) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);


            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static bool DeleteGroupReportByID(int ID)
        {
            bool result = false;

            try
            {
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM TBL_GROUP_REPORT WHERE ID_GROUP = @ID_GROUP");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID_GROUP", ID) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return result;
        }
        public static bool AddGroupReport(GroupReportModel obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@GROUP_NO", obj.GroupCode),
                                                     new SqlParameter("@GROUP_NAME", obj.GroupName),
                                                     new SqlParameter("@ACTIVE", obj.Active),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_GROUP_REPORT", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@ID", obj.ID),
                                                     new SqlParameter("@GROUP_NO", obj.GroupCode),
                                                     new SqlParameter("@GROUP_NAME", obj.GroupName),
                                                     new SqlParameter("@ACTIVE", obj.Active),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_GROUP_REPORT", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static DataTable GetRole()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT ROLE_LEVEL,ROLE_DEPARTMENT,ROLE_ACTIVE,ROLE_CREATED,ROLE_UPDATED FROM TBL_ROLES ORDER BY ROLE_LEVEL", null,1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A2") };
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static bool AddRole(RoleModels obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@ROLE_LEVEL", obj.ROLE_LEVEL),
                                                     new SqlParameter("@ROLE_DEPARTMENT", obj.ROLE_DEPARTMENT),
                                                     new SqlParameter("@ROLE_ACTIVE", obj.ROLE_ACTIVE)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_ROLE", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool NdInsertRole(RoleModels m)
        {
            bool result = false;
            try
            {
                var group_no = Convert.ToDecimal(m.Group_No);
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@Role_Name", m.Role_Name),
                                                     new SqlParameter("@Position_Role", m.Position_Role),
                                                     new SqlParameter("@Module_No", m.Module_No),
                                                     new SqlParameter("@Module_Name", m.Module_Name),
                                                     new SqlParameter("@Group_No", group_no),
                                                     new SqlParameter("@Group_Module", m.Group_Module),
                                                     new SqlParameter("@Group_Report", m.Group_Report),
                                                     new SqlParameter("@RoleReport_Name", m.RoleReport_Name),
                                                     new SqlParameter("@List_Role", m.List_Role),
                                                     new SqlParameter("@Create_Role", m.Create_Role),
                                                     new SqlParameter("@View_Role", m.View_Role),
                                                     new SqlParameter("@Update_Role", m.Update_Role),
                                                     new SqlParameter("@Delete_Role", m.Delete_Role),
                                                     new SqlParameter("@Export_Role", m.Export_Role),
                                                     new SqlParameter("@Is_Active", m.Is_Active),
                                                     //new SqlParameter("@Create_Date", m.Create_Date),
                                                     //new SqlParameter("@Update_Date", m.Update_Date),
                                                     new SqlParameter("@Remark", m.Remark)
                                                 };
                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_ROLE_MANAGEMENT", sqlParameter, 1);
            }
            catch (Exception exc)
            {                
                exc.Message.ToString();
                throw exc;
            }
            return result;
        }

        public static bool NdEditRole(RoleModels m)
        {
            bool result = false;
            try
            {
                var group_no = Convert.ToDecimal(m.Group_No);
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@Role_Name", m.Role_Name),
                                                     new SqlParameter("@Position_Role", m.Position_Role),
                                                     new SqlParameter("@Module_No", m.Module_No),
                                                     new SqlParameter("@Module_Name", m.Module_Name),
                                                     new SqlParameter("@Group_No", group_no),
                                                     new SqlParameter("@Group_Module", m.Group_Module),
                                                     new SqlParameter("@Group_Report", m.Group_Report),
                                                     new SqlParameter("@RoleReport_Name", m.RoleReport_Name),
                                                     new SqlParameter("@List_Role", m.List_Role),
                                                     new SqlParameter("@Create_Role", m.Create_Role),
                                                     new SqlParameter("@View_Role", m.View_Role),
                                                     new SqlParameter("@Update_Role", m.Update_Role),
                                                     new SqlParameter("@Delete_Role", m.Delete_Role),
                                                     new SqlParameter("@Export_Role", m.Export_Role),
                                                     new SqlParameter("@Is_Active", m.Is_Active),
                                                     //new SqlParameter("@Create_Date", m.Create_Date),
                                                     //new SqlParameter("@Update_Date", m.Update_Date),
                                                     new SqlParameter("@Remark", m.Remark)
                                                 };
                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_ROLE_MANAGEMENT", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                exc.Message.ToString();
                throw exc;
            }
            return result;
        }
        public static DataTable GetAlldRole()
        {
            DataTable dt = new DataTable();
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A29") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        public static DataTable GetRoleByID(long ID)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ROLE_LEVEL,ROLE_DEPARTMENT,ROLE_ACTIVE,ROLE_CREATED,ROLE_UPDATED FROM TBL_ROLES WHERE ROLE_LEVEL = @ROLE_LEVEL");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ROLE_LEVEL", ID) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static bool EditRole(RoleModels obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@ROLE_LEVEL", obj.ROLE_LEVEL),
                                                     new SqlParameter("@ROLE_DEPARTMENT", obj.ROLE_DEPARTMENT),
                                                     new SqlParameter("@ROLE_ACTIVE", obj.ROLE_ACTIVE),

                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_ROLE", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM TBL_ROLES WHERE ROLE_LEVEL = @ROLE_LEVEL");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ROLE_LEVEL", lngID) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return result;
        }
        public static DataTable GetBranch()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT * FROM Table_Branch ORDER BY BRANCH DESC",null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A3") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetBranchByID(string BRANCH)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM Table_Branch WHERE BRANCH = @BRANCH");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@BRANCH", BRANCH) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static bool AddBranch(BranchModel obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@BRANCH", obj.BRANCH),
                                                     new SqlParameter("@DESC", obj.DESC),
                                                     new SqlParameter("@DESC_ENG", obj.DESC_ENG),
                                                     new SqlParameter("@ZONE", obj.ZONE),
                                                     new SqlParameter("@REGION", obj.REGION),
                                                     new SqlParameter("@EMAIL", obj.EMAIL),
                                                     new SqlParameter("@EXCEPTREGION", obj.EXCEPTREGION),
                                                     new SqlParameter("@BNREGION", obj.BNREGION),
                                                     new SqlParameter("@FLAGCLOSE", obj.FLAGCLOSE),
                                                     new SqlParameter("@Alt1", obj.Alt1),
                                                     new SqlParameter("@Alt2", obj.Alt2),
                                                     new SqlParameter("@Alt3", obj.Alt3),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_BRANCH", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@BRANCH", obj.BRANCH),
                                                     new SqlParameter("@DESC", obj.DESC),
                                                     new SqlParameter("@DESC_ENG", obj.DESC_ENG),
                                                     new SqlParameter("@ZONE", obj.ZONE),
                                                     new SqlParameter("@REGION", obj.REGION),
                                                     new SqlParameter("@EMAIL", obj.EMAIL),
                                                     new SqlParameter("@EXCEPTREGION", obj.EXCEPTREGION),
                                                     new SqlParameter("@BNREGION", obj.BNREGION),
                                                     new SqlParameter("@FLAGCLOSE", obj.FLAGCLOSE),
                                                     new SqlParameter("@Alt1", obj.Alt1),
                                                     new SqlParameter("@Alt2", obj.Alt2),
                                                     new SqlParameter("@Alt3", obj.Alt3),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_BRANCH", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM Table_Branch WHERE BRANCH = @BRANCH");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@BRANCH", BRANCH) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return result;
        }
        public static DataTable GetGroupDetailReport()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT a.*,b.GROUP_NAME FROM TBL_GROUP_DETAIL_REPORT a left join TBL_GROUP_REPORT b on a.GROUP_NO = b.GROUP_NO", null,1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A4") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetGroupDetailReportByID(int intID)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM TBL_GROUP_DETAIL_REPORT WHERE ID_REPORT = @ID_REPORT");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID_REPORT", intID) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static bool AddGroupDetailReport(GroupDetailReportModel obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@GROUP_NO", obj.GROUP_NO),
                                                     new SqlParameter("@REPORT_NO", obj.REPORT_NO),
                                                     new SqlParameter("@REPORT_NAME", obj.REPORT_NAME),
                                                     new SqlParameter("@ACTIVE", obj.ACTIVE),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_GROUP_DETAIL_REPORT", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@ID_REPORT", obj.ID_REPORT),
                                                     new SqlParameter("@GROUP_NO", obj.GROUP_NO),
                                                     new SqlParameter("@REPORT_NO", obj.REPORT_NO),
                                                     new SqlParameter("@REPORT_NAME", obj.REPORT_NAME),
                                                     new SqlParameter("@ACTIVE", obj.ACTIVE),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_GROUP_DETAIL_REPORT", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM TBL_GROUP_DETAIL_REPORT WHERE ID_REPORT = @ID_REPORT");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID_REPORT", ID) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return result;
        }
        public static DataTable GetUser()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT * FROM TBL_USERS", null,1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A28") };//A5-old

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable NdGetRoleManagement()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT * FROM TBL_USERS", null,1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A32") };//A5-old

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable NdCheckDupRoleName(string _c, string _n, string _o)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@code", _c),
                                                new SqlParameter("@name", _n),
                                                new SqlParameter("@opt", _o),
                };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GET_MS", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable NdGetRoleManagementDetials(string _c, string _n, string _o)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@code", _c),
                                                new SqlParameter("@name", _n),
                                                new SqlParameter("@opt", _o),
                };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GET_MS", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable NdGetUserTransfer()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A31") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable NdSearchUserTransfer(UserTransferScheduleModel obj)
        {
            DataTable dt = new DataTable();
            string sch_time = obj.sch_time.ToString("yyyy-MM-dd") == "0001-01-01"?"": obj.sch_time.ToString("yyyy-MM-dd");
            string start_date = obj.start_date.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : obj.start_date.ToString("yyyy-MM-dd");
            string end_date = obj.end_date.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : obj.end_date.ToString("yyyy-MM-dd");
            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@emp_code", obj.emp_code),
                                                     new SqlParameter("@fname", obj.fname),
                                                     new SqlParameter("@lname", obj.lname),
                                                     new SqlParameter("@sch_time", sch_time),
                                                     new SqlParameter("@start_date", start_date),
                                                     new SqlParameter("@end_date", end_date)
                };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_SEARCH_USER_TRANSFER_SCHEDULE", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable NdGetUserTransfer_param(string empcode, long id)
        {
            DataTable dt = new DataTable();
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@emp_code", empcode),
                                                     new SqlParameter("@transfer_id", id)
                                                 };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_USER_TRANSFER_SCHEDULE", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        public static DataTable NdCheckTblTransfer(string _c, string _n, string _o)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@code", _c),
                                                new SqlParameter("@name", _n),
                                                new SqlParameter("@opt", _o),
                };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GET_MS", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable NdGetUserSysTransfer(string _c, string _n, string _o)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@code", _c),
                                                new SqlParameter("@name", _n),
                                                new SqlParameter("@opt", _o),
                };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GET_MS", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GetUserProfile()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A22") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetUserByID(long ID)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                //sb.Append("SELECT * FROM TBL_USERS WHERE [USER_ID] = @USER_ID");
                sb.Append(" SELECT a.*, b.ROLE_DEPARTMENT as ROLE_NAME, c.* FROM TBL_USERS a LEFT JOIN[TBL_ROLES] b on b.ROLE_LEVEL = a.USER_LEVEL INNER JOIN[SYSTEMOFFICER_Details] c on c.emp_code = a.USER_EMP_CODE WHERE [USER_ID] = @USER_ID");
                //SELECT a.*, b.ROLE_DEPARTMENT as ROLE_NAME, c.* FROM TBL_USERS a LEFT JOIN[TBL_ROLES] b on b.ROLE_LEVEL = a.USER_LEVEL INNER JOIN[SYSTEMOFFICER_Details] c on c.emp_code = a.USER_EMP_CODE
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@USER_ID", ID) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GetUserByParam(List<UserModels> model, UserModels param)
        {
            DataTable dt = new DataTable();
            string isActive = string.Empty;
            try
            {
                DBAccess dbAccess = new DBAccess();
                if (param.IS_ACTIVE)
                {
                    isActive = "true";
                }
                else
                {
                    isActive = "false";
                }
                SqlParameter[] sqlParameter = new SqlParameter[] {
                    new SqlParameter("@USER_LOGIN", param.USER_LOGIN),
                    new SqlParameter("@USER_EMP_CODE", param.USER_EMP_CODE),
                    new SqlParameter("@USER_FIRSTNAME", param.USER_FIRSTNAME),
                    new SqlParameter("@USER_LASTNAME", param.USER_LASTNAME),
                    new SqlParameter("@Dept_Code", param.Dept_Code),
                    new SqlParameter("@Hub_Code", param.Hub_Code),
                    new SqlParameter("@SOL_CODE", param.SOL_CODE),
                    new SqlParameter("@IS_ACTIVE", param.IS_ACTIVE2)
                };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_USER_BY_SEARCH", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static int GetCountUserByUserName(string UserName)
        {
            DataTable dt = new DataTable();
            int intCount = 0;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@USER_LOGIN", UserName) };
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_COUNT_USER", sqlParameter, 1);
                if (dt != null)
                {
                    intCount = Convert.ToInt32(dt.Rows[0]["CountUser"]);
                }
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
            string userId = string.Empty;
            string solCode = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     //@USER_LOGIN as varchar(50),
                                                     new SqlParameter("@USER_LOGIN",obj.USER_LOGIN),
                                                    //@USER_FIRSTNAME as varchar(150),
                                                    new SqlParameter("@USER_FIRSTNAME", obj.USER_FIRSTNAME),
                                                    //@USER_LASTNAME as varchar(150),
                                                    new SqlParameter("@USER_LASTNAME", obj.USER_LASTNAME),
                                                    //@USER_DEPARTMENT as nvarchar(50),
                                                    //@USER_LEVEL as int,
                                                    new SqlParameter("@USER_LEVEL", obj.USER_LEVEL),
                                                    //@USER_EMAIL as nvarchar(50)
                                                    new SqlParameter("@USER_EMAIL", obj.USER_EMAIL),
                                                    //@USER_FLAG as bit,
                                                    new SqlParameter("@USER_FLAG", obj.USER_FLAG),
                                                    new SqlParameter("@USER_EMP_CODE", obj.USER_EMP_CODE),
                                                    new SqlParameter("@SOL_CODE", obj.SOL_CODE)
                                                 };

                //result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_USER_LDAP", sqlParameter, 1);
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_ADD_USER_LDAP", sqlParameter, 1);
                if (dt.Rows.Count > 0)
                {
                    //update branch when regis first times
                    foreach (DataRow dr in dt.Rows)
                    {
                        userId = Convert.ToString(dr["USER_ID"]);
                        solCode = Convert.ToString(dr["SOL_CODE"]);
                    }

                    SqlParameter[] sqlParameter1 = new SqlParameter[] {
                                                    //@ID_USER AS bigint
                                                    new SqlParameter("@ID_USER", userId),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",solCode),
                                                 };

                    bool updateSolCode = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter1, 1);
                    result = false;
                }
                else
                {
                    //insert branch when regis more than one times
                    foreach (DataRow dr in dt.Rows)
                    {
                        userId = Convert.ToString(dr["USER_ID"]);
                        solCode = Convert.ToString(dr["SOL_CODE"]);
                    }

                    SqlParameter[] sqlParameter1 = new SqlParameter[] {
                                                    //@ID_USER AS bigint
                                                    new SqlParameter("@ID_USER", userId),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",solCode),
                                                 };

                    bool updateSolCode = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter1, 1);
                    result = true;
                }
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
            string userId = string.Empty;
            string solCode = string.Empty;
            DataTable dt = new DataTable();
            DataTable dtExist = new DataTable();
            dtExist = userExist(obj);
            try
            {
                if (dtExist.Rows.Count == 0)
                {
                    try
                    {
                        DBAccess dbAccess = new DBAccess();

                        SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@USER_LOGIN",obj.USER_LOGIN),//@USER_LOGIN as varchar(50),  
                                                    new SqlParameter("@USER_FIRSTNAME", obj.USER_FIRSTNAME),//@USER_FIRSTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LASTNAME", obj.USER_LASTNAME),//@USER_LASTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LEVEL", obj.USER_LEVEL),//@USER_LEVEL as int,      
                                                    new SqlParameter("@USER_EMAIL", obj.USER_EMAIL),//@USER_EMAIL as nvarchar(50), 
                                                    new SqlParameter("@USER_FLAG", obj.USER_FLAG),//@USER_FLAG as bit,
                                                    new SqlParameter("@USER_EMP_CODE", obj.USER_EMP_CODE),//@USER_EMP_CODE as varchar(50)
                                                    new SqlParameter("@SOL_CODE", obj.SOL_CODE),//@SOL_CODE as varchar(10)
                                                    new SqlParameter("@IS_ACTIVE", obj.IS_ACTIVE)//@IS_ACTIVE as bit
                                                 };

                        //result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_USER_LDAP", sqlParameter, 1);
                        dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_ADD_USER_LDAP", sqlParameter, 1);
                        if (dt.Rows.Count > 0 && dt.Rows.Count < 2)
                        {
                            //update branch when regis first times
                            foreach (DataRow dr in dt.Rows)
                            {
                                userId = Convert.ToString(dr["USER_ID"]);
                                solCode = Convert.ToString(dr["SOL_CODE"]);
                            }

                            SqlParameter[] sqlParameter1 = new SqlParameter[] {
                                                    //@ID_USER AS bigint
                                                    new SqlParameter("@ID_USER", userId),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",solCode),
                                                 };

                            bool updateSolCode = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter1, 1);
                            result = updateSolCode;
                        }
                        //else
                        //{
                        //    //insert branch when regis more than one times
                        //    foreach (DataRow dr in dt.Rows)
                        //    {
                        //        userId = Convert.ToString(dr["USER_ID"]);
                        //        solCode = Convert.ToString(dr["SOL_CODE"]);
                        //    }

                        //    SqlParameter[] sqlParameter1 = new SqlParameter[] {
                        //                                    //@ID_USER AS bigint
                        //                                    new SqlParameter("@ID_USER", userId),
                        //                                    //@ID_BRANCH AS nvarchar(5)
                        //                                    new SqlParameter("@ID_BRANCH",solCode),
                        //                                 };

                        //    bool updateSolCode = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter1, 1);
                        //    result = true;
                        //}
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
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
            string userId = string.Empty;
            string solCode = string.Empty;
            DataTable dt = new DataTable();
            DataTable dtExist = new DataTable();
            dtExist = userExist(obj);
            try
            {
                if (dtExist.Rows.Count == 0)
                {
                    try
                    {
                        DBAccess dbAccess = new DBAccess();

                        SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@USER_LOGIN",obj.USER_LOGIN),//@USER_LOGIN as varchar(50),  
                                                    new SqlParameter("@USER_FIRSTNAME", obj.USER_FIRSTNAME),//@USER_FIRSTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LASTNAME", obj.USER_LASTNAME),//@USER_LASTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LEVEL", obj.USER_LEVEL),//@USER_LEVEL as int,      
                                                    new SqlParameter("@USER_EMAIL", obj.USER_EMAIL),//@USER_EMAIL as nvarchar(50), 
                                                    new SqlParameter("@USER_FLAG", obj.USER_FLAG),//@USER_FLAG as bit,
                                                    new SqlParameter("@USER_EMP_CODE", obj.USER_EMP_CODE),//@USER_EMP_CODE as varchar(50)
                                                    new SqlParameter("@SOL_CODE", obj.SOL_CODE),//@SOL_CODE as varchar(10)
                                                    new SqlParameter("@IS_ACTIVE", obj.IS_ACTIVE)//@IS_ACTIVE as bit
                                                 };

                        //result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_USER_LDAP", sqlParameter, 1);
                        dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_ADD_USER_LDAP", sqlParameter, 1);
                        if (dt.Rows.Count > 0 && dt.Rows.Count < 2)
                        {
                            //update branch when regis first times
                            foreach (DataRow dr in dt.Rows)
                            {
                                userId = Convert.ToString(dr["USER_ID"]);
                                solCode = Convert.ToString(dr["SOL_CODE"]);
                            }

                            SqlParameter[] sqlParameter1 = new SqlParameter[] {
                                                    //@ID_USER AS bigint
                                                    new SqlParameter("@ID_USER", userId),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",solCode),
                                                 };

                            bool updateSolCode = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter1, 1);
                            result = updateSolCode;
                        }

                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
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
            string userId = string.Empty;
            string solCode = string.Empty;
            DataTable dt = new DataTable();
            DataTable dtExist = new DataTable();
            DataTable Nd_dtExist = new DataTable();
            dtExist = userExist(obj);
            Nd_dtExist = Nd_userExist(obj);
            try
            {
                if (dtExist.Rows.Count == 0 && Nd_dtExist.Rows.Count == 0)//ไม่พบ user in webapp
                {
                    try
                    {
                        DBAccess dbAccess = new DBAccess();

                        SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@USER_LOGIN",obj.USER_LOGIN),//@USER_LOGIN as varchar(50),  
                                                    new SqlParameter("@USER_FIRSTNAME", obj.USER_FIRSTNAME),//@USER_FIRSTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LASTNAME", obj.USER_LASTNAME),//@USER_LASTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LEVEL", obj.USER_LEVEL),//@USER_LEVEL as int,      
                                                    new SqlParameter("@USER_EMAIL", obj.USER_EMAIL),//@USER_EMAIL as nvarchar(50), 
                                                    new SqlParameter("@USER_FLAG", obj.USER_FLAG),//@USER_FLAG as bit,
                                                    new SqlParameter("@USER_EMP_CODE", obj.USER_EMP_CODE),//@USER_EMP_CODE as varchar(50)
                                                    new SqlParameter("@SOL_CODE", obj.SOL_CODE),//@SOL_CODE as varchar(10)
                                                    new SqlParameter("@IS_ACTIVE", obj.IS_ACTIVE),//@IS_ACTIVE as bit
                                                    new SqlParameter("@Remark", obj.Remark)
                                                 };

                        //result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_USER_LDAP", sqlParameter, 1);
                        dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_ADD_USER_LDAP2", sqlParameter, 1);
                        if (dt.Rows.Count > 0 && dt.Rows.Count < 2)
                        {
                            result = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool InsertDataFromAPI(ApiPostResponse2 ldapUser, DataTable _dt, UserModels obj, string tableName)
        {
            bool _result = false;
            string _inSsql = string.Empty;
            DataTable dtExist = new DataTable();
            DataTable Nd_dtExist = new DataTable();
            DataTable dt1 = new DataTable();
            dtExist = userExist(obj);
            Nd_dtExist = Nd_userExist(obj);
            try
            {
                if (dtExist.Rows.Count == 0 && Nd_dtExist.Rows.Count == 0)//ไม่พบ user in webapp
                {
                    //1.add local user to tbl_user
                    DBAccess dbAccess = new DBAccess();

                    SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@USER_LOGIN",obj.USER_LOGIN),//@USER_LOGIN as varchar(50),  
                                                    new SqlParameter("@USER_FIRSTNAME", obj.USER_FIRSTNAME),//@USER_FIRSTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LASTNAME", obj.USER_LASTNAME),//@USER_LASTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LEVEL", obj.USER_LEVEL),//@USER_LEVEL as int,      
                                                    new SqlParameter("@USER_EMAIL", obj.USER_EMAIL),//@USER_EMAIL as nvarchar(50), 
                                                    new SqlParameter("@USER_FLAG", obj.USER_FLAG),//@USER_FLAG as bit,
                                                    new SqlParameter("@USER_EMP_CODE", obj.USER_EMP_CODE),//@USER_EMP_CODE as varchar(50)
                                                    new SqlParameter("@SOL_CODE", obj.SOL_CODE),//@SOL_CODE as varchar(10)
                                                    new SqlParameter("@IS_ACTIVE", obj.IS_ACTIVE),//@IS_ACTIVE as bit
                                                    new SqlParameter("@Role_Name", obj.Role_Name),
                                                    new SqlParameter("@Remark", obj.Remark)
                                                 };

                    //result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_USER_LDAP", sqlParameter, 1);
                    dt1 = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_ADD_USER_LDAP2", sqlParameter, 1);
                    if (dt1.Rows.Count > 0 && dt1.Rows.Count < 2)
                    {
                        //2.add user ldap to SYSTEMOFFICER_Details
                        _result = dbAccess.CopyToDataSource(ldapUser, _dt, tableName, 1);
                    }                    
                }
                else
                {
                    _result = false;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return _result;
        }


        public static bool EditUser(UserModels obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    //@ID AS bigint,
                                                     new SqlParameter("@ID", obj.USER_ID),
                                                     //@USER_LOGIN as varchar(50),
                                                     new SqlParameter("@USER_LOGIN",obj.USER_LOGIN),
                                                     //@USER_PASSWORD as varchar(50),
                                                     new SqlParameter("@USER_PASSWORD",obj.USER_PASSWORD),
                                                    //@USER_FIRSTNAME as varchar(150),
                                                    new SqlParameter("@USER_FIRSTNAME", obj.USER_FIRSTNAME),
                                                    //@USER_LASTNAME as varchar(150),
                                                    new SqlParameter("@USER_LASTNAME", obj.USER_LASTNAME),
                                                    //@USER_LEVEL as int,
                                                    new SqlParameter("@USER_LEVEL", obj.USER_LEVEL),
                                                    //@USER_EMAIL as nvarchar(50)
                                                    new SqlParameter("@USER_EMAIL", obj.USER_EMAIL),
                                                    new SqlParameter("@USER_LOCK", obj.USER_LOCK)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_USER", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {                                                    
                                                    new SqlParameter("@ID", obj.USER_ID),//@ID AS bigint,                                                     
                                                    new SqlParameter("@USER_LOGIN",obj.USER_LOGIN),//@USER_LOGIN as varchar(50),   
                                                    new SqlParameter("@USER_PASSWORD",obj.USER_PASSWORD),//@USER_PASSWORD as varchar(50),
                                                    new SqlParameter("@USER_FIRSTNAME", obj.USER_FIRSTNAME),//@USER_FIRSTNAME as varchar(150), 
                                                    new SqlParameter("@USER_LASTNAME", obj.USER_LASTNAME),//@USER_LASTNAME as varchar(150),
                                                    new SqlParameter("@USER_LEVEL", obj.USER_LEVEL),//@USER_LEVEL as int,
                                                    new SqlParameter("@USER_EMAIL", obj.USER_EMAIL),//@USER_EMAIL as nvarchar(50)
                                                    new SqlParameter("@USER_LOCK", obj.USER_LOCK),
                                                    new SqlParameter("@Role_Name", obj.Role_Name),
                                                    new SqlParameter("@IS_ACTIVE", obj.IS_ACTIVE),
                                                    new SqlParameter("@Remark", obj.Remark)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_USER3", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool UPDATEROLE(UserModels obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    //@ID AS bigint,
                                                     new SqlParameter("@ID", obj.USER_ID),
                                                     //@USER_LOGIN as varchar(50),
                                                     new SqlParameter("@USER_LOGIN",obj.USER_LOGIN),
                                                     //@ADMIN_BY as nvarchar(20)
                                                     new SqlParameter("@ADMIN_BY",obj.ADMIN_BY),
                                                     //@ADMIN_CREATE_DATE as nvarchar(20)
                                                     new SqlParameter("@ADMIN_CREATE_DATE",obj.ADMIN_CREATE_DATE),
                                                     //@USER_LEVEL as int,
                                                     new SqlParameter("@USER_LEVEL", obj.USER_LEVEL),
                                                     //@IS_ACTIVE as bit
                                                     new SqlParameter("@IS_ACTIVE", obj.IS_ACTIVE)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_MAINUSER_UPDATEROLE", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();


                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM TBL_USERS WHERE USER_ID = @USER_ID");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@USER_ID", ID) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return result;
        }
        public static DataTable GetMap_User_Branch()
        {
            DataTable dt = new DataTable();

            try
            {

                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A19") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMap_Branch_Report()
        {
            DataTable dt = new DataTable();

            try
            {

                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A18") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMap_User_Report()
        {
            DataTable dt = new DataTable();

            try
            {
                /*
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT");
                sb.Append(" a.*,b.USER_LOGIN as UserName,c.GROUP_NAME as GroupName,d.REPORT_NAME as ReportName,e.[DESC] as BranchName ");
                sb.Append("FROM");
                sb.Append(" TBL_MAP_USER_REPORT a left join TBL_USERS b on a.ID_USER = b.USER_ID");
                sb.Append(" left join TBL_GROUP_REPORT c on a.GROUP_NO = c.GROUP_NO");
                sb.Append(" left join TBL_GROUP_DETAIL_REPORT d on a.REPORT_NO = d.REPORT_NO");
                sb.Append(" left join Table_Branch e on a.ID_BRANCH = e.BRANCH");

                DBAccess dbAccess = new DBAccess();

                dt = dbAccess.GetTbDataWithQSeluey(sb.ToString(), null,1);
                */
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A11") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMap_Branch_ReportByID(long ID)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM TBL_MAP_BRANCH_REPORTS WHERE ID = @ID");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID", ID) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMap_User_BranchByID(long ID)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM TBL_MAP_USER_BRANCH WHERE ID = @ID");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID", ID) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMap_User_ReportByID(long ID)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM TBL_MAP_USER_REPORT WHERE ID_MENU = @ID_MENU");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID_MENU", ID) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static bool AddMap_User_Branch(MAP_USER_BRANCHModel obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    //@ID_USER AS bigint
                                                    new SqlParameter("@ID_USER",obj.ID_USER),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",obj.ID_BRANCH),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_BRANCH", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    //@GROUP_NO AS numeric(18,2),
                                                    new SqlParameter("@GROUP_NO",obj.GROUP_NO),
                                                    //@REPORT_NO AS numeric(18,2),
                                                    new SqlParameter("@REPORT_NO",obj.REPORT_NO),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",obj.ID_BRANCH),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_BRANCH_REPORT", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    //@GROUP_NO AS numeric(18,2),
                                                    new SqlParameter("@GROUP_NO",obj.GROUP_NO),
                                                    //@REPORT_NO AS numeric(18,2),
                                                    new SqlParameter("@REPORT_NO",obj.REPORT_NO),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",obj.ID_BRANCH),
                                                    //@Seleted AS bit
                                                    new SqlParameter("@Seleted",obj.IsSelected),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_BRANCH_REPORT2", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    //@ID_USER AS bigint,
                                                    new SqlParameter("@ID_USER",obj.ID_USER),
                                                    //@GROUP_NO AS numeric(18,2),
                                                    new SqlParameter("@GROUP_NO",obj.GROUP_NO),
                                                    //@REPORT_NO AS numeric(18,2),
                                                    new SqlParameter("@REPORT_NO",obj.REPORT_NO),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",obj.ID_BRANCH),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_MAP_USER_REPORT", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@ID",obj.ID),
                                                    
                                                    //@GROUP_NO AS numeric(18,2),
                                                    new SqlParameter("@GROUP_NO",obj.GROUP_NO),
                                                    //@REPORT_NO AS numeric(18,2),
                                                    new SqlParameter("@REPORT_NO",obj.REPORT_NO),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",obj.ID_BRANCH),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_MAP_BRANCH_REPORT", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@ID",obj.ID),
                                                    //@ID_USER AS bigint,
                                                    new SqlParameter("@ID_USER",obj.ID_USER),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",obj.ID_BRANCH),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_MAP_USER_BRANCH", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@ID",obj.ID),
                                                    //@ID_USER AS bigint,
                                                    new SqlParameter("@ID_USER",obj.ID_USER),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",obj.ID_BRANCH),
                                                    //@ID_BRANCH2 AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH2",obj.ID_BRANCH2),
                                                    //@B_DATE ASdatetime
                                                    new SqlParameter("@B_DATE",obj.B_DATE),
                                                    //@E_DATE AS datetime
                                                    new SqlParameter("@E_DATE",obj.E_DATE),
                                                    //@PERMA AS bit
                                                    new SqlParameter("@PERMA",obj.PERMA),
                                                    //@OPEN_OPT AS bit
                                                    new SqlParameter("@OPEN_OPT",obj.OPEN_OPT)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_MAP_USER_BRANCH2", sqlParameter, 1);
            }
            catch (SqlException ex)
            {
                ex.Message.ToString();
                //Console.WriteLine($"SQL Error: {ex.Message}");
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
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@ID_MENU",obj.ID_MENU),
                                                    //@ID_USER AS bigint,
                                                    new SqlParameter("@ID_USER",obj.ID_USER),
                                                    //@GROUP_NO AS numeric(18,2),
                                                    new SqlParameter("@GROUP_NO",obj.GROUP_NO),
                                                    //@REPORT_NO AS numeric(18,2),
                                                    new SqlParameter("@REPORT_NO",obj.REPORT_NO),
                                                    //@ID_BRANCH AS nvarchar(5)
                                                    new SqlParameter("@ID_BRANCH",obj.ID_BRANCH),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_MAP_USER_REPORT", sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM TBL_MAP_USER_REPORT WHERE ID_MENU = @ID_MENU");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID_MENU", ID) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM TBL_MAP_USER_BRANCH WHERE ID = @ID");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID", ID) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);
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
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM TBL_MAP_BRANCH_REPORTS WHERE ID = @ID");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID", ID) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return result;
        }
        public static DataTable GetBranchByUserID(string UserID)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT DISTINCT ID_BRANCH FROM TBL_MAP_USER_REPORT WHERE ID_USER = @ID_USER");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@ID_USER", UserID) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable NdGetBrandRelation(string brandcode)
        {
            DataTable dt = new DataTable();
            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@BRANCH_CODE", brandcode) };
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_BRANCH_RELATION", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        public static DataTable NdGetBrandRelation2(string emp_code)
        {
            DataTable dt = new DataTable();
            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@EMP_CODE", emp_code)
                };
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_BRANCH_RELATION2", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        public static bool NdInsertUserTransfer(UserTransferScheduleModel obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@emp_code", obj.emp_code),
                                                     new SqlParameter("@fname", obj.fname),
                                                     new SqlParameter("@lname", obj.lname),
                                                     new SqlParameter("@is_active", obj.is_active),
                                                     new SqlParameter("@sch_time", obj.sch_time),
                                                     new SqlParameter("@start_date", obj.start_date),
                                                     new SqlParameter("@end_date", obj.end_date),
                                                     new SqlParameter("@branch_code", obj.branch_code),
                                                     new SqlParameter("@branch_name", obj.branch_name),
                                                     new SqlParameter("@hub_code", obj.hub_code),
                                                     new SqlParameter("@hub_name", obj.hub_name),
                                                     new SqlParameter("@dept_code", obj.dept_code),
                                                     new SqlParameter("@dept_name", obj.dept_name),
                                                     new SqlParameter("@branch_code2", obj.branch_code2),
                                                     new SqlParameter("@branch_name2", obj.branch_name2),
                                                     new SqlParameter("@hub_code2", obj.hub_code2),
                                                     new SqlParameter("@hub_name2", obj.hub_name2),
                                                     new SqlParameter("@dept_code2", obj.dept_code2),
                                                     new SqlParameter("@dept_name2", obj.dept_name2),
                                                     new SqlParameter("@user_level", obj.user_level),
                                                     new SqlParameter("@user_level2", obj.user_level2),
                                                     new SqlParameter("@transfer_branch", obj.transfer_branch),
                                                     new SqlParameter("@remark", obj.remark),
                                                     new SqlParameter("@role_name1", obj.role_name1),
                                                     new SqlParameter("@role_name2", obj.role_name2),
                                                     new SqlParameter("@type", obj.type)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_USER_TRANSFER_SCHEDULE", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool NdUpdateUserTransfer(UserTransferScheduleModel obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@transfer_id", obj.Transfer_Id),
                                                     new SqlParameter("@emp_code", obj.emp_code),
                                                     new SqlParameter("@fname", obj.fname),
                                                     new SqlParameter("@lname", obj.lname),
                                                     new SqlParameter("@is_active", obj.is_active),
                                                     new SqlParameter("@sch_time", obj.sch_time),
                                                     new SqlParameter("@start_date", obj.start_date),
                                                     new SqlParameter("@end_date", obj.end_date),
                                                     new SqlParameter("@branch_code", obj.branch_code),
                                                     new SqlParameter("@branch_name", obj.branch_name),
                                                     new SqlParameter("@hub_code", obj.hub_code),
                                                     new SqlParameter("@hub_name", obj.hub_name),
                                                     new SqlParameter("@dept_code", obj.dept_code),
                                                     new SqlParameter("@dept_name", obj.dept_name),
                                                     new SqlParameter("@branch_code2", obj.branch_code2),
                                                     new SqlParameter("@branch_name2", obj.branch_name2),
                                                     new SqlParameter("@hub_code2", obj.hub_code2),
                                                     new SqlParameter("@hub_name2", obj.hub_name2),
                                                     new SqlParameter("@dept_code2", obj.dept_code2),
                                                     new SqlParameter("@dept_name2", obj.dept_name2),
                                                     new SqlParameter("@user_level", obj.user_level),
                                                     new SqlParameter("@user_level2", obj.user_level2),
                                                     new SqlParameter("@transfer_branch", obj.transfer_branch),
                                                     new SqlParameter("@remark", obj.remark),
                                                     new SqlParameter("@role_name1", obj.role_name1),
                                                     new SqlParameter("@role_name2", obj.role_name2),
                                                     new SqlParameter("@type", obj.type)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_UPDATE_USER_TRANSFER_SCHEDULE", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }



        #region Master data
        public static DataTable GetMasterRole()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT ROLE_LEVEL,ROLE_DEPARTMENT FROM TBL_ROLES ORDER BY ROLE_LEVEL", null,1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A6") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMasterUser()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT [USER_ID],USER_LOGIN FROM TBL_USERS", null,1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A7") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMasterGroupReport()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT GROUP_NO,GROUP_NAME FROM TBL_GROUP_REPORT", null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A8") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMasterGroupDetailReport()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT REPORT_NO,REPORT_NAME FROM TBL_GROUP_DETAIL_REPORT", null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A9") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }
        public static DataTable GetMasterGroupDetailReportByGroupNo(decimal? GroupNo)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT REPORT_NO,REPORT_NAME FROM TBL_GROUP_DETAIL_REPORT", null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@GROUP_NO", GroupNo) };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_GROUP_DETAIL_REPORTByGroupNo", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GetMasterGroupDetailReportByGroupNo2(decimal? GroupNo, string branchId)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@GROUP_NO", GroupNo),
                                                new SqlParameter("@ID_BRANCH", branchId) };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_GROUP_DETAIL_REPORTByGroupNo2", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GenGroupNo()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A21") };
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GenerateReportNo(decimal? GroupNo)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@GROUP_NO", GroupNo) };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GENERATE_REPORT_NO", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GetSystemBranch()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT BRANCH,[DESC] FROM Table_Branch", null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A24") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GetSystemHub()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT BRANCH,[DESC] FROM Table_Branch", null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A25") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GetSystemDistrict()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT BRANCH,[DESC] FROM Table_Branch", null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A26") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable LoadWorkFunction()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A30") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GetSystemDepartment()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT BRANCH,[DESC] FROM Table_Branch", null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A27") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GetMasterBranch()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                //dt = dbAccess.GetTbDataWithQSeluey("SELECT BRANCH,[DESC] FROM Table_Branch", null, 1);
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A10") };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable GenBranchNo()
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", "A23") };
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_APP", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        public static DataTable getMS(string _c, string _n, string _o)
        {
            DataTable dt = new DataTable();

            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@code", _c),
                                                new SqlParameter("@name", _n),
                                                new SqlParameter("@opt", _o)
                };
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GET_MS", sqlParameter, 1);

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
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@code", _c),
                                                new SqlParameter("@name", _n),
                                                new SqlParameter("@opt", _o)
                };
                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GET_MS", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }

            return dt;
        }

        #endregion

        #region Get Config

        public static DataTable GetUSER_ATTRIB(short NCFGVARIABLEID, string SCFGVARIABLE)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] sqlParameter = new SqlParameter[] {
                    new SqlParameter("@NCFGVARIABLEID", NCFGVARIABLEID)
                    ,new SqlParameter("@SCFGVARIABLE", SCFGVARIABLE) };

                DBAccess dbAccess = new DBAccess();

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_USER_ATTRIB", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        public static DataTable SearchUSER_ATTRIB(string SCFGVARIABLE, string Category)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] sqlParameter = new SqlParameter[] {
                    new SqlParameter("@SCFGVARIABLE", SCFGVARIABLE)
                    ,new SqlParameter("@Category", Category) };

                DBAccess dbAccess = new DBAccess();

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_SEARCH_USER_ATTRIB", sqlParameter, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }


        public static DataTable GetUSER_ATTRIB()
        {
            DataTable dt = new DataTable();
            try
            {


                DBAccess dbAccess = new DBAccess();

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_USER_ATTRIB_LIST", null, 1);

            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }
        public static bool AddUserAttrib(UserAttribModel obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@NCFGVARIABLEID", obj.NCFGVARIABLEID),
                                                     new SqlParameter("@SCFGVARIABLE", obj.SCFGVARIABLE),
                                                     new SqlParameter("@SCFGVALUE", obj.SCFGVALUE),
                                                     new SqlParameter("@SDESCRIPTION",obj.SDESCRIPTION),
                                                     new SqlParameter("@Category",obj.Category)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_USER_ATTRIB", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static DataTable GetAudiLogDetails1()
        {
            DataTable dt = new DataTable();
            long id = 0;
            try
            {
                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@Audi_Id", id),
                                                     new SqlParameter("@workfunction", ""),
                                                     new SqlParameter("@Username", ""),
                                                     new SqlParameter("@empcode", ""),
                                                     new SqlParameter("@created",""),
                                                     new SqlParameter("@update","")
                                                 };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_PROCESS_LOG", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        public static DataTable GetAudiLogDetails(AudiLogModel obj)
        {
            DataTable dt = new DataTable();
            try
            {
                DBAccess dbAccess = new DBAccess();
                string workfunction = (obj.WorkFunction != "All" && obj.WorkFunction != null) ? obj.WorkFunction.ToString() : "";
                string username = (obj.Username != "" && obj.Username != null) ? obj.Username.ToString() : "";
                string empcoe = (obj.EmpCode != "" && obj.EmpCode != null) ? obj.EmpCode.ToString() : "";
                string created = obj.frdate2 != null? obj.frdate2 : "" ;//obj.FrDate.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : obj.FrDate.ToString("yyyy-MM-dd");
                string updated = obj.todate2 != null ? obj.todate2 : "";//obj.ToDate.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : obj.ToDate.ToString("yyyy-MM-dd");
                if (obj.Audi_Id == 0)
                {

                }
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@Audi_Id", obj.Audi_Id),
                                                     new SqlParameter("@workfunction", workfunction),
                                                     new SqlParameter("@Username", username),
                                                     new SqlParameter("@empcode", empcoe),
                                                     new SqlParameter("@created",created),
                                                     new SqlParameter("@update",updated)
                                                 };

                dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_GET_PROCESS_LOG", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return dt;
        }

        public static bool EditUserAttrib(UserAttribModel obj)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@NCFGVARIABLEID", obj.NCFGVARIABLEID),
                                                     new SqlParameter("@SCFGVARIABLE", obj.SCFGVARIABLE),
                                                     new SqlParameter("@SCFGVALUE", obj.SCFGVALUE),
                                                     new SqlParameter("@SDESCRIPTION",obj.SDESCRIPTION)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_EDIT_USER_ATTRIB", sqlParameter, 1);
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

                DBAccess dbAccess = new DBAccess();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM TBL_USER_ATTRIB WHERE NCFGVARIABLEID = @NCFGVARIABLEID AND SCFGVARIABLE = @SCFGVARIABLE");
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@NCFGVARIABLEID", shtID), new SqlParameter("@SCFGVARIABLE", strVariable) };
                result = dbAccess.ExecuteQueryAddParameter(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool UpdateLoginLogout(string emp_code, string userid, int mode)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@MODE", mode),
                                                     new SqlParameter("@USERID", userid),
                                                     new SqlParameter("@LOGOUT_TIME", DateTime.Now),
                                                     new SqlParameter("@USER_EMP_CODE",emp_code)
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_UPDATE_USER_DETAILS", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
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

        //        DBAccess dbAccess = new DBAccess();
        //        result = Convert.ToInt32(dbAccess.GetTbDataWithQSeluey(strQuery, null, 1).Rows[0]["CountRow"]);

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

                DBAccess dbAccess = new DBAccess();
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@TYPEFN", typefn) };
                result = Convert.ToInt32(dbAccess.ExecuteStoreProceduerMoreOneParameters(strName, sqlParameter, 1).Rows[0]["CountRow"]);

            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static DataTable userExist(UserModels obj)
        {
            DBAccess dbAccess = new DBAccess();
            DataTable dt = new DataTable();
            string _sqlExist = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT * FROM TBL_USERS WHERE USER_LOGIN = @uname COLLATE Latin1_General_CS_AS OR USER_EMP_CODE = @USER_EMP_CODE ");
                //SELECT 1 FROM [dbo].[TBL_USERS] WHERE USER_LOGIN = @USER_LOGIN COLLATE Latin1_General_CS_AS OR USER_EMP_CODE = @USER_EMP_CODE
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                                    new SqlParameter("@uname", obj.USER_LOGIN),
                                                                    new SqlParameter("@USER_EMP_CODE", obj.USER_EMP_CODE)
                };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return dt;
        }

        public static DataTable Nd_userExist(UserModels obj)
        {
            DBAccess dbAccess = new DBAccess();
            DataTable dt = new DataTable();
            string _sqlExist = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT * FROM SYSTEMOFFICER_Details WHERE emp_code = @USER_EMP_CODE ");
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                                    new SqlParameter("@USER_EMP_CODE", obj.USER_EMP_CODE)
                };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return dt;
        }

        #endregion
    }
}