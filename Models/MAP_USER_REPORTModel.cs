using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class MAP_USER_REPORTModel
    {
        public long ID_MENU { get; set; }
        //<ID_USER, bigint,>
        public long? ID_USER { get; set; }
        public string UserName { get; set; }
        //,<GROUP_NO, numeric(18,2),>
        public decimal? GROUP_NO { get; set; }
        public string GroupName { get; set; }
        //,<REPORT_NO, numeric(18,2),>
        public decimal? REPORT_NO { get; set; }
        public string ReportName { get; set; }
        //,<ID_BRANCH, nvarchar(5),>
        public string ID_BRANCH { get; set; }
        public string BranchName { get; set; }
        //,<CREATE_DATE, datetime,>
        public DateTime? CREATE_DATE { get; set; }
        //,<UPDATE_DATE, datetime,>
        public DateTime? UPDATE_DATE { get; set; }
        public bool IsSelected { get; set; }
    }
}