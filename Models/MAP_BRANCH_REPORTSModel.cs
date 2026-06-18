using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class MAP_BRANCH_REPORTSModel
    {
        public long ID { get; set; }
        //<ID_BRANCH, nvarchar(5),>
        public string ID_BRANCH { get; set; }
        public string BRANCH_NAME { get; set; }
        //,<GROUP_NO, numeric(18,2),>
        public decimal? GROUP_NO { get; set; }
        public string GROUP_NAME { get; set; }
        //,<REPORT_NO, numeric(18,2),>
        public decimal? REPORT_NO { get; set; }
        public string REPORT_NAME { get; set; }
        //,<CREATE_DATE, datetime,>
        public DateTime CREATE_DATE { get; set; }
        //,<UPDATE_DATE, datetime,>
        public DateTime UPDATE_DATE { get; set; }
        public bool IsSelected { get; set; }
    }
}