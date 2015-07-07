/**
 * JBoss, Home of Professional Open Source
 * Copyright Red Hat, Inc., and individual contributors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * 	http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;

namespace AeroGear.OAuth2
{
    public class OAuth2Module : AuthzModule
    {
        private const string PARAM_TEMPLATE = @"?scope={0}&redirect_uri={1}&client_id={2}&response_type=code";

        protected SessionRepositry repository = new TrustedSessionRepository();
        protected Session session;

        public Config config { get; private set; }

        public async static Task<OAuth2Module> Create(Config config)
        {
            OAuth2Module module = new OAuth2Module();
            await module.init(config);
            return module; 
        }

        public async static Task<OAuth2Module> Create(Config config, SessionRepositry repository)
        {
            OAuth2Module module = new OAuth2Module();
            module.repository = repository;
            await module.init(config);
            return module;
        }

        public async Task init(Config config)
        {
            this.config = config;
            try
            {
                session = await repository.Read(config.accountId);
            }
            catch (IOException)
            {
                session = new Session() { accountId = config.accountId };
            }
        }

        public async Task<string> RequestAccess()
        {
            if (session.accessToken == null || !session.TokenIsNotExpired())
            {
                if (session.refreshToken != null && session.RefreshTokenIsNotExpired())
                {
                    await RefreshAccessToken();
                }
                else
                {
                    await ExtractCode(await RequestAuthorizationCode());
                }
            }

            return session.accessToken;
        }

        public async virtual Task<WebAuthenticationResult> RequestAuthorizationCode()
        {
            var param = string.Format(PARAM_TEMPLATE, config.scope, Uri.EscapeDataString(config.redirectURL), Uri.EscapeDataString(config.clientId));
            var uri = new Uri(config.baseURL + config.authzEndpoint).AbsoluteUri + param;

            var values = new ValueSet() { { "name", config.accountId } };
            return await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(uri), new Uri(config.redirectURL));
        }

        public Tuple<string, string> AuthorizationFields()
        {
            if (session.accessToken != null)
            {
                return Tuple.Create("Authorization", "Bearer " + session.accessToken);
            }
            return null;
        }

        public AuthenticationHeaderValue AuthenticationHeaderValue()
        {
            return new AuthenticationHeaderValue("Bearer", session.accessToken);
        }

        protected virtual async Task RefreshAccessToken()
        {
            var parameters = new Dictionary<string, string>() { { "refresh_token", session.refreshToken }, { "client_id", config.clientId }, { "grant_type", "refresh_token" } };
            if (config.clientSecret != null)
            {
                parameters["client_secret"] = config.clientSecret;
            }
            await UpdateToken(parameters, config.refreshTokenEndpoint);
        }

        public async Task ExtractCode(WebAuthenticationResult result)
        {
            if (result.ResponseStatus == WebAuthenticationStatus.Success) 
            {
                IDictionary<string, string> queryParams = ParseQueryString(new Uri(result.ResponseData).Query);
                if (queryParams.ContainsKey("code"))
                {
                    await ExchangeAuthorizationCodeForAccessToken(queryParams["code"]);
                }
                else
                {
                    throw new Exception("no code parameter found in redirect");
                }
            }
            else
            {
                throw new Exception(string.Format("user cancelled the authorization status: '{0}': details: {1}", result.ResponseStatus, result.ResponseErrorDetail));
            }
        }

        private async Task ExchangeAuthorizationCodeForAccessToken(string code)
        {
            var parameters = new Dictionary<string, string>() { { "grant_type", "authorization_code" }, { "code", code }, { "client_id", config.clientId }, { "redirect_uri", config.redirectURL } };
            if (config.clientSecret != null)
            {
                parameters["client_secret"] = config.clientSecret;
            }
            await UpdateToken(parameters, config.accessTokenEndpoint);
        }

        private async Task UpdateToken(Dictionary<string, string> parameters, string endpoint)
        {
            var request = WebRequest.Create(config.baseURL + endpoint);
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
                session = await ParseResponse(response.GetResponseStream());
                session.accountId = config.accountId;
                await repository.Save(session);
            }
        }

        protected virtual async Task<Session> ParseResponse(Stream respondeStream)
        {
            using (var stream = respondeStream)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Session));
                var session = (Session)serializer.ReadObject(stream);
                return session;
            }
        }

        protected IDictionary<string, string> ParseQueryString(string query)
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
