using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class ORPViewModel
    {
        public string BRANCH_ID { get; set; }
        public string USER_ID { get; set; }
        public string T_DATE { get; set; }
        public string F_DATE { get; set; }
        public string ToDate { get; set; }
        public string FrDate { get; set; }
        public string REPORT_NAME { get; set; }

        public string SEARCH_KEY { get; set; }

        public string DISPLAY_FILTER { get; set; }

        public string TITLE_REPORT { get; set; }

        public string MESSAGE { get; set; }
    }
}