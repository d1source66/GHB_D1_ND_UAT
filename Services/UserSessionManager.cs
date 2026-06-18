using GHB_D1.Code.DAL;
using GHB_D1.Code.Util;
using GHB_D1.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace GHB_D1.Services
{
    public class UserSessionManager
    {
        //private readonly string connectionString = ConfigurationManager.ConnectionStrings["strCon"].ConnectionString;
        private string connectionString = string.Empty;
        iniConnection _iniCon = null;
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;

        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        public UserSessionManager()
        {
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            connectionString = ModConf.ReadIni(_iniCon.iniFile, "DB", "conStr1");
            _logSys = new Loger();
        }
        public void HandleUserSession(long userId, string sessionId)
        {


            _logSys.WriteProcessLogFile(_strPathFile, " UserSessionManager --> HandleUserSession : line 28 " + userId);
            _logSys.WriteProcessLogFile(_strPathFile, " UserSessionManager --> HandleUserSession : line 29 " + sessionId);

            try {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "DELETE FROM UserSessions WHERE UserId = @UserId";
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.ExecuteNonQuery();

                        _logSys.WriteProcessLogFile(_strPathFile, " UserSessionManager --> ExecuteNonQuery : line 42 " + userId);
                        _logSys.WriteProcessLogFile(_strPathFile, " UserSessionManager --> INSERT INTO UserSessions | SessionId : line 48 " + sessionId);
                        _logSys.WriteProcessLogFile(_strPathFile, " UserSessionManager --> ExecuteNonQuery  : line 42 DateTime " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")));
                        command.CommandText = "INSERT INTO UserSessions (UserId, SessionId, LastActivityTime) VALUES (@InsertUserId, @InsertSessionId, @LastActivityTime)";
                        command.Parameters.AddWithValue("@InsertUserId", userId);
                        command.Parameters.AddWithValue("@InsertSessionId", sessionId);
                        command.Parameters.AddWithValue("@LastActivityTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")));
                        command.ExecuteNonQuery();                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, " UserSessionManager --> Exception ex : line 35 " + ex.Message.ToString());

            }
          
        }

        public void SignOutUser(long userId, string sessionId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM UserSessions WHERE UserId = @UserId AND SessionId = @SessionId";
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@SessionId", sessionId);
                    command.ExecuteNonQuery();
                }
            }
        }
        public string GetSessionId(long userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT SessionId FROM UserSessions WHERE UserId = @UserId";
                    command.Parameters.AddWithValue("@UserId", userId);
                    return command.ExecuteScalar() as string;
                }
            }
        }

        public string ChkandGetSessionId(long userId)
        {
            string r = string.Empty;
            DBAccess dbAccess = new DBAccess();
            try
            {
                SqlParameter[] sqlParameter = new SqlParameter[] {
                                                    new SqlParameter("@UserId", userId),
                                                    new SqlParameter("@datelogin", DateTime.Now.ToString("yyyy-MM-dd", new CultureInfo("en-US"))),
                                                 };
                r = dbAccess.ExecuteStoreProceduerAddParameter2("AMS_SP_CHK_USER_SESSION", sqlParameter, 1);                
            }
            catch(Exception ex)
            {
                _logSys.WriteProcessLogFile(_strPathFile, " ChkandGetSessionId --> Exception ex : " + ex.Message.ToString());
            }

            return r;
        }
    }
}