using InlineEditingWebgrid.Models;
using InlineEditingWebgrid.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace InlineEditingWebgrid.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            List<SiteUserModel> list = new List<SiteUserModel>();
            using (MyDatabaseEntities2 dc = new MyDatabaseEntities2())
            {
                var v = (from a in dc.SiteUsers
                         join b in dc.UserRoles on a.RoleID equals b.ID
                         select new SiteUserModel
                         {
                             ID = a.ID,
                             FirstName = a.FirstName,
                              LastName = a.LastName,
                               DOB =a.DOB,
                                RoleID = a.RoleID,
                                 RoleName =b.RoleName
                         });
                list = v.ToList();
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult saveuser(int id, string propertyName, string value)
        {
            var status = false;
            var message = "";

            //Update data to database 
            using (MyDatabaseEntities2 dc = new MyDatabaseEntities2())
            {
                var user = dc.SiteUsers.Find(id);

                object updateValue = value;
                bool isValid = true;

                if (propertyName == "RoleID") {
                    int newRoleID = 0;
                    if (int.TryParse(value, out newRoleID))
                    {
                        updateValue = newRoleID;
                        value = dc.UserRoles.Where(a => a.ID == newRoleID).First().RoleName;
                    }
                    else {
                        isValid = false;
                    }
                }
                else if (propertyName == "DOB")
                {
                    DateTime dob;
                    if (DateTime.TryParseExact(value,"dd-MM-yyyy",new CultureInfo("en-US"),System.Globalization.DateTimeStyles.None,out dob))
                    {
                        updateValue = dob;                        
                    }
                    else
                    {
                        isValid = false;
                    }
                }


                if (user != null && isValid)
                {
                    dc.Entry(user).Property(propertyName).CurrentValue = updateValue;
                    dc.SaveChanges();
                    status = true;
                }
                else
                {
                    message = "Error!";
                }
            }

            var response = new { value = value, status = status, message = message };
            JObject o = JObject.FromObject(response);
            return Content(o.ToString());
        }

        public ActionResult GetUserRoles(int id) {
            int selectedRoleID = 0;
            StringBuilder sb = new StringBuilder();
            using (MyDatabaseEntities2 dc = new MyDatabaseEntities2())
            {
                var roles = dc.UserRoles.OrderBy(a => a.RoleName).ToList();
                foreach (var item in roles) {
                    sb.Append(string.Format("'{0}': '{1}',", item.ID, item.RoleName));
                }
                selectedRoleID = dc.SiteUsers.Where(a => a.ID == id).First().RoleID;
            }
            sb.Append(string.Format("'selected': '{0}'", selectedRoleID));
            return Content("{"+sb.ToString() + "}");
        }
    }
}