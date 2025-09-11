using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class CRMOnDemandViewModel
    {
        public string T_DATE { get; set; }

        public string REPORT_NAME { get; set; }

        public string SEARCH_KEY { get; set; }

        public string DISPLAY_FILTER { get; set; }

        public string TITLE_REPORT { get; set; }

        public string USER_ID { get; set; }

        public string BRANCH_ID { get; set; }

        public string MESSAGE { get; set; }

        public string px { get; set; }

        public string py { get; set; }

        public string pz { get; set; }

        public List<LRMOnDemandDetailViewModel> _lrmOnDemandDTVMList { get; set; }
    }

    public class CRMOnDemandDetailViewModel
    {
        public string REPORT_NAME { get; set; }
        public bool STATUS_DOWNLOAD { get; set; }
    }
}