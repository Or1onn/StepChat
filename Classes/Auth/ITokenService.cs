using StepChat.Models;

namespace StepChat.Classes.Auth
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, UsersModel user, double expiration = 30);
        bool ValidateToken(string key, string issuer, string audience, string token);
    }
}
