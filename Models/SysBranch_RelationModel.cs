using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class SysBranch_RelationModel
    {
        public string brand_code { get; set; }
        public string department_code { get; set; }
        public string hub_code { get; set; }
        public string brand_name { get; set; }
        public string department_name { get; set; }
        public string hub_name { get; set; }
        public DateTime? last_update { get; set; }
    }
}