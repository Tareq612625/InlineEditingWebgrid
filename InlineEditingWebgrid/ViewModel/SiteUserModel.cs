using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InlineEditingWebgrid.ViewModel
{
    public class SiteUserModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }

    }
}