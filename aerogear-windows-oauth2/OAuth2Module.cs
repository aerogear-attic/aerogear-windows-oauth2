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
        private Config config;
        public OAuth2Session oauth2Session { get; set; }

        public OAuth2Module(Config config)
        {
            this.config = config;
            this.oauth2Session = new TrustedPersistantOAuth2Session(config.accountId);
        }

        public OAuth2Module(Config config, OAuth2Session session)
            : this(config)
        {
            this.oauth2Session = session;
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

        public void extractCode(string query)
        {
            IDictionary<string, string> queryParams = ParseQueryString(query);
            if (queryParams.ContainsKey("code"))
            {
                exchangeAuthorizationCodeForAccessToken(queryParams["code"]);
            }
            else
            {
                throw new Exception("user cancelled the authorization");
            }
        }

        private async void exchangeAuthorizationCodeForAccessToken(string code)
        {
            var body = new Dictionary<string, string>() { { "grant_type", "authorization_code" }, { "code", code }, { "client_id", config.clientId }, { "redirect_uri", config.redirectURL } };
            var request = WebRequest.Create(config.baseURL + config.accessTokenEndpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var postStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
            {
                foreach (KeyValuePair<string, string> entry in body)
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
                    var session = serializer.ReadObject(stream);
                    Debug.WriteLine(session);
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

        public Tuple<string, string> authorizationFields()
        {
            throw new NotImplementedException();
        }
    }
}
