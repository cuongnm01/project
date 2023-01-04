
using Project.Model.DbSet;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Common.Constants;

namespace Project.Service.Controllers
{
    public class ProductController : BaseController
    {
        private string url = "http://bottega.sapp.asia";

        [HttpGet]
        [Route("by_category/{categoryId}")]    
        public IHttpActionResult ProductByCategory([FromUri] int categoryId = 0, int pageIndex = 1, int pageSize = 8)
        {
            try
            {
                var product = db.Products.Where(x => x.CategoryId == categoryId).ToList();

                var products = (from a in product
                                select new
                                {
                                    code = a.ProductId,
                                    name = a.Name,
                                    image = GetBaseUrl + a.Image.GetImage(),
                                    isNew = a.IsNew == 1 ? true : false,
                                }).ToList();

                return Json(new { isSuccess = true, data = new { total = product.Count, products = product, }, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

        [HttpGet]
        [Route("detail/{code}")]
        public IHttpActionResult ProductDetail([FromUri]string code = "")
        {
            try
            {
                var product = db.Products.FirstOrDefault(x => x.Code == code);
                if (product == null)
                    return Json(new { isSuccess = false, data = new { }, message = "Sản phẩm không xác định", version = "", code = "" });


                var steps = db.ProductDirections.Where(x => x.ProductId == product.ProductId).ToList();
                var sizeIds = db.ProductIngredients.Where(x => x.ProductId == product.ProductId).Select(x => x.SizeId).Distinct().ToList();
                var sizes = db.Sizes.Where(x => sizeIds.Contains(x.SizeId)).ToList();

                var ingredients = db.ProductIngredients.Where(x => x.ProductId == product.ProductId).GroupBy(x => x.IngredientId).Select(x => new { Key = db.Ingredients.FirstOrDefault(y => y.IngredientId == x.Key), List = x.ToList() }).ToList();


                var obj = new
                {
                    code = product.Code,
                    name = product.Name,
                    image = !string.IsNullOrEmpty(product.Image) ? url + product.Image : "",
                    background = !string.IsNullOrEmpty(product.Background) ? url + product.Background : "",
                    isNew = product.IsNew == 1 ? true : false,
                    direction = (from a in steps
                                 select new
                                 {
                                     code = a.ProductDirectionId,
                                     name = a.Name,
                                     image = !string.IsNullOrEmpty(a.Image) ? url + a.Image : "",
                                     description = a.Description ?? "",
                                 }).ToList(),
                    video = new
                    {
                        url = !string.IsNullOrEmpty(product.VideoUrl) ? url + product.VideoUrl : "",
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
                                       image = !string.IsNullOrEmpty(a.Key.Image) ? url + a.Key.Image : "",
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

                return Json(new { isSuccess = true, data = product, obj = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }
    }
}
