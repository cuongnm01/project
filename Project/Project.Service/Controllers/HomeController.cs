using Base;
using Common.Constants;
using Project.Model;
using Project.Model.DbSet;
using Project.Model.Model;
using Project.Model.Respone;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Project.Service.Controllers
{
    public class HomeController : BaseController
    {
        AppDbContext db = new AppDbContext();

        [HttpGet]
        [ActionName("test")]
        public IHttpActionResult Index()
        {
            var size = db.Sizes.ToList();
            return Ok(new CxResponse<List<Size>>(size, "ok"));
        }

        private string url = "http://cafebottega.com/";
        // GET: Api/Bottega

        [HttpPost]
        [Route("api/auth/login")]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var appConfig = db.AppConfigs.FirstOrDefault();

                request.password = request.password.Encode();
                var user = db.Users.FirstOrDefault(x => (x.PermissionID == EnumUserType.EMPLOYEE || x.PermissionID == EnumUserType.MANAGER || x.PermissionID == EnumUserType.GUEST) && (x.UserName == request.username || x.Phone == request.username || x.Email == request.username));
                if (user == null)
                    return Json(new { isSuccess = false, data = new { }, message = "Unknown account", version = "", code = "" });

                if (user.Password != request.password)
                    return Json(new { isSuccess = false, data = new { }, message = "Wrong password", version = "", code = "" });

                if(user.StatusID == EnumStatus.INACTIVE)
                    return Json(new { isSuccess = false, data = new { }, message = "Account blocked", version = "", code = "" });

                if (request.deviceToken != "")
                {
                    var listDeViceOld = db.Users.Where(x => x.UserID != user.UserID && x.DeviceToken == request.deviceToken);
                    if (listDeViceOld != null && listDeViceOld.Count() > 0)
                        listDeViceOld.ToList().ForEach(x => x.DeviceToken = "");
                }

                user.LanguageCode = "vi";
                if (user.DeviceToken != request.deviceToken)
                    user.DeviceToken = request.deviceToken;

                Token token = new Token();
                token.TokenID = Guid.NewGuid();
                token.UserID = user.UserID;
                token.CreateDate = DateTime.Now;
                token.ExpirationDate = DateTime.Now.AddYears(1);
                token.TokenKey = (Guid.NewGuid().ToString() + token.UserID.ToString() + token.CreateDate.ToString()).Encode().Encode();
                db.Tokens.Add(token);

                db.SaveChanges();

                var obj = new
                {
                    token = token.TokenKey,
                    user = new
                    {
                        userCode = user.UserCode ?? "",
                        userName = user.UserName ?? "",
                        fullName = user.FullName ?? "",
                        avatar = !string.IsNullOrEmpty(user.Avatar) ? url + user.Avatar : "",
                        email = user.Email ?? "",
                        permission = user.PermissionID ?? EnumUserType.GUEST,
                    }
                };

                return Json(new { isSuccess = true, data = obj, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

        [HttpGet]
        [Route("api/home/dashboard")]
        public IHttpActionResult Home()
        {
            try
            {
                var slide = db.Sliders.ToList();

                var sliders = (from a in slide
                               select new
                               {
                                   id = a.SliderId,
                                   image = !string.IsNullOrEmpty(a.Url) ? url + a.Url.Replace("~/", "/") : "",
                                   productId = a.ProductId,
                               }).ToList();

                var category = db.Categorys.FirstOrDefault(x => x.IsHomePage == EnumStatus.ACTIVE) ?? new Category();

                var product = db.Products.Where(x=> x.StatusID != EnumStatus.DELETE && x.CategoryId == category.CategoryId).OrderBy(x=>x.Name).ToList();


                var products = (from a in product
                                select new
                                {
                                    code = a.ProductId,
                                    name = a.Name,
                                    image = !string.IsNullOrEmpty(a.Image) ? url + a.Image.Replace("~/","/") : "",
                                    isNew = a.IsNew == 1 ? true : false,
                                }).ToList();

                return Json(new { isSuccess = true, data = new { sliders = sliders, products = products, }, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

    }
}
