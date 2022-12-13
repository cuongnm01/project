using Common.Exceptions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Common.ActionFilters
{
    public class ValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var error = (from x in actionContext.ModelState
                             where x.Value.Errors.Count > 0
                             select new
                             {
                                 x.Key,
                                 x.Value.Errors.FirstOrDefault().ErrorMessage
                             }).FirstOrDefault();
                throw new ExceptionValidationError(error.Key, error.ErrorMessage);
            }
        }

    }
}
