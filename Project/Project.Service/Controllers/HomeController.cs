﻿using Base;
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
using System.Net.Http;
using System.Web.Http;
using System.Drawing.Printing;
using System.Net;
using System.Web.Razor.Tokenizer.Symbols;
using System.Runtime.InteropServices.ComTypes;

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

                // Cách 1: Lấy IP từ Request.UserHostAddress
                string clientIp = HttpContext.Current.Request.UserHostAddress;

                // Cách 2: Nếu sử dụng proxy, lấy từ header X-Forwarded-For
                string forwardedFor = HttpContext.Current.Request.Headers["X-Forwarded-For"];
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    // Nếu có header X-Forwarded-For, lấy IP đầu tiên trong chuỗi
                    clientIp = forwardedFor.Split(',')[0];
                }


                var appConfig = db.AppConfigs.FirstOrDefault();

                request.password = request.password.Encode();
                var user = db.Users.FirstOrDefault(x => (x.PermissionID == EnumUserType.EMPLOYEE || x.PermissionID == EnumUserType.MANAGER || x.PermissionID == EnumUserType.GUEST) && (x.UserName == request.username || x.Phone == request.username || x.Email == request.username));
                if (user == null)
                    return Json(new { isSuccess = false, data = new { }, message = "Unknown account", version = "", code = "" });

                if (user.Password != request.password)
                    return Json(new { isSuccess = false, data = new { }, message = "Wrong password", version = "", code = "" });

                if (user.StatusID == EnumStatus.INACTIVE)
                    return Json(new { isSuccess = false, data = new { }, message = "Account blocked", version = "", code = "" });

                if (!string.IsNullOrEmpty(user.Address) && user.Address != clientIp)
                    return Json(new { isSuccess = false, data = new { }, message = "IP not allow", version = "", code = "" });

                if (request.deviceToken != "")
                {
                    var listDeViceOld = db.Users.Where(x => x.UserID != user.UserID && x.DeviceToken == request.deviceToken);
                    if (listDeViceOld != null && listDeViceOld.Count() > 0)
                        listDeViceOld.ToList().ForEach(x => x.DeviceToken = "");
                }

                user.LanguageCode = "vi";
                if (user.DeviceToken != request.deviceToken)
                    user.DeviceToken = request.deviceToken;

                if (!string.IsNullOrEmpty(clientIp))
                    user.Address = clientIp;

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

                var product = db.Products.Where(x => x.StatusID == EnumStatus.ACTIVE && x.IsNew == EnumStatus.ACTIVE).OrderBy(x => x.Name).Take(20).ToList();
                var products = (from a in product
                                select new
                                {
                                    code = a.ProductId,
                                    name = a.Name,
                                    image = !string.IsNullOrEmpty(a.Image) ? url + a.Image.Replace("~/", "/") : "",
                                    isNew = a.CreateDate.Value.AddDays(15) > DateTime.Now ? true : false,
                                }).ToList();

                return Json(new { isSuccess = true, data = new { sliders = sliders, products = products, }, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

        [HttpGet]
        [Route("api/data/sync")]
        public IHttpActionResult SyncData()
        {
            try
            {
                var author = Authorize();
                if (author == null || author.HttpStatusCode != HttpStatusCode.OK)
                {
                    return Unauthorized();
                }
                var token = db.Tokens.FirstOrDefault(x => x.TokenKey.Equals(author.Token.TokenKey));
                if (token == null || (token.ExpirationDate <= DateTime.Now))
                {
                    return Unauthorized();
                }
                var user = db.Users.FirstOrDefault(x => x.UserID == token.UserID);
                if (user == null)
                {
                    return Ok(new CxResponse("err", "User not found"));
                }
                var response = new
                {
                    categories = db.Categorys.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    provinces = db.Provinces.ToList(),
                    districts = db.Districts.ToList(),
                    wards = db.Wards.ToList(),
                    ingredientGroups = db.IngredientGroups.Where(x=>x.StatusID != EnumStatus.DELETE).ToList(),
                    ingredients = db.Ingredients.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    productDirectionGroups = db.ProductDirectionGroup.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    productDirections = db.ProductDirections.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    productIngredientGroups = db.ProductIngredientGroup.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    productIngredients = db.ProductIngredients.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    products = db.Products.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    productSizes = db.ProductSizes.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    sizes = db.Sizes.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    sliders = db.Sliders.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    units = db.Units.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    unitGroups = db.UnitGroups.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                    users = db.Users.Where(x => x.StatusID != EnumStatus.DELETE).ToList(),
                };

                return Json(new { isSuccess = true, data = response, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

        //[HttpGet]
        //[Route("api/data/sync")]
        //public IHttpActionResult SyncData()
        //{
        //    try
        //    {
        //        var sizes = db.ProductIngredients.Where(x => x.SizeId == EnumSize.REGULAR);

        //        List<ProductIngredient> lstProductIngredients = new List<ProductIngredient>();

        //        foreach (var productIngredient in sizes)
        //        {
        //            var sizeLarge = new ProductIngredient();
        //            sizeLarge.ProductIngredientId = Guid.NewGuid();
        //            sizeLarge.ProductId = productIngredient.ProductId;
        //            sizeLarge.SizeId = EnumSize.LARGE;
        //            sizeLarge.IngredientId = productIngredient.IngredientId;
        //            sizeLarge.Value = productIngredient.Value * 1.5;
        //            sizeLarge.Price = productIngredient.Price * 1.5;
        //            sizeLarge.StatusID = productIngredient.StatusID;
        //            sizeLarge.CreateDate = productIngredient.CreateDate;
        //            sizeLarge.UnitId = productIngredient.UnitId;
        //            sizeLarge.Unit = productIngredient.Unit;
        //            sizeLarge.ProductIngredientGroupId = productIngredient.ProductIngredientGroupId;
        //            sizeLarge.SortOrder = productIngredient.SortOrder;
        //            lstProductIngredients.Add(sizeLarge);
        //        }

        //        db.ProductIngredients.AddRange(lstProductIngredients);
        //        db.SaveChanges();



        //        return Json(new { isSuccess = true, data = new { }, message = "", version = "", code = "" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
        //    }
        //}
    }
}
