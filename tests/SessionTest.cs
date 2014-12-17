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
