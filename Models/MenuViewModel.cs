using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
            _groupReportVMList = new List<GroupReportViewModel>();
        }

        public string BRANCH_ID { get; set; }

        public string BRANCH_NAME { get; set; }
        public string USERNAME  { get; set; }
        public string USER_ID { get; set; }

        public string FULLNAME { get; set; }

        public string T_DATE { get; set; }
        public string F_DATE { get; set; }

        public string SEARCH_KEY { get; set; }

        public string DISPLAY_FILTER { get; set; }

        public string TITLE_REPORT { get; set; }

        public string MESSAGE { get; set; }

        public bool DownloadCompleted { get; set; }
        public List<GroupReportViewModel> _groupReportVMList { get; set; }

        public List<GroupDetailReportViewModel> _groupDetailReportVMList { get; set; }
        public List<string> files { get; set; }
        public groupRole roleList { get; set; }
    }
}