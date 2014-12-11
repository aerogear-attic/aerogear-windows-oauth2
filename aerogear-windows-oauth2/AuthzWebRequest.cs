using System;
using System.Net;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public class AuthzWebRequest : WebRequest
    {
        private WebRequest webRequest;

        public AuthzWebRequest(WebRequest request)
        {
            this.webRequest = request;
        }

        private AuthzModule _authzModule;
        public AuthzModule authzModule
        {
            get
            {
                return _authzModule;
            }
            set
            {
                _authzModule = value;
                if (authzModule != null)
                {
                    authzModule.RequestAccess((response, error) =>
                    {
                        if (error != null)
                        {
                            throw error;
                        }
                    });
                }
            }
        }

        public new static WebRequest Create(string requestUriString)
        {
            var request = WebRequest.Create(requestUriString);
            return new AuthzWebRequest(request);
        }

        public new static WebRequest Create(Uri requestUri)
        {
            var webRequest = WebRequest.Create(requestUri);
            return new AuthzWebRequest(webRequest);
        }

        public override void Abort()
        {
            webRequest.Abort();
        }

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            return webRequest.BeginGetRequestStream(callback, state);
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return webRequest.BeginGetResponse(callback, state);
        }

        public override string ContentType
        {
            get
            {
                return webRequest.ContentType;
            }
            set
            {
                webRequest.ContentType = value;
            }
        }

        public override System.IO.Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return webRequest.EndGetRequestStream(asyncResult);
        }

        public override WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            return webRequest.EndGetResponse(asyncResult);
        }

        public override WebHeaderCollection Headers
        {
            get
            {
                return webRequest.Headers;
            }
            set
            {
                webRequest.Headers = value;
            }
        }

        public override string Method
        {
            get
            {
                return webRequest.Method;
            }
            set
            {
                webRequest.Method = value;
            }
        }

        public override Uri RequestUri
        {
            get { return webRequest.RequestUri; }
        }
    }
}
