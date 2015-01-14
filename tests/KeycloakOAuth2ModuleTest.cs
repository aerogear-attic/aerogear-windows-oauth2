using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    [TestClass]
    public class KeycloakOAuth2ModuleTest
    {
        [TestMethod]
        public void ParseKeyCloakResponse()
        {
            //given
            string token = "eyJhbGciOiJSUzI1NiJ9.eyJqdGkiOiI0NGRlOGU3Zi0xYWEwLTQ4MDUtYjdkMy00MTUxZTgyMTE1ZWYiLCJleHAiOjE0MjA0NTE5MTEsIm5iZiI6MCwiaWF0IjoxNDIwNDUxNjExLCJpc3MiOiJzaG9vdC1yZWFsbSIsImF1ZCI6InNob290LXRoaXJkLXBhcnR5Iiwic3ViIjoiMGQ0NDU2YTktYjQ0NS00YTQxLWIyMWYtNDZkZWM3OGE4ODgyIiwiYXpwIjoic2hvb3QtdGhpcmQtcGFydHkiLCJzZXNzaW9uX3N0YXRlIjoiNmE0NmEyNzMtYTVkZi00ZTQyLTlhZWMtYjI5NmUzMmI2MzZlIiwiYWxsb3dlZC1vcmlnaW5zIjpbXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbInVzZXIiXX0sInJlc291cmNlX2FjY2VzcyI6e319.Bp5TThWGGIK3NMQUZZTcRrHQm26pY41AlPNFenYs21ogiiYwUCaFq3R-FCzDnvK2bjwy3wfuW1THf0KsBrcPW17V7JOgIE_53YwcHnHYKrIVOzZ0nXgqWQAR99wm4EiECA5W1AHJTAb7ROhC9_26nWSMMZdBdV_Ea5pKdyNZz5M";
            
            //when
            var result = new KeycloakOAuth2Module().DecodeToken(token);

            //then
            Assert.IsNotNull(result);
            Assert.AreEqual(1420451611, result.iat);
        }
    }
}
