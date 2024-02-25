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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
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
                        where a.PermissionID == EnumUserType.MANAGER && a.StatusID == status && (keyword == "" ? true : a.FullName.RemoveUnicode().ToLower().Contains(keyword))
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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
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
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
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
                        where (a.PermissionID == EnumUserType.EMPLOYEE || a.PermissionID == EnumUserType.MANAGER || a.PermissionID == EnumUserType.GUEST)
                        && (keyword != "" ? (a.FullName.RemoveUnicode().ToLower().Contains(keyword) || a.Email.RemoveUnicode().ToLower().Contains(keyword)) : true)
                        && a.StatusID != EnumStatus.DELETE
                        select a).OrderBy(x => x.FullName);

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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var new_record = new User();
            var users = _db.Users.FirstOrDefault(x => x.UserID == id);
            users = users == null ? new_record : users;


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
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
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
                    obj.PermissionID = obj.PermissionID ?? EnumUserType.EMPLOYEE;
                    obj.CreateDate = DateTime.Now;
                    obj.PositionID = obj.PositionID;
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
                    old.PositionID = obj.PositionID;
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
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Users.FirstOrDefault(x => x.UserID == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
            obj.StatusID = EnumStatus.DELETE;

            var a = _db.User_Category.Where(x => x.UserID == id);
            _db.User_Category.RemoveRange(a);

            var b = _db.User_Product.Where(x => x.UserID == id);
            _db.User_Product.RemoveRange(b);

            var c = _db.User_Permission.Where(x => x.UserID == id);
            _db.User_Permission.RemoveRange(c);

            var d = _db.Tokens.Where(x => x.UserID == id);
            _db.Tokens.RemoveRange(d);

            _db.Users.Remove(obj);
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

        #region permission
        [Route("employee/permission")]
        public ActionResult Role_User_Permission(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);

            var category = _db.Categorys.ToList();
            ViewBag.categories = category;

            var user_Category = _db.User_Category.Where(x => x.UserID == id).OrderBy(x => x.CategoryId).Select(x => x.CategoryId).ToList();
            ViewBag.userCategories = user_Category;

            var products = from a in _db.User_Product
                           join b in _db.Products on a.ProductId equals b.ProductId
                           where a.UserID == id
                           select b;

            ViewBag.products = products;

            var userProducts = _db.User_Product.Where(x => x.UserID == id).ToList();
            ViewBag.userProducts = userProducts;

            return PartialView(user);
        }
        #endregion


        #region permission access type
        [Route("employee/permission/access-type")]
        public ActionResult Role_User_Permission_Access_Type(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);

            return PartialView(user);
        }

        [Route("employee/permission/select/access")]
        public ActionResult Role_User_Permission_Select_Access(Guid? id = null, int? permission = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);
            user.PermissionID = permission;
            _db.SaveChanges();
            return Json(new { kq = "ok", id = id, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region permission block category
        //Product in table User_Category is product blocked
        [Route("employee/permission/category/list")]
        public ActionResult Role_User_Permission_List(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);

            var user_Category = _db.User_Category.Where(x => x.UserID == id).Select(x => x.CategoryId).ToList();

            var category = _db.Categorys.Where(x => user_Category.Contains(x.CategoryId) && x.StatusID != EnumStatus.DELETE).ToList();
            ViewBag.categories = category;

            return PartialView(user);
        }

        [Route("employee/permission/category/add")]
        public ActionResult Role_User_Permission_Add_Category(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);

            var user_Category = _db.User_Category.Where(x => x.UserID == id).OrderBy(x => x.CategoryId).Select(x => x.CategoryId).ToList();

            var category = _db.Categorys.Where(x => !user_Category.Contains(x.CategoryId) && x.StatusID != EnumStatus.DELETE).ToList();
            string cbxCategory = "";
            foreach (var item in category)
            {
                cbxCategory += string.Format("<option value=\"{0}\">{1}</option>", item.CategoryId, item.Name);
            }
            ViewBag.cbxCategory = cbxCategory;

            User_Category obj = new User_Category();
            obj.UserID = user.UserID;

            return PartialView(obj);
        }

        [Route("employee/permission/category/add")]
        [HttpPost]
        public ActionResult Role_User_Permission_Add_Category(User_Category obj)
        {
            try
            {
                CheckPermission(EnumFunctions.Employee, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                var old = _db.User_Category.Count(x => x.UserID == obj.UserID && x.CategoryId == obj.CategoryId);
                if (old == 0)
                {
                    obj.User_CategoryID = Guid.NewGuid();
                    _db.User_Category.Add(obj);
                    _db.SaveChanges();
                }

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));


            }
            catch (Exception e)
            {
                return Json(new CxResponse("err", Message.MSG_EXCEPTION));
            }

        }

        [Route("employee/permission/category/remove")]
        public ActionResult Role_User_Permission_Remove_Category(Guid? id = null, int? category = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var userCategory = _db.User_Category.FirstOrDefault(x => x.UserID == id && x.CategoryId == category);

            var products = _db.Products.Where(x => x.CategoryId == category).Select(x => x.ProductId).ToList();
            var userProducts = _db.User_Product.Where(x => products.Contains(x.ProductId) && x.UserID == id).ToList();
            if (userProducts != null && userProducts.Count() > 0)
            {
                _db.User_Product.RemoveRange(userProducts);
            }
            _db.User_Category.Remove(userCategory);
            _db.SaveChanges();

            return Json(new { kq = "ok", id = id, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region permission block recipe
        //Product in table User_Product is product blocked
        [Route("employee/permission/product")]
        public ActionResult Role_User_Permission_Product(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);
            var userProducts = _db.User_Product.Where(x => x.UserID == id).Select(x => x.ProductId).ToList();

            var products = _db.Products.Where(x => userProducts.Contains(x.ProductId) && x.StatusID != EnumStatus.DELETE).ToList();
            ViewBag.products = products;

            return PartialView(user);
        }

        [Route("employee/permission/product/add")]
        public ActionResult Role_User_Permission_Add_Product(Guid? id = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var user = _db.Users.FirstOrDefault(x => x.UserID == id);

            var user_Product = _db.User_Product.Where(x => x.UserID == id).OrderBy(x => x.ProductId).Select(x => x.ProductId).ToList();

            var products = _db.Products.Where(x => !user_Product.Contains(x.ProductId) && x.StatusID != EnumStatus.DELETE).ToList();
            string cbxProducts = "";
            foreach (var item in products)
            {
                cbxProducts += string.Format("<option value=\"{0}\">{1}</option>", item.ProductId, item.Name);
            }
            ViewBag.cbxProducts = cbxProducts;

            User_Product obj = new User_Product();
            obj.UserID = user.UserID;

            return PartialView(obj);
        }

        [Route("employee/permission/product/add")]
        [HttpPost]
        public ActionResult Role_User_Permission_Add_Product(User_Product obj)
        {
            try
            {
                CheckPermission(EnumFunctions.Employee, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                var old = _db.User_Product.Count(x => x.UserID == obj.UserID && x.ProductId == obj.ProductId);
                if (old == 0)
                {
                    obj.User_ProductID = Guid.NewGuid();
                    obj.StatusID = EnumStatus.ACTIVE;
                    _db.User_Product.Add(obj);
                    _db.SaveChanges();
                }
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
            catch (Exception e)
            {
                return Json(new CxResponse("err", Message.MSG_EXCEPTION));
            }

        }

        [Route("employee/permission/product/remove")]
        public ActionResult Role_User_Permission_Remove_Product(Guid? id = null, Guid? product = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var userProduct = _db.User_Product.FirstOrDefault(x => x.UserID == id && x.ProductId == product);
            _db.User_Product.Remove(userProduct);
            _db.SaveChanges();

            return Json(new { kq = "ok", id = id, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}