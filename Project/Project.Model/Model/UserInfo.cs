using Project.Model.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.Model
{
    public class UserInfo
    {
        public User Users { get; set; }
        public List<User_Permission> User_Permissions { get; set; }
        public int AccessDenied { get; set; }
    }

    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }

        public string deviceToken { get; set; }

    }
}
