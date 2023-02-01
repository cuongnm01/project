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
    public class UsersController : BaseController
    {
        // GET: Admin/Users
        AppDbContext _db = new AppDbContext();

        #region manager
        [Route("manager/main-page")]
        public ActionResult MainPage_Manager()
        {
            CheckPermission(EnumFunctions.Manager, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("manager/list")]
        public ActionResult List_Manager(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {

            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Users.ToList()
                        where a.PermissionID == EnumUserType.MANAGER && a.StatusID == status
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

        [Route("manager/update")]
        public ActionResult Update_Manager(Guid? id)
        {
            CheckPermission(EnumFunctions.Manager, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var users = _db.Users.FirstOrDefault(x => x.UserID == id);
            return PartialView(users);
        }

        [Route("manager/update")]
        [HttpPost]
        public ActionResult Update_Manager(User obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                CheckPermission(EnumFunctions.Manager, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                if (obj.UserID == Guid.Empty)
                {
                    var checkUserName = _db.Users.FirstOrDefault(x => x.UserName == obj.UserName);
                    if (checkUserName != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params(Message.F_USERNAME)));
                    }

                    var checkPhone = _db.Users.FirstOrDefault(x => x.Phone == obj.Phone);
                    if (obj.Phone != null && checkPhone != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params(Message.F_PHONE)));
                    }

                    var checkEmail = _db.Users.FirstOrDefault(x => x.Email == obj.Email);
                    if (obj.Email != null && checkEmail != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params("Email")));
                    }
                    // tao moi
                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        obj.Avatar = fileImage[1];
                    }
                    obj.UserID = Guid.NewGuid();
                    obj.UserCode = Helpers.CreateCode9Char();
                    obj.Password = Base.Security.Encode(obj.Password);
                    obj.PermissionID = EnumUserType.MANAGER;
                    obj.CreateDate = DateTime.Now;
                    _db.Users.Add(obj);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.Users.FirstOrDefault(x => x.UserID == obj.UserID);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }

                    var checkUserName = _db.Users.FirstOrDefault(x => x.UserID != obj.UserID && x.UserName == obj.UserName);
                    if (checkUserName != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params(Message.F_USERNAME)));
                    }

                    var checkPhone = _db.Users.FirstOrDefault(x => x.UserID != obj.UserID && x.Phone == obj.Phone);
                    if (obj.Phone != null && checkPhone != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params(Message.F_PHONE)));
                    }

                    var checkEmail = _db.Users.FirstOrDefault(x => x.UserID != obj.UserID && x.Email == obj.Email);
                    if (obj.Email != null && checkEmail != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params("Email")));
                    }

                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        old.Avatar = fileImage[1];
                    }
                    old.UserName = obj.UserName;
                    if (obj.Password != null)
                        old.Password = Base.Security.Encode(obj.Password);

                    old.Phone = obj.Phone;
                    old.Email = obj.Email;
                    old.FullName = obj.FullName;
                    old.StatusID = obj.StatusID;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }

            }
            catch (Exception e)
            {
                return Json(new CxResponse("err", Message.MSG_EXCEPTION));
            }

        }

        [Route("manager/delete")]
        public ActionResult Delete_Manager(Guid id)
        {
            CheckPermission(EnumFunctions.Manager, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Users.FirstOrDefault(x => x.UserID == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
            obj.StatusID = EnumStatus.DELETE;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("manager/change-status")]
        public ActionResult Change_Status_Manager(Guid id)
        {
            CheckPermission(EnumFunctions.Manager, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Users.FirstOrDefault(x => x.UserID == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));

            if (obj.StatusID == EnumStatus.ACTIVE)
                obj.StatusID = EnumStatus.INACTIVE;
            else
                obj.StatusID = EnumStatus.ACTIVE;

            _db.SaveChanges();
            return Json(new CxResponse<object>(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region permission
        [Route("manager/permission")]
        public ActionResult Role_Permission(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);

            var role_Details = _db.User_Permission.Where(x => x.UserID == id).OrderBy(x => x.Functions).ToList();

            var role = Base.Functions.EnumList<int>(typeof(EnumFunctions));
            ViewBag.role = role;
            return PartialView(user);
        }

        [Route("manager/permission/list")]
        public ActionResult Role_Permission_List(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);

            var role_Details = _db.User_Permission.Where(x => x.UserID == id).OrderBy(x => x.Functions).ToList();

            var role = Base.Functions.EnumList<int>(typeof(EnumFunctions));
            ViewBag.role = role;
            ViewBag.role_Details = role_Details;
            return PartialView(user);
        }


        [Route("manager/permission/select")]
        public ActionResult Role_Permission_Select(Guid? id = null, int? function = null, int? option = null, int? choose = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var role_Details = _db.User_Permission.FirstOrDefault(x => x.UserID == id && x.Functions == function);
            if (role_Details == null)
            {
                role_Details = new User_Permission();
                role_Details.User_PermissionID = Guid.NewGuid();
                role_Details.UserID = id;
                role_Details.Functions = function;
                role_Details.Fulls = option == EnumOptions.FULL ? EnumStatus.ACTIVE : EnumStatus.INACTIVE;
                role_Details.Views = option == EnumOptions.VIEW ? EnumStatus.ACTIVE : EnumStatus.INACTIVE;
                role_Details.Updates = option == EnumOptions.ADD ? EnumStatus.ACTIVE : EnumStatus.INACTIVE;
                role_Details.Deletes = option == EnumOptions.DELETE ? EnumStatus.ACTIVE : EnumStatus.INACTIVE;
                _db.User_Permission.Add(role_Details);
            }
            else
            {
                if (option == EnumOptions.FULL)
                    role_Details.Fulls = role_Details.Fulls == EnumStatus.ACTIVE ? EnumStatus.INACTIVE : EnumStatus.ACTIVE;
                else if (option == EnumOptions.VIEW)
                    role_Details.Views = role_Details.Views == EnumStatus.ACTIVE ? EnumStatus.INACTIVE : EnumStatus.ACTIVE;
                else if (option == EnumOptions.ADD)
                    role_Details.Updates = role_Details.Updates == EnumStatus.ACTIVE ? EnumStatus.INACTIVE : EnumStatus.ACTIVE;
                else if (option == EnumOptions.DELETE)
                    role_Details.Deletes = role_Details.Deletes == EnumStatus.ACTIVE ? EnumStatus.INACTIVE : EnumStatus.ACTIVE;
            }
            _db.SaveChanges();

            return Json(new { kq = "ok", id = id, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region employee
        [Route("employee/main-page")] 
        public ActionResult MainPage()
        {
            CheckPermission(EnumFunctions.Employee, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("employee/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Users.ToList()
                        where a.PermissionID == EnumUserType.EMPLOYEE && a.StatusID == status
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

        [Route("employee/update")]
        public ActionResult Update(Guid? id)
        {
            CheckPermission(EnumFunctions.Employee, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var users = _db.Users.FirstOrDefault(x => x.UserID == id);
            return PartialView(users);
        }

        [Route("employee/update")]
        [HttpPost]
        public ActionResult Update(User obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                CheckPermission(EnumFunctions.Employee, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                if (obj.UserID == Guid.Empty)
                {
                    var checkUserName = _db.Users.FirstOrDefault(x => x.UserName == obj.UserName);
                    if (checkUserName != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params(Message.F_USERNAME)));
                    }

                    var checkPhone = _db.Users.FirstOrDefault(x => x.Phone == obj.Phone);
                    if (obj.Phone != null && checkPhone != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params(Message.F_PHONE)));
                    }

                    var checkEmail = _db.Users.FirstOrDefault(x => x.Email == obj.Email);
                    if (obj.Email != null && checkEmail != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params("Email")));
                    }
                    // tao moi
                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        obj.Avatar = fileImage[1];
                    }
                    obj.UserID = Guid.NewGuid();
                    obj.UserCode = Helpers.CreateCode9Char();
                    obj.Password = Base.Security.Encode(obj.Password);
                    obj.PermissionID = EnumUserType.EMPLOYEE;
                    obj.CreateDate = DateTime.Now;
                    _db.Users.Add(obj);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.Users.FirstOrDefault(x => x.UserID == obj.UserID);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }

                    var checkUserName = _db.Users.FirstOrDefault(x => x.UserID != obj.UserID && x.UserName == obj.UserName);
                    if (checkUserName != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params(Message.F_USERNAME)));
                    }

                    var checkPhone = _db.Users.FirstOrDefault(x => x.UserID != obj.UserID && x.Phone == obj.Phone);
                    if (obj.Phone != null && checkPhone != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params(Message.F_PHONE)));
                    }

                    var checkEmail = _db.Users.FirstOrDefault(x => x.UserID != obj.UserID && x.Email == obj.Email);
                    if (obj.Email != null && checkEmail != null)
                    {
                        return Json(new CxResponse("err", Message.MSG_EXISTS.Params("Email")));
                    }

                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        old.Avatar = fileImage[1];
                    }
                    old.UserName = obj.UserName;
                    if (obj.Password != null)
                        old.Password = Base.Security.Encode(obj.Password);

                    old.Phone = obj.Phone;
                    old.Email = obj.Email;
                    old.FullName = obj.FullName;
                    old.StatusID = obj.StatusID;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }

            }
            catch (Exception e)
            {
                return Json(new CxResponse("err", Message.MSG_EXCEPTION));
            }

        }

        [Route("employee/delete")]
        public ActionResult Delete(Guid id)
        {
            CheckPermission(EnumFunctions.Employee, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Users.FirstOrDefault(x => x.UserID == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
            obj.StatusID = EnumStatus.DELETE;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("employee/change-status")]
        public ActionResult Change_Status(Guid id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var obj = _db.Users.FirstOrDefault(x => x.UserID == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));

            if (obj.StatusID == EnumStatus.ACTIVE)
                obj.StatusID = EnumStatus.INACTIVE;
            else
                obj.StatusID = EnumStatus.ACTIVE;

            _db.SaveChanges();
            return Json(new CxResponse<object>(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}