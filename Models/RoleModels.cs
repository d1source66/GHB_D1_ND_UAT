using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class RoleModels
    {
        //<ROLE_LEVEL, bigint,>
        public long ROLE_LEVEL { get; set; }
        //,<ROLE_DEPARTMENT, nvarchar(max),>
        public string ROLE_DEPARTMENT { get; set; }
        //,<ROLE_ACTIVE, bit,>
        public bool ROLE_ACTIVE { get; set; }

        //,<ROLE_CREATED, datetime,>
        public DateTime? ROLE_CREATED { get; set; }
        //,<ROLE_UPDATED, datetime,>
        public DateTime? ROLE_UPDATED { get; set; }
        public bool IsSelected { get; set; }

        //20250222 TBL_ROLE_MANAGEMENT
        public long ID_Role { get; set; }
        public string Position_Role { get; set; }
        public string Role_Name { get; set; }
        public string Module_No { get; set; }
        public string Module_Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N5}")]
        public decimal? Group_Module { get; set; }

        //[DisplayFormat(DataFormatString = "{0:N5}")]
        //public decimal? RoleReport_No { get; set; }
        public string Group_No { get; set; }
        public string Group_Report { get; set; }
        public string RoleReport_Name { get; set; }
        public bool List_Role { get; set; }
        public bool Create_Role { get; set; }
        public bool View_Role { get; set; }
        public bool Update_Role { get; set; }
        public bool Delete_Role { get; set; }
        public bool Export_Role { get; set; }
        public bool Is_Active { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Remark { get; set; }
        public string MESSAGE { get; set; }
    }

    public class groupRole
    {
        public string role_name { get; set; }
        public List<RoleModels> group_role { get; set; }
    }
}