using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public sealed class AccountManager
    {
        private static readonly AccountManager instance = new AccountManager();

        private AccountManager() { }

        public static AccountManager Instance
        {
            get
            {
                return instance;
            }
        }

        public static OAuth2Module AddAccount(Config config)
        {
            OAuth2Module module = new OAuth2Module(config);
            return module;
        }
    }
}
