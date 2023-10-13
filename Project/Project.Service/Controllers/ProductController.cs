
using Project.Model.DbSet;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using Common.Constants;
using Project.Model.Model;

namespace Project.Service.Controllers
{
    public class ProductController : BaseController
    {
        private string url = "http://cafebottega.com/";

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

                var product = db.Products.Where(x => x.Name.ToLower().Contains(keyword) && !userProduct.Contains(x.ProductId) && x.StatusID == EnumStatus.ACTIVE).OrderBy(x => x.Name).ToList();

                var products = (from a in product
                                select new
                                {
                                    code = a.ProductId,
                                    name = a.Name,
                                    image = GetBaseUrl + a.Image.GetImage(),
                                    isNew = a.CreateDate.Value.AddDays(15) > DateTime.Now ? true : false,
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

                var product = db.Products.Where(x => x.CategoryId == categoryId && !userProduct.Contains(x.ProductId) && x.StatusID == EnumStatus.ACTIVE).OrderBy(x => x.Name).ToList();

                var products = (from a in product
                                select new
                                {
                                    code = a.ProductId,
                                    name = a.Name,
                                    image = GetBaseUrl + a.Image.GetImage(),
                                    isNew = a.CreateDate.Value.AddDays(15) > DateTime.Now ? true : false,
                                }).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                return Json(new { isSuccess = true, data = new { total = product.Count, products = products, }, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }

        [HttpGet]
        [Route("api/product/detail/{code}")]
        public IHttpActionResult ProductDetail([FromUri] Guid? code = null)
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

                var steps = (from a in db.ProductDirections
                             join c in db.ProductDirectionGroup on a.ProductDirectionGroupId equals c.ProductDirectionGroupId into c1
                             from c in c1.DefaultIfEmpty()
                             where a.ProductId == product.ProductId
                             select new ProductDirectionInfo()
                             {
                                 ProductDirection = a,
                                 ProductDirectionGroup = c,
                             }).OrderBy(x => x.ProductDirectionGroup != null ? x.ProductDirectionGroup.SortOrder : 0).ThenBy(x => x.ProductDirection.SortOrder).ToList();


                var sizeIds = db.ProductIngredients.Where(x => x.ProductId == product.ProductId).Select(x => x.SizeId).Distinct().ToList();
                var sizes = db.Sizes.Where(x => sizeIds.Contains(x.SizeId)).ToList();

                var ingredientsTmp = (from a in db.ProductIngredients
                                      join b in db.Ingredients on a.IngredientId equals b.IngredientId
                                      join c in db.ProductIngredientGroup on a.ProductIngredientGroupId equals c.ProductIngredientGroupId into c1
                                      from c in c1.DefaultIfEmpty()
                                      where a.ProductId == product.ProductId && a.SizeId != null
                                      select new ProductIngredientInfo()
                                      {
                                          Header = c != null ? c.Name : "",
                                          ProductIngredient = a,
                                          Ingredient = b,
                                          ProductIngredientGroup = c,
                                      }).OrderBy(x => x.ProductIngredientGroup != null ? x.ProductIngredientGroup.SortOrder : 0).ThenBy(x => x.ProductIngredient.SortOrder).ToList();

                var ingredients = ingredientsTmp.GroupBy(x => x.Ingredient).Select(x => new { Key = x.Key, List = x.ToList() }).ToList();

                int i = 0;
                foreach (var item in steps)
                {
                    item.ProductDirection.Name = string.Format("Step {0}", ++i);
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
                                     code = a.ProductDirection.ProductDirectionId,
                                     name = a.ProductDirection.Name,
                                     image = !string.IsNullOrEmpty(a.ProductDirection.Image) ? url + a.ProductDirection.Image.Replace("~/", "/") : "",
                                     description = a.ProductDirection.Description ?? "",
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
                                              join c in db.Sizes on b.ProductIngredient.SizeId equals c.SizeId
                                              select new
                                              {
                                                  sizeId = c.SizeId,
                                                  value = b.ProductIngredient.Value ?? 0,
                                                  unit = b.ProductIngredient.Unit ?? "",
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

        [HttpGet]
        [Route("api/recipe/detail/{code}")]
        public IHttpActionResult RecipeDetail([FromUri] Guid? code = null)
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

                var steps = (from a in db.ProductDirections
                             join c in db.ProductDirectionGroup on a.ProductDirectionGroupId equals c.ProductDirectionGroupId into c1
                             from c in c1.DefaultIfEmpty()
                             where a.ProductId == product.ProductId
                             select new ProductDirectionInfo()
                             {
                                 Header = c != null ? c.Name.Trim() : "",
                                 ProductDirection = a,
                                 ProductDirectionGroup = c,
                             }).OrderBy(x => x.ProductDirectionGroup != null ? x.ProductDirectionGroup.SortOrder : 0).ThenBy(x => x.ProductDirection.SortOrder).ToList();

                var sizeIds = db.ProductIngredients.Where(x => x.ProductId == product.ProductId).Select(x => x.SizeId).Distinct().ToList();
                var sizes = db.Sizes.Where(x => sizeIds.Contains(x.SizeId)).ToList();

                var ingredientsTmp = (from a in db.ProductIngredients
                                      join b in db.Ingredients on a.IngredientId equals b.IngredientId
                                      join c in db.ProductIngredientGroup on a.ProductIngredientGroupId equals c.ProductIngredientGroupId into c1
                                      from c in c1.DefaultIfEmpty()
                                      where a.ProductId == product.ProductId && a.SizeId != null
                                      select new ProductIngredientInfo()
                                      {
                                          Header = c != null ? c.Name.Trim() : "",
                                          ProductIngredient = a,
                                          Ingredient = b,
                                          ProductIngredientGroup = c,
                                      }).OrderBy(x => x.ProductIngredientGroup != null ? x.ProductIngredientGroup.SortOrder : 0).ThenBy(x => x.ProductIngredient.SortOrder).ToList();

                var ingredients = ingredientsTmp.GroupBy(x => x.Ingredient).Select(x => new { Key = x.Key, List = x.ToList() }).ToList();

                int i = 0;
                foreach (var item in steps)
                {
                    item.ProductDirection.Name = string.Format("Step {0}", ++i);
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
                                     header = a.Header ?? "",
                                     code = a.ProductDirection.ProductDirectionId,
                                     name = a.ProductDirection.Name,
                                     image = !string.IsNullOrEmpty(a.ProductDirection.Image) ? url + a.ProductDirection.Image.Replace("~/", "/") : "",
                                     description = a.ProductDirection.Description ?? "",
                                 }).ToList(),
                    video = new
                    {
                        url =  product.VideoUrl ?? "",
                        title = product.VideoTitle ?? "",
                        description = product.VideoDescription ?? "",
                    },
                    size = (from a in sizes
                            select new
                            {
                                id = a.SizeId,
                                name = a.Name ?? "",
                                ingredients = (from b in ingredientsTmp
                                               where b.ProductIngredient.SizeId == a.SizeId
                                               select new
                                               {
                                                   header = b.Header ?? "",
                                                   id = b.Ingredient.IngredientId,
                                                   name = b.Ingredient.Name,
                                                   image = !string.IsNullOrEmpty(b.Ingredient.Image) ? url + b.Ingredient.Image.Replace("~/", "/") : "",
                                                   value = b.ProductIngredient.Value ?? 0,
                                                   unit = b.ProductIngredient.Unit ?? "",
                                               }).ToList(),
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
