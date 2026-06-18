using GHB_D1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GHB_D1.Code.DAL
{
    public class ProcessLogDAL
    {
        public static bool AddProcessLog(string strUsername, string strMessage)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@Username", strUsername),
                                                     new SqlParameter("@Message", strMessage),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_PROCESS_LOG", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        public static bool AddProcessLog2(AudiLogModel audilog)
        {
            bool result = false;
            try
            {
                DBAccess dbAccess = new DBAccess();

                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@Username", audilog.Username),
                                                     new SqlParameter("@Message", audilog.Message),
                                                     new SqlParameter("@WorkFunction", audilog.WorkFunction),
                                                     new SqlParameter("@Details01", audilog.Details01 == null ? "-":audilog.Details01),
                                                     new SqlParameter("@Details02", audilog.Details02 == null ? "-":audilog.Details02),
                                                     new SqlParameter("@EmpCode", audilog.EmpCode),
                                                     new SqlParameter("@EmpName", audilog.EmpName),
                                                     new SqlParameter("@MAC_Address", audilog.MAC_Address),
                                                     new SqlParameter("@OS", audilog.OS),
                                                     new SqlParameter("@URL", audilog.URL),
                                                     new SqlParameter("@Device", audilog.Device),
                                                     new SqlParameter("@IP_Address", audilog.IP_Address),
                                                     new SqlParameter("@Latitude", audilog.Latitude),
                                                     new SqlParameter("@Longitude", audilog.Longitude),
                                                 };

                result = dbAccess.ExecuteStoreProceduerAddParameter("AMS_SP_ADD_PROCESS_LOG", sqlParameter, 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }
    }
}