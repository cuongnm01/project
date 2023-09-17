using Common.Constants;
using Common.Helpers;
using Common.Resources;
using Project.Model;
using Project.Model.Configuration;
using Project.Model.DbSet;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Service.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "admin")]
    [Route("{action}")]
    public class ProfileController : BaseController
    {
        AppDbContext _db = new AppDbContext();
        [Route("profile/main-page")]
        public ActionResult MainPage()
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            var user = _db.Users.FirstOrDefault(x => x.UserID == nd_dv.Users.UserID);

            return View(user);
        }

        [Route("profile/update")]
        public ActionResult Update(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Users.FirstOrDefault(x => x.UserID == id);
            return PartialView(obj);
        }

        [Route("profile/update")]
        [HttpPost]
        public ActionResult Update(User obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });
                var old = _db.Users.FirstOrDefault(x => x.UserID == obj.UserID);
                if (old == null)
                    return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params("User")));

                if (_Logo != null)
                {
                    string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("yyyy/MM/dd"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                    old.Avatar = fileImage[1];
                }

                old.FullName = obj.FullName;
                old.Phone = obj.Phone;
                old.Email = obj.Email;
                _db.SaveChanges();

                nd_dv.Users = old;
                Session[ConfigKey.SESSION_LOGIN] = nd_dv;

                return Json(new CxResponse(Message.MSG_SUCESS.Params("Success")));

            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }


        [Route("profile/password")]
        public ActionResult Password(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Users.FirstOrDefault(x => x.UserID == id);
            return PartialView(obj);
        }

        [Route("profile/password")]
        [HttpPost]
        public ActionResult Password(User obj, string Oldpass = "")
        {
            try
            {
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });
                var old = _db.Users.FirstOrDefault(x => x.UserID == obj.UserID);
                if (old == null)
                    return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params("User")));

                Oldpass = Base.Security.Encode(Oldpass);
                if (old.Password != Oldpass)
                    return Json(new CxResponse("err", "Wrong password"));

                old.Password = Base.Security.Encode(obj.Password);
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params("Success")));
            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }
    }
}