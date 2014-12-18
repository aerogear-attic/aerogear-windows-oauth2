using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    [TestClass]
    public class OAuth2ModuleTest
    {
        [TestMethod]
        public async void ShouldRequestAccessCode()
        {
            //given
            MockOAuth2Module module = new MockOAuth2Module();

            //when
            await module.RequestAccess();

            //then
            module.AssertCalled("RequestAuthorizationCode");
        }

        [TestMethod]
        public async void ShouldRequestRenewToken()
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
            : base(new Config() { accountId = "test" })
        {

        }

        public MockOAuth2Module(Session session) : this()
        {
            this.session = session;
        }

        public override Task RequestAuthorizationCode()
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
