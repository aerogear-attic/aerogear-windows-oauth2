﻿/**
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public sealed class AccountManager
    {
        private IDictionary<string, OAuth2Module> modules = new Dictionary<string, OAuth2Module>();
        private static readonly AccountManager instance = new AccountManager();

        private AccountManager() { }

        public static AccountManager Instance
        {
            get
            {
                return instance;
            }
        }

        public static async Task<OAuth2Module> AddAccount(Config config)
        {
            if (Instance.modules.ContainsKey(config.accountId))
            {
                return Instance.modules[config.accountId];
            }
            else
            {
                OAuth2Module module = await OAuth2Module.Create(config);
                Instance.modules[config.accountId] = module;
                return module;
            }
        }

        public static async Task<OAuth2Module> AddKeyCloak(KeycloakConfig config)
        {
            OAuth2Module module = await KeycloakOAuth2Module.Create(config);
            Instance.modules[config.accountId] = module;
            return module;
        }

        public static async Task<OAuth2Module> AddFacebook(FacebookConfig config)
        {
            OAuth2Module module = await FacebookOAuth2Module.Create(config);
            Instance.modules[config.accountId] = module;
            return module;
        }

        public static OAuth2Module GetAccountByName(string name)
        {
            return Instance.modules[name];
        }

        public static OAuth2Module GetAccountByClientId(string clientId)
        {
            return Instance.modules.Where(entry => entry.Value.config.clientId == clientId).Single().Value;
        }

        public async static Task<OAuth2Module> ParseContinuationEvent(Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args)
        {
            var module = GetAccountByName((string)args.ContinuationData["name"]);
            await module.ExtractCode(args);
            return module;
        }
    }

    public class GoogleConfig : Config
    {
        public async static Task<GoogleConfig> Create(string clientId, List<string> scopes, string accountId)
        {
            var protocol = await ManifestInfo.GetProtocol();
            return new GoogleConfig()
            {
                baseURL = "https://accounts.google.com/",
                authzEndpoint = "o/oauth2/auth",
                redirectURL = protocol + ":/oauth2Callback",
                accessTokenEndpoint = "o/oauth2/token",
                refreshTokenEndpoint = "o/oauth2/token",
                revokeTokenEndpoint = "rest/revoke",
                clientId = clientId,
                scopes = scopes,
                accountId = accountId
            };
        }
    }

    public class KeycloakConfig : Config
    {
        public async static Task<KeycloakConfig> Create(string clientId, string host, string realm)
        {
            var protocol = await ManifestInfo.GetProtocol();
            var defaulRealmName = clientId + "-realm";
            var realmName = realm != null ? realm : defaulRealmName;
            return new KeycloakConfig() {
                baseURL = host + "/auth/",
                authzEndpoint = string.Format("realms/{0}/protocol/openid-connect/auth", realmName),
                redirectURL = protocol + ":/oauth2Callback",
                accessTokenEndpoint = string.Format("realms/{0}/protocol/openid-connect/token", realmName),
                clientId = clientId,
                refreshTokenEndpoint = string.Format("realms/{0}/protocol/openid-connect/token", realmName),
                revokeTokenEndpoint = string.Format("realms/%@/protocol/openid-connect/logout", realmName),
                accountId = clientId
            };
        }
    }

    public class FacebookConfig : Config
    {
        public static FacebookConfig Create(string clientId, string clientSecret, List<string> scopes, string accountId)
        {
            return new FacebookConfig()
            {
                baseURL = "",
                authzEndpoint = "https://www.facebook.com/dialog/oauth",
                redirectURL = "fb" + clientId + "://authorize/",
                accessTokenEndpoint = "https://graph.facebook.com/oauth/access_token",
                clientId = clientId,
                refreshTokenEndpoint = "https://graph.facebook.com/oauth/access_token",
                clientSecret = clientSecret,
                revokeTokenEndpoint = "https://www.facebook.com/me/permissions",
                scopes = scopes,
                accountId = accountId
            };
        }
    }
}
