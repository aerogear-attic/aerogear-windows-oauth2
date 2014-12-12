using System;
using Windows.System;

namespace AeroGear.OAuth2
{
    public class OAuth2Module : AuthzModule
    {
        private const string PARAM_TEMPLATE = @"?scope={0}&redirect_uri={1}&client_id={2}&response_type=code";
        private Config config;

        public OAuth2Module(Config config)
        {
            // TODO: Complete member initialization
            this.config = config;
        }

        public void RequestAccess(Action<object, Exception> callback)
        {
            RequestAuthorizationCode(callback);
        }

        public async void RequestAuthorizationCode(Action<object, Exception> callback)
        {
            var param = string.Format(PARAM_TEMPLATE, config.scope, config.redirectURL, config.clientId);
            var uri = new Uri(config.baseURL, config.authzEndpoint).AbsoluteUri + param;

            await Launcher.LaunchUriAsync(new Uri(uri));
        }

        public Tuple<string, string> authorizationFields()
        {
            throw new NotImplementedException();
        }
    }
}
