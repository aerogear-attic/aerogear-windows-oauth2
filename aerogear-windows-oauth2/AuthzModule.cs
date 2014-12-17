using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public interface AuthzModule
    {
        void RequestAccess();

        Tuple<string, string> AuthorizationFields();
    }
}
