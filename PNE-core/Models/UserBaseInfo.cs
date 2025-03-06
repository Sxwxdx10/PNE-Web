using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Models
{
    public class UserBaseInfo
    {
        public UserBaseInfo(string _name, string _token, string _uid, string? email)
        {
            Name = _name;
            Token = _token;
            Uid = _uid;
            Email = email;
        }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Uid { get; set; }
        public string? Email { get; set; }
    }
}
