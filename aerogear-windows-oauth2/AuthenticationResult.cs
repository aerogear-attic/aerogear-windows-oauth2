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
using Windows.Security.Authentication.Web;

namespace AeroGear.OAuth2
{
    public class AuthenticationResult
    {
        //
        // Summary:
        //     Contains the protocol data when the operation successfully completes.
        //
        // Returns:
        //     The returned data.
        public System.String ResponseData { get; internal set; }
        //
        // Summary:
        //     Returns the HTTP error code when ResponseStatus is equal to WebAuthenticationStatus.ErrorHttp.
        //     This is only available if there is an error.
        //
        // Returns:
        //     The specific HTTP error, for example 400.
        public System.UInt32 ResponseErrorDetail { get; internal set; }
        //
        // Summary:
        //     Contains the status of the asynchronous operation when it completes.
        //
        // Returns:
        //     One of the enumerations.
        public WebAuthenticationStatus ResponseStatus { get; internal set; }

        public static AuthenticationResult CopyOf(WebAuthenticationResult result)
        {
            return new AuthenticationResult()
            {
                ResponseData = result.ResponseData,
                ResponseErrorDetail = result.ResponseErrorDetail,
                ResponseStatus = result.ResponseStatus
            };
        }
    }
}