using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class UserAttribModel
    {
        //<NCFGVARIABLEID, smallint,>
        public short NCFGVARIABLEID { get; set; }
        //,<SCFGVARIABLE, varchar(50),>
        public string SCFGVARIABLE { get; set; }
        //,<SCFGVALUE, varchar(50),>
        public string SCFGVALUE { get; set; }
        //,<SDESCRIPTION, nvarchar(500),>
        public string SDESCRIPTION { get; set; }
        public bool IsSelected { get; set; }
        public string Category { get; set; }
    }
}