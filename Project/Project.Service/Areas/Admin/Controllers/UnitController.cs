using Common.Constants;
using Common.Helpers;
using Common.Resources;
using Project.Model;
using Project.Model.DbSet;
using Project.Model.Model;
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

        #region unit group
        [Route("unit-group/main-page")]
        public ActionResult MainPage_Group()
        {
            CheckPermission(EnumFunctions.Unit, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("unit-group/list")]
        public ActionResult List_Group(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var ingredients = _db.Ingredients;

            var list = (from a in _db.UnitGroups.ToList()
                        where (keyword == "" ? true : a.Name.RemoveUnicode().ToLower().Contains(keyword))
                        select new UnitGroupInfo
                        {
                            UnitGroup = a,
                            TotalIngredient = ingredients.Count(x => x.UnitGroupId == a.UnitGroupId),
                            TotalUnit = _db.Units.Count(x => x.UnitGroupId == a.UnitGroupId),
                        }).OrderBy(x => x.UnitGroup.Name);

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

        [Route("unit-group/update")]
        public ActionResult Update_Group(int? id)
        {
            CheckPermission(EnumFunctions.Unit, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var new_record = new UnitGroup();
            var sortOrder = _db.UnitGroups.OrderByDescending(x => x.SortOrder).Select(x => x.SortOrder).FirstOrDefault();
            new_record.SortOrder = sortOrder != null ? (sortOrder + 1) : 1;

            var obj = _db.UnitGroups.FirstOrDefault(x => x.UnitGroupId == id);
            obj = obj == null ? new_record : obj;

            return PartialView(obj);
        }

        [Route("unit-group/update")]
        [HttpPost]
        public ActionResult Update_Group(UnitGroup obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                CheckPermission(EnumFunctions.Unit, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                if (obj.UnitGroupId == 0)
                {
                    obj.CreateDate = DateTime.Now;
                    obj.StatusID = EnumStatus.ACTIVE;
                    _db.UnitGroups.Add(obj);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.UnitGroups.FirstOrDefault(x => x.UnitGroupId == obj.UnitGroupId);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }

                    old.Name = obj.Name;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }
            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }

        [Route("unit-group/delete")]
        public ActionResult Delete_Group(int id)
        {
            CheckPermission(EnumFunctions.Unit, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var unit = _db.UnitGroups.FirstOrDefault(x => x.UnitGroupId == id);
            if (unit == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SIZE)));
            _db.UnitGroups.Remove(unit);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("unit-group/delete-all")]
        public ActionResult Delete_GroupAll(string ids)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var unitIds = ids.Split(',').ToList();

            var unitGroups = _db.UnitGroups.Where(x => unitIds.Contains(x.UnitGroupId.ToString()));
            if (unitGroups == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);

            _db.UnitGroups.RemoveRange(unitGroups);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region unit
        [Route("unit/main-page")]
        public ActionResult MainPage(int? unitGroupId = null)
        {
            CheckPermission(EnumFunctions.Unit, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            ViewBag.unitGroupId = unitGroupId ?? 0;
            return View();
        }

        [Route("unit/list")]
        public ActionResult List(int? unitGroupId = null, string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Units.ToList()
                        join b in _db.UnitGroups on a.UnitGroupId equals b.UnitGroupId
                        where (unitGroupId == null || unitGroupId == 0 ? true : a.UnitGroupId == unitGroupId)
                         && (keyword == "" ? true : a.Name.RemoveUnicode().ToLower().Contains(keyword))
                        select new UnitInfo
                        {
                            Unit = a,
                            UnitGroup = b,
                        }).OrderBy(x => x.UnitGroup.Name).ThenBy(x=>x.Unit.Name);

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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var new_record = new Unit();
            var sortOrder = _db.Units.OrderByDescending(x => x.SortOrder).Select(x => x.SortOrder).FirstOrDefault();
            new_record.SortOrder = sortOrder != null ? (sortOrder + 1) : 1;

            var unit = _db.Units.FirstOrDefault(x => x.UnitId == id);
            unit = unit == null ? new_record : unit;

            var category = _db.UnitGroups
               .Where(x => x.StatusID == EnumStatus.ACTIVE)
               .ToList();
            string cbxCategory = "";
            foreach (var item in category)
            {
                cbxCategory += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.UnitGroupId, item.Name, unit != null && unit.UnitGroupId == item.UnitGroupId ? "selected" : "");
            }
            ViewBag.cbxCategory = cbxCategory;
            return PartialView(unit);
        }

        [Route("unit/update")]
        [HttpPost]
        public ActionResult Update(Unit obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                CheckPermission(EnumFunctions.Unit, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                if (obj.UnitId == 0)
                {
                    var ortherItem = _db.Units.FirstOrDefault(x => x.UnitGroupId == obj.UnitGroupId && x.IsDefault == true);
                    if (ortherItem == null)
                    {
                        obj.IsDefault = true;
                        obj.Rate = 1;
                    }
                    else
                    {
                        if (obj.IsDefault == true)
                        {
                            obj.Rate = 1;
                            ortherItem.IsDefault = false;
                        }
                    }

                    obj.CreateDate = DateTime.Now;
                    obj.StatusID = EnumStatus.ACTIVE;
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

                    var ortherItem = _db.Units.FirstOrDefault(x => x.UnitGroupId == obj.UnitGroupId && x.UnitId != obj.UnitId && x.IsDefault == true);
                    if (obj.IsDefault == true)
                    {
                        if (ortherItem != null)
                            ortherItem.IsDefault = false;
                    }
                    else
                    {
                        if (ortherItem == null)
                            return Json(new CxResponse("err", "Unit group does not have a base yet if updated"));
                    }

                    old.UnitGroupId = obj.UnitGroupId;
                    old.Code = obj.Code;
                    old.Name = obj.Name;
                    old.IsDefault = obj.IsDefault;
                    old.Rate = obj.IsDefault == true ? 1 : obj.Rate;
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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var unit = _db.Units.FirstOrDefault(x => x.UnitId == id);
            if (unit == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SIZE)));

            _db.Units.Remove(unit);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("unit/delete-all")]
        public ActionResult DeleteAll(string ids)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var unitIds = ids.Split(',').ToList();

            var units = _db.Units.Where(x => unitIds.Contains(x.UnitId.ToString()));
            if (units == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);

            _db.Units.RemoveRange(units);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}