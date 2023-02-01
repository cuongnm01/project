using Common.Constants;
using Common.Helpers;
using Common.Resources;
using Project.Model;
using Project.Model.DbSet;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Service.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "admin")]
    [Route("{action}")]
    public class UnitController : BaseController
    {
        // GET: Admin/Unit
        AppDbContext _db = new AppDbContext();
        [Route("unit/main-page")]
        public ActionResult MainPage()
        {
            CheckPermission(EnumFunctions.Unit, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("unit/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Units.ToList()
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

        [Route("unit/update")]
        public ActionResult Update(int? id)
        {
            CheckPermission(EnumFunctions.Unit, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var size = _db.Units.FirstOrDefault(x => x.UnitId == id);

            return PartialView(size);
        }

        [Route("unit/update")]
        [HttpPost]
        public ActionResult Update(Unit obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                CheckPermission(EnumFunctions.Unit, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                if (obj.UnitId == 0)
                {
                    obj.CreateDate = DateTime.Now;
                    _db.Units.Add(obj);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.Units.FirstOrDefault(x => x.UnitId == obj.UnitId);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }
                    old.Code = obj.Code;
                    old.Name = obj.Name;
                    old.IsDefault = obj.IsDefault;
                    old.Rate = obj.Rate;
                    old.SortOrder = obj.SortOrder;
                    old.StatusID = obj.StatusID;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }
            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }

        [Route("unit/delete")]
        public ActionResult Delete(int id)
        {
            CheckPermission(EnumFunctions.Unit, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var unit = _db.Units.FirstOrDefault(x => x.UnitId == id);
            if (unit == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SIZE)));
            unit.StatusID = EnumStatus.DELETE;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("unit/change-status")]
        public ActionResult Change_Status(int id)
        {
            CheckPermission(EnumFunctions.Unit, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var unit = _db.Units.FirstOrDefault(x => x.UnitId == id);
            if (unit == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SIZE)));

            if (unit.StatusID == EnumStatus.ACTIVE)
                unit.StatusID = EnumStatus.INACTIVE;
            else
                unit.StatusID = EnumStatus.ACTIVE;

            _db.SaveChanges();
            return Json(new CxResponse<object>(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }
    }
}