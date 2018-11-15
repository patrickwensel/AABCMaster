using ATrack.Authenticator;
using Unity;

namespace AABC.ATrack.Authenticator.ProviderApp
{
    public class UserProviderFactory : IUserProviderFactory
    {
        private readonly IUnityContainer Container;

        public UserProviderFactory(IUnityContainer container)
        {
            Container = container;
        }

        public IUserProvider Create()
        {
            return Container.Resolve<IUserProvider>();
        }
    }
}
