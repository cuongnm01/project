
using Common.Constants;
using Project.Model.DbSet;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Project.Service.Controllers
{
    public class CategoryController : BaseController
    {
        private string url = "https://appcafebottega.com/";

        [HttpGet]
        [ActionName("list")]
        public IHttpActionResult List()
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

                var userCategory = db.User_Category.Where(x => x.UserID == user.UserID).Select(x => x.CategoryId).ToList();

                var list = db.Categorys.Where(x=> x.StatusID == EnumStatus.ACTIVE && !userCategory.Contains(x.CategoryId)).OrderBy(x=>x.SortOrder).ToList();

                var obj = (from a in list
                           select new
                           {
                               id = a.CategoryId,
                               name = a.Name ?? "",
                               image = !string.IsNullOrEmpty(a.Image) ? url + a.Image.Replace("~/", "/") : "",
                           }).ToList();

                return Json(new { isSuccess = true, data = obj, message = "", version = "", code = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
            }
        }
    }
}
