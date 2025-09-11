using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class BranchModel
    {
        public string BRANCH { get; set; }
        public string DESC { get; set; }
        public string DESC_ENG { get; set; }
        public string ZONE { get; set; }
        public string REGION { get; set; }
        public string EMAIL { get; set; }
        public string EXCEPTREGION { get; set; }
        public string BNREGION { get; set; }
        public string FLAGCLOSE { get; set; }
        public string Alt1 { get; set; }
        public string Alt2 { get; set; }
        public string Alt3 { get; set; }
        public bool IsSelected { get; set; }
        public string SOL_CODE { get; set; }
        public string CATEGORY { get; set; }
        public string CATEGORY_DESC { get; set; }
    }
}