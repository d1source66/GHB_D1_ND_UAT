using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class RATMOnDemandViewModel
    {
        public string T_DATE { get; set; }

        public string REPORT_NAME { get; set; }

        public string SEARCH_KEY { get; set; }

        public string DISPLAY_FILTER { get; set; }

        public string TITLE_REPORT { get; set; }

        public string USER_ID { get; set; }

        public string BRANCH_ID { get; set; }

        public string MESSAGE { get; set; }

        public List<RATMOnDemandDetailViewModel> _ratmOnDemandDTVMList { get; set; }
    }

    public class RATMOnDemandDetailViewModel
    {
        public string REPORT_NAME { get; set; }
        public bool STATUS_DOWNLOAD { get; set; }
    }
}