using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class TL02DataModel
    {
		public string TERM_FIID { get; set; }
		public string TERM_ID { get; set; }
		public string CARD_LN { get; set; }
		public string CARD_FIID { get; set; }
		public DateTime ADMIN_DTIME { get; set; }
		public string ADMIN_CODE { get; set; }
		public string BRANCH_ID { get; set; }
		public string REGION_ID { get; set; }
		public short HOPR1_CNT { get; set; }
		public double HOPR1_CASH { get; set; }
		public double HOPR1_INCR { get; set; }
		public double HOPR1_DECR { get; set; }
		public double HOPR1_OUT { get; set; }
		public double HOPR1_END { get; set; }
		public string HOPR1_CDE { get; set; }
		public int HOPR1_REJ { get; set; }
		public short HOPR2_CNT { get; set; }
		public double HOPR2_CASH { get; set; }
		public double HOPR2_INCR { get; set; }
		public double HOPR2_DECR { get; set; }
		public double HOPR2_OUT { get; set; }
		public double HOPR2_END { get; set; }
		public string HOPR2_CDE { get; set; }
		public int HOPR2_REJ { get; set; }
		public short HOPR3_CNT { get; set; }
		public double HOPR3_CASH { get; set; }
		public double HOPR3_INCR { get; set; }
		public double HOPR3_DECR { get; set; }
		public double HOPR3_OUT { get; set; }
		public double HOPR3_END { get; set; }
		public string HOPR3_CDE { get; set; }
		public int HOPR3_REJ { get; set; }
		public short HOPR4_CNT { get; set; }
		public double HOPR4_CASH { get; set; }
		public double HOPR4_INCR { get; set; }
		public double HOPR4_DECR { get; set; }
		public double HOPR4_OUT { get; set; }
		public double HOPR4_END { get; set; }
		public string HOPR4_CDE { get; set; }
		public int HOPR4_REJ { get; set; }
		public string ADMIN_DATE { get; set; }
		public string TOPUPDATE { get; set; }
		public double TOPUPCASH { get; set; }
		public double BEGINCASH { get; set; }
		public double CASHUSE_INDAY { get; set; }
		public double TOTAL_USE { get; set; }
		public double TLAMT { get; set; }
		public double TOTAL_USE_Day { get; set; }
		public double TOTALHOPR_END { get; set; }
		public string REMARK { get; set; }
		public string CHK_REPORT { get; set; }
		public DateTime TIMETL_START { get; set; }
		public DateTime TIMETL_END { get; set; }
		public long ADMIN_KEY { get; set; }
		public string CRNCY_CDE { get; set; }
		public string T_SEQ_NUM { get; set; }
		public double TLAMT_DEP { get; set; }
		public double TOTAL_USE_DEP { get; set; }
		public double TOTAL_USE_Day_DEP { get; set; }

	}
}