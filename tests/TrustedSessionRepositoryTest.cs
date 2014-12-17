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
            var file = await session.SaveAccessToken(token);
            
            //then
            Assert.IsNotNull(file);
            var readToken = await session.ReadAccessToken();
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
                await session.ReadAccessToken();
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
