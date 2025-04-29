
using Base;
using Project.Model;
using Project.Model.DbSet;
using Project.Model.Model;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;


namespace Project.Service.Controllers
{

    public class AuthController : BaseController
    {
        AppDbContext db = new AppDbContext();
        private string url = "https://appcafebottega.com/";

        //// GET: Auth
        //[HttpPost]
        //[ActionName("login")]
        //public IHttpActionResult Login([FromBody] LoginRequest request)
        //{
        //    try
        //    {
        //        var appConfig = db.AppConfigs.FirstOrDefault();

        //        request.password = request.password.Encode();
        //        var user = db.Users.FirstOrDefault(x => x.UserName == request.username || x.Phone == request.username || x.Email == request.username);
        //        if (user == null)
        //            return Json(new { isSuccess = false, data = new { }, message = "Unknow account", version = "", code = "" });

        //        if (user.Password != request.password)
        //            return Json(new { isSuccess = false, data = new { }, message = "wrong password", version = "", code = "" });

        //        if (request.deviceToken != "")
        //        {
        //            var listDeViceOld = db.Users.Where(x => x.UserID != user.UserID && x.DeviceToken == request.deviceToken);
        //            if (listDeViceOld != null && listDeViceOld.Count() > 0)
        //                listDeViceOld.ToList().ForEach(x => x.DeviceToken = "");
        //        }

        //        user.LanguageCode = "vi";
        //        if (user.DeviceToken != request.deviceToken)
        //            user.DeviceToken = request.deviceToken;

        //        Token token = new Token();
        //        token.TokenID = Guid.NewGuid();
        //        token.UserID = user.UserID;
        //        token.CreateDate = DateTime.Now;
        //        token.ExpirationDate = DateTime.Now.AddYears(1);
        //        token.TokenKey = (Guid.NewGuid().ToString() + token.UserID.ToString() + token.CreateDate.ToString()).Encode().Encode();
        //        db.Tokens.Add(token);

        //        db.SaveChanges();

        //        var obj = new
        //        {
        //            token = token.TokenKey,
        //            user = new
        //            {
        //                userCode = user.UserCode ?? "",
        //                userName = user.UserName ?? "",
        //                fullName = user.FullName ?? "",
        //                avatar = !string.IsNullOrEmpty(user.Avatar) ? url + user.Avatar : "",
        //                email = user.Email ?? "",
        //            }
        //        };

        //        return Json(new { isSuccess = true, data = obj, message = "", version = "", code = "" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { isSuccess = false, data = new { }, message = "", version = "", code = "" });
        //    }
        //}

    }
}