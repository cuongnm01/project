using System;
using System.Net;

namespace Common.Exceptions
{
    public class ExceptionBase : Exception
    {
        public string Code { get; set; }

        public override string Message { get; }

        public HttpStatusCode StatusCode { get; }

        public ExceptionBase(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public ExceptionBase(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ExceptionBase(HttpStatusCode statusCode, string code, string message)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
        }
    }
}
