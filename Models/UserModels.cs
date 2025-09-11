using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class UserModels
    {
        public long USER_ID { get; set; }
        public string USER_LOGIN { get; set; }
        public string USER_PASSWORD { get; set; }
        public DateTime USER_PASSWORD_UPDATE_TIME { get; set; }
        public string USER_FIRSTNAME { get; set; }
        public string USER_LASTNAME { get; set; }
        public string USER_DEPARTMENT { get; set; }
        public string USER_POSITION { get; set; }
        public int USER_LEVEL { get; set; }
        public bool USER_LOGIN_FLAG { get; set; }
        public string USER_FONT_NAME { get; set; }
        public short USER_FONT_SIZE { get; set; }
        public bool USER_FONT_BOLD { get; set; }
        public bool USER_FONT_ITALIC { get; set; }
        public bool USER_RUN_OUTSIDEVIEW { get; set; }
        public string USER_DEFAULT_PAGE { get; set; }
        public string USER_CONDITION { get; set; }
        public DateTime USER_UPDATE_TIME { get; set; }
        public DateTime USER_LAST_LOGON { get; set; }
        public DateTime USER_LAST_LOGOFF { get; set; }
        public string ADMIN_LAST_MA { get; set; }
        public string ADMIN_BY { get; set; }
        public DateTime ADMIN_CREATE_DATE { get; set; }
        public string ADMIN_CREATE_BY { get; set; }
        public int USER_WRONG_PWD { get; set; }
        public string USER_EMAIL { get; set; }
        public bool IsSelected { get; set; }
        public bool USER_FLAG { get; set; }

        public bool USER_LOCK { get; set; }


        public string User_Role_Level { get; set; }
        public string User_Branch { get; set; }
        public string USER_EMP_CODE { get; set; }
        public string SOL_CODE { get; set; }
        public string SOL_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string IS_ACTIVE2 { get; set; }

        //เขต
        public string Hub_Code { get; set; }
        public string Hub_Name { get; set; }
        //ฝ่าย
        public string Dept_Code { get; set; }
        public string Dept_Name { get; set; }
        public string Remark { get; set; }
        public string Role_Name { get; set; }


    }
}