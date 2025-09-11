using GHB_D1.Code.DAL;
using GHB_D1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GHB_D1.Code.BAL
{
    public class GroupReportBAL
    {
        public static List<GroupDetailReportViewModel> GetGroupDetailReport(string branchId,string UserId,string strGroup_no,string strSearch)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();

            DBAccess dbAccess = new DBAccess();

            DataTable dt = new DataTable();
            /*
            string strQuery = "";

            SqlParameter[] parameters = new SqlParameter[3];

            strQuery = " SELECT t1.GROUP_NAME ,t2.REPORT_NO,t2.REPORT_NAME  FROM [TBL_GROUP_REPORT] t1 LEFT JOIN [TBL_GROUP_DETAIL_REPORT] t2 ON t1.GROUP_NO = t2.GROUP_NO WHERE t1.ACTIVE = @ACTIVE AND  t2.ACTIVE = @ACTIVE ";
            int intActive = 1;
            
            parameters[0] = new SqlParameter("@ACTIVE", intActive);
            if (strGroup_no != null && strGroup_no != "")
            {
                strQuery += " AND t2.[GROUP_NO]=@GROUP_NO";

                parameters[1] = new SqlParameter("@GROUP_NO", strGroup_no);
            }
            else {
                parameters[1] = new SqlParameter("@GROUP_NO", "");
            }       
          

            if (strSearch != null && strSearch != "" && strSearch != "%") {
                strQuery += " AND t2.REPORT_NAME LIKE '%@REPORT_NAME%'";
                
                parameters[2] = new SqlParameter("@REPORT_NAME", strSearch);
            }
            else
            {
                parameters[2] = new SqlParameter("@REPORT_NAME", "");
            }

            dt = dbAccess.GetTbDataWithQSeluey(strQuery, parameters, 1);
            */
            SqlParameter[] sqlParameter = new SqlParameter[] {
                                                new SqlParameter("@ID_BRANCH", branchId),
                                                new SqlParameter("@ID_USER", UserId),                                                 
                                                new SqlParameter("@GROUP_NO", strGroup_no),
                                                new SqlParameter("@REPORT_NAME", strSearch) };
            //dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GetGroupDetailReport_v1", sqlParameter, 1);
            dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GetGroupDetailReport_add_branch_v1", sqlParameter, 1);

            if (dt.Rows.Count > 0)
            {
                int intID = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    GroupDetailReportViewModel obj = new GroupDetailReportViewModel();
                    obj.ID = intID;
                    obj.GROUP_NAME = dr["GROUP_NAME"].ToString();
                    obj.REPORT_NO = dr["REPORT_NO"].ToString();
                    obj.REPORT_NAME = dr["REPORT_NAME"].ToString();
                    list.Add(obj);
                    intID++;
                }
            }

            return list;
        }

        public static List<GroupDetailReportViewModel> GetGroupDetailReport2(string roleName, string strGroup_no)
        {
            List<GroupDetailReportViewModel> list = new List<GroupDetailReportViewModel>();
            DBAccess dbAccess = new DBAccess();
            DataTable dt = new DataTable();
            SqlParameter[] sqlParameter = new SqlParameter[] {
                                                //new SqlParameter("@ID_BRANCH", branchId),
                                                //new SqlParameter("@ID_USER", UserId),
                                                new SqlParameter("@Role_Name", roleName),
                                                new SqlParameter("@Group_No", strGroup_no) 
            };

            dt = dbAccess.ExecuteStoreProceduerMoreOneParameters("AMS_SP_MAIN_GetGroupDetailReport_add_branch_v2", sqlParameter, 1);

            if (dt.Rows.Count > 0)
            {
                int intID = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    GroupDetailReportViewModel obj = new GroupDetailReportViewModel();
                    obj.ID = intID;
                    obj.GROUP_NAME = dr["GROUP_NAME"].ToString();
                    obj.REPORT_NO = dr["REPORT_NO"].ToString();
                    obj.REPORT_NAME = dr["REPORT_NAME"].ToString();
                    list.Add(obj);
                    intID++;
                }
            }

            return list;
        }
    }
}