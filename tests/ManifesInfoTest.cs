using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AeroGear.OAuth2;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace AeroGear.OAuth2
{
    [TestClass]
    public class ManifesInfoTest
    {
        [TestMethod]
        public async Task ReturnProtocolInformation()
        {

            //when
            var protocol = await ManifestInfo.GetProtocol();

            //then
            Assert.AreEqual("unit-test", protocol);
        }
    }
}
