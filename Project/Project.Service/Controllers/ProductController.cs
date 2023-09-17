
using Project.Model.DbSet;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using Common.Constants;

namespace Project.Service.Controllers
{
    public class ProductController : BaseController
    {
        private string url = "http://bottega.sapp.asia";

        [HttpGet]
        [Route("api/product/list")]
        public IHttpActionResult ProductByCategory(string keyword = "", int pageIndex = 1, int pageSize = 8)
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
                var userProduct = db.User_Product.Where(x => x.UserID == user.UserID && x.StatusID == EnumStatus.ACTIVE).Select(x => x.ProductId).ToList();

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();
                }

                var product = db.Products.Where(x=>x.Name.ToLower().Contains(keyword) && !userProduct.Contains(x.ProductId) && x.StatusID == EnumStatus.ACTIVE).OrderBy(x => x.CreateDate).ToList();

                var products = (from a in product
                                select new
                                {
                                    code = a.ProductId,
                                    name = a.Name,
                                    image = GetBaseUrl + a.Image.GetImage(),
                                    isNew = a.IsNew == 1 ? true : false,
                                }).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                return Json(new { isSuccess = true, data = new { total = product.Count, products = products, }, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

        [HttpGet]
        [Route("api/product/by_category/{categoryId}")]    
        public IHttpActionResult ProductByCategory([FromUri] int categoryId = 0, int pageIndex = 1, int pageSize = 8)
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
                var userProduct = db.User_Product.Where(x => x.UserID == user.UserID && x.StatusID == EnumStatus.ACTIVE).Select(x => x.ProductId).ToList();

                var product = db.Products.Where(x => x.CategoryId == categoryId && !userProduct.Contains(x.ProductId) && x.StatusID == EnumStatus.ACTIVE).OrderBy(x=>x.CreateDate).ToList();

                var products = (from a in product
                                select new
                                {
                                    code = a.ProductId,
                                    name = a.Name,
                                    image = GetBaseUrl + a.Image.GetImage(),
                                    isNew = a.IsNew == 1 ? true : false,
                                }).Skip(pageSize * (pageIndex -1)).Take(pageSize).ToList();

                return Json(new { isSuccess = true, data = new { total = product.Count, products = products, }, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

        [HttpGet]
        [Route("api/product/detail/{code}")]
        public IHttpActionResult ProductDetail([FromUri]Guid? code = null)
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
                var userProduct = db.User_Product.Where(x => x.UserID == user.UserID && x.StatusID == EnumStatus.ACTIVE).Select(x => x.ProductId).ToList();

                var product = db.Products.FirstOrDefault(x => x.ProductId == code && !userProduct.Contains(x.ProductId) && x.StatusID == EnumStatus.ACTIVE);
                if (product == null)
                    return Json(new { isSuccess = false, data = new { }, message = "Unknown Recipe", version = "", code = "" });

                var steps = db.ProductDirections.Where(x => x.ProductId == product.ProductId).OrderBy(x=>x.SortOrder).ToList();
                var sizeIds = db.ProductIngredients.Where(x => x.ProductId == product.ProductId).Select(x => x.SizeId).Distinct().ToList();
                var sizes = db.Sizes.Where(x => sizeIds.Contains(x.SizeId)).ToList();

                var ingredients = db.ProductIngredients.Where(x => x.ProductId == product.ProductId).GroupBy(x => x.IngredientId).Select(x => new { Key = db.Ingredients.FirstOrDefault(y => y.IngredientId == x.Key), List = x.ToList() }).ToList();

                int i = 0;
                foreach(var item in steps)
                {
                    item.Name = string.Format("Step {0}", ++i);
                }

                var obj = new
                {
                    code = product.Code,
                    name = product.Name,
                    image = !string.IsNullOrEmpty(product.Image) ? url + product.Image.Replace("~/", "/") : "",
                    background = !string.IsNullOrEmpty(product.Background) ? url + product.Background.Replace("~/", "/") : "",
                    isNew = product.IsNew == 1 ? true : false,
                    direction = (from a in steps
                                 select new
                                 {
                                     code = a.ProductDirectionId,
                                     name = a.Name,
                                     image = !string.IsNullOrEmpty(a.Image) ? url + a.Image.Replace("~/", "/") : "",
                                     description = a.Description ?? "",
                                 }).ToList(),
                    video = new
                    {
                        url = !string.IsNullOrEmpty(product.VideoUrl) ? url + product.VideoUrl.Replace("~/", "/") : "",
                        title = product.VideoTitle ?? "",
                        description = product.VideoDescription ?? "",
                    },
                    size = (from a in sizes
                            select new
                            {
                                id = a.SizeId,
                                name = a.Name ?? "",
                            }).ToList(),

                    ingredients = (from a in ingredients
                                   select new
                                   {
                                       id = a.Key.IngredientId,
                                       name = a.Key.Name,
                                       image = !string.IsNullOrEmpty(a.Key.Image) ? url + a.Key.Image.Replace("~/", "/") : "",
                                       mass = from b in a.List
                                              join c in db.Sizes on b.SizeId equals c.SizeId
                                              select new
                                              {
                                                  sizeId = c.SizeId,
                                                  value = b.Value ?? 0,
                                                  unit = b.Unit ?? "",
                                              }
                                   }).ToList(),



                };

                return Json(new { isSuccess = true, data = obj, version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }
    }
}
