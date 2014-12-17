using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.System;

namespace AeroGear.OAuth2
{
    public class OAuth2Module : AuthzModule
    {
        private const string PARAM_TEMPLATE = @"?scope={0}&redirect_uri={1}&client_id={2}&response_type=code";

        private SessionRepositry repository = new TrustedSessionRepository();
        protected Session session;

        private Config _config;
        public Config config { get { return _config; } }

        public OAuth2Module(Config config)
        {
            this._config = config;
            init();
        }

        public async void init()
        {
            try
            {
                session = await repository.Read(config.accountId);
            }
            catch (Exception e)
            {
                session = new Session() { accountId = config.accountId };
            }
        }

        public OAuth2Module(Config config, SessionRepositry session)
            : this(config)
        {
            this.repository = session;
        }

        public void RequestAccess()
        {
            if (session.accessToken == null || !session.TokenIsNotExpired())
            {
                if (session.refreshToken != null && session.RefreshTokenIsNotExpired())
                {
                    RefreshAccessToken();
                }
                else
                {
                    RequestAuthorizationCode();
                }
            }
        }

        public virtual async void RequestAuthorizationCode()
        {
            var param = string.Format(PARAM_TEMPLATE, _config.scope, _config.redirectURL, _config.clientId);
            var uri = new Uri(_config.baseURL, _config.authzEndpoint).AbsoluteUri + param;

            await Launcher.LaunchUriAsync(new Uri(uri));
        }

        public Tuple<string, string> AuthorizationFields()
        {
            if (session.accessToken != null)
            {
                return Tuple.Create("Authorization", "Bearer " + session.accessToken);
            }
            return null;
        }

        protected virtual async void RefreshAccessToken()
        {
            var parameters = new Dictionary<string, string>() { { "refresh_token", session.refreshToken }, { "client_id", _config.clientId }, { "grant_type", "refresh_token" } };
            if (_config.clientSecret != null)
            {
                parameters["client_secret"] = _config.clientSecret;
            }
            await UpdateToken(parameters);
        }

        public void ExtractCode(string query)
        {
            IDictionary<string, string> queryParams = ParseQueryString(query);
            if (queryParams.ContainsKey("code"))
            {
                ExchangeAuthorizationCodeForAccessToken(queryParams["code"]);
            }
            else
            {
                throw new Exception("user cancelled the authorization");
            }
        }

        private async void ExchangeAuthorizationCodeForAccessToken(string code)
        {
            var parameters = new Dictionary<string, string>() { { "grant_type", "authorization_code" }, { "code", code }, { "client_id", _config.clientId }, { "redirect_uri", _config.redirectURL } };
            if (_config.clientSecret != null)
            {
                parameters["client_secret"] = _config.clientSecret;
            }
            await UpdateToken(parameters);
        }

        private async Task UpdateToken(Dictionary<string, string> parameters)
        {
            var request = WebRequest.Create(_config.baseURL + _config.accessTokenEndpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var postStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
            {
                foreach (KeyValuePair<string, string> entry in parameters)
                {
                    var bytes = Encoding.UTF8.GetBytes(entry.Key + "=" + WebUtility.UrlEncode(entry.Value) + "&");
                    postStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request))
            {
                using (var stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Session));
                    session = (Session)serializer.ReadObject(stream);
                    await repository.Save(session);
                }
            }
        }

        private IDictionary<string, string> ParseQueryString(string query)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            query = query.Substring(query.IndexOf('?') + 1);

            foreach (string valuePair in Regex.Split(query, "&"))
            {
                string[] pair = Regex.Split(valuePair, "=");
                result.Add(WebUtility.UrlDecode(pair[0]), WebUtility.UrlDecode(pair[1]));
            }

            return result;
        }
    }
}
