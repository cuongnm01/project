using Common.Constants;
using Project.Model;
using Project.Model.Configuration;
using Project.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Controller = System.Web.Mvc.Controller;

namespace Project.Service.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {

        // GET: Admin/Base
        public UserInfo GetUserLogin
        {
            get
            {
                try
                {
                    UserInfo nd_dv = (UserInfo)Session[ConfigKey.SESSION_LOGIN];
                    return nd_dv;
                }
                catch
                {
                    return new UserInfo();
                }
            }
        }

        public ActionResult CheckPermission(int? functions = null, int? option = null)
        {
            AppDbContext _db = new AppDbContext();
            UserInfo nd_dv = GetUserLogin;
            if (nd_dv == null)
                return RedirectToAction("Timeout", "SessionLogin", new { area = "" });

            nd_dv.AccessDenied = EnumStatus.INACTIVE;

            //var permission = _db.User_Permission.FirstOrDefault(x => x.UserID == nd_dv.Users.UserID && x.Functions == functions);
            //if (nd_dv.Users.PermissionID != EnumUserType.ADMIN && permission == null)
            //{
            //    nd_dv.AccessDenied = EnumStatus.ACTIVE;
            //    return Json(new {}, JsonRequestBehavior.AllowGet);
            //}

            //if (nd_dv.Users.PermissionID != EnumUserType.ADMIN && permission.Fulls != EnumStatus.ACTIVE)
            //{
            //    if (option == EnumOptions.VIEW && permission.Views != EnumStatus.ACTIVE)
            //        nd_dv.AccessDenied = EnumStatus.ACTIVE;

            //    if (option == EnumOptions.ADD && permission.Updates != EnumStatus.ACTIVE)
            //        nd_dv.AccessDenied = EnumStatus.ACTIVE;

            //    if (option == EnumOptions.DELETE && permission.Deletes != EnumStatus.ACTIVE)
            //        nd_dv.AccessDenied = EnumStatus.ACTIVE;

            //    if (option == EnumOptions.FULL && permission.Fulls != EnumStatus.ACTIVE)
            //        nd_dv.AccessDenied = EnumStatus.ACTIVE;
            //}

            Session[ConfigKey.SESSION_LOGIN] = nd_dv;
            return Json(new {}, JsonRequestBehavior.AllowGet);

        }

    }
}