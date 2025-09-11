using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class GroupDetailReportModel
    {
        public int ID_REPORT { get; set; }

        public decimal? GROUP_NO { get; set; }

        public string GROUP_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal? REPORT_NO { get; set; }

        public string REPORT_NAME { get; set; }

        public bool? ACTIVE { get; set; }

        public DateTime? CREATE_DATE { get; set; }

        public DateTime? UPDATE_DATE { get; set; }

        public bool IsSelected { get; set; }
    }
}