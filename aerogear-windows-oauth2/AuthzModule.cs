using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public interface AuthzModule
    {
        void RequestAccess(Action<Object, Exception> callback);

        Tuple<string, string> authorizationFields();
    }
}
