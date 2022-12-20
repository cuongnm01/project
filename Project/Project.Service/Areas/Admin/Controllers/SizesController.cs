using Common.Constants;
using Common.Helpers;
using Common.Resources;
using Project.Model;
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
    public class SizesController : BaseController
    {
        AppDbContext _db = new AppDbContext();
        [Route("size/main-page")]
        public ActionResult MainPage()
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            ViewBag.User = nd_dv;
            return View();
        }

        [Route("size/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Sizes.ToList()
                            where status == null ? true : a.StatusID == status
                        select a).OrderByDescending(x => x.CreateDate);

            int tongso = list.Count();

            sotrang = sotrang <= 0 ? 1 : sotrang;
            tongsodong = tongsodong <= 0 ? 10 : tongsodong;
            int tongsotrang = tongso % tongsodong > 0 ? tongso / tongsodong + 1 : tongso / tongsodong;
            tongsotrang = tongsotrang <= 0 ? 1 : tongsotrang;
            sotrang = sotrang > tongsotrang ? tongsotrang - 1 : sotrang - 1;
            ViewBag.sotrang = sotrang + 1;
            ViewBag.tongsotrang = tongsotrang;
            ViewBag.tongso = tongso;
            return PartialView(list == null ? list : list.Skip(sotrang * tongsodong).Take(tongsodong));
        }

        [Route("size/update")]
        public ActionResult Update(int? id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var size = _db.Sizes.FirstOrDefault(x => x.SizeId == id);
            return PartialView(size);
        }

        [Route("size/update")]
        [HttpPost]
        public ActionResult Update(Size size, HttpPostedFileBase _Logo = null)
        {
            try
            {
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                    return RedirectToAction("Index", "Home", new { area = "" });
                if (size.SizeId == 0)
                {
                    _db.Sizes.Add(size);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.Sizes.FirstOrDefault(x => x.SizeId == size.SizeId);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }
                    old.Name = size.Name;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }
            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }

        [Route("size/delete")]
        public ActionResult Delete(int id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var size = _db.Sizes.FirstOrDefault(x => x.SizeId == id);
            if (size == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SIZE)));
            //slider.sta = EnumStatus.DELETE;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("size/change-status")]
        public ActionResult Change_Status(int id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var size = _db.Sizes.FirstOrDefault(x => x.SizeId == id);
            if (size == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SIZE)));

            //if (slider.StatusID == EnumStatus.ACTIVE)
            //    slider.StatusID = EnumStatus.INACTIVE;
            //else
            //    slider.StatusID = EnumStatus.ACTIVE;

            _db.SaveChanges();
            return Json(new CxResponse<object>(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }
    }
}