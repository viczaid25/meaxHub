namespace MeaxHub.Services
{
    public interface ILdapAuthService
    {
        bool ValidateUser(string username, string password);
    }
}
