using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itProgerWPF.Models
{
    [Serializable]
    public class AuthUser
    {

        public string Login { get; set; }

        public string Email { get; set; }

        public AuthUser()
        {
        }

        public AuthUser(string login, string email)
        {
            Login = login;
            Email = email;
        }
    }
}
