using System.Net;

namespace Common.Exceptions
{
    public class ExceptionValidationError : ExceptionBase
    {
        public ExceptionValidationError() : base(HttpStatusCode.OK)
        {
        }

        public ExceptionValidationError(string message) : base(HttpStatusCode.OK, message)
        {
        }

        public ExceptionValidationError(string code, string message) : base(HttpStatusCode.OK, code, message)
        {
        }
    }
}
