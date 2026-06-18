using GHB_D1.Models;
using GHB_D1.Code.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GHB_D1.Code.Util;

namespace GHB_D1.Code.BAL
{
    public class CommondBAL
    {
        CommondDAL _cdal = new CommondDAL();
        Loger _logSys = new Loger();
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");

        public List<ADMModel> GetADMDataByStoreBal(string branchId,string storeName, string rptDate,string termId)
        {
            List<ADMModel> _result = new List<ADMModel>();
            DataSet _ds = new DataSet();
            DataTable _dt;

            try
            {
                _dt = _cdal.GetADMDataByStore(branchId, storeName, rptDate,termId);

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow _dr in _dt.Rows)
                    {
                        ADMModel data = new ADMModel();
                        data.T_BRCH_ID = _dr["T_BRCH_ID"].ToString();
                        data.BRCH_NAME = _dr["BRCH_NAME"].ToString();
                        data.WSID = _dr["WSID"].ToString();
                        data.T_TERM_T_TERM_ID = _dr["T_TERM_T_TERM_ID"].ToString();
                        data.T_TERM_NAME_LOC = _dr["T_TERM_NAME_LOC"].ToString();
                        data.T_TRAN_DAT = _dr["T_TRAN_DAT"].ToString();
                        data.T_TRAN_TIM = _dr["T_TRAN_TIM"].ToString();
                        data.T_CRD_T_PAN = _dr["T_CRD_T_PAN"].ToString();
                        data.T_T_CDE = _dr["T_T_CDE"].ToString();
                        data.T_T_FROM = _dr["T_T_FROM"].ToString();
                        data.T_T_TO = _dr["T_T_TO"].ToString();
                        data.T_FROM_ACCT = _dr["T_FROM_ACCT"].ToString();
                        data.T_TO_ACCT = _dr["T_TO_ACCT"].ToString();
                        data.T_MULT_ACCT = _dr["T_MULT_ACCT"].ToString();
                        data.T_DEP_TYP = _dr["T_DEP_TYP"].ToString();
                        data.REQ_AMT = _dr["REQ_AMT"].ToString();
                        data.ACT_AMT = _dr["ACT_AMT"].ToString();
                        data.FEE_AMT = _dr["FEE_AMT"].ToString();
                        data.RV = _dr["RV"].ToString();
                        data.RP = _dr["RP"].ToString();
                        data.T_SEQ_NUM = _dr["T_SEQ_NUM"].ToString();
                        data.BILL_CNT = _dr["BILL_CNT"].ToString();
                        data.BILL_CNT_CDM = _dr["BILL_CNT_CDM"].ToString();
                        data.T_CRD_T_FIID = _dr["T_CRD_T_FIID"].ToString();
                        data.T_RESP_BYTE_2 = _dr["T_RESP_BYTE_2"].ToString();
                        data.FLAG_REVERSE = _dr["FLAG_REVERSE"].ToString();
                        data.BK_C = _dr["BK_C"].ToString();
                        data.CNT = _dr["CNT"].ToString();
                        data.ITEM05 = _dr["ITEM05"].ToString();
                        data.HOPR_END = _dr["05HOPR_END"].ToString();//05HOPR_END
                        data.TERM_ID05 = _dr["TERM_ID"].ToString();
                        data.ITEM03 = _dr["ITEM03"].ToString();
                        data.HOPR1_INCR = _dr["HOPR1_INCR"].ToString();
                        data.HOPR2_INCR = _dr["HOPR2_INCR"].ToString();
                        data.HOPR3_INCR = _dr["HOPR3_INCR"].ToString();
                        data.HOPR4_INCR = _dr["HOPR4_INCR"].ToString();
                        data.TERM_ID03 = _dr["TERM_ID"].ToString();
                        data.ITEM07 = _dr["ITEM07"].ToString();
                        data.HOPR1_DECR = _dr["HOPR1_DECR"].ToString();
                        data.HOPR2_DECR = _dr["HOPR2_DECR"].ToString();
                        data.HOPR3_DECR = _dr["HOPR3_DECR"].ToString();
                        data.HOPR4_DECR = _dr["HOPR4_DECR"].ToString();
                        data.TERM_ID07 = _dr["TERM_ID"].ToString();
                        data.ITEM09 = _dr["ITEM09"].ToString();
                        data.HOPR1_END = _dr["HOPR1_END"].ToString();
                        data.HOPR2_END = _dr["HOPR2_END"].ToString();
                        data.HOPR3_END = _dr["HOPR3_END"].ToString();
                        data.HOPR4_END = _dr["HOPR4_END"].ToString();
                        data.TERM_ID09 = _dr["TERM_ID09"].ToString();
                        data.spSP = _dr["spSP"].ToString();
                        data.spFL = _dr["spFL"].ToString();
                        _result.Add(data);

                    }
                }
                _logSys.WriteProcessLogFile(_strPathFile, "CommondBAL.cs_fn_GetADMDataByStoreBal : Success1_Store Name" + storeName);
            }
            catch(Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "CommondBAL.cs_fn_GetADMDataByStoreBal : _Store Name" + storeName + "Message Detail : " + ex.Message.ToString());
            }
            _logSys.WriteProcessLogFile(_strPathFile, "CommondBAL.cs_fn_GetADMDataByStoreBal : Success2_Store Name" + storeName);

            return _result;
        }

        public List<HomeModel> GetHomeDataByStoreBal(string branchID,string storeName, string rptDate,string termId)
        {
            List<HomeModel> _result = new List<HomeModel>();
            DataSet _ds = new DataSet();
            DataTable _dt;

            try
            {
                branchID = (branchID != null && branchID != "") ? branchID.PadLeft(4,'0') :"0001";
                _dt = _cdal.GetADMDataByStore(branchID, storeName, rptDate, termId);

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow _dr in _dt.Rows)
                    {
                        HomeModel data = new HomeModel();
                        data.T_BRCH_ID = _dr["T_BRCH_ID"].ToString();
                        data.BRCH_NAME = _dr["BRCH_NAME"].ToString();
                        data.WSID = _dr["WSID"].ToString();
                        data.T_TERM_T_TERM_ID = _dr["T_TERM_T_TERM_ID"].ToString();
                        data.T_TERM_NAME_LOC = _dr["T_TERM_NAME_LOC"].ToString();
                        data.T_TRAN_DAT = _dr["T_TRAN_DAT"].ToString();
                        data.T_TRAN_TIM = _dr["T_TRAN_TIM"].ToString();
                        data.T_CRD_T_PAN = _dr["T_CRD_T_PAN"].ToString();
                        data.T_T_CDE = _dr["T_T_CDE"].ToString();
                        data.T_T_FROM = _dr["T_T_FROM"].ToString();
                        data.T_T_TO = _dr["T_T_TO"].ToString();
                        data.T_FROM_ACCT = _dr["T_FROM_ACCT"].ToString();
                        data.T_TO_ACCT = _dr["T_TO_ACCT"].ToString();
                        data.T_MULT_ACCT = _dr["T_MULT_ACCT"].ToString();
                        data.T_DEP_TYP = _dr["T_DEP_TYP"].ToString();
                        data.REQ_AMT = _dr["REQ_AMT"].ToString();
                        data.ACT_AMT = _dr["ACT_AMT"].ToString();
                        data.FEE_AMT = _dr["FEE_AMT"].ToString();
                        data.RV = _dr["RV"].ToString();
                        data.RP = _dr["RP"].ToString();
                        data.T_SEQ_NUM = _dr["T_SEQ_NUM"].ToString();
                        data.BILL_CNT = _dr["BILL_CNT"].ToString();
                        data.BILL_CNT_CDM = _dr["BILL_CNT_CDM"].ToString();
                        data.T_CRD_T_FIID = _dr["T_CRD_T_FIID"].ToString();
                        data.T_RESP_BYTE_2 = _dr["T_RESP_BYTE_2"].ToString();
                        data.FLAG_REVERSE = _dr["FLAG_REVERSE"].ToString();
                        data.BK_C = _dr["BK_C"].ToString();
                        data.CNT = _dr["CNT"].ToString();
                        data.ITEM05 = _dr["ITEM05"].ToString();
                        data.HOPR_END = _dr["05HOPR_END"].ToString();//05HOPR_END
                        data.TERM_ID05 = _dr["TERM_ID"].ToString();
                        data.ITEM03 = _dr["ITEM03"].ToString();
                        data.HOPR1_INCR = _dr["HOPR1_INCR"].ToString();
                        data.HOPR2_INCR = _dr["HOPR2_INCR"].ToString();
                        data.HOPR3_INCR = _dr["HOPR3_INCR"].ToString();
                        data.HOPR4_INCR = _dr["HOPR4_INCR"].ToString();
                        data.TERM_ID03 = _dr["TERM_ID"].ToString();
                        data.ITEM07 = _dr["ITEM07"].ToString();
                        data.HOPR1_DECR = _dr["HOPR1_DECR"].ToString();
                        data.HOPR2_DECR = _dr["HOPR2_DECR"].ToString();
                        data.HOPR3_DECR = _dr["HOPR3_DECR"].ToString();
                        data.HOPR4_DECR = _dr["HOPR4_DECR"].ToString();
                        data.TERM_ID07 = _dr["TERM_ID"].ToString();
                        data.ITEM09 = _dr["ITEM09"].ToString();
                        data.HOPR1_END = _dr["HOPR1_END"].ToString();
                        data.HOPR2_END = _dr["HOPR2_END"].ToString();
                        data.HOPR3_END = _dr["HOPR3_END"].ToString();
                        data.HOPR4_END = _dr["HOPR4_END"].ToString();
                        data.TERM_ID09 = _dr["TERM_ID09"].ToString();
                        data.spSP = _dr["spSP"].ToString();
                        data.spFL = _dr["spFL"].ToString();
                        _result.Add(data);

                    }
                }
                _logSys.WriteProcessLogFile(_strPathFile, "fn_GetADMDataByStoreBal : Success");
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "fn_GetADMDataByStoreBal : " + ex.Message.ToString());
            }


            return _result;
        }
    }
}