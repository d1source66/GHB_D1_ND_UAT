using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class ReportModel
    {
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal? REPORT_NO { get; set; }
        public string REPORT_NAME { get; set; }
        public bool IsSelected { get; set; }
    }
}