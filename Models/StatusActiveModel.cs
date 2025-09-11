using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class StatusActiveModel
    {

        public int activeValue { get; set; }
        public string activeName { get; set; }
        //public List<SelectListItem> Options { get; set; } 
    }

    public class SelectListItemStatus
    {
        public int activeValue { get; set; }
        public string activeName { get; set; }
    }
}