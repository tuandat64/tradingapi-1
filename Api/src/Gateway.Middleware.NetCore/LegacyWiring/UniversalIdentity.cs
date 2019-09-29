using System.Security.Principal;

namespace Gateway.Middleware.LegacyWiring
{
    public class UniversalIdentity : IIdentity
    {
        public string AuthenticationType { get; } = "Universal";
        public bool IsAuthenticated { get; } = true;
        public string Name { get; } = nameof(UniversalIdentity);
    }
}
