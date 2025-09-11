using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class RoleModuleMasterModel
    {
        public long Id { get; set; }
        public string Module_No { get; set; }

        [DisplayFormat(DataFormatString = "{0:N5}")]
        public decimal? Group_Module { get; set; }
        public string Module_Name { get; set; }
        public string Description { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? Update_Date { get; set; }
        public bool IsSelected { get; set; }
        public bool List_Role { get; set; }
        public bool Create_Role { get; set; }
        public bool View_Role { get; set; }
        public bool Update_Role { get; set; }
        public bool Delete_Role { get; set; }
        public bool Export_Role { get; set; }
        public string Group_Report { get; set; }
        public string Group_No { get; set; }
    }
}