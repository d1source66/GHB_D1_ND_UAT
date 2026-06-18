using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using GHB_D1.Code.Util;
using GHB_D1.Models;

namespace GHB_D1.Code.DAL
{
    public class CommondDAL
    {
        DBAccess _objDBAcc = new DBAccess();
        Loger _logSys = new Loger();
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");

        public DataTable GetADMDataByStore(string branchId,string storeName, string rptDate,string termId)
        {
            DataTable _result = new DataTable();
            //termId = (termId != "" && termId != null) ? termId : "%";
            try
            {
                SqlParameter[] SqlParameter = new SqlParameter[] {
                                                     new SqlParameter("@T_DATE", rptDate)
                                                     //new SqlParameter("@TERM_ID", termId)

                                                    };//return

                //string readStoreName = storeName + "_4";
                _result = _objDBAcc.ExecuteStoreProceduerMoreOneParameters(storeName, SqlParameter, 2);
            }
            catch (Exception ex)
            {             
                _logSys.WriteProcessLogFile(_strPathFile, "CommondDAL.GetADMDataByStore : BranchId : "+ branchId + "_Store Name" + storeName + "Message Detail : " + ex.Message.ToString());
            }
            return _result;
        }
    }
}