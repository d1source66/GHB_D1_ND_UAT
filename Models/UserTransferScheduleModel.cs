using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class UserTransferScheduleModel 
    {
        public long Transfer_Id { get; set; }
        public string emp_code { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public bool is_active { get; set; }

        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime sch_time { get; set; }

        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime start_date { get; set; }

        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime end_date { get; set; }
        public string branch_code { get; set; }
        public string branch_name { get; set; }
        public string hub_code { get; set; }
        public string hub_name { get; set; }
        public string dept_code { get; set; }
        public string dept_name { get; set; }
        public string branch_code2 { get; set; }
        public string branch_name2 { get; set; }
        public string hub_code2 { get; set; }
        public string hub_name2 { get; set; }
        public string dept_code2 { get; set; }
        public string dept_name2 { get; set; }
        public int user_level { get; set; }
        public int user_level2 { get; set; }
        public string transfer_branch { get; set; }
        public string remark { get; set; }
        public DateTime? create_mg { get; set; }
        public DateTime? update_mg { get; set; }
        public string role_name1 { get; set; }
        public string role_name2 { get; set; }
        public int diff { get; set; }
        public string type { get; set; }
    }
}