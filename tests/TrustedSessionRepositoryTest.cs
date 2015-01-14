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
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace tests
{
    [TestClass]
    public class TrustedSessionRepositoryTest
    {
        [TestMethod]
        public async Task SaveAndRead()
        {
            //given
            string token = "test token";
            TrustedSessionRepository session = new TrustedSessionRepository();

            //when
            var file = await session.SaveAccessToken(token, "dummy");
            
            //then
            Assert.IsNotNull(file);
            var readToken = await session.ReadAccessToken("dummy");
            Assert.AreEqual(token, readToken);
        }

        [TestMethod]
        public async Task Read()
        {
            //given
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await local.CreateFileAsync("token-file.txt", CreationCollisionOption.OpenIfExists);
            await file.DeleteAsync();

            TrustedSessionRepository session = new TrustedSessionRepository();

            //when
            try
            {
                await session.ReadAccessToken("none");
                Assert.Fail("excption should have been thrown as there is no token saved yet");
            }
            catch (Exception e)
            {
                //then
                Assert.IsTrue(e.Message.Contains("The system cannot find the file specified."));
            }
        }
    }
}
