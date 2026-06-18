using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{
    public class GroupUserModel
    {
        public UserModels m1 { get; set; }
        public UserTransferScheduleModel m2 { get; set; }
        public RoleModels m3 { get; set; }
        public groupRole groupRole { get; set; }

        public List<UserModels> lst_m1 { get; set; }
        public List<RoleModels> lst_role { get; set; }
        public List<UserTransferScheduleModel> lst_m2 { get; set; }

    }
}