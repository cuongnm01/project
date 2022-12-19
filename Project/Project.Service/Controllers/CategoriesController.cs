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
    public class CategoriesController : BaseController
    {
        [HttpGet]
        [ActionName("list")]
        public IHttpActionResult List()
        {
            var category = (from a in db.Categorys.ToList()
                           select new CategoryResponse()
                           {
                               id = a.CategoryId,
                               name = a.Name,
                               image = a.Image ??"",
                           }).ToList()
                           ;
            return Ok(new CxResponse<List<CategoryResponse>>(category, "ok"));
        }
    }
}
