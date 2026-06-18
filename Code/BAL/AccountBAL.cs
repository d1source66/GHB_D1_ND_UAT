using GHB_D1.Code.DAL;
using GHB_D1.Code.Util;
using GHB_D1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHB_D1.Code.BAL
{
    public class AccountBAL
    {
        #region Private Variable
        DBAccess _objDBAcc = null;
        Loger _logSys = null;
        AccountDB _acc = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        #endregion

        public AccountBAL()
        {
            _objDBAcc = new DBAccess();
            _logSys = new Loger();
            _acc = new AccountDB();
        }

        public bool insertRegisDetailBiz(RegisterViewModel model)
        {
            bool _result = false;
            DataTable dt = new DataTable();
            _logSys.WriteProcessLogFile(_strPathFile, "fn_insertReg");
            if (model != null)
            {
                _result = _acc.insertRegisDetail(model);
                _logSys.WriteProcessLogFile(_strPathFile, "fn_insertRegisDetailBiz : " + _result);

            }

            return _result;
        }

        public bool forgotPwdBiz(ForgotPasswordViewModel model)
        {
            bool _result = false;
            try
            {
                if (null != model)
                {
                    _result = _acc.forgotPwd(model);
                    _logSys.WriteProcessLogFile(_strPathFile, "fn_forgotPwd : " + _result);
                }
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "fn_forgotPwd : " + ex.Message.ToString());
            }

            return _result;
        }
    }
}