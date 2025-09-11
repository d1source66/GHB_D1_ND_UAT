using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class AudiLogModel
    {
        public long Audi_Id { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public string WorkFunction { get; set; }
        public string Details01 { get; set; }
        public string Details02 { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string MAC_Address { get; set; }
        public string OS { get; set; }
        public string URL { get; set; }
        public string Device { get; set; }
        public string IP_Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FrDate { get; set; }
        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ToDate { get; set; }
        public string frdate2 { get; set; }
        public string todate2 { get; set; }
    }
}