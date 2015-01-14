using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public interface SessionRepositry
    {
        Task Save(string accessToken, string refreshToken, string accessTokenExpiration, string refreshTokenExpiration);
        Task Save(Session session);

        Task<Session> Read(string accountId);
    }

    [DataContract]
    public class Session
    {
        [DataMember]
        public string accountId { get; set; }
        [DataMember(Name = "access_token")]
        public string accessToken { get; set; }
        [DataMember]
        public DateTime accessTokenExpirationDate { get; set; }
        [DataMember]
        public DateTime refreshTokenExpirationDate { get; set; }

        [DataMember(Name = "expires_in")]
        public int accessTokenExpiration
        {
            get
            {
                return -1;
            }
            set
            {
                if (value != -1)
                {
                    accessTokenExpirationDate = DateTime.Now.AddSeconds(value);
                }
            }
        }

        [DataMember(Name = "refresh_token")]
        public string refreshToken { get; set; }

        public bool TokenIsNotExpired()
        {
            return DateTime.Now < accessTokenExpirationDate;
        }

        public bool RefreshTokenIsNotExpired()
        {
            return DateTime.Now < refreshTokenExpirationDate;
        }
    }
}
