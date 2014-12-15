using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public interface OAuth2Session
    {
        void SaveAccessToken();
        void SaveAccessToken(string accessToken, string refreshToken, string accessTokenExpiration, string refreshTokenExpiration);
        void SaveAccessToken(Session session);
    }

    public class Session
    {
        public string accountId { get; set; }
        public string accessToken { get; set; }
        public DateTime accessTokenExpirationDate { get; set; }
        public DateTime refreshTokenExpirationDate { get; set; }
        public string refreshToken { get; set; }
    }
}
