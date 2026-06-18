using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class GroupReportViewModel
    {

        public GroupReportViewModel() {
            _groupDetailVMList = new List<GroupDetailReportViewModel>();
        }

        public string GROUP_NO { get; set; }
        public string GROUP_NAME { get; set; }

        public List<GroupDetailReportViewModel> _groupDetailVMList { get; set; }
        public bool IsSelected { get; set; }
    }


    public class GroupDetailReportViewModel { 
        public int ID { get; set; }
        public string GROUP_NAME { get; set; }
        public string REPORT_NO { get; set; }
        public string REPORT_NAME { get; set; }
        public string T_TERM_T_TERM_ID { get; set; }
        public string BRANCH_NAME { get; set; }
    }
}