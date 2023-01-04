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
                               }).ToList();


                var product = db.Products.ToList();


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
