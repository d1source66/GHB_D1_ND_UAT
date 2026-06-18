using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    /// <summary>
    /// Report to regenerate from .rpt file which is in Db but not in the Dir
    /// </summary>
    public class ReportRegenerate
    {
        public ReportRegenerate()
        {
        }

        public ReportRegenerate(string report_name_db, string report_file_name)
        {
            this.Report_File_Name = report_file_name;
            this.Report_Name_Db = report_name_db;
        }

        /// <summary>
        /// Report name from Db ([dbo].[AMS_SP_MAIN_GetGroupDetailReport_add_branch_v2])
        /// Ex.: ACCT_NON_TYPE_15_ORFT_EXCEL
        /// </summary>
        public string Report_Name_Db { get; set; }

        /// <summary>
        /// Report file name. Ex.: ACCT_NON_TYPE_15_ORFT.xls
        /// </summary>
        public string Report_File_Name { get; set; }
    }
}