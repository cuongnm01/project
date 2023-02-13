using Project.Model.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.Respone
{
    public class AuthorizeRespone
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public Token Token { get; set; }
    }
}
