using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AeroGear.OAuth2
{
    public class TrustedPersistantOAuth2Session : OAuth2Session
    {
        private const BinaryStringEncoding ENCODING = BinaryStringEncoding.Utf8;

        public async void SaveAccessToken()
        {
        }

        public async Task SaveAccessToken(string accessToken, string refreshToken, string accessTokenExpiration, string refreshTokenExpiration)
        {
            await SaveAccessToken(new Session() {
                accessToken = accessToken, 
                refreshToken = refreshToken, 
                accessTokenExpirationDate = DateTime.Now.AddSeconds(Double.Parse(accessTokenExpiration)),
                refreshTokenExpirationDate = DateTime.Now.AddSeconds(Double.Parse(refreshTokenExpiration)) 
            });
        }

        public async Task SaveAccessToken(Session session)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Session));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, session);
                var bytes = ms.ToArray();
                await SaveAccessToken(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
            }
        }

        public async Task<string> ReadAccessToken()
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await local.GetFileAsync("token-file.txt");

            var text = await FileIO.ReadBufferAsync(file);

            DataProtectionProvider Provider = new DataProtectionProvider();
            IBuffer buffUnprotected = await Provider.UnprotectAsync(text);

            return CryptographicBuffer.ConvertBinaryToString(ENCODING, buffUnprotected);
        }

        public async Task<IStorageFile> SaveAccessToken(string token)
        {
            DataProtectionProvider Provider = new DataProtectionProvider("LOCAL=user");
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(token, ENCODING);
            IBuffer buffProtected = await Provider.ProtectAsync(buffMsg);

            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await local.CreateFileAsync("token-file.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(file, buffProtected);
            return file;
        }
    }
}
