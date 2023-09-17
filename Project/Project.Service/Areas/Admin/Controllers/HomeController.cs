using Base;
using Common.Constants;
using Common.Helpers;
using Common.Resources;
using Project.Model;
using Project.Model.Configuration;
using Project.Model.Model;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Service.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        AppDbContext _db = new AppDbContext();
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }


        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(string u = "", string p = "")
        {
            try
            {

                p = p.Encode();
                var users = (from a in _db.Users
                             where a.UserName == u && a.Password == p 
                             && ( a.PermissionID == EnumUserType.ADMIN || a.PermissionID == EnumUserType.MANAGER || a.PermissionID == EnumUserType.EMPLOYEE)
                             select a).FirstOrDefault();
                if (users != null)
                {
                    // login thanh cong
                    UserInfo userInfo = new UserInfo();
                    userInfo.Users = users;
                    userInfo.User_Permissions = _db.User_Permission.Where(x => x.UserID == users.UserID).ToList();
                    Session[ConfigKey.SESSION_LOGIN] = userInfo;
                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.F_LOGIN)));
                }
                else
                {
                    return Json(new CxResponse("err", Message.MSG_LOGIN_FAILED));
                }
            }
            catch (Exception)
            {
                return Json(new CxResponse("err", Message.MSG_LOGIN_FAILED));
            }

        }

        // GET: NguoiDung
        [Route("logout")]
        public ActionResult Logout()
        {
            Session[ConfigKey.SESSION_LOGIN] = "";
            return RedirectToAction("Login");
        }

        [Route("account/change-password")]
        public ActionResult ChangePassword()
        {
            return PartialView();
        }

        [HttpPost]
        [Route("account/change-password")]
        public ActionResult ChangePassword(string OldPass = "", string NewPass = "")
        {
            UserInfo nd = GetUserLogin;

            return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
    }
}