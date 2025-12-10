using System.DirectoryServices.AccountManagement;

namespace MeaxHub.Services
{
    public class LdapAuthService : ILdapAuthService
    {
        private readonly string _domain = "ad.meax.mx";

        public bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
#pragma warning disable CA1416
                using var context = new PrincipalContext(
                    ContextType.Domain,
                    _domain
                );

                return context.ValidateCredentials(username, password);
#pragma warning restore CA1416
            }
            catch
            {
                return false;
            }
        }
    }
}
