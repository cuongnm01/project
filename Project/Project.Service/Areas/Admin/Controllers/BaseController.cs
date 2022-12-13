using Project.Model.Configuration;
using Project.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Service.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        public UserInfo GetUserLogin
        {
            get
            {
                UserInfo nd_dv = (UserInfo)Session[ConfigKey.SESSION_LOGIN];
                return nd_dv;


            }
        }
    }
}