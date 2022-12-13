using System.Net;

namespace Common.Exceptions
{
    public class ExceptionBadRequest : ExceptionBase
    {
        public ExceptionBadRequest() : base(HttpStatusCode.BadRequest)
        {
        }

        public ExceptionBadRequest(string message) : base(HttpStatusCode.BadRequest, message)
        {
        }

        public ExceptionBadRequest(string code, string message) : base(HttpStatusCode.BadRequest, code, message)
        {
        }
    }
}
