using System;
namespace Ario.API.Models.Authentication
{
    public class AuthUserInfo
    {
        public string sub { get; set; }
        public string name { get; set; }
        public string locale { get; set; }
        public string email { get; set; }
        public string preferred_username { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string zoneinfo { get; set; }
        public int updated_at { get; set; }
        public bool email_verified { get; set; }

    }
}
