using Common.Constants;
using Project.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Helpers;
using Project.Model.DbSet;
using System.IO;
using Project.Model.Respone;
using Common.Resources;

namespace Project.Service.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "admin")]
    [Route("{action}")]
    public class IngredientController : BaseController
    {
        // GET: Admin/Ingredient
        AppDbContext _db = new AppDbContext();
        [Route("ingredient/main-page")]
        public ActionResult MainPage()
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("ingredient/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        { 
            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Ingredients.ToList()
                        where status == null ? true : a.StatusID == status && a.StatusID != EnumStatus.DELETE
                        select a)
            .OrderByDescending(x => x.CreateDate);

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

        [Route("ingredient/update")]
        public ActionResult Update(int? id)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var listIngredients = _db.Ingredients.ToList();
            var slider = _db.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            return PartialView(slider);
        }

        [Route("ingredient/update")]
        [HttpPost]
        public ActionResult Update(Ingredient obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                CheckPermission(EnumFunctions.Ingredient, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                if (obj.IngredientId == 0)
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
                        obj.Image = fileImage[1];
                    }
                    _db.Ingredients.Add(obj);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.Ingredients.FirstOrDefault(x => x.IngredientId == obj.IngredientId);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }
                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        old.Image = fileImage[1];
                    }
                    old.Name = obj.Name;
                    old.SortOrder = obj.SortOrder;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }

            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }

        [Route("ingredient/delete")]
        public ActionResult Delete(int id)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
            obj.StatusID = EnumStatus.DELETE;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("ingredient/change-status")]
        public ActionResult Change_Status(int id)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));

            if (obj.StatusID == EnumStatus.ACTIVE)
                obj.StatusID = EnumStatus.INACTIVE;
            else
                obj.StatusID = EnumStatus.ACTIVE;

            _db.SaveChanges();
            return Json(new CxResponse<object>(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }
    }
}