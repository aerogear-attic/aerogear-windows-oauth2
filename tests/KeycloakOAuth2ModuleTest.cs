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
            Assert.AreEqual("1420451611", result["iat"].ToString());
        }
    }
}
