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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using System.Collections.Generic;

namespace AeroGear.OAuth2
{
    public class ManifestInfo
    {
        private XDocument document;
        private XNamespace xname;
        private static ManifestInfo instance;

        protected ManifestInfo() { }

        private async static Task<ManifestInfo> GetInstance()
        {
            if (instance == null)
            {
                instance = new ManifestInfo();
                await instance.init();
            }
            return instance;
        }

        private async Task init()
        {
            StorageFile file = await Package.Current.InstalledLocation.GetFileAsync("AppxManifest.xml");
            string manifestXml = await FileIO.ReadTextAsync(file);
            document = XDocument.Parse(manifestXml);
            xname = XNamespace.Get("http://schemas.microsoft.com/appx/manifest/uap/windows10");
        }

        public async static Task<string> GetProtocol()
        {
            var instance = await GetInstance();

            var attribute = (from node in instance.document.Descendants(instance.xname + "Extension")
                             where (string)node.Attribute("Category") == "windows.protocol"
                             select node.Element(instance.xname + "Protocol").Attribute("Name")).Single();

            return attribute.Value;
        }
    }
}
