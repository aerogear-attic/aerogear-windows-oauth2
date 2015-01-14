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
    }
}
