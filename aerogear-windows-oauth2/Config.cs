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
using System.Linq;
using System.Net;

namespace AeroGear.OAuth2
{
    public class Config
    {
        public string baseURL { get; set; }

        /**
        Applies the "callback URL" once request token issued.
        */
        public string redirectURL { get; set; }

        /**
        Applies the "authorization endpoint" to the request token.
        */
        public string authzEndpoint { get; set; }

        /**
        Applies the "access token endpoint" to the exchange code for access token.
        */
        public string accessTokenEndpoint { get; set; }

        /**
        Endpoint for request to invalidate both accessToken and refreshToken.
        */
        public string revokeTokenEndpoint { get; set; }

        /**
        Endpoint for request a refreshToken.
        */
        public string refreshTokenEndpoint { get; set; }

        public string clientId { get; set; }

        /**
        Applies the "client secret" obtained with the client registration process.
        */
        public string clientSecret { get; set; }

        public string accountId { get; set; }

        /**
        Applies the various scopes of the authorization.
        */
        public IList<string> scopes { get; set; }

        public string scope
        {
            get
            {
                var scopeString = "";
                if (scopes != null)
                {
                    foreach (string scope in scopes)
                    {
                        scopeString += WebUtility.UrlEncode(scope);
                        if (scope != scopes.Last())
                        {
                            scopeString += "+";
                        }
                    }
                }
                return scopeString;
            }
        }
    }
}
