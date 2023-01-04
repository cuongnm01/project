using Base;
using Project.Model;
using Project.Model.DbSet;
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

        private string url = "http://bottega.sapp.asia";
        // GET: Api/Bottega
        [HttpPost]
        [Route("auth/login")]
        public IHttpActionResult Login(string username = "", string password = "", string deviceToken = "")
        {
            try
            {
                var appConfig = db.AppConfigs.FirstOrDefault();

                password = password.Encode();
                var user = db.Users.FirstOrDefault(x => (x.Phone == username || x.Email == username));
                if (user == null)
                    return Json(new { isSuccess = false, data = new { }, message = "Unknow account", version = "", code = "" });


                if (user.Password != password)
                    return Json(new { isSuccess = false, data = new { }, message = "wrong password", version = "", code = "" });

                if (deviceToken != "")
                {
                    var listDeViceOld = db.Users.Where(x => x.UserID != user.UserID && x.DeviceToken == deviceToken);
                    if (listDeViceOld != null && listDeViceOld.Count() > 0)
                        listDeViceOld.ToList().ForEach(x => x.DeviceToken = "");
                }

                user.LanguageCode = "vi";
                if (user.DeviceToken != deviceToken)
                    user.DeviceToken = deviceToken;

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
        [Route("category/list")]
        public IHttpActionResult category()
        {
            try
            {
                var list = db.Categorys.ToList();

                var obj = (from a in list
                           select new
                           {
                               id = a.CategoryId,
                               name = a.Name ?? "",
                               image = !string.IsNullOrEmpty(a.Image) ? url + a.Image : "",
                           }).ToList();

                return Json(new { isSuccess = true, data = list, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

        [HttpGet]
        [Route("dashboard")]
        public IHttpActionResult Home()
        {
            try
            {
                var slide = db.Sliders.ToList();

                var sliders = (from a in slide
                               select new
                               {
                                   id = a.SliderId,
                                   image = !string.IsNullOrEmpty(a.Url) ? url + a.Url : "",
                               }).ToList();


                var product = db.Products.ToList();


                var products = (from a in product
                                select new
                                {
                                    code = a.ProductId,
                                    name = a.Name,
                                    image = !string.IsNullOrEmpty(a.Image) ? url + a.Image : "",
                                    isNew = a.IsNew == 1 ? true : false,
                                }).ToList();

                return Json(new { isSuccess = true, data = new { sliders = sliders, products = product, }, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

       
    }
}
