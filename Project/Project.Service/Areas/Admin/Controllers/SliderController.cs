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
    public class SliderController : BaseController
    {
        AppDbContext _db = new AppDbContext();
        [Route("banner/main-page")]
        public ActionResult MainPage()
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            ViewBag.User = nd_dv;
            return View();
        }

        [Route("banner/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Sliders.ToList()
                            //where status == null ? true : a.StatusID == status
                        select a);
            //.OrderByDescending(x => x.cre);

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

        [Route("banner/update")]
        public ActionResult Update(int? id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var listSlider = _db.Sliders.ToList();
            var slider = _db.Sliders.FirstOrDefault(x => x.SliderId == id);
            return PartialView(slider);
        }

        [Route("banner/update")]
        [HttpPost]
        public ActionResult Update(Slider slider, HttpPostedFileBase _Logo = null)
        {
            try
            {
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                    return RedirectToAction("Index", "Home", new { area = "" });
                if (slider.SliderId == 0)
                {
                    //var checkTontai = _db.Sliders.FirstOrDefault(x => x.StatusID == EnumStatus.ACTIVE && x.STT == slider.STT);
                    //if (checkTontai != null)
                    //    return Json(new { kq = "err", msg = Translate.SLIDER_STT_IS_USED }, JsonRequestBehavior.AllowGet);

                    // tao moi
                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        slider.Url = fileImage[1];
                    }
                    _db.Sliders.Add(slider);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.Sliders.FirstOrDefault(x => x.SliderId == slider.SliderId);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }
                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        old.Url = fileImage[1];
                    }
                    old.SortOrder = slider.SortOrder;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }

            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }

        [Route("banner/delete")]
        public ActionResult Delete(int id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var slider = _db.Sliders.FirstOrDefault(x => x.SliderId == id);
            if (slider == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
            //slider.sta = EnumStatus.DELETE;
            _db.Sliders.Remove(slider);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("banner/change-status")]
        public ActionResult Change_Status(int id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var slider = _db.Sliders.FirstOrDefault(x => x.SliderId == id);
            if (slider == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));

            //if (slider.StatusID == EnumStatus.ACTIVE)
            //    slider.StatusID = EnumStatus.INACTIVE;
            //else
            //    slider.StatusID = EnumStatus.ACTIVE;

            _db.SaveChanges();
            return Json(new CxResponse<object>(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }
    }
}