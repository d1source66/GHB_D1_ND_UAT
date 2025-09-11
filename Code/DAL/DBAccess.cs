using GHB_D1.Code.Util;
using GHB_D1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace GHB_D1.Code.DAL
{
    public class DBAccess
    {
        Loger _logSys = new Loger();
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        private string _strConnection = string.Empty;
        private string _strConnection2 = string.Empty;
        iniConnection _iniCon = null;
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;


        public DBAccess()
        {
            //_strConnection = WebConfigurationManager.ConnectionStrings["strCon"].ToString();
            //_strConnection2 = WebConfigurationManager.ConnectionStrings["strCon2"].ToString();
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            _strConnection = ModConf.ReadIni(_iniCon.iniFile, "DB", "conStr1");
            _strConnection2 = ModConf.ReadIni(_iniCon.iniFile, "DB", "conStr2");
        }
        public bool ExcuteDataWithQuery(string psql, SqlParameter[] pSPParams, int intConnection)
        {
            DataTable _dt = new DataTable();
            bool _result = false;
            SqlConnection _con = null;
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }

                if (_con.State != ConnectionState.Open) _con.Open();
                SqlCommand _cmd = new SqlCommand(psql, _con);
                _cmd.CommandType = CommandType.Text;
                _cmd.CommandTimeout = 1000;
                if (pSPParams != null)
                    _cmd.Parameters.AddRange(pSPParams);
                _cmd.ExecuteNonQuery();
                _result = true;
            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _result;
        }

        /*
        public DataTable GetTbDataWithQSeluey(string query, SqlParameter[] pSPParams, int intConnection)
        {
            DataTable _dt = new DataTable();
            SqlConnection _con = null;
            bool i = IsSafeSqlQuery(query);
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }


                if (_con.State != ConnectionState.Open) _con.Open();
                if (i == true)
                {
                    using (SqlCommand _cmd = new SqlCommand(query, _con))
                    {
                        _cmd.CommandType = CommandType.Text;
                        _cmd.CommandTimeout = 1000;
                        if (pSPParams != null)
                        {
                            _cmd.Parameters.AddRange(pSPParams);
                        }
                        SqlDataAdapter _adp = new SqlDataAdapter(_cmd);
                        _adp.Fill(_dt);
                    }
                }
                //SqlC
                //SqlCommand _cmd = new SqlCommand(sql, _con);
                //_cmd.CommandType = CommandType.Text;
                //_cmd.CommandTimeout = 1000;
                //if (pSPParams != null)
                //    _cmd.Parameters.AddRange(pSPParams);
                //SqlDataAdapter _adp = new SqlDataAdapter(_cmd);
                //_adp.Fill(_dt);
            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _dt;
        }
        */
        public DataTable ExecuteStoreProceduerMoreOneParameters(string storename, SqlParameter[] pSPParams, int intConnection)
        {
            DataTable _result = new DataTable();
            SqlConnection _con = null;
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }

                if (_con.State != ConnectionState.Open) _con.Open();
                SqlCommand cmd = new SqlCommand(storename, _con);

                cmd.CommandType = CommandType.StoredProcedure;
                if (pSPParams != null)
                    cmd.Parameters.AddRange(pSPParams);
                cmd.CommandTimeout = 1000;

                SqlDataAdapter _adp = new SqlDataAdapter(cmd);
                _adp.Fill(_result);

            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _result;
        }
        public DataTable ExecuteQueryMoreOneParameters(string strSQL, SqlParameter[] pSPParams, int intConnection)
        {
            DataTable _result = new DataTable();
            SqlConnection _con = null;
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }

                if (_con.State != ConnectionState.Open) _con.Open();
                SqlCommand cmd = new SqlCommand(strSQL, _con);

                cmd.CommandType = CommandType.Text;
                if (pSPParams != null)
                    cmd.Parameters.AddRange(pSPParams);
                cmd.CommandTimeout = 1000;

                SqlDataAdapter _adp = new SqlDataAdapter(cmd);
                _adp.Fill(_result);

            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _result;
        }
        

        

        public bool ExecuteStoreProceduerOneParameter(string storename, string paramValue, int intConnection)
        {
            bool _result = false;
            SqlConnection _con = null;
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }

                if (_con.State != ConnectionState.Open) _con.Open();
                SqlCommand cmd = new SqlCommand(storename, _con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;

                cmd.Parameters.Add(new SqlParameter("@tablename", paramValue));

                cmd.ExecuteNonQuery();
                _result = true;
            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _result;
        }
        public bool ExecuteStoreProceduerAddParameter(string storename, SqlParameter[] pSPParams, int intConnection)
        {
            bool _result = false;
            SqlConnection _con = null;
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }

                if (_con.State != ConnectionState.Open) _con.Open();
                SqlCommand cmd = new SqlCommand(storename, _con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;

                cmd.Parameters.AddRange(pSPParams);

                cmd.ExecuteNonQuery();
                _result = true;
            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _result;
        }
        public bool ExecuteQueryAddParameter(string strSQL, SqlParameter[] pSPParams, int intConnection)
        {
            bool _result = false;
            SqlConnection _con = null;
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }

                if (_con.State != ConnectionState.Open) _con.Open();
                SqlCommand cmd = new SqlCommand(strSQL, _con);

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 1000;

                cmd.Parameters.AddRange(pSPParams);

                cmd.ExecuteNonQuery();
                _result = true;
            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _result;
        }

        public bool ExecuteQueryMoreOneParametersBool(string strSQL, SqlParameter[] pSPParams, int intConnection)
        {
            bool _result = false;
            SqlConnection _con = null;
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }

                if (_con.State != ConnectionState.Open) _con.Open();
                SqlCommand cmd = new SqlCommand(strSQL, _con);

                cmd.CommandType = CommandType.Text;
                if (pSPParams != null)
                    cmd.Parameters.AddRange(pSPParams);
                cmd.CommandTimeout = 1000;

                

                cmd.ExecuteNonQuery();
                _result = true;
            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _result;
        }

        public static bool IsSafeSqlQuery(string query)
        {
            // Common SQL injection patterns to check
            string[] sqlInjectionPatterns = {
            "--", ";--", ";", "/*", "*/", "@@",
            "char", "nchar", "varchar", "nvarchar",
            "alter", "begin", "cast", "cursor", "declare",
            "drop", "end", "exec", "execute", "fetch", "insert", "kill",
             "sys", "sysobjects", "syscolumns"
        };

            // Check if the query contains any SQL injection patterns
            foreach (string pattern in sqlInjectionPatterns)
            {
                if (query.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public string ExecuteStoreProceduerAddParameter2(string storename, SqlParameter[] pSPParams, int intConnection)
        {
            string _result = string.Empty;
            SqlConnection _con = null;
            try
            {
                switch (intConnection)
                {
                    case 1:
                        _con = new SqlConnection(_strConnection);
                        break;
                    case 2:
                        _con = new SqlConnection(_strConnection2);
                        break;
                }

                if (_con.State != ConnectionState.Open) _con.Open();
                SqlCommand cmd = new SqlCommand(storename, _con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;
                cmd.Parameters.AddRange(pSPParams);

                cmd.ExecuteNonQuery();
                _result = cmd.ExecuteScalar() as string;                
            }
            catch (Exception ex)
            {
                _logSys.WriteErrLog(_strPathFile, "ExecuteStoreProceduerAddParameter2 SQL Exec Error: " + ex.Message.ToString());
            }
            finally { if (_con.State != ConnectionState.Closed) _con.Close(); }
            return _result;
        }

        public bool CopyToDataSource(ApiPostResponse2 ldapUser, DataTable _dt, string tableName, int intConnection)
        {
            SqlConnection _con = null;
            switch (intConnection)
            {
                case 1:
                    _con = new SqlConnection(_strConnection);
                    break;
                case 2:
                    _con = new SqlConnection(_strConnection2);
                    break;
            }
            if (_con.State == ConnectionState.Closed) _con.Open();

            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_con))
                {
                    bulkCopy.DestinationTableName = tableName;
                    try
                    {
                        bulkCopy.WriteToServer(_dt);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.Message);
                        ex.Message.ToString();
                        return false;
                    }
                }
                //return true;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }
        }

    }
}