using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StepChat.Classes
{
    public class AuthOptions
    {
        public const string ISSUER = "FlexibleSoftware"; // издатель токена
        public const string AUDIENCE = "StepChat_User"; // потребитель токена
        const string KEY = "FleXibLesOftWare_StepChatuSER";   // ключ для шифрации
        public const int LIFETIME = 1440; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
