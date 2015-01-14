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
using AeroGear.OAuth2;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace tests
{
    [TestClass]
    public class SessionTest
    {
        [TestMethod]
        public void ShouldTellWhenExpiredRefresh()
        {
            //given
            Session session = new Session();
            session.refreshTokenExpirationDate = new DateTime(2014, 12, 16);

            //when
            var test = session.RefreshTokenIsNotExpired();

            //then
            Assert.IsFalse(test);
        }

        [TestMethod]
        public void ShouldTellWhenExpiredAccess()
        {
            //given
            Session session = new Session();
            session.accessTokenExpirationDate = new DateTime(2014, 12, 16);

            //when
            var test = session.TokenIsNotExpired();

            //then
            Assert.IsFalse(test);
        }

        [TestMethod]
        public void serialisationTest()
        {
            //given
            Session session = new Session();
            session.accessTokenExpirationDate = new DateTime(2014, 12, 16);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Session));
            Session readBackSession = null;

            //when
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, session);
                var bytes = ms.ToArray();

                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    readBackSession = (Session)serializer.ReadObject(ms);
                }
            }

            //then
            Assert.AreEqual(session.accessTokenExpirationDate, readBackSession.accessTokenExpirationDate);
        }

        [TestMethod]
        public void serialisationTestExpiresIn()
        {
            //given
            string json = "{\"expires_in\": 300}";
            Session session = null;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Session));

            //when
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                session = (Session)serializer.ReadObject(stream);
            }

            //then
            Assert.AreEqual(DateTime.Now.AddSeconds(300), session.accessTokenExpirationDate);
        }
    }
}
