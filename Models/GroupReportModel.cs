using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class GroupReportModel
    {
        public int ID { get; set; }
        public decimal? GroupCode { get; set; }
        public string GroupName { get; set; }
        public bool? Active { get; set; }
        public DateTime? Created {get;set;}
        public DateTime? Updated { get; set; }
        public bool IsSelected { get; set; }
    }
}