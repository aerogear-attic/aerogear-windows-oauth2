using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OAuth2
{
    public sealed class AccountManager
    {
        private IDictionary<string, OAuth2Module> modules = new Dictionary<string, OAuth2Module>();
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
            Instance.modules.Add(module.oauth2Session.GetSession().accountId, module);
            return module;
        }

        public static OAuth2Module GetAccountByName(string name)
        {
            return Instance.modules[name];
        }
    }
}
