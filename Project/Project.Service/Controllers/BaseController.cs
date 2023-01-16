using Common.ActionFilters;
using Common.Constants;
using Project.Model;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Project.Service.Controllers
{
    [System.Web.Mvc.RouteArea("api", AreaPrefix = "api")]
    [ExceptionFilter]
    [ValidationFilter]
    public class BaseController : ApiController
    {
        protected AppDbContext db = new AppDbContext();
        //protected Users getUserPrincipal(string token)
        //{
        //    try
        //    {
        //        string userId = token.Decode();
        //        Guid userIdGuid = Guid.Parse(userId);
        //        var tokens = db.Tokens.FirstOrDefault(x => x.TokenKey.Equals(token));
        //        if (tokens != null)
        //        {
        //            var users = db.Users.FirstOrDefault(x => x.Id == tokens.UserId);
        //            return users;
        //        }
        //        return null;
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //}
        //protected AuthorizeRespone Authorize()
        //{
        //    AuthorizeRespone res = new AuthorizeRespone();
        //    System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
        //    var tokenKey = headers.GetValues("Authorization").ToList()[0].Replace("Basic ", "");
        //    var token = db.Tokens.FirstOrDefault(x => x.TokenKey == tokenKey && x.ExpirationDate >= DateTime.Now && x.StatusID == EnumStatus.ACTIVE);
        //    if (token == null)
        //    {
        //        res.HttpStatusCode = HttpStatusCode.Unauthorized;

        //    }
        //    res.HttpStatusCode = HttpStatusCode.OK;
        //    //res.Token = token;
        //    return res;

        //}

        public string GetBaseUrl
        {
            get
            {
                var request = HttpContext.Current.Request;
                var appUrl = HttpRuntime.AppDomainAppVirtualPath;
                if (appUrl != "/")
                    appUrl = "/" + appUrl;
                var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);
                return baseUrl;
            }
        }

    }
}
