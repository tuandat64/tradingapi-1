using System.Security.Principal;

namespace Gateway.Middleware.LegacyWiring
{
    public class UniversalPrincipal : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity { get; } = new UniversalIdentity();
    }
}
