using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class MAP_USER_BRANCHModel
    {
        public long ID { get; set; }
        //<ID_USER, bigint,>
        public long ID_USER { get; set; }
        public string USER_NAME { get; set; }
        //,<ID_BRANCH, nvarchar(5),>
        public string ID_BRANCH { get; set; }
        public string BRANCH_NAME { get; set; }
        //,<CREATE_DATE, datetime,>
        public DateTime CREATE_DATE { get; set; }
        //,<UPDATE_DATE, datetime,>
        public DateTime UPDATE_DATE { get; set; }
        public bool IsSelected { get; set; }

        public string ID_BRANCH2 { get; set; }
        public string B_DATE { get; set; }
        public string E_DATE { get; set; }
        public bool PERMA { get; set; }
        public bool OPEN_OPT { get; set; }
    }
}