using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace StepChat.Classes
{
    public class AuthOptions
    {
        public const string ISSUER = "FlexibleSoftware"; // token publisher
        public const string AUDIENCE = "StepChat_User"; // token consumer
        public const string KEY = "401b09eab3cF013d4ca54922_fAbb802bec8Lfd5318192b0a75Ef2ghkHHFD01d8b3X727429090Ifb337591abd3e44453b954555b7aB0812e1081c39b7402L93f765eae731Ef5a65SOFTWARE80ed1"; // token consumer

        public const int LIFETIME = 10080; // lifetime - 1 week
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
