using Common.Exceptions;
using Common.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace Common.ActionFilters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception == null) return;
            ExceptionBase exception = null;
            var isBase = true;
            if (!(context.Exception is ExceptionBase))
            {
                exception = new ExceptionBase(
                System.Net.HttpStatusCode.InternalServerError
                , context.Exception.Message
                );
                isBase = false;
            }else
            {
                exception = (ExceptionBase)context.Exception;

            }
            
            var output = new ErrorResult
            {
                isSuccess = false,
                code = exception.Code,
                message = isBase ? exception.Message : Message.MSG_EXCEPTION,
                version = ""
            };
            context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var json = JsonConvert.SerializeObject(output).Trim();
            var stringContent = new StringContent(json);
            context.Response.Content = stringContent;
        }
    }
}
