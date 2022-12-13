using System.Net;

namespace Common.Exceptions
{
    internal class ExceptionForbidden : ExceptionBase
    {
        public ExceptionForbidden() : base(HttpStatusCode.Forbidden)
        {
        }

        public ExceptionForbidden(string message) : base(HttpStatusCode.Forbidden, message)
        {
        }

        public ExceptionForbidden(string code, string message) : base(HttpStatusCode.Forbidden, code, message)
        {
        }
    }
}
