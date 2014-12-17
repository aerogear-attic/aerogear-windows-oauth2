using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AeroGear.OAuth2;

namespace tests
{
    [TestClass]
    public class AccountManagerTest
    {
        [TestMethod]
        public void AddAccountTest()
        {
            //given
            string name = "test";
            Config config = new Config() { accountId = name };

            //when
            AccountManager.AddAccount(config);

            //then
            Assert.IsNotNull(AccountManager.GetAccountByName(name));
        }

        [TestMethod]
        public void AddAccountMultipleTimesTest()
        {
            //given
            string name = "test";
            Config config = new Config() { accountId = name };

            //when
            var module1 = AccountManager.AddAccount(config);
            config.accessTokenEndpoint = "other";
            var module2 = AccountManager.AddAccount(config);

            //then
            Assert.AreSame(module1, module2);
        }

        [TestMethod]
        public void GetByClientId()
        {
            //given
            string clientId = "test";
            Config config = new Config() { accountId = "dummy", clientId = clientId };

            //when
            var module1 = AccountManager.AddAccount(config);
            var module2 = AccountManager.GetAccountByClientId(clientId);

            //then
            Assert.AreSame(module1, module2);
        }

        [TestMethod]
        public void GetByClientIdNotFound()
        {
            //when
            try
            {
                AccountManager.GetAccountByClientId("not found");
                Assert.Fail("exception should have been thrown, there was no module with this client id");
            }
            catch (InvalidOperationException e)
            {
                //success
            }
        }

    }
}
