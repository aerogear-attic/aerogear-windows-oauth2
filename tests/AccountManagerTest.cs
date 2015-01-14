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
using AeroGear.OAuth2;
using System.Threading.Tasks;

namespace tests
{
    [TestClass]
    public class AccountManagerTest
    {
        [TestMethod]
        public async Task AddAccountTest()
        {
            //given
            string name = "test";
            Config config = new Config() { accountId = name };

            //when
            await AccountManager.AddAccount(config);

            //then
            Assert.IsNotNull(AccountManager.GetAccountByName(name));
        }

        [TestMethod]
        public async Task AddAccountMultipleTimesTest()
        {
            //given
            string name = "test";
            Config config = new Config() { accountId = name };

            //when
            var module1 = await AccountManager.AddAccount(config);
            config.accessTokenEndpoint = "other";
            var module2 = await AccountManager.AddAccount(config);

            //then
            Assert.AreSame(module1, module2);
        }

        [TestMethod]
        public async Task GetByClientId()
        {
            //given
            string clientId = "test";
            Config config = new Config() { accountId = "dummy", clientId = clientId };

            //when
            var module1 = await AccountManager.AddAccount(config);
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
                Assert.AreEqual("Sequence contains no elements", e.Message);
            }
        }

    }
}
