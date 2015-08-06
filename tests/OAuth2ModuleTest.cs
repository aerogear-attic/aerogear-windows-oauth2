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
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace AeroGear.OAuth2
{
    [TestClass]
    public class OAuth2ModuleTest
    {
        [TestMethod]
        public async Task ShouldRequestAccessCode()
        {
            //given
            MockOAuth2Module module = new MockOAuth2Module();

            //when
            await module.RequestAccess();

            //then
            module.AssertCalled("RequestAuthorizationCode");
        }

        [TestMethod]
        public async Task ShouldRequestRenewToken()
        {
            //given
            Session session = new Session()
            {
                accessToken = "dummy",
                accessTokenExpirationDate = DateTime.Now.AddDays(-1),
                refreshTokenExpirationDate = DateTime.Now.AddDays(1),
                refreshToken = "dummy-token"
            };
            MockOAuth2Module module = new MockOAuth2Module(session);

            //when
            await module.RequestAccess();

            //then
            module.AssertCalled("RefreshAccessToken");
        }

        [TestMethod]
        public void ShouldCreateAuthenticationTuple()
        {
            //given
            string token = "token";
            Session session = new Session()
            {
                accessToken = token,
                accessTokenExpirationDate = DateTime.Now.AddDays(-1),
                refreshTokenExpirationDate = DateTime.Now.AddDays(1),
                refreshToken = "dummy-token"
            };
            MockOAuth2Module module = new MockOAuth2Module(session);

            //when
            var field = module.AuthorizationFields();

            //then
            Assert.IsNotNull(field);
            Assert.AreEqual("Bearer " + token, field.Item2);
        }
    }

    public class MockOAuth2Module : OAuth2Module
    {
        private IList<string> called = new List<string>();

        public MockOAuth2Module()
            : this(new Session() { accountId = "test" })
        {
        }

        public MockOAuth2Module(Session session)
        {
            this.session = session;
        }

        public override Task<WebAuthenticationResult> RequestAuthorizationCode()
        {
            called.Add("RequestAuthorizationCode");
            return null;
        }

        protected override Task RefreshAccessToken()
        {
            called.Add("RefreshAccessToken");
            return null;
        }

        public void AssertCalled(string method)
        {
            Assert.IsTrue(called.Contains(method));
        }
    }
}
