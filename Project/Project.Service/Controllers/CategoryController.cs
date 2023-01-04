
using Project.Model.DbSet;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project.Service.Controllers
{
    public class CategoryController : BaseController
    {
        private string url = "http://bottega.sapp.asia";

        [HttpGet]
        [ActionName("list")]
        public IHttpActionResult List()
        {
            try
            {
                var list = db.Categorys.ToList();

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
