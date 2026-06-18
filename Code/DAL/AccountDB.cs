using GHB_D1.Code.Util;
using GHB_D1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace GHB_D1.Code.DAL
{
    public class AccountDB
    {
        #region Private Variable
        DBAccess _objDBAcc = null;
        iniConnection _iniCon = null;
        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");                
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        private ASCIIEncoding textConverter = new ASCIIEncoding();
        private byte[] iv = null;
        #endregion

        public AccountDB()
        {
            _objDBAcc = new DBAccess();
            _iniCon = new iniConnection();
            _logSys = new Loger();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            _iniCon.cryptoKey = ModConf.ReadIni(_iniCon.iniFile, "appSetting", "cryptoKey");
            _iniCon.ivk = ModConf.ReadIni(_iniCon.iniFile, "appSetting", "ivk");
            iv = textConverter.GetBytes(_iniCon.ivk);//new byte[16];
        }

        public DataTable userExist(RegisterViewModel model)
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
                SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@uname", model.Username) };
                dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "fn_userExist : " + ex.Message.ToString());
            }
            return dt;
        }

        public static bool ValidatePassword(string password)
        {
            // Define the regex pattern for the password
            const string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";

            // Create a regex object
            Regex regex = new Regex(pattern);

            // Check if the password matches the pattern
            return regex.IsMatch(password);
        }

        public bool insertRegisDetail(RegisterViewModel model)
        {
            DBAccess dbAccess = new DBAccess();
            string _sqlIns = string.Empty;
            bool _result = false;
            DataTable dt = new DataTable();
            dt = userExist(model);
            try
            {
                if (dt.Rows.Count == 0)
                {


                    bool isValid = ValidatePassword(model.Password);

                    if (isValid)
                    {
                        var encryptpwd = AesUtil.EncryptString(_iniCon.cryptoKey, model.Password, iv);
                        //_sqlIns = " INSERT INTO TBL_USERS (USER_LOGIN, USER_PASSWORD, ADMIN_CREATE_DATE, USER_EMAIL) VALUES('" + model.Username + "','" + encryptpwd + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")) + "' , '" + model.Email + " ' )";
                        //_result = _objDBAcc.ExcuteDataWithQuery(_sqlIns, null,1);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("INSERT INTO TBL_USERS (USER_LOGIN, USER_PASSWORD, ADMIN_CREATE_DATE, USER_EMAIL, USER_FLAG) VALUES( @uname, @encrypted, @createDate, @uemail, @uflag)");
                        SqlParameter[] sqlParameter = new SqlParameter[] {
                            new SqlParameter("@uname", model.Username),
                            new SqlParameter("@encrypted", encryptpwd),
                            new SqlParameter("@createDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"))),
                            new SqlParameter("@uemail", model.Email),
                            new SqlParameter("@uflag", false)
                        };
                        //dt = dbAccess.ExecuteQueryMoreOneParameters(sb.ToString(), sqlParameter, 1);
                        _result = dbAccess.ExcuteDataWithQuery(sb.ToString(), sqlParameter, 1);
                    }
                    else
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "ValidatePassword : " + isValid);
                    }


                }
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "fn_insertRegisDetail : " + ex.Message.ToString());
            }
            return _result;
        }







//public bool insertRegisDetail(RegisterViewModel model)
//{
//    string _sqlIns = string.Empty;
//    bool _result = false;
//    DataTable dt = new DataTable();
//    dt = userExist(model);
//    try
//    {
//        if (dt.Rows.Count == 0)
//        {
//            var encryptpwd = AesUtil.EncryptString(_iniCon.cryptoKey, model.Password, iv);
//            _sqlIns = " INSERT INTO TBL_USERS (USER_LOGIN, USER_PASSWORD, ADMIN_CREATE_DATE, USER_EMAIL) VALUES('" + model.Username + "','" + encryptpwd + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")) + "' , '" + model.Email + " ' )";
//            _result = _objDBAcc.ExcuteDataWithQuery(_sqlIns, 1);
//        }
//    }
//    catch (Exception ex)
//    {
//        _logSys.WriteProcessLogFile(_strPathFile, "fn_insertRegisDetail : " + ex.Message.ToString());
//    }
//    return _result;
//}

public bool forgotPwd(ForgotPasswordViewModel model)
        {
            bool _result = false;
            string _sqlforgot = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                /*
                _sqlforgot = "SELECT * FROM TBL_USERS WHERE USER_EMAIL = '" + model.Email + "' ";
                dt = _objDBAcc.GetTbDataWithQSeluey(_sqlforgot, null,1);
                _logSys.WriteProcessLogFile(_strPathFile, "fn_insertRegisDetail : got Email ");

                if (dt.Rows.Count > 0)
                {
                    var getBackPwd = AesUtil.DecryptString(_iniCon.cryptoKey, dt.Rows[0]["USER_PASSWORD"].ToString(), iv);
                    #region formatter
                    model.Subject = "Request details for login";
                    //******************************************************************
                    StringBuilder sb = new StringBuilder();
                    sb.Append("  <html>  ");
                    sb.Append("  <head>  ");
                    sb.Append("  <style> ");
                    sb.Append("  body { ");
                    sb.Append(" font-size: 100%;  ");
                    sb.Append(" } ");
                    sb.Append(" h3.text { ");
                    sb.Append("  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;  ");
                    sb.Append(" font-size: 16px;  ");
                    sb.Append(" } ");
                    sb.Append(" p { ");
                    sb.Append(" font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;  ");
                    sb.Append(" font-size: 16px; ");
                    sb.Append(" font-weight: bold; ");
                    sb.Append(" } ");
                    sb.Append(" table,th,td { ");
                    sb.Append(" border: 1px solid black; ");
                    sb.Append(" border-collapse: collapse;");
                    sb.Append("   } ");
                    sb.Append(" </style> ");
                    sb.Append(" </head> ");

                    sb.Append(" <body> ");
                    sb.Append("   เรียน " + dt.Rows[0]["USER_LOGIN"].ToString());
                    sb.Append("   <br> ");
                    sb.Append("   <br> ");
                    sb.Append("   เนื่องจากท่านเข้าใช้งานระบบไม่ได้ และท่านได้ทำการขอรหัสเข้าใช้งานกับระบบ");
                    sb.Append("   <br> ");
                    sb.Append("   รหัสที่ท่านใช้เข้าระบบเป็นตามด้านล่างนี้");
                    sb.Append("   <br> ");
                    sb.Append("   " + getBackPwd);
                    sb.Append("  <div> ");
                    sb.Append(" </div> ");
                    sb.Append(" <br>");
                    sb.Append(" <br>");
                    sb.Append(" ขอแสดงความนับถือ");
                    sb.Append(" <br>");
                    sb.Append(" ");
                    sb.Append(" <br>");
                    sb.Append(" </body> ");
                    sb.Append(" </html> ");

                    string html = sb.ToString();
                    //string text = string.Format("Please click on this link to {0}: {1}", model.Subject, model.Body);
                    //string html = "Please confirm your account by clicking this link: <a href=\"" + model.Body + "\">link</a><br/>";

                    //html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + model.Body);
                    //******************************************************************
                    #endregion
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("d1source66@gmail.com");
                    msg.To.Add(new MailAddress(model.Email));
                    msg.Subject = model.Subject;
                    msg.Body = html;
                    msg.IsBodyHtml = true;
                    //msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                    //msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                    SmtpClient smtpClient = new SmtpClient();// new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = Convert.ToInt32(587);// Convert.ToInt16(SMTPPort);
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("d1source66@gmail.com", "fwcx ndrp fzbx goxg");
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = credentials;
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(msg);

                    _result = true;

                }
                */
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "fn_insertRegisDetail : " + ex.Message.ToString());
            }
            return _result;
        }
    }
}