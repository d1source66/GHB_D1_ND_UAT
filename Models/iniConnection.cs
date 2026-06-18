using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class iniConnection
    {
        public string iniFile { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DBAMS { get; set; }
        public string DBTLF { get; set; }
        public string cryptoKey { get; set; }
        public string _strConnection { get; set; }
        public string _strConnection2 { get; set; }
        public string stp { get; set; }
        public string ivk { get; set; }
        public string rptPath { get; set; }
        public string filePath { get; set; }
        public string ldap1 { get; set; }
        public string ldap2 { get; set; }
        public string itldap1 { get; set; }
        public string itldap2 { get; set; }
        public string itldap3 { get; set; }
        public string itUrl { get; set; }
        public string authenUrl { get; set; }
        public string userldap1 { get; set; }
        public string codeUrl { get; set; }
        public string itldap1_v2 { get; set; }
        public string itldap2_v2 { get; set; }
        public string itldap3_v2 { get; set; }
        public string itUrl_v2 { get; set; }
        public string authenUrl_v2 { get; set; }
        public string userldap1_v2 { get; set; }
        public string codeUrl_v2 { get; set; }
    }
}